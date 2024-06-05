using AutoMapper;
using HybridMessenger.Application.Chat.Commands;
using HybridMessenger.Application.Chat.DTOs;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using Moq;

namespace HybridMessenger.Tests.Application.Chat.Commands
{
    public class ChangeGroupNameCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IChatRepository> _mockChatRepository;
        private readonly Mock<IChatMemberRepository> _mockChatMemberRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ChangeGroupNameCommandHandler _handler;

        public ChangeGroupNameCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockChatRepository = new Mock<IChatRepository>();
            _mockChatMemberRepository = new Mock<IChatMemberRepository>();
            _mockMapper = new Mock<IMapper>();

            _mockUnitOfWork.Setup(u => u.GetRepository<IChatRepository>()).Returns(_mockChatRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IChatMemberRepository>()).Returns(_mockChatMemberRepository.Object);

            _handler = new ChangeGroupNameCommandHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_UserIsInChat_UpdatesChatNameAndReturnsDto()
        {
            // Arrange
            var command = new ChangeGroupNameCommand { UserId = 1, ChatId = 1, NewChatName = "New Group Name" };
            var chat = new Domain.Entities.Chat { ChatId = 1, ChatName = "Old Group Name", IsGroup = true, CreatedAt = DateTime.UtcNow };
            var chatDto = new ChatDto { ChatId = 1, ChatName = "New Group Name", IsGroup = true, CreatedAt = DateTime.UtcNow };

            _mockChatMemberRepository.Setup(repo => repo.IsUserMemberOfChatAsync(command.UserId, command.ChatId))
                                     .ReturnsAsync(true);
            _mockChatRepository.Setup(repo => repo.GetByIdAsync(command.ChatId))
                               .ReturnsAsync(chat);
            _mockMapper.Setup(m => m.Map<ChatDto>(It.IsAny<Domain.Entities.Chat>()))
                       .Returns(chatDto);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
                           .ReturnsAsync(1); 

            // Act
            var result = await _handler.Handle(command, new CancellationToken());

            // Assert
            Assert.Equal("New Group Name", chat.ChatName);
            Assert.Equal("New Group Name", result.ChatName);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once); 
            _mockChatRepository.Verify(repo => repo.GetByIdAsync(command.ChatId), Times.Once); 
        }

        [Fact]
        public async Task Handle_UserNotInChat_ThrowsInvalidOperationException()
        {
            // Arrange
            var command = new ChangeGroupNameCommand { UserId = 1, ChatId = 1, NewChatName = "New Group Name" };

            // Setup the repository to return false, indicating the user is not in the chat
            _mockChatMemberRepository.Setup(repo => repo.IsUserMemberOfChatAsync(command.UserId, command.ChatId))
                                     .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, new CancellationToken()));

            // Check the exception message
            Assert.Equal("User is not a member of the specified chat.", exception.Message);
        }
    }
}
