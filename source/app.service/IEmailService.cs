using app.domain.Model.Entities;
using System.Collections.Generic;

namespace app.service
{
    public interface IEmailService
    {
        void Send(List<Mail> model, bool stopOnException);
        void SendAysnc(List<Mail> model, int delay, bool stopOnException);
        string GenerateConfirmationMailBody(User model);
        string GenerateRegisteredMailBody(User model, string code);
        string GenerateResetMailBody(User model, string code);
        string GenerateResetAlreadyMailBody(User model);

        void SendMailTest(string subject, string body);

    }
}
