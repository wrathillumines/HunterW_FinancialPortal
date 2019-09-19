using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HunterW_FinancialPortal.Models
{
    public class Budget
    {
        public int Id { get; set; }
        public int? HouseholdId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        //virtual
        public virtual Household Household { get; set; }
        public virtual ICollection<BudgetItem> Items { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public Budget()
        {
            Items = new HashSet<BudgetItem>();
            Transactions = new HashSet<Transaction>();
        }
    }
}