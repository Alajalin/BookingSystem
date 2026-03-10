namespace BookingSystem
{
	public class Doctor
	{
		public int Id { get; set; }

		public string Name { get; set; }
		public string? Specialty { get; set; }

		public int UserId { get; set; }
		public User User { get; set; }

		public List<TimeSlot> TimeSlots { get; set; } = new();
		public List<Appointment> Appointments { get; set; } = new();
	}
}