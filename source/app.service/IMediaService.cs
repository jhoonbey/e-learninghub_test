using app.domain.Model.Entities;
using app.service.Model.Response;

namespace app.service
{
    public interface IMediaService
    {
        GenericServiceResponse<Image> ExtractImageFromVideo(string videoPath);
    }
}