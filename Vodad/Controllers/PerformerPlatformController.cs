using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using VodadModel;
using VodadModel.Repository;
using Vodad.Models;
using VodadModel.Helpers;

namespace Vodad.Controllers
{
    public class PerformerPlatformController : BaseController
    {
        [Authorize(Roles = "Performer")]
        public ActionResult AddPlatform(string success)
        {
            if (success != null)
            {
                if (success.Equals(VodadModel.Utilities.Constants.AlertMessages.Failed))
                {
                    ViewBag.AlertMessage = "You have filled the form incorrectly.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
            }

            ViewBag.ThemesList = GetThemesList();

            return View();
        }

        [Authorize(Roles = "Performer")]
        [HttpPost]
        public ActionResult AddPlatform(PerformerPlatformModel model)
        {
            if (ModelState.IsValid)
            {
                var performerPlatformRepository = new Repository<PerformerPlatform>(Entities);
                var userRepository = new Repository<User>(Entities);

                var user = userRepository.GetSingle(w => w.Email == User.Identity.Name);

                var performerPlatform = new PerformerPlatform();

                model.PerformerPlatformLogin = model.PerformerPlatformLogin.ToLower();

                if (!IsUniquePerformerPlatforms(model.PerformerPlatformLogin))
                    return RedirectToAction("ManagePlatforms", "PerformerPlatform", new { success = "notuniqueplatform" });

                performerPlatform.PerformerId = user.Id;
                performerPlatform.ChannelName = model.ChannelName;
                performerPlatform.Login = Server.HtmlEncode(model.PerformerPlatformLogin);
                performerPlatform.Password = EncryptionHelper.EncryptStringAES(Server.HtmlEncode(model.PerformerPlatformPassword), user.PasswordSalt); //FormsAuthentication.HashPasswordForStoringInConfigFile(Server.HtmlEncode(model.PerformerPlatformPassword), "sha1");
                performerPlatform.ThemeId = model.PerformerPlatformThemeId;
                performerPlatform.Verified = VodadModel.Utilities.Constants.CredentialCheckStatuses.Unverified;
                performerPlatform.Date = DateTime.Today;
                performerPlatform.Status = VodadModel.Utilities.Constants.OnlineStatuses.Offline;
                performerPlatform.LogoPrice = model.LogoPrice ?? 0;
                performerPlatform.VideoPrice = model.VideoPrice ?? 0;

                performerPlatformRepository.Add(performerPlatform);
                performerPlatformRepository.Save();

                var brandNewPerformerPlatformId = (int)performerPlatformRepository.GetSingle(w => w.PerformerId == user.Id && w.Login.Equals(performerPlatform.Login)).Id;

                Logger.Info(string.Format("PerformerPlatform id = {0} has been added", brandNewPerformerPlatformId));

                return RedirectToAction("ManagePlatforms", "PerformerPlatform", new { success = "platformadded" });
            }

            return RedirectToAction("AddPlatform", "PerformerPlatform", new { success = VodadModel.Utilities.Constants.AlertMessages.Failed });
        }

        [Authorize(Roles = "Performer")]
        public ActionResult ChangePerformerPlatformStatus(int ppid, string status)
        {
            if (ppid != null)
            {
                if (status.Equals(VodadModel.Utilities.Constants.OrdersStatuses.Deleted))
                {
                    int userId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;

                    if (IsUsersPerformerPlatform(ppid, userId))
                    {
                        var performerPlatformRepository = new Repository<PerformerPlatform>(Entities);
                        var orderPerformedRepository = new Repository<OrderPerformed>(Entities);

                        var performerPlatform = performerPlatformRepository.GetSingle(w => w.Id == ppid);

                        if (performerPlatform != null)
                        {
                            if (performerPlatform.OrderPerformed.Any(w => w.Status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Inaction)))
                            {
                                var inactionOrderPerformed = orderPerformedRepository.GetAll(w => w.PerformerPlatformId == ppid && w.Status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Inaction));

                                foreach (var op in inactionOrderPerformed)
                                {
                                    OrderHelper.ChangeOrderPerformedStatus((int)op.Id, VodadModel.Utilities.Constants.OrdersStatuses.Pay, (int)op.PerformerPlatform.User.Id);
                                }
                            }
                            else
                            {
                                DeletePerformerPlatform(ppid, userId);
                                Logger.Info(string.Format("PerformerPlatform id = {0} has been deleted", ppid));
                            }

                            return RedirectToAction("ManagePlatforms", "PerformerPlatform", new { success = "platformdeleted" });
                        }
                        else
                        {
                            DeletePerformerPlatform(ppid, userId);
                            Logger.Info(string.Format("PerformerPlatform id = {0} has been deleted", ppid));
                        }

                        return RedirectToAction("ManagePlatforms", "PerformerPlatform", new { status = VodadModel.Utilities.Constants.AlertMessages.Success });
                    }
                }
            }

