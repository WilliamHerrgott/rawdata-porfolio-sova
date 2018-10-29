using System;
using Microsoft.EntityFrameworkCore;

namespace StackOverflowData {
    class DataService {
        private static void GetPost() {
            using (var db = new StackOverflowContext()) {
                foreach (var result in db.GetPostResults.FromSql("select * from search({0})", 1240)) {
                    Console.WriteLine($"Result: {result.Body}, {result.Score}, {result.CreationDate}");
                }
            }
        }
    }
}