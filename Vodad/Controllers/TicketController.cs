﻿using System;
using System.Collections.Generic;
using System.Linq;
﻿using System.Web.Mvc;
using VodadModel;
using VodadModel.Repository;
using VodadModel.Helpers;
using Vodad.Models;
using System.Globalization;

namespace Vodad.Controllers
{
    [Authorize]
    public class TicketController : BaseController
    {
        [Authorize]
        public ActionResult AddNewTicket(bool? success)
        {
            if (success != null && success == true)
            {
                ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                ViewBag.AlertMessage = "Your ticket has been sent, thank you";
            }
            else if (success != null && success == false)
            {
                ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                ViewBag.AlertMessage = "Something went wrong, please try again later";
            }

            ViewBag.TicketThemesList = GetTicketThemeList();

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddNewTicket(AddNewTicketModel model)
        {
            bool success = false;

            if (ModelState.IsValid)
            {
                Repository<Tickets> ticketRepository = new Repository<Tickets>(Entities);
                Repository<Image> imageRepository = new Repository<Image>(Entities);

                Tickets ticket = new Tickets();
                Image image = new Image();
                OrderController orderController = new OrderController();
                int userId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;

                ticket.Title = Server.HtmlEncode(model.Title);
                ticket.ThemeId = model.ThemeId;

                if (model.Text != null && model.Text.Length > 8000 && model.Text.Length < 12000)
                {
                    ticket.Text1 = model.Text.Substring(0, 4000);
                    ticket.Text2 = model.Text.Substring(4000, 4000);
                    ticket.Text3 = model.Text.Substring(8000, model.Text.Length - 8000);
                }

                if (model.Text != null && model.Text.Length < 8000 && model.Text.Length > 4001)
                {
                    ticket.Text1 = model.Text.Substring(0, 4000);
                    ticket.Text2 = model.Text.Substring(4000, model.Text.Length - 4000);
                }

                if (model.Text != null && model.Text.Length < 4001)
                {
                    ticket.Text1 = model.Text.Substring(0, model.Text.Length);
                }

                if (model.Text == null)
                {
                    ticket.Text1 = "";
                }

                // Получить и сохранить изображение
                if (model.ImageData != null)
                {
                    var imageFromStream = System.Drawing.Image.FromStream(model.ImageData.InputStream, true, true);
                    var buffer = new System.Drawing.Bitmap(imageFromStream.Size.Width, imageFromStream.Size.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    using (var gr = System.Drawing.Graphics.FromImage(buffer))
                    {
                        gr.DrawImage(imageFromStream, 0, 0);
                    }
                    using (var sr = new System.IO.MemoryStream())
                    {
                        buffer.Save(sr, System.Drawing.Imaging.ImageFormat.Bmp);
                        image.ImageSize = sr.Length;
                        image.ImageData = sr.ToArray();
                    }

                    image.UserId = UserHelper.GetUserByEmail(User.Identity.Name).Id;
                    image.ImageName = model.ImageData.FileName;
                    image.ImageHeight = imageFromStream.Size.Height;
                    image.ImageWidth = imageFromStream.Size.Width;
                    image.CreationDate = DateTime.Today;
                    image.Extension = model.ImageData.ContentType;
                    imageRepository.Add(image);
                    imageRepository.Save();

                    int brandNewImageId = (int)imageRepository.GetSingle(w => w.UserId == image.UserId && w.CreationDate == image.CreationDate).Id;

                    Logger.Info(string.Format("Image id = {0} has been added", brandNewImageId));

                    // Добавляем связь тикета с изображением
                    ticket.ImageId = orderController.GetBrandNewImageIdByUserId(userId);
                }

                ticket.CreationDate = DateTime.Now;
                ticket.CreatorId = userId;
                ticket.Status = VodadModel.Utilities.Constants.VerificationStatuses.Verifying;

                if (model.ParentTicketId != null)
                    ticket.ParentTicketId = model.ParentTicketId;

                ticketRepository.Add(ticket);
                ticketRepository.Save();

                Logger.Info(string.Format("Ticket id = {0} has been added", GetBrandNewUsersTicketId()));

                success = true;

                int brandNewTicketId = GetBrandNewUsersTicketId();

                return RedirectToAction("ViewTicket", "Ticket", new { tid = brandNewTicketId, success });
            }

            return RedirectToAction("AddNewTicket", "Ticket", new { success });
        }

        public int GetBrandNewUsersTicketId()
        {
            Repository<Tickets> ticketRepository = new Repository<Tickets>(Entities);
            int userId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;
            return (int)ticketRepository.GetAll(w => w.CreatorId == userId).ToList().Last().Id;
        }

        public int? GetTicketThemeIdByThemeName(string ThemeName)
        {
            Repository<TicketThemes> ticketThemeRepository = new Repository<TicketThemes>(Entities);
            return (int)ticketThemeRepository.GetSingle(w => w.ThemeName == ThemeName).Id;
        }

        public string GetTicketThemeNameByThemeId(int tid)
        {
            Repository<TicketThemes> ticketThemeRepository = new Repository<TicketThemes>(Entities);
            return ticketThemeRepository.GetSingle(w => w.Id == tid).ThemeName;
        }

        public List<TicketThemes> GetTicketThemeList()
        {
            Repository<TicketThemes> ticketThemeRepository = new Repository<TicketThemes>(Entities);
            return ticketThemeRepository.GetAll().ToList();
        }

        [Authorize]
        public ActionResult ManageTickets(string status, int? themeId)
        {
            Repository<Tickets> ticketRepository = new Repository<Tickets>(Entities);

            IQueryable<Tickets> tickets = null;
            List<ManageTicketsModel> ticketsList = new List<ManageTicketsModel>();

            if (User.IsInRole(VodadModel.Utilities.Constants.UserRoles.Helper) || User.IsInRole(VodadModel.Utilities.Constants.UserRoles.Administrator))
            {
                if (status == null && themeId == null)
                    tickets = ticketRepository.GetAll();
                else if (status != null && themeId == null)
                    tickets = ticketRepository.GetAll(w => w.Status.Equals(status));
                else if (status == null && themeId != null)
                    tickets = ticketRepository.GetAll(w => w.ThemeId == themeId);
                else if (status != null && themeId != null)
                    tickets = ticketRepository.GetAll(w => (w.ThemeId == themeId && w.Status.Equals(status)));
            }
            else
            {
                int userId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;

                if (status == null && themeId == null)
                    tickets = ticketRepository.GetAll(w => w.CreatorId == userId);
                else if (status != null && themeId == null)
                    tickets = ticketRepository.GetAll(w => w.Status.Equals(status) && w.CreatorId == userId);
                else if (status == null && themeId != null)
                    tickets = ticketRepository.GetAll(w => w.ThemeId == themeId && w.CreatorId == userId);
                else if (status != null && themeId != null)
                    tickets = ticketRepository.GetAll(w => (w.ThemeId == themeId && w.Status.Equals(status) && w.CreatorId == userId));
            }

            if (tickets != null)
            {

                foreach (var t in tickets)
                {
                    ManageTicketsModel manageTicketElement = new ManageTicketsModel();

                    manageTicketElement.TicketId = (int)t.Id;
                    manageTicketElement.TicketTheme = GetTicketThemeNameByThemeId((int)t.ThemeId);
                    manageTicketElement.TicketTitle = t.Title;

                    if (t.Title.Length > 20)
                        manageTicketElement.TicketShortTitle = t.Title.Substring(0, 20) + "...";
                    else
                        manageTicketElement.TicketShortTitle = t.Title + "...";

                    manageTicketElement.TicketText = t.Text1 + t.Text2 + t.Text3;
                    manageTicketElement.TicketStatus = t.Status;

                    if (t.ImageId != null)
                        manageTicketElement.TicketImageId = (int)t.ImageId;

                    manageTicketElement.TicketCreationDate = t.CreationDate.Value.ToString("MM/dd/yyyy tt hh:mm:ss", DateTimeFormatInfo.InvariantInfo);

                    if (t.ParentTicketId != null)
                    {
                        manageTicketElement.TicketParentTicketId = (int)t.ParentTicketId;
                        manageTicketElement.TicketParentTicketShortTitle = ticketRepository.GetSingle(w => w.Id == manageTicketElement.TicketParentTicketId).Title;

                        if (manageTicketElement.TicketParentTicketShortTitle.Length > 20)
                            manageTicketElement.TicketParentTicketShortTitle = manageTicketElement.TicketParentTicketShortTitle.Substring(0, 20) + "...";
                    }

                    manageTicketElement.TicketAdminAnswer = t.AdminAnswer;
                    manageTicketElement.TicketAdminCloseComment = t.AdminCloseComment;
                    manageTicketElement.TicketCreatorId = (int)t.CreatorId;

                    if (t.AnswerAdminId != null)
                    {
                        manageTicketElement.TicketAnswerAdminId = (int)t.AnswerAdminId;
                        manageTicketElement.TicketAnswerAdminName = UserHelper.GetUserNameByEmail(UserHelper.GetUserById(manageTicketElement.TicketAnswerAdminId).Email);
                        manageTicketElement.TicketAnswerDate = t.AnswerDate.Value.ToString("MM/dd/yyyy tt hh:mm:ss", DateTimeFormatInfo.InvariantInfo);
                    }

                    if (t.CloseAdminId != null)
                    {
                        manageTicketElement.TicketCloseAdminId = (int)t.CloseAdminId;
                        manageTicketElement.TicketCloseAdminName = UserHelper.GetUserNameByEmail(UserHelper.GetUserById(manageTicketElement.TicketCloseAdminId).Email);
                        manageTicketElement.TicketCloseDate = t.CloseDate.Value.ToString("MM/dd/yyyy tt hh:mm:ss", DateTimeFormatInfo.InvariantInfo);
                    }

                    manageTicketElement.TicketCreatorName = UserHelper.GetUserNameByEmail(UserHelper.GetUserById(manageTicketElement.TicketCreatorId).Email);

                    ticketsList.Add(manageTicketElement);
                }
            }

            return View(ticketsList);
        }

        [Authorize(Roles = "Helper, Administrator")]
        public ActionResult TicketDetails(int tid, bool? success)
        {
            if (success != null && success == true)
            {
                ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                ViewBag.AlertMessage = "Your message has been sent";
            }
            else if (success != null && success == false)
            {
                ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                ViewBag.AlertMessage = "Something went wrong, please try again later";
            }

            Repository<Tickets> ticketRepository = new Repository<Tickets>(Entities);

            bool b = true;

            int ticketId = tid;

            // Получение id крайнего тикета в разговора
            while (b)
            {
                var checkTicket = ticketRepository.GetSingle(w => w.ParentTicketId == ticketId);

                if (checkTicket != null)
                    ticketId = (int)checkTicket.Id;
                else
                    b = false;
            }

            var ticket = ticketRepository.GetSingle(w => w.Id == ticketId);

            TicketDetailsModel ticketDetails = new TicketDetailsModel();

            if (ticket != null)
            {
                ticketDetails.TicketId = (int)ticket.Id;
                ticketDetails.TicketTitle = ticket.Title;
                ticketDetails.TicketTheme = GetTicketThemeNameByThemeId((int)ticket.ThemeId);
                ticketDetails.TicketCreatorId = (int)ticket.CreatorId;
                ticketDetails.TicketCreatorName = UserHelper.GetUserNameByEmail(UserHelper.GetUserById(ticketDetails.TicketCreatorId).Email);

                if (ticket.CloseAdminId != null)
                {
                    ticketDetails.TicketCloseAdminId = (int)ticket.CloseAdminId;

                    ticketDetails.TicketCloseAdminName = UserHelper.GetUserNameByEmail(UserHelper.GetUserById(ticketDetails.TicketCloseAdminId).Email);
                    ticketDetails.TicketCloseDate = ticket.CloseDate.Value.ToString("MM/dd/yyyy tt hh:mm:ss", DateTimeFormatInfo.InvariantInfo);
                    ticketDetails.TicketAdminCloseComment = ticket.AdminCloseComment;
                }
            }

            List<TicketHistoryModel> ticketHistoryList = new List<TicketHistoryModel>();

            b = true;

            while (b)
            {
                var historyTicket = ticketRepository.GetSingle(w => w.Id == ticketId);

                if (historyTicket != null)
                {
                    TicketHistoryModel ticketHistoryElement = new TicketHistoryModel();

                    ticketHistoryElement.TicketCreatorId = (int)historyTicket.CreatorId;
                    ticketHistoryElement.TicketCreatorName = UserHelper.GetUserNameByEmail(UserHelper.GetUserById(ticketHistoryElement.TicketCreatorId).Email);
                    ticketHistoryElement.TicketTitle = historyTicket.Title;
                    ticketHistoryElement.TicketCreationDate = historyTicket.CreationDate.Value.ToString("MM/dd/yyyy tt hh:mm:ss", DateTimeFormatInfo.InvariantInfo);
                    ticketHistoryElement.TicketText = historyTicket.Text1 + historyTicket.Text2 + historyTicket.Text3;

                    if (historyTicket.ImageId != null)
                        ticketHistoryElement.TicketImageId = (int)historyTicket.ImageId;

                    if (historyTicket.AnswerAdminId != null)
                    {
                        ticketHistoryElement.TicketAdminAnswerId = (int)historyTicket.AnswerAdminId;
                        ticketHistoryElement.TicketAdminAnswerName = UserHelper.GetUserNameByEmail(UserHelper.GetUserById(ticketHistoryElement.TicketAdminAnswerId).Email);
                        ticketHistoryElement.TicketAnswerDate = historyTicket.AnswerDate.Value.ToString("MM/dd/yyyy tt hh:mm:ss", DateTimeFormatInfo.InvariantInfo);
                        ticketHistoryElement.TicketAnswer = historyTicket.AdminAnswer;
                    }

                    ticketHistoryList.Add(ticketHistoryElement);
                }

                if (historyTicket.ParentTicketId != null)
                    ticketId = (int)historyTicket.ParentTicketId;
                else
                    b = false;

            }

            ViewBag.TicketHistory = ticketHistoryList;

            return View(ticketDetails);
        }

        [HttpPost]
        [Authorize(Roles = "Helper, Administrator")]
        public ActionResult TicketDetails(TicketDetailsModel model, string submit)
        {
            Repository<Tickets> ticketRepository = new Repository<Tickets>(Entities);

            var ticket = ticketRepository.GetSingle(w => w.Id == model.TicketId);

            bool success = false;

            if (submit.Equals("Send answer"))
            {
                ticket.AnswerDate = DateTime.Now;
                ticket.AnswerAdminId = UserHelper.GetUserByEmail(User.Identity.Name).Id;
                ticket.AdminAnswer = model.TicketAnswer;
                ticket.Status = VodadModel.Utilities.Constants.TicketStatuses.Answered;
                ticketRepository.Save();

                Logger.Info(string.Format("Ticket id = {0} status has been changed to answered", ticket.Id));
            }
            else
                if (submit.Equals("Close ticket") && !ticket.Status.Equals("closed"))
                {
                    ticket.CloseDate = DateTime.Now;
                    ticket.CloseAdminId = UserHelper.GetUserByEmail(User.Identity.Name).Id;
                    ticket.AdminCloseComment = model.TicketAnswer;
                    ticket.Status = VodadModel.Utilities.Constants.OrdersStatuses.Closed;
                    ticketRepository.Save();

                    Logger.Info(string.Format("Ticket id = {0} status has been changed to closed", ticket.Id));

                    // Закрыть всю историю сообщений по тикету
                    if (ticket.ParentTicketId != null)
                    {
                        bool b = true;

                        int ticketId = (int)ticket.Id;
                        while (b)
                        {
                            ticket = ticketRepository.GetSingle(w => w.Id == ticketId);

                            ticket.Status = VodadModel.Utilities.Constants.OrdersStatuses.Closed;
                            ticketRepository.Save();

                            Logger.Info(string.Format("Ticket id = {0} status has been changed to closed", ticket.Id));

                            if (ticket.ParentTicketId != null)
                                ticketId = (int)ticket.ParentTicketId;
                            else
                                b = false;
                        }
                    }
                }

            ticketRepository.Save();
            success = true;

            return RedirectToAction("TicketDetails", "Ticket", new { tid = ticket.Id, success });
        }

        [Authorize]
        public ActionResult ViewTicket(int? tid, bool? success)
        {
            if (tid != null)
            {
                if (success != null && success == true)
                {
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Success;
                    ViewBag.AlertMessage = "Your message has been sent";
                }
                else if (success != null && success == false)
                {
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                    ViewBag.AlertMessage = "Something went wrong, please try again later";
                }

                Repository<Tickets> ticketRepository = new Repository<Tickets>(Entities);

                bool b = true;

                int ticketId = (int)tid;

                // Получение id крайнего тикета в разговора
                while (b)
                {
                    var checkTicket = ticketRepository.GetSingle(w => w.ParentTicketId == ticketId);

                    if (checkTicket != null)
                        ticketId = (int)checkTicket.Id;
                    else
                        b = false;
                }

                var ticket = ticketRepository.GetSingle(w => w.Id == ticketId);

                TicketDetailsModel ticketDetails = new TicketDetailsModel();

                if (ticket != null)
                {
                    ticketDetails.TicketId = (int)ticket.Id;
                    ticketDetails.TicketTitle = ticket.Title;
                    ticketDetails.TicketTheme = GetTicketThemeNameByThemeId((int)ticket.ThemeId);
                    ticketDetails.TicketCreatorId = (int)ticket.CreatorId;
                    ticketDetails.TicketCreatorName =
                        UserHelper.GetUserNameByEmail(UserHelper.GetUserById(ticketDetails.TicketCreatorId).Email);

                    if (ticket.CloseAdminId != null)
                    {
                        ticketDetails.TicketCloseAdminId = (int)ticket.CloseAdminId;

                        ticketDetails.TicketCloseAdminName =
                            UserHelper.GetUserNameByEmail(UserHelper.GetUserById(ticketDetails.TicketCloseAdminId).Email);
                        ticketDetails.TicketCloseDate = ticket.CloseDate.Value.ToString("MM/dd/yyyy tt hh:mm:ss",
                                                                                        DateTimeFormatInfo.InvariantInfo);
                        ticketDetails.TicketAdminCloseComment = ticket.AdminCloseComment;
                    }
                }

                List<TicketHistoryModel> ticketHistoryList = new List<TicketHistoryModel>();

                b = true;

                while (b)
                {
                    var historyTicket = ticketRepository.GetSingle(w => w.Id == ticketId);

                    if (historyTicket != null)
                    {
                        TicketHistoryModel ticketHistoryElement = new TicketHistoryModel();

                        ticketHistoryElement.TicketCreatorId = (int)historyTicket.CreatorId;
                        ticketHistoryElement.TicketCreatorName =
                            UserHelper.GetUserNameByEmail(
                                UserHelper.GetUserById(ticketHistoryElement.TicketCreatorId).Email);
                        ticketHistoryElement.TicketTitle = historyTicket.Title;
                        ticketHistoryElement.TicketCreationDate =
                            historyTicket.CreationDate.Value.ToString("MM/dd/yyyy tt hh:mm:ss",
                                                                      DateTimeFormatInfo.InvariantInfo);
                        ticketHistoryElement.TicketText = historyTicket.Text1 + historyTicket.Text2 +
                                                          historyTicket.Text3;

                        if (historyTicket.ImageId != null)
                            ticketHistoryElement.TicketImageId = (int)historyTicket.ImageId;

                        if (historyTicket.AnswerAdminId != null)
                        {
                            ticketHistoryElement.TicketAdminAnswerId = (int)historyTicket.AnswerAdminId;
                            ticketHistoryElement.TicketAdminAnswerName =
                                UserHelper.GetUserNameByEmail(
                                    UserHelper.GetUserById(ticketHistoryElement.TicketAdminAnswerId).Email);
                            ticketHistoryElement.TicketAnswerDate =
                                historyTicket.AnswerDate.Value.ToString("MM/dd/yyyy tt hh:mm:ss",
                                                                        DateTimeFormatInfo.InvariantInfo);
                            ticketHistoryElement.TicketAnswer = historyTicket.AdminAnswer;
                        }

                        ticketHistoryList.Add(ticketHistoryElement);
                    }

                    if (historyTicket.ParentTicketId != null)
                        ticketId = (int)historyTicket.ParentTicketId;
                    else
                        b = false;

                }

                ViewBag.TicketHistory = ticketHistoryList;

                return View(ticketDetails);
            }
            else
                return RedirectToAction("Error404", "Error");
        }

        [HttpPost]
        [Authorize]
        public ActionResult ViewTicket(TicketDetailsModel model)
        {
            bool success = false;

            AddNewTicketModel ticket = new AddNewTicketModel();

            ticket.Title = "Re: " + model.TicketTitle;
            ticket.ThemeId = GetTicketThemeIdByThemeName(model.TicketTheme);
            ticket.Text = model.TicketAnswer;
            ticket.ImageData = model.ImageData;
            ticket.ParentTicketId = model.TicketId;

            AddNewTicket(ticket);
            success = true;

            int brandNewTicketId = GetBrandNewUsersTicketId();

            return RedirectToAction("ViewTicket", "Ticket", new { tid = brandNewTicketId, success });
        }
    }
}