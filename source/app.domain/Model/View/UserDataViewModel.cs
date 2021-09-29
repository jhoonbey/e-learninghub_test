using System.Collections.Generic;

namespace app.domain.Model.View
{
    public class UserDataViewModel : ViewBaseModel
    {
        public int UserId { get; set; }
        public string Info { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Position { get; set; }
        public string Workplace { get; set; }
        public string Address { get; set; }
        public string University { get; set; }
        public List<int> CategoryIdSet { get; set; }
        public List<int> InterestedCategoryIdSet { get; set; }

        public string FbLink { get; set; }
        public string LinkedinLink { get; set; }
        public string TwitterLink { get; set; }
    }
}
