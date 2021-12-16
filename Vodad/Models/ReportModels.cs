using System;

namespace Vodad.Models
{
    /*public class CampaignReportModel
    {
        public int OId { get; set; }

        public DateTime StartDate { get; set; }

        public long Duration { get; set; }

        public int UniqueVisitors { get; set; }

        public int TimeWatched { get; set; }

        public int AverageConcurrentViewers { get; set; }

        public decimal? MoneySpent { get; set; }

        public int TotalVideos { get; set; }

        public decimal? AveragePricePerMinute { get; set; }

        public int TotalOrderPerformed { get; set; }
    }*/

    public class OrderReportModel
    {
        public int OPId { get; set; }

        public DateTime StartDate { get; set; }

        public long Duration { get; set; }

        public int UniqueVisitors { get; set; }

        public int TimeWatched { get; set; }

        public int AverageConcurrentViewers { get; set; }

        public decimal? MoneySpent { get; set; }

        public int TotalVideos { get; set; }

        public decimal? PricePerMinute { get; set; }
    }

    public class OrderPerformedReportModel
    {
        public DateTime Date { get; set; }

        public string Link { get; set; }

        public long Duration { get; set; }

        public int UniqueVisitors { get; set; }

        public int TimeWatched { get; set; }

        public int AverageConcurrentViewers { get; set; }

        public int VideoCounter { get; set; }

        public decimal? MoneySpent { get; set; }
    }

    public class SumOrderPerformedReportModel
    {
        public long SumDuration { get; set; }

        public int SumUniqueVisitors { get; set; }

        public int SumTimeWatched { get; set; }

        public int SumAverageConcurrentViewers { get; set; }

        public decimal? SumMoneySpent { get; set; }
    }

    /*public class SumOrderReportModel
    {
        public long SumDuration { get; set; }

        public int SumUniqueVisitors { get; set; }

        public int SumTimeWatched { get; set; }

        public int AverageConcurrentViewers { get; set; }

        public decimal? SumMoneySpent { get; set; }

        public int TotalVideos { get; set; }

        public decimal? AveragePricePerMinute { get; set; }
    }*/
}