using app.data;
using app.domain.Enums;
using app.domain.Exceptions;
using app.domain.Model.Criterias;
using app.domain.Model.Entities;
using app.domain.Model.View;
using app.service.Model.Response;
using app.service.Validations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace app.service
{
    public class CommonService : ICommonService
    {
        private readonly IEntityRepository _entityRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountService> _logger;

        public CommonService(
            IEntityRepository entityRepository,
            IConfiguration configuration,
            IEmailService emailService,
            ILogger<AccountService> logger
            )
        {
            _entityRepository = entityRepository;
            _configuration = configuration;
            _emailService = emailService;
            _logger = logger;
        }

        public BoolServiceResponse SendContactForm(ContactViewModel model)
        {
            var response = new BoolServiceResponse();
            try
            {
                Validator.ContactForm(model);

                Mail mail = new Mail
                {
                    FromMail = _configuration["MailConfig:FromMailNoreply"].ToString(),
                    FromDisplayName = "From Site",
                    ToMail = _configuration["MailConfig:FromMail"].ToString(),
                    Body = "Sender: " + model.Name + "  [ " + model.Email + " ]  " + "  [ " + model.Phone + " ]  " + string.Format(" <br /> <br />") + model.Comment,
                    Subject = "This mail has sent from contact page of site",
                    Purpose = (int)EnumEmailPurpose.FromSite
                };

                int mailId = _entityRepository.Create<Mail>(mail, "", "", false);
                if (mailId > 0) // mail save is ok
                {
                    mail.Id = mailId;

                    //send mail
                    _emailService.Send(new List<Mail> { mail }, true);
                }

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

        public GenericServiceResponse<HomeViewModel> GetHomeViewModel()
        {
            var response = new GenericServiceResponse<HomeViewModel>();
            try
            {
                response.Model = new HomeViewModel();

                response.Model.AboutInfo = _entityRepository.GetEntityBy<Option>(new Dictionary<string, object> { { "Sec", "About_Info" } });
                response.Model.Skills_Description = _entityRepository.GetEntityBy<Option>(new Dictionary<string, object> { { "Sec", "Skills_Description" } });
                response.Model.Skills = _entityRepository.LoadEntitiesByCriteria<Skill>(new BaseCriteriaModel { PageNumber = 1, RowsPerPage = 50 }).Items;

                response.Model.PopularCourses = _entityRepository.LoadEntitiesByCriteria<Course>(
                    new BaseCriteriaModel
                    {
                        Top = 4,
                        IntCriteria = (int)EnumUserRole.Instructor,
                        OrderByModels = new List<OrderByModel>
                        {
                             new OrderByModel { ColumnName = "ViewCount", OrderBy = EnumOrderBy.DESC}
                        }
                    }).Items;

                response.Model.Instructors = _entityRepository.LoadEntitiesByCriteria<User>(
                  new BaseCriteriaModel
                  {
                      Top = 4,
                      OrderByModels = new List<OrderByModel>
                      {
                             new OrderByModel { ColumnName = "CreateDate", OrderBy = EnumOrderBy.DESC}
                      }
                  }).Items;

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
