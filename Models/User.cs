using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRCodeAttendanceSystem.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string? StudentCode { get; set; }

        public string? Email { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey("Role")]
        public int RoleId { get; set; }
        public Role? Role { get; set; }

        [ForeignKey("Class")]
        public int? ClassId { get; set; }
        public Class? Class { get; set; }

        public ICollection<AttendanceRecord>? AttendanceRecords { get; set; }
        public ICollection<AttendanceSession>? CreatedSessions { get; set; }
    }
}