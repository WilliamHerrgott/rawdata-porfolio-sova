﻿using Microsoft.AspNetCore.Mvc;
using StackOverflowData;
using StackOverflowData.Functions;
using AutoMapper;
using WebService.Models;
using System;
using System.Linq;

namespace WebService.Controllers
{
    [Route("api/history")]
    [ApiController]
    public class HistoryController : Controller
    {
        private readonly DataService _dataService;

        public HistoryController(DataService dataService)
        {
            _dataService = dataService;
        }

        [HttpDelete("{userId}")]
        public IActionResult DeleteHistory(int userId)
        {
            var deleted = _dataService.DeleteHistory(userId);

            if (deleted == false)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpGet("{userId}", Name = nameof(GetHistory))]
        public IActionResult GetHistory(int userId, int page = 0, int pageSize = 10)
        {
            var history = _dataService.GetHistory(userId, page, pageSize)
                .Select(CreateHistoryModel);

            //if (history == null)
            //{
            //    return NotFound();
            //}

            var numberOfItems = history.Count();
            var numberOfPages = ComputeNumberOfPages(pageSize, numberOfItems);

            var result = new
            {
                NumberOfItems = numberOfItems,
                NumberOfPages = numberOfPages,
                First = CreateLink(0, pageSize),
                Prev = CreateLinkToPrevPage(page, pageSize),
                Next = CreateLinkToNextPage(page, pageSize, numberOfPages),
                Last = CreateLink(numberOfPages - 1, pageSize),
                Items = history
            };
            return Ok(result);
        }

        private HistoryModel CreateHistoryModel(GetHistoryResult history)
        {
            var model = Mapper.Map<HistoryModel>(history);
            model.Url = Url.Link(nameof(GetHistory), new { id = history.Id });
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
            return Url.Link(nameof(GetHistory), new { page, pageSize });
        }
    }
}
