using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Middlewares
{
    public class KeycloakRoleTransformer : IClaimsTransformation
    {
        private readonly IConfiguration _configuration;

        public KeycloakRoleTransformer(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = principal.Identity as ClaimsIdentity;
            if (identity == null) return Task.FromResult(principal);

            var resourcesAccessClaim = principal.FindFirst("resource_access");
            if (resourcesAccessClaim != null)
            {
                Console.WriteLine($"Found resource_access claim: {resourcesAccessClaim.Value}");
                Console.WriteLine($"Client ID: {_configuration["Keycloak:ClientId"]}");
                // var realmAccess = JsonDocument.Parse(resourcesAccessClaim.Value);
                // if (realmAccess.RootElement.TryGetProperty($"{clientId}:roles", out var roles))
                // {
                //     Console.WriteLine($"Found roles in resource_access: {roles}");
                //     foreach (var role in roles.EnumerateArray())
                //     {
                //         var roleString = role.GetString();
                //         Console.WriteLine($"Adding role: {roleString}");
                //         identity.AddClaim(new Claim(ClaimTypes.Role, roleString!));
                //     }
                // }
                // else
                // {
                //     Console.WriteLine("No roles property in resource_access");
                // }
                var resource_access = JsonDocument.Parse(resourcesAccessClaim.Value);
                if (resource_access.RootElement.TryGetProperty(_configuration["Keycloak:ClientId"]!, out var clientElement))
                {
                    if (clientElement.TryGetProperty("roles", out var roles))
                    {
                        Console.WriteLine($"Roles found under the client ID {_configuration["Keycloak:ClientId"]}: {roles}");
                        foreach (var role in roles.EnumerateArray())
                        {
                            string roleValue = role.GetString()!;
                            Console.WriteLine($"Adding role: {roleValue}");
                            identity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
                        }
                    } else
                    {
                        Console.WriteLine("No roles property in resource_access");
                    }
                } else
                {
                    Console.WriteLine($"Client ID '{_configuration["Keycloak:ClientId"]}' not found inside resource_access");
                }
            }
            else
            {
                Console.WriteLine("No resource_access claim found");
            }

            return Task.FromResult(principal);
        }
    }
}