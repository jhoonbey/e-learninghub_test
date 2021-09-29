using System;
using System.Collections.Generic;
using System.Text;

namespace app.service
{
    public interface ISecurityService
    {
        string GetSha256Hash(string input);
        Guid CreateCryptographicallySecureGuid();
    }
}
