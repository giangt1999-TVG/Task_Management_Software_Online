using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using tms_api.Models;
using tms_api.Repositories.Interfaces;
using tms_api.RequestModels.LabelRequestModel;
using tms_api.RequestModels.NotificationRequestModels;
using tms_api.RequestModels.ProjectRequestModels;
using tms_api.ViewModels.ProjectViewModel;

namespace tms_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFbUserTokenRepository _fbUserTokenRepository;
        private readonly IConfiguration _configuration;
        private readonly INotificationRepository _notificationRepository;
        private readonly ILabelRepository _labelRepository;

        public ProjectController(ILogger<ProjectController> logger,
            UserManager<User> userManager,
            IProjectRepository projectRepository,
            IUserRepository userRepository,
            RoleManager<Role> roleManager,
            IFbUserTokenRepository fbUserTokenRepository,
            IConfiguration configuration,
            INotificationRepository notificationRepository,
            ILabelRepository labelRepository)
        {
            _logger = logger;
            _userManager = userManager;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _roleManager = roleManager;
            _fbUserTokenRepository = fbUserTokenRepository;
            _configuration = configuration;
            _notificationRepository = notificationRepository;
            _labelRepository = labelRepository;
        }

        /// <summary>
        /// Create a new project
        /// </summary>
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateNewProject([FromBody] NewProjectRequestModel model)
        {
            try
            {
                if (model.MemberIds.Contains(model.TeamleadId))
                {
                    return BadRequest("Teamlead can not be assigned as member!");
                }

                var isExistedProject = await _projectRepository.CheckExistProject(model.ProjectCode);

                if (isExistedProject)
                {
                    return BadRequest("Project code is existed. Please choose another one!");
                }

                var teacher = await _userManager.FindByIdAsync(model.TeacherId);
                var teamlead = await _userManager.FindByIdAsync(model.TeamleadId);

                // TODO: Check tai sao su dung check role bi conflict khi add user role moi
                if (teamlead == null || teacher == null)
                {
                    return BadRequest("An error occurred during create project!");
                }

                var project = new Project
                {
                    Name = model.ProjectName,
                    ProjectCode = model.ProjectCode,
                    Description = model.Description,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                };

                // Thêm member vào project
                var listMemberProject = new List<ProjectMember>
                {
                    new ProjectMember
                    {
                        Project = project,
                        User = teacher,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    },
                    new ProjectMember
                    {
                        Project = project,
                        User = teamlead,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    }
                };

                // Lấy tất cả team member với list userID
                var teamMembers = await _userManager.Users.Where(u => model.MemberIds.Contains(u.Id) && !u.IsDeleted).ToListAsync();

                foreach (var teamMember in teamMembers)
                {
                    var projectMemeber = new ProjectMember
                    {
                        Project = project,
                        User = teamMember,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };

                    listMemberProject.Add(projectMemeber);
                }

                project.ProjectMembers = listMemberProject;

                // Tạo list mặc định cho project
                var lists = new List<Lists>
                {
                    new Lists
                    {
                        Name = "TODO",
                        Project = project,
                        Index = 1,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    },
                    new Lists
                    {
                        Name = "IN PROGRESS",
                        Project = project,
                        Index = 2,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    },
                    new Lists
                    {
                        Name = "DONE",
                        Project = project,
                        Index = 3,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    }
                };

                project.List = lists;

                await _projectRepository.AddAsync(project);
                await _projectRepository.SaveChangeAsync();

                // Add user's role with project
                var teamleadRole = await _roleManager.Roles.Where(r => r.Name == "Teamlead").FirstOrDefaultAsync();
                var teacherRole = await _roleManager.Roles.Where(r => r.Name == "Teacher").FirstOrDefaultAsync();
                var teamMemberRole = await _roleManager.Roles.Where(r => r.Name == "Member").FirstOrDefaultAsync();

                await _userRepository.AddUserRoleWithProject(teacher.Id, teacherRole.Id, project.ProjectId);
                await _userRepository.AddUserRoleWithProject(teamlead.Id, teamleadRole.Id, project.ProjectId);

                foreach (var memberId in model.MemberIds)
                {
                    await _userRepository.AddUserRoleWithProject(memberId, teamMemberRole.Id, project.ProjectId);
                }

                //await _userRepository.SaveChangeAsync();

                // Thông báo cho user đã được thêm vào dự án
                var notificationUserIds = model.MemberIds;
                notificationUserIds.Add(model.TeacherId);
                notificationUserIds.Add(model.TeamleadId);

                var notificationTokens = (await _fbUserTokenRepository.GetTokensByUserIds(notificationUserIds)).Select(n => n.Token).ToList();

                // Gửi notification đến user
                var baseUrl = _configuration["ApplicationUrl"];
                var accessToken = Request.Headers["Authorization"].ToString();

                using (var httpClient = new HttpClient())
                {
                    var notificationModel = new PushNotificationRequestModel
                    {
                        Tokens = notificationTokens,
                        Notification = new RequestModels.NotificationRequestModels.Notification
                        {
                            Title = "TMSO Software",
                            Body = $"You are invited to '{project.Name}' project!",
                            Icon = ""
                        }
                    };

                    var body = new StringContent(JsonConvert.SerializeObject(notificationModel), System.Text.Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Add("Authorization", accessToken);
                    await httpClient.PostAsync(baseUrl + "/api/notification/push", body);
                }

                // Lưu notification history
                var notificationHistories = new List<UserNotification>();
                var notificationUsers = teamMembers;
                notificationUsers.Add(teacher);
                notificationUsers.Add(teamlead);

                var messageObj = new
                {
                    Title = $"You are invited to '{project.Name}' project!",
                    Content = ""
                };
                var message = JsonConvert.SerializeObject(messageObj);

                var notification = new Models.Notification
                {
                    Message = message,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                };

                foreach (var user in notificationUsers)
                {
                    var notificationHistory = new UserNotification
                    {
                        User = user,
                        Notification = notification,
                        IsViewed = false,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };

                    notificationHistories.Add(notificationHistory);
                }

                await _notificationRepository.AddRangeAsync(notificationHistories);
                await _notificationRepository.SaveChangeAsync();

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Create new project successfully!", Data = new { Id = project.ProjectId, Name = project.Name, ProjectCode = project.ProjectCode, Description = project.Description } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Get all user's projects
        /// </summary>
        [HttpGet("my-projects")]
        [Authorize]
        public async Task<IActionResult> GetProjectsByUser([FromQuery] string userId)
        {
            try
            {
                if (String.IsNullOrEmpty(userId))
                {
                    return BadRequest("Parameter 'userId' can not be null or empty!");
                }

                var projects = await _projectRepository.GetProjectByUser(userId);
                var returnData = projects.Select(p => new MyProjectDto
                {
                    Id = p.ProjectId,
                    Code = p.ProjectCode,
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Members = (_userRepository.GetUserInfoInProject(p.ProjectId).Result).Where(m => m.Role == "Teacher" || m.Role == "Teamlead").ToList()
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
        /// Get project information
        /// </summary>
        [HttpGet("information/{id}")]
        [Authorize]
        public async Task<IActionResult> GetProjectInformation(int id)
        {
            try
            {
                var project = await _projectRepository.GetProjectInformation(id);
                var returnData = new ProjectInformationViewModel
                {
                    Id = project.ProjectId,
                    Code = project.ProjectCode,
                    Name = project.Name,
                    Description = project.Description,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    // TODO: Check truong hop user bi xoa
                    listUser = project.ProjectMembers.Select(pm => new UserInProjectViewModel
                    {
                        UserId = pm.User.Id,
                        FullName = pm.User.FullName,
                        AvatarUrl = pm.User.AvatarUrl,
                        Role = (_userRepository.GetRoleByUserId(pm.User.Id, id).Result).Name
                    }).ToList(),
                    ListLabel = project.Label.Select(l => new LabelViewModel { 
                        LabelId = l.LabelId,
                        Name = l.Name,
                        Color = l.Color
                    }).ToList()
                };

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Request successfully!", Data = returnData });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Get user's projects by search keyWord
        /// </summary>
        [HttpGet("search-projects")]
        [Authorize]
        public async Task<IActionResult> GetSearchProjectsByUser([FromQuery] string userId, string keyWord)
        {
            try
            {
                if (String.IsNullOrEmpty(userId) || String.IsNullOrEmpty(keyWord))
                {
                    return BadRequest("Parameter 'userId' or 'keyWord' can not be null or empty!");
                }
                var projects = await _projectRepository.GetSearchProjectByUser(userId, keyWord.Trim());
                var returnData = projects.Select(p => new MyProjectDto
                {
                    Id = p.ProjectId,
                    Code = p.ProjectCode,
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Members = (_userRepository.GetUserInfoInProject(p.ProjectId).Result).Where(m => m.Role == "Teacher" || m.Role == "Teamlead").ToList()
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
        /// Add project member
        /// </summary>
        [HttpPost("add-member")]
        [Authorize]
        public async Task<IActionResult> AddMember([FromBody] AddMemberRequestModel model)
        {
            try
            {
                var project = await _projectRepository.GetProjectInformation(model.ProjectId);

                var projectMember = new ProjectMember
                {
                    ProjectId = model.ProjectId,
                    UserId = model.UserId,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                };

                project.ProjectMembers.Add(projectMember);

                var teamMemberRole = await _roleManager.Roles.Where(r => r.Name == "Member").FirstOrDefaultAsync();
                await _userRepository.AddUserRoleWithProject(model.UserId, teamMemberRole.Id, model.ProjectId);

                await _projectRepository.SaveChangeAsync();

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Add member successfully!", Data = new { } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Update teacher and teamlead in project
        /// </summary>
        [HttpPut("update-member")]
        [Authorize]
        public async Task<IActionResult> UpdateTeacherTeamlead([FromBody] UpdateTeacherTeamleadRequestModel model)
        {
            try
            {
                var project = await _projectRepository.GetProjectInformation(model.ProjectId);

                if (project == null)
                {
                    return BadRequest("Can not find project information!");
                }

                var projectMember = project.ProjectMembers.Where(pm => pm.UserId == model.OldUserId && pm.ProjectId == model.ProjectId).FirstOrDefault();

                if (projectMember == null)
                {
                    return BadRequest("Can not find project member information!");
                }

                projectMember.UserId = model.NewUserId;

                await _userRepository.RemoveProjectMember(model.OldUserId, model.ProjectId);

                var role = await _roleManager.Roles.Where(r => r.Name.ToLower().Equals(model.Role.ToLower())).FirstOrDefaultAsync();
                await _userRepository.AddUserRoleWithProject(model.NewUserId, role.Id, model.ProjectId);

                await _projectRepository.SaveChangeAsync();

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Update member successfully!", Data = new { } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Delete project member
        /// </summary>
        [HttpDelete("delete-member")]
        [Authorize]
        public async Task<IActionResult> DeleteMember([FromBody] RemoveMemberRequestModel model)
        {
            try
            {
                var project = await _projectRepository.GetProjectInformation(model.ProjectId);

                if (project == null)
                {
                    return BadRequest("Can not find project information!");
                }

                var projectMember = project.ProjectMembers.Where(pm => pm.UserId == model.UserId && pm.ProjectId == model.ProjectId).FirstOrDefault();

                if (projectMember == null)
                {
                    return BadRequest("Can not find project member information!");
                }

                projectMember.IsDeleted = true;
                await _projectRepository.SaveChangeAsync();

                await _userRepository.RemoveProjectMember(model.UserId, model.ProjectId);

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Delete member successfully!", Data = new { } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Delete project label
        /// </summary>
        [HttpDelete("delete-label")]
        [Authorize]
        public async Task<IActionResult> DeleteLabel([FromBody] DeleteLabelRequestModel model)
        {
            try
            {
                var projectLabel = await _labelRepository.GetLabel(model.LabelId, model.ProjectId);

                if (projectLabel == null)
                {
                    return BadRequest("Can not find label in project!");
                }

                projectLabel.IsDeleted = true;
                await _labelRepository.SaveChangeAsync();

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Delete label successfully!", Data = new { } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Update project information
        /// </summary>
        [HttpPut("update-info")]
        [Authorize]
        public async Task<IActionResult> UpdateProjectInformation([FromBody] UpdateProjectInformationRequestModel model)
        {
            try
            {
                var project = await _projectRepository.GetProjectById(model.ProjectId);

                if (project == null)
                {
                    return BadRequest("Can not find project information!");
                }

                if (!string.IsNullOrEmpty(model.ProjectCode))
                {
                    project.ProjectCode = model.ProjectCode;
                }

                if (!string.IsNullOrEmpty(model.ProjectName))
                {
                    project.Name = model.ProjectName;
                }

                if (!string.IsNullOrEmpty(model.Description))
                {
                    project.Description = model.Description;
                }

                if (model.StartDate.HasValue)
                {
                    project.StartDate = model.StartDate.Value;
                }

                if (model.EndDate.HasValue)
                {
                    project.EndDate = model.EndDate.Value;
                }

                await _projectRepository.SaveChangeAsync();
                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Update project successfully!", Data = project });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }
    }
}