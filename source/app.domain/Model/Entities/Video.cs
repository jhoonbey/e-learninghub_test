namespace app.domain.Model.Entities
{
    public class Video : EntityBaseModel
    {
        public string Name { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }
        public string MediaType { get; set; }
        public int CourseId { get; set; }
        public int SectionId { get; set; }
        public string Snapshot { get; set; }
    }
}

