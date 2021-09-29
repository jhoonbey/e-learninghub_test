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
    public class BranchController : app.web.client.Core.BaseController
    {
        public ActionResult List(int pageNumber = 1)
        {
            int rowsPerPage = 10000;
            try
            {
                BranchCriteriaModel criteria = new BranchCriteriaModel();
                var result = Database.LoadBranchesByCriteria(criteria, rowsPerPage, pageNumber);

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Branch model)
        {
            try
            {
                var result = Database.CreateBranch(model);
                return RedirectToAction("List", "Branch");
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
                var result = Database.GetBranchById(id);
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("List", "Branch");
            }
        }

        [HttpPost]
        public ActionResult Edit(Branch model)
        {
            try
            {
                var result = Database.EditBranch(model);
                return RedirectToAction("List", "Branch");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                Database.DeleteBranch(id);
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Success, "Location deleted successfully");
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
            }

            return RedirectToAction("List", "Branch", new { area = "Addmein" });
        }
    }
}
