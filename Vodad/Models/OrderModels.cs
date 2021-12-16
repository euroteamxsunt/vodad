using System;
using System.Collections.Generic;
using System.Web;
using System.ComponentModel.DataAnnotations;
using VodadModel;

namespace Vodad.Models
{
    public class OrderModel
    {
        public OrderModel()
        {
            SelectAllThemes = true;
        }

        [Display(Name = "Comment:")]
        public string OrderComment { get; set; }

        [Required]
        [Display(Name = "Expire date(mm/dd/yyyy). Today for non-expire:")]
        [RegularExpression(@"^(0?[1-9]|1[012])\/(0?[1-9]|[12][0-9]|3[01])\/((19|20)\d\d)$")]
        public string OrderExpireDate { get; set; }

        [Required]
        [Display(Name = "You must choose content for order!")]
        public HttpPostedFileBase ContentData { get; set; }

        [Display(Name = "Select prefered region(optional):")]
        public int? RegionId { get; set; }

        [Required]
        [Display(Name = "All themes:")]
        public bool SelectAllThemes { get; set; }

        [Required]
        [Display(Name = "Order themes:")]
        public List<ThemesListForOrderModel> OrderThemesList { get; set; }
    }

    public class OrderEditModel
    {
        public int? OrderId { get; set; }

        [Display(Name = "Comment:")]
        public string OrderComment { get; set; }

        [Required]
        [Display(Name = "Expire date(mm/dd/yyyy). Today for non-expire:")]
        [RegularExpression(@"^(0?[1-9]|1[012])\/(0?[1-9]|[12][0-9]|3[01])\/((19|20)\d\d)$")]
        public string OrderExpireDate { get; set; }

        [Display(Name = "Select new content:")]
        public HttpPostedFileBase ContentData { get; set; }

        /*[Display(Name = "Content")]
        public object Content { get; set; }*/

        [Required]
        [Display(Name = "Select prefered region(optional):")]
        public int? RegionId { get; set; }

        public List<Image> ImagesList { get; set; }

        public List<Video> VideosList { get; set; }
    }

    public class OrderListModel
    {
        public int? OrderId { get; set; }

        public int? OrderCampaignId { get; set; }

        public string OrderName { get; set; }

        public decimal? MaxBudgetForOrder { get; set; }

        public string OrderComment { get; set; }

        public string OrderRegion { get; set; }

        public string OrderThemes { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public string OrderCreationDate { get; set; }

        [RegularExpression(@"^(0?[1-9]|1[012])\/(0?[1-9]|[12][0-9]|3[01])\/((19|20)\d\d)$")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        //[Range(typeof(DateTime), "1.1.2012", "1.1.2099", ErrorMessage = "Change expire date(dd.mm.yyyy)")]
        public string OrderExpireDate { get; set; }

        public string OrderStatus { get; set; }

        public List<Image> OrderImagesList { get; set; }

        public List<Video> OrderVideosList { get; set; }

        public string MinMaxStreamingTime { get; set; }
    }

    public class OrderContentModel
    {
        public int? ContentId { get; set; }

        [Required]
        [Display(Name = "Content name:")]
        public string ContentName { get; set; }

        [Required]
        [Display(Name = "Content:")]
        public byte[] ContentData { get; set; }

        [Required]
        [Display(Name = "Content size")]
        public string ContentSize { get; set; }

        [Required]
        [Display(Name = "Content creation date:")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? ContentCreationDate { get; set; }
    }

    public class OrderContentListModel
    {
        public int? OrderContentId { get; set; }

        public int? ContentId { get; set; }

        public string ContentName { get; set; }

        public byte[] ContentData { get; set; }

        public long? COntentSize { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? ContentCreationDate { get; set; }
    }

    public class ThemesListForOrderModel
    {
        [Required]
        public int? Id { get; set; }

        [Required]
        public string ThemeName { get; set; }

        [Required]
        public bool IsChecked { get; set; }
    }
}