            return RedirectToAction("ManagePlatforms", "PerformerPlatform", new { status = VodadModel.Utilities.Constants.AlertMessages.Failed });
        }

        private void DeletePerformerPlatform(int ppid, int userId)
        {
            if (IsUsersPerformerPlatform(ppid, userId))
            {
                var performerPlatformRepository = new Repository<PerformerPlatform>(Entities);

                var performerPlatform = performerPlatformRepository.GetSingle(w => w.Id == ppid);

                performerPlatformRepository.Delete(performerPlatform);
                performerPlatformRepository.Save();
                Logger.Info(string.Format("PerformerPlatform id = {0} has been deleted", performerPlatform.Id));
            }
        }

        [Authorize(Roles = "Performer")]
        public ActionResult EditPerformerPlatform(int? ppid)
        {
            if (ppid != null)
            {
                int userId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;

                if (IsUsersPerformerPlatform(ppid, userId))
                {
                    ViewBag.PerformerPlatform = GetPerformerPlatformById(ppid);

                    ViewBag.ThemesList = GetThemesList();

                    return View();
                }
                else
                    return RedirectToAction("ManagePlatforms", "PerformerPlatform");
            }
            else
                return RedirectToAction("Error404", "Error");
        }

        [Authorize(Roles = "Performer")]
        [HttpPost]
        public ActionResult EditPerformerPlatform(PerformerPlatformModel model, int? ppid)
        {
            var user = UserHelper.GetUserByEmail(User.Identity.Name);

            if (ModelState.IsValid && ppid != null && IsUsersPerformerPlatform(ppid, (int)user.Id))
            {
                var performerPlatformRepository = new Repository<PerformerPlatform>(Entities);

                var performerPlatform = performerPlatformRepository.GetSingle(w => w.Id == ppid);

                performerPlatform.ChannelName = model.ChannelName;
                performerPlatform.Login = Server.HtmlEncode(model.PerformerPlatformLogin);
                performerPlatform.Password = EncryptionHelper.EncryptStringAES(Server.HtmlEncode(model.PerformerPlatformPassword), user.PasswordSalt); //FormsAuthentication.HashPasswordForStoringInConfigFile(Server.HtmlEncode(model.PerformerPlatformPassword), "sha1");
                performerPlatform.ThemeId = model.PerformerPlatformThemeId;
                performerPlatform.Verified = VodadModel.Utilities.Constants.CredentialCheckStatuses.Unverified;
                performerPlatform.Status = VodadModel.Utilities.Constants.OnlineStatuses.Offline;
                performerPlatform.LogoPrice = model.LogoPrice ?? 0;
                performerPlatform.VideoPrice = model.VideoPrice ?? 0;

                performerPlatformRepository.Save();

                Logger.Info(string.Format("PerformerPlatform id = {0} has been modified", performerPlatform.Id));
            }

            return RedirectToAction("ManagePlatforms", "PerformerPlatform");
        }

        private PerformerPlatform GetPerformerPlatformById(int? ppid)
        {
            var performerPlatformRepository = new Repository<PerformerPlatform>(Entities);

            return performerPlatformRepository.GetSingle(w => w.Id == ppid);
        }

        private IEnumerable<Themes> GetThemesList()
        {
            var themesRepository = new Repository<Themes>(Entities);
            return themesRepository.GetAll().ToList();
        }

