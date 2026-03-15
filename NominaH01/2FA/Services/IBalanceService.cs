using System.Threading.Tasks;
using _2FA.Models;

namespace _2FA.Services
{
    public interface IBalanceService
    {
        Task<LeaveBalanceDto> GetBalancesAsync(int employeeId);
    }
}
