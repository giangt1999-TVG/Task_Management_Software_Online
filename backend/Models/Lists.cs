using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace tms_api.Models
{
    public partial class Lists
    {
        public Lists()
        {
            Tasks = new HashSet<Tasks>();
        }

        public int ListId { get; set; }
        public string Name { get; set; }
        public int ProjectId { get; set; }
        public int Index { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Project Project { get; set; }
        public virtual ICollection<Tasks> Tasks { get; set; }
    }
}
