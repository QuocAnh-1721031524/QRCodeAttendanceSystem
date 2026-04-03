using Microsoft.EntityFrameworkCore;
using QRCodeAttendanceSystem.Models;

namespace QRCodeAttendanceSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AttendanceSession> AttendanceSessions { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.StudentCode)
                .IsUnique();

            modelBuilder.Entity<AttendanceSession>()
                .HasIndex(s => s.QRCodeValue)
                .IsUnique();

            modelBuilder.Entity<AttendanceRecord>()
                .HasIndex(a => new { a.SessionId, a.UserId })
                .IsUnique();

            modelBuilder.Entity<AttendanceSession>()
                .HasOne(s => s.CreatedByUser)
                .WithMany(u => u.CreatedSessions)
                .HasForeignKey(s => s.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}