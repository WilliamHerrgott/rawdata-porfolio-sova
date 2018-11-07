using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackOverflowData;
using StackOverflowData.Functions;
using WebService.Models;

namespace WebService.Controllers {
    [Route("api/marks")]
    [ApiController]
    public class MarkController : Controller {
        private readonly IDataService _dataService;

        public MarkController(IDataService dataService) {
            _dataService = dataService;
        }

        [Authorize]
        [HttpPost("{postId}", Name = nameof(Mark))]
        public IActionResult Mark(int postId) {
            int.TryParse(HttpContext.User.Identity.Name, out var userId);
            var marked = _dataService.CreateMark(userId, postId);

            if (marked == false) {
                return BadRequest();
            }

            return Created("", new {userId, postId});
        }

        [Authorize]
        [HttpDelete]
        public IActionResult DeleteMark() {
            int.TryParse(HttpContext.User.Identity.Name, out var userId);
            var deleted = _dataService.DeleteMark(userId);

            if (deleted == false) {
                return NotFound();
            }

            return Ok();
        }

        [Authorize]
        [HttpDelete("{postId}")]
        public IActionResult DeleteMarksOfUser(int postId) {
            int.TryParse(HttpContext.User.Identity.Name, out var userId);
            var deleted = _dataService.DeleteMark(userId, postId);

            if (deleted == false) {
                return NotFound();
            }

            return Ok();
        }

        [Authorize]
        [HttpPut("{postId}/{text}", Name = nameof(MakeOrUpdateAnnotation))]
        public IActionResult MakeOrUpdateAnnotation(int postId, string text) {
            int.TryParse(HttpContext.User.Identity.Name, out var userId);
            var successful = _dataService.MakeOrUpdateAnnotation(userId, postId, text);

            if (successful == false) {
                return NotFound();
            }

            return Ok();
        }

        [Authorize]
        [HttpDelete("annotation/{postId}")]
        public IActionResult DeleteAnnotation(int postId) {
            int.TryParse(HttpContext.User.Identity.Name, out var userId);
            var deleted = _dataService.DeleteAnnotation(userId, postId);

            if (deleted == false) {
                return NotFound();
            }

            return Ok();
        }

        [Authorize]
        [HttpGet(Name = nameof(GetMarked))]
        public IActionResult GetMarked(int page = 0, int pageSize = 10) {
            int.TryParse(HttpContext.User.Identity.Name, out var userId);
            var marks = _dataService.GetMarked(userId, page, pageSize)
                .Select(CreateMarkModel);
            //if (marks == null)
            //{
            //    return NotFound();
            //}

            var numberOfItems = _dataService.GetNoOfMarks(userId);
            var numberOfPages = ComputeNumberOfPages(pageSize, numberOfItems);

            var result = new {
                NumberOfItems = numberOfItems,
                NumberOfPages = numberOfPages,
                First = CreateLink(0, pageSize),
                Prev = CreateLinkToPrevPage(page, pageSize),
                Next = CreateLinkToNextPage(page, pageSize, numberOfPages),
                Last = CreateLink(numberOfPages - 1, pageSize),
                Items = marks
            };
            return Ok(result);
        }

        private MarkModel CreateMarkModel(GetMarkedResult marks) {
            var model = Mapper.Map<MarkModel>(marks);
            model.Post = Url.Link(nameof(StackOverflowController.GetPost), new {id = marks.PostId});
            return model;
        }

        //Helper functions for paging
        private string CreateLinkToNextPage(int page, int pageSize, int numberOfPages) {
            return page >= numberOfPages - 1
                ? null
                : CreateLink(page = page + 1, pageSize);
        }

        private string CreateLinkToPrevPage(int page, int pageSize) {
            return page == 0
                ? null
                : CreateLink(page - 1, pageSize);
        }

        private static int ComputeNumberOfPages(int pageSize, int numberOfItems) {
            return (int) Math.Ceiling((double) numberOfItems / pageSize);
        }

        private string CreateLink(int page, int pageSize) {
            return Url.Link(nameof(GetMarked), new {page, pageSize});
        }
    }
}