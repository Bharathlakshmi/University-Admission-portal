using System.ComponentModel.DataAnnotations;

namespace University_Admission_Portal.Models
{
  
    public class Staff
    {
        [Key]
        public int StaffId { get; set; }
        public int UserId { get; set; }
        public string Designation { get; set; } = string.Empty;

        public User? User { get; set; }
        public ICollection<Application>? ApprovedApplications { get; set; }
    }
}
