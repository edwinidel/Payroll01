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
    [Route("api/employee/notifications")]
    [Authorize(Roles = "Empleado")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationsController(INotificationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<NotificationDto>>> List([FromQuery] bool unreadOnly = false, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var employeeId = User.RequireEmployeeId();
            var items = await _service.GetAsync(employeeId, unreadOnly, page, pageSize);
            return Ok(items);
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkRead(int id)
        {
            var employeeId = User.RequireEmployeeId();
            await _service.MarkAsReadAsync(employeeId, id);
            return NoContent();
        }
    }
}
