using _2FA.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace _2FA.Helpers
{
    public interface IUserHelper
    {
        Task<IdentityResult> AddUserAsync(AppUser user, string password);
        Task AddUserToRoleAsync(AppUser user, string roleName);
        Task<IdentityResult> ChangePasswordAsync(AppUser user, string oldPassword, string newPassword);
        Task CheckRoleAsync(string roleName);
        Task<IdentityResult> DeleteUserAsync(AppUser user);
        Task<AppUser> GetUserByEmailAsync(string email);
        Task<AppUser> GetUserByIAsync(string id);
        Task<bool> IsUserInRoleAsync(AppUser user, string roleName);
        Task<IdentityResult> UpdateUserAsync(AppUser user);
        Task<AppUser> GetUserNameForIdAsync(string id);
        Task LogoutAsync();
    }
}
