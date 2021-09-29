using Microsoft.AspNetCore.Http;

namespace app.domain.Model.View
{
    public class VideoUploadModel : ViewBaseModel
    {
        public string Name { get; set; }
        public IFormFile PostedFile { get; set; }
        public int CourseId { get; set; }
        public int SectionId { get; set; }
        public string RootPath { get; set; }
    }
}
