using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using tms_api.Common;
using tms_api.Models;
using tms_api.Repositories.Interfaces;
using tms_api.RequestModels.ChecklistRequestModels;
using tms_api.RequestModels.CommentRequestModels;
using tms_api.RequestModels.NotificationRequestModels;
using tms_api.RequestModels.TaskRequestModels;
using tms_api.Services.UploadFile;
using tms_api.ViewModels.TaskViewModels;

namespace tms_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ILogger<TaskController> _logger;
        private readonly ITaskRepository _taskRepository;
        private readonly IListRepository _listRepository;
        private readonly IFileAttachmentRepository _fileAttachmentRepository;
        private readonly IDependencyRepository _dependencyRepository;
        private readonly ITaskPriorityRepository _taskPriorityRepository;
        private readonly ITaskStatusRepository _taskStatusRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IChecklistRepository _checklistRepository;
        private readonly ITaskLabelRepository _taskLabelRepository;
        private readonly ICloudStorage _cloudStorage;
        private readonly IFbUserTokenRepository _fbUserTokenRepository;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly INotificationRepository _notificationRepository;

        public TaskController(ILogger<TaskController> logger,
            ITaskRepository taskRepository,
            IListRepository listRepository,
            ICloudStorage cloudStorage,
            IFileAttachmentRepository fileAttachmentRepository,
            IDependencyRepository dependencyRepository,
            ITaskPriorityRepository taskPriorityRepository,
            ITaskStatusRepository taskStatusRepository,
            ICommentRepository commentRepository,
            IChecklistRepository checklistRepository,
            ITaskLabelRepository taskLabelRepository,
            IFbUserTokenRepository fbUserTokenRepository,
            IConfiguration configuration,
            IUserRepository userRepository,
            UserManager<User> userManager,
            INotificationRepository notificationRepository)
        {
            _logger = logger;
            _taskRepository = taskRepository;
            _listRepository = listRepository;
            _cloudStorage = cloudStorage;
            _fileAttachmentRepository = fileAttachmentRepository;
            _taskPriorityRepository = taskPriorityRepository;
            _taskStatusRepository = taskStatusRepository;
            _dependencyRepository = dependencyRepository;
            _commentRepository = commentRepository;
            _checklistRepository = checklistRepository;
            _taskLabelRepository = taskLabelRepository;
            _fbUserTokenRepository = fbUserTokenRepository;
            _configuration = configuration;
            _userRepository = userRepository;
            _userManager = userManager;
            _notificationRepository = notificationRepository;
        }

        /// <summary>
        /// Get all task due soon
        /// </summary>
        [HttpGet("due-soon-task")]
        [Authorize]
        public async Task<IActionResult> GetDueSoonTask(string userId, int daysDueIn)
        {
            try
            {
                var dueSoonTask = await _taskRepository.GetListDueSoonTask(userId, daysDueIn);
                var returnData = dueSoonTask.Select(d => new
                {
                    Id = d.TaskId,
                    Name = d.Name,
                    DueDate = d.DueDate
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
        /// Create a new task
        /// </summary>
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateNewTask([FromBody] NewTaskRequestModel model)
        {
            try
            {
                var currentList = await _listRepository.GetListById(model.ListId);
                var newTask = new Tasks
                {
                    Name = model.Name.Trim(),
                    TaskPriorityId = AppConstants.TaskPriority.MEDIUM,
                    TaskStatusId = AppConstants.TaskStatus.CREATE,
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                };

                newTask.List = currentList;

                await _taskRepository.AddAsync(newTask);
                await _taskRepository.SaveChangeAsync();

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Create new task successfully!", Data = new { Id = newTask.TaskId, Name = newTask.Name, IsActive = newTask.IsActive } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var task = await _taskRepository.GetTaskById(id);
                if (task == null)
                {
                    return BadRequest("An error occurred during deleting task!");
                }

                task.IsDeleted = true;
                await _taskRepository.SaveChangeAsync();
                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Delete task successfully!", Data = task });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Upload file in task
        /// </summary>
        [HttpPost("upload-file")]
        [Authorize]
        public async Task<IActionResult> UploadFile([FromForm] int taskId, IFormFile file)
        {
            try
            {
                var task = await _taskRepository.GetTaskByIdAsync(taskId);
                if (task == null)
                {
                    return BadRequest("An error occurred during upload file!");
                }

                // TODO: Validate file upload
                var bucketName = "tmso-file-bucket";
                var mediaLink = await _cloudStorage.UploadFileAsync(bucketName, file, file.FileName);
                var fileAttachment = new FileAttachment
                {
                    Name = file.FileName,
                    MediaLink = mediaLink,
                    Task = task,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                };

                await _fileAttachmentRepository.AddAsync(fileAttachment);
                await _fileAttachmentRepository.SaveChangeAsync();

                var authorization = Request.Headers[HeaderNames.Authorization];

                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var token = headerValue.Parameter;
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    var requestUserId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;
                    var assigneeUserId = task.UserId;

                    if (!string.IsNullOrEmpty(assigneeUserId) && !requestUserId.Equals(assigneeUserId))
                    {
                        var assigneeUserIds = new List<string>
                        {
                            assigneeUserId
                        };

                        var notificationTokens = (await _fbUserTokenRepository.GetTokensByUserIds(assigneeUserIds)).Select(n => n.Token).ToList();
                        var requestUser = await _userManager.FindByIdAsync(requestUserId);
                        var assigneeUser = await _userManager.FindByIdAsync(assigneeUserId);

                        if (requestUser != null && assigneeUser != null)
                        {
                            // Gửi notification đến user
                            var baseUrl = _configuration["ApplicationUrl"];

                            using (var httpClient = new HttpClient())
                            {
                                var notificationModel = new PushNotificationRequestModel
                                {
                                    Tokens = notificationTokens,
                                    Notification = new RequestModels.NotificationRequestModels.Notification
                                    {
                                        Title = "TMSO Software",
                                        Body = $"{requestUser.FullName} upload file",
                                        Icon = ""
                                    }
                                };

                                var body = new StringContent(JsonConvert.SerializeObject(notificationModel), System.Text.Encoding.UTF8, "application/json");
                                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                                await httpClient.PostAsync(baseUrl + "/api/notification/push", body);
                            }

                            // Lưu notification history
                            var messageObj = new
                            {
                                Title = $"{requestUser.FullName} upload file attachment",
                                Content = $"TASK-{task.TaskId} | {file.FileName}"
                            };
                            var message = JsonConvert.SerializeObject(messageObj);

                            var notification = new Models.Notification
                            {
                                Message = message,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            var notificationHistory = new UserNotification
                            {
                                User = assigneeUser,
                                Notification = notification,
                                IsViewed = false,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            await _notificationRepository.AddAsync(notificationHistory);
                            await _notificationRepository.SaveChangeAsync();
                        }
                    }
                }

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Request successfully!", Data = new { Id = fileAttachment.FileAttachmentId, Name = fileAttachment.Name, MediaLink = fileAttachment.MediaLink } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Delete a upload file
        /// </summary>
        [HttpDelete("file-attachment")]
        [Authorize]
        public async Task<IActionResult> DeleteUpdaloadFile(int idFileAttachment, int taskId)
        {
            try
            {
              
                var task = await _taskRepository.GetTaskByIdAsync(taskId);

                if (task == null)
                {
                    return BadRequest("Can not find task information!");
                }
                var uploadFile = await _fileAttachmentRepository.GetFileAttachment(idFileAttachment);
                if (uploadFile == null)
                {
                    return BadRequest("An error occurred during upload file!");
                }
                uploadFile.IsDeleted = true;
                await _taskRepository.SaveChangeAsync();

                var authorization = Request.Headers[HeaderNames.Authorization];

                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var token = headerValue.Parameter;
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    var requestUserId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;
                    var assigneeUserId = task.UserId;

                    if (!string.IsNullOrEmpty(assigneeUserId) && !requestUserId.Equals(assigneeUserId))
                    {
                        var assigneeUserIds = new List<string>
                        {
                            assigneeUserId
                        };

                        var notificationTokens = (await _fbUserTokenRepository.GetTokensByUserIds(assigneeUserIds)).Select(n => n.Token).ToList();
                        var requestUser = await _userManager.FindByIdAsync(requestUserId);
                        var assigneeUser = await _userManager.FindByIdAsync(assigneeUserId);

                        if (requestUser != null && assigneeUser != null)
                        {
                            // Gửi notification đến user
                            var baseUrl = _configuration["ApplicationUrl"];

                            using (var httpClient = new HttpClient())
                            {
                                var notificationModel = new PushNotificationRequestModel
                                {
                                    Tokens = notificationTokens,
                                    Notification = new RequestModels.NotificationRequestModels.Notification
                                    {
                                        Title = "TMSO Software",
                                        Body = $"{requestUser.FullName} delete file attachment",
                                        Icon = ""
                                    }
                                };

                                var body = new StringContent(JsonConvert.SerializeObject(notificationModel), System.Text.Encoding.UTF8, "application/json");
                                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                                await httpClient.PostAsync(baseUrl + "/api/notification/push", body);
                            }

                            // Lưu notification history
                            var messageObj = new
                            {
                                Title = $"{requestUser.FullName} delete file attachment ",
                                Content = $"TASK-{task.TaskId} "
                            };
                            var message = JsonConvert.SerializeObject(messageObj);

                            var notification = new Models.Notification
                            {
                                Message = message,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            var notificationHistory = new UserNotification
                            {
                                User = assigneeUser,
                                Notification = notification,
                                IsViewed = false,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            await _notificationRepository.AddAsync(notificationHistory);
                            await _notificationRepository.SaveChangeAsync();
                        }
                    }
                }
                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Delete task successfully!", Data = uploadFile });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }


        /// <summary>
        /// Get task detail by id
        /// </summary>
        [HttpGet("detail/{id}")]
        [Authorize]
        public async Task<IActionResult> GetTaskDetail(int id)
        {
            try
            {
                var task = await _taskRepository.GetTaskDetailByIdAsync(id);
                if (task == null)
                {
                    return BadRequest("Can not find task information!");
                }

                var returnData = new TaskDetailViewModel
                {
                    TaskId = task.TaskId,
                    Name = task.Name,
                    Description = task.Description,
                    ProjectId = task.List.ProjectId,
                    ProjectName = task.List.Project.Name,
                    StartDate = task.StartDate,
                    DueDate = task.DueDate,
                    IsCompleted = task.IsCompleted,
                    IsActive = task.IsActive,
                    ParentId = task.ParentId,
                };

                returnData.Priority = (task.TaskPriority != null) ? new PriorityViewModel { PriorityId = task.TaskPriority.TaskPriorityId, Name = task.TaskPriority.Name } : null;
                returnData.Status = (task.TaskStatus != null) ? new StatusViewModel { StatusId = task.TaskStatus.TaskStatusId, Name = task.TaskStatus.Name } : null;
                returnData.Section = (task.List != null) ? new SectionViewModel { SectionId = task.List.ListId, Name = task.List.Name } : null;
                returnData.Assignee = (task.User != null) ? new AssigneeViewModel { UserId = task.User.Id, FullName = task.User.FullName, AvatarUrl = task.User.AvatarUrl } : null;

                returnData.Comments = task.Comment.Select(c => new CommentViewModel
                {
                    CommentId = c.CommentId,
                    Author = new UserViewModel { UserId = c.User.Id, UserName = c.User.UserName, FullName = c.User.FullName, AvatarUrl = c.User.AvatarUrl },
                    Content = c.Content,
                    AttachFile = c.AttachFile,
                    CreatedDate = c.CreatedDate,
                    ModifiedDate = c.ModifiedDate
                }).ToList();

                returnData.Checklists = task.Checklist.Select(c => new ChecklistViewModel
                {
                    ChecklistId = c.ChecklistId,
                    Name = c.Name,
                    IsCompleted = c.IsCompleted,
                    CreatedDate = c.CreatedDate,
                    ModifiedDate = c.ModifiedDate
                }).ToList();

                returnData.Dependencies = task.TaskDependencies.Select(td => new DependenciesViewModel
                {
                    DependencyId = td.TaskDependencyId,
                    TaskDenpendencyId = td.TaskDependId,
                    TaskName = (_taskRepository.GetTaskByIdAsync(td.TaskDependId).Result)?.Name ?? "Deleted task",
                    DependencyName = td.Dependency.Name,
                    CreatedDate = td.CreatedDate
                }).ToList();

                returnData.Labels = task.TaskLabel.Select(tl => new LabelViewModel
                {
                    LabelId = tl.Label.LabelId,
                    Name = tl.Label.Name,
                    Color = tl.Label.Color
                }).ToList();

                returnData.FileAttachments = task.FileAttachments.Select(fa => new FileAttachmentViewModel
                {
                    FileAttachmentId = fa.FileAttachmentId,
                    Name = fa.Name,
                    MediaLink = fa.MediaLink,
                    CreatedDate = fa.CreatedDate
                }).ToList();

                returnData.Subtasks = (await _taskRepository.GetListSubtask(task.TaskId)).Select(st => new SubtaskViewModel { TaskId = st.TaskId, Name = st.Name, StatusName = st.TaskStatus.Name }).ToList();

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Request successfully!", Data = returnData });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Get all priorities of task
        /// </summary>
        [HttpGet("priority")]
        [Authorize]
        public async Task<IActionResult> GetPriorities()
        {
            try
            {
                var priorities = await _taskPriorityRepository.GetAllTaskPriority();
                var returnData = priorities.Select(p => new
                {
                    Id = p.TaskPriorityId,
                    Name = p.Name
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
        /// Get all dependencies of task
        /// </summary>
        [HttpGet("dependency")]
        [Authorize]
        public async Task<IActionResult> GetDependencies()
        {
            try
            {
                var dependencies = await _dependencyRepository.GetAllDependencies();
                var returnData = dependencies.Select(d => new
                {
                    Id = d.DependencyId,
                    Name = d.Name
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
        /// Get all statuses of task
        /// </summary>
        [HttpGet("status")]
        [Authorize]
        public async Task<IActionResult> GetStatuses()
        {
            try
            {
                var statuses = await _taskStatusRepository.GetAllTaskStatus();
                var returnData = statuses.Select(s => new
                {
                    Id = s.TaskStatusId,
                    Name = s.Name
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
        /// Update a status of task
        /// </summary>
        [HttpPut("status")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateTaskStatusRequestModel model)
        {
            try
            {
                var task = await _taskRepository.GetTaskByIdAsync(model.TaskId);

                if (task == null)
                {
                    return BadRequest("Can not find task information!");
                }

                task.TaskStatusId = model.StatusId;
                await _taskRepository.SaveChangeAsync();

                var authorization = Request.Headers[HeaderNames.Authorization];

                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var token = headerValue.Parameter;
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    var requestUserId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;
                    var assigneeUserId = task.UserId;

                    if (!string.IsNullOrEmpty(assigneeUserId) && !requestUserId.Equals(assigneeUserId))
                    {
                        var assigneeUserIds = new List<string>
                        {
                            assigneeUserId
                        };

                        var notificationTokens = (await _fbUserTokenRepository.GetTokensByUserIds(assigneeUserIds)).Select(n => n.Token).ToList();
                        var requestUser = await _userManager.FindByIdAsync(requestUserId);
                        var assigneeUser = await _userManager.FindByIdAsync(assigneeUserId);

                        if (requestUser != null && assigneeUser != null)
                        {
                            // Gửi notification đến user
                            var baseUrl = _configuration["ApplicationUrl"];

                            using (var httpClient = new HttpClient())
                            {
                                var notificationModel = new PushNotificationRequestModel
                                {
                                    Tokens = notificationTokens,
                                    Notification = new RequestModels.NotificationRequestModels.Notification
                                    {
                                        Title = "TMSO Software",
                                        Body = $"{requestUser.FullName} changed task status in '{task.Name}'",
                                        Icon = ""
                                    }
                                };

                                var body = new StringContent(JsonConvert.SerializeObject(notificationModel), System.Text.Encoding.UTF8, "application/json");
                                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                                await httpClient.PostAsync(baseUrl + "/api/notification/push", body);
                            }

                            // Lưu notification history
                            var messageObj = new
                            {
                                Title = $"{requestUser.FullName} changed task status in '{task.Name}'",
                                Content = $"TASK-{task.TaskId} | {task.Name}"
                            };
                            var message = JsonConvert.SerializeObject(messageObj);

                            var notification = new Models.Notification
                            {
                                Message = message,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            var notificationHistory = new UserNotification
                            {
                                User = assigneeUser,
                                Notification = notification,
                                IsViewed = false,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            await _notificationRepository.AddAsync(notificationHistory);
                            await _notificationRepository.SaveChangeAsync();
                        }
                    }
                }

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Request successfully!", Data = task });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Create a new comment
        /// </summary>
        [HttpPost("comment")]
        [Authorize]
        public async Task<IActionResult> CreateNewComment([FromBody] NewCommentRequestModel model)
        {
            try
            {
                var task = await _taskRepository.GetTaskByIdAsync(model.TaskId);

                if (task == null)
                {
                    return BadRequest("Can not find task information!");
                }

                var comment = new Comment
                {
                    UserId = model.UserId,
                    TaskId = model.TaskId,
                    Content = model.Content,
                    AttachFile = model.AttachFile,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                };

                await _commentRepository.AddAsync(comment);
                await _commentRepository.SaveChangeAsync();

                var authorization = Request.Headers[HeaderNames.Authorization];

                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var token = headerValue.Parameter;
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    var requestUserId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;
                    var assigneeUserId = task.UserId;

                    if (!string.IsNullOrEmpty(assigneeUserId) && !requestUserId.Equals(assigneeUserId))
                    {
                        var assigneeUserIds = new List<string>
                        {
                            assigneeUserId
                        };

                        var notificationTokens = (await _fbUserTokenRepository.GetTokensByUserIds(assigneeUserIds)).Select(n => n.Token).ToList();
                        var requestUser = await _userManager.FindByIdAsync(requestUserId);
                        var assigneeUser = await _userManager.FindByIdAsync(assigneeUserId);

                        if (requestUser != null && assigneeUser != null)
                        {
                            // Gửi notification đến user
                            var baseUrl = _configuration["ApplicationUrl"];

                            using (var httpClient = new HttpClient())
                            {
                                var notificationModel = new PushNotificationRequestModel
                                {
                                    Tokens = notificationTokens,
                                    Notification = new RequestModels.NotificationRequestModels.Notification
                                    {
                                        Title = "TMSO Software",
                                        Body = $"{requestUser.FullName} commented in '{task.Name}'",
                                        Icon = ""
                                    }
                                };

                                var body = new StringContent(JsonConvert.SerializeObject(notificationModel), System.Text.Encoding.UTF8, "application/json");
                                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                                await httpClient.PostAsync(baseUrl + "/api/notification/push", body);
                            }

                            // Lưu notification history
                            var messageObj = new
                            {
                                Title = $"{requestUser.FullName} commented in '{task.Name}'",
                                Content = $"TASK-{task.TaskId} | {task.Name}"
                            };
                            var message = JsonConvert.SerializeObject(messageObj);

                            var notification = new Models.Notification
                            {
                                Message = message,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            var notificationHistory = new UserNotification
                            {
                                User = assigneeUser,
                                Notification = notification,
                                IsViewed = false,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            await _notificationRepository.AddAsync(notificationHistory);
                            await _notificationRepository.SaveChangeAsync();
                        }
                    }
                }

                return Ok(new { StatusCode = HttpStatusCode.Created, Message = "Create new comment successfully!", Data = comment });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Update a comment
        /// </summary>
        [HttpPut("comment")]
        [Authorize]
        public async Task<IActionResult> UpdateComment([FromBody] UpdateCommentRequestModel model)
        {
            try
            {
                var comment = await _commentRepository.GetCommentByIdAsync(model.CommentId);

                if (comment == null)
                {
                    return BadRequest("Can not find comment information!");
                }

                var task = await _taskRepository.GetTaskByIdAsync(comment.TaskId);

                if (task == null)
                {
                    return BadRequest("Can not find task information!");
                }

                if (!string.IsNullOrEmpty(model.Content))
                {
                    comment.Content = model.Content;
                }

                if (!string.IsNullOrEmpty(model.AttachFile))
                {
                    comment.AttachFile = model.AttachFile;
                }

                if (model.IsDeleted)
                {
                    comment.IsDeleted = model.IsDeleted;
                }

                await _commentRepository.SaveChangeAsync();

                var authorization = Request.Headers[HeaderNames.Authorization];

                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var token = headerValue.Parameter;
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    var requestUserId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;
                    var assigneeUserId = task.UserId;

                    if (!string.IsNullOrEmpty(assigneeUserId) && !requestUserId.Equals(assigneeUserId))
                    {
                        var assigneeUserIds = new List<string>
                        {
                            assigneeUserId
                        };

                        var notificationTokens = (await _fbUserTokenRepository.GetTokensByUserIds(assigneeUserIds)).Select(n => n.Token).ToList();
                        var requestUser = await _userManager.FindByIdAsync(requestUserId);
                        var assigneeUser = await _userManager.FindByIdAsync(assigneeUserId);

                        if (requestUser != null && assigneeUser != null)
                        {
                            // Gửi notification đến user
                            var baseUrl = _configuration["ApplicationUrl"];
                            var bodyNotification = "";
                            if (!model.IsDeleted)
                            {
                                bodyNotification = $"{requestUser.FullName} updated a checklist in '{task.Name}'";
                            }
                            else
                            {
                                bodyNotification = $"{requestUser.FullName} deleted a comment in '{task.Name}'";
                            }

                            using (var httpClient = new HttpClient())
                            {
                                var notificationModel = new PushNotificationRequestModel
                                {
                                    Tokens = notificationTokens,
                                    Notification = new RequestModels.NotificationRequestModels.Notification
                                    {
                                        Title = "TMSO Software",
                                        Body = bodyNotification,
                                        Icon = ""
                                    }
                                };

                                var body = new StringContent(JsonConvert.SerializeObject(notificationModel), System.Text.Encoding.UTF8, "application/json");
                                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                                await httpClient.PostAsync(baseUrl + "/api/notification/push", body);
                            }

                            // Lưu notification history
                            var messageObj = new
                            {
                                Title = bodyNotification,
                                Content = $"TASK-{task.TaskId} | {task.Name}"
                            };
                            var message = JsonConvert.SerializeObject(messageObj);

                            var notification = new Models.Notification
                            {
                                Message = message,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            var notificationHistory = new UserNotification
                            {
                                User = assigneeUser,
                                Notification = notification,
                                IsViewed = false,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            await _notificationRepository.AddAsync(notificationHistory);
                            await _notificationRepository.SaveChangeAsync();
                        }
                    }
                }

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Update comment successfully!", Data = comment });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Update task name
        /// </summary>
        [HttpPut("name")]
        [Authorize]
        public async Task<IActionResult> UpdateTaskName([FromBody] UpdateTaskNameRequestModel model)
        {
            try
            {
                var task = await _taskRepository.GetTaskByIdAsync(model.TaskId);

                if (task == null)
                {
                    return BadRequest("Can not find task information!");
                }

                var oldTaskName = task.Name;

                if (!string.IsNullOrEmpty(model.Name))
                {
                    task.Name = model.Name.Trim();
                    await _taskRepository.SaveChangeAsync();
                }

                var authorization = Request.Headers[HeaderNames.Authorization];

                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var token = headerValue.Parameter;
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    var requestUserId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;
                    var assigneeUserId = task.UserId;

                    if (!string.IsNullOrEmpty(assigneeUserId) && !requestUserId.Equals(assigneeUserId))
                    {
                        var assigneeUserIds = new List<string>
                        {
                            assigneeUserId
                        };

                        var notificationTokens = (await _fbUserTokenRepository.GetTokensByUserIds(assigneeUserIds)).Select(n => n.Token).ToList();
                        var requestUser = await _userManager.FindByIdAsync(requestUserId);
                        var assigneeUser = await _userManager.FindByIdAsync(assigneeUserId);

                        if (requestUser != null && assigneeUser != null)
                        {
                            // Gửi notification đến user
                            var baseUrl = _configuration["ApplicationUrl"];

                            using (var httpClient = new HttpClient())
                            {
                                var notificationModel = new PushNotificationRequestModel
                                {
                                    Tokens = notificationTokens,
                                    Notification = new RequestModels.NotificationRequestModels.Notification
                                    {
                                        Title = "TMSO Software",
                                        Body = $"{requestUser.FullName} updated task name to '{task.Name}'",
                                        Icon = ""
                                    }
                                };

                                var body = new StringContent(JsonConvert.SerializeObject(notificationModel), System.Text.Encoding.UTF8, "application/json");
                                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                                await httpClient.PostAsync(baseUrl + "/api/notification/push", body);
                            }

                            // Lưu notification history
                            var messageObj = new
                            {
                                Title = $"{requestUser.FullName} updated task name to '{task.Name}'",
                                Content = $"TASK-{task.TaskId} | {oldTaskName}"
                            };
                            var message = JsonConvert.SerializeObject(messageObj);

                            var notification = new Models.Notification
                            {
                                Message = message,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            var notificationHistory = new UserNotification
                            {
                                User = assigneeUser,
                                Notification = notification,
                                IsViewed = false,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            await _notificationRepository.AddAsync(notificationHistory);
                            await _notificationRepository.SaveChangeAsync();
                        }
                    }
                }

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Update task name successfully!", Data = new { } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Create a new checklist
        /// </summary>
        [HttpPost("checklist")]
        [Authorize]
        public async Task<IActionResult> CreateNewChecklist([FromBody] NewChecklistRequestModel model)
        {
            try
            {
                var task = await _taskRepository.GetTaskByIdAsync(model.TaskId);

                if (task == null)
                {
                    return BadRequest("Can not find task information!");
                }

                var checklist = new Checklist
                {
                    Name = model.Name.Trim(),
                    IsCompleted = model.IsCompleted,
                    TaskId = model.TaskId,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                };

                await _checklistRepository.AddAsync(checklist);
                await _checklistRepository.SaveChangeAsync();

                var authorization = Request.Headers[HeaderNames.Authorization];

                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var token = headerValue.Parameter;
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    var requestUserId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;
                    var assigneeUserId = task.UserId;

                    if (!string.IsNullOrEmpty(assigneeUserId) && !requestUserId.Equals(assigneeUserId))
                    {
                        var assigneeUserIds = new List<string>
                        {
                            assigneeUserId
                        };

                        var notificationTokens = (await _fbUserTokenRepository.GetTokensByUserIds(assigneeUserIds)).Select(n => n.Token).ToList();
                        var requestUser = await _userManager.FindByIdAsync(requestUserId);
                        var assigneeUser = await _userManager.FindByIdAsync(assigneeUserId);

                        if (requestUser != null && assigneeUser != null)
                        {
                            // Gửi notification đến user
                            var baseUrl = _configuration["ApplicationUrl"];

                            using (var httpClient = new HttpClient())
                            {
                                var notificationModel = new PushNotificationRequestModel
                                {
                                    Tokens = notificationTokens,
                                    Notification = new RequestModels.NotificationRequestModels.Notification
                                    {
                                        Title = "TMSO Software",
                                        Body = $"{requestUser.FullName} created a new checklist in '{task.Name}'",
                                        Icon = ""
                                    }
                                };

                                var body = new StringContent(JsonConvert.SerializeObject(notificationModel), System.Text.Encoding.UTF8, "application/json");
                                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                                await httpClient.PostAsync(baseUrl + "/api/notification/push", body);
                            }

                            // Lưu notification history
                            var messageObj = new
                            {
                                Title = $"{requestUser.FullName} created a new checklist in '{task.Name}'",
                                Content = $"TASK-{task.TaskId} | {task.Name}"
                            };
                            var message = JsonConvert.SerializeObject(messageObj);

                            var notification = new Models.Notification
                            {
                                Message = message,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            var notificationHistory = new UserNotification
                            {
                                User = assigneeUser,
                                Notification = notification,
                                IsViewed = false,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            await _notificationRepository.AddAsync(notificationHistory);
                            await _notificationRepository.SaveChangeAsync();
                        }
                    }
                }

                return Ok(new { StatusCode = HttpStatusCode.Created, Message = "Create new checklist successfully!", Data = new { checklist.ChecklistId, checklist.Name, checklist.IsCompleted, checklist.CreatedDate, checklist.ModifiedDate } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Update a checklist
        /// </summary>
        [HttpPut("checklist")]
        [Authorize]
        public async Task<IActionResult> UpdateChecklist([FromBody] UpdateChecklistRequestModel model)
        {
            try
            {
                var checklist = await _checklistRepository.GetChecklistByIdAsync(model.ChecklistId);

                if (checklist == null)
                {
                    return BadRequest("Can not find checklist information!");
                }

                var task = await _taskRepository.GetTaskByIdAsync(checklist.TaskId);

                if (task == null)
                {
                    return BadRequest("Can not find task information!");
                }

                if (!string.IsNullOrEmpty(model.Name))
                {
                    checklist.Name = model.Name.Trim();
                }

                checklist.IsCompleted = model.IsCompleted;

                if (model.IsDeleted)
                {
                    checklist.IsDeleted = model.IsDeleted;
                }

                await _checklistRepository.SaveChangeAsync();

                var authorization = Request.Headers[HeaderNames.Authorization];

                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var token = headerValue.Parameter;
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    var requestUserId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;
                    var assigneeUserId = task.UserId;

                    if (!string.IsNullOrEmpty(assigneeUserId) && !requestUserId.Equals(assigneeUserId))
                    {
                        var assigneeUserIds = new List<string>
                        {
                            assigneeUserId
                        };

                        var notificationTokens = (await _fbUserTokenRepository.GetTokensByUserIds(assigneeUserIds)).Select(n => n.Token).ToList();
                        var requestUser = await _userManager.FindByIdAsync(requestUserId);
                        var assigneeUser = await _userManager.FindByIdAsync(assigneeUserId);

                        if (requestUser != null && assigneeUser != null)
                        {
                            // Gửi notification đến user
                            var baseUrl = _configuration["ApplicationUrl"];
                            var bodyNotification = "";
                            if (!model.IsDeleted)
                            {
                                bodyNotification = $"{requestUser.FullName} updated a checklist in '{task.Name}'";
                            }
                            else
                            {
                                bodyNotification = $"{requestUser.FullName} deleted a checklist in '{task.Name}'";
                            }

                            using (var httpClient = new HttpClient())
                            {
                                var notificationModel = new PushNotificationRequestModel
                                {
                                    Tokens = notificationTokens,
                                    Notification = new RequestModels.NotificationRequestModels.Notification
                                    {
                                        Title = "TMSO Software",
                                        Body = bodyNotification,
                                        Icon = ""
                                    }
                                };

                                var body = new StringContent(JsonConvert.SerializeObject(notificationModel), System.Text.Encoding.UTF8, "application/json");
                                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                                await httpClient.PostAsync(baseUrl + "/api/notification/push", body);
                            }

                            // Lưu notification history
                            var messageObj = new
                            {
                                Title = bodyNotification,
                                Content = $"TASK-{task.TaskId} | {task.Name}"
                            };
                            var message = JsonConvert.SerializeObject(messageObj);

                            var notification = new Models.Notification
                            {
                                Message = message,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            var notificationHistory = new UserNotification
                            {
                                User = assigneeUser,
                                Notification = notification,
                                IsViewed = false,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            await _notificationRepository.AddAsync(notificationHistory);
                            await _notificationRepository.SaveChangeAsync();
                        }
                    }
                }

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Update checklist successfully!", Data = new { checklist.ChecklistId, checklist.Name, checklist.IsCompleted } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Create a new subtask
        /// </summary>
        [HttpPost("subtask")]
        [Authorize]
        public async Task<IActionResult> CreateNewSubtask([FromBody] NewSubtaskRequestModel model)
        {
            try
            {
                var subtask = await _taskRepository.GetTaskById(model.SubtaskId);
                var task = await _taskRepository.GetTaskById(model.TaskId);

                if (subtask == null || task == null)
                {
                    return BadRequest("Can not find task information!");
                }

                subtask.ParentId = model.TaskId;
                // TODO: Xử lý trường hợp xóa hết subtask
                task.ParentId = 0;

                await _taskRepository.SaveChangeAsync();

                var authorization = Request.Headers[HeaderNames.Authorization];

                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var token = headerValue.Parameter;
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    var requestUserId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;
                    var assigneeUserId = task.UserId;

                    if (!string.IsNullOrEmpty(assigneeUserId) && !requestUserId.Equals(assigneeUserId))
                    {
                        var assigneeUserIds = new List<string>
                        {
                            assigneeUserId
                        };

                        var notificationTokens = (await _fbUserTokenRepository.GetTokensByUserIds(assigneeUserIds)).Select(n => n.Token).ToList();
                        var requestUser = await _userManager.FindByIdAsync(requestUserId);
                        var assigneeUser = await _userManager.FindByIdAsync(assigneeUserId);

                        if (requestUser != null && assigneeUser != null)
                        {
                            // Gửi notification đến user
                            var baseUrl = _configuration["ApplicationUrl"];

                            using (var httpClient = new HttpClient())
                            {
                                var notificationModel = new PushNotificationRequestModel
                                {
                                    Tokens = notificationTokens,
                                    Notification = new RequestModels.NotificationRequestModels.Notification
                                    {
                                        Title = "TMSO Software",
                                        Body = $"{requestUser.FullName} create new subtask '{subtask.Name}'",
                                        Icon = ""
                                    }
                                };

                                var body = new StringContent(JsonConvert.SerializeObject(notificationModel), System.Text.Encoding.UTF8, "application/json");
                                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                                await httpClient.PostAsync(baseUrl + "/api/notification/push", body);
                            }

                            // Lưu notification history
                            var messageObj = new
                            {
                                Title = $"{requestUser.FullName} create new subtask '{subtask.Name}'",
                                Content = ""
                            };
                            var message = JsonConvert.SerializeObject(messageObj);

                            var notification = new Models.Notification
                            {
                                Message = message,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            var notificationHistory = new UserNotification
                            {
                                User = assigneeUser,
                                Notification = notification,
                                IsViewed = false,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            await _notificationRepository.AddAsync(notificationHistory);
                            await _notificationRepository.SaveChangeAsync();
                        }
                    }
                }
                return Ok(new { StatusCode = HttpStatusCode.Created, Message = "Create new subtask successfully!", Data = new { subtask.TaskId, subtask.Name, StatusName = subtask.TaskStatus.Name } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Update a subtask
        /// </summary>
        [HttpPut("subtask")]
        [Authorize]
        public async Task<IActionResult> UpdateSubtask([FromBody] UpdateSubtaskRequestModel model)
        {
            try
            {
                var currentSubtask = await _taskRepository.GetTaskByIdAsync(model.CurrentSubtaskId);
                var updateSubtask = await _taskRepository.GetTaskByIdAsync(model.UpdateSubtaskId);
                var task = await _taskRepository.GetTaskByIdAsync(model.TaskId);

                if (currentSubtask == null || updateSubtask == null || task == null)
                {
                    return BadRequest("Can not find task information!");
                }

                currentSubtask.ParentId = null;
                updateSubtask.ParentId = model.TaskId;
                await _taskRepository.SaveChangeAsync();

                var authorization = Request.Headers[HeaderNames.Authorization];

                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var token = headerValue.Parameter;
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    var requestUserId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;
                    var assigneeUserId = task.UserId;

                    if (!string.IsNullOrEmpty(assigneeUserId) && !requestUserId.Equals(assigneeUserId))
                    {
                        var assigneeUserIds = new List<string>
                        {
                            assigneeUserId
                        };

                        var notificationTokens = (await _fbUserTokenRepository.GetTokensByUserIds(assigneeUserIds)).Select(n => n.Token).ToList();
                        var requestUser = await _userManager.FindByIdAsync(requestUserId);
                        var assigneeUser = await _userManager.FindByIdAsync(assigneeUserId);

                        if (requestUser != null && assigneeUser != null)
                        {
                            // Gửi notification đến user
                            var baseUrl = _configuration["ApplicationUrl"];

                            using (var httpClient = new HttpClient())
                            {
                                var notificationModel = new PushNotificationRequestModel
                                {
                                    Tokens = notificationTokens,
                                    Notification = new RequestModels.NotificationRequestModels.Notification
                                    {
                                        Title = "TMSO Software",
                                        Body = $"{requestUser.FullName} updated a subtask",
                                        Icon = ""
                                    }
                                };

                                var body = new StringContent(JsonConvert.SerializeObject(notificationModel), System.Text.Encoding.UTF8, "application/json");
                                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                                await httpClient.PostAsync(baseUrl + "/api/notification/push", body);
                            }

                            // Lưu notification history
                            var messageObj = new
                            {
                                Title = $"{requestUser.FullName} updated a subtask",
                                Content = ""
                            };
                            var message = JsonConvert.SerializeObject(messageObj);

                            var notification = new Models.Notification
                            {
                                Message = message,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            var notificationHistory = new UserNotification
                            {
                                User = assigneeUser,
                                Notification = notification,
                                IsViewed = false,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            await _notificationRepository.AddAsync(notificationHistory);
                            await _notificationRepository.SaveChangeAsync();
                        }
                    }
                }
                return Ok(new { StatusCode = HttpStatusCode.Created, Message = "Create new subtask successfully!", Data = new { } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }


        /// <summary>
        /// Get list task of a project for searching subtask
        /// </summary>
        [HttpGet("subtask")]
        [Authorize]
        public async Task<IActionResult> GetListSubtask([FromQuery] int projectId)
        {
            try
            {
                var subtasks = await _taskRepository.GetSubtaskByProject(projectId);

                var returnData = subtasks.Select(s => new SubtaskViewModel
                {
                    TaskId = s.TaskId,
                    Name = s.Name
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
        /// Get list task of a project for searching dependency
        /// </summary>
        [HttpGet("dependency-task")]
        [Authorize]
        public async Task<IActionResult> GetListTask([FromQuery] int projectId)
        {
            try
            {
                var tasks = await _taskRepository.GetAllTaskInProject(projectId);

                var returnData = tasks.Select(s => new SubtaskViewModel
                {
                    TaskId = s.TaskId,
                    Name = s.Name
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
        /// Delete a subtask
        /// </summary>
        [HttpDelete("subtask")]
        [Authorize]
        public async Task<IActionResult> DeleteSubTask(int id)
        {
            try
            {
                var task = await _taskRepository.GetTaskById(id);
                if (task == null)
                {
                    return BadRequest("An error occurred during deleting task!");
                }

                var oldTaskParentId = task.ParentId;
                var parentTask = await _taskRepository.GetTaskById(task.ParentId.Value);
                task.ParentId = null;
                await _taskRepository.SaveChangeAsync();
                
                // Xử lý trường hợp xóa hết subtask của task cha
                var subtasks = await _taskRepository.GetListSubtask(oldTaskParentId.Value);
                if (subtasks.Count == 0)
                {
                    var taskParent = await _taskRepository.GetTaskByIdAsync(oldTaskParentId.Value);
                    taskParent.ParentId = null;
                    await _taskRepository.SaveChangeAsync();
                }

                var authorization = Request.Headers[HeaderNames.Authorization];

                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var token = headerValue.Parameter;
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    var requestUserId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;
                    var assigneeUserId = parentTask.UserId;

                    if (!string.IsNullOrEmpty(assigneeUserId) && !requestUserId.Equals(assigneeUserId))
                    {
                        var assigneeUserIds = new List<string>
                        {
                            assigneeUserId
                        };

                        var notificationTokens = (await _fbUserTokenRepository.GetTokensByUserIds(assigneeUserIds)).Select(n => n.Token).ToList();
                        var requestUser = await _userManager.FindByIdAsync(requestUserId);
                        var assigneeUser = await _userManager.FindByIdAsync(assigneeUserId);

                        if (requestUser != null && assigneeUser != null)
                        {
                            // Gửi notification đến user
                            var baseUrl = _configuration["ApplicationUrl"];

                            using (var httpClient = new HttpClient())
                            {
                                var notificationModel = new PushNotificationRequestModel
                                {
                                    Tokens = notificationTokens,
                                    Notification = new RequestModels.NotificationRequestModels.Notification
                                    {
                                        Title = "TMSO Software",
                                        Body = $"{requestUser.FullName} delete subtask",
                                        Icon = ""
                                    }
                                };

                                var body = new StringContent(JsonConvert.SerializeObject(notificationModel), System.Text.Encoding.UTF8, "application/json");
                                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                                await httpClient.PostAsync(baseUrl + "/api/notification/push", body);
                            }

                            // Lưu notification history
                            var messageObj = new
                            {
                                Title = $"{requestUser.FullName} delete subtask",
                                Content = ""
                            };
                            var message = JsonConvert.SerializeObject(messageObj);

                            var notification = new Models.Notification
                            {
                                Message = message,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            var notificationHistory = new UserNotification
                            {
                                User = assigneeUser,
                                Notification = notification,
                                IsViewed = false,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            await _notificationRepository.AddAsync(notificationHistory);
                            await _notificationRepository.SaveChangeAsync();
                        }
                    }
                }
                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Delete subtask successfully!", Data = new { } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }


        /// <summary>
        /// Update task position in section
        /// </summary>
        [HttpPut("task-position")]
        [Authorize()]
        public async Task<IActionResult> UpdateTaskPosition([FromBody] UpdateTaskPositionRequestModel model)
        {
            try
            {
                var task = await _taskRepository.GetTaskById(model.TaskId);
                Lists updatedList = null;

                if (task == null)
                {
                    return BadRequest("Can not find task information!");
                }
                
                if (task.ListId != model.ListId)
                {
                    updatedList = await _listRepository.GetListById(model.ListId);

                    if (updatedList == null)
                    {
                        return BadRequest("Can not find list information!");
                    }

                    task.ListId = model.ListId;
                    task.TaskStatusId = AppConstants.TaskStatus.NONE;

                    var subtasks = await _taskRepository.GetListSubtask(task.TaskId);

                    if (subtasks.Count != 0)
                    {
                        var changeSubtasks = subtasks.Where(s => s.List.Index < updatedList.Index).ToList();

                        foreach (var subtask in changeSubtasks)
                        {
                            subtask.ListId = model.ListId;
                            subtask.TaskStatusId = AppConstants.TaskStatus.NONE;
                        }
                    }  
                }
                
                await _taskRepository.SaveChangeAsync();

                var authorization = Request.Headers[HeaderNames.Authorization];

                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var token = headerValue.Parameter;
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    var requestUserId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;

                    var assigneeUserIds = new List<string>
                    {
                        task.UserId
                    };

                    var notificationTokens = (await _fbUserTokenRepository.GetTokensByUserIds(assigneeUserIds)).Select(n => n.Token).ToList();
                    var requestUser = await _userManager.FindByIdAsync(requestUserId);
                    var assigneeUser = await _userManager.FindByIdAsync(task.UserId);

                    if (requestUser != null && assigneeUser != null)
                    {
                        // Gửi notification đến user
                        var baseUrl = _configuration["ApplicationUrl"];

                        using (var httpClient = new HttpClient())
                        {
                            var notificationModel = new PushNotificationRequestModel
                            {
                                Tokens = notificationTokens,
                                Notification = new RequestModels.NotificationRequestModels.Notification
                                {
                                    Title = "TMSO Software",
                                    Body = $"{requestUser.FullName} changed a task to '{updatedList.Name}' section!",
                                    Icon = ""
                                }
                            };

                            var body = new StringContent(JsonConvert.SerializeObject(notificationModel), System.Text.Encoding.UTF8, "application/json");
                            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                            await httpClient.PostAsync(baseUrl + "/api/notification/push", body);
                        }

                        // Lưu notification history
                        var messageObj = new
                        {
                            Title = $"{requestUser.FullName} changed a task to '{updatedList.Name}' section!",
                            Content = $"TASK-{task.TaskId} | {task.Name}"
                        };
                        var message = JsonConvert.SerializeObject(messageObj);

                        var notification = new Models.Notification
                        {
                            Message = message,
                            CreatedDate = DateTime.Now,
                            IsDeleted = false
                        };

                        var notificationHistory = new UserNotification
                        {
                            User = assigneeUser,
                            Notification = notification,
                            IsViewed = false,
                            CreatedDate = DateTime.Now,
                            IsDeleted = false
                        };

                        await _notificationRepository.AddAsync(notificationHistory);
                        await _notificationRepository.SaveChangeAsync();
                    }
                }

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Update successfully!", Data = task });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Get list task of list view screen
        /// </summary>
        [HttpGet("listview")]
        [Authorize]
        public async Task<IActionResult> GetTaskListView([FromQuery] int projectId)
        {
            try
            {
                var tasks = await _taskRepository.GetTaskListView(projectId);
               
                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Request successfully!", Data = tasks });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Update information which is assigneed
        /// </summary>
        [HttpPut("informationn-assignee")]
        [Authorize]
        public async Task<IActionResult> UpdateInformationAssignee([FromBody] UpdateInformationAssigneeRequestModel model)
        {
            try
            {
                var task = await _taskRepository.GetTaskById(model.TaskId);

                if (task == null)
                {
                    return BadRequest("An error occurred during updating task!");
                }

                task.UserId = model.UserId;
                await _taskRepository.SaveChangeAsync();

                var authorization = Request.Headers[HeaderNames.Authorization];

                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var token = headerValue.Parameter;
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    var requestUserId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;

                    var assigneeUserIds = new List<string>
                    {
                        task.UserId
                    };

                    var notificationTokens = (await _fbUserTokenRepository.GetTokensByUserIds(assigneeUserIds)).Select(n => n.Token).ToList();
                    var requestUser = await _userManager.FindByIdAsync(requestUserId);
                    var assigneeUser = await _userManager.FindByIdAsync(task.UserId);

                    if (requestUser != null && assigneeUser != null)
                    {
                        // Gửi notification đến user
                        var baseUrl = _configuration["ApplicationUrl"];

                        using (var httpClient = new HttpClient())
                        {
                            var notificationModel = new PushNotificationRequestModel
                            {
                                Tokens = notificationTokens,
                                Notification = new RequestModels.NotificationRequestModels.Notification
                                {
                                    Title = "TMSO Software",
                                    Body = $"{requestUser.FullName} assigned a task to you",
                                    Icon = ""
                                }
                            };

                            var body = new StringContent(JsonConvert.SerializeObject(notificationModel), System.Text.Encoding.UTF8, "application/json");
                            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                            await httpClient.PostAsync(baseUrl + "/api/notification/push", body);
                        }

                        // Lưu notification history
                        var messageObj = new
                        {
                            Title = $"{requestUser.FullName} assigned a task to you",
                            Content = $"TASK-{task.TaskId} | {task.Name}"
                        };
                        var message = JsonConvert.SerializeObject(messageObj);

                        var notification = new Models.Notification
                        {
                            Message = message,
                            CreatedDate = DateTime.Now,
                            IsDeleted = false
                        };

                        var notificationHistory = new UserNotification
                        {
                            User = assigneeUser,
                            Notification = notification,
                            IsViewed = false,
                            CreatedDate = DateTime.Now,
                            IsDeleted = false
                        };

                        await _notificationRepository.AddAsync(notificationHistory);
                        await _notificationRepository.SaveChangeAsync();
                    }
                }

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Update successfully!", Data = task });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }

        }

        /// <summary>
        /// Update priority for task
        /// </summary>
        [HttpPut("priority")]
        [Authorize]
        public async Task<IActionResult> UpdatePriority([FromBody] UpdatePriorityRequestModel model)
        {
            try
            {
                var task = await _taskRepository.GetTaskByIdAsync(model.TaskId);
                if (task == null)
                {
                    return BadRequest("Can not find task information!");
                }
                var oldPriority = task.TaskPriorityId;
                task.TaskPriorityId = model.PriorityId;

                await _taskRepository.SaveChangeAsync();

                var authorization = Request.Headers[HeaderNames.Authorization];

                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var token = headerValue.Parameter;
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    var requestUserId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;
                    var assigneeUserId = task.UserId;

                    if (!string.IsNullOrEmpty(assigneeUserId) && !requestUserId.Equals(assigneeUserId))
                    {
                        var assigneeUserIds = new List<string>
                        {
                            assigneeUserId
                        };

                        var notificationTokens = (await _fbUserTokenRepository.GetTokensByUserIds(assigneeUserIds)).Select(n => n.Token).ToList();
                        var requestUser = await _userManager.FindByIdAsync(requestUserId);
                        var assigneeUser = await _userManager.FindByIdAsync(assigneeUserId);

                        if (requestUser != null && assigneeUser != null)
                        {
                            // Gửi notification đến user
                            var baseUrl = _configuration["ApplicationUrl"];

                            using (var httpClient = new HttpClient())
                            {
                                var notificationModel = new PushNotificationRequestModel
                                {
                                    Tokens = notificationTokens,
                                    Notification = new RequestModels.NotificationRequestModels.Notification
                                    {
                                        Title = "TMSO Software",
                                        Body = $"{requestUser.FullName} updated priority",
                                        Icon = ""
                                    }
                                };

                                var body = new StringContent(JsonConvert.SerializeObject(notificationModel), System.Text.Encoding.UTF8, "application/json");
                                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                                await httpClient.PostAsync(baseUrl + "/api/notification/push", body);
                            }

                            // Lưu notification history
                            var messageObj = new
                            {
                                Title = $"{requestUser.FullName} updated priority",
                                Content = $"TASK-{task.TaskId} | {oldPriority}"
                            };
                            var message = JsonConvert.SerializeObject(messageObj);

                            var notification = new Models.Notification
                            {
                                Message = message,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            var notificationHistory = new UserNotification
                            {
                                User = assigneeUser,
                                Notification = notification,
                                IsViewed = false,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            await _notificationRepository.AddAsync(notificationHistory);
                            await _notificationRepository.SaveChangeAsync();
                        }
                    }
                }
                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Update successfully!", Data = new { } });
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Update section for task
        /// </summary>
        [HttpPut("section-taskdetail")]
        [Authorize]
        public async Task<IActionResult> UpdateSection([FromBody] UpdateSectionTaskDetailRequestModel model)
        {
            try
            {
                var task = await _taskRepository.GetTaskByIdAsync(model.TaskId);
                if (task == null)
                {
                    return BadRequest("Can not find task information!");
                }
                var oldList = task.ListId;
                task.ListId = model.ListId;

                await _taskRepository.SaveChangeAsync();

                var authorization = Request.Headers[HeaderNames.Authorization];

                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var token = headerValue.Parameter;
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    var requestUserId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;
                    var assigneeUserId = task.UserId;

                    if (!string.IsNullOrEmpty(assigneeUserId) && !requestUserId.Equals(assigneeUserId))
                    {
                        var assigneeUserIds = new List<string>
                        {
                            assigneeUserId
                        };

                        var notificationTokens = (await _fbUserTokenRepository.GetTokensByUserIds(assigneeUserIds)).Select(n => n.Token).ToList();
                        var requestUser = await _userManager.FindByIdAsync(requestUserId);
                        var assigneeUser = await _userManager.FindByIdAsync(assigneeUserId);

                        if (requestUser != null && assigneeUser != null)
                        {
                            // Gửi notification đến user
                            var baseUrl = _configuration["ApplicationUrl"];

                            using (var httpClient = new HttpClient())
                            {
                                var notificationModel = new PushNotificationRequestModel
                                {
                                    Tokens = notificationTokens,
                                    Notification = new RequestModels.NotificationRequestModels.Notification
                                    {
                                        Title = "TMSO Software",
                                        Body = $"{requestUser.FullName} updated section",
                                        Icon = ""
                                    }
                                };

                                var body = new StringContent(JsonConvert.SerializeObject(notificationModel), System.Text.Encoding.UTF8, "application/json");
                                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                                await httpClient.PostAsync(baseUrl + "/api/notification/push", body);
                            }

                            // Lưu notification history
                            var messageObj = new
                            {
                                Title = $"{requestUser.FullName} updated section",
                                Content = $"TASK-{task.TaskId} | {oldList}"
                            };
                            var message = JsonConvert.SerializeObject(messageObj);

                            var notification = new Models.Notification
                            {
                                Message = message,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            var notificationHistory = new UserNotification
                            {
                                User = assigneeUser,
                                Notification = notification,
                                IsViewed = false,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            await _notificationRepository.AddAsync(notificationHistory);
                            await _notificationRepository.SaveChangeAsync();
                        }
                    }
                }
                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Update successfully!", Data = new { } });
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }


        /// <summary>
        /// Update description for task
        /// </summary>
        [HttpPut("description")]
        [Authorize]
        public async Task<IActionResult> UpdateDescription([FromBody] UpdateDescriptionRequestModel model)
        {
            try
            {
                var task = await _taskRepository.GetTaskByIdAsync(model.TaskId);
                if (task == null)
                {
                    return BadRequest("Can not find task information!");
                }
                task.Description = model.Description;

                await _taskRepository.SaveChangeAsync();

                var authorization = Request.Headers[HeaderNames.Authorization];

                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var token = headerValue.Parameter;
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    var requestUserId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;
                    var assigneeUserId = task.UserId;

                    if (!string.IsNullOrEmpty(assigneeUserId) && !requestUserId.Equals(assigneeUserId))
                    {
                        var assigneeUserIds = new List<string>
                        {
                            assigneeUserId
                        };

                        var notificationTokens = (await _fbUserTokenRepository.GetTokensByUserIds(assigneeUserIds)).Select(n => n.Token).ToList();
                        var requestUser = await _userManager.FindByIdAsync(requestUserId);
                        var assigneeUser = await _userManager.FindByIdAsync(assigneeUserId);

                        if (requestUser != null && assigneeUser != null)
                        {
                            // Gửi notification đến user
                            var baseUrl = _configuration["ApplicationUrl"];

                            using (var httpClient = new HttpClient())
                            {
                                var notificationModel = new PushNotificationRequestModel
                                {
                                    Tokens = notificationTokens,
                                    Notification = new RequestModels.NotificationRequestModels.Notification
                                    {
                                        Title = "TMSO Software",
                                        Body = $"{requestUser.FullName} updated description for task",
                                        Icon = ""
                                    }
                                };

                                var body = new StringContent(JsonConvert.SerializeObject(notificationModel), System.Text.Encoding.UTF8, "application/json");
                                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                                await httpClient.PostAsync(baseUrl + "/api/notification/push", body);
                            }

                            // Lưu notification history
                            var messageObj = new
                            {
                                Title = $"{requestUser.FullName} updated description for task",
                                Content = $"TASK-{task.TaskId} | {task.Description}"
                            };
                            var message = JsonConvert.SerializeObject(messageObj);

                            var notification = new Models.Notification
                            {
                                Message = message,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            var notificationHistory = new UserNotification
                            {
                                User = assigneeUser,
                                Notification = notification,
                                IsViewed = false,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            await _notificationRepository.AddAsync(notificationHistory);
                            await _notificationRepository.SaveChangeAsync();
                        }
                    }
                }
                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Update successfully!", Data = new { } });
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Update active/deactive for task
        /// </summary>
        [HttpPut("active")]
        [Authorize]
        public async Task<IActionResult> UpdateActive([FromBody] UpdateActiveRequestModel model)
        {
            try
            {
                var task = await _taskRepository.GetTaskById(model.TaskId);
                if (task == null)
                {
                    return BadRequest("Can not find task information!");
                }

                task.IsActive = model.IsActive;

                await _taskRepository.SaveChangeAsync();

                var authorization = Request.Headers[HeaderNames.Authorization];

                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var token = headerValue.Parameter;
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    var requestUserId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;
                    var assigneeUserId = task.UserId;

                    if (!string.IsNullOrEmpty(assigneeUserId) && !requestUserId.Equals(assigneeUserId))
                    {
                        var assigneeUserIds = new List<string>
                        {
                            assigneeUserId
                        };

                        var notificationTokens = (await _fbUserTokenRepository.GetTokensByUserIds(assigneeUserIds)).Select(n => n.Token).ToList();
                        var requestUser = await _userManager.FindByIdAsync(requestUserId);
                        var assigneeUser = await _userManager.FindByIdAsync(assigneeUserId);

                        if (requestUser != null && assigneeUser != null)
                        {
                            // Gửi notification đến user
                            var baseUrl = _configuration["ApplicationUrl"];

                            using (var httpClient = new HttpClient())
                            {
                                var notificationModel = new PushNotificationRequestModel
                                {
                                    Tokens = notificationTokens,
                                    Notification = new RequestModels.NotificationRequestModels.Notification
                                    {
                                        Title = "TMSO Software",
                                        Body = $"{requestUser.FullName} active task '{task.Name}'",
                                        Icon = ""
                                    }
                                };

                                var body = new StringContent(JsonConvert.SerializeObject(notificationModel), System.Text.Encoding.UTF8, "application/json");
                                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                                await httpClient.PostAsync(baseUrl + "/api/notification/push", body);
                            }

                            // Lưu notification history
                            var messageObj = new
                            {
                                Title = $"{requestUser.FullName} active task '{task.Name}'",
                                Content = $"TASK-{task.TaskId} | {task.IsActive}"
                            };
                            var message = JsonConvert.SerializeObject(messageObj);

                            var notification = new Models.Notification
                            {
                                Message = message,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            var notificationHistory = new UserNotification
                            {
                                User = assigneeUser,
                                Notification = notification,
                                IsViewed = false,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            await _notificationRepository.AddAsync(notificationHistory);
                            await _notificationRepository.SaveChangeAsync();
                        }
                    }
                }

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Update successfully!", Data = new { } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Update duedate for task
        /// </summary>
        [HttpPut("duedate")]
        [Authorize]
        public async Task<IActionResult> UpdateDuedate([FromBody] UpdateDuedateRequestModel model)
        {
            try
            {
                var task = await _taskRepository.GetTaskByIdAsync(model.TaskId);
                if (task == null)
                {
                    return BadRequest("Can not find task information!");
                }

                task.DueDate = model.DueDate;

                await _taskRepository.SaveChangeAsync();
                var authorization = Request.Headers[HeaderNames.Authorization];

                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var token = headerValue.Parameter;
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    var requestUserId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;
                    var assigneeUserId = task.UserId;

                    if (!string.IsNullOrEmpty(assigneeUserId) && !requestUserId.Equals(assigneeUserId))
                    {
                        var assigneeUserIds = new List<string>
                        {
                            assigneeUserId
                        };

                        var notificationTokens = (await _fbUserTokenRepository.GetTokensByUserIds(assigneeUserIds)).Select(n => n.Token).ToList();
                        var requestUser = await _userManager.FindByIdAsync(requestUserId);
                        var assigneeUser = await _userManager.FindByIdAsync(assigneeUserId);

                        if (requestUser != null && assigneeUser != null)
                        {
                            // Gửi notification đến user
                            var baseUrl = _configuration["ApplicationUrl"];

                            using (var httpClient = new HttpClient())
                            {
                                var notificationModel = new PushNotificationRequestModel
                                {
                                    Tokens = notificationTokens,
                                    Notification = new RequestModels.NotificationRequestModels.Notification
                                    {
                                        Title = "TMSO Software",
                                        Body = $"{requestUser.FullName} updated end date for task '{task.Name}'",
                                        Icon = ""
                                    }
                                };

                                var body = new StringContent(JsonConvert.SerializeObject(notificationModel), System.Text.Encoding.UTF8, "application/json");
                                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                                await httpClient.PostAsync(baseUrl + "/api/notification/push", body);
                            }

                            // Lưu notification history
                            var messageObj = new
                            {
                                Title = $"{requestUser.FullName} updated end date for task '{task.Name}'",
                                Content = $"TASK-{task.TaskId} | {task.DueDate}"
                            };
                            var message = JsonConvert.SerializeObject(messageObj);

                            var notification = new Models.Notification
                            {
                                Message = message,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            var notificationHistory = new UserNotification
                            {
                                User = assigneeUser,
                                Notification = notification,
                                IsViewed = false,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            await _notificationRepository.AddAsync(notificationHistory);
                            await _notificationRepository.SaveChangeAsync();
                        }
                    }
                }

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Update successfully!", Data = task });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Update start for task
        /// </summary>
        [HttpPut("startdate")]
        [Authorize]
        public async Task<IActionResult> UpdateStartdate([FromBody] UpdateStartdateRequestModel model)
        {
            try
            {
                var task = await _taskRepository.GetTaskByIdAsync(model.TaskId);
                if (task == null)
                {
                    return BadRequest("Can not find task information!");
                }

                task.StartDate = model.StartDate;

                await _taskRepository.SaveChangeAsync();
                var authorization = Request.Headers[HeaderNames.Authorization];

                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var token = headerValue.Parameter;
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    var requestUserId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;
                    var assigneeUserId = task.UserId;

                    if (!string.IsNullOrEmpty(assigneeUserId) && !requestUserId.Equals(assigneeUserId))
                    {
                        var assigneeUserIds = new List<string>
                        {
                            assigneeUserId
                        };

                        var notificationTokens = (await _fbUserTokenRepository.GetTokensByUserIds(assigneeUserIds)).Select(n => n.Token).ToList();
                        var requestUser = await _userManager.FindByIdAsync(requestUserId);
                        var assigneeUser = await _userManager.FindByIdAsync(assigneeUserId);

                        if (requestUser != null && assigneeUser != null)
                        {
                            // Gửi notification đến user
                            var baseUrl = _configuration["ApplicationUrl"];

                            using (var httpClient = new HttpClient())
                            {
                                var notificationModel = new PushNotificationRequestModel
                                {
                                    Tokens = notificationTokens,
                                    Notification = new RequestModels.NotificationRequestModels.Notification
                                    {
                                        Title = "TMSO Software",
                                        Body = $"{requestUser.FullName} updated start date for task '{task.Name}'",
                                        Icon = ""
                                    }
                                };

                                var body = new StringContent(JsonConvert.SerializeObject(notificationModel), System.Text.Encoding.UTF8, "application/json");
                                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                                await httpClient.PostAsync(baseUrl + "/api/notification/push", body);
                            }

                            // Lưu notification history
                            var messageObj = new
                            {
                                Title = $"{requestUser.FullName} updated start date task '{task.Name}'",
                                Content = $"TASK-{task.TaskId} | {task.StartDate}"
                            };
                            var message = JsonConvert.SerializeObject(messageObj);

                            var notification = new Models.Notification
                            {
                                Message = message,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            var notificationHistory = new UserNotification
                            {
                                User = assigneeUser,
                                Notification = notification,
                                IsViewed = false,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            await _notificationRepository.AddAsync(notificationHistory);
                            await _notificationRepository.SaveChangeAsync();
                        }
                    }
                }

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Update successfully!", Data = task });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Update label for task
        /// </summary>
        [HttpPut("label")]
        [Authorize]
        public async Task<IActionResult> UpdateLabel([FromBody] UpdateTaskLabelRequestModel model)
        {
            try
            {
                //var taskLabel = new TaskLabel
                //{
                //    TaskId = model.TaskId,
                //    LabelId = model.LabelId,
                //    CreatedDate = DateTime.Now,
                //    IsDeleted = false
                //};

                //await _taskLabelRepository.AddAsync(taskLabel);
                //await _taskLabelRepository.SaveChangeAsync();

                var taskLabel = await _taskLabelRepository.GetTaskLabelByIdAsync(model.TaskId);

                if (taskLabel == null)
                {
                    var newtaskLabel = new TaskLabel
                    {
                        TaskId = model.TaskId,
                        LabelId = model.LabelId,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };

                    await _taskLabelRepository.AddAsync(newtaskLabel);
                    await _taskLabelRepository.SaveChangeAsync();
                    return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Update successfully!", Data = newtaskLabel });
                }
                var task = await _taskRepository.GetTaskByIdAsync(model.TaskId);

                if (task == null)
                {
                    return BadRequest("Can not find task information!");
                }
                taskLabel.TaskId = model.TaskId;
                taskLabel.LabelId = model.LabelId;
                await _taskLabelRepository.SaveChangeAsync();

                var authorization = Request.Headers[HeaderNames.Authorization];

                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var token = headerValue.Parameter;
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    var requestUserId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;
                    var assigneeUserId = task.UserId;

                    if (!string.IsNullOrEmpty(assigneeUserId) && !requestUserId.Equals(assigneeUserId))
                    {
                        var assigneeUserIds = new List<string>
                        {
                            assigneeUserId
                        };

                        var notificationTokens = (await _fbUserTokenRepository.GetTokensByUserIds(assigneeUserIds)).Select(n => n.Token).ToList();
                        var requestUser = await _userManager.FindByIdAsync(requestUserId);
                        var assigneeUser = await _userManager.FindByIdAsync(assigneeUserId);

                        if (requestUser != null && assigneeUser != null)
                        {
                            // Gửi notification đến user
                            var baseUrl = _configuration["ApplicationUrl"];

                            using (var httpClient = new HttpClient())
                            {
                                var notificationModel = new PushNotificationRequestModel
                                {
                                    Tokens = notificationTokens,
                                    Notification = new RequestModels.NotificationRequestModels.Notification
                                    {
                                        Title = "TMSO Software",
                                        Body = $"{requestUser.FullName} update label ",
                                        Icon = ""
                                    }
                                };

                                var body = new StringContent(JsonConvert.SerializeObject(notificationModel), System.Text.Encoding.UTF8, "application/json");
                                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                                await httpClient.PostAsync(baseUrl + "/api/notification/push", body);
                            }

                            // Lưu notification history
                            var messageObj = new
                            {
                                Title = $"{requestUser.FullName} update label ",
                                Content = $"TASK-{task.TaskId}"
                            };
                            var message = JsonConvert.SerializeObject(messageObj);

                            var notification = new Models.Notification
                            {
                                Message = message,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            var notificationHistory = new UserNotification
                            {
                                User = assigneeUser,
                                Notification = notification,
                                IsViewed = false,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            await _notificationRepository.AddAsync(notificationHistory);
                            await _notificationRepository.SaveChangeAsync();
                        }
                    }
                }
                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Update successfully!", Data = taskLabel });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }


        /// <summary>
        /// Get all user's tasks
        /// </summary>
        [HttpGet("my-tasks")]
        [Authorize]
        public async Task<IActionResult> GetTasksByUser([FromQuery] string userId)
        {
            try
            {
                if (String.IsNullOrEmpty(userId))
                {
                    return BadRequest("Parameter 'userId' can not be null or empty!");
                }

                var tasks = await _taskRepository.GetTasksByUser(userId);

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Request successfully!", Data = tasks });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Get search user's tasks
        /// </summary>
        [HttpGet("search-tasks")]
        [Authorize]
        public async Task<IActionResult> GetSearchTasksOfUser([FromQuery] string userId, string keyWord)
        {
            try
            {
                if (String.IsNullOrEmpty(userId) || String.IsNullOrEmpty(keyWord))
                {
                    return BadRequest("Parameter 'userId' or 'keyWord' can not be null or empty!");
                }

                //var tasks = await _taskRepository.GetSearchTasksOfUser(userId, keyWord.Trim());
                var tasks = await _taskRepository.GetListSearchTMyTask(userId, keyWord.Trim());
                var returnData = tasks.Select(t => new MyTasksViewModel
                {
                    TaskId = t.TaskId,
                    Name = t.Name,
                    DueDate = t.DueDate,
                    Assignee = t.Assignee,
                    TaskStatus = t.TaskStatus,
                    ProjectName = t.ProjectName
                });

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Request successfully!", Data = tasks });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }
    }
}