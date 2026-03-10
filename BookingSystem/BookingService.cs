using System;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem
{
	public class BookingService
	{
		private readonly AppDbContext _db;

		public BookingService(AppDbContext db)
		{
			_db = db;
		}

		public async Task<object> GetAvailableSlots()
		{
			return await _db.TimeSlots
				.Where(x => x.IsAvailable)
				.Select(x => new
				{
					x.Id,
					x.DoctorId,
					DoctorName = x.Doctor.Name,
					x.SlotDate,
					x.StartTime,
					x.EndTime
				})
				.ToListAsync();
		}

		public async Task<string> BookAppointment(int userId, BookAppointmentRequest request)
		{
			var slot = await _db.TimeSlots
				.FirstOrDefaultAsync(x => x.Id == request.TimeSlotId);

			if (slot == null)
				return "Time slot not found";

			if (!slot.IsAvailable)
				return "This time is not available";

			var alreadyBooked = await _db.Appointments.AnyAsync(x =>
				x.UserId == userId &&
				x.TimeSlotId == request.TimeSlotId &&
				x.Status != "Cancelled" &&
				x.Status != "Rejected");

			if (alreadyBooked)
				return "You already booked this slot";

			var appointment = new Appointment
			{
				UserId = userId,
				DoctorId = slot.DoctorId,
				TimeSlotId = slot.Id,
				Status = "Pending",
				Notes = request.Notes,
				CreatedAt = DateTime.Now
			};

			_db.Appointments.Add(appointment);
			await _db.SaveChangesAsync();

			return "Appointment booked successfully";
		}

		public async Task<object> GetMyAppointments(int userId)
		{
			return await _db.Appointments
				.Where(x => x.UserId == userId)
				.Select(x => new
				{
					x.Id,
					DoctorName = x.Doctor.Name,
					x.Status,
					x.Notes,
					x.CreatedAt,
					x.TimeSlot.SlotDate,
					x.TimeSlot.StartTime,
					x.TimeSlot.EndTime
				})
				.ToListAsync();
		}

		public async Task<string> CancelAppointment(int appointmentId, int userId)
		{
			var appointment = await _db.Appointments
				.FirstOrDefaultAsync(x => x.Id == appointmentId && x.UserId == userId);

			if (appointment == null)
				return "Appointment not found";

			if (appointment.Status == "Approved")
				return "Approved appointment cannot be cancelled";

			appointment.Status = "Cancelled";
			await _db.SaveChangesAsync();

			return "Appointment cancelled";
		}

		public async Task<object> GetDoctorRequests(int doctorId)
		{
			return await _db.Appointments
				.Where(x => x.DoctorId == doctorId)
				.Select(x => new
				{
					x.Id,
					UserName = x.User.Username,
					x.Status,
					x.Notes,
					x.TimeSlot.SlotDate,
					x.TimeSlot.StartTime,
					x.TimeSlot.EndTime
				})
				.ToListAsync();
		}

		public async Task<string> ApproveAppointment(int appointmentId, int doctorId)
		{
			var appointment = await _db.Appointments
				.Include(x => x.TimeSlot)
				.FirstOrDefaultAsync(x => x.Id == appointmentId);

			if (appointment == null)
				return "Appointment not found";

			if (appointment.DoctorId != doctorId)
				return "This appointment does not belong to you";

			if (appointment.Status != "Pending")
				return "Only pending appointments can be approved";

			if (!appointment.TimeSlot.IsAvailable)
				return "This slot is no longer available";

			appointment.Status = "Approved";
			appointment.TimeSlot.IsAvailable = false;

			await _db.SaveChangesAsync();

			return "Appointment approved";
		}

		public async Task<string> RejectAppointment(int appointmentId, int doctorId, string? notes)
		{
			var appointment = await _db.Appointments
				.FirstOrDefaultAsync(x => x.Id == appointmentId);

			if (appointment == null)
				return "Appointment not found";

			if (appointment.DoctorId != doctorId)
				return "This appointment does not belong to you";

			if (appointment.Status != "Pending")
				return "Only pending appointments can be rejected";

			appointment.Status = "Rejected";
			appointment.Notes = notes;

			await _db.SaveChangesAsync();

			return "Appointment rejected";
		}

		public async Task<object> GetAllAppointments()
		{
			return await _db.Appointments
				.Select(x => new
				{
					x.Id,
					UserName = x.User.Username,
					DoctorName = x.Doctor.Name,
					x.Status,
					x.Notes,
					x.TimeSlot.SlotDate,
					x.TimeSlot.StartTime,
					x.TimeSlot.EndTime
				})
				.ToListAsync();
		}
	}
}