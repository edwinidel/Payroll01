using _2FA.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace _2FA.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UserHelper(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> AddUserAsync(AppUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task AddUserToRoleAsync(AppUser user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> ChangePasswordAsync(AppUser user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            bool roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName
                });
            }
        }

        public async Task<IdentityResult> DeleteUserAsync(AppUser user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<AppUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<AppUser> GetUserByIAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public Task<AppUser> GetUserNameForIdAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> IsUserInRoleAsync(AppUser user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> UpdateUserAsync(AppUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

    }

}
