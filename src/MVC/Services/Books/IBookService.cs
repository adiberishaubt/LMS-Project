using Library_Managment_System.Models.Common;
using Library_Managment_System.Models.DTOs.Books;
using Library_Managment_System.Models.Entities;
using Library_Managment_System.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Library_Managment_System.Services.Books
{
    public interface IBookService
    {
        Task<BookDetailsViewModel> BookDetailsAsync(Guid id);
        Task<Book> GetBookAsync(Guid id);
        Task<BaseViewModel<BookViewModel>> GetBooksAsync(bool is_desc = false, int? itemsPerPage = default, int? currentPage = default, Expression<Func<BookViewModel, bool>> func = default);
        Task<Result> CreateBookAsync(CreateBook input, string userId);
        Task<Result> EditBookAsync(EditBook input);
        Task DeleteBookAsync(Guid bookId);
    }
}
