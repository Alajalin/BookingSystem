using System;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly AppDbContext _db;
		private readonly TokenService _tokenService;

		public AuthController(AppDbContext db, TokenService tokenService)
		{
			_db = db;
			_tokenService = tokenService;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginRequest request)
		{
			var user = await _db.Users
				.FirstOrDefaultAsync(x =>
					x.Username == request.Username &&
					x.Password == request.Password);

			if (user == null)
				return Unauthorized("Wrong username or password");

			int? doctorId = null;

			if (user.Role == "Doctor")
			{
				doctorId = await _db.Doctors
					.Where(d => d.UserId == user.Id)
					.Select(d => (int?)d.Id)
					.FirstOrDefaultAsync();
			}

			var token = _tokenService.CreateToken(user, doctorId);

			return Ok(new
			{
				token,
				user.Username,
				user.Role
			});
		}
	}
}