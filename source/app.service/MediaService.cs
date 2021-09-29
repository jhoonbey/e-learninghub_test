using app.domain.Model.Entities;
using app.service.Model.Response;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using System;

namespace app.service
{
    public class MediaService : IMediaService
    {
        public GenericServiceResponse<Image> ExtractImageFromVideo(string videoPath)
        {
            throw new NotImplementedException();
        }
    }
}
