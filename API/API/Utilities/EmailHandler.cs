using API.Common;
using API.Configurations;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace API.Utilities
{
    public static class EmailHandler
    {
        private static readonly string EmailDisplayName = ConfigManager.gI().EmailDisplayName;
        private static readonly string EmailHost = ConfigManager.gI().EmailHost;
        private static readonly string EmailUsername = ConfigManager.gI().EmailUsername;
        private static readonly string EmailPassword = ConfigManager.gI().EmailPassword;

        public static async Task<string> SendEmailAsync(string To, string Subject, string Body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(EmailDisplayName, EmailUsername));
            email.To.Add(MailboxAddress.Parse(To));
            email.Subject = Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = Body };

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(EmailHost, 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(EmailUsername, EmailPassword);
                await smtp.SendAsync(email);
            }
            catch (Exception e) { return $"{e.Message}: inner: {e.InnerException}"; }
            finally { smtp.Disconnect(true); }

            return "";
        }

        public static async Task<string> SendOtpAndSaveSession(string email, HttpContext httpContext)
        {
            int otp = Utils.Generate6Number();
            httpContext.Session.SetString("Otp", otp.ToString()); // Lưu OTP

            var emailVerify = httpContext.Session.GetString("email_verify");
            if (string.IsNullOrEmpty(emailVerify))
                httpContext.Session.SetString("email_verify", email); // Lưu email to verify

            await httpContext.Session.CommitAsync(); // ✅ Đảm bảo session được lưu

            string msg = await SendEmailAsync(email, "Xác thực Email của bạn", $"Đây là mã xác thực của bạn: {otp}");
            if (msg.Length > 0) return msg;
            return "";
        }

        public static async Task<(string message, string pass)> SendEmailAndPassword(string email, HttpContext httpContext)
        {
            string pass = Utils.Generate6Character();

            string msg = await SendEmailAsync(email, "Khôi phục mật khẩu", $"Đây là mật khẩu mới của bạn: {pass}");
            if (msg.Length > 0) return (msg, "");
            return (string.Empty, pass);
        }
    }

}
