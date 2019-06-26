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

        public ActionResult UserNavPartial()
        {
            string username = User.Identity.Name;

            UserNavPartialViewModel model;
            using (Db db=new Db())
            {
                UserDTO dto = db.Users.FirstOrDefault(x=>x.Username == username);

                model = new UserNavPartialViewModel()
                {
                    FirstName = dto.FirstName,
                    LastName =dto.LastName
                };
            }    
            return PartialView(model);
        }

        [HttpGet]
        [ActionName("user-profile")]
        public ActionResult UserProfile()
        {
            string username = User.Identity.Name;

            UserProfileViewModel model;

            using (Db db=new Db())
            {
                UserDTO dto = db.Users.FirstOrDefault(x=>x.Username == username);

                model = new UserProfileViewModel(dto);
            }
           return View("UserProfile", model);
        }
        [HttpPost]
        [ActionName("user-profile")]
        public ActionResult UserProfile(UserProfileViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Password and Confirm password are not match");
                    return View("UserProfile", model);
                }
            }
            using (Db db = new Db())
            {
                string username = User.Identity.Name;
                if (db.Users.Where(x=>x.Id !=model.Id).Any(x=>x.Username == username))
                {
                    ModelState.AddModelError("","Username "+ model.Username+ " Alredy exist");
                    model.Username = "";
                    return View("UserProfile", model);
                }
                UserDTO dto = db.Users.Find(model.Id);

                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.EmailAddress = model.EmailAddress;
                dto.Username = model.Username;
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;
                }
                db.SaveChanges();
            }
            TempData["SM"] = "Successfully Registered";

            return Redirect("~/account/user-profile");
        }
    }
}


