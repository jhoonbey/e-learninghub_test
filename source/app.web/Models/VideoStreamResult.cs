using app.web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace app.web.Models
{
    public class VideoStreamResult : FileStreamResult //, IDisposable
    {
        private const int BufferSize = 256 * 1024;

        public VideoStreamResult(Stream fileStream, string contentType) : base(fileStream, contentType) { }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;

            var bufferingFeature = response.HttpContext.Features.Get<IHttpBufferingFeature>();
            bufferingFeature?.DisableResponseBuffering();

            var length = FileStream.Length;
            var range = response.HttpContext.GetRanges(length);
            response.ContentType = ContentType;
            response.Headers.Add("Accept-Ranges", "bytes");
            if (IsRangeRequest(range))
            {
                response.StatusCode = (int)HttpStatusCode.PartialContent;
                response.Headers.Add("Content-Range", $"bytes {range.Ranges.First().From}-{range.Ranges.First().To}/{length}");

                await WriteDataToResponseBody(range.Ranges.First(), response);
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.PartialContent;
                // await FileStream.CopyToAsync(response.Body);
            }
        }

        private async Task WriteDataToResponseBody(RangeItemHeaderValue rangeValue, HttpResponse response)
        {
            var startIndex = rangeValue.From ?? 0;
            var endIndex = FileStream.Length;

            var buffer = new byte[BufferSize];
            var totalToSend = endIndex - startIndex;

            var bytesRemaining = totalToSend + 1;
            response.ContentLength = bytesRemaining;

            FileStream.Seek(startIndex, SeekOrigin.Begin);

            while (bytesRemaining > 0)
            {
                try
                {
                    int count;
                    count = bytesRemaining <= buffer.Length
                        ? FileStream.Read(buffer, 0, (int)bytesRemaining)
                        : FileStream.Read(buffer, 0, buffer.Length);

                    if (count == 0)
                        return;

                    await response.Body.WriteAsync(buffer, 0, count);

                    bytesRemaining -= count;
                }
                catch (IndexOutOfRangeException)
                {
                    await response.Body.FlushAsync();
                    return;
                }
                finally
                {
                    await response.Body.FlushAsync();
                }
            }
        }

        private bool IsRangeRequest(RangeHeaderValue range)
        {
            return range?.Ranges != null && range.Ranges.Count > 0;
        }

        //public void Dispose()
        //{
        //    if (FileStream != null)
        //    {
        //        try
        //        {
        //            FileStream.Dispose();
        //        }
        //        catch (Exception)
        //        {
        //        }
        //    }
        //}

        //public void DisposeFileStream()
        //{
            //if (FileStream != null)
            //{
            //    try
            //    {
            //        FileStream.Dispose();
            //    }
            //    catch (Exception)
            //    {
            //    }
            //}  
        //}
    }
}
