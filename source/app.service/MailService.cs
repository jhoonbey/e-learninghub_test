using app.domain.Exceptions;
using app.domain.Model.Entities;
using app.service.Model.Mail;
using app.service.Model.Response;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace app.service
{
    public class MailService : IMailService
    {
        private readonly IOptionsSnapshot<MailConfigOptions> _configuration;
        private readonly ILogger<MailService> _logger;

        public MailService(IOptionsSnapshot<MailConfigOptions> configuration, ILogger<MailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public GenericServiceResponse<Mail> Send(Mail model)
        {
            var response = new GenericServiceResponse<Mail>();
            try
            {
                ServicePointManager.ServerCertificateValidationCallback =
                delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };

                var config = _configuration.Value;

                ThreadPool.QueueUserWorkItem(t =>
                {
                    MailMessage message = new MailMessage
                    {
                        From = new MailAddress(model.FromMail, config.FromDisplayName, Encoding.UTF8),
                        Subject = model.Subject,
                        IsBodyHtml = true,
                        Body = model.Body,
                        SubjectEncoding = Encoding.UTF8,
                        BodyEncoding = Encoding.UTF8
                    };

                    //to 
                    message.To.Add(model.ToMail);

                    using (var client = new SmtpClient())
                    {
                        client.EnableSsl = config.EnableSSl;
                        client.Host = config.Host;
                        client.Port = config.Port;
                        client.Timeout = config.TimeOut;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.Credentials = new NetworkCredential(config.Username, config.Password);
                        try
                        {
                            client.SendAsync(message, message);

                            _logger.LogError($"Mail sent. id = {  model.Id } ");
                            model.HasSent = true;
                            model.SentDate = DateTime.UtcNow;
                            response.IsSuccessfull = true;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error on mail sent id = {  model.Id } Error: " + ex.ToString());
                            model.HasSent = false;
                            model.SentError = ex.Message;
                            response.IsSuccessfull = false;
                        }

                    }
                });


                response.Model = model;
            }
            catch (BusinessException exp)
            {
                response.IsBusinessError = true;
                response.BusinessMessage = exp.Message;
            }
            catch (Exception exp)
            {
                response.ErrorMessage = exp.ToString();
            }
            return response;
        }



        //private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        //{
        //    // Get the message we sent
        //    MailMessage msg = (MailMessage)e.UserState;

        //    if (e.Cancelled)
        //    {
        //        // prompt user with "send cancelled" message 
        //    }
        //    if (e.Error != null)
        //    {
        //        _logger.LogError($"Mail can not sent = { e.Error.Message  } ");
        //    }
        //    else
        //    {
        //        // prompt user with message sent!
        //        // as we have the message object we can also display who the message
        //        // was sent to etc 
        //    }

        //    // finally dispose of the message
        //    if (msg != null)
        //        msg.Dispose();
        //}


        //public async Task<ServiceResponseBase> SendEmailAsync(
        //    MailAddress sender, MailAddress[] recepients, string subject, string body, bool isHtml = true, MailAddress[] bccList = null)
        //{
        //    var response = new ServiceResponseBase();
        //    try
        //    {
        //        MailMessage message = new MailMessage
        //        {
        //            From = sender,
        //            Subject = subject,
        //            IsBodyHtml = isHtml,
        //            Body = body,
        //            SubjectEncoding = Encoding.UTF8,
        //            BodyEncoding = Encoding.UTF8
        //        };

        //        //to list
        //        foreach (var recepient in recepients)
        //            message.To.Add(recepient);

        //        //bbc list
        //        if (bccList != null)
        //        {
        //            foreach (var recepient in recepients)
        //                message.Bcc.Add(recepient);
        //        }

        //        var emailMessage = ""; // AddEmailMessageToStore(message);
        //        using (var client = new SmtpClient())
        //        {
        //            var config = _configuration.Value;

        //            if (config.EnableSSl)
        //                client.EnableSsl = true;
        //            client.Host = config.Host;
        //            client.Port = config.Port;
        //            client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //            client.Credentials = new NetworkCredential(config.Username, rootConfig[config.Password]);


        //            await client.SendMailAsync(message);
        //            emailMessage.IsSent = true;
        //            emailMessage.IsSuccessful = true;
        //            emailMessage.DateSent = DateTime.Now;
        //            emailMessage.DateModified = DateTime.Now;
        //            SaveEmailMessage(emailMessage);
        //        }

        //        response.IsSuccessfull = true;
        //    }
        //    catch (BusinessException exp)
        //    {
        //        response.IsBusinessError = true;
        //        response.BusinessMessage = exp.Message;
        //    }
        //    catch (Exception exp)
        //    {
        //        response.ErrorMessage = exp.ToString();
        //    }
        //    return response;
        //}
    }
}
