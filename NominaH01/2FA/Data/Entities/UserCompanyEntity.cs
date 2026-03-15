using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    public class UserCompanyEntity : BaseEntity
    {
        [Display(Name = "Usuario")]
        public string UserId { get; set; } = string.Empty;

        [Display(Name = "Compañía")]
        public int CompanyId { get; set; }

        public AppUser? User { get; set; }
        public CompanyEntity? Company { get; set; }
    }
}