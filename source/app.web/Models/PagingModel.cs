namespace app.web.Models
{
    public class PagingModel
    {
        public string Action { get; set; }
        public string Controller { get; set; }
        public string Area { get; set; }

        public int NumberOfPages { get; set; }
        public int CurrentPage { get; set; }
    }

}
