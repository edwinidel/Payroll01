using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("EmployeeObservacions")]
    public class EmployeeObservationEntity : BaseEntity
    {
        [Display(Name = "Empledo")]
        public int EmployeeId { get; set; }

        [Display(Name = "Tipo de Observación")]
        public int ObservationTypeId { get; set; }

        [Display(Name = "Observaciones")]
        [StringLength(3000, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres")]
        public string Observations { get; set; } = string.Empty;

        public EmployeeEntity? Employee { get; set; }
        public ObservationTypeEntity? ObservationType { get; set; }
    }
}
