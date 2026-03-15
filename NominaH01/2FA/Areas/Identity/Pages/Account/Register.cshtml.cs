// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using _2FA.Data;
using _2FA.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace _2FA.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserStore<AppUser> _userStore;
        private readonly IUserEmailStore<AppUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<AppUser> userManager,
            IUserStore<AppUser> userStore,
            SignInManager<AppUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = (IUserEmailStore<AppUser>)GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
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
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public SelectList BusinessGroups { get; set; }
        public SelectList Companies { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [StringLength(100,ErrorMessage="El campo {0} no puede exceder los {1} caracteres")]
            public string FirstName { get; set; }

            [StringLength(100, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres")]
            public string LastName { get; set; }

            [Phone]
            [Display(Name = "Teléfono")]
            [StringLength(20, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres")]
            public string PhoneNumber { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "El campo {0} no puede ser menor de {2} y mayor de {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Debe seleccionar un Grupo de Negocio.")]
            [Display(Name = "Grupo de Negocio")]
            public int BusinessGroupId { get; set; }

            [Display(Name = "Compañías")]
            public List<int> SelectedCompanies { get; set; } = new List<int>();
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            BusinessGroups = new SelectList(await _context.BusinessGroups.Where(bg => bg.IsActive).ToListAsync(), "Id", "Name");
            Companies = new SelectList(new List<CompanyEntity>(), "Id", "Name"); // Empty initially
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                if( user != null)
                {
                    user.FirstName = Input.FirstName;
                    user.LastName = Input.LastName;
                    user.BusinessGroupId = Input.BusinessGroupId;
                }

                await _userStore.SetUserNameAsync((AppUser)user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync((AppUser)user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync((AppUser)user, Input.Password);

                if (result.Succeeded && Input.SelectedCompanies.Any())
                {
                    foreach (var companyId in Input.SelectedCompanies)
                    {
                        var userCompany = new UserCompanyEntity
                        {
                            UserId = user.Id,
                            CompanyId = companyId
                        };
                        _context.UserCompanies.Add(userCompany);
                    }
                    await _context.SaveChangesAsync();
                }

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Record password last changed timestamp for new users
                    try
                    {
                        ((AppUser)user).PasswordLastChanged = DateTime.UtcNow;
                        await _userManager.UpdateAsync((AppUser)user);
                    }
                    catch
                    {
                        // non-fatal: log if user update fails later
                    }

                    var userId = await _userManager.GetUserIdAsync((AppUser)user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync((AppUser)user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync((AppUser)user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private AppUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<AppUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(AppUser)}'. " +
                    $"Ensure that '{nameof(AppUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        public async Task<IActionResult> OnGetCompaniesByBusinessGroup(int businessGroupId)
        {
            var companies = await _context.Companies
                .Where(c => c.BusinessGroupId == businessGroupId && c.IsActive)
                .Select(c => new { id = c.Id, name = c.Name })
                .ToListAsync();

            return new JsonResult(companies);
        }

        private IUserEmailStore<AppUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<AppUser>)_userStore;
        }
    }
}
