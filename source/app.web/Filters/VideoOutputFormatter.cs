using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace app.web.Filters
{
    public class VideoOutputFormatter : IOutputFormatter
    {
        public bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.Object is PushStreamContent)
                return true;

            return false;
        }

        public async Task WriteAsync(OutputFormatterWriteContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            using (var stream = ((PushStreamContent)context.Object))
            {
                var response = context.HttpContext.Response;
                if (context.ContentType != null)
                {
                    response.ContentType = context.ContentType.ToString();
                }

                await stream.CopyToAsync(response.Body);
            }
        }
    }
}
