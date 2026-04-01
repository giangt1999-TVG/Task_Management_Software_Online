using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace tms_api.ViewModels.IdentityViewModels
{
    public class UserManagerViewModel : BaseViewModel<UserDto>
    {
        public UserManagerViewModel(HttpStatusCode statusCode, string message, UserDto data)
        {
            StatusCode = statusCode;
            Data = data;
            Message = message;
        }
    }

    public class UserDto
    {
        public string Email { get; set; }
        public string UserName { get; set; }
    }
}
