// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using _2FA.Data;
using _2FA.Data.Entities;

namespace _2FA.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public LoginModel(SignInManager<AppUser> signInManager, ILogger<LoginModel> logger, ApplicationDbContext context, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // Attempt to sign in using the user's username when an account exists; enable lockout on failure
                var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);
                var userNameForSignIn = user?.UserName ?? Input.Email;
                var result = await _signInManager.PasswordSignInAsync(userNameForSignIn, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    // Ensure we have the AppUser instance
                    user ??= await _signInManager.UserManager.FindByNameAsync(userNameForSignIn);
                    // Check password expiry
                    var expiryDays = _configuration.GetValue<int?>("Security:PasswordExpirationDays") ?? 90;
                    if (user != null)
                    {
                        var lastChanged = user.PasswordLastChanged;
                        var expired = lastChanged == null || lastChanged.Value.AddDays(expiryDays) <= DateTime.UtcNow;
                        if (expired)
                        {
                            if (user.RequirePasswordChangeOnExpiry)
                            {
                                // Force change password
                                return RedirectToPage("/Account/Manage/ChangePassword", new { area = "Identity" });
                            }
                            else
                            {
                                // Prompt user to keep or change password
                                return RedirectToPage("PasswordExpiry", new { returnUrl });
                            }
                        }
                    }
                    var userCompanies = await _context.UserCompanies
                        .Where(uc => uc.UserId == user.Id)
                        .Include(uc => uc.Company)
                        .ToListAsync();

                    // Check if user is administrator
                    var isAdmin = await _signInManager.UserManager.IsInRoleAsync(user, "Administrator");

                    if (userCompanies.Count == 0)
                    {
                        // No companies assigned
                        ModelState.AddModelError(string.Empty, "No tiene compañías asignadas.");
                        return Page();
                    }
                    else if (userCompanies.Count == 1 && !isAdmin)
                    {
                        // Auto-select for regular users with single company
                        HttpContext.Session.SetInt32("SelectedCompanyId", userCompanies.First().CompanyId);
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        // Redirect to company selection for admins or users with multiple companies
                        return RedirectToPage("CompanySelection", new { returnUrl });
                    }
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
