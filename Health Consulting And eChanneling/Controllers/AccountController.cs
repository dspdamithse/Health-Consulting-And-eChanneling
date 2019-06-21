using Health_Consulting_And_eChanneling.Models.Data;
using Health_Consulting_And_eChanneling.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Health_Consulting_And_eChanneling.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return Redirect("~/Account/Login");
        }
        [HttpGet]
        public ActionResult Login()
        {
            string username = User.Identity.Name;

            if (!string.IsNullOrEmpty(username))
                return RedirectToAction("user-profile");

            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            bool isValid = false;
            using (Db db= new Db())
            {
                if (db.Users.Any(x=>x.Username.Equals(model.Username) && x.Password.Equals(model.Password)) )
                {
                    isValid = true;
                }
                if (!isValid)
                {
                    ModelState.AddModelError("", "Invalid Credentials");
                    return View(model);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                    return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, model.RememberMe));
                }
            }
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return Redirect("~/account/login");
        }
        
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("","Password and Confirm password are not match");
                return View("CreateAccount", model);
            }
            using (Db db = new Db())
            {
                if (db.Users.Any(x=>x.Username.Equals(model.Username)))
                {
                    ModelState.AddModelError("", "Username "+model.Username+" Alredy exist");
                    model.Username = "";
                    return View("CreateAccount", model);
                }
                if (db.Users.Any(x => x.EmailAddress.Equals(model.EmailAddress)))
                {
                    ModelState.AddModelError("", "Email " + model.EmailAddress + " Alredy exist");
                    model.EmailAddress = "";
                    return View("CreateAccount", model);
                }
                UserDTO userDTO = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAddress = model.EmailAddress,
                    Username = model.Username,
                    Password = model.Password
                };

                db.Users.Add(userDTO);

                db.SaveChanges();

                int id = userDTO.Id;
                UserRoleDTO userRoleDto = new UserRoleDTO()
                {
                    UserId = id,
                    RoleId = 2
                };
                db.UserRoles.Add(userRoleDto);
                db.SaveChanges();

            }
            TempData["SM"] = "Successfully Registered";

            return Redirect("~/account/Login");
        }
    }
}
