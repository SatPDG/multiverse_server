using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MultiverseServer.Security.Token
{
    public class JwtTokenService
    {
        private readonly string secret;
        private readonly string expDate;

        public JwtTokenService(IConfiguration config)
        {
            secret = config.GetSection("JwtConfig").GetSection("secret").Value;
            expDate = config.GetSection("JwtConfig").GetSection("expirationInSeconds").Value;
        }

        public string GenerateToken(int userID, string username, string tokenLevel)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    //new Claim(ClaimTypes.UserData, userID),
                    new Claim("username", username),
                    new Claim("userID", userID.ToString()),
                    new Claim("level", tokenLevel)
                }),
                Expires = DateTime.UtcNow.AddSeconds(double.Parse(expDate)),
                NotBefore = DateTime.Now,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            using(var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        public bool ValidateJwtTokenWithoutLifetime(string token)
        {
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters()
            {
                ValidateLifetime = false, // Because there is no expiration in the generated token
                ValidateAudience = false, // Because there is no audiance in the generated token
                ValidateIssuer = false,   // Because there is no issuer in the generated token
                ValidIssuer = "Sample",
                ValidAudience = "Sample",
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            try
            {
                SecurityToken validatedToken;
                tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public string GetJwtClaim(string token, string claim)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            string claimValue = securityToken.Claims.First(c => c.Type == claim).Value;
            return claimValue;
        }
    }
}
