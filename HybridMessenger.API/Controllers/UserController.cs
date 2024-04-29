using HybridMessenger.Application.User.Commands;
using HybridMessenger.Application.User.Queries;
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

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("get-by-id")]
        public async Task<ActionResult> GetUser([FromQuery] GetUserByIdQuery query)
        {
            var userResult = await _mediator.Send(query);

            return Ok(userResult);
        }

        [Authorize]
        [HttpPost("get-paged")]
        public async Task<ActionResult> GetPagedUsers([FromBody] GetPagedUsersQuery query)
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] VerifyByEmailPasswordCommand command)
        {
            var token = await _mediator.Send(command);

            return Ok(new { 
                AccessToken = token.Item1, 
                RefreshToken = token.Item2 
            });
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}
