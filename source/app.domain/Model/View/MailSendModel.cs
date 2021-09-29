namespace app.domain.Model.View
{
    public class MailSendModel : ViewBaseModel
    {
        public int To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
