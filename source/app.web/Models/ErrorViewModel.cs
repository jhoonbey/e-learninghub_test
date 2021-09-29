using System;

namespace app.web.Models
{
    //public class ErrorViewModel
    //{
    //    public string RequestId { get; set; }

    //    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    //    public string Message { get; set; }
    //}

    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }

}