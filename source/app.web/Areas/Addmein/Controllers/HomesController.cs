using app.client.core;
using app.Enums;
using app.Model.Entities;
using System;
using System.Web.Mvc;

namespace app.web.client.Areas.Addmein.Controllers
{
    [User(AllowedRole = EnumUserRole.Admin)]
    public class HomesController : app.web.client.Core.BaseController
    {
        public ActionResult ManageTitle()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("HomeTitle");
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
                if (model == null || model.Sec != "HomeTitle") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewTitle", "Homes");
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
                var result = Database.GetOptionBySecIfNotExistCreate("HomeTitle");
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
                var result = Database.GetOptionBySecIfNotExistCreate("HomeDescription");
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
                if (model == null || model.Sec != "HomeDescription") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewDescription", "Homes");
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
                var result = Database.GetOptionBySecIfNotExistCreate("HomeDescription");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }
    }
}
