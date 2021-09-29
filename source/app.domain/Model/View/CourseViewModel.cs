using app.domain.Model.Entities;
using System.Collections.Generic;

namespace app.domain.Model.View
{
    public class CourseViewModel : ViewBaseModel
    {
        public Course Course { get; set; }
        public List<Video> Videos { get; set; }
        public List<Section> Sections { get; set; }
    }
}
