namespace app.domain.Model.View
{
    public class LeftJoinModel : ViewBaseModel
    {
        public string Alias { get; set; }
        public string HelperEntityName { get; set; }
        public string JoinMainColumn { get; set; }
        public string JoinHelperColumn { get; set; }
        public string TakeHelperColumn { get; set; }
        public string AsResultColumn { get; set; }
    }
}
