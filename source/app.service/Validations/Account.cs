using app.domain.Enums;
using app.domain.Exceptions;
using app.domain.Languages;
using app.domain.Model.Entities;
using app.domain.Model.View;
using app.domain.Static;
using System.Text.RegularExpressions;

namespace app.service.Validations
{
    public static partial class Validator
    {
        public static void ConfirmEmailCode(string code)
        {
            if (string.IsNullOrEmpty(code) || code.Length != 36)
                throw new BusinessException("Confirmation code is incorrect");
        }

        public static void ResetPassword(PasswordResetModel model)
        {
            ModelIsNull(model);

            if (string.IsNullOrEmpty(model.Code)) throw new BusinessException("Reset Password code is incorrect");

            ValidatePasswordText(new Regex(StaticValues.RegexPassword), StaticValues.RegexPassword_Info, model.Password, Lang.PasswordText, 5, 50, true);

            ValidatePasswordText(new Regex(StaticValues.RegexPassword), StaticValues.RegexPassword_Info, model.ConfirmPassword, Lang.ConfirmPasswordText, 5, 50, true);
        }

        public static void UserCreate(StartViewModel model)
        {
            ModelIsNull(model);

            ValidateText(new Regex(StaticValues.RegexNameUnicode), StaticValues.RegexNameUnicode_Info, model.Name, Lang.NameText, 2, 100, true);

            ValidateText(new Regex(StaticValues.RegexNameUnicode), StaticValues.RegexNameUnicode_Info, model.Surname, Lang.SurnameText, 2, 100, true);

            ValidateText(new Regex(StaticValues.RegexMobile), StaticValues.RegexMobile_Info, model.Mobile, Lang.MobileText, 0, 0, false);

            ValidateText(new Regex(StaticValues.RegexEmail), model.Email, Lang.EmailText);

            ValidatePasswordText(new Regex(StaticValues.RegexPassword), StaticValues.RegexPassword_Info, model.Password, Lang.PasswordText, 5, 50, true);

            ValidatePasswordText(new Regex(StaticValues.RegexPassword), StaticValues.RegexPassword_Info, model.ConfirmPassword, Lang.ConfirmPasswordText, 5, 50, true);

            if (model.ConfirmPassword != model.Password)
            {
                throw new BusinessException(Lang.ErrorPasswordsNotSameText);
            }

            ValidateText(model.Position, Lang.PositionText, 250);

            ValidateText(model.Workplace, Lang.WorkplaceText, 250);

            if (string.IsNullOrEmpty(model.Rolename) && model.Rolename != "instructor" && model.Rolename != "learner")
            {
                throw new BusinessException(Lang.ErrorRoleIsIncorrectText);
            }

            if (model.Rolename == "instructor")
            {
                ValidateIntPositiveIsIncorrect(model.CategoryId, Lang.CategoryText);
            }

            if (!model.IsAgreeTerms)
            {
                throw new BusinessException(Lang.ErrorIsAgreeTerms);
            }
        }

        public static void UpdateProfile(UserDataViewModel model, User user)
        {
            if (user.Role != (int)EnumUserRole.Learner)
            {
                if (model == null || model.CategoryIdSet == null || model.CategoryIdSet.Count < 1)
                {
                    throw new BusinessException("Select minimum 1 category");
                }
            }

            ModelIsNull(model);

            ValidateText(new Regex(StaticValues.RegexNameUnicode), StaticValues.RegexNameUnicode_Info, model.Name, Lang.NameText, 2, 100, true);
            ValidateText(new Regex(StaticValues.RegexNameUnicode), StaticValues.RegexNameUnicode_Info, model.Surname, Lang.SurnameText, 2, 100, true);

            ValidateText(model.Position, Lang.PositionText, 250);
            ValidateText(model.Workplace, Lang.WorkplaceText, 250);
            ValidateText(model.Address, Lang.AddressText, 250);
            ValidateText(model.University, Lang.UniversityText, 250);

            //ValidateText(model.Info, Lang.InfoText, 3000);
        }


        public static void ChangePassword(PasswordChangeModel model)
        {
            ModelIsNull(model);

            ValidatePasswordText(new Regex(StaticValues.RegexPassword), StaticValues.RegexPassword_Info, model.OldPassword, Lang.OldPasswordText, 5, 50, true);

            ValidatePasswordText(new Regex(StaticValues.RegexPassword), StaticValues.RegexPassword_Info, model.NewPassword, Lang.NewPasswordText, 5, 50, true);

            ValidatePasswordText(new Regex(StaticValues.RegexPassword), StaticValues.RegexPassword_Info, model.NewPasswordConfirm, Lang.ConfirmPasswordText, 5, 50, true);

            if (model.NewPassword != model.NewPasswordConfirm)
            {
                throw new BusinessException(Lang.ErrorPasswordsNotSameText);
            }
        }

