using System;

namespace WebService.Models
{
    public class MarkModel
    {
        public string User { get; set; }
        public string Post { get; set; }
        public string Annotation { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime AnnotationDate { get; set; }
    }
}