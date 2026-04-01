using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using tms_api.Models;
using tms_api.Repositories.Interfaces;
using tms_api.RequestModels.ListRequestModels;
using tms_api.ViewModels.SectionViewModel;

namespace tms_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListController : ControllerBase
    {
        private readonly ILogger<ListController> _logger;
        private readonly IListRepository _listRepository;

        public ListController(ILogger<ListController> logger,
            IListRepository listRepository)
        {
            _logger = logger;
            _listRepository = listRepository;
        }

        /// <summary>
        /// Create a new list
        /// </summary>
        [HttpPost("create")]
        // TODO: Chỉ teamlead có thể tạo list
        [Authorize]
        public async Task<IActionResult> CreateNewList([FromQuery] CreateNewListRequestModel model)
        {
            try
            {
                var isExistedList = await _listRepository.CheckListExistInProject(model.Name, model.ProjectId);
                var projectLists = await _listRepository.GetAllListsByProject(model.ProjectId);

                if (isExistedList)
                {
                    return BadRequest("Duplicate list in same project!");
                }

                var list = new Lists
                {
                    Name = model.Name,
                    ProjectId = model.ProjectId,
                    Index = projectLists.Count + 1,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                };

                await _listRepository.AddAsync(list);
                await _listRepository.SaveChangeAsync();

                return Ok(new { StatusCode = HttpStatusCode.Created, Message = "Create new list successfully!", Data = new { list.ListId, list.Name, list.ProjectId, list.Index } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Delete a list
        /// </summary>
        [HttpDelete("delete-section")]
        [Authorize]
        public async Task<IActionResult> DeleteList(int id, int projectId)
        {
            try
            {
                var list = await _listRepository.GetListById(id);

                if (list == null)
                {
                    return BadRequest("An error occurred during deleting list!");
                }

                var lists = await _listRepository.GetAllListsByProject(projectId);

                var tasks = await _listRepository.GetTaskInSection(id);
                
                foreach (var task in tasks)
                {
                    task.IsDeleted = true;
                }

                var startIndex = list.Index;
                for (int i = startIndex; i < lists.Count; i++)
                {
                    lists[i].Index--;
                }
                list.IsDeleted = true;

                await _listRepository.SaveChangeAsync();
                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Delete section successfully!", Data = list });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Update list information
        /// </summary>
        [HttpPut]
        [Authorize()]
        public async Task<IActionResult> UpdateSection([FromBody] UpdateListRequestModel model)
        {
            try
            {
                var list = await _listRepository.GetListById(model.Id);

                if (list == null)
                {
                    return BadRequest("An error occurred during updating list!");
                }

                if (!string.IsNullOrEmpty(model.Name))
                {
                    list.Name = model.Name;
                }

                if (model.Index.HasValue)
                {
                    var projectLists = await _listRepository.GetAllListsByProject(list.ProjectId);

                    if (model.Index < 1 || model.Index > projectLists.Count)
                    {
                        return BadRequest($"Index can not smaller than 1 and larger than {projectLists.Count}");
                    }

                    if (list.Index < model.Index)
                    {
                        var lists = await _listRepository.GetAllListsInRange(list.Index + 1, model.Index.Value, list.ProjectId);
                        foreach (var item in lists)
                        {
                            item.Index--;
                        }
                        list.Index = model.Index.Value;
                    }
                    else if (list.Index > model.Index) 
                    { 
                        var lists = await _listRepository.GetAllListsInRange(model.Index.Value, list.Index - 1, list.ProjectId);
                        foreach (var item in lists)
                        {
                            item.Index++;
                        }
                        list.Index = model.Index.Value;
                    }
                }

                await _listRepository.SaveChangeAsync();
                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Update successfully!", Data = list });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Get list and tasks
        /// </summary>
        [HttpGet("list-task")]
        [Authorize]
        public async Task<IActionResult> GetSectionListTask([FromQuery] int projectId)
        {
            try
            {
                var lists = await _listRepository.GetSectionListTaskById(projectId);

                var returnData = lists.Select(l => new SectionListTaskViewModel
                {
                    ListId = l.ListId,
                    Name = l.Name,
                    Index = l.Index,
                    listTasks = l.Tasks.Select(k => new TaskViewModel
                    {
                        TaskId = k.TaskId,
                        Assignee = (k.User != null) ? new AssigneeViewModelTask { UserId = k.User.Id, FullName = k.User.FullName, AvatarUrl = k.User.AvatarUrl } : null,
                        Name = k.Name,
                        DueDate = k.DueDate,
                        Category = (k.ParentId == null || k.ParentId == 0) ? "Task" : "Subtask",
                        ListLabel = k.TaskLabel.Select(tl => new LabelViewModel
                        {
                            LabelId = tl.Label.LabelId,
                            Name = tl.Label.Name,
                            Color = tl.Label.Color
                        }).ToList()
                       
                    }).ToList()

                });
                
                
                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Request successfully!", Data = returnData });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Get list section in project
        /// </summary>
        [HttpGet("sections")]
        [Authorize]
        public async Task<IActionResult> GetListSectionInProject([FromQuery] int projectId)
        {
            try
            {
                var sections = await _listRepository.GetSectionsByProject(projectId);
                var returnData = sections.Select(s => new
                {
                    Id = s.ListId,
                    Name = s.Name,
                    Index = s.Index
                });

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Request successfully!", Data = returnData });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }
    }
}