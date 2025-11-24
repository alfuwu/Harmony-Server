using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Server.Models;

namespace Server.Helpers;
public static class JwtTokenHelper {
    public static string GenerateToken(User user, IConfiguration configuration) {
        var jwtSection = configuration.GetSection("Jwt");
        var key = jwtSection.GetValue<string>("Key");
        var issuer = jwtSection.GetValue<string>("Issuer");
        var audience = jwtSection.GetValue<string>("Audience");
        var expiryMin = jwtSection.GetValue<int>("ExpiryMinutes");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username!),
            new Claim("uid", user.Id.ToString())
        };

        var token = new JwtSecurityToken(issuer, audience, claims,
        expires: DateTime.UtcNow.AddMinutes(expiryMin), signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}