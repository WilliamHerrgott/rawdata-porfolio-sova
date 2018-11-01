using StackOverflowData;
using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers {
    [Route("api/StackOverflow")]
    [ApiController]
    public class StackOverflowController : Controller {
        private readonly DataService _dataService;

        public StackOverflowController(DataService dataService) {
            _dataService = dataService;
        }
    }
}