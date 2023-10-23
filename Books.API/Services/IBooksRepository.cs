using Books.API.Entities;
using Books.API.Models.External;

namespace Books.API.Services
{
    public interface IBooksRepository
    {
        IEnumerable<Book> GetBooks();
        Task<IEnumerable<Book>> GetBooksAsync(IEnumerable<Guid> bookIds);
        Task<IEnumerable<Book>> GetBooksAsync();
        Task<Book?> GetBookAsync(Guid id);
        IAsyncEnumerable<Book> GetBookAsAsyncEnumerable();
        Task<BookCoverDto?> GetBookCoverAsync(string id);
        Task<IEnumerable<BookCoverDto>> GetBookCoversProcessOneByOneAsync(
            Guid bookId, CancellationToken cancellationToken);
        Task<IEnumerable<BookCoverDto>> GetBookCoversProcessAfterWaitForAllAsync(Guid bookId);
        void AddBook(Book bookToAdd);
        Task<bool> SaveChangesAsync();
    }
}
