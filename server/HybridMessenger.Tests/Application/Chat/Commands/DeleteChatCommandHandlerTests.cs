using HybridMessenger.Application.Chat.Commands;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using Moq;

namespace HybridMessenger.Tests.Application.Chat.Commands
{
    public class DeleteChatCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IChatRepository> _mockChatRepository;
        private readonly DeleteChatCommandHandler _handler;

        public DeleteChatCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockChatRepository = new Mock<IChatRepository>();
            _mockUnitOfWork.Setup(u => u.GetRepository<IChatRepository>()).Returns(_mockChatRepository.Object);

            _handler = new DeleteChatCommandHandler(_mockUnitOfWork.Object);
        }


        [Fact]
        public async Task Handle_ChatExists_DeletesChatAndSavesChanges()
        {
            // Arrange
            var command = new DeleteChatCommand { ChatId = 1 };
            var chat = new Domain.Entities.Chat { ChatId = 1 };

            _mockChatRepository.Setup(repo => repo.GetByIdAsync(command.ChatId)).ReturnsAsync(chat);
            _mockChatRepository.Setup(repo => repo.Remove(chat)).Verifiable();
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _handler.Handle(command, new CancellationToken());

            // Assert
            _mockChatRepository.Verify(repo => repo.GetByIdAsync(command.ChatId), Times.Once);
            _mockChatRepository.Verify(repo => repo.Remove(chat), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
