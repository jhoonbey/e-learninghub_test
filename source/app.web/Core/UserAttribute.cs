using app.domain.Enums;
using app.domain.Model.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace app.web.Core
{
    public class UserAttribute : ActionFilterAttribute
    {
        public EnumUserRole AllowedRole { get; set; }

        public UserAttribute()
        {
        }

        private bool IsAuthorizedUser(BaseController controller)
        {
            if (controller.IsAuthorized)
            {
                //from Redis
                //var token = controller.GetFromCache(controller.Id.ToString());

                //From DB
                var response = controller._entityService.GetEntityBy<User>(new Dictionary<string, object> { { "Id", controller.Id } });
                if (response.IsSuccessfull)
                {
                    response.Model.Password = null;
                    controller.CurrentUser = response.Model;
                    controller.ViewBag.CurrentUser = controller.CurrentUser;

                    string token = controller.CreateAccessToken(response.Model);
                    if (token == controller.Token && response.Model.Id == controller.Id)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            BaseController controller = null;

            try
            {
                controller = filterContext.Controller as BaseController;
                if (controller != null)
                {
                    //bool isLoginPage = IsLoginPage(filterContext);
                    bool isAuthorizedUser = IsAuthorizedUser(controller);

                    if (!controller.IsAuthorized || !isAuthorizedUser)
                    {
                        controller.ClearAuthData();
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Start", controller = "Account", area = "" }));
                        return;
                    }

                    if (controller.CurrentUser.Role >= (int)EnumUserRole.Admin)
                    {
                        // it is ok, user can pass
                    }
                    else
                    {
                        if (controller.CurrentUser.Role < (int)AllowedRole)
                        {
                            controller.ClearAuthData();
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Start", controller = "Account", area = "" }));
                            return;
                        }
                    }
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Start", controller = "Account", area = "" }));
                    return;
                }
            }
            catch (Exception)
            {
                if (controller != null)
                {
                    controller.ClearAuthData();
                }

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Start", controller = "Account", area = "" }));
                return;
            }
        }
    }
}
