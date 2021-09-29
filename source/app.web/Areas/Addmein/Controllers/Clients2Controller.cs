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
    public class Clients2Controller : BaseController
    {
        public ActionResult List(int pageNumber = 1)
        {
            int rowsPerPage = 10;
            try
            {
                var result = Database.LoadClientsByCriteria(new ClientCriteriaModel(), rowsPerPage, pageNumber);

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
                var result = Database.GetClientById(id);
                ViewBag.Gallery = Database.LoadImagesByCriteria(new ImageCriteriaModel { Sector = "Client", RelatedObjectId = id }, 1000, 1).Images;

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("List", "Clients");
            }
        }
        [HttpPost]
        public ActionResult UpdateImage(HttpPostedFileBase postedFile, int id)
        {
            try
            {
                var result = Database.UpdateClientImage(postedFile, id, 259, 259, 262, 262, false);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
            }

            return RedirectToAction("View", "Clients", new { id = id });
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Client model)
        {
            try
            {
                var result = Database.CreateClient(model);
                return RedirectToAction("List", "Clients");
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
                var result = Database.GetClientById(id);
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("List", "Clients");
            }
        }

        [HttpPost]
        public ActionResult Edit(Client model)
        {
            try
            {
                var result = Database.EditClient(model);
                return RedirectToAction("List", "Clients");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }

        //public ActionResult ManageTitle()
        //{
        //    try
        //    {
        //        var result = Database.GetOptionBySecIfNotExistCreate("ClientTitle");
        //        return View(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
        //        return RedirectToAction("Index", "Dashboard");
        //    }
        //}

        //[HttpPost]
        //public ActionResult ManageTitle(Option model)
        //{
        //    try
        //    {
        //        if (model == null || model.Sec != "ClientTitle") return RedirectToAction("Index", "Dashboard");

        //        var result = Database.EditOption(model);

        //        return RedirectToAction("ViewTitle", "Clients");
        //    }
        //    catch (Exception ex)
        //    {
        //        AddError(ex.Message);
        //        return View(model);
        //    }
        //}

        //public ActionResult ViewTitle()
        //{
        //    try
        //    {
        //        var result = Database.GetOptionBySecIfNotExistCreate("ClientTitle");
        //        return View(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
        //        return RedirectToAction("Index", "Dashboard");
        //    }
        //}

        ////Description
        //public ActionResult ManageDescription()
        //{
        //    try
        //    {
        //        var result = Database.GetOptionBySecIfNotExistCreate("ClientDescription");
        //        return View(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
        //        return RedirectToAction("Index", "Dashboard");
        //    }
        //}

        //[HttpPost]
        //public ActionResult ManageDescription(Option model)
        //{
        //    try
        //    {
        //        if (model == null || model.Sec != "ClientDescription") return RedirectToAction("Index", "Dashboard");

        //        var result = Database.EditOption(model);

        //        return RedirectToAction("ViewDescription", "Clients");
        //    }
        //    catch (Exception ex)
        //    {
        //        AddError(ex.Message);
        //        return View(model);
        //    }
        //}

        //public ActionResult ViewDescription()
        //{
        //    try
        //    {
        //        var result = Database.GetOptionBySecIfNotExistCreate("ClientDescription");
        //        return View(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
        //        return RedirectToAction("Index", "Dashboard");
        //    }
        //}

        public ActionResult Delete(int id)
        {
            try
            {
                Database.DeleteClient(id);
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Success, "Client deleted successfully");
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
            }

            return RedirectToAction("List", "Clients");
        }

        public ActionResult DeleteImage(int id)
        {
            try
            {
                Database.DeleteClientImage(id);
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
