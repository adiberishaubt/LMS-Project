using System;
using System.ComponentModel.DataAnnotations;

namespace Library_Managment_System.Models.DTOs.Reviews
{
    public class CreateReview
    {
        [Required]
        public string Comment { get; set; }
        [Required]

        public string ReviewerId { get; set; }
        [Required]

        public Guid BookId { get; set; }
    }
}
