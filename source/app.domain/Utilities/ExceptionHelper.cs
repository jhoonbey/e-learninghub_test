using System;
using System.Collections.Generic;
using System.Text;

namespace app.domain.Utilities
{
    public static class ExceptionHelper
    {
        public static IEnumerable<string> GetExceptionFullMessage(Exception exception)
        {
            var e = exception;
            while (e != null)
            {
                yield return e.Message;
                e = e.InnerException;
            }
        }

        public static string GetExceptionFullMessageText(Exception exception)
        {
            StringBuilder result = new StringBuilder();
            var e = exception;
            while (e != null)
            {
                result.AppendLine(e.Message);
                e = e.InnerException;
            }
            return result.ToString();
        }
    }
}
