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
            var evidences = new List<Evidence>
            {
                new Evidence { EvidencePic="https://www.womensrefugeecommission.org/images/Legal-Protection_Header.jpg", EvidencePath="https://player.vimeo.com/video/236658328?title=0&amp;byline=0&amp;portrait=0", EvidenceLocation="United State", Date=DateTime.Now.Date, Title="Is israel a legitimate state?", UserName="Jack", Prolog="Alan Dershowitz professor of Law at Harvard law school trying to answer", Content="What is the difference between Anti-Semitism and Racism? Expelled jews were never defined as refugees, So what have changed? Alan Dershowitz will try to anwer these questions by explaining the importance of legal training.", Likes=23, Quote="Israel sought the way of the pen, rather than of the sword" },
                new Evidence { EvidencePic="http://www.haaretz.com/polopoly_fs/1.632606!/image/1035438674.jpg_gen/derivatives/headline_609x343/1035438674.jpg", EvidencePath="https://player.vimeo.com/video/236657998?title=0&amp;byline=0&amp;portrait=0", EvidenceLocation="Egypt", Date=DateTime.Now.Date, Title="Ex. Extremist Islamist About Hamas", UserName="Mario", Prolog="Dr. Taufik Hamid, explains why people of Gaza is still suffering these days", Content="Hamas part in the current situation in Gaza strip. Is it has anything to do with Israel policy?", Likes=13, Quote="The suffer of the Palestinians is because of their leadership" },
                new Evidence { EvidencePic="http://beapeacekeeper.com/unjobs/img/unitednationswall.jpg", EvidencePath="https://player.vimeo.com/video/236485822?title=0&amp;byline=0&amp;portrait=0", EvidenceLocation="New York", Date=DateTime.Now.Date, Title="Is UN Stegthening The Terrorism?", UserName="Eva", Prolog="Nikki Haley talkes after visiting the Middle East ", Content="USA Ambassador in UN congress is sharing her thoughts after visiting and talking to both sides.", Likes=22, Quote="Hamas has no cares for the Palestinians nor for Israelis" },

                //new Evidence { EvidencePic="", EvidencePath="?title=0&amp;byline=0&amp;portrait=0", EvidenceLocation="", Date=DateTime.Now.Date, Title="", UserName="", Prolog="", Content="", Likes=0, Quote="" },
            };

            var comments = new List<EvidenceComment>
            {
                //new EvidenceComment { EvidenceId=1, Name="Moshiko", Date=DateTime.Now.Date, Content="ABCDEFGHI JKLMNOP QRSTUVWXYZ" },
                //new EvidenceComment { EvidenceId=2, Name="Shooki", Date=DateTime.Now.Date, Content="ABCDEFGHI JKLMNOP QRSTUVWXYZ" },

            };

            var subcomments = new List<SubComment>
            {
                //new SubComment { EvidenceCommentId=1, Date=DateTime.Now.Date, Name="Jinken Tinus", Content="And you've loved your lord godness in all your heart in all your soul in all your....."},
                //new SubComment { EvidenceCommentId=2, Date=DateTime.Now.Date, Name="Sandal Zevel", Content="And you've loved your lord godness in all your heart in all your soul in all your....."},

            };
            var tags = new List<Tag>
            {
                new Tag { EvidenceId=1, Categoty="Clip", TagName="Racism"},
                new Tag { EvidenceId=1, Categoty="Clip", TagName="Legal"},
                new Tag { EvidenceId=1, Categoty="People", TagName="Alan Dershowitz"},

                new Tag { EvidenceId=2, Categoty="Palestine", TagName="Gaza"},
                new Tag { EvidenceId=2, Categoty="Palestine", TagName="Hamas"},
                new Tag { EvidenceId=2, Categoty="People", TagName="Taufik Hamid"},

                new Tag { EvidenceId=3, Categoty="World", TagName="UN"},
                new Tag { EvidenceId=3, Categoty="Palestine", TagName="Hamas"},
                new Tag { EvidenceId=3, Categoty="People", TagName="Nikki Haley"},


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

            //if (com.EvidenceCommentId == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            EvidenceComment evcom = db.EvidenceComments.Find(com.EvidenceCommentId);
            if (evcom == null)
            {
                return HttpNotFound();
            }
            db.SubComments.Add(com);
            db.SaveChanges();
            return RedirectToAction("Evidence/" + evcom.EvidenceId);
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