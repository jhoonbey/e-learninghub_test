using app.domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;

namespace app.web.Core
{
    //public class MyCustomExceptionFilter : IExceptionFilter
    //{
    //    public void OnException(ExceptionContext context)
    //    {
    //        HttpStatusCode status = HttpStatusCode.InternalServerError;
    //        String message = String.Empty;

    //        var exceptionType = context.Exception.GetType();
    //        if (exceptionType == typeof(UnauthorizedAccessException))
    //        {
    //            message = "Unauthorized Access";
    //            status = HttpStatusCode.Unauthorized;
    //        }
    //        else if (exceptionType == typeof(NotImplementedException))
    //        {
    //            message = "A server error occurred.";
    //            status = HttpStatusCode.NotImplemented;
    //        }
    //        else if (exceptionType == typeof(BusinessException))
    //        {
    //            message = context.Exception.ToString();
    //            status = HttpStatusCode.NotImplemented;
    //        }
    //        else
    //        {
    //            message = context.Exception.Message;
    //            status = HttpStatusCode.NotFound;
    //        }
    //        context.ExceptionHandled = true;

    //        HttpResponse response = context.HttpContext.Response;
    //        response.StatusCode = (int)status;
    //        response.ContentType = "text/html";
    //        var err = "custom error: " + message + " " + context.Exception.StackTrace;
    //        response.WriteAsync(err);
    //    }
    //}

    //public class HandleCustomError : HandleErrorAttribute
    //{
    //    public override void OnException(ExceptionContext filterContext)
    //    {
    //        if (filterContext.ExceptionHandled)
    //        {
    //            return;
    //        }
    //        else
    //        {
    //            string actionName = filterContext.RouteData.Values["action"].ToString();
    //            string controllerName = filterContext.RouteData.Values["controller"].ToString();
    //            var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

    //            try
    //            {
    //                Type controllerType = filterContext.Controller.GetType();
    //                var method = controllerType.GetMethod(actionName);
    //                var returnType = method.ReturnType;

    //                if (returnType.Equals(typeof(ActionResult)) || (returnType).IsSubclassOf(typeof(ActionResult)))
    //                {
    //                    //filterContext.Result = new ViewResult
    //                    //{
    //                    //    ViewName = "Error"
    //                    //};
    //                    filterContext.Result = new ViewResult()
    //                    {
    //                        ViewName = "Error",
    //                        ViewData = new ViewDataDictionary(model)
    //                    };
    //                }
    //            }
    //            catch (Exception)
    //            {
    //                filterContext.Result = new ViewResult()
    //                {
    //                    ViewName = "Error",
    //                    ViewData = new ViewDataDictionary(model)
    //                };
    //            }

    //            //Exception ex = filterContext.Exception;
    //        }

    //        filterContext.ExceptionHandled = true;
    //    }
    //}
}
