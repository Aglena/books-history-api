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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<BookEventDto>>> GetAll([FromQuery] BookEventQueryDto query)
        {
            var history = await _service.GetAll(query);
            return Ok(history);
        }

        [HttpGet("{bookId:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<BookEventDto>>> GetByBookId(int bookId, [FromQuery] BookEventQueryDto query)
        {
            var history = await _service.GetByBookIdAsync(bookId, query);
            return Ok(history);
        }
    }
}
