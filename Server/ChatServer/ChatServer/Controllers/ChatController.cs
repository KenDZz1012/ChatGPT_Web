using ChatServer.Models;
using ChatServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace ChatServer.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatRepository _repository;
        public ChatController(IChatRepository repository)
        {
            _repository = repository;
        }
        [HttpPost]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<string>> CreateDiscount([FromBody] Message message)
        {
            return await _repository.SendMsg(message);
        }
    }
}
