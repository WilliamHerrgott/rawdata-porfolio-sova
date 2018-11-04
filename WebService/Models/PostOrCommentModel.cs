using System;

namespace WebService.Models {
    public class PostOrCommentModel {
        public string Url { get; set; }
        public string Body { get; set; }
        public string Score { get; set; }
        public DateTime CreationDate { get; set; }
    }
}