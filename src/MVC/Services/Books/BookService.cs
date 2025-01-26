using Library_Managment_System.Data;
using Library_Managment_System.Models.Common;
using Library_Managment_System.Models.DTOs.Books;
using Library_Managment_System.Models.Entities;
using Library_Managment_System.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Library_Managment_System.Services.Books
{
    public class BookService : IBookService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public BookService(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<Result> CreateBookAsync(CreateBook input, string userId)
        {
            byte[] imageBytes = new byte[input.Image.Length];
            using (var stream = input.Image.OpenReadStream())
            {
                await stream.ReadAsync(imageBytes);        
            }

            var image = new Image
            {
                Bytes = imageBytes
            };

            var book = new Book
            {
                Title = input.Title,
                Author = input.Author,
                Link = input.Link,
                PublishedDate = DateTime.Now,
                PublisherId = userId,
                Image = image,
                Description = input.Description
            };

            await _context.Books.AddAsync(book);
            var result = await _context.SaveChangesAsync();

            return result > 0 ? Result.Succed() : Result.Failure("Something went wrong adding the book");
        }

        public Task<Book> GetBookAsync(Guid id)
        {
            return _context.Books.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task DeleteBookAsync(Guid bookId)
        {
            var book = await GetBookAsync(bookId);
           
            if(book == null) throw new ArgumentNullException();

            try
            {
                _context.Books.Remove(book);
                _context.Images.Remove(new Image { Id = book.ImageId });
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task<Result> EditBookAsync(EditBook input)
        {
            try
            {
                var bookToUpdate = await _context.Books.Include(x => x.Image).FirstOrDefaultAsync(x => x.Id == input.Id);

                if (bookToUpdate == null)
                {
                    throw new ArgumentNullException();
                }

                bookToUpdate.Link = input.Link ?? bookToUpdate.Link;
                bookToUpdate.Author = input.Author ?? bookToUpdate.Author;
                bookToUpdate.Description = input.Description ?? bookToUpdate.Description;
                bookToUpdate.Title = input.Title ?? bookToUpdate.Title;

                if (input.Image != null)
                {
                    var validationErrors = ValidateImage(input.Image);

                    if (validationErrors != null)
                    {
                        return Result.Failure(validationErrors);
                    }

                    using (var stream = new MemoryStream())
                    {
                        await input.Image.CopyToAsync(stream);
                        bookToUpdate.Image.Bytes = stream.ToArray();
                    }
                }

                _context.Books.Update(bookToUpdate);

                await _context.SaveChangesAsync();
            }catch(Exception)
            {
                return Result.Failure("Something went wrong editing book details");
            }

            return Result.Succed();
        }

        public Task<BookDetailsViewModel> BookDetailsAsync(Guid id)
        {
            return _context.Books.Include(x => x.Publisher)
                                .Include(x => x.Reviews)
                                .ThenInclude(x => x.Reviewer)
                                .Select(x => new BookDetailsViewModel
                                {
                                    Author = x.Author,
                                    Id = x.Id,
                                    Image = x.ImageId,
                                    Link = x.Link,
                                    Description = x.Description,
                                    Title = x.Title,
                                    PublishedDate = x.PublishedDate,
                                    PublisherId = x.PublisherId,
                                    PublisherName = x.Publisher.UserName,
                                    Reviews = x.Reviews.Select(y => new ReviewViewModel
                                    {
                                        Comment = y.Comment,
                                        Id = y.Id,
                                        ReviewDate = y.ReviewDate,
                                        ReviewerId = y.ReviewerId,
                                        ReviewerName = y.Reviewer.UserName
                                    }).ToList()
                                }).FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<BaseViewModel<BookViewModel>> GetBooksAsync(
            bool is_desc = false,
            int? itemsPerPage = default,
            int? currentPage = default,
            Expression<Func<BookViewModel, bool>> func = default)
        {
            var query = _context.Books.Include(x => x.Publisher).Select(x => new BookViewModel
            {
                Id = x.Id,
                PublishedDate = x.PublishedDate,
                Author = x.Author,
                Image = x.ImageId,
                Link = x.Link,
                PublisherName = x.Publisher.UserName,
                Title = x.Title,
                PublisherId = x.PublisherId
            })
            .AsNoTracking();

            var results = new BaseViewModel<BookViewModel>(query, is_desc, itemsPerPage, currentPage, func).GetResultsAsync();

            return results;
        }

        private string ValidateImage(IFormFile file)
        {
           long fileSizeLimit = _configuration.GetValue<long>("FileSizeLimit");
           string[] allowedExtensions = { ".png", ".jpg", ".jpeg" };
           Dictionary<string, List<byte[]>> fileSignature = new Dictionary<string, List<byte[]>>{
                { ".jpeg", new List<byte[]>{
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                    }
                }, 
                { ".jpg", new List<byte[]>{
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
                    }
                },
                { ".png", new List<byte[]>{
                        new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A}
                    }
                }
           };

            var fileExtenstion = Path.GetExtension(file.FileName).ToLowerInvariant();

            if(!allowedExtensions.Contains(fileExtenstion))
            {
                return $"The file extension must be one of : ";
            }

            using(var reader = new BinaryReader(file.OpenReadStream()))
            {
                var signatures = fileSignature[fileExtenstion];
                var headerBytes = reader.ReadBytes(signatures.Max(x => x.Length));

                if (!signatures.Any(x => headerBytes.Take(x.Length).SequenceEqual(x))){
                    return "This file is not a image";
                }
            }
            
            if(file.Length > fileSizeLimit)
            {
                return "The file size is over the limit";
            }
           
            return default;
        }

        private Task<bool> BookExists(Guid id)
        {
            return _context.Books.AnyAsync(x => x.Id == id);
        }
    }
}
