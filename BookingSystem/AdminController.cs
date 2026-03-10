using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Admin")]
	public class AdminController : ControllerBase
	{
		private readonly BookingService _service;

		public AdminController(BookingService service)
		{
			_service = service;
		}

		[HttpGet("all-appointments")]
		public async Task<IActionResult> AllAppointments()
		{
			var result = await _service.GetAllAppointments();
			return Ok(result);
		}
	}
}