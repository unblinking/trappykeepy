using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TrappyKeepy.Domain.Interfaces;

namespace TrappyKeepy.Service
{
    public class TokenService : ITokenService
    {
        private string key;

        public TokenService()
        {
            this.key = $"{Environment.GetEnvironmentVariable("TK_CRYPTO_KEY")}";
        }

        public string EncodeJwt(List<Claim> claims)
        {
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
