using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using tms_api.Models;
using tms_api.Repositories.Interfaces;
using tms_api.RequestModels.UserRequestModel;
using tms_api.ViewModels.UserViewModel;

namespace tms_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;

        public UserController(ILogger<UserController> logger,
            UserManager<User> userManager,
            IUserRepository userRepository)
        {
            _logger = logger;
            _userManager = userManager;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Get list students
        /// </summary>
        [HttpGet("students")]
        [Authorize]
        public async Task<IActionResult> GetListStudent()
        {
            try
            {
                var students = await _userRepository.GetUserByRole("Student");
                var availableStudents = students.Select(s => new AvailableStudentDto
                {
                    Id = s.Id,
                    UserName = s.UserName,
                    FullName = s.FullName,
                    Email = s.Email,
                    AvatarUrl = s.AvatarUrl
                }).ToList();

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Request successfully!", Data = availableStudents });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Get list teachers
        /// </summary>
        [HttpGet("teachers")]
        [Authorize]
        public async Task<IActionResult> GetListTeacher()
        {
            try
            {
                var teachers = await _userRepository.GetUserByRole("Teacher");
                var availableTeachers = teachers.Select(s => new AvailableTeacherDto
                {
                    Id = s.Id,
                    UserName = s.UserName,
                    FullName = s.FullName,
                    Email = s.Email,
                    AvatarUrl = s.AvatarUrl
                }).ToList();

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Request successfully!", Data = availableTeachers });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Get all users in project
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserInProject([FromQuery] int projectId)
        {
            try
            {
                var users = await _userRepository.GetUserInProject(projectId);
                var returnData = users.Select(s => new AvailableStudentDto
                {
                    Id = s.Id,
                    UserName = s.UserName,
                    FullName = s.FullName,
                    Email = s.Email,
                    AvatarUrl = s.AvatarUrl
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
        /// Get user info
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("info")]
        [Authorize]
        public async Task<IActionResult> GetUserInfo([FromQuery] string userId)
        {
            try
            {
                var user = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();

                if (user == null)
                {
                    return BadRequest("Can not find user information!");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var mainRole = roles.Where(r => r == "Teacher" || r == "Student" || r == "Admin").FirstOrDefault();

                var returnData = new
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = mainRole,
                    Description = user.Description,
                    AvatarUrl = user.AvatarUrl,
                    UserName=user.UserName,
                    RollNumber = user.RollNumber
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
        /// Update information of user
        /// </summary>
        [HttpPut("user-information")]
        [Authorize]
        public async Task<IActionResult> UpdateInformationDependency([FromBody] UserRequestModel model)
        {
            try
            {
                var user = await _userManager.Users.Where(u => u.Id == model.Id).FirstOrDefaultAsync();

                if (user == null)
                {
                    return BadRequest("Can not find user information!");
                }

                if (model.IsDeleted)
                {
                    user.IsDeleted = model.IsDeleted;
                }

                if (!string.IsNullOrEmpty(model.FullName))
                {
                    user.FullName = model.FullName;
                }

                if (!string.IsNullOrEmpty(model.AvatarUrl))
                {
                    user.AvatarUrl = model.AvatarUrl;
                }

                if (!string.IsNullOrEmpty(model.Description))
                {
                    user.Description = model.Description;
                }

                if (!string.IsNullOrEmpty(model.RollNumber))
                {
                    user.RollNumber = model.RollNumber;
                }

                await _userRepository.SaveChangeAsync();
                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Update successfully!", Data = user });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

    }
}