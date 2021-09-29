using app.domain.Enums;
using app.domain.Languages;
using app.domain.Model.Criterias;
using app.domain.Model.Entities;
using app.domain.Model.View;
using app.service;
using app.web.Core;
using app.web.Extensions;
using app.web.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace app.web.Controllers
{
    [EnableCors("CorsPolicy")]
    public class HomeController : BaseController
    {
        private readonly ICommonService _commonService;

        public HomeController(
                                ILogger<HomeController> logger,
                                IEntityService entityService,
                                ICommonService commonService,
                                IConfiguration configuration,
                               ICipherService cipherService,
                                IHostingEnvironment hostingEnvironment
                               ) : base(logger, configuration, hostingEnvironment, entityService, cipherService) => _commonService = commonService;

        public IActionResult SetLanguage(string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return GoToReturnUrl(Request.Headers["Referer"].ToString());
        }

        public IActionResult Index()
        {
            var response = _commonService.GetHomeViewModel();
            if (!response.IsSuccessfull)
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForClient));
                return RedirectToAction("Error", "Home");
            }

            return View(response.Model);
        }

        //public IActionResult RenderCategoryMenu()
        //{
        //    CategoryMenuViewModel categoryMenuViewModel = new CategoryMenuViewModel
        //    {
        //        Categories = _entityService.LoadEntitiesByCriteria<Category>(new BaseCriteriaModel { PageNumber = 1, RowsPerPage = 20 }).Model?.Items,
        //        SubCategories = _entityService.LoadEntitiesByCriteria<SubCategory>(new BaseCriteriaModel { PageNumber = 1, RowsPerPage = 500 }).Model?.Items,
        //        SubOfSubCategories = _entityService.LoadEntitiesByCriteria<SubOfSubCategory>(new BaseCriteriaModel { PageNumber = 1, RowsPerPage = 10000 }).Model?.Items
        //    };

        //    return PartialView("_CategoriesMenu", categoryMenuViewModel);
        //}


        public IActionResult About()
        {
            ViewBag.About_Info = _entityService.GetEntityBy<Option>(new Dictionary<string, object> { { "Sec", "About_Info" } }).Model;
            ViewBag.Skills_Description = _entityService.GetEntityBy<Option>(new Dictionary<string, object> { { "Sec", "Skills_Description" } }).Model;
            ViewBag.Skills = _entityService.LoadEntitiesByCriteria<Skill>(new BaseCriteriaModel { PageNumber = 1, RowsPerPage = 50 }).Model;
            ViewBag.Features = _entityService.LoadEntitiesByCriteria<Feature>(new BaseCriteriaModel { PageNumber = 1, RowsPerPage = 50 }).Model;
            ViewBag.OurVision = _entityService.LoadEntitiesByCriteria<Vision>(new BaseCriteriaModel { PageNumber = 1, RowsPerPage = 50 }).Model;
            ViewBag.OurHappyClients = _entityService.LoadEntitiesByCriteria<Client>(new BaseCriteriaModel { PageNumber = 1, RowsPerPage = 50 }).Model;

            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Contact_Address = _entityService.GetEntityBy<Option>(new Dictionary<string, object> { { "Sec", "Contact_Address" } }).Model;
            ViewBag.Contact_Phone = _entityService.GetEntityBy<Option>(new Dictionary<string, object> { { "Sec", "Contact_Phone" } }).Model;
            ViewBag.Contact_Email = _entityService.GetEntityBy<Option>(new Dictionary<string, object> { { "Sec", "Contact_Email" } }).Model;
            ViewBag.Contact_Info = _entityService.GetEntityBy<Option>(new Dictionary<string, object> { { "Sec", "Contact_Info" } }).Model;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(ContactViewModel model)
        {
            var response = _commonService.SendContactForm(model);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Contact SendContactForm result.IsSuccessfull");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Success, Lang.MailSentMessageSuccess));
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForClient));
            }

            return RedirectToAction("Contact", "Home");
        }

        public IActionResult Instructors()
        {
            ViewBag.Instructors_Info = _entityService.GetEntityBy<Option>(new Dictionary<string, object> { { "Sec", "Instructors_Info" } }).Model;

            var response = _entityService.LoadEntitiesByCriteria<User>(new BaseCriteriaModel { IntCriteria = (int)EnumUserRole.Instructor, PageNumber = 1, RowsPerPage = 100 });
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Instructors.LoadEntitiesByCriteria result.IsSuccessfull");

                return View(response.Model);
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForClient));
                return RedirectToAction("Error");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Test()
        {
            //_logger.LogTrace("HomeController.Index method       wwww!!! LogTrace");
            //_logger.LogDebug("HomeController.Index method       wwww!!! LogDebug");
            //_logger.LogInformation("HomeController.Index method wwww!!! LogInformation");
            //_logger.LogWarning("HomeController.Index method     wwww!!! LogWarning");
            //_logger.LogError("HomeController.Index method       wwww!!! LogError");
            //_logger.LogCritical("HomeController.Index method    wwww!!! LogFatal");

            //ViewBag.uiCultureInfo = Thread.CurrentThread.CurrentUICulture.Name;
            //ViewBag.cultureInfo = Thread.CurrentThread.CurrentCulture.Name;

            //string url = "6Oat/qA6H+VnzenkbgST19OIBZja+VEcj1D8GVQ0tGY=";
            //ViewBag.url = url;

            //var encode = System.Net.WebUtility.UrlEncode(url);
            //ViewBag.encode = encode;

            //var decode = System.Web.HttpUtility.UrlDecode(encode);
            //ViewBag.decode = decode;


            return View();
        }
    }
}
