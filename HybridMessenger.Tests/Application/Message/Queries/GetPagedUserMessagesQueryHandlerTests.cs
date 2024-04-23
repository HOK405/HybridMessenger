using HybridMessenger.Application.Message.DTOs;
using HybridMessenger.Application.Message.Queries;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.Services;
using HybridMessenger.Domain.UnitOfWork;
using Moq;

namespace HybridMessenger.Tests.Application.Message.Queries
{
    public class GetPagedUserMessagesQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMessageRepository> _mockMessageRepository;
        private readonly Mock<IDynamicProjectionService> _mockDynamicProjectionService;
        private readonly GetPagedUserMessagesQueryHandler _handler;

        public GetPagedUserMessagesQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMessageRepository = new Mock<IMessageRepository>();
            _mockDynamicProjectionService = new Mock<IDynamicProjectionService>();

            _mockUnitOfWork.Setup(u => u.GetRepository<IMessageRepository>()).Returns(_mockMessageRepository.Object);

            _handler = new GetPagedUserMessagesQueryHandler(_mockUnitOfWork.Object, _mockDynamicProjectionService.Object);
        }

        [Fact]
        public async Task Handle_ReturnsCorrectlyProjectedMessages()
        {
            // Arrange
            var request = new GetPagedUserMessagesQuery
            {
                UserId = 1,
                PageNumber = 1,
                PageSize = 10,
                SortBy = "SentAt",
                Fields = ["MessageText", "SentAt"]
            };
            var messages = new List<Domain.Entities.Message>
            {
                new Domain.Entities.Message { MessageId = 1, MessageText = "Hello", UserId = 1, SentAt = DateTime.UtcNow }
            };
            var projectedMessages = new List<object>
            {
                new { MessageText = "Hello", SentAt = DateTime.UtcNow }
            };

            _mockMessageRepository.Setup(repo => repo.GetPagedAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
                .ReturnsAsync(messages.AsQueryable());

            _mockDynamicProjectionService.Setup(service => service.ProjectToDynamic<Domain.Entities.Message, MessageDto>(
                It.IsAny<List<Domain.Entities.Message>>(),
                It.IsAny<IEnumerable<string>>()))
                .Returns(projectedMessages);

            // Act
            var result = await _handler.Handle(request, new CancellationToken());

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(projectedMessages, result);
            _mockMessageRepository.Verify(repo => repo.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                request.SortBy,
                It.IsAny<Dictionary<string, object>>(),
                request.SearchValue,
                request.Ascending), Times.Once);
            _mockDynamicProjectionService.Verify(service => service.ProjectToDynamic<Domain.Entities.Message, MessageDto>(
                It.IsAny<List<Domain.Entities.Message>>(),
                It.IsAny<IEnumerable<string>>()), Times.Once);
        }
    }
}
