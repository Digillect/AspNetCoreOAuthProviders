// Copyright (c) Andrew Nefedkin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Digillect.AspNetCore.Authentication.Odnoklassniki
{
    internal class OdnoklassnikiHandler : OAuthHandler<OdnoklassnikiOptions>
    {
        public OdnoklassnikiHandler([NotNull] HttpClient client)
            : base(client)
        {
        }

        protected override async Task<AuthenticationTicket> CreateTicketAsync([NotNull] ClaimsIdentity identity, [NotNull] AuthenticationProperties properties, [NotNull] OAuthTokenResponse tokens)
        {
            // Call API methods using access_token instead of session_key parameter
            var address = QueryHelpers.AddQueryString(Options.UserInformationEndpoint, "access_token", tokens.AccessToken);

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

            queryString.Add("sig", ComputeSignature(tokens.AccessToken, queryString));

            address = QueryHelpers.AddQueryString(address, queryString);

            HttpResponseMessage response = await Backchannel.GetAsync(address, Context.RequestAborted);
            if (!response.IsSuccessStatusCode)
            {
                Logger.LogError("An error occurred while retrieving the user profile: the remote server " +
                                "returned a {Status} response with the following payload: {Headers} {Body}.",
                                /* Status: */ response.StatusCode,
                                /* Headers: */ response.Headers.ToString(),
                                /* Body: */ await response.Content.ReadAsStringAsync());

                throw new HttpRequestException($"An error occurred when retrieving user information ({response.StatusCode}).");
            }

            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());

            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), properties, Options.AuthenticationScheme);
            var context = new OAuthCreatingTicketContext(ticket, Context, Options, Backchannel, tokens, payload);

            string identifier = OdnoklassnikiHelper.GetId(payload);
            if (!string.IsNullOrEmpty(identifier))
            {
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, identifier, ClaimValueTypes.String, Options.ClaimsIssuer));
            }

            string name = OdnoklassnikiHelper.GetName(payload);
            if (!string.IsNullOrEmpty(name))
            {
                identity.AddClaim(new Claim(ClaimTypes.Name, name, ClaimValueTypes.String, Options.ClaimsIssuer));
            }

            string email = OdnoklassnikiHelper.GetEmail(payload);
            if (!string.IsNullOrEmpty(email))
            {
                identity.AddClaim(new Claim(ClaimTypes.Email, email, ClaimValueTypes.Email, Options.ClaimsIssuer));
            }

            string firstName = OdnoklassnikiHelper.GetFirstName(payload);
            if (!string.IsNullOrEmpty(firstName))
            {
                identity.AddClaim(new Claim(ClaimTypes.GivenName, firstName, ClaimValueTypes.String, Options.ClaimsIssuer));
            }

            string lastName = OdnoklassnikiHelper.GetLastName(payload);
            if (!string.IsNullOrEmpty(lastName))
            {
                identity.AddClaim(new Claim(ClaimTypes.Surname, lastName, ClaimValueTypes.String, Options.ClaimsIssuer));
            }

            string link = OdnoklassnikiHelper.GetLink(payload);
            if (!string.IsNullOrEmpty(link))
            {
                identity.AddClaim(new Claim("urn:odnoklassniki:link", link, ClaimValueTypes.String, Options.ClaimsIssuer));
            }

            await Options.Events.CreatingTicket(context);

            return context.Ticket;
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

            var utf8nobom = new UTF8Encoding(false);

            string GetMd5Hash(string input)
            {
                using (var provider = MD5.Create())
                {
                    var bytes = utf8nobom.GetBytes(input);
                    bytes = provider.ComputeHash(bytes);
                    return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
                }
            }

            return GetMd5Hash(parametersValue + GetMd5Hash(accessToken + Options.ClientSecret));
        }
    }
}
