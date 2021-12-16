using System.ComponentModel.DataAnnotations;

namespace Vodad.Models
{
    public class OrdersPerformedListForPerformerModel
    {
        public string UserName { get; set; }

        public string AuthorName { get; set; }

        public int PerformerId { get; set; }

        public string PerformerName { get; set; }

        public int? AuthorId { get; set; }

        public int? OrderPerformedId { get; set; }

        public string Status { get; set; }

        public decimal? MoneyPaid { get; set; }

        public string OrderComment { get; set; }

        public string VideoLink { get; set; }

        public string Date { get; set; }

        public string ActionStatus { get; set; }

        public int? ContentId { get; set; }

        public string ContentType { get; set; }
    }

    public class OrderPerformedStatisticsModel
    {
        public string UsersRequestOrderPerformedCount { get; set; }

        public string ToUserRequestOrderPerformedCount { get; set; }

        public string InactionOrderPerformedCount { get; set; }

        public string CompleteOrderPerformedCount { get; set; }

        public decimal? Spent { get; set; }
    }

    public class OrderPerformedToPerformerModel
    {
        [Required]
        [Display(Name = "Performer platform id:")]
        public int? PerformerPlatformId { get; set; }

        [Required]
        [Display(Name = "Content id:")]
        public int? ContentId { get; set; }

        [Required]
        [Display(Name = "Expire date(mm/dd/yyyy):")]
        [RegularExpression(@"^(0?[1-9]|1[012])\/(0?[1-9]|[12][0-9]|3[01])\/((19|20)\d\d)$")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        //[Range(typeof(DateTime), "1.1.2012", "1.1.2099", ErrorMessage = "Change expire date(dd.mm.yyyy)")]
        public string ExpireDate { get; set; }
    }
}
