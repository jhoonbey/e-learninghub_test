using app.domain.Enums;
using app.domain.Model.Criterias;
using app.domain.Model.Entities;
using app.service;
using app.web.Core;
using app.web.Extensions;
using app.web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace app.web.Areas.Addmein.Controllers
{
    [User(AllowedRole = EnumUserRole.Admin)]
    [Area("Addmein")]
    public class CategoriesController : BaseController
    {
        private readonly IOptionService _optionService;
        private readonly Func<EnumFileType, IFileService> _fileService;

        public CategoriesController(
                               ILogger<CategoriesController> logger,
                               IOptionService optionService,
                               IEntityService entityService,
                               IConfiguration configuration,
                               ICipherService cipherService,
                               IHostingEnvironment hostingEnvironment,
                               Func<EnumFileType, IFileService> serviceResolver

            ) : base(logger, configuration, hostingEnvironment, entityService, cipherService)
        {
            _optionService = optionService;
            _fileService = serviceResolver;
        }

        public IActionResult List()
        {
            int pageNumber = 1;
            int rowsPerPage = 50;

            var response = _entityService.LoadEntitiesByCriteria<Category>(new BaseCriteriaModel { PageNumber = pageNumber, RowsPerPage = rowsPerPage });
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Categories-List result.IsSuccessfull");
                return View(response.Model);
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult View(int id)
        {
            var response = _entityService.GetEntityById<Category>(id);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Categories-View get result.IsSuccessfull");
                return View(response.Model);
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("List", "Categories");
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category model)
        {
            var response = _entityService.Create<Category>(model, "", "", false);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Categories Create post result.IsSuccessfull");
                return RedirectToAction("List", "Categories");
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
            var response = _entityService.GetEntityById<Category>(id);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Categories-Edit get result.IsSuccessfull");
                return View(response.Model);
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("List", "Categories");
        }

        [HttpPost]
        public IActionResult Edit(Category model)
        {
            var response = _entityService.UpdateByAll<Category>(model, "Id", model.Id, false, "", "");
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Categories Edit post result.IsSuccessfull");
                return RedirectToAction("List", "Categories");
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
            try
            {
                //delete entity
                var response = _entityService.GetEntityById<Category>(id);
                if (!response.IsSuccessfull)
                {
                    _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                    TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
                }

                //delete image
                if (!string.IsNullOrEmpty(response.Model.Imagename))
                {
                    var responseImage = _fileService(EnumFileType.Image).Delete(Path.Combine(_hostingEnvironment.WebRootPath, _configuration["Site:ImagesPath"]), "Category", response.Model.Imagename);
                    if (!responseImage.IsSuccessfull)
                    {
                        throw new Exception("Error on image deleting");
                    }
                }


                //delete entity
                var response2 = _entityService.DeleteById<Category>(id);
                if (response2.IsSuccessfull)
                {
                    _logger.LogInformation("Category Delete result.IsSuccessfull");
                }
                else
                {
                    _logger.LogError(response2.ErrorForLog);
                    TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response2.ErrorForLog));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, ex.ToString()));
            }

            return RedirectToAction("List", "Categories");
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public ActionResult UpdateImage(IFormFile postedFile, int id)
        {
            try
            {
                if (postedFile == null)
                {
                    throw new Exception("Image is empty. Please select an image");
                }

                _logger.LogInformation("Category _hostingEnvironment is starting = " + _hostingEnvironment.WebRootPath);

                var response = _entityService.GetEntityById<Category>(id);
                if (!response.IsSuccessfull)
                {
                    _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                    TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
                }

                _logger.LogInformation("Category _hostingEnvironment.WebRootPath = " + _hostingEnvironment.WebRootPath);
                _logger.LogInformation("Category _configuration[Site:ImagePath] = " + _configuration["Site:ImagesPath"]);

                //save image
                var responseImage = _fileService(EnumFileType.Image).Save_Create(postedFile, response.Model.Id.ToString(),
                    Path.Combine(_hostingEnvironment.WebRootPath, _configuration["Site:ImagesPath"]), "Category");
                if (!responseImage.IsSuccessfull)
                {
                    throw new Exception("Error on image saving. imageName is null or empty");
                }

                _entityService.UpdateBy<Category>(new Dictionary<string, object> { { "Imagename", responseImage.Model } }, "Id", response.Model.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, ex.ToString()));
            }

            return RedirectToAction("View", "Categories", new { id });
        }
        public IActionResult DeleteImage(int id)
        {
            try
            {
                var response = _entityService.GetEntityById<Category>(id);
                if (!response.IsSuccessfull)
                {
                    _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                    TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
                }

                //delete image
                string path = Path.Combine(_hostingEnvironment.WebRootPath, _configuration["Site:ImagesPath"]);
                var responseImage = _fileService(EnumFileType.Image).Delete(path, "Category", response.Model.Imagename);
                if (!responseImage.IsSuccessfull)
                {
                    throw new Exception("Error on image deleting");
                }

                _entityService.UpdateBy<Category>(new Dictionary<string, object> { { "Imagename", null } }, "Id", response.Model.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, ex.ToString()));
            }

            return RedirectToAction("View", "Categories", new { id });
        }
    }
}
