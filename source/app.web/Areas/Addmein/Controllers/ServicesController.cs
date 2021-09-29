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
    public class ServicesController : BaseController
    {
        public ActionResult List(int pageNumber = 1)
        {
            int rowsPerPage = 10;
            try
            {
                var result = Database.LoadServicesByCriteria(new ServiceCriteriaModel(), rowsPerPage, pageNumber);

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
                var result = Database.GetServiceById(id);
                ViewBag.Gallery = Database.LoadImagesByCriteria(new ImageCriteriaModel { Sector = "Service", RelatedObjectId = id }, 1000, 1).Images;

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("List", "Services");
            }
        }
        [HttpPost]
        public ActionResult UpdateImage(HttpPostedFileBase postedFile, int id)
        {
            try
            {
                var result = Database.UpdateServiceImage(postedFile, id, 259, 259, 262, 262, false);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
            }

            return RedirectToAction("View", "Services", new { id = id });
        }

        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase postedFile2, int id2)
        {
            try
            {
                Image model = new Image { Sector = "Service", RelatedObjectId = id2 };
                var result = Database.CreateImage(postedFile2, model, 0, 0, 0, 0, false);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);

            }

            return RedirectToAction("View", "Services", new { id = id2 });
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Service model)
        {
            try
            {
                var result = Database.CreateService(model);
                return RedirectToAction("List", "Services");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                var result = Database.GetServiceById(id);
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("List", "Services");
            }
        }

        [HttpPost]
        public ActionResult Edit(Service model)
        {
            try
            {
                var result = Database.EditService(model);
                return RedirectToAction("List", "Services");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }

        public ActionResult ManageTitle()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("ServiceTitle");
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
                if (model == null || model.Sec != "ServiceTitle") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewTitle", "Services");
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
                var result = Database.GetOptionBySecIfNotExistCreate("ServiceTitle");
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
                var result = Database.GetOptionBySecIfNotExistCreate("ServiceDescription");
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
                if (model == null || model.Sec != "ServiceDescription") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewDescription", "Services");
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
                var result = Database.GetOptionBySecIfNotExistCreate("ServiceDescription");
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
                Database.DeleteService(id);
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Success, "Service deleted successfully");
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
            }

            return RedirectToAction("List", "Services");
        }
    }
}
