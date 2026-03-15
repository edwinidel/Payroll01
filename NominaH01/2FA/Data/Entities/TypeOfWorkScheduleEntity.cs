using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace _2FA.Data.Entities
{
    [Table("TypeOfWorkSchedules")]
    public class TypeOfWorkScheduleEntity : BaseEntity
    {
        private const string V = "El campo {0} no puede exceder los {1} caracteres";
        private const string V1 = "El campo {0} es requerido";

        [Display(Name = "Código")]
        [StringLength(100, ErrorMessage = V)]
        [Required(ErrorMessage = V1)]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        [StringLength(200, ErrorMessage = V)]
        [Required(ErrorMessage = V1)]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Activo?")]
        public bool IsActive { get; set; }
    }
}