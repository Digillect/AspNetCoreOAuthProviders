// Copyright (c) aspnet-contrib project (Albert Zakiev, Kévin Chalet). All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.
// See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers for more information.

using System;
using Digillect.AspNetCore.Authentication.VKontakte;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods to add Vkontakte authentication handler.
    /// </summary>
    public static class VKontakteExtensions
    {
        /// <summary>
        /// Adds the <see cref="VKontakteHandler"/> to the specified
        /// <see cref="AuthenticationBuilder"/>, which enables Vkontakte authentication capabilities.
        /// </summary>
        /// <param name="builder">The authentication builder.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static AuthenticationBuilder AddVKontakte([NotNull] this AuthenticationBuilder builder)
            => builder.AddVKontakte(VKontakteDefaults.AuthenticationScheme, _ => { });

        /// <summary>
        /// Adds the <see cref="VKontakteHandler"/> to the specified
        /// <see cref="AuthenticationBuilder"/>, which enables Vkontakte authentication capabilities.
        /// </summary>
        /// <param name="builder">The authentication builder.</param>
        /// <param name="configureOptions">An action delegate to configure the provided <see cref="VKontakteOptions"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static AuthenticationBuilder AddVKontakte(
            [NotNull] this AuthenticationBuilder builder,
            [CanBeNull] Action<VKontakteOptions> configureOptions)
            => builder.AddVKontakte(VKontakteDefaults.AuthenticationScheme, configureOptions);

        /// <summary>
        /// Adds the <see cref="VKontakteHandler"/> to the specified
        /// <see cref="AuthenticationBuilder"/>, which enables Vkontakte authentication capabilities.
        /// </summary>
        /// <param name="builder">The authentication builder.</param>
        /// <param name="authenticationScheme"></param>
        /// <param name="configureOptions">An action delegate to configure the provided <see cref="VKontakteOptions"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static AuthenticationBuilder AddVKontakte(
            [NotNull] this AuthenticationBuilder builder,
            [NotNull] string authenticationScheme,
            [CanBeNull] Action<VKontakteOptions> configureOptions)
            => builder.AddVKontakte(authenticationScheme, VKontakteDefaults.DisplayName, configureOptions);

        /// <summary>
        /// Adds the <see cref="VKontakteHandler"/> to the specified
        /// <see cref="AuthenticationBuilder"/>, which enables Vkontakte authentication capabilities.
        /// </summary>
        /// <param name="builder">The authentication builder.</param>
        /// <param name="authenticationScheme">The name of the scheme being added.</param>
        /// <param name="displayName">The display name for the scheme.</param>
        /// <param name="configureOptions">An action delegate to configure the provided <see cref="VKontakteOptions"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static AuthenticationBuilder AddVKontakte(
            [NotNull] this AuthenticationBuilder builder,
            [NotNull] string authenticationScheme,
            [CanBeNull] string displayName,
            [CanBeNull] Action<VKontakteOptions> configureOptions)
            => builder.AddOAuth<VKontakteOptions, VKontakteHandler>(authenticationScheme, displayName, configureOptions);
    }
}
