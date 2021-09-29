using app.domain.Enums;
using app.domain.Languages;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace app.web.Controllers
{
    [EnableCors("CorsPolicy")]
    [User]
    public class ProfileController : BaseController
    {
        private readonly IAccountService _accountService;

        public ProfileController(
                                ILogger<HomeController> logger,
                                IEntityService entityService,
                                IAccountService accountService,
                                IConfiguration configuration,
                               ICipherService cipherService,
                                IHostingEnvironment hostingEnvironment
                                ) : base(logger, configuration, hostingEnvironment, entityService, cipherService)
        {
            _accountService = accountService;
        }

        public IActionResult Room()
        {
            return View(CurrentUser);
        }


        public IActionResult Info()
        {
            //Categories
            var responseCategory = _entityService.LoadEntitiesByCriteria<Category>(new CategoryCriteriaModel { RowsPerPage = 50, PageNumber = 1 });

            if (CurrentUser.Role != (int)EnumUserRole.Learner)
            {
                if (responseCategory.IsSuccessfull)
                {
                    ViewBag.Categories = new MultiSelectList(responseCategory.Model.Items, "Id", GetNameByLangugage(), CurrentUser.CategoryIdSet);
                }
            }

            var userData = _entityService.GetEntityBy<UserData>(new Dictionary<string, object> { { "UserId", Id } }).Model;

            //InterestedCategory
            if (responseCategory.IsSuccessfull)
            {
                ViewBag.InterestedCategories = new MultiSelectList(responseCategory.Model.Items, "Id", GetNameByLangugage(), userData != null ? userData.InterestedCategoryIdSet : string.Empty);
            }

            UserDataViewModel response = new UserDataViewModel
            {
                UserId = CurrentUser.Id,

                Name = CurrentUser.Name,
                Surname = CurrentUser.Surname,
                Position = CurrentUser.Position,
                Workplace = CurrentUser.Workplace,
                FbLink = userData?.FbLink,
                LinkedinLink = userData?.LinkedinLink,
                TwitterLink = userData?.TwitterLink,

                Info = userData?.Info,
                Address = userData?.Address,
                University = userData?.University,
                CategoryIdSet = (CurrentUser.Role != (int)EnumUserRole.Learner) ? Common.ConvertStringToIdSet(CurrentUser.CategoryIdSet) : new List<int>(),
                InterestedCategoryIdSet = userData != null ? Common.ConvertStringToIdSet(userData.InterestedCategoryIdSet) : new List<int>()
            };

            return View(response);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Info(UserDataViewModel model)
        {
            model.UserId = Id;

            var response = _accountService.UpdateProfile(model, CurrentUser);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Profile Info post result.IsSuccessfull");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Success, Lang.InformationSavedSuccessfully));

                try
                {
                    string Fullname = model.Name + " " + model.Surname;
                    SetCookie("Fullname", model.Name + " " + model.Surname);
                    ViewBag.Fullname = Fullname;
                }
                catch (Exception)
                {
                }
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForClient));
            }

            return RedirectToAction("Info", "Profile");
        }

        [HttpPost]
        public ActionResult UpdateImage(IFormFile postedFile)
        {
            var response = _accountService.UpdateImage(postedFile, CurrentUser, Path.Combine(_hostingEnvironment.WebRootPath, _configuration["Site:ImagesPath"]));
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Profile UpdateImage result.IsSuccessfull");

                //??? display "ok" alert
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForClient));
            }

            return RedirectToAction("Room");
        }

        public IActionResult DeleteImage()
        {
            var response = _accountService.DeleteImage(Id, Path.Combine(_hostingEnvironment.WebRootPath, _configuration["Site:ImagesPath"]));
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Profile UpdateImage result.IsSuccessfull");

                //??? display "ok" alert
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForClient));
            }

            return RedirectToAction("Room");
        }


        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(PasswordChangeModel model)
        {
            var response = _accountService.ChangePassword(model, Id);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Profile ChangePassword result.IsSuccessfull");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Success, Lang.PasswordChangedSuccessfully));
                return RedirectToAction("Room", "Profile");
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForClient));
                return View(model);
            }
        }

        public IActionResult Courses()
        {
            var response = _entityService.LoadEntitiesByCriteria<Course>(new BaseCriteriaModel { IntCriteria = Id, RowsPerPage = 50, PageNumber = 1 });
            if (response.IsSuccessfull)
            {
                return View(response.Model.Items);
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForClient));
                return RedirectToAction("Room");
            }
        }

        public IActionResult BecomeInstructor()
        {
            if (CurrentUser.Role != (int)EnumUserRole.Learner)
            {
                return RedirectToAction("Index", "Home");
            }

            var response = _entityService.LoadEntitiesByCriteria<Category>(new CategoryCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
            if (response.IsSuccessfull)
            {
                ViewBag.Categories = new MultiSelectList(response.Model.Items, "Id", GetNameByLangugage());
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BecomeInstructor(BecomeInstructorViewModel model)
        {
            var response = _accountService.BecomeInstructor(model, CurrentUser);

            var responseCategory = _entityService.LoadEntitiesByCriteria<Category>(new CategoryCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
            if (responseCategory.IsSuccessfull)
            {
                ViewBag.Categories = new MultiSelectList(responseCategory.Model.Items, "Id", GetNameByLangugage(), model.CategoryIdSet);
            }

            if (response.IsSuccessfull)
            {
                _logger.LogInformation($"{ MethodBase.GetCurrentMethod().Name } - _entityService.UpdateBy<User> result.IsSuccessfull");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Success, "You became an Instructor successfully"));
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForClient);
                //TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForClient));
            }

            return View(model);

        }
    }
}
