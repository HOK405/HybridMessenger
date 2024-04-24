using AutoMapper;
using HybridMessenger.Application.User.DTOs;
using HybridMessenger.Application.User.Queries;
using HybridMessenger.Domain.Services;
using Moq;

namespace HybridMessenger.Tests.Application.User.Queries
{
    public class GetUserByIdQueryHandlerTests
    {
        private readonly Mock<IUserIdentityService> _mockUserService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetUserByIdQueryHandler _handler;

        public GetUserByIdQueryHandlerTests()
        {
            _mockUserService = new Mock<IUserIdentityService>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetUserByIdQueryHandler(_mockUserService.Object, _mockMapper.Object);
        }


        [Fact]
        public async Task Handle_ValidQuery_ReturnsMappedUserDto()
        {
            // Arrange
            var userId = 1;
            var user = new Domain.Entities.User { Id = userId, UserName = "JohnDoe", Email = "johndoe@example.com" };
            var userDto = new UserDto { Id = userId, UserName = "JohnDoe", Email = "johndoe@example.com" };
            var query = new GetUserByIdQuery { Id = userId };

            _mockUserService.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userDto.UserName, result.UserName);
            Assert.Equal(userDto.Email, result.Email);
            _mockUserService.Verify(x => x.GetUserByIdAsync(userId), Times.Once);
            _mockMapper.Verify(m => m.Map<UserDto>(user), Times.Once);
        }
    }
}
