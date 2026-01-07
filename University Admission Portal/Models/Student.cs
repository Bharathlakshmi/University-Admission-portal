using System.ComponentModel.DataAnnotations;

namespace University_Admission_Portal.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }
        public int UserId { get; set; }
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public byte[]? Photo { get; set; }
        public byte[]? Marksheet { get; set; }

        public User? User { get; set; }
        public ICollection<Application>? Applications { get; set; }
    }
}
