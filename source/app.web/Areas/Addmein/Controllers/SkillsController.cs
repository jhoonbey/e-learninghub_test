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
    public class SkillsController : BaseController
    {
        private readonly IOptionService _optionService;

        public SkillsController(
                               ILogger<SkillsController> logger,
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

            var response = _entityService.LoadEntitiesByCriteria<Skill>(new BaseCriteriaModel { PageNumber = pageNumber, RowsPerPage = rowsPerPage });
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Skills-List result.IsSuccessfull");
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
        public IActionResult Create(Skill model)
        {
            var response = _entityService.Create<Skill>(model, "", "", false);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Skills Create post result.IsSuccessfull");
                return RedirectToAction("List", "Skills");
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
            var response = _entityService.GetEntityById<Skill>(id);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Skills-Edit get result.IsSuccessfull");
                return View(response.Model);
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("List", "Skills");
        }

        [HttpPost]
        public IActionResult Edit(Skill model)
        {
            var response = _entityService.UpdateByAll<Skill>(model, "Id", model.Id, false, "", "");
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Skills Edit post result.IsSuccessfull");
                return RedirectToAction("List", "Skills");
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
            var response = _entityService.DeleteById<Skill>(id);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Skills Delete result.IsSuccessfull");
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("List", "Skills");
        }
    }
}
