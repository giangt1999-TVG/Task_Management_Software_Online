using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace tms_api.Models
{
    public partial class Project
    {
        public Project()
        {
            Label = new HashSet<Label>();
            List = new HashSet<Lists>();
            ProjectMembers = new HashSet<ProjectMember>();
        }

        public int ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Label> Label { get; set; }
        public virtual ICollection<Lists> List { get; set; }
        public virtual ICollection<ProjectMember> ProjectMembers { get; set; }
    }
}
