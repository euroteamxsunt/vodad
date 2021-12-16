using System.Collections.Generic;
using System.Web.Mvc;
using VodadModel.Repository;
using VodadModel;
using VodadModel.Helpers;
using Vodad.Models;

namespace Vodad.Controllers
{
    public class BlackAndWhiteListsController : BaseController
    {
        public ActionResult AddUserToBlackList(int? userId, int ownerId)
        {
            string success = VodadModel.Utilities.Constants.AlertMessages.Failed;

            var blackListRepository = new Repository<BlackList>(Entities);

            if (IsUserInWhiteList(userId, ownerId))
                RemoveUserFromWhiteList(userId, ownerId);

            if (!IsUserInBlackList(userId, ownerId))
            {
                var newBlackListElement = new BlackList();

                newBlackListElement.OwnerId = ownerId;
                newBlackListElement.UserId = userId;

                blackListRepository.Add(newBlackListElement);
                blackListRepository.Save();

                Logger.Info(string.Format("{0} added {1} to black list", ownerId, userId));

                success = "blacksuccess";
            }
            return RedirectToAction("Profile", "User", new { id = (long?)userId, success });
        }

        public ActionResult AddUserToWhiteList(int? userId, int ownerId)
        {
            string success = VodadModel.Utilities.Constants.AlertMessages.Failed;

            var whiteListRepository = new Repository<WhiteList>(Entities);

            if (IsUserInBlackList(userId, ownerId))
                RemoveUserFromBlackList(userId, ownerId);

            if (!IsUserInWhiteList(userId, ownerId))
            {
                var newWhiteListElement = new WhiteList();

                newWhiteListElement.OwnerId = ownerId;
                newWhiteListElement.UserId = userId;

                whiteListRepository.Add(newWhiteListElement);
                whiteListRepository.Save();

                Logger.Info(string.Format("{0} added {1} to white list", ownerId, userId));

                success = "whitesuccess";
            }

            return RedirectToAction("Profile", "User", new { id = (long?)userId, success });
        }

        public bool AreUsersInBlackLists(int firstUserId, int secondUserId)
        {
            if (IsUserInBlackList(firstUserId, secondUserId))
                return true;
            else
                if (IsUserInBlackList(secondUserId, firstUserId))
                    return true;
                else
                    return false;
        }

        public ActionResult GetUserBlackList()
        {
            var blackListRepository = new Repository<BlackList>(Entities);

            var blackListElementsList = new List<BlackAndWhiteListsModel>();

            var ownerId = UserHelper.GetUserByEmail(User.Identity.Name).Id;

            var blackList = blackListRepository.GetAll(w => w.OwnerId == ownerId);
            foreach (var b in blackList)
            {
                var blackListElement = new BlackAndWhiteListsModel();
                blackListElement.UserId = (int?)b.UserId;
                blackListElement.UserEmail = UserHelper.GetUserNameByEmail(UserHelper.GetUserById((int?)b.UserId).Email);

                blackListElementsList.Add(blackListElement);
            }

            ViewBag.BlackList = blackListElementsList;

            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetUserBlackList(string submit, ICollection<string> checkme)
        {
            var ownerId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;

            if (submit.Equals("Remove"))
            {
                foreach (var c in checkme)
                {
                    if (!c.Equals("false"))
                        RemoveUserFromBlackList(int.Parse(c), ownerId);
                }
            }
            else
                if (submit.Equals("Add to white list"))
                {
                    foreach (var c in checkme)
                    {
                        if (!c.Equals("false"))
                            AddUserToWhiteList(int.Parse(c), ownerId);
                    }
                }

            return RedirectToAction("GetUserBlackList", "BlackAndWhiteLists");
        }

        public ActionResult GetUserWhiteList()
        {
            var whiteListRepository = new Repository<WhiteList>(Entities);

            var whiteListElementsList = new List<BlackAndWhiteListsModel>();

            var ownerId = UserHelper.GetUserByEmail(User.Identity.Name).Id;

            var whiteList = whiteListRepository.GetAll(w => w.OwnerId == ownerId);
            foreach (var b in whiteList)
            {
                var whiteListElement = new BlackAndWhiteListsModel();
                whiteListElement.UserId = (int?)b.UserId;
                whiteListElement.UserEmail = UserHelper.GetUserNameByEmail(UserHelper.GetUserById((int?)b.UserId).Email);

                whiteListElementsList.Add(whiteListElement);
            }

            ViewBag.WhiteList = whiteListElementsList;

            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetUserWhiteList(string submit, ICollection<string> checkme)
        {
            var ownerId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;

            if (submit.Equals("Remove"))
            {
                foreach (var c in checkme)
                {
                    if (!c.Equals("false"))
                        RemoveUserFromWhiteList(int.Parse(c), ownerId);
                }
            }
            else
                if (submit.Equals("Add to black list"))
                {
                    foreach (var c in checkme)
                    {
                        if (!c.Equals("false"))
                            AddUserToBlackList(int.Parse(c), ownerId);
                    }
                }

            return RedirectToAction("GetUserWhiteList", "BlackAndWhiteLists");
        }

        public bool IsUserInBlackList(int? userId, int ownerId)
        {
            var blackListRepository = new Repository<BlackList>(Entities);

            var blackListElement = blackListRepository.GetSingle(w => w.UserId == userId && w.OwnerId == ownerId);

            if (blackListElement != null)
                return true;
            else
                return false;
        }

        public bool IsUserInWhiteList(int? userId, int ownerId)
        {
            var whiteListRepository = new Repository<WhiteList>(Entities);

            var whiteListElement = whiteListRepository.GetSingle(w => w.UserId == userId && w.OwnerId == ownerId);

            if (whiteListElement != null)
                return true;
            else
                return false;
        }

        public ActionResult RemoveUserFromBlackList(int? userId, int ownerId)
        {
            var blackListRepository = new Repository<BlackList>(Entities);

            var blackListElement = blackListRepository.GetSingle(w => w.UserId == userId && w.OwnerId == ownerId);

            if (blackListElement != null)
            {
                blackListRepository.Delete(blackListElement);
                blackListRepository.Save();

                Logger.Info(string.Format("{0} removed {1} from black list", ownerId, userId));
            }

            return RedirectToAction("Profile", "User", new { id = (long?)userId });
        }

        public ActionResult RemoveUserFromWhiteList(int? userId, int ownerId)
        {
            var whiteListRepository = new Repository<WhiteList>(Entities);

            var whiteListElement = whiteListRepository.GetSingle(w => w.UserId == userId && w.OwnerId == ownerId);

            if (whiteListElement != null)
            {
                whiteListRepository.Delete(whiteListElement);
                whiteListRepository.Save();

                Logger.Info(string.Format("{0} removed {1} from white list", ownerId, userId));
            }

            return RedirectToAction("Profile", "User", new { id = (long?)userId });
        }
    }
}