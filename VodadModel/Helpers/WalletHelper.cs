using System.Transactions;
using VodadModel.Repository;
namespace VodadModel.Helpers
{
    public class WalletHelper
    {
        VodadEntities Entities = new VodadEntities();
        public static NLog.Logger Logger = NLogWrapper.Logger;

        protected bool BlockMoneyBeforeTransferingBetweenUsers(int advertiserId, int performerId, decimal sum)
        {
            using (var transaction = new TransactionScope())
            {
                var walletRepository = new Repository<Wallet>(Entities);

                var advertiserWallet = walletRepository.GetSingle(w => w.UserId == advertiserId);
                var performerWallet = walletRepository.GetSingle(w => w.UserId == performerId);
                var adminWallet = walletRepository.GetSingle(w => w.User.Roles.RoleName.Equals(Utilities.Constants.UserRoles.Administrator) && w.User.Email.Equals("klamar.meriason@gmail.com"));

                if (adminWallet == null)
                {
                    return false;
                }

                if (advertiserWallet.Account != null && advertiserWallet.Account.Value >= sum)
                {
                    decimal sumForPerformerReferrer = 0;
                    decimal sumForAdvertiserReferrer = 0;

                    advertiserWallet.Account -= sum;
                    advertiserWallet.Transfer += sum;

                    decimal sumForPerformer = sum * (decimal)0.75;

                    performerWallet.Transfer += sumForPerformer;

                    if (performerWallet.User.ReferrerId != null)
                    {
                        sumForPerformerReferrer = sum * (decimal)0.01;
                    }

                    if (advertiserWallet.User.ReferrerId != null)
                    {
                        sumForAdvertiserReferrer = sum * (decimal)0.02;
                    }

                    decimal sumForAdmin = sum - sumForPerformer - sumForAdvertiserReferrer - sumForPerformerReferrer;
                    adminWallet.Transfer += sumForAdmin;
                    walletRepository.Save();

                    transaction.Complete();

                    return true;
                }
                transaction.Complete();

                return false;
            }
        }

