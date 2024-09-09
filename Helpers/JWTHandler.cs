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

    public class JWTPayload
    {
        public string UserID { get; set; }
    }
    public class JWTHandler
    {
        private static Claim[] generateClaim(JWTPayload payload)
        {
            return [
                new Claim("userID", payload.UserID)
            ];
        }

        public static IJWTResponse generateToken(JWTPayload payload, string secretKey)
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

        public static string parseToken(string secretKey, string token)
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
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return principal.ToString();
        }
    }
}
