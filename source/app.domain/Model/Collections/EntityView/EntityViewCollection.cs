using app.domain.Model.Entities;
using System.Collections.Generic;

namespace app.domain.Model.Collections.Entity
{
    public class EntityViewCollection<T> : BaseCollectionModel where T : EntityBaseModel
    {
        public EntityViewCollection()
        {
            Items = new List<T>();
        }
        public List<T> Items { get; set; }
    }
}
