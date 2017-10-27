// Copyright (c) aspnet-contrib project (Albert Zakiev, Kévin Chalet). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers for more information.

using Microsoft.AspNetCore.Builder;

namespace Digillect.AspNetCore.Authentication.VKontakte
{
    /// <summary>
    /// Default values used by the VKontakte authentication middleware.
    /// </summary>
    public static class VKontakteDefaults
    {
        /// <summary>
        /// Default value for <see cref="AuthenticationOptions.AuthenticationScheme"/>.
        /// </summary>
        public const string AuthenticationScheme = "VKontakte";

        /// <summary>
        /// Default value for <see cref="RemoteAuthenticationOptions.DisplayName"/>.
        /// </summary>
        public const string DisplayName = "VKontakte";

        /// <summary>
        /// Default value for <see cref="AuthenticationOptions.ClaimsIssuer"/>.
        /// </summary>
        public const string Issuer = "VKontakte";

        /// <summary>
        /// Default value for <see cref="OAuthOptions.AuthorizationEndpoint"/>.
        /// </summary>
        public const string AuthorizationEndpoint = "https://oauth.vk.com/authorize";

        /// <summary>
        /// Default value for <see cref="OAuthOptions.TokenEndpoint"/>.
        /// </summary>
        public const string TokenEndpoint = "https://oauth.vk.com/access_token";

        /// <summary>
        /// Default value for <see cref="OAuthOptions.UserInformationEndpoint"/>.
        /// </summary>
        public const string UserInformationEndpoint = "https://api.vk.com/method/users.get.json";
    }
}
