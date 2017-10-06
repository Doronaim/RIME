using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RIME.Models;
using System.Net;
using LinqToTwitter;
using System.Threading.Tasks;

namespace RIME.Controllers
{
    public class BlogController : Controller
    {
        private RimeContext db = new RimeContext();
        public string Dummy()
        {
            //var users = new List<User>
            //{
            //    new User { UserName="Admin", FirstName="Doron", LastName="Naim", UserHash="Password", Country="Israel", DOB=DateTime.Now.Date, Photo="/img/gallery/users/admin.jpg", Summary="The one and only - I'm the Admin of this site"},
            //    new User { UserName="Guest", FirstName="Eran", LastName="Lunenfeld", UserHash="Password", Country="Palastine", DOB=DateTime.Now.Date, Photo="/img/gallery/users/guest.jpg", Summary="The one and not the only - I'm the Guest of this site"},

            //};

            

            var evidences = new List<Evidence>
            {
                new Evidence { EvidencePic="/img/gallery/ev1.jpg", EvidencePath="/img/videos/ev1.mp4", EvidenceLocation="United State", Date=DateTime.Now.Date, Title="Harame in Israel", UserName="Admin", Prolog="prolog prolog prolog.........", Content="content, content,content................................", Likes=23 },
                new Evidence { EvidencePic="/img/gallery/ev2.jpg", EvidencePath="/img/videos/ev2.mp4", EvidenceLocation="Iraq", Date=DateTime.Now.Date, Title="The Other Side is Listening...", UserName="Guest", Prolog="prolog prolog prolog.........", Content="content, content,content................................", Likes=3 },

            };

            var comments = new List<EvidenceComment>
            {
                new EvidenceComment { EvidenceId=1, Name="Moshiko", Date=DateTime.Now.Date, Content="ABCDEFGHI JKLMNOP QRSTUVWXYZ" },
                new EvidenceComment { EvidenceId=2, Name="Shooki", Date=DateTime.Now.Date, Content="ABCDEFGHI JKLMNOP QRSTUVWXYZ" },

            };

            var subcomments = new List<SubComment>
            {
                new SubComment { EvidenceCommentId=1, Date=DateTime.Now.Date, Name="Jinken Tinus", Content="And you've loved your lord godness in all your heart in all your soul in all your....."},
                new SubComment { EvidenceCommentId=2, Date=DateTime.Now.Date, Name="Sandal Zevel", Content="And you've loved your lord godness in all your heart in all your soul in all your....."},

            };
            var tags = new List<Tag>
            {
                new Tag { EvidenceId=1, Categoty="Field", TagName="Golda Meir"},
                new Tag { EvidenceId=1, Categoty="Field", TagName="War"},
                new Tag { EvidenceId=2, Categoty="Israel", TagName="War"},
                new Tag { EvidenceId=2, Categoty="Palastine", TagName="War"},

            };

            //users.ForEach(s => db.Users.Add(s));
            //db.SaveChanges();
            evidences.ForEach(s => db.Evidences.Add(s));
            db.SaveChanges();
            comments.ForEach(s => db.EvidenceComments.Add(s));
            db.SaveChanges();
            subcomments.ForEach(s => db.SubComments.Add(s));
            db.SaveChanges();
            tags.ForEach(s => db.Tags.Add(s));
            db.SaveChanges();
            return "OK!";
        }

        // GET: Blog
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

        public ActionResult Evidence(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evidence evidence = db.Evidences.Find(id);
            if (evidence == null)
            {
                return HttpNotFound();
            }
            return View(evidence);
        }

        public ActionResult PostComment()
        {
            EvidenceComment com = new EvidenceComment();

            string[] keys = Request.Form.AllKeys;
            com.EvidenceId = int.Parse(Request.Form.Keys[3]);
            com.Name = Request.Form[keys[0]];
            com.Email = Request.Form[keys[1]];
            com.Date = DateTime.Now.Date;

            com.Content = Request.Form[keys[2]];
            db.EvidenceComments.Add(com);
            db.SaveChanges();
            return RedirectToAction("Evidence/" + com.EvidenceId);
        }
        public ActionResult PostSubComment()
        {
            SubComment com = new SubComment();

            string[] keys = Request.Form.AllKeys;
            com.EvidenceCommentId = int.Parse(Request.Form.Keys[3]);
            com.Name = Request.Form[keys[0]];
            com.Email = Request.Form[keys[1]];
            com.Date = DateTime.Now.Date;

            com.Content = Request.Form[keys[2]];
            db.SubComments.Add(com);
            db.SaveChanges();
            return RedirectToAction("Evidence/" + com.EvidenceCommentId);
        }

        [HttpGet]
        public async Task<JsonResult> GetTweets()
        {
            var auth = new ApplicationOnlyAuthorizer()
            {
                CredentialStore = new InMemoryCredentialStore()
                {
                    ConsumerKey = "82zN11OQVwqsOINZkJAdd3riP",
                    ConsumerSecret = "99mXIHxfwRtstF2nIBNaqsVe3hAdiUERZEeIULs0eXfsp2Khvf"
                }
            };

            await auth.AuthorizeAsync();

            var twitterCtx = new TwitterContext(auth);
                
          
            var srch =
                    await
            (from search in twitterCtx.Search
             where search.Type == SearchType.Search &&
                   search.Query == "MiddleEast"
             select search)
            .SingleOrDefaultAsync();
            return Json(srch.Statuses.ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}