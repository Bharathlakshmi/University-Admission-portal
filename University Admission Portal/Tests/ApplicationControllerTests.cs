using Microsoft.AspNetCore.Mvc;
using Moq;
using University_Admission_Portal.Controllers;
using University_Admission_Portal.DTOs;
using University_Admission_Portal.Models;
using University_Admission_Portal.Service;
using Xunit;

namespace University_Admission_Portal.Tests
{
    public class ApplicationControllerTests
    {
        private readonly Mock<IApplicationService> _mockApplicationService;
        private readonly ApplicationController _controller;

        public ApplicationControllerTests()
        {
            _mockApplicationService = new Mock<IApplicationService>();
            _controller = new ApplicationController(_mockApplicationService.Object);
        }

        [Fact]
        public async Task GetAllApplications_ReturnsOkResult_WithApplicationList()
        {
            // Arrange
            var applications = new List<GetApplicationDTO>
            {
                new GetApplicationDTO { ApplicationId = 1, StudentName = "John Doe", CourseName = "Computer Science" }
            };
            _mockApplicationService.Setup(s => s.GetAllApplications()).ReturnsAsync(applications);

            // Act
            var result = await _controller.GetAllApplications();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(applications, okResult.Value);
        }

        [Fact]
        public async Task GetApplicationById_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var application = new GetApplicationDTO { ApplicationId = 1, StudentName = "John Doe" };
            _mockApplicationService.Setup(s => s.GetApplicationById(1)).ReturnsAsync(application);

            // Act
            var result = await _controller.GetApplicationById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(application, okResult.Value);
        }

        [Fact]
        public async Task GetApplicationById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _mockApplicationService.Setup(s => s.GetApplicationById(999)).ReturnsAsync((GetApplicationDTO)null);

            // Act
            var result = await _controller.GetApplicationById(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateApplicationStatus_ValidData_ReturnsOkResult()
        {
            // Arrange
            _mockApplicationService.Setup(s => s.UpdateApplicationStatus(1, ApplicationStatus.Approved, 1))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateApplicationStatus(1, ApplicationStatus.Approved, 1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Application status updated successfully", okResult.Value);
        }
    }
}