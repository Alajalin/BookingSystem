
namespace BookingSystem
{
	public class TimeSlot
	{
		public int Id { get; set; }

		public int DoctorId { get; set; }
		public Doctor Doctor { get; set; }

		public DateTime SlotDate { get; set; }
		public TimeSpan StartTime { get; set; }
		public TimeSpan EndTime { get; set; }

		public bool IsAvailable { get; set; }
	}
}