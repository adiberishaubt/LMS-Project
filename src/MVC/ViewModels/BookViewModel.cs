using System;

namespace Library_Managment_System.ViewModels
{
    public class BookViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Link { get; set; }
        public DateTime PublishedDate { get; set; }
        public Guid Image { get; set; }
        public string PublisherName { get; set; }
        public string PublisherId { get; set; }
    }
}
