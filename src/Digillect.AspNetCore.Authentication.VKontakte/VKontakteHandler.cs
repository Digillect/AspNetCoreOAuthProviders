// Copyright (c) aspnet-contrib project (Albert Zakiev, Kévin Chalet). All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.
// See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers for more information.
// E-mail retrieval routine by f14shm4n.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Digillect.AspNetCore.Authentication.VKontakte
{
    internal class VKontakteHandler : OAuthHandler<VKontakteOptions>
    {
        public VKontakteHandler(
            [NotNull] IOptionsMonitor<VKontakteOptions> options,
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
                ["access_token"] = tokens.AccessToken,
                ["v"] = Options.ApiVersion
            };

            if (Options.Fields.Count != 0)
            {
                queryString.Add("fields", string.Join(",", Options.Fields));
            }

            var address = QueryHelpers.AddQueryString(Options.UserInformationEndpoint, queryString);

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

            if (Options.Scope.Contains("email"))
            {
                var email = tokens.Response.RootElement.GetString("email");
                if (!string.IsNullOrEmpty(email))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Email, email, ClaimValueTypes.Email, Options.ClaimsIssuer));
                }
            }

            using (var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync()))
            {
                if (payload.RootElement.TryGetProperty("error", out var error))
                {
                    Logger.LogError("An error occurred while retrieving the user profile: the provider returned an error {ErrorCode} with the message: \"{ErrorMessage}\"",
                                    /* ErrorCode */ error.GetProperty("error_code").GetInt32(),
                                    /* ErrorMessage */ error.GetProperty("error_msg").GetString());

                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, tokens.Response.RootElement.GetString("user_id"), ClaimValueTypes.String, Options.ClaimsIssuer));

                    return await base.CreateTicketAsync(identity, properties, tokens);
                }

                var user = payload.RootElement.TryGetProperty("response", out var users) ? users[0] : default;

                var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, user);
                context.RunClaimActions();

                await Events.CreatingTicket(context);

                return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
            }
        }

        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            var url = base.BuildChallengeUrl(properties, redirectUri);

            if (!string.IsNullOrEmpty(Options.AuthorizationPageAppearance))
            {
                url = QueryHelpers.AddQueryString(url, "display", Options.AuthorizationPageAppearance);
            }

            return url;
        }
    }
}
