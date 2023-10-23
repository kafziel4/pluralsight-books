using AutoMapper;
using Books.API.Entities;
using Books.API.Filters;
using Books.API.Models;
using Books.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Books.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBooksRepository _booksRepository;
        private readonly IMapper _mapper;

        public BooksController(IBooksRepository booksRepository, IMapper mapper)
        {
            _booksRepository = booksRepository ?? throw new ArgumentNullException(nameof(booksRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("books")]
        [TypeFilter(typeof(BooksResultFilter))]
        public async Task<IActionResult> GetBooks()
        {
            var bookEntities = await _booksRepository.GetBooksAsync();
            return Ok(bookEntities);
        }

        [HttpGet("booksstream")]
        public async IAsyncEnumerable<BookDto> StreamBooks()
        {
            await foreach (var bookFromRepository in _booksRepository.GetBookAsAsyncEnumerable())
            {
                // add a delay to visually see the effect
                await Task.Delay(500);
                yield return _mapper.Map<BookDto>(bookFromRepository);
            }
        }

        [HttpGet("books/{id}", Name = "GetBook")]
        [TypeFilter(typeof(BookWithCoversResultFilter))]
        public async Task<IActionResult> GetBook(Guid id, CancellationToken cancellationToken)
        {
            var bookEntity = await _booksRepository.GetBookAsync(id);
            if (bookEntity == null)
                return NotFound();

            //var bookCover = await _booksRepository.GetBookCoverAsync("dummycover");

            var bookCovers = await _booksRepository.GetBookCoversProcessOneByOneAsync(id, cancellationToken);

            //var bookCovers = await _booksRepository.GetBookCoversProcessAfterWaitForAllAsync(id);

            return Ok((bookEntity, bookCovers));
        }

        [HttpPost("books")]
        [TypeFilter(typeof(BookResultFilter))]
        public async Task<IActionResult> CreateBook([FromBody] BookForCreationDto bookForCreation)
        {
            var bookEntity = _mapper.Map<Book>(bookForCreation);

            _booksRepository.AddBook(bookEntity);
            await _booksRepository.SaveChangesAsync();

            await _booksRepository.GetBookAsync(bookEntity.Id);

            return CreatedAtRoute("GetBook", new { id = bookEntity.Id }, bookEntity);
        }
    }
}
