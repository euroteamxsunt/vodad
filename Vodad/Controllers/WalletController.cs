using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using VodadModel;
using VodadModel.Helpers;
using VodadModel.Model;
using VodadModel.Repository;
using Vodad.Models;

namespace Vodad.Controllers
{
    public class WalletController : BaseController
    {
        public WalletController() { }

        public WalletController(IQueryableRepository repository)
            : base(repository)
        {

        }

        //[RequireHttps]
        [Authorize(Roles = "Administrator")]
        public ActionResult AcceptWalletChanges(int userId, int umid)
        {
            if (IsUsersMerchantAccount(userId, umid))
            {
                Repository<UserMerchants> userMerchantRepository = new Repository<UserMerchants>(Entities);

                var userMerchant = userMerchantRepository.GetSingle(w => w.Id == umid);

                if (userMerchant != null && userMerchant.Status.Equals(VodadModel.Utilities.Constants.UserMerchantAccountStatus.Changing))
                {
                    using (var transaction = new TransactionScope())
                    {
                        userMerchant.PreviousAccount = userMerchant.Account;
                        userMerchant.Status = VodadModel.Utilities.Constants.UserMerchantAccountStatus.Waiting;

                        userMerchantRepository.Save();

                        transaction.Complete();
                    }
                    Logger.Info("User's {0} merchant account {1} has been changed to {2}", userMerchant.User.Email, userMerchant.Id, userMerchant.Account);

                    return RedirectToAction("ManageWalletChanges", "Wallet", new { success = "accountaccepted" });
                }

                return RedirectToAction("ManageWalletChanges", "Wallet", new { success = "accountfailed" });
            }

            return RedirectToAction("ManageWalletChanges", "Wallet", new { success = "userfailed" });
        }

        [HttpPost]
        //[RequireHttps]
        [Authorize(Roles = "Advertiser")]
        public ActionResult ActivateCertificate(CertificateActivationModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserHelper.GetUserById(model.UserId);

                if (user != null)
                {
                    Repository<Certificates> certificateRepository = new Repository<Certificates>(Entities);

                    var certificate = certificateRepository.GetSingle(w => w.Code.Equals(model.Code) && w.Status.Equals(VodadModel.Utilities.Constants.CertificatesStatus.Waiting));

                    if (certificate != null)
                    {
                        using (var transaction = new TransactionScope())
                        {
                            Repository<Wallet> walletRepository = new Repository<Wallet>(Entities);
                            Repository<MoneyTransfers> moneyTransferRepository = new Repository<MoneyTransfers>(Entities);

                            var wallet = walletRepository.GetSingle(w => w.UserId == model.UserId);

                            wallet.Account += 100;

                            MoneyTransfers moneyTransfer = new MoneyTransfers();

                            moneyTransfer.AccountMerchantSystem = "certificate";
                            moneyTransfer.Action = VodadModel.Utilities.Constants.MoneyTransferAction.Income;
                            moneyTransfer.Amount = 100;
                            moneyTransfer.DateTime = DateTime.Now;
                            moneyTransfer.UserId = user.Id;

                            moneyTransferRepository.Add(moneyTransfer);
                            moneyTransferRepository.Save();

                            walletRepository.Save();

                            certificate.Status = VodadModel.Utilities.Constants.CertificatesStatus.Activated;
                            certificate.UserId = user.Id;

                            certificateRepository.Save();

                            Logger.Info("User {0} {1} has activated promo certificate {2} {3}", user.Id, user.Email, certificate.Id, certificate.Code);

                            transaction.Complete();

                            return RedirectToAction("WalletManager", "Wallet", new { success = "certificateactivated" });
                        }
                    }

                    return RedirectToAction("WalletManager", "Wallet", new { success = "incorrectcertificate" });
                }
            }
            return RedirectToAction("WalletManager", "Wallet", new { success = "chargefailed" });
        }

