using Library_Managment_System.Models.Entities;
using Library_Managment_System.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using static Library_Managment_System.Areas.Identity.Pages.Account.LoginModel;
using static Library_Managment_System.Areas.Identity.Pages.Account.RegisterModel;

namespace Library_Managment_System.Services.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public UserService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<string> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user == null)
                {
                    return "Account doesn't exists!";
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, request.RememberMe, false);

                if (!result.Succeeded)
                {
                    return "Wrong password!";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "Something went wrong!";
            }

            return null;
        }
        public async Task<IEnumerable<string>> RegsiterAsync(RegisterRequest registerUser)
        {
            var user = new User
            {
                Email = registerUser.Email,
                UserName = registerUser.Username
            };

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var result = await _userManager.CreateAsync(user, registerUser.Password);

                    if (!result.Succeeded)
                    {
                        return result.Errors.Select(x => x.Description);
                    }

                    await _userManager.AddToRoleAsync(user, Roles.User.ToString());
                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                return new List<string>
                {
                    "Something went wrong!"
                };
            }

            await _signInManager.PasswordSignInAsync(user.UserName, registerUser.Password, false, false);
            return new List<string>();
        }

        public Task LogoutAsync()
        {
            return _signInManager.SignOutAsync();
        }

    }
}
