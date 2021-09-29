using app.domain.Model.Entities;
using app.domain.Static;
using app.domain.Utilities;
using app.service;
using app.web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading;

namespace app.web.Core
{
    public class BaseController : Controller
    {
        internal User CurrentUser;
        protected readonly ILogger<BaseController> _logger;
        protected readonly IHostingEnvironment _hostingEnvironment;
        protected readonly IConfiguration _configuration;
        internal readonly IEntityService _entityService;
        internal readonly ICipherService _cipherService;

        //private readonly IDistributedCache _distributedCache;

        public BaseController(
            ILogger<BaseController> logger,
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment,
            IEntityService entityService,
            ICipherService cipherService
            //IDistributedCache distributedCache
            )
        {
            _logger = logger;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _entityService = entityService;
            _cipherService = cipherService;
            //_distributedCache = distributedCache;

            ViewBag.SeoKeyword = _configuration["Site:Seokeyword"].ToString();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            LoadAuthData();

            if (!RememberMe)
            {
                var diffMinutes = DateTime.UtcNow.Subtract(LastActiveDate).TotalMinutes;
                if (diffMinutes > Convert.ToInt32(_configuration["Site:ExpiryTimeInMinute"].ToString()))
                {
                    ClearAuthData();
                }
                else
                {
                    SetCookie("MNR", Convert.ToString(Common.ConvertDateTimeToUnixTime(DateTime.UtcNow)));
                }
            }

            base.OnActionExecuting(filterContext);
        }


        public string Token { get; set; }
        public string TempToken { get; set; }

        public bool IsAuthorized { get; set; }
        public int Id { get; set; }
        public string Fullname { get; set; }
        public int Role { get; set; }
        public bool RememberMe { get; set; }
        public DateTime LastActiveDate { get; set; }