        public void ActivateNewUsersMerchantAccount(int userId)
        {
            Repository<UserMerchants> userMerchantRepository = new Repository<UserMerchants>(Entities);

            var userMerchant =
                userMerchantRepository.GetAll(
                    w =>
                    w.UserId == userId &&
                    w.Status.Equals(VodadModel.Utilities.Constants.UserMerchantAccountStatus.Waiting));

            if (userMerchant.Any())
            {
                foreach (var um in userMerchant)
                {
                    if ((DateTime.Now - (DateTime)um.LastChangeDateTime).TotalDays >= 5)
                    {
                        if (um.NextAccount != "")
                        {
                            um.Account = um.NextAccount;
                        }
                        um.NextAccount = "";
                        um.Status = VodadModel.Utilities.Constants.UserMerchantAccountStatus.Active;
                        um.LastChangeDateTime = DateTime.Now;
                    }

                    Logger.Info("UserMerchant {1} has been activated", um.Id);
                }

                userMerchantRepository.Save();


            }
        }

        //[RequireHttps]
        [Authorize(Roles = "Administrator, Performer, Advertiser")]
        public ActionResult AddMerchantAccount(string merchant, string success)
        {
            if (success != null)
            {
                if (success.Equals("accountfail"))
                {
                    ViewBag.AlertMessage = "You need to enter correct account";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
            }

            if (merchant == null)
                return RedirectToAction("Error404", "Error");

            Repository<Merchants> merchantRepository = new Repository<Merchants>(Entities);

            ViewBag.UserId = UserHelper.GetUserByEmail(User.Identity.Name).Id;
            ViewBag.MerchantId = merchantRepository.GetSingle(w => w.MerchantName.Equals(merchant)).Id;
            ViewBag.Merchant = merchant;

            if (ViewBag.MerchantId == null)
                return RedirectToAction("Error404", "Error");

            return View();
        }

        [HttpPost]
        //[RequireHttps]
        [Authorize(Roles = "Administrator, Performer, Advertiser")]
        public ActionResult AddMerchantAccount(AddMerchantAccountModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserHelper.GetUserById(model.UserId);

                Repository<UserMerchants> userMerchantRepository = new Repository<UserMerchants>(Entities);

                var userMerchant =
                    userMerchantRepository.GetSingle(w => w.UserId == user.Id && w.Merchants.Id == model.MerchantId);

                if (!IsUniqueMerchantAccount(model.Account))
                    return RedirectToAction("ChangeProfile", "Account", new { success = "walletexists" });

                if (userMerchant == null)
                {
                    userMerchant = new UserMerchants();

                    userMerchant.UserId = model.UserId;
                    userMerchant.MerchantId = model.MerchantId;
                    userMerchant.Account = model.Account;
                    userMerchant.Status = VodadModel.Utilities.Constants.UserMerchantAccountStatus.Active;
                    userMerchant.LastChangeDateTime = DateTime.Now;

                    userMerchantRepository.Add(userMerchant);
                    userMerchantRepository.Save();

                    Logger.Info("User {1} has added new {1} wallet {2}", user.Email, userMerchant.Merchants.MerchantName,
                                model.Account);

                    return RedirectToAction("ChangeProfile", "Account", new { success = "usermerchantadded" });
                }

                return RedirectToAction("ChangeProfile", "Account", new { success = "walletexists" });
            }

            return RedirectToAction("AddMerchantAccount", "Wallet", new { merchant = model.Merchant, success = "accountfail" });
        }

        protected bool AddMoneyToUsersWallet(int userId, decimal sum)
        {
            using (var transaction = new TransactionScope())
            {
                Repository<Wallet> walletRepository = new Repository<Wallet>(Entities);

                var wallet = walletRepository.GetSingle(w => w.UserId == userId);

                if (wallet != null)
                {
                    wallet.Account += sum;

                    walletRepository.Save();

                    transaction.Complete();

                    return true;
                }
            }

            return false;
        }

        //[RequireHttps]
        [Authorize(Roles = "Administrator, Performer, Advertiser")]
        public ActionResult CancelUserMerchantAccountChanges(int userId, int umid, string success)
        {
            if (IsUsersMerchantAccount(userId, umid))
            {
                Repository<UserMerchants> userMerchantRepository = new Repository<UserMerchants>(Entities);

                var userMerchant = userMerchantRepository.GetSingle(w => w.Id == umid);

                if (userMerchant != null)
                {
                    userMerchant.NextAccount = "";
                    userMerchant.Status = VodadModel.Utilities.Constants.UserMerchantAccountStatus.Waiting;

                    userMerchantRepository.Save();

                    Logger.Info("User {0} UserMerchantAccount {1} has been canceled", userId, umid);

                    if (User.IsInRole("Administrator"))
                        return RedirectToAction("ManageCheaters", "Cheaters");
                    else
                        return RedirectToAction("ChangeProfile", "Account");
                }
            }

            return RedirectToAction("Error404", "Error");
        }

