namespace app.domain.Model.Entities
{
    public class Deed : EntityBaseModel
    {
        public int Type { get; set; }
        public string DeedKey { get; set; }
        public string DeedValue { get; set; }
        public bool Done { get; set; }
        public int UserId { get; set; }
    }
}

