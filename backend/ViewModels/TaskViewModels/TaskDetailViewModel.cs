using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.ViewModels.TaskViewModels
{
    public class TaskDetailViewModel
    {
        public int TaskId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public bool? IsCompleted { get; set; }
        public bool IsActive { get; set; }
        public int? ParentId { get; set; }
        public PriorityViewModel Priority { get; set; }
        public StatusViewModel Status { get; set; }
        public SectionViewModel Section { get; set; }
        public AssigneeViewModel Assignee { get; set; }
        public List<SubtaskViewModel> Subtasks { get; set; }
        public List<CommentViewModel> Comments { get; set; } 
        public List<ChecklistViewModel> Checklists { get; set; } 
        public List<DependenciesViewModel> Dependencies { get; set; } 
        public List<LabelViewModel> Labels { get; set; } 
        public List<FileAttachmentViewModel> FileAttachments { get; set; } 
    }

    public class CommentViewModel
    {
        public int CommentId { get; set; }
        public UserViewModel Author { get; set; }
        public string Content { get; set; }
        public string AttachFile { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class ChecklistViewModel
    {
        public int ChecklistId { get; set; }
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class StatusViewModel
    {
        public int StatusId { get; set; }
        public string Name { get; set; }
    }

    public class SectionViewModel
    {
        public int SectionId { get; set; }
        public string Name { get; set; }
    }

    public class PriorityViewModel
    {
        public int PriorityId { get; set; }
        public string Name { get; set; }
    }

    public class DependenciesViewModel
    {
        public int DependencyId { get; set; }
        public int TaskDenpendencyId { get; set; }
        public string TaskName { get; set; }
        public string DependencyName { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class AssigneeViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
    }

    public class LabelViewModel
    {
        public int LabelId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }

    public class FileAttachmentViewModel
    {
        public int FileAttachmentId { get; set; }
        public string Name { get; set; }
        public string MediaLink { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class UserViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
    }

    public class SubtaskViewModel
    {
        public int TaskId { get; set; }
        public string Name { get; set; }
        public string StatusName { get; set; }
    }
}
