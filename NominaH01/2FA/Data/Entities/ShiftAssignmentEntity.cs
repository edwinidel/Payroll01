using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    public class ShiftAssignmentEntity : BaseEntity
    {
        [Display(Name = "Empleado")]
        public int EmployeeId { get; set; }

        [Display(Name = "Turno")]
        public int ShiftId { get; set; }

        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Horario")]
        public int ScheduleId { get; set; }

        public EmployeeEntity? Employee { get; set; }
        public ShiftEntity? Shift { get; set; }
        public ScheduleEntity? Schedule { get; set; }
    }
}