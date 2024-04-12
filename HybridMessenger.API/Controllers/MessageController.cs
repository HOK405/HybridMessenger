using HybridMessenger.Application.Message.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HybridMessenger.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private readonly IMediator _mediator;

        public MessageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost("get-paged-messages")]
        public async Task<ActionResult> GetUserMessages([FromBody] GetPagedUserMessagesQuery query)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            query.UserId = userId;

            if (!string.IsNullOrEmpty(userId))
            {
                var result = await _mediator.Send(query); 
                return Ok(result);
            }
            else
            {
                return Unauthorized("UserId claim is missing in the token.");
            }
        }
    }
}
