using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.RequestModels.CommentRequestModels
{
    public class UpdateCommentRequestModel
    {
        [Required]
        public int CommentId { get; set; }
        public string Content { get; set; }
        public string AttachFile { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
