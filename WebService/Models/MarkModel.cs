using System;

namespace WebService.Models {
    public class MarkModel {
        public string Url { get; set; }
        public int PostId { get; set; }
        public string Annotation { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime AnnotationDate { get; set; }
    }
}