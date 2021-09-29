using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace app.domain.Utilities
{
    public class EmailManager
    {
        public static void SendEmail(string host, int port, int timeout, bool enableSsl, string username, string password,
                                            string emailFrom, string emailTo, string subject, string body)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate,
                X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };

            using (var client = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = username,
                    Password = password
                };

                client.Credentials = credential;
                client.Host = host;
                client.Port = port;
                client.Timeout = timeout;
                client.EnableSsl = enableSsl;

                using (var emailMessage = new MailMessage())
                {
                    emailMessage.To.Add(new MailAddress(emailTo));
                    emailMessage.From = new MailAddress(emailFrom);
                    emailMessage.Subject = subject;
                    emailMessage.IsBodyHtml = true;
                    emailMessage.Body = body;
                    client.Send(emailMessage);
                }
            }
        }
    }
}
