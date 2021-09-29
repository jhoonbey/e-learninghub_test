using app.domain.Enums;
using System;

namespace app.domain.Model.Entities
{
    public class Mail : EntityBaseModel
    {
        public string FromMail { get; set; }
        public string FromDisplayName { get; set; }
        public string ToMail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool HasSent { get; set; }
        public DateTime? SentDate { get; set; }
        public string SentError { get; set; }
        public int Purpose { get; set; }
    }
}