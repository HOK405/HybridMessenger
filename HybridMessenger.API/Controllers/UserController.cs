using HybridMessenger.Application.User.Commands;
using HybridMessenger.Application.User.Queries;
using HybridMessenger.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

            /*...*/

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(CreateUserCommand command)
        {
            try
            {
                var user = await _mediator.Send(command);
                return Ok(user);
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
            var user = await _mediator.Send(command); 

            if (user != null)
            {
                var token = GenerateJwtToken(user); 
                return Ok(new { Token = token }); 
            }
            else
            {
                return Unauthorized();
            }
        }


        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("MyVeryVeryVeryLongSecretAndItShouldBePlacedSomewhereInSafePlace"); // store somewhere in secure place
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    [
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Email, user.Email)
                    ]),
                Expires = DateTime.UtcNow.AddDays(7), // Token expiration
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
