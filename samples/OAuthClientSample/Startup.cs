using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Digillect.AspNetCore.Authentication.VKontakte;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace OAuthClientSample
{
    /* Note all servers must use the same address and port because these are pre-registered with the various providers. */
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options => options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                LoginPath = new PathString("/login")
            });

            // You must first create an app with VKontakte and add its ID and Secret to your user-secrets.
            // https://vk.com/apps?act=manage
            app.UseOAuthAuthentication(new OAuthOptions
            {
                AuthenticationScheme = "VKontakte-AccessToken",
                DisplayName = "VKontakte AccessToken only",
                ClaimsIssuer = VKontakteDefaults.Issuer,
                CallbackPath = new PathString("/signin-vkontakte-token"),
                AuthorizationEndpoint = VKontakteDefaults.AuthorizationEndpoint,
                TokenEndpoint = VKontakteDefaults.TokenEndpoint,
                Scope = { "email" },
                ClientId = Configuration["VKontakte:ClientId"],
                ClientSecret = Configuration["VKontakte:ClientSecret"],
                SaveTokens = true,
                Events = new OAuthEvents
                {
                    OnCreatingTicket = context =>
                    {
                        var email = context.TokenResponse.Response["email"]?.Value<string>();
                        if (!string.IsNullOrEmpty(email))
                        {
                            context.Identity.AddClaim(new Claim(ClaimTypes.Email, email, ClaimValueTypes.Email, context.Options.ClaimsIssuer));
                        }
                        return Task.FromResult(0);
                    },
                    OnRemoteFailure = HandleOnRemoteFailure
                }
            });

            // You must first create an app with VKontakte and add its ID and Secret to your user-secrets.
            // https://vk.com/apps?act=manage
            app.UseVKontakteAuthentication(o =>
            {
                o.ClientId = Configuration["VKontakte:ClientId"];
                o.ClientSecret = Configuration["VKontakte:ClientSecret"];
                o.SaveTokens = true;
                o.Events = new OAuthEvents
                {
                    OnRemoteFailure = HandleOnRemoteFailure
                };
            });

            // A guard against tricky browsers
            app.Map("/favicon.ico", notFoundApp =>
            {
                notFoundApp.Run(context =>
                {
                    context.Response.StatusCode = 404;
                    return Task.FromResult(0);
                });
            });

            // Choose an authentication type
            app.Map("/login", signinApp =>
            {
                signinApp.Run(async context =>
                {
                    var authType = context.Request.Query["authscheme"];
                    if (!string.IsNullOrEmpty(authType))
                    {
                        // By default the client will be redirect back to the URL that issued the challenge (/login?authscheme=foo),
                        // send them to the home page instead (/).
                        await context.Authentication.ChallengeAsync(authType, new AuthenticationProperties { RedirectUri = "/" });
                        return;
                    }

                    var response = context.Response;
                    response.ContentType = "text/html; charset=utf-8";
                    await response.WriteAsync("<html><body>");
                    await response.WriteAsync("Choose an authentication scheme:<ul>");
                    foreach (var provider in context.Authentication.GetAuthenticationSchemes())
                    {
                        await response.WriteAsync("<li><a href=\"?authscheme=" + UrlEncoder.Default.Encode(provider.AuthenticationScheme) + "\">" + (provider.DisplayName ?? "(suppressed)") + "</a></li>");
                    }
                    await response.WriteAsync("</ul></body></html>");
                });
            });

            // Sign-out to remove the user cookie.
            app.Map("/logout", signoutApp =>
            {
                signoutApp.Run(async context =>
                {
                    var response = context.Response;
                    response.ContentType = "text/html; charset=utf-8";
                    await context.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    await response.WriteAsync("<html><body>");
                    await response.WriteAsync("You have been logged out. Goodbye " + context.User.Identity.Name + "<br>");
                    await response.WriteAsync("<a href=\"/\">Home</a>");
                    await response.WriteAsync("</body></html>");
                });
            });

            // Display the remote error
            app.Map("/error", errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var response = context.Response;
                    response.ContentType = "text/html; charset=utf-8";
                    await response.WriteAsync("<html><body>");
                    await response.WriteAsync("An remote failure has occurred: " + context.Request.Query["FailureMessage"] + "<br>");
                    await response.WriteAsync("<a href=\"/\">Home</a>");
                    await response.WriteAsync("</body></html>");
                });
            });

            app.Run(async context =>
            {
                // CookieAuthenticationOptions.AutomaticAuthenticate = true (default) causes User to be set
                var user = context.User;

                // This is what [Authorize] calls
                // var user = await context.Authentication.AuthenticateAsync(AuthenticationManager.AutomaticScheme);

                // Deny anonymous request beyond this point.
                if (user == null || !user.Identities.Any(identity => identity.IsAuthenticated))
                {
                    // This is what [Authorize] calls
                    // The cookie middleware will handle this and redirect to /login
                    await context.Authentication.ChallengeAsync();

                    return;
                }

                // Display user information
                var response = context.Response;
                response.ContentType = "text/html; charset=utf-8";
                await response.WriteAsync("<html><body>");
                await response.WriteAsync("<h1>Hello " + (context.User.Identity.Name ?? "anonymous") + "</h1><h2>Claims:</h2><ul>");
                foreach (var claim in context.User.Claims)
                {
                    await response.WriteAsync($"<dt>{HtmlEncoder.Default.Encode(claim.Type)}</dt><dd>{HtmlEncoder.Default.Encode(claim.Value)}</dd>");
                }
                await response.WriteAsync("</ul><h2>Tokens:</h2><ul>");
                foreach (var token in new[] { "access_token", "refresh_token", "token_type", "expires_at" })
                {
                    await response.WriteAsync("<dt>" + token + "</dt><dd>" + HtmlEncoder.Default.Encode(await context.Authentication.GetTokenAsync(token) ?? "") + "</dd>");
                }
                await response.WriteAsync("</ul><a href=\"/logout\">Logout</a><br>");
                await response.WriteAsync("</body></html>");
            });
        }

        private async Task HandleOnRemoteFailure(FailureContext context)
        {
            var response = context.Response;
            response.StatusCode = 500;
            response.ContentType = "text/html; charset=utf-8";
            await response.WriteAsync("<html><body>");
            await response.WriteAsync("A remote failure has occurred: " + HtmlEncoder.Default.Encode(context.Failure.Message) + "<br>");
            await response.WriteAsync("<a href=\"/\">Home</a>");
            await response.WriteAsync("</body></html>");

            //response.Redirect("/error?FailureMessage=" + UrlEncoder.Default.Encode(context.Failure.Message));

            context.HandleResponse();
        }
    }
}
