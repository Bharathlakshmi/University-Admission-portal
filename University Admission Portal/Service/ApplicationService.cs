using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using University_Admission_Portal.DTOs;
using University_Admission_Portal.Email_Notification;
using University_Admission_Portal.Interface;
using University_Admission_Portal.Models;


namespace University_Admission_Portal.Service
{
    public class ApplicationService : IApplicationService
    {
        private readonly IUniversity<Application, int> _appRepo;
        private readonly IApplication _app;
        private readonly IUniversity<Student, int> _studentRepo;
        private readonly IUniversity<Course, int> _courseRepo;
        private readonly IUniversity<Staff, int> _staffRepo;
        private readonly IEmailService _emailService;
        private readonly ILogger<ApplicationService> _logger;

        public ApplicationService(
            IUniversity<Application, int> appRepo,
            IApplication app,
            IUniversity<Student, int> studentRepo,
            IUniversity<Course, int> courseRepo,
            IUniversity<Staff, int> staffRepo,
            IEmailService emailService,
             ILogger<ApplicationService> logger)
        {
            _appRepo = appRepo;
            _app = app;
            _studentRepo = studentRepo;
            _courseRepo = courseRepo;
            _staffRepo = staffRepo;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<IEnumerable<GetApplicationDTO>> GetAllApplications()
        {
            var apps = await _appRepo.GetAll();
            var students = await _studentRepo.GetAll();
            var courses = await _courseRepo.GetAll();
            var staff = await _staffRepo.GetAll();

            return apps.Select(a => new GetApplicationDTO
            {
                ApplicationId = a.ApplicationId,
                StudentId = a.StudentId,
                CourseId = a.CourseId,
                ApprovedByStaffId = a.ApprovedByStaffId,
                AppliedOn = a.AppliedOn,
                Status = a.Status,

                StudentName = students.FirstOrDefault(s => s.StudentId == a.StudentId)?.User?.Name ?? "Unknown Student",
                CourseName = courses.FirstOrDefault(c => c.CourseId == a.CourseId)?.CourseName ?? "Unknown Course",
                ApprovedByStaff = staff.FirstOrDefault(st => st.StaffId == a.ApprovedByStaffId)?.User?.Name
            });
        }

        public async Task<GetApplicationDTO?> GetApplicationById(int id)
        {
            var app = await _appRepo.GetById(id);
            if (app == null) return null;

            var student = await _studentRepo.GetById(app.StudentId);
            var course = await _courseRepo.GetById(app.CourseId);

            Staff? staff = null;
            if (app.ApprovedByStaffId.HasValue)
                staff = await _staffRepo.GetById(app.ApprovedByStaffId.Value);

            return new GetApplicationDTO
            {
                ApplicationId = app.ApplicationId,
                StudentId = app.StudentId,
                CourseId = app.CourseId,
                ApprovedByStaffId = app.ApprovedByStaffId,
                AppliedOn = app.AppliedOn,
                Status = app.Status,

                StudentName = student?.User?.Name ?? "Unknown Student",
                CourseName = course?.CourseName ?? "Unknown Course",
                ApprovedByStaff = staff?.User?.Name
            };
        }

        public async Task<ApplicationDTO> CreateApplication(CreateApplicationDTO dto)
        {
            //get course
            var course = await _courseRepo.GetById(dto.CourseId);
            if (course == null)
                throw new Exception("Course not found");

            //total seats
            var allApplications = await _appRepo.GetAll();
            var approvedCount = allApplications.Count(a =>
                a.CourseId == dto.CourseId &&
                a.Status == ApplicationStatus.Approved);

            if (approvedCount >= course.TotalSeats)
                throw new Exception("No seats available for this course");

            // Checking - already applied for 3 courses
            var studentApplications = allApplications
                .Where(a => a.StudentId == dto.StudentId)
                .ToList();

            if (studentApplications.Count >= 3)
                throw new Exception("You have already applied for 3 courses");

            // Prevent - duplicate application for the same course
            if (studentApplications.Any(a => a.CourseId == dto.CourseId))
                throw new Exception("You have already applied for this particular course");


            var newApp = new Application
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                AppliedOn = DateTime.Now,
                Status = ApplicationStatus.Pending
            };

            var created = await _appRepo.Add(newApp);

            return new ApplicationDTO
            {
                ApplicationId = created.ApplicationId,
                StudentId = created.StudentId,
                CourseId = created.CourseId,
                ApprovedByStaffId = created.ApprovedByStaffId,
                AppliedOn = created.AppliedOn,
                Status = created.Status
            };
        }

