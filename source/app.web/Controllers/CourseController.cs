using app.domain.Enums;
using app.domain.Model.Criterias;
using app.domain.Model.Entities;
using app.domain.Model.View;
using app.domain.Utilities;
using app.service;
using app.web.Core;
using app.web.Extensions;
using app.web.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace app.web.Controllers
{
    [EnableCors("CorsPolicy")]
    public class CourseController : BaseController
    {
        private readonly ICourseService _courseService;
        private readonly IAntiForgeryCookieService _antiforgery;

        public CourseController(ILogger<CourseController> logger,
                                ICourseService courseService,
                                IAntiForgeryCookieService antiforgery,
                                ICipherService cipherService,
                                IEntityService entityService,
                                IConfiguration configuration,
                                IHostingEnvironment hostingEnvironment
                                ) : base(logger, configuration, hostingEnvironment, entityService, cipherService)
        {
            _courseService = courseService;
            _antiforgery = antiforgery;
        }

        [User(AllowedRole = EnumUserRole.Learner)]
        public IActionResult View(int id)
        {
            var response = _courseService.GetViewModel(id);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation($"{ MethodBase.GetCurrentMethod().Name } - GetViewModel result.IsSuccessfull");

                if (response.Model.Course.UserId != CurrentUser.Id)
                {
                    if (response.Model.Course.Status != (int)EnumCourseStatus.Approved)
                    {
                        _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - response.Model.Course.Status != (int)EnumCourseStatus.Approved "}");
                        TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, "Video not found"));
                        return RedirectToAction("Courses", "Profile");
                    }
                }

                return View(response.Model);
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForClient));
            }

            return RedirectToAction("Courses", "Profile");
        }

        [User(AllowedRole = EnumUserRole.Instructor)]
        public IActionResult Create()
        {
            List<Category> filteredCategories = new List<Category>();
            var responseCategory = _entityService.LoadEntitiesByCriteria<Category>(new BaseCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
            if (responseCategory.IsSuccessfull)
            {
                var idSet = Common.ConvertStringToIdSet(CurrentUser.CategoryIdSet);
                filteredCategories = responseCategory.Model.Items.Where(item => idSet.Contains(item.Id)).ToList();
                ViewBag.Categories = new SelectList(filteredCategories, "Id", GetNameByLangugage());
            }

            var responseSubCategory = _entityService.LoadEntitiesByCriteria<SubCategory>(new BaseCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
            if (responseSubCategory.IsSuccessfull)
            {
                var idSet = filteredCategories.Select(e => e.Id).ToList();
                var filteredSubCategories = responseSubCategory.Model.Items.Where(item => idSet.Contains(item.CategoryId)).ToList();
                ViewBag.SubCategories = new SelectList(filteredSubCategories, "Id", GetNameByLangugage());
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [User(AllowedRole = EnumUserRole.Instructor)]
        public IActionResult Create(CourseCreateViewModel model)
        {
            var response = _courseService.Create(model, CurrentUser);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation($"{ MethodBase.GetCurrentMethod().Name } - _courseService.Create result.IsSuccessfull");

                return RedirectToAction("View", "Course", new { id = response.Model.Id });
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForClient));

                List<Category> filteredCategories = new List<Category>();
                var responseCategory = _entityService.LoadEntitiesByCriteria<Category>(new BaseCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
                if (responseCategory.IsSuccessfull)
                {
                    var idSet = Common.ConvertStringToIdSet(CurrentUser.CategoryIdSet);
                    filteredCategories = responseCategory.Model.Items.Where(item => idSet.Contains(item.Id)).ToList();
                    ViewBag.Categories = new SelectList(filteredCategories, "Id", GetNameByLangugage(), model.CategoryId);
                }
                var responseSubCategory = _entityService.LoadEntitiesByCriteria<SubCategory>(new BaseCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
                if (responseSubCategory.IsSuccessfull)
                {
                    var idSet = filteredCategories.Select(e => e.Id).ToList();
                    var filteredSubCategories = responseSubCategory.Model.Items.Where(item => idSet.Contains(item.CategoryId)).ToList();
                    ViewBag.SubCategories = new SelectList(filteredSubCategories, "Id", GetNameByLangugage(), model.SubCategoryId);
                }

                return View(model);
            }
        }

        [User(AllowedRole = EnumUserRole.Instructor)]
        public IActionResult Edit(int id)
        {
            var response = _entityService.GetEntityById<Course>(id);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation($"{ MethodBase.GetCurrentMethod().Name } - GetEntityById get result.IsSuccessfull");

                //see only your course
                if (response.Model.UserId != Id)
                {
                    return RedirectToAction("Error", "Home");
                }

                List<Category> filteredCategories = new List<Category>();
                var responseCategory = _entityService.LoadEntitiesByCriteria<Category>(new BaseCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
                if (responseCategory.IsSuccessfull)
                {
                    var idSet = Common.ConvertStringToIdSet(CurrentUser.CategoryIdSet);
                    filteredCategories = responseCategory.Model.Items.Where(item => idSet.Contains(item.Id)).ToList();
                    ViewBag.Categories = new SelectList(filteredCategories, "Id", GetNameByLangugage(), response.Model.CategoryId);
                }
                var responseSubCategory = _entityService.LoadEntitiesByCriteria<SubCategory>(new BaseCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
                if (responseSubCategory.IsSuccessfull)
                {
                    var idSet = filteredCategories.Select(e => e.Id).ToList();
                    var filteredSubCategories = responseSubCategory.Model.Items.Where(item => idSet.Contains(item.CategoryId)).ToList();
                    ViewBag.SubCategories = new SelectList(filteredSubCategories, "Id", GetNameByLangugage(), response.Model.SubCategoryId);
                }

                return View(response.Model);
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
                return RedirectToAction("Courses", "Profile");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [User(AllowedRole = EnumUserRole.Instructor)]
        public IActionResult Edit(Course model)
        {
            var responseEntity = _entityService.GetEntityById<Course>(model.Id);
            if (responseEntity.IsSuccessfull)
            {
                //see only your course
                if (responseEntity.Model.UserId != Id || responseEntity.Model.Status != (int)EnumCourseStatus.Draft)
                {
                    return View("Error");
                }
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + responseEntity.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, responseEntity.ErrorForLog));
                return RedirectToAction("Courses", "Profile");
            }

            var response = _entityService.UpdateBy<Course>(new Dictionary<string, object>
            {
                { "Name", model.Name },
                { "Description", model.Description },
                { "WhatObjectives", model.WhatObjectives },
                { "WhatSkills", model.WhatSkills },
                { "WhoShouldTake", model.WhoShouldTake },
                { "CategoryId", model.CategoryId },
                { "SubCategoryId", model.SubCategoryId }
            }, "Id", responseEntity.Model.Id);


            if (response.IsSuccessfull)
            {
                _logger.LogInformation($"{ MethodBase.GetCurrentMethod().Name } - _courseService.UpdateBy result.IsSuccessfull");
                return RedirectToAction("Courses", "Profile");
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForClient));

                List<Category> filteredCategories = new List<Category>();
                var responseCategory = _entityService.LoadEntitiesByCriteria<Category>(new BaseCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
                if (responseCategory.IsSuccessfull)
                {
                    var idSet = Common.ConvertStringToIdSet(CurrentUser.CategoryIdSet);
                    filteredCategories = responseCategory.Model.Items.Where(item => idSet.Contains(item.Id)).ToList();
                    ViewBag.Categories = new SelectList(filteredCategories, "Id", GetNameByLangugage(), model.CategoryId);
                }
                var responseSubCategory = _entityService.LoadEntitiesByCriteria<SubCategory>(new BaseCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
                if (responseSubCategory.IsSuccessfull)
                {
                    var idSet = filteredCategories.Select(e => e.Id).ToList();
                    var filteredSubCategories = responseSubCategory.Model.Items.Where(item => idSet.Contains(item.CategoryId)).ToList();
                    ViewBag.SubCategories = new SelectList(filteredSubCategories, "Id", GetNameByLangugage(), model.SubCategoryId);
                }

                return View(model);
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [User(AllowedRole = EnumUserRole.Instructor)]
        public IActionResult SendApproval(int id)
        {
            var response = _courseService.SendApproval(id, CurrentUser);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation($"{ MethodBase.GetCurrentMethod().Name } - _courseService.SendApproval result.IsSuccessfull");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Success, "Your course sent to Administrator successfully"));
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForClient));
            }

            return RedirectToAction("View", "Course", new { id });
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

    }
}