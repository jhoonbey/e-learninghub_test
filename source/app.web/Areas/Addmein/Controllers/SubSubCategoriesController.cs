using app.client.core;
using app.Enums;
using app.Model.Criterias;
using app.Model.Entities;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace app.web.client.Areas.Addmein.Controllers
{
    [User(AllowedRole = EnumUserRole.Admin)]
    public class SubSubCategoriesController : BaseController
    {
        public ActionResult List(int pageNumber = 1)
        {
            int rowsPerPage = 100;
            try
            {
                var result = Database.LoadSubSubCategoriesByCriteria(new SubSubCategoryCriteriaModel(), rowsPerPage, pageNumber);

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }


        public ActionResult LoadSubSubCategoriesByCategoryId(string parentId)
        {
            try
            {
                if (string.IsNullOrEmpty(parentId))
                    return Json(new List<SubSubCategory>(), JsonRequestBehavior.AllowGet);

                var response = Database.LoadSubSubCategoriesByCriteria(new SubSubCategoryCriteriaModel { SubCategoryId = Convert.ToInt32(parentId) }, 1000, 1);

                return Json(response.SubSubCategories, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return Json(new List<SubSubCategory>(), JsonRequestBehavior.AllowGet);
            }
        }


        //public ActionResult View(int id)
        //{
        //    try
        //    {
        //        var result = Database.GetSubSubCategoryById(id);
        //        ViewBag.Gallery = Database.LoadImagesByCriteria(new ImageCriteriaModel { Sector = "SubSubCategory", RelatedObjectId = id }, 1000, 1).Images;

        //        return View(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
        //        return RedirectToAction("List", "SubSubCategories");
        //    }
        //}


        public ActionResult Create()
        {
            ViewBag.Categories = new SelectList(Database.LoadCategoriesByCriteria(new CategoryCriteriaModel(), 1000, 1).Categories, "Id", "Name");
            ViewBag.SubCategories = new SelectList(Database.LoadSubCategoriesByCriteria(new SubCategoryCriteriaModel(), 1000, 1).SubCategories, "Id", "Name");

            return View();
        }
        [HttpPost]
        public ActionResult Create(SubSubCategory model)
        {
            try
            {
                var result = Database.CreateSubSubCategory(model);
                return RedirectToAction("List", "SubSubCategories");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);

                ViewBag.Categories = new SelectList(Database.LoadCategoriesByCriteria(new CategoryCriteriaModel(), 1000, 1).Categories, "Id", "Name", model.CategoryId);
                ViewBag.SubCategories = new SelectList(Database.LoadSubCategoriesByCriteria(new SubCategoryCriteriaModel { CategoryId = model.CategoryId }, 1000, 1).SubCategories, "Id", "Name", model.SubCategoryId);

                return View(model);
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                var result = Database.GetSubSubCategoryById(id);

                ViewBag.Categories = new SelectList(Database.LoadCategoriesByCriteria(new CategoryCriteriaModel(), 1000, 1).Categories, "Id", "Name", result.CategoryId);
                ViewBag.SubCategories = new SelectList(Database.LoadSubCategoriesByCriteria(new SubCategoryCriteriaModel { CategoryId = result.CategoryId }, 1000, 1).SubCategories, "Id", "Name", result.SubCategoryId);

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("List", "SubSubCategories");
            }
        }

        [HttpPost]
        public ActionResult Edit(SubSubCategory model)
        {
            try
            {
                var result = Database.EditSubSubCategory(model);
                return RedirectToAction("List", "SubSubCategories");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);

                ViewBag.Categories = new SelectList(Database.LoadCategoriesByCriteria(new CategoryCriteriaModel(), 1000, 1).Categories, "Id", "Name", model.CategoryId);
                ViewBag.SubCategories = new SelectList(Database.LoadSubCategoriesByCriteria(new SubCategoryCriteriaModel { CategoryId = model.CategoryId }, 1000, 1).SubCategories, "Id", "Name", model.SubCategoryId);

                return View(model);
            }
        }


        //public ActionResult Delete(int id)
        //{
        //    try
        //    {
        //        Database.DeleteSubSubCategory(id);
        //        TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Success, "Case Study deleted successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
        //    }

        //    return RedirectToAction("List", "SubSubCategories");
        //}
    }
}
