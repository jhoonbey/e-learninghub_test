using System.Collections.Generic;
using System.Security.Claims;

namespace app.service
{
    public interface IAntiForgeryCookieService
    {
        void RegenerateAntiForgeryCookies(IEnumerable<Claim> claims);
        void DeleteAntiForgeryCookies();
    }
}
