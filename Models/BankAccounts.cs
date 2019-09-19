using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HunterW_FinancialPortal.Models
{
    public class BankAccounts
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        public int BankAccountTypeId { get; set; }
        public string Name { get; set; }
        public double Balance { get; set; }
        public double LowAlertLevel { get; set; }

        //virtual
        public virtual BankAccountType BankAccountTypes { get; set; }
        public virtual Household Household { get; set; }
        public virtual ICollection<Transaction> Transaction { get; set; }

        public BankAccounts()
        {
            Transaction = new HashSet<Transaction>();
        }
    }
}