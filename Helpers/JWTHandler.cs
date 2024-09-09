using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace locket.Helpers
{
    public class IJWTResponse
    {
        public string Token { get; set; }
        public string? Refresh { get; set; }
    }

    public class IJWTPayload
    {
        public string UserID { get; set; }
        public string Username { get; set; }
    }

    public class JWTClaimType
    {
        public static readonly string Username = "username";
        public static readonly string UserID = "userID";
    }
    
    public class JWTHandler
    {


        private static Claim[] generateClaim(IJWTPayload payload)
        {
            return [
                new Claim(JWTClaimType.UserID, payload.UserID),
                new Claim(JWTClaimType.Username, payload.Username)
            ];
        }

        public static IJWTResponse generateToken(IJWTPayload payload, string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddMinutes(30),
                claims: generateClaim(payload),
                signingCredentials: credentials);

            var jwt = tokenHandler.WriteToken(token);

            return new IJWTResponse
            {
                Token = jwt
            };
        }

        public static IJWTPayload parseToken(string secretKey, string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
            };
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);

            IJWTPayload response = new()
            {
                UserID = principal.FindFirstValue(JWTClaimType.UserID) ?? String.Empty,
                Username = principal.FindFirstValue(JWTClaimType.Username) ?? String.Empty,
            };
            return response;

        }
    }
}
