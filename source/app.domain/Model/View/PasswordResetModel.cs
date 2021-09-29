namespace app.domain.Model.View
{
    public class PasswordResetModel : ViewBaseModel
    {
        public string Code { get; set; }

        [Password]
        public string Password { get; set; }

        [ConfirmPassword("Password")]
        public string ConfirmPassword { get; set; }
    }
}
