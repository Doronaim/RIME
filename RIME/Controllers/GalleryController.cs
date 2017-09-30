using RIME.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIME.Controllers
{
    public class GalleryController : Controller
    {
        private RimeContext db = new RimeContext();
        // GET: Gallery
        public ActionResult Index()
        {
            return View(db.Evidences.ToList());
        }
    }
}