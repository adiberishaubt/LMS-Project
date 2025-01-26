using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Library_Managment_System.Models.DTOs.Books
{
    public class EditBook
    {
        [Required]
        public Guid Id { get; set; }
        [Required]

        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Link { get; set; }
        [Required]
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public Guid CurrentImage { get; set; }
    }
}
