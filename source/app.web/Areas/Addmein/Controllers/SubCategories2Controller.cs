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
    public class SubCategories2Controller : BaseController
    {
        public ActionResult List(int pageNumber = 1)
        {
            int rowsPerPage = 100;
            try
            {
                var result = Database.LoadSubCategoriesByCriteria(new SubCategoryCriteriaModel(), rowsPerPage, pageNumber);

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult LoadSubCategoriesByCategoryId(string parentId)
        {
            try
            {
                if (string.IsNullOrEmpty(parentId))
                    return Json(new List<SubCategory>(), JsonRequestBehavior.AllowGet);

                var response = Database.LoadSubCategoriesByCriteria(new SubCategoryCriteriaModel { CategoryId = Convert.ToInt32(parentId) }, 1000, 1);

                return Json(response.SubCategories, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return Json(new List<SubCategory>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Create()
        {
            ViewBag.Categories = new SelectList(Database.LoadCategoriesByCriteria(new CategoryCriteriaModel(), 1000, 1).Categories, "Id", "Name");

            return View();
        }
        [HttpPost]
        public ActionResult Create(SubCategory model)
        {
            try
            {
                var result = Database.CreateSubCategory(model);
                return RedirectToAction("List", "SubCategories");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);

                ViewBag.Categories = new SelectList(Database.LoadCategoriesByCriteria(new CategoryCriteriaModel(), 1000, 1).Categories, "Id", "Name", model.CategoryId);

                return View(model);
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                var result = Database.GetSubCategoryById(id);

                ViewBag.Categories = new SelectList(Database.LoadCategoriesByCriteria(new CategoryCriteriaModel(), 1000, 1).Categories, "Id", "Name", result.CategoryId);

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("List", "SubCategories");
            }
        }

        [HttpPost]
        public ActionResult Edit(SubCategory model)
        {
            try
            {
                var result = Database.EditSubCategory(model);
                return RedirectToAction("List", "SubCategories");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);

                ViewBag.Categories = new SelectList(Database.LoadCategoriesByCriteria(new CategoryCriteriaModel(), 1000, 1).Categories, "Id", "Name", model.CategoryId);

                return View(model);
            }
        }


        //public ActionResult Delete(int id)
        //{
        //    try
        //    {
        //        Database.DeleteSubCategory(id);
        //        TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Success, "Case Study deleted successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
        //    }

        //    return RedirectToAction("List", "SubCategories");
        //}


    }
}
