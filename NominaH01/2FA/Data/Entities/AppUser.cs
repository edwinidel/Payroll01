using Microsoft.AspNetCore.Identity;

namespace _2FA.Data.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public int BusinessGroupId { get; set; }
        public BusinessGroupEntity? BusinessGroup { get; set; }

        public ICollection<UserCompanyEntity>? UserCompanies { get; set; }

        // Vinculación opcional al empleado para portal de autoservicio
        public int? EmployeeId { get; set; }

        // Password expiry tracking
        public DateTime? PasswordLastChanged { get; set; }

        // When true, admin requires user to change password on expiry
        public bool RequirePasswordChangeOnExpiry { get; set; } = false;
    }
}
