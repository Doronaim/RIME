using System;
using System.Collections.Generic;
using System.Linq;
using Accord.MachineLearning;
using System.Web.Mvc;
using RIME.Models;
using System.Net;
using LinqToTwitter;
using System.Data;
using System.Threading.Tasks;
using System.Text;

namespace RIME.Controllers
{
    public class BlogController : Controller
    {
        private RimeContext db = new RimeContext();
        public string Dummy()
        {
            var dates = new List<DateTime>
            {
               new DateTime(2017,1,1,12,5,7,123),
               new DateTime(2017,4,1,17,5,7,123),
               new DateTime(2017,6,1,19,5,7,123),
               new DateTime(2017,8,1,6,5,7,123)
            };
            var evidences = new List<Evidence>
            {
                new Evidence { EvidencePic="https://goo.gl/9jWfp1", EvidencePath="https://goo.gl/VCTmmi", EvidenceLocation="United State", Date=dates[0], Title="Is israel a legitimate state?", UserName="Jack", Prolog="Alan Dershowitz professor of Law at Harvard law school is trying to answer", Content="What is the difference between Anti-Semitism and Racism? Expelled jews were never defined as refugees, So what have changed? Alan Dershowitz will try to anwer these questions by explaining the importance of legal training.", Likes=23, Quote="Israel sought the way of the pen, rather than of the sword" },
                new Evidence { EvidencePic="https://goo.gl/CbLJsQ", EvidencePath="https://goo.gl/vzjKf6", EvidenceLocation="Egypt", Date=dates[0], Title="Ex. Extremist Islamist About Hamas", UserName="Mario", Prolog="Dr. Taufik Hamid, explains why people of Gaza are still suffering these days", Content="Hamas part in the current situation in Gaza strip. Is it has anything to do with Israel policy?", Likes=13, Quote="The suffer of the Palestinians is because of their leadership" },
                new Evidence { EvidencePic="https://goo.gl/6NzJ7G", EvidencePath="https://goo.gl/m4Fzgr", EvidenceLocation="New York", Date=dates[2], Title="Is UN Stegthening The Terrorism?", UserName="Eva", Prolog="Nikki Haley talkes after visiting into the Middle East. What've changed? ", Content="USA Ambassador in UN congress is sharing her thoughts after visiting and talking to both sides.", Likes=22, Quote="Hamas has no cares for the Palestinians nor for Israelis" },

                new Evidence { EvidencePic="https://goo.gl/xRCzxs", EvidencePath="https://goo.gl/7oRxjm", EvidenceLocation="Israel", Date=dates[0], Title="Is This Where History Begins?", UserName="Mario", Prolog="Israel's fourth elected Prime Minister Golda Meir interview", Content="Golda Meir shares her beliefs, explains what happened in the past and what she thinks will happen in the future. Was she right?", Likes=4, Quote="They just refuse to believe that we have the right to exist at all" },
                new Evidence { EvidencePic="https://goo.gl/TSAnQN", EvidencePath="https://goo.gl/5yjQpM", EvidenceLocation="London", Date=dates[2], Title="Israel Loses Where Palastin Wins", UserName="Eva", Prolog="Melanie Phillips a British journalist critisize the west ideology", Content="Maybe Israel wins in battles on the field - but they absolutely loses in every other battle which involves Hasbara.", Likes=8, Quote="Israeli Hasbara is a Joke" },
                new Evidence { EvidencePic="https://goo.gl/RTncTJ", EvidencePath="https://goo.gl/tpjDTE", EvidenceLocation="Jerusalem", Date=dates[1], Title="Palestinian Woman After Terror Attack", UserName="Eva", Prolog="Woman who hope for violence on friday outside the mosque", Content="An Interview with a woman who claims she lives inside the mosque, gave an interview to european media. Do her sentences sound rational?", Likes=19, Quote="Jews always lie" },

                new Evidence { EvidencePic="https://goo.gl/KGqyYP", EvidencePath="https://goo.gl/fda1vK", EvidenceLocation="Boston", Date=dates[1], Title="History Lesson: Israel OR Syria-Palastina?", UserName="Jack", Prolog="What happened in the middle-east during the history?", Content="Romans, Greeks, Persians, Ottomans and basically almost everyone has putted their legs in this land. Who invented the Syria-Palastina name? all in here, in history pages.", Likes=10, Quote="Rename Judea to Syria-Palastina" },
                new Evidence { EvidencePic="https://goo.gl/umo1Lp", EvidencePath="https://goo.gl/FQRGZH", EvidenceLocation="Sudan", Date=dates[3], Title="Sudanese Minister about the Palestinian Problem", UserName="Mario", Prolog="Minister that thinks differently and doesn't afraid to say it", Content="Mubarak al Fadil al Mahdi, a Minister of Investments in Sudan share his thoughts about the Palestinian problem. Mubarak talks about the mistakes the Palestinians did, and about the approach of all arab middle east countries.", Likes=6, Quote="The Arab countries peddled in the Palestinian cause" },

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
                new Tag { EvidenceId=1, Categoty="Figure", TagName="Alan Dershowitz"},

                new Tag { EvidenceId=2, Categoty="Media", TagName="Interview"},
                new Tag { EvidenceId=2, Categoty="Palestine", TagName="Hamas"},
                new Tag { EvidenceId=2, Categoty="Figure", TagName="Taufik Hamid"},

                new Tag { EvidenceId=3, Categoty="World", TagName="UN"},
                new Tag { EvidenceId=3, Categoty="Palestine", TagName="Hamas"},
                new Tag { EvidenceId=3, Categoty="Figure", TagName="Nikki Haley"},

                new Tag { EvidenceId=4, Categoty="Media", TagName="Interview"},
                new Tag { EvidenceId=4, Categoty="Palestine", TagName="Arabs"},
                new Tag { EvidenceId=4, Categoty="Figure", TagName="Golda Meir"},

                new Tag { EvidenceId=5, Categoty="Figure", TagName="Melanie Phillips"},
                new Tag { EvidenceId=5, Categoty="Media", TagName="Interview"},
                new Tag { EvidenceId=5, Categoty="Solution", TagName="two states"},
                
                new Tag { EvidenceId=6, Categoty="World", TagName="Terror"},
                new Tag { EvidenceId=6, Categoty="Solution", TagName="Mosque"},
                new Tag { EvidenceId=6, Categoty="Media", TagName="Interview"},

                new Tag { EvidenceId=7, Categoty="History", TagName="History"},
                new Tag { EvidenceId=7, Categoty="Clip", TagName="Interview"},
         
                new Tag { EvidenceId=8, Categoty="World", TagName="Sudan"},
                new Tag { EvidenceId=8, Categoty="Media", TagName="Interview"},





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
            RelatedBlogs();
            return View(evidence);
        }

        [HttpGet]
        public ActionResult GetRelatedEvidance(int? id) { 
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evidence evidence = db.Evidences.Find(id);
            if (evidence == null)
            {
                return HttpNotFound();
            }
            return Json(evidence, JsonRequestBehavior.AllowGet);
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

        public void RelatedBlogs()
        {
            var evidances = (from e in db.Evidences
                         select e).ToList();

            // Create Array of all posts content
            var docs = new List<string>();
            foreach (Evidence e in db.Evidences)
            {
                docs.Add(e.ToString());
            }
            string[] documents = docs.ToArray(); //(from p in db.Evidences.Include("Tags")
                                  //select p.Content + " " + p.Title).ToArray(); //  + String.Join(",", p.Tags.Select(x => x.TagName))

            ///Apply TF*IDF to the documents and get the resulting vectors.
            double[][] inputs = TFIDFEX.TFIDF.Transform(documents,2);
            inputs = TFIDFEX.TFIDF.Normalize(inputs);

            // Create a new K-Means algorithm with Posts/2 clusters (create couples) 
            KMeans kmeans = new KMeans(Convert.ToInt32(evidances.Count() / 2));

            // Compute the algorithm, retrieving an integer array
            //  containing the labels for each of the observations
            KMeansClusterCollection clusters = kmeans.Learn(inputs);
            int[] labels = clusters.Decide(inputs);


            // Create list with clusters couples
            var clustersList = new List<List<int>>();
            for (int j = 0; j < Convert.ToInt32(evidances.Count() / 2); j++)
            {
                clustersList.Add(labels.Select((s, i) => new { i, s })
                                       .Where(t => t.s == j)
                                       .Select(t => t.i)
                                       .ToList());
            }

            // Adjust all posts and thier related by clustering results
            var dict = new Dictionary<string, string>();
            foreach (var cluster in clustersList)
            {
                // In case cluster contains 3 posts and not 2
                if (cluster.Count() > 2)
                {


                    evidances[cluster[0]].RelatedEvidance = evidances[cluster[2]].EvidenceId;
                    evidances[cluster[1]].RelatedEvidance = evidances[cluster[0]].EvidenceId;
                    evidances[cluster[2]].RelatedEvidance = evidances[cluster[1]].EvidenceId;


                }
                else
                {
                  
                    evidances[cluster.FirstOrDefault()].RelatedEvidance  = evidances[cluster.LastOrDefault()].EvidenceId;
                    evidances[cluster.LastOrDefault()].RelatedEvidance = evidances[cluster.FirstOrDefault()].EvidenceId;

                }

            }
            // Update Changes in DB
            foreach (var p in evidances)
            {
                db.Entry(p).State = System.Data.Entity.EntityState.Modified;
            }
            db.SaveChanges();

        }

    }
}