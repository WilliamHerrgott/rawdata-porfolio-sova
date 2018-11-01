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
        
        [HttpGet]
        public IActionResult GetAnswer(int questionId) {
            var answer = _dataService.GetAnswers(questionId);

            if (answer == null) {
                return NotFound();
            }
            
            return Ok(answer);
        }

        [HttpGet]
        public IActionResult GetComment(int postId) {
            var comment = _dataService.GetComments(postId);

            if (comment == null) {
                return NotFound();
            }

            return Ok(comment);
        }
    }
}