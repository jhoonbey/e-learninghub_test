using app.domain.Model.Entities;
using app.service.Model.Response;

namespace app.service
{
    public interface ISectionService
    {
        GenericServiceResponse<Section> Create(int courseId, string name, User currrentUser);
    }
}