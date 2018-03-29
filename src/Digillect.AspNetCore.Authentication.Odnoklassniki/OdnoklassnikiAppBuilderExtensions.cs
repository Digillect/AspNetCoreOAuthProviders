// Copyright (c) Andrew Nefedkin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System;
using Digillect.AspNetCore.Authentication.Odnoklassniki;
using JetBrains.Annotations;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extension methods to add Odnoklassniki authentication capabilities to an HTTP application pipeline.
    /// </summary>
    public static class OdnoklassnikiAppBuilderExtensions
    {
        /// <summary>
        /// Obsolete, see https://go.microsoft.com/fwlink/?linkid=845470
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the handler to.</param>
        /// <param name="options">A <see cref="OdnoklassnikiOptions"/> that specifies options for the handler.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470", true)]
        public static IApplicationBuilder UseOdnoklassnikiAuthentication(
            [NotNull] this IApplicationBuilder app,
            [NotNull] OdnoklassnikiOptions options)
        {
            throw new NotSupportedException("This method is no longer supported, see https://go.microsoft.com/fwlink/?linkid=845470");
        }

        /// <summary>
        /// Obsolete, see https://go.microsoft.com/fwlink/?linkid=845470
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the handler to.</param>
        /// <param name="configuration">An action delegate to configure the provided <see cref="OdnoklassnikiOptions"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470", true)]
        public static IApplicationBuilder UseOdnoklassnikiAuthentication(
            [NotNull] this IApplicationBuilder app,
            [NotNull] Action<OdnoklassnikiOptions> configuration)
        {
            throw new NotSupportedException("This method is no longer supported, see https://go.microsoft.com/fwlink/?linkid=845470");
        }
    }
}
