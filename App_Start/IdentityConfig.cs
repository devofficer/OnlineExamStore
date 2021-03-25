using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using OnlineExam.Helpers;
using OnlineExam.Infrastructure;
using OnlineExam.Utils;
using System.Globalization;

namespace OnlineExam.Models
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is: {0}"
            });
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "SecurityCode",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        public UserProfile GetUserProfileById(string email)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.UserProfiles.FirstOrDefault(x => x.Email == email);
            }
        }
        /// <summary>
        /// Added by RAVI K SINGH on 27 DEC 2015
        /// To fetch User & User Profile data.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public override Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return Users.Include("UserProfile").Where(x => x.Email == email).FirstOrDefaultAsync();
        }


        //    return Users.Where(u => u.Email == email && u.TenantId.Equals(TenantId)).FirstOrDefaultAsync();
        //}
        /// <summary>
        /// Method to add user to multiple roles
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="roles">list of role names</param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> AddUserToRolesAsync(string userId, IList<string> roles)
        {
            var userRoleStore = (IUserRoleStore<ApplicationUser, string>)Store;

            var user = await FindByIdAsync(userId).ConfigureAwait(false);
            if (user == null)
            {
                throw new InvalidOperationException("Invalid user Id");
            }

            var userRoles = await userRoleStore.GetRolesAsync(user).ConfigureAwait(false);
            // Add user to each role using UserRoleStore
            foreach (var role in roles.Where(role => !userRoles.Contains(role)))
            {
                await userRoleStore.AddToRoleAsync(user, role).ConfigureAwait(false);
            }

            // Call update once when all roles are added
            return await UpdateAsync(user).ConfigureAwait(false);
        }

        /// <summary>
        /// Remove user from multiple roles
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="roles">list of role names</param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> RemoveUserFromRolesAsync(string userId, IList<string> roles)
        {
            var userRoleStore = (IUserRoleStore<ApplicationUser, string>)Store;

            var user = await FindByIdAsync(userId).ConfigureAwait(false);
            if (user == null)
            {
                throw new InvalidOperationException("Invalid user Id");
            }

            var userRoles = await userRoleStore.GetRolesAsync(user).ConfigureAwait(false);
            // Remove user to each role using UserRoleStore
            foreach (var role in roles.Where(userRoles.Contains))
            {
                await userRoleStore.RemoveFromRoleAsync(user, role).ConfigureAwait(false);
            }

            // Call update once when all roles are removed
            return await UpdateAsync(user).ConfigureAwait(false);
        }
    }

    // Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var manager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));

            return manager;
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {

#if DEBUG
            // Credentials:
            var host = ConfigurationManager.AppSettings["MailingHost"];
            var sentFrom = ConfigurationManager.AppSettings["FromMailAddress"];
            var sendGridPassword = ConfigurationManager.AppSettings["MailingPassword"];
            var port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
            // Configure the client:
            var client = new System.Net.Mail.SmtpClient(host, port);

            //client.Port = 587;
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            // Creatte the credentials:
            var credentials = new System.Net.NetworkCredential(sentFrom, sendGridPassword);

          //  client.EnableSsl = true;
            client.Credentials = credentials;

            // Create the message:
            var mail = new System.Net.Mail.MailMessage(sentFrom, message.Destination);
            mail.IsBodyHtml = true;
            mail.Subject = message.Subject;
            mail.Body = message.Body;
            //Send:
            try
            {
                return client.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                throw;
            }
            

#else

            //Create the msg object to be sent
            MailMessage msg = new MailMessage();
            //Add your email address to the recipients
            msg.To.Add(message.Destination);
            //Configure the address we are sending the mail from
            //MailAddress address = new MailAddress("no-reply@acadastore.com");
            MailAddress address = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["Prod_NoReplyMailAddress"]);
            msg.From = address;
            msg.Subject = message.Subject;
            msg.Body = message.Body;
            msg.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.Host = System.Configuration.ConfigurationManager.AppSettings["Prod_MailingHost"];
            client.Port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Prod_Port"]);
            client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["Prod_NoReplyMailAddress"], ConfigurationManager.AppSettings["Prod_MailingPassword"]);
            //Send the msg
            return client.SendMailAsync(msg);
