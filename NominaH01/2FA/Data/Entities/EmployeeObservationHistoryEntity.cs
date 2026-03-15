using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("EmployeeObservationHistories")]
    public class EmployeeObservationHistoryEntity : BaseEntity
    {
        [Display(Name = "Compañía")]
        public int CompanyId { get; set; }

        [Display(Name = "Empleado")]
        public int EmployeeId { get; set; }

        [Display(Name = "Tipo de Observación")]
        public int ObservationTypeId { get; set; }

        [Display(Name = "Observaciones")]
        [StringLength(3000, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres")]
        public string Observations { get; set; } = string.Empty;

        [Display(Name = "Creado por (nombre de usuario)")]
        [StringLength(200)]
        public string? CreatedByUserName { get; set; }

        public CompanyEntity? Company { get; set; }
        public EmployeeEntity? Employee { get; set; }
        public ObservationTypeEntity? ObservationType { get; set; }
    }
}
