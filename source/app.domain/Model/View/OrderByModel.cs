using app.domain.Enums;

namespace app.domain.Model.View
{
    public class OrderByModel
    {
        public string ColumnName { get; set; }
        public EnumOrderBy OrderBy { get; set; }
    }
}