        protected void LoadAuthData()
        {
            Token = Request.Cookies.ContainsKey("Token") ? Convert.ToString(Request.Cookies["Token"].ToString()) : null;
            TempToken = Request.Cookies.ContainsKey("TempToken") ? Convert.ToString(Request.Cookies["TempToken"].ToString()) : null;

            try
            {
                List<string> list = Common.ConvertStringToStringSet(_cipherService.Decrypt(StaticValues.CookieKey, TempToken), '¶');

                IsAuthorized = Convert.ToBoolean(list[0]);
                Id = Convert.ToInt32(list[1]);
                Fullname = list[2];

                Role = Convert.ToInt32(list[3]);
                RememberMe = Convert.ToBoolean(list[4]);
                LastActiveDate = DateTime.ParseExact(list[5], "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                ClearAuthData();
                return;
            }

            //IsAuthorized = Request.Cookies.ContainsKey("IsAuthorized") ? Convert.ToBoolean(Request.Cookies["IsAuthorized"]) : false;
            //Id = Request.Cookies.ContainsKey("Id") ? Convert.ToInt32(Request.Cookies["Id"]) : 0;
            //Fullname = Request.Cookies.ContainsKey("Fullname") ? Convert.ToString(Request.Cookies["Fullname"]) : null;
            //LastActiveDate = Request.Cookies.ContainsKey("MNR") ? Common.ConvertUnixTimeToDateTime(Convert.ToInt64(Request.Cookies["MNR"])) : DateTime.MinValue;
            //RememberMe = Request.Cookies.ContainsKey("OCR") ? Convert.ToBoolean(Request.Cookies["OCR"]) : false;
            //Email = Request.Cookies.ContainsKey("Email") ? Convert.ToString(Request.Cookies["Email"]) : null;
            //Role = Request.Cookies.ContainsKey("Role") ? Convert.ToInt32(SecurityManager.DecryptString("role", Convert.ToString(Request.Cookies["Role"]))) : 0;

            ViewBag.IsAuthorized = IsAuthorized;
            ViewBag.Id = Id;
            ViewBag.Fullname = Fullname;
            ViewBag.Role = Role;
            ViewBag.RememberMe = RememberMe;
            ViewBag.LastActiveDate = LastActiveDate;

            ViewBag.Token = Token;
            ViewBag.TempToken = TempToken;
        }
        protected void LoadAuthData(User user, bool rememberMe)
        {
            IsAuthorized = true;
            Id = user.Id;
            Fullname = user.Name + " " + user.Surname;
            Role = user.Role;
            RememberMe = rememberMe;
            LastActiveDate = DateTime.UtcNow;

            //tokens
            Token = CreateAccessToken(user);
            TempToken = _cipherService.Encrypt(StaticValues.CookieKey, $"{IsAuthorized}¶{Id}¶{Fullname}¶{Role}¶{RememberMe}¶{LastActiveDate.ToString("yyyyMMddHHmmss")}");

            ViewBag.IsAuthorized = IsAuthorized;
            ViewBag.Id = Id;
            ViewBag.Fullname = Fullname;
            ViewBag.Role = Role;
            ViewBag.RememberMe = RememberMe;
            ViewBag.LastActiveDate = LastActiveDate;

            //tokens
            ViewBag.Token = Token;
            ViewBag.TempToken = TempToken;
        }

        public void SaveAuthData()
        {
            SetCookie("Token", Token);
            SetCookie("TempToken", TempToken);

            SetCookie("IsAuthorized", Convert.ToString(this.IsAuthorized));
            SetCookie("Id", Convert.ToString(this.Id));
            SetCookie("Fullname", this.Fullname);
            SetCookie("MNR", Convert.ToString(Common.ConvertDateTimeToUnixTime(this.LastActiveDate)));
            SetCookie("OCR", Convert.ToString(this.RememberMe));
            Common.ConvertDateTimeToUnixTime(this.LastActiveDate);
        }

        public void ClearAuthData()
        {
            DeleteCookie("Token");
            DeleteCookie("TempToken");

            // Removing Session
            HttpContext.Session.Clear();
        }

        internal void SetCookie(string key, string value)
        {
            CookieOptions option = new CookieOptions
            {
                HttpOnly = true
                //Secure = true
            };
            //if (rememberMe)
            //    option.Expires = LAD.AddMinutes(Convert.ToInt32(_configuration["Site:ExpiryTimeInMinute"].ToString()));
            //else
            //    option.Expires = DateTime.Now.AddMilliseconds(20);

            option.Expires = DateTime.UtcNow.AddYears(1);
            Response.Cookies.Append(key, value, option);
        }

        private void DeleteCookie(string key)
        {
            Response.Cookies.Delete(key);
        }

        public static int GetPageNumber(int allCount, int rowsPerPage)
        {
            if (rowsPerPage != 0)
            {
                var pageCount = allCount;
                if (pageCount % rowsPerPage == 0)
                    return pageCount / rowsPerPage;
                return (pageCount / rowsPerPage) + 1;
            }
            return allCount;
        }
        public ClientAlertModel FillAlertModel(AlertStatus status, string message)
        {
            return new ClientAlertModel(status, message);
        }
        public void AddError(string errorText)
        {
            AddError("errorKey", errorText);
        }
        public void AddError(string key, string errorText)
        {
            ModelState.AddModelError(key ?? "errorKey", errorText);
            _logger.LogError($"Key = {key ?? "errorKey" }   Message = " + errorText);
        }

        public string CreateAccessToken(User user)
        {
            string keyFromConfig = _configuration["Security:Key"].ToString();
            string keyResult = user.Id.ToString() + keyFromConfig + user.ConfirmationGuid;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyResult));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        protected IActionResult GoToReturnUrl(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        public string GetNameByLangugage()
        {
            string lang = Thread.CurrentThread.CurrentUICulture.Name.Substring(0, 2);
            if (string.IsNullOrEmpty(lang))
                return string.Empty;
            switch (lang)
            {
                case "az": return "NameAZ";
                case "en": return "NameEN";
                case "ru": return "NameRU";
                case "tr": return "NameTR";
                default: return string.Empty;
            }
        }

        /////////////
        public void AddToCache(string key, string value)
        {
            //_distributedCache.SetString(key, value);
        }
        public void RemoveFromCache(string key, string value)
        {
            //_distributedCache.Remove(key);
        }
    }
}
