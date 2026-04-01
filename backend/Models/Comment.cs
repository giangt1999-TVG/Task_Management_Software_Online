using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace tms_api.Models
{
    public partial class Comment
    {
        public int CommentId { get; set; }
        public string UserId { get; set; }
        public int TaskId { get; set; }
        public string Content { get; set; }
        public string AttachFile { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Tasks Task { get; set; }
        public virtual User User { get; set; }
    }
}