        private List<PerformerPlatformListModel> GetUserPlatformsList(int? userId)
        {
            var performerPlatformRepository = new Repository<PerformerPlatform>(Entities);
            var themesRepository = new Repository<Themes>(Entities);
            var geolocationPlatformPercentageRepository = new Repository<GeolocationPlatformPercentage>(Entities);

            var platforms = performerPlatformRepository.GetAll(w => w.PerformerId == userId && !w.Verified.Equals(VodadModel.Utilities.Constants.OrdersStatuses.Deleted));

            var platformsList = new List<PerformerPlatformListModel>();

            foreach (var p in platforms)
            {
                var performerPlatform = new PerformerPlatformListModel();

                var theme = themesRepository.GetSingle(w => w.Id == p.ThemeId);

                performerPlatform.PerformerPlatformId = (int)p.Id;
                performerPlatform.ChannelName = p.ChannelName;
                performerPlatform.PerformerPlatformLogin = p.Login;

                if (p.Verified.Equals(VodadModel.Utilities.Constants.CredentialCheckStatuses.Unverified))
                    performerPlatform.PerformerPlatformVerified = "Unverified";
                else if (p.Verified.Equals(VodadModel.Utilities.Constants.CredentialCheckStatuses.Verified))
                    performerPlatform.PerformerPlatformVerified = "Verified";
                else if (p.Verified.Equals(VodadModel.Utilities.Constants.CredentialCheckStatuses.Failed))
                    performerPlatform.PerformerPlatformVerified = VodadModel.Utilities.Constants.AlertMessages.Failed;
                else
                    performerPlatform.PerformerPlatformVerified = "Notpassed";

                performerPlatform.VideoPrice = p.VideoPrice ?? 0;
                performerPlatform.LogoPrice = p.LogoPrice ?? 0;
                performerPlatform.PerformerPlatformTheme = theme.Name;
                performerPlatform.PerformerPlatformLink = p.Link;

                try
                {
                    performerPlatform.UniqueViewersForMonth = (int)p.PerformerStatistics.FirstOrDefault(w => w.PerformerPlatformId == p.Id).UniqueViewersForMonth;
                }
                catch
                {
                    performerPlatform.UniqueViewersForMonth = 0;
                }

                try
                {
                    performerPlatform.AverageViewersPerHour = (int)p.PerformerStatistics.FirstOrDefault(w => w.PerformerPlatformId == p.Id).AverageViewerCountPerHour;
                }
                catch
                {
                    performerPlatform.AverageViewersPerHour = 0;
                }

                try
                {
                    performerPlatform.TotalFollowers = (int)p.PerformerStatistics.FirstOrDefault(w => w.PerformerPlatformId == p.Id).TotalFollowers;
                }
                catch
                {
                    performerPlatform.TotalFollowers = 0;
                }

                try
                {
                    performerPlatform.MaxViewersCount = (int)p.PerformerStatistics.FirstOrDefault(w => w.PerformerPlatformId == p.Id).MaxViewersCount;
                }
                catch
                {
                    performerPlatform.MaxViewersCount = 0;
                }

                try
                {
                    performerPlatform.TotalUniqueViews = (int)p.PerformerStatistics.FirstOrDefault(w => w.PerformerPlatformId == p.Id).TotalUniqueViews;
                }
                catch
                {
                    performerPlatform.TotalUniqueViews = 0;
                }

                try
                {
                    performerPlatform.TotalViews = (int)p.PerformerStatistics.FirstOrDefault(w => w.PerformerPlatformId == p.Id).TotalViews;
                }
                catch
                {
                    performerPlatform.TotalViews = 0;
                }

                try
                {
                    performerPlatform.AverageComplitionSpeed = p.PerformerStatistics.FirstOrDefault(w => w.PerformerPlatformId == p.Id).AverageComplitionSpeed;
                }
                catch
                {
                    performerPlatform.AverageComplitionSpeed = 0;
                }

                try
                {
                    performerPlatform.TotalOrders = (int)p.PerformerStatistics.FirstOrDefault(w => w.PerformerPlatformId == p.Id).TotalOrders;
                }
                catch
                {
                    performerPlatform.TotalOrders = 0;
                }

                performerPlatform.GeolocationAndPercentage = new List<string>();

                var geolocation = geolocationPlatformPercentageRepository.GetAll(w => w.PerformerPlatformId == p.Id).OrderByDescending(w => w.Percentage).Take(5);

                if (geolocation != null)
                {
                    foreach (var g in geolocation)
                    {
                        performerPlatform.GeolocationAndPercentage.Add(g.Geolocation.CountryName + " - " + g.Percentage + "%");
                    }
                }

                platformsList.Add(performerPlatform);
            }

            return platformsList;
        }

