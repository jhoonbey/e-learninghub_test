using app.domain.Languages;

namespace app.service.Validations
{
    public static partial class Validator
    {
        public static void SectionCreate(int courseId, string name)
        {
            ValidateText(name, Lang.NameText, 1, 250, true);

            ValidateIntPositiveIsIncorrect(courseId, Lang.CourseText);
        }
    }
}
