// Copyright (c) Andrew Nefedkin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace Digillect.AspNetCore.Authentication.Odnoklassniki
{
    /// <summary>
    /// Contains static methods that allow to extract user's information from a <see cref="JObject"/>
    /// instance retrieved from Odnoklassniki after a successful authentication process.
    /// </summary>
    public static class OdnoklassnikiHelper
    {
        /// <summary>
        /// Gets the identifier associated with the logged in user.
        /// </summary>
        public static string GetId([NotNull] JObject user) => user?.Value<string>("uid");

        /// <summary>
        /// Gets the name associated with the logged in user.
        /// </summary>
        public static string GetName([NotNull] JObject user) => user?.Value<string>("name");

        /// <summary>
        /// Gets the e-mail associated with the logged in user.
        /// </summary>
        public static string GetEmail([NotNull] JObject user) => user?.Value<string>("email");

        /// <summary>
        /// Gets the first name associated with the logged in user.
        /// </summary>
        public static string GetFirstName([NotNull] JObject user) => user?.Value<string>("first_name");

        /// <summary>
        /// Gets the last name associated with the logged in user.
        /// </summary>
        public static string GetLastName([NotNull] JObject user) => user?.Value<string>("last_name");

        /// <summary>
        /// Gets the URL of the user profile picture.
        /// </summary>
        public static string GetLink([NotNull] JObject user) => user?.Value<string>("pic_1");
    }
}