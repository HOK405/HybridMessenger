using AutoMapper;
using HybridMessenger.Application.Chat.Commands;
using HybridMessenger.Application.Chat.DTOs;
using HybridMessenger.Domain.Entities;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HybridMessenger.Tests.Application.Chat.Commands
{
    public class CreateGroupCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IChatRepository> _mockChatRepository;
        private readonly Mock<IChatMemberRepository> _mockChatMemberRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly CreateGroupCommandHandler _handler;

        public CreateGroupCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockChatRepository = new Mock<IChatRepository>();
            _mockChatMemberRepository = new Mock<IChatMemberRepository>();
            _mockUserRepository = new Mock<IUserRepository>();

            _mockUnitOfWork.Setup(u => u.GetRepository<IChatRepository>()).Returns(_mockChatRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IChatMemberRepository>()).Returns(_mockChatMemberRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IUserRepository>()).Returns(_mockUserRepository.Object);

            _handler = new CreateGroupCommandHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesGroupChatAndAddsUser()
        {
            // Arrange
            var command = new CreateGroupCommand { UserId = 1, ChatName = "New Group" };
            var chatMember = new ChatMember { ChatId = 1, UserId = 1, JoinedAt = DateTime.UtcNow };
            var user = new Domain.Entities.User { Id = 1 };
            var chat = new Domain.Entities.Chat { ChatId = 1, ChatName = "New Group", IsGroup = true };
            var chatDto = new ChatDto { ChatId = 1, ChatName = "New Group" };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(command.UserId)).ReturnsAsync(user);
            _mockChatRepository.Setup(repo => repo.CreateChatAsync(command.ChatName, true)).ReturnsAsync(chat);
            _mockChatMemberRepository.Setup(repo => repo.AddUserToChatAsync(user, chat)).ReturnsAsync(chatMember);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _mockMapper.Setup(mapper => mapper.Map<ChatDto>(chat)).Returns(chatDto);

            // Act
            var result = await _handler.Handle(command, new CancellationToken());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(chatDto.ChatName, result.ChatName);
            _mockChatRepository.Verify(repo => repo.CreateChatAsync(command.ChatName, true), Times.Once);
            _mockChatMemberRepository.Verify(repo => repo.AddUserToChatAsync(user, chat), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

    }
}
