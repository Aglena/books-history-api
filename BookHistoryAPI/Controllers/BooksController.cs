using BookHistoryApi.DTOs;
using BookHistoryApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookHistoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _service;

        public BooksController(IBookService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookDto dto)
        {
            var id = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id }, null);
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<BookDto>> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            return Ok(dto);
        }

        [HttpPatch("{id:int:min(1)}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBookDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }
    }
}

