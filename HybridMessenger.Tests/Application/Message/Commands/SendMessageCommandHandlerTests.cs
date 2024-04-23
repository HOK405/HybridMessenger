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
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMessageRepository> _mockMessageRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly SendMessageCommandHandler _handler;

        public SendMessageCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMessageRepository = new Mock<IMessageRepository>();
            _mockMapper = new Mock<IMapper>();

            _mockUnitOfWork.Setup(u => u.GetRepository<IMessageRepository>()).Returns(_mockMessageRepository.Object);

            _handler = new SendMessageCommandHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsMessageDto()
        {
            // Arrange
            var command = new SendMessageCommand { MessageText = "Hello, World!", ChatId = 1, UserId = 1 };
            var message = new Domain.Entities.Message
            {
                MessageText = command.MessageText,
                ChatId = command.ChatId,
                UserId = command.UserId,
                SentAt = DateTime.UtcNow
            };
            var messageDto = new MessageDto { MessageText = message.MessageText, ChatId = message.ChatId, UserId = message.UserId };

            _mockMessageRepository.Setup(repo => repo.AddAsync(It.IsAny<Domain.Entities.Message>()))
                                  .ReturnsAsync(message);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _mockMapper.Setup(mapper => mapper.Map<MessageDto>(It.IsAny<Domain.Entities.Message>()))
                       .Returns(messageDto);

            // Act
            var result = await _handler.Handle(command, new CancellationToken());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(command.MessageText, result.MessageText);
            Assert.Equal(command.ChatId, result.ChatId);
            Assert.Equal(command.UserId, result.UserId);
            _mockMessageRepository.Verify(repo => repo.AddAsync(It.IsAny<Domain.Entities.Message>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
