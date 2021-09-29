using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace app.web.client.Areas.Addmein.Models
{
    public class PasswordChangeModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordAgain { get; set; }
    }
}