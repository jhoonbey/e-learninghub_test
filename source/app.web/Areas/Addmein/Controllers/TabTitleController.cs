using app.client.core;
using app.Enums;
using app.Model.Entities;
using System;
using System.Web.Mvc;

namespace app.web.client.Areas.Addmein.Controllers
{
    [User(AllowedRole = EnumUserRole.Admin)]
    public class TabTitleController : BaseController
    {
        public ActionResult ViewHomeTT()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("HomeTT");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }
        public ActionResult ManageHomeTT()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("HomeTT");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }
        [HttpPost]
        public ActionResult ManageHomeTT(Option model)
        {
            try
            {
                if (model == null || model.Sec != "HomeTT") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewHomeTT", "TabTitle");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }


        public ActionResult ViewAboutTT()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("AboutTT");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }
        public ActionResult ManageAboutTT()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("AboutTT");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }
        [HttpPost]
        public ActionResult ManageAboutTT(Option model)
        {
            try
            {
                if (model == null || model.Sec != "AboutTT") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewAboutTT", "TabTitle");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }


        public ActionResult ViewContactTT()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("ContactTT");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }
        public ActionResult ManageContactTT()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("ContactTT");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }
        [HttpPost]
        public ActionResult ManageContactTT(Option model)
        {
            try
            {
                if (model == null || model.Sec != "ContactTT") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewContactTT", "TabTitle");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }


        public ActionResult ViewServiceTT()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("ServiceTT");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }
        public ActionResult ManageServiceTT()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("ServiceTT");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }
        [HttpPost]
        public ActionResult ManageServiceTT(Option model)
        {
            try
            {
                if (model == null || model.Sec != "ServiceTT") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewServiceTT", "TabTitle");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }


        public ActionResult ViewCaseStudyTT()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("CaseStudyTT");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }
        public ActionResult ManageCaseStudyTT()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("CaseStudyTT");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }
        [HttpPost]
        public ActionResult ManageCaseStudyTT(Option model)
        {
            try
            {
                if (model == null || model.Sec != "CaseStudyTT") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewCaseStudyTT", "TabTitle");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }


        public ActionResult ViewBlogTT()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("BlogTT");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }
        public ActionResult ManageBlogTT()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("BlogTT");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }
        [HttpPost]
        public ActionResult ManageBlogTT(Option model)
        {
            try
            {
                if (model == null || model.Sec != "BlogTT") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewBlogTT", "TabTitle");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }


        public ActionResult ViewQuoteTT()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("QuoteTT");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }
        public ActionResult ManageQuoteTT()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("QuoteTT");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }
        [HttpPost]
        public ActionResult ManageQuoteTT(Option model)
        {
            try
            {
                if (model == null || model.Sec != "QuoteTT") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewQuoteTT", "TabTitle");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }
    }
}
