using System.ComponentModel.DataAnnotations;

namespace QRCodeAttendanceSystem.Models
{
    public class Class
    {
        [Key]
        public int ClassId { get; set; }

        [Required]
        public string ClassName { get; set; } = string.Empty;

        public ICollection<User>? Users { get; set; }
    }
}