using app.domain.Model.Entities;
using System.Collections.Generic;

namespace app.domain.Model.View
{
    public class CategoryMenuViewModel : ViewBaseModel
    {
        public List<Category> Categories { get; set; }
        public List<SubCategory> SubCategories { get; set; }
        public List<SubOfSubCategory> SubOfSubCategories { get; set; }
    }
}
