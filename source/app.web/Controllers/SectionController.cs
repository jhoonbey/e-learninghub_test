using app.domain.Enums;
using app.domain.Model.Entities;
using app.service;
using app.web.Core;
using app.web.Extensions;
using app.web.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace app.web.Controllers
{
    [EnableCors("CorsPolicy")]
    [User(AllowedRole = EnumUserRole.Instructor)]
    public class SectionController : BaseController
    {
        private readonly ISectionService _sectionService;

        public SectionController(
                                ILogger<SectionController> logger,
                                IAntiForgeryCookieService antiforgery,
                                ICipherService cipherService,
                                IEntityService entityService,
                                IConfiguration configuration,
                                IHostingEnvironment hostingEnvironment,
                                ISectionService sectionService
                                ) : base(logger, configuration, hostingEnvironment, entityService, cipherService)
        {
            _sectionService = sectionService;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int courseId, string name)
        {
            var response = _sectionService.Create(courseId, name, CurrentUser);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation($"{ MethodBase.GetCurrentMethod().Name } - _sectionService.Create result.IsSuccessfull");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Success, "Section created successfully"));
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForClient);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForClient));
            }

            return RedirectToAction("View", "Course", new { id = courseId });
        }
    }
}