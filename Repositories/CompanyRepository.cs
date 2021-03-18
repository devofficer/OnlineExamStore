using System;
using System.Collections.Generic;
using System.Data.Entity;
using Microsoft.Ajax.Utilities;
using OnlineExam.Helpers;
using OnlineExam.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using OnlineExam.Utils;


namespace OnlineExam.Repositories
{
    public class CompanyRepository
    {
        public bool AddOrUpdate(CompanyViewModel companyViewModel)
        {
            using (var context = new ApplicationDbContext())
            {
                var company = new Company
                {
                    CompanyId = companyViewModel.CompanyId,
                    CompanyCode = companyViewModel.CompanyCode,
                    Status = companyViewModel.Status,

                    Name = companyViewModel.Name,
                    RelationshipStatus = companyViewModel.RelationshipStatus,
                    Location = companyViewModel.Location,
                    Description = companyViewModel.Description,

                    //ADDRESS DETAIL
                    AddressLine1 = companyViewModel.AddressLine1,
                    AddressLine2 = companyViewModel.AddressLine2,
                    ZipCode = companyViewModel.ZipCode,
                    City = companyViewModel.City,
                    State = companyViewModel.State,
                    Country = companyViewModel.Country,

                    //CONTACT DETAIL
                    ContactNumber = companyViewModel.ContactNumber,
                    ContactPerson = companyViewModel.ContactPerson,
                    PrimaryEmail = companyViewModel.PrimaryEmail,
                    SecondaryEmail = companyViewModel.SecondaryEmail,
                    ContactDetail = companyViewModel.ContactDetail,

                    //COMPANY DETAIL
                    TinNumber = companyViewModel.TinNumber,
                    ServiceNumber = companyViewModel.ServiceNumber,
                    OtherNumber = companyViewModel.OtherNumber,
                    BusinessType = companyViewModel.BusinessType,

                    RowVersion = companyViewModel.RowVersion,
                    //DOCUMENTS 
                    Documents = companyViewModel.Documents,
                    CreatedBy = companyViewModel.CreatedBy

                };
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (context.Companies.Any(c => c.CompanyId == companyViewModel.CompanyId))
                        {
                            var companyObj =
                                context.Companies.FirstOrDefault(c => c.CompanyId == companyViewModel.CompanyId);
                            if (companyObj != null)
                            {
                                //Scalar
                                context.Entry(companyObj).CurrentValues.SetValues(company);
                                context.Entry(companyObj).State = EntityState.Modified;
                            }
                            foreach (var document in companyViewModel.Documents)
                            {
                                document.Company = companyObj;
                                if (document.DocumentId == 0)
                                {
                                    context.Documents.Add(document);
                                }
                                else
                                {
                                    if (context.Set<Document>().Local.All(e => e.DocumentId != document.DocumentId))
                                    {
                                        context.Documents.Attach(document);
                                    }
                                    var documentObj = context.Documents.Single(x => x.DocumentId == document.DocumentId);
                                    context.Entry(documentObj).CurrentValues.SetValues(document);
                                    context.Entry(documentObj).State = EntityState.Modified;
                                }
                            }
                        }
                        else
                        {
                            context.Companies.Add(company);

                        }
                        int result = 0;
                        if (CustomClaimsPrincipal.Current.IsInRole(AppConstants.Roles.ProspectiveClient))
                        {
                            var user = context.Users.FirstOrDefault(x => x.Id == CustomClaimsPrincipal.Current.UserId);
                            if (user != null)
                            {
                                user.CompanyId =
                                    context.Companies.FirstOrDefault(x => x.CompanyCode == company.CompanyCode)
                                        .IfNotNull(x => x.CompanyId);
                                context.Entry(user).State = EntityState.Modified;
                            }
                        }
                        else
                        {
                            result = context.SaveChanges();
                            trans.Commit();
                            return result > 0;
                        }
                        result = context.SaveChanges();
                        trans.Commit();
                        return result > 0;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return false;
                    }
                }
            }

        }

        public CompanyViewModel FindOneById(int? id)
        {
            var companyViewModel = new CompanyViewModel();
            using (var context = new ApplicationDbContext())
            {
                var companyObject = context.Companies.Include("Documents").FirstOrDefault(c => c.CompanyId == id);
                if (companyObject != null)
                {
                    companyViewModel.CompanyId = companyObject.CompanyId;
                    companyViewModel.CompanyCode = companyObject.CompanyCode;
                    companyViewModel.Status = companyObject.Status;

                    companyViewModel.Name = companyObject.Name;
                    companyViewModel.RelationshipStatus = companyObject.RelationshipStatus;
                    companyViewModel.Location = companyObject.Location;
                    companyViewModel.Description = companyObject.Description;

                    //ADDRESS DETAIL
                    companyViewModel.AddressLine1 = companyObject.AddressLine1;
                    companyViewModel.AddressLine2 = companyObject.AddressLine2;
                    companyViewModel.ZipCode = companyObject.ZipCode;
                    companyViewModel.City = companyObject.City;
                    companyViewModel.State = companyObject.State;
                    companyViewModel.Country = companyObject.Country;

                    //CONTACT DETAIL
                    companyViewModel.ContactNumber = companyObject.ContactNumber;
                    companyViewModel.ContactPerson = companyObject.ContactPerson;
                    companyViewModel.PrimaryEmail = companyObject.PrimaryEmail;
                    companyViewModel.SecondaryEmail = companyObject.SecondaryEmail;
                    companyViewModel.ContactDetail = companyObject.ContactDetail;

                    //COMPANY DETAIL
                    companyViewModel.TinNumber = companyObject.TinNumber;
                    companyViewModel.ServiceNumber = companyObject.ServiceNumber;
                    companyViewModel.OtherNumber = companyObject.OtherNumber;
                    companyViewModel.BusinessType = companyObject.BusinessType;

                    companyViewModel.RowVersion = companyObject.RowVersion;

                    //DOCUMENTS 
                    companyViewModel.Documents = companyObject.Documents;
                }
            }

            return companyViewModel;
        }

        public Company FindCompanyById(int? id)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Companies.Include("Documents").FirstOrDefault(c => c.CompanyId == id);
            }
        }
        public List<Company> GetCompaniesForEditUser(string email)
        {
            var companies = new List<Company>();
            using (var context = new ApplicationDbContext())
            {
                if (!email.Contains(AppConstants.DomainName)) // NON- AUS.VISA USER
                {
                    companies = context.Companies.Where(x => x.Status == "Approved").ToList();
                    if (companies.Any(c => c.Name.Contains("Aus-visa.com")))
                    {
                        var companyObj = companies.FirstOrDefault(c => c.Name.Contains("Aus-visa.com"));
                        companies.Remove(companyObj);
                    }
                }
                else // SHOW ONLY AUS-VISA COMPANY FOR AUS VISA USER
                {
                    var companyObj = context.Companies.FirstOrDefault(c => c.Name == "Aus-visa.com");
                    companies.Add(companyObj);
                }
            }
            return companies;
        }
        public bool Update(int companyId, CompanyStatus companyStatus)
        {
            int result = 0;
            using (var context = new ApplicationDbContext())
            {
                var companyObj = context.Companies.FirstOrDefault(c => c.CompanyId == companyId);
                if (companyObj != null)
                {
                    switch (companyStatus)
                    {
                        case CompanyStatus.Approved:
                            companyObj.Status = CompanyStatus.Approved.ToString();
                            break;
                        case CompanyStatus.Rejected:
                            companyObj.Status = CompanyStatus.Rejected.ToString();
                            break;
                        case CompanyStatus.Suspended:
                            companyObj.Status = CompanyStatus.Suspended.ToString();
                            break;
                        case CompanyStatus.Active:
                            companyObj.Status = CompanyStatus.Active.ToString();
                            break;
                    }

                    context.Entry(companyObj).State = EntityState.Modified;
                    result = context.SaveChanges();
                }
            }

            return result > 0;
        }
    }
}