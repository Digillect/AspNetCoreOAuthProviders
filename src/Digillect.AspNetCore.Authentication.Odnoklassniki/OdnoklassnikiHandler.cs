// Copyright (c) Andrew Nefedkin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Digillect.AspNetCore.Authentication.Odnoklassniki
{
    internal class OdnoklassnikiHandler : OAuthHandler<OdnoklassnikiOptions>
    {
        public OdnoklassnikiHandler(
            [NotNull] IOptionsMonitor<OdnoklassnikiOptions> options,
            [NotNull] ILoggerFactory logger,
            [NotNull] UrlEncoder encoder,
            [NotNull] ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticationTicket> CreateTicketAsync([NotNull] ClaimsIdentity identity, [NotNull] AuthenticationProperties properties, [NotNull] OAuthTokenResponse tokens)
        {
            var queryString = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["application_key"] = Options.ApplicationKey,
                ["format"] = "json",
                ["__online"] = "false"
            };

            if (Options.Fields.Count != 0)
            {
                queryString.Add("fields", string.Join(",", Options.Fields));
            }

            string address = Options.UserInformationEndpoint;

            if (address.IndexOf('?') >= 0)
            {
                var uri = new Uri(address);
                address = uri.GetLeftPart(UriPartial.Path);
                foreach (var item in QueryHelpers.ParseQuery(uri.Query))
                {
                    queryString.TryAdd(item.Key, item.Value);
                }
            }

            queryString.Add("sig", ComputeSignature(tokens.AccessToken, queryString));

            // Call API methods using access_token instead of session_key parameter
            queryString.Add("access_token", tokens.AccessToken);

            address = QueryHelpers.AddQueryString(address, queryString);

            var response = await Backchannel.GetAsync(address, Context.RequestAborted);
            if (!response.IsSuccessStatusCode)
            {
                Logger.LogError("An error occurred while retrieving the user profile: the remote server " +
                                "returned a {Status} response with the following payload: {Headers} {Body}.",
                                /* Status: */ response.StatusCode,
                                /* Headers: */ response.Headers.ToString(),
                                /* Body: */ await response.Content.ReadAsStringAsync());

                throw new HttpRequestException($"An error occurred when retrieving user information ({response.StatusCode}).");
            }

            using (var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync()))
            {
                if (payload.RootElement.TryGetProperty("error_code", out var errorCode))
                {
                    Logger.LogError("An error occurred while retrieving the user profile: the provider returned an error {ErrorCode} with the message \"{ErrorMessage}\" and data \"{ErrorData}\"",
                                    /* ErrorCode */ errorCode.GetInt32(),
                                    /* ErrorMessage */ payload.RootElement.GetString("error_msg"),
                                    /* ErrorData */ payload.RootElement.GetString("error_data"));

                    return null;
                }

                var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, payload.RootElement);
                context.RunClaimActions();

                await Events.CreatingTicket(context);

                return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
            }
        }

        protected override string FormatScope() => string.Join(",", Options.Scope);

        protected string ComputeSignature(string accessToken, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            // Signing.
            // Calculate every request signature parameter sig as described in
            // https://apiok.ru/en/dev/methods/
            // * session_secret_key = MD5(access_token + application_secret_key), convert the value to the lower case;
            // * take session_key/access_token away from the list of parameters, if applicable;
            // * parameters are sorted lexicographically by keys;
            // * parameters are joined in the format key=value;
            // * sig = MD5(parameters_value + session_secret_key);
            // * the sig value is changed to the lower case.

            var parametersValue = string.Concat(from parameter in parameters
                                                orderby parameter.Key
                                                select $"{parameter.Key}={parameter.Value}");

            using (var provider = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(accessToken + Options.ClientSecret);
                bytes = provider.ComputeHash(bytes);
                bytes = Encoding.UTF8.GetBytes(parametersValue + BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant());
                bytes = provider.ComputeHash(bytes);
                return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
