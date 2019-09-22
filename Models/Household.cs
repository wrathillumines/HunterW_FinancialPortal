using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HunterW_FinancialPortal.Models
{
    public class Household
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        //virtual
       
        public virtual ICollection<Budget> Budget { get; set; }
        public virtual ICollection<BankAccounts> BankAccounts { get; set; }
        public virtual ICollection<ApplicationUser> MyHouse { get; set; }
        public virtual ICollection<Invitation> Invitations { get; set; }

        public Household()
        {
            BankAccounts = new HashSet<BankAccounts>();
            Budget = new HashSet<Budget>();
            MyHouse = new HashSet<ApplicationUser>();
            Invitations = new HashSet<Invitation>();
        }
    }
}