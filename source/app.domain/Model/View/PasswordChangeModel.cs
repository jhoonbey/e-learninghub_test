namespace app.domain.Model.View
{
    public class PasswordChangeModel : ViewBaseModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirm { get; set; }
    }
}
