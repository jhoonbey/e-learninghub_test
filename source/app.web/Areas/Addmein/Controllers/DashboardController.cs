using app.domain.Enums;
using app.domain.Model.Entities;
using app.service;
using app.web.Core;
using app.web.Extensions;
using app.web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Reflection;

namespace app.web.Areas.Addmein.Controllers
{
    [User(AllowedRole = EnumUserRole.Admin)]
    [Area("Addmein")]
    public class DashboardController : BaseController
    {
        public DashboardController(
                               ILogger<DashboardController> logger,
                               IEntityService entityService,
                               IConfiguration configuration,
                               ICipherService cipherService,
                               IHostingEnvironment hostingEnvironment
                              ) : base(logger, configuration, hostingEnvironment, entityService, cipherService)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Test()
        {
            Dictionary<string, object> columns = new Dictionary<string, object> { { "Role", 50 } };

            var response = _entityService.UpdateBy<User>(columns, "Email", "gulnar.ahmadova@gmail.com");
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("dashboard Test get result.IsSuccessfull");
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("Index", "Dashboard");
        }

    }
}
