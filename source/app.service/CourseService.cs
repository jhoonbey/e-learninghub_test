using app.data;
using app.domain.Enums;
using app.domain.Exceptions;
using app.domain.Model.Criterias;
using app.domain.Model.Entities;
using app.domain.Model.View;
using app.service.Model.Mail;
using app.service.Model.Response;
using app.service.Validations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace app.service
{
    public class CourseService : ICourseService
    {
        private readonly IEntityRepository _entityRepository;
        private readonly IOptionsSnapshot<MailConfigOptions> _emailConfiguration;
        private readonly IEmailService _emailService;
        //private readonly ILogger<AccountService> _logger;

        public CourseService(ISecurityService securityService,
            IEntityRepository entityRepository,
            IPromoCodeRepository promoCodeRepository,
            IOptionsSnapshot<MailConfigOptions> emailConfiguration,
            IConfiguration configuration,
            IEmailService emailService,
            ILogger<AccountService> logger,
            ICipherService cipherService
            )
        {
            _entityRepository = entityRepository;
            _emailConfiguration = emailConfiguration;
            _emailService = emailService;
            //_logger = logger;
        }

        public GenericServiceResponse<Course> Create(CourseCreateViewModel viewModel, User currrentUser)
        {
            var response = new GenericServiceResponse<Course>();
            try
            {
                Validator.CourseCreate(viewModel);

                //db validations
                //draft
                var draftCourses = _entityRepository.LoadEntitiesByCriteria<Course>(new BaseCriteriaModel
                {
                    IntCriteria = currrentUser.Id,
                    IntCriteria2 = (int)EnumCourseStatus.Draft,
                    RowsPerPage = 4,
                    PageNumber = 1
                });
                if (draftCourses != null && draftCourses.Items.Count > 3)
                {
                    throw new BusinessException("You have a 3 draft courses. You can not add a new course");
                }

                //Sent
                var sentCourses = _entityRepository.LoadEntitiesByCriteria<Course>(new BaseCriteriaModel
                {
                    IntCriteria = currrentUser.Id,
                    IntCriteria2 = (int)EnumCourseStatus.Sent,
                    RowsPerPage = 4,
                    PageNumber = 1
                });
                if (sentCourses != null && sentCourses.Items.Count > 3)
                {
                    throw new BusinessException("You have a 3 unapproved courses. You can not add a new course");
                }

                Course model = AutoMapper.Mapper.Map<Course>(viewModel);

                //default values
                model.UserId = currrentUser.Id;
                model.Status = (int)EnumCourseStatus.Draft;
                model.CategoryId = viewModel.CategoryId;
                model.SubCategoryId = viewModel.SubCategoryId;
                model.ViewCount = 0;

                int entityId = _entityRepository.Create<Course>(model, "", "", false);

                if (entityId > 0) //entity save is ok
                {
                    model.Id = entityId;

                    //create and save mail
                    //Mail mail = new Mail
                    //{
                    //    FromMail = _emailConfiguration.Value.FromMail,
                    //    FromDisplayName = _emailConfiguration.Value.FromDisplayName,
                    //    ToMail = currrentUser.Email,
                    //    //Body = _emailService.GenerateConfirmationMailBody(model),
                    //    //Subject = Lang.EmailConfirmationMailSubject,
                    //    Purpose = (int)EnumEmailPurpose.Confirm
                    //};

                    //int mailId = _entityRepository.Create<Mail>(mail, "", "", false);
                    //if (mailId > 0) // mail save is ok
                    //{
                    //    mail.Id = mailId;

                    //    //send mail
                    //    _emailService.Send(new List<Mail> { mail }, true);
                    //}
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

        public GenericServiceResponse<CourseViewModel> GetViewModel(int id)
        {
            var response = new GenericServiceResponse<CourseViewModel>();
            try
            {
                response.Model = new CourseViewModel();

                response.Model.Course = _entityRepository.GetEntityById<Course>(id);
                if (response.Model.Course != null)
                {
                    response.Model.Videos = _entityRepository.LoadEntitiesByCriteria<Video>(new BaseCriteriaModel { IntCriteria = response.Model.Course.Id, RowsPerPage = 100, PageNumber = 1 }).Items;
                    response.Model.Sections = _entityRepository.LoadEntitiesByCriteria<Section>(new BaseCriteriaModel { IntCriteria = response.Model.Course.Id, RowsPerPage = 50, PageNumber = 1 }).Items;
                }
                else
                    throw new BusinessException("Course not found");

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



        public GenericServiceResponse<Course> SendApproval(int id, User currrentUser)
        {
            var response = new GenericServiceResponse<Course>();
            try
            {
                Validator.CourseSendApproval(id);

                //db validations
                Course course = _entityRepository.GetEntityBy<Course>(new Dictionary<string, object>
                {
                    { "Id", id },
                    { "UserId", currrentUser.Id },
                    { "Status", (int)EnumCourseStatus.Draft }
                });
                if (course == null)
                {
                    throw new BusinessException("You can not send this course for approval");
                }

                //Sent
                var sentCourses = _entityRepository.LoadEntitiesByCriteria<Course>(new BaseCriteriaModel
                {
                    IntCriteria = currrentUser.Id,
                    IntCriteria2 = (int)EnumCourseStatus.Sent,
                    RowsPerPage = 4,
                    PageNumber = 1
                });
                if (sentCourses != null && sentCourses.Items.Count > 3)
                {
                    throw new BusinessException("You have a 3 unapproved courses. You can not add a new course");
                }

                //Course courseSent = _entityRepository.GetEntityBy<Course>(new Dictionary<string, object>
                //{
                //    { "UserId", currrentUser.Id },
                //    { "Status", (int)EnumCourseStatus.Sent }
                //});
                //if (courseSent != null)
                //{
                //    throw new BusinessException("You have one unapproved Course. You can not send another");
                //}

                //update user entity
                _entityRepository.UpdateBy<Course>(new Dictionary<string, object>
                {
                    { "Status", (int)EnumCourseStatus.Sent },
                    { "RejectReason", string.Empty }
                }, "Id", id);

                response.Model = course;
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
