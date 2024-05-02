using HybridMessenger.Application.Message.Commands;
using HybridMessenger.Application.Message.DTOs;
using HybridMessenger.Domain.Services;
using HybridMessenger.Infrastructure.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Moq;
using System.Security.Claims;

namespace HybridMessenger.Tests.Infrastructure.Hubs
{
    public class ChatHubTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<IUserClaimsService> _mockUserClaimsService;
        private readonly Mock<IClientProxy> _mockClientProxy;
        private readonly Mock<IGroupManager> _mockGroups;
        private readonly ChatHub _hub;

        public ChatHubTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockUserClaimsService = new Mock<IUserClaimsService>();
            _mockClientProxy = new Mock<IClientProxy>();
            _mockGroups = new Mock<IGroupManager>();  

            _hub = new ChatHub(_mockMediator.Object, _mockUserClaimsService.Object);

            var mockClients = new Mock<IHubCallerClients>();
            mockClients.Setup(clients => clients.Group(It.IsAny<string>())).Returns(_mockClientProxy.Object);
            _hub.Clients = mockClients.Object;

            _hub.Groups = _mockGroups.Object;  

            var mockContext = new Mock<HubCallerContext>();
            mockContext.Setup(context => context.ConnectionId).Returns("connection-id");
            _hub.Context = mockContext.Object;
        }


        [Fact]
        public async Task JoinChat_AddsConnectionToGroup()
        {
            // Arrange
            int groupId = 1;
            var groupName = groupId.ToString();

            // Act
            await _hub.JoinChat(groupId);

            // Assert
            _mockGroups.Verify(groups => groups.AddToGroupAsync("connection-id", groupName, It.IsAny<CancellationToken>()), Times.Once());
        }


        [Fact]
        public async Task LeaveChat_RemovesConnectionFromGroup()
        {
            // Arrange
            int groupId = 1;
            var groupName = groupId.ToString();

            // Act
            await _hub.LeaveChat(groupId);

            // Assert
            _mockGroups.Verify(groups => groups.RemoveFromGroupAsync("connection-id", groupName, It.IsAny<CancellationToken>()), Times.Once());
        }


        [Fact]
        public async Task SendMessage_CallsMediatorAndSendsMessage()
        {
            // Arrange
            int groupId = 1;
            string messageText = "Hello, world!";
            int userId = 1;
            var messageDto = new MessageDto();

            _mockUserClaimsService.Setup(service => service.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            _mockMediator.Setup(mediator => mediator.Send(It.IsAny<SendMessageCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(messageDto);

            // Act
            await _hub.SendMessage(groupId, messageText);

            // Assert
            _mockMediator.Verify(mediator => mediator.Send(It.Is<SendMessageCommand>(cmd => cmd.ChatId == groupId && cmd.MessageText == messageText && cmd.UserId == userId), It.IsAny<CancellationToken>()), Times.Once());
            _mockClientProxy.Verify(client => client.SendCoreAsync("ReceiveMessage", It.Is<object[]>(o => o.Contains(messageDto)), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
