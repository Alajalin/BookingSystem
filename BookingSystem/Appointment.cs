using System.Numerics;

namespace BookingSystem
{
	public class Appointment
	{
		public int Id { get; set; }

		public int UserId { get; set; }
		public User User { get; set; }

		public int DoctorId { get; set; }
		public Doctor Doctor { get; set; }

		public int TimeSlotId { get; set; }
		public TimeSlot TimeSlot { get; set; }

		public string Status { get; set; }
		public string? Notes { get; set; }

		public DateTime CreatedAt { get; set; }
	}
}