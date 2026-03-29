using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SlotsController : ControllerBase
	{
		private readonly BookingService _service;

		public SlotsController(BookingService service)
		{
			_service = service;
		}

		[AllowAnonymous]
		[HttpGet("available")]
		public async Task<IActionResult> GetAvailable()
		{
			var result = await _service.GetAvailableSlots();
			return Ok(result);
		}
	}
}