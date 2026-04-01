using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace tms_api.Models
{
    public partial class Label
    {
        public Label()
        {
            TaskLabel = new HashSet<TaskLabel>();
        }

        public int LabelId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int ProjectId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Project Project { get; set; }
        public virtual ICollection<TaskLabel> TaskLabel { get; set; }
    }
}