        public static void UserReset(StartViewModel model)
        {
            ModelIsNull(model);

            ValidateText(new Regex(StaticValues.RegexEmail), model.ResetEmail, Lang.EmailText);
        }

        public static void UserFind(StartViewModel model)
        {
            ModelIsNull(model);

            ValidateText(new Regex(StaticValues.RegexEmail), model.LoginEmail, Lang.EmailText);

            ValidatePasswordText(new Regex(StaticValues.RegexPassword), StaticValues.RegexPassword_Info, model.LoginPassword, Lang.PasswordText, 5, 50, true);
        }


        //public static void UserChangePasswordValidation(string oldPass, string newPass, string newPass2)
        //{
        //    //oldPass
        //    Regex rxOld = new Regex(@"^[a-zA-Z0-9_?!-&]+$");
        //    if (string.IsNullOrEmpty(oldPass) || oldPass.Count() < 3 || oldPass.Count() > 50 || !rxOld.IsMatch(oldPass))
        //        throw new Exception("Uncorrect old password. Use a-z, A-Z, 0-9 and _ ? ! - & symbols only. Minimum 3, maximum 50 length");

        //    //newPass
        //    Regex rxNew = new Regex(@"^[a-zA-Z0-9_?!-&]+$");
        //    if (string.IsNullOrEmpty(newPass) || newPass.Count() < 3 || newPass.Count() > 50 || !rxNew.IsMatch(newPass))
        //        throw new Exception("Uncorrect new password. Use a-z, A-Z, 0-9 and _ ? ! - & symbols only. Minimum 3, maximum 50 length");

        //    //newPass2
        //    Regex rxNew2 = new Regex(@"^[a-zA-Z0-9_?!-&]+$");
        //    if (string.IsNullOrEmpty(newPass2) || newPass2.Count() < 3 || newPass2.Count() > 50 || !rxNew2.IsMatch(newPass2))
        //        throw new Exception("Uncorrect repeated new password. Use a-z, A-Z, 0-9 and _ ? ! - & symbols only. Minimum 3, maximum 50 length");

        //    //matching...
        //    if (newPass.ToLower() != newPass2.ToLower()) throw new Exception("New passwords are not match");
        //}
        //public static void UserIdentificationValidation(string username, string password, int role)
        //{
        //    //Username
        //    Regex rxUsername = new Regex(@"^[a-zA-Z0-9_?!-&]+$");
        //    if (string.IsNullOrEmpty(username) || username.Count() < 3 || username.Count() > 50 || !rxUsername.IsMatch(username))
        //        throw new Exception("Uncorrect username. Use a-z, A-Z, 0-9 and _ ? ! - & symbols only. Minimum 3, maximum 50 length");

        //    //Password
        //    Regex rxPassword = new Regex(@"^[a-zA-Z0-9_?!-&]+$");
        //    if (string.IsNullOrEmpty(password) || password.Count() < 3 || password.Count() > 50 || !rxPassword.IsMatch(password))
        //        throw new Exception("Uncorrect password. Use a-z, A-Z, 0-9 and _ ? ! - & symbols only. Minimum 3, maximum 50 length");

        //    //role
        //    if (role < 1) throw new Exception("Uncorrect role");
        //}
        //public static void UserLoginValidation(string username, string password)
        //{
        //    //Username
        //    Regex rxUsername = new Regex(@"^[a-zA-Z0-9_?!-&]+$");
        //    if (string.IsNullOrEmpty(username) || username.Count() < 3 || username.Count() > 50 || !rxUsername.IsMatch(username))
        //        throw new Exception("Uncorrect username. Use a-z, A-Z, 0-9 and _ ? ! - & symbols only. Minimum 3, maximum 50 length");

        //    //Password
        //    Regex rxPassword = new Regex(@"^[a-zA-Z0-9_?!-&]+$");
        //    if (string.IsNullOrEmpty(password) || password.Count() < 3 || password.Count() > 50 || !rxPassword.IsMatch(password))
        //        throw new Exception("Uncorrect password. Use a-z, A-Z, 0-9 and _ ? ! - & symbols only. Minimum 3, maximum 50 length");
        //}

    }
}
