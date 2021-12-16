using System.Linq;
using System.Web.Mvc;
using VodadModel;
using VodadModel.Repository;

namespace Vodad.Controllers
{
    public class AdministratorController : BaseController
    {
        public ActionResult Index()
        {
            Repository<Wallet> walletRepository = new Repository<Wallet>(Entities);
            Repository<MoneyTransfers> moneyTransferRepository = new Repository<MoneyTransfers>(Entities);

            ViewBag.TotalCash = walletRepository.GetAll().Sum(w => w.Account);
            ViewBag.TotalTransfer = walletRepository.GetAll().Sum(w => w.Transfer);
            ViewBag.TotalIncome = moneyTransferRepository.GetAll(w => w.Action.Equals(VodadModel.Utilities.Constants.MoneyTransferAction.Income)).Sum(w => w.Amount);
            ViewBag.TotalOutcome = moneyTransferRepository.GetAll(w => w.Action.Equals(VodadModel.Utilities.Constants.MoneyTransferAction.Outcome)).Sum(w => w.Amount);

            if (ViewBag.TotalOutcome == null)
                ViewBag.TotalOutcome = 0;
            if (ViewBag.TotalIncome == null)
                ViewBag.TotalIncome = 0;
            if (ViewBag.TotalTransfer == null)
                ViewBag.TotalTransfer = 0;
            if (ViewBag.TotalCash == null)
                ViewBag.TotalCash = 0;

            return View();
        }

    }
}
