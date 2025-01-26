using System;
using System.Collections.Generic;

namespace Library_Managment_System.Models.Entities
{
    public class Book
    {
        public Guid Id{ get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }
        public Guid ImageId { get; set; }
        public Image Image { get; set; }
        public User Publisher { get; set; }
        public string PublisherId { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
