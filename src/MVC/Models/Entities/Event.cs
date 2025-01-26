using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_Managment_System.Models.Entities
{
    public class Event
    {
        public Guid ID { get; set; }
        public string Title { get; set; }
        public DateTime Time { get; set; }
        public string Place { get; set; }
    }
}
