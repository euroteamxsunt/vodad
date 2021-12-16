﻿using System;
using System.Collections.Generic;
﻿using System.Linq;
﻿using System.Web.Mvc;
﻿using VodadModel;
using VodadModel.Repository;
using VodadModel.Helpers;
using Vodad.Models;
using System.Globalization;
using System.Drawing.Imaging;
using System.IO;
//using Google.GData.Extensions;

namespace Vodad.Controllers
{
    public class OrderController : BaseController
    {
        [Authorize(Roles = "Advertiser")]
        public ActionResult AddNewOrder(string success)
        {
            if (success != null)
            {
                if (success.Equals("contentfailed"))
                {
                    ViewBag.AlertMessage = "You must choose content for this order";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
                else if (success.Equals("contentsizefailed"))
                {
                    ViewBag.AlertMessage = "Content size can't be over " + VodadModel.Utilities.Constants.BannerParameters.MaxWidth + "x" + VodadModel.Utilities.Constants.BannerParameters.MaxHeight + "!";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
                else if (success.Equals("badcontentformat"))
                {
                    ViewBag.AlertMessage = "Only bmp, gif, jpeg, jpg or png images and mp4, mpeg, wmv videos.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
            }

            var order = new OrderModel();
            ViewBag.RegionsList = GetRegionsList();
            order.OrderThemesList = GetThemesListForOrder();
            order.OrderExpireDate = DateTime.Today.ToString("MM/dd/yyyy", DateTimeFormatInfo.InvariantInfo);

            return View(order);
        }

        [HttpPost]
        [Authorize(Roles = "Advertiser")]
        public ActionResult AddNewOrder(OrderModel model)
        {
            int userId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;

            if (ModelState.IsValid)
            {
                var orderRepository = new Repository<Order>(Entities);
                var orderContentRepository = new Repository<OrderContent>(Entities);
                var order = new Order();
                var orderContent = new OrderContent();

                int brandNewContentId = 0;
                if (model.ContentData.ContentType.Equals("image/gif") ||
                    model.ContentData.ContentType.Equals("image/png") ||
                    model.ContentData.ContentType.Equals("image/jpeg"))
                {
                    // Получить и сохранить контент
                    var contentFromStream = System.Drawing.Image.FromStream(model.ContentData.InputStream, true, true);
                    var buffer = new System.Drawing.Bitmap(contentFromStream.Size.Width, contentFromStream.Size.Height,
                                                           PixelFormat.Format24bppRgb);

                    var image = new Image();

                    using (var gr = System.Drawing.Graphics.FromImage(buffer))
                    {
                        gr.DrawImage(contentFromStream, 0, 0);
                    }
                    using (var sr = new MemoryStream())
                    {
                        buffer.Save(sr, ImageFormat.Bmp);
                        image.ImageSize = sr.Length;
                        image.ImageData = sr.ToArray();
                    }
                    image.UserId = UserHelper.GetUserByEmail(User.Identity.Name).Id;
                    image.ImageName = model.ContentData.FileName;

                    if (contentFromStream.Size.Height > VodadModel.Utilities.Constants.BannerParameters.MaxHeight ||
                        contentFromStream.Size.Width > VodadModel.Utilities.Constants.BannerParameters.MaxWidth)
                        return RedirectToAction("AddNewOrder", "Order", new { success = "contentsizefailed" });

                    image.ImageHeight = contentFromStream.Size.Height;
                    image.ImageWidth = contentFromStream.Size.Width;


                    image.CreationDate = DateTime.Now;
                    image.Extension = model.ContentData.ContentType;

                    var imageRepository = new Repository<Image>(Entities);
                    imageRepository.Add(image);
                    imageRepository.Save();

                    Logger.Info(string.Format("Image id = {0} has been added", GetBrandNewImageIdByUserId((int)image.UserId)));

                    brandNewContentId = (int)GetBrandNewImageIdByUserId(userId);
                    orderContent.ContentType = VodadModel.Utilities.Constants.ContentType.Image;
                }
                else if (model.ContentData.ContentType.Equals("video/mp4") ||
                    model.ContentData.ContentType.Equals("video/mpeg") ||
                    model.ContentData.ContentType.Equals("video/quicktime") ||
                    model.ContentData.ContentType.Equals("wmv") ||
                    model.ContentData.ContentType.Equals("video/x-ms-wmv"))
                {
                    var video = new Video();

                    var serverPath = Server.MapPath("~/files/" + string.Format(@"{0}", Guid.NewGuid()) + Path.GetExtension(model.ContentData.FileName));
                    model.ContentData.SaveAs(serverPath);
                    video.VideoSize = model.ContentData.ContentLength;
                    video.VideoLink = serverPath;
                    video.UserId = UserHelper.GetUserByEmail(User.Identity.Name).Id;
                    video.VideoName = model.ContentData.FileName;
                    video.CreationDate = DateTime.Now;
                    video.Extension = model.ContentData.ContentType;

                    var videoRepository = new Repository<Video>(Entities);
                    videoRepository.Add(video);
                    videoRepository.Save();

                    Logger.Info(string.Format("Video id = {0} has been added", GetBrandNewImageIdByUserId((int)video.UserId)));

                    brandNewContentId = (int)GetBrandNewVideoIdByUserId(userId);
                    orderContent.ContentType = VodadModel.Utilities.Constants.ContentType.Video;
                }
                else
                {
                    return RedirectToAction("AddNewOrder", "Order", new { success = "badcontentformat" });
                }

                // Добавляем заказ
                order.UserId = userId;
                order.Comment = model.OrderComment;
                order.CreationDate = DateTime.Now;
                order.RegionId = model.RegionId;

                if (order.ExpireDate > DateTime.Today)
                {
                    try
                    {
                        order.ExpireDate = DateTime.Parse(model.OrderExpireDate, DateTimeFormatInfo.InvariantInfo);
                    }
                    catch (FormatException)
                    {
                        throw new FormatException("Cannot parse the date");
                    }
                }
                else
                {
                    order.ExpireDate = DateTime.Today;
                }

                order.Status = VodadModel.Utilities.Constants.OrdersStatuses.Open;

                orderRepository.Add(order);
                orderRepository.Save();

                var brandNewOrderId = GetBrandNewOrderIdByUserId(userId);

                Logger.Info(string.Format("Order id = {0} has been added", brandNewOrderId));

                Repository<OrderThemes> orderThemesRepository = new Repository<OrderThemes>(Entities);

                bool b = false;

                if (model.SelectAllThemes)
                {
                    b = true;
                    foreach (var t in model.OrderThemesList)
                    {
                        var orderThemes = new OrderThemes();

                        orderThemes.OrderId = brandNewOrderId;
                        orderThemes.ThemeId = t.Id;

                        orderThemesRepository.Add(orderThemes);
                    }

                    orderThemesRepository.Save();

                    Logger.Info(string.Format("For order id = {0} has been added themes list", brandNewOrderId));
                }
                else
                {
                    foreach (var t in model.OrderThemesList)
                    {
                        if (t.IsChecked)
                        {
                            b = true;
                            var orderThemes = new OrderThemes();

                            orderThemes.OrderId = brandNewOrderId;
                            orderThemes.ThemeId = t.Id;

                            orderThemesRepository.Add(orderThemes);
                            orderThemesRepository.Save();

                            Logger.Info(string.Format("For campaign id = {0} has been added themes list", brandNewOrderId));
                        }
                    }
                }

                if (!b)
                {
                    foreach (var t in model.OrderThemesList)
                    {
                        var orderThemes = new OrderThemes();

                        orderThemes.OrderId = brandNewOrderId;
                        orderThemes.ThemeId = t.Id;

                        orderThemesRepository.Add(orderThemes);
                    }

                    orderThemesRepository.Save();
                }

                // Добавляем связь заказа с изображением
                orderContent.IdOrder = (long)brandNewOrderId;
                orderContent.IdContent = brandNewContentId;
                if (orderRepository.GetSingle(w => w.Id == brandNewOrderId).Status.Equals(VodadModel.Utilities.Constants.OrdersStatuses.Open))
                    orderContent.Status = VodadModel.Utilities.Constants.OrdersStatuses.Open;
                else
                    orderContent.Status = VodadModel.Utilities.Constants.OrdersStatuses.Closed;

                orderContentRepository.Add(orderContent);
                orderContentRepository.Save();

                var brandNewOrderContent = (int)orderContentRepository.GetSingle(w => w.IdOrder == orderContent.IdOrder && w.IdContent == orderContent.IdContent && w.ContentType.Equals(orderContent.ContentType)).Id;

                Logger.Info(string.Format("OrderContent id = {0} has been added", brandNewOrderContent));

                return RedirectToAction("ManageOrders", "Order");
            }
            else if (model.ContentData == null)
                return RedirectToAction("AddNewOrder", "Order", new { success = "contentfailed" });

            return RedirectToAction("Error404", "Error");
        }

        /*[Authorize(Roles = "Advertiser")]
        public ActionResult OpenOrders()
        {
            int userId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;

            ViewBag.OpenOrdersList = GetUsersOpenOrdersList(userId);

            return View();

        }*/

        [Authorize(Roles = "Advertiser")]
        public ActionResult ChangeOrderStatus(string userEmail, int? oid, string status)
        {
            if (IsUsersOrder(oid, User.Identity.Name))
            {
                var orderRepository = new Repository<Order>(Entities);

                var order = orderRepository.GetSingle(w => w.Id == oid);

                if (order != null)
                {
                    if (status.Equals(VodadModel.Utilities.Constants.OrdersStatuses.Open) && (order.CreationDate == order.ExpireDate || order.ExpireDate > DateTime.Today))
                        order.Status = VodadModel.Utilities.Constants.OrdersStatuses.Open;
                    else if (status.Equals(VodadModel.Utilities.Constants.OrdersStatuses.Closed))
                        order.Status = VodadModel.Utilities.Constants.OrdersStatuses.Closed;
                    else if (status.Equals(VodadModel.Utilities.Constants.OrdersStatuses.Deleted))
                    {
                        var orderContentRepository = new Repository<OrderContent>(Entities);

                        var orderContent = orderContentRepository.GetAll(w => w.IdOrder == oid);

                        if (orderContent != null)
                        {
                            var orderPerformedRepository = new Repository<OrderPerformed>(Entities);

                            foreach (var i in orderContent)
                            {
                                if (!orderPerformedRepository.GetAll(w => w.OrderContentId == (int)i.Id).Any())
                                    DeleteOrder(oid, User.Identity.Name);
                                else
                                    foreach (var oc in orderContent)
                                    {
                                        oc.Status = VodadModel.Utilities.Constants.OrdersStatuses.Deleted;
                                        order.Status = VodadModel.Utilities.Constants.OrdersStatuses.Deleted;
                                    }
                            }
                        }
                    }
                }

                orderRepository.Save();

                Logger.Info(string.Format("Order id = {0} status has been changed", oid));
            }

            return RedirectToAction("ManageOrders", "Order");
        }

        [Authorize(Roles = "Advertiser")]
        public ActionResult DeleteContentFromOrder(int? oid, int? cid, string userEmail)
        {
            if (IsUsersOrder(oid, userEmail))
            {
                var orderContentRepository = new Repository<OrderContent>(Entities);
                var imageRepository = new Repository<Image>(Entities);
                var videoRepository = new Repository<Video>(Entities);

                var orderContent = orderContentRepository.GetSingle(w => w.IdContent == cid);
                int? content = null;

                content = (int) orderContent.IdContent;

                if (orderContent != null && content == cid)
                {
                    if (orderContent.ContentType.Equals(VodadModel.Utilities.Constants.ContentType.Image))
                    {
                        var image = imageRepository.GetSingle(w => w.Id == orderContent.IdContent);

                        imageRepository.Delete(image);
                        Logger.Info(string.Format("Image id = {0}  has been deleted", image.Id));
                    }
                    else if (orderContent.ContentType.Equals(VodadModel.Utilities.Constants.ContentType.Video))
                    {
                        var video = videoRepository.GetSingle(w => w.Id == orderContent.IdContent);

                        videoRepository.Delete(video);
                        Logger.Info(string.Format("Video id = {0}  has been deleted", video.Id));
                    }

                    orderContentRepository.Delete(orderContent);
                    Logger.Info(string.Format("OrderContent id = {0} has been deleted", orderContent.Id));
                    orderContentRepository.Save();
                    imageRepository.Save();
                    videoRepository.Save();
                }
            }

            return RedirectToAction("EditOrder", "Order", new { cid, oid });
        }

        private void DeleteOrder(int? oid, string userEmail)
        {
            if (IsUsersOrder(oid, userEmail))
            {
                var orderContentRepository = new Repository<OrderContent>(Entities);
                var orderRepository = new Repository<Order>(Entities);
                var imageRepository = new Repository<Image>(Entities);
                var videoRepository = new Repository<Video>(Entities);

                var orderContent = orderContentRepository.GetAll(w => w.IdOrder == oid);
                var order = orderRepository.GetSingle(w => w.Id == oid);

                if (orderContent.Any())
                {
                    foreach (var oc in orderContent)
                    {
                        if (oc.ContentType.Equals(VodadModel.Utilities.Constants.ContentType.Image))
                        {
                            var image = imageRepository.GetSingle(w => w.Id == oc.IdContent);

                            imageRepository.Delete(image);
                            Logger.Info(string.Format("Image id = {0}  has been deleted", image.Id));
                        }
                        else if (oc.ContentType.Equals(VodadModel.Utilities.Constants.ContentType.Video))
                        {
                            var video = videoRepository.GetSingle(w => w.Id == oc.IdContent);

                            videoRepository.Delete(video);
                            Logger.Info(string.Format("Video id = {0}  has been deleted", video.Id));
                        }
                        orderContentRepository.Delete(oc);
                        Logger.Info(string.Format("OrderContent id = {0}  has been deleted", oc.Id));
                    }
                }

                orderRepository.Delete(order);
                Logger.Info(string.Format("Order id = {0} has been deleted", order.Id));
                orderRepository.Save();
                orderContentRepository.Save();
                imageRepository.Save();
                videoRepository.Save();
            }
        }

        [Authorize(Roles = "Advertiser")]
        public ActionResult EditOrder(int? oid, string success)
        {
            if (success != null)
            {
                if (success.Equals("badcontentformat"))
                {
                    ViewBag.AlertMessage = "Only bmp, gif, jpeg, jpg or png images and mp4, mpeg, wmv videos.";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
            }

            if (oid != null && IsUsersOrder(oid, User.Identity.Name))
            {
                var orderRepository = new Repository<Order>(Entities);
                var orderContentRepository = new Repository<OrderContent>(Entities);
                var imageRepository = new Repository<Image>(Entities);
                var videoRepository = new Repository<Video>(Entities);

                var order = orderRepository.GetSingle(w => w.Id == oid);
                var orderContentList = orderContentRepository.GetAll(w => w.IdOrder == oid);
                var imagesList = new List<Image>();
                var videosList = new List<Video>();

                foreach (var o in orderContentList)
                {
                    if (o.ContentType.Equals(VodadModel.Utilities.Constants.ContentType.Image))
                    {
                        imagesList.Add(imageRepository.GetSingle(w => w.Id == o.IdContent));
                    }
                    else if (o.ContentType.Equals(VodadModel.Utilities.Constants.ContentType.Video))
                    {
                        videosList.Add(videoRepository.GetSingle(w => w.Id == o.IdContent));
                    }
                }

                var orderEditModel = new OrderEditModel();
                orderEditModel.OrderId = (int?)order.Id;
                orderEditModel.OrderComment = order.Comment;
                orderEditModel.OrderExpireDate = order.ExpireDate.Value.ToString("MM/dd/yyyy", DateTimeFormatInfo.InvariantInfo);
                orderEditModel.ImagesList = imagesList;
                orderEditModel.VideosList = videosList;
                ViewBag.RegionsList = GetRegionsList();
                ViewBag.RegionId = order.RegionId;

                return View(orderEditModel);
            }
            else
                return RedirectToAction("ManageOrders", "Order");
        }

        [HttpPost]
        [Authorize(Roles = "Advertiser")]
        public ActionResult EditOrder(OrderEditModel model)
        {
            if (ModelState.IsValid && IsUsersOrder(model.OrderId, User.Identity.Name) && model.OrderId != null)
            {
                int userId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;

                var orderRepository = new Repository<Order>(Entities);

                var order = orderRepository.GetSingle(w => w.Id == model.OrderId);

                if (order.ExpireDate > DateTime.Today)
                {
                    try
                    {
                        order.ExpireDate = DateTime.Parse(model.OrderExpireDate, new CultureInfo("en-US", false));
                    }
                    catch (FormatException)
                    {
                        throw new FormatException("Cannot parse the date");
                    }
                }
                else
                {
                    order.ExpireDate = order.CreationDate;
                }

                order.Comment = model.OrderComment;
                order.RegionId = model.RegionId;

                orderRepository.Save();

                Logger.Info(string.Format("Order id = {0} has been modified", order.Id));

                if (model.ContentData != null)
                {
                    var orderContentRepository = new Repository<OrderContent>(Entities);

                    var orderContent = new OrderContent();
                    int brandNewContentId = 0;

                    if (model.ContentData.ContentType.Equals("image/gif") ||
                        model.ContentData.ContentType.Equals("image/png") ||
                        model.ContentData.ContentType.Equals("image/jpeg"))
                    {
                        // Получить и сохранить контент
                        var contentFromStream = System.Drawing.Image.FromStream(model.ContentData.InputStream, true, true);
                        var buffer = new System.Drawing.Bitmap(contentFromStream.Size.Width, contentFromStream.Size.Height,
                                                               PixelFormat.Format24bppRgb);

                        var image = new Image();

                        using (var gr = System.Drawing.Graphics.FromImage(buffer))
                        {
                            gr.DrawImage(contentFromStream, 0, 0);
                        }
                        using (var sr = new MemoryStream())
                        {
                            buffer.Save(sr, ImageFormat.Bmp);
                            image.ImageSize = sr.Length;
                            image.ImageData = sr.ToArray();
                        }
                        image.UserId = UserHelper.GetUserByEmail(User.Identity.Name).Id;
                        image.ImageName = model.ContentData.FileName;

                        if (contentFromStream.Size.Height > VodadModel.Utilities.Constants.BannerParameters.MaxHeight ||
                            contentFromStream.Size.Width > VodadModel.Utilities.Constants.BannerParameters.MaxWidth)
                            return RedirectToAction("AddNewOrder", "Order", new { success = "contentsizefailed" });

                        image.ImageHeight = contentFromStream.Size.Height;
                        image.ImageWidth = contentFromStream.Size.Width;


                        image.CreationDate = DateTime.Now;
                        image.Extension = model.ContentData.ContentType;

                        var imageRepository = new Repository<Image>(Entities);
                        imageRepository.Add(image);
                        imageRepository.Save();

                        Logger.Info(string.Format("Image id = {0} has been added", GetBrandNewImageIdByUserId((int)image.UserId)));

                        brandNewContentId = (int)GetBrandNewImageIdByUserId(userId);
                        orderContent.ContentType = VodadModel.Utilities.Constants.ContentType.Image;
                    }
                    else if (model.ContentData.ContentType.Equals("video/mp4") ||
                        model.ContentData.ContentType.Equals("video/mpeg") ||
                        model.ContentData.ContentType.Equals("video/quicktime") ||
                        model.ContentData.ContentType.Equals("wmv") ||
                        model.ContentData.ContentType.Equals("video/x-ms-wmv"))
                    {
                        var video = new Video();

                        var serverPath = Server.MapPath("~/files/" + string.Format(@"{0}", Guid.NewGuid()) + Path.GetExtension(model.ContentData.FileName));
                        model.ContentData.SaveAs(serverPath);
                        video.VideoSize = model.ContentData.ContentLength;
                        video.VideoLink = serverPath;
                        video.UserId = UserHelper.GetUserByEmail(User.Identity.Name).Id;
                        video.VideoName = model.ContentData.FileName;
                        video.CreationDate = DateTime.Now;
                        video.Extension = model.ContentData.ContentType;

                        var videoRepository = new Repository<Video>(Entities);
                        videoRepository.Add(video);
                        videoRepository.Save();

                        Logger.Info(string.Format("Video id = {0} has been added", GetBrandNewImageIdByUserId((int)video.UserId)));

                        brandNewContentId = (int)GetBrandNewVideoIdByUserId(userId);
                        orderContent.ContentType = VodadModel.Utilities.Constants.ContentType.Video;
                    }
                    else
                    {
                        return RedirectToAction("EditOrder", "Order", new { oid = model.OrderId, success = "badcontentformat" });
                    }

                    orderContent.IdOrder = (long)model.OrderId;
                    orderContent.IdContent = brandNewContentId;
                    orderContent.Status = VodadModel.Utilities.Constants.OrdersStatuses.Open;

                    var previousOrderContent = orderContentRepository.GetSingle(w => w.IdOrder == model.OrderId);

                    if (previousOrderContent != null)
                    {
                        orderContentRepository.Delete(previousOrderContent);
                    }

                    orderContentRepository.Add(orderContent);
                    orderContentRepository.Save();

                    var brandNewOrderContent = (int)orderContentRepository.GetSingle(w => w.IdOrder == orderContent.IdOrder && w.IdContent == orderContent.IdContent).Id;

                    Logger.Info(string.Format("OrderContent id = {0} has been added", brandNewOrderContent));
                }
            }

            return RedirectToAction("ManageOrders", "Order");
        }

        public int? GetBrandNewImageIdByUserId(int? userId)
        {
            var imageRepository = new Repository<Image>(Entities);

            var brandNewImage = imageRepository.GetAll(w => w.UserId == userId).ToList().Last();

            return (int?)brandNewImage.Id;
        }

        public int? GetBrandNewOrderIdByUserId(int? userId)
        {
            var orderRepository = new Repository<Order>(Entities);

            try
            {
                var brandNewOrder =
                    orderRepository.GetAll(
                        w =>
                            w.UserId == userId &&
                            !w.Status.Equals(VodadModel.Utilities.Constants.OrdersStatuses.Deleted)).ToList().Last();

                return (int?)brandNewOrder.Id;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public int? GetBrandNewVideoIdByUserId(int? userId)
        {
            var videoRepository = new Repository<Video>(Entities);

            var brandNewVideo = videoRepository.GetAll(w => w.UserId == userId).ToList().Last();

            return (int?)brandNewVideo.Id;
        }

        /*private Campaign GetCampaignById(int? campaignId)
        {
            var campaignRepository = new Repository<Campaign>(Entities);

            return campaignRepository.GetSingle(w => w.Id == campaignId);
        }

        private string GetCampaignName(int? campaignId)
        {
            var campaignRepository = new Repository<Campaign>(Entities);

            var campaign = campaignRepository.GetSingle(w => w.Id == campaignId);

            return campaign.Name;
        }*/

        /*private List<OrderModel> GetUsersOpenOrdersList(int? userId)
        {
            var orderRepository = new Repository<Order>(Entities);

            var orders = orderRepository.GetAll(w => w.UserId == userId && w.Status.Equals("open")).ToList();

            var orderModelList = new List<OrderModel>();

            foreach (var o in orders)
            {
                var newOrderModel = new OrderModel();

                newOrderModel.OrderExpireDate = o.ExpireDate.Value.ToString("MM/dd/yyyy");
                newOrderModel.OrderComment = o.Comment;
                newOrderModel.OrderId = (int)o.Id;

                orderModelList.Add(newOrderModel);
            }

            return orderModelList;
        }*/

        private List<Regions> GetRegionsList()
        {
            var regionRepository = new Repository<Regions>(Entities);
            return regionRepository.GetAll().ToList();
        }

        /*private bool IsUsersCampaign(int? campaignId)
        {
            var campaignRepository = new Repository<Campaign>(Entities);

            var user = UserHelper.GetUserByEmail(User.Identity.Name);

            var campaign = campaignRepository.GetSingle(w => w.Id == campaignId);

            if (user.Id == campaign.UserId)
                return true;
            else
                return false;
        }*/

        public bool IsUsersOrder(int? orderId, string userEmail)
        {
            if (orderId != null)
            {
                var orderRepository = new Repository<Order>(Entities);

                var userId = UserHelper.GetUserByEmail(userEmail).Id;

                var order = orderRepository.GetSingle(w => w.Id == orderId);

                if (userId == order.UserId)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        /*private bool IsCampaignsOrder(int? orderId, int? campaignId)
        {
            var orderRepository = new Repository<Order>(Entities);

            var order = orderRepository.GetSingle(w => w.Id == orderId);

            if (order.CampaignId == campaignId)
                return true;
            else
                return false;
        }*/

        [Authorize(Roles = "Advertiser")]
        public ActionResult ManageOrders(string status)
        {
            var orderRepository = new Repository<Order>(Entities);
            var imageRepository = new Repository<Image>(Entities);
            var videoRepository = new Repository<Video>(Entities);

            var userId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;

            // Получаем все необходимые заказы/картинки/картинки заказов для формирования отчета
            var order = orderRepository.GetAll(w => w.UserId == userId && !w.Status.Equals(VodadModel.Utilities.Constants.OrdersStatuses.Deleted));
            if (status != null)
                order = orderRepository.GetAll(w => w.UserId == userId && w.Status.Equals(status) && !w.Status.Equals(VodadModel.Utilities.Constants.OrdersStatuses.Deleted));

            var orderContentList = new List<OrderContent>();
            var imagesList = new List<Image>();
            var videosList = new List<Video>();

            foreach (var o in order)
            {
                var orderContent = o.OrderContent.ToList();
                foreach (var oc in orderContent)
                {
                    orderContentList.Add(oc);

                    if (oc.ContentType.Equals(VodadModel.Utilities.Constants.ContentType.Image))
                    {
                        var image = imageRepository.GetSingle(w => w.Id == oc.IdContent);

                        imagesList.Add(image);
                    }
                    else if (oc.ContentType.Equals(VodadModel.Utilities.Constants.ContentType.Video))
                    {
                        var video = videoRepository.GetSingle(w => w.Id == oc.IdContent);

                        videosList.Add(video);
                    }
                }
            }

            // Формируем отчет
            var orderList = new List<OrderListModel>();

            /*try
            {*/
            foreach (var o in order)
            {
                var orderListModel = new OrderListModel();
                orderListModel.OrderId = (int?)o.Id;
                orderListModel.OrderName = o.Name;
                orderListModel.OrderComment = o.Comment;
                orderListModel.OrderRegion = o.Regions.RegionName;
                orderListModel.OrderCreationDate = o.CreationDate.Value.ToString("MM/dd/yyyy");
                orderListModel.OrderExpireDate = o.ExpireDate.Value.ToString("MM/dd/yyyy");
                if (o.Status.Equals(VodadModel.Utilities.Constants.OrdersStatuses.Open))
                    orderListModel.OrderStatus = "Open";
                else if (o.Status.Equals(VodadModel.Utilities.Constants.OrdersStatuses.Closed))
                    orderListModel.OrderStatus = "Closed";
                orderListModel.OrderImagesList = imagesList;
                orderListModel.OrderVideosList = videosList;
                orderList.Add(orderListModel);

                // Извлечение списка допустимых тематик
                var orderThemesList = o.OrderThemes.ToList();

                string themesListForView = "";

                foreach (var p in orderThemesList)
                    themesListForView += p.Themes.Name + ", ";

                orderListModel.OrderThemes = themesListForView.Remove(themesListForView.Length - 2);
            }
            /*}
            catch
            {
                return RedirectToAction("ManageOrders", "Order", new { status });
            }*/

            ViewBag.OrdersList = orderList;

            ViewBag.AlertMessage = "There must be enough money to open order";
            ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;

            return View();

        }

        [HttpPost]
        [Authorize(Roles = "Advertiser")]
        public ActionResult ManageOrders(string submit, ICollection<string> checkme)
        {
            if (checkme.Count > 1)
            {
                try
                {
                    if (submit.Equals("Open"))
                    {
                        foreach (var c in checkme)
                        {
                            if (!c.Equals("false"))
                                ChangeOrderStatus(User.Identity.Name, int.Parse(c), VodadModel.Utilities.Constants.OrdersStatuses.Open);
                        }
                    }
                    else
                        if (submit.Equals("Close"))
                        {
                            foreach (var c in checkme)
                            {
                                if (!c.Equals("false"))
                                    ChangeOrderStatus(User.Identity.Name, int.Parse(c), VodadModel.Utilities.Constants.OrdersStatuses.Closed);
                            }
                        }
                        else
                            if (submit.Equals("Delete"))
                            {
                                foreach (var c in checkme)
                                {
                                    if (!c.Equals("false"))
                                        ChangeOrderStatus(User.Identity.Name, int.Parse(c), VodadModel.Utilities.Constants.OrdersStatuses.Deleted);
                                }
                            }
                }
                catch { }
            }

            return RedirectToAction("ManageOrders", "Order");
        }

        public ActionResult RequestOrderToAdvertiser(int? ppid, int? ocid)
        {
            if (ocid == null && ppid != null)
                return RedirectToAction("SearchForAdvertiser", "Search", new { ppid });
            else
                if (ocid != null && ppid == null)
                    return RedirectToAction("Index", "Home");
                else
                    if (ocid != null && ppid != null)
                        return RedirectToAction("SendOrderToAdvertiser", "OrderPerformed", new { ppid, ocid });

            return RedirectToAction("Index", "Home");
        }

        public ActionResult RequestOrderToPerformer(int? ppid, int? oid)
        {
            if (oid == null && ppid != null)
                return RedirectToAction("ManageOrders", "Order", new { status = VodadModel.Utilities.Constants.OrdersStatuses.Open });
            else
                if (oid != null && ppid == null)
                    return RedirectToAction("SearchForPerformer", "Search", new { oid });
                else
                    if (oid != null && ppid != null)
                        return RedirectToAction("SendOrderToPerformer", "OrderPerformed", new { ppid, oid });

            return RedirectToAction("Index", "Home");
        }

        /*public static string UploadVideo(string FilePath, string Title, string Description)
        {
            YouTubeRequestSettings settings;
            YouTubeRequest request;
            string devkey = VodadModel.Utilities.Constants.YouTube.Devkey;
            string username = VodadModel.Utilities.Constants.YouTube.Username;
            string password = VodadModel.Utilities.Constants.YouTube.Password;
            settings = new YouTubeRequestSettings(VodadModel.Utilities.Constants.YouTube.AppName, devkey, username, password) { Timeout = -1 };
            request = new YouTubeRequest(settings);

            Google.YouTube.Video newVideo = new Google.YouTube.Video();

            newVideo.Title = Title;
            newVideo.Description = Description;
            newVideo.Private = true;
            newVideo.YouTubeEntry.Private = false;

            //newVideo.Tags.Add(new MediaCategory("Autos", YouTubeNameTable.CategorySchema));

            //newVideo.Tags.Add(new MediaCategory("mydevtag, anotherdevtag", YouTubeNameTable.DeveloperTagSchema));

            //newVideo.YouTubeEntry.setYouTubeExtension("location", "Paris, FR");
            // You can also specify just a descriptive string ==>
            // newVideo.YouTubeEntry.Location = new GeoRssWhere(71, -111);
            // newVideo.YouTubeEntry.setYouTubeExtension("location", "Paris, France.");

            newVideo.YouTubeEntry.MediaSource = new MediaFileSource(FilePath, "video/mp4");
            Google.YouTube.Video createdVideo = request.Upload(newVideo);

            return createdVideo.VideoId;
        }

        public static bool DeleteVideo(string VideoId)
        {
            try
            {
                YouTubeRequestSettings settings;
                YouTubeRequest request;
                string devkey = VodadModel.Utilities.Constants.YouTube.Devkey;
                string username = VodadModel.Utilities.Constants.YouTube.Username;
                string password = VodadModel.Utilities.Constants.YouTube.Password;
                settings = new YouTubeRequestSettings(VodadModel.Utilities.Constants.YouTube.AppName, devkey, username, password) { Timeout = -1 };
                request = new YouTubeRequest(settings);

                Uri videoEntryUrl = new Uri(String.Format("http://gdata.youtube.com/feeds/api/users/{0}/uploads/{1}", username, VideoId));
                Google.YouTube.Video video = request.Retrieve<Google.YouTube.Video>(videoEntryUrl);
                request.Delete(video);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }*/
    }
}