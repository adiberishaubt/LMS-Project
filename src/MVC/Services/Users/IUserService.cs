using System.Collections.Generic;
using System.Threading.Tasks;
using static Library_Managment_System.Areas.Identity.Pages.Account.LoginModel;
using static Library_Managment_System.Areas.Identity.Pages.Account.RegisterModel;

namespace Library_Managment_System.Services.Users
{
    public interface IUserService
    {
        Task<string> LoginAsync(LoginRequest request);
        Task<IEnumerable<string>> RegsiterAsync(RegisterRequest registerUser);
        Task LogoutAsync();
    }
}
