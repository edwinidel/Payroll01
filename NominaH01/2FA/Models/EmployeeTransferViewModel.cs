using System.ComponentModel.DataAnnotations;
using _2FA.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _2FA.Models
{
    public class EmployeeTransferViewModel
    {
        [Required]
        [Display(Name = "Empleados a Transferir")]
        public List<int> SelectedEmployeeIds { get; set; } = new();

        [Required]
        [Display(Name = "Compañía Origen")]
        public int SourceCompanyId { get; set; }

        [Required]
        [Display(Name = "Compañía Destino")]
        public int TargetCompanyId { get; set; }

        [Display(Name = "Comentarios")]
        [StringLength(500, ErrorMessage = "Los comentarios no pueden exceder los 500 caracteres.")]
        public string? Comments { get; set; }

        // Data for dropdowns
        public List<SelectListItem> AvailableEmployees { get; set; } = new();
        public List<SelectListItem> SourceCompanies { get; set; } = new();
        public List<SelectListItem> TargetCompanies { get; set; } = new();

        // Employee details for confirmation
        public List<EmployeeSummaryViewModel> Employees { get; set; } = new();
        
        public string SourceCompanyName { get; set; } = string.Empty;
        public string TargetCompanyName { get; set; } = string.Empty;
        public string BusinessGroupName { get; set; } = string.Empty;
    }

    public class EmployeeSummaryViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string CurrentPosition { get; set; } = string.Empty;
        public string CurrentDepartment { get; set; } = string.Empty;
        public DateTime HiringDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}