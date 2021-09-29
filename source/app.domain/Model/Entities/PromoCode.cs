using System;

namespace app.domain.Model.Entities
{
    public class PromoCode : EntityBaseModel
    {
        public int UserId { get; set; }
        public string Code { get; set; }
        public bool IsUsed { get; set; }
        public int WhereUsed { get; set; }
        public int MailId { get; set; }
    }
}