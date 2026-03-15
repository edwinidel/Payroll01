using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using _2FA.Data.Entities;
using System.Linq;

namespace _2FA.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminUsersController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminUsersController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.RequirePasswordChangeOnExpiry,
                    u.PasswordLastChanged
                })
                .ToList();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleRequireChange(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.RequirePasswordChangeOnExpiry = !user.RequirePasswordChangeOnExpiry;
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }
    }
}
