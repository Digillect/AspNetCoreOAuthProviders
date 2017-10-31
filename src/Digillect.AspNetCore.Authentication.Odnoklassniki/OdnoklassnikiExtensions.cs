/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

using System;
using Digillect.AspNetCore.Authentication.Odnoklassniki;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OdnoklassnikiExtensions
    {
        /// <summary>
        /// Adds the <see cref="OdnoklassnikiHandler"/> to the specified
        /// <see cref="AuthenticationBuilder"/>, which enables Odnoklassniki authentication capabilities.
        /// </summary>
        /// <param name="builder">The authentication builder.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static AuthenticationBuilder AddOdnoklassniki([NotNull] this AuthenticationBuilder builder)
            => builder.AddOdnoklassniki(OdnoklassnikiDefaults.AuthenticationScheme, _ => { });

        /// <summary>
        /// Adds the <see cref="OdnoklassnikiHandler"/> to the specified
        /// <see cref="AuthenticationBuilder"/>, which enables Odnoklassniki authentication capabilities.
        /// </summary>
        /// <param name="builder">The authentication builder.</param>
        /// <param name="configureOptions">An action delegate to configure the provided <see cref="OdnoklassnikiOptions"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static AuthenticationBuilder AddOdnoklassniki(
            [NotNull] this AuthenticationBuilder builder,
            [CanBeNull] Action<OdnoklassnikiOptions> configureOptions)
            => builder.AddOdnoklassniki(OdnoklassnikiDefaults.AuthenticationScheme, configureOptions);

        /// <summary>
        /// Adds the <see cref="OdnoklassnikiHandler"/> to the specified
        /// <see cref="AuthenticationBuilder"/>, which enables Odnoklassniki authentication capabilities.
        /// </summary>
        /// <param name="builder">The authentication builder.</param>
        /// <param name="authenticationScheme"></param>
        /// <param name="configureOptions">An action delegate to configure the provided <see cref="OdnoklassnikiOptions"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static AuthenticationBuilder AddOdnoklassniki(
            [NotNull] this AuthenticationBuilder builder,
            [NotNull] string authenticationScheme,
            [CanBeNull] Action<OdnoklassnikiOptions> configureOptions)
            => builder.AddOdnoklassniki(authenticationScheme, OdnoklassnikiDefaults.DisplayName, configureOptions);

        /// <summary>
        /// Adds the <see cref="OdnoklassnikiHandler"/> to the specified
        /// <see cref="AuthenticationBuilder"/>, which enables Odnoklassniki authentication capabilities.
        /// </summary>
        /// <param name="builder">The authentication builder.</param>
        /// <param name="authenticationScheme">The name of the scheme being added.</param>
        /// <param name="displayName">The display name for the scheme.</param>
        /// <param name="configureOptions">An action delegate to configure the provided <see cref="OdnoklassnikiOptions"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static AuthenticationBuilder AddOdnoklassniki(
            [NotNull] this AuthenticationBuilder builder,
            [NotNull] string authenticationScheme,
            [CanBeNull] string displayName,
            [CanBeNull] Action<OdnoklassnikiOptions> configureOptions)
            => builder.AddOAuth<OdnoklassnikiOptions, OdnoklassnikiHandler>(authenticationScheme, displayName, configureOptions);
    }
}
