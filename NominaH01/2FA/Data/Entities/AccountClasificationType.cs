using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    public enum AccountClasificationType
    {
        [Display(Name = "Activo")]
        Activo = 1,

        [Display(Name = "Pasivo")]
        Pasivo = 2,

        [Display(Name = "Patrimonio")]
        Patrimonio = 3,

        [Display(Name = "Ingreso")]
        Ingreso = 4,

        [Display(Name = "Gasto")]
        Gasto = 5,

        [Display(Name = "Costo")]
        Costo = 6
    }
}