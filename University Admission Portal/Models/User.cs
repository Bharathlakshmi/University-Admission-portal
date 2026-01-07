using System.ComponentModel.DataAnnotations;

namespace University_Admission_Portal.Models
{
    public enum UserRole
    {
        Student=1,
        Staff=2,
        admin=3
    }
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }

        public Student? StudentProfile { get; set; }
        public Staff? StaffProfile { get; set; }
    }
}
