using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Middlewares
{
    public class KeycloakRoleTransformer : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = principal.Identity as ClaimsIdentity;
            if (identity == null) return Task.FromResult(principal);

            var realmAccessClaim = principal.FindFirst("realm_access");
            if (realmAccessClaim != null)
            {
                Console.WriteLine($"Found realm_access claim: {realmAccessClaim.Value}");
                var realmAccess = JsonDocument.Parse(realmAccessClaim.Value);
                if (realmAccess.RootElement.TryGetProperty("roles", out var roles))
                {
                    Console.WriteLine($"Found roles in realm_access: {roles}");
                    foreach (var role in roles.EnumerateArray())
                    {
                        var roleString = role.GetString();
                        Console.WriteLine($"Adding role: {roleString}");
                        identity.AddClaim(new Claim(ClaimTypes.Role, roleString!));
                    }
                }
                else
                {
                    Console.WriteLine("No roles property in realm_access");
                }
            }
            else
            {
                Console.WriteLine("No realm_access claim found");
            }

            return Task.FromResult(principal);
        }
    }
}