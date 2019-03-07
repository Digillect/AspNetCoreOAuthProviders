// Copyright (c) Andrew Nefedkin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;

namespace Digillect.AspNetCore.Authentication.Odnoklassniki
{
    /// <summary>
    /// Configuration options for <see cref="OdnoklassnikiHandler"/>.
    /// </summary>
    public class OdnoklassnikiOptions : OAuthOptions
    {
        /// <summary>
        /// Initializes a new <see cref="OdnoklassnikiOptions"/>.
        /// </summary>
        public OdnoklassnikiOptions()
        {
            ClaimsIssuer = OdnoklassnikiDefaults.Issuer;

            CallbackPath = new PathString("/signin-odnoklassniki");

            AuthorizationEndpoint = OdnoklassnikiDefaults.AuthorizationEndpoint;
            TokenEndpoint = OdnoklassnikiDefaults.TokenEndpoint;
            UserInformationEndpoint = OdnoklassnikiDefaults.UserInformationEndpoint;

            Scope.Add("VALUABLE_ACCESS");
            Scope.Add("GET_EMAIL");

            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "uid");
            ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
            ClaimActions.MapJsonKey(ClaimTypes.Email, "email", ClaimValueTypes.Email);
            ClaimActions.MapJsonKey(ClaimTypes.GivenName, "first_name");
            ClaimActions.MapJsonKey(ClaimTypes.Surname, "last_name");
            ClaimActions.MapJsonKey("urn:odnoklassniki:link", "pic_1");
        }

        /// <summary>
        /// Gets or sets the application key used to retrieve user details from Odnoklassniki's API.
        /// </summary>
        public string ApplicationKey { get; set; }

        /// <summary>
        /// Gets the list of fields to retrieve from the user information endpoint.
        /// See https://apiok.ru/en/dev/methods/rest/users/users.getCurrentUser for more information.
        /// </summary>
        public ICollection<string> Fields { get; } = new HashSet<string>
        {
            "uid",
            "name",
            "email",
            "first_name",
            "last_name",
            "pic_1"
        };

        /// <summary>
        /// Check that the options are valid.  Should throw an exception if things are not ok.
        /// </summary>
        public override void Validate()
        {
            base.Validate();

            if (string.IsNullOrEmpty(ApplicationKey))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.Exception_OptionMustBeProvided, nameof(ApplicationKey)), nameof(ApplicationKey));
            }
        }
    }
}
