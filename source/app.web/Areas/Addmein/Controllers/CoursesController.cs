using app.domain.Enums;
using app.domain.Model.Criterias;
using app.domain.Model.Entities;
using app.domain.Utilities;
using app.service;
using app.web.Core;
using app.web.Extensions;
using app.web.Models;
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

namespace app.web.Areas.Addmein.Controllers
{
    [User(AllowedRole = EnumUserRole.Admin)]
    [Area("Addmein")]
    public class CoursesController : BaseController
    {
        private readonly IOptionService _optionService;
        private readonly Func<EnumFileType, IFileService> _fileService;

        public CoursesController(
                               ILogger<CoursesController> logger,
                               IOptionService optionService,
                               IEntityService entityService,
                               IConfiguration configuration,
                               ICipherService cipherService,
                               IHostingEnvironment hostingEnvironment,
                               Func<EnumFileType, IFileService> fileService

            ) : base(logger, configuration, hostingEnvironment, entityService, cipherService)
        {
            _optionService = optionService;
            _fileService = fileService;
        }

        public IActionResult List(string keyword = null, int? status = null, DateTime? minCD = null, DateTime? maxCD = null, int pageNumber = 1)
        {
            int rowsPerPage = 50;

            BaseCriteriaModel criteria = new BaseCriteriaModel
            {
                Keyword = keyword,
                IntCriteria2 = status,
                MinCreateDate = minCD,
                MaxCreateDate = maxCD,
                PageNumber = pageNumber,
                RowsPerPage = rowsPerPage
            };

            var response = _entityService.LoadEntitiesByCriteria<Course>(criteria);

            ViewBag.Criteria = criteria;
            ViewBag.PageNumber = pageNumber;
            ViewBag.RowsPerPage = rowsPerPage;
            ViewBag.NumberOfPages = GetPageNumber(response.Model.AllCount, rowsPerPage);

            //Statuses
            if (status == 0)
                ViewBag.Statuses = new SelectList(Common.ConvertEnumToDDL(typeof(EnumCourseStatus), null), "Id", "Name");
            else
                ViewBag.Statuses = new SelectList(Common.ConvertEnumToDDL(typeof(EnumCourseStatus), null), "Id", "Name", status);

            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Courses-List result.IsSuccessfull");
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
            var response = _entityService.GetEntityById<Course>(id);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Courses-View get result.IsSuccessfull");

                //videos
                ViewBag.Videos = _entityService.LoadEntitiesByCriteria<Video>(new BaseCriteriaModel { IntCriteria = response.Model.Id, PageNumber = 1, RowsPerPage = 100 }).Model;

                return View(response.Model);
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("List", "Courses");
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Course model)
        {
            var response = _entityService.Create<Course>(model, "", "", false);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Courses Create post result.IsSuccessfull");
                return RedirectToAction("List", "Courses");
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
            var response = _entityService.GetEntityById<Course>(id);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Courses-Edit get result.IsSuccessfull");
                return View(response.Model);
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("List", "Courses");
        }

