using System.Web.Mvc;

namespace ticket_vendor_demo.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home/Index
        public ActionResult Index()
        {
            return View();
        }

        // GET: Home/DisplayData
        public ActionResult DisplayData()
        {
            return View();
        }
    }
}