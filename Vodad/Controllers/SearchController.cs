﻿using System.Globalization;
﻿using VodadModel;
using VodadModel.Helpers;
using VodadModel.Repository;
using Vodad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vodad.Controllers
{
    public class SearchController : BaseController
    {
        public ActionResult AdvertiserSearchResult(
                    bool allthemes,
                    bool games,
                    bool animals,
                    bool sport,
                    bool news,
                    bool politics,
                    bool show,
                    bool cinema,
                    int? minWidth,
                    int? minHeight,
                    int? maxWidth,
                    int? maxHeight,
                    decimal? price,
                    decimal? budget,
                    int? rows,
                    int? bmid,
                    bool sss,
                    int userId
            )
        {
            if (rows == null)
                rows = 25;
            ViewBag.RowsCount = rows;

            var model = new AdvertiserSearchModel();
            model.SelectAllThemes = allthemes;

            List<ThemesListForOrderModel> themesListForOrder = GetThemesListForOrder();

            themesListForOrder[0].IsChecked = games;
            themesListForOrder[1].IsChecked = animals;
            themesListForOrder[2].IsChecked = sport;
            themesListForOrder[3].IsChecked = news;
            themesListForOrder[4].IsChecked = politics;
            themesListForOrder[5].IsChecked = show;
            themesListForOrder[6].IsChecked = cinema;

            model.OrderThemesList = themesListForOrder;
            model.SaveSearchSettings = sss;

            var orderContentRepository = new Repository<OrderContent>(Entities);

            var orderContentList = new List<OrderContent>();

            if (model.SelectAllThemes)
            {

                var result = orderContentRepository.GetAll(w => w.Order.Status.Equals(VodadModel.Utilities.Constants.OrdersStatuses.Open)).ToList();
                if (result != null)
                    orderContentList.AddRange(result);
            }
            else
            {
                foreach (var t in model.OrderThemesList)
                {

                    var result = orderContentRepository.GetAll(w => w.Order.Status.Equals(VodadModel.Utilities.Constants.OrdersStatuses.Open) &&
                                            w.Order.OrderThemes.FirstOrDefault(v => v.ThemeId == t.Id).ThemeId == t.Id).ToList();
                    if (result != null)
                        orderContentList.AddRange(result);
                }
            }

            var advertiserSearchResultsModelList = new List<AdvertiserSearchResultsModel>();

            foreach (var s in orderContentList)
            {
                var advertiserSearchResultElement = new AdvertiserSearchResultsModel();

                advertiserSearchResultElement.AdvertiserName = UserHelper.GetUserNameByEmail(s.Order.User.Email);
                advertiserSearchResultElement.AdvertiserId = (int?)s.Order.User.Id;
                advertiserSearchResultElement.ContentId = (int?)s.Id;
                advertiserSearchResultElement.Region = s.Order.Regions.RegionName;

                if (s.ContentType.Equals(VodadModel.Utilities.Constants.ContentType.Image))
                {
                    Repository<Image> imageRepository = new Repository<Image>(Entities);

                    var image = imageRepository.GetSingle(w => w.Id == s.Id);

                    advertiserSearchResultElement.ImageData = image.ImageData;
                    advertiserSearchResultElement.Size = image.ImageWidth + "x" + image.ImageHeight + "px";
                }
                else if (s.ContentType.Equals(VodadModel.Utilities.Constants.ContentType.Video))
                {
                    Repository<Video> videoRepository = new Repository<Video>(Entities);

                    var video = videoRepository.GetSingle(w => w.Id == s.Id);

                    advertiserSearchResultElement.VideoLink = video.VideoLink;
                    advertiserSearchResultElement.Size = video.VideoSize.ToString(CultureInfo.InvariantCulture);
                }

                advertiserSearchResultElement.ContentId = (int?)s.IdContent;

                var blackAndWhiteListsController = new BlackAndWhiteListsController();
                if (!blackAndWhiteListsController.AreUsersInBlackLists((int)s.Order.User.Id, userId))
                    advertiserSearchResultsModelList.Add(advertiserSearchResultElement);
            }

            // Save advertiser search settings to cookies
            if (sss)
            {
                HttpCookie advertiserSearchSettingCookie = new HttpCookie("AdvertiserSearch");

                advertiserSearchSettingCookie["AllThemes"] = allthemes.ToString();

                advertiserSearchSettingCookie["Games"] = games.ToString();
                advertiserSearchSettingCookie["Animals"] = animals.ToString();
                advertiserSearchSettingCookie["Sport"] = sport.ToString();
                advertiserSearchSettingCookie["News"] = news.ToString();
                advertiserSearchSettingCookie["Politics"] = politics.ToString();
                advertiserSearchSettingCookie["Show"] = show.ToString();
                advertiserSearchSettingCookie["Cinema"] = cinema.ToString();

                advertiserSearchSettingCookie.Expires = DateTime.Now.AddDays(30);
                advertiserSearchSettingCookie.Secure = false;

                Response.Cookies.Add(advertiserSearchSettingCookie);
            }

            return View("AdvertiserSearchResult", advertiserSearchResultsModelList);
        }

        public ActionResult AdvertiserSearchResultByName(int? uid)
        {
            if (uid != null)
            {
                var userRepository = new Repository<User>(Entities);
                var orderContentRepository = new Repository<OrderContent>(Entities);

                var user = userRepository.GetSingle(w => w.Id == uid);

                if ((user != null) && user.Roles.RoleName.Equals(VodadModel.Utilities.Constants.UserRoles.Advertiser))
                {
                    var orderContentList = new List<OrderContent>();

                    var result =
                        orderContentRepository.GetAll(
                            w => w.Order.Status.Equals(VodadModel.Utilities.Constants.OrdersStatuses.Open) && w.Order.User.Id == uid).ToList();

                    if (result != null)
                        orderContentList.AddRange(result);

                    var advertiserSearchResultsModelList = new List<AdvertiserSearchResultsModel>();

                    foreach (var s in orderContentList)
                    {
                        var advertiserSearchResultElement = new AdvertiserSearchResultsModel();

                        advertiserSearchResultElement.AdvertiserName =
                            UserHelper.GetUserNameByEmail(s.Order.User.Email);
                        advertiserSearchResultElement.AdvertiserId = (int?)s.Order.User.Id;
                        advertiserSearchResultElement.ContentId = (int?)s.IdContent;

                        if (s.ContentType.Equals(VodadModel.Utilities.Constants.ContentType.Image))
                        {
                            Repository<Image> imageRepository = new Repository<Image>(Entities);

                            var image = imageRepository.GetSingle(w => w.Id == s.Id);

                            advertiserSearchResultElement.ImageData = image.ImageData;
                            advertiserSearchResultElement.Size = image.ImageWidth + "x" + image.ImageHeight + "px";
                        }
                        else if (s.ContentType.Equals(VodadModel.Utilities.Constants.ContentType.Video))
                        {
                            Repository<Video> videoRepository = new Repository<Video>(Entities);

                            var video = videoRepository.GetSingle(w => w.Id == s.Id);

                            advertiserSearchResultElement.VideoLink = video.VideoLink;
                            advertiserSearchResultElement.Size = video.VideoSize.ToString(CultureInfo.InvariantCulture);
                        }

                        advertiserSearchResultsModelList.Add(advertiserSearchResultElement);
                    }

                    return View("AdvertiserSearchResult", advertiserSearchResultsModelList);
                }
                else
                    return RedirectToAction("Error404", "Error");
            }
            else
                return RedirectToAction("Error404", "Error");
        }

        [Authorize]
        public ActionResult SearchForAdvertiser(int? ppid)
        {
            var performerPlatformRepository = new Repository<PerformerPlatform>(Entities);
            var performerPlatform = performerPlatformRepository.GetSingle(w => w.Id == ppid);

            var advertiserSearchModel = new AdvertiserSearchModel();

            advertiserSearchModel.OrderThemesList = GetThemesListForOrder();

            if (performerPlatform != null)
            {
                advertiserSearchModel.OrderThemesList.FirstOrDefault(w => w.Id == performerPlatform.ThemeId).IsChecked = true;
            }
            else
            {
                HttpCookie advertiserSearchSettingCookie = Request.Cookies.Get("AdvertiserSearch");

                if (advertiserSearchSettingCookie != null)
                {
                    try
                    {
                        advertiserSearchModel.SelectAllThemes = bool.Parse(advertiserSearchSettingCookie["AllThemes"]);
                    }
                    catch
                    {
                        advertiserSearchSettingCookie["AllThemes"] = null;
                    }

                    List<ThemesListForOrderModel> themesListForCampaign = GetThemesListForOrder();

                    try
                    {
                        themesListForCampaign[0].IsChecked = bool.Parse(advertiserSearchSettingCookie["Games"]);
                    }
                    catch
                    {
                        advertiserSearchSettingCookie["Games"] = null;
                    }

                    try
                    {
                        themesListForCampaign[1].IsChecked = bool.Parse(advertiserSearchSettingCookie["Animals"]);
                    }
                    catch
                    {
                        advertiserSearchSettingCookie["Animals"] = null;
                    }

                    try
                    {
                        themesListForCampaign[2].IsChecked = bool.Parse(advertiserSearchSettingCookie["Sport"]);
                    }
                    catch
                    {
                        advertiserSearchSettingCookie["Sport"] = null;
                    }

                    try
                    {
                        themesListForCampaign[3].IsChecked = bool.Parse(advertiserSearchSettingCookie["News"]);
                    }
                    catch
                    {
                        advertiserSearchSettingCookie["News"] = null;
                    }

                    try
                    {
                        themesListForCampaign[4].IsChecked = bool.Parse(advertiserSearchSettingCookie["Politics"]);
                    }
                    catch
                    {
                        advertiserSearchSettingCookie["Politics"] = null;
                    }

                    try
                    {
                        themesListForCampaign[5].IsChecked = bool.Parse(advertiserSearchSettingCookie["Show"]);
                    }
                    catch
                    {
                        advertiserSearchSettingCookie["Show"] = null;
                    }

                    try
                    {
                        themesListForCampaign[6].IsChecked = bool.Parse(advertiserSearchSettingCookie["Cinema"]);
                    }
                    catch
                    {
                        advertiserSearchSettingCookie["Cinema"] = null;
                    }

                    advertiserSearchModel.OrderThemesList = themesListForCampaign;
                }
            }

            ViewBag.PerformerPlatformId = ppid;


            return View(advertiserSearchModel);
        }

        [HttpPost]
        [Authorize]
        public ActionResult SearchForAdvertiser(AdvertiserSearchModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;

                return RedirectToAction("AdvertiserSearchResult", new
                {
                    allthemes = model.SelectAllThemes,
                    games = model.OrderThemesList[0].IsChecked,
                    animals = model.OrderThemesList[1].IsChecked,
                    sport = model.OrderThemesList[2].IsChecked,
                    news = model.OrderThemesList[3].IsChecked,
                    politics = model.OrderThemesList[4].IsChecked,
                    show = model.OrderThemesList[5].IsChecked,
                    cinema = model.OrderThemesList[6].IsChecked,
                    sss = model.SaveSearchSettings,
                    userId
                });
            }
            else
            {
                var performerPlatformRepository = new Repository<PerformerPlatform>(Entities);
                var performerPlatform = performerPlatformRepository.GetSingle(w => w.Id == model.PerformerPlatformId);

                var advertiserSearchModel = new AdvertiserSearchModel();

                advertiserSearchModel.OrderThemesList = GetThemesListForOrder();

                if (performerPlatform != null)
                {
                    advertiserSearchModel.OrderThemesList.FirstOrDefault(w => w.Id == performerPlatform.ThemeId).IsChecked = true;
                }

                ViewBag.PerformerPlatformId = model.PerformerPlatformId;

                return View(advertiserSearchModel);
            }
        }

        [Authorize]
        public ActionResult SearchForPerformer(int? oid, string success)
        {
            if (success != null)
            {
                if (success.Equals("smallbudget"))
                {
                    ViewBag.AlertMessage = "Max campaign's price is less than ad's platform price";
                    ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                }
            }

            var performerSearchModel = new PerformerSearchModel();

            performerSearchModel.OrderThemesList = GetThemesListForOrder();
            performerSearchModel.MinAverageViewersPerHour = 0;
            performerSearchModel.TotalUniqueViews = 0;
            performerSearchModel.TotalFollowers = 0;

            HttpCookie performerSearchSettingCookie = Request.Cookies.Get("PerformerSearch");

            if (performerSearchSettingCookie != null)
            {
                try
                {
                    performerSearchModel.MinAverageViewersPerHour = Int32.Parse(performerSearchSettingCookie["mavph"]);
                }
                catch
                {
                    performerSearchSettingCookie["mavph"] = null;
                }

                try
                {
                    performerSearchModel.TotalUniqueViews = Int32.Parse(performerSearchSettingCookie["TotalUniqueViews"]);
                }
                catch
                {
                    performerSearchSettingCookie["TotalUniqueViews"] = null;
                }

                try
                {
                    performerSearchModel.TotalFollowers = Decimal.Parse(performerSearchSettingCookie["TotalFollowers"]);
                }
                catch
                {
                    performerSearchSettingCookie["TotalFollowers"] = null;
                }

                try
                {
                    performerSearchModel.SelectAllThemes = bool.Parse(performerSearchSettingCookie["AllThemes"]);
                }
                catch
                {
                    performerSearchSettingCookie["AllThemes"] = null;
                }

                List<ThemesListForOrderModel> themesListForCampaign = GetThemesListForOrder();

                try
                {
                    themesListForCampaign[0].IsChecked = bool.Parse(performerSearchSettingCookie["Games"]);
                }
                catch
                {
                    performerSearchSettingCookie["Games"] = null;
                }

                try
                {
                    themesListForCampaign[1].IsChecked = bool.Parse(performerSearchSettingCookie["Animals"]);
                }
                catch
                {
                    performerSearchSettingCookie["Animals"] = null;
                }

                try
                {
                    themesListForCampaign[2].IsChecked = bool.Parse(performerSearchSettingCookie["Sport"]);
                }
                catch
                {
                    performerSearchSettingCookie["Sport"] = null;
                }

                try
                {
                    themesListForCampaign[3].IsChecked = bool.Parse(performerSearchSettingCookie["News"]);
                }
                catch
                {
                    performerSearchSettingCookie["News"] = null;
                }

                try
                {
                    themesListForCampaign[4].IsChecked = bool.Parse(performerSearchSettingCookie["Politics"]);
                }
                catch
                {
                    performerSearchSettingCookie["Politics"] = null;
                }

                try
                {
                    themesListForCampaign[5].IsChecked = bool.Parse(performerSearchSettingCookie["Show"]);
                }
                catch
                {
                    performerSearchSettingCookie["Show"] = null;
                }

                try
                {
                    themesListForCampaign[6].IsChecked = bool.Parse(performerSearchSettingCookie["Cinema"]);
                }
                catch
                {
                    performerSearchSettingCookie["Cinema"] = null;
                }

                performerSearchModel.OrderThemesList = themesListForCampaign;

                try
                {
                    performerSearchModel.OrderId = Int32.Parse(performerSearchSettingCookie["Oid"]);
                }
                catch
                {
                    performerSearchSettingCookie["Oid"] = null;
                }
            }

            ViewBag.OrderId = oid;

            return View(performerSearchModel);
        }

        [HttpPost]
        [Authorize]
        public ActionResult SearchForPerformer(PerformerSearchModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = (int)UserHelper.GetUserByEmail(User.Identity.Name).Id;

                return RedirectToAction("PerformerSearchResult", new
                {
                    allthemes = model.SelectAllThemes,
                    games = model.OrderThemesList[0].IsChecked,
                    animals = model.OrderThemesList[1].IsChecked,
                    sport = model.OrderThemesList[2].IsChecked,
                    news = model.OrderThemesList[3].IsChecked,
                    politics = model.OrderThemesList[4].IsChecked,
                    show = model.OrderThemesList[5].IsChecked,
                    cinema = model.OrderThemesList[6].IsChecked,
                    mavph = model.MinAverageViewersPerHour,
                    tuv = model.TotalUniqueViews,
                    tf = model.TotalFollowers,
                    oid = model.OrderId,
                    sss = model.SaveSearchSettings,
                    userId
                });
            }
            else
            {
                var performerSearchModel = new PerformerSearchModel();

                performerSearchModel.OrderThemesList = GetThemesListForOrder();
                ViewBag.OrderId = model.OrderId;

                ViewBag.AlertMessage = "Check the form filling";
                ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;

                return View(performerSearchModel);
            }
        }

        [Authorize]
        public ActionResult SearchUserByName(bool? success)
        {
            if (success != null && success == false)
            {
                ViewBag.Success = VodadModel.Utilities.Constants.AlertMessages.Failed;
                ViewBag.AlertMessage = "None is found, try to enter another name.";
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult SearchUserByName(SearchUserByNameModel model)
        {
            const bool success = false;

            if (ModelState.IsValid)
            {
                return RedirectToAction("SearchUserByNameResult", "Search", new { UserName = HttpUtility.HtmlEncode(model.UserName) });
            }
            else
                return RedirectToAction("SearchUserByName", "Search", new { success });
        }

        [Authorize]
        public ActionResult SearchUserByNameResult(string UserName)
        {
            const bool success = false;

            var userRepository = new Repository<User>(Entities);

            string userNameEncode = HttpUtility.HtmlEncode(UserName).ToLower();

            var userListStartsWith = userRepository.GetAll(w => w.Name.ToLower().StartsWith(userNameEncode) && !w.Roles.RoleName.Equals(VodadModel.Utilities.Constants.UserRoles.Administrator) && !w.Roles.RoleName.Equals(VodadModel.Utilities.Constants.UserRoles.Helper)).ToList();
            var userListContainsExceptStartsWith = userRepository.GetAll(w => w.Name.ToLower().Contains(userNameEncode) && !w.Roles.RoleName.Equals(VodadModel.Utilities.Constants.UserRoles.Administrator) && !w.Roles.RoleName.Equals(VodadModel.Utilities.Constants.UserRoles.Helper)).ToList();
            var userList = userListStartsWith.Union(userListContainsExceptStartsWith).ToList();

            if (userList.Count > 0)
            {
                var searchUserByNameList = new List<SearchUserByNameResultModel>();

                foreach (var u in userList)
                {
                    var user = userRepository.GetSingle(w => w.Id == u.Id);

                    if (user != null)
                    {
                        var userListElement = new SearchUserByNameResultModel();
                        userListElement.UserId = (int)user.Id;
                        userListElement.UserRole = user.Roles.RoleName;
                        userListElement.UserName = UserHelper.GetUserNameByEmail(user.Email);

                        searchUserByNameList.Add(userListElement);
                    }
                }

                return View(searchUserByNameList);
            }
            else
                return RedirectToAction("SearchUserByName", "Search", new { success });
        }

        public ActionResult PerformerSearchResult(
                    bool allthemes,
                    bool games,
                    bool animals,
                    bool sport,
                    bool news,
                    bool politics,
                    bool show,
                    bool cinema,
                    int? width,
                    int? height,
                    int? mavph,
                    int? tuv,
                    int? tf,
                    int? rows,
                    int? oid,
                    bool sss,
                    int userId
            )
        {
            if (rows == null)
                rows = 25;
            ViewBag.RowsCount = rows;

            var model = new PerformerSearchModel();
            model.MinAverageViewersPerHour = mavph;
            model.TotalUniqueViews = tuv;
            model.TotalFollowers = tf;
            model.SelectAllThemes = allthemes;

            List<ThemesListForOrderModel> themesListForCampaign = GetThemesListForOrder();

            themesListForCampaign[0].IsChecked = games;
            themesListForCampaign[1].IsChecked = animals;
            themesListForCampaign[2].IsChecked = sport;
            themesListForCampaign[3].IsChecked = news;
            themesListForCampaign[4].IsChecked = politics;
            themesListForCampaign[5].IsChecked = show;
            themesListForCampaign[6].IsChecked = cinema;

            model.OrderThemesList = themesListForCampaign;
            model.OrderId = oid;

            var performerPlatformRepository = new Repository<PerformerPlatform>(Entities);
            var geolocationPlatformPercentageRepository = new Repository<GeolocationPlatformPercentage>(Entities);

            var performerPlatformList = new List<PerformerPlatform>();

            if (model.SelectAllThemes)
            {

                var result = performerPlatformRepository.GetAll(w => w.Verified == VodadModel.Utilities.Constants.CredentialCheckStatuses.Verified &&
                                                            w.PerformerStatistics.FirstOrDefault(v => v.PerformerPlatformId == w.Id).AverageViewerCountPerHour >= model.MinAverageViewersPerHour &&
                                                            w.PerformerStatistics.FirstOrDefault(v => v.PerformerPlatformId == w.Id).TotalUniqueViews >= model.TotalUniqueViews &&
                                                            w.PerformerStatistics.FirstOrDefault(v => v.PerformerPlatformId == w.Id).TotalFollowers >= model.TotalFollowers &&
                                                            !w.Status.Equals(VodadModel.Utilities.Constants.CredentialCheckStatuses.Deleted))
                                                            .ToList();
                if (result != null)
                    performerPlatformList.AddRange(result);
            }
            else
            {
                foreach (var t in model.OrderThemesList)
                {
                    var result = performerPlatformRepository.GetAll(w => w.Verified == VodadModel.Utilities.Constants.CredentialCheckStatuses.Verified &&
                                                            w.PerformerStatistics.FirstOrDefault(v => v.PerformerPlatformId == w.Id).AverageViewerCountPerHour >= model.MinAverageViewersPerHour &&
                                                            w.PerformerStatistics.FirstOrDefault(v => v.PerformerPlatformId == w.Id).TotalUniqueViews >= model.TotalUniqueViews &&
                                                            w.PerformerStatistics.FirstOrDefault(v => v.PerformerPlatformId == w.Id).TotalFollowers >= model.TotalFollowers &&
                                                            w.ThemeId == t.Id &&
                                                            !w.Status.Equals(VodadModel.Utilities.Constants.CredentialCheckStatuses.Deleted))
                                                            .ToList();
                    if (result != null)
                        performerPlatformList.AddRange(result);
                }
            }

            var performerSearchResultsModelList = new List<PerformerSearchResultsModel>();

            foreach (var s in performerPlatformList)
            {
                var performerSearchResultElement = new PerformerSearchResultsModel();

                performerSearchResultElement.PerformerId = (int)s.User.Id;
                performerSearchResultElement.PerformerPlatformId = (int)s.Id;
                performerSearchResultElement.PerformerName = UserHelper.GetUserNameByEmail(s.User.Email);
                performerSearchResultElement.PerformerPlatformLink = s.Link;

                performerSearchResultElement.PerformerThemeName= s.Themes.Name;

                // Извлекаем значения геолокации
                performerSearchResultElement.GeolocationAndPercentage = new List<string>();

                var geolocation = geolocationPlatformPercentageRepository.GetAll(w => w.PerformerPlatformId == s.Id).OrderByDescending(w => w.Percentage).Take(5);

                if (geolocation != null)
                {
                    foreach (var g in geolocation)
                    {
                        performerSearchResultElement.GeolocationAndPercentage.Add(g.Geolocation.CountryName + " - " + g.Percentage + "%");
                    }
                }

                try
                {
                    performerSearchResultElement.AverageViewersPerHour = (int)s.PerformerStatistics.FirstOrDefault(w => w.PerformerPlatformId == s.Id).AverageViewerCountPerHour;
                }
                catch
                {
                    performerSearchResultElement.AverageViewersPerHour = 0;
                }

                try
                {
                    performerSearchResultElement.MaxViewersCount = (int)s.PerformerStatistics.FirstOrDefault(w => w.PerformerPlatformId == s.Id).MaxViewersCount;
                }
                catch
                {
                    performerSearchResultElement.MaxViewersCount = 0;
                }

                try
                {
                    performerSearchResultElement.TotalUniqueViews = (int)s.PerformerStatistics.FirstOrDefault(w => w.PerformerPlatformId == s.Id).TotalUniqueViews;
                }
                catch
                {
                    performerSearchResultElement.TotalUniqueViews = 0;
                }

                try
                {
                    performerSearchResultElement.TotalViews = (int)s.PerformerStatistics.FirstOrDefault(w => w.PerformerPlatformId == s.Id).TotalViews;
                }
                catch
                {
                    performerSearchResultElement.TotalViews = 0;
                }

                try
                {
                    performerSearchResultElement.TotalFollowers = (int)s.PerformerStatistics.FirstOrDefault(w => w.PerformerPlatformId == s.Id).TotalFollowers;
                }
                catch
                {
                    performerSearchResultElement.TotalFollowers = 0;
                }

                try
                {
                    performerSearchResultElement.Likes = (int)s.User.Karma;
                }
                catch
                {
                    performerSearchResultElement.Likes = 0;
                }

                try
                {
                    performerSearchResultElement.AverageComplitionSpeed = s.PerformerStatistics.FirstOrDefault(w => w.PerformerPlatformId == s.Id).AverageComplitionSpeed;
                }
                catch
                {
                    performerSearchResultElement.AverageComplitionSpeed = 0;
                }

                try
                {
                    performerSearchResultElement.TotalOrders = (int)s.PerformerStatistics.FirstOrDefault(w => w.PerformerPlatformId == s.Id).TotalOrders;
                }
                catch
                {
                    performerSearchResultElement.TotalOrders = 0;
                }

                try
                {
                    performerSearchResultElement.UniqueViewersForMonth = (int)s.PerformerStatistics.FirstOrDefault(w => w.PerformerPlatformId == s.Id).UniqueViewersForMonth;
                }
                catch
                {
                    performerSearchResultElement.UniqueViewersForMonth = 0;
                }

                var blackAndWhiteListsController = new BlackAndWhiteListsController();
                if (!blackAndWhiteListsController.AreUsersInBlackLists((int)s.User.Id, userId))
                    performerSearchResultsModelList.Add(performerSearchResultElement);
            }

            ViewBag.OrderId = model.OrderId;

            // Save performer search settings to cookies
            if (sss)
            {
                HttpCookie performerSearchSettingCookie = new HttpCookie("PerformerSearch");

                performerSearchSettingCookie["TotalUniqueViews"] = tuv.ToString();
                performerSearchSettingCookie["TotalFollowers"] = tf.ToString();
                performerSearchSettingCookie["mavph"] = mavph.ToString();
                performerSearchSettingCookie["AllThemes"] = allthemes.ToString();

                performerSearchSettingCookie["Games"] = games.ToString();
                performerSearchSettingCookie["Animals"] = animals.ToString();
                performerSearchSettingCookie["Sport"] = sport.ToString();
                performerSearchSettingCookie["News"] = news.ToString();
                performerSearchSettingCookie["Politics"] = politics.ToString();
                performerSearchSettingCookie["Show"] = show.ToString();
                performerSearchSettingCookie["Cinema"] = cinema.ToString();

                performerSearchSettingCookie["Oid"] = oid.ToString();

                performerSearchSettingCookie.Expires = DateTime.Now.AddDays(30);
                performerSearchSettingCookie.Secure = false;

                Response.Cookies.Add(performerSearchSettingCookie);
            }

            return View("PerformerSearchResult", performerSearchResultsModelList);
        }

        public ActionResult PerformerSearchResultByName(int? uid)
        {
            if (uid != null)
            {
                var userRepository = new Repository<User>(Entities);
                var performerPlatformRepository = new Repository<PerformerPlatform>(Entities);

                var user = userRepository.GetSingle(w => w.Id == uid);

                if ((user != null) && user.Roles.RoleName.Equals(VodadModel.Utilities.Constants.UserRoles.Performer))
                {
                    var performerPlatformList = new List<PerformerPlatform>();


                    var result = performerPlatformRepository.GetAll(w => w.Verified == VodadModel.Utilities.Constants.CredentialCheckStatuses.Verified &&
                                                                     w.User.Id == uid &&
                                                                     !w.Status.Equals(VodadModel.Utilities.Constants.CredentialCheckStatuses.Deleted))
                        .ToList();
                    if (result != null)
                        performerPlatformList.AddRange(result);

                    var performerSearchResultsModelList = new List<PerformerSearchResultsModel>();

                    var geolocationPlatformPercentageRepository = new Repository<GeolocationPlatformPercentage>(Entities);

                    foreach (var s in performerPlatformList)
                    {
                        var performerSearchResultElement = new PerformerSearchResultsModel();

                        performerSearchResultElement.PerformerId = (int)s.User.Id;
                        performerSearchResultElement.PerformerPlatformId = (int)s.Id;
                        performerSearchResultElement.PerformerName =
                            UserHelper.GetUserNameByEmail(s.User.Email);

                        performerSearchResultElement.PerformerThemeName = s.Themes.Name;

                        // Извлекаем значения геолокации
                        performerSearchResultElement.GeolocationAndPercentage = new List<string>();

                        var geolocation =
                            geolocationPlatformPercentageRepository.GetAll(
                                w => w.PerformerPlatformId == s.Id).OrderByDescending(w => w.Percentage).
                                Take(5);

                        if (geolocation != null)
                        {
                            foreach (var g in geolocation)
                            {
                                performerSearchResultElement.GeolocationAndPercentage.Add(g.Geolocation.CountryName +
                                                                                         " - " + g.Percentage + "%");
                            }
                        }

                        performerSearchResultElement.AverageViewersPerHour =
                            (int)
                            s.PerformerStatistics.FirstOrDefault(
                                w => w.PerformerPlatformId == s.Id).AverageViewerCountPerHour;
                        performerSearchResultElement.MaxViewersCount =
                            (int)
                            s.PerformerStatistics.FirstOrDefault(
                                w => w.PerformerPlatformId == s.Id).MaxViewersCount;
                        performerSearchResultElement.TotalUniqueViews =
                            (int)
                            s.PerformerStatistics.FirstOrDefault(
                                w => w.PerformerPlatformId == s.Id).TotalUniqueViews;
                        performerSearchResultElement.TotalViews =
                            (int)
                            s.PerformerStatistics.FirstOrDefault(
                                w => w.PerformerPlatformId == s.Id).TotalViews;
                        performerSearchResultElement.TotalFollowers =
                            (int)
                            s.PerformerStatistics.FirstOrDefault(
                                w => w.PerformerPlatformId == s.Id).TotalFollowers;
                        performerSearchResultElement.Likes =
                            (int)
                            s.PerformerStatistics.FirstOrDefault(
                                w => w.PerformerPlatformId == s.Id).Likes;
                        performerSearchResultElement.AverageComplitionSpeed =
                            s.PerformerStatistics.FirstOrDefault(
                                w => w.PerformerPlatformId == s.Id).AverageComplitionSpeed;
                        performerSearchResultElement.TotalOrders =
                            (int)
                            s.PerformerStatistics.FirstOrDefault(
                                w => w.PerformerPlatformId == s.Id).TotalOrders;

                        performerSearchResultsModelList.Add(performerSearchResultElement);
                    }

                    return View("PerformerSearchResult", performerSearchResultsModelList);
                }
                else
                    return RedirectToAction("Error404", "Error");
            }
            else
                return RedirectToAction("Error404", "Error");
        }

        public int? mavph { get; set; }
    }
}