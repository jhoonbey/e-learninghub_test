namespace app.domain.Model.Entities
{
    public class Course : EntityBaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string WhatObjectives { get; set; }
        public string WhatSkills { get; set; }
        public string WhoShouldTake { get; set; }

        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }

        public int Status { get; set; }
        public int UserId { get; set; }

        public int ViewCount { get; set; }
        public double Price { get; set; }
        public double ActualPrice { get; set; }

        public string Filename { get; set; }
        public string Extension { get; set; }
        public string MediaType { get; set; }
        public string Snapshot { get; set; }

        public string RejectReason { get; set; }
    }
}