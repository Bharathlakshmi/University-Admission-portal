using Moq;
using University_Admission_Portal.DTOs;
using University_Admission_Portal.Email_Notification;
using University_Admission_Portal.Interface;
using University_Admission_Portal.Models;
using University_Admission_Portal.Service;
using Microsoft.Extensions.Logging;
using Xunit;

namespace University_Admission_Portal.Tests
{
    public class ApplicationServiceTests
    {
        private readonly Mock<IUniversity<Application, int>> _mockAppRepo;
        private readonly Mock<IApplication> _mockApp;
        private readonly Mock<IUniversity<Student, int>> _mockStudentRepo;
        private readonly Mock<IUniversity<Course, int>> _mockCourseRepo;
        private readonly Mock<IUniversity<Staff, int>> _mockStaffRepo;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<ILogger<ApplicationService>> _mockLogger;
        private readonly ApplicationService _service;

        public ApplicationServiceTests()
        {
            _mockAppRepo = new Mock<IUniversity<Application, int>>();
            _mockApp = new Mock<IApplication>();
            _mockStudentRepo = new Mock<IUniversity<Student, int>>();
            _mockCourseRepo = new Mock<IUniversity<Course, int>>();
            _mockStaffRepo = new Mock<IUniversity<Staff, int>>();
            _mockEmailService = new Mock<IEmailService>();
            _mockLogger = new Mock<ILogger<ApplicationService>>();
            
            _service = new ApplicationService(
                _mockAppRepo.Object,
                _mockApp.Object,
                _mockStudentRepo.Object,
                _mockCourseRepo.Object,
                _mockStaffRepo.Object,
                _mockEmailService.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetAllApplications_ReturnsApplicationsWithStudentAndCourseNames()
        {
            // Arrange
            var applications = new List<Application>
            {
                new Application { ApplicationId = 1, StudentId = 1, CourseId = 1 }
            };
            var students = new List<Student>
            {
                new Student { StudentId = 1, User = new User { Name = "John Doe" } }
            };
            var courses = new List<Course>
            {
                new Course { CourseId = 1, CourseName = "Computer Science" }
            };
            
            _mockAppRepo.Setup(r => r.GetAll()).ReturnsAsync(applications);
            _mockStudentRepo.Setup(r => r.GetAll()).ReturnsAsync(students);
            _mockCourseRepo.Setup(r => r.GetAll()).ReturnsAsync(courses);
            _mockStaffRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Staff>());

            // Act
            var result = await _service.GetAllApplications();

            // Assert
            var appDto = result.First();
            Assert.Equal("John Doe", appDto.StudentName);
            Assert.Equal("Computer Science", appDto.CourseName);
        }

        [Fact]
        public async Task CreateApplication_ValidData_ReturnsCreatedApplication()
        {
            // Arrange
            var createDto = new CreateApplicationDTO { StudentId = 1, CourseId = 1 };
            var course = new Course { CourseId = 1, TotalSeats = 50 };
            var createdApp = new Application { ApplicationId = 1, StudentId = 1, CourseId = 1 };
            
            _mockCourseRepo.Setup(r => r.GetById(1)).ReturnsAsync(course);
            _mockAppRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Application>());
            _mockAppRepo.Setup(r => r.Add(It.IsAny<Application>())).ReturnsAsync(createdApp);

            // Act
            var result = await _service.CreateApplication(createDto);

            // Assert
            Assert.Equal(1, result.ApplicationId);
            Assert.Equal(1, result.StudentId);
            Assert.Equal(1, result.CourseId);
        }

        [Fact]
        public async Task CreateApplication_NoSeatsAvailable_ThrowsException()
        {
            // Arrange
            var createDto = new CreateApplicationDTO { StudentId = 1, CourseId = 1 };
            var course = new Course { CourseId = 1, TotalSeats = 1 };
            var applications = new List<Application>
            {
                new Application { CourseId = 1, Status = ApplicationStatus.Approved }
            };
            
            _mockCourseRepo.Setup(r => r.GetById(1)).ReturnsAsync(course);
            _mockAppRepo.Setup(r => r.GetAll()).ReturnsAsync(applications);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.CreateApplication(createDto));
        }

        [Fact]
        public async Task UpdateApplicationStatus_ValidData_UpdatesSuccessfully()
        {
            // Arrange
            var application = new Application { ApplicationId = 1, StudentId = 1, CourseId = 1 };
            var course = new Course { CourseId = 1, CourseName = "Computer Science", TotalSeats = 50 };
            var student = new Student { StudentId = 1, User = new User { Name = "John", Email = "john@test.com" } };
            
            _mockAppRepo.Setup(r => r.GetById(1)).ReturnsAsync(application);
            _mockCourseRepo.Setup(r => r.GetById(1)).ReturnsAsync(course);
            _mockStudentRepo.Setup(r => r.GetById(1)).ReturnsAsync(student);
            _mockAppRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Application>());

            // Act
            await _service.UpdateApplicationStatus(1, ApplicationStatus.Approved, 1);

            // Assert
            _mockApp.Verify(a => a.Update(It.IsAny<Application>()), Times.Once);
        }
    }
}