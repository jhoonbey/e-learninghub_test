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
using System.Collections.Generic;
using System.Reflection;

namespace app.web.Areas.Addmein.Controllers
{
    [User(AllowedRole = EnumUserRole.Admin)]
    [Area("Addmein")]
    public class OptionController : BaseController
    {
        private readonly IOptionService _optionService;

        public OptionController(
                               ILogger<OptionController> logger,
                               IOptionService optionService,
                               IEntityService entityService,
                               IConfiguration configuration,
                               ICipherService cipherService,
                               IHostingEnvironment hostingEnvironment
                              ) : base(logger, configuration, hostingEnvironment, entityService, cipherService)
        {
            _optionService = optionService;
        }

        public IActionResult ViewOption(string sec)
        {
            var response = _optionService.GetOrCreate(sec);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("ViewOption result.IsSuccessfull");
                return View(response.Model);
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult ManageOption(string sec)
        {
            var response = _entityService.GetEntityBy<Option>(new Dictionary<string, object> { { "Sec", sec } });
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("ManageOption get result.IsSuccessfull");
                return View(response.Model);
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("ViewOption", "Option", new { sec });
        }

        [HttpPost]
        public IActionResult ManageOption(Option model)
        {
            if (model == null || string.IsNullOrEmpty(model.Sec))
                return RedirectToAction("Index", "Dashboard");

            Dictionary<string, object> columns = new Dictionary<string, object>{
                                                                                    { "NameAZ", model.NameAZ },
                                                                                    { "NameEN", model.NameEN },
                                                                                    { "NameRU", model.NameRU },
                                                                                 };
            var response = _entityService.UpdateBy<Option>(columns, "Sec", model.Sec);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("ManageOption post result.IsSuccessfull");
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("ViewOption", "Option", new { model.Sec });
        }
    }
}
