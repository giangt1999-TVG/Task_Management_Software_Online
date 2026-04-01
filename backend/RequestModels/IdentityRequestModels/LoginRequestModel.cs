using System.ComponentModel.DataAnnotations;

namespace tms_api.RequestModels.IdentityRequestModels
{
    public class LoginRequestModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PassWord { get; set; }
    }
}
