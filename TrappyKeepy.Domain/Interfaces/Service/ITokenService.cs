using System.Security.Claims;

namespace TrappyKeepy.Domain.Interfaces
{
    public interface ITokenService
    {
        string Encode(Guid id, string role);
    }
}
