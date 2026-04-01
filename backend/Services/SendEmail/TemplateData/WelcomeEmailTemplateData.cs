using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.Services.SendEmail.TemplateData
{
    public class WelcomeEmailTemplateData
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email_address")]
        public string EmailAddress { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
