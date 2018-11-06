using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebService.Models
{
    public class GetUserModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Location { get; set; }
        public string Marks { get; set; }
        public string History { get; set; }
    }
}
