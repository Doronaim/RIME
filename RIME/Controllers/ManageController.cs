using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using RIME.Models;
using System.Net;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace RIME.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private RimeContext db = new RimeContext();

        public Boolean isAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ApplicationDbContext context = new ApplicationDbContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Admin")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };
            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // GET: /Manage/RemovePhoneNumber
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        // GET: Tags
        public ActionResult TagIndex(string name, string category)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            var tags = from e in db.Tags select e;


            if (!String.IsNullOrEmpty(name))
            {
                tags = tags.Where(e => e.TagName.Contains(name));
            }

            if (!String.IsNullOrEmpty(category))
            {
                tags = tags.Where(e => e.Categoty.Contains(category));
            }

     
            return View("Tag/Index", tags.ToList());
        }


        // GET: Tags/Details/5
        public ActionResult TagDetails(int? id)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tag tag = db.Tags.Find(id);
            if (tag == null)
            {
                return HttpNotFound();
            }
            return View("Tag/Details",tag);
        }

        // GET: Tags/Create
        public ActionResult TagCreate()
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            return View("Tag/Create");
        }

        // POST: Tags/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TagCreate([Bind(Include = "TagId,EvidenceId,Categoty,TagName")] Tag tag)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            if (ModelState.IsValid)
            {
                db.Tags.Add(tag);
                db.SaveChanges();
                return RedirectToAction("TagIndex");
            }

            return View(tag);
        }

        // GET: Tags/Edit/5
        public ActionResult TagEdit(int? id)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tag tag = db.Tags.Find(id);
            if (tag == null)
            {
                return HttpNotFound();
            }
            return View("Tag/Edit",tag);
        }

        // POST: Tags/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TagEdit([Bind(Include = "TagId,EvidenceId,Categoty,TagName")] Tag tag)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            if (ModelState.IsValid)
            {
                db.Entry(tag).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("TagIndex");
            }
            return View("Tag/Edit",tag);
        }

        // GET: Tags/Delete/5
        public ActionResult TagDelete(int? id)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tag tag = db.Tags.Find(id);
            if (tag == null)
            {
                return HttpNotFound();
            }
            return View("Tag/Delete",tag);
        }

        // POST: Tags/Delete/5
        [HttpPost, ActionName("TagDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            Tag tag = db.Tags.Find(id);
            db.Tags.Remove(tag);
            db.SaveChanges();
            return RedirectToAction("TagIndex");
        }

        // GET: Evidences
        public ActionResult EvidenceIndex(string title, string author, string content, string date)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
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
            return View("Evidence/Index", evidences.ToList());
        }


        // GET: Evidences/Details/5
        public ActionResult EvidenceDetails(int? id)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evidence evidence = db.Evidences.Find(id);
            if (evidence == null)
            {
                return HttpNotFound();
            }
            return View("Evidence/Details",evidence);
        }


        // GET: Evidences/Create
        public ActionResult CreateEvidence()
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            return View("Evidence/Create");
        }

        // POST: Evidences/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEvidence([Bind(Include = "EvidenceId,UserName,EvidencePic,EvidencePath,EvidenceLocation,Title,Prolog,Content,Date,Likes,Quote")] Evidence evidence)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            if (ModelState.IsValid)
            {
                string[] keys = Request.Form.AllKeys;
                string[] tokens = User.Identity.Name.Split('@');

                evidence.UserName = tokens[0];
                
                db.Evidences.Add(evidence);
                
                
                if (Request.Form[keys[11]] != "")
                {
                    string[] values = Request.Form[keys[11]].Split(',');
                    foreach (var x in values)
                    {
                        Tag tg = new Tag();


                        tg.EvidenceId = db.Evidences.Local[0].EvidenceId;
                        tg.TagName = x;
                        tg.Categoty = Request.Form[keys[12]];

                        db.Tags.Add(tg);
                    }
                }
      
                db.SaveChanges();

                
                return RedirectToAction("EvidenceIndex");
            }

            return View("Evidence/Create",evidence);
        }

        // GET: Evidences/Edit/5
        public ActionResult EvidenceEdit(int? id)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evidence evidence = db.Evidences.Find(id);
            if (evidence == null)
            {
                return HttpNotFound();
            }
            return View("Evidence/Edit",evidence);
        }

        // POST: Evidences/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EvidenceEdit([Bind(Include = "EvidenceId,UserName,EvidencePic,EvidencePath,EvidenceLocation,Title,Prolog,Content,Date,Likes,Quote")] Evidence evidence)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            if (ModelState.IsValid)
            {
                db.Entry(evidence).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("EvidenceIndex");
            }
            return View("Evidence/Edit",evidence);
        }

        // GET: Evidences/Delete/5
        public ActionResult EvidenceDelete(int? id)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evidence evidence = db.Evidences.Find(id);
            if (evidence == null)
            {
                return HttpNotFound();
            }
            return View("Evidence/Delete",evidence);
        }

        // POST: Evidences/Delete/5
        [HttpPost, ActionName("EvidenceDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult EvidenceDeleteConfirmed(int id)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            Evidence evidence = db.Evidences.Find(id);
            db.Evidences.Remove(evidence);
            db.SaveChanges();
            return RedirectToAction("EvidenceIndex");
        }

        // GET: EvidenceComments
        public ActionResult EvidenceCommentsIndex(string author, string email, string content, string date)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            var comments = from e in db.EvidenceComments select e;


            if (!String.IsNullOrEmpty(author))
            {
                comments = comments.Where(e => e.Name.Contains(author));
            }

            if (!String.IsNullOrEmpty(email))
            {
                comments = comments.Where(e => e.Email.Contains(email));
            }

            if (!String.IsNullOrEmpty(content))
            {
                comments = comments.Where(e => e.Content.Contains(content));
            }

            if (!String.IsNullOrEmpty(date))
            {
                var dt = Convert.ToDateTime(date);
                comments = comments.Where(e => e.Date == dt);

            }
            return View("EvidenceComments/Index", comments.ToList());
        }

        // GET: EvidenceComments/Details/5
        public ActionResult EvidenceCommentsDetails(int? id)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvidenceComment evidenceComment = db.EvidenceComments.Find(id);
            if (evidenceComment == null)
            {
                return HttpNotFound();
            }
            return View("EvidenceComments/Details", evidenceComment);
        }
        // GET: Evidences
        public ActionResult EvidenceStatistics(bool? publisher, bool? dates)
        {
            /*Get post per user count */
            var postsPerUser = from e in db.Evidences
                               group e by e.UserName into author
                               select new { Name = author.Key, Evidences = author.Count() };

            /* Get post per month count */
            var postsPerDates = (from e in db.Evidences
                                 group e by new { month = e.Date.Month, year = e.Date.Year } into d
                                 select new { dt = d.Key.month.ToString() + "/" + d.Key.year.ToString(), count = d.Count() }).OrderByDescending(g => g.dt);

            /* Create list of selected dates, 12 month back */
            var selectedDates = new List<String>();
            for (var date = DateTime.Now.AddYears(-1); date <= DateTime.Now; date = date.AddMonths(1))
            {
                selectedDates.Add(date.Month.ToString() + "/" + date.Year.ToString());
            }
            /* Create final json for post per month graph including months with zero posts */
            var DatesCount = from d in selectedDates
                             join p in postsPerDates on d equals p.dt into dd
                             from count in dd.DefaultIfEmpty()
                             select new { Date = d, Count = count == null ? 0 : count.count };

            /* For ajax requests */
            if (Request.AcceptTypes.Contains("application/json"))
            {
                if (publisher.GetValueOrDefault())
                    return Json(postsPerUser, JsonRequestBehavior.AllowGet);

                if (dates.GetValueOrDefault())
                    return Json(DatesCount, JsonRequestBehavior.AllowGet);
            }

            return View("Evidence/Statistics", db.Evidences.ToList());
        }
        // GET: EvidenceComments/Create
        public ActionResult EvidenceCommentsCreate()
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            return View("EvidenceComments/Create");
        }

        // POST: EvidenceComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EvidenceCommentsCreate([Bind(Include = "EvidenceCommentId,EvidenceId,Name,Email,Date,Content")] EvidenceComment evidenceComment)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            if (ModelState.IsValid)
            {
                db.EvidenceComments.Add(evidenceComment);
                db.SaveChanges();
                return RedirectToAction("EvidenceCommentIndex");
            }

            return View("EvidenceComments/Create", evidenceComment);
        }

        // GET: EvidenceComments/Edit/5
        public ActionResult EvidenceCommentsEdit(int? id)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvidenceComment evidenceComment = db.EvidenceComments.Find(id);
            if (evidenceComment == null)
            {
                return HttpNotFound();
            }
            return View("EvidenceComments/Edit", evidenceComment);
        }

        // POST: EvidenceComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EvidenceCommentsEdit([Bind(Include = "EvidenceCommentId,EvidenceId,Name,Email,Date,Content")] EvidenceComment evidenceComment)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            if (ModelState.IsValid)
            {
                db.Entry(evidenceComment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("EvidenceCommentsIndex");
            }
            return View("EvidenceComments/Edit", evidenceComment);
        }

        // GET: EvidenceComments/Delete/5
        public ActionResult EvidenceCommentsDelete(int? id)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvidenceComment evidenceComment = db.EvidenceComments.Find(id);
            if (evidenceComment == null)
            {
                return HttpNotFound();
            }
            return View("EvidenceComments/Delete", evidenceComment);
        }

        // POST: EvidenceComments/Delete/5
        [HttpPost, ActionName("EvidenceCommentsDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult EvidenceCommentsDeleteConfirmed(int id)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            EvidenceComment evidenceComment = db.EvidenceComments.Find(id);
            db.EvidenceComments.Remove(evidenceComment);
            db.SaveChanges();
            return RedirectToAction("EvidenceCommentsIndex");
        }

        // GET: SubComments
        public ActionResult SubCommentsIndex(string author, string email, string content, string date)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            var comments = from e in db.SubComments select e;


            if (!String.IsNullOrEmpty(author))
            {
                comments = comments.Where(e => e.Name.Contains(author));
            }

            if (!String.IsNullOrEmpty(email))
            {
                comments = comments.Where(e => e.Email.Contains(email));
            }

            if (!String.IsNullOrEmpty(content))
            {
                comments = comments.Where(e => e.Content.Contains(content));
            }

            if (!String.IsNullOrEmpty(date))
            {
                var dt = Convert.ToDateTime(date);
                comments = comments.Where(e => e.Date == dt);

            }
            return View("SubComments/Index", comments.ToList());
        }

        // GET: SubComments/Details/5
        public ActionResult SubCommentsDetails(int? id)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubComment subComment = db.SubComments.Find(id);
            if (subComment == null)
            {
                return HttpNotFound();
            }
            return View("SubComments/Details", subComment);
        }

        // GET: SubComments/Create
        public ActionResult SubCommentsCreate()
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            return View("SubComments/Create");
        }

        // POST: SubComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubCommentsCreate([Bind(Include = "SubCommentId,EvidenceCommentId,Name,Email,Date,Content")] SubComment subComment)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            if (ModelState.IsValid)
            {
                db.SubComments.Add(subComment);
                db.SaveChanges();
                return RedirectToAction("SubCommentsIndex");
            }

            return View("SubComments/Create", subComment);
        }

        // GET: SubComments/Edit/5
        public ActionResult SubCommentsEdit(int? id)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubComment subComment = db.SubComments.Find(id);
            if (subComment == null)
            {
                return HttpNotFound();
            }
            return View("SubComments/Edit", subComment);
        }

        // POST: SubComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubCommentsEdit([Bind(Include = "SubCommentId,EvidenceCommentId,Name,Email,Date,Content")] SubComment subComment)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            if (ModelState.IsValid)
            {
                db.Entry(subComment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SubCommentsIndex");
            }
            return View("SubComments/Edit", subComment);
        }

        // GET: SubComments/Delete/5
        public ActionResult SubCommentsDelete(int? id)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubComment subComment = db.SubComments.Find(id);
            if (subComment == null)
            {
                return HttpNotFound();
            }
            return View("SubComments/Delete", subComment);
        }

        // POST: SubComments/Delete/5
        [HttpPost, ActionName("SubCommentsDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult SubCommentsDeleteConfirmed(int id)
        {
            if (!isAdminUser())
                return RedirectToAction("Index", "User");
            SubComment subComment = db.SubComments.Find(id);
            db.SubComments.Remove(subComment);
            db.SaveChanges();
            return RedirectToAction("SubCommentsIndex");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

#region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion
    }
}