using app.domain.Model.View;
using app.service.Model.Response;

namespace app.service
{
    public interface ICommonService
    {
        BoolServiceResponse SendContactForm(ContactViewModel viewModel);
        GenericServiceResponse<HomeViewModel> GetHomeViewModel();
    }
}