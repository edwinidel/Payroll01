using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using _2FA.Models;

namespace _2FA.Services
{
    public interface IPaystubService
    {
        Task<IReadOnlyList<PaystubSummaryDto>> GetPaystubsAsync(int employeeId, DateOnly? from, DateOnly? to, int page, int pageSize);
        Task<PaystubDownloadDto> DownloadPaystubAsync(int employeeId, int paystubId);
        Task RegisterPaystubAsync(int employeeId, int payrollHeaderId, DateOnly start, DateOnly end, decimal gross, decimal net, Stream file, string fileName);
    }
}
