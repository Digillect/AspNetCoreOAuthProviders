// Copyright (c) aspnet-contrib project (Albert Zakiev, Kévin Chalet). All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.
// See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers for more information.

using System;
using Digillect.AspNetCore.Authentication.VKontakte;
using JetBrains.Annotations;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extension methods to add VKontakte authentication capabilities to an HTTP application pipeline.
    /// </summary>
    public static class VKontakteAppBuilderExtensions
    {
        /// <summary>
        /// Obsolete, see https://go.microsoft.com/fwlink/?linkid=845470
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the handler to.</param>
        /// <param name="options">A <see cref="VKontakteOptions"/> that specifies options for the handler.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470", true)]
        public static IApplicationBuilder UseVKontakteAuthentication(
            [NotNull] this IApplicationBuilder app,
            [NotNull] VKontakteOptions options)
        {
            throw new NotSupportedException("This method is no longer supported, see https://go.microsoft.com/fwlink/?linkid=845470");
        }

        /// <summary>
        /// Obsolete, see https://go.microsoft.com/fwlink/?linkid=845470
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the handler to.</param>
        /// <param name="configuration">An action delegate to configure the provided <see cref="VKontakteOptions"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470", true)]
        public static IApplicationBuilder UseVKontakteAuthentication(
            [NotNull] this IApplicationBuilder app,
            [NotNull] Action<VKontakteOptions> configuration)
        {
            throw new NotSupportedException("This method is no longer supported, see https://go.microsoft.com/fwlink/?linkid=845470");
        }
    }
}
