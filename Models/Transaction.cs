using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HunterW_FinancialPortal.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double TransactionAmount { get; set; }
        public string CreatedById { get; set; }
        [Display(Name="Bank Account")]
        public int BankAccountsId { get; set; }
        [Display(Name="Type")]
        public int TransactionTypeId { get; set; }
        [Display(Name="Budget")]
        public int BudgetId { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMM d, yy} at {0:h:mm tt}")]
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }

        //virtual
        public virtual ApplicationUser CreatedBy { get; set; }
        public virtual BankAccounts BankAccount { get; set; }
        public virtual TransactionType TransactionType { get; set; }
        public virtual Budget Budget { get; set; }
    }
}