namespace Library.Presentation.Controllers
{
    using global::Library.Domain.Abstractions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    namespace Library.Web.Controllers
    {
        [Authorize]
        public class BookController : Controller
        {
            private readonly IUnitOfWork _unitOfWork;

            public BookController(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            // GET: /Book
            public async Task<IActionResult> Index()
            {
                var books = await _unitOfWork.BookRepository.GetAllBooksAsync();
                return View(books);
            }

            // GET: /Book/Details/{id}
            public async Task<IActionResult> Details(Guid id)
            {
                var book = await _unitOfWork.BookRepository.GetBookByIdAsync(id);
                if (book == null)
                {
                    return NotFound();
                }
                return View(book);
            }

            // GET: /Book/Search
            public async Task<IActionResult> Search(string searchText, string? title, string? author, string? isbn)
            {
                var books = await _unitOfWork.BookRepository.SearchBooksAsync(searchText, title, author, isbn);
                return View(books);
            }

            // GET: /Book/Borrowings
            public async Task<IActionResult> Borrowings()
            {
                Guid userId = Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value);

                var books = await _unitOfWork.BookRepository.GetBorrowingsByUserIdAsync(userId);
                return View(books);
            }

            public async Task<IActionResult> Borrow(Guid id)
            {
                Guid userId = Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value);

                await _unitOfWork.BorrowingRepository.BorrowBookAsync(userId, id);

                return RedirectToAction("Index");
            }

            public async Task<IActionResult> Return(Guid id)
            {
                Guid userId = Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value);

                await _unitOfWork.BorrowingRepository.ReturnBookAsync(userId, id);

                return RedirectToAction("Index");
            }
        }
    }
}
