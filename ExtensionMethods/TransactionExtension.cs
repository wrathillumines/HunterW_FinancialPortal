using HunterW_FinancialPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HunterW_FinancialPortal.ExtensionMethods
{
    public static class TransactionExtensions
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        // Update Account Balance
        public static void UpdateBalance(this Transaction transaction)
        {
            var bankAccount = db.BankAccounts.Find(transaction.BankAccountsId);

            if (transaction.TransactionTypeId.ToString() == "1" 
                || transaction.TransactionTypeId.ToString() == "2" 
                || transaction.TransactionTypeId.ToString() == "6" 
                || transaction.TransactionTypeId.ToString() == "7")
            {
                bankAccount.Balance -= transaction.TransactionAmount;
            }
            else
            {
                bankAccount.Balance += transaction.TransactionAmount;
            }

            db.SaveChanges();
        }
    }
}