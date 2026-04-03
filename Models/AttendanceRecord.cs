using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRCodeAttendanceSystem.Models
{
    public class AttendanceRecord
    {
        [Key]
        public int RecordId { get; set; }

        [ForeignKey("AttendanceSession")]
        public int SessionId { get; set; }
        public AttendanceSession? AttendanceSession { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }

        public DateTime? CheckInTime { get; set; }

        [Required]
        public string Status { get; set; } = "Absent";

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}