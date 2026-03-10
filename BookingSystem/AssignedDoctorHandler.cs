using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem
{
	public class AssignedDoctorHandler : AuthorizationHandler<AssignedDoctorRequirement>
	{
		private readonly AppDbContext _db;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AssignedDoctorHandler(AppDbContext db, IHttpContextAccessor httpContextAccessor)
		{
			_db = db;
			_httpContextAccessor = httpContextAccessor;
		}

		protected override async Task HandleRequirementAsync(
			AuthorizationHandlerContext context,
			AssignedDoctorRequirement requirement)
		{
			var httpContext = _httpContextAccessor.HttpContext;
			if (httpContext == null)
				return;

			var routeValue = httpContext.Request.RouteValues["id"]?.ToString();
			if (string.IsNullOrEmpty(routeValue))
				return;

			if (!int.TryParse(routeValue, out int appointmentId))
				return;

			var doctorClaim = context.User.FindFirst("DoctorId")?.Value;
			if (string.IsNullOrEmpty(doctorClaim))
				return;

			int doctorId = int.Parse(doctorClaim);

			var appointment = await _db.Appointments
				.FirstOrDefaultAsync(x => x.Id == appointmentId);

			if (appointment == null)
				return;

			if (appointment.DoctorId == doctorId)
			{
				context.Succeed(requirement);
			}
		}
	}
}