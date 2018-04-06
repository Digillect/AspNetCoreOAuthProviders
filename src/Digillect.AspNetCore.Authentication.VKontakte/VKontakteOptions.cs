// Copyright (c) aspnet-contrib project (Albert Zakiev, Kévin Chalet). All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.
// See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
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

            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
            ClaimActions.MapJsonKey(ClaimTypes.GivenName, "first_name");
            ClaimActions.MapJsonKey(ClaimTypes.Surname, "last_name");
            ClaimActions.MapJsonKey(ClaimTypes.Name, "screen_name");
            ClaimActions.MapJsonKey("urn:vkontakte:link", "photo_50");
        }

        /// <summary>
        /// Gets or sets the VK API version to use.
        /// See https://vk.com/dev/versions for more information.
        /// </summary>
        public string ApiVersion { get; set; } = VKontakteDefaults.ApiVersion;

        /// <summary>
        /// Gets or sets authorization page appearance.
        /// </summary>
        /// <value>
        /// <list type="bullet">
        /// <listheader>The supported values are:</listheader>
        /// <item>page — authorization form in a separate window;</item>
        /// <item>popup — a pop-up window;</item>
        /// <item>mobile — authorization for mobile devices(uses no Javascript).</item>
        /// </list>
        /// </value>
        /// <remarks>
        /// See https://vk.com/dev/authcode_flow_user of https://vk.com/dev/oauth_dialog for actual information.
        /// </remarks>
        public string AuthorizationPageAppearance { get; set; }

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

        /// <summary>
        /// Check that the options are valid.  Should throw an exception if things are not ok.
        /// </summary>
        public override void Validate()
        {
            base.Validate();

            if (string.IsNullOrEmpty(ApiVersion))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.Exception_OptionMustBeProvided, nameof(ApiVersion)), nameof(ApiVersion));
            }
        }
    }
}
