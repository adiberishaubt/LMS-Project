using Library_Managment_System.Models.DTOs.Reviews;
using System.Threading.Tasks;

namespace Library_Managment_System.Services.Reviews
{
    public interface IReviewService
    {
        Task<string> CreateReview(CreateReview input);
    }
}
