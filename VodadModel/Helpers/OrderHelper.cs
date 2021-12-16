using System;
using System.Linq;
using VodadModel.Repository;

namespace VodadModel.Helpers
{
    public static class OrderHelper
    {
        static VodadEntities Entities = new VodadEntities();
        public static NLog.Logger Logger = NLogWrapper.Logger;

        /*public static void CheckForRequiredAmountForTheOrders(int userId)
        {
            var orderRepository = new Repository<Order>(Entities);
            var walletRepository = new Repository<Wallet>(Entities);

            var orders = orderRepository.GetAll(w => w.User.Id == userId);
            var wallet = walletRepository.GetSingle(w => w.User.Id == userId);
            var userEmail = UserHelper.GetUserById(userId).Email;

            foreach (var o in orders)
            {
                if (wallet.Account.Value < o.MaxBudget)
                    ChangeOrderStatus(userEmail, (int)o.Campaign.Id, (int)o.Id, SNGModel.Utilities.Constants.OrdersStatuses.Closed);
            }
        }*/

        public static bool ChangeOrderPerformedStatus(int opid, string status, int userId)
        {
            var orderPerformedRepository = new Repository<OrderPerformed>(Entities);

            var orderPerformed = orderPerformedRepository.GetSingle(w => w.Id == opid);

            // Если Advertiser подтверждает OP, необходимо заблокировать деньги
            if (IsUsersOrderPerformed(opid, userId) && orderPerformed.AuthorId != orderPerformed.OrderContent.Order.User.Id && status.Equals(Utilities.Constants.VerificationStatuses.Inaction))
            {
                var walletHelper = new WalletHelper();

                walletHelper.ChangeWalletState(orderPerformed, Utilities.Constants.OrdersStatuses.Block, userId);
            }
            else
                // Если Performer отклоняет заказ, необходимо разблокировать деньги
                if (IsUsersOrderPerformed(opid, userId) && orderPerformed.AuthorId == orderPerformed.OrderContent.Order.User.Id && orderPerformed.PerformerPlatform.User.Id == userId && status.Equals(Utilities.Constants.OrdersStatuses.Deny))
                {
                    var walletHelper = new WalletHelper();

                    walletHelper.ChangeWalletState(orderPerformed, Utilities.Constants.OrdersStatuses.Unblock, userId);
                }
                else if (IsUsersOrderPerformed(opid, userId) && orderPerformed.AuthorId == orderPerformed.OrderContent.Order.User.Id && status.Equals(Utilities.Constants.OrdersStatuses.Deny))
                {
                    var walletHelper = new WalletHelper();

                    walletHelper.ChangeWalletState(orderPerformed, Utilities.Constants.OrdersStatuses.Unblock, userId);
                }

            if (IsUsersOrderPerformed(opid, userId)/* && !orderPerformed.AuthorId.Equals(userId)*/ && (status.Equals(Utilities.Constants.VerificationStatuses.Inaction) || status.Equals(Utilities.Constants.OrdersStatuses.Deny)))
            {
                orderPerformed.Status = status;
                orderPerformed.LastStatusChangeDateTime = DateTime.Today;
                orderPerformedRepository.Save();

                Logger.Info(string.Format("Order performed {0} status has been changed to {1} by {2}", orderPerformed.Id, status, userId));
            }

            // Сделать после walletcontroller'а расчет
            // Если заказ выполнен, необходимо разблокировать деньги и расплатиться
            if (IsUsersOrderPerformed(opid, userId) && /*!orderPerformed.AuthorId.Equals(userId) && */(status.Equals(Utilities.Constants.VerificationStatuses.Complete)))
            {
                orderPerformed.Status = status;
                orderPerformed.LastStatusChangeDateTime = DateTime.Today;
                orderPerformedRepository.Save();

                Repository<PerformerStatistics> performerStatisticsRepository = new Repository<PerformerStatistics>(Entities);

                var performerStatistics =
                    performerStatisticsRepository.GetSingle(
                        w => w.PerformerPlatformId == orderPerformed.PerformerPlatform.Id);

                performerStatistics.CompletedOrders++;
                performerStatistics.TotalOrders++;

                performerStatisticsRepository.Save();

                ChangeUsersKarma(opid, userId);

                var walletHelper = new WalletHelper();

                if (walletHelper.ChangeWalletState(orderPerformed, Utilities.Constants.OrdersStatuses.Pay, userId))
                {
                    Logger.Info(string.Format("Order performed {0} status has been changed to {1} by {2}", orderPerformed.Id, status, userId));

                    return true;
                }
                else
                {
                    Logger.Error("Something went wrong with money transaction for orderPerformed {0}", orderPerformed.Id);
                }
            }

            return false;
        }

