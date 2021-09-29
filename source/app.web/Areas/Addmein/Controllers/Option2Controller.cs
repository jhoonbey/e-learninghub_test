using app.client.core;
using app.Enums;
using app.Model.Entities;
using System;
using System.Web.Mvc;

namespace app.web.client.Areas.Addmein.Controllers
{
    [User(AllowedRole = EnumUserRole.Admin)]
    public class Option2Controller : app.web.client.Core.BaseController
    {
        public ActionResult ViewAbout()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("About");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult ManageAbout()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("About");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [HttpPost]
        public ActionResult ManageAbout(Option model)
        {
            try
            {
                if (model == null || model.Sec != "About") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewAbout", "Option");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }

    }
}
