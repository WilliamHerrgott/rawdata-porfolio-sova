using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StackOverflowData;
using StackOverflowData.Functions;
using WebService.Models;

namespace WebService.Controllers {
    [Route("api/StackOverflow")]
    [ApiController]
    public class StackOverflowController : Controller {
        private readonly IDataService _dataService;

        public StackOverflowController(IDataService dataService) {
            _dataService = dataService;
        }

        [HttpGet("answers/{questionId}", Name = nameof(GetAnswers))]
        public IActionResult GetAnswers(int questionId, int page = 0, int pageSize = 5) {
            var answers = _dataService.GetAnswers(questionId, page, pageSize)
                .Select(CreateAnswersModel);

            //if (answers == null) {
            //    return NotFound();
            //}
            var numberOfItems = answers.Count();
            var numberOfPages = ComputeNumberOfPages(pageSize, numberOfItems);

            var result = new {
                NumberOfItems = numberOfItems,
                NumberOfPages = numberOfPages,
                First = CreateAnswersLink(0, pageSize),
                Prev = page == 0 ? null : CreateAnswersLink(page - 1, pageSize),
                Next = page >= numberOfPages - 1 ? null : CreateAnswersLink(page = page + 1, pageSize),
                Last = CreateAnswersLink(numberOfPages - 1, pageSize),
                Items = answers
            };

            return Ok(result);
        }

        private PostOrCommentModel CreateAnswersModel(GetPostOrCommentResult answers) {
            var model = Mapper.Map<PostOrCommentModel>(answers);
            model.Url = Url.Link(nameof(GetAnswers), new {answers.Id});
            return model;
        }

        [HttpGet("comments/{postId}", Name = nameof(GetComments))]
        public IActionResult GetComments(int postId, int page = 0, int pageSize = 5) {
            var comments = _dataService.GetComments(postId, page, pageSize)
                .Select(CreateCommentsModel);

            //if (comment == null) {
            //    return NotFound();
            //}
            var numberOfItems = comments.Count();
            var numberOfPages = ComputeNumberOfPages(pageSize, numberOfItems);

            var result = new {
                NumberOfItems = numberOfItems,
                NumberOfPages = numberOfPages,
                First = CreateCommentsLink(0, pageSize),
                Prev = page == 0 ? null : CreateCommentsLink(page - 1, pageSize),
                Next = page >= numberOfPages - 1 ? null : CreateCommentsLink(page = page + 1, pageSize),
                Last = CreateCommentsLink(numberOfPages - 1, pageSize),
                Items = comments
            };

            return Ok(result);
        }

        private PostOrCommentModel CreateCommentsModel(GetPostOrCommentResult comments) {
            var model = Mapper.Map<PostOrCommentModel>(comments);
            model.Url = Url.Link(nameof(GetComments), new {comments.Id});
            return model;
        }

        [HttpGet("post/{id}", Name = nameof(GetPost))]
        public IActionResult GetPost(int id) {
            var post = _dataService.GetPost(id);

            if (post == null) {
                return NotFound();
            }

            var model = Mapper.Map<PostOrCommentModel>(post);
            model.Url = Url.Link(nameof(GetPost), new {id = post.Id});
            return Ok(post);
        }

        [HttpGet("{userId}/{text}", Name = nameof(Search))]
        public IActionResult Search(string text, int userId, int page = 0, int pageSize = 10) {
            var searchResult = _dataService.Search(text, userId, page, pageSize)
                .Select(CreateSearchModel);
            //if (searchResult == null)
            //{
            //    return NotFound();
            //}
            var searchModels = searchResult as SearchModel[] ?? searchResult.ToArray();
            var numberOfItems = searchModels.Length;
            var numberOfPages = ComputeNumberOfPages(pageSize, numberOfItems);

            var result = new {
                NumberOfItems = numberOfItems,
                NumberOfPages = numberOfPages,
                First = CreateCommentsLink(0, pageSize),
                Prev = page == 0 ? null : CreateCommentsLink(page - 1, pageSize),
                Next = page >= numberOfPages - 1 ? null : CreateSearchedLink(page = page + 1, pageSize),
                Last = CreateSearchedLink(numberOfPages - 1, pageSize),
                Items = searchModels
            };
            return Ok(result);
        }

        private SearchModel CreateSearchModel(SearchResult search) {
            var model = Mapper.Map<SearchModel>(search);
            model.Url = Url.Link(nameof(Search), new {search.Id});
            return model;
        }

        //helper functions
        private static int ComputeNumberOfPages(int pageSize, int numberOfItems) {
            return (int) Math.Ceiling((double) numberOfItems / pageSize);
        }

        private string CreateAnswersLink(int page, int pageSize) {
            return Url.Link(nameof(GetAnswers), new {page, pageSize});
        }

        private string CreateCommentsLink(int page, int pageSize) {
            return Url.Link(nameof(GetComments), new {page, pageSize});
        }

        private string CreateSearchedLink(int page, int pageSize) {
            return Url.Link(nameof(Search), new {page, pageSize});
        }
    }
}