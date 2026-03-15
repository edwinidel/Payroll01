using System.Threading.Tasks;
using _2FA.Helpers;
using _2FA.Models;
using _2FA.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _2FA.Controllers.Api
{
    [ApiController]
    [Route("api/employee/balances")]
    [Authorize(Roles = "Empleado")]
    public class BalancesController : ControllerBase
    {
        private readonly IBalanceService _service;

        public BalancesController(IBalanceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<LeaveBalanceDto>> Get()
        {
            var employeeId = User.RequireEmployeeId();
            var dto = await _service.GetBalancesAsync(employeeId);
            return Ok(dto);
        }
    }
}
