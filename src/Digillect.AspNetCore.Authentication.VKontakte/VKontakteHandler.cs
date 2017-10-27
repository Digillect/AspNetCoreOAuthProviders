// Copyright (c) aspnet-contrib project (Albert Zakiev, Kévin Chalet). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers for more information.
// E-mail retrieval routine by f14shm4n.

using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Digillect.AspNetCore.Authentication.VKontakte
{
    internal class VKontakteHandler : OAuthHandler<VKontakteOptions>
    {
        public VKontakteHandler(HttpClient client)
            : base(client)
        {
        }

        protected override async Task<AuthenticationTicket> CreateTicketAsync([NotNull] ClaimsIdentity identity, [NotNull] AuthenticationProperties properties, [NotNull] OAuthTokenResponse tokens)
        {
            var address = QueryHelpers.AddQueryString(Options.UserInformationEndpoint, "access_token", tokens.AccessToken);

            if (Options.Fields.Count != 0)
            {
                address = QueryHelpers.AddQueryString(address, "fields", string.Join(",", Options.Fields));
            }

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

            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
            var user = (JObject) payload["response"][0];

            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), properties, Options.AuthenticationScheme);
            var context = new OAuthCreatingTicketContext(ticket, Context, Options, Backchannel, tokens, user);

            string identifier = VKontakteHelper.GetId(user);
            if (!string.IsNullOrEmpty(identifier))
            {
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, identifier, ClaimValueTypes.String, Options.ClaimsIssuer));
            }

            string firstName = VKontakteHelper.GetFirstName(user);
            if (!string.IsNullOrEmpty(firstName))
            {
                identity.AddClaim(new Claim(ClaimTypes.GivenName, firstName, ClaimValueTypes.String, Options.ClaimsIssuer));
            }

            string lastName = VKontakteHelper.GetLastName(user);
            if (!string.IsNullOrEmpty(lastName))
            {
                identity.AddClaim(new Claim(ClaimTypes.Surname, lastName, ClaimValueTypes.String, Options.ClaimsIssuer));
            }

            string name = VKontakteHelper.GetScreenName(user);
            if (!string.IsNullOrEmpty(name))
            {
                identity.AddClaim(new Claim(ClaimTypes.Name, name, ClaimValueTypes.String, Options.ClaimsIssuer));
            }

            string photo = VKontakteHelper.GetPhoto(user);
            if (!string.IsNullOrEmpty(photo))
            {
                identity.AddClaim(new Claim("urn:vkontakte:link", photo, ClaimValueTypes.String, Options.ClaimsIssuer));
            }

            if (!identity.HasClaim(claim => claim.Type == ClaimTypes.Email) && Options.Scope.Contains("email"))
            {
                var email = tokens.Response["email"]?.Value<string>();
                if (!string.IsNullOrEmpty(email))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Email, email, ClaimValueTypes.Email, Options.ClaimsIssuer));
                }
            }

            await Options.Events.CreatingTicket(context);

            return context.Ticket;
        }
    }
}
