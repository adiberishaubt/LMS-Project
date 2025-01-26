using Library_Managment_System.Data;
using Library_Managment_System.Models.DTOs.Books;
using Library_Managment_System.Models.DTOs.Reviews;
using Library_Managment_System.Services.Books;
using Library_Managment_System.Services.Reviews;
using Library_Managment_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Library_Managment_System.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IReviewService _reviewService;
        private readonly ApplicationDbContext _cntx;

        public BookController(IBookService bookService, IReviewService reviewService, ApplicationDbContext context)
        {
            _bookService = bookService;
            _reviewService = reviewService;
            _cntx = context;
        }

        public async Task<IActionResult> Index(
            string title = null,
            string author = null,
            DateTime? publishedDate = null,
            int? currentPage = null,
            string orderBy = null,
            int? itemsPerPage = null,
            bool desc = false)
        {
            title = title ?? "";
            author = author ?? "";
            itemsPerPage = itemsPerPage ?? 5;
            publishedDate = publishedDate ?? DateTime.Now;

            ViewData["Author"] = author;
            ViewData["Title"] = title;
            ViewData["itemsPerPage"] = itemsPerPage;

            Expression<Func<BookViewModel, bool>> func = x => x.Title.Contains(title) && x.Author.Contains(author) && x.PublishedDate <= publishedDate;

            var books = await _bookService.GetBooksAsync(desc, itemsPerPage, currentPage, func);

            switch (orderBy)
            {
                case "Title":
                {
                    books.Items = desc ? books.Items.OrderByDescending(x => x.Title) : books.Items.OrderBy(x => x.Title);
                    break;
                }
                case "Author":
                {
                    books.Items = desc ? books.Items.OrderByDescending(x => x.Author) : books.Items.OrderBy(x => x.Author);
                    break;
                }
                case "PublishedDate":
                {
                    books.Items = desc ? books.Items.OrderByDescending(x => x.PublishedDate) : books.Items.OrderBy(x => x.PublishedDate);
                    break;
                }
            }
            
            if(books.Errors.Count > 0)
            {
                foreach(var error in books.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            return View(books);
        }

        [HttpGet]
        public async Task<IActionResult> Image(Guid id)
        {
            var image = await _cntx.Images.FirstOrDefaultAsync(x => x.Id == id);
            byte[] buffer = image.Bytes;
            var imageExtension = GetImageExtension(buffer);

            return File(buffer, $"image/{imageExtension}");
        }

        private string GetImageExtension(byte[] buffer)
        {
            Dictionary<string, List<byte[]>> fileSignature = new Dictionary<string, List<byte[]>>{
                { "jpeg", new List<byte[]>{
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                    }
                },
                { "jpg", new List<byte[]>{
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
                    }
                },
                { "png", new List<byte[]>{
                        new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A}
                    }
                }
            };

            foreach(var key in fileSignature.Keys)
            {
                bool match = fileSignature[key].Any(x => x.SequenceEqual(buffer.Take(x.Length)));

                if (match) return key;
            }

            throw new FileLoadException("This is not an image");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateBook input)
        {
            if(ModelState.IsValid)
            {
                var result = await _bookService.CreateBookAsync(input, User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);

                if(result.Success)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            return View();
        }

        public async Task<IActionResult> Details(Guid bookId)
        {
            var viewModel = await _bookService.BookDetailsAsync(bookId);

            if(viewModel == null)
            {
                return NotFound();
            }

            return View("Details", viewModel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid bookId)
        {
            if (bookId == null)
            {
                return NotFound();
            }

            try
            {
                await _bookService.DeleteBookAsync(bookId);
            }
            catch(ArgumentNullException)
            {
                return NotFound();
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid bookId)
        {
            var book = await _bookService.GetBookAsync(bookId);

            if(book == null)
            {
                return NotFound();
            }

            return View(new EditBook
            {
                Id = book.Id,
                Author = book.Author,
                Description = book.Description,
                CurrentImage = book.ImageId,
                Link = book.Link,
                Title = book.Title
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(EditBook input)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var result = await _bookService.EditBookAsync(input);

                    if(result.Success)
                    {
                        return RedirectToAction("Details", new { bookId = input.Id });
                    }

                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
                catch(ArgumentNullException)
                {
                    return NotFound();
                }
            }

            return await Edit(input.Id);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(CreateReview input)
        {
            if(ModelState.IsValid)
            {
                var errors = await _reviewService.CreateReview(input);

                if(errors == null)
                {
                    return RedirectToAction(nameof(Details), new { bookId = input.BookId });    
                }

                ModelState.AddModelError("", errors);
            }

            return await Details(input.BookId);
        }
    }
}