using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRCodeAttendanceSystem.Models
{
    public class AttendanceSession
    {
        [Key]
        public int SessionId { get; set; }

        [Required]
        public string SessionName { get; set; } = string.Empty;

        [Required]
        public DateTime SessionDate { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public int LateThreshold { get; set; } = 15;

        public string? QRCodeValue { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        [ForeignKey("CreatedByUser")]
        public int CreatedBy { get; set; }
        public User? CreatedByUser { get; set; }

        public ICollection<AttendanceRecord>? AttendanceRecords { get; set; }
    }
}