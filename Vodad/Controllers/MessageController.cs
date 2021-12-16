﻿using VodadModel;
using VodadModel.Helpers;
using VodadModel.Repository;
using Vodad.Models;
using System;
using System.Collections.Generic;
﻿using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vodad.Controllers
{
    public class MessageController : BaseController
    {
        private void AddMessage(Messages message)
        {
            var messagesRepo = new Repository<Messages>(Entities);

            messagesRepo.Add(message);
            messagesRepo.Save();

            var brandNewMessageId = (int)messagesRepo.GetAll(w => w.FromUserId == message.FromUserId && w.ToUserId == message.ToUserId).ToList().Last().Id;

            Logger.Info(string.Format("Message {0} has been added", brandNewMessageId));
        }

        [Authorize]
        public ActionResult ChatWithUser(int? companionId)
        {
            ViewBag.Messages = GetUserMessages(companionId);
            ViewBag.RecieverId = companionId;

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult ChatWithUser(int? companionId, SendMessageModel model)
        {
            ViewBag.Messages = GetUserMessages(companionId).ToList();
            ViewBag.RecieverId = companionId;

            if (ModelState.IsValid)
            {
                var message = new Messages();

                var fromUser = UserHelper.GetUserByEmail(User.Identity.Name);

                message.CreationDate = DateTime.Now;
                message.FromUserId = fromUser.Id;
                message.ToUserId = long.Parse(model.ToUserId);
                message.MessageText = Server.HtmlEncode(model.MessageText);
                message.IsRead = false;
                message.IsDeletedForAuthor = false;
                message.IsDeletedForReciever = false;

                AddMessage(message);
            }

            return RedirectToAction("ChatWithUser", "Message", new { companionId });
        }

        private List<ReadMessageModel> GetUserMessages(int? companionId)
        {
            var messagesRepo = new Repository<Messages>(Entities);
            var userRep = new UserRepository();

            var user = UserHelper.GetUserByEmail(User.Identity.Name);

            var messages = messagesRepo.GetAll(w => (w.FromUserId == companionId && w.ToUserId == user.Id) || (w.ToUserId == companionId && w.FromUserId == user.Id)); // Проверка относятся ли эти сообщения к данному конкретному пользователю, просматривающему их

            var messageList = new List<ReadMessageModel>();

            foreach (var m in messages)
            {
                if ((user.Id == m.FromUserId && m.IsDeletedForAuthor == false) || (user.Id == m.ToUserId && m.IsDeletedForReciever == false))
                {
                    var newReadMessage = new ReadMessageModel();

                    newReadMessage.UserId = (int)m.FromUserId;
                    newReadMessage.DateTime = m.CreationDate.Value.ToString("hh:mm:ss tt MM/dd");
                    newReadMessage.DateTimeForSorting = m.CreationDate.ToString();
                    newReadMessage.MessageText = HttpUtility.HtmlDecode(m.MessageText);
                    newReadMessage.FromUserEmail = userRep.GetUserNameById((int)m.FromUserId);

                    messageList.Add(newReadMessage);

                    if (user.Id == m.ToUserId)
                    {
                        m.IsRead = true;
                    }

                    Logger.Info(string.Format("Message {0} status has been changed to IsRead", m.Id));
                }
            }

            messagesRepo.Save();

            return messageList;
        }

        private List<MessengersModel> GetUserMessengers()
        {
            var messagesRepo = new Repository<Messages>(Entities);
            var userRep = new UserRepository();

            var user = UserHelper.GetUserByEmail(User.Identity.Name);

            var messengers = messagesRepo.GetAll(w => w.FromUserId == user.Id || w.ToUserId == user.Id);

            var messengersList = new List<MessengersModel>();

            foreach (var m in messengers)
            {
                var newMessenger = new MessengersModel();

                bool a = false;

                // Проверка существует ли хоть одно неудаленное сообщение со стороны пользователя в чате с данным messenger

                // Поиск всех сообщений, относящихся к этому чату этих двух пользователей
                var messages = messagesRepo.GetAll(w => (w.FromUserId == m.FromUserId && w.ToUserId == m.ToUserId) || (w.FromUserId == m.ToUserId && w.ToUserId == m.FromUserId));
                foreach (var message in messages)
                {
                    if (user.Id == message.FromUserId && message.IsDeletedForAuthor == false)
                    {
                        a = true;
                        break;
                    }
                    if (user.Id == message.ToUserId && message.IsDeletedForReciever == false)
                    {
                        a = true;
                        break;
                    }
                }

                if (a)
                {
                    string companionEmail;
                    int companionId;

                    // Необходимо определить собеседника, чтобы именно его имя выводилось как имя собеседника, а не имя создателя первого сообщения,
                    // для этого проверяем кем является залогиненный пользователь - получателем или автором, соответственно выводу присваиваем значения
                    if (User.Identity.Name == userRep.GetUserEmailById((int)m.FromUserId))
                    {
                        companionEmail = userRep.GetUserNameById((int)m.ToUserId);
                        companionId = (int)m.ToUserId;
                        newMessenger.UserId = (int)m.ToUserId;
                    }
                    else
                    {
                        companionEmail = userRep.GetUserNameById((int)m.FromUserId);
                        companionId = (int)m.FromUserId;
                        newMessenger.UserId = (int)m.FromUserId;
                    }

                    newMessenger.CompanionId = companionId;
                    newMessenger.FromUserEmail = companionEmail;
                    newMessenger.LastMessageDateTime = messages.ToList().Last().CreationDate;

                    // Подсчет количества не удаленных сообщений
                    newMessenger.MessagesCount = 0;
                    newMessenger.MessagesUnreadCount = 0;
                    foreach (var message in messages)
                    {
                        if (user.Id == message.FromUserId && message.IsDeletedForAuthor == false)
                        {
                            newMessenger.MessagesCount++;
                            if (!(bool)message.IsRead)
                                newMessenger.MessagesUnreadCount++;
                        }
                        if (user.Id == message.ToUserId && message.IsDeletedForReciever == false)
                        {
                            newMessenger.MessagesCount++;
                            if (!(bool)message.IsRead)
                                newMessenger.MessagesUnreadCount++;
                        }
                    }

                    // Проверка на наличие собеседника в списке собеседников
                    bool b = false;
                    foreach (var v in messengersList)
                    {
                        if (v.FromUserEmail == newMessenger.FromUserEmail)
                            b = true;
                    }

                    var blackAndWhiteListsController = new BlackAndWhiteListsController();

                    if (b == false && !blackAndWhiteListsController.IsUserInBlackList(companionId, (int)user.Id))
                        messengersList.Add(newMessenger);
                }
            }

            return messengersList;
        }

        public void MarkMessagesDeleteForUser(int whoDeletes, int companion)
        {
            var userId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;

            if (whoDeletes == userId)
            {
                var messagesRepo = new Repository<Messages>(Entities);
                var messages = messagesRepo.GetAll(w => (w.FromUserId == whoDeletes && w.ToUserId == companion) || (w.ToUserId == whoDeletes && w.FromUserId == companion));

                foreach (var m in messages)
                {
                    if (m.FromUserId == whoDeletes)
                    {
                        m.IsDeletedForAuthor = true;
                        Logger.Info(string.Format("Message {0} is deleted for author", m.Id));
                    }
                    else if (m.ToUserId == whoDeletes)
                    {
                        m.IsDeletedForReciever = true;
                        Logger.Info(string.Format("Message {0} is deleted for reciever", m.Id));
                    }
                }

                messagesRepo.Save();
            }
        }

        [Authorize]
        public ActionResult MessageSummary(int? companionId)
        {
            ViewBag.Messages = GetUserMessages(companionId);

            return PartialView("MessageSummary");
        }

        public ActionResult PrivateMessages()
        {
            ViewBag.Messengers = GetUserMessengers();

            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult PrivateMessages(string submit, ICollection<string> checkme)
        {
            var userId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;

            var lists = new BlackAndWhiteListsController();

            if (submit.Equals("Delete"))
            {
                foreach (var c in checkme)
                {
                    if (!c.Equals("false"))
                        MarkMessagesDeleteForUser(userId, int.Parse(c));
                }
            }
            else
                if (submit.Equals("Add to white list"))
                {
                    foreach (var c in checkme)
                    {
                        if (!c.Equals("false"))
                            lists.AddUserToWhiteList(int.Parse(c), userId);
                    }
                }
                else
                    if (submit.Equals("Add to black list"))
                    {
                        foreach (var c in checkme)
                        {
                            if (!c.Equals("false"))
                                lists.AddUserToBlackList(int.Parse(c), userId);
                        }
                    }

            return RedirectToAction("PrivateMessages", "Message");
        }
    }
}