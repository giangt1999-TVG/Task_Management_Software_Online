using Newtonsoft.Json;

namespace tms_api.Services.SendEmail.TemplateData
{
    public class SignUpEmailTemplateData
    {
        [JsonProperty("url_homepage")]
        public string HomepageUrl { get; set; }

        [JsonProperty("confirm_link")]
        public string ConfirmLink { get; set; }
    }
}
