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
    public class VisionsController : BaseController
    {
        private readonly IOptionService _optionService;

        public VisionsController(
                               ILogger<VisionsController> logger,
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

            var response = _entityService.LoadEntitiesByCriteria<Vision>(new BaseCriteriaModel { PageNumber = pageNumber, RowsPerPage = rowsPerPage });
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Visions-List result.IsSuccessfull");
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
        public IActionResult Create(Vision model)
        {
            var response = _entityService.Create<Vision>(model, "", "", false);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Visions Create post result.IsSuccessfull");
                return RedirectToAction("List", "Visions");
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
            var response = _entityService.GetEntityById<Vision>(id);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Visions-Edit get result.IsSuccessfull");
                return View(response.Model);
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("List", "Visions");
        }

        [HttpPost]
        public IActionResult Edit(Vision model)
        {
            var response = _entityService.UpdateByAll<Vision>(model, "Id", model.Id, false, "", "");
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Visions Edit post result.IsSuccessfull");
                return RedirectToAction("List", "Visions");
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
            var response = _entityService.DeleteById<Vision>(id);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Visions Delete result.IsSuccessfull");
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("List", "Visions");
        }
    }
}