#endif

        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your sms service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // This is useful if you do not want to tear down the database each time you run the application.
    // public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    // This example shows you how to create a new database if the Model changes
    public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(OnlineExam.Models.ApplicationDbContext context)
        {
            //THIS SEED FUNCTION IS NOT USING
            //  This method will be called after migrating to the latest version.
            if (HttpContext.Current == null) return;
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            const string name = "mcamail2002@acadastore.com";
            const string password = "India@123";

            #region ROLES
            context.Roles.AddOrUpdate(r => r.Name,
                 new IdentityRole { Name = "Student" },
                 new IdentityRole { Name = "Teacher" },
                 new IdentityRole { Name = "Parent" },
                 new IdentityRole { Name = "StaffAdmin" }
                 );
            #endregion

            #region LOOKUP

            context.Lookup.AddOrUpdate(l => l.Value, new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.UserType),
                Text = "Student",
                Value = "Student",
                Order = 1,
                Description = "Student",
                IsActive = true
            },

            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.UserType),
                Text = "Parent",
                Value = "Parent",
                Order = 2,
                Description = "Parent",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.UserType),
                Text = "Teacher",
                Value = "Teacher",
                Order = 2,
                Description = "Teacher",
                IsActive = true
            },
                //new Lookup
                //{
                //    ModuleCode = Convert.ToString(Enums.LookupType.Denomination),
                //    Text = "10",
                //    Value = "10",
                //    Order = 1,
                //    Description = "10",
                //    IsActive = true
                //},
                //new Lookup
                //{
                //    ModuleCode = Convert.ToString(Enums.LookupType.Denomination),
                //    Text = "20",
                //    Value = "20",
                //    Order = 2,
                //    Description = "20",
                //    IsActive = true
                //},

            //new Lookup
                //{
                //    ModuleCode = Convert.ToString(Enums.LookupType.Denomination),
                //    Text = "50",
                //    Value = "50",
                //    Order = 3,
                //    Description = "50",
                //    IsActive = true
                //},

            //new Lookup
                //{
                //    ModuleCode = Convert.ToString(Enums.LookupType.Denomination),
                //    Text = "100",
                //    Value = "100",
                //    Order = 4,
                //    Description = "100",
                //    IsActive = true
                //},
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.ClassType),
                Text = "Primary School",
                Value = "PS",
                Order = 1,
                Description = "Primary School",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.ClassType),
                Text = "JSS 1- JSS 3",
                Value = "JSS13",
                Order = 2,
                Description = "JSS 1- JSS 3",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.ClassType),
                Text = "SS 1- SS 3",
                Value = "SS13",
                Order = 3,
                Description = "SS 1- SS 3",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.ClassType),
                Text = "JAMBITE",
                Value = "JAMBITE",
                Order = 4,
                Description = "JAMBITE",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.ExamType),
                Text = "JUNIOR NECO",
                Value = "JN",
                Order = 1,
                Description = "JUNIOR NECO",
                IsActive = true
            }
            ,
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.ExamType),
                Text = "SENIOR WAEC",
                Value = "SW",
                Order = 2,
                Description = "SENIOR WAEC",
                IsActive = true
            }
            ,
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.ExamType),
                Text = "JAMB",
                Value = "JAMB",
                Order = 3,
                Description = "JAMB",
                IsActive = true
            }
            ,
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.ExamType),
                Text = "POST JAMB or POST UME",
                Value = "PJPU",
                Order = 4,
                Description = "POST JAMB or POST UME",
                IsActive = true
            }
            ,
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.Subject),
                Text = "English",
                Value = "English",
                Order = 1,
                Description = "English",
                IsActive = true
            }
            ,
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.Subject),
                Text = "Maths",
                Value = "Maths",
                Order = 2,
                Description = "Maths",
                IsActive = true
            }
            ,
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.Subject),
                Text = "Biology",
                Value = "Biology",
                Order = 3,
                Description = "Biology",
                IsActive = true
            }
            ,
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.Subject),
                Text = "Physics",
                Value = "Physics",
                Order = 4,
                Description = "Physics",
                IsActive = true
            }
            ,
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.Subject),
                Text = "Chemistry",
                Value = "Chemistry",
                Order = 5,
                Description = "Chemistry",
                IsActive = true
            }
            ,
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.Subject),
                Text = "Economics",
                Value = "Economics",
                Order = 6,
                Description = "Economics",
                IsActive = true
            }
            ,
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.Subject),
                Text = "Government",
                Value = "Government",
                Order = 7,
                Description = "Government",
                IsActive = true
            }
            ,
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.Subject),
                Text = "Geography",
                Value = "Geography",
                Order = 8,
                Description = "Geography",
                IsActive = true
            }
            ,
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.Subject),
                Text = "Commerce",
                Value = "Commerce",
                Order = 9,
                Description = "Commerce",
                IsActive = true
            }
            ,
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.Subject),
                Text = "Literature in English",
                Value = "LE",
                Order = 10,
                Description = "Literature in English",
                IsActive = true
            }
            ,
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.QuestionFormatType),
                Text = "Single Question,Multiple Answers",
                Value = "SQMA",
                Order = 1,
                Description = "Single Question,Multiple Answers",
                IsActive = true
            }
            ,
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.QuestionFormatType),
                Text = "Single Image, Single Question",
                Value = "SISQ",
                Order = 2,
                Description = "Single Image, Single Question",
                IsActive = true
            }
            ,
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.QuestionFormatType),
                Text = "Comprehension Passage",
                Value = "CP",
                Order = 3,
                Description = "Comprehension Passage",
                IsActive = true
            }
            ,
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.QuestionFormatType),
                Text = "Single Instruction, Multiple Questions",
                Value = "SIMQ",
                Order = 4,
                Description = "Single Instruction, Multiple Questions",
                IsActive = true
            }
            ,
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.QuestionFormatType),
                Text = "Single Image, Multiple Questions",
                Value = "OIMQ",
                Order = 5,
                Description = "Single Image, Multiple Questions",
                IsActive = true
            },
            #region Class Category
 new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.ClassCategory),
                Text = "JUNIOR NECO",
                Value = "JN",
                Order = 0,
                Description = "JUNIOR NECO",
                Parent = "PS",
                IsActive = true
            }
            ,
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.ClassCategory),
                Text = "JUNIOR NECO",
                Value = "JN",
                Order = 0,
                Description = "JUNIOR NECO",
                Parent = "JSS13",
                IsActive = true
            }
            ,
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.ClassCategory),
                Text = "SENIOR WAEC",
                Value = "SW",
                Order = 1,
                Description = "SENIOR WAEC",
                Parent = "JSS13",
                IsActive = true
            }
            ,
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.ClassCategory),
                Text = "SENIOR WAEC",
                Value = "SW",
                Order = 0,
                Description = "SENIOR WAEC",
                Parent = "SS13",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.ClassCategory),
                Text = "JAMB",
                Value = "JAMB",
                Order = 1,
                Description = "JAMB",
                Parent = "SS13",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.ClassCategory),
                Text = "JAMB",
                Value = "JAMB",
                Order = 0,
                Description = "JAMB",
                Parent = "JAMBITE",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.ClassCategory),
                Text = "SENIOR WAEC",
                Value = "SW",
                Order = 1,
                Description = "SENIOR WAEC",
                Parent = "JAMBITE",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.ClassCategory),
                Text = "POST JAMB or POST UME",
                Value = "PJPU",
                Order = 2,
                Description = "POST JAMB or POST UME",
                Parent = "JAMBITE",
                IsActive = true
            },
            #endregion
 new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "English",
                Value = "English",
                Order = 0,
                Description = "English",
                Parent = "JN",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "English",
                Value = "English",
                Order = 0,
                Description = "English",
                Parent = "JAMB",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "English",
                Value = "English",
                Order = 0,
                Description = "English",
                Parent = "PJPU",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Maths",
                Value = "Maths",
                Order = 1,
                Description = "Maths",
                Parent = "JN",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Maths",
                Value = "Maths",
                Order = 1,
                Description = "Maths",
                Parent = "JAMB",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Maths",
                Value = "Maths",
                Order = 1,
                Description = "Maths",
                Parent = "PJPU",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Biology",
                Value = "Biology",
                Order = 2,
                Description = "Biology",
                Parent = "SW",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Biology",
                Value = "Biology",
                Order = 2,
                Description = "Biology",
                Parent = "JAMB",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Biology",
                Value = "Biology",
                Order = 2,
                Description = "Biology",
                Parent = "PJPU",
                IsActive = true
            }
                ,
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Physics",
                Value = "Physics",
                Order = 3,
                Description = "Physics",
                Parent = "SW",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Physics",
                Value = "Physics",
                Order = 3,
                Description = "Physics",
                Parent = "JAMB",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Physics",
                Value = "Physics",
                Order = 3,
                Description = "Physics",
                Parent = "PJPU",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Chemistry",
                Value = "Chemistry",
                Order = 4,
                Description = "Chemistry",
                Parent = "SW",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Chemistry",
                Value = "Chemistry",
                Order = 4,
                Description = "Chemistry",
                Parent = "JAMB",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Chemistry",
                Value = "Chemistry",
                Order = 4,
                Description = "Chemistry",
                Parent = "PJPU",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Economics",
                Value = "Economics",
                Order = 5,
                Description = "Economics",
                Parent = "SW",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Economics",
                Value = "Economics",
                Order = 5,
                Description = "Economics",
                Parent = "JAMB",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Economics",
                Value = "Economics",
                Order = 5,
                Description = "Economics",
                Parent = "PJPU",
                IsActive = true
            },

            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Government",
                Value = "Government",
                Order = 6,
                Description = "Government",
                Parent = "SW",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Government",
                Value = "Government",
                Order = 6,
                Description = "Government",
                Parent = "JAMB",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Government",
                Value = "Government",
                Order = 6,
                Description = "Government",
                Parent = "PJPU",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Geography",
                Value = "Geography",
                Order = 7,
                Description = "Geography",
                Parent = "SW",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Geography",
                Value = "Geography",
                Order = 7,
                Description = "Geography",
                Parent = "JAMB",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Geography",
                Value = "Geography",
                Order = 7,
                Description = "Geography",
                Parent = "PJPU",
                IsActive = true
            },
            new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Commerce",
                Value = "Commerce",
                Order = 8,
                Description = "Commerce",
                Parent = "SW",
                IsActive = true
            }, new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Commerce",
                Value = "Commerce",
                Order = 8,
                Description = "Commerce",
                Parent = "JAMB",
                IsActive = true
            }, new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Commerce",
                Value = "Commerce",
                Order = 8,
                Description = "Commerce",
                Parent = "PJPU",
                IsActive = true
            }, new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Literature in English",
                Value = "LE",
                Order = 9,
                Description = "Literature in English",
                Parent = "SW",
                IsActive = true
            }, new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Literature in English",
                Value = "LE",
                Order = 9,
                Description = "Literature in English",
                Parent = "JAMB",
                IsActive = true
            }, new Lookup
            {
                ModuleCode = Convert.ToString(Enums.LookupType.SubjectCategory),
                Text = "Literature in English",
                Value = "LE",
                Order = 9,
                Description = "Literature in English",
                Parent = "PJPU",
                IsActive = true
            }


                );
            #endregion

            var user = userManager.FindByName(name);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = name,
                    Email = name,
                    EmailConfirmed = true,
                    Status = AppConstants.UserStatus.Active,
                    ReferedBy = AppConstants.MyRMA,
                    UserProfile = new UserProfile { FirstName = "Admin", LastName = "Admin", DOB = DateTime.Now }
                };
                var result = userManager.Create(user, password);
                if (result.Succeeded)
                    result = userManager.SetLockoutEnabled(user.Id, false);
            }

            // Add user admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(AppConstants.Roles.StaffAdmin))
            {
                var result = userManager.AddToRole(user.Id, AppConstants.Roles.StaffAdmin);
            }


            //#region POSITIONS
            //context.Positions.AddOrUpdate(p => p,
            //     new Position { PositionName = "RMA" },
            //     new Position { PositionName = "Consultant" }
            //     );
            //#endregion
            //Create Role Admin if it does not exist
            //var role = roleManager.FindByName(roleName);
            //if (role == null)
            //{
            //    role = new IdentityRole(roleName);
            //    var roleresult = roleManager.Create(role);
            //}

            //#region TAX
            //context.Taxes.AddOrUpdate(t => t.Name, new Tax
            //{
            //    Name = "Service Tax",
            //    Description = "Services being rendered to local client",
            //    IsAppliedOnBasePrice = true,
            //    TaxValue = 12.33M,
            //    CreatedOn = DateTime.Now
            //});
            //#endregion

            //#region TAXABLE PRODUCTS
            //var tax = context.Taxes.FirstOrDefault(t => t.Name == "Service Tax");

            //if (tax != null)
            //{
            //    context.Products.AddOrUpdate(p => p.Name,
            //        new Product
            //        {
            //            Name = "One Month Licence Fee",
            //            Description = "One Month Licence Fee",
            //            IsActive = true,
            //            Validity = 30,
            //            ProductCode = "OMLF",
            //            ProductType = "S",
            //            Price = 1500.40M,
            //            IsTaxApplicable = true,
            //            TaxId = tax.TaxId,
            //            CreatedOn = DateTime.Now
            //        },
            //         new Product
            //         {
            //             Name = "Three  Month Licence Fee",
            //             Description = "Three  Month Licence Fee",
            //             IsActive = true,
            //             Validity = 90,
            //             ProductCode = "TMLF",
            //             ProductType = "S",
            //             Price = 4000.25M,
            //             IsTaxApplicable = true,
            //             TaxId = tax.TaxId,
            //             CreatedOn = DateTime.Now
            //         },
            //          new Product
            //          {
            //              Name = "Online Tourist Visa Service Charge",
            //              Description = "Online Tourist Visa Service Charge",
            //              IsActive = true,
            //              Validity = 100,
            //              ProductCode = "OTVSC",
            //              ProductType = "M",
            //              Price = 500.00M,
            //              IsTaxApplicable = true,
            //              TaxId = tax.TaxId,
            //              CreatedOn = DateTime.Now
            //          }
            //        );
            //}
            //#endregion

            //#region NON-TAXABLE PRODUCTS
            //context.Products.AddOrUpdate(p => p.Name,
            //      new Product
            //      {
            //          Name = "VFS Fee",
            //          Description = "VFS Fee",
            //          IsActive = true,
            //          Validity = 100,
            //          ProductCode = "VFSF",
            //          ProductType = "M",
            //          Price = 658.00M,
            //          IsTaxApplicable = false,
            //          CreatedOn = DateTime.Now
            //      },
            //       new Product
            //       {
            //           Name = "Legal Fee",
            //           Description = "Legal fee for tourist visa processing",
            //           IsActive = true,
            //           Validity = 100,
            //           ProductCode = "LF",
            //           ProductType = "M",
            //           Price = 8000.00M,
            //           IsTaxApplicable = false,
            //           CreatedOn = DateTime.Now
            //       },
            //        new Product
            //        {
            //            Name = "Notary",
            //            Description = "Notary",
            //            IsActive = true,
            //            Validity = 100,
            //            ProductCode = "Notary",
            //            ProductType = "S",
            //            Price = 200.00M,
            //            IsTaxApplicable = false,
            //            CreatedOn = DateTime.Now
            //        }
            //      );
            //#endregion

            #region COUNTRY
            context.Countries.AddOrUpdate(c => c.CountryCode,
                   new Country { CountryCode = "USA", CountryText = "USA" },
                   new Country { CountryCode = "IND", CountryText = "India" },
                   new Country { CountryCode = "AUS", CountryText = "Australia" }
                   );
            #endregion
        }
    }

    public enum SignInStatus
    {
        Success,
        LockedOut,
        RequiresTwoFactorAuthentication,
        Failure,
        EmailNotVerified,
        MembershipExpired
    }

    // These help with sign and two factor (will possibly be moved into identity framework itself)
    public class SignInHelper
    {
        public SignInHelper(ApplicationUserManager userManager, IAuthenticationManager authManager)
        {
            UserManager = userManager;
            AuthenticationManager = authManager;
        }

        public ApplicationUserManager UserManager { get; private set; }
        public IAuthenticationManager AuthenticationManager { get; private set; }

        public async Task SignInAsync(ApplicationUser user, bool isPersistent, bool rememberBrowser)
        {
            // Clear any partial cookies from external or two factor partial sign ins
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            var userIdentity = await user.GenerateUserIdentityAsync(UserManager);
            userIdentity.AddClaim(new Claim(ClaimTypes.Surname, user.UserProfile.FullName));
            userIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            userIdentity.AddClaim(new Claim("ClassTypes", string.IsNullOrWhiteSpace(user.UserProfile.ClassTypes) ? "" : user.UserProfile.ClassTypes));
            userIdentity.AddClaim(new Claim("UserType", string.IsNullOrWhiteSpace(user.UserType) ? "" : user.UserType));
            userIdentity.AddClaim(new Claim("Avatar", string.IsNullOrWhiteSpace(user.UserProfile.Avatar) ? "" : user.UserProfile.Avatar));

            var userPlan = user.UserPlans.FirstOrDefault(p => p.UserId == user.Id && p.IsActive);
            if (userPlan != null && userPlan.MembershipPlan != null)
            {
                userIdentity.AddClaim(new Claim("MembershipPlan", string.IsNullOrWhiteSpace(userPlan.MembershipPlan.Name) ? "" : userPlan.MembershipPlan.Name));
                userIdentity.AddClaim(new Claim("MembershipPlanCode", string.IsNullOrWhiteSpace(userPlan.MembershipPlan.MembershipPlanCode) ? "" : userPlan.MembershipPlan.MembershipPlanCode));
            }
            else
            {
                userIdentity.AddClaim(new Claim("MembershipPlan", string.Empty));
                userIdentity.AddClaim(new Claim("MembershipPlanCode", string.Empty));
            }
            if (rememberBrowser)
            {
                var rememberBrowserIdentity = AuthenticationManager.CreateTwoFactorRememberBrowserIdentity(user.Id);
                AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, userIdentity, rememberBrowserIdentity);
            }
            else
            {
                AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, userIdentity);
            }
        }

        public async Task<bool> SendTwoFactorCode(string provider)
        {
            var userId = await GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return false;
            }

            var token = await UserManager.GenerateTwoFactorTokenAsync(userId, provider);
            // See IdentityConfig.cs to plug in Email/SMS services to actually send the code
            await UserManager.NotifyTwoFactorTokenAsync(userId, provider, token);
            return true;
        }

        public async Task<string> GetVerifiedUserIdAsync()
        {
            var result = await AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.TwoFactorCookie);
            if (result != null && result.Identity != null && !String.IsNullOrEmpty(result.Identity.GetUserId()))
            {
                return result.Identity.GetUserId();
            }
            return null;
        }

        public async Task<bool> HasBeenVerified()
        {
            return await GetVerifiedUserIdAsync() != null;
        }

        public async Task<SignInStatus> TwoFactorSignIn(string provider, string code, bool isPersistent, bool rememberBrowser)
        {
            var userId = await GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return SignInStatus.Failure;
            }
            var user = await UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return SignInStatus.Failure;
            }
            if (await UserManager.IsLockedOutAsync(user.Id))
            {
                return SignInStatus.LockedOut;
            }
            if (await UserManager.VerifyTwoFactorTokenAsync(user.Id, provider, code))
            {
                // When token is verified correctly, clear the access failed count used for lockout
                await UserManager.ResetAccessFailedCountAsync(user.Id);
                await SignInAsync(user, isPersistent, rememberBrowser);
                return SignInStatus.Success;
            }
            // If the token is incorrect, record the failure which also may cause the user to be locked out
            await UserManager.AccessFailedAsync(user.Id);
            return SignInStatus.Failure;
        }

        public async Task<SignInStatus> ExternalSignIn(ExternalLoginInfo loginInfo, bool isPersistent)
        {
            var user = await UserManager.FindAsync(loginInfo.Login);
            var userProfile = UserManager.Users.Include("UserProfile").FirstOrDefault(x => x.Id == user.Id);
            if (userProfile == null)
            {
                return SignInStatus.Failure;
            }
            if (await UserManager.IsLockedOutAsync(userProfile.Id))
            {
                return SignInStatus.LockedOut;
            }
            return await SignInOrTwoFactor(userProfile, isPersistent);
        }

        private async Task<SignInStatus> SignInOrTwoFactor(ApplicationUser user, bool isPersistent)
        {
            if (await UserManager.GetTwoFactorEnabledAsync(user.Id) &&
                !await AuthenticationManager.TwoFactorBrowserRememberedAsync(user.Id))
            {
                var identity = new ClaimsIdentity(DefaultAuthenticationTypes.TwoFactorCookie);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));

                AuthenticationManager.SignIn(identity);
                return SignInStatus.RequiresTwoFactorAuthentication;
            }
            await SignInAsync(user, isPersistent, false);
            return SignInStatus.Success;

        }

        public async Task<SignInStatus> PasswordSignIn(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            var signInStatus = SignInStatus.Failure;
            var user = UserManager.Users
                 .Include("UserPlans")
                .Include("UserPlans.MembershipPlan")
                .Include("UserProfile")
                .FirstOrDefault(x => x.UserName == userName);
            //var user = await UserManager.FindByNameAsync(userName);

            if (user == null)
            {
                return SignInStatus.Failure;
            }
            if (!UserManager.IsEmailConfirmed(user.Id))
            {
                return signInStatus = SignInStatus.EmailNotVerified;
            }
            if (await UserManager.IsLockedOutAsync(user.Id))
            {
                return signInStatus = SignInStatus.LockedOut;
            }
            if (await UserManager.CheckPasswordAsync(user, password))
            {
                signInStatus = await SignInOrTwoFactor(user, isPersistent);
            }
            if (shouldLockout)
            {
                // If lockout is requested, increment access failed count which might lock out the user
                await UserManager.AccessFailedAsync(user.Id);
                if (await UserManager.IsLockedOutAsync(user.Id))
                {
                    return signInStatus = SignInStatus.LockedOut;
                }
            }
            if (signInStatus == SignInStatus.Success)
            {
                // CHECK, IF LOGGED-IN USER IS ADMIN OR STAFF
                if (!user.Email.Contains(AppConstants.DomainName))
                {
                    using (var dbContext = new ApplicationDbContext())
                    {
                        var userPlanObj =
                            dbContext.UserPlans.Include("MembershipPlan")
                                .FirstOrDefault(x => x.IsActive && x.UserId == user.Id);
                        if (userPlanObj != null && userPlanObj.MembershipPlan != null)
                        {
                            #region MyRegion

                            if (userPlanObj.ExpiryDate.HasValue)
                            {
                                int compareValue =
                                    Convert.ToDateTime(userPlanObj.ExpiryDate).Date.CompareTo(DateTime.Today);
                                if (compareValue < 0)
                                    signInStatus = SignInStatus.MembershipExpired;
                                else if (compareValue == 0 || compareValue > 0)
                                    signInStatus = SignInStatus.Success;
                            }
                            else
                            {
                                // First login
                                userPlanObj.ExpiryDate = DateTime.Now.AddDays(userPlanObj.MembershipPlan.ValidityInDays);
                                userPlanObj.ModifiedBy = CustomClaimsPrincipal.Current.UserId;
                                userPlanObj.ModifiedOn = DateTime.Now;

                                dbContext.Entry(userPlanObj).State = EntityState.Modified;
                                if (dbContext.SaveChanges() > 0)
                                {
                                    signInStatus = SignInStatus.Success;
                                }
                            }

                            #endregion
                        }
                        else
                            signInStatus = SignInStatus.MembershipExpired;

                        return signInStatus;
                    }
                }
            }
            return signInStatus;
        }
    }
}