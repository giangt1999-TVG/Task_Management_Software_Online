using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tms_api.Services.SendEmail
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string templateId, object templateData);
    }
}
