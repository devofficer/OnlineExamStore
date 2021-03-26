using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System;

namespace OnlineExam.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        public ApplicationUser()
        {
            UserPlans = new List<UserPlan>();
            AttemptedQuestionPapars = new List<AttemptedQuestionPapar>();
        }
        public string Description { get; set; }
        public string Status { get; set; }
        public string ReferedBy { get; set; }
        public bool IsAgreementAccpeted { get; set; }
        public string UserType { get; set; }
        public UserProfile UserProfile { get; set; }
        public ICollection<UserPlan> UserPlans { get; set; }
        public ICollection<AttemptedQuestionPapar> AttemptedQuestionPapars { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
        public int? CompanyId { get; set; }
    }

    public class UserProfile
    {
        public UserProfile()
        {
            CreatedOn = DateTime.UtcNow;
        }
        [Key]
        public int UserProfileId { get; set; }

        public string Avatar { get; set; }
        public string Email { get; set; }
        public string SchoolName { get; set; }
        public string SchoolAddress { get; set; }
        public string Hobbies { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        [NotMapped]
        public string FullName
        {
            get { return FirstName + " " + MiddleName + " " + LastName; }
        }

        public DateTime? DOB { get; set; }
        public string Gender { get; set; }

        public string SecondaryContactNo { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public int? ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ClassTypes { get; set; }
        public string SubjectCategory { get; set; }
        public bool IsCorrectionRequired { get; set; }
        public bool IsBankDetailReadOnly { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string ReferrerEmail { get; set; }
        public DateTime CreatedOn { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }

        [Required]
        [Index(IsUnique = true)]
        public ApplicationUser ApplicationUser { get; set; }
    }
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false) { }

        public DbSet<Document> Documents { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Country> Countries { get; set; }

        public DbSet<Lookup> Lookup { get; set; }

        public DbSet<QuestionPaper> QuestionPapers { get; set; }
        public DbSet<QuestionPaperMapping> QuestionPaperMappings { get; set; }
        public DbSet<QuestionBank> QuestionBank { get; set; }
        public DbSet<AnswerQuestion> AnswerQuestions { get; set; }
        public DbSet<AttemptedQuestionPapar> AttemptedQuestionPapars { get; set; }
        public DbSet<AttemptedQuestion> AttemptedQuestions { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Instruction> Instructions { get; set; }
        public DbSet<Tax> Taxes { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<Receipt> Receipts { get; set; }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }

        // VENDOR RELATED CLASSES
        public DbSet<MembershipPlan> MembershipPlans { get; set; }
        public DbSet<UserPlan> UserPlans { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }

        public DbSet<Message> Messages { get; set; }

        //public DbSet<ANZSCOCodes> ANZSCOCodes { get; set; }
        //public DbSet<CompanyTempRegisterUser> CompanyTempRegisterUsers { get; set; }
        public DbSet<Position> Positions { get; set; }

        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<PostCode> PostCodes { get; set; }

        public DbSet<School> Schools { get; set; }
        public DbSet<Banner> Banners { get; set; }

        public DbSet<SchoolNews> SchoolNews { get; set; }

        public DbSet<QuestionError> QuestionErrors { get; set; }

        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<UserBankPayment> UserBankPayments { get; set; }
        public DbSet<ReferralPragram> ReferralPragrams { get; set; }

        public DbSet<ReferralOrder> ReferralOrders { get; set; }

        public DbSet<TeachersProfileExtended> TeachersProfileExtended { get; set; }
        public DbSet<Followers> Followers { get; set; }
        public DbSet<LessonNotes> LessonNotes { get; set; }
        public DbSet<Lessons> Lessons { get; set; }
        public DbSet<LessonItems> LessonItems { get; set; }
        public DbSet<LessonDiscussions> LessonDiscussions { get; set; }
        public DbSet<LessonUsers> LessonUsers { get; set; }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<Invoice>().HasOptional(x => x.Receipt).WithOptionalDependent().Map(e => e.MapKey("ReceiptId"));

            this.Configuration.LazyLoadingEnabled = true;
        }

        static ApplicationDbContext()
        {
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            //Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, OnlineExam.Migrations.Configuration>("DefaultConnection"));
            //Database.SetInitializer<ApplicationDbContext>(new CreateDatabaseIfNotExists<ApplicationDbContext>());

            //PRODUCTION SETTINGS
            Database.SetInitializer<ApplicationDbContext>(null);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}