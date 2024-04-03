using HybridMessenger.Application.User.Commands;
using HybridMessenger.Application.User.Queries;
using MediatR;
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

        [HttpGet("get-user")]
        public async Task<ActionResult> GetUser([FromQuery] GetUserByIdQuery command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(new { Token = result }); 
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] VerifyByEmailPasswordCommand command)
        {
            var token = await _mediator.Send(command);

            if (!string.IsNullOrEmpty(token))
            {
                return Ok(new { Token = token });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
