using app.domain.Enums;
using app.domain.Model.Entities;
using app.service;
using app.web.Core;
using app.web.Extensions;
using app.web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace app.web.client.Areas.Addmein.Controllers
{
    [User(AllowedRole = EnumUserRole.Admin)]
    public class AboutController : BaseController
    {
        private readonly ILogger<AboutController> _logger;
        private readonly IOptionService _optionService;

        public AboutController(
                               ILogger<AboutController> logger,
                               IOptionService optionService,
                               IEntityService entityService,
                               IConfiguration configuration,
                               IHostingEnvironment hostingEnvironment
                              ) : base(configuration, hostingEnvironment, entityService)
        {
            _logger = logger;
            _optionService = optionService;
        }

        public ActionResult ViewTitle()
        {
            var response = _optionService.GetOrCreate("AboutTitle");
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("ViewTitle result.IsSuccessfull");

                //??? display "ok" alert
            }
            else
            {
                _logger.LogError(response.ErrorForLog);
                AddError(response.Key, response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("Index", "Dashboard");
        }

        public ActionResult ManageTitle()
        {
            var response = _optionService.GetOrCreate("AboutTitle");
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("ManageTitle get result.IsSuccessfull");
                //??? display "ok" alert
            }
            else
            {
                _logger.LogError(response.ErrorForLog);
                AddError(response.Key, response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("ViewTitle", "About");
        }
        [HttpPost]
        public ActionResult ManageTitle(Option model)
        {
            if (model == null || model.Sec != "AboutTitle")
                return RedirectToAction("ViewTitle", "About");

            Dictionary<string, object> columns = new Dictionary<string, object>{
                                                                                    { "NameAZ", model.NameAZ },
                                                                                    { "NameEN", model.NameEN },
                                                                                    { "NameRU", model.NameRU },
                                                                                 };
            var response = _entityService.UpdateBy<Option>(columns, "Sec", model.Sec, false);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("ManageTitle post result.IsSuccessfull");
                //??? display "ok" alert
            }
            else
            {
                _logger.LogError(response.ErrorForLog);
                AddError(response.Key, response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("ViewTitle", "About");
        }


        public ActionResult ViewDescription()
        {
            var response = _optionService.GetOrCreate("AboutDescription");
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("AboutDescription result.IsSuccessfull");

                //??? display "ok" alert
            }
            else
            {
                _logger.LogError(response.ErrorForLog);
                AddError(response.Key, response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("Index", "Dashboard");
        }

        public ActionResult ManageDescription()
        {
            var response = _optionService.GetOrCreate("AboutDescription");
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("AboutDescription get result.IsSuccessfull");
                //??? display "ok" alert
            }
            else
            {
                _logger.LogError(response.ErrorForLog);
                AddError(response.Key, response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("ViewDescription", "About");
        }

        [HttpPost]
        public ActionResult ManageDescription(Option model)
        {
            if (model == null || model.Sec != "AboutDescription")
                return RedirectToAction("ViewDescription", "About");

            Dictionary<string, object> columns = new Dictionary<string, object>{
                                                                                    { "NameAZ", model.NameAZ },
                                                                                    { "NameEN", model.NameEN },
                                                                                    { "NameRU", model.NameRU },
                                                                                 };
            var response = _entityService.UpdateBy<Option>(columns, "Sec", model.Sec, false);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("AboutDescription post result.IsSuccessfull");
                //??? display "ok" alert
            }
            else
            {
                _logger.LogError(response.ErrorForLog);
                AddError(response.Key, response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("ViewDescription", "About");
        }





        public ActionResult ViewAbout()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("About");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult ManageAbout()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("About");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [HttpPost]
        public ActionResult ManageAbout(Option model)
        {
            try
            {
                if (model == null || model.Sec != "About") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewAbout", "About");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }

        public ActionResult ManageVisionTitle()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("AboutVisionTitle");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [HttpPost]
        public ActionResult ManageVisionTitle(Option model)
        {
            try
            {
                if (model == null || model.Sec != "AboutVisionTitle") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewVisionTitle", "About");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }

        public ActionResult ViewVisionTitle()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("AboutVisionTitle");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult ManageVisionDescription()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("AboutVisionDescription");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [HttpPost]
        public ActionResult ManageVisionDescription(Option model)
        {
            try
            {
                if (model == null || model.Sec != "AboutVisionDescription") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewVisionDescription", "About");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }

        public ActionResult ViewVisionDescription()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("AboutVisionDescription");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult ManageMissionTitle()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("AboutMissionTitle");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [HttpPost]
        public ActionResult ManageMissionTitle(Option model)
        {
            try
            {
                if (model == null || model.Sec != "AboutMissionTitle") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewMissionTitle", "About");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }

        public ActionResult ViewMissionTitle()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("AboutMissionTitle");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult ManageMissionDescription()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("AboutMissionDescription");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [HttpPost]
        public ActionResult ManageMissionDescription(Option model)
        {
            try
            {
                if (model == null || model.Sec != "AboutMissionDescription") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewMissionDescription", "About");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }

        public ActionResult ViewMissionDescription()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("AboutMissionDescription");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

    }
}
