using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tms_api.Services.SendEmail;

namespace tms_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IEmailSender _emailSender;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public async Task<IEnumerable<object>> Get()
        {
            var rng = new Random();

            // Test send email
            //await _emailSender.SendEmailAsync("cong.lee2025@gmail.com", "test send email xem co bi spam ko", "No content No contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo content" +
            //    "No contentNo contentNo contentNo contentNo contentNo contentNo contentNo content" +
            //    "No contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo content" +
            //    "No contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo content" +
            //    "No contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo contentNo content");

            return Enumerable.Range(1, 5).Select(index => new
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
