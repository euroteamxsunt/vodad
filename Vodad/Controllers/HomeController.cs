using System.Linq;
using System.Web.Mvc;
using VodadModel.Repository;
using VodadModel;
using VodadModel.Helpers;
using System;
using Vodad.Models;

namespace Vodad.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult AdvertiserGuide()
        {
            return View();
        }

        public ActionResult Banners()
        {
            if (User.Identity.IsAuthenticated)
            {
                int userId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;
                ViewBag.Referral = "http://localhost:56975/Account/Register?r=" + userId;
            }

            return View();
        }

        public ActionResult Index(string success)
        {
            Response.Cookies["ASP.NET_SessionId"].Secure = true;

            var performerPlatformRepository = new Repository<PerformerPlatform>(Entities);
            var orderPerformedRepository = new Repository<OrderPerformed>(Entities);
            var performerStatisticsRepository = new Repository<PerformerStatistics>(Entities);
            var newsRepository = new Repository<News>(Entities);

            // Получить три последних новости
            try
            {
                var newsList = newsRepository.GetAll().ToList();

                newsList.Reverse();

                if (newsList.Count > 3)
                {
                    newsList = newsList.Take(3).ToList();
                }

                ViewBag.ThreeLastNews = newsList;

                ViewBag.VerifiedPerformerPlatformsCount =
                    performerPlatformRepository.GetAll(
                        w => w.Verified.Equals(VodadModel.Utilities.Constants.CredentialCheckStatuses.Verified)).Count();
                ViewBag.CompleteOrderPerformedCount =
                    orderPerformedRepository.GetAll(
                        w => w.Status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Complete)).Count();
                ViewBag.ViewersPerHour = performerStatisticsRepository.GetAll().Sum(w => w.AverageViewerCountPerHour);
            }
            catch (Exception) { }

            if (User.Identity.IsAuthenticated)
            {

                try
                {
                    if (User.IsInRole(VodadModel.Utilities.Constants.UserRoles.Advertiser))
                        return RedirectToAction("OrderPerformedStatistics", "OrderPerformed", new { success });
                    else
                        if (User.IsInRole(VodadModel.Utilities.Constants.UserRoles.Performer))
                            return RedirectToAction("ManagePlatforms", "PerformerPlatform", new { success });
                        else if (User.IsInRole("Banned") || User.IsInRole("Helper"))
                        {
                            return RedirectToAction("ManageTickets", "Ticket");
                        }
                        else if (User.IsInRole(VodadModel.Utilities.Constants.UserRoles.Administrator))
                        {
                            return RedirectToAction("Index", "Administrator");
                        }
                        else
                        {
                            var userId = (int?)UserHelper.GetUserByEmail(User.Identity.Name).Id;

                            ViewBag.Role = UserHelper.GetUserById(userId).Roles.RoleName;
                        }
                }
                catch (NullReferenceException)
                {
                    return RedirectToAction("LogOff", "Account");
                }
            }

            return View();
        }

        public ActionResult FAQ()
        {
            return View();
        }

        public ActionResult License()
        {
            return View();
        }

        public ActionResult Policy()
        {
            return View();
        }

        public ActionResult Welcome()
        {
            return View();
        }

        public ActionResult PerformerGuide()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "The team of Vodad co.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize(Roles = "0")]
        public ActionResult Admin()
        {
            return View();
        }

        public ActionResult UserName()
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    ViewBag.UserName = UserHelper.GetUserByEmail(User.Identity.Name).Name;

                    if (ViewBag.UserName == null || ViewBag.UserName.Equals(""))
                    {
                        ViewBag.UserName = UserHelper.GetUserByEmail(User.Identity.Name).Email;
                    }

                    return PartialView();
                }
                catch (NullReferenceException) { }
            }

            return null;
        }

        public ActionResult Wallet()
        {
            if (User.Identity.IsAuthenticated)
            {
                var wallet = new WalletHelper();

                try
                {
                    var userId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;


                    ViewBag.Wallet = wallet.GetUsersWallet(userId);

                    // Проверка наличия непрочитанных сообщений
                    var messagesRepo = new Repository<Messages>(Entities);

                    var blackAndWhiteListController = new BlackAndWhiteListsController();

                    var messages = messagesRepo.GetAll(w => (w.ToUserId == userId && w.IsRead == false && w.IsDeletedForReciever == false));

                    if (messages.Any())
                    {
                        bool b = true;

                        foreach (var m in messages)
                            if (blackAndWhiteListController.IsUserInBlackList((int)m.FromUserId, userId))
                                b = false;

                        if (b)
                            ViewBag.HasUnreadMessages = "true";
                    }

                    // Проверка есть ли answered тикеты
                    Repository<Tickets> ticketRepository = new Repository<Tickets>(Entities);

                    var tickets =
                        ticketRepository.GetAll(
                            w =>
                            w.CreatorId == userId && w.Status.Equals(VodadModel.Utilities.Constants.TicketStatuses.Answered));

                    if (tickets.Any())
                    {
                        bool b = false;

                        foreach (var t in tickets)
                        {
                            if (!ticketRepository.GetAll(w => w.ParentTicketId == t.Id).Any())
                            {
                                b = true;
                                break;
                            }
                        }

                        if (b)
                            ViewBag.HasUnansweredTicket = "true";
                    }

                    if (User.IsInRole(VodadModel.Utilities.Constants.UserRoles.Performer))
                    {
                        var userRepository = new Repository<User>(Entities);

                        ViewBag.Karma = userRepository.GetSingle(w => w.Id == userId).Karma;
                    }

                    return PartialView();
                }
                catch (NullReferenceException e)
                {
                    return null;
                }
            }
            else
                return null;
        }
    }
}