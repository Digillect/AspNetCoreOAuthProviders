// Copyright (c) aspnet-contrib project (Albert Zakiev, Kévin Chalet). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers for more information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Digillect.AspNetCore.Authentication.VKontakte
{
    /// <summary>
    /// Configuration options for <see cref="VKontakteMiddleware"/>.
    /// </summary>
    public class VKontakteOptions : OAuthOptions
    {
        /// <summary>
        /// Initializes a new <see cref="VKontakteOptions"/>.
        /// </summary>
        public VKontakteOptions()
        {
            AuthenticationScheme = VKontakteDefaults.AuthenticationScheme;
            DisplayName = VKontakteDefaults.DisplayName;
            ClaimsIssuer = VKontakteDefaults.Issuer;

            CallbackPath = new PathString("/signin-vkontakte");

            AuthorizationEndpoint = VKontakteDefaults.AuthorizationEndpoint;
            TokenEndpoint = VKontakteDefaults.TokenEndpoint;
            UserInformationEndpoint = VKontakteDefaults.UserInformationEndpoint;

            Scope.Add("email");
        }

        /// <summary>
        /// Gets or sets the VK API version to use.
        /// See https://vk.com/dev/versions for more information.
        /// </summary>
        public string ApiVersion { get; set; } = VKontakteDefaults.ApiVersion;

        /// <summary>
        /// Gets the list of fields to retrieve from the user information endpoint.
        /// See https://vk.com/dev/objects/user for more information.
        /// </summary>
        public ISet<string> Fields { get; } = new HashSet<string>
        {
            "id",
            "first_name",
            "last_name",
            "screen_name",
            "photo_50"
        };
    }
}
