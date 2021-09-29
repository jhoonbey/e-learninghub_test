using app.web.client.Areas.Addmein.Models;
using app.Enums;
using app.Model.Criterias;
using app.Model.Entities;
using JhoonHelper;
using System;
using System.Linq;
using System.Web.Mvc;

namespace app.web.client.Areas.Addmein.Controllers
{
    public class AccountController : app.web.client.Core.BaseController
    {
        [User(AllowedRole = EnumUserRole.SuperAdmin)]
        public ActionResult List(int pageNumber = 1)
        {
            int rowsPerPage = 10;
            try
            {
                var result = Database.LoadUsersByCriteria(new UserCriteriaModel(), rowsPerPage, pageNumber);
                ViewBag.PageNumber = pageNumber;
                ViewBag.RowsPerPage = rowsPerPage;
                ViewBag.NumberOfPages = GetPageNumber(result.AllCount, rowsPerPage);

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Success, ex.Message);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [User(AllowedRole = EnumUserRole.SuperAdmin)]
        public ActionResult Create()
        {
            try
            {
                var extractList = new int[] { (int)EnumUserRole.SuperAdmin, (int)EnumUserRole.User };
                ViewBag.Roles = new SelectList(Database.LoadAllUserRoleEnumsWithout(extractList.ToList()), "Id", "Name");
                return View();
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("List", "Account");
            }
        }

        [HttpPost]
        [User(AllowedRole = EnumUserRole.SuperAdmin)]
        public ActionResult Create(User model)
        {
            try
            {
                var result = Database.CreateUser(model);
                return RedirectToAction("List", "Account");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                var extractList = new int[] { (int)EnumUserRole.SuperAdmin, (int)EnumUserRole.User };
                ViewBag.Roles = new SelectList(Database.LoadAllUserRoleEnumsWithout(extractList.ToList()), "Id", "Name", model.Role);
                return View(model);
            }
        }

        [User(AllowedRole = EnumUserRole.SuperAdmin)]
        public ActionResult Edit(int id)
        {
            try
            {
                var result = Database.GetUserById(id);
                var extractList = new int[] { (int)EnumUserRole.SuperAdmin, (int)EnumUserRole.User };
                ViewBag.Roles = new SelectList(Database.LoadAllUserRoleEnumsWithout(extractList.ToList()), "Id", "Name", result.Role);
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
                return RedirectToAction("List", "Account");
            }
        }

        [HttpPost]
        [User(AllowedRole = EnumUserRole.SuperAdmin)]
        public ActionResult Edit(User model)
        {
            try
            {
                var result = Database.EditUser(model);
                return RedirectToAction("List", "Account");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                var extractList = new int[] { (int)EnumUserRole.SuperAdmin, (int)EnumUserRole.User };
                ViewBag.Roles = new SelectList(Database.LoadAllUserRoleEnumsWithout(extractList.ToList()), "Id", "Name", model.Role);
                return View(model);
            }
        }

        [User(AllowedRole = EnumUserRole.SuperAdmin)]
        public ActionResult Delete(int id)
        {
            try
            {
                Database.DeleteUser(id);
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Success, "User deleted successfully");
            }
            catch (Exception ex)
            {
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Error, ex.Message);
            }
            return RedirectToAction("List", "Account", new { area = "Addmein" });
        }


        [User(AllowedRole = EnumUserRole.SuperAdmin)]
        public ActionResult ResetPassword(int id)
        {
            try
            {
                var result = Database.ResetUserPassword(SessionInfo.Id, SessionInfo.Role, id);
                return this.Json(new { success = true, pass = result, error = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return this.Json(new { success = false, pass = "", error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [User]
        public ActionResult ChangePassword()
        {
            return View();
        }


        [User]
        [HttpPost]
        public ActionResult ChangePassword(PasswordChangeModel model)
        {
            try
            {
                var result = Database.ChangeUserPassword(SessionInfo.Id, model.OldPassword, model.NewPassword, model.NewPasswordAgain);
                TempData["RedirectAlert"] = FillAlertModel(AlertStatus.Success, "Şifrəniz müvəffəqiyyətlə dəyişdirildi");

                //change password in cookie
                SessionInfo.ChangePassword(result);
                return RedirectToAction("ChangePasswordResult");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }

        [User]
        public ActionResult ChangePasswordResult()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            try
            {
                var result = Database.GetUserByUsernameAndPassword(model.Username, model.Password);
                if (result == null)
                {
                    throw new Exception("Uncorrect username or password");
                }
                else
                {
                    SessionInfo.IsAuthorized = true;
                    SessionInfo.Id = result.Id;
                    SessionInfo.Fullname = result.Fullname;
                    SessionInfo.Username = result.Username;
                    SessionInfo.Password = Reverse.E_C(result.Password);
                    SessionInfo.Role = result.Role;
                    SessionInfo.SaveValues();
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return View(model);
            }
        }

        public ActionResult Logout()
        {
            if (SessionInfo.IsAuthorized)
                SessionInfo.ClearValues();
            return RedirectToAction("Login", "Account");
        }
    }
}
