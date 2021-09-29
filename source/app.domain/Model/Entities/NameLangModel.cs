namespace app.domain.Model.Entities
{
    public class NameLangModel : EntityBaseModel
    {
        public string NameAZ { get; set; }
        public string NameEN { get; set; }
        public string NameRU { get; set; }

        public string GetName(string lang)
        {
            if (string.IsNullOrEmpty(lang))
                return string.Empty;
            switch (lang)
            {
                case "az": return this.NameAZ;
                case "en": return this.NameEN;
                case "ru": return this.NameRU;
                default: return string.Empty;
            }
        }
    }
}
