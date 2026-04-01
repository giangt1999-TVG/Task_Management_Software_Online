using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.ViewModels.ProjectViewModel
{
    public class ProjectInformationViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<UserInProjectViewModel> listUser { get; set; }
        public List<LabelViewModel> ListLabel { get; set; }
    }
    public class UserInProjectViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        public string Role { get; set; }
    }

    public class LabelViewModel
    {
        public int LabelId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }
}
