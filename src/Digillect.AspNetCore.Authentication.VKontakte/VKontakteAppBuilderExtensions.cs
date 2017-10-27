// Copyright (c) aspnet-contrib project (Albert Zakiev, Kévin Chalet). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers for more information.

using System;
using Digillect.AspNetCore.Authentication.VKontakte;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extension methods to add VKontakte authentication capabilities to an HTTP application pipeline.
    /// </summary>
    public static class VKontakteAppBuilderExtensions
    {
        /// <summary>
        /// Adds the <see cref="VKontakteMiddleware"/> middleware to the specified
        /// <see cref="IApplicationBuilder"/>, which enables VKontakte authentication capabilities.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <param name="options">A <see cref="VKontakteOptions"/> that specifies options for the middleware.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseVKontakteAuthentication(
            [NotNull] this IApplicationBuilder app,
            [NotNull] VKontakteOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return app.UseMiddleware<VKontakteMiddleware>(Options.Create(options));
        }

        /// <summary>
        /// Adds the <see cref="VKontakteMiddleware"/> middleware to the specified <see cref="IApplicationBuilder"/>, which enables VKontakte authentication capabilities.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <param name="configuration">An action delegate to configure the provided <see cref="VKontakteOptions"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseVKontakteAuthentication(
            [NotNull] this IApplicationBuilder app,
            [NotNull] Action<VKontakteOptions> configuration)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var options = new VKontakteOptions();
            configuration(options);

            return app.UseMiddleware<VKontakteMiddleware>(Options.Create(options));
        }
    }
}
