using System.Threading.Tasks;
using _2FA.Models;

namespace _2FA.Services
{
    public interface ICertificateService
    {
        Task<CertificateResponseDto> GenerateAsync(int employeeId, CertificateRequestDto request);
    }
}
