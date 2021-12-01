using System.Security.Claims;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface ITokenService
    {
        string EncodeJwt(List<Claim> claims);
    }
}
