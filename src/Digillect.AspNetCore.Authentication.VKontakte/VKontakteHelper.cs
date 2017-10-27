// Copyright (c) aspnet-contrib project (Albert Zakiev, Kévin Chalet). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers for more information.

using System;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace Digillect.AspNetCore.Authentication.VKontakte
{
    /// <summary>
    /// Contains static methods that allow to extract user's information from a <see cref="JObject"/>
    /// instance retrieved from VKontakte after a successful authentication process.
    /// </summary>
    public static class VKontakteHelper
    {
        /// <summary>
        /// Gets the identifier associated with the logged in user.
        /// </summary>
        public static string GetId([NotNull] JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("uid");
        }

        /// <summary>
        /// Gets the first name associated with the logged in user.
        /// </summary>
        public static string GetFirstName([NotNull] JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("first_name");
        }

        /// <summary>
        /// Gets the last name associated with the logged in user.
        /// </summary>
        public static string GetLastName([NotNull] JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("last_name");
        }

        /// <summary>
        /// Gets the screen name associated with the logged in user.
        /// </summary>
        public static string GetScreenName([NotNull] JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("screen_name");
        }

        /// <summary>
        /// Gets the URL of the user profile picture.
        /// </summary>
        public static string GetPhoto([NotNull] JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("photo_50");
        }
    }
}
