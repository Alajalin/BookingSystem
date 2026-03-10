using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingSystem.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "User")]
	public class AppointmentsController : ControllerBase
	{
		private readonly BookingService _service;

		public AppointmentsController(BookingService service)
		{
			_service = service;
		}

		[HttpPost("book")]
		public async Task<IActionResult> Book(BookAppointmentRequest request)
		{
			int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
			var result = await _service.BookAppointment(userId, request);

			return Ok(new { message = result });
		}

		[HttpGet("my-appointments")]
		public async Task<IActionResult> MyAppointments()
		{
			int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
			var result = await _service.GetMyAppointments(userId);

			return Ok(result);
		}

		[HttpPut("cancel/{id}")]
		public async Task<IActionResult> Cancel(int id)
		{
			int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
			var result = await _service.CancelAppointment(id, userId);

			return Ok(new { message = result });
		}
	}
}