        public bool ChangeWalletState(OrderPerformed op, string status, int userId)
        {
            var orderPerformedRepository = new Repository<OrderPerformed>(Entities);

            var orderPerformed = orderPerformedRepository.GetSingle(w => w.Id == op.Id);

            if (orderPerformed != null)
            {
                var performerId = (int)orderPerformed.PerformerPlatform.User.Id;
                var advertiserId = (int)orderPerformed.OrderContent.Order.User.Id;

                Repository<User> userRepository = new Repository<User>(Entities);

                var admin = userRepository.GetSingle(w => w.Roles.RoleName.Equals(Utilities.Constants.UserRoles.Administrator) && w.Email.Equals("klamar.meriason@gmail.com"));

                if (admin == null)
                    return false;

                //var orderPerformedController = new OrderPerformedController();

                if (UserHelper.IsUsersOrderPerformed((int)orderPerformed.Id, userId))
                {
                    // Разблокировка денег
                    if (status.Equals(Utilities.Constants.OrdersStatuses.Unblock))
                    {
                        var performer = userRepository.GetSingle(w => w.Id == performerId);
                        var advertiser = userRepository.GetSingle(w => w.Id == advertiserId);

                        decimal advertiserUnblock = (decimal)orderPerformed.MoneyPaid;
                        decimal performerUnblock = advertiserUnblock * (decimal)0.75;

                        decimal sumForPerformerReferrer = 0;
                        decimal sumForAdvertiserReferrer = 0;

                        if (performer.ReferrerId != null)
                        {
                            sumForPerformerReferrer = advertiserUnblock * (decimal)0.01;
                        }

                        if (advertiser.ReferrerId != null)
                        {
                            sumForAdvertiserReferrer = advertiserUnblock * (decimal)0.02;
                        }

                        decimal adminUnblock = advertiserUnblock - performerUnblock - sumForPerformerReferrer -
                                               sumForAdvertiserReferrer;

                        if (UnblockBlockedUsersMoney(advertiserId, advertiserUnblock) && UnblockBlockedUsersMoney(performerId, performerUnblock) && UnblockBlockedUsersMoney((int)admin.Id, adminUnblock))
                            return true;
                        else
                            return false;
                    }
                    else
                        // Блокирование денег
                        if (status.Equals(Utilities.Constants.OrdersStatuses.Block))
                        {
                            if (BlockMoneyBeforeTransferingBetweenUsers(advertiserId, performerId, (decimal)orderPerformed.MoneyPaid))
                                return true;
                            else
                                return false;
                        }
                        // Расчет по заказу
                        else if (status.Equals(Utilities.Constants.OrdersStatuses.Pay))
                        {
                            using (var transaction = new TransactionScope())
                            {
                                if (TransferBlockedMoneyBetweenUsers(advertiserId, performerId, (decimal)op.MoneyPaid))
                                {
                                    orderPerformed.MoneyPaid = op.MoneyPaid;

                                    orderPerformedRepository.Save();

                                    transaction.Complete();

                                    return true;
                                }
                                else
                                {
                                    transaction.Dispose();
                                    return false;
                                }
                            }
                        }
                        else
                            return false;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public void CreateNewWallet(int? userId)
        {
            var walletRepository = new Repository<Wallet>(Entities);

            var testForWallet = walletRepository.GetSingle(w => w.UserId == userId);

            if (testForWallet == null)
            {
                var wallet = new Wallet { UserId = userId, Account = 0, Transfer = 0, ReferralsIncome = 0 };

                walletRepository.Add(wallet);
                walletRepository.Save();

                var brandNewWallet = (int)walletRepository.GetSingle(w => w.UserId == userId).Id;

                Logger.Info(string.Format("Wallet id = {0} has been added", brandNewWallet));
            }
        }

        public Wallet GetUsersWallet(int? userId)
        {
            var walletRepository = new Repository<Wallet>(Entities);

            var wallet = walletRepository.GetSingle(w => w.UserId == userId);

            if (wallet == null)
            {
                CreateNewWallet(userId);
                wallet = walletRepository.GetSingle(w => w.UserId == userId);
            }

            return wallet;
        }

        protected bool TransferBlockedMoneyBetweenUsers(int? advertiserId, int? performerId, decimal sum)
        {
            using (var transaction = new TransactionScope())
            {
                var walletRepository = new Repository<Wallet>(Entities);

                var advertiserWallet = walletRepository.GetSingle(w => w.UserId == advertiserId);
                var performerWallet = walletRepository.GetSingle(w => w.UserId == performerId);
                var adminWallet = walletRepository.GetSingle(w => w.User.Roles.RoleName.Equals(Utilities.Constants.UserRoles.Administrator) && w.User.Email.Equals("klamar.meriason@gmail.com"));

                if (advertiserWallet.Transfer.Value >= sum && performerWallet.Transfer.Value >= sum * (decimal)0.75)
                {
                    decimal sumForPerformerReferrer = 0;
                    decimal sumForAdvertiserReferrer = 0;

                    advertiserWallet.Transfer -= sum;

                    decimal sumForPerformer = sum * (decimal)0.75;

                    if (performerWallet.User.ReferrerId != null)
                    {
                        Wallet performerReferrerWallet = walletRepository.GetSingle(w => w.UserId == performerWallet.User.ReferrerId);
                        sumForPerformerReferrer = sum * (decimal)0.01;
                        performerReferrerWallet.Account += sumForPerformerReferrer;
                        performerReferrerWallet.ReferralsIncome += sumForPerformerReferrer;
                    }

                    if (advertiserWallet.User.ReferrerId != null)
                    {
                        Wallet advertiserReferrerWallet = walletRepository.GetSingle(w => w.UserId == advertiserWallet.User.ReferrerId);
                        sumForAdvertiserReferrer = sum * (decimal)0.02;
                        advertiserReferrerWallet.Account += sumForAdvertiserReferrer;
                        advertiserReferrerWallet.ReferralsIncome += sumForAdvertiserReferrer;
                    }

                    performerWallet.Transfer -= sumForPerformer;
                    performerWallet.Account += sumForPerformer;

                    decimal sumForAdmin = sum - sumForPerformer - sumForAdvertiserReferrer - sumForPerformerReferrer;
                    adminWallet.Transfer -= sumForAdmin;
                    adminWallet.Account += sumForAdmin;
                    walletRepository.Save();

                    Logger.Info(string.Format("{0} money has been transfered between Advertiser {1} and Performer {2}, advertiser spent {3}, admin got {4}, performer got {5}, performer's referal got {6}, advertiser's referal got {7}", sum, advertiserId, performerId, sum, sumForAdmin, sumForPerformer, sumForPerformerReferrer, sumForAdvertiserReferrer));

                    transaction.Complete();

                    return true;
                }
                else
                {
                    transaction.Complete();

                    return false;
                }
            }
        }

        protected bool UnblockBlockedUsersMoney(int? userId, decimal sum)
        {
            var walletRepository = new Repository<Wallet>(Entities);

            var userWallet = walletRepository.GetSingle(w => w.UserId == userId);

            if (userWallet.Transfer.Value >= sum)
            {
                Repository<User> userRepository = new Repository<User>(Entities);

                var user = userRepository.GetSingle(w => w.Id == userId);

                userWallet.Transfer -= sum;

                if (user.Roles.RoleName.Equals(Utilities.Constants.UserRoles.Administrator))
                {
                    userWallet.Account += sum;
                }


                walletRepository.Save();

                Logger.Info(string.Format("{0} money has been unblocked for user with id {1}, email {2}", sum, userId, user.Email));

                return true;
            }
            else
                return false;
        }
    }
}
