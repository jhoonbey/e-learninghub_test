using app.client.core;
using app.Enums;
using app.Model.Criterias;
using app.Model.Entities;
using System;
using System.Web.Mvc;

namespace app.web.client.Areas.Addmein.Controllers
{
    [User(AllowedRole = EnumUserRole.Admin)]
    public class QuotesController : BaseController
    {
        public ActionResult List(int pageNumber = 1)
        {
            int rowsPerPage = 10;
            try
            {
                var result = Database.LoadQuoteDataModelsByCriteria(new QuoteCriteriaModel(), rowsPerPage, pageNumber);

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        //public ActionResult View(int id)
        //{
        //    try
        //    {
        //        var result = Database.GetQuoteById(id);
        //        return View(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
        //        return RedirectToAction("List", "Quotes");
        //    }
        //}


        //Title

        //Title
        public ActionResult ManageTitle()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("QuoteTitle");
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
                if (model == null || model.Sec != "QuoteTitle") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewTitle", "Quotes");
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
                var result = Database.GetOptionBySecIfNotExistCreate("QuoteTitle");
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
                var result = Database.GetOptionBySecIfNotExistCreate("QuoteDescription");
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
                if (model == null || model.Sec != "QuoteDescription") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewDescription", "Quotes");
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
                var result = Database.GetOptionBySecIfNotExistCreate("QuoteDescription");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }


        //Content title
        public ActionResult ManageContentTitle()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("QuoteContentTitle");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [HttpPost]
        public ActionResult ManageContentTitle(Option model)
        {
            try
            {
                if (model == null || model.Sec != "QuoteContentTitle") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewContentTitle", "Quotes");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }

        public ActionResult ViewContentTitle()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("QuoteContentTitle");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }


        //Content Description
        public ActionResult ManageContentDescription()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("QuoteContentDescription");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [HttpPost]
        public ActionResult ManageContentDescription(Option model)
        {
            try
            {
                if (model == null || model.Sec != "QuoteContentDescription") return RedirectToAction("Index", "Dashboard");

                var result = Database.EditOption(model);

                return RedirectToAction("ViewContentDescription", "Quotes");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }

        public ActionResult ViewContentDescription()
        {
            try
            {
                var result = Database.GetOptionBySecIfNotExistCreate("QuoteContentDescription");
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }


        //public ActionResult Delete(int id)
        //{
        //    try
        //    {
        //        Database.DeleteQuote(id);
        //        TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Success, "Quote deleted successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
        //    }

        //    return RedirectToAction("List", "Quotes");
        //}
    }
}
