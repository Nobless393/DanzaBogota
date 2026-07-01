using DanzaBogota.Domain.Entity.Identities;

namespace DanzaBogota.Transversal.Common.Jwt
{
    public interface IJwtService
    {
        string GenerateToken(User user, IList<string> roles);
    }
}
