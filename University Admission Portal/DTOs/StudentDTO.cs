namespace University_Admission_Portal.DTOs
{
    public class StudentDTO
    {
        public int StudentId { get; set; }
        public int UserId { get; set; }
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public byte[]? Photo { get; set; }
        public byte[]? Marksheet { get; set; }
    }

    public class CreateStudentDTO
    {
        public int UserId { get; set; }
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public IFormFile? Photo { get; set; }
        public IFormFile? Marksheet { get; set; }

    }

    public class GetStudentDTO
    {
        public int StudentId { get; set; }
        public int UserId { get; set; }
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;

        public string? PhotoBase64 { get; set; }
        public string? MarksheetBase64 { get; set; }

        // List of course names applied by this student
        public List<string> ApplicationCourse { get; set; } = new();
    }
}
