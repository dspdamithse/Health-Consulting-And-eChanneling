using Health_Consulting_And_eChanneling.Models.Data;
using Health_Consulting_And_eChanneling.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
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
        public ActionResult CreateAccount(UserViewModel model, HttpPostedFileBase file)
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
            int userid;
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
                userid = userDTO.Id;
                UserRoleDTO userRoleDto = new UserRoleDTO()
                {
                    UserId = id,
                    RoleId = 2
                };
                db.UserRoles.Add(userRoleDto);
                db.SaveChanges();

            }
            TempData["SM"] = "Successfully Registered";
            var originalDirectory = new DirectoryInfo(string.Format("{0}Content", Server.MapPath(@"\")));

            string pathString1 = Path.Combine(originalDirectory.ToString(), "Registration\\Users");
            string pathString2 = Path.Combine(originalDirectory.ToString(), "Registration\\Users\\" + userid.ToString());
            string pathString3 = Path.Combine(originalDirectory.ToString(), "Registration\\Users\\" + userid.ToString() + "\\Thumbs");


            string pathString4 = Path.Combine(originalDirectory.ToString(), "MedicalImages\\" + userid.ToString());

            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);
            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);
            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);
            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);

            if (file != null && file.ContentLength > 0)
            {
                string ext = file.ContentType.ToLower();
                if (ext != "image/jpg" && ext != "image/jpeg" && ext != "image/pjpeg" && ext != "image/gif" && ext != "image/x-png" && ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        ModelState.AddModelError("", "Image was Not Uploaded- Image format is wrong");
                        return View("CreateAccont", model);
                    }
                }

                string imageName = file.FileName;
                using (Db db = new Db())
                {
                    UserDTO dto = db.Users.Find(userid);
                    dto.ProfileImage = imageName;

                    db.SaveChanges();
                }
                var path = string.Format("{0}\\{1}", pathString2, imageName);
                var path2 = string.Format("{0}\\{1}", pathString3, imageName);

                file.SaveAs(path);

                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }

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
        public ActionResult DoctorNavPartial()
        {
            string username = User.Identity.Name;

            UserNavPartialViewModel model;
            using (Db db = new Db())
            {
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == username);

                model = new UserNavPartialViewModel()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
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

            int id = 0;
            using (Db db=new Db())
            {
                UserDTO dto = db.Users.FirstOrDefault(x=>x.Username == username);
                id = dto.Id;

                model = new UserProfileViewModel(dto);
            }

            //Medical Records
            model.MedicalImages = Directory.EnumerateFiles(Server.MapPath("~/Content/MedicalImages/"+id+"/"))
                                               .Select(fn => Path.GetFileName(fn));
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


