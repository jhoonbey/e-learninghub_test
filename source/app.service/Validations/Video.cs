using app.domain.Languages;
using app.domain.Model.View;

namespace app.service.Validations
{
    public static partial class Validator
    {
        public static void VideoCreate(VideoUploadModel model)
        {
            ModelIsNull(model);

            ValidateText(model.Name, Lang.NameText, 1, 250, true);
        }

        public static void VideoCheckAuthorization(int videoId, int courseId)
        {
            ValidateIntPositiveIsIncorrect(videoId, Lang.VideoText);

            VideoCheckAuthorization(courseId);
        }

        public static void VideoCheckAuthorization(int courseId)
        {
            ValidateIntPositiveIsIncorrect(courseId, Lang.CourseText);
        }
    }
}
