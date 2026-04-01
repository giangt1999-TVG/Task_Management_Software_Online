using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.ViewModels.SectionViewModel
{
    public class SectionListTaskViewModel
    {
        public int ListId { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }
        public List<TaskViewModel> listTasks { get; set; }
       
    }
    public class TaskViewModel
    {
        public int TaskId { get; set; }
        public string Name { get; set; }
        public DateTime? DueDate { get; set; }
        public string Category { get; set; }
        public AssigneeViewModelTask Assignee { get; set; }
        public List<LabelViewModel> ListLabel { get; set; }
    }
    public class AssigneeViewModelTask
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
}
