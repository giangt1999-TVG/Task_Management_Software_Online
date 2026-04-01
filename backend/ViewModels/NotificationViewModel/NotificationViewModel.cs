using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.ViewModels.NotificationViewModel
{
    public class NotificationViewModel
    {
        public int NotificationId { get; set; }
        public string Content { get; set; }
        public bool IsViewed { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
