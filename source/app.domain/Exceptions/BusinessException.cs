using app.domain.Utilities;
using System;

namespace app.domain.Exceptions
{
    public class BusinessException : Exception  //, ICloneable
    {
        public BusinessException()
        {

        }
        public BusinessException(string message)
            : base(message)
        {
            Key = "errorKey";
        }

        public BusinessException(string key, string message) : base(message)
        {
            Key = key;
        }

        public string Key { get; set; }

        public override string ToString()
        {
            return ExceptionHelper.GetExceptionFullMessageText(this);
        }
    }
}
