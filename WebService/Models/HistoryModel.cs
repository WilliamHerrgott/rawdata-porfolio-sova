using System;

namespace WebService.Models {
    public class HistoryModel {
        public string Url { get; set; }
        public string SearchedText { get; set; }
        public DateTime Date { get; set; }
    }
}