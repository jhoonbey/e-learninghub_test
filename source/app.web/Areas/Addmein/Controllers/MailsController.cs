using app.domain.Enums;
using app.domain.Model.Criterias;
using app.domain.Model.Entities;
using app.domain.Model.View;
using app.domain.Utilities;
using app.service;
using app.web.Core;
using app.web.Extensions;
using app.web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace app.web.Areas.Addmein.Controllers
{
    [User(AllowedRole = EnumUserRole.Admin)]
    [Area("Addmein")]
    public class MailsController : BaseController
    {
        private readonly IOptionService _optionService;
        private readonly IEmailService _emailService;

        public MailsController(
                               ILogger<MailsController> logger,
                               IOptionService optionService,
                               IEmailService emailService,
                               IEntityService entityService,
                               IConfiguration configuration,
                               ICipherService cipherService,
                               IHostingEnvironment hostingEnvironment
                              ) : base(logger, configuration, hostingEnvironment, entityService, cipherService)
        {
            _optionService = optionService;
            _emailService = emailService;
        }


        public IActionResult List(int pageNumber = 1)
        {
            int rowsPerPage = 20;

            var response = _entityService.LoadEntitiesByCriteria<Mail>(new BaseCriteriaModel { PageNumber = pageNumber, RowsPerPage = rowsPerPage, WillCount = true });
            if (response.IsSuccessfull)
            {
                ViewBag.PageNumber = pageNumber;
                ViewBag.RowsPerPage = rowsPerPage;
                ViewBag.NumberOfPages = GetPageNumber(response.Model.AllCount, rowsPerPage);

                _logger.LogInformation("Mails-List result.IsSuccessfull");
                return View(response.Model);
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public IActionResult SendBulkEmail()
        {
            var response = _entityService.ConvertEnumToEnumModelList<EnumUserRole>(new List<int> { (int)EnumUserRole.Admin, (int)EnumUserRole.Superadmin });
            if (response.IsSuccessfull)
            {
                ViewBag.To = new SelectList(response.Model, "Id", "Name");
            }

            return View();
        }

        [HttpPost]
        public IActionResult SendBulkEmail(MailSendModel model)
        {
            try
            {
                //validation
                if (model == null) throw new Exception("Model is null");
                if (string.IsNullOrEmpty(model.Subject)) throw new Exception("Subject is empty");
                if (string.IsNullOrEmpty(model.Body)) throw new Exception("Body is empty");
                string stripped = Common.StripHTML2(model.Body);
                if (string.IsNullOrEmpty(stripped)) throw new Exception("Body is empty");

                //purpose
                int purpose = 0;
                EnumUserRole role = (EnumUserRole)model.To;
                if (role == EnumUserRole.Learner)
                {
                    purpose = (int)EnumEmailPurpose.BulkToLearners;
                }
                else
                if (role == EnumUserRole.Instructor)
                {
                    purpose = (int)EnumEmailPurpose.BulkToInstructors;
                }
                else
                {
                    throw new Exception("Select users who you want to send email to");
                }

                //get users
                var response = _entityService.LoadEntitiesByCriteria<User>(new BaseCriteriaModel { IntCriteria = model.To, PageNumber = 1, RowsPerPage = 10000000 });
                if (!response.IsSuccessfull)
                {
                    throw new Exception(response.ErrorForLog);
                }
                if (response.Model.Items.Count < 1)
                {
                    throw new Exception("No any user in this type on the system");
                }

                //fill mail list
                List<Mail> list = new List<Mail>();
                foreach (var item in response.Model.Items.Where(e => e.IsEmailConfirmed))
                {
                    Mail mail = new Mail
                    {
                        FromMail = _configuration["MailConfig:FromMailNoreply"].ToString(),
                        FromDisplayName = _configuration["MailConfig:FromDisplayName"].ToString(),
                        ToMail = item.Email,
                        Body = model.Body,
                        Subject = model.Subject,
                        Purpose = purpose
                    };

                    var response2 = _entityService.Create<Mail>(mail, "", "", false);
                    if (response2.IsSuccessfull) // mail save is ok
                    {
                        mail.Id = response2.Model;
                        list.Add(mail);
                    }

                    Thread.Sleep(100);
                }

                //send email
                _emailService.SendAysnc(list, 500, false);

                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Success, "Email sending is in progress, to check mails sent status, refresh Mails -> Mails list page"));
                return RedirectToAction("List", "Mails");
            }
            catch (Exception ex)
            {
                var response = _entityService.ConvertEnumToEnumModelList<EnumUserRole>(new List<int> { (int)EnumUserRole.Admin, (int)EnumUserRole.Superadmin });
                if (response.IsSuccessfull)
                {
                    ViewBag.To = new SelectList(response.Model, "Id", "Name");
                }

                _logger.LogError(ex.ToString());
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, ex.Message));
                return View(model);
            }
        }

        public IActionResult Edit(int id)
        {
            var response = _entityService.GetEntityById<Mail>(id);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Mails-Edit get result.IsSuccessfull");
                return View(response.Model);
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("List", "Mails");
        }

        [HttpPost]
        public IActionResult Edit(Mail model)
        {
            var response = _entityService.UpdateByAll<Mail>(model, "Id", model.Id, false, "", "");
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Mails Edit post result.IsSuccessfull");
                return RedirectToAction("List", "Mails");
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForLog);
                return View(model);
            }
        }

        public IActionResult Delete(int id)
        {
            var response = _entityService.DeleteById<Mail>(id);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Mails Delete result.IsSuccessfull");
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("List", "Mails");
        }
    }
}
