using University_Admission_Portal.Models;

namespace University_Admission_Portal.DTOs
{
    public class ApplicationDTO
    {
        public int ApplicationId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int? ApprovedByStaffId { get; set; }
        public DateTime AppliedOn { get; set; }
        public ApplicationStatus Status { get; set; }
    }

    public class CreateApplicationDTO
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        
       // public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    }

    public class GetApplicationDTO
    {
        public int ApplicationId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int? ApprovedByStaffId { get; set; }
        public DateTime AppliedOn { get; set; }
        public ApplicationStatus Status { get; set; }

        //
        public string StudentName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string? ApprovedByStaff { get; set; }
    }



    public class ApplicationWithStudentDTO
    {
        public int ApplicationId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? PhotoBase64 { get; set; }
        public string? MarksheetBase64 { get; set; }
    }

    public class CourseApplicationsDTO
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int TotalSeats { get; set; }
        public int RemainingSeats { get; set; }
        public List<ApplicationWithStudentDTO> Applications { get; set; } = new();
    }
}
