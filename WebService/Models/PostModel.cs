using System;

namespace WebService.Models {
    public class PostModel {
        public string Body { get; set; }
        public int Score { get; set; }
        public DateTime CreationDate { get; set; }
        public string Author { get; set; }
    }
}