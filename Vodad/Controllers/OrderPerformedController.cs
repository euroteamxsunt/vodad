using VodadModel;
using VodadModel.Repository;
using Vodad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Globalization;
using VodadModel.Helpers;
using System.Data.Objects;

namespace Vodad.Controllers
{
    public class OrderPerformedController : BaseController
    {
        public void ChangeUsersKarma(int opid, int userId)
        {
            if (IsUsersOrderPerformed(opid, userId))
            {
                Repository<User> userRepository = new Repository<User>(Entities);
                Repository<OrderPerformed> orderPerformedRepository = new Repository<OrderPerformed>(Entities);
                Repository<PerformerPlatform> performerPlatformRepository = new Repository<PerformerPlatform>(Entities);

                var orderPerformed = orderPerformedRepository.GetSingle(w => w.Id == opid);
                var user = userRepository.GetSingle(w => w.Id == orderPerformed.PerformerPlatform.User.Id);

                var performerPlatforms =
                    performerPlatformRepository.GetAll(
                        w =>
                        w.PerformerId == user.Id &&
                        w.Verified.Equals(VodadModel.Utilities.Constants.CredentialCheckStatuses.Verified));

                int totalCompletedOrders = 0;
                foreach (var sp in performerPlatforms)
                {
                    totalCompletedOrders += (int)sp.PerformerStatistics.FirstOrDefault(w => w.PerformerPlatformId == sp.Id).TotalOrders;
                }

                if (totalCompletedOrders != 0)
                    user.Karma = 100 - (user.Rating / totalCompletedOrders);

                userRepository.Save();

                Logger.Info(string.Format("User {0} id = {1} karma has been shanged", user.Email, user.Id));
            }
        }

        public void ChangeOrderPerformedIsLiked(int? opid)
        {
            var orderPerformedRepository = new Repository<OrderPerformed>(Entities);

            var orderPerformed = orderPerformedRepository.GetSingle(w => w.Id == opid);

            orderPerformed.IsLiked = true;

            orderPerformedRepository.Save();

            Logger.Info(string.Format("OrderPerformed id = {0} has been liked", opid));
        }

        [Authorize(Roles = "Performer, Advertiser, Administrator")]
        public ActionResult ChangeOrderPerformedStatus(int opid, string status, int userId)
        {
            var orderPerformedRepository = new Repository<OrderPerformed>(Entities);

            var orderPerformed = orderPerformedRepository.GetSingle(w => w.Id == opid);

            bool b = true;

            // Если Advertiser подтверждает OP, необходимо заблокировать деньги
            if (IsUsersOrderPerformed(opid, userId) && orderPerformed.AuthorId != orderPerformed.OrderContent.Order.User.Id && status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Inaction))
            {
                var walletHelper = new WalletHelper();

                b = walletHelper.ChangeWalletState(orderPerformed, VodadModel.Utilities.Constants.OrdersStatuses.Block, userId);
            }
            else
                // Если Performer отклоняет заказ, необходимо разблокировать деньги
                if (IsUsersOrderPerformed(opid, userId) && orderPerformed.AuthorId == orderPerformed.OrderContent.Order.User.Id && orderPerformed.PerformerPlatform.User.Id == userId && status.Equals(VodadModel.Utilities.Constants.OrdersStatuses.Deny))
                {
                    var walletHelper = new WalletHelper();

                    b = walletHelper.ChangeWalletState(orderPerformed, VodadModel.Utilities.Constants.OrdersStatuses.Unblock, userId);
                }
                else if (IsUsersOrderPerformed(opid, userId) && orderPerformed.AuthorId == orderPerformed.OrderContent.Order.User.Id && status.Equals(VodadModel.Utilities.Constants.OrdersStatuses.Deny))
                {
                    var walletHelper = new WalletHelper();

                    b = walletHelper.ChangeWalletState(orderPerformed, VodadModel.Utilities.Constants.OrdersStatuses.Unblock, userId);
                }

            if (!b)
                return null;

            if (IsUsersOrderPerformed(opid, userId)/* && !orderPerformed.AuthorId.Equals(userId)*/ && (status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Inaction) || status.Equals(VodadModel.Utilities.Constants.OrdersStatuses.Deny)))
            {
                orderPerformed.Status = status;
                orderPerformed.LastStatusChangeDateTime = DateTime.Today;
                orderPerformedRepository.Save();
                Logger.Info(string.Format("Order performed {0} status has been changed to {1} by {2}", orderPerformed.Id, status, userId));
            }

