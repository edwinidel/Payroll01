using System.Collections.Generic;
using System.Threading.Tasks;
using _2FA.Helpers;
using _2FA.Models;
using _2FA.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _2FA.Controllers.Api
{
    [ApiController]
    [Route("api/employee/paystubs")]
    [Authorize(Roles = "Empleado")]
    public class EmployeePaystubsController : ControllerBase
    {
        private readonly IPaystubService _service;

        public EmployeePaystubsController(IPaystubService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<PaystubSummaryDto>>> List([FromQuery] DateOnly? from, [FromQuery] DateOnly? to, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var employeeId = User.RequireEmployeeId();
            var result = await _service.GetPaystubsAsync(employeeId, from, to, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(int id)
        {
            var employeeId = User.RequireEmployeeId();
            var file = await _service.DownloadPaystubAsync(employeeId, id);
            return File(file.Content, file.ContentType, file.FileName);
        }
    }
}
