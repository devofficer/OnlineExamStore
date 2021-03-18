using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
//using OnlineExam.Areas.Admin.ViewModels;
using OnlineExam.Infrastructure.Tasks;
using OnlineExam.Models;

namespace OnlineExam
{
    public class AutoMapperConfig : IRunAtInit
    {
        public void Execute()
        {
            //Mapper.CreateMap<VisaApplication, VisaApplicationViewModel>();
            //Mapper.CreateMap<PassportDetail, PassportDetailViewModel>();

            //Mapper.CreateMap<VisaUtils.FamilyVisitingAlongWith, FamilyTravellingViewModel>();
            //Mapper.CreateMap<FamilyTravellingViewModel, VisaUtils.FamilyVisitingAlongWith>();

            //Mapper.CreateMap<VisaUtils.FamilyNotVisitingAlong, FamilyNoTTravellingViewModel>();
            //Mapper.CreateMap<FamilyNoTTravellingViewModel, VisaUtils.FamilyNotVisitingAlong>();


            //Mapper.CreateMap<VisaUtils.RelativesOrFriendInAustralia, RelativesOrFriendViewModel>();
            //Mapper.CreateMap<RelativesOrFriendViewModel, VisaUtils.RelativesOrFriendInAustralia>();

            //Mapper.CreateMap<VisaUtils.EmploymentStatus, EmploymentDetailViewModel>();
            //Mapper.CreateMap<EmploymentDetailViewModel, VisaUtils.EmploymentStatus>();

            //Mapper.CreateMap<VisaUtils.Address, AddressViewModel>();
            //Mapper.CreateMap<AddressViewModel, VisaUtils.Address>();

            //Mapper.CreateMap<VisaUtils.ContactPhones, ContactPhonesViewModel>();
            //Mapper.CreateMap<ContactPhonesViewModel, VisaUtils.ContactPhones>();

            //Mapper.CreateMap<VisaUtils.Sponsor, SponsorViewModel>();
            //Mapper.CreateMap<SponsorViewModel, VisaUtils.Sponsor>();

            //Mapper.CreateMap<VisaUtils.BirthPlace, BirthPlaceViewModel>();
            //Mapper.CreateMap<BirthPlaceViewModel, VisaUtils.BirthPlace>();

            //Mapper.CreateMap<TouristVisaApplication, TouristVisaApplicationViewModel>();

            Mapper.CreateMap<QuestionBank, QuestionBankViewModel>();
            Mapper.CreateMap<QuestionBankViewModel, QuestionBank>();
            
            //Mapper.CreateMap<Consultant, ConsultantVM>();
            //Mapper.CreateMap<ConsultantVM, Consultant>();

            //Mapper.CreateMap<Booking, BookingVM>();
            //Mapper.CreateMap<BookingVM, Booking>();

        }
    }
}