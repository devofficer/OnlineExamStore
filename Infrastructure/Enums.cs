using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
namespace OnlineExam.Infrastructure
{
    public class Enums
    {
        public enum ReferralType
        {
            FirstLevel,
            SecondLevel 
        }
        public enum ReferralPaymentStatus
        {
            Paid,
            Pending
        }
        public enum RedeemStatus
        {
            InProgress,
            Redeemed
        }
        public enum QuestionFormatType
        {
            Comprehensive,
            SingleQuestionMultipleChoice
        }
        public enum QuestionOptions
        {
            A, B, C, D, E
        }

        public enum LookupType
        {
            UserType,
            ClassType,
            ExamType,
            Subject,
            QuestionFormatType,
            ClassCategory,
            SubjectCategory,
            DenominationType,
            CustomCBTCounter,
            VoucherCodeDigitCounter,
            Topic
        }

        public enum CompareOperator
        {
            GreaterThan,
            GreaterThanOrEqual,
            LessThan,
            LessThanOrEqual
        }

        public enum TouristVisaStep
        {
            StepOne = 1,
            StepTwo = 2,
            StepThree = 3,
            StepFour = 4,
            StepFive = 5,
            StepSix = 6,
            Finish = 10
        }
        public enum MemberType
        {
            Applicant,
            Sponsor,
            RelativeInAustralia,
            FriendOrContactInAustralia
        }
        public enum EmploymentStatusType
        {
            Employed,
            [Display(Name = "Self Employed")]
            SelfEmployed,
            Retired,
            Student,
            Other,
            Unemployed
        }
        public enum ApplicationStatus
        {
            Pending = 0,
            Errors = 1, Completed = 2, Submitted = 3, PaymentPending = 4, ReceivedCompleted = 5, UnderProcess = 6,
            Finalized = 7
        }
        public enum ApplicationActions
        {
            ToComplete = 0, ToSubmit = 1, ToPay = 2, Wait = 3, ReceivedCompleted = 4
        }

        public enum VisaApplicationType
        {
            Individual = 1,
            Group = 2
        }
        public enum ApplicantShore
        {
            [Display(Name = "On Shore")]
            OnShore = 1,
            [Display(Name = "Off Shore")]
            OffShore = 2
        }
        public enum TouristVisaStream
        {
            [Display(Name = "Family Visitor")]
            FamilyVisitor = 1,

            [Display(Name = "Business Visitor")]
            BusinessVisitor = 2,

            [Display(Name = "Tourist Visitor")]
            TouristVisitor = 3
        }
        public enum LegalStatusType
        {
            Citizen,
            [Display(Name = "Permanent Resident")]
            PermanentResident,

            Visitor,
            Student,
            [Display(Name = "Work Visa")]
            WorkVisa,
            [Display(Name = "No Legal Status")]
            NoLegalStatus,

            Other
        }
        public enum PaymentMethod
        {
            [Display(Name = "Direct Deposit")]
            DirectDeposit = 1,

            [Display(Name = "Bank Cheque")]
            BankCheque = 2,

            [Display(Name = "Money Order")]
            MoneyOrder = 3,

            [Display(Name = "Debit Card")]
            DebitCard = 4,

            [Display(Name = "Credit Card")]
            CreditCard = 5
        }
        public enum RelationShipStatus : int
        {
            None = 0,
            Sister = 1,
            Brother = 2,
            Mother = 3,
            Husband = 4,
            Wife = 5,
            Father = 6,
            Son = 7,
            Daughter = 8
        }
        public enum MaritalStatus
        {
            Married = 1,
            Single = 2,
            Divorced = 3,
            [Display(Name = "Never Married")]
            NeverMarried = 4,
            DeFacto = 5,
            Widowed = 6,
            Engaged = 7,
            Seprated = 8
        }
        public enum EducationDetail
        {
            None,

            [Display(Name = "Higher Degree Course")]
            HigherDegreeCourse,

            [Display(Name = "Degree Course")]
            DegreeCourse,

            [Display(Name = "Diploma Course")]
            DiplomaCourse,

            [Display(Name = "Technical Or Training Certificate")]
            TechnicalOrTrainingCertificate,

            [Display(Name = "College Degree")]
            CollegeDegree,

            [Display(Name = "Senior High School Degree Or Certificate")]
            SeniorHighSchoolDegreeOrCertificate,

            [Display(Name = "Junior High School Degree Or Certificate Or Report")]
            JuniorHighSchoolDegreeOrCertificateOrReport
        }

        public enum Gender
        {
            Male = 1,
            Female = 2
        }
        public enum EducationMode
        {
            Campus,
            [Display(Name = "PartTime Campus")]
            PartTimeCampus,
            [Display(Name = "Distant Education")]
            DistantEducation,
            Other
        }
        public enum ProfessionIndustry
        {

            Mechanical,
            Electronics,
            Education,
            Medical,
            Other
        }
        public enum EducationTitle
        {
            Doctorate,
            Masters,
            Bachelors,
            Diploma,
            Other
        }

        public enum WorkVisaStep
        {
            Step1 = 1,
            Step2 = 2,
            Step3 = 3,
            Step4 = 4,
            Step5 = 5,
            Final = 6,
            View


        }
        //******************** 9 SEPTEMBER 2014 *********************//
        public enum ExpertType
        {
            RMA = 1,
            Consultant
        }
        public enum Day
        {
            Monday = 1,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday,
            Sunday
        }

    }
}