using HybridMessenger.Application.User.DTOs;
using HybridMessenger.Application.User.Queries;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.Services;
using HybridMessenger.Domain.UnitOfWork;
using Moq;

namespace HybridMessenger.Tests.Application.User.Queries
{
    public class GetPagedUsersQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IDynamicProjectionService> _mockDynamicProjectionService;
        private readonly GetPagedUsersQueryHandler _handler;

        public GetPagedUsersQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockDynamicProjectionService = new Mock<IDynamicProjectionService>();

            _mockUnitOfWork.Setup(u => u.GetRepository<IUserRepository>()).Returns(_mockUserRepository.Object);

            _handler = new GetPagedUsersQueryHandler(_mockUnitOfWork.Object, _mockDynamicProjectionService.Object);
        }

        [Fact]
        public async Task Handle_ReturnsCorrectlyProjectedUsers()
        {
            // Arrange
            var request = new GetPagedUsersQuery
            {
                PageNumber = 1,
                PageSize = 10,
                SortBy = "UserName",
                Ascending = true,
                Fields = new string[] { "UserName", "Email" }
            };
            var users = new List<Domain.Entities.User>
            {
                new Domain.Entities.User { UserName = "JohnDoe", Email = "john@example.com" }
            };
            var projectedUsers = new List<object>
            {
                new { UserName = "JohnDoe", Email = "john@example.com" }
            };

            _mockUserRepository.Setup(repo => repo.GetPagedAsync(request.PageNumber, request.PageSize, request.SortBy, null, null, request.Ascending))
                               .ReturnsAsync(users.AsQueryable());
            _mockDynamicProjectionService.Setup(service => service.ProjectToDynamic<Domain.Entities.User, UserDto>(It.IsAny<List<Domain.Entities.User>>(), It.IsAny<IEnumerable<string>>()))
                                         .Returns(projectedUsers);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(projectedUsers, result);
            _mockUserRepository.Verify(repo => repo.GetPagedAsync(request.PageNumber, request.PageSize, request.SortBy, null, null, request.Ascending), Times.Once);
            _mockDynamicProjectionService.Verify(service => service.ProjectToDynamic<Domain.Entities.User, UserDto>(It.IsAny<List<Domain.Entities.User>>(), It.IsAny<IEnumerable<string>>()), Times.Once);
        }

    }
}
