using Microsoft.AspNetCore.Mvc;
using StackOverflowData;
using StackOverflowData.Functions;
using AutoMapper;
using WebService.Models;
using System;
using System.Linq;

namespace WebService.Controllers {
    [Route("api/marks")]
    [ApiController]
    public class MarkController : Controller {
        private readonly IDataService _dataService;

        public MarkController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpPost]
        public IActionResult Mark(int userId, int postId) {
            var marked = _dataService.CreateMark(userId, postId);

            if (marked == false) {
                return BadRequest();
            }

            return Created("", new { userId, postId });
        }

        [HttpDelete("{userId}")]
        public IActionResult DeleteMark(int userId) {
            var deleted = _dataService.DeleteMark(userId);

            if (deleted == false) {
                return NotFound();
            }

            return Ok();
        }

        [HttpDelete("{userId}")]
        public IActionResult DeleteMarksOfUser(int userId)
        {
            var deleted = _dataService.DeleteMark(userId);

            if (deleted == false)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpDelete("{userId}/{postId}")]
        public IActionResult DeleteMark(int userId, int postId)
        {
            var deleted = _dataService.DeleteMark(userId, postId);

            if (deleted == false)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPut("{userId}/{postId}")]
        public IActionResult MakeOrUpdateAnnotation(int userId, int postId, string text)
        {
            var successful = _dataService.MakeOrUpdateAnnotation(userId, postId, text);

            if (successful == false)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpDelete("annotation/{userId}/{postId}")]
        public IActionResult DeleteAnnotation(int userId, int postId)
        {
            var deleted = _dataService.DeleteAnnotation(userId, postId);

            if (deleted == false)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpGet("{userId}", Name = nameof(GetMarked))]
        public IActionResult GetMarked(int userId, int page = 0, int pageSize = 10)
        {
            var marks = _dataService.GetMarked(userId, page, pageSize)
                .Select(CreateMarkModel);
            //if (marks == null)
            //{
            //    return NotFound();
            //}

            var numberOfItems = marks.Count();
            var numberOfPages = ComputeNumberOfPages(pageSize, numberOfItems);

            var result = new
            {
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

        private MarkModel CreateMarkModel(GetMarkedResult marks)
        {
            var model = Mapper.Map<MarkModel>(marks);
            model.Url = Url.Link(nameof(GetMarked), new { userId = marks.UserId, postId = marks.PostId });
            return model;
        }

        //Helper functions for paging
        private string CreateLinkToNextPage(int page, int pageSize, int numberOfPages)
        {
            return page >= numberOfPages - 1
                ? null
                : CreateLink(page = page + 1, pageSize);
        }

        private string CreateLinkToPrevPage(int page, int pageSize)
        {
            return page == 0
                ? null
                : CreateLink(page - 1, pageSize);
        }

        private static int ComputeNumberOfPages(int pageSize, int numberOfItems)
        {
            return (int)Math.Ceiling((double)numberOfItems / pageSize);
        }

        private string CreateLink(int page, int pageSize)
        {
            return Url.Link(nameof(GetMarked), new { page, pageSize });
        }
    }
}
