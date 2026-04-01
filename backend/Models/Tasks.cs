using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace tms_api.Models
{
    public partial class Tasks
    {
        public Tasks()
        {
            TaskLabel = new HashSet<TaskLabel>();
            Comment = new HashSet<Comment>();
            Checklist = new HashSet<Checklist>();
            Activities = new HashSet<Activity>();
            TaskDependencies = new HashSet<TaskDependency>();
            FileAttachments = new HashSet<FileAttachment>();
        }

        public int TaskId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public bool? IsCompleted { get; set; }
        public bool IsActive { get; set; }
        public int ListId { get; set; }
        public int TaskPriorityId { get; set; }
        public int TaskStatusId { get; set; }
        public string UserId { get; set; }
        public int? ParentId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Lists List { get; set; }
        public virtual User User { get; set; }
        public virtual TaskPriority TaskPriority { get; set; }
        public virtual TaskStatuses TaskStatus { get; set; }
        public virtual ICollection<TaskLabel> TaskLabel { get; set; }
        public virtual ICollection<Comment> Comment { get; set; }
        public virtual ICollection<Checklist> Checklist { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
        public virtual ICollection<TaskDependency> TaskDependencies { get; set; }
        public virtual ICollection<FileAttachment> FileAttachments { get; set; }
    }
}
