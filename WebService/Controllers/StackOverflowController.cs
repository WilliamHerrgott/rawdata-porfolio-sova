using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
            var numberOfItems = _dataService.GetNoOfAnswers(questionId);
            var numberOfPages = ComputeNumberOfPages(pageSize, numberOfItems);

            var result = new {
                NumberOfItems = numberOfItems,
                NumberOfPages = numberOfPages,
                First = CreateAnswersLink(0, pageSize),
                Prev = page == 0 ? null : CreateAnswersLink(page - 1, pageSize),
                Next = page >= numberOfPages - 1 ? null : CreateAnswersLink(page = page + 1, pageSize),
                Last = numberOfPages == 0 ? null : CreateAnswersLink(numberOfPages - 1, pageSize),
                Items = answers
            };

            return Ok(result);
        }

        private AnswerModel CreateAnswersModel(GetPostOrCommentResult answers) {
            var model = Mapper.Map<AnswerModel>(answers);
            model.Comments = Url.Link(nameof(GetComments), new {postId = answers.Id});
            model.Author = Url.Link(nameof(GetAuthorOfPost), new {postId = answers.Id});
            return model;
        }

        [HttpGet("comments/{postId}", Name = nameof(GetComments))]
        public IActionResult GetComments(int postId, int page = 0, int pageSize = 5) {
            var comments = _dataService.GetComments(postId, page, pageSize)
                .Select(CreateCommentsModel);

            //if (comment == null) {
            //    return NotFound();
            //}
            var numberOfItems = _dataService.GetNoOfComments(postId);
            var numberOfPages = ComputeNumberOfPages(pageSize, numberOfItems);

            var result = new {
                NumberOfItems = numberOfItems,
                NumberOfPages = numberOfPages,
                First = CreateCommentsLink(0, pageSize),
                Prev = page == 0 ? null : CreateCommentsLink(page - 1, pageSize),
                Next = page >= numberOfPages - 1 ? null : CreateCommentsLink(page = page + 1, pageSize),
                Last = numberOfPages == 0 ? null : CreateCommentsLink(numberOfPages - 1, pageSize),
                Items = comments
            };

            return Ok(result);
        }

        private CommentModel CreateCommentsModel(GetPostOrCommentResult comments) {
            var model = Mapper.Map<CommentModel>(comments);
            model.Author = Url.Link(nameof(GetAuthorOfComment), new {commentId = comments.Id});
            return model;
        }

        [HttpGet("post/{id}", Name = nameof(GetPost))]
        public IActionResult GetPost(int id) {
            var post = _dataService.GetPost(id);
            
            if (post == null) {
                return NotFound();
            }
            bool isQuestion;
            var model = Mapper.Map<PostModel>(post);
            using (var context = new StackOverflowContext())
            {
                if (context.Answers.Any(i => i.Id == post.Id))
                    isQuestion = false;
                else
                    isQuestion = true;
            }

            
            model.Author = Url.Link(nameof(GetAuthorOfPost), new {postId = post.Id});
            model.Answers = (isQuestion == true)? Url.Link(nameof(GetAnswers), new { questionId = post.Id}): null;
            model.Comments = Url.Link(nameof(GetComments), new { postId = post.Id});
            return Ok(model);
        }

        [HttpGet("author/post/{postId}", Name = nameof(GetAuthorOfPost))]
        public IActionResult GetAuthorOfPost(int postId) {
            var author = _dataService.GetAuthorOfPost(postId);
            if (author == null) {
                return NotFound();
            }

            var model = Mapper.Map<AuthorModel>(author);
            return Ok(model);
        }

        [HttpGet("author/comment/{commentId}", Name = nameof(GetAuthorOfComment))]
        public IActionResult GetAuthorOfComment(int commentId) {
            var author = _dataService.GetAuthorOfComment(commentId);
            if (author == null) {
                return NotFound();
            }

            var model = Mapper.Map<AuthorModel>(author);
            return Ok(model);
        }

        [Authorize]
        [HttpGet("search/{text}", Name = nameof(Search))]
        public IActionResult Search(string text, int page = 0, int pageSize = 10) {
            int.TryParse(HttpContext.User.Identity.Name, out var userId);
            var searchResult = _dataService.Search(text, userId, page, pageSize)
                .Select(CreateSearchModel);
            //if (searchResult == null)
            //{
            //    return NotFound();
            //}
            var numberOfItems = _dataService.GetSearchedCount(text);
            var numberOfPages = ComputeNumberOfPages(pageSize, numberOfItems);

            var result = new {
                NumberOfItems = numberOfItems,
                NumberOfPages = numberOfPages,
                First = CreateCommentsLink(0, pageSize),
                Prev = page == 0 ? null : CreateCommentsLink(page - 1, pageSize),
                Next = page >= numberOfPages - 1 ? null : CreateSearchedLink(page = page + 1, pageSize),
                Last = numberOfPages == 0 ? null : CreateSearchedLink(numberOfPages - 1, pageSize),
                Items = searchResult
            };
            return Ok(result);
        }

        private SearchModel CreateSearchModel(SearchResult search) {
            var model = Mapper.Map<SearchModel>(search);
            model.Url = Url.Link(nameof(GetPost), new {id = search.Id});
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