namespace app.domain.Model.Entities
{
    public class Image : EntityBaseModel
    {
        public string Name { get; set; }
        public string Sector { get; set; }
        public int? RelatedObjectId { get; set; }
    }
}






