using Library_Managment_System.Data;
using Library_Managment_System.Models.DTOs.Reviews;
using Library_Managment_System.Models.Entities;
using System;
using System.Threading.Tasks;

namespace Library_Managment_System.Services.Reviews
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> CreateReview(CreateReview input)
        {
            try
            {
                await _context.Reviews.AddAsync(new Review
                {
                    BookId = input.BookId,
                    ReviewerId = input.ReviewerId,
                    ReviewDate = DateTime.Now,
                    Comment = input.Comment
                });

                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                return "Something went wrong!";
            }

            return default;
        }
    }
}
