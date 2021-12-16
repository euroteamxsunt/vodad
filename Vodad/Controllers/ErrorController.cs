using System.Web.Mvc;

namespace Vodad.Controllers
{
    public class ErrorController : BaseController
    {
        public ActionResult Error404(string url)
        {
            return View();
        }
    }
}
