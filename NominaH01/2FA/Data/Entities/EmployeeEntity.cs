
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    public class EmployeeEntity : BaseEntity
    {
        private const string StrLength = "El Campo {0} no puede exceder los {1} caracteres.";

        [Display(Name = "Código")]
        [StringLength(10, ErrorMessage = StrLength)]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Horario")]
        public int ScheduleId { get; set; }

        [Display(Name = "Id. de Reloj")]
        [StringLength(20, ErrorMessage = StrLength)]
        public string CodOfClock { get; set; } = string.Empty;

        [Display(Name = "Nombre")]
        [StringLength(20, ErrorMessage = StrLength)]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string FirstName { get; set; } = String.Empty;

        [Display(Name = "Apellido")]
        [StringLength(20, ErrorMessage = StrLength)]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Compañía")]
        public int CompanyId { get; set; }

        [Display(Name = "Sucursal")]
        public int BranchId { get; set; }

        [Display(Name = "Departamento")]
        public int DepartmentId { get; set; }

        [Display(Name = "Centro de Costo")]
        public int CostCenterId { get; set; }

        [Display(Name = "Sección")]
        public int SectionId { get; set; }

        [Display(Name = "División")]
        public int DivisionId { get; set; }

        [Display(Name = "Proyecto")]
        public int ProjectId { get; set; }

        [Display(Name = "Fase")]
        public int PhaseId { get; set; }

        [Display(Name = "Actividad")]
        public int? ActivityId { get; set; }

        [Display(Name = "Cargo")]
        public int PositionId { get; set; }

        [Display(Name = "Fecha de Nacimiento")]
        public DateTime? DateOfBird { get; set; }

        [Display(Name = "Cédula")]
        [StringLength(20, ErrorMessage = StrLength)]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string IdDocument { get; set; } = string.Empty;

        [Display(Name = "DV")]
        [StringLength(2, ErrorMessage = StrLength)]
        public string Dv { get; set; } = string.Empty;

        [Display(Name = "Est. Civil")]
        [StringLength(20, ErrorMessage = StrLength)]
        public string CivilStatus { get; set; } = string.Empty;

        [Display(Name = "Género")]
        [StringLength(20, ErrorMessage = StrLength)]
        public string Genere { get; set; } = string.Empty;

        [Display(Name = "Seguro Social")]
        [StringLength(20, ErrorMessage = StrLength)]
        public string SocSecNum { get; set; } = string.Empty;

        [Display(Name = "Grupo ISR")]
        [StringLength(3, ErrorMessage = StrLength)]
        public string IsrGroup { get; set; } = "A0";

        [Display(Name = "Tipo de Emp.")]
        public int EmployeeTypeId { get; set; }

        [Display(Name = "País de Origen")]
        public int OriginCountryId { get; set; }

        [Display(Name = "Tipo de Sangre")]
        [StringLength(3, ErrorMessage = StrLength)]
        public string BloodType { get; set; } = "A+";

        [Display(Name = "Dirección")]
        [StringLength(300, ErrorMessage = StrLength)]
        public string? CDIR1 { get; set; } = string.Empty;

        [Display(Name = "Dirección")]
        [StringLength(300, ErrorMessage = StrLength)]
        public string? CDIR2 { get; set; } = string.Empty;

        [Display(Name = "Correo Electrónico")]
        [StringLength(100, ErrorMessage = StrLength)]
        [DataType(DataType.EmailAddress)]
        public string CEMAIL { get; set; } = string.Empty;

        [Display(Name = "Teléfono Fijo")]
        [StringLength(20, ErrorMessage = StrLength)]
        public string? FixedPhone { get; set; } = string.Empty;

        [Display(Name = "Celular")]
        [StringLength(20, ErrorMessage = StrLength)]
        public string? CellPhone { get; set; } = string.Empty;

        [Display(Name = "Estatus")]
        [StringLength(20, ErrorMessage = StrLength)]
        public string Status { get; set; } = string.Empty;

        [Display(Name = "Tipo de Salario")]
        [StringLength(10, ErrorMessage = StrLength)]
        public string SalaryType { get; set; } = string.Empty;

        [Display(Name = "Horas Reg.")]
        public decimal RegularHours { get; set; }

        [Display(Name = "Salario Pactado")]
        [DataType(DataType.Currency)]
        public decimal AgreeSalary { get; set; }

        [Display(Name = "Salario por Hora")]
        [DataType(DataType.Currency)]
        public decimal HourSalary { get; set; }

        [Display(Name = "Valor por Unidad")]
        [DataType(DataType.Currency)]
        public decimal UnitValue { get; set; }

        [Display(Name = "Tipo de Unidad")]
        public int? PieceworkUnitTypeId { get; set; }

        [Display(Name = "Tipo de Unidad")]
        [StringLength(50, ErrorMessage = StrLength)]
        public string UnitType { get; set; } = string.Empty;

        [Display(Name = "Retener ISR")]
        public bool RetainISR { get; set; }

        [Display(Name = "Método de ISR")]
        [StringLength(1, ErrorMessage = StrLength)]
        public string IsrMethod { get; set; } = string.Empty;

        [Display(Name = "ISR Fijo")]
        [DataType(DataType.Currency)]
        public decimal IsrFixed { get; set; }

        [Display(Name = "ISR Adicional")]
        [DataType(DataType.Currency)]
        public decimal AdditionalIsr { get; set; }

        [Display(Name = "¿Declara ISR?")]
        public bool DeclareIsr { get; set; }

        [Display(Name = "% de Descuento")]
        public decimal DiscountPercentage { get; set; }

        [Display(Name = "Pagos Fijos")]
        public List<FixedPaymentEntity> FixedPayments { get; set; } = new List<FixedPaymentEntity>();

        [Display(Name = "¿Sindicalizado?")]
        public bool Unionized { get; set; }

        [Display(Name = "Tipo de Trabajador")]
        public int TypeOfWorkerId { get; set; }

        [Display(Name = "Tipo de Cuenta")]
        [StringLength(20, ErrorMessage = StrLength)]
        public string PayAccountType { get; set; } = string.Empty;

        [Display(Name = "Num. Cuenta")]
        [StringLength(50, ErrorMessage = StrLength)]
        public string PayAccount { get; set; } = string.Empty;

        [Display(Name = "Banco")]
        public int BankId { get; set; }

        [Display(Name = "Forma de Pago")]
        [StringLength(20, ErrorMessage = StrLength)]
        public string PaymentMethod { get; set; } = string.Empty;

        [Display(Name = "Banco Pagador")]
        public int? PayingBankId { get; set; }

        [Display(Name = "Usuario API OTC")]
        [StringLength(100, ErrorMessage = StrLength)]
        public string? ApiOtcUsername { get; set; } = string.Empty;

        [Display(Name = "Password API OTC")]
        [StringLength(200, ErrorMessage = StrLength)]
        public string? ApiOtcPassword { get; set; } = string.Empty;

        [Display(Name = "Tipo de Doc. de Identidad")]
        public int IdentityDocumentTypeId { get; set; }

        [Display(Name = "Fecha de Ingreso")]
        [DataType(DataType.Date)]
        public DateTime HiringDate { get; set; }

        [Display(Name = "Fecha de Cesantía")]
        [DataType(DataType.Date)]
        public DateTime LiquidationDate { get; set; }

        [Display(Name = "Fin de Contrato")]
        [DataType(DataType.Date)]
        public DateTime EndOfContract { get; set; }

        [Display(Name = "Ultimo Aumento")]
        [DataType(DataType.Date)]
        public DateTime LastSalaryIncrease { get; set; }

        [Display(Name = "Observaciones")]
        public int EmployeeObservationId { get; set; }

        [Display(Name = "Grupo de Pago")]
        public int PaymentGroupId { get; set; }

        public PieceworkUnitTypeEntity? PieceworkUnitType { get; set; }

        [Display(Name = "Foto")]
        [StringLength(200, ErrorMessage = StrLength)]
        public string? PhotoPath { get; set; } = string.Empty;

        [Display(Name = "Servicios Profesionales")]
        public bool IsContractor { get; set; }

        [NotMapped]
        public string PhotoImage { get; set; } = string.Empty;

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        [NotMapped]
        public string FullNameWithCode => $"{FirstName} {LastName} ({Code})";

        public ActivityEntity? Activity { get; set; }
        
        [ForeignKey("BankId")]
        public BankEntity? Bank { get; set; }
        
        [ForeignKey("PayingBankId")]
        public BankEntity? PayingBank { get; set; }
        
        public BranchEntity? Branch { get; set; }
        public DepartmentEntity? Department { get; set; }
        public DivisionEntity? Division { get; set; }
        public SectionEntity? Section { get; set; }
        public ProjectEntity? Project { get; set; }
        public PhaseEntity? Phase { get; set; }
        public CostCenterEntity? CostCenter { get; set; }
        public CompanyEntity? Company { get; set; }
        public ScheduleEntity? Schedule { get; set; }
        public PositionEntity? Position { get; set; }
        public EmployeeTypeEntity? EmployeeType { get; set; }
        public TypeOfWorkerEntity? TypeOfWorker { get; set; }
        public PaymentGroupEntity? PaymentGroup { get; set; }
        public IdentityDocumentTypeEntity? IdentityDocumentType { get; set; }
        public EmployeeObservationEntity? EmployeeObservation { get; set; }
        
        public CountryEntity? OriginCountry { get; set; }

        public ICollection<ShiftAssignmentEntity>? ShiftAssignments { get; set; }

    }
}
