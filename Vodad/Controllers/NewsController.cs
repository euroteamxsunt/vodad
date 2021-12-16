﻿using System.Web;
﻿using VodadModel;
using VodadModel.Helpers;
using VodadModel.Repository;
using Vodad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
﻿using System.Web.Mvc;

namespace Vodad.Controllers
{
    public class NewsController : BaseController
    {
        [ValidateInput(false)]
        [Authorize(Roles = "Administrator")]
        public ActionResult AddNews()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = "Administrator")]
        public ActionResult AddNews(AddNewsModel model)
        {
            var newsRepository = new Repository<News>(Entities);

            var news = new News();
            news.CreatorId = UserHelper.GetUserByEmail(User.Identity.Name).Id;
            news.CreationDate = DateTime.Now;
            news.Title = HttpUtility.HtmlEncode(model.NewsTitle);
            if (model.NewsText.Length > 8000 && model.NewsText.Length < 12000)
            {
                news.Text = HttpUtility.HtmlEncode(model.NewsText.Substring(0, 4000));
                news.Text1 = HttpUtility.HtmlEncode(model.NewsText.Substring(4000, 4000));
                news.Text2 = HttpUtility.HtmlEncode(model.NewsText.Substring(8000, model.NewsText.Length - 8000));
            }

            if (model.NewsText.Length < 8000 && model.NewsText.Length > 4001)
            {
                news.Text = HttpUtility.HtmlEncode(model.NewsText.Substring(0, 4000));
                news.Text1 = HttpUtility.HtmlEncode(model.NewsText.Substring(4000, model.NewsText.Length - 4000));
            }

            if (model.NewsText.Length < 4001)
            {
                news.Text = HttpUtility.HtmlEncode(model.NewsText.Substring(0, model.NewsText.Length));
            }

            newsRepository.Add(news);
            newsRepository.Save();

            var brandNewNews = newsRepository.GetAll(w => w.CreatorId == news.CreatorId).ToList().LastOrDefault();

            Logger.Info(string.Format("News id = {0} has been added by {1}", brandNewNews.Id, brandNewNews.CreatorId));

            return RedirectToAction("NewsList", "News");
        }

        [ValidateInput(false)]
        [Authorize(Roles = "Administrator")]
        public ActionResult EditNews(int? nid)
        {
            var newsRepository = new Repository<News>(Entities);

            var news = newsRepository.GetSingle(w => w.Id == nid);

            if (news != null)
            {
                var editNews = new EditNewsModel();

                editNews.NewsId = (int?)news.Id;
                editNews.NewsTitle = HttpUtility.HtmlDecode(news.Title);
                editNews.NewsText = HttpUtility.HtmlDecode(news.Text) + HttpUtility.HtmlDecode(news.Text1) + HttpUtility.HtmlDecode(news.Text2);

                return View(editNews);
            }

            return RedirectToAction("NewsList", "News", new { page = 1 });
        }

        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = "Administrator")]
        public ActionResult EditNews(EditNewsModel model)
        {
            if (ModelState.IsValid)
            {
                var newsRepository = new Repository<News>(Entities);

                var news = newsRepository.GetSingle(w => w.Id == model.NewsId);

                if (news != null)
                {
                    news.CreatorId = UserHelper.GetUserByEmail(User.Identity.Name).Id;
                    news.CreationDate = DateTime.Today;
                    news.Title = HttpUtility.HtmlEncode(model.NewsTitle);
                    if (model.NewsText.Length > 8000 && model.NewsText.Length < 12000)
                    {
                        news.Text = HttpUtility.HtmlEncode(model.NewsText.Substring(0, 4000));
                        news.Text1 = HttpUtility.HtmlEncode(model.NewsText.Substring(4000, 4000));
                        news.Text2 = HttpUtility.HtmlEncode(model.NewsText.Substring(8000, model.NewsText.Length - 8000));
                    }

                    if (model.NewsText.Length < 8000 && model.NewsText.Length > 4001)
                    {
                        news.Text = HttpUtility.HtmlEncode(model.NewsText.Substring(0, 4000));
                        news.Text1 = HttpUtility.HtmlEncode(model.NewsText.Substring(4000, model.NewsText.Length - 4000));
                    }

                    if (model.NewsText.Length < 4001)
                    {
                        news.Text = HttpUtility.HtmlEncode(model.NewsText.Substring(0, model.NewsText.Length));
                    }

                    newsRepository.Save();

                    Logger.Info(string.Format("News id = {0} has been modified", news.Id));

                    return RedirectToAction("News", "News", new { nid = model.NewsId });
                }

                return RedirectToAction("EditNews", "News", new { nid = model.NewsId });
            }
            else
                return RedirectToAction("EditNews", "News", new { nid = model.NewsId });
        }

        public ActionResult News(int? nid)
        {
            var newsRepository = new Repository<News>(Entities);

            var news = newsRepository.GetSingle(w => w.Id == nid);

            var newsForView = new NewsList();

            newsForView.NewsId = (int?)news.Id;
            newsForView.AuthorName = UserHelper.GetUserById((int?)news.CreatorId).Name;
            newsForView.CreationDate = news.CreationDate;
            newsForView.NewsTitle = HttpUtility.HtmlDecode(news.Title);
            newsForView.NewsText = HttpUtility.HtmlDecode(news.Text) + HttpUtility.HtmlDecode(news.Text1) + HttpUtility.HtmlDecode(news.Text2);

            ViewBag.NewsForView = newsForView;

            return View();
        }

        public ActionResult NewsList(int? page)
        {
            var newsRepository = new Repository<News>(Entities);
            var newsList = new List<NewsList>();

            int skipPages;

            if (page == null)
                skipPages = 1;
            else
                skipPages = Int32.Parse(page.ToString());

            var news = newsRepository.GetAll().ToList();

            news.Reverse();

            news = news.Skip((skipPages - 1) * 10).Take(10).ToList();

            foreach (var n in news)
            {
                var newNewsForList = new NewsList();

                newNewsForList.NewsId = (int?)n.Id;
                newNewsForList.AuthorName = UserHelper.GetUserById((int?)n.CreatorId).Name;
                newNewsForList.CreationDate = n.CreationDate;
                newNewsForList.NewsTitle = HttpUtility.HtmlDecode(n.Title);
                if (n.Text.Length > 400)
                    newNewsForList.NewsAnons = HttpUtility.HtmlDecode(n.Text.Substring(0, 400));
                else
                    newNewsForList.NewsAnons = HttpUtility.HtmlDecode(n.Text);

                newsList.Add(newNewsForList);
            }

            ViewBag.Page = skipPages;
            ViewBag.AllNews = newsList;
            ViewBag.NewsPagesCount = (int)(newsRepository.GetAll().Count() / 10);

            if ((ViewBag.NewsPagesCount % 10) > 0)
                ViewBag.NewsPagesCount++;

            return View();
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult RemoveNews(int? nid)
        {
            var newsRepository = new Repository<News>(Entities);

            var news = newsRepository.GetSingle(w => w.Id == nid);

            if (news != null)
            {
                newsRepository.Delete(news);
                newsRepository.Save();

                Logger.Info(string.Format("News id = {0} has been deleted", news.Id));
            }

            return RedirectToAction("NewsList", "News", new { page = 1 });
        }
    }
}