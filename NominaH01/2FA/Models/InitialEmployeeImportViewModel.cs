using System.Collections.Generic;
using System.Linq;

namespace _2FA.Models
{
    public class InitialEmployeeImportViewModel
    {
        public string CompanyName { get; set; } = string.Empty;
        public int ExistingEmployeeCount { get; set; }
        public List<string> PrerequisiteIssues { get; set; } = new List<string>();
        public List<InitialEmployeeImportRowViewModel> PreviewRows { get; set; } = new List<InitialEmployeeImportRowViewModel>();

        public int ValidRowsCount => PreviewRows.Count(r => r.IsValid);
        public int InvalidRowsCount => PreviewRows.Count(r => !r.IsValid);
        public bool HasPreview => PreviewRows.Any();
        public bool CanImport => !PrerequisiteIssues.Any();
    }
}