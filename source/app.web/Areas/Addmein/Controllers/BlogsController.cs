using app.client.core;
using app.Enums;
using app.Model.Criterias;
using app.Model.Entities;
using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;

namespace app.web.client.Areas.Addmein.Controllers
{
    [User(AllowedRole = EnumUserRole.Admin)]
    public class BlogsController : BaseController
    {
        public ActionResult List(string keyword, int pageNumber = 1)
        {
            int rowsPerPage = 100;
            try
            {
                var result = Database.LoadBlogDataModelsByCriteria(new BlogCriteriaModel { Keyword = keyword }, rowsPerPage, pageNumber);

                ViewBag.Keyword = keyword;

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult View(int id)
        {
            try
            {
                var result = Database.GetBlogById(id);
                ViewBag.Gallery = Database.LoadImagesByCriteria(new ImageCriteriaModel { Sector = "Blog", RelatedObjectId = id }, 1000, 1).Images;

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("List", "Blogs");
            }
        }
        [HttpPost]
        public ActionResult UpdateImage(HttpPostedFileBase postedFile, int id)
        {
            try
            {
                var result = Database.UpdateBlogImage(postedFile, id, 259, 259, 262, 262, false);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
            }

            return RedirectToAction("View", "Blogs", new { id = id });
        }

        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase postedFile2, int id2)
        {
            try
            {
                Image model = new Image { Sector = "Blog", RelatedObjectId = id2 };
                var result = Database.CreateImage(postedFile2, model, 0, 0, 0, 0, false);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);

            }

            return RedirectToAction("View", "Blogs", new { id = id2 });
        }

        public ActionResult Create()
        {
            ViewBag.Categories = new SelectList(Database.LoadCategoriesByCriteria(new CategoryCriteriaModel(), 1000, 1).Categories, "Id", "Name");
            ViewBag.SubCategories = new SelectList(Database.LoadSubCategoriesByCriteria(new SubCategoryCriteriaModel(), 1000, 1).SubCategories, "Id", "Name");
            ViewBag.SubSubCategories = new SelectList(Database.LoadSubSubCategoriesByCriteria(new SubSubCategoryCriteriaModel(), 1000, 1).SubSubCategories, "Id", "Name");

            return View();
        }
        [HttpPost]
        public ActionResult Create(Blog model)
        {
            try
            {
                var result = Database.CreateBlog(model);
                return RedirectToAction("List", "Blogs");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);

                ViewBag.Categories = new SelectList(Database.LoadCategoriesByCriteria(new CategoryCriteriaModel(), 1000, 1).Categories, "Id", "Name", model.CategoryId);
                ViewBag.SubCategories = new SelectList(Database.LoadSubCategoriesByCriteria(new SubCategoryCriteriaModel { CategoryId = model.CategoryId }, 1000, 1).SubCategories, "Id", "Name", model.SubCategoryId);
                ViewBag.SubSubCategories = new SelectList(Database.LoadSubSubCategoriesByCriteria(
                    new SubSubCategoryCriteriaModel { CategoryId = model.CategoryId, SubCategoryId = model.SubCategoryId }, 1000, 1).SubSubCategories, "Id", "Name", model.SubSubCategoryId);

                return View(model);
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                var result = Database.GetBlogById(id);

                ViewBag.Categories = new SelectList(Database.LoadCategoriesByCriteria(new CategoryCriteriaModel(), 1000, 1).Categories, "Id", "Name", result.CategoryId);
                ViewBag.SubCategories = new SelectList(Database.LoadSubCategoriesByCriteria(new SubCategoryCriteriaModel { CategoryId = result.CategoryId }, 1000, 1).SubCategories, "Id", "Name", result.SubCategoryId);
                ViewBag.SubSubCategories = new SelectList(Database.LoadSubSubCategoriesByCriteria(
                    new SubSubCategoryCriteriaModel { CategoryId = result.CategoryId, SubCategoryId = result.SubCategoryId }, 1000, 1).SubSubCategories, "Id", "Name", result.SubSubCategoryId);

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("List", "Blogs");
            }
        }

        [HttpPost]
        public ActionResult Edit(Blog model)
        {
            try
            {
                var result = Database.EditBlog(model);
                return RedirectToAction("List", "Blogs");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);

                ViewBag.Categories = new SelectList(Database.LoadCategoriesByCriteria(new CategoryCriteriaModel(), 1000, 1).Categories, "Id", "Name", model.CategoryId);
                ViewBag.SubCategories = new SelectList(Database.LoadSubCategoriesByCriteria(new SubCategoryCriteriaModel { CategoryId = model.CategoryId }, 1000, 1).SubCategories, "Id", "Name", model.SubCategoryId);
                ViewBag.SubSubCategories = new SelectList(Database.LoadSubSubCategoriesByCriteria(
                    new SubSubCategoryCriteriaModel { CategoryId = model.CategoryId, SubCategoryId = model.SubCategoryId }, 1000, 1).SubSubCategories, "Id", "Name", model.SubSubCategoryId);

                return View(model);
            }
        }

        public ActionResult ManageTitle()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("BlogTitle");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [HttpPost]
        public ActionResult ManageTitle(Option model)
        {
            try
            {
                if (model == null || model.Sec != "BlogTitle") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewTitle", "Blogs");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }

        public ActionResult ViewTitle()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("BlogTitle");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        //Description
        public ActionResult ManageDescription()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("BlogDescription");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [HttpPost]
        public ActionResult ManageDescription(Option model)
        {
            try
            {
                if (model == null || model.Sec != "BlogDescription") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewDescription", "Blogs");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }

        public ActionResult ViewDescription()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("BlogDescription");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                Database.DeleteBlog(id);
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Success, "Case Study deleted successfully");
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
            }

            return RedirectToAction("List", "Blogs");
        }

        public ActionResult DeleteImage(int id)
        {
            try
            {
                Database.DeleteBlogImage(id);
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Success, "Image deleted successfully");
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
            }

            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}
