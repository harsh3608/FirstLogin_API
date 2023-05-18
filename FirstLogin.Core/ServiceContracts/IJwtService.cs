using FirstLogin.Core.DTO;
using FirstLogin.Core.Identity;
using System.Security.Claims;

namespace FirstLogin.Core.ServiceContracts
{
    public interface IJwtService
    {
        AuthenticationResponse CreateJwtToken(ApplicationUser user);
    }
}
