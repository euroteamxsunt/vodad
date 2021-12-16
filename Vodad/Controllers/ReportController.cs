using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VodadModel;
using VodadModel.Repository;
using Vodad.Models;
using VodadModel.Helpers;
using System.Globalization;

namespace Vodad.Controllers
{
    public class ReportController : BaseController
    {
    }
}
/*{
    [Authorize(Roles = "Advertiser")]
    public ActionResult CampaignReport(int? id, DateTime? startdate, DateTime? expiredate)
    {
        if (id != null)
        {
            Repository<Campaign> campaignRepository = new Repository<Campaign>(Entities);

            var campaign = campaignRepository.GetSingle(w => w.Id == id);

            if (startdate == null)
                startdate = new DateTime(1970, 1, 1, new GregorianCalendar());

            if (expiredate == null)
                expiredate = DateTime.Today.AddDays(1);

            ViewBag.StartDate = new DateTime(2013, 2, 3, new GregorianCalendar());

            if (User.Identity.Name != null)
            {
                var user = UserHelper.GetUserByEmail(User.Identity.Name);

                CampaignController campaignController = new CampaignController();

                if (campaign != null && user != null && campaignController.IsUsersCampaign(id, user.Email))
                {
                    List<CampaignReportModel> orderSumList = new List<CampaignReportModel>();

                    foreach (var oi in campaign.Order)
                    {
                        try
                        {
                            var campaignReportElement = GetReportSumOrder((int)oi.Id, (int)user.Id, startdate, expiredate);

                            if (campaignReportElement != null)
                                orderSumList.Add(campaignReportElement);
                        }
                        catch (Exception)
                        { }
                    }

                    if (orderSumList.Any())
                    {
                        CampaignReportModel campaignReportSum = new CampaignReportModel();

                        try
                        {
                            campaignReportSum.AverageConcurrentViewers = (int)orderSumList.Average(v => v.AverageConcurrentViewers);
                        }
                        catch (Exception)
                        { }

                        try
                        {
                            campaignReportSum.AveragePricePerMinute = orderSumList.Average(v => v.AveragePricePerMinute);
                        }
                        catch (Exception)
                        { }

                        try
                        {
                            campaignReportSum.Duration = orderSumList.Sum(v => v.Duration);
                        }
                        catch (Exception)
                        { }

                        try
                        {
                            campaignReportSum.MoneySpent = orderSumList.Sum(v => v.MoneySpent);
                        }
                        catch (Exception)
                        { }

                        try
                        {
                            campaignReportSum.TimeWatched = orderSumList.Sum(v => v.TimeWatched);
                        }
                        catch (Exception)
                        { }

                        try
                        {
                            campaignReportSum.UniqueVisitors = orderSumList.Sum(v => v.UniqueVisitors);
                        }
                        catch (Exception)
                        { }

                        try
                        {
                            campaignReportSum.TotalVideos = orderSumList.Sum(v => v.TotalVideos);
                        }
                        catch (Exception)
                        { }

                        try
                        {
                            campaignReportSum.TotalOrderPerformed = orderSumList.Sum(v => v.TotalOrderPerformed);
                        }
                        catch (Exception)
                        { }

                        ViewBag.SumCampaignReport = campaignReportSum;

                        try
                        {
                            ViewBag.StartDate = orderSumList.Min(v => v.StartDate);
                        }
                        catch (Exception)
                        {
                            ViewBag.StartDate = new DateTime(2013, 1, 1, new GregorianCalendar());
                        }

                        ViewBag.Id = id;

                        return View(orderSumList);
                    }
                }
            }
        }

        return View();
    }

    private CampaignReportModel GetReportSumOrder(int? oid, int? userId, DateTime? startdate, DateTime? expiredate)
    {
        if (oid != null && userId != null)
        {
            Repository<Order> orderRepository = new Repository<Order>(Entities);
            Repository<OrderVideos> orderVideosRepository = new Repository<OrderVideos>(Entities);

            if (startdate == null)
                startdate = new DateTime(1970, 1, 1, new GregorianCalendar());

            if (expiredate == null)
                expiredate = DateTime.Today.AddDays(1);

            var user = UserHelper.GetUserById(userId);

            var order = orderRepository.GetSingle(w => w.Id == oid);

            OrderController orderController = new OrderController();

            if (order != null && user != null && orderController.IsUsersOrder(oid, user.Email))
            {
                var orderVideos = orderVideosRepository.GetAll(w => w.OrderPerformed.OrderImage.Order.Id == oid && w.Status.Equals(SNGModel.Utilities.Constants.ImageStatuses.Contains) && w.OrderPerformed.OrderImage.Order.CreationDate >= startdate && w.OrderPerformed.OrderImage.Order.CreationDate <= expiredate);

                List<CampaignReportModel> orderVideosList = orderVideos.GroupJoin(orderVideos, v => v.Videos.VideoLink, c => c.Videos.VideoLink, (v, c) => new CampaignReportModel
                {
                    Duration = c.Sum(o => o.Videos.VideoDuration),
                    UniqueVisitors = c.Sum(o => o.Videos.UniqueVisitors),
                    TimeWatched = c.Sum(o => o.Videos.TimeWatched),
                    AverageConcurrentViewers = (int)c.Sum(o => (o.Videos.UniqueVisitors) / c.Sum(b => b.Videos.VideoDuration)),
                    MoneySpent = c.Sum(o => (o.OrderPerformed.PricePerMinute * o.Videos.VideoDuration)),
                }).ToList();

                CampaignReportModel result = new CampaignReportModel();

                result.AverageConcurrentViewers = (int)orderVideosList.Average(v => v.AverageConcurrentViewers);
                result.OId = (int)oid;
                result.AveragePricePerMinute = orderVideos.Average(v => v.OrderPerformed.PricePerMinute);
                result.StartDate = (DateTime)orderVideos.First().OrderPerformed.OrderImage.Order.CreationDate;
                result.Duration = orderVideosList.Sum(v => v.Duration);
                result.MoneySpent = orderVideosList.Sum(v => v.MoneySpent);
                result.TimeWatched = orderVideosList.Sum(v => v.TimeWatched);
                result.UniqueVisitors = orderVideosList.Sum(v => v.UniqueVisitors);
                result.TotalVideos = orderVideosList.Count();
                result.TotalOrderPerformed = orderRepository.GetAll(v => v.Id == oid).Count();

                return result;
            }
        }

        return null;
    }

    private OrderReportModel GetReportSumOrderPerformed(int? opid, int? userId, DateTime? startdate, DateTime? expiredate)
    {
        if (opid != null && userId != null)
        {
            if (startdate == null)
                startdate = new DateTime(1970, 1, 1, new GregorianCalendar());

            if (expiredate == null)
                expiredate = DateTime.Today.AddDays(1);

            Repository<OrderPerformed> orderPerformedRepository = new Repository<OrderPerformed>(Entities);
            Repository<OrderVideos> orderVideosRepository = new Repository<OrderVideos>(Entities);

            var orderPerformed = orderPerformedRepository.GetSingle(w => w.Id == opid);

            OrderPerformedController orderPerformedController = new OrderPerformedController();

            if (orderPerformed != null && orderPerformedController.IsUsersOrderPerformed(opid, (int)userId))
            {
                if (orderPerformed.Status.Equals(SNGModel.Utilities.Constants.VerificationStatuses.Inaction) || orderPerformed.Status.Equals(SNGModel.Utilities.Constants.VerificationStatuses.Complete))
                {
                    var orderVideos = orderVideosRepository.GetAll(w => w.OrderPerformedId == opid && w.Status.Equals(SNGModel.Utilities.Constants.ImageStatuses.Contains) && w.OrderPerformed.StartDate >= startdate && w.OrderPerformed.StartDate <= expiredate);

                    List<OrderReportModel> orderVideosList = orderVideos.GroupJoin(orderVideos, v => v.Videos.VideoLink, c => c.Videos.VideoLink, (v, c) => new OrderReportModel
                    {
                        Duration = c.Sum(o => o.Videos.VideoDuration),
                        UniqueVisitors = c.Sum(o => o.Videos.UniqueVisitors),
                        TimeWatched = c.Sum(o => o.Videos.TimeWatched),
                        AverageConcurrentViewers = (int)c.Sum(o => (o.Videos.UniqueVisitors) / c.Sum(b => b.Videos.VideoDuration)),
                        MoneySpent = c.Sum(o => (o.OrderPerformed.PricePerMinute * o.Videos.VideoDuration)),
                    }).ToList();

                    OrderReportModel result = new OrderReportModel();

                    result.AverageConcurrentViewers = (int)orderVideosList.Average(v => v.AverageConcurrentViewers);
                    result.OPId = (int)opid;
                    result.PricePerMinute = orderVideos.First().OrderPerformed.PricePerMinute;
                    result.StartDate = (DateTime)orderVideos.First().OrderPerformed.StartDate;
                    result.Duration = orderVideosList.Sum(v => v.Duration);
                    result.MoneySpent = orderVideosList.Sum(v => v.MoneySpent);
                    result.TimeWatched = orderVideosList.Sum(v => v.TimeWatched);
                    result.UniqueVisitors = orderVideosList.Sum(v => v.UniqueVisitors);
                    result.TotalVideos = orderVideosList.Count();

                    return result;
                }
            }
        }

        return null;
    }

    [Authorize(Roles = "Advertiser")]
    public ActionResult OrderPerformedReport(int? id, DateTime? startdate, DateTime? expiredate)
    {
        if (id != null)
        {
            if (startdate == null)
                startdate = new DateTime(1970, 1, 1, new GregorianCalendar());

            if (expiredate == null)
                expiredate = DateTime.Today.AddDays(1);

            Repository<OrderPerformed> orderPerformedRepository = new Repository<OrderPerformed>(Entities);
            Repository<OrderVideos> orderVideosRepository = new Repository<OrderVideos>(Entities);

            var orderPerformed = orderPerformedRepository.GetSingle(w => w.Id == id);
            var userId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;

            OrderPerformedController orderPerformedController = new OrderPerformedController();

            if (orderPerformed != null && orderPerformedController.IsUsersOrderPerformed(id, userId))
            {
                if (orderPerformed.Status.Equals(SNGModel.Utilities.Constants.VerificationStatuses.Inaction) || orderPerformed.Status.Equals(SNGModel.Utilities.Constants.VerificationStatuses.Complete))
                {
                    var orderVideos = orderVideosRepository.GetAll(w => w.OrderPerformedId == id && w.Status.Equals(SNGModel.Utilities.Constants.ImageStatuses.Contains) && w.Videos.DateStreamed >= startdate && w.Videos.DateStreamed <= expiredate);

                    ViewBag.TotalVideos = orderVideos.Count();

                    if (orderVideos.Any())
                    {
                        List<OrderPerformedReportModel> orderPerformedReportList = new List<OrderPerformedReportModel>();

                        foreach (var v in orderVideos)
                        {
                            if (orderPerformedReportList.FirstOrDefault(w => w.Link.Equals(v.Videos.VideoLink)) != null)
                            {
                                var notUniqueLink = orderPerformedReportList.FirstOrDefault(w => w.Link.Equals(v.Videos.VideoLink));

                                try
                                {
                                    notUniqueLink.AverageConcurrentViewers += (int)(v.Videos.UniqueVisitors / v.Videos.VideoDuration);
                                }
                                catch (Exception)
                                { }

                                try
                                {
                                    notUniqueLink.UniqueVisitors += v.Videos.UniqueVisitors;
                                }
                                catch (Exception)
                                { }

                                try
                                {
                                    notUniqueLink.Duration += v.Videos.VideoDuration;
                                }
                                catch (Exception)
                                { }

                                try
                                {
                                    notUniqueLink.TimeWatched += v.Videos.TimeWatched;
                                }
                                catch (Exception)
                                { }

                                try
                                {
                                    notUniqueLink.MoneySpent += v.OrderPerformed.PricePerMinute * v.Videos.VideoDuration;
                                }
                                catch (Exception)
                                { }

                                notUniqueLink.VideoCounter++;
                            }
                            else
                            {
                                OrderPerformedReportModel orderPerformedReport = new OrderPerformedReportModel();

                                orderPerformedReport.VideoCounter = 1;

                                try
                                {
                                    orderPerformedReport.AverageConcurrentViewers = (int)(v.Videos.UniqueVisitors / v.Videos.VideoDuration);
                                }
                                catch (Exception)
                                {
                                    orderPerformedReport.AverageConcurrentViewers = 0;
                                }

                                try
                                {
                                    orderPerformedReport.Date = v.Videos.DateStreamed;
                                }
                                catch (Exception)
                                {
                                    orderPerformedReport.Date = new DateTime();
                                }

                                try
                                {
                                    orderPerformedReport.Duration = v.Videos.VideoDuration;
                                }
                                catch (Exception)
                                {
                                    orderPerformedReport.Duration = 0;
                                }

                                try
                                {
                                    orderPerformedReport.Link = v.Videos.VideoLink;
                                }
                                catch (Exception)
                                {
                                    orderPerformedReport.Link = "none";
                                }

                                try
                                {
                                    orderPerformedReport.TimeWatched = v.Videos.TimeWatched;
                                }
                                catch (Exception)
                                {
                                    orderPerformedReport.TimeWatched = 0;
                                }

                                try
                                {
                                    orderPerformedReport.UniqueVisitors = v.Videos.UniqueVisitors;
                                }
                                catch (Exception)
                                {
                                    orderPerformedReport.UniqueVisitors = 0;
                                }

                                try
                                {
                                    orderPerformedReport.MoneySpent = v.OrderPerformed.PricePerMinute * v.Videos.VideoDuration;
                                }
                                catch (Exception)
                                {
                                    orderPerformedReport.MoneySpent = 0;
                                }

                                orderPerformedReportList.Add(orderPerformedReport);
                            }
                        }

                        if (orderPerformedReportList.Any())
                        {
                            SumOrderPerformedReportModel sumOrderPerformedReport = new SumOrderPerformedReportModel();

                            try
                            {
                                sumOrderPerformedReport.SumAverageConcurrentViewers = (int)orderPerformedReportList.Average(op => op.AverageConcurrentViewers);
                            }
                            catch (Exception)
                            {
                                sumOrderPerformedReport.SumAverageConcurrentViewers = 0;
                            }

                            try
                            {
                                sumOrderPerformedReport.SumDuration = orderPerformedReportList.Sum(op => op.Duration);
                            }
                            catch (Exception)
                            {
                                sumOrderPerformedReport.SumDuration = 0;
                            }

                            try
                            {
                                sumOrderPerformedReport.SumTimeWatched = orderPerformedReportList.Sum(op => op.TimeWatched);
                            }
                            catch (Exception)
                            {
                                sumOrderPerformedReport.SumTimeWatched = 0;
                            }

                            try
                            {
                                sumOrderPerformedReport.SumUniqueVisitors = orderPerformedReportList.Sum(op => op.UniqueVisitors);
                            }
                            catch (Exception)
                            {
                                sumOrderPerformedReport.SumUniqueVisitors = 0;
                            }

                            try
                            {
                                sumOrderPerformedReport.SumMoneySpent = orderPerformedReportList.Sum(op => op.MoneySpent);
                            }
                            catch (Exception)
                            {
                                sumOrderPerformedReport.SumMoneySpent = 0;
                            }

                            try
                            {
                                sumOrderPerformedReport.SumAverageConcurrentViewers /= orderPerformedReportList.Count;
                            }
                            catch (Exception)
                            {
                                sumOrderPerformedReport.SumAverageConcurrentViewers /= 1;
                            }

                            ViewBag.SumOrderPerformedReport = sumOrderPerformedReport;
                            ViewBag.OrderPerformedId = id;

                            try
                            {
                                ViewBag.PricePerMinute = orderVideos.First().OrderPerformed.PricePerMinute;
                            }
                            catch (Exception)
                            {
                                ViewBag.PricePerMinute = 0;
                            }

                            try
                            {
                                ViewBag.StartDate = orderPerformedReportList.Min(v => v.Date);
                            }
                            catch (Exception)
                            {
                                ViewBag.StartDate = new DateTime(2013, 1, 1, new GregorianCalendar());
                            }

                            ViewBag.Id = id;

                            return View(orderPerformedReportList);
                        }
                    }
                }

                return View();
            }
            else
            {
                if (User.IsInRole("Advertiser"))
                    return RedirectToAction("ManageOrderPerformedForAdvertiser", "OrderPerformed", new { success = "notfororder" });
                else if (User.IsInRole(VodadModel.Utilities.Constants.UserRoles.Performer))
                    return RedirectToAction("ManageOrderPerformedForPerformer", "OrderPerformed", new { success = "notfororder" });
            }
        }

        return RedirectToAction("Error404", "Error");
    }

    [Authorize(Roles = "Advertiser")]
    public ActionResult OrderReport(int id, DateTime? startdate, DateTime? expiredate)
    {
        if (id != null)
        {
            if (startdate == null)
                startdate = new DateTime(1970, 1, 1, new GregorianCalendar());

            if (expiredate == null)
                expiredate = DateTime.Today.AddDays(1);

            Repository<Order> orderRepository = new Repository<Order>(Entities);

            var order = orderRepository.GetSingle(w => w.Id == id);

            if (User.Identity.Name != null)
            {
                var user = UserHelper.GetUserByEmail(User.Identity.Name);

                OrderController orderController = new OrderController();

                if (order != null && user != null && orderController.IsUsersOrder(id, user.Email))
                {
                    List<OrderReportModel> orderPerformedSumList = new List<OrderReportModel>();

                    foreach (var oi in order.OrderImages)
                    {
                        foreach (var op in oi.OrderPerformeds)
                        {
                            try
                            {
                                var orderReportElement = GetReportSumOrderPerformed((int)op.Id, (int)user.Id, startdate, expiredate);

                                if (orderReportElement != null)
                                    orderPerformedSumList.Add(orderReportElement);
                            }
                            catch (Exception)
                            { }
                        }
                    }

                    if (orderPerformedSumList.Any())
                    {
                        SumOrderReportModel sumOrderReport = new SumOrderReportModel();

                        try
                        {
                            sumOrderReport.AverageConcurrentViewers = (int)orderPerformedSumList.Average(v => v.AverageConcurrentViewers);
                        }
                        catch (Exception)
                        { }

                        try
                        {
                            sumOrderReport.AveragePricePerMinute = orderPerformedSumList.Average(v => v.PricePerMinute);
                        }
                        catch (Exception)
                        { }

                        try
                        {
                            sumOrderReport.SumDuration = orderPerformedSumList.Sum(v => v.Duration);
                        }
                        catch (Exception)
                        { }

                        try
                        {
                            sumOrderReport.SumMoneySpent = orderPerformedSumList.Sum(v => v.MoneySpent);
                        }
                        catch (Exception)
                        { }

                        try
                        {
                            sumOrderReport.SumTimeWatched = orderPerformedSumList.Sum(v => v.TimeWatched);
                        }
                        catch (Exception)
                        { }

                        try
                        {
                            sumOrderReport.SumUniqueVisitors = orderPerformedSumList.Sum(v => v.UniqueVisitors);
                        }
                        catch (Exception)
                        { }

                        try
                        {
                            sumOrderReport.TotalVideos = orderPerformedSumList.Sum(v => v.TotalVideos);
                        }
                        catch (Exception)
                        { }

                        ViewBag.SumOrderReport = sumOrderReport;

                        try
                        {
                            ViewBag.StartDate = orderPerformedSumList.Min(v => v.StartDate);
                        }
                        catch (Exception)
                        {
                            ViewBag.StartDate = new DateTime(2013, 1, 1, new GregorianCalendar());
                        }

                        ViewBag.Id = id;

                        return View(orderPerformedSumList);
                    }
                }
            }
        }

        return View();
    }
}
}
*/