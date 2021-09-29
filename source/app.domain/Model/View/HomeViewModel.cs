using app.domain.Model.Entities;
using System.Collections.Generic;

namespace app.domain.Model.View
{
    public class HomeViewModel : ViewBaseModel
    {
        public Option AboutInfo { get; set; }
        public Option Skills_Description { get; set; }
        public List<Skill> Skills { get; set; }
        public List<Course> PopularCourses { get; set; }
        public List<User> Instructors { get; set; }

        //public Image TestimonialImage { get; set; }
        //public List<Client> Clients { get; set; }
        //public List<CaseStudy> CaseStudies { get; set; }
        //public QuoteViewModel QuoteViewModel { get; set; }
        //public Option TabTitle { get; set; }
    }
}
