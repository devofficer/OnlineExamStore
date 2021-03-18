using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;
using Microsoft.Ajax.Utilities;
using OnlineExam.Models;
using OnlineExam.Utils;

namespace OnlineExam.Repositories
{
    public class InvoiceRepository
    {
        public bool Add(Invoice invoice)
        {
            int result = 0;
            using (var context = new ApplicationDbContext())
            {
                using (var scope = new TransactionScope())
                {
                    if (context.Invoices.Any(x => x.RefNumber == invoice.RefNumber))
                    {
                        if (invoice.Status == InvoiceStatus.Draft.ToString())
                        {
                            var invoiceObj =
                                context.Invoices.FirstOrDefault(
                                    x =>
                                        x.RefNumber == invoice.RefNumber &&
                                        x.Status == InvoiceStatus.Draft.ToString());
                            if (invoiceObj != null)
                            {
                                // DELETE INVOICE & INVOICE ITEMS DATA
                                invoiceObj.InvoiceItems.ToList().ForEach(i => context.InvoiceItems.Remove(i));
                                context.Entry(invoiceObj).State = EntityState.Deleted;
                                context.SaveChanges();

                            }
                        }
                    }

                    context.Invoices.Add(invoice);
                    result = context.SaveChanges();

                    scope.Complete();
                    if (result > 0)
                    {
                        // SAVE
                    }
                    else
                    {
                        // FALID
                        Transaction.Current.Rollback();
                    }

                }
            }
            return result > 0;
        }
        public int GetLatestInvoiceCode()
        {
            using (var context = new ApplicationDbContext())
            {
                var invoiceCode = Convert.ToInt32(context.Invoices.Max(i => i.InvoiceCode));
                if (invoiceCode > 0)
                {
                    return invoiceCode + 1;
                }
                return 10021;
            }
        }
        public bool CancelInvoiceById(int invoiceId)
        {
            int result = 0;
            using (var context = new ApplicationDbContext())
            {
                if (context.Invoices.Any(i => i.InvoiceId == invoiceId))
                {
                    var invoiceObj = context.Invoices.FirstOrDefault(i => i.InvoiceId == invoiceId && i.Status != InvoiceStatus.Canceled.ToString());
                    if (invoiceObj != null)
                    {
                        invoiceObj.Status = InvoiceStatus.Canceled.ToString();
                        result = context.SaveChanges();
                    }
                }
            }
            return (result > 0);
        }

        public List<Invoice> GetAll()
        {
            var context = new ApplicationDbContext();
            return context.Invoices.Include("InvoiceItems").Include("InvoiceItems.Product").Where(i => i.Status != InvoiceStatus.Canceled.ToString()).ToList();
        }
        public List<Invoice> GetInvoicesByRefNumber(string refNumber)
        {
            var context = new ApplicationDbContext();
            return
                context.Invoices.Include("Receipt")
                    .Include("InvoiceItems")
                    .Include("InvoiceItems.Product")
                    .Where(i => i.RefNumber == refNumber && i.Status != InvoiceStatus.Canceled.ToString())
                    .OrderByDescending(i => i.InvoiceCode).ToList();


        }
        public List<Invoice> GetInvoicesById(int invoiceId)
        {
            var context = new ApplicationDbContext();
            return context.Invoices.Include("InvoiceItems").Include("InvoiceItems.Product").Where(i => i.InvoiceId == invoiceId && i.Status != InvoiceStatus.Canceled.ToString()).OrderByDescending(i => i.InvoiceCode).ToList();
        }
        public Invoice FindOneById(int invoiceId)
        {
            var context = new ApplicationDbContext();
            return context.Invoices.Include("InvoiceItems").Include("InvoiceItems.Product").FirstOrDefault(i => i.InvoiceId == invoiceId && i.Status != InvoiceStatus.Canceled.ToString());
        }
        public List<Invoice> GetInvoicesByCompanyId(int companyId)
        {
            var context = new ApplicationDbContext();
            var company = context.Companies.FirstOrDefault(c => c.CompanyId == companyId);
            if (company != null)
                return context.Invoices.Include("InvoiceItems").Include("InvoiceItems.Product").Where(i => i.RefNumber == company.CompanyCode && i.Status != InvoiceStatus.Canceled.ToString()).OrderByDescending(i => i.InvoiceCode).ToList();
            return new List<Invoice>();
        }


    }
}