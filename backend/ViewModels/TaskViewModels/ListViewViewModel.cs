using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.ViewModels.TaskViewModels
{
    public class ListViewViewModel
    {
        public int TaskId { get; set; }
        public string Name { get; set; }
        public DateTime? DueDate { get; set; }
        public string Category { get; set; }
        public AssigneeViewModelTask Assignee { get; set; }
        public TaskStatusViewModel TaskStatus { get; set; }
        public SectionInListViewModel Section { get; set; }
        //public List<SectionInListViewModel> listSection { get; set; }
        public List<LabelViewModel> ListLabel { get; set; }
    }
    public class AssigneeViewModelTask
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
    }

    public class TaskStatusViewModel
    {
        public int TaskStatusId { get; set; }
        public string Name { get; set; }
    }
    public class SectionInListViewModel
    {
        public int ListId { get; set; }
        public string Name { get; set; }
    }

    public class MyTasksViewModel
    {
        public int TaskId { get; set; }
        public string Name { get; set; }
        public DateTime? DueDate { get; set; }
        public string Category { get; set; }
        public AssigneeViewModelTask Assignee { get; set; }
        public TaskStatusViewModel TaskStatus { get; set; }
        public string ProjectName { get; set; }
        public int ProjectId { get; set; }
    }
    public class LabelViewModelNew
    {
        public int LabelId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }
}
