using app.domain.Model.Entities;
using app.service.Model.Response;

namespace app.service
{
    public interface IOptionService
    {
        GenericServiceResponse<Option> GetOrCreate(string sec);
    }
}