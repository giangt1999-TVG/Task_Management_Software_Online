using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.RequestModels.NotificationRequestModels
{
    public class PushNotificationRequestModel
    {
        [Required]
        public List<string> Tokens { get; set; }

        [Required]
        public Notification Notification { get; set; }
    }

    public class Notification
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Icon { get; set; }
    }
}
