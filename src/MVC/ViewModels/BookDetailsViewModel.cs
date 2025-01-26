using System;
using System.Collections.Generic;

namespace Library_Managment_System.ViewModels
{
    public class BookDetailsViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }
        public Guid Image { get; set; }
        public string PublisherId { get; set; }
        public string PublisherName { get; set; }
        public ICollection<ReviewViewModel> Reviews { get; set; }
    }
}
