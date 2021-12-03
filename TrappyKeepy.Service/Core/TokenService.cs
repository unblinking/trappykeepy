using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TrappyKeepy.Domain.Interfaces;

namespace TrappyKeepy.Service
{
    /// <summary>
    /// The token service.
    /// Create JSON Web Tokens for role based authorization.
    /// </summary>
    public class TokenService : ITokenService
    {
        /// <summary>
        /// Secret cryptographic key/string.
        /// </summary>
        private string key;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TokenService()
        {
            this.key = $"{Environment.GetEnvironmentVariable("TK_CRYPTO_KEY")}";
        }

        /// <summary>
        /// Encode a JSON Web Token.
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public string Encode(Guid id, string role)
        {
            var claims = new List<Claim>()
            {
                new Claim("id", id.ToString()),
                new Claim("role", role)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var jwtTokenDescriptor = new JwtSecurityToken(
                issuer: "TrappyKeepy",
                audience: "TrappyKeepy",
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(60)),
                claims: claims,
                signingCredentials: signingCredentials
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(jwtTokenDescriptor);
            return jwt;
        }
    }
}
