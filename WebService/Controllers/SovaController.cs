using StackOverflowData;
using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers {
    [Route("api/SOVA")]
    [ApiController]
    public class SovaController : Controller {
        private readonly DataService _dataService;

        public SovaController(DataService dataService) {
            _dataService = dataService;
        }
    }
}