// Copyright (c) Andrew Nefedkin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Text.Encodings.Web;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Digillect.AspNetCore.Authentication.Odnoklassniki
{
    public class OdnoklassnikiMiddleware : OAuthMiddleware<OdnoklassnikiOptions>
    {
        public OdnoklassnikiMiddleware(
            [NotNull] RequestDelegate next,
            [NotNull] IDataProtectionProvider dataProtectionProvider,
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] UrlEncoder encoder,
            [NotNull] IOptions<SharedAuthenticationOptions> sharedOptions,
            [NotNull] IOptions<OdnoklassnikiOptions> options)
            : base(next, dataProtectionProvider, loggerFactory, encoder, sharedOptions, options)
        {
            if (string.IsNullOrEmpty(Options.ApplicationKey))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.Exception_OptionMustBeProvided, nameof(Options.ApplicationKey)));
            }
        }

        protected override AuthenticationHandler<OdnoklassnikiOptions> CreateHandler()
        {
            return new OdnoklassnikiHandler(Backchannel);
        }
    }
}
