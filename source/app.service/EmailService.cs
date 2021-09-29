using app.data;
using app.domain.Exceptions;
using app.domain.Languages;
using app.domain.Model.Entities;
using app.service.Model.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace app.service
{
    public class EmailService : IEmailService //, IDisposable //IHostedService,
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IOptionsSnapshot<MailConfigOptions> _mailConfiguration;
        private readonly IConfiguration _configuration;
        private readonly List<MailViewModel> _mails;
        private readonly SmtpClient _smtpClient;
        private readonly IEntityRepository _entityRepository;

        public EmailService(ILogger<EmailService> logger,
                            IOptionsSnapshot<MailConfigOptions> mailConfiguration,
                            IConfiguration configuration,
                            IEntityRepository entityRepository)
        {
            _logger = logger;
            _mailConfiguration = mailConfiguration;
            _configuration = configuration;
            _mails = new List<MailViewModel>();
            _smtpClient = CreateSmtpClient();
            _entityRepository = entityRepository;
        }

        private SmtpClient CreateSmtpClient()
        {
            return new SmtpClient
            {
                Host = _mailConfiguration.Value.Host,
                Port = _mailConfiguration.Value.Port,
                EnableSsl = _mailConfiguration.Value.EnableSSl,
                Credentials = new NetworkCredential(userName: _mailConfiguration.Value.Username, password: _mailConfiguration.Value.Password),
                Timeout = _mailConfiguration.Value.TimeOut

                //Host = _configuration["Smtp:Host"],
                //Port = Convert.ToInt32(_configuration["Smtp:Port"]),
                //EnableSsl = Convert.ToBoolean(_configuration["Smtp:Ssl"]),
                //UseDefaultCredentials = false,
                //DeliveryMethod = SmtpDeliveryMethod.Network,
                //Credentials = new NetworkCredential(userName: _configuration["Smtp:Username"], password: _configuration["Smtp:Password"])
            };
        }
        private void Load(List<Mail> model)
        {
            foreach (var item in model)
            {
                _mails.Add
                (
                    new MailViewModel
                    (
                        new MailMessage
                        (
                            from: item.FromMail, //new MailAddress(model.FromMail, model.FromDisplayName ?? model.FromMail, Encoding.UTF8),
                            to: item.ToMail      //new MailAddress(model.ToMail, model.FromDisplayName ?? model.ToMail, Encoding.UTF8),
                        )
                        {
                            Subject = item.Subject,
                            SubjectEncoding = Encoding.UTF8,
                            Body = item.Body,
                            IsBodyHtml = true,
                            BodyEncoding = Encoding.UTF8
                        },
                        item.Id
                    )
                );
            }
        }
        public void SendAysnc(List<Mail> model, int delay, bool stopOnException = false)
        {
            Task.Run(async () =>
            {
                await SendAysncWithDelay(model, delay, stopOnException);
            });
        }
        public void Send(List<Mail> model, bool stopOnException = false)
        {
            _logger.LogInformation("E-mail background delivery started loading mails");
            Load(model);
            _logger.LogInformation("E-mail background delivery started while loop");

            MailViewModel mailViewModel = null;

            while (_mails.Count > 0)
            {
                try
                {
                    _logger.LogInformation($"Buffer count before =  {_mails.Count}");
                    mailViewModel = _mails.ElementAt(0);

                    _logger.LogInformation($"Mail will send with id = {mailViewModel.Id}");
                    _smtpClient.SendAsync(mailViewModel.MailMessage, null);
                    _logger.LogInformation($"E-mail sent with id =  {mailViewModel.Id}");

                    //update mail in db
                    Dictionary<string, object> columns = new Dictionary<string, object>
                                {
                                    { "HasSent", true },
                                    { "SentDate", DateTime.UtcNow }
                                };
                    _entityRepository.UpdateBy<Mail>(columns, "Id", mailViewModel.Id);
                    _logger.LogInformation($"Mail updated in DB with id = {mailViewModel.Id}");

                    //remove message from list
                    _mails.Remove(mailViewModel);
                    _logger.LogInformation($"Buffer count after =  {_mails.Count}");

                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Error on e-mail sending: { ex.ToString()} ");

                    //update mail in db
                    Dictionary<string, object> columns = new Dictionary<string, object>
                                {
                                    { "SentError", ex.Message },
                                    { "SentDate", DateTime.UtcNow }
                                };
                    _entityRepository.UpdateBy<Mail>(columns, "Id", mailViewModel.Id);
                    _logger.LogInformation($"Mail sending error updated in DB with id = {mailViewModel.Id}");

                    if (stopOnException)
                    {
                        throw new BusinessException("Email", Lang.ErrorOnMailSending);
                    }
                }
                finally
                {
                }
            }

            _logger.LogInformation("E-mail background delivery stopped");
        }

        private async Task SendAysncWithDelay(List<Mail> model, int delay, bool stopOnException = false)
        {
            _logger.LogInformation("E-mail background delivery started loading mails");
            Load(model);
            _logger.LogInformation("E-mail background delivery started while loop");

            MailViewModel mailViewModel = null;

            while (_mails.Count > 0)
            {
                try
                {
                    _logger.LogInformation($"Buffer count before =  {_mails.Count}");
                    mailViewModel = _mails.ElementAt(0);

                    _logger.LogInformation($"Mail will send with id = {mailViewModel.Id}");
                    _smtpClient.SendAsync(mailViewModel.MailMessage, null);
                    _logger.LogInformation($"E-mail sent with id =  {mailViewModel.Id}");

                    //update mail in db
                    Dictionary<string, object> columns = new Dictionary<string, object>
                                {
                                    { "HasSent", true },
                                    { "SentDate", DateTime.UtcNow }
                                };
                    _entityRepository.UpdateBy<Mail>(columns, "Id", mailViewModel.Id);
                    _logger.LogInformation($"Mail updated in DB with id = {mailViewModel.Id}");

                    //remove message from list
                    _mails.Remove(mailViewModel);
                    _logger.LogInformation($"Buffer count after =  {_mails.Count}");

                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Error on e-mail sending: { ex.ToString()} ");

                    //update mail in db
                    Dictionary<string, object> columns = new Dictionary<string, object>
                                {
                                    { "SentError", ex.Message },
                                    { "SentDate", DateTime.UtcNow }
                                };
                    _entityRepository.UpdateBy<Mail>(columns, "Id", mailViewModel.Id);
                    _logger.LogInformation($"Mail sending error updated in DB with id = {mailViewModel.Id}");

                    if (stopOnException)
                    {
                        throw new BusinessException("Email", Lang.ErrorOnMailSending);
                    }
                }
                finally
                {
                }

                await Task.Delay(1000);
            }

            _logger.LogInformation("E-mail background delivery stopped");
        }

        public string GenerateConfirmationMailBody(User model)
        {
            return @"<div><div>
                <table  cellpadding=""0"" cellspacing=""0"" border=""0"" height=""100%"" width=""100%"" style=""border-spacing:0;border-collapse:collapse;margin:0 auto"" bgcolor=""#f0f0f0"">
                  <tbody><tr>
                    <td valign=""top"">
                      <center style=""width:100%"">
                        <div  style=""max-width:600px;margin:auto"">
                           <br/>
		                        <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" width=""100%"" style=""border-spacing:0;border-collapse:collapse;max-width:600px;margin:0 auto"" bgcolor=""#ffffff"">
			                        <tbody>
				                        <tr>
					                        <td>
						                        <table border=""0"" cellpadding=""30"" cellspacing=""0"" width=""100%"" style=""border-spacing:0;border-collapse:collapse;margin:0 auto"">
							                        <tbody>
								                        <tr>
									                        <td valign=""top""  style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#444;font-size:14px;line-height:150%"">
										                        <div style=""margin-bottom:20px;padding-bottom:25px;border-bottom-width:1px;border-bottom-color:#eee;border-bottom-style:solid"">
											                        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border-spacing:0;border-collapse:collapse;margin:0 auto"">
												                        <tbody>
													                        <tr>
														                        <td>
															                        <h1  style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#444;display:block;font-size:15px;font-weight:500;line-height:1.3;margin:0"">"
                                                                                      + Lang.EmailConfirmationMailBodyHi1 + " " + model.Name + " " + model.Surname + "," +

                                                                                           @"</h1>
														                        </td>
														                        <td width=""58"" align=""right"" >
															                        <a  rel=""contact"" 
                                                                                        href=""" + _configuration["Site:Link"].ToString() + @""" 
															                        style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none""                                                    target=""_blank"" >
																                    <img  width=""48"" height=""48"" src=""" + _configuration["Site:Link"].ToString() + @"/img/logo.jpeg"" 
																      style=""height:auto;line-height:100%;outline:none;text-decoration:none;display:inline;width:130px;border-radius:50%;border:0"">
                                                                                    </a>
															                        </td>
														                        </tr>
													                        </tbody>
												                        </table>
											                        </div>
											                        <p style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">
                                                                        " + Lang.EmailConfirmationMailBodyConfirmPart21 + @"
												                        <a href=""mailto:" + model.Email + @""" target=""_blank"">" + model.Email + @"</a> " + Lang.EmailConfirmationMailBodyConfirmPart22 +
                                                                           @"</p>
											                        <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center""  style=""border-spacing:0;border-collapse:collapse;margin:auto"">
												                        <tbody>
													                        <tr>
														                        <td style=""border-radius:4px"" align=""center"" bgcolor=""#95a844"">
															                        <a  href=""" + _configuration["Site:Link"].ToString() + "/Account/Confirm?code=" + WebUtility.UrlEncode(model.ConfirmationGuid) + @"""  
															                        style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none;background-color:#95a844;font-size:13px;line-height:1.1;text-align:center;display:block;border-radius:4px;border:15px solid #95a844"" target=""_blank"" >
                                &nbsp;&nbsp;&nbsp;&nbsp;
																                        <span style=""color:#ffffff"">
                                   " + Lang.EmailConfirmationMailBodyButton3 + @"
                                </span>
                                &nbsp;&nbsp;&nbsp;&nbsp;

															                        </a>
														                        </td>
													                        </tr>
												                        </tbody>
											                        </table>
											                        <p style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">
                                                                   " + Lang.EmailConfirmationMailBodyIf4 + @" </p> <br/>
                                                                        
                                                                    <a href=""" + _configuration["Site:Link"].ToString() + "/Account/Confirm?code=" + WebUtility.UrlEncode(model.ConfirmationGuid) + @"""  
													                        style=""color:#15c;font-family:&quot;Helvetica Neue&quot;,sans-serif;font-weight:500;text-decoration:none"" target=""_blank"" >
													                       " + _configuration["Site:Link"].ToString() + "/Account/Confirm?code=" + WebUtility.UrlEncode(model.ConfirmationGuid) + @"
													                </a>

                                                                    <br />
											                        <p style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">" + Lang.EmailConfirmationMailBodyBye5 + @"   <br />
                                                                            " + Lang.EmailMailBodyTeam + @"
												                    </p>
                                                                    
                                                                    <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">"
                                                                            + Lang.EmailBodyFooterTitle1 + @"<br>
												                    </p>
                                                                      <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">

                                                                      <a  rel=""contact"" href=""https://www.facebook.com/work.az/"" align=""center"" 
															             style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none"" target=""_blank"" >
																         <img  width=""30"" height=""30"" src=""https://instagram.fgyd4-2.fna.fbcdn.net/vp/ce1894490512b913cc07fa2fb9fc006a/5CE27941/t51.2885-19/s150x150/45428020_1448496848586347_8199584612402331648_n.jpg?_nc_ht=instagram.fgyd4-2.fna.fbcdn.net"" 
																         style=""height:auto;line-height:100%;outline:none;text-decoration:none;display:inline;width:50px;border-radius:50%;border:0"">
                                                                        </a>
                                                                       <a  rel=""contact"" href=""https://www.linkedin.com/company/work-az/about/""  align=""center"" 
															             style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none"" target=""_blank"" >
																         <img  width=""30"" height=""30"" src=""https://instagram.fgyd4-2.fna.fbcdn.net/vp/ce1894490512b913cc07fa2fb9fc006a/5CE27941/t51.2885-19/s150x150/45428020_1448496848586347_8199584612402331648_n.jpg?_nc_ht=instagram.fgyd4-2.fna.fbcdn.net"" 
																         style=""height:auto;line-height:100%;outline:none;text-decoration:none;display:inline;width:50px;border-radius:50%;border:0"">
																         </a>
                                                                       <a  rel=""contact"" href=""https://www.instagram.com/cocacola/"" align=""center"" 
                                                                        style =""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none"" target=""_blank"" >
																         <img  width=""30"" height=""30"" src=""https://instagram.fgyd4-2.fna.fbcdn.net/vp/ce1894490512b913cc07fa2fb9fc006a/5CE27941/t51.2885-19/s150x150/45428020_1448496848586347_8199584612402331648_n.jpg?_nc_ht=instagram.fgyd4-2.fna.fbcdn.net"" 
																         style=""height:auto;line-height:100%;outline:none;text-decoration:none;display:inline;width:50px;border-radius:50%;border:0"">
																         </a>
                                                                        <a  rel=""contact"" href=""https://twitter.com/work_az""  align=""center"" 
															             style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none"" target=""_blank"" >
																         <img  width=""30"" height=""30"" src=""https://instagram.fgyd4-2.fna.fbcdn.net/vp/ce1894490512b913cc07fa2fb9fc006a/5CE27941/t51.2885-19/s150x150/45428020_1448496848586347_8199584612402331648_n.jpg?_nc_ht=instagram.fgyd4-2.fna.fbcdn.net"" 
																         style=""height:auto;line-height:100%;outline:none;text-decoration:none;display:inline;width:50px;border-radius:50%;border:0"">
																         </a>
                                                                        </p>
                                                                        
                                                                        <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:12px;line-height:1.5;color:#555"">"
                                                                                + Lang.EmailBodyFooterTitle2 + @"<br>
												                        </p>
											                        </td>
										                        </tr>
									                        </tbody>
								                        </table>
							                        </td>
						                        </tr>
					                        </tbody>
				                        </table>
                                 </div>
                            </div>
                        </div>";
        }
        public string GenerateRegisteredMailBody(User model, string code)
        {
            return @"<div><div>
                <table  cellpadding=""0"" cellspacing=""0"" border=""0"" height=""100%"" width=""100%"" style=""border-spacing:0;border-collapse:collapse;margin:0 auto"" bgcolor=""#f0f0f0"">
                  <tbody><tr>
                    <td valign=""top"">
                      <center style=""width:100%"">
                        <div  style=""max-width:600px;margin:auto"">
                           <br/>
		                        <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" width=""100%"" style=""border-spacing:0;border-collapse:collapse;max-width:600px;margin:0 auto"" bgcolor=""#ffffff"">
			                        <tbody>
				                        <tr>
					                        <td>
						                        <table border=""0"" cellpadding=""30"" cellspacing=""0"" width=""100%"" style=""border-spacing:0;border-collapse:collapse;margin:0 auto"">
							                        <tbody>
								                        <tr>
									                        <td valign=""top""  style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#444;font-size:14px;line-height:150%"">
										                        <div style=""margin-bottom:20px;padding-bottom:25px;border-bottom-width:1px;border-bottom-color:#eee;border-bottom-style:solid"">
											                        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border-spacing:0;border-collapse:collapse;margin:0 auto"">
												                        <tbody>
													                        <tr>
														                          <td>
															                            <P  
                                                                                        style=""float:left; font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#444;display:block;font-size:15px;font-weight:500;line-height:1.3;margin:0"">"
                                                                                         + Lang.EmailConfirmationMailBodyHi1 + " " + model.Name + " " + model.Surname + "," +
                                                                                        @"</P> 
														                          </td>
														                          <td>
																                    <a  rel=""contact"" href=""" + _configuration["Site:Link"].ToString() + @""" 
															                                                style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none""                                                    target=""_blank"" >
																                         <img  width=""48"" height=""48"" src=""" + _configuration["Site:Link"].ToString() + @"/img/logo.jpeg"" 
																                                        style=""height:auto;line-height:100%;outline:none;text-decoration:none;display:inline;width:130px;border-radius:50%;border:0"">
                                                                                    </a>
														                          </td>
														                        </tr>
															                   <tr>    
															                        <td>  
                                                                                        <p  style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#444;display:block;font-size:15px;font-weight:500;line-height:1.3;margin:0"">"
                                                                                          + Lang.EmailRegisteredMailBodyText1 +
                                                                                      @"</p> <br /> 
														                            </td>
															                    </tr> 
                                                                                 <tr>   
                                                                                     <td>  
                                                                                       <h1  style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#444;display:block;font-size:15px;font-weight:500;line-height:1.3;margin:0"">"
                                                                                          + Lang.EmailRegisteredMailBodyText2 +
                                                                                     @"</h1> <br /> <br />
														                             </td>
														                      </tr>
													                        </tbody>
												                        </table>
											                        </div>

                                                                      <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">"
                                                                            + "<b>" + Lang.EmailRegisteredMailBodyText3 + @" </b> <br />
												                      </p>
                                                                      <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">"
                                                                            + Lang.EmailRegisteredMailBodyText4 + @" <br />
												                      </p>
                                                                      <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">
                                                                            <img  width=""80"" height=""80"" src=""" + _configuration["Site:Link"].ToString() + @"/img/discount/discount10.png"" 
																                              style=""height:auto;line-height:100%;outline:none;text-decoration:none;display:inline;width:130px;border-radius:50%;border:0"" />" +
                                                                            Lang.EmailRegisteredMailDiscountText +
                                                                      @"</p>
                                                                      <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">"
                                                                             + "<b>" + Lang.EmailRegisteredMailBodyText5 + "</b> " + code + @"<br />
												                      </p>
                                                                      <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">"
                                                                             + Lang.EmailRegisteredMailBodyText6 + " " + DateTime.Today.AddMonths(1).ToString("yyy-MM-dd") + @" <br /> <br />
												                      </p>  

                                                                      <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">
                                                                         <a  href=""" + _configuration["Site:Link"].ToString() + @"""  
															               style=""width: 30%;font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none;background-color:#95a844;font-size:13px;line-height:1.1;text-align:center;display:block;border-radius:4px;border:15px solid #95a844"" target=""_blank"" >
                                                                           &nbsp;&nbsp;&nbsp;&nbsp;
																           <span style=""color:#ffffff"">
                                                                            " + Lang.EmailRegisteredMailBodyButtonText7 + @"
                                                                            </span>
                                                                        </a>
												                      </p> <br /> <br />

                                                                      <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">"
                                                                            + Lang.EmailBodyFooterTitle1 + @"<br />
												                      </p>
                                                                      <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">
                                                                      <a  rel=""contact"" href=""https://www.facebook.com/work.az/"" align=""center"" 
															             style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none"" target=""_blank"" >
																         <img  width=""30"" height=""30"" src=""https://instagram.fgyd4-2.fna.fbcdn.net/vp/ce1894490512b913cc07fa2fb9fc006a/5CE27941/t51.2885-19/s150x150/45428020_1448496848586347_8199584612402331648_n.jpg?_nc_ht=instagram.fgyd4-2.fna.fbcdn.net"" 
																         style=""height:auto;line-height:100%;outline:none;text-decoration:none;display:inline;width:50px;border-radius:50%;border:0"">
                                                                        </a>
                                                                       <a  rel=""contact"" href=""https://www.linkedin.com/company/work-az/about/""  align=""center"" 
															             style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none"" target=""_blank"" >
																         <img  width=""30"" height=""30"" src=""https://instagram.fgyd4-2.fna.fbcdn.net/vp/ce1894490512b913cc07fa2fb9fc006a/5CE27941/t51.2885-19/s150x150/45428020_1448496848586347_8199584612402331648_n.jpg?_nc_ht=instagram.fgyd4-2.fna.fbcdn.net"" 
																         style=""height:auto;line-height:100%;outline:none;text-decoration:none;display:inline;width:50px;border-radius:50%;border:0"">
																         </a>
                                                                       <a  rel=""contact"" href=""https://www.instagram.com/cocacola/"" align=""center"" 
                                                                        style =""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none"" target=""_blank"" >
																         <img  width=""30"" height=""30"" src=""https://instagram.fgyd4-2.fna.fbcdn.net/vp/ce1894490512b913cc07fa2fb9fc006a/5CE27941/t51.2885-19/s150x150/45428020_1448496848586347_8199584612402331648_n.jpg?_nc_ht=instagram.fgyd4-2.fna.fbcdn.net"" 
																         style=""height:auto;line-height:100%;outline:none;text-decoration:none;display:inline;width:50px;border-radius:50%;border:0"">
																         </a>
                                                                        <a  rel=""contact"" href=""https://twitter.com/work_az""  align=""center"" 
															             style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none"" target=""_blank"" >
																         <img  width=""30"" height=""30"" src=""https://instagram.fgyd4-2.fna.fbcdn.net/vp/ce1894490512b913cc07fa2fb9fc006a/5CE27941/t51.2885-19/s150x150/45428020_1448496848586347_8199584612402331648_n.jpg?_nc_ht=instagram.fgyd4-2.fna.fbcdn.net"" 
																         style=""height:auto;line-height:100%;outline:none;text-decoration:none;display:inline;width:50px;border-radius:50%;border:0"">
																         </a>
                                                                        </p>
                                                                        <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:13px;line-height:1.5;color:#555"">"
                                                                                + Lang.EmailBodyFooterTitle2 + @"<br />
												                        </p>
                                                                        <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:12px;line-height:1.5;color:#555"">"
                                                                            + Lang.EmailRegisteredMailBodyText11 + @"<br />
												                      </p>
											                        </td>
										                        </tr>
									                        </tbody>
								                        </table>
							                        </td>
						                        </tr>
					                        </tbody>
				                        </table>
                                 </div>
                            </div>
                        </div>";
        }
        public string GenerateResetMailBody(User model, string code)
        {
            return @"<div><div>
                <table  cellpadding=""0"" cellspacing=""0"" border=""0"" height=""100%"" width=""100%"" style=""border-spacing:0;border-collapse:collapse;margin:0 auto"" bgcolor=""#f0f0f0"">
                  <tbody><tr>
                    <td valign=""top"">
                      <center style=""width:100%"">
                        <div  style=""max-width:600px;margin:auto"">
                           <br/>
		                        <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" width=""100%"" style=""border-spacing:0;border-collapse:collapse;max-width:600px;margin:0 auto"" bgcolor=""#ffffff"">
			                        <tbody>
				                        <tr>
					                        <td>
						                        <table border=""0"" cellpadding=""30"" cellspacing=""0"" width=""100%"" style=""border-spacing:0;border-collapse:collapse;margin:0 auto"">
							                        <tbody>
								                        <tr>
									                        <td valign=""top""  style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#444;font-size:14px;line-height:150%"">
										                        <div style=""margin-bottom:20px;padding-bottom:25px;border-bottom-width:1px;border-bottom-color:#eee;border-bottom-style:solid"">
											                        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border-spacing:0;border-collapse:collapse;margin:0 auto"">
												                        <tbody>
													                        <tr>
														                          <td>
															                            <P  
                                                                                        style=""float:left; font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#444;display:block;font-size:15px;font-weight:500;line-height:1.3;margin:0"">"
                                                                                         + Lang.EmailResetMailBodyHi + " " + model.Name + " " + model.Surname + "," +
                                                                                        @"</P> 
														                          </td>
														                          <td>
																                    <a  rel=""contact"" href=""" + _configuration["Site:Link"].ToString() + @""" 
															                                                style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none""                                                    target=""_blank"" >
																                         <img  width=""48"" height=""48"" src=""" + _configuration["Site:Link"].ToString() + @"/img/logo.jpeg"" 
																                                        style=""height:auto;line-height:100%;outline:none;text-decoration:none;display:inline;width:130px;border-radius:50%;border:0"">
                                                                                    </a>
														                          </td>
														                        </tr>
															                   <tr>    
															                        <td>  
                                                                                        <p  style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#444;display:block;font-size:15px;font-weight:500;line-height:1.3;margin:0"">"
                                                                                          + Lang.EmailResetMailBodyText +
                                                                                      @"</p> <br /> 
														                            </td>
															                    </tr> 
                                                                                <tr>    
															                        <td>  
                                                                                        <p  style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#444;display:block;font-size:15px;font-weight:500;line-height:1.3;margin:0"">"
                                                                                          + Lang.EmailResetMailBodyText2 +
                                                                                      @"</p> <br /> 
														                            </td>
															                    </tr> 
													                        </tbody>
												                        </table>
											                        </div>"

                                                                     + @" <br /> <br />
												                      </p>
                                                                      <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">
															                        <a  href=""" + _configuration["Site:Link"].ToString() + "/Account/ResetPassword?code=" + WebUtility.UrlEncode(code) + @"""  
															                        style=""width: 30%;font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none;background-color:#95a844;font-size:13px;line-height:1.1;text-align:center;display:block;border-radius:4px;border:15px solid #95a844"" target=""_blank"" >
                                                                                        &nbsp;&nbsp;&nbsp;&nbsp; <span style=""color:#ffffff"">
                                                                                           " + Lang.EmailResetMailBodyButtonText + @"
                                                                                        </span> &nbsp;&nbsp;&nbsp;&nbsp;
															                        </a>
                                                                        <br />
												                      </p>

                                                                    <p style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">
                                                                   " + Lang.EmailResetMailBodyText3 + @" </p> <br/>
                                                                        
                                                                     <a href=""" + _configuration["Site:Link"].ToString() + "/Account/ResetPassword?code=" + WebUtility.UrlEncode(code) + @"""  
													                        style=""color:#15c;font-family:&quot;Helvetica Neue&quot;,sans-serif;font-weight:500;text-decoration:none"" target=""_blank"" >
													                       " + _configuration["Site:Link"].ToString() + "/Account/ResetPassword?code=" + WebUtility.UrlEncode(code) + @"
													                 </a> <br />


                                                                      <p  style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">"
                                                                            + Lang.EmailResetMailBodyText4 + @"
												                      </p> <br /> <br />
                                                                      <p  style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">"
                                                                            + Lang.EmailResetMailBodyText5 + @"
												                      </p> <br /> 

                                                                    <br>
											                        <p style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">" + Lang.EmailResetMailBodyBye + @" <br/>
                                                                            " + Lang.EmailResetMailBodyTechnicalTeam + @"
												                    </p>
                                                                    <br>
                                                                    <br>

                                                                      <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">"
                                                                            + Lang.EmailBodyFooterTitle1 + @"<br>
												                      </p>
                                                                      <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">
                                                                      <a  rel=""contact"" href=""https://www.facebook.com/work.az/"" align=""center"" 
															             style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none"" target=""_blank"" >
																         <img  width=""30"" height=""30"" src=""https://instagram.fgyd4-2.fna.fbcdn.net/vp/ce1894490512b913cc07fa2fb9fc006a/5CE27941/t51.2885-19/s150x150/45428020_1448496848586347_8199584612402331648_n.jpg?_nc_ht=instagram.fgyd4-2.fna.fbcdn.net"" 
																         style=""height:auto;line-height:100%;outline:none;text-decoration:none;display:inline;width:50px;border-radius:50%;border:0"">
                                                                        </a>
                                                                       <a  rel=""contact"" href=""https://www.linkedin.com/company/work-az/about/""  align=""center"" 
															             style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none"" target=""_blank"" >
																         <img  width=""30"" height=""30"" src=""https://instagram.fgyd4-2.fna.fbcdn.net/vp/ce1894490512b913cc07fa2fb9fc006a/5CE27941/t51.2885-19/s150x150/45428020_1448496848586347_8199584612402331648_n.jpg?_nc_ht=instagram.fgyd4-2.fna.fbcdn.net"" 
																         style=""height:auto;line-height:100%;outline:none;text-decoration:none;display:inline;width:50px;border-radius:50%;border:0"">
																         </a>
                                                                       <a  rel=""contact"" href=""https://www.instagram.com/cocacola/"" align=""center"" 
                                                                        style =""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none"" target=""_blank"" >
																         <img  width=""30"" height=""30"" src=""https://instagram.fgyd4-2.fna.fbcdn.net/vp/ce1894490512b913cc07fa2fb9fc006a/5CE27941/t51.2885-19/s150x150/45428020_1448496848586347_8199584612402331648_n.jpg?_nc_ht=instagram.fgyd4-2.fna.fbcdn.net"" 
																         style=""height:auto;line-height:100%;outline:none;text-decoration:none;display:inline;width:50px;border-radius:50%;border:0"">
																         </a>
                                                                        <a  rel=""contact"" href=""https://twitter.com/work_az""  align=""center"" 
															             style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none"" target=""_blank"" >
																         <img  width=""30"" height=""30"" src=""https://instagram.fgyd4-2.fna.fbcdn.net/vp/ce1894490512b913cc07fa2fb9fc006a/5CE27941/t51.2885-19/s150x150/45428020_1448496848586347_8199584612402331648_n.jpg?_nc_ht=instagram.fgyd4-2.fna.fbcdn.net"" 
																         style=""height:auto;line-height:100%;outline:none;text-decoration:none;display:inline;width:50px;border-radius:50%;border:0"">
																         </a>
                                                                        </p>
                                                                        <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:12px;line-height:1.5;color:#555"">"
                                                                                + Lang.EmailBodyFooterTitle2 + @"<br>
												                        </p>
											                        </td>
										                        </tr>
									                        </tbody>
								                        </table>
							                        </td>
						                        </tr>
					                        </tbody>
				                        </table>
                                 </div>
                            </div>
                        </div>";
        }
        public string GenerateResetAlreadyMailBody(User model)
        {
            return @"<div><div>
                <table  cellpadding=""0"" cellspacing=""0"" border=""0"" height=""100%"" width=""100%"" style=""border-spacing:0;border-collapse:collapse;margin:0 auto"" bgcolor=""#f0f0f0"">
                  <tbody><tr>
                    <td valign=""top"">
                      <center style=""width:100%"">
                        <div  style=""max-width:600px;margin:auto"">
                           <br/>
		                        <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" width=""100%"" style=""border-spacing:0;border-collapse:collapse;max-width:600px;margin:0 auto"" bgcolor=""#ffffff"">
			                        <tbody>
				                        <tr>
					                        <td>
						                        <table border=""0"" cellpadding=""30"" cellspacing=""0"" width=""100%"" style=""border-spacing:0;border-collapse:collapse;margin:0 auto"">
							                        <tbody>
								                        <tr>
									                        <td valign=""top""  style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#444;font-size:14px;line-height:150%"">
										                        <div style=""margin-bottom:20px;padding-bottom:25px;border-bottom-width:1px;border-bottom-color:#eee;border-bottom-style:solid"">
											                        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border-spacing:0;border-collapse:collapse;margin:0 auto"">
												                        <tbody>
													                        <tr>
														                          <td>
															                            <P  
                                                                                        style=""float:left; font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#444;display:block;font-size:15px;font-weight:500;line-height:1.3;margin:0"">"
                                                                                         + Lang.EmailConfirmationMailBodyHi1 + " " + model.Name + " " + model.Surname + "," +
                                                                                        @"</P> 
														                          </td>
														                          <td>
																                    <a  rel=""contact"" href=""" + _configuration["Site:Link"].ToString() + @""" 
															                                                style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none""                                                    target=""_blank"" >
																                         <img  width=""48"" height=""48"" src=""" + _configuration["Site:Link"].ToString() + @"/img/logo.jpeg"" 
																                                        style=""height:auto;line-height:100%;outline:none;text-decoration:none;display:inline;width:130px;border-radius:50%;border:0"">
                                                                                    </a>
														                          </td>
														                        </tr>
															                   <tr>    
															                        <td>  
                                                                                        <p  style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#444;display:block;font-size:15px;font-weight:500;line-height:1.3;margin:0"">"
                                                                                          + Lang.EmailRegisteredMailBodyText1 +
                                                                                      @"</p> <br /> 
														                            </td>
															                    </tr> 
                                                                                 <tr>   
                                                                                     <td>  
                                                                                       <h1  style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#444;display:block;font-size:15px;font-weight:500;line-height:1.3;margin:0"">"
                                                                                          + Lang.EmailRegisteredMailBodyText2 +
                                                                                     @"</h1> <br /> <br />
														                             </td>
														                      </tr>
													                        </tbody>
												                        </table>
											                        </div>

                                                                      <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">"
                                                                            + "<b>" + Lang.EmailRegisteredMailBodyText3 + @" </b> <br />
												                      </p>
                                                                      <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">"
                                                                            + Lang.EmailRegisteredMailBodyText4 + @" <br />
												                      </p>
                                                                      <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">
                                                                            <img  width=""80"" height=""80"" src=""" + _configuration["Site:Link"].ToString() + @"/img/discount/discount10.png"" 
																                              style=""height:auto;line-height:100%;outline:none;text-decoration:none;display:inline;width:130px;border-radius:50%;border:0"" />" +
                                                                            Lang.EmailRegisteredMailDiscountText +
                                                                      @"</p>
                                                                      <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">"
                                                                             + "<b>" + Lang.EmailRegisteredMailBodyText5 + "</b> " + "code" + @"<br />
												                      </p>
                                                                      <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">"
                                                                             + Lang.EmailRegisteredMailBodyText6 + " " + DateTime.Today.AddMonths(1).ToString("yyy-MM-dd") + @" <br /> <br />
												                      </p>  

                                                                      <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">
                                                                         <a  href=""" + _configuration["Site:Link"].ToString() + "/Home/Index" + @"""  
															               style=""width: 30%;font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none;background-color:#95a844;font-size:13px;line-height:1.1;text-align:center;display:block;border-radius:4px;border:15px solid #95a844"" target=""_blank"" >
                                                                           &nbsp;&nbsp;&nbsp;&nbsp;
																           <span style=""color:#ffffff"">
                                                                            " + Lang.EmailRegisteredMailBodyButtonText7 + @"
                                                                            </span>
                                                                        </a>
												                      </p> <br /> <br />

                                                                      <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">"
                                                                            + Lang.EmailBodyFooterTitle1 + @"<br />
												                      </p>
                                                                      <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:15px;line-height:1.5;color:#555"">
                                                                      <a  rel=""contact"" href=""https://www.facebook.com/work.az/"" align=""center"" 
															             style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none"" target=""_blank"" >
																         <img  width=""30"" height=""30"" src=""https://instagram.fgyd4-2.fna.fbcdn.net/vp/ce1894490512b913cc07fa2fb9fc006a/5CE27941/t51.2885-19/s150x150/45428020_1448496848586347_8199584612402331648_n.jpg?_nc_ht=instagram.fgyd4-2.fna.fbcdn.net"" 
																         style=""height:auto;line-height:100%;outline:none;text-decoration:none;display:inline;width:50px;border-radius:50%;border:0"">
                                                                        </a>
                                                                       <a  rel=""contact"" href=""https://www.linkedin.com/company/work-az/about/""  align=""center"" 
															             style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none"" target=""_blank"" >
																         <img  width=""30"" height=""30"" src=""https://instagram.fgyd4-2.fna.fbcdn.net/vp/ce1894490512b913cc07fa2fb9fc006a/5CE27941/t51.2885-19/s150x150/45428020_1448496848586347_8199584612402331648_n.jpg?_nc_ht=instagram.fgyd4-2.fna.fbcdn.net"" 
																         style=""height:auto;line-height:100%;outline:none;text-decoration:none;display:inline;width:50px;border-radius:50%;border:0"">
																         </a>
                                                                       <a  rel=""contact"" href=""https://www.instagram.com/cocacola/"" align=""center"" 
                                                                        style =""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none"" target=""_blank"" >
																         <img  width=""30"" height=""30"" src=""https://instagram.fgyd4-2.fna.fbcdn.net/vp/ce1894490512b913cc07fa2fb9fc006a/5CE27941/t51.2885-19/s150x150/45428020_1448496848586347_8199584612402331648_n.jpg?_nc_ht=instagram.fgyd4-2.fna.fbcdn.net"" 
																         style=""height:auto;line-height:100%;outline:none;text-decoration:none;display:inline;width:50px;border-radius:50%;border:0"">
																         </a>
                                                                        <a  rel=""contact"" href=""https://twitter.com/work_az""  align=""center"" 
															             style=""font-family:&quot;Helvetica Neue&quot;,sans-serif;color:#3a8bbb;font-weight:500;text-decoration:none"" target=""_blank"" >
																         <img  width=""30"" height=""30"" src=""https://instagram.fgyd4-2.fna.fbcdn.net/vp/ce1894490512b913cc07fa2fb9fc006a/5CE27941/t51.2885-19/s150x150/45428020_1448496848586347_8199584612402331648_n.jpg?_nc_ht=instagram.fgyd4-2.fna.fbcdn.net"" 
																         style=""height:auto;line-height:100%;outline:none;text-decoration:none;display:inline;width:50px;border-radius:50%;border:0"">
																         </a>
                                                                        </p>
                                                                        <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:13px;line-height:1.5;color:#555"">"
                                                                                + Lang.EmailBodyFooterTitle2 + @"<br />
												                        </p>
                                                                        <p  align=""center"" style=""margin-top:15px;margin-bottom:15px;font-size:12px;line-height:1.5;color:#555"">"
                                                                            + Lang.EmailRegisteredMailBodyText11 + @"<br />
												                      </p>
											                        </td>
										                        </tr>
									                        </tbody>
								                        </table>
							                        </td>
						                        </tr>
					                        </tbody>
				                        </table>
                                 </div>
                            </div>
                        </div>";
        }

        public void SendMailTest(string subject, string body)
        {
            Console.WriteLine("SendMail started");

            string fromMailAddress = "Ceyhun.Rahimov@kapitalbank.az";
            string fromMailPassword = "Monster44";
            string fromMailName = "mamed";

            var networkConfig = new NetworkCredential(fromMailAddress, fromMailPassword);
            var mailServer = new SmtpClient()
            {
                Host = "192.168.11.35",
                UseDefaultCredentials = false,
                Credentials = networkConfig
            };

            mailServer.Port = 25;

            var message = new MailMessage()
            {
                Subject = subject,
                SubjectEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
            };

            //message send config
            message.To.Add(new MailAddress("Ceyhun.Rahimov@kapitalbank.az"));
            //message.To.Add(new MailAddress("r.ceyhun2011@gmail.com"));
            message.From = new MailAddress(fromMailAddress, fromMailName);
            message.Body = body;

            try
            {
                mailServer.SendAsync(message, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }

            Console.WriteLine("SendMail end");
        }
    }
}
