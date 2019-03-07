using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Digillect.AspNetCore.Authentication.Odnoklassniki;
using Digillect.AspNetCore.Authentication.VKontakte;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OAuthClientSample
{
    /* Note all servers must use the same address and port because these are pre-registered with the various providers. */
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var auth = services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);

            auth.AddCookie(o => o.LoginPath = new PathString("/login"));

            // You must first create an app with Odnoklassniki and add its ID, Key and Secret to your user-secrets.
            // https://apiok.ru/en/dev/app/create
            auth.AddOAuth("Odnoklassniki-AccessToken", "Odnoklassniki AccessToken only", o =>
            {
                o.ClaimsIssuer = OdnoklassnikiDefaults.Issuer;
                o.CallbackPath = new PathString("/signin-odnoklassniki-token");
                o.AuthorizationEndpoint = OdnoklassnikiDefaults.AuthorizationEndpoint;
                o.TokenEndpoint = OdnoklassnikiDefaults.TokenEndpoint;
                //o.Scope.Add("VALUABLE_ACCESS,GET_EMAIL");
                o.ClientId = Configuration["Odnoklassniki:ClientId"];
                o.ClientSecret = Configuration["Odnoklassniki:ClientSecret"];
                o.SaveTokens = true;
                o.Events = new OAuthEvents
                {
                    OnRemoteFailure = HandleOnRemoteFailure
                };
            });

            // You must first create an app with Odnoklassniki and add its ID, Key and Secret to your user-secrets.
            // https://apiok.ru/en/dev/app/create
            auth.AddOdnoklassniki(o =>
            {
                //o.UserInformationEndpoint = "https://api.ok.ru/fb.do?method=users.getCurrentUser";
                o.ClientId = Configuration["Odnoklassniki:ClientId"];
                o.ApplicationKey = Configuration["Odnoklassniki:ApplicationKey"];
                o.ClientSecret = Configuration["Odnoklassniki:ClientSecret"];
                o.SaveTokens = true;
                o.Events = new OAuthEvents
                {
                    OnRemoteFailure = HandleOnRemoteFailure
                };
            });

            // You must first create an app with VKontakte and add its ID and Secret to your user-secrets.
            // https://vk.com/apps?act=manage
            auth.AddOAuth("VKontakte-AccessToken", "VKontakte AccessToken only", o =>
            {
                o.ClaimsIssuer = VKontakteDefaults.Issuer;
                o.CallbackPath = new PathString("/signin-vkontakte-token");
                o.AuthorizationEndpoint = VKontakteDefaults.AuthorizationEndpoint;
                o.TokenEndpoint = VKontakteDefaults.TokenEndpoint;
                o.Scope.Add("email");
                o.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "user_id");
                o.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                o.ClientId = Configuration["VKontakte:ClientId"];
                o.ClientSecret = Configuration["VKontakte:ClientSecret"];
                o.SaveTokens = true;
                o.Events = new OAuthEvents
                {
                    OnCreatingTicket = context =>
                    {
                        context.RunClaimActions(context.TokenResponse.Response.RootElement);
                        return Task.CompletedTask;
                    },
                    OnRemoteFailure = HandleOnRemoteFailure
                };
            });

            // You must first create an app with VKontakte and add its ID and Secret to your user-secrets.
            // https://vk.com/apps?act=manage
            auth.AddVKontakte(o =>
            {
                o.ClientId = Configuration["VKontakte:ClientId"];
                o.ClientSecret = Configuration["VKontakte:ClientSecret"];
                o.SaveTokens = true;
                o.Events = new OAuthEvents
                {
                    OnRemoteFailure = HandleOnRemoteFailure
                };
            });

            auth.AddOAuth("GitLab-AccessToken", "GitLab AccessToken only", o =>
            {
                o.CallbackPath = new PathString("/signin-gitlab-token");
                o.AuthorizationEndpoint = "https://gitlab.com/oauth/authorize";
                o.TokenEndpoint = "https://gitlab.com/oauth/token";
                o.Scope.Add("openid");
                o.ClientId = Configuration["GitLab:ClientId"];
                o.ClientSecret = Configuration["GitLab:ClientSecret"];
                o.SaveTokens = true;
                o.Events = new OAuthEvents
                {
                    OnRemoteFailure = HandleOnRemoteFailure
                };
            });

            auth.AddOAuth("GitLab", "GitLab", o =>
            {
                o.ClaimsIssuer = "GitLab";
                o.CallbackPath = new PathString("/signin-gitlab");
                o.AuthorizationEndpoint = "https://gitlab.com/oauth/authorize";
                o.TokenEndpoint = "https://gitlab.com/oauth/token";
                o.UserInformationEndpoint = "https://gitlab.com/api/v4/user";
                o.Scope.Add("openid");
                o.Scope.Add("api");
                o.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                o.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                o.ClaimActions.MapJsonKey(ClaimTypes.Email, "email", ClaimValueTypes.Email);
                o.ClaimActions.MapJsonKey(ClaimTypes.Locality, "location");
                o.ClaimActions.MapJsonKey(ClaimTypes.Webpage, "website");
                o.ClaimActions.MapJsonKey("urn:gitlab:avatar_url", "avatar_url");
                o.ClientId = Configuration["GitLab:ClientId"];
                o.ClientSecret = Configuration["GitLab:ClientSecret"];
                o.SaveTokens = true;
                o.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                        var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();

                        using (var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync()))
                        {
                            context.RunClaimActions(user.RootElement);
                        }
                    },
                    OnRemoteFailure = HandleOnRemoteFailure
                };
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseAuthentication();

            // A guard against tricky browsers
            app.Map("/favicon.ico", notFoundApp =>
            {
                notFoundApp.Run(context =>
                {
                    context.Response.StatusCode = 404;
                    return Task.CompletedTask;
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
                        await context.ChallengeAsync(authType, new AuthenticationProperties { RedirectUri = "/" });
                        return;
                    }

                    var response = context.Response;
                    response.ContentType = "text/html; charset=utf-8";
                    await response.WriteAsync("<html><body>");
                    await response.WriteAsync("Choose an authentication scheme:<ul>");
                    var schemeProvider = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
                    foreach (var provider in await schemeProvider.GetAllSchemesAsync())
                    {
                        await response.WriteAsync("<li><a href=\"?authscheme=" + UrlEncoder.Default.Encode(provider.Name) + "\">" + (provider.DisplayName ?? "(suppressed)") + "</a></li>");
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
                    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
                // var user = await context.AuthenticateAsync(AuthenticationManager.AutomaticScheme);

                // Deny anonymous request beyond this point.
                if (user == null || !user.Identities.Any(identity => identity.IsAuthenticated))
                {
                    // This is what [Authorize] calls
                    // The cookie middleware will handle this and redirect to /login
                    await context.ChallengeAsync();

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
                    await response.WriteAsync("<dt>" + token + "</dt><dd>" + HtmlEncoder.Default.Encode(await context.GetTokenAsync(token) ?? "") + "</dd>");
                }
                await response.WriteAsync("</ul><a href=\"/logout\">Logout</a><br>");
                await response.WriteAsync("</body></html>");
            });
        }

        private static async Task HandleOnRemoteFailure(RemoteFailureContext context)
        {
            var response = context.Response;
            response.StatusCode = 500;
            response.ContentType = "text/html; charset=utf-8";
            await response.WriteAsync("<html><body>");
            await response.WriteAsync("<h1>A remote failure has occurred</h1>");
            await response.WriteAsync(HtmlEncoder.Default.Encode(context.Failure.Source ?? "Unknown") + ": " + HtmlEncoder.Default.Encode(context.Failure.Message) + "<br>");
            await response.WriteAsync("<a href=\"/\">Home</a>");
            await response.WriteAsync("</body></html>");

            //response.Redirect("/error?FailureMessage=" + UrlEncoder.Default.Encode(context.Failure.Message));

            context.HandleResponse();
        }
    }
}
