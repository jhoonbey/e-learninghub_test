using app.domain.Model.View;
using System.Collections.Generic;

namespace app.domain.Model.Collections.View
{
    public class ViewCollection<T> : BaseCollectionModel where T : ViewBaseModel
    {
        public ViewCollection()
        {
            Items = new List<T>();
        }
        public List<T> Items { get; set; }
    }
}
