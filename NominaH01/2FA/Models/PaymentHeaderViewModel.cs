using _2FA.Data.Entities;

namespace _2FA.Models
{
    public class PayrollHeaderViewModel : PayrollTmpHeaderEntity
    {
        public List<int> SelectedEmployeeIds { get; set; } = [];
    }
}
