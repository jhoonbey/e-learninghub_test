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
    public class SubOfSubCategoriesController : BaseController
    {
        //private readonly IOptionService _optionService;

        public SubOfSubCategoriesController(
                               ILogger<SubOfSubCategoriesController> logger,
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
                     },
                    new LeftJoinModel
                     {
                          Alias = "C2",
                          HelperEntityName = typeof(SubCategory).Name,
                          JoinHelperColumn = "Id",
                          JoinMainColumn = "SubCategoryId",
                          TakeHelperColumn = GetNameByLangugage(),
                          AsResultColumn = "SubCategoryName"
                     }
                }
            };


            var response = _entityService.LoadEntityViewModelsByCriteria<SubOfSubCategoryViewModel, SubOfSubCategory>(criteria);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("SubOfSubCategories List -  result.IsSuccessfull");
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

            var responseSubCategory = _entityService.LoadEntitiesByCriteria<SubCategory>(new BaseCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
            if (responseSubCategory.IsSuccessfull)
            {
                ViewBag.SubCategories = new SelectList(responseSubCategory.Model.Items, "Id", GetNameByLangugage());
            }

            return View();
        }

        [HttpPost]
        public IActionResult Create(SubOfSubCategory model)
        {
            var response = _entityService.Create<SubOfSubCategory>(model, "", "", false);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("SubOfSubCategories Create post result.IsSuccessfull");
                return RedirectToAction("List", "SubOfSubCategories");
            }
            else
            {
                var responseCategory = _entityService.LoadEntitiesByCriteria<Category>(new BaseCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
                if (responseCategory.IsSuccessfull)
                {
                    ViewBag.Categories = new SelectList(responseCategory.Model.Items, "Id", GetNameByLangugage(), model.CategoryId);
                }

                var responseSubCategory = _entityService.LoadEntitiesByCriteria<SubCategory>(new BaseCriteriaModel { IntCriteria = model.CategoryId, RowsPerPage = 50, PageNumber = 1 });
                if (responseSubCategory.IsSuccessfull)
                {
                    ViewBag.SubCategories = new SelectList(responseSubCategory.Model.Items, "Id", GetNameByLangugage(), model.SubCategoryId);
                }

                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForLog);
                return View(model);
            }
        }

        public IActionResult LoadSubCategoriesByCategoryId(string parentId)
        {
            if (string.IsNullOrEmpty(parentId))
                return Json(new List<SubCategory>());

            var response = _entityService.LoadEntitiesByCriteria<SubCategory>(new BaseCriteriaModel { IntCriteria = Convert.ToInt32(parentId), PageNumber = 1, RowsPerPage = 100 });
            if (!response.IsSuccessfull)
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForLog);
                return Json(new List<SubCategory>());
            }

            return Json(response.Model.Items);
        }


        public IActionResult Edit(int id)
        {
            var response = _entityService.GetEntityById<SubOfSubCategory>(id);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("SubOfSubCategories-Edit get result.IsSuccessfull");

                var responseCategory = _entityService.LoadEntitiesByCriteria<Category>(new BaseCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
                if (responseCategory.IsSuccessfull)
                {
                    ViewBag.Categories = new SelectList(responseCategory.Model.Items, "Id", GetNameByLangugage(), response.Model.CategoryId);
                }

                var responseSubCategory = _entityService.LoadEntitiesByCriteria<SubCategory>(new BaseCriteriaModel { IntCriteria = response.Model.CategoryId, RowsPerPage = 50, PageNumber = 1 });
                if (responseSubCategory.IsSuccessfull)
                {
                    ViewBag.SubCategories = new SelectList(responseSubCategory.Model.Items, "Id", GetNameByLangugage(), response.Model.SubCategoryId);
                }

                return View(response.Model);
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("List", "SubOfSubCategories");
        }

        [HttpPost]
        public IActionResult Edit(SubOfSubCategory model)
        {
            var response = _entityService.UpdateByAll<SubOfSubCategory>(model, "Id", model.Id, false, "", "");
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("SubOfSubCategories Edit post result.IsSuccessfull");
                return RedirectToAction("List", "SubOfSubCategories");
            }
            else
            {
                var responseCategory = _entityService.LoadEntitiesByCriteria<Category>(new BaseCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
                if (responseCategory.IsSuccessfull)
                {
                    ViewBag.Categories = new SelectList(responseCategory.Model.Items, "Id", GetNameByLangugage(), model.CategoryId);
                }

                var responseSubCategory = _entityService.LoadEntitiesByCriteria<SubCategory>(new BaseCriteriaModel { IntCriteria = model.CategoryId, RowsPerPage = 50, PageNumber = 1 });
                if (responseSubCategory.IsSuccessfull)
                {
                    ViewBag.SubCategories = new SelectList(responseSubCategory.Model.Items, "Id", GetNameByLangugage(), model.SubCategoryId);
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
                var response = _entityService.DeleteById<SubOfSubCategory>(id);
                if (response.IsSuccessfull)
                {
                    _logger.LogInformation("SubOfSubCategories Delete result.IsSuccessfull");
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

            return RedirectToAction("List", "SubOfSubCategories");
        }
    }
}
