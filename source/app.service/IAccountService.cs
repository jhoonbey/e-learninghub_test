using app.domain.Model.Entities;
using app.domain.Model.View;
using app.service.Model.Response;
using Microsoft.AspNetCore.Http;

namespace app.service
{
    public interface IAccountService
    {
        GenericServiceResponse<User> Create(StartViewModel viewModel);
        BoolServiceResponse UpdateProfile(UserDataViewModel model, User currentUser);
        BoolServiceResponse BecomeInstructor(BecomeInstructorViewModel model, User currrentUser);
        BoolServiceResponse ChangePassword(PasswordChangeModel model, int id);
        GenericServiceResponse<User> ResetRequest(StartViewModel viewModel);
        BoolServiceResponse ConfirmEmail(string code);
        BoolServiceResponse ValidateResetLink(string code);
        GenericServiceResponse<User> ConfirmSuccess(string code);
        BoolServiceResponse ResetPassword(PasswordResetModel model);
        GenericServiceResponse<User> Find(StartViewModel viewModel);
        GenericServiceResponse<User> UpdateImage(IFormFile file, User user, string pathOnly);
        VoidServiceResponse DeleteImage(int id, string pathOnly);
    }
}