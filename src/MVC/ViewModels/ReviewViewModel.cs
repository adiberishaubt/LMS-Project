using System;

namespace Library_Managment_System.ViewModels
{
    public class ReviewViewModel
    {
        public Guid Id { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        public string ReviewerId { get; set; }
        public string ReviewerName { get; set; }
    }
}
