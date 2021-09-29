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
    public class ImageController : app.web.client.Core.BaseController
    {
        public ActionResult AboutPhoto(int pageNumber = 1)
        {
            int rowsPerPage = 1;
            try
            {
                ImageCriteriaModel criteria = new ImageCriteriaModel { Sector = "AboutBG" };
                var result = Database.LoadImagesByCriteria(criteria, rowsPerPage, pageNumber);

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult ContactPhoto(int pageNumber = 1)
        {
            int rowsPerPage = 1;
            try
            {
                ImageCriteriaModel criteria = new ImageCriteriaModel { Sector = "Contact" };
                var result = Database.LoadImagesByCriteria(criteria, rowsPerPage, pageNumber);

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult ServicePhoto(int pageNumber = 1)
        {
            int rowsPerPage = 1;
            try
            {
                ImageCriteriaModel criteria = new ImageCriteriaModel { Sector = "ServiceBG" };
                var result = Database.LoadImagesByCriteria(criteria, rowsPerPage, pageNumber);

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult CaseStudyPhoto(int pageNumber = 1)
        {
            int rowsPerPage = 1;
            try
            {
                ImageCriteriaModel criteria = new ImageCriteriaModel { Sector = "CaseStudyBG" };
                var result = Database.LoadImagesByCriteria(criteria, rowsPerPage, pageNumber);

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult BlogPhoto(int pageNumber = 1)
        {
            int rowsPerPage = 1;
            try
            {
                ImageCriteriaModel criteria = new ImageCriteriaModel { Sector = "BlogBG" };
                var result = Database.LoadImagesByCriteria(criteria, rowsPerPage, pageNumber);

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult HomePhoto(int pageNumber = 1)
        {
            int rowsPerPage = 1;
            try
            {
                ImageCriteriaModel criteria = new ImageCriteriaModel { Sector = "HomeBG" };
                var result = Database.LoadImagesByCriteria(criteria, rowsPerPage, pageNumber);

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult AboutInfoPhoto(int pageNumber = 1)
        {
            int rowsPerPage = 1;
            try
            {
                ImageCriteriaModel criteria = new ImageCriteriaModel { Sector = "About" };
                var result = Database.LoadImagesByCriteria(criteria, rowsPerPage, pageNumber);

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult AboutVisionPhoto(int pageNumber = 1)
        {
            int rowsPerPage = 1;
            try
            {
                ImageCriteriaModel criteria = new ImageCriteriaModel { Sector = "AboutVision" };
                var result = Database.LoadImagesByCriteria(criteria, rowsPerPage, pageNumber);

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult AboutMissionPhoto(int pageNumber = 1)
        {
            int rowsPerPage = 1;
            try
            {
                ImageCriteriaModel criteria = new ImageCriteriaModel { Sector = "AboutMission" };
                var result = Database.LoadImagesByCriteria(criteria, rowsPerPage, pageNumber);

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult TestimonialPhoto(int pageNumber = 1)
        {
            int rowsPerPage = 1;
            try
            {
                ImageCriteriaModel criteria = new ImageCriteriaModel { Sector = "TestimonialBG" };
                var result = Database.LoadImagesByCriteria(criteria, rowsPerPage, pageNumber);

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult QuotePhoto(int pageNumber = 1)
        {
            int rowsPerPage = 1;
            try
            {
                ImageCriteriaModel criteria = new ImageCriteriaModel { Sector = "Quote" };
                var result = Database.LoadImagesByCriteria(criteria, rowsPerPage, pageNumber);

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult QuoteBGPhoto(int pageNumber = 1)
        {
            int rowsPerPage = 1;
            try
            {
                ImageCriteriaModel criteria = new ImageCriteriaModel { Sector = "QuoteBG" };
                var result = Database.LoadImagesByCriteria(criteria, rowsPerPage, pageNumber);

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult CreateImage(string sec, int? relatedObjectId = null)
        {
            Image model = new Image { Sector = sec, RelatedObjectId = relatedObjectId };
            return View(model);
        }
        [HttpPost]
        public ActionResult CreateImage(Image model, HttpPostedFileBase postedFile)
        {
            try
            {
                bool watchSize = false;
                int minHeight = 0;
                int maxHeight = 0;
                int minWidth = 0;
                int maxWidth = 0;

                switch (model.Sector)
                {
                    case "AboutBG": watchSize = false; minHeight = maxHeight = 205; minWidth = maxWidth = 360; break;
                    case "HomeSlide": watchSize = false; minHeight = maxHeight = 532; minWidth = maxWidth = 1200; break;
                    case "Contact": watchSize = false; minHeight = maxHeight = 532; minWidth = maxWidth = 1200; break;
                    case "ServiceBG": watchSize = false; minHeight = maxHeight = 532; minWidth = maxWidth = 1200; break;
                    case "CaseStudyBG": watchSize = false; minHeight = maxHeight = 532; minWidth = maxWidth = 1200; break;
                    case "BlogBG": watchSize = false; minHeight = maxHeight = 532; minWidth = maxWidth = 1200; break;
                    case "HomeBG": watchSize = false; minHeight = maxHeight = 532; minWidth = maxWidth = 1200; break;
                    case "About": watchSize = false; minHeight = maxHeight = 532; minWidth = maxWidth = 1200; break;
                    case "AboutVision": watchSize = false; minHeight = maxHeight = 532; minWidth = maxWidth = 1200; break;
                    case "AboutMission": watchSize = false; minHeight = maxHeight = 532; minWidth = maxWidth = 1200; break;
                    case "TestimonialBG": watchSize = false; minHeight = maxHeight = 532; minWidth = maxWidth = 1200; break;
                    case "Quote": watchSize = false; minHeight = maxHeight = 532; minWidth = maxWidth = 1200; break;
                    case "QuoteBG": watchSize = false; minHeight = maxHeight = 532; minWidth = maxWidth = 1200; break;
                    default: break;
                }

                var result = Database.CreateImage(postedFile, model, minHeight, maxHeight, minWidth, maxWidth, watchSize);
                switch (model.Sector)
                {
                    case "AboutBG": return RedirectToAction("AboutPhoto", "Image");
                    case "HomeSlide": return RedirectToAction("HomeSlidePhotos", "Image");
                    case "Contact": return RedirectToAction("ContactPhoto", "Image");
                    case "ServiceBG": return RedirectToAction("ServicePhoto", "Image");
                    case "CaseStudyBG": return RedirectToAction("CaseStudyPhoto", "Image");
                    case "BlogBG": return RedirectToAction("BlogPhoto", "Image");
                    case "HomeBG": return RedirectToAction("HomePhoto", "Image");
                    case "About": return RedirectToAction("AboutInfoPhoto", "Image");
                    case "AboutVision": return RedirectToAction("AboutVisionPhoto", "Image");
                    case "AboutMission": return RedirectToAction("AboutMissionPhoto", "Image");
                    case "TestimonialBG": return RedirectToAction("TestimonialPhoto", "Image");
                    case "Quote": return RedirectToAction("QuotePhoto", "Image");
                    case "QuoteBG": return RedirectToAction("QuoteBGPhoto", "Image");
                    default: return RedirectToAction("Index", "Dashboard");
                }
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }

        public ActionResult CreateFile(string sec, int? relatedObjectId = null)
        {
            sec = "Brochure";
            Image model = new Image { Sector = sec, RelatedObjectId = relatedObjectId };
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateFile(Image model, HttpPostedFileBase postedFile)
        {
            try
            {
                model.Sector = "Brochure";
                var result = Database.CreateFile(postedFile, model);
                switch (model.Sector)
                {
                    case "Brochure": return RedirectToAction("BrochureFile", "Image");
                    default: return RedirectToAction("Index", "Dashboard");
                }
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }

        public ActionResult BrochureFile(int pageNumber = 1)
        {
            int rowsPerPage = 3;
            try
            {
                ImageCriteriaModel criteria = new ImageCriteriaModel { Sector = "Brochure" };
                var result = Database.LoadImagesByCriteria(criteria, rowsPerPage, pageNumber);

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
            string sector = string.Empty;

            try
            {
                sector = Database.DeleteImage(id).Sector;
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Success, "Photo deleted successfully");
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult DeleteFile(int id)
        {
            string sector = string.Empty;

            try
            {
                sector = Database.DeleteFile(id).Sector;
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Success, "File deleted successfully");
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
            }

            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}
