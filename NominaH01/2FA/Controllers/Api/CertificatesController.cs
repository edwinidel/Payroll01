using System.Threading.Tasks;
using _2FA.Helpers;
using _2FA.Models;
using _2FA.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _2FA.Controllers.Api
{
    [ApiController]
    [Route("api/employee/certificates")]
    [Authorize(Roles = "Empleado")]
    public class CertificatesController : ControllerBase
    {
        private readonly ICertificateService _service;

        public CertificatesController(ICertificateService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CertificateRequestDto request)
        {
            var employeeId = User.RequireEmployeeId();
            var file = await _service.GenerateAsync(employeeId, request);
            return File(file.Content, file.ContentType, file.FileName);
        }
    }
}
