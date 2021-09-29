using app.domain.Model.Entities;
using System.Collections.Generic;

namespace app.domain.Model.Collections.Entity
{
    public class StatusEntityCollection : BaseCollectionModel
    {
        public StatusEntityCollection()
        {
            Statuses = new List<Category>();
        }
        public List<Category> Statuses { get; set; }
    }
}
