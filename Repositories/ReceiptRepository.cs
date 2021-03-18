using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using OnlineExam.Models;

namespace OnlineExam.Repositories
{
    public class ReceiptRepository
    {
        public bool Add(ReceiptModelView receiptModelView)
        {
            int result = 0;
            using (var context = new ApplicationDbContext())
            {
                var receipt = new Receipt
                {
                    InvoiceId = receiptModelView.InvoiceId,
                    ReceiptCode = "R" + receiptModelView.InvoiceCode,
                    CompanyName = receiptModelView.CompanyName,
                    CompanyAddress = receiptModelView.CompanyAddress,
                    TotalAmount = receiptModelView.TotalAmount,
                    ReceivedAmount = receiptModelView.ReceivedAmount,
                    ModeOfPayment = receiptModelView.ModeOfPayment.ToString(),
                    BankName = receiptModelView.BankName,
                    ChequeNo = receiptModelView.ChequeNo,
                    ChequeDate = receiptModelView.ChequeDate,
                    CreatedOn = DateTime.Now
                };

                var xx = context.Invoices.FirstOrDefault(x => x.InvoiceId == receiptModelView.InvoiceId);
                xx.Receipt = receipt;
                context.Entry(xx).State = EntityState.Modified;
                
                result = context.SaveChanges();
            }
            return result > 0;
        }

        public Receipt FindOneById(int invoiceId)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Receipts.FirstOrDefault(x => x.InvoiceId == invoiceId);
            }
        }

        public bool IsReceiptExists(int invoiceId)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Receipts.Any(x => x.InvoiceId == invoiceId);
            }
        }
    }
}