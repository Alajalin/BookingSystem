using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Doctor")]
	public class DoctorController : ControllerBase
	{
		private readonly BookingService _service;

		public DoctorController(BookingService service)
		{
			_service = service;
		}

		[HttpGet("my-requests")]
		public async Task<IActionResult> MyRequests()
		{
			int doctorId = int.Parse(User.FindFirst("DoctorId")!.Value);
			var result = await _service.GetDoctorRequests(doctorId);

			return Ok(result);
		}

		[Authorize(Policy = "AssignedDoctorOnly")]
		[HttpPut("approve/{id}")]
		public async Task<IActionResult> Approve(int id)
		{
			int doctorId = int.Parse(User.FindFirst("DoctorId")!.Value);
			var result = await _service.ApproveAppointment(id, doctorId);

			return Ok(new { message = result });
		}

		[Authorize(Policy = "AssignedDoctorOnly")]
		[HttpPut("reject/{id}")]
		public async Task<IActionResult> Reject(int id, AppointmentActionRequest request)
		{
			int doctorId = int.Parse(User.FindFirst("DoctorId")!.Value);
			var result = await _service.RejectAppointment(id, doctorId, request.Notes);

			return Ok(new { message = result });
		}
	}
}