        [HttpPost]
        public IActionResult Edit(Course model)
        {
            var response = _entityService.UpdateByAll<Course>(model, "Id", model.Id, false, "", "");
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Courses Edit post result.IsSuccessfull");
                return RedirectToAction("List", "Courses");
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForLog);
                return View(model);
            }
        }

        public IActionResult Sendback(int id)
        {
            var response = _entityService.GetEntityById<Course>(id);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Courses-Sendback get response.IsSuccessfull");
                return View(response.Model);
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("List", "Courses");
        }

        [HttpPost]
        public IActionResult Sendback(Course model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.RejectReason) || model.RejectReason.Length < 10)
                {
                    throw new Exception("Rejecet reason is required. Minimum symbol count is 10");
                }

                var response = _entityService.UpdateBy<Course>(new Dictionary<string, object> {
                { "RejectReason", model.RejectReason },
                { "Status", (int)EnumCourseStatus.Draft }
                }, "Id", model.Id);

                if (!response.IsSuccessfull)
                {
                    _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                    AddError(response.Key, response.ErrorForLog);
                    return View(model);
                }
                else
                {
                    _logger.LogInformation("Courses Sendback  UpdateBy post result.IsSuccessfull");
                    return RedirectToAction("List", "Courses");
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, ex.Message));
                return View(model);
            }
        }

        public IActionResult Approve(int id)
        {
            var response = _entityService.GetEntityById<Course>(id);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Courses-Approve get response.IsSuccessfull");
                return View(response.Model);
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
            }

            return RedirectToAction("List", "Courses");
        }

        [HttpPost]
        public IActionResult Approve(Course model)
        {
            try
            {
                if (model == null)
                {
                    throw new Exception("Model is null");
                }

                var response = _entityService.UpdateBy<Course>(new Dictionary<string, object>
                {
                    { "RejectReason", string.Empty },
                    { "Status", (int)EnumCourseStatus.Approved },
                    { "ActualPrice", model.ActualPrice }
                }, "Id", model.Id);

                if (!response.IsSuccessfull)
                {
                    _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                    AddError(response.Key, response.ErrorForLog);
                    return View(model);
                }
                else
                {
                    _logger.LogInformation("Courses Approve  UpdateBy post result.IsSuccessfull");
                    return RedirectToAction("List", "Courses");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, ex.Message));
                return View(model);
            }
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public ActionResult UpdatePreviewVideo(IFormFile postedFile, int id)
        {
            try
            {
                //postedFile validations
                //???

                if (postedFile == null)
                {
                    throw new Exception("Video is empty. Please select an video");
                }

                var response = _entityService.GetEntityById<Course>(id);
                if (!response.IsSuccessfull)
                {
                    _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                    TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
                    return RedirectToAction("View", "Courses", new { id });
                }

                response.Model.Extension = Path.GetExtension(postedFile.FileName);
                response.Model.Filename = string.Empty;
                response.Model.Snapshot = string.Empty;
                response.Model.MediaType = postedFile.ContentType;

                //save video file ---------------------------
                string pathVideo = Path.Combine(_hostingEnvironment.WebRootPath, _configuration["Site:VideosPath"], Path.Combine("Course", "Preview", response.Model.Id.ToString()));
                var responseVideo = _fileService(EnumFileType.Video).Save_Create(postedFile, response.Model.Id.ToString(), pathVideo, string.Empty);

                if (!responseVideo.IsSuccessfull)
                {
                    throw new Exception("Error on video saving");
                }

                response.Model.Filename = responseVideo.Model;

                //save Snapshot file ---------------------------
                response.Model.Snapshot = Common.CRP(12) + ".png";
                string pathSnapshot = Path.Combine(_hostingEnvironment.WebRootPath, _configuration["Site:ImagesPath"], Path.Combine("Snapshot", "Preview", response.Model.Id.ToString(), "Original"));
                string pathVideoFull = Path.Combine(pathVideo, "Original", response.Model.Filename);

                string ffmpegPath = Path.Combine(_configuration["Site:FFmpegPath"], "ffmpeg.exe");

                var responseSnapshot = _fileService(EnumFileType.Image).ExtractImageFromVideo(ffmpegPath, pathVideoFull, pathSnapshot, response.Model.Snapshot, 0.1);
                if (!responseSnapshot.IsSuccessfull)
                {
                    _logger.LogInformation($"{ MethodBase.GetCurrentMethod().Name } - ErrorForLog - {responseSnapshot.ErrorForLog}");
                    throw new Exception("Error on extraction of snapshot: " + responseSnapshot.ErrorForClient);
                }

                //update ---------------------------
                _entityService.UpdateBy<Course>(new Dictionary<string, object> {
                    { "Extension", response.Model.Extension },
                    { "Filename", response.Model.Filename },
                    { "Snapshot", response.Model.Snapshot },
                    { "MediaType", response.Model.MediaType }
                }, "Id", response.Model.Id);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, ex.ToString()));
            }

            return RedirectToAction("View", "Courses", new { id });
        }

        //public IActionResult Delete(int id)
        //{
        //    try
        //    {
        //        //delete entity
        //        var response = _entityService.GetEntityById<Course>(id);
        //        if (!response.IsSuccessfull)
        //        {
        //            _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
        //            TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
        //        }

        //        //delete entity
        //        var response2 = _entityService.DeleteById<Course>(id);
        //        if (response2.IsSuccessfull)
        //        {
        //            _logger.LogInformation("Course Delete result.IsSuccessfull");
        //        }
        //        else
        //        {
        //            _logger.LogError(response2.ErrorForLog);
        //            TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response2.ErrorForLog));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.ToString());
        //        TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, ex.ToString()));
        //    }

        //    return RedirectToAction("List", "Courses");
        //}

        //[HttpPost]
        //[IgnoreAntiforgeryToken]
        //public ActionResult UpdateImage(IFormFile postedFile, int id)
        //{
        //    try
        //    {
        //        if (postedFile == null)
        //        {
        //            throw new Exception("Image is empty. Please select an image");
        //        }

        //        _logger.LogInformation("Course _hostingEnvironment is starting = " + _hostingEnvironment.WebRootPath);

        //        var response = _entityService.GetEntityById<Course>(id);
        //        if (!response.IsSuccessfull)
        //        {
        //            _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
        //            TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
        //        }

        //        _logger.LogInformation("Course _hostingEnvironment.WebRootPath = " + _hostingEnvironment.WebRootPath);
        //        _logger.LogInformation("Course _configuration[Site:ImagePath] = " + _configuration["Site:ImagesPath"]);

        //        //save image
        //        var responseImage = _fileService(EnumFileType.Image).Save_Create(postedFile, response.Model.Id.ToString(), Path.Combine(_hostingEnvironment.WebRootPath, _configuration["Site:ImagesPath"]), "Course");
        //        if (!responseImage.IsSuccessfull)
        //        {
        //            throw new Exception("Error on image saving. imageName is null or empty");
        //        }

        //        _entityService.UpdateBy<Course>(new Dictionary<string, object> { { "Imagename", responseImage.Model } }, "Id", response.Model.Id);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.ToString());
        //        TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, ex.ToString()));
        //    }

        //    return RedirectToAction("View", "Courses", new { id });
        //}
        //public IActionResult DeleteImage(int id)
        //{
        //    try
        //    {
        //        var response = _entityService.GetEntityById<Course>(id);
        //        if (!response.IsSuccessfull)
        //        {
        //            _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
        //            TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
        //        }

        //        //delete image
        //        var responseImage = _fileService(EnumFileType.Image).Delete(Path.Combine(_hostingEnvironment.WebRootPath, _configuration["Site:ImagesPath"]), "Course", response.Model.Imagename);
        //        if (!responseImage.IsSuccessfull)
        //        {
        //            throw new Exception("Error on image deleting");
        //        }

        //        _entityService.UpdateBy<Course>(new Dictionary<string, object> { { "Imagename", null } }, "Id", response.Model.Id);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.ToString());
        //        TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, ex.ToString()));
        //    }

        //    return RedirectToAction("View", "Courses", new { id });
        //}
    }
}
