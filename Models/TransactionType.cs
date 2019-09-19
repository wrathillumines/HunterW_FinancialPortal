using System.Collections.Generic;

namespace HunterW_FinancialPortal.Models
{
    public class TransactionType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        //virtual
        public virtual ICollection<Transaction> Transactions { get; set; }
        public TransactionType()
        {
            Transactions = new HashSet<Transaction>();
        }
    }
}