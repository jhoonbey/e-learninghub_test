using app.domain.Languages;
using app.domain.Model.View;

namespace app.service.Validations
{
    public static partial class Validator
    {
        public static void CourseCreate(CourseCreateViewModel model)
        {
            ModelIsNull(model);

            ValidateText(model.Name, Lang.CourseNameText, 1, 250, true);

            ValidateText(model.Description, Lang.CourseDescriptionText, 10, 1000, true);

            ValidateText(model.WhatObjectives, Lang.WhatObjectives, 10, 1000, true);

            ValidateText(model.WhatSkills, Lang.WhatSkills, 10, 1000, true);

            ValidateText(model.WhoShouldTake, Lang.WhoShouldTake, 10, 1000, true);

            ValidateIntPositiveIsIncorrect(model.CategoryId, Lang.CategoryText);

            ValidateIntPositiveIsIncorrect(model.SubCategoryId, Lang.SubCategoryText);

            ValidateDoublePositiveIsIncorrect(model.Price, Lang.PriceText);
        }

        public static void CourseSendApproval(int id)
        {
            ValidateIntPositiveIsNotSelected(id, Lang.CourseText);
        }
    }
}
