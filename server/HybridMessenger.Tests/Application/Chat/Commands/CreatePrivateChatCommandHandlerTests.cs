using AutoMapper;
using HybridMessenger.Application.Chat.Commands;
using HybridMessenger.Application.Chat.DTOs;
using HybridMessenger.Domain.Entities;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using Moq;

namespace HybridMessenger.Tests.Application.Chat.Commands
{
    public class CreatePrivateChatCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IChatRepository> _mockChatRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IChatMemberRepository> _mockChatMemberRepository;
        private readonly CreatePrivateChatCommandHandler _handler;

        public CreatePrivateChatCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockChatRepository = new Mock<IChatRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockChatMemberRepository = new Mock<IChatMemberRepository>();

            _mockUnitOfWork.Setup(u => u.GetRepository<IChatRepository>()).Returns(_mockChatRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IUserRepository>()).Returns(_mockUserRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IChatMemberRepository>()).Returns(_mockChatMemberRepository.Object);

            _handler = new CreatePrivateChatCommandHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesPrivateChatAndAddsUsers()
        {
            // Arrange
            var command = new CreatePrivateChatCommand { UserCreatorId = 1, UserNameToCreateWith = "user2" };
            var chatMember1 = new ChatMember { UserId = 1, ChatId = 1, JoinedAt = DateTime.UtcNow };
            var chatMember2 = new ChatMember { UserId = 2, ChatId = 1, JoinedAt = DateTime.UtcNow };
            var userCreator = new Domain.Entities.User { Id = 1 };
            var userToCreateWith = new Domain.Entities.User { Id = 2, UserName = "user2" };
            var chat = new Domain.Entities.Chat { ChatId = 1 };
            var chatDto = new ChatDto { ChatId = 1 };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(command.UserCreatorId)).ReturnsAsync(userCreator);
            _mockUserRepository.Setup(repo => repo.GetUserByUsernameAsync(command.UserNameToCreateWith)).ReturnsAsync(userToCreateWith);
            _mockChatRepository.Setup(repo => repo.CreateChatAsync(null, false)).ReturnsAsync(chat);
            _mockChatMemberRepository.Setup(repo => repo.AddUserToChatAsync(userCreator, chat)).ReturnsAsync(chatMember1);
            _mockChatMemberRepository.Setup(repo => repo.AddUserToChatAsync(userToCreateWith, chat)).ReturnsAsync(chatMember2);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _mockMapper.Setup(mapper => mapper.Map<ChatDto>(chat)).Returns(chatDto);

            // Act
            var result = await _handler.Handle(command, new CancellationToken());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(chatDto.ChatId, result.ChatId);
            _mockChatRepository.Verify(repo => repo.CreateChatAsync(null, false), Times.Once);
            _mockChatMemberRepository.Verify(repo => repo.AddUserToChatAsync(userCreator, chat), Times.Once);
            _mockChatMemberRepository.Verify(repo => repo.AddUserToChatAsync(userToCreateWith, chat), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
