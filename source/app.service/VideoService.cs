using app.data;
using app.domain.Enums;
using app.domain.Exceptions;
using app.domain.Model.Entities;
using app.domain.Model.View;
using app.domain.Utilities;
using app.service.Model.Response;
using app.service.Validations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace app.service
{
    public class VideoService : IVideoService
    {
        private readonly IEntityRepository _entityRepository;
        private readonly ILogger<VideoService> _logger;
        private readonly IConfiguration _configuration;
        private readonly Func<EnumFileType, IFileService> _fileService;

        public VideoService(
            ISecurityService securityService,
            IEntityRepository entityRepository,
            IConfiguration configuration,
            ILogger<VideoService> logger,
            Func<EnumFileType, IFileService> fileService
            )
        {
            _entityRepository = entityRepository;
            _configuration = configuration;
            _logger = logger;
            _fileService = fileService;
        }

        public GenericServiceResponse<Video> Create(VideoUploadModel viewModel, User currrentUser)
        {
            var response = new GenericServiceResponse<Video>();
            try
            {
                Validator.VideoCreate(viewModel);

                //db validations
                Course course = _entityRepository.GetEntityById<Course>(viewModel.CourseId);
                if (course == null)
                {
                    throw new BusinessException("Video should be related to any Course");
                }
                if (course.UserId != currrentUser.Id)
                {
                    throw new BusinessException("You can add Video only to own Course");
                }
                Section section = _entityRepository.GetEntityById<Section>(viewModel.SectionId);
                if (section == null)
                {
                    throw new BusinessException("Section not found");
                }
                if (section.CourseId != course.Id)
                {
                    throw new BusinessException("Section is not related to your own Course");
                }

                Video model = AutoMapper.Mapper.Map<Video>(viewModel);

                //default values
                model.CourseId = course.Id;
                model.SectionId = section.Id;
                model.Extension = Path.GetExtension(viewModel.PostedFile.FileName);
                model.Filename = string.Empty;
                model.Snapshot = string.Empty;
                model.MediaType = viewModel.PostedFile.ContentType;

                int entityId = _entityRepository.Create<Video>(model, "", "", false);
                if (entityId > 0) //entity save is ok
                {
                    model.Id = entityId;

                    //save video
                    string pathVideo = Path.Combine(viewModel.RootPath, _configuration["Site:VideosPath"], "Video");
                    var responseVideo = _fileService(EnumFileType.Video).Save_Create(viewModel.PostedFile, entityId.ToString(), pathVideo, course.Id.ToString());

                    if (!responseVideo.IsSuccessfull)
                    {
                        //delete entity
                        _entityRepository.DeleteById<Video>(entityId);

                        throw new Exception("Error on video saving");
                    }
                    else
                    {
                        //Update entity
                        model.Filename = responseVideo.Model;
                        _entityRepository.UpdateBy<Video>(new Dictionary<string, object> { { "Filename", model.Filename } }, "Id", model.Id);

                        //save snapshot
                        model.Snapshot = Common.CRP(12) + ".png";
                        Image image = new Image
                        {
                            Name = model.Snapshot,
                            Sector = "Snapshot",
                            RelatedObjectId = entityId //videoid
                        };

                        int image_entityId = _entityRepository.Create<Image>(image, "", "", false);
                        if (image_entityId > 0) //entity save is ok
                        {
                            string pathSnapshot = Path.Combine(viewModel.RootPath, _configuration["Site:ImagesPath"], Path.Combine("Snapshot", "Video", entityId.ToString(), "Original"));
                            string pathVideoFull = Path.Combine(pathVideo, course.Id.ToString(), "Original", model.Filename);

                            string ffmpegPath = Path.Combine(_configuration["Site:FFmpegPath"], "ffmpeg.exe");

                            var responseSnapshot = _fileService(EnumFileType.Image).ExtractImageFromVideo(ffmpegPath, pathVideoFull, pathSnapshot, model.Snapshot, 0.1);
                            if (!responseSnapshot.IsSuccessfull)
                            {
                                _logger.LogInformation($"{ MethodBase.GetCurrentMethod().Name } - ErrorForLog - {responseSnapshot.ErrorForLog}");
                                throw new BusinessException("Error on extraction of snapshot: " + responseSnapshot.ErrorForClient);
                            }
                        }

                        //Update entity
                        _entityRepository.UpdateBy<Video>(new Dictionary<string, object> { { "Snapshot", model.Snapshot } }, "Id", model.Id);
                    }
                }
                else
                {
                    throw new BusinessException("Error on course creation");
                }

                response.Model = model;
                response.IsSuccessfull = true;
            }
            catch (BusinessException exp)
            {
                response.LoadFrom(exp);
            }
            catch (Exception exp)
            {
                response.ErrorMessage = exp.ToString();
            }
            return response;
        }

        public GenericServiceResponse<Video> CheckAuthorization(int videoId, int courseId, User currrentUser)
        {
            var response = new GenericServiceResponse<Video>();
            try
            {
                Validator.VideoCheckAuthorization(videoId, courseId);

                //db validations
                Video video = _entityRepository.GetEntityById<Video>(videoId);
                if (video == null)
                {
                    throw new BusinessException("Video not found");
                }

                Course course = _entityRepository.GetEntityById<Course>(courseId);
                if (course == null)
                {
                    throw new BusinessException("Course not found");
                }

                if (currrentUser.Role < (int)EnumUserRole.Admin)
                {
                    if (course.UserId != currrentUser.Id)
                    {
                        throw new BusinessException("You can not watch this Video");
                    }
                }

                response.Model = video;
                response.IsSuccessfull = true;
            }
            catch (BusinessException exp)
            {
                response.LoadFrom(exp);
            }
            catch (Exception exp)
            {
                response.ErrorMessage = exp.ToString();
            }
            return response;
        }

        public GenericServiceResponse<Course> CheckAuthorization(int courseId)
        {
            var response = new GenericServiceResponse<Course>();
            try
            {
                Validator.VideoCheckAuthorization(courseId);

                Course course = _entityRepository.GetEntityById<Course>(courseId);
                response.Model = course ?? throw new BusinessException("Course not found");
                response.IsSuccessfull = true;
            }
            catch (BusinessException exp)
            {
                response.LoadFrom(exp);
            }
            catch (Exception exp)
            {
                response.ErrorMessage = exp.ToString();
            }
            return response;
        }

        public GenericServiceResponse<Video> DeleteRequest(int videoId, User currrentUser)
        {
            var response = new GenericServiceResponse<Video>();
            try
            {
                //db validations
                Video video = _entityRepository.GetEntityById<Video>(videoId);
                if (video == null)
                {
                    throw new BusinessException("Video not found");
                }
                Course course = _entityRepository.GetEntityById<Course>(video.CourseId);
                if (course == null)
                {
                    throw new BusinessException("Course not found");
                }
                if (course.UserId != currrentUser.Id)
                {
                    throw new BusinessException("You can only delete your own videos");
                }
                if (course.Status != (int)EnumCourseStatus.Draft)
                {
                    throw new BusinessException("You can delete your own videos only in Draft mode");
                }

                response.Model = video;
                response.IsSuccessfull = true;
            }
            catch (BusinessException exp)
            {
                response.LoadFrom(exp);
            }
            catch (Exception exp)
            {
                response.ErrorMessage = exp.ToString();
            }
            return response;
        }
    }
}
