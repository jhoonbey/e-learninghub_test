using app.domain;
using app.domain.Exceptions;
using app.domain.Languages;

namespace app.service.Model.Response
{
    public class ServiceResponseBase
    {
        private bool _isSuccessfull;
        private string _errorMessage;
        private bool _isBusinessError;
        private string _businessMessage;
        private string _key;

        public bool IsSuccessfull
        {
            get { return _isSuccessfull; }
            set { _isSuccessfull = value; }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        public bool IsBusinessError
        {
            get { return _isBusinessError; }
            set { _isBusinessError = value; }
        }

        public string BusinessMessage
        {
            get { return _businessMessage; }
            set { _businessMessage = value; }
        }

        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public string ErrorForClient
        {
            get
            {
                if (_isBusinessError) { return _businessMessage; } else { return Lang.FatalError; }
            }
            private set
            {
            }
        }

        public string ErrorForLog
        {
            get
            {
                if (_isBusinessError) { return _businessMessage; } else { return _errorMessage; }
            }
            private set
            {
            }
        }

        public void LoadFrom(BusinessException businessException)
        {
            _key = businessException.Key;
            _isBusinessError = true;
            _businessMessage = businessException.Message;
        }
    }
}
