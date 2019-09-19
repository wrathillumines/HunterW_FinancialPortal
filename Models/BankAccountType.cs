using System.Collections.Generic;

namespace HunterW_FinancialPortal.Models
{
    public class BankAccountType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //virtual
        public virtual ICollection<BankAccounts> BankAccounts { get; set; }
        public BankAccountType()
        {
            BankAccounts = new HashSet<BankAccounts>();
        }
    }
}