using System.IO;
using OnlineExam.Helpers;
using OnlineExam.Infrastructure;
using OnlineExam.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OnlineExam.Repositories;
using OnlineExam.Utils;
using OnlineExam.Infrastructure.Alerts;
using OnlineExam.Infrastructure.Attributes;
using System.Security.Claims;
using Microsoft.Owin.Security;

namespace OnlineExam.Controllers
{
    [Authorize()]
    public class UsersAdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public UsersAdminController()
        {
        }

        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /Users/
        [Authorize(Roles = "Admin, StaffAdmin")]
        public ActionResult Index()
        {
            var tuple = new Tuple<List<UserViewModel>, RegisterViewModel>(new List<UserViewModel>(), new RegisterViewModel());
            //var roles = GetRolesByLoggedInUser();
            //if (!CustomClaimsPrincipal.Current.IsAusVisaUser &&
            //    CustomClaimsPrincipal.Current.CurrentRole == AppConstants.Roles.ClientAdmin)
            //{
            //    tuple.Item1.AddRange((from u in UserManager.Users.Include("UserProfile").Include("Company")
            //                          where u.CompanyId == UserManager.Users.Where(x=>x.Id==CustomClaimsPrincipal.Current.UserId).Select(x=>x.CompanyId).FirstOrDefault()

            //                          select new UserViewModel
            //                          {
            //                              UserId = u.Id,
            //                              Email = u.Email,
            //                              Name = u.UserProfile != null ? u.UserProfile.FirstName + " " + u.UserProfile.LastName : "NA",
            //                              Company = u.Company != null ? u.Company.Name : "NA",
            //                              Status = u.Status
            //                          }).ToList());
            //}
            //else
            //{
            //    tuple.Item1.AddRange((from u in UserManager.Users.Include("UserProfile").Include("Company")
            //                          //  where u.Status == AppConstants.UserStatus.Active
            //                          select new UserViewModel
            //                          {
            //                              UserId = u.Id,
            //                              Email = u.Email,
            //                              Name = u.UserProfile != null ? u.UserProfile.FirstName + " " + u.UserProfile.LastName : "NA",
            //                              Company = u.Company != null ? u.Company.Name : "NA",
            //                              Status = u.Status
            //                          }).ToList()); 
            //}

            tuple.Item1.AddRange((from u in UserManager.Users.Include("UserProfile")
                                  //  where u.Status == AppConstants.UserStatus.Active
                                  select new UserViewModel
                                  {
                                      UserId = u.Id,
                                      Email = u.Email,
                                      Name = u.UserProfile != null ? u.UserProfile.FirstName + " " + u.UserProfile.LastName : "NA",
                                      Status = u.Status,
                                      CreatedOn = u.UserProfile.CreatedOn
                                  }).ToList());

            tuple.Item2.Roles = RoleManager.Roles.ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id
            }).ToList();


            foreach (var user in tuple.Item1)
            {
                user.Role = user.Role = UserManager.GetRoles(user.UserId).FirstOrDefault();
            }

            if (tuple.Item1.Any(x => x.UserId == CustomClaimsPrincipal.Current.UserId))
            {
                var currentUser = tuple.Item1.First(x => x.UserId == CustomClaimsPrincipal.Current.UserId);
                // REMOVE LOGGED-IN USER
                if (currentUser != null)
                    tuple.Item1.Remove(currentUser);
            }

            return View(tuple);
        }
        [HttpPost]

        [MultipleButton(Name = "Action", Argument = "CreateUser")]
        public async Task<ActionResult> CreateUser([Bind(Prefix = "Item2")]  RegisterViewModel registerViewModel)
        {
            //var isEmailAusVisa = false;
            //isEmailAusVisa = registerViewModel.Email.Split('@')[1] == AppConstants.DomainName
            //   ? true
            //   : false;

            var user = new ApplicationUser
            {
                IsAgreementAccpeted = true,
                UserType = "",
                UserName = registerViewModel.Email,
                Email = registerViewModel.Email,
                // ReferedBy = AppConstants.MyRMA,
                Status = AppConstants.UserStatus.Created,
                //CompanyId = CustomClaimsPrincipal.Current.IsAusVisaUser?null:UserManager.Users.Where(x=>x.Id==CustomClaimsPrincipal.Current.UserId).Select(x=>x.CompanyId).FirstOrDefault(),

                UserProfile =
                    new UserProfile
                    {
                        FirstName = registerViewModel.FirstName,
                        LastName = registerViewModel.LastName,
                        DOB = registerViewModel.DOB
                    }
            };


            var result = await UserManager.CreateAsync(user, registerViewModel.Password);
            if (result.Succeeded)
            {
                string roleName = RoleManager.FindById(registerViewModel.RoleId).Name;
                result = UserManager.AddToRole(user.Id, roleName);

                var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = Url.Action("ConfirmEmail", "Account",
                    new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await
                    UserManager.SendEmailAsync(user.Id, "Confirm your account",
                        "Please confirm your account by clicking this link: <a href=\"" + callbackUrl +
                        "\">link</a>");
                ViewBag.Link = callbackUrl;


                // else AddErrors(result);
            }
            //AddErrors(result);
            return RedirectToAction("Index").WithSuccess("User has been created successfully.Account Will activate after email confirmation.");

        }
        public List<IdentityRole> GetRolesByLoggedInUser()
        {
            var roles = CustomClaimsPrincipal.Current.IsAusVisaUser
                 ? RoleManager.Roles.ToList()
                 : RoleManager.Roles.Where(e => e.Name.Contains(AppConstants.Client) && e.Name != AppConstants.Roles.ProspectiveClient).ToList();

            if (!string.IsNullOrWhiteSpace(CustomClaimsPrincipal.Current.CurrentRole))
            {
                switch (CustomClaimsPrincipal.Current.CurrentRole)
                {
                    case AppConstants.Roles.StaffAdmin: // DO NOTHING
                        break;
                    case AppConstants.Roles.StaffManager: // REMOVE COMPANY ADMIN
                        roles.RemoveAll(e => e.Name == AppConstants.Roles.StaffAdmin);
                        break;
                    case AppConstants.Roles.StaffOperator: // REMOVE COMPANY ADMIN AND MANAGER
                        roles.RemoveAll(
                            e => e.Name == AppConstants.Roles.StaffAdmin || e.Name == AppConstants.Roles.StaffManager);
                        break;
                    // CLIENT
                    case AppConstants.Roles.ClientAdmin: // DO NOTHING
                        break;
                    case AppConstants.Roles.ClientManager: // REMOVE CLIENT ADMIN
                        roles.RemoveAll(e => e.Name == AppConstants.Roles.ClientAdmin);
                        break;
                    case AppConstants.Roles.ClientOperator: // REMOVE CLIENT ADMIN & MANAGER
                        roles.RemoveAll(
                            e => e.Name == AppConstants.Roles.ClientAdmin || e.Name == AppConstants.Roles.ClientManager);
                        break;
                    case AppConstants.Roles.Visitor: // DO NOTHING
                        break;
                }
            }
            else
            {
                roles = new List<IdentityRole>();
            }
            return roles;
        }




        //
        // GET: /Users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            return View(user);
        }

        //
        // GET: /Users/Create
        [Authorize(Roles = "Admin, StaffAdmin")]
        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }

        //
        // POST: /Users/Create
        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = userViewModel.Email, Email = userViewModel.Email };
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                //Add User to the selected Roles 
                if (adminresult.Succeeded)
                {
                    if (selectedRoles != null)
                    {
                        var result = await UserManager.AddUserToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    return View();

                }
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            return View();
        }

        //
        // GET: /Users/Edit/1
        [HttpGet]

        public async Task<ActionResult> MyProfile()
        {
            string id = User.Identity.GetUserId();
            return await EditUserDetail(id, true);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            var userObj = db.Users.Include("UserProfile").FirstOrDefault(a => a.Id == id);

            int profileId = userObj.UserProfile.UserProfileId;

            return await EditUserDetail(id, false);
        }
        [HttpPost]
        private async Task<ActionResult> EditUserDetail(string id, bool isMyProfile)
        {
            var roles = GetRolesByLoggedInUser();

            if (id == null || roles == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //var userObj = db.Users.Include("UserProfile").Include("Company").FirstOrDefault(a => a.Id == id);
            var userObj = db.Users.Include("UserProfile").FirstOrDefault(a => a.Id == id);
            if (userObj == null)
            {
                return HttpNotFound();
            }

            var userRoles = await UserManager.GetRolesAsync(userObj.Id);

            // var UsrProfile = db.UserProfiles.Where(a => a.ApplicationUser.Id == id).Select(s=>s).ToList(); 

            var editUserViewModel = new EditUserViewModel
            {
                IsMyProfile = isMyProfile,
                Id = userObj.Id,
                DOB = Convert.ToDateTime(userObj.UserProfile.DOB),
                Status = userObj.Status,
                ClassName = userObj.UserProfile.ClassTypes,
                UserType = userObj.UserType,
                Email = userObj.Email,
                Address = userObj.UserProfile.AddressLine1,
                City = userObj.UserProfile.City,
                State = userObj.UserProfile.State,
                PrimaryContactNo = userObj.PhoneNumber,
                SecondaryContactNo = userObj.UserProfile.SecondaryContactNo,
                //CompanyId= userObj.CompanyId==null?0:(int)userObj.CompanyId,

                SchoolName = db.Schools.Any(x => x.Id.ToString() == userObj.UserProfile.SchoolName) ? userObj.UserProfile.SchoolName : "Others",
                OthersName = db.Schools.Any(x => x.Id.ToString() == userObj.UserProfile.SchoolName) ? "" : userObj.UserProfile.SchoolName,
                SchoolAddress = userObj.UserProfile.SchoolAddress,
                Hobbies = userObj.UserProfile.Hobbies,
                Avatar = userObj.UserProfile.Avatar,
                RoleId = userObj.Roles.Where(x => x.UserId == userObj.Id).Select(x => x.RoleId).FirstOrDefault(),
                CountryId = userObj.UserProfile.Country,
                Countries = db.Countries.ToList().Select(x => new SelectListItem()
                {
                    Selected = userObj.UserProfile != null && userObj.UserProfile.Country == x.CountryText,
                    Text = x.CountryText,
                    Value = x.CountryCode
                }).ToList(),
                States = db.States.Where(x => x.CountryCode == userObj.UserProfile.Country).ToList().Select(x => new SelectListItem()
                {
                    Selected = userObj.UserProfile != null && userObj.UserProfile.Country == x.CountryCode,
                    Text = x.StateText,
                    Value = x.StateCode
                }).ToList(),
                Cities = db.Cities.ToList().Select(x => new SelectListItem()
                {
                    Selected = userObj.UserProfile != null && userObj.UserProfile.State == x.StateCode,
                    Text = x.CityText,
                    Value = x.CityCode
                }).ToList(),
                Schools = db.Schools.ToList().Where(s => s.State == userObj.UserProfile.State && userObj.UserProfile.City == s.City && s.IsActive).Select(x => new SelectListItem()
                {
                    Selected = userObj.UserProfile != null && userObj.UserProfile.State == x.State && userObj.UserProfile.City == x.City && x.IsActive,
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList(),
                Roles = roles.Select(x => new SelectListItem()
                {
                    Selected = userRoles.FirstOrDefault() == x.Name,
                    Text = x.Name,
                    Value = x.Id
                }).ToList(),
                FirstName = userObj.UserProfile.FirstName,
                LastName = userObj.UserProfile.LastName,
                AccountName = userObj.UserProfile.AccountName,
                AccountNumber = userObj.UserProfile.AccountNumber,
                BankName = userObj.UserProfile.BankName,
                ReferrerEmail = userObj.UserProfile.ReferrerEmail,
                IsCorrectionRequired = userObj.UserProfile.IsCorrectionRequired,
                IsBankDetailReadOnly = userObj.UserProfile.IsBankDetailReadOnly
            };

            //if (!string.IsNullOrWhiteSpace(userObj.UserProfile.AccountNumber))
            //{
            //    editUserViewModel.IsBankDetailReadOnly = true;
            //}

            if (userObj.UserProfile.ClassTypes != null && userObj.UserProfile.ClassTypes.Contains("|"))
                editUserViewModel.SelectedClasses = userObj.UserProfile.ClassTypes.Split('|').ToArray();
            else
                editUserViewModel.SelectedClasses = new string[] { userObj.UserProfile.ClassTypes };

            // editUserViewModel.ClassName = userObj.UserProfile.ClassTypes;

            var newItem = new SelectListItem { Text = "Others", Value = "Others" };
            editUserViewModel.Schools.Add(newItem);

            ViewBag.Expertise = "";
            ViewBag.Qualifications = "";
            ViewBag.Offering = "";
            ViewBag.Lessons = "";

            int profileId = userObj.UserProfile.UserProfileId;
            if (db.TeachersProfileExtended.Where(x => x.UserProfileId == profileId).Count() > 0)
            {
                var extendedInfo = db.TeachersProfileExtended.FirstOrDefault(x => x.UserProfileId == profileId);
                ViewBag.Expertise = extendedInfo.Expertise;
                ViewBag.Qualifications = extendedInfo.Qualifications;
                ViewBag.Offering = extendedInfo.Offering;
                ViewBag.Lessons = extendedInfo.Lessons;
            }

            // ADMIN CAN CHANGE END USER'S CLASS, SO WE ARE LOADING ALL CLASSES
            if (!isMyProfile)
            {
                editUserViewModel.ClassTypes = CommonRepository.GetClasses(Enums.LookupType.ClassType.ToString()).ToList();
            }
            return View("Edit", editUserViewModel);
        }

        [HttpGet]
        public JsonResult GetStates(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                return Json(HttpNotFound());

            var stateList = CommonRepository.GetSateList(countryCode);
            return Json(stateList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCities(string stateCode)
        {
            if (string.IsNullOrWhiteSpace(stateCode))
                return Json(HttpNotFound());

            var cityList = CommonRepository.GetCitiesList(stateCode);
            return Json(cityList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSchools(string stateCode, string cityCode)
        {
            if (string.IsNullOrWhiteSpace(stateCode) || string.IsNullOrWhiteSpace(cityCode))
                return Json(HttpNotFound());

            var schools = CommonRepository.GetSchools(stateCode, cityCode);
            return Json(schools, JsonRequestBehavior.AllowGet);
        }
        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditUserViewModel editUserViewModel, HttpPostedFileBase file)
        {
            ApplicationUser user = db.Users.Include("UserProfile").FirstOrDefault(a => a.Id == editUserViewModel.Id);
            if (user == null)
            {
                return HttpNotFound();
            }

            if (editUserViewModel.SchoolName != "Others")
            {
                ModelState.Remove("OthersName");
            }

            if (!editUserViewModel.IsMyProfile && user.UserType == "Student")
            {
                if (editUserViewModel.SelectedClasses != null && editUserViewModel.SelectedClasses.Count() > 1)
                {
                    ModelState.AddModelError("", "Only one Class is allowed.");
                }
            }

            if (!editUserViewModel.IsMyProfile && (editUserViewModel.SelectedClasses == null || !editUserViewModel.SelectedClasses.Any()))
            {
                ModelState.AddModelError("", "Please select a Class.");
            }
            if (ModelState.IsValid)
            {
                foreach (object t in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files["file"];
                    if (hpf != null && hpf.ContentLength == 0)
                        continue;

                    if (hpf != null)
                    {
                        var fileName = Path.GetFileName(hpf.FileName);
                        var type = Path.GetExtension(hpf.FileName);
                        // You have to make sure ‘FileUploads’ directory exists 
                        // string savedFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "FileUploads/", fileName);
                        if (file != null)
                        {
                            byte[] fileBytes = new byte[file.ContentLength];
                            file.InputStream.Read(fileBytes, 0, file.ContentLength);

                            var fullPath = Path.Combine(Server.MapPath(CreateIfMissing("~/FileUploads/UserProfiles/" + editUserViewModel.Id) + "/"), fileName);

                            //var path = Path.Combine(Server.MapPath("~/FileUploads/"), fileName);
                            hpf.SaveAs(fullPath);
                            user.UserProfile.Avatar = "~/FileUploads/UserProfiles/" + editUserViewModel.Id + "/" + fileName;
                        }
                    }
                }
                if (!editUserViewModel.IsMyProfile)
                {
                    user.UserProfile.ClassTypes = editUserViewModel.SelectedClasses != null ? string.Join("|", editUserViewModel.SelectedClasses) : string.Empty;
                    // UPDATE ROLE
                    if (!string.IsNullOrWhiteSpace(editUserViewModel.RoleId))
                    {
                        string existingRole = string.Empty;
                        var roleDetail = RoleManager.FindById(editUserViewModel.RoleId);
                        var userRoles = UserManager.GetRoles(user.Id);
                        if (userRoles.Count > 0)
                        {
                            existingRole = Convert.ToString(userRoles[0]);
                        }
                        if (roleDetail != null)
                        {
                            if (roleDetail.Name != existingRole && !string.IsNullOrWhiteSpace(existingRole))
                            {
                                UserManager.RemoveFromRole(user.Id, existingRole);
                            }
                            UserManager.AddToRole(user.Id, roleDetail.Name);
                        }
                    }
                }
                else
                {
                    user.UserProfile.ClassTypes = editUserViewModel.ClassName;
                }

                // ONLY ADMIN CAN ACTIVATE ANY END USER ACCOUNT
                // ADMIN CAN CHANGE END USER'S CLASS ALSO
                // TBD: EMAIL NOTIFICATION SHOULD ALSO SEND ON ACTIVATION/DEACTIVATION & CLASS CHANGE
                if (user.Email != CustomClaimsPrincipal.Current.CurrentUserEmail)
                {
                    user.Status = editUserViewModel.Status == "true" ? AppConstants.UserStatus.Active : AppConstants.UserStatus.Created;
                    user.EmailConfirmed = editUserViewModel.Status == "true" ? true : false;
                    //user.UserProfile.ClassTypes = editUserViewModel.ClassName;
                }
                user.UserProfile.Email = editUserViewModel.Email;
                user.UserProfile.Hobbies = editUserViewModel.Hobbies;
                user.UserProfile.FirstName = editUserViewModel.FirstName;
                user.UserProfile.LastName = editUserViewModel.LastName;
                user.UserProfile.AddressLine1 = editUserViewModel.Address;
                user.UserProfile.DOB = editUserViewModel.DOB;
                user.UserProfile.City = editUserViewModel.City;
                user.UserProfile.State = editUserViewModel.State;
                user.UserProfile.Country = editUserViewModel.CountryId;
                //user.UserProfile.Status = editUserViewModel.State;
                user.UserProfile.City = editUserViewModel.City;
                user.PhoneNumber = editUserViewModel.PrimaryContactNo;
                user.UserProfile.SecondaryContactNo = editUserViewModel.SecondaryContactNo;
                user.UserProfile.SchoolName = editUserViewModel.SchoolName == "Others" ? editUserViewModel.OthersName : editUserViewModel.SchoolName;
                user.UserProfile.SchoolAddress = editUserViewModel.SchoolAddress;
                user.UserProfile.Hobbies = editUserViewModel.Hobbies;

                user.UserProfile.AccountName = editUserViewModel.AccountName;
                user.UserProfile.AccountNumber = editUserViewModel.AccountNumber;
                user.UserProfile.BankName = editUserViewModel.BankName;
                user.UserProfile.IsCorrectionRequired = editUserViewModel.IsCorrectionRequired;
                //user.UserProfile.ReferrerEmail = editUserViewModel.ReferrerEmail;

                //db.Users.Attach(user);
                //db.Entry(user.UserProfile).State = EntityState.Modified;
                db.Entry(user).State = EntityState.Modified;
                var result = db.SaveChanges();

                //UPDATE AVATAR PATH
                if (result > 0)
                {
                    var authenticationManager = HttpContext.GetOwinContext().Authentication;
                    var identity = new ClaimsIdentity(User.Identity);
                    if (identity.Claims.Any(c => c.Type == "Avatar"))
                    {
                        var avatarClaimObj = identity.Claims.FirstOrDefault(c => c.Type == "Avatar");
                        if (avatarClaimObj != null && user.UserProfile.Avatar != avatarClaimObj.Value && !string.IsNullOrWhiteSpace(user.UserProfile.Avatar))
                        {
                            identity.RemoveClaim(identity.Claims.FirstOrDefault(c => c.Type == "Avatar"));
                            identity.AddClaim(new Claim("Avatar", user.UserProfile.Avatar));
                            authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant
                                 (new ClaimsPrincipal(identity), new AuthenticationProperties { IsPersistent = true });
                        }
                    }
                }

                //todo check if password change then update
                //if (user.PasswordHash != editUser.Password && editUser.Password != null)
                //{
                //    IdentityResult ResultD = await UserManager.RemovePasswordAsync(editUser.Id);
                //    IdentityResult ResultA = await UserManager.AddPasswordAsync(editUser.Id, editUser.Password);
                //}
                if (User.IsInRole("Teacher"))
                {
                    int profileId = user.UserProfile.UserProfileId;

                    if (db.TeachersProfileExtended.Where(x => x.UserProfileId == profileId).Count() > 0)
                    {
                        var extendedInfo = db.TeachersProfileExtended.FirstOrDefault(x => x.UserProfileId == profileId);
                        extendedInfo.Expertise = Request.Form["txtExpertise"];
                        extendedInfo.Qualifications = Request.Form["txtQualifications"];
                        extendedInfo.Offering = Request.Form["txtOffering"];
                        extendedInfo.Lessons = Request.Form["txtLesson"];
                        db.SaveChanges();
                    }
                    else
                    {
                        var extendedInfo = new TeachersProfileExtended();
                        extendedInfo.UserProfileId = profileId;
                        extendedInfo.Expertise = Request.Form["txtExpertise"];
                        extendedInfo.Qualifications = Request.Form["txtQualifications"];
                        extendedInfo.Offering = Request.Form["txtOffering"];
                        extendedInfo.Lessons = Request.Form["txtLesson"];

                        db.TeachersProfileExtended.Add(extendedInfo);
                        db.SaveChanges();
                    }
                }

                if (result == 0)
                {
                    // ModelState.AddModelError("", AppConstants.ErrorMessageText);
                    return View(editUserViewModel).WithError(AppConstants.ErrorMessageText);
                }

                if (!editUserViewModel.IsMyProfile)
                    return RedirectToAction("Index", "UsersAdmin").WithSuccess(AppConstants.SuccessMessageText);
                else
                    return RedirectToAction("Dashboard", "Account").WithSuccess(AppConstants.SuccessMessageText);
            }
            editUserViewModel.UserType = user.UserType;
            editUserViewModel.Countries = db.Countries.ToList().Select(x => new SelectListItem()
            {
                Selected = user.UserProfile != null && user.UserProfile.Country == x.CountryText,
                Text = x.CountryText,
                Value = x.CountryCode
            }).ToList();

            editUserViewModel.States = db.States.Where(x => x.CountryCode == user.UserProfile.Country).ToList().Select(x => new SelectListItem()
                {
                    Selected = user.UserProfile != null && user.UserProfile.Country == x.CountryCode,
                    Text = x.StateText,
                    Value = x.StateCode
                }).ToList();
            editUserViewModel.Cities = db.Cities.ToList().Select(x => new SelectListItem()
            {
                Selected = user.UserProfile != null && user.UserProfile.State == x.StateCode,
                Text = x.CityText,
                Value = x.CityCode
            }).ToList();
            editUserViewModel.Schools = db.Schools.ToList().Where(s => s.IsActive).Select(x => new SelectListItem()
            {
                Selected = user.UserProfile != null && user.UserProfile.State == x.State && user.UserProfile.City == x.City && x.IsActive,
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
            var newItem = new SelectListItem { Text = "Others", Value = "Others" };
            editUserViewModel.Schools.Add(newItem);

            editUserViewModel.ClassTypes = CommonRepository.GetClasses(Enums.LookupType.ClassType.ToString()).ToList();
            return View(editUserViewModel);


        }
        private string CreateIfMissing(string path)
        {
            bool folderExists = Directory.Exists(Server.MapPath(path));
            if (!folderExists)
                Directory.CreateDirectory(Server.MapPath(path));
            return path;
        }
        //
        // GET: /Users/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        public ActionResult AgreementAccpetedRejected(string email, bool isAgreementAccpeted)
        {
            var userObj = db.Users.FirstOrDefault(x => x.Email == email || x.UserName == email);
            if (userObj != null)
            {
                if (userObj.IsAgreementAccpeted != isAgreementAccpeted)
                {
                    userObj.IsAgreementAccpeted = isAgreementAccpeted;
                    db.Entry(userObj).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Json(email, JsonRequestBehavior.AllowGet);
        }
        public ActionResult IsAgreementAccpeted(string email)
        {

            bool isUserAccpeted = false;
            var userObj = UserManager.Users.FirstOrDefault(x => x.Email == email || x.UserName == email);
            if (userObj != null)
            {
                isUserAccpeted = userObj.IsAgreementAccpeted || !userObj.EmailConfirmed;
            }
            return Json(isUserAccpeted, JsonRequestBehavior.AllowGet);
        }
        //
        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<ActionResult> UpdatePassword()
        {
            var code = await UserManager.GeneratePasswordResetTokenAsync(CustomClaimsPrincipal.Current.UserId);
            return RedirectToAction("ResetPassword", "Account", new { code = code });
        }
    }
}
