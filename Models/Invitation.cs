using System;

namespace HunterW_FinancialPortal.Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        public bool IsValid { get; set; }
        public Guid Code { get; set; }
        public string RecipientEmail { get; set; }
        public string SentBy { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public DateTimeOffset TTL { get; set; }
        public DateTimeOffset Created { get; set; }

        public virtual Household Household { get; set; }
    }
}