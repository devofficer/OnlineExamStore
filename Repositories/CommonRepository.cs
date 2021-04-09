using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using OnlineExam.Helpers;
using OnlineExam.Infrastructure;
using OnlineExam.Models;
using OnlineExam.Utils;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;

namespace OnlineExam.Repositories
{
    public class CommonRepository
    {
        public static string GetUserClass(string userId)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var sp = @"SELECT ClassTypes FROM UserProfile WHERE ApplicationUser_Id='" + userId + "'";
                return dbContext.Database.SqlQuery<string>(sp).FirstOrDefault<string>();
            }
        }
        public static MembershipPlan GetMembershipPlan(string registerType)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                if (registerType == AppConstants.RegisterType.Demo)
                {
                    return dbContext.MembershipPlans.FirstOrDefault(x => x.MembershipPlanCode == AppConstants.TrailMembershipPlanCode && x.IsActive);
                }
                return dbContext.MembershipPlans.FirstOrDefault(x => x.MembershipPlanCode != AppConstants.TrailMembershipPlanCode && x.IsActive);
            }
        }
        public static Voucher GetDemoVoucher()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.Vouchers.FirstOrDefault(x => x.VoucherCode == AppConstants.DemoVoucherCode && x.IsActive);
            }
        }
        public static Voucher GetVoucherByCode(string voucherCode)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var voucherObj = dbContext.Vouchers.FirstOrDefault(x => x.VoucherCode == voucherCode && x.IsActive);
                if (voucherObj != null)
                {
                    if (dbContext.UserPlans.Any(p => p.VoucherId == voucherObj.Id))
                    {
                        return null;
                    }
                }
                return voucherObj;
            }
        }

        public static IEnumerable<SelectListItem> GetCountryList()
        {
            if (MemoryCache.Default.Contains("CountryList"))
            {
                // The CountryList already exists in the cache,
                return (IEnumerable<SelectListItem>)MemoryCache.Default.Get("CountryList");
            }
            else
            {
                // The select list does not yet exists in the cache, fetch items from the data store.

                var dbContext = new ApplicationDbContext();
                {

                    return from c in dbContext.Countries
                           select new SelectListItem
                           {
                               Value = c.CountryCode,
                               Text = c.CountryText
                           };

                }
            }
        }

        public static IEnumerable<SelectListItem> GetCountryContactCode()
        {
            if (MemoryCache.Default.Contains("GetCountryContactCode"))
            {
                // The CountryList already exists in the cache,
                return (IEnumerable<SelectListItem>)MemoryCache.Default.Get("GetCountryContactCode");
            }
            else
            {
                // The select list does not yet exists in the cache, fetch items from the data store.

                var dbContext = new ApplicationDbContext();
                {

                    return from c in dbContext.Countries
                           select new SelectListItem
                           {
                               Value = c.ContactCountryCode,
                               Text = c.CountryText + "(" + c.ContactCountryCode + ")"
                           };

                }
            }
        }

        public static IEnumerable<SelectListItem> GetSateList(string CountryCode)
        {
            if (MemoryCache.Default.Contains("GetSateList"))
            {
                // The CountryList already exists in the cache,
                return (IEnumerable<SelectListItem>)MemoryCache.Default.Get("GetSateList");
            }
            else
            {
                // The select list does not yet exists in the cache, fetch items from the data store.

                var dbContext = new ApplicationDbContext();
                {

                    return from c in dbContext.States.Where(x => x.CountryCode == CountryCode)
                           select new SelectListItem
                           {
                               Value = c.StateCode,
                               Text = c.StateText
                           };

                }
            }
        }
        public static IEnumerable<SelectListItem> GetCitiesList(string stateCode)
        {
            if (MemoryCache.Default.Contains("GetCitiesList"))
            {
                // The CitiesList already exists in the cache,
                return (IEnumerable<SelectListItem>)MemoryCache.Default.Get("GetCitiesList");
            }
            else
            {
                // The select list does not yet exists in the cache, fetch items from the data store.

                var dbContext = new ApplicationDbContext();
                {

                    return from c in dbContext.Cities.Where(x => x.StateCode == stateCode)
                           select new SelectListItem
                           {
                               Value = c.CityText,
                               Text = c.CityText
                           };

                }
            }
        }
        public static IEnumerable<SelectListItem> GetSchools(string stateCode, string cityCode)
        {
            if (MemoryCache.Default.Contains("GetSchools"))
            {
                // The CitiesList already exists in the cache,
                return (IEnumerable<SelectListItem>)MemoryCache.Default.Get("GetSchools");
            }
            else
            {
                // The select list does not yet exists in the cache, fetch items from the data store.

                var dbContext = new ApplicationDbContext();
                {

                    var schools = (from c in dbContext.Schools.Where(x => x.State == stateCode && x.City == cityCode && x.IsActive && x.Status == "Approved")
                                   select new SelectListItem
                                   {
                                       Value = c.Id.ToString(),
                                       Text = c.Name
                                   }).ToList();
                    var newItem = new SelectListItem { Text = "Others", Value = "Others" };
                    schools.Add(newItem);
                    return schools;
                }
            }
        }

        public static IEnumerable<SelectListItem> GetClasses(string moduleCode)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                if (CustomClaimsPrincipal.Current.IsACDAStoreUser)
                {
                    return dbContext.Lookup.Where(x => x.ModuleCode == moduleCode && x.IsActive)
                    .ToList()
                    .Select(x => new SelectListItem
                    {
                        Value = x.Value.Trim(),
                        Text = x.Text.Trim()
                    });
                }
                else
                {
                    string[] classTypes = CustomClaimsPrincipal.Current.ClassTypes.Split('|');
                    return dbContext.Lookup.Where(x => x.ModuleCode == moduleCode && x.IsActive && classTypes.Contains(x.Value))
                        .ToList()
                        .Select(x => new SelectListItem
                        {
                            Value = x.Value.Trim(),
                            Text = x.Text.Trim()
                        });
                }
            }
        }
        public static IEnumerable<SelectListItem> GetLookups(string moduleCode)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.Lookup.Where(x => x.ModuleCode == moduleCode && x.IsActive)
                    .ToList()
                    .Select(x => new SelectListItem
                    {
                        Value = x.Value.Trim(),
                        Text = x.Text.Trim()
                    }).OrderBy(x => x.Text);
            }
        }

        public static int GetCustomCBTCounter()
        {
            int counter = 1;//MIN QUESTION
            using (var dbContext = new ApplicationDbContext())
            {
                if (dbContext.Lookup.Any(x => x.ModuleCode == Enums.LookupType.CustomCBTCounter.ToString() && x.IsActive))
                {
                    counter = Convert.ToInt32(dbContext.Lookup.FirstOrDefault(x => x.ModuleCode == Enums.LookupType.CustomCBTCounter.ToString() && x.IsActive).Value);
                }
            }
            return counter;
        }
        public static int GetReferralCounter()
        {
            int counter = 10;
            using (var dbContext = new ApplicationDbContext())
            {
                if (dbContext.Lookup.Any(x => x.ModuleCode == "ReferralCounter" && x.IsActive))
                {
                    counter = Convert.ToInt32(dbContext.Lookup.FirstOrDefault(x => x.ModuleCode == "ReferralCounter" && x.IsActive).Value);
                }
            }
            return counter;
        }
        public static int GetVoucherCodeDigitCounter()
        {
            int counter = 6;//MIN DIGITS
            using (var dbContext = new ApplicationDbContext())
            {
                if (dbContext.Lookup.Any(x => x.ModuleCode == Enums.LookupType.VoucherCodeDigitCounter.ToString() && x.IsActive))
                {
                    counter = Convert.ToInt32(dbContext.Lookup.FirstOrDefault(x => x.ModuleCode == Enums.LookupType.VoucherCodeDigitCounter.ToString() && x.IsActive).Value);
                }
            }
            return counter;
        }


        public static IEnumerable<SelectListItem> GetMemberships()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.MembershipPlans.Where(x => x.IsActive)
                    .ToList().SkipWhile(x => x.Name == "Trial")// TBD: SPELLING MISTAKE
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name.Trim()
                    });
            }
        }
        public static IEnumerable<SelectListItem> GetVendors()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.Vendors.Where(x => x.IsActive).OrderBy(x => x.Name)//.SkipWhile(x => x.VendorCode == "JHSXX" && x.VendorCode == "BANK1")
                    .ToList().Where(x => x.VendorCode != "JHSXX" && x.VendorCode != "BANK1")
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name.Trim()
                    });
            }
        }
        public static IEnumerable<SelectListItem> GetLookups(string moduleCode, string filter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.Lookup.Where(x => x.ModuleCode == moduleCode && x.IsActive && x.Parent == filter)
                    .ToList()
                    .Select(x => new SelectListItem
                    {
                        Value = x.Value.Trim(),
                        Text = x.Text.Trim()
                    });
            }
        }
        public static IEnumerable<SelectListItem> GetTopics(string subject)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var topics = (from s in dbContext.Subjects
                              join t in dbContext.Topics
                              on s.Id equals t.SubjectId
                              where s.Name == subject && s.IsActive == true && t.IsActive == true
                              select new SelectListItem
                              {
                                  Text = t.Name,
                                  Value = t.Id.ToString()
                              }).ToList();
                return topics;
            }
        }
        public static UserPlanViewModel GetUserBankPaymentDetails(string moduleCode)
        {
            var userPlanViewModelObj = new UserPlanViewModel();
            using (var dbContext = new ApplicationDbContext())
            {
               // var lookupList = dbContext.Lookup.Where(x => x.ModuleCode == moduleCode && x.IsActive).ToList()
                //    .GroupBy(e => e.ModuleCode).Select(m => new UserPlanViewModel { Despositor = m.Key });

            }
            return userPlanViewModelObj;
        }
        public static string ConvertSecToMinutes(int seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);

            //here backslash is must to tell that colon is
            //not the part of format, it just a character that we want in output
            return time.ToString(@"hh\:mm\:ss");
        }
        public static int GetPercentile(int obtainedMarks, int totalMarks)
        {
            return (int)Math.Round((double)(100 * obtainedMarks) / totalMarks); ;
        }

        public static IEnumerable<SelectListItem> GetAllClassTypesList()
        {
            var dbContext = new ApplicationDbContext();
            var classTypesList = dbContext.Lookup.Where(x => x.ModuleCode == "ClassType" && x.IsActive)
                           .ToList()
                           .Select(x => new SelectListItem
                           {
                               Value = x.Value.Trim(),
                               Text = x.Text.Trim()
                           });

            return classTypesList;
        }

        public static IEnumerable<SelectListItem> GetCurrentTeacherClassTypesList()
        {
            var dbContext = new ApplicationDbContext();
            string userId = CustomClaimsPrincipal.Current.UserId;
            string[] classTypes = dbContext.UserProfiles.FirstOrDefault(x => x.ApplicationUser.Id == userId).ClassTypes.Split('|');
           // string[] classCategoryTypes = dbContext.Lookup.Where(x => x.ModuleCode == "SubjectCategory" && x.IsActive && subjectCategoryTypes.Contains(x.Value)).Select(x => x.Parent).ToArray();
           // string[] classTypes = dbContext.Lookup.Where(x => x.ModuleCode == "ClassCategory" && x.IsActive && classCategoryTypes.Contains(x.Value)).Select(x => x.Parent).ToArray();
            var classTypesList = dbContext.Lookup.Where(x => x.ModuleCode == "ClassType" && x.IsActive && classTypes.Contains(x.Value))
                           .ToList()
                           .Select(x => new SelectListItem
                           {
                               Value = x.Value.Trim(),
                               Text = x.Text.Trim()
                           });

            return classTypesList;
        }

        public static IEnumerable<SelectListItem> GetCurrentTeacherSubjectCategoryList()
        {
            var dbContext = new ApplicationDbContext();
            string userId = CustomClaimsPrincipal.Current.UserId;
            string[] subjectCategory = dbContext.UserProfiles.FirstOrDefault(x => x.ApplicationUser.Id == userId).SubjectCategory.Split('|');
            var subjectCategoryList = dbContext.Lookup.Where(x => x.ModuleCode == "SubjectCategory" && x.IsActive && subjectCategory.Contains(x.Value))
                           .ToList()
                           .Select(x => new SelectListItem
                           {
                               Value = x.Value.Trim(),
                               Text = x.Text.Trim()
                           }).DistinctBy(x=>x.Value);

            return subjectCategoryList;
        }

        public static string GetTeacherClassTypes(string sujectCategory)
        {
            string strClassTypes = "";
            var dbContext = new ApplicationDbContext();
            string[] subjectCategoryTypes = sujectCategory.Split('|');
            string[] classCategoryTypes = dbContext.Lookup.Where(x => x.ModuleCode == "SubjectCategory" && x.IsActive && subjectCategoryTypes.Contains(x.Value)).Select(x => x.Parent).ToArray();
            string[] classTypes = dbContext.Lookup.Where(x => x.ModuleCode == "ClassCategory" && x.IsActive && classCategoryTypes.Contains(x.Value)).Select(x => x.Parent).ToArray();
            var classTypesList = dbContext.Lookup.Where(x => x.ModuleCode == "ClassType" && x.IsActive && classTypes.Contains(x.Value))
                           .ToList();
            foreach(var item in classTypesList)
            {
                if (strClassTypes == "")
                    strClassTypes = item.Value;
                else
                    strClassTypes += "|" + item.Value;
            }              

            return strClassTypes;
        }

        public static IEnumerable<SelectListItem> GetSubjectCategoryByClassTypes(string classTypes)
        {
            var dbContext = new ApplicationDbContext();
            string[] classTypesArray = classTypes.Split('|');
            string[] classCategoryTypes = dbContext.Lookup.Where(x => x.ModuleCode == "ClassCategory" && x.IsActive && classTypesArray.Contains(x.Parent)).Select(x => x.Value).ToArray();
            var subjectList = dbContext.Lookup.Where(x => x.ModuleCode == "SubjectCategory" && x.IsActive && classCategoryTypes.Contains(x.Parent))
                            .ToList()
                           .Select(x => new SelectListItem
                           {
                               Value = x.Value.Trim(),
                               Text = x.Text.Trim()
                           }).DistinctBy(x => x.Value);

            return subjectList;
        }

        public static int GetCurrentUserProfileId()
        {
            string currentUserId = CustomClaimsPrincipal.Current.UserId;
            int userId = 0;
            var dbContext = new ApplicationDbContext();
            userId = dbContext.UserProfiles.FirstOrDefault(x => x.ApplicationUser.Id == currentUserId).UserProfileId;
            return userId;
        }

        public static string GetUserTypeByProfileId(int profileId)
        {
            string userType = "Free";
            try
            {
                var dbContext = new ApplicationDbContext();
                if (dbContext.UserProfiles.Where(x => x.UserProfileId == profileId).Count() > 0)
                {
                    string currentUserId = dbContext.UserProfiles.FirstOrDefault(x => x.UserProfileId == profileId).ApplicationUser.Id;
                    userType = dbContext.UserPlans.FirstOrDefault(x => x.UserId == currentUserId).MembershipPlanId == 1 ? "Free" : "Paid";
                }
            }
            catch
            {
                userType = "Free";
            }
            return userType;
        }
    }
}