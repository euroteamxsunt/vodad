using VodadModel;
using VodadModel.Helpers;
using VodadModel.Repository;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Vodad.Controllers
{
    public class PayPalController : WalletController
    {
        public bool IsUniqueTransaction(string transactionId)
        {
            Repository<MoneyTransfers> moneyTransferRepository = new Repository<MoneyTransfers>(Entities);

            var moneyTransfer = moneyTransferRepository.GetSingle(w => w.TransactionId.Equals(transactionId));

            if (moneyTransfer == null)
                return true;

            return false;
        }

        //RequireHttps]
        [AllowAnonymous]
        public ActionResult PayPal_Listener()
        {
            return View();
        }

        [HttpPost]
        //[RequireHttps]
        [AllowAnonymous]
        public ActionResult PayPal_Listener(object sender, EventArgs e)
        {
            //Post back to either sandbox or live
            const string strSandbox = "https://www.sandbox.paypal.com/cgi-bin/webscr";
            string strLive = "https://www.paypal.com/cgi-bin/webscr";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strSandbox);

            //Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] param = Request.BinaryRead(HttpContext.Request.ContentLength);
            string strRequest = Encoding.ASCII.GetString(param);
            string strResponse_copy = strRequest; //Save a copy of the initial info sent by PayPal
            strRequest += "&cmd=_notify-validate";
            req.ContentLength = strRequest.Length;

            //for proxy
            //WebProxy proxy = new WebProxy(new Uri("http://url:port#"));
            //req.Proxy = proxy;

            //Send the request to PayPal and get the response
            StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), Encoding.ASCII);
            streamOut.Write(strRequest);
            streamOut.Close();
            StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
            string strResponse = streamIn.ReadToEnd();
            streamIn.Close();

            if (strResponse == "VERIFIED")
            {
                //check the payment_status is Completed
                //check that txn_id has not been previously processed
                //check that receiver_email is your Primary PayPal email
                //check that payment_amount/payment_currency are correct
                //process payment

                NameValueCollection these_argies = HttpUtility.ParseQueryString(strResponse_copy);
                string user_paypal_email = these_argies["payer_email"];
                string pay_stat = these_argies["payment_status"];
                string payment_gross = these_argies["payment_gross"];
                string mc_currency = these_argies["mc_currency"];
                string mc_fee = these_argies["mc_fee"];
                string user_email = these_argies["item_name"];
                string transaction_id = these_argies["txn_id"];

                if (pay_stat.Equals("Completed"))
                {
                    if (IsUniqueTransaction(transaction_id))
                    {
                        var user = UserHelper.GetUserByEmail(user_email);

                        if (user != null)
                        {
                            Repository<UserMerchants> userMerchantRepository = new Repository<UserMerchants>(Entities);

                            var userMerchant = userMerchantRepository.GetSingle(w => w.UserId == user.Id && w.Account.Equals(user_paypal_email));

                            if (userMerchant.Merchants.MerchantName.Equals("PayPal"))
                            {
                                decimal sum = decimal.Parse(payment_gross, CultureInfo.InvariantCulture) - decimal.Parse(mc_fee, CultureInfo.InvariantCulture);

                                if (base.AddMoneyToUsersWallet((int)user.Id, sum))
                                {
                                    Repository<MoneyTransfers> moneyTransferRepository = new Repository<MoneyTransfers>(Entities);

                                    MoneyTransfers moneyTransfer = new MoneyTransfers();

                                    moneyTransfer.UserId = user.Id;
                                    moneyTransfer.DateTime = DateTime.Now;
                                    moneyTransfer.Action = VodadModel.Utilities.Constants.MoneyTransferAction.Income;
                                    moneyTransfer.Amount = sum;
                                    moneyTransfer.MerchantId = userMerchant.Merchants.Id;
                                    moneyTransfer.AccountMerchantSystem = user_paypal_email;
                                    moneyTransfer.TransactionId = transaction_id;

                                    moneyTransferRepository.Add(moneyTransfer);
                                    moneyTransferRepository.Save();

                                    Logger.Info("User {0} {1} has charged his wallet with {2} {3} with PayPal account {4}", user.Id, user.Email, sum, mc_currency, user_paypal_email);

                                }
                                else
                                    Logger.Error("User {0} {1} can't charged his wallet with {2} {3} with PayPal account {4}, because AddMoneyToUsersWallet returns false", user.Id, user.Email, sum, mc_currency, user_paypal_email);
                            }
                            else
                                Logger.Error("User {0} {1} can't charged his wallet with {2} {3} with PayPal account {4}, because not PayPal account", user.Id, user.Email, decimal.Parse(payment_gross) - decimal.Parse(mc_fee), mc_currency, user_paypal_email);
                        }
                        else
                            Logger.Error("Can't charge user's wallet, because there is no user {0}", user_email);
                    }
                    Logger.Warn("Can't charge user's wallet, because transaction is not unique {0}", transaction_id);
                }
            }
            else if (strResponse == "INVALID")
            {
                //log for manual investigation
                Logger.Error("Can't charge user's wallet, because strResponse is Invalid. StrRequest: {0}", strRequest);
            }
            else
            {
                //log response/ipn data for manual investigation
                Logger.Error("Can't charge user's wallet, because strResponse is not satandart. StrRequest: {0}", strRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}
