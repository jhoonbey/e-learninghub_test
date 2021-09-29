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
//    public class TestimonialsController : BaseController
//    {
//        public ActionResult List(int pageNumber = 1)
//        {
//            int rowsPerPage = 10;
//            try
//            {
//                var result = Database.LoadTestimonialsByCriteria(new TestimonialCriteriaModel(), rowsPerPage, pageNumber);

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
//                var result = Database.GetTestimonialById(id);
//                ViewBag.Gallery = Database.LoadImagesByCriteria(new ImageCriteriaModel { Sector = "Testimonial", RelatedObjectId = id }, 1000, 1).Images;

//                return View(result);
//            }
//            catch (Exception ex)
//            {
//                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
//                return RedirectToAction("List", "Testimonials");
//            }
//        }
//        [HttpPost]
//        public ActionResult UpdateImage(HttpPostedFileBase postedFile, int id)
//        {
//            try
//            {
//                var result = Database.UpdateTestimonialImage(postedFile, id, 259, 259, 262, 262, false);
//            }
//            catch (Exception ex)
//            {
//                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
//            }

//            return RedirectToAction("View", "Testimonials", new { id = id });
//        }

//        public ActionResult Create()
//        {
//            return View();
//        }
//        [HttpPost]
//        public ActionResult Create(Testimonial model)
//        {
//            try
//            {
//                var result = Database.CreateTestimonial(model);
//                return RedirectToAction("List", "Testimonials");
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
//                var result = Database.GetTestimonialById(id);
//                return View(result);
//            }
//            catch (Exception ex)
//            {
//                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
//                return RedirectToAction("List", "Testimonials");
//            }
//        }

//        [HttpPost]
//        public ActionResult Edit(Testimonial model)
//        {
//            try
//            {
//                var result = Database.EditTestimonial(model);
//                return RedirectToAction("List", "Testimonials");
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
//                Database.DeleteTestimonial(id);
//                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Success, "Testimonial deleted successfully");
//            }
//            catch (Exception ex)
//            {
//                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
//            }

//            return RedirectToAction("List", "Testimonials");
//        }

//        public ActionResult DeleteImage(int id)
//        {
//            try
//            {
//                Database.DeleteTestimonialImage(id);
//                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Success, "Image deleted successfully");
//            }
//            catch (Exception ex)
//            {
//                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
//            }

//            return Redirect(Request.UrlReferrer.ToString());
//        }
//    }
//}
