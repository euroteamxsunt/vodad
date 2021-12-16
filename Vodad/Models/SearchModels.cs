using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VodadModel;

namespace Vodad.Models
{
    public class AdvertiserSearchModel
    {
        public AdvertiserSearchModel()
        {
            SelectAllThemes = true;
        }

        [Display(Name = "Performer platform id:")]
        public int? PerformerPlatformId { get; set; }

        [Required]
        [Display(Name = "All themes")]
        public bool SelectAllThemes { get; set; }

        [Required]
        [Display(Name = "Thtmes:")]
        public List<ThemesListForOrderModel> OrderThemesList { get; set; }

        [Required]
        [Display(Name = "Save search settings")]
        public bool SaveSearchSettings { get; set; }
    }

    public class AdvertiserSearchResultsModel
    {
        public string AdvertiserName { get; set; }

        public int? AdvertiserId { get; set; }

        public string Region { get; set; }

        public string Size { get; set; }

        public byte[] ImageData { get; set; }

        public string VideoLink { get; set; }

        public int? ContentId { get; set; }
    }

    public class SearchUserByNameModel
    {
        [Required]
        [Display(Name = "Enter username or it's part here:")]
        public string UserName { get; set; }
    }

    public class SearchUserByNameResultModel
    {
        public string UserName { get; set; }

        public string UserRole { get; set; }

        public int UserId { get; set; }
    }

    public class PerformerSearchModel
    {
        public PerformerSearchModel()
        {
            SelectAllThemes = true;
        }

        [Display(Name = "Order id:")]
        public int? OrderId { get; set; }

        [Required(ErrorMessage = "You need to enter min average viewers per hour!")]
        [Display(Name = "Min average viewers per hour:")]
        [Range(0, 1000000000)]
        public int? MinAverageViewersPerHour { get; set; }

        [Required(ErrorMessage = "You need to enter min total unique views!")]
        [Display(Name = "Min total unique views:")]
        [Range(0, 1000000000)]
        public int? TotalUniqueViews { get; set; }

        [Required(ErrorMessage = "You need to enter min total followers!")]
        [Display(Name = "Total followers")]
        [Range(0, 1000000000)]
        public decimal? TotalFollowers { get; set; }

        [Required]
        [Display(Name = "All themes")]
        public bool SelectAllThemes { get; set; }

        [Required]
        [Display(Name = "Themes:")]
        public List<ThemesListForOrderModel> OrderThemesList { get; set; }

        [Required]
        [Display(Name = "Save search settings")]
        public bool SaveSearchSettings { get; set; }
    }

    public class PerformerSearchResultsModel
    {
        public int? PerformerId { get; set; }

        public int? PerformerPlatformId { get; set; }

        public string PerformerName { get; set; }

        public string PerformerThemeName { get; set; }

        public string PerformerPlatformName{ get; set; }

        public string PerformerPlatformLink { get; set; }

        public List<string> GeolocationAndPercentage { get; set; }

        public int? AverageViewersPerHour { get; set; }

        public int? MaxViewersCount { get; set; }

        public int? TotalUniqueViews { get; set; }

        public int? TotalFollowers { get; set; }

        public decimal? TotalViews { get; set; }

        public int? Likes { get; set; }

        public decimal? AverageComplitionSpeed { get; set; }

        public int? TotalOrders { get; set; }

        public int? UniqueViewersForMonth { get; set; }
    }
}
