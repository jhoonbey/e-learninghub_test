namespace app.domain.Model.View
{
    public class CourseCreateViewModel : ViewBaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string WhatObjectives { get; set; }
        public string WhatSkills { get; set; }
        public string WhoShouldTake { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public double Price { get; set; }
    }
}
