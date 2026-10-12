using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Middlewares
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IConfiguration _configuration;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IConfiguration configuration
        ) : base(options, logger, encoder)
        {
            _configuration = configuration;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }
            else
            {
                try
                {
                    var authorizationHeader = AuthenticationHeaderValue.Parse(Request.Headers.Authorization!);

                    if (!"Basic".Equals(authorizationHeader.Scheme, StringComparison.OrdinalIgnoreCase))
                        return AuthenticateResult.Fail("Not a Basic Authentication header");

                    var credentialBytes = Convert.FromBase64String(authorizationHeader.Parameter ?? string.Empty);
                    var credentials = Encoding.UTF8.GetString(credentialBytes).Split(":", 2);
                    string username = credentials[0];
                    string password = credentials[1];

                    if (username == _configuration["ApiCredentials:Username"] && password == _configuration["ApiCredentials:Password"])
                    {
                        var claims = new Claim[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, username),
                            new Claim(ClaimTypes.Name, username),
                            new Claim(ClaimTypes.Role, "Admin")
                        };

                        var identity = new ClaimsIdentity(claims, Scheme.Name);
                        var principal = new ClaimsPrincipal(identity);
                        var ticket = new AuthenticationTicket(principal, Scheme.Name);

                        return AuthenticateResult.Success(ticket);
                    }

                    return AuthenticateResult.Fail("Invalid Username or Password");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error when authenticating basic authentication due to:", ex.Message);
                    return AuthenticateResult.Fail("Invalid Authorization Header Format");
                }
            }
        }
    }
}