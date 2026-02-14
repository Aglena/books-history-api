using BookHistoryApi.DTOs;
using BookHistoryApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookHistoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoryController : ControllerBase
    {
        private readonly IHistoryService _service;

        public HistoryController(IHistoryService service)
        {
            _service = service;
        }


        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<List<BookEventDto>>> GetByBookId(int id, [FromQuery] BookEventQueryDto query)
        {
            var history = await _service.GetByBookIdAsync(id, query);
            return Ok(history);
        }
    }
}