        public async Task UpdateApplication(int id, CreateApplicationDTO dto)
        {
            var app = new Application
            {
                ApplicationId = id,
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                AppliedOn = DateTime.Now,
                Status = ApplicationStatus.Pending
            };
            await _appRepo.Update(id, app);
        }

        public async Task<bool> DeleteApplication(int id)
        {
            try { await _appRepo.Delete(id); return true; }
            catch { return false; }
        }

        public async Task UpdateApplicationStatus(int id, ApplicationStatus newStatus, int staffId)
        {
            var app = await _appRepo.GetById(id);
            if (app == null)
                throw new Exception("Application not found");

            // Fetch the related course
            var course = await _courseRepo.GetById(app.CourseId);
            if (course == null)
                throw new Exception("Course not found");

            // Count how many applications for this course are already approved
            var allApplications = await _appRepo.GetAll();
            var approvedCount = allApplications
                .Count(a => a.CourseId == app.CourseId && a.Status == ApplicationStatus.Approved);

            //  Check seat availability before approving
            if (newStatus == ApplicationStatus.Approved && approvedCount >= course.TotalSeats)
                throw new Exception($"No seats available for course '{course.CourseName}'. Total seats: {course.TotalSeats}");

            // Check if student already has another approved application
            var student = await _studentRepo.GetById(app.StudentId);
            if (student == null)
                throw new Exception("Student not found");

            if (newStatus == ApplicationStatus.Approved)
            {
                var allApplications1 = await _appRepo.GetAll();
                bool alreadyApproved = allApplications1.Any(a =>
                    a.StudentId == student.StudentId &&
                    a.ApplicationId != id &&
                    a.Status == ApplicationStatus.Approved);

                if (alreadyApproved)
                    throw new Exception($"Student '{student.User?.Name}' already has an approved course. Only one approval is allowed per student.");
            }


            //Update status and approval staff
            app.Status = newStatus;
            app.ApprovedByStaffId = staffId;

            await _app.Update(app);


            //  Send notification email for any status change
           // var student = await _studentRepo.GetById(app.StudentId);
            if (student?.User?.Email != null)
            {
                string statusMessage = newStatus switch
                {
                    ApplicationStatus.Approved => $"Congratulations! Your application for {course?.CourseName ?? "your course"} has been approved.",
                    ApplicationStatus.Disapproved => $"We regret to inform you that your application for {course?.CourseName ?? "your course"} has been disapproved.",
                    ApplicationStatus.Pending => $"Your application status for {course?.CourseName ?? "your course"} has been changed back to Pending. Please await review.",
                    _ => $"Your application status for {course?.CourseName ?? "your course"} has been updated."
                };

                try
                {
                    await _emailService.SendEmailAsync(
                        student.User.Email,
                        "Application Status Update",
                        $"Hi {student.User.Name ?? "Student"},\n\n{statusMessage}\n\nBest regards,\nAdmission Office,\nOdyssey University",
                        isHtml: false
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send status email for application {ApplicationId}", id);
                }
            }
        }


        public async Task<IEnumerable<CourseApplicationsDTO>> GetAllApplicationsGroupedByCourse()
        {
            var courses = await _app.GetAllCoursesWithApplications();
            var allApplications = await _appRepo.GetAll();

            var result = courses.Select(c => 
            {
                // Calculate approved applications for this course
                var approvedCount = allApplications.Count(a => 
                    a.CourseId == c.CourseId && 
                    a.Status == ApplicationStatus.Approved);
                
                return new CourseApplicationsDTO
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    TotalSeats = c.TotalSeats,
                    RemainingSeats = c.TotalSeats - approvedCount,
                    Applications = c.Applications
                        .Where(a => a.Student != null)
                        .Select(a => new ApplicationWithStudentDTO
                        {
                            ApplicationId = a.ApplicationId,
                            StudentId = a.Student.StudentId,
                            StudentName = a.Student.User?.Name ?? "Unknown",
                            City = a.Student.City,
                            Status = a.Status.ToString(),
                            PhotoBase64 = a.Student.Photo != null ? Convert.ToBase64String(a.Student.Photo) : null,
                            MarksheetBase64 = a.Student.Marksheet != null ? Convert.ToBase64String(a.Student.Marksheet) : null
                        })
                        .ToList()
                };
            });

            return result;
        }


        public async Task<IEnumerable<object>> GetApplicationsByStudentId(int studentId)
        {
            var applications = await _app.GetApplicationsByStudentId(studentId);

            if (!applications.Any())
                throw new Exception("No applications found for this student.");

            var result = applications.Select(a => new
            {
                ApplicationId = a.ApplicationId,
                CourseName = a.Course?.CourseName ?? "N/A",
                AppliedOn = a.AppliedOn,
                Status = (int)a.Status  // Explicitly cast to int
            });

            return result;
        }





    }
}