        //[RequireHttps]
        [Authorize(Roles = "Performer, Advertiser")]
        public ActionResult CancelWithdraw(int mtid)
        {
            Repository<MoneyTransfers> moneyTransferRepository = new Repository<MoneyTransfers>(Entities);

            var moneyTransfer = moneyTransferRepository.GetSingle(w => w.Id == mtid);

            if (moneyTransfer != null)
            {
                var user = UserHelper.GetUserByEmail(User.Identity.Name);

                if (user.Id != null && moneyTransfer.UserId == user.Id)
                {
                    using (var transaction = new TransactionScope())
                    {
                        Repository<Wallet> walletRepository = new Repository<Wallet>(Entities);

                        var wallet = walletRepository.GetSingle(w => w.UserId == user.Id);

                        wallet.Account += moneyTransfer.Amount;

                        walletRepository.Save();
                        moneyTransferRepository.Delete(moneyTransfer);
                        moneyTransferRepository.Save();

                        Logger.Info("User {0} {1} has canceled withdraw {2} and {3} USD has been returned to his wallet {4}", user.Id, user.Email, moneyTransfer.Id, moneyTransfer.Amount, wallet.Id);

                        transaction.Complete();

                        return RedirectToAction("WalletManager", "Wallet", new { success = "withdrawalcanceled" });
                    }
                }
            }

            return RedirectToAction("Error404", "Error");
        }

        //[RequireHttps]
        [Authorize(Roles = "Administrator, Performer, Advertiser")]
        public ActionResult ChangeMerchantAccount(int? umid, string success)
        {
            if (success != null)
            {
                if (success.Equals("accountfail"))
                {
                    ViewBag.AlertMessage = "You need to enter correct account";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
            }

            if (umid == null)
                return RedirectToAction("Error404", "Error");

            Repository<UserMerchants> userMerchantRepository = new Repository<UserMerchants>(Entities);

            int userId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;

            ViewBag.UserId = userId;

            var userMerchant = userMerchantRepository.GetSingle(w => w.Id == umid);

            if (userMerchant == null || !IsUsersMerchantAccount(userId, (int)userMerchant.Id))
                return RedirectToAction("Error404", "Error");

            ViewBag.UserMerchantId = userMerchant.Id;
            ViewBag.Merchant = userMerchant.Merchants.MerchantName;
            ViewBag.MerchantId = userMerchant.Merchants.Id;

            return View();
        }

        [HttpPost]
        //[RequireHttps]
        [Authorize(Roles = "Administrator, Performer, Advertiser")]
        public ActionResult ChangeMerchantAccount(ChangeMerchantAccountModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserHelper.GetUserById(model.UserId);

                Repository<UserMerchants> userMerchantRepository = new Repository<UserMerchants>(Entities);

                var userMerchant =
                    userMerchantRepository.GetSingle(w => w.Id == model.UserMerchantId);

                if (!IsUniqueMerchantAccount(model.Account))
                    return RedirectToAction("ChangeProfile", "Account", new { success = "walletexists" });

                if (userMerchant != null && IsUsersMerchantAccount((int)user.Id, (int)userMerchant.Id) && ChangeMerchantAccountStatus((int)user.Id, model.UserMerchantId))
                {
                    userMerchant.NextAccount = model.Account;

                    userMerchantRepository.Save();

                    Logger.Info("User {0} userMerchant account {1} has been changed next account to {2}", user.Email, userMerchant.Id, model.Account);

                    return RedirectToAction("ChangeProfile", "Account", new { success = "usermerchantchanged" });
                }
            }

            return RedirectToAction("ChangeMerchantAccount", "Wallet",
                                        new { umid = model.UserMerchantId, success = "accountfail" });
        }

