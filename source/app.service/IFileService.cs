using app.service.Model.Response;
using Microsoft.AspNetCore.Http;

namespace app.service
{
    public interface IFileService
    {
        GenericServiceResponse<string> Save_Create(IFormFile attachedFile, string id, string pathOnly, string folderName);
        BoolServiceResponse Delete(string pathOnly, string folderName, string fileName);
        void ResizeToTarget(int size, string folderName, string pathOnly, string newFileName, string newFilePathOriginal);
        BoolServiceResponse ExtractImageFromVideo(string ffmpegPath, string videoFullPath, string imagePath, string imageName, double second);
    }
}