        public static void ChangeUsersKarma(int opid, int userId)
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
                        w.Verified.Equals(Utilities.Constants.CredentialCheckStatuses.Verified));

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

        public static void ChangeOrderStatus(string userEmail, int? oid, string status)
        {

            if (IsUsersOrder(oid, userEmail) && GetOrderStatusById(oid).Equals(Utilities.Constants.OrdersStatuses.Open))
            {
                var orderRepository = new Repository<Order>(Entities);

                var order = orderRepository.GetSingle(w => w.Id == oid);

                if (order != null)
                {
                    if (status.Equals(Utilities.Constants.OrdersStatuses.Open))
                        order.Status = Utilities.Constants.OrdersStatuses.Open;
                    else if (status.Equals(Utilities.Constants.OrdersStatuses.Closed))
                        order.Status = Utilities.Constants.OrdersStatuses.Closed;
                    else if (status.Equals(Utilities.Constants.OrdersStatuses.Deleted))
                    {
                        var orderContentRepository = new Repository<OrderContent>(Entities);

                        var orderContent = orderContentRepository.GetAll(w => w.IdOrder == oid);

                        if (orderContent != null)
                        {
                            var orderPerformedRepository = new Repository<OrderPerformed>(Entities);

                            foreach (var i in orderContent)
                            {
                                if (orderPerformedRepository.GetAll(w => w.OrderContentId == (int)i.Id) != null)
                                    order.Status = Utilities.Constants.OrdersStatuses.Deleted;
                                else
                                    DeleteOrder(oid, userEmail);
                            }
                        }
                    }
                }

                orderRepository.Save();

            }
        }

        private static void DeleteOrder(int? oid, string userEmail)
        {
            if (OrderHelper.IsUsersOrder(oid, userEmail))
            {
                var orderRepository = new Repository<Order>(Entities);
                var orderContentRepository = new Repository<OrderContent>(Entities);
                var imageRepository = new Repository<Image>(Entities);
                var videoRepository = new Repository<Video>(Entities);

                var order = orderRepository.GetSingle(w => w.Id == oid);
                var orderContentList = orderContentRepository.GetAll(w => w.IdOrder == order.Id);

                foreach (var o in orderContentList)
                {
                    if (o.ContentType.FirstOrDefault().Equals(Utilities.Constants.ContentType.Image))
                    {
                        var image = imageRepository.GetSingle(w => w.Id == o.IdContent);

                        imageRepository.Delete(image);
                        Logger.Info(string.Format("Image id = {0}  has been deleted", image.Id));
                    }
                    else if (o.ContentType.FirstOrDefault().Equals(Utilities.Constants.ContentType.Video))
                    {
                        var video = videoRepository.GetSingle(w => w.Id == o.IdContent);

                        videoRepository.Delete(video);
                        Logger.Info(string.Format("Video id = {0}  has been deleted", video.Id));
                    }

                    orderContentRepository.Delete(o);
                    Logger.Info(string.Format("OrderContent id = {0}  has been deleted", o.Id));
                }

                orderRepository.Delete(order);
                //Logger.Info(string.Format("Order id = {0} has been deleted", order.Id));
                orderRepository.Save();
                orderContentRepository.Save();
                imageRepository.Save();
                videoRepository.Save();
            }
        }

        public static bool IsUsersOrderPerformed(int? opid, int userId)
        {
            var orderPerformedRepository = new Repository<OrderPerformed>(Entities);

            var orderPerformed = orderPerformedRepository.GetSingle(w => w.Id == opid);

            if (orderPerformed.PerformerPlatform.User.Id == userId || orderPerformed.OrderContent.Order.User.Id == userId)
                return true;
            else
                return false;
        }

        public static bool IsUsersOrder(int? o, string userEmail)
        {
            var orderRepository = new Repository<Order>(Entities);

            var order = orderRepository.GetSingle(w => w.Id == o);

            if (order.User.Email.Equals(userEmail))
                return true;
            else
                return false;
        }

        public static string GetOrderStatusById(int? o)
        {
            var orderRepository = new Repository<Order>(Entities);

            try
            {
                return orderRepository.GetSingle(w => w.Id == o).Status;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
