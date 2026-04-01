using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace tms_api.Models
{
    public partial class Checklist
    {
        public int ChecklistId { get; set; }
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
        public int TaskId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Tasks Task { get; set; }
    }
}
