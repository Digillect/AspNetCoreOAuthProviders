// Copyright (c) Andrew Nefedkin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Digillect.AspNetCore.Authentication.Odnoklassniki
{
    /// <summary>
    /// Configuration options for <see cref="OdnoklassnikiMiddleware"/>.
    /// </summary>
    public class OdnoklassnikiOptions : OAuthOptions
    {
        /// <summary>
        /// Initializes a new <see cref="OdnoklassnikiOptions"/>.
        /// </summary>
        public OdnoklassnikiOptions()
        {
            AuthenticationScheme = OdnoklassnikiDefaults.AuthenticationScheme;
            DisplayName = OdnoklassnikiDefaults.DisplayName;
            ClaimsIssuer = OdnoklassnikiDefaults.Issuer;

            CallbackPath = new PathString("/signin-odnoklassniki");

            AuthorizationEndpoint = OdnoklassnikiDefaults.AuthorizationEndpoint;
            TokenEndpoint = OdnoklassnikiDefaults.TokenEndpoint;
            UserInformationEndpoint = OdnoklassnikiDefaults.UserInformationEndpoint;

            Scope.Add("VALUABLE_ACCESS");
            Scope.Add("GET_EMAIL");
        }

        /// <summary>
        /// Gets or sets the application key used to retrieve user details from Odnoklassniki's API.
        /// </summary>
        public string ApplicationKey { get; set; }

        /// <summary>
        /// Gets the list of fields to retrieve from the user information endpoint.
        /// See https://apiok.ru/en/dev/methods/rest/users/users.getCurrentUser for more information.
        /// </summary>
        public ISet<string> Fields { get; } = new HashSet<string>
        {
            "uid",
            "name",
            "email",
            "first_name",
            "last_name",
            "pic_1"
        };
    }
}
