namespace app.domain.Model.Entities
{
    public class UserData : EntityBaseModel
    {
        public int UserId { get; set; }

        public string Info { get; set; }
        public string Address { get; set; }
        public string University { get; set; }
        public string InterestedCategoryIdSet { get; set; }

        public string FbLink { get; set; }
        public string LinkedinLink { get; set; }
        public string TwitterLink { get; set; }
    }
}