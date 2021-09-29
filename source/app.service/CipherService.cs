using Microsoft.AspNetCore.DataProtection;

namespace app.service
{
    public class CipherService : ICipherService
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public CipherService(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        public string Encrypt(string key, string input)
        {
            var protector = _dataProtectionProvider.CreateProtector(key);
            return protector.Protect(input);
        }

        public string Decrypt(string key, string input)
        {
            var protector = _dataProtectionProvider.CreateProtector(key);
            return protector.Unprotect(input);
        }
    }
}
