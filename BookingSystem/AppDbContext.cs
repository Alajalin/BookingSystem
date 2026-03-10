using System.Numerics;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Doctor> Doctors { get; set; }
		public DbSet<TimeSlot> TimeSlots { get; set; }
		public DbSet<Appointment> Appointments { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Doctor>()
				.HasOne(d => d.User)
				.WithMany()
				.HasForeignKey(d => d.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<TimeSlot>()
				.HasOne(t => t.Doctor)
				.WithMany(d => d.TimeSlots)
				.HasForeignKey(t => t.DoctorId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Appointment>()
				.HasOne(a => a.User)
				.WithMany(u => u.Appointments)
				.HasForeignKey(a => a.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Appointment>()
				.HasOne(a => a.Doctor)
				.WithMany(d => d.Appointments)
				.HasForeignKey(a => a.DoctorId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Appointment>()
				.HasOne(a => a.TimeSlot)
				.WithMany()
				.HasForeignKey(a => a.TimeSlotId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}