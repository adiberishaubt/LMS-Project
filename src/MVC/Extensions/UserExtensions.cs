using System.Linq;
using System.Security.Claims;

namespace Library_Managment_System.Extensions
{
    public static class UserExtensions
    {

        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
