using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace tms_api.Services.SendEmail
{
    public class SendGridEmailSender : IEmailSender
    {
        public SendGridEmailSenderOptions Options { get; set; }

        public SendGridEmailSender(IOptions<SendGridEmailSenderOptions> options)
        {
            Options = options.Value;
        }

        private async Task<Response> Execute(string apiKey, string email, string templateId, object templateData)
        {
            var client = new SendGridClient(apiKey);

            var msg = new SendGridMessage();

            msg.SetFrom(new EmailAddress(Options.SenderEmail, Options.SenderName));
            msg.SetTemplateId(templateId);
            msg.SetTemplateData(templateData);
            msg.AddTo(new EmailAddress(email));

            // disable tracking settings
            msg.SetClickTracking(false, false);
            msg.SetOpenTracking(false);
            msg.SetGoogleAnalytics(false);
            msg.SetSubscriptionTracking(false);

            return await client.SendEmailAsync(msg);
        }

        public async Task SendEmailAsync(string email, string templateId, object templateData)
        {
            await Execute(Options.ApiKey, email, templateId, templateData);
        }
    }
}