        public bool IsUniquePerformerPlatforms(string platformLogin)
        {
            Repository<PerformerPlatform> performerPlatformRepository = new Repository<PerformerPlatform>(Entities);

            var performerPlatform =
                performerPlatformRepository.GetAll(w => w.Login.Equals(platformLogin));

            if (!performerPlatform.Any())
                return true;

            return false;
        }

        public bool IsUsersPerformerPlatform(int? ppid, int userId)
        {
            var performerPlatformRepository = new Repository<PerformerPlatform>(Entities);
            var userRepository = new Repository<User>(Entities);

            var performerPlatform = performerPlatformRepository.GetSingle(w => w.Id == ppid);
            var user = userRepository.GetSingle(w => w.Id == userId);

            if (performerPlatform != null && user != null && performerPlatform.PerformerId == user.Id)
                return true;
            else
                return false;
        }

        [Authorize(Roles = "Performer")]
        public ActionResult ManagePlatforms(string success)
        {
            if (success != null)
            {
                if (success.Equals("notuniqueplatform"))
                {
                    ViewBag.AlertMessage = "This account has been already added.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
                else if (success.Equals("platformadded"))
                {
                    ViewBag.AlertMessage = "Platform has been added.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                }
                else if (success.Equals("platformdeleted"))
                {
                    ViewBag.AlertMessage = "Platform has been deleted.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                }
                else if (success.Equals("retrying"))
                {
                    ViewBag.AlertMessage = "Platform status has been changed.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                }
                else if (success.Equals("notretrying"))
                {
                    ViewBag.AlertMessage = "Something went wrong, please, try again later.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
                else if (success.Equals("accessdenied"))
                {
                    ViewBag.AlertMessage = "You have to grant access to channel.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
            }

            var userRepository = new Repository<User>(Entities);

            var user = userRepository.GetSingle(w => w.Email == User.Identity.Name);

            ViewBag.PerformerPlatformsList = GetUserPlatformsList((int)user.Id);

            return View();
        }

        [Authorize(Roles = "Performer")]
        public ActionResult RetryVerifying(int? ppid)
        {
            if (ppid != null)
            {
                var performerPlatformRepository = new Repository<PerformerPlatform>(Entities);

                var user = UserHelper.GetUserByEmail(User.Identity.Name);

                var PerformerPlatform = performerPlatformRepository.GetSingle(w => w.Id == ppid);

                if (user != null && IsUsersPerformerPlatform(ppid, (int)user.Id) && PerformerPlatform.Verified.Equals(VodadModel.Utilities.Constants.CredentialCheckStatuses.Notpassed))
                {
                    PerformerPlatform.Verified = VodadModel.Utilities.Constants.CredentialCheckStatuses.Unverified;

                    Logger.Info(string.Format("PerformerPlatform id = {0} status verified has been changed to notpassed", PerformerPlatform.Id));

                    performerPlatformRepository.Save();

                    return RedirectToAction("ManagePlatforms", "PerformerPlatform", new { success = "retrying" });
                }

                return RedirectToAction("ManagePlatforms", "PerformerPlatform", new { success = "notretrying" });
            }

            return RedirectToAction("ManagePlatforms", "PerformerPlatform");
        }

        [Authorize(Roles = "Performer")]
        public ActionResult VerifyPlatform(string error, string code)
        {
            if (error != null)
                return RedirectToAction("ManagePlatforms", "PerformerPlatform", new {success = "accessdenied"});

            if (code != null)
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create("https://accounts.google.com/o/oauth2/token");
                ASCIIEncoding encoding = new ASCIIEncoding();

                string postData = code;
                postData += "&client_id=516698891787-2080512g0ml237fdi1d8c8rsfd8ske1h.apps.googleusercontent.com";
                postData += "&client_secret=8oICTRoydh9YlO_uLsvhajCD";
                postData += "&redirect_uri=http://siniykot.in.ua/PerformerPlatform/VerifyPlatform/";
                postData += "&grant_type=authorization_code";
                byte[] data = encoding.GetBytes(postData);

                httpWReq.Method = "POST";
                httpWReq.ProtocolVersion = HttpVersion.Version11;
                httpWReq.Host = "accounts.google.com";
                httpWReq.ContentType = "application/x-www-form-urlencoded";
                httpWReq.ContentLength = data.Length;

                using (Stream stream = httpWReq.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();

                string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            }

            return RedirectToAction("ManagePlatforms", "PerformerPlatform", new { success = "notretrying" });
        }
    }
}