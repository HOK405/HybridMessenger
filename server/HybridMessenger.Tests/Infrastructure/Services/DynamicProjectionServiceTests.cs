using AutoMapper;
using HybridMessenger.Application.User.DTOs;
using HybridMessenger.Domain.Entities;
using HybridMessenger.Infrastructure.Services;
using Moq;

namespace HybridMessenger.Tests.Infrastructure.Services
{
    public class DynamicProjectionServiceTests
    {
        private readonly Mock<IMapper> _mockMapper;
        private readonly DynamicProjectionService _service;

        public DynamicProjectionServiceTests()
        {
            _mockMapper = new Mock<IMapper>();
            _service = new DynamicProjectionService(_mockMapper.Object);
        }

        [Fact]
        public void ProjectToDynamic_MapPropertiesCorrect_ShouldHave2ElementsInList()
        {
            // Arrange
            var entities = new List<User>
            {
                new User { Id = 1, UserName = "JohnDoe", Email = "john@example.com" },
                new User { Id = 2, UserName = "JaneDoe", Email = "jane@example.com" }
            };
            var userDtos = new List<UserDto>
            {
                new UserDto { Id = 1, UserName = "JohnDoe", Email = "john@example.com" },
                new UserDto { Id = 2, UserName = "JaneDoe", Email = "jane@example.com" }
            };
            var fieldsToInclude = new List<string> { "UserName", "Email" };

            _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
                       .Returns((User src) => userDtos.FirstOrDefault(dto => dto.Id == src.Id));

            // Act
            var result = _service.ProjectToDynamic<User, UserDto>(entities, fieldsToInclude).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(userDtos[0].UserName, ((IDictionary<string, object>)result[0])["UserName"]);
            Assert.Equal(userDtos[0].Email, ((IDictionary<string, object>)result[0])["Email"]);
            Assert.Equal(userDtos[1].UserName, ((IDictionary<string, object>)result[1])["UserName"]);
            Assert.Equal(userDtos[1].Email, ((IDictionary<string, object>)result[1])["Email"]);
        }
    }
}
