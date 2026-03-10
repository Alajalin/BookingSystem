
namespace BookingSystem
{
	public class TimeSlot
	{
		public int Id { get; set; }

		public int DoctorId { get; set; }
		public Doctor Doctor { get; set; }

		public DateOnly SlotDate { get; set; }
		public TimeOnly StartTime { get; set; }
		public TimeOnly EndTime { get; set; }

		public bool IsAvailable { get; set; }
	}
}