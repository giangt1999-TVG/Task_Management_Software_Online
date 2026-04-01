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
using tms_api.Models;
using tms_api.Repositories.Interfaces;
using tms_api.RequestModels.DependencyRequestModel;
using tms_api.RequestModels.NotificationRequestModels;

namespace tms_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DependencyController : ControllerBase
    {
        private readonly ILogger<DependencyController> _logger;
        private readonly IDependencyRepository _dependencyRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly ITaskDependencyRepository _taskDependencyRepository;
        private readonly IFbUserTokenRepository _fbUserTokenRepository;
        private readonly UserManager<User> _userManager;
        private readonly INotificationRepository _notificationRepository;
        private readonly IConfiguration _configuration;
        public DependencyController(ILogger<DependencyController> logger,
            IDependencyRepository dependencyRepository, 
            ITaskRepository taskRepository,
            ITaskDependencyRepository taskDependencyRepository,
            IFbUserTokenRepository fbUserTokenRepository,
            UserManager<User> userManager,
            INotificationRepository notificationRepository,
            IConfiguration configuration)
        {
            _logger = logger;
            _dependencyRepository = dependencyRepository;
            _taskRepository = taskRepository;
            _taskDependencyRepository = taskDependencyRepository;
            _fbUserTokenRepository = fbUserTokenRepository;
            _userManager = userManager;
            _notificationRepository = notificationRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// add dependency in task
        /// </summary>
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateDependency([FromBody] NewDependencyRequestModel model)
        {
            try
            {
                var task = await _taskRepository.GetTaskByIdAsync(model.TaskId);

                if (task == null)
                {
                    return BadRequest("Can not find task information!");
                }

                var taskDependency = new TaskDependency
                {
                    TaskId = model.TaskId,
                    TaskDependId = model.TaskDependId,
                    DependencyId= model.DependencyId,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                };
               
                await _taskDependencyRepository.AddAsync(taskDependency);
                await _taskDependencyRepository.SaveChangeAsync();

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
                                        Body = $"{requestUser.FullName} created a new dependency in '{task.Name}'",
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
                                Title = $"{requestUser.FullName} created a new dependency in '{task.Name}'",
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

                return Ok(new { StatusCode = HttpStatusCode.Created, Message = "Create new dependency successfully!", Data = new { taskDependency.TaskDependencyId, taskDependency.TaskDependId, taskDependency.DependencyId} });
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
        [HttpPut("informationn-dependency-task")]
        [Authorize()]
        public async Task<IActionResult> UpdateInformationDependency([FromBody] UpdateDependencyInTaskRequestModel model)
        {
            try
            {
                var dependency = await _taskDependencyRepository.GetTaskDependencyByIdAsync(model.TaskDependencyId);
                
                if (dependency == null)
                {
                    return BadRequest("An error occurred during updating task!");
                }

                var task = await _taskRepository.GetTaskByIdAsync(dependency.TaskId);

                if (task == null)
                {
                    return BadRequest("Can not find task information!");
                }
               
                if (model.IsDeleted)
                {
                    dependency.IsDeleted = model.IsDeleted;
                }

                if (model.TaskDependId.HasValue) {
                    dependency.TaskDependId = model.TaskDependId.Value;
                }

                if (model.DependencyId.HasValue)
                {
                    dependency.DependencyId = model.DependencyId.Value;
                }

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
                            var bodyNotification = "";
                            if (!model.IsDeleted)
                            {
                                bodyNotification = $"{requestUser.FullName} updated a dependency in '{task.Name}'";
                            }
                            else
                            {
                                bodyNotification = $"{requestUser.FullName} deleted a dependency in '{task.Name}'";
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

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Update successfully!", Data = dependency });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }
    }
}
