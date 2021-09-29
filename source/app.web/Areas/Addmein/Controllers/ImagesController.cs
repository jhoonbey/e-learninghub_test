using app.domain.Enums;
using app.domain.Model.Entities;
using app.service;
using app.web.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace app.web.Areas.Addmein.Controllers
{
    [User(AllowedRole = EnumUserRole.Admin)]
    [Area("Addmein")]
    public class ImageController : BaseController
    {
        //private readonly IEntityService _ImageService;

        public ImageController(
                               ILogger<ImageController> logger,
                               //IImageService ImageService,
                               IEntityService entityService,
                               IConfiguration configuration,
                               ICipherService cipherService,
                               IHostingEnvironment hostingEnvironment
                              ) : base(logger, configuration, hostingEnvironment, entityService, cipherService)
        {
            //_ImageService = ImageService;
        }

        //public IActionResult ViewImage(string sec)
        //{
        //    var response = _ImageService.GetOrCreate(sec);
        //    if (response.IsSuccessfull)
        //    {
        //        _logger.LogInformation("ViewImage result.IsSuccessfull");
        //        return View(response.Model);
        //    }
        //    else
        //    {
        //        _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
        //        AddError(response.Key, response.ErrorForClient);
        //        TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
        //    }

        //    return RedirectToAction("Index", "Dashboard");
        //}

        //public IActionResult CreateImage(string sec, int? relatedObjectId = null)
        //{
        //    Image model = new Image { Sector = sec, RelatedObjectId = relatedObjectId };
        //    return View(model);
        //}

        //[HttpPost]
        //public IActionResult CreateImage(Image model, IFormFile postedFile)
        //{

        //    try
        //    {
        //        if (postedFile == null)
        //        {
        //            throw new Exception("Select image");
        //        }

        //        string newFullFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(postedFile.FileName);


        //        string imageName = MediaHelper.SaveImageCreate(postedFile, model.Id.ToString(), pathOnly, "Profile");


        //        TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Success, "Image saved successfully"));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.ToString());
        //        AddError(ex.ToString());
        //        TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, ex.ToString()));
        //    }



        //if (string.IsNullOrEmpty(imageName))
        //{
        //    throw new BusinessException("Error on image saving");
        //}



        //var result = Database.CreateImage(postedFile, model, minHeight, maxHeight, minWidth, maxWidth, watchSize);
        //switch (model.Sector)
        //{
        //    case "AboutBG": return RedirectToAction("AboutPhoto", "Image");
        //    case "HomeSlide": return RedirectToAction("HomeSlidePhotos", "Image");
        //    case "Contact": return RedirectToAction("ContactPhoto", "Image");
        //    case "ServiceBG": return RedirectToAction("ServicePhoto", "Image");
        //    case "CaseStudyBG": return RedirectToAction("CaseStudyPhoto", "Image");
        //    case "BlogBG": return RedirectToAction("BlogPhoto", "Image");
        //    case "HomeBG": return RedirectToAction("HomePhoto", "Image");
        //    case "About": return RedirectToAction("AboutInfoPhoto", "Image");
        //    case "AboutVision": return RedirectToAction("AboutVisionPhoto", "Image");
        //    case "AboutMission": return RedirectToAction("AboutMissionPhoto", "Image");
        //    case "TestimonialBG": return RedirectToAction("TestimonialPhoto", "Image");
        //    case "Quote": return RedirectToAction("QuotePhoto", "Image");
        //    case "QuoteBG": return RedirectToAction("QuoteBGPhoto", "Image");
        //    default: return RedirectToAction("Index", "Dashboard");
        //}

    }


    //[HttpPost]
    //public IActionResult ManageImage(Image model)
    //{
    //    if (model == null || string.IsNullOrEmpty(model.Sec))
    //        return RedirectToAction("Index", "Dashboard");

    //    Dictionary<string, object> columns = new Dictionary<string, object>{
    //                                                                            { "NameAZ", model.NameAZ },
    //                                                                            { "NameEN", model.NameEN },
    //                                                                            { "NameRU", model.NameRU },
    //                                                                         };
    //    var response = _entityService.UpdateBy<Image>(columns, "Sec", model.Sec);
    //    if (response.IsSuccessfull)
    //    {
    //        _logger.LogInformation("ManageImage post result.IsSuccessfull");
    //    }
    //    else
    //    {
    //        _logger.LogError($"{ MethodBase.GetCurrentMethod().Name + " - " + response.ErrorForLog}");
    //        AddError(response.Key, response.ErrorForClient);
    //        TempData.Put("RedirectAlert", FillAlertModel(AlertStatus.Error, response.ErrorForLog));
    //    }

    //    return RedirectToAction("ViewImage", "Image", new { model.Sec });
    //}
}
