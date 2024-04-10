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
        [HttpGet("get-user")]
        public async Task<ActionResult> GetUser([FromQuery] GetUserByIdQuery query)
        {
            var userResult = await _mediator.Send(query);

            return Ok(userResult);
        }

        [HttpGet("get-email")]
        public async Task<ActionResult> GetEmailByUsername([FromQuery] GetEmailByUsernameQuery query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpPost("get-paged-users")]
        public async Task<ActionResult> GetPagedUsers([FromBody] GetPagedUsersQuery query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(new { AccessToken = result.Item1, RefreshToken = result.Item2 }); 
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

            if (!string.IsNullOrEmpty(token.Item1) && !string.IsNullOrEmpty(token.Item2))
            {
                return Ok(new { AccessToken = token.Item1, RefreshToken = token.Item2 });
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /*[HttpPost("create-role")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Role name is required.");
            }

            // Check if the role already exists
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (roleExists)
            {
                return BadRequest("Role already exists.");
            }

            // Create the new role
            var result = await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            if (result.Succeeded)
            {
                return Ok($"Role {roleName} created successfully.");
            }

            // If we got this far, something failed
            return BadRequest(result.Errors);
        }*/
    }
}
