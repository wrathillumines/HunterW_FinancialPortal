using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HunterW_FinancialPortal.Models
{
    public class DashboardViewModel
    {
        public virtual ICollection<BankAccounts> BankAccounts { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<Budget> Budgets { get; set; }
        public virtual ICollection<BudgetItem> BudgetItems { get; set; }
        public virtual Household Household { get; set; }

        public DashboardViewModel()
        {
            BankAccounts = new HashSet<BankAccounts>();
            Transactions = new HashSet<Transaction>();
            Budgets = new HashSet<Budget>();
            BudgetItems = new HashSet<BudgetItem>();
        }
    }
}