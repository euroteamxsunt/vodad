﻿using System;
﻿using System.Linq;
using System.Web;
using System.Web.Mvc;
using VodadModel.Repository;
using VodadModel;
using VodadModel.Helpers;

namespace Vodad.Controllers
{
    public class UserController : BaseController
    {
        public new ActionResult Profile(long? id, string success)
        {
            Repository<User> userRepository = new Repository<User>(Entities);
            Repository<OrderPerformed> orderPerformedRepository = new Repository<OrderPerformed>(Entities);

            var user = userRepository.GetSingle(u => u.Id == id);

            if (success != null)
            {
                if (success.Equals("blacksuccess"))
                {
                    ViewBag.AlertMessage = "User has been added to Your black list";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                }
                else if (success.Equals("whitesuccess"))
                {
                    ViewBag.AlertMessage = "User has been added to Your white list";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                }
                else if (success.Equals("userbanned"))
                {
                    ViewBag.AlertMessage = "User has been banned";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                }
                else if (success.Equals("userunbanned"))
                {
                    ViewBag.AlertMessage = "User has been unbanned";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                }
                else if (success.Equals("canntbeunbanned"))
                {
                    ViewBag.AlertMessage = "User can not be unbanned";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
                else if (success.Equals("userisnotbanned"))
                {
                    ViewBag.AlertMessage = "User is not banned";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
                else if (success.Equals("rolechanged"))
                {
                    ViewBag.AlertMessage = "Role has been changed";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                }
                else if (success.Equals("rolenotchanged"))
                {
                    ViewBag.AlertMessage = "Role hasn't been changed";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
            }

            if (user != null && user.Id > 10)
            {
                if (id == null)
                    throw new HttpException(404, "User not found!");

                ViewBag.UserId = user.Id;
                ViewBag.UserName = user.Name;
                ViewBag.RoleName = user.Roles.RoleName;
                ViewBag.Age = (DateTime.Today - (DateTime)user.RegistrationDate).TotalDays;
                ViewBag.LastOnlineDate = (int)(DateTime.Today - (DateTime)user.LastOnlineDateTime).TotalDays;
                if (ViewBag.LastOnlineDate == 0)
                    ViewBag.LastOnlineDate = "Today";
                if ((DateTime.Today - (DateTime)user.LastOnlineDateTime).TotalMinutes < 5)
                    ViewBag.LastOnlineDate = "Online";

                if (user.Roles.RoleName.Equals(VodadModel.Utilities.Constants.UserRoles.Advertiser))
                    ViewBag.PaidOrderPerformed = orderPerformedRepository.GetAll(w => w.OrderContent.Order.User.Id == id && w.Status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Complete)).Count();
                else
                    if (user.Roles.RoleName.Equals(VodadModel.Utilities.Constants.UserRoles.Performer))
                        ViewBag.PaidOrderPerformed = orderPerformedRepository.GetAll(w => w.PerformerPlatform.User.Id == id && w.Status.Equals(VodadModel.Utilities.Constants.VerificationStatuses.Complete)).Count();

                ViewBag.Karma = user.Karma;

                if (!User.Identity.Name.Equals(""))
                {
                    ViewBag.OwnerId = UserHelper.GetUserByEmail(User.Identity.Name).Id;
                }

                ViewBag.Comments = HttpUtility.HtmlDecode(user.Comments);

                return View();
            }
            else
                return RedirectToAction("Error404", "Error");
        }
    }
}