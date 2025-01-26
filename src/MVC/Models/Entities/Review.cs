using System;

namespace Library_Managment_System.Models.Entities
{
    public class Review
    {
        public Guid Id { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        public User Reviewer { get; set; }
        public string ReviewerId { get; set; }
        public Book Book { get; set; }
        public Guid BookId { get; set; }
    }
}
