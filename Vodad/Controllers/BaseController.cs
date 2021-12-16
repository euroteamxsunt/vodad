using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VodadModel;
using VodadModel.Model;
using VodadModel.Repository;
using Vodad.Models;

namespace Vodad.Controllers
{
    public abstract class BaseController : Controller
    {
        protected IQueryableRepository repo;
        protected VodadEntities Entities;

        protected BaseController()
            : this(new QueryableRepository())
        {
            Entities = new VodadEntities();
        }

        protected BaseController(IQueryableRepository repository)
        {
            repo = repository;
        }

        public static NLog.Logger Logger = NLogWrapper.Logger;

        public ActionResult Image(int? iid)
        {
            var imageRepository = new Repository<Image>(Entities);

            var imageData = imageRepository.GetSingle(w => w.Id == iid).ImageData;

            return File(imageData, "image/jpeg");
        }

        protected List<ThemesListForOrderModel> GetThemesListForOrder()
        {
            var themesRepository = new Repository<Themes>(Entities);
            var themesListForCampaign = new List<ThemesListForOrderModel>();
            var themesList = themesRepository.GetAll().ToList();

            foreach (var t in themesList)
            {
                var theme = new ThemesListForOrderModel();

                theme.Id = (int?)t.Id;
                theme.ThemeName = Server.HtmlDecode(t.Name);
                theme.IsChecked = false;

                themesListForCampaign.Add(theme);
            }

            return themesListForCampaign;
        }
    }
}