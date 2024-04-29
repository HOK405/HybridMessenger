using HybridMessenger.Application.Chat.Commands;
using HybridMessenger.Domain.Entities;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using Moq;

namespace HybridMessenger.Tests.Application.Chat.Commands
{
    public class AddGroupMemberCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IChatRepository> _mockChatRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IChatMemberRepository> _mockChatMemberRepository;
        private readonly AddGroupMemberCommandHandler _handler;

        public AddGroupMemberCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockChatRepository = new Mock<IChatRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockChatMemberRepository = new Mock<IChatMemberRepository>();

            _mockUnitOfWork.Setup(u => u.GetRepository<IChatRepository>()).Returns(_mockChatRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IUserRepository>()).Returns(_mockUserRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IChatMemberRepository>()).Returns(_mockChatMemberRepository.Object);

            _handler = new AddGroupMemberCommandHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_UserAndChatExist_UserAddedToChat()
        {
            // Arrange
            var username = "testUser";
            var chatId = 1;
            var userId = 1;
            var command = new AddGroupMemberCommand { UserNameToAdd = username, ChatId = chatId, UserId = userId };
            var user = new Domain.Entities.User { Id = userId, UserName = username };
            var chat = new Domain.Entities.Chat { ChatId = chatId, ChatName = "chatName", IsGroup = true, CreatedAt = DateTime.UtcNow };
            var chatMember = new ChatMember { UserId = user.Id, ChatId = chat.ChatId };

            _mockChatMemberRepository.Setup(repo => repo.IsUserMemberOfChatAsync(userId, chatId))
                               .ReturnsAsync(true);
            _mockUserRepository.Setup(repo => repo.GetUserByUsernameAsync(username))
                               .ReturnsAsync(user);
            _mockChatRepository.Setup(repo => repo.GetByIdAsync(chatId))
                               .ReturnsAsync(chat);
            _mockChatMemberRepository.Setup(repo => repo.AddUserToChatAsync(user, chat))
                                     .ReturnsAsync(chatMember); 

            // Act
            await _handler.Handle(command, new CancellationToken());

            // Assert
            _mockChatMemberRepository.Verify(repo => repo.AddUserToChatAsync(user, chat), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }


        [Fact]
        public async Task Handle_UserNotInChat_ThrowsInvalidOperationException()
        {
            // Arrange
            var username = "testUser";
            var chatId = 1;
            var userId = 1;
            var command = new AddGroupMemberCommand { UserNameToAdd = username, ChatId = chatId, UserId = userId };

            _mockChatMemberRepository.Setup(repo => repo.IsUserMemberOfChatAsync(userId, chatId))
                                     .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, new CancellationToken()));
            _mockChatMemberRepository.Verify(repo => repo.IsUserMemberOfChatAsync(userId, chatId), Times.Once);
        }
    }
}
