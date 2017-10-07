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
        public ActionResult Index(string title, string author, string content, string date)
        {
            var evidences = from e in db.Evidences select e;

            if (!String.IsNullOrEmpty(title))
            {
                evidences = evidences.Where(e => e.Title.Contains(title));
            }

            if (!String.IsNullOrEmpty(author))
            {
                evidences = evidences.Where(e => e.UserName.Contains(author));
            }

            if (!String.IsNullOrEmpty(content))
            {
                evidences = evidences.Where(e => e.Content.Contains(content));
            }

            if (!String.IsNullOrEmpty(date))
            {
                var dt = Convert.ToDateTime(date);
                evidences = evidences.Where(e => e.Date == dt);

            }

            return View(evidences.ToList());
        }
    }
}