using MediatR;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("get-messages")]
        public async Task<ActionResult> GetUserMessages(GetUserMessagesQuery query)
        {

        }
    }
}
