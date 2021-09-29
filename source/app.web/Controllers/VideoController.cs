using app.domain.Enums;
using app.domain.Model.Entities;
using app.domain.Model.View;
using app.service;
using app.web.Core;
using app.web.Extensions;
using app.web.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;

namespace app.web.Controllers
{
    [EnableCors("CorsPolicy")]
    [User(AllowedRole = EnumUserRole.Instructor)]
    public class VideoController : BaseController
    {
        private readonly IVideoService _videoService;
        private readonly Func<EnumFileType, IFileService> _fileService;

        public VideoController(
                                ILogger<VideoController> logger,
                                IAntiForgeryCookieService antiforgery,
                                ICipherService cipherService,
                                IEntityService entityService,
                                IConfiguration configuration,
                                IHostingEnvironment hostingEnvironment,
                                IVideoService videoService,
                                Func<EnumFileType, IFileService> fileService
                                ) : base(logger, configuration, hostingEnvironment, entityService, cipherService)
        {
            _videoService = videoService;
            _fileService = fileService;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IFormFile postedFile, int courseId, int sectionId, string name)
        {
            VideoUploadModel videoUploadModel = new VideoUploadModel
            {
                PostedFile = postedFile,
                CourseId = courseId,
                SectionId = sectionId,
                Name = name,
                RootPath = _hostingEnvironment.WebRootPath
            };

            var response = _videoService.Create(videoUploadModel, CurrentUser);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation($"{ MethodBase.GetCurrentMethod().Name } - _videoService.Create result.IsSuccessfull");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Success, "Video uploaded successfully"));
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                AddError(response.Key, response.ErrorForLog);
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForClient));
            }

            return RedirectToAction("View", "Course", new { id = courseId });
        }

        public IActionResult Delete(int id)
        {
            var response = _videoService.DeleteRequest(id, CurrentUser);
            if (!response.IsSuccessfull)
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - GetEntityById<Video - " + response.ErrorForLog}");
                TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
                return Redirect(Request.Headers["Referer"].ToString());
            }

            //delete video 
            if (!string.IsNullOrEmpty(response.Model.Filename))
            {
                //file
                string deletePath = Path.Combine(_hostingEnvironment.WebRootPath, _configuration["Site:VideosPath"], "Video");
                var responseVideo = _fileService(EnumFileType.Video).Delete(deletePath, response.Model.CourseId.ToString(), response.Model.Filename);
                if (responseVideo.IsSuccessfull)
                {
                    //delete entity Video
                    var response2 = _entityService.DeleteById<Video>(response.Model.Id);
                    if (response2.IsSuccessfull)
                    {
                        TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Success, "Video deleted successfully"));
                        _logger.LogInformation("Video Delete result.IsSuccessfull");
                    }
                    else
                    {
                        _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - _fileService.Delete - " + response2.ErrorForLog}");
                        TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, "Error on video deleting. Please try again later"));
                    }

                    //delete snapshot
                    string imagePathSnapshot = Path.Combine(_hostingEnvironment.WebRootPath, _configuration["Site:ImagesPath"], "Snapshot", "Video");
                    var responseSnapshot = _fileService(EnumFileType.Image).Delete(imagePathSnapshot, response.Model.Id.ToString(), response.Model.Snapshot);
                    if (responseSnapshot.IsSuccessfull)
                    {
                        //delete entity Image
                        var response3 = _entityService.DeleteBy<Image>("RelatedObjectId", response.Model.Id);
                        if (response3.IsSuccessfull)
                        {
                            _logger.LogInformation("Video image Delete result.IsSuccessfull");
                        }
                        else
                        {
                            _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - Image _fileService.DeleteBy - " + response3.ErrorForLog}");
                        }
                    }
                    else
                    {
                        _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - Image _fileService.DeleteBy - " + responseSnapshot.ErrorForLog}");
                    }
                }
                else
                {
                    _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - _fileService.Delete - " + response.ErrorForLog}");
                    TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, "Error on video deleting. Please try again later"));
                }
            }

            return RedirectToAction("View", "Course", new { id = response.Model.CourseId });
        }
    }
}