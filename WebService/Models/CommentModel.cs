using System;

namespace WebService.Models {
    public class CommentModel {
        public string Body { get; set; }
        public int Score { get; set; }
        public DateTime CreationDate { get; set; }
        public string Author { get; set; }
    }
}