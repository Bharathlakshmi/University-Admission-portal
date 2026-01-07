namespace University_Admission_Portal.DTOs
{
    public class StaffDTO
    {
        public int StaffId { get; set; }
        public int UserId { get; set; }
        public string Designation { get; set; } = string.Empty;
    }
    public class CreateStaffDTO
    {
        public int UserId { get; set; }
        public string Designation { get; set; } = string.Empty;
    }

    public class GetStaffDTO
    {
        public int StaffId { get; set; }
        public int UserId { get; set; }
        public string Designation { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;

        // list of course names approved by this staff
        public List<string> ApprovedApplications { get; set; } = new();
    }
}
