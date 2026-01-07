using University_Admission_Portal.Data;
using University_Admission_Portal.DTOs;
using University_Admission_Portal.Email_Notification;
using University_Admission_Portal.Interface;
using University_Admission_Portal.Models;

namespace University_Admission_Portal.Service
{
    public class StaffService : IStaffService
    {
        private readonly IUniversity<Staff, int> _repo;
        private readonly IUniversity<Application, int> _applicationRepo;
        private readonly IUniversity<Course, int> _courseRepo;
        private readonly IUniversity<User, int> _userRepo;
        private readonly IEmailService _emailService; 
        private readonly ILogger<StaffService> _logger;
        public StaffService(
            IUniversity<Staff, int> repo,
            IUniversity<Application, int> applicationRepo,
            IUniversity<Course, int> courseRepo,
            IUniversity<User, int> userRepo,
            IEmailService emailService,
            ILogger<StaffService> logger)
        {
            _repo = repo;
            _applicationRepo = applicationRepo;
            _courseRepo = courseRepo;
            _userRepo = userRepo;
            _emailService = emailService; 
            _logger = logger; 
        }

        public async Task<IEnumerable<GetStaffDTO>> GetAllStaff()
        {
            var staffList = await _repo.GetAll();
            var applications = await _applicationRepo.GetAll();
            var courses = await _courseRepo.GetAll();

            return staffList.Select(staff =>
            {
                Console.WriteLine($"Staff ID: {staff.StaffId}, User ID: {staff.UserId}, User is null: {staff.User == null}");
                if (staff.User != null)
                {
                    Console.WriteLine($"User Name: {staff.User.Name}, User Email: {staff.User.Email}");
                }

                var approvedCourses = applications
                    .Where(a => a.ApprovedByStaffId == staff.StaffId && a.Status == ApplicationStatus.Approved)
                    .Select(a => courses.FirstOrDefault(c => c.CourseId == a.CourseId)?.CourseName ?? "Unknown")
                    .ToList();

                return new GetStaffDTO
                {
                    StaffId = staff.StaffId,
                    UserId = staff.UserId,
                    Designation = staff.Designation,
                    ApprovedApplications = approvedCourses,
                    UserName = staff.User?.Name ?? "N/A",
                    UserEmail = staff.User?.Email ?? "N/A"
                };
            });
        }

        public async Task<GetStaffDTO> GetStaffById(int id)
        {
            var staff = await _repo.GetById(id);
            if (staff == null) return null;

            var applications = await _applicationRepo.GetAll();
            var courses = await _courseRepo.GetAll();

            var approvedCourses = applications
                .Where(a => a.ApprovedByStaffId == staff.StaffId && a.Status == ApplicationStatus.Approved)
                .Select(a => courses.FirstOrDefault(c => c.CourseId == a.CourseId)?.CourseName ?? "Unknown")
                .ToList();

            return new GetStaffDTO
            {
                StaffId = staff.StaffId,
                UserId = staff.UserId,
                Designation = staff.Designation,
                ApprovedApplications = approvedCourses,
                UserName = staff.User?.Name ?? "N/A",
                UserEmail = staff.User?.Email ?? "N/A"
            };
        }

        public async Task<StaffDTO> CreateStaff(CreateStaffDTO dto)
        {
            var s = new Staff
            {
                UserId = dto.UserId,
                Designation = dto.Designation
            };
            var created = await _repo.Add(s);

            try
            {
                var user = await _userRepo.GetById(dto.UserId);
                if (user?.Email != null)
                {
                    string emailBody = $@"Dear {user.Name ?? "Staff Member"},

  Welcome to Odyssey University!

Your staff account has been successfully created. Here are your login credentials:

Email: {user.Email}
Password: {user.Password}
Designation: {dto.Designation}

Please keep this information secure.

Best regards,
Admin Team
Odyssey University";

                    await _emailService.SendEmailAsync(
                        user.Email,
                        "Welcome to Odyssey University - Staff Account Created",
                        emailBody,
                        isHtml: false
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email for staff {StaffId}", created.StaffId);
            }
            return new StaffDTO
            {
                StaffId = created.StaffId,
                UserId = created.UserId,
                Designation = created.Designation
            };
        }

        public async Task UpdateStaff(int id, CreateStaffDTO dto)
        {
            var s = new Staff
            {
                StaffId = id,
                UserId = dto.UserId,
                Designation = dto.Designation
            };
            await _repo.Update(id, s);
        }

        public async Task<bool> DeleteStaff(int id)
        {
            try { await _repo.Delete(id); return true; }
            catch { return false; }
        }
    }
}
