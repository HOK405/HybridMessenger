using AutoMapper;
using HybridMessenger.Application.Message.Commands;
using HybridMessenger.Application.Message.DTOs;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using Moq;

namespace HybridMessenger.Tests.Application.Message.Commands
{
    public class SendMessageCommandHandlerTests
    {
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMessageRepository> _mockMessageRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly SendMessageCommandHandler _handler;

        public SendMessageCommandHandlerTests()
        {
            _mockMapper = new Mock<IMapper>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMessageRepository = new Mock<IMessageRepository>();
            _mockUserRepository = new Mock<IUserRepository>();

            _mockUnitOfWork.Setup(u => u.GetRepository<IMessageRepository>()).Returns(_mockMessageRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IUserRepository>()).Returns(_mockUserRepository.Object);

            _handler = new SendMessageCommandHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsCorrectResponse()
        {
            // Arrange
            var command = new SendMessageCommand { MessageText = "Hello, World!", ChatId = 1, UserId = 1 };
            var user = new Domain.Entities.User { Id = 1, UserName = "JohnDoe", CreatedAt = DateTime.UtcNow };
            var message = new Domain.Entities.Message { MessageText = command.MessageText, ChatId = command.ChatId, User = user, SentAt = DateTime.UtcNow };
            var messageDto = new MessageDto { MessageText = message.MessageText, ChatId = message.ChatId, SenderUserName = user.UserName };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(user);
            _mockMessageRepository.Setup(repo => repo.AddAsync(It.IsAny<Domain.Entities.Message>())).ReturnsAsync(message);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<MessageDto>(It.IsAny<Domain.Entities.Message>())).Returns(messageDto);

            // Act
            var result = await _handler.Handle(command, new CancellationToken());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(command.MessageText, result.MessageText);
            Assert.Equal(command.ChatId, result.ChatId);
            Assert.Equal(user.UserName, result.SenderUserName);
        }

        [Fact]
        public async Task Handle_InvalidCommand_ReturnsArgumentNullException()
        {
            // Arrange
            var command = new SendMessageCommand { MessageText = "", ChatId = 1, UserId = 0 };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(command, new CancellationToken()));
            Assert.StartsWith("User doesn't exist in database.", exception.Message);
        }
    }
}
