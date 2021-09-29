using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;

namespace app.web.Filters
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<CustomExceptionFilterAttribute> _logger;

        public CustomExceptionFilterAttribute(ILogger<CustomExceptionFilterAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            string strLogText = "";
            Exception ex = context.Exception;
            context.ExceptionHandled = true;

            strLogText += "----------- CustomExceptionFilterAttribute - Message --------\n{0}" + ex.Message;

            if (context.HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest")
            {
                strLogText += Environment.NewLine + ".Net Error ---\n{0}" + "Check MVC Ajax Code For Error";
            }

            strLogText += Environment.NewLine + "Source ---\n{0}" + ex.Source;
            strLogText += Environment.NewLine + "StackTrace ---\n{0}" + ex.StackTrace;
            strLogText += Environment.NewLine + "TargetSite ---\n{0}" + ex.TargetSite;
            if (ex.InnerException != null)
            {
                strLogText += Environment.NewLine + "Inner Exception is {0}" + ex.InnerException;
            }
            if (ex.HelpLink != null)
            {
                strLogText += Environment.NewLine + "HelpLink ---\n{0}" + ex.HelpLink; //error prone
            }

            var controllerName = (string)context.RouteData.Values["controller"];
            var actionName = (string)context.RouteData.Values["action"];

            strLogText += Environment.NewLine + "ActionName ---\n{0}" + actionName;
            strLogText += Environment.NewLine + "ControllerName ---\n{0}" + controllerName;
            strLogText += "-----------END of Message --------\n{0}";

            _logger.LogError(strLogText);

            context.HttpContext.Session.Clear();

            var result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Error" } });

            // TODO: Pass additional detailed data via ViewData
            context.Result = result;
        }
    }
}
