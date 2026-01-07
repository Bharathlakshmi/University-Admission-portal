using Microsoft.AspNetCore.Mvc;
using Moq;
using University_Admission_Portal.Controllers;
using University_Admission_Portal.DTOs;
using University_Admission_Portal.Service;
using Xunit;

namespace University_Admission_Portal.Tests
{
    public class CourseControllerTests
    {
        private readonly Mock<ICourseService> _mockCourseService;
        private readonly CourseController _controller;

        public CourseControllerTests()
        {
            _mockCourseService = new Mock<ICourseService>();
            _controller = new CourseController(_mockCourseService.Object);
        }

        [Fact]
        public async Task GetAllCourses_ReturnsOkResult_WithCourseList()
        {
            // Arrange
            var courses = new List<GetCourseDTO>
            {
                new GetCourseDTO { CourseId = 1, CourseName = "Computer Science", TotalSeats = 50, RemainingSeats = 30 }
            };
            _mockCourseService.Setup(s => s.GetAllCourses()).ReturnsAsync(courses);

            // Act
            var result = await _controller.GetAllCourses();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(courses, okResult.Value);
        }

        [Fact]
        public async Task GetCourseById_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var course = new GetCourseDTO { CourseId = 1, CourseName = "Computer Science" };
            _mockCourseService.Setup(s => s.GetCourseById(1)).ReturnsAsync(course);

            // Act
            var result = await _controller.GetCourseById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(course, okResult.Value);
        }

        [Fact]
        public async Task GetCourseById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _mockCourseService.Setup(s => s.GetCourseById(999)).ReturnsAsync((GetCourseDTO)null);

            // Act
            var result = await _controller.GetCourseById(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateCourse_ValidData_ReturnsOkResult()
        {
            // Arrange
            var createDto = new CreateCourseDTO { CourseName = "New Course", TotalSeats = 40 };
            var createdCourse = new CourseDTO { CourseId = 1, CourseName = "New Course" };
            _mockCourseService.Setup(s => s.CreateCourse(createDto)).ReturnsAsync(createdCourse);

            // Act
            var result = await _controller.CreateCourse(createDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(createdCourse, okResult.Value);
        }
    }
}