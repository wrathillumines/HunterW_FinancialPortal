using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HunterW_FinancialPortal.Models
{
    public class BudgetItem
    {
        public int Id { get; set; }
        public int BudgetId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public string Payee { get; set; }

        //virtual
        public virtual Budget Budget { get; set; }
    }
}