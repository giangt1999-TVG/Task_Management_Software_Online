using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using tms_api.Extensions;
using tms_api.Models;
using tms_api.Repositories.Interfaces;
using tms_api.RequestModels;
using tms_api.RequestModels.IdentityRequestModels;
using tms_api.RequestModels.UserRequestModel;
using tms_api.Services.SendEmail;
using tms_api.Services.SendEmail.TemplateData;
using tms_api.Services.UploadFile;
using tms_api.ViewModels.IdentityViewModels;
using tms_api.ViewModels.UserViewModel;

namespace tms_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly ICloudStorage _cloudStorage;

        public AccountController(ILogger<AccountController> logger,
            IEmailSender emailSender,
            UserManager<User> userManager,
            IConfiguration configuration,
            IUserRepository userRepository,
            RoleManager<Role> roleManager,
            ICloudStorage cloudStorage,
            SignInManager<User> signInManager)
        {
            _logger = logger;
            _emailSender = emailSender;
            _userManager = userManager;
            _configuration = configuration;
            _userRepository = userRepository;
            _roleManager = roleManager;
            _cloudStorage = cloudStorage;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Import users
        /// </summary>
        [HttpPost("import")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ImportUsers([FromBody] List<ImportUserRequestModel> listUserModel)
        {
            try
            {
                var successes = new List<ImportUserRequestModel>();
                var failures = new List<ImportUserRequestModel>();

                foreach (var userModel in listUserModel)
                {
                    var randomPassword = UserManager.GenerateRandomPassword();
                    var user = new User
                    {
                        Email = userModel.Email,
                        UserName = userModel.Email.Split('@')[0],
                        FullName = userModel.FullName,
                        RollNumber = userModel.RollNumber,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };

                    var result = await _userManager.CreateAsync(user, randomPassword);

                    if (!result.Succeeded)
                    {
                        _logger.LogInformation("[Error] Fail to create account with email '" + userModel.Email + "'!");
                        failures.Add(userModel);
                    }
                    else
                    {
                        _logger.LogInformation("Create account successfully with email '" + userModel.Email + "'!");

                        await _userManager.AddToRoleAsync(user, userModel.Role);

                        var templateId = _configuration["ExternalProviders:SendGrid:WelcomeEmailTemplateId"];
                        var templateData = new WelcomeEmailTemplateData
                        {
                            Name = user.FullName.Split(' ').Last(),
                            EmailAddress = user.Email,
                            Password = randomPassword
                        };

                        await _emailSender.SendEmailAsync(user.Email, templateId, templateData);
                        successes.Add(userModel);
                    }
                }

                return Ok(new { StatusCode = HttpStatusCode.Created, Message = "Import users successfully!", Data = new { SuccessRecords = successes, FailuresRecords = failures } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Change password
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.UserId);

                if (user == null) 
                {
                    return BadRequest("Can not find user information!");
                }

                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                else
                {
                    return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Change password successfully!", Data = new { } });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Create a new account
        /// </summary>
        [HttpPost("new")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateNewAccount([FromBody] NewAccountRequestModel model)
        {

            try
            {
                var randomPassword = UserManager.GenerateRandomPassword();
                var user = new User
                {
                    Email = model.Email,
                    UserName = model.Email.Split('@')[0],
                    FullName = model.FullName,
                    RollNumber=model.RollNumber,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                };

                var result = await _userManager.CreateAsync(user, randomPassword);

                if (!result.Succeeded)
                {
                    _logger.LogInformation("[Error] Fail to create account with email '" + model.Email + "'!");
                    return BadRequest("Email existed in system!");
                }
                else
                {
                    _logger.LogInformation("Create account successfully with email '" + model.Email + "'!");

                    await _userManager.AddToRoleAsync(user, model.Role);

                    var templateId = _configuration["ExternalProviders:SendGrid:WelcomeEmailTemplateId"];
                    var templateData = new WelcomeEmailTemplateData
                    {
                        Name = user.FullName.Split(' ').Last(),
                        EmailAddress = user.Email,
                        Password = randomPassword
                    };

                    await _emailSender.SendEmailAsync(user.Email, templateId, templateData);
                }

                return Ok(new { StatusCode = HttpStatusCode.Created, Message = "Create new account successfully!", Data = user });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Authenticate user
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
            try
            {
                var userName = model.Email.Split('@')[0];
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    return Ok(new UserManagerViewModel(HttpStatusCode.BadRequest, "Email address or password is incorrect!", null));
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, model.PassWord, false);

                if (!result.Succeeded)
                {
                    return Ok(new UserManagerViewModel(HttpStatusCode.BadRequest, "Email address or password is incorrect!", null));
                }


                var roles = await _userManager.GetRolesAsync(user);
                var mainRole = roles.Where(r => r == "Teacher" || r == "Student" || r == "Admin").FirstOrDefault();

                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim("Id", user.Id),
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim(ClaimTypes.Role, mainRole)
                    }),
                    Expires = DateTime.Now.AddHours(3),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = jwtTokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = jwtTokenHandler.WriteToken(token);

                var userInfo = new
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    MainRole = mainRole,
                    AvatarUrl = user.AvatarUrl,
                    Token = jwtToken
                };

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Login successfully!", Data = new { UserInfo = userInfo } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Set up account for first login
        /// </summary>
        [HttpPost("setup")]
        public async Task<IActionResult> SetUp([FromBody] SetupAccountRequestModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.UserId);

                if (user == null)
                {
                    return BadRequest("Can not find user with ID '" + model.UserId + "'!");
                }

                user.FullName = model.FullName;
                user.ModifiedDate = DateTime.Now;
                await _userManager.AddToRoleAsync(user, model.Role);

                if (model.AvatarUrl != null)
                {
                    user.AvatarUrl = model.AvatarUrl;
                }

                if (model.Description != null)
                {
                    user.Description = model.Description;
                }

                await _userManager.UpdateAsync(user);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Get all roles in system
        /// </summary>
        [HttpGet("role")]
        [Authorize]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _roleManager.Roles
                                .Where(r => r.ParentId == null)
                                .Select(r => new { r.Id, r.Name })
                                .ToListAsync();

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Request successfully!", Data = roles });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Upload avatar image
        /// </summary>
        [HttpPost("avatar")]
        [Authorize]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            try
            {
                // TODO: Validate file upload
                var bucketName = "avatar-image-bucket";
                var avatarUrl = await _cloudStorage.UploadFileAsync(bucketName, file, file.FileName);
                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Request successfully!", Data = avatarUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Get account information
        /// </summary>
        [HttpGet("account-information")]
        [Authorize]
        public async Task<IActionResult> GetListAccountInformation()
        {
            try
            {
                var accounts = await _userRepository.GetListUsersInformation();
                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Request successfully!", Data = accounts });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Get search account information
        /// </summary>
        [HttpGet("search-account")]
        [Authorize]
        public async Task<IActionResult> GetSearchAccount([FromQuery] string keyWord)
        {
            try
            {
                if (String.IsNullOrEmpty(keyWord))
                {
                    return BadRequest("Parameter can not be null or empty!");
                }
                var users = await _userRepository.GetSearchAccount(keyWord.Trim());
                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Request successfully!", Data = users });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Update information of account
        /// </summary>
        [HttpPut("account-information")]
        [Authorize()]
        public async Task<IActionResult> UpdateInformationAccount([FromBody] UpdateAccountRequestModel model)
        {
            try
            {
                var user = await _userManager.Users.Where(u => u.Id == model.Id).FirstOrDefaultAsync();
                var roles = await _userManager.GetRolesAsync(user);
                var mainRoles = roles.Where(r => r == "Teacher" || r == "Student" || r == "Admin").ToList();
                
                if (user == null)
                {
                    return BadRequest("Can not find user information!");
                }

               
                if (!string.IsNullOrEmpty(model.FullName))
                {
                    user.FullName = model.FullName;
                }
                if (!string.IsNullOrEmpty(model.Usename))
                {
                    user.UserName = model.Usename;
                }
                if (!string.IsNullOrEmpty(model.Email))
                {
                    user.Email = model.Email;
                }
                if (!string.IsNullOrEmpty(model.Role))
                {
                    await _userManager.RemoveFromRolesAsync(user, mainRoles);
                    await _userManager.AddToRoleAsync(user, model.Role);
                }

                user.RollNumber = model.RollNumber;

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