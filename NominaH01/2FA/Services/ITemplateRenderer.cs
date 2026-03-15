using System.Threading.Tasks;
using _2FA.Data.Entities;

namespace _2FA.Services
{
    public interface ITemplateRenderer
    {
        Task<string> RenderAsync(DocumentTemplateEntity template, EmployeeEntity employee);
    }
}
