using CarRentalSystem.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CarRentalSystem.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(
            IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendPasswordResetEmailAsync(
            string email,
            string resetLink)
        {
            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    _emailSettings.FromName,
                    _emailSettings.FromEmail));

            message.To.Add(
                MailboxAddress.Parse(email));

            message.Subject =
                "Reset your DriveEase password";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"""
                    <div style="font-family: Arial, sans-serif;">
                        <h2>Reset Your Password</h2>

                        <p>
                            We received a request to reset
                            your DriveEase password.
                        </p>

                        <p>
                            Click the button below to choose
                            a new password.
                        </p>

                        <a href="{resetLink}"
                           style="
                               display:inline-block;
                               padding:12px 20px;
                               background:#1f3c56;
                               color:white;
                               text-decoration:none;
                               border-radius:6px;
                           ">
                            Reset Password
                        </a>

                        <p style="margin-top:20px;">
                            This link will expire in 30 minutes.
                        </p>

                        <p>
                            If you did not request a password
                            reset, you can ignore this email.
                        </p>
                    </div>
                    """
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var smtpClient = new SmtpClient();

            await smtpClient.ConnectAsync(
                _emailSettings.Host,
                _emailSettings.Port,
                SecureSocketOptions.StartTls);

            await smtpClient.AuthenticateAsync(
                _emailSettings.Username,
                _emailSettings.Password);

            await smtpClient.SendAsync(message);

            await smtpClient.DisconnectAsync(true);
        }
    }
}