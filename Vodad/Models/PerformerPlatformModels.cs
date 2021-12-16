using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Vodad.Models
{
    public class PerformerPlatformModel
    {
        [Required]
        [Display(Name = "Platform login:")]
        public string PerformerPlatformLogin { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Platform password:")]
        public string PerformerPlatformPassword { get; set; }

        [Required]
        [Display(Name = "Platform theme:")]
        public int? PerformerPlatformThemeId { get; set; }

        [Required]
        [Display(Name = "Channel name:")]
        public string ChannelName { get; set; }

        [Display(Name = "Price for video ad(optional):")]
        public decimal? VideoPrice { get; set; }

        [Display(Name = "Price for image ad(optional):")]
        public decimal? LogoPrice { get; set; }
    }

    public class PerformerPlatformListModel
    {
        [Required]
        [Display(Name = "Platform id:")]
        public int? PerformerPlatformId { get; set; }

        [Required]
        [Display(Name = "Channel name")]
        public string ChannelName { get; set; }

        [Required]
        [Display(Name = "Platform login:")]
        public string PerformerPlatformLogin { get; set; }

        [Required]
        [Display(Name = "Platform theme:")]
        public string PerformerPlatformTheme { get; set; }

        [Required]
        [Display(Name = "Is verified:")]
        public string PerformerPlatformVerified { get; set; }

        public List<string> GeolocationAndPercentage { get; set; }

        public int? AverageViewersPerHour { get; set; }

        public int? MaxViewersCount { get; set; }

        public int? TotalUniqueViews { get; set; }

        public int? TotalFollowers { get; set; }

        public decimal? TotalViews { get; set; }

        public decimal? AverageComplitionSpeed { get; set; }

        public int? TotalOrders { get; set; }

        public int? UniqueViewersForMonth { get; set; }

        public string PerformerPlatformLink { get; set; }

        public decimal? VideoPrice { get; set; }

        public decimal? LogoPrice { get; set; }
    }
}