            // Сделать после walletcontroller'а расчет
            // Если заказ выполнен, необходимо разблокировать деньги и расплатиться
            if (IsUsersOrderPerformed(opid, userId) && /*!orderPerformed.AuthorId.Equals(userId) && */(status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Complete)))
            {
                orderPerformed.Status = status;
                orderPerformed.LastStatusChangeDateTime = DateTime.Today;
                orderPerformedRepository.Save();

                Logger.Info(string.Format("Order performed {0} status has been changed to {1} by {2}", orderPerformed.Id, status, userId));

                Repository<PerformerStatistics> PerformerStatisticsRepository = new Repository<PerformerStatistics>(Entities);

                var PerformerStatistics =
                    PerformerStatisticsRepository.GetSingle(
                        w => w.PerformerPlatformId == orderPerformed.PerformerPlatform.Id);

                PerformerStatistics.CompletedOrders++;
                PerformerStatistics.TotalOrders++;

                PerformerStatisticsRepository.Save();

                ChangeUsersKarma(opid, userId);

                var walletHelper = new WalletHelper();

                if (walletHelper.ChangeWalletState(orderPerformed, VodadModel.Utilities.Constants.OrdersStatuses.Pay, userId))
                {
                    Logger.Info(string.Format("Order performed {0} status has been changed to {1} by {2}", orderPerformed.Id, status, userId));

                    return RedirectToAction("ManageOrderPerformedForAdvertiser", "OrderPerformed", new { status = VodadModel.Utilities.Constants.VerificationStatuses.Inaction });
                }
                else
                {
                    Logger.Error("Something went wrong with money transaction for orderPerformed {0}", orderPerformed.Id);
                }
            }

            return null;
        }

        public bool IsUsersOrderPerformed(int? opid, int userId)
        {
            var orderPerformedRepository = new Repository<OrderPerformed>(Entities);

            var orderPerformed = orderPerformedRepository.GetSingle(w => w.Id == opid);

            if (orderPerformed.PerformerPlatform.User.Id == userId || orderPerformed.OrderContent.Order.User.Id == userId)
                return true;
            else
                return false;
        }

        [Authorize(Roles = VodadModel.Utilities.Constants.UserRoles.Administrator)]
        public ActionResult LowerKarmaForPerformer(int opid, string status)
        {
            Repository<User> userRepository = new Repository<User>(Entities);

            var user = userRepository.GetSingle(w => w.Email.Equals(User.Identity.Name));

            if (user != null && IsUsersOrderPerformed(opid, (int)user.Id))
            {
                var orderPerformedRepository = new Repository<OrderPerformed>(Entities);

                var orderPerformed = orderPerformedRepository.GetSingle(w => w.Id == opid);

                if (orderPerformed.Status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Complete) && orderPerformed.IsLiked == false)
                {
                    var userId = (int)orderPerformed.PerformerPlatform.User.Id;
                    var Performer = userRepository.GetSingle(w => w.Id == userId);

                    Performer.Rating++;

                    ChangeUsersKarma(opid, (int)user.Id);

                    userRepository.Save();

                    Logger.Info(string.Format("User {0} id = {1} karma has been lowered", Performer.Email, Performer.Id));
                }
            }

            return RedirectToAction("ManageOrderPerformedForAdvertiser", "OrderPerformed", new { status });
        }

        [HttpPost]
        [Authorize]
        public ActionResult ManageOrderPerformed(string submit, ICollection<string> checkme)
        {
            string success = VodadModel.Utilities.Constants.AlertMessages.Failed;

            var user = UserHelper.GetUserByEmail(User.Identity.Name);

            if (checkme.Any())
            {
                if (submit.Equals("Accept"))
                {
                    foreach (var c in checkme)
                    {
                        if (!c.Equals("false"))
                            ChangeOrderPerformedStatus(int.Parse(c), VodadModel.Utilities.Constants.VerificationStatuses.Inaction, (int)user.Id);
                    }

                    success = "acceptsuccess";
                }
                else
                    if (submit.Equals("Decline"))
                    {
                        foreach (var c in checkme)
                        {
                            if (!c.Equals("false"))
                                ChangeOrderPerformedStatus(int.Parse(c), VodadModel.Utilities.Constants.OrdersStatuses.Deny, (int)user.Id);
                        }

                        success = "declinesuccess";
                    }
                    else
                        if (submit.Equals("Cancel"))
                        {
                            foreach (var c in checkme)
                            {
                                if (!c.Equals("false"))
                                    ChangeOrderPerformedStatus(int.Parse(c),
                                                               VodadModel.Utilities.Constants.OrdersStatuses.Deny, (int)user.Id);
                            }

                            success = "cancelsuccess";
                        }

                return RedirectToAction("Index", "Home", new { success });
            }

            return RedirectToAction("Error404", "Error");
        }

        [Authorize(Roles = VodadModel.Utilities.Constants.UserRoles.Advertiser)]
        public ActionResult ManageOrderPerformedForAdvertiser(string status, string author, string success)
        {
            if (success != null)
            {
                if (success.Equals("notfororder"))
                {
                    ViewBag.AlertMessage = "This order has no reports yet.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
            }

            var orderPerformedRepository = new Repository<OrderPerformed>(Entities);

            var orderPerformedList = new List<OrdersPerformedListForPerformerModel>();

            var userId = (int?)UserHelper.GetUserByEmail(User.Identity.Name).Id;

            IQueryable<OrderPerformed> ordersPerformed = null;

            if (status != null && (status == VodadModel.Utilities.Constants.VerificationStatuses.Request || status == VodadModel.Utilities.Constants.VerificationStatuses.Inaction || status == VodadModel.Utilities.Constants.VerificationStatuses.Complete))
            {
                if (author != null && author.Equals(User.Identity.Name) && status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Request))
                    ordersPerformed = orderPerformedRepository.GetAll(w => w.AuthorId == userId && w.Status.Equals(status));
                else
                    if (status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Request))
                        ordersPerformed = orderPerformedRepository.GetAll(w => w.AuthorId != userId && (w.PerformerPlatform.User.Id == userId || w.OrderContent.Order.User.Id == userId) && w.Status.Equals(status));
                    else
                        ordersPerformed = orderPerformedRepository.GetAll(w => (w.PerformerPlatform.User.Id == userId || w.OrderContent.Order.User.Id == userId) && w.Status.Equals(status));
            }
            else
                if (status == null)
                {
                    ordersPerformed = orderPerformedRepository.GetAll(w => (w.PerformerPlatform.User.Id == userId || w.OrderContent.Order.User.Id == userId));
                }

            if (ordersPerformed != null)
            {
                foreach (var op in ordersPerformed)
                {
                    if (op.Status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Complete) && op.LastStatusChangeDateTime < DateTime.Today.AddDays(-10))
                        ChangeOrderPerformedIsLiked((int)op.Id);

                    var orderPerformedListElement = new OrdersPerformedListForPerformerModel();

                    orderPerformedListElement.UserName = UserHelper.GetUserNameByEmail(User.Identity.Name);
                    orderPerformedListElement.AuthorName = UserHelper.GetUserNameByEmail(UserHelper.GetUserById((int?)op.AuthorId).Email);
                    orderPerformedListElement.PerformerId = (int)op.PerformerPlatform.User.Id;
                    orderPerformedListElement.PerformerName = UserHelper.GetUserNameByEmail(UserHelper.GetUserById(orderPerformedListElement.PerformerId).Email);
                    orderPerformedListElement.AuthorId = (int)op.AuthorId;
                    orderPerformedListElement.OrderPerformedId = (int?)op.Id;
                    orderPerformedListElement.Status = op.Status;
                    orderPerformedListElement.MoneyPaid = op.MoneyPaid;
                    orderPerformedListElement.OrderComment = op.OrderContent.Order.Comment;

                    try
                    {
                        orderPerformedListElement.VideoLink = op.VideoLink;
                    }
                    catch (Exception)
                    {
                        orderPerformedListElement.VideoLink = "none";
                    }
                    orderPerformedListElement.ActionStatus = "";

                    // Проверка возможности выставить негативную оценку
                    if (op.Status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Complete) && op.IsLiked == false)
                        orderPerformedListElement.ActionStatus = VodadModel.Utilities.Constants.ActionOrderPerformedStatus.IsNotLiked;

                    // Проверка можно ли закрыть давно неактивный OrderPerformed
                    if (op.Status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Inaction) && op.LastStatusChangeDateTime < DateTime.Today.AddDays(-10))
                        orderPerformedListElement.ActionStatus = VodadModel.Utilities.Constants.ActionOrderPerformedStatus.CanBeDeleted;

                    orderPerformedList.Add(orderPerformedListElement);
                }
            }

            ViewBag.Author = author;
            ViewBag.Status = status;
            ViewBag.UserId = userId;

            return View(orderPerformedList);
        }

        [Authorize(Roles = "Performer")]
        public ActionResult ManageOrderPerformedForPerformer(int? ppid, string status, string author, string success)
        {
            if (success != null)
            {
                if (success.Equals("notfororder"))
                {
                    ViewBag.AlertMessage = "This order has no reports yet.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
            }

            var performerPlatform = new PerformerPlatformController();

            var userId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;

            if (performerPlatform.IsUsersPerformerPlatform(ppid, userId))
            {
                var orderPerformedRepository = new Repository<OrderPerformed>(Entities);

                var orderPerformedList = new List<OrdersPerformedListForPerformerModel>();

                IQueryable<OrderPerformed> ordersPerformed = null;

                if (status != null && (status == VodadModel.Utilities.Constants.VerificationStatuses.Request || status == VodadModel.Utilities.Constants.VerificationStatuses.Inaction || status == VodadModel.Utilities.Constants.VerificationStatuses.Complete))
                {

                    if (author != null && author.Equals(User.Identity.Name) && status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Request))
                        ordersPerformed = orderPerformedRepository.GetAll(w => w.AuthorId == userId && w.Status.Equals(status) && w.PerformerPlatformId == ppid);
                    else
                        if (status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Request))
                            ordersPerformed = orderPerformedRepository.GetAll(w => w.AuthorId != userId && (w.PerformerPlatform.User.Id == userId || w.OrderContent.Order.User.Id == userId) && w.Status.Equals(status) && w.PerformerPlatformId == ppid);
                        else
                            ordersPerformed = orderPerformedRepository.GetAll(w => (w.PerformerPlatform.User.Id == userId || w.OrderContent.Order.User.Id == userId) && w.Status.Equals(status) && w.PerformerPlatformId == ppid);
                }
                else
                    if (status == null)
                    {
                        ordersPerformed = orderPerformedRepository.GetAll(w => (w.PerformerPlatform.User.Id == userId || w.OrderContent.Order.User.Id == userId));
                    }

                if (ordersPerformed != null)
                {
                    foreach (var op in ordersPerformed)
                    {
                        var orderPerformedListElement = new OrdersPerformedListForPerformerModel();

                        orderPerformedListElement.UserName = UserHelper.GetUserNameByEmail(User.Identity.Name);
                        orderPerformedListElement.AuthorName = UserHelper.GetUserNameByEmail(UserHelper.GetUserById((int?)op.AuthorId).Email);
                        orderPerformedListElement.PerformerId = (int)op.PerformerPlatform.User.Id;
                        orderPerformedListElement.PerformerName = UserHelper.GetUserNameByEmail(UserHelper.GetUserById(orderPerformedListElement.PerformerId).Email);
                        orderPerformedListElement.AuthorId = (int)op.AuthorId;
                        orderPerformedListElement.OrderPerformedId = (int?)op.Id;
                        orderPerformedListElement.Status = op.Status;
                        orderPerformedListElement.MoneyPaid = op.MoneyPaid;
                        orderPerformedListElement.OrderComment = op.OrderContent.Order.Comment;
                        orderPerformedListElement.ContentType = op.OrderContent.ContentType;

                        try
                        {
                            orderPerformedListElement.VideoLink = op.VideoLink;
                        }
                        catch (Exception)
                        {
                            orderPerformedListElement.VideoLink = "none";
                        }

                        orderPerformedList.Add(orderPerformedListElement);
                    }
                }

                ViewBag.Author = author;
                ViewBag.Status = status;

                if (orderPerformedList.Count > 0)
                    return View(orderPerformedList);
                else
                    return RedirectToAction("BannerMeasureList", "BannerMeasure");
            }
            else
                return RedirectToAction("ManageBannerMeasure", "BannerMeasure");
        }

        [Authorize(Roles = "Advertiser")]
        public ActionResult OrderPerformedStatistics(string success)
        {
            if (success != null)
            {
                if (success.Equals("sentorder"))
                {
                    ViewBag.AlertMessage = "Order has been sent to Performer";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                }
                else if (success.Equals("sentorderfailed"))
                {
                    ViewBag.AlertMessage = "Something went wrong. Please, try again later";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
                else if (success.Equals("acceptsuccess"))
                {
                    ViewBag.AlertMessage = "Order(s) has been accepted";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                }
                else if (success.Equals("declinesuccess"))
                {
                    ViewBag.AlertMessage = "Order(s) has been declined";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                }
                else if (success.Equals("cancelsuccess"))
                {
                    ViewBag.AlertMessage = "Order(s) has been canceled";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                }
            }

            var orderPerformedRepository = new Repository<OrderPerformed>(Entities);
            var userRepository = new Repository<User>(Entities);

            var user = userRepository.GetSingle(w => w.Email.Equals(User.Identity.Name));

            if (user != null)
            {
                user.LastOnlineDateTime = DateTime.Now;
                userRepository.Save();
            }

            int userId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;

            var orderPerformed = orderPerformedRepository.GetAll(w => w.OrderContent.Order.User.Id == userId && w.Status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Inaction));

            foreach (var op in orderPerformed)
            {
                if (op.Status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Request) && op.LastStatusChangeDateTime < DateTime.Today.AddDays(-7))
                {
                    ChangeOrderPerformedStatus((int)op.Id, VodadModel.Utilities.Constants.OrdersStatuses.Deny, userId);
                }
            }

            var orderPerformedStatistics = new OrderPerformedStatisticsModel();

            orderPerformedStatistics.UsersRequestOrderPerformedCount = orderPerformedRepository.GetAll(w => w.OrderContent.Order.User.Id == userId && w.AuthorId == userId && w.Status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Request)).Count().ToString(CultureInfo.InvariantCulture);
            orderPerformedStatistics.ToUserRequestOrderPerformedCount = orderPerformedRepository.GetAll(w => w.OrderContent.Order.User.Id == userId && w.Status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Request) && w.AuthorId != userId).Count().ToString(CultureInfo.InvariantCulture);
            orderPerformedStatistics.InactionOrderPerformedCount = orderPerformedRepository.GetAll(w => w.OrderContent.Order.User.Id == userId && w.Status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Inaction)).Count().ToString(CultureInfo.InvariantCulture);
            orderPerformedStatistics.CompleteOrderPerformedCount = orderPerformedRepository.GetAll(w => w.OrderContent.Order.User.Id == userId && w.Status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Complete)).Count().ToString(CultureInfo.InvariantCulture);
            var completeOrders = orderPerformedRepository.GetAll(w => w.Status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Complete) && w.OrderContent.Order.User.Id == userId);

            decimal? spent = 0;
            foreach (var co in completeOrders)
                spent += co.MoneyPaid;

            orderPerformedStatistics.Spent = spent;

            return View(orderPerformedStatistics);
        }

        [Authorize(Roles = "Performer")]
        public ActionResult SendOrderToAdvertiser(int? ppid, int? ocid)
        {
            var performerPlatformController = new PerformerPlatformController();
            var orderContentRepository = new Repository<OrderContent>(Entities);

            var orderContent = orderContentRepository.GetSingle(w => w.Id == ocid);
            Image image = new Image();
            Video video = new Video();

            if (orderContent != null)
            {
                if (orderContent.ContentType.FirstOrDefault().Equals(VodadModel.Utilities.Constants.ContentType.Image))
                {
                    var imageRepository = new Repository<Image>(Entities);
                    image = imageRepository.GetSingle(w => w.Id == ocid);
                }
                else if (orderContent.ContentType.FirstOrDefault().Equals(VodadModel.Utilities.Constants.ContentType.Video))
                {
                    var videoRepository = new Repository<Video>(Entities);
                    video = videoRepository.GetSingle(w => w.Id == ocid);
                }
            }

            var userId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;

            if ((video != null || image != null) && ModelState.IsValid && performerPlatformController.IsUsersPerformerPlatform(ppid, userId))
            {
                Repository<User> userRepository = new Repository<User>(Entities);

                var advertiser = new User();

                var performerPlatofrmRepository = new Repository<PerformerPlatform>(Entities);

                var orderRepository = new Repository<Order>(Entities);
                var order = new Order();
                var orderPerformed = new OrderPerformed();

                if (image != null)
                {
                    advertiser = userRepository.GetSingle(w => w.Id == image.UserId);
                    orderPerformed.OrderContentId = image.Id;
                    order = orderRepository.GetSingle(w => w.OrderContent.FirstOrDefault(v => v.IdContent == image.Id).IdContent == image.Id && w.OrderContent.FirstOrDefault(v => v.IdContent == image.Id).ContentType.Equals(VodadModel.Utilities.Constants.ContentType.Image));
                }
                else if (video != null)
                {
                    advertiser = userRepository.GetSingle(w => w.Id == video.UserId);
                    orderPerformed.OrderContentId = video.Id;
                    order = orderRepository.GetSingle(w => w.OrderContent.FirstOrDefault(v => v.IdContent == video.Id).IdContent == video.Id && w.OrderContent.FirstOrDefault(v => v.IdContent == video.Id).ContentType.Equals(VodadModel.Utilities.Constants.ContentType.Video));
                }

                var user = UserHelper.GetUserByEmail(User.Identity.Name);

                orderPerformed.AuthorId = user.Id;

                orderPerformed.PerformerPlatformId = ppid;
                orderPerformed.Status = VodadModel.Utilities.Constants.VerificationStatuses.Request;
                orderPerformed.StartDate = DateTime.Now;
                orderPerformed.MoneyPaid = 0;
                orderPerformed.LastStatusChangeDateTime = DateTime.Today;
                orderPerformed.IsLiked = false;

                var orderPerformedRepository = new Repository<OrderPerformed>(Entities);
                orderPerformedRepository.Add(orderPerformed);
                orderPerformedRepository.Save();
                Logger.Info(string.Format("Order performed {0} has been created by {1}", orderPerformed.Id, User.Identity.Name));

                if (advertiser != null && advertiser.LastEmailSendDateTime != DateTime.Today)
                {
                    string messageBody = advertiser.Name +
                                         ",\r\n You have recieved new request to Your order - http://showngain.com/ \r\n\r\n Thank you. \r\n ShowNGain team.";
                    const string messageSubject = "ShowNGain new event";
                    UserHelper.SendMessage(advertiser.Email, advertiser.Name, messageBody, messageSubject);

                    advertiser.LastEmailSendDateTime = DateTime.Today;

                    userRepository.Save();

                    Logger.Info(string.Format("Message {0} was sent to {1} and his LastEmailSendDateTime has been changed to {2}", messageSubject, advertiser.Email, advertiser.LastEmailSendDateTime));
                }
            }

            return RedirectToAction("BannerMeasureList", "BannerMeasure", new { success = "sentorder" });
        }

        [Authorize(Roles = "Advertiser")]
        public ActionResult SendOrderToPerformer(int? ppid, int? oid, string success)
        {
            if (success != null)
            {
                if (success.Equals("contentsizefailed"))
                {
                    ViewBag.AlertMessage = "Content size is too large.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
            }

            var orderController = new OrderController();
            Repository<Order> orderRepository = new Repository<Order>(Entities);
            Repository<PerformerPlatform> performerPlatformRepository = new Repository<PerformerPlatform>(Entities);

            var order = orderRepository.GetSingle(w => w.Id == oid);
            var bannerMeasure = performerPlatformRepository.GetSingle(w => w.Id == ppid);

            var orderPerformedToPerformerModel = new OrderPerformedToPerformerModel();

            if (order != null && bannerMeasure != null && orderController.IsUsersOrder(oid, User.Identity.Name) && order.Status == VodadModel.Utilities.Constants.OrdersStatuses.Open)
            {
                var imageRepository = new Repository<Image>(Entities);
                var videoRepository = new Repository<Video>(Entities);
                var OrderContentsRepository = new Repository<OrderContent>(Entities);

                var OrderContentsList = OrderContentsRepository.GetAll(w => w.IdOrder == oid).ToList();

                var imagesList = new List<Image>();
                var videoList = new List<Video>();

                orderPerformedToPerformerModel.ExpireDate = DateTime.Today.ToString("MM/dd/yyyy", DateTimeFormatInfo.InvariantInfo);

                foreach (var o in OrderContentsList)
                {
                    if (o.ContentType.Equals(VodadModel.Utilities.Constants.ContentType.Image))
                    {
                        var image = imageRepository.GetSingle(w => w.Id == o.IdContent);
                        imagesList.Add(image);
                    }
                    else if (o.ContentType.Equals(VodadModel.Utilities.Constants.ContentType.Video))
                    {
                        var video = videoRepository.GetSingle(w => w.Id == o.IdContent);
                        videoList.Add(video);
                    }
                }

                if (imagesList.Count > 0)
                    ViewBag.ImagesList = imagesList;

                if (videoList.Count > 0)
                    ViewBag.VideoList = videoList;

                ViewBag.Order = order;
                ViewBag.BannerMeasureId = ppid;

                return View(orderPerformedToPerformerModel);
            }

            else
            {
                success = "closedordersearch";
                return RedirectToAction("ManageCampaigns", "Campaign", new { success });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Advertiser")]
        public ActionResult SendOrderToPerformer(OrderPerformedToPerformerModel model)
        {
            var orderController = new OrderController();
            var OrderContentsRepository = new Repository<OrderContent>(Entities);

            var OrderContents = OrderContentsRepository.GetSingle(w => w.IdContent == model.ContentId);

            if (ModelState.IsValid && orderController.IsUsersOrder((int?)OrderContents.IdOrder, User.Identity.Name))
            {
                var performerPlatformRepository = new Repository<PerformerPlatform>(Entities);

                var performerPlatform = performerPlatformRepository.GetSingle(w => w.Id == model.PerformerPlatformId);

                var orderPerformed = new OrderPerformed();
                var user = UserHelper.GetUserByEmail(User.Identity.Name);

                orderPerformed.AuthorId = user.Id;
                orderPerformed.OrderContentId = OrderContents.Id;
                orderPerformed.PerformerPlatformId = model.PerformerPlatformId;
                orderPerformed.Status = VodadModel.Utilities.Constants.VerificationStatuses.Request;
                orderPerformed.StartDate = DateTime.Now;
                orderPerformed.MoneyPaid = 0;
                orderPerformed.LastStatusChangeDateTime = DateTime.Today;

                DateTime expireDate = DateTime.Today;

                if (model.ExpireDate == null || model.ExpireDate == DateTime.Today.ToString("MM/dd/yyyy", DateTimeFormatInfo.InvariantInfo))
                    expireDate = DateTime.Today;
                else
                    try
                    {
                        expireDate = DateTime.Parse(model.ExpireDate,
                                                                new CultureInfo("en-US", false));
                    }
                    catch (FormatException)
                    {
                        throw new FormatException("Cannot parse the date");
                    }

                var orderPerformedRepository = new Repository<OrderPerformed>(Entities);
                orderPerformedRepository.Add(orderPerformed);
                orderPerformedRepository.Save();

                OrderPerformed brandNewOrderPerformed =
                    orderPerformedRepository.GetAll(w => w.AuthorId == user.Id).ToList().Last();

                if (brandNewOrderPerformed.Id != null)
                {
                    WalletHelper walletHelper = new WalletHelper();

                    walletHelper.ChangeWalletState(brandNewOrderPerformed,
                                                       VodadModel.Utilities.Constants.OrdersStatuses.Block,
                                                       (int)user.Id);

                    Logger.Info(string.Format("Order performed {0} has been created by {1}", orderPerformed.Id,
                                          User.Identity.Name));

                    if (performerPlatform.User.LastEmailSendDateTime != DateTime.Today)
                    {
                        Repository<User> userRepository = new Repository<User>(Entities);

                        var Performer = userRepository.GetSingle(w => w.Id == performerPlatform.User.Id);

                        if (Performer != null)
                        {
                            string messageBody = Performer.Name +
                                                 ",\r\n You have recieved new order - http://vodad.com/ \r\n\r\n Thank you. \r\n Vodad team.";
                            const string messageSubject = "Vodad new event";
                            UserHelper.SendMessage(Performer.Email, Performer.Name, messageBody, messageSubject);

                            Performer.LastEmailSendDateTime = DateTime.Today;

                            userRepository.Save();

                            Logger.Info(
                                string.Format(
                                    "Message {0} was sent to {1} and his LastEmailSendDateTime has been changed to {2}",
                                    messageSubject, Performer.Email, Performer.LastEmailSendDateTime));
                        }
                    }

                }
            }

            return RedirectToAction("OrderPerformedStatistics", "OrderPerformed", new { success = "sentorder" });
        }
    }
}