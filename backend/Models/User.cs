using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            Activity = new HashSet<Activity>();
            Tasks = new HashSet<Tasks>();
            UserNotification = new HashSet<UserNotification>();
            ProjectMembers = new HashSet<ProjectMember>();
            Comments = new HashSet<Comment>();
            FbUserTokens = new HashSet<FbUserToken>();
        }

        public string FullName { get; set; }
        public string Description { get; set; }
        public string RollNumber { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Activity> Activity { get; set; }
        public virtual ICollection<Tasks> Tasks { get; set; }
        public virtual ICollection<UserNotification> UserNotification { get; set; }
        public virtual ICollection<ProjectMember> ProjectMembers { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<FbUserToken> FbUserTokens { get; set; }
    }
}
