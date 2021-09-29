using app.domain.Exceptions;
using app.domain.Languages;
using app.domain.Model.View;
using app.domain.Static;
using app.domain.Utilities;
using System.Text.RegularExpressions;

namespace app.service.Validations
{
    public static partial class Validator
    {
        public static void ContactForm(ContactViewModel model)
        {
            ModelIsNull(model);

            ValidateText(new Regex(StaticValues.RegexNameUnicode), StaticValues.RegexNameUnicode_Info, model.Name, Lang.NameText, 2, 100, true);

            ValidateText(new Regex(StaticValues.RegexEmail), model.Email, Lang.EmailText);

            //comment
            if (string.IsNullOrEmpty(model.Comment) || Common.IsOnlySpace(model.Comment))
            {
                throw new BusinessException(Lang.CommentText + Lang.ErrorIsIncorrectText);
            }

            ValidateText(model.Comment, Lang.CommentText, 10, 2000, true);
        }
    }
}
