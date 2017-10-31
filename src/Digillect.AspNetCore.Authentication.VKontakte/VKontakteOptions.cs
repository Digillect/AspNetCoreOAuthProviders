// Copyright (c) aspnet-contrib project (Albert Zakiev, Kévin Chalet). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers for more information.

using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;

namespace Digillect.AspNetCore.Authentication.VKontakte
{
    /// <summary>
    /// Configuration options for <see cref="VKontakteHandler"/>.
    /// </summary>
    public class VKontakteOptions : OAuthOptions
    {
        /// <summary>
        /// Initializes a new <see cref="VKontakteOptions"/>.
        /// </summary>
        public VKontakteOptions()
        {
            ClaimsIssuer = VKontakteDefaults.Issuer;

            CallbackPath = new PathString("/signin-vkontakte");

            AuthorizationEndpoint = VKontakteDefaults.AuthorizationEndpoint;
            TokenEndpoint = VKontakteDefaults.TokenEndpoint;
            UserInformationEndpoint = VKontakteDefaults.UserInformationEndpoint;

            Scope.Add("email");

            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "uid");
            ClaimActions.MapJsonKey(ClaimTypes.GivenName, "first_name");
            ClaimActions.MapJsonKey(ClaimTypes.Surname, "last_name");
            ClaimActions.MapJsonKey(ClaimTypes.Name, "screen_name");
            ClaimActions.MapJsonKey("urn:vkontakte:link", "photo_50");
        }

        /// <summary>
        /// Gets the list of fields to retrieve from the user information endpoint.
        /// See https://vk.com/dev/objects/user for more information.
        /// </summary>
        public ISet<string> Fields { get; } = new HashSet<string>
        {
            "uid",
            "first_name",
            "last_name",
            "screen_name",
            "photo_50"
        };
    }
}
