using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    public class UniformDeliveryEntity : BaseEntity
    {
        private const string StrLength = "El campo {0} no puede exceder los {1} caracteres.";

        [Display(Name = "Compañía")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public int CompanyId { get; set; }

        [Display(Name = "Empleado")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Prenda/Artículo")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [StringLength(100, ErrorMessage = StrLength)]
        public string ItemName { get; set; } = string.Empty;

        [Display(Name = "Marca")]
        [StringLength(100, ErrorMessage = StrLength)]
        public string Brand { get; set; } = string.Empty;

        [Display(Name = "Talla")]
        [StringLength(20, ErrorMessage = StrLength)]
        public string Size { get; set; } = string.Empty;

        [Display(Name = "Color")]
        [StringLength(50, ErrorMessage = StrLength)]
        public string Color { get; set; } = string.Empty;

        [Display(Name = "Cantidad")]
        [Range(1, 500, ErrorMessage = "La {0} debe estar entre {1} y {2}.")]
        public int Quantity { get; set; } = 1;

        [Display(Name = "Fecha de Entrega")]
        [DataType(DataType.Date)]
        public DateTime DeliveryDate { get; set; } = DateTime.UtcNow.Date;

        [Display(Name = "Vigencia (días)")]
        [Range(1, 1825, ErrorMessage = "La {0} debe estar entre {1} y {2} días.")]
        public int ValidityDays { get; set; } = 365;

        [Display(Name = "Vence el")]
        [DataType(DataType.Date)]
        public DateTime ExpirationDate { get; set; }

        [Display(Name = "Avisar días antes")]
        [Range(0, 180, ErrorMessage = "El valor debe estar entre {1} y {2} días.")]
        public int AlertDaysBefore { get; set; } = 30;

        [Display(Name = "Notas")]
        [StringLength(500, ErrorMessage = StrLength)]
        public string Notes { get; set; } = string.Empty;

        public CompanyEntity? Company { get; set; }
        public EmployeeEntity? Employee { get; set; }

        [NotMapped]
        public int DaysToExpire => (ExpirationDate.Date - DateTime.UtcNow.Date).Days;

        [NotMapped]
        public bool IsExpired => !IsDeleted && ExpirationDate.Date < DateTime.UtcNow.Date;

        [NotMapped]
        public bool IsExpiringSoon => !IsExpired && ExpirationDate.Date <= DateTime.UtcNow.Date.AddDays(AlertDaysBefore);

        [NotMapped]
        public string Status => IsExpired ? "Vencido" : IsExpiringSoon ? "Próximo a vencer" : "Vigente";
    }
}
