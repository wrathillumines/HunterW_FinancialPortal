using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HunterW_FinancialPortal.Models
{
    public class BudgetViewModel
    {
        public virtual ICollection<BudgetItem> BudgetItems { get; set; }
        public virtual Budget Budget { get; set; }

        public BudgetViewModel()
        {
            BudgetItems = new HashSet<BudgetItem>();
        }
    }
}