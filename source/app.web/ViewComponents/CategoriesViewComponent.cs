using app.domain.Model.Criterias;
using app.domain.Model.Entities;
using app.domain.Model.View;
using app.service;
using Microsoft.AspNetCore.Mvc;

namespace app.web.ViewComponents
{
    public class CategoriesViewComponent : ViewComponent
    {
        private readonly IEntityService _entityService;

        public CategoriesViewComponent(IEntityService entityService)
        {
            _entityService = entityService;
        }

        public IViewComponentResult Invoke()
        {
            CategoryMenuViewModel categoryMenuViewModel = new CategoryMenuViewModel
            {
                Categories = _entityService.LoadEntitiesByCriteria<Category>(new BaseCriteriaModel { PageNumber = 1, RowsPerPage = 20 }).Model?.Items,
                SubCategories = _entityService.LoadEntitiesByCriteria<SubCategory>(new BaseCriteriaModel { PageNumber = 1, RowsPerPage = 500 }).Model?.Items,
                SubOfSubCategories = _entityService.LoadEntitiesByCriteria<SubOfSubCategory>(new BaseCriteriaModel { PageNumber = 1, RowsPerPage = 10000 }).Model?.Items
            };

            return View(categoryMenuViewModel);
        }
    }
}
