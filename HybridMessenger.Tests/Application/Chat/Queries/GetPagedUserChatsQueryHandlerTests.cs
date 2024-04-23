using HybridMessenger.Application.Chat.DTOs;
using HybridMessenger.Application.Chat.Queries;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.Services;
using HybridMessenger.Domain.UnitOfWork;
using Moq;

namespace HybridMessenger.Tests.Application.Chat.Queries
{
    public class GetPagedUserChatsQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IDynamicProjectionService> _mockDynamicProjectionService;
        private readonly Mock<IChatRepository> _mockChatRepository;
        private readonly GetPagedUserChatsQueryHandler _handler;

        public GetPagedUserChatsQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockDynamicProjectionService = new Mock<IDynamicProjectionService>();
            _mockChatRepository = new Mock<IChatRepository>();

            _mockUnitOfWork.Setup(uow => uow.GetRepository<IChatRepository>()).Returns(_mockChatRepository.Object);

            _handler = new GetPagedUserChatsQueryHandler(_mockUnitOfWork.Object, _mockDynamicProjectionService.Object);
        }

        [Fact]
        public async Task Handle_DefaultFields_ReturnsProjectedChats()
        {
            // Arrange
            var request = new GetPagedUserChatsQuery { UserId = 1, PageNumber = 1, PageSize = 10 };
            var fakeChats = new List<Domain.Entities.Chat> { new Domain.Entities.Chat(), new Domain.Entities.Chat() };
            var projectedChats = new List<object> { new { Id = 1 }, new { Id = 2 } };

            _mockChatRepository.Setup(repo => repo.GetPagedUserChatsAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(fakeChats.AsQueryable());

            _mockDynamicProjectionService.Setup(svc => svc.ProjectToDynamic<Domain.Entities.Chat, ChatDto>(
                It.IsAny<List<Domain.Entities.Chat>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(projectedChats);

            // Act
            var result = await _handler.Handle(request, new CancellationToken());

            // Assert
            Assert.Equal(2, result.Count());
            _mockDynamicProjectionService.Verify(svc => svc.ProjectToDynamic<Domain.Entities.Chat, ChatDto>(
                It.IsAny<List<Domain.Entities.Chat>>(), It.IsAny<IEnumerable<string>>()), Times.Once);
        }
    }
}
