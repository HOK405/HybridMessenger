using HybridMessenger.Application.User.Commands;
using HybridMessenger.Application.User.Queries;
using HybridMessenger.Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HybridMessenger.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IUserClaimsService _userClaimsService;

        public UserController(IMediator mediator, IUserClaimsService userClaimsService)
        {
            _mediator = mediator;
            _userClaimsService = userClaimsService;
        }

        [Authorize]
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetUser([FromQuery] GetUserByIdQuery query)
        {
            var userResult = await _mediator.Send(query);

            return Ok(userResult);
        }

        [HttpPost("get-paged")]
        public async Task<IActionResult> GetPagedUsers([FromBody] GetPagedUsersQuery query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(new { 
                AccessToken = result.Item1, 
                RefreshToken = result.Item2 
            }); 
        }

        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserCommand command)
        {
            command.UserId = _userClaimsService.GetUserId(User);

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("upload-avatar")]
        public async Task<IActionResult> UploadAvatar([FromForm] IFormFile file)
        {
            var userId = _userClaimsService.GetUserId(User);
            var command = new UploadAvatarCommand { UserId = userId, File = file };
            var result = await _mediator.Send(command);
            return Ok(new { Message = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] VerifyByEmailPasswordCommand command)
        {
            var token = await _mediator.Send(command);

            return Ok(new { 
                AccessToken = token.Item1, 
                RefreshToken = token.Item2 
            });
        }

        [Authorize]
        [HttpPost("delete-by-id")]
        public async Task<IActionResult> Delete([FromBody] DeleteUserCommand command)
        {
            await _mediator.Send(command);

            return Ok(new { Message = "User is successfully deleted." });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(new { AccessToken = result });
        }
    }
}