        protected bool ChangeMerchantAccountStatus(int userId, int umid)
        {
            if (IsUsersMerchantAccount(userId, umid))
            {
                Repository<UserMerchants> userMerchantRepository = new Repository<UserMerchants>(Entities);

                var userMerchant = userMerchantRepository.GetSingle(w => w.Id == umid);

                var user = UserHelper.GetUserById(userId);

                if (user != null)
                {
                    bool b = false;

                    var userWallet = user.Wallet.FirstOrDefault(w => w.UserId == userId);

                    if (((userWallet.Account + userWallet.Transfer) == 0) || !user.Roles.RoleName.Equals("Advertiser"))
                    {
                        userMerchant.Status = VodadModel.Utilities.Constants.UserMerchantAccountStatus.Waiting;
                        userMerchant.LastChangeDateTime = DateTime.Now;

                        b = true;
                    }
                    else
                    {
                        userMerchant.Status = VodadModel.Utilities.Constants.UserMerchantAccountStatus.Changing;
                        userMerchant.LastChangeDateTime = DateTime.Now;

                        Repository<MoneyTransfers> moneyTransferRepository = new Repository<MoneyTransfers>(Entities);

                        var transfersSinceLastAccountChange =
                            moneyTransferRepository.GetAll(
                                w =>
                                w.UserId == userId && w.DateTime > userMerchant.LastChangeDateTime &&
                                w.Action.Equals(VodadModel.Utilities.Constants.MoneyTransferAction.Income));

                        if (transfersSinceLastAccountChange.Any())
                        {
                            if (!user.OrderPerformed.Any(w => w.Status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Inaction)) && !user.OrderPerformed.Any(w => w.Status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Complete)))
                            {
                                CheatersController cheatersController = new CheatersController();
                                cheatersController.AddToCheaters(userId);
                            }
                        }

                        b = true;
                    }

                    if (b)
                    {
                        userMerchantRepository.Save();

                        Logger.Info("User {0} userMerchant account {1} has been changed to status {2} before changing account", user.Email, userMerchant.Id, userMerchant.Status);
                    }

                    return b;
                }
            }

            return false;
        }

        //[RequireHttps]
        [Authorize(Roles = "Advertiser")]
        public ActionResult ChargeMoneyForAdvertiserWithPaypal(decimal sum, int userId)
        {
            ViewBag.Sum = sum;
            ViewBag.UserId = userId;

            var user = UserHelper.GetUserById(userId);

            ViewBag.UserEmail = user.Email;

            Repository<UserMerchants> userMerchantsRepository = new Repository<UserMerchants>(Entities);

            var userMerchant =
                userMerchantsRepository.GetSingle(
                    w =>
                    w.UserId == user.Id && w.Merchants.MerchantName.Equals("PayPal") &&
                    w.Status.Equals(VodadModel.Utilities.Constants.UserMerchantAccountStatus.Active));

            if (userMerchant != null)
                ViewBag.UserMerchantAccount = userMerchant.Account;

            if (ViewBag.UserMerchantAccount != null)
                return View();
            else
                return RedirectToAction("WalletManager", "Wallet", new { success = "hasnopaypalaccount" });
        }

        public Merchants GetMerchantById(int id)
        {
            Repository<Merchants> merchantRepository = new Repository<Merchants>(Entities);

            return merchantRepository.GetSingle(w => w.Id == id);
        }


        public List<Merchants> GetMerchantsList()
        {
            Repository<Merchants> merchantRepository = new Repository<Merchants>(Entities);

            return new List<Merchants>(merchantRepository.GetAll());
        }

        public IQueryable<MoneyTransfers> GetUserMoneyTransactions(int userId)
        {
            if (userId != null)
            {
                Repository<MoneyTransfers> moneyTransferRepository = new Repository<MoneyTransfers>(Entities);

                return moneyTransferRepository.GetAll(w => w.UserId == userId);
            }

            return null;
        }

        public Wallet GetUsersWallet(int? userId)
        {
            var walletRepository = new Repository<Wallet>(Entities);

            var wallet = walletRepository.GetSingle(w => w.UserId == userId);

            if (wallet == null)
            {
                WalletHelper walletHelper = new WalletHelper();

                walletHelper.CreateNewWallet(userId);
                wallet = walletRepository.GetSingle(w => w.UserId == userId);
            }

            return wallet;
        }

        public bool IsUniqueMerchantAccount(string account)
        {
            if (!account.Equals(""))
            {
                Repository<UserMerchants> userMerchantRepository = new Repository<UserMerchants>(Entities);

                var userMerchant = userMerchantRepository.GetSingle(w => w.Account.Equals(account));

                if (userMerchant == null)
                    return true;
            }

            return false;
        }

        public bool IsUsersMerchantAccount(int userId, int userMerchantId)
        {
            Repository<UserMerchants> userMerchantRepository = new Repository<UserMerchants>(Entities);

            var userMerchant = userMerchantRepository.GetSingle(w => w.Id == userMerchantId);

            if (userMerchant != null)
            {
                if (userMerchant.UserId == userId)
                    return true;
            }

            return false;
        }

        //[RequireHttps]
        [Authorize(Roles = "Administrator, Helper")]
        public ActionResult ManageWalletChanges(string success)
        {
            if (success != null)
            {
                if (success.Equals("addedtocheaters"))
                {
                    ViewBag.AlertMessage = "User has been added to cheaters";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                }
                else if (success.Equals("userfailed"))
                {
                    ViewBag.AlertMessage = "Not users userMerchantAccount";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
                else if (success.Equals("accountfailed"))
                {
                    ViewBag.AlertMessage = "Not found userMerchantAccount";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
                else if (success.Equals("accountaccepted"))
                {
                    ViewBag.AlertMessage = "Account accepted";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                }
            }

            Repository<UserMerchants> userMerchantRepository = new Repository<UserMerchants>(Entities);

            var changingUserMerchants =
                userMerchantRepository.GetAll(
                    w => w.Status.Equals(VodadModel.Utilities.Constants.UserMerchantAccountStatus.Changing));

            if (changingUserMerchants != null)
            {
                Repository<OrderPerformed> orderPerformedRepository = new Repository<OrderPerformed>(Entities);
                List<ManageWalletChangesModel> walletChangesList = new List<ManageWalletChangesModel>();

                foreach (var changingUserMerchant in changingUserMerchants)
                {
                    ManageWalletChangesModel walletChangesElement = new ManageWalletChangesModel();

                    var user = UserHelper.GetUserById((int)changingUserMerchant.UserId);

                    walletChangesElement.UserId = (int)user.Id;
                    walletChangesElement.UserMerchantId = (int)changingUserMerchant.Id;
                    walletChangesElement.CurrentAccount = changingUserMerchant.Account;
                    walletChangesElement.PreviousAccount = changingUserMerchant.PreviousAccount;
                    walletChangesElement.NextAccount = changingUserMerchant.NextAccount;
                    walletChangesElement.TotalWalletSum =
                        (decimal)changingUserMerchant.User.Wallet.FirstOrDefault(w => w.UserId == changingUserMerchant.UserId).
                            Account +
                        (decimal)changingUserMerchant.User.Wallet.FirstOrDefault(w => w.UserId == changingUserMerchant.UserId).
                            Transfer;
                    walletChangesElement.OrderPerformedInactionAndCompleteSumAfterLastAccountChanges = 0;

                    // Подсчет суммы, потраченной на заказы после последней смены кошелька

                    List<OrderPerformed> orderPerformeds = orderPerformedRepository.GetAll(w => w.OrderContent.Order.User.Id == user.Id && w.Status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Inaction)).ToList();
                    orderPerformeds.Concat(
                        orderPerformedRepository.GetAll(
                            w =>
                            w.OrderContent.Order.User.Id == user.Id &&
                            w.Status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Complete)).ToList());

                    foreach (var orderPerformed in orderPerformeds)
                    {
                        walletChangesElement.OrderPerformedInactionAndCompleteSumAfterLastAccountChanges +=
                            (decimal)orderPerformed.MoneyPaid;
                    }

                    var lastTransfer = user.MoneyTransfers.LastOrDefault(w => w.UserId == user.Id);

                    if (lastTransfer != null)
                    {
                        walletChangesElement.LastCurrentAccountChargeSum = (decimal)lastTransfer.Amount;

                        walletChangesElement.LastCurrentAccountChargeDateTime = lastTransfer.DateTime.ToString();
                    }
                    else
                    {
                        walletChangesElement.LastCurrentAccountChargeSum = 0;
                        walletChangesElement.LastCurrentAccountChargeDateTime = "never";
                    }

                    if ((DateTime.Today - changingUserMerchant.LastChangeDateTime.Value).TotalDays > 4)
                        walletChangesElement.CanBeChanged = true;
                    else
                        walletChangesElement.CanBeChanged = false;

                    walletChangesElement.CurrentAccountChangeDateTime = changingUserMerchant.LastChangeDateTime.ToString();

                    walletChangesList.Add(walletChangesElement);
                }

                return View(walletChangesList);
            }

            return View();
        }

        //[RequireHttps]
        [Authorize(Roles = "Performer, Advertiser, Administrator")]
        public ActionResult WalletManager(string success)
        {
            if (success != null)
            {
                if (success.Equals("chargefailed"))
                {
                    ViewBag.AlertMessage = "Something went wrong. Please, try again later.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
                else if (success.Equals("confirmationfailed"))
                {
                    ViewBag.AlertMessage = "Confirmation failed. Please, try again.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
                else if (success.Equals("hasnopaypalaccount"))
                {
                    ViewBag.AlertMessage = "You have no PayPal account or account is not active yet, please add it or wait for activation for further work.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
                else if (success.Equals("notenoughmoney"))
                {
                    ViewBag.AlertMessage = "You don't have enough money for executing this operation.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
                else if (success.Equals("threedayspenalty"))
                {
                    ViewBag.AlertMessage = "You can withdraw money only once every three days.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
                else if (success.Equals("withdrawalcomplete"))
                {
                    ViewBag.AlertMessage = "Your order has been formed and will be processed after three days.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                }
                else if (success.Equals("certificateactivated"))
                {
                    ViewBag.AlertMessage = "Certificate has been succesfully activated.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                }
                else if (success.Equals("incorrectcertificate"))
                {
                    ViewBag.AlertMessage = "You have entered incorrect code.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
                else if (success.Equals("withdrawalcanceled"))
                {
                    ViewBag.AlertMessage = "Withdrawal has been canceled.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                }
            }

            Repository<Wallet> walletRepository = new Repository<Wallet>(Entities);

            var wallet = walletRepository.GetSingle(w => w.User.Email.Equals(User.Identity.Name));

            // Проверка и активирование изменений кошелька
            WalletController walletController = new WalletController();
            walletController.ActivateNewUsersMerchantAccount((int)wallet.User.Id);

            ViewBag.Cash = wallet.Account;
            ViewBag.Transfers = wallet.Transfer;
            ViewBag.ReferralsIncome = wallet.ReferralsIncome;

            ViewBag.Merchants = GetMerchantsList();
            ViewBag.UserId = wallet.User.Id;

            int userId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;

            var moneyTransfersList = GetUserMoneyTransactions(userId);

            if (moneyTransfersList.Any())
            {
                List<MoneyTransferTableModel> moneyTransferTableList = new List<MoneyTransferTableModel>();

                foreach (var m in moneyTransfersList)
                {
                    MoneyTransferTableModel moneyTransferListElement = new MoneyTransferTableModel();

                    moneyTransferListElement.MoneyTransferId = (int)m.Id;
                    moneyTransferListElement.AccountMerchantSystem = m.AccountMerchantSystem;
                    moneyTransferListElement.Action = m.Action.ToUpperInvariant();
                    moneyTransferListElement.Amount = m.Amount.Value;
                    moneyTransferListElement.DateTime = m.DateTime.Value.ToString("MM/dd/yyyy hh:mm:ss", DateTimeFormatInfo.InvariantInfo);
                    moneyTransferListElement.MerchantName = m.Merchants.MerchantName;
                    moneyTransferListElement.Status = m.Status;

                    moneyTransferTableList.Add(moneyTransferListElement);
                }

                ViewBag.MoneyTransferTableList = moneyTransferTableList;
            }

            return View();
        }

        [HttpPost]
        //[RequireHttps]
        [Authorize(Roles = "Performer, Advertiser, Administrator")]
        public ActionResult WalletManager(WalletManagerModel model)
        {
            const string SUCCESS = VodadModel.Utilities.Constants.AlertMessages.Failed;
            if (ModelState.IsValid)
            {
                Repository<Merchants> merchantRepository = new Repository<Merchants>(Entities);

                var merchant = merchantRepository.GetSingle(w => w.MerchantName.Equals(model.Merchant));

                int userId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;

                if (merchant != null)
                {
                    if (model.Action.Equals("Charge")) //Charge
                    {
                        if (merchant.MerchantName.Equals("PayPal"))
                        {
                            return RedirectToAction("ChargeMoneyForAdvertiserWithPaypal", "Wallet", new { sum = model.ChargeSum, userId });
                        }
                    }
                    else if (model.Action.Equals("Withdraw")) //Withdraw
                    {
                        if (merchant.MerchantName.Equals("PayPal"))
                        {
                            return RedirectToAction("WithdrawMoneyWithPayPal", "Wallet", new { sum = model.ChargeSum, userId });
                        }
                    }
                }
            }

            return RedirectToAction("WalletManager", "Wallet", new { success = SUCCESS });
        }

        //[RequireHttps]
        [Authorize(Roles = "Advertiser, Performer")]
        public ActionResult WithdrawMoneyWithPayPal(decimal sum, int userId)
        {
            var user = UserHelper.GetUserById(userId);

            var userMerchant =
                user.UserMerchants.FirstOrDefault(
                    w =>
                    w.UserId == user.Id && w.Merchants.MerchantName.Equals("PayPal") &&
                    w.Status.Equals(VodadModel.Utilities.Constants.UserMerchantAccountStatus.Active));

            if (userMerchant != null)
            {
                if (user.Wallet.FirstOrDefault(w => w.UserId == user.Id).Account >= sum)
                {
                    Repository<MoneyTransfers> moneyTransferRepository = new Repository<MoneyTransfers>(Entities);

                    var moneyTransfers = moneyTransferRepository.GetAll(w => w.UserId == user.Id && w.Action.Equals(VodadModel.Utilities.Constants.MoneyTransferAction.Outcome));

                    if ((moneyTransfers.Any() && (DateTime.Today - moneyTransfers.ToList().Last().DateTime.Value).TotalDays >= 3) || !moneyTransfers.Any())
                    {
                        using (var transaction = new TransactionScope())
                        {
                            MoneyTransfers moneyTransfer = new MoneyTransfers();

                            moneyTransfer.UserId = user.Id;
                            moneyTransfer.DateTime = DateTime.Today;
                            moneyTransfer.Action = VodadModel.Utilities.Constants.MoneyTransferAction.Outcome;
                            moneyTransfer.Amount = sum;
                            moneyTransfer.MerchantId = userMerchant.Merchants.Id;
                            moneyTransfer.AccountMerchantSystem = userMerchant.Account;
                            moneyTransfer.Status = VodadModel.Utilities.Constants.WithdrawalStatus.Waiting;

                            moneyTransferRepository.Add(moneyTransfer);
                            moneyTransferRepository.Save();

                            Repository<Wallet> walletRepository = new Repository<Wallet>(Entities);

                            var wallet = walletRepository.GetSingle(w => w.UserId == user.Id);

                            wallet.Account -= sum;

                            walletRepository.Save();


                            Logger.Info("User {0} {1} has withdrawed {3} with {4} account {5}", user.Id, user.Email, sum, userMerchant.Merchants.MerchantName, userMerchant.Account);

                            transaction.Complete();

                            return RedirectToAction("WalletManager", "Wallet", new { success = "withdrawalcomplete" });
                        }
                    }
                    else
                        return RedirectToAction("WalletManager", "Wallet", new { success = "threedayspenalty" });
                }
                else
                    return RedirectToAction("WalletManager", "Wallet", new { success = "notenoughmoney" });
            }
            else
                return RedirectToAction("WalletManager", "Wallet", new { success = "hasnopaypalaccount" });
        }
    }
}