using System;

namespace WebService.Models
{
    public class AuthorModel
    {
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Location { get; set; }
        public int? Age { get; set; }
    }
}
