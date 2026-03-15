using System.Threading;
using System.Threading.Tasks;

namespace _2FA.Services
{
    public interface INotificationGenerationService
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}
