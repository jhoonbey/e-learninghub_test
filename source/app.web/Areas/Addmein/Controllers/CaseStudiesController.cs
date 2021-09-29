using app.client.core;
using app.Enums;
using app.Model.Criterias;
using app.Model.Entities;
using System;
using System.Web;
using System.Web.Mvc;

namespace app.web.client.Areas.Addmein.Controllers
{
    [User(AllowedRole = EnumUserRole.Admin)]
    public class CaseStudiesController : BaseController
    {
        public ActionResult List(int pageNumber = 1)
        {
            int rowsPerPage = 10;
            try
            {
                var result = Database.LoadCaseStudiesByCriteria(new CaseStudyCriteriaModel(), rowsPerPage, pageNumber);

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
                var result = Database.GetCaseStudyById(id);
                ViewBag.Gallery = Database.LoadImagesByCriteria(new ImageCriteriaModel { Sector = "CaseStudy", RelatedObjectId = id }, 1000, 1).Images;

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("List", "CaseStudies");
            }
        }
        [HttpPost]
        public ActionResult UpdateImage(HttpPostedFileBase postedFile, int id)
        {
            try
            {
                var result = Database.UpdateCaseStudyImage(postedFile, id, 259, 259, 262, 262, false);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
            }

            return RedirectToAction("View", "CaseStudies", new { id = id });
        }

        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase postedFile2, int id2)
        {
            try
            {
                Image model = new Image { Sector = "CaseStudy", RelatedObjectId = id2 };
                var result = Database.CreateImage(postedFile2, model, 0, 0, 0, 0, false);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);

            }

            return RedirectToAction("View", "CaseStudies", new { id = id2 });
        }

        public ActionResult Create()
        {
            ViewBag.Categories = new SelectList(Database.LoadCategoriesByCriteria(new CategoryCriteriaModel(), 1000, 1).Categories, "Id", "Name");

            return View();
        }
        [HttpPost]
        public ActionResult Create(CaseStudy model)
        {
            try
            {
                var result = Database.CreateCaseStudy(model);
                return RedirectToAction("List", "CaseStudies");
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
                var result = Database.GetCaseStudyById(id);

                ViewBag.Categories = new SelectList(Database.LoadCategoriesByCriteria(new CategoryCriteriaModel(), 1000, 1).Categories, "Id", "Name", result.CategoryId);

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("List", "CaseStudies");
            }
        }

        [HttpPost]
        public ActionResult Edit(CaseStudy model)
        {
            try
            {
                var result = Database.EditCaseStudy(model);
                return RedirectToAction("List", "CaseStudies");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);

                ViewBag.Categories = new SelectList(Database.LoadCategoriesByCriteria(new CategoryCriteriaModel(), 1000, 1).Categories, "Id", "Name", model.CategoryId);

                return View(model);
            }
        }

        public ActionResult ManageTitle()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("CaseStudyTitle");
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
                if (model == null || model.Sec != "CaseStudyTitle") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewTitle", "CaseStudies");
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
                var result = Database.GetOptionBySecIfNotExistCreate("CaseStudyTitle");
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
                var result = Database.GetOptionBySecIfNotExistCreate("CaseStudyDescription");
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
                if (model == null || model.Sec != "CaseStudyDescription") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewDescription", "CaseStudies");
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
                var result = Database.GetOptionBySecIfNotExistCreate("CaseStudyDescription");
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
                Database.DeleteCaseStudy(id);
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Success, "Case Study deleted successfully");
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
            }

            return RedirectToAction("List", "CaseStudies");
        }
    }
}
