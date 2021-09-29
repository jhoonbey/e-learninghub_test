//using app.client.core;
//using app.Enums;
//using app.Model.Criterias;
//using app.Model.Entities;
//using System;
//using System.Web;
//using System.Web.Mvc;

//namespace app.web.client.Areas.Addmein.Controllers
//{
//    [User(AllowedRole = EnumUserRole.Admin)]
//    public class TeamsController : BaseController
//    {
//        public ActionResult List(int pageNumber = 1)
//        {
//            int rowsPerPage = 10;
//            try
//            {
//                var result = Database.LoadTeamsByCriteria(new TeamCriteriaModel(), rowsPerPage, pageNumber);

//                return View(result);
//            }
//            catch (Exception ex)
//            {
//                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
//                return RedirectToAction("Index", "Dashboard");
//            }
//        }

//        public ActionResult View(int id)
//        {
//            try
//            {
//                var result = Database.GetTeamById(id);
//                ViewBag.Gallery = Database.LoadImagesByCriteria(new ImageCriteriaModel { Sector = "Team", RelatedObjectId = id }, 1000, 1).Images;

//                return View(result);
//            }
//            catch (Exception ex)
//            {
//                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
//                return RedirectToAction("List", "Teams");
//            }
//        }
//        [HttpPost]
//        public ActionResult UpdateImage(HttpPostedFileBase postedFile, int id)
//        {
//            try
//            {
//                var result = Database.UpdateTeamImage(postedFile, id, 259, 259, 262, 262, false);
//            }
//            catch (Exception ex)
//            {
//                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
//            }

//            return RedirectToAction("View", "Teams", new { id = id });
//        }

//        public ActionResult Create()
//        {
//            return View();
//        }
//        [HttpPost]
//        public ActionResult Create(Team model)
//        {
//            try
//            {
//                var result = Database.CreateTeam(model);
//                return RedirectToAction("List", "Teams");
//            }
//            catch (Exception ex)
//            {
//                AddError(ex.Message);
//                return View(model);
//            }
//        }

//        public ActionResult Edit(int id)
//        {
//            try
//            {
//                var result = Database.GetTeamById(id);
//                return View(result);
//            }
//            catch (Exception ex)
//            {
//                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
//                return RedirectToAction("List", "Teams");
//            }
//        }

//        [HttpPost]
//        public ActionResult Edit(Team model)
//        {
//            try
//            {
//                var result = Database.EditTeam(model);
//                return RedirectToAction("List", "Teams");
//            }
//            catch (Exception ex)
//            {
//                AddError(ex.Message);
//                return View(model);
//            }
//        }

//        public ActionResult Delete(int id)
//        {
//            try
//            {
//                Database.DeleteTeam(id);
//                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Success, "Team member deleted successfully");
//            }
//            catch (Exception ex)
//            {
//                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
//            }

//            return RedirectToAction("List", "Teams");
//        }
//    }
//}
