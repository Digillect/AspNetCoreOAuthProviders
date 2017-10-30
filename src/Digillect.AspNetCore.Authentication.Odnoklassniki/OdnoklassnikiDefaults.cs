// Copyright (c) Andrew Nefedkin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Builder;

namespace Digillect.AspNetCore.Authentication.Odnoklassniki
{
    /// <summary>
    /// Default values used by the Odnoklassniki authentication middleware.
    /// </summary>
    public static class OdnoklassnikiDefaults
    {
        /// <summary>
        /// Default value for <see cref="AuthenticationOptions.AuthenticationScheme"/>
        /// </summary>
        public const string AuthenticationScheme = "Odnoklassniki";

        /// <summary>
        /// Default value for <see cref="RemoteAuthenticationOptions.DisplayName"/>
        /// </summary>
        public const string DisplayName = "Odnoklassniki";

        /// <summary>
        /// Default value for <see cref="AuthenticationOptions.ClaimsIssuer"/>
        /// </summary>
        public const string Issuer = "Odnoklassniki";

        /// <summary>
        /// Default value for <see cref="OAuthOptions.AuthorizationEndpoint"/>.
        /// </summary>
        public const string AuthorizationEndpoint = "https://connect.ok.ru/oauth/authorize";

        /// <summary>
        /// Default value for <see cref="OAuthOptions.TokenEndpoint"/>.
        /// </summary>
        public const string TokenEndpoint = "https://api.ok.ru/oauth/token.do";

        /// <summary>
        /// Default value for <see cref="OAuthOptions.UserInformationEndpoint"/>.
        /// </summary>
        public const string UserInformationEndpoint = "https://api.ok.ru/api/users/getCurrentUser";
    }
}
