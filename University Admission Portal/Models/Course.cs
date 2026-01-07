using System.ComponentModel.DataAnnotations;

namespace University_Admission_Portal.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public int TotalSeats { get; set; }

        public ICollection<Application>? Applications { get; set; }
    }
}
