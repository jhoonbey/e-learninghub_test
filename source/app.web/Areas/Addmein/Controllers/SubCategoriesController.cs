using app.domain.Enums;
using app.domain.Model.Criterias;
using app.domain.Model.Entities;
using app.domain.Model.View;
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
using System.Reflection;

namespace app.web.Areas.Addmein.Controllers
{
    [User(AllowedRole = EnumUserRole.Admin)]
    [Area("Addmein")]
    public class SubCategoriesController : BaseController
    {
        //private readonly IOptionService _optionService;

        public SubCategoriesController(
                               ILogger<SubCategoriesController> logger,
                               //IOptionService optionService,
                               IEntityService entityService,
                               IConfiguration configuration,
                               ICipherService cipherService,
                               IHostingEnvironment hostingEnvironment
                              ) : base(logger, configuration, hostingEnvironment, entityService, cipherService)
        {
            //_optionService = optionService;
        }

        public IActionResult List()
        {
            var criteria = new BaseCriteriaModel
            {
                PageNumber = 1,
                RowsPerPage = 200,
                LeftJoinModels = new List<LeftJoinModel>
                {
                     new LeftJoinModel
                     {
                          Alias = "C",
                          HelperEntityName = typeof(Category).Name,
                          JoinHelperColumn = "Id",
                          JoinMainColumn = "CategoryId",
                          TakeHelperColumn = GetNameByLangugage(),
                          AsResultColumn = "CategoryName"
                     }
                }
            };

            var response = _entityService.LoadEntityViewModelsByCriteria<SubCategoryViewModel, SubCategory>(criteria);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("SubCategories List -  result.IsSuccessfull");
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
            var responseCategory = _entityService.LoadEntitiesByCriteria<Category>(new BaseCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
            if (responseCategory.IsSuccessfull)
            {
                ViewBag.Categories = new SelectList(responseCategory.Model.Items, "Id", GetNameByLangugage());
            }

            return View();
        }

        [HttpPost]
        public IActionResult Create(SubCategory model)
        {
            var response = _entityService.Create<SubCategory>(model, "", "", false);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("SubCategories Create post result.IsSuccessfull");
                return RedirectToAction("List", "SubCategories");
            }
            else
            {
                var responseCategory = _entityService.LoadEntitiesByCriteria<Category>(new BaseCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
                if (responseCategory.IsSuccessfull)
                {
                    ViewBag.Categories = new SelectList(responseCategory.Model.Items, "Id", GetNameByLangugage(), model.CategoryId);
                }

                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForLog);
                return View(model);
            }
        }

        public IActionResult Edit(int id)
        {
            var response = _entityService.GetEntityById<SubCategory>(id);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("SubCategories-Edit get result.IsSuccessfull");

                var responseCategory = _entityService.LoadEntitiesByCriteria<Category>(new BaseCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
                if (responseCategory.IsSuccessfull)
                {
                    ViewBag.Categories = new SelectList(responseCategory.Model.Items, "Id", GetNameByLangugage(), response.Model.CategoryId);
                }

                return View(response.Model);
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("List", "SubCategories");
        }

        [HttpPost]
        public IActionResult Edit(SubCategory model)
        {
            var response = _entityService.UpdateByAll<SubCategory>(model, "Id", model.Id, false, "", "");
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("SubCategories Edit post result.IsSuccessfull");
                return RedirectToAction("List", "SubCategories");
            }
            else
            {
                var responseCategory = _entityService.LoadEntitiesByCriteria<Category>(new BaseCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
                if (responseCategory.IsSuccessfull)
                {
                    ViewBag.Categories = new SelectList(responseCategory.Model.Items, "Id", GetNameByLangugage(), model.CategoryId);
                }

                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForLog);
                return View(model);
            }
        }

        public IActionResult Delete(int id)
        {
            try
            {
                //delete entity
                var response = _entityService.DeleteById<SubCategory>(id);
                if (response.IsSuccessfull)
                {
                    _logger.LogInformation("SubCategories Delete result.IsSuccessfull");
                }
                else
                {
                    _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                    TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, ex.ToString()));
            }

            return RedirectToAction("List", "SubCategories");
        }
    }
}
