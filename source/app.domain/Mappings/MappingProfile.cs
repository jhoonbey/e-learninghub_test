using app.domain.Model.Entities;
using app.domain.Model.View;
using AutoMapper;

namespace app.domain.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<StartViewModel, User>();

            CreateMap<CourseCreateViewModel, Course>();

            CreateMap<VideoUploadModel, Video>();
        }
    }
}
