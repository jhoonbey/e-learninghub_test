using app.domain.Model.Entities;
using app.service;
using app.web.Core;
using app.web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Reflection;

namespace app.web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoStreamController : BaseController
    {
        private readonly IVideoService _videoService;

        public VideoStreamController(ILogger<VideoStreamController> logger,
                            IVideoService videoService,
                            IAntiForgeryCookieService antiforgery,
                            ICipherService cipherService,
                            IEntityService entityService,
                            IConfiguration configuration,
                            IHostingEnvironment hostingEnvironment
                            ) : base(logger, configuration, hostingEnvironment, entityService, cipherService)
        {
            _videoService = videoService;
        }

        [HttpGet]
        [Route("[action]")]
        [User]
        public FileStreamResult GetVideo(int videoId, int courseId)
        {
            //get video
            var response = _videoService.CheckAuthorization(videoId, courseId, CurrentUser);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation($"{ MethodBase.GetCurrentMethod().Name } - " +
                    $"_videoService.CheckAuthorization result.IsSuccessfull. videoId = {videoId}  courseId = {courseId} ");
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                return new FileStreamResult(null, "");
            }

            //create path
            string pathFolder = Path.Combine(_hostingEnvironment.WebRootPath, _configuration["Site:VideosPath"], "Video");
            string path = Path.Combine(pathFolder, courseId.ToString(), "Original");
            string fullFilename = Path.Combine(path, response.Model.Filename);

            var fs = new FileStream(fullFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return new VideoStreamResult(fs, response.Model.MediaType);
        }

        [HttpGet]
        [Route("[action]")]
        public FileStreamResult GetPreviewVideo(int courseId)
        {
            //get video
            var response = _videoService.CheckAuthorization(courseId);
            if (response.IsSuccessfull)
            {
                _logger.LogInformation($"{ MethodBase.GetCurrentMethod().Name } - " +
                    $"_videoService.CheckAuthorization (courseid) result.IsSuccessfull. courseId = {courseId} ");
            }
            else
            {
                _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
                return new FileStreamResult(null, "");
            }

            //create path
            string pathFolder = Path.Combine(_hostingEnvironment.WebRootPath, _configuration["Site:VideosPath"], "Course", "Preview");
            string path = Path.Combine(pathFolder, courseId.ToString(), "Original");
            string fullFilename = Path.Combine(path, response.Model.Filename);

            var fs = new FileStream(fullFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return new VideoStreamResult(fs, response.Model.MediaType);
        }

    }
}
