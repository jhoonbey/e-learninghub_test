using app.domain.Model.Entities;
using app.service.Model.Response;
using System.Threading.Tasks;

namespace app.service
{
    public interface IMailService
    {
        GenericServiceResponse<Mail> Send(Mail model);
    }
}
