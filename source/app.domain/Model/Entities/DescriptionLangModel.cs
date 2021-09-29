namespace app.domain.Model.Entities
{
    public class DescriptionLangModel : NameLangModel
    {
        public string DescriptionAZ { get; set; }
        public string DescriptionEN { get; set; }
        public string DescriptionRU { get; set; }

        public string GetDescription(string lang)
        {
            if (string.IsNullOrEmpty(lang))
                return string.Empty;
            switch (lang)
            {
                case "az": return this.DescriptionAZ;
                case "en": return this.DescriptionEN;
                case "ru": return this.DescriptionRU;
                default: return string.Empty;
            }
        }
    }
}
