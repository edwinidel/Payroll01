using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("CompanyPayrollVouchers")]
    public class CompanyPayrollVoucherEntity : BaseEntity
    {
        [Display(Name = "Compañía")]
        public int CompanyId { get; set; }

        [Display(Name = "Tipo de Planilla")]
        public int PayrollTypeId { get; set; }

        [Display(Name = "Formato de Voucher")]
        public int PayrollVoucherFormatId { get; set; }

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; }

        public CompanyEntity? Company { get; set; }
        public PayrollTypeEntity? PayrollType { get; set; }
        public PayrollVoucherFormatEntity? PayrollVoucherFormat { get; set; }
    }
}