using app.data;
using app.domain.Enums;
using app.domain.Exceptions;
using app.domain.Languages;
using app.domain.Model.Entities;
using app.domain.Model.View;
using app.domain.Static;
using app.domain.Utilities;
using app.service.Model.Mail;
using app.service.Model.Response;
using app.service.Validations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace app.service
{
    public class AccountService : IAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly ISecurityService _securityService;
        private readonly IEntityRepository _entityRepository;
        private readonly IPromoCodeRepository _promoCodeRepository;
        private readonly IOptionsSnapshot<MailConfigOptions> _emailConfiguration;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountService> _logger;
        private readonly ICipherService _cipherService;
        private readonly IFileService _fileService;

        public AccountService(
            ISecurityService securityService,
            IEntityRepository entityRepository,
            IPromoCodeRepository promoCodeRepository,
            IOptionsSnapshot<MailConfigOptions> emailConfiguration,
            IConfiguration configuration,
            IEmailService emailService,
            ILogger<AccountService> logger,
            ICipherService cipherService,
            Func<EnumFileType, IFileService> serviceResolver
            )
        {
            _securityService = securityService;
            _entityRepository = entityRepository;
            _promoCodeRepository = promoCodeRepository;
            _emailConfiguration = emailConfiguration;
            _configuration = configuration;
            _emailService = emailService;
            _logger = logger;
            _cipherService = cipherService;
            _fileService = serviceResolver(EnumFileType.Image);

        }

        public GenericServiceResponse<User> Create(StartViewModel viewModel)
        {
            var response = new GenericServiceResponse<User>();
            try
            {
                Validator.UserCreate(viewModel);

                //db calidations
                //db validations
                //User user = _entityRepository.GetEntityBy<User>(new Dictionary<string, object>
                //{
                //    { "Email", viewModel.Email }
                //});
                //if (user == null)
                //{
                //    throw new BusinessException("This email is busy");
                //}

                //1. map and save user
                User model = AutoMapper.Mapper.Map<User>(viewModel);

                //default values
                model.ConfirmationGuid = Guid.NewGuid().ToString();
                model.Password = _securityService.GetSha256Hash(viewModel.ConfirmPassword);
                model.SerialNumber = Guid.NewGuid().ToString("N");
                model.CategoryIdSet = viewModel.Rolename == "instructor" ? viewModel.CategoryId.ToString() : null;

                if (viewModel.Rolename == "instructor")
                    model.Role = (int)EnumUserRole.Instructor;
                else
                    model.Role = (int)EnumUserRole.Learner;

                int userId = _entityRepository.Create<User>(model, "Email", model.Email, true);
                model.Password = null;

                //test ********************************************
                //_emailService.SendMailTest(Lang.EmailConfirmationMailSubject, _emailService.GenerateConfirmationMailBody(model));
                //test ********************************************

                if (userId > 0) //user save is ok
                {
                    model.Id = userId;

                    //create and save mail
                    Mail mail = new Mail
                    {
                        FromMail = _emailConfiguration.Value.FromMail,
                        FromDisplayName = _emailConfiguration.Value.FromDisplayName,
                        ToMail = viewModel.Email,
                        Body = _emailService.GenerateConfirmationMailBody(model),
                        Subject = Lang.EmailConfirmationMailSubject,
                        Purpose = (int)EnumEmailPurpose.Confirm
                    };

                    int mailId = _entityRepository.Create<Mail>(mail, "", "", false);
                    if (mailId > 0) // mail save is ok
                    {
                        mail.Id = mailId;

                        //send mail
                        _emailService.Send(new List<Mail> { mail }, true);
                    }
                }
                else
                {
                    throw new BusinessException("Error on account creation");
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

        public BoolServiceResponse UpdateProfile(UserDataViewModel model, User currentUser)
        {
            var response = new BoolServiceResponse();
            try
            {
                Validator.UpdateProfile(model, currentUser);

                //update user entity
                if (currentUser.Role != (int)EnumUserRole.Learner)
                {
                    _entityRepository.UpdateBy<User>(new Dictionary<string, object>
                    {
                        { "Name", model.Name },
                        { "Surname", model.Surname },
                        { "Position", model.Position },
                        { "Workplace", model.Workplace },
                        { "CategoryIdSet", Common.ConvertIdSetToString(model.CategoryIdSet)}
                    }, "Id", model.UserId);
                }
                else
                {
                    _entityRepository.UpdateBy<User>(new Dictionary<string, object>
                    {
                        { "Name", model.Name },
                        { "Surname", model.Surname },
                        { "Position", model.Position },
                        { "Workplace", model.Workplace }
                    }, "Id", model.UserId);
                }


                //update user other data
                var userData = _entityRepository.GetEntityBy<UserData>(new Dictionary<string, object> { { "UserId", model.UserId } });
                if (userData == null) //no user data, create one
                {
                    userData = new UserData
                    {
                        UserId = model.UserId,
                        Info = model.Info,
                        Address = model.Address,
                        University = model.University,
                        FbLink = model.FbLink,
                        LinkedinLink = model.LinkedinLink,
                        TwitterLink = model.TwitterLink,
                        InterestedCategoryIdSet = Common.ConvertIdSetToString(model.InterestedCategoryIdSet)
                    };

                    var userDataId = _entityRepository.Create<UserData>(userData, "", "", false);
                }
                else //update user data
                {
                    _entityRepository.UpdateBy<UserData>(new Dictionary<string, object>
                    {
                        { "Info", model.Info },
                        { "Address", model.Address },
                        { "University", model.University },
                        { "FbLink", model.FbLink },
                        { "LinkedinLink", model.LinkedinLink },
                        { "TwitterLink", model.TwitterLink },
                        { "InterestedCategoryIdSet", Common.ConvertIdSetToString(model.InterestedCategoryIdSet) }
                     }, "UserId", model.UserId);
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

        public BoolServiceResponse BecomeInstructor(BecomeInstructorViewModel model, User currrentUser)
        {
            var response = new BoolServiceResponse();
            try
            {
                if (currrentUser.Role != (int)EnumUserRole.Learner)
                {
                    throw new BusinessException("You can not be an Instructor");
                }
                if (model == null || model.CategoryIdSet == null || model.CategoryIdSet.Count < 1)
                {
                    throw new BusinessException("Select minimum 1 category");
                }

                _entityRepository.UpdateBy<User>(new Dictionary<string, object>
                {
                    { "CategoryIdSet", string.Join(",", model.CategoryIdSet)},
                    { "Role", (int)EnumUserRole.Instructor }
                }, "Id", currrentUser.Id);

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

        public BoolServiceResponse ChangePassword(PasswordChangeModel model, int id)
        {
            var response = new BoolServiceResponse();
            try
            {
                Validator.ChangePassword(model);

                User user = _entityRepository.GetEntityBy<User>(new Dictionary<string, object>
                {
                    { "Id", id },
                    { "Password", _securityService.GetSha256Hash(model.OldPassword) }
                });
                if (user == null)
                {
                    throw new BusinessException("Old password is incorrect");
                }

                _entityRepository.UpdateBy<User>(new Dictionary<string, object>
                {
                    { "Password", _securityService.GetSha256Hash(model.NewPassword) }
                }, "Id", id);

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

        public BoolServiceResponse ConfirmEmail(string code)
        {
            var response = new BoolServiceResponse();
            try
            {
                Validator.ConfirmEmailCode(code);

                User model = _entityRepository.GetEntityBy<User>(new Dictionary<string, object> { { "ConfirmationGuid", code } });
                if (model == null)
                {
                    throw new BusinessException("Confirmation code is incorrect");
                }
                model.Password = null;

                if (model.IsEmailConfirmed)
                {
                    throw new BusinessException("Mail is already confirmed");
                }

                //update user
                Dictionary<string, object> columns = new Dictionary<string, object>
                {
                    { "IsEmailConfirmed", true}
                };

                _entityRepository.UpdateBy<User>(columns, "Id", model.Id);

                response.Model = true;
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

        public GenericServiceResponse<User> ConfirmSuccess(string code)
        {
            var response = new GenericServiceResponse<User>();
            try
            {
                Validator.ConfirmEmailCode(code);

                //get user
                User model = _entityRepository.GetEntityBy<User>(new Dictionary<string, object> { { "ConfirmationGuid", code } });
                if (model == null)
                {
                    throw new BusinessException("Confirmation code is incorrect");
                }
                model.Password = null;

                //get email
                Dictionary<string, object> columns = new Dictionary<string, object>
                {
                    { "ToMail", model.Email},
                    { "Purpose", (int)EnumEmailPurpose.Registered}
                };
                Mail mail = _entityRepository.GetEntityBy<Mail>(columns);
                if (mail != null)
                {
                    throw new BusinessException("Registration mail is already sent");
                }

                //create and save, send mail

                string pCode = _promoCodeRepository.Generate();

                //test ********************************************
                //_emailService.SendMailTest(Lang.EmailRegisteredMailSubject, _emailService.GenerateRegisteredMailBody(model, pCode));
                //test ********************************************

                mail = new Mail
                {
                    FromMail = _emailConfiguration.Value.FromMail,
                    FromDisplayName = _emailConfiguration.Value.FromDisplayName,
                    ToMail = model.Email,
                    Body = _emailService.GenerateRegisteredMailBody(model, pCode),
                    Subject = Lang.EmailRegisteredMailSubject,
                    Purpose = (int)EnumEmailPurpose.Registered
                };

                int mailId = _entityRepository.Create<Mail>(mail, "", "", false);
                if (mailId > 0) // mail save is ok
                {
                    mail.Id = mailId;

                    //generate and save promo code
                    PromoCode promoCode = new PromoCode
                    {
                        UserId = model.Id,
                        Code = pCode,
                        WhereUsed = (int)EnumPromoCodeWhereUsed.Registered,
                        IsUsed = false,
                        MailId = mail.Id
                    };

                    int promoCodeId = _entityRepository.Create<PromoCode>(promoCode, "Code", promoCode.Code, false);
                    if (promoCodeId > 0) // promo save is ok
                    {
                        //send mail
                        _emailService.Send(new List<Mail> { mail }, true);
                    }
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

        public GenericServiceResponse<User> ResetRequest(StartViewModel viewModel)
        {
            var response = new GenericServiceResponse<User>();
            try
            {
                Validator.UserReset(viewModel);

                //get user
                User model = _entityRepository.GetEntityBy<User>(new Dictionary<string, object> { { "Email", viewModel.ResetEmail } });
                if (model == null)
                {
                    throw new BusinessException(Lang.ErrorUserNotFound);
                }
                model.Password = null;

                //2. create and save mail
                string codeSecure = _cipherService.Encrypt(StaticValues.ResetKey, model.Id.ToString() + "W" + DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                Mail mail = new Mail
                {
                    FromMail = _emailConfiguration.Value.FromMail,
                    FromDisplayName = _emailConfiguration.Value.FromDisplayName,
                    ToMail = model.Email,
                    Body = _emailService.GenerateResetMailBody(model, codeSecure),
                    Subject = Lang.EmailResetMailSubject,
                    Purpose = (int)EnumEmailPurpose.Reset
                };

                int mailId = _entityRepository.Create<Mail>(mail, "", "", false);
                if (mailId > 0) // mail save is ok
                {
                    mail.Id = mailId;

                    //save Deed
                    Deed deed = new Deed
                    {
                        Type = (int)EnumDeedType.Reset,
                        DeedKey = model.Email,
                        DeedValue = codeSecure,
                        Done = false,
                        UserId = model.Id
                    };

                    int deedId = _entityRepository.Create<Deed>(deed, "", "", false);
                    if (deedId > 0) // deed save is ok
                    {
                        //send mail
                        _emailService.Send(new List<Mail> { mail }, true);

                    }
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

        public BoolServiceResponse ResetPassword(PasswordResetModel model)
        {
            var response = new BoolServiceResponse();
            try
            {
                Validator.ResetPassword(model);

                string decryptedCode = _cipherService.Decrypt(StaticValues.ResetKey, model.Code);
                string[] parts = decryptedCode.Split('W', 2);
                int id = Convert.ToInt32(parts[0]);

                //validate user
                User user = _entityRepository.GetEntityBy<User>(new Dictionary<string, object> { { "Id", id } });
                if (user == null)
                {
                    throw new BusinessException("Reset Password code is incorrect");
                }

                //validate deed
                Dictionary<string, object> columns = new Dictionary<string, object> { { "Type", (int)EnumDeedType.Reset }, { "DeedKey", user.Email }, { "DeedValue", model.Code }, { "UserId", user.Id } };
                Deed deed = _entityRepository.GetEntityBy<Deed>(columns, "desc");
                if (deed != null)
                {
                    if (DateTime.UtcNow.Subtract(deed.CreateDate).TotalHours > 24) //link is expired
                    {
                        throw new BusinessException("Reset link is expired");
                    }
                    if (deed.Done)
                    {
                        throw new BusinessException("Reset link is already used");  //link is used
                    }
                }
                else
                {
                    throw new BusinessException("Reset Password code is incorrect");
                }


                //update user
                string newPassword = _securityService.GetSha256Hash(model.Password);
                _entityRepository.UpdateBy<User>(new Dictionary<string, object> { { "Password", newPassword } }, "Id", user.Id);


                //update deed
                _entityRepository.UpdateBy<Deed>(new Dictionary<string, object> { { "Done", true } }, "Id", deed.Id);

                //// create and save mail
                //Mail mail = new Mail
                //{
                //    FromMail = _emailConfiguration.Value.FromMail,
                //    FromDisplayName = _emailConfiguration.Value.FromDisplayName,
                //    ToMail = user.Email,
                //    Body = _emailService.GenerateResetAlreadyMailBody(user),
                //    Subject = Lang.EmailRegisteredMailSubject,
                //    Purpose = (int)EnumEmailPurpose.Registered
                //};

                //int mailId = _entityRepository.Create<Mail>(mail, "", "", false);
                //????????????? mail gedeyeh

                response.Model = true;
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

        public GenericServiceResponse<User> Find(StartViewModel model)
        {
            var response = new GenericServiceResponse<User>();
            try
            {
                Validator.UserFind(model);

                var passwordHash = _securityService.GetSha256Hash(model.LoginPassword);
                response.Model = _entityRepository.GetEntityBy<User>(new Dictionary<string, object>
                {
                    { "Email", model.LoginEmail },
                    { "Password", passwordHash },
                });

                if (response.Model == null)
                {
                    throw new BusinessException(Lang.ErrorUserNotFound);
                }

                if (!response.Model.IsEmailConfirmed)
                {
                    throw new BusinessException(Lang.ErrorUserNotFound);
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

        public GenericServiceResponse<User> UpdateImage(IFormFile file, User model, string pathOnly)
        {
            var response = new GenericServiceResponse<User>();
            try
            {
                //??? get validation here
                if (file == null)
                {
                    throw new BusinessException("Select image");
                }

                var responseImage = _fileService.Save_Create(file, model.Id.ToString(), pathOnly, "Profile");
                if (!responseImage.IsSuccessfull)
                {
                    throw new BusinessException("Error on image saving");
                }

                model.Imagename = responseImage.Model;
                _entityRepository.UpdateBy<User>(new Dictionary<string, object> { { "Imagename", model.Imagename } }, "Id", model.Id);

                response.Model = model;
                if (response.Model == null)
                {
                    throw new BusinessException(Lang.ErrorUserNotFound);
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

        public VoidServiceResponse DeleteImage(int id, string pathOnly)
        {
            var response = new VoidServiceResponse();
            try
            {
                var user = _entityRepository.GetEntityById<User>(id);
                if (user == null)
                {
                    throw new BusinessException(Lang.ErrorUserNotFound);
                }

                //delete image
                var responseImage = _fileService.Delete(pathOnly, "Profile", user.Imagename);
                if (!responseImage.IsSuccessfull)
                {
                    throw new Exception("Error on image deleting");
                }

                //update entity
                _entityRepository.UpdateBy<User>(new Dictionary<string, object> { { "Imagename", null } }, "Id", user.Id);

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

        public BoolServiceResponse ValidateResetLink(string code)
        {
            var response = new BoolServiceResponse();
            try
            {
                if (string.IsNullOrEmpty(code)) throw new BusinessException("Reset Password code is incorrect");

                string decryptedCode = _cipherService.Decrypt(StaticValues.ResetKey, code);
                string[] parts = decryptedCode.Split('W', 2);
                int id = Convert.ToInt32(parts[0]);

                //validate user
                User user = _entityRepository.GetEntityBy<User>(new Dictionary<string, object> { { "Id", id } });
                if (user == null)
                {
                    throw new BusinessException("Reset Password code is incorrect");
                }

                //validate deed
                Dictionary<string, object> columns = new Dictionary<string, object> { { "Type", (int)EnumDeedType.Reset }, { "DeedKey", user.Email }, { "DeedValue", code }, { "UserId", user.Id } };
                Deed deed = _entityRepository.GetEntityBy<Deed>(columns, "desc");
                if (deed != null)
                {
                    if (DateTime.UtcNow.Subtract(deed.CreateDate).TotalHours > 24) //link is expired
                    {
                        throw new BusinessException("Reset link is expired");
                    }
                    if (deed.Done)
                    {
                        throw new BusinessException("Reset link is already used");  //link is used
                    }
                }
                else
                {
                    throw new BusinessException("Reset Password code is incorrect");
                }

                response.Model = true;
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
