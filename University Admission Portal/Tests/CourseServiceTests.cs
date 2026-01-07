using Moq;
using University_Admission_Portal.DTOs;
using University_Admission_Portal.Interface;
using University_Admission_Portal.Models;
using University_Admission_Portal.Service;
using University_Admission_Portal.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace University_Admission_Portal.Tests
{
    public class CourseServiceTests
    {
        private readonly Mock<IUniversity<Course, int>> _mockCourseRepo;
        private readonly Mock<IUniversity<Application, int>> _mockApplicationRepo;
        private readonly Mock<UnivContext> _mockContext;
        private readonly CourseService _service;

        public CourseServiceTests()
        {
            _mockCourseRepo = new Mock<IUniversity<Course, int>>();
            _mockApplicationRepo = new Mock<IUniversity<Application, int>>();
            _mockContext = new Mock<UnivContext>();
            _service = new CourseService(_mockCourseRepo.Object, _mockApplicationRepo.Object, _mockContext.Object);
        }

        [Fact]
        public async Task GetAllCourses_ReturnsCoursesWithRemainingSeats()
        {
            // Arrange
            var courses = new List<Course>
            {
                new Course { CourseId = 1, CourseName = "Computer Science", TotalSeats = 50 }
            };
            var applications = new List<Application>
            {
                new Application { CourseId = 1, Status = ApplicationStatus.Approved }
            };
            _mockCourseRepo.Setup(r => r.GetAll()).ReturnsAsync(courses);
            _mockApplicationRepo.Setup(r => r.GetAll()).ReturnsAsync(applications);

            // Act
            var result = await _service.GetAllCourses();

            // Assert
            var courseDto = result.First();
            Assert.Equal(49, courseDto.RemainingSeats);
            Assert.Equal("Computer Science", courseDto.CourseName);
        }

        [Fact]
        public async Task GetCourseById_ExistingId_ReturnsCourseWithRemainingSeats()
        {
            // Arrange
            var course = new Course { CourseId = 1, CourseName = "Computer Science", TotalSeats = 50 };
            var applications = new List<Application>
            {
                new Application { CourseId = 1, Status = ApplicationStatus.Approved }
            };
            _mockCourseRepo.Setup(r => r.GetById(1)).ReturnsAsync(course);
            _mockApplicationRepo.Setup(r => r.GetAll()).ReturnsAsync(applications);

            // Act
            var result = await _service.GetCourseById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(49, result.RemainingSeats);
            Assert.Equal("Computer Science", result.CourseName);
        }

        [Fact]
        public async Task GetCourseById_NonExistingId_ReturnsNull()
        {
            // Arrange
            _mockCourseRepo.Setup(r => r.GetById(999)).ReturnsAsync((Course)null);

            // Act
            var result = await _service.GetCourseById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateCourse_ValidData_ReturnsCreatedCourse()
        {
            // Arrange
            var createDto = new CreateCourseDTO { CourseCode = "CS101", CourseName = "New Course", TotalSeats = 40 };
            var createdCourse = new Course { CourseId = 1, CourseCode = "CS101", CourseName = "New Course", TotalSeats = 40 };
            
            var mockCourses = new Mock<DbSet<Course>>();
            _mockContext.Setup(c => c.Courses).Returns(mockCourses.Object);
            _mockCourseRepo.Setup(r => r.Add(It.IsAny<Course>())).ReturnsAsync(createdCourse);

            // Act
            var result = await _service.CreateCourse(createDto);

            // Assert
            Assert.Equal(1, result.CourseId);
            Assert.Equal("CS101", result.CourseCode);
            Assert.Equal("New Course", result.CourseName);
            Assert.Equal(40, result.TotalSeats);
        }

        [Fact]
        public async Task CreateCourse_DuplicateCourseCode_ThrowsException()
        {
            // Arrange
            var createDto = new CreateCourseDTO { CourseCode = "CS101", CourseName = "New Course" };
            
            var mockCourses = new Mock<DbSet<Course>>();
            _mockContext.Setup(c => c.Courses).Returns(mockCourses.Object);
            _mockContext.Setup(c => c.Courses.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Course, bool>>>(), default))
                      .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateCourse(createDto));
            Assert.Equal("This Course Code is already registered", exception.Message);
        }

        [Fact]
        public async Task CreateCourse_DuplicateCourseName_ThrowsException()
        {
            // Arrange
            var createDto = new CreateCourseDTO { CourseCode = "CS102", CourseName = "Existing Course" };
            
            var mockCourses = new Mock<DbSet<Course>>();
            _mockContext.Setup(c => c.Courses).Returns(mockCourses.Object);
            _mockContext.SetupSequence(c => c.Courses.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Course, bool>>>(), default))
                      .ReturnsAsync(false)  // Course code check
                      .ReturnsAsync(true);   // Course name check

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateCourse(createDto));
            Assert.Equal("This Course Name is already registered", exception.Message);
        }
    }
}