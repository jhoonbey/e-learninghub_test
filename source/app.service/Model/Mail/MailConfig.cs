namespace app.service.Model.Mail
{
    public class MailConfigOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public int TimeOut { get; set; }
        public bool EnableSSl { get; set; }
        public string FromMail { get; set; }
        public string FromDisplayName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
