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
    public class SubServicesController : BaseController
    {
        public ActionResult List(int pageNumber = 1)
        {
            int rowsPerPage = 10;
            try
            {
                var result = Database.LoadSubServicesByCriteria(new SubServiceCriteriaModel(), rowsPerPage, pageNumber);

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
            ViewBag.Services = new SelectList(Database.LoadServicesByCriteria(new ServiceCriteriaModel(), 1000, 1).Services, "Id", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult Create(SubService model)
        {
            try
            {
                var result = Database.CreateSubService(model);
                return RedirectToAction("List", "SubServices");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);

                ViewBag.Services = new SelectList(Database.LoadServicesByCriteria(new ServiceCriteriaModel(), 1000, 1).Services, "Id", "Name", model.ServiceId);

                return View(model);
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                var result = Database.GetSubServiceById(id);

                ViewBag.Services = new SelectList(Database.LoadServicesByCriteria(new ServiceCriteriaModel(), 1000, 1).Services, "Id", "Name", result.ServiceId);

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("List", "SubServices");
            }
        }

        [HttpPost]
        public ActionResult Edit(SubService model)
        {
            try
            {
                var result = Database.EditSubService(model);
                return RedirectToAction("List", "SubServices");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                ViewBag.Services = new SelectList(Database.LoadServicesByCriteria(new ServiceCriteriaModel(), 1000, 1).Services, "Id", "Name", model.ServiceId);

                return View(model);
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                Database.DeleteSubService(id);
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Success, "Case Study deleted successfully");
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
            }

            return RedirectToAction("List", "SubServices");
        }
    }
}
