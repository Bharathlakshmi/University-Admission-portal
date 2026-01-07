using System.ComponentModel.DataAnnotations;

namespace University_Admission_Portal.Models
{

    public enum ApplicationStatus
    {
        Pending=1,
        Approved=2,
        Disapproved=3
    }
    public class Application
    {
        [Key]
        public int ApplicationId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int? ApprovedByStaffId { get; set; }
        public DateTime AppliedOn { get; set; } = DateTime.Now;
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

        public Student? Student { get; set; }
        public Course? Course { get; set; }
        public Staff? ApprovedByStaff { get; set; }
    }
}
