using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RIME.Models;

namespace RIME.Controllers
{
    public class HomeController : Controller
    {
        private RimeContext db = new RimeContext();
        public ActionResult Index()
        {
            return View(db.Evidences.ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}