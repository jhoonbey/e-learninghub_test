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
    public class ClientsController : BaseController
    {
        private readonly Func<EnumFileType, IFileService> _fileService;

        public ClientsController(
                               ILogger<ClientsController> logger,
                               IEntityService entityService,
                               IConfiguration configuration,
                               ICipherService cipherService,
                               IHostingEnvironment hostingEnvironment,
                               Func<EnumFileType, IFileService> serviceResolver
            ) : base(logger, configuration, hostingEnvironment, entityService, cipherService)
        {
            _fileService = serviceResolver;
        }


        public IActionResult List()
        {
            int pageNumber = 1;
            int rowsPerPage = 50;

            var response = _entityService.LoadEntitiesByCriteria<Client>(new BaseCriteriaModel { PageNumber = pageNumber, RowsPerPage = rowsPerPage });
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Clients-List result.IsSuccessfull");
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
            var response = _entityService.GetEntityById<Client>(id);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Clients-View get result.IsSuccessfull");
                return View(response.Model);
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("List", "Clients");
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult Create(Client model)
        {
            var response = _entityService.Create<Client>(model, "", "", false);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Clients Create post result.IsSuccessfull");
                return RedirectToAction("List", "Clients");
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
            var response = _entityService.GetEntityById<Client>(id);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Clients-Edit get result.IsSuccessfull");
                return View(response.Model);
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("List", "Clients");
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult Edit(Client model)
        {
            var response = _entityService.UpdateByAll<Client>(model, "Id", model.Id, false, "", "");
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Clients Edit post result.IsSuccessfull");
                return RedirectToAction("List", "Clients");
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
                var response = _entityService.GetEntityById<Client>(id);
                if (!response.IsSuccessfull)
                {
                    _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                    TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
                }

                //delete image
                if (!string.IsNullOrEmpty(response.Model.Imagename))
                {
                    var responseImage = _fileService(EnumFileType.Image).Delete(Path.Combine(_hostingEnvironment.WebRootPath, _configuration["Site:ImagesPath"]), "Client", response.Model.Imagename);
                    if (!responseImage.IsSuccessfull)
                    {
                        throw new Exception("Error on image deleting");
                    }
                }


                //delete entity
                var response2 = _entityService.DeleteById<Client>(id);
                if (response2.IsSuccessfull)
                {
                    _logger.LogInformation("Clients Delete result.IsSuccessfull");
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

            return RedirectToAction("List", "Clients");
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

                _logger.LogInformation("_hostingEnvironment is starting = " + _hostingEnvironment.WebRootPath);

                var response = _entityService.GetEntityById<Client>(id);
                if (!response.IsSuccessfull)
                {
                    _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                    TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
                }

                _logger.LogInformation("_hostingEnvironment.WebRootPath = " + _hostingEnvironment.WebRootPath);
                _logger.LogInformation("_configuration[Site:ImagePath] = " + _configuration["Site:ImagesPath"]);

                //save image
                string path = Path.Combine(_hostingEnvironment.WebRootPath, _configuration["Site:ImagesPath"]);
                var responseImage = _fileService(EnumFileType.Image).Save_Create(postedFile, response.Model.Id.ToString(), path, "Client");
                if (!responseImage.IsSuccessfull)
                {
                    throw new Exception("Error on image saving. imageName is null or empty");
                }

                _entityService.UpdateBy<Client>(new Dictionary<string, object> { { "Imagename", responseImage.Model } }, "Id", response.Model.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, ex.ToString()));
            }

            return RedirectToAction("View", "Clients", new { id });
        }
        public IActionResult DeleteImage(int id)
        {
            try
            {
                var response = _entityService.GetEntityById<Client>(id);
                if (!response.IsSuccessfull)
                {
                    _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                    TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
                }

                //delete image
                string path = Path.Combine(_hostingEnvironment.WebRootPath, _configuration["Site:ImagesPath"]);
                var responseImage = _fileService(EnumFileType.Image).Delete(path, "Client", response.Model.Imagename);
                if (!responseImage.IsSuccessfull)
                {
                    throw new Exception("Error on image deleting");
                }

                _entityService.UpdateBy<Client>(new Dictionary<string, object> { { "Imagename", null } }, "Id", response.Model.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, ex.ToString()));
            }

            return RedirectToAction("View", "Clients", new { id });
        }
    }
}
