namespace University_Admission_Portal.DTOs
{
    public class CourseDTO
    {
        public int CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public int TotalSeats { get; set; }
    }

    public class CreateCourseDTO
    {
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public int TotalSeats { get; set; }
    }

    public class GetCourseDTO
    {
        public int CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public int TotalSeats { get; set; }

        public int RemainingSeats { get; set; }
    }

   
}
