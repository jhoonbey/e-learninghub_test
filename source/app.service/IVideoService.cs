using app.domain.Model.Entities;
using app.domain.Model.View;
using app.service.Model.Response;

namespace app.service
{
    public interface IVideoService
    {
        GenericServiceResponse<Video> Create(VideoUploadModel viewModel, User currrentUser);
        GenericServiceResponse<Video> CheckAuthorization(int videoId, int courseId, User currrentUser);
        GenericServiceResponse<Course> CheckAuthorization(int courseId);
        GenericServiceResponse<Video> DeleteRequest(int videoId, User currrentUser);

        //BoolServiceResponse UpdateProfile(UserDataViewModel model);
        //BoolServiceResponse ChangePassword(PasswordChangeModel model, int id);
        //GenericServiceResponse<User> ResetRequest(StartViewModel viewModel);
        //BoolServiceResponse ConfirmEmail(string code);
        //BoolServiceResponse ValidateResetLink(string code);
        //GenericServiceResponse<User> ConfirmSuccess(string code);
        //BoolServiceResponse ResetPassword(PasswordResetModel model);
        //GenericServiceResponse<User> Find(StartViewModel viewModel);
        //GenericServiceResponse<User> UpdateImage(IFormFile file, User user, string pathOnly);
        //VoidServiceResponse DeleteImage(int id, string pathOnly);
    }
}