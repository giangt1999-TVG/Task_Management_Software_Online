using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace tms_api.Models
{
    public partial class ActivityDetail
    {
        public ActivityDetail()
        {
            Activity = new HashSet<Activity>();
        }

        public int ActivityDetailId { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Activity> Activity { get; set; }
    }
}
