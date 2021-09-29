using app.domain.Enums;
using app.domain.Model.Criterias;
using app.domain.Model.Entities;
using app.service;
using app.web.Core;
using app.web.Extensions;
using app.web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace app.web.Areas.Addmein.Controllers
{
    [User(AllowedRole = EnumUserRole.Admin)]
    [Area("Addmein")]
    public class FeaturesController : BaseController
    {
        private readonly IOptionService _optionService;

        public FeaturesController(
                               ILogger<FeaturesController> logger,
                               IOptionService optionService,
                               IEntityService entityService,
                               IConfiguration configuration,
                               ICipherService cipherService,
                               IHostingEnvironment hostingEnvironment
                              ) : base(logger, configuration, hostingEnvironment, entityService, cipherService)
        {
            _optionService = optionService;
        }


        public IActionResult List()
        {
            int pageNumber = 1;
            int rowsPerPage = 50;

            var response = _entityService.LoadEntitiesByCriteria<Feature>(new BaseCriteriaModel { PageNumber = pageNumber, RowsPerPage = rowsPerPage });
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Features-List result.IsSuccessfull");
                return View(response.Model);
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Feature model)
        {
            var response = _entityService.Create<Feature>(model, "", "", false);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Features Create post result.IsSuccessfull");
                return RedirectToAction("List", "Features");
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForLog);
                return View(model);
            }
        }


        public IActionResult Edit(int id)
        {
            var response = _entityService.GetEntityById<Feature>(id);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Features-Edit get result.IsSuccessfull");
                return View(response.Model);
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("List", "Features");
        }

        [HttpPost]
        public IActionResult Edit(Feature model)
        {
            var response = _entityService.UpdateByAll<Feature>(model, "Id", model.Id, false, "", "");
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Features Edit post result.IsSuccessfull");
                return RedirectToAction("List", "Features");
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
            var response = _entityService.DeleteById<Feature>(id);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Features Delete result.IsSuccessfull");
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("List", "Features");
        }
    }
}
