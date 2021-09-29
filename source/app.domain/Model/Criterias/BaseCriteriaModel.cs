using app.domain.Model.View;
using System;
using System.Collections.Generic;

namespace app.domain.Model.Criterias
{
    public class BaseCriteriaModel
    {
        public string Keyword { get; set; }
        public int? IntCriteria { get; set; }
        public int? IntCriteria2 { get; set; }
        public DateTime? MinCreateDate { get; set; }
        public DateTime? MaxCreateDate { get; set; }

        public int Top { get; set; }

        public bool WillCount { get; set; }
        public int RowsPerPage { get; set; }
        public int PageNumber { get; set; }

        public List<LeftJoinModel> LeftJoinModels { get; set; }
        public List<OrderByModel> OrderByModels { get; set; }
    }
}
