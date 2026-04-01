using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using tms_api.Models;
using tms_api.Repositories.Interfaces;
using tms_api.RequestModels.NotificationRequestModels;
using tms_api.ViewModels.NotificationViewModel;

namespace tms_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly IFbUserTokenRepository _fbUserTokenRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IConfiguration _configuration;

        public NotificationController(ILogger<NotificationController> logger,
            IFbUserTokenRepository fbUserTokenRepository,
            INotificationRepository notificationRepository,
            IConfiguration configuration)
        {
            _logger = logger;
            _fbUserTokenRepository = fbUserTokenRepository;
            _notificationRepository = notificationRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// Register token
        /// </summary>
        [HttpPost("register-token")]
        [Authorize]
        public async Task<IActionResult> RegisterToken([FromBody] RegisterTokenRequestModel model)
        {
            try
            {
                var isTokenExisted = await _fbUserTokenRepository.CheckTokenExisted(model.Token);
                
                if (!isTokenExisted)
                {
                    var userToken = new FbUserToken
                    {
                        Token = model.Token,
                        UserId = model.UserId,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };

                    await _fbUserTokenRepository.AddAsync(userToken);
                    await _fbUserTokenRepository.SaveChangeAsync();

                    return Ok(new { StatusCode = HttpStatusCode.Created, Message = "Register token successfully!", Data = model.Token });
                }

                return Ok($"'{model.Token}' is already existed in database.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Get all notification by userId
        /// </summary>
        [HttpGet("{userId}")]
        [Authorize]
        public async Task<IActionResult> GetNotifications(string userId)
        {
            try
            {
                var userNotifications = await _notificationRepository.GetNotificationByUserId(userId);

                var returnData = userNotifications.Select(u => new NotificationViewModel
                {
                    NotificationId = u.UserNotificationId,
                    Content = u.Notification.Message,
                    IsViewed = u.IsViewed,
                    CreatedDate = u.CreatedDate
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
        /// Read all notification by userId
        /// </summary>
        [HttpPost("read/{userId}")]
        [Authorize]
        public async Task<IActionResult> ReadNotification(string userId)
        {
            try
            {
                var unReadNotification = await _notificationRepository.GetUnReadNotificationByUserId(userId);

                foreach (var item in unReadNotification)
                {
                    item.IsViewed = true;
                }

                await _notificationRepository.SaveChangeAsync();

                return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Request successfully!", Data = new { } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        /// <summary>
        /// Read all notification by userId
        /// </summary>
        [HttpPost("push")]
        [Authorize]
        public async Task<IActionResult> PushNotification([FromBody] PushNotificationRequestModel model)
        {
            try
            {
                var serverApiKey = _configuration["FirebaseConfig:ServerApiKey"];
                var senderId = _configuration["FirebaseConfig:SenderId"];
                var firebasePushNotificationUrl = _configuration["FirebaseConfig:FireBasePushNotificationsURL"];

                WebRequest tRequest;
                tRequest = WebRequest.Create(firebasePushNotificationUrl);
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add(string.Format("Authorization: key={0}", serverApiKey));

                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));

                var data = new
                {
                    registration_ids = model.Tokens,
                    notification = new
                    {
                        body = model.Notification.Body,
                        title = model.Notification.Title,
                        icon = model.Notification.Icon
                    }
                };

                var json = JsonConvert.SerializeObject(data);

                Byte[] byteArray = Encoding.UTF8.GetBytes(json);

                tRequest.ContentLength = byteArray.Length;

                Stream dataStream = tRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse tResponse = tRequest.GetResponse();

                dataStream = tResponse.GetResponseStream();

                StreamReader tReader = new StreamReader(dataStream);

                String sResponseFromServer = tReader.ReadToEnd();


                tReader.Close();
                dataStream.Close();
                tResponse.Close();
                return Ok(sResponseFromServer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }
    }
}