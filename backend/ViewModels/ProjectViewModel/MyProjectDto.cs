using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.ViewModels.ProjectViewModel
{
    public class MyProjectDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<UserDto> Members { get; set; }
    }

    public class UserDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        public string Role { get; set; }
    }
}
