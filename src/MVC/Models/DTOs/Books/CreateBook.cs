using Library_Managment_System.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Library_Managment_System.Models.DTOs.Books
{
    public class CreateBook
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string Link { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Image]
        public IFormFile Image { get; set; }
    }
}
