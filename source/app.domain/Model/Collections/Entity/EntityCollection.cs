using app.domain.Model.Entities;
using System.Collections.Generic;

namespace app.domain.Model.Collections.Entity
{
    public class EntityCollection<T> : BaseCollectionModel //where T : EntityBaseModel
    {
        public EntityCollection()
        {
            Items = new List<T>();
        }
        public List<T> Items { get; set; }
    }
}
