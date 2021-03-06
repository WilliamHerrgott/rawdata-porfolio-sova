﻿using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackOverflowData;
using StackOverflowData.Functions;
using WebService.Models;

namespace WebService.Controllers {
    [Route("api/history")]
    [ApiController]
    public class HistoryController : Controller {
        private readonly IDataService _dataService;

        public HistoryController(IDataService dataService) {
            _dataService = dataService;
        }

        [Authorize]
        [HttpDelete]
        public IActionResult DeleteHistory() {
            int.TryParse(HttpContext.User.Identity.Name, out var userId);
            var deleted = _dataService.DeleteHistory(userId);

            if (deleted == false) return NotFound();

            return Ok();
        }

        [Authorize]
        [HttpGet(Name = nameof(GetAllHistoryOfUser))]
        public IActionResult GetAllHistoryOfUser(int page = 0, int pageSize = 10) {
            int.TryParse(HttpContext.User.Identity.Name, out var userId);
            var history = _dataService.GetHistory(userId, page, pageSize)
                .Select(CreateHistoryModel);

            var numberOfItems = _dataService.GetHistoryCount(userId);
            var numberOfPages = ComputeNumberOfPages(pageSize, numberOfItems);

            var result = new {
                NumberOfItems = numberOfItems,
                NumberOfPages = numberOfPages,
                First = CreateLink(0, pageSize),
                Prev = CreateLinkToPrevPage(page, pageSize),
                Next = CreateLinkToNextPage(page, pageSize, numberOfPages),
                Last = numberOfPages == 0 ? null : CreateLink(numberOfPages - 1, pageSize),
                Items = history
            };
            return Ok(result);
        }

        private HistoryModel CreateHistoryModel(GetHistoryResult history) {
            var model = Mapper.Map<HistoryModel>(history);
            model.Url = Url.Link(nameof(StackOverflowController.Search),
                new {text = history.SearchedText});
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
            return Url.Link(nameof(GetAllHistoryOfUser), new {page, pageSize});
        }
    }
}