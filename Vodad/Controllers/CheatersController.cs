using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VodadModel;
using VodadModel.Helpers;
using VodadModel.Repository;
using Vodad.Models;

namespace Vodad.Controllers
{
    public class CheatersController : BaseController
    {
        [Authorize(Roles = "Helper, Administrator")]
        public ActionResult AddToCheaters(int userId)
        {
            var user = UserHelper.GetUserById(userId);

            if (user != null)
            {
                Repository<Cheaters> cheatersRepository = new Repository<Cheaters>(Entities);

                Cheaters cheater = new Cheaters();

                if (cheatersRepository.GetSingle(w => w.UserId == userId) == null)
                {
                    cheater.UserId = userId;

                    cheatersRepository.Add(cheater);
                    cheatersRepository.Save();

                    Logger.Warn("User {0} has been added to cheaters", user.Email);
                }

                return RedirectToAction("ManageWalletChanges", "Wallet", new { success = "addedtocheaters" });
            }

            return RedirectToAction("Error404", "Error");
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult ManageCheaters(string success)
        {
            if (success != null)
            {
                if (success.Equals("userfailed"))
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

            Repository<Cheaters> cheaterRepository = new Repository<Cheaters>(Entities);

            var cheaters = cheaterRepository.GetAll();

            if (cheaters.Any())
            {
                Repository<OrderPerformed> orderPerformedRepository = new Repository<OrderPerformed>(Entities);
                List<ManageWalletChangesModel> walletChangesList = new List<ManageWalletChangesModel>();

                foreach (var changingUserMerchant in cheaters)
                {
                    ManageWalletChangesModel walletChangesElement = new ManageWalletChangesModel();

                    var user = UserHelper.GetUserById((int)changingUserMerchant.UserId);

                    walletChangesElement.UserId = (int)user.Id;
                    walletChangesElement.UserMerchantId = (int)changingUserMerchant.Id;

                    // Only for single PayPal account
                    walletChangesElement.CurrentAccount = changingUserMerchant.User.UserMerchants.FirstOrDefault().Account;
                    walletChangesElement.PreviousAccount = changingUserMerchant.User.UserMerchants.FirstOrDefault().PreviousAccount;
                    walletChangesElement.NextAccount = changingUserMerchant.User.UserMerchants.FirstOrDefault().NextAccount;
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

                    walletChangesElement.LastCurrentAccountChargeSum = (decimal)user.MoneyTransfers.LastOrDefault(w => w.UserId == user.Id).Amount;

                    walletChangesElement.LastCurrentAccountChargeDateTime =
                        user.MoneyTransfers.LastOrDefault(w => w.UserId == user.Id).DateTime.ToString();

                    walletChangesElement.CurrentAccountChangeDateTime = changingUserMerchant.User.UserMerchants.FirstOrDefault().LastChangeDateTime.ToString();

                    walletChangesList.Add(walletChangesElement);
                }

                return View(walletChangesList);
            }

            return View();
        }
    }
}
