namespace app.domain.Model.Entities
{
    public class SubOfSubCategory : NameLangModel
    {
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
    }
}