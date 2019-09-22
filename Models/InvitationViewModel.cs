using System.ComponentModel.DataAnnotations;

namespace HunterW_FinancialPortal.Models
{
    public class InvitationViewModel
    {
        [EmailAddress]
        [Required]
        public string RecipientEmail { get; set; }
        
        [Required]
        public int HouseholdId { get; set; }
    }
}