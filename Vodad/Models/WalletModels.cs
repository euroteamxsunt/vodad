using System.ComponentModel.DataAnnotations;
using VodadModel;

namespace Vodad.Models
{
    public class AddMerchantAccountModel
    {
        [Required]
        [Display(Name = "Enter Your merchant account name here:")]
        public string Account { get; set; }

        public string Merchant { get; set; }

        public int MerchantId { get; set; }

        public int UserId { get; set; }
    }

    public class CertificateActivationModel
    {
        public int UserId { get; set; }

        [Required]
        public string Code { get; set; }
    }

    public class ChangeMerchantAccountModel
    {
        [Required]
        [Display(Name = "Enter Your merchant account name here:")]
        public string Account { get; set; }

        public string Merchant { get; set; }

        public int UserMerchantId { get; set; }

        public int MerchantId { get; set; }

        public int UserId { get; set; }
    }

    public class ChargeMoneyModel
    {
        [Display(Name = "Enter charge sum:")]
        [Required(ErrorMessage = "Sum is required!")]
        [RegularExpression(@"^[0-9]*(\,[0-9]*)?$", ErrorMessage = "Sum is not valid!")]
        [Range(0, 1000000000)]
        public decimal ChargeSum { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Select merchant")]
        public int MerchantId { get; set; }
    }

    public class ManageWalletChangesModel
    {
        public int UserId { get; set; }

        public int UserMerchantId { get; set; }

        public string CurrentAccount { get; set; }

        public string PreviousAccount { get; set; }

        public string NextAccount { get; set; }

        public decimal TotalWalletSum { get; set; }

        public decimal OrderPerformedInactionAndCompleteSumAfterLastAccountChanges { get; set; }

        public decimal LastCurrentAccountChargeSum { get; set; }

        public string LastCurrentAccountChargeDateTime { get; set; }

        public string CurrentAccountChangeDateTime { get; set; }

        public bool CanBeChanged { get; set; }
    }

    public class WalletManagerModel
    {
        [Display(Name = "Enter sum:")]
        [Required(ErrorMessage = "Enter sum!")]
        [RegularExpression(@"^[0-9]*(\,[0-9]*)?$", ErrorMessage= "Sum is not valid")]
        [Range(0, 1000000000)]
        public decimal ChargeSum { get; set; }

        [Display(Name = "Select merchant")]
        [Required(ErrorMessage = "Select merchant!")]
        public string Merchant { get; set; }

        [Display(Name = "Choose action")]
        [Required(ErrorMessage = "Choose action!")]
        public string Action { get; set; }
    }

    public class MoneyTransferTableModel
    {
        public int MoneyTransferId { get; set; }

        public decimal Amount { get; set; }

        public string Action { get; set; }

        public string AccountMerchantSystem { get; set; }

        public string MerchantName { get; set; }

        public string DateTime { get; set; }

        public string Status { get; set; }
    }
}
