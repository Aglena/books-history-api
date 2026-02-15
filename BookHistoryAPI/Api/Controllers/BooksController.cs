using BookHistoryApi.Application.DTOs;
using BookHistoryApi.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookHistoryApi.Api.Controllers
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] BookDto dto)
        {
            var id = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id }, null);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<BookDto>> GetAll([FromQuery] BookQueryDto dto)
        {
            var books = await _service.GetAll(dto);
            return Ok(books);
        }

        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDto>> GetById(int id)
        {
            var book = await _service.GetByIdAsync(id);
            return Ok(book);
        }

        [HttpPatch("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBookDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }
    }
}

