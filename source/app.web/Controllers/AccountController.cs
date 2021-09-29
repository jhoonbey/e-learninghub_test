using app.domain.Languages;
using app.domain.Model.Criterias;
using app.domain.Model.Entities;
using app.domain.Model.View;
using app.domain.Static;
using app.service;
using app.web.Core;
using app.web.Extensions;
using app.web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;

namespace app.web.Controllers
{
    [EnableCors("CorsPolicy")]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IAntiForgeryCookieService _antiforgery;

        public AccountController(ILogger<AccountController> logger,
                                IAccountService accountService,
                                IAntiForgeryCookieService antiforgery,
                                ICipherService cipherService,
                                IEntityService entityService,
                                IConfiguration configuration,
                                IHostingEnvironment hostingEnvironment
                                //IDistributedCache distributedCache
                                ) : base(logger, configuration, hostingEnvironment, entityService, cipherService)
        {
            _accountService = accountService;
            _antiforgery = antiforgery;
        }

        public IActionResult Start(string act, string rd = "l")
        {
            var result = _entityService.LoadEntitiesByCriteria<Category>(new CategoryCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
            if (result.IsSuccessfull)
            {
                ViewBag.Categories = new SelectList(result.Model.Items, "Id", GetNameByLangugage());
            }

            ViewData["Act"] = act;
            ViewData["RD"] = rd;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(StartViewModel model)
        {
            ViewData["Act"] = "register";

            //client side validation
            ModelState.Remove("LoginEmail");
            ModelState.Remove("LoginPassword");
            ModelState.Remove("LoginRememberMe");
            ModelState.Remove("ResetEmail");
            if (!ModelState.IsValid)
            {
                _logger.LogError("client side validation occured.");

                var responseCategory = _entityService.LoadEntitiesByCriteria<Category>(new CategoryCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
                if (responseCategory.IsSuccessfull)
                {
                    ViewBag.Categories = new SelectList(responseCategory.Model.Items, "Id", GetNameByLangugage());
                }

                return View("Start", model);
            }

            var response = _accountService.Create(model);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("_service.Register result.IsSuccessfull");

                return RedirectToAction("Registered", "Account", new { code = _cipherService.Encrypt(StaticValues.RegisterKey, model.Name + "#" + model.Surname) });
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForClient));

                var resultCategory = _entityService.LoadEntitiesByCriteria<Category>(new CategoryCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
                if (resultCategory.IsSuccessfull)
                {
                    ViewBag.Categories = new SelectList(resultCategory.Model.Items, "Id", GetNameByLangugage());
                }

                return View("Start", model);
            }
        }

        [AllowAnonymous]
        public IActionResult Registered(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                try
                {
                    ViewBag.Message = _cipherService.Decrypt(StaticValues.RegisterKey, code).Replace('#', ' ');
                }
                catch (Exception)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return View();
        }

        [AllowAnonymous]
        public IActionResult Confirm(string code)
        {
            var response = _accountService.ConfirmEmail(WebUtility.UrlEncode(code));
            if (response.IsSuccessfull)
            {
                return RedirectToAction("ConfirmSuccess", "Account", new { code = WebUtility.UrlEncode(code) });
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForClient));
                return RedirectToAction("ConfirmFailed", "Account", new { code = WebUtility.UrlEncode(code) });
            }
        }

        [AllowAnonymous]
        public IActionResult ConfirmSuccess(string code)
        {
            var response = _accountService.ConfirmSuccess(WebUtility.UrlEncode(code));
            if (response.IsSuccessfull)
            {
                ViewBag.Message = response.Model.Email;
                return View();
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForClient));
                return RedirectToAction("ConfirmFailed", "Account", new { code = WebUtility.UrlEncode(code) });
            }
        }

        [AllowAnonymous]
        public IActionResult ConfirmFailed(string code = "")
        {
            _logger.LogError("ConfirmFailed: " + WebUtility.UrlEncode(code));
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(StartViewModel model)
        {
            ViewData["Act"] = "login";

            //client side validation
            ModelState.Remove("Name");
            ModelState.Remove("Surname");
            ModelState.Remove("Mobile");
            ModelState.Remove("Email");
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("Position");
            ModelState.Remove("Workplace");
            ModelState.Remove("Role");
            ModelState.Remove("CategoryId");
            ModelState.Remove("IsAgreeTerms");
            ModelState.Remove("Rolename");
            ModelState.Remove("ResetEmail");

            if (!ModelState.IsValid)
            {
                _logger.LogError("Login client side validation occured.");

                var resultCategory = _entityService.LoadEntitiesByCriteria<Category>(new CategoryCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
                if (resultCategory.IsSuccessfull)
                {
                    ViewBag.Categories = new SelectList(resultCategory.Model.Items, "Id", GetNameByLangugage());
                }

                return View("Start", model);
            }

            var response = _accountService.Find(model);
            if (response.IsSuccessfull)
            {
                response.Model.Password = null;

                //fill redis
                //AddToCache(result.Model.Id.ToString(), CreateAccessToken(result.Model));

                //fill cookies
                LoadAuthData(response.Model, model.LoginRememberMe);
                SaveAuthData();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var resultCategory = _entityService.LoadEntitiesByCriteria<Category>(new CategoryCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
                if (resultCategory.IsSuccessfull)
                {
                    ViewBag.Categories = new SelectList(resultCategory.Model.Items, "Id", GetNameByLangugage());
                }

                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForClient));
                return View("Start", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reset(StartViewModel model)
        {
            ViewData["Act"] = "reset";

            //client side validation
            ModelState.Remove("Name");
            ModelState.Remove("Surname");
            ModelState.Remove("Mobile");
            ModelState.Remove("Email");
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("Position");
            ModelState.Remove("Workplace");
            ModelState.Remove("Role");
            ModelState.Remove("CategoryId");
            ModelState.Remove("IsAgreeTerms");
            ModelState.Remove("Rolename");
            ModelState.Remove("LoginEmail");
            ModelState.Remove("LoginPassword");
            ModelState.Remove("LoginRememberMe");

            if (!ModelState.IsValid)
            {
                _logger.LogError("Reset - client side validation occured.");

                var resultCategory = _entityService.LoadEntitiesByCriteria<Category>(new CategoryCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
                if (resultCategory.IsSuccessfull)
                {
                    ViewBag.Categories = new SelectList(resultCategory.Model.Items, "Id", GetNameByLangugage());
                }

                return View("Start", model);
            }

            var response = _accountService.ResetRequest(model);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("_service.Reset result.IsSuccessfull");
                return RedirectToAction("ResetSent", "Account", new { code = WebUtility.UrlEncode(_cipherService.Encrypt(StaticValues.ResetPublicKey, response.Model.Id.ToString())) });
            }
            else
            {
                var resultCategory = _entityService.LoadEntitiesByCriteria<Category>(new CategoryCriteriaModel { RowsPerPage = 50, PageNumber = 1 });
                if (resultCategory.IsSuccessfull)
                {
                    ViewBag.Categories = new SelectList(resultCategory.Model.Items, "Id", GetNameByLangugage());
                }

                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForClient));
                return View("Start", model);
            }
        }

        [AllowAnonymous]
        public IActionResult ResetSent(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                try
                {
                    var idStr = _cipherService.Decrypt(StaticValues.ResetPublicKey, WebUtility.UrlDecode(code));
                    int id = Convert.ToInt32(idStr);
                    var response = _entityService.GetEntityById<User>(id);
                    if (response.IsSuccessfull)
                    {
                        ViewBag.Message = response.Model.Email;
                    }
                    else
                        throw new Exception(response.ErrorForClient);
                }
                catch (Exception ex)
                {
                    _logger.LogError("ResetSent error:" + ex.Message);
                    return RedirectToAction("Index", "Home");
                }
            }

            return View();
        }



        [AllowAnonymous]
        public IActionResult ResetPassword(string code)
        {
            var response = _accountService.ValidateResetLink(WebUtility.UrlDecode(code));
            if (!response.IsSuccessfull)
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForClient));
                return RedirectToAction("ResetFailed", "Account");
            }

            ViewData["Code"] = WebUtility.UrlDecode(code);

            return View();
        }


        [AllowAnonymous]
        public IActionResult ResetFailed()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(PasswordResetModel model)
        {
            ViewData["Code"] = model.Code;

            if (!ModelState.IsValid)
            {
                _logger.LogError("ResetPassword - client side validation occured.");
                return View(model);
            }

            var response = _accountService.ResetPassword(model);
            if (response.IsSuccessfull)
            {
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Success, Lang.PasswordChangedSuccessfullyText));
                return RedirectToAction("Start", "Account");
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForClient);
                return View(model);
            }
        }


        [AllowAnonymous]
        public IActionResult Logout()
        {
            ClearAuthData();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //public IActionResult About()
        //{
        //    ViewData["Message"] = "Your application description page.";
        //    List<User> result = _appRepo.LoadEntitiesByCriteria<User>(new BaseCriteriaModel { WillCount = false }, 100, 1).Items;
        //    return View(result);
        //}

        //public IActionResult Contact()
        //{
        //    try
        //    {
        //        ViewBag.ContactData = _appRepo.GetContactViewModel();
        //    }
        //    catch (Exception)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    return View();
        //}

        //[HttpPost]
        //public IActionResult Contact(Quote model)
        //{
        //    try
        //    {
        //        string host = _configuration["Email:Host"].ToString();
        //        int port = Convert.ToInt32(_configuration["Email:Port"].ToString());
        //        int timeout = Convert.ToInt32(_configuration["Email:TimeOut"].ToString());
        //        bool enableSsl = Convert.ToBoolean(_configuration["Email:Ssl"].ToString());
        //        string username = _configuration["Email:Username"].ToString();
        //        string password = _configuration["Email:Password"].ToString();
        //        string emailFrom = _configuration["Email:From"].ToString();
        //        string emailTo = _configuration["Email:To"].ToString();
        //        string subject = "E-mail which sent from " + _configuration["Site:NameAdjacent"].ToString() + " site";

        //        _appRepo.SaveQuote(host, port, timeout, enableSsl, username, password, emailFrom, emailTo, subject, model);

        //        TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Success, "Mail sent successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.ContactData = _appRepo.GetContactViewModel();
        //        TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
        //    }

        //    return RedirectToAction("Contact", "Home");
        //}
    }
}