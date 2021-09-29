using System.Net.Mail;

namespace app.service.Model.Mail
{
    public class MailViewModel
    {
        private int _id;
        private MailMessage _mailMessage;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public MailMessage MailMessage
        {
            get { return _mailMessage; }
            set { _mailMessage = value; }
        }

        public MailViewModel(MailMessage mailMessage, int id)
        {
            _mailMessage = mailMessage;
            _id = id;
        }
    }
}
