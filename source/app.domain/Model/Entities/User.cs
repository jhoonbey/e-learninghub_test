using System;

namespace app.domain.Model.Entities
{
    public class User : EntityBaseModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Position { get; set; }
        public string Workplace { get; set; }
        public string CategoryIdSet { get; set; }
        public int Role { get; set; }
        public string SerialNumber { get; set; }
        public string Imagename { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string ConfirmationGuid { get; set; }
    }
}