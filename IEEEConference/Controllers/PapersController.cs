using IEEEConference.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace IEEEConference.Controllers
{
    public class PapersController : Controller
    {
        private IEEEConferenceDBContext db = new IEEEConferenceDBContext();

        // GET: Papers
        public ActionResult Index()
        {
            return RedirectToAction("", "Home", null);
        }

        // GET: Papers/AuthorIndex
        public ActionResult AuthorIndex()
        {
            Cmn cmn = new Cmn();
            cmn.UserName = "User Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            UserInformation userInformation = db.UserInformations.Where(p => p.UserAccount == cmn.UserName)
                                              .Where(p => p.PermissionEnabled == "Enabled")
                                              .FirstOrDefault();
            if (userInformation == null) { return RedirectToAction("", "Home", null); }

            //  Selected conference
            int conferenceID = userInformation.SelectedConference;
            if (conferenceID == null) { return RedirectToAction("", "Home", null); }
            Conference conference = db.Conferences.Find(conferenceID);
            //  paper owner
            List<int> paperIDs = db.Authors.Where(p => p.UserAccount == userInformation.UserAccount).Select(p => p.PaperID).ToList();

            ViewModel_PaperProfile viewModel_PaperProfile = new ViewModel_PaperProfile();
            viewModel_PaperProfile.ConferenceProfile = conference;
            viewModel_PaperProfile.ViewModel_Paper = GetViewModels(conferenceID, paperIDs);

            return View(viewModel_PaperProfile);
            //  View Menu:
            //  1. Enabled Return conference                                            Conferences/Index
            //  2. Enabled Create new paper as controlDate                              Papers/Create
            //  List Paper
            //  3. Linked to Details                                                    Papers/Details/5
        }

        // GET: Papers/ReviewerIndex
        public ActionResult ReviewerIndex()
        {
            Cmn cmn = new Cmn();
            cmn.UserName = "User Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            UserInformation userInformation = db.UserInformations.Where(p => p.UserAccount == cmn.UserName)
                                              .Where(p => p.PermissionEnabled == "Enabled")
                                              .FirstOrDefault();
            if (userInformation == null) { return RedirectToAction("", "Home", null); }

            //  Selected conference
            int conferenceID = userInformation.SelectedConference;
            if (conferenceID == null) { return RedirectToAction("", "Home", null); }
            Conference conference = db.Conferences.Find(conferenceID);
            //  paper owner
            List<int> paperIDs = db.Observations.Where(p => p.UserAccount == userInformation.UserAccount).Select(p => p.PaperID).ToList();

            ViewModel_PaperProfile viewModel_PaperProfile = new ViewModel_PaperProfile();
            viewModel_PaperProfile.ConferenceProfile = conference;
            viewModel_PaperProfile.ViewModel_Paper = GetViewModels(conferenceID, paperIDs);

            return View(viewModel_PaperProfile);
            //  View Menu:
            //  1. Enabled Return conference                                            Conferences/Index
            //  List Paper
            //  2. Enabled Comment as controlDate                                       Papers/Comment/5
        }

        // GET: Papers/TPCIndex
        public ActionResult TPCIndex()
        {
            Cmn cmn = new Cmn();
            cmn.UserName = "TPC Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }

            //  Selected conference
            UserInformation userInformation = db.UserInformations.Where(p => p.UserAccount == cmn.UserName && p.PermissionEnabled == "Enabled").FirstOrDefault();
            if (userInformation == null) { return RedirectToAction("", "Home", null); }
            int conferenceID = userInformation.SelectedConference;
            if (conferenceID == null) { return RedirectToAction("", "Home", null); }
            Conference conference = db.Conferences.Find(conferenceID);

            //  Check committee role
            int isCommittee = db.Committees.Where(p => p.ConferenceID == conferenceID)
                              .Where(p => p.UserAccount == userInformation.UserAccount)
                              .Where(p => p.PermissionGroup == "Chair" || p.PermissionGroup == "TPC")
                              .Count();
            //  paper owner
            List<int> paperIDs = db.Observations.Where(p => p.UserAccount == userInformation.UserAccount).Select(p => p.PaperID).ToList();

            ViewModel_PaperProfile viewModel_PaperProfile = new ViewModel_PaperProfile();
            viewModel_PaperProfile.ConferenceProfile = conference;
            viewModel_PaperProfile.ViewModel_Paper = GetViewModels(conferenceID, paperIDs);

            return View(viewModel_PaperProfile);
            //  View Menu:
            //  1. Enabled Return conference                                            Conferences/Index
            //  2. Enabled Accept Paper                                                 Papers/TPCAccept
            //  List Paper
            //  3. Enabled Manage                                                       Papers/Manage/5
        }

        // GET: Papers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            Paper paper = db.Papers.Find(id);
            if (paper == null) { return HttpNotFound(); };

            ViewModel_Paper viewModel_Paper = new ViewModel_Paper();
            viewModel_Paper = GetViewModel(id);

            return View(viewModel_Paper);
            //  View Menu:
            //  1. Enabled Return paper                                                 Papers/Index
            //  List Paper
            //  2. Enbaled Withdraw as controlDate                                      Papers/Withdraw/5
            //  3. Enabled Edit as controlDate                                          Papers/Edit/5
            //  4. Enabled Improve as controlDate                                       Papers/Improve/5
        }

        // GET: Papers/Manage/5
        public ActionResult Manage(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            Paper paper = db.Papers.Find(id);
            if (paper == null) { return HttpNotFound(); };

            ViewModel_Paper viewModel_Paper = new ViewModel_Paper();
            viewModel_Paper = GetViewModel(id);

            return View(viewModel_Paper);
            //  View Menu:
            //  1. Enabled Return paper                                                 Papers/Index
            //  List Paper
            //  2. Enbaled Withdraw as controlDate                                      Papers/Withdraw/5
            //  3. Enabled Edit as controlDate                                          Papers/Edit/5
            //  4. Enabled Improve as controlDate                                       Papers/Improve/5
            //  5. Enabled Reviewer Assignment as controlDate                           Papers/ReviewerAssignment/5
            //  6. Enabled Comment as controlDate                                       Papers/Comment/5
            //  7. Enabled Accepted paper as controlDate                                Papers/Accepted/5
        }

        // GET: Papers/Create
        public ActionResult Create()
        {
            Cmn cmn = new Cmn();
            cmn.UserName = "Create Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }

            UserInformation userInformation = db.UserInformations.Where(p => p.UserAccount == cmn.UserName && p.PermissionEnabled == "Enabled").FirstOrDefault();
            if (userInformation == null) { return RedirectToAction("", "Home", null); }
            int conferenceID = userInformation.SelectedConference;
            if (conferenceID == null) { return RedirectToAction("", "Home", null); }
            Conference conference = db.Conferences.Find(conferenceID);

            ViewModel_Paper viewModel_Paper = new ViewModel_Paper();
            viewModel_Paper.Conference = conference;

            return View(viewModel_Paper);
        }

        // POST: Papers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Created(ViewModel_Paper viewModel_Paper)
        {
            if (viewModel_Paper == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_Paper.Conference == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_Paper.Conference.ID == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            //  Procedure of form creation
            Cmn cmn = new Cmn();
            cmn.UserName = "Create Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }

            //  Validate conference code as unique
            Paper paper = new Paper();
            paper.ConferenceID = viewModel_Paper.Conference.ID;
            paper.SequenceNumber = "";
            //  Set system log
            paper.DateCreate = cmn.CurrentDatetime;
            paper.UserCreate = cmn.UserName;
            paper.DateEdit = cmn.CurrentDatetime;
            paper.UserEdit = cmn.UserName;
            db.Papers.Add(paper);
            db.SaveChanges();
            //  Set Author
            Author newItem = new Author();
            newItem.PaperID = paper.ID;
            newItem.UserAccount = cmn.UserName;
            newItem.DateCreate = cmn.CurrentDatetime;
            newItem.UserCreate = cmn.UserName;
            newItem.DateEdit = cmn.CurrentDatetime;
            newItem.UserEdit = cmn.UserName;
            db.Authors.Add(newItem);
            db.SaveChanges();

            return RedirectToAction("Edit", "Papers", new { id = paper.ID });
        }

        // GET: Papers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            Paper paper = db.Papers.Find(id);
            if (paper == null) { return HttpNotFound(); };
            if (paper.DateConfirmed != null) { return RedirectToAction("Details", "Papers", new { id = paper.ID }); }
            if (paper.DateWithdraw != null) { return RedirectToAction("Details", "Papers", new { id = paper.ID }); }

            ViewModel_Paper viewModel_Paper = new ViewModel_Paper();
            viewModel_Paper = GetViewModel(id);

            //  Select persentation type
            List<SelectedListItem> selectStyles = new List<SelectedListItem>();
            List<CommonDropList> commonDropLists = db.CommonDropLists.Where(p => p.Category == "StyleType").OrderBy(p => p.SequenceNum).ToList();
            if (commonDropLists != null)
            {
                foreach (CommonDropList commonDropList in commonDropLists)
                {
                    SelectedListItem selectStyle = new SelectedListItem();
                    selectStyle.SequenceNumber = commonDropList.SequenceNum;
                    selectStyle.ItemText = commonDropList.Text;
                    selectStyle.ItemValue = commonDropList.Value;
                    selectStyles.Add(selectStyle);
                }
            }
            viewModel_Paper.SelectStyles = selectStyles;

            return View(viewModel_Paper);
        }

        // POST: Papers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edited(ViewModel_Paper viewModel_Paper, HttpPostedFileBase docFile)
        {
            if (viewModel_Paper == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_Paper.Paper == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_Paper.Paper.ID == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (!ModelState.IsValid)
            {
                ViewData["error"] = "Please check below item(s), again";
                return View(viewModel_Paper);
            }
            //  Save
            //  Procedure of form edition
            Cmn cmn = new Cmn();
            cmn.UserName = "Edit Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            Paper paper = db.Papers.Find(viewModel_Paper.Paper.ID);
            //  Save as Authors
            if (viewModel_Paper.EditAuthors != null)
            {
                foreach (Author item in viewModel_Paper.EditAuthors)
                {
                    Author author = db.Authors.Find(item.ID);
                    // ***  Non-system attribute (Begin)    *** //
                    author.UserAccount = item.UserAccount;
                    // ***  Non-system attribute (Finish)   *** //
                    //  Set system log
                    author.DateEdit = cmn.CurrentDatetime;
                    author.UserEdit = cmn.UserName;
                    db.Entry(author).State = EntityState.Modified;
                }
            }
            // ***  Non-system attribute (Begin)    *** //
            paper.Title = viewModel_Paper.Paper.Title;
            paper.Keywords = viewModel_Paper.Paper.Keywords;
            paper.FieldCode = viewModel_Paper.Paper.FieldCode;
            paper.Style = viewModel_Paper.Paper.Style;
            paper.Remarks = viewModel_Paper.Paper.Remarks;
            if ((docFile == null || docFile.ContentLength < 0) && paper.FileNamePaper == null)
            {
                ViewData["uploadMsg"] = "Upload paper";
                viewModel_Paper = GetViewModel(viewModel_Paper.Paper.ID);
                return View(viewModel_Paper);
            }
            if (docFile != null)    {
                try {
                    var fileName = cmn.GetFileName("Paper", "Original", viewModel_Paper.Paper.ID.ToString(), viewModel_Paper.Conference.Code.ToString(), viewModel_Paper.Paper.SequenceNumber.ToString(), Path.GetExtension(docFile.FileName).ToString());
                    var filePath = Path.Combine(Server.MapPath(cmn.GetStoredFilePath()), fileName);
                    docFile.SaveAs(filePath);
                    paper.FileNamePaper = Path.GetFileName(docFile.FileName);
                    paper.FilePathPaper = filePath.ToString();
                    paper.DatePaper = cmn.CurrentDatetime;
                    paper.UserPaper = cmn.UserName;
                }   catch   {
                    ViewData["uploadMsg"] = "Upload failed";
                    return View(viewModel_Paper);   }
            }
            // ***  Non-system attribute (Finish)   *** //
            //  Set system log
            paper.DateEdit = cmn.CurrentDatetime;
            paper.UserEdit = cmn.UserName;
            db.Entry(paper).State = EntityState.Modified;
            //  Update database
            db.SaveChanges();
            String ctrlBtn = Request.Form["ctrlBtn"];
            if (ctrlBtn == "Refresh")
            { return RedirectToAction("Edit", "Papers", new { id = paper.ID }); }
            if (ctrlBtn == "Add Item")
            {
                //  Set row number
                Author newItem = new Author();
                newItem.PaperID = paper.ID;
                newItem.DateCreate = cmn.CurrentDatetime;
                newItem.UserCreate = cmn.UserName;
                newItem.DateEdit = cmn.CurrentDatetime;
                newItem.UserEdit = cmn.UserName;
                db.Authors.Add(newItem);
                db.SaveChanges();
                return RedirectToAction("Edit", "Papers", new { id = paper.ID });
            }
            if (ctrlBtn == "Submit")
            {
                List<Author> removeItems = db.Authors.Where(p => p.PaperID == paper.ID)
                                           .Where(p => p.UserAccount == null)
                                           .ToList();
                if (removeItems == null) { return RedirectToAction("Details", "Papers", new { id = paper.ID }); };
                foreach (Author removeItem in removeItems)
                {   db.Authors.Remove(removeItem);  }
                db.SaveChanges();
                return RedirectToAction("Confirm", "Papers", new { id = paper.ID });
            }
            return RedirectToAction("", "Home", null);
        }

        // GET: Papers/Confirm/5
        public ActionResult Confirm(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            Paper paper = db.Papers.Find(id);
            if (paper == null) { return HttpNotFound(); };
            if (paper.DateConfirmed != null) { return RedirectToAction("Details", "Papers", new { id = paper.ID }); }
            if (paper.DateWithdraw != null) { return RedirectToAction("Details", "Papers", new { id = paper.ID }); }

            ViewModel_Paper viewModel_Paper = new ViewModel_Paper();
            viewModel_Paper = GetViewModel(id);

            return View(viewModel_Paper);
        }

        // POST: Papers/Confirm/5
        [HttpPost, ActionName("Confirm")]
        [ValidateAntiForgeryToken]
        public ActionResult Confirmed(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

            Cmn cmn = new Cmn();
            cmn.UserName = "Confirm Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            Paper paper = db.Papers.Find(id);
            if (ModelState.IsValid == false)
            { return RedirectToAction("Confirm", "Papers", new { id = paper.ID }); }

            // ***  Generate Paper Number as empty (Begin)    *** //
            if (paper.SequenceNumber == "")
            {
                String maxVal = db.Papers.Where(p => p.ConferenceID == paper.ConferenceID && p.SequenceNumber != "").Max(p => p.SequenceNumber);
                paper.SequenceNumber = cmn.GetVersionID("1000", maxVal, 4);
            }
            // ***  Generate Paper Number as empty (Finish)    *** //
            //  set confirm date time
            paper.DateConfirmed = cmn.CurrentDatetime;
            paper.UserConfirmed = cmn.UserName;
            db.Entry(paper).State = EntityState.Modified;
            //  Update database
            db.SaveChanges();

            //  return RedirectToAction("Index");
            return RedirectToAction("Details", "Papers", new { id = paper.ID });
        }

        // GET: Papers/Withdraw/5
        public ActionResult Withdraw(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            Paper paper = db.Papers.Find(id);
            if (paper == null) { return HttpNotFound(); };
            if (paper.DateConfirmed != null) { return RedirectToAction("Details", "Papers", new { id = paper.ID }); }
            if (paper.DateWithdraw != null) { return RedirectToAction("Details", "Papers", new { id = paper.ID }); }

            ViewModel_Paper viewModel_Paper = new ViewModel_Paper();
            viewModel_Paper = GetViewModel(id);

            return View(viewModel_Paper);
        }

        // POST: Papers/Confirm/5
        [HttpPost, ActionName("Withdraw")]
        [ValidateAntiForgeryToken]
        public ActionResult Withdrawed(ViewModel_Paper viewModel_Paper)
        {
            if (viewModel_Paper == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_Paper.Paper == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_Paper.Paper.ID == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (!ModelState.IsValid)
            {
                ViewData["error"] = "Please check below item(s), again";
                return View(viewModel_Paper);
            }
            //  Save
            //  Procedure of form improve
            Cmn cmn = new Cmn();
            cmn.UserName = "Withdraw Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            Paper paper = db.Papers.Find(viewModel_Paper.Paper.ID);
            // ***  Non-system attribute (Begin)    *** //
            paper.ReasonWithdraw = viewModel_Paper.Paper.ReasonWithdraw;
            paper.DateWithdraw = viewModel_Paper.Paper.DateWithdraw;
            paper.UserWithdraw = viewModel_Paper.Paper.UserWithdraw;
            //  set confirm date time
            paper.DateEdit = cmn.CurrentDatetime;
            paper.UserEdit = cmn.UserName;
            db.Entry(paper).State = EntityState.Modified;
            //  Update database
            db.SaveChanges();

            //  return RedirectToAction("Index");
            return RedirectToAction("Details", "Papers", new { id = paper.ID });
        }

        // GET: Papers/Improve/5
        public ActionResult Improve(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            Paper paper = db.Papers.Find(id);
            if (paper == null) { return HttpNotFound(); };
            if (paper.DateConfirmed == null) { return RedirectToAction("Details", "Papers", new { id = paper.ID }); }
            if (paper.DateWithdraw != null) { return RedirectToAction("Details", "Papers", new { id = paper.ID }); }

            ViewModel_Paper viewModel_Paper = new ViewModel_Paper();
            viewModel_Paper = GetViewModel(id);

            //  Select persentation type
            List<SelectedListItem> selectStyles = new List<SelectedListItem>();
            List<CommonDropList> commonDropLists = db.CommonDropLists.Where(p => p.Category == "StyleType").OrderBy(p => p.SequenceNum).ToList();
            if (commonDropLists != null)
            {
                foreach (CommonDropList commonDropList in commonDropLists)
                {
                    SelectedListItem selectStyle = new SelectedListItem();
                    selectStyle.SequenceNumber = commonDropList.SequenceNum;
                    selectStyle.ItemText = commonDropList.Text;
                    selectStyle.ItemValue = commonDropList.Value;
                    selectStyles.Add(selectStyle);
                }
            }
            viewModel_Paper.SelectStyles = selectStyles;

            return View(viewModel_Paper);
        }

        // POST: Papers/Improve/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Improve")]
        [ValidateAntiForgeryToken]
        public ActionResult Improved(ViewModel_Paper viewModel_Paper, HttpPostedFileBase docFile, HttpPostedFileBase docFileCopyright)
        {
            if (viewModel_Paper == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_Paper.Paper == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_Paper.Paper.ID == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (!ModelState.IsValid)
            {
                ViewData["error"] = "Please check below item(s), again";
                return View(viewModel_Paper);
            }
            //  Save
            //  Procedure of form improve
            Cmn cmn = new Cmn();
            cmn.UserName = "Improve Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            Paper paper = db.Papers.Find(viewModel_Paper.Paper.ID);
            // ***  Non-system attribute (Begin)    *** //
            paper.Keywords = viewModel_Paper.Paper.Keywords;
            paper.FieldCode = viewModel_Paper.Paper.FieldCode;
            paper.Style = viewModel_Paper.Paper.Style;
            paper.Remarks = viewModel_Paper.Paper.Remarks;
            if (docFile != null)
            {
                try
                {
                    var fileName = cmn.GetFileName("Paper", "Original", viewModel_Paper.Paper.ID.ToString(), viewModel_Paper.Conference.Code.ToString(), viewModel_Paper.Paper.SequenceNumber.ToString(), Path.GetExtension(docFile.FileName).ToString());
                    var filePath = Path.Combine(Server.MapPath(cmn.GetStoredFilePath()), fileName);
                    docFile.SaveAs(filePath);
                    paper.FileNamePaper = Path.GetFileName(docFile.FileName);
                    paper.FilePathPaper = filePath.ToString();
                    paper.DatePaper = cmn.CurrentDatetime;
                    paper.UserPaper = cmn.UserName;
                }
                catch
                {
                    ViewData["uploadMsg"] = "Upload failed";
                    return View(viewModel_Paper);
                }
            }
            if (docFileCopyright != null)
            {
                try {
                    var fileName = cmn.GetFileName("Paper", "Copyright", viewModel_Paper.Paper.ID.ToString(), viewModel_Paper.Conference.Code.ToString(), viewModel_Paper.Paper.SequenceNumber.ToString(), Path.GetExtension(docFileCopyright.FileName).ToString());
                    var filePath = Path.Combine(Server.MapPath(cmn.GetStoredFilePath()), fileName);
                    docFile.SaveAs(filePath);
                    paper.FileNameCopyright = Path.GetFileName(docFileCopyright.FileName);
                    paper.FilePathCopyright = filePath.ToString();
                    paper.DateCopyright = cmn.CurrentDatetime;
                    paper.UserCopyright = cmn.UserName;
                }
                catch
                {
                    ViewData["uploadMsg"] = "Upload failed";
                    return View(viewModel_Paper);
                }
            }
            // ***  Non-system attribute (Finish)   *** //
            //  Set system log
            paper.DateEdit = cmn.CurrentDatetime;
            paper.UserEdit = cmn.UserName;
            db.Entry(paper).State = EntityState.Modified;
            //  Update database
            db.SaveChanges();
            String ctrlBtn = Request.Form["ctrlBtn"];
            if (ctrlBtn == "Refresh")
            { return RedirectToAction("Improve", "Papers", new { id = paper.ID }); }
            if (ctrlBtn == "Submit")
            { return RedirectToAction("Details", "Papers", new { id = paper.ID }); }
            return RedirectToAction("", "Home", null);
        }

        // GET: Papers/TPCEdit/5
        public ActionResult TPCEdit(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            Paper paper = db.Papers.Find(id);
            if (paper == null) { return HttpNotFound(); };

            Cmn cmn = new Cmn();
            cmn.UserName = "TCP Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            UserInformation userInformation = db.UserInformations.Where(p => p.UserAccount == cmn.UserName && p.PermissionEnabled == "Enabled").FirstOrDefault();
            if (userInformation == null) { return RedirectToAction("", "Home", null); }

            //  Return to home as not administrator and committee member
            if (userInformation.PermissionGroup != "System Administrator")
            {
                Committee committee = db.Committees.Where(p => p.ConferenceID == userInformation.SelectedConference)
                                      .Where(p => p.UserAccount == userInformation.UserAccount)
                                      .Where(p => p.PermissionGroup == "Chair" || p.PermissionGroup == "TPC")
                                      .FirstOrDefault();
                if (committee == null) { return RedirectToAction("", "Home", null); };
            }

            ViewModel_Paper viewModel_Paper = new ViewModel_Paper();
            viewModel_Paper = GetViewModel(id);

            //  Select persentation type
            //  Get Style Type
            viewModel_Paper.SelectStyles = GetDefaultItems("StyleType");
            //  Get TPC status
            viewModel_Paper.SelectTPCStatus = GetDefaultItems("TPCStatus");

            return View(viewModel_Paper);
        }

        // POST: Papers/TPCEdit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("TPCEdit")]
        [ValidateAntiForgeryToken]
        public ActionResult TPCEdit(ViewModel_Paper viewModel_Paper, HttpPostedFileBase docFile, HttpPostedFileBase docFileCopyright)
        {
            if (viewModel_Paper == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_Paper.Paper == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_Paper.Paper.ID == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (!ModelState.IsValid)
            {
                ViewData["error"] = "Please check below item(s), again";
                return View(viewModel_Paper);
            }
            //  Save
            //  Procedure of form TPCEdit
            Cmn cmn = new Cmn();
            cmn.UserName = "TPCEdit Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            Paper paper = db.Papers.Find(viewModel_Paper.Paper.ID);
            //  Save as Authors
            if (viewModel_Paper.EditAuthors != null)
            {
                foreach (Author item in viewModel_Paper.EditAuthors)
                {
                    Author author = db.Authors.Find(item.ID);
                    // ***  Non-system attribute (Begin)    *** //
                    author.UserAccount = item.UserAccount;
                    // ***  Non-system attribute (Finish)   *** //
                    //  Set system log
                    author.DateEdit = cmn.CurrentDatetime;
                    author.UserEdit = cmn.UserName;
                    db.Entry(author).State = EntityState.Modified;
                }
            }
            // ***  Non-system attribute (Begin)    *** //
            paper.Title = viewModel_Paper.Paper.Title; 
            paper.Keywords = viewModel_Paper.Paper.Keywords;
            paper.FieldCode = viewModel_Paper.Paper.FieldCode;
            paper.Style = viewModel_Paper.Paper.Style;
            paper.Remarks = viewModel_Paper.Paper.Remarks;
            if (paper.StatusTPC != viewModel_Paper.Paper.StatusTPC) {
                paper.StatusTPC = viewModel_Paper.Paper.StatusTPC;
                paper.DateTPC = cmn.CurrentDatetime;
                paper.UserTPC = cmn.UserName;
            }
            if (docFile != null)
            {
                try
                {
                    var fileName = cmn.GetFileName("Paper", "Original", viewModel_Paper.Paper.ID.ToString(), viewModel_Paper.Conference.Code.ToString(), viewModel_Paper.Paper.SequenceNumber.ToString(), Path.GetExtension(docFile.FileName).ToString());
                    var filePath = Path.Combine(Server.MapPath(cmn.GetStoredFilePath()), fileName);
                    docFile.SaveAs(filePath);
                    paper.FileNamePaper = Path.GetFileName(docFile.FileName);
                    paper.FilePathPaper = filePath.ToString();
                    paper.DatePaper = cmn.CurrentDatetime;
                    paper.UserPaper = cmn.UserName;
                }
                catch
                {
                    ViewData["uploadMsg"] = "Upload failed";
                    return View(viewModel_Paper);
                }
            }
            if (docFileCopyright != null)
            {
                try
                {
                    var fileName = cmn.GetFileName("Paper", "Copyright", viewModel_Paper.Paper.ID.ToString(), viewModel_Paper.Conference.Code.ToString(), viewModel_Paper.Paper.SequenceNumber.ToString(), Path.GetExtension(docFileCopyright.FileName).ToString());
                    var filePath = Path.Combine(Server.MapPath(cmn.GetStoredFilePath()), fileName);
                    docFile.SaveAs(filePath);
                    paper.FileNameCopyright = Path.GetFileName(docFileCopyright.FileName);
                    paper.FilePathCopyright = filePath.ToString();
                    paper.DateCopyright = cmn.CurrentDatetime;
                    paper.UserCopyright = cmn.UserName;
                }
                catch
                {
                    ViewData["uploadMsg"] = "Upload failed";
                    return View(viewModel_Paper);
                }
            }
            // ***  Non-system attribute (Finish)   *** //
            //  Set system log
            paper.DateEdit = cmn.CurrentDatetime;
            paper.UserEdit = cmn.UserName;
            db.Entry(paper).State = EntityState.Modified;
            //  Update database
            db.SaveChanges();
            String ctrlBtn = Request.Form["ctrlBtn"];
            if (ctrlBtn == "Refresh")
            { return RedirectToAction("TPCEdit", "Papers", new { id = paper.ID }); }
            if (ctrlBtn == "Add Item")
            {
                //  Set row number
                Author newItem = new Author();
                newItem.PaperID = paper.ID;
                newItem.DateCreate = cmn.CurrentDatetime;
                newItem.UserCreate = cmn.UserName;
                newItem.DateEdit = cmn.CurrentDatetime;
                newItem.UserEdit = cmn.UserName;
                db.Authors.Add(newItem);
                db.SaveChanges();
                return RedirectToAction("TPCEdit", "Papers", new { id = paper.ID });
            }
            if (ctrlBtn == "Submit")
            {
                List<Author> removeItems = db.Authors.Where(p => p.PaperID == paper.ID)
                                           .Where(p => p.UserAccount == null)
                                           .ToList();
                if (removeItems == null) { return RedirectToAction("Manage", "Papers", new { id = paper.ID }); };
                foreach (Author removeItem in removeItems)
                { db.Authors.Remove(removeItem); }
                db.SaveChanges();
                return RedirectToAction("Manage", "Papers", new { id = paper.ID });
            }
            return RedirectToAction("", "Home", null);
        }

        public ActionResult ReviewerAssignment(int? id, string group)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            Paper paper = db.Papers.Find(id);
            if (paper == null) { return HttpNotFound(); };
            if (group == null) { group = ""; };

            Cmn cmn = new Cmn();
            cmn.UserName = "TCP Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            UserInformation userInformation = db.UserInformations.Where(p => p.UserAccount == cmn.UserName && p.PermissionEnabled == "Enabled").FirstOrDefault();
            if (userInformation == null) { return RedirectToAction("", "Home", null); }

            //  Return to home as not administrator and committee member
            if (userInformation.PermissionGroup != "System Administrator")  {
                Committee committee = db.Committees.Where(p => p.ConferenceID == userInformation.SelectedConference)
                                      .Where(p => p.UserAccount == userInformation.UserAccount)
                                      .Where(p => p.PermissionGroup == "Chair" || p.PermissionGroup == "TPC")
                                      .FirstOrDefault();
                if (committee == null) { return RedirectToAction("", "Home", null); };
            }

            //  Return as not same conference
            ViewModel_ReviewerAssignment viewModel_ReviewerAssignment = db.Papers.Where(p => p.ID == id).Select(p =>
                                                                        new ViewModel_ReviewerAssignment()  {
                                                                              Conference = p.Conference
                                                                            , Paper = p
                                                                        }).FirstOrDefault();
            if (viewModel_ReviewerAssignment == null) { return RedirectToAction("", "Home", null);}
            if (viewModel_ReviewerAssignment.Paper == null) { return RedirectToAction("", "Home", null);}
            if (viewModel_ReviewerAssignment.Paper.ConferenceID == null) { return RedirectToAction("", "Home", null); }
            if (userInformation.SelectedConference != viewModel_ReviewerAssignment.Paper.ConferenceID)
            { return RedirectToAction("", "Home", null); }

            //  Get name list from committee group
            List<Committee> committees = db.Committees.Where(p => p.ConferenceID == userInformation.SelectedConference)
                                      .Where(p => p.PermissionGroup == group).ToList();
            //  Get reviewer name list from observation
            List<Observation> observations = db.Observations.Where(p => p.PaperID == viewModel_ReviewerAssignment.Paper.ID).ToList();
            List<string> userObservations = observations.Select(p => p.UserAccount).ToList();
            //  Convert observation view combine from committee and existed reviewer
            //  Left join, more details see https://smehrozalam.wordpress.com/2009/06/10/c-left-outer-joins-with-linq/
            //  Not in, more details see http://electricharbour.com/Articles/LINQ-Not-In-Example.aspx
            List<Observation> observationReviewers = (observations).Union(
                                                      committees.Where(p => !(userObservations.Contains(p.UserAccount)))
                                                      .Select(p => new Observation()    {
                                                            UserAccount = p.UserAccount
                                                          , Participate = "Dropped"})
                                                     ).ToList();
            List<string> userReviewers = observationReviewers.Select(p => p.UserAccount).ToList();
            //  Get user information
            List<UserInformation> userInformations = db.UserInformations.Where(p => userReviewers.Contains(p.UserAccount)).ToList();
            //  Convert to view model
            List<ViewModel_Observation> ViewModel_observations = (from item in observationReviewers
                                                                  join c in committees on item.UserAccount equals c.UserAccount into grpJoinC_item
                                                                  join u in userInformations on item.UserAccount equals u.UserAccount into grpJoinU_item
                                                                  orderby item
                                                                  from grpJoinC in grpJoinC_item.DefaultIfEmpty()
                                                                  from grpJoinU in grpJoinU_item.DefaultIfEmpty()
                                                                  select new ViewModel_Observation()    {
                                                                        Observation = item
                                                                      , Committee = (grpJoinC != null ? grpJoinC : null)
                                                                      , UserInformation = (grpJoinU != null ? grpJoinU : null)  }
                                                                 ).ToList();

            return View(viewModel_ReviewerAssignment);
        }

        // POST: Papers/ReviewerAssignment/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("ReviewerAssignment")]
        [ValidateAntiForgeryToken]
        public ActionResult ReviewerAssignmented(ViewModel_ReviewerAssignment viewModel_ReviewerAssignment)
        {
            if (viewModel_ReviewerAssignment == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_ReviewerAssignment.Paper == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_ReviewerAssignment.Paper.ID == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (!ModelState.IsValid)
            {
                ViewData["error"] = "Please check below item(s), again";
                return View(viewModel_ReviewerAssignment);
            }
            //  Save
            //  Procedure of form edition
            Cmn cmn = new Cmn();
            cmn.UserName = "ReviewerAssignment Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            Paper paper = db.Papers.Find(viewModel_ReviewerAssignment.Paper.ID);
            //  Save as reviewer
            if (viewModel_ReviewerAssignment.ViewModel_Observation != null)
            {
                foreach (ViewModel_Observation item in viewModel_ReviewerAssignment.ViewModel_Observation)
                {
                    if (item != null && item.Observation != null) {
                        //  Dropped mode
                        if (item.Observation.ID!= null && item.Observation.Participate == "Dropped") {
                            Observation observation = db.Observations.Find(item.Observation.ID);
                            // ***  Non-system attribute (Begin)    *** //
                            observation.Participate = item.Observation.Participate;
                            // ***  Non-system attribute (Finish)   *** //
                            db.Entry(observation).State = EntityState.Modified;
                        }
                        //  Added mode
                        if (item.Observation.ID == null && item.Observation.Participate == "Added") {
                            Observation observation = new Observation();
                            observation.UserAccount = item.Observation.UserAccount;
                            observation.Participate = item.Observation.Participate;
                    //  Set system log
                            observation.DateCreate = cmn.CurrentDatetime;
                            observation.UserCreate = cmn.UserName;
                            observation.DateEdit = cmn.CurrentDatetime;
                            observation.UserEdit = cmn.UserName;
                            db.Observations.Add(observation);   }
                    }
                }
            }
            //  Update database
            db.SaveChanges();
            String ctrlBtn = Request.Form["ctrlBtn"];
            if (ctrlBtn == "Refresh")
            { return RedirectToAction("ReviewerAssignment", "Papers", new { id = paper.ID }); }
            if (ctrlBtn == "Submit")
            {
                return RedirectToAction("Manage", "Papers", new { id = paper.ID });
            }
            return RedirectToAction("", "Home", null);
        }

        public ActionResult Comment(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            Paper paper = db.Papers.Find(id);
            if (paper == null) { return HttpNotFound(); };

            Cmn cmn = new Cmn();
            cmn.UserName = "Comment Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            UserInformation userInformation = db.UserInformations.Where(p => p.UserAccount == cmn.UserName && p.PermissionEnabled == "Enabled").FirstOrDefault();
            if (userInformation == null) { return RedirectToAction("", "Home", null); }

            ViewModel_Paper viewModel_Paper = new ViewModel_Paper();
            viewModel_Paper = GetViewModel(id);

            //  Return home as the paper of conference is not equal to user of conference
            if (viewModel_Paper.Conference.ID != userInformation.SelectedConference) { return RedirectToAction("", "Home", null); }
            //  System Administrator
            if (userInformation.PermissionGroup == "System Administrator")
            {
                viewModel_Paper.PermissionGroup = "Administrator";
                return View(viewModel_Paper);
            }
            //  Is not Organizing Committee
            Committee committee = db.Committees.Where(p => p.ConferenceID == paper.ConferenceID && p.UserAccount == userInformation.UserAccount)
                                 .FirstOrDefault();
            if (committee == null) { return RedirectToAction("", "Home", null); }
            //  Chair
            if (committee.PermissionGroup == "Chair")
            {
                viewModel_Paper.PermissionGroup = "Administrator";
                return View(viewModel_Paper);
            }
            //  Technical Programme Committee (TPC)
            if (committee.PermissionGroup == "TPC")
            {
                viewModel_Paper.PermissionGroup = "Administrator";
                return View(viewModel_Paper);
            }

            //  Remove author information and observation without reviewer owner
            viewModel_Paper.EditAuthors = null;
            viewModel_Paper.ViewModel_Author = null;
            viewModel_Paper.EditObservations = viewModel_Paper.EditObservations.Where(p => p.UserAccount == userInformation.UserAccount).ToList();
            viewModel_Paper.ViewModel_Observation = (from item in viewModel_Paper.EditObservations
                                                     join p in db.UserInformations on item.UserAccount equals p.UserAccount into grpJoin_item
                                                     from grpJoin in grpJoin_item.DefaultIfEmpty()
                                                     select new ViewModel_Observation()
                                                     {
                                                         Observation = item
                                                         ,
                                                         UserInformation = grpJoin
                                                     }
                                                    ).ToList();

            //  Reviewer
            if (committee.PermissionGroup == "Reviewer")
            {
                viewModel_Paper.PermissionGroup = "Reviewer";
                return View(viewModel_Paper);
            }

            return View(viewModel_Paper);
        }

        // POST: Papers/Comment/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Comment")]
        [ValidateAntiForgeryToken]
        public ActionResult Commented(ViewModel_Paper viewModel_Paper, HttpPostedFileBase docFile)
        {
            if (viewModel_Paper == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_Paper.Paper == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_Paper.Paper.ID == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (!ModelState.IsValid)
            {
                ViewData["error"] = "Please check below item(s), again";
                return View(viewModel_Paper);
            }
            //  Save
            //  Procedure of form commention
            Cmn cmn = new Cmn();
            cmn.UserName = "Comment Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            Paper paper = db.Papers.Find(viewModel_Paper.Paper.ID);
            //  Save as Authors
            if (viewModel_Paper.EditObservations != null)
            {
                foreach (Observation item in viewModel_Paper.EditObservations)
                {
                    Observation observation = db.Observations.Find(item.ID);
                    // ***  Non-system attribute (Begin)    *** //
                    observation.Grade = item.Grade;
                    observation.Comment = item.Comment;
                    // ***  Non-system attribute (Finish)   *** //
                    //  Set system log
                    observation.DateEdit = cmn.CurrentDatetime;
                    observation.UserEdit = cmn.UserName;
                    db.Entry(observation).State = EntityState.Modified;
                }
            }
            //  Update database
            db.SaveChanges();
            String ctrlBtn = Request.Form["ctrlBtn"];
            if (ctrlBtn == "Refresh")
            { return RedirectToAction("Comment", "Papers", new { id = paper.ID }); }
            if (ctrlBtn == "Add Item")
            {
                //  Set row number
                Observation newItem = new Observation();
                newItem.PaperID = paper.ID;
                newItem.UserAccount = cmn.UserName;
                newItem.DateCreate = cmn.CurrentDatetime;
                newItem.UserCreate = cmn.UserName;
                newItem.DateEdit = cmn.CurrentDatetime;
                newItem.UserEdit = cmn.UserName;
                db.Observations.Add(newItem);
                db.SaveChanges();
                return RedirectToAction("Comment", "Papers", new { id = paper.ID });
            }
            if (ctrlBtn == "Submit")
            {
                return RedirectToAction("Details", "Papers", new { id = paper.ID });
            }
            return RedirectToAction("", "Home", null);
        }

        // GET: Papers/TPCAccept
        public ActionResult TPCAccept()
        {
            Cmn cmn = new Cmn();
            cmn.UserName = "TPC Accept Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }

            //  Selected conference
            UserInformation userInformation = db.UserInformations.Where(p => p.UserAccount == cmn.UserName && p.PermissionEnabled == "Enabled").FirstOrDefault();
            if (userInformation == null) { return RedirectToAction("", "Home", null); }
            int conferenceID = userInformation.SelectedConference;
            if (conferenceID == null) { return RedirectToAction("", "Home", null); }
            Conference conference = db.Conferences.Find(conferenceID);

            //  Check committee role
            int isCommittee = db.Committees.Where(p => p.ConferenceID == conferenceID)
                              .Where(p => p.UserAccount == userInformation.UserAccount)
                              .Where(p => p.PermissionGroup == "Chair" || p.PermissionGroup == "TPC")
                              .Count();
            //  paper owner
            List<int> paperIDs = db.Observations.Where(p => p.UserAccount == userInformation.UserAccount).Select(p => p.PaperID).ToList();

            ViewModel_PaperProfile viewModel_PaperProfile = new ViewModel_PaperProfile();
            viewModel_PaperProfile.ConferenceProfile = conference;
            viewModel_PaperProfile.ViewModel_Paper = GetViewModels(conferenceID, paperIDs);
            //  Get TPC status
            viewModel_PaperProfile.SelectTPCStatus = GetDefaultItems("TPCStatus");


            return View(viewModel_PaperProfile);
            //  View Menu:
            //  1. Enabled Return conference                                            Conferences/Index
            //  2. Enabled Index paper                                                  Papers/TPCIndex
            //  List Paper
            //  3. Enabled Accepted
        }

        // POST: Papers/TPCAccept/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("TPCAccept")]
        [ValidateAntiForgeryToken]
        public ActionResult TPCAccepted(ViewModel_PaperProfile viewModel_PaperProfile)
        {
            if (viewModel_PaperProfile == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_PaperProfile.ViewModel_Paper == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (!ModelState.IsValid)
            {
                ViewData["error"] = "Please check below item(s), again";
                return View(viewModel_PaperProfile);
            }
            //  Save
            Cmn cmn = new Cmn();
            cmn.UserName = "TPC Accepted Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            //  Save as Authors
            if (viewModel_PaperProfile.ViewModel_Paper != null)
            {
                foreach (ViewModel_Paper item in viewModel_PaperProfile.ViewModel_Paper)
                {
                    if (item.Paper.ID != null)  {
                        Paper paper = db.Papers.Find(item.Paper.ID);
                        // ***  Non-system attribute (Begin)    *** //
                        paper.StatusTPC = item.Paper.StatusTPC;
                        // ***  Non-system attribute (Finish)   *** //
                        //  Set system log
                        paper.DateEdit = cmn.CurrentDatetime;
                        paper.UserEdit = cmn.UserName;
                        db.Entry(paper).State = EntityState.Modified;
                    }
                }
            }
            //  Update database
            db.SaveChanges();
            String ctrlBtn = Request.Form["ctrlBtn"];
            if (ctrlBtn == "Refresh")
            { return RedirectToAction("TPCAccept", "Papers", null); }
            if (ctrlBtn == "Submit")
            {   return RedirectToAction("TPCAcceptConfirm", "Papers", null);    }
            return RedirectToAction("", "Home", null);
        }

        // GET: Papers/TPCAcceptConfirm
        public ActionResult TPCAcceptConfirm()
        {
            Cmn cmn = new Cmn();
            cmn.UserName = "TPC Accept Confirm Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }

            //  Selected conference
            UserInformation userInformation = db.UserInformations.Where(p => p.UserAccount == cmn.UserName && p.PermissionEnabled == "Enabled").FirstOrDefault();
            if (userInformation == null) { return RedirectToAction("", "Home", null); }
            int conferenceID = userInformation.SelectedConference;
            if (conferenceID == null) { return RedirectToAction("", "Home", null); }
            Conference conference = db.Conferences.Find(conferenceID);

            //  Check committee role
            int isCommittee = db.Committees.Where(p => p.ConferenceID == conferenceID)
                              .Where(p => p.UserAccount == userInformation.UserAccount)
                              .Where(p => p.PermissionGroup == "Chair" || p.PermissionGroup == "TPC")
                              .Count();
            //  paper owner
            List<int> paperIDs = db.Observations.Where(p => p.UserAccount == userInformation.UserAccount).Select(p => p.PaperID).ToList();

            ViewModel_PaperProfile viewModel_PaperProfile = new ViewModel_PaperProfile();
            viewModel_PaperProfile.ConferenceProfile = conference;
            viewModel_PaperProfile.ViewModel_Paper = GetViewModels(conferenceID, paperIDs);

            return View(viewModel_PaperProfile);
            //  View Menu:
            //  1. Enabled Return conference                                            Conferences/Index
            //  2. Enabled Index paper                                                  Papers/TPCIndex
            //  List Paper
        }

        // POST: Papers/TPCAcceptConfirm/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("TPCAcceptConfirm")]
        [ValidateAntiForgeryToken]
        public ActionResult TPCAcceptConfirmed(ViewModel_PaperProfile viewModel_PaperProfile)
        {
            if (viewModel_PaperProfile == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_PaperProfile.ViewModel_Paper == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (!ModelState.IsValid)
            {
                ViewData["error"] = "Please check below item(s), again";
                return View(viewModel_PaperProfile);
            }
            //  Save
            Cmn cmn = new Cmn();
            cmn.UserName = "TPC Accept Confirm Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            //  Save as Authors
            if (viewModel_PaperProfile.ViewModel_Paper != null)
            {
                foreach (ViewModel_Paper item in viewModel_PaperProfile.ViewModel_Paper)
                {
                    if (item.Paper.ID != null && item.Paper.DateTPC == null)
                    {
                        Paper paper = db.Papers.Find(item.Paper.ID);
                        // ***  Non-system attribute (Begin)    *** //
                        paper.DateTPC = cmn.CurrentDatetime;
                        paper.UserTPC = cmn.UserName;
                        // ***  Non-system attribute (Finish)   *** //
                        //  Set system log
                        paper.DateEdit = cmn.CurrentDatetime;
                        paper.UserEdit = cmn.UserName;
                        db.Entry(paper).State = EntityState.Modified;
                    }
                }
            }
            //  Update database
            db.SaveChanges();
            String ctrlBtn = Request.Form["ctrlBtn"];
            if (ctrlBtn == "Submit")
            { return RedirectToAction("TPCIndex", "Papers", null); }
            return RedirectToAction("", "Home", null);
        }



        public ViewModel_Paper GetViewModel(int? id)
        {
            ViewModel_Paper viewModel_Paper = new ViewModel_Paper();
            //  Return as record null
            if (id == null) { return viewModel_Paper; }

            viewModel_Paper = db.Papers.Where(p => p.ID == id).Select(p =>
                                                      new ViewModel_Paper()  {
                                                            Conference = p.Conference
                                                          , Paper = p
                                                          , EditAuthors = (db.Authors.Where(a => a.PaperID == p.ID).ToList())
                                                          , EditObservations = (db.Observations.Where(o => o.PaperID == p.ID).ToList())
                                                      }).FirstOrDefault();
            if (viewModel_Paper == null) { return viewModel_Paper; };
            if (viewModel_Paper.Paper == null) { return viewModel_Paper; };
            //  Calculate number of paid
            if (viewModel_Paper.EditAuthors == null) {
                viewModel_Paper.NumPaid = 0;
            }   else    {
                List<string> userAccounts = (db.Payments.Where(p => p.ConferenceID == viewModel_Paper.Paper.ConferenceID)
                                            .Where(p => p.ResultPayment == "Final" || p.MerchVPC != null)        //  Paid
                                            .Where(p => viewModel_Paper.EditAuthors.Select(a => a.UserAccount).Contains(p.UserAccount))
                                            .Select(p => p.UserAccount)
                                           ).Union(
                                            db.Committees.Where(c => c.ConferenceID == viewModel_Paper.Paper.ConferenceID)
                                            .Where(p => viewModel_Paper.EditAuthors.Select(a => a.UserAccount).Contains(p.UserAccount))
                                            .Select(p => p.UserAccount)
                                           ).ToList();
                viewModel_Paper.NumPaid = userAccounts.GroupBy(p => p).Count();
            };
            //  Link full name on authors
            if (viewModel_Paper.EditAuthors != null) {
                viewModel_Paper.ViewModel_Author = (from item in viewModel_Paper.EditAuthors
                                                    join p in db.UserInformations on item.UserAccount equals p.UserAccount into grpJoin_item
                                                    from grpJoin in grpJoin_item.DefaultIfEmpty()
                                                    select new ViewModel_Author()   {
                                                          Author = item
                                                        , UserInformation = grpJoin }
                                                   ).ToList();  };
            //  Link full name on reviewers
            if (viewModel_Paper.EditObservations != null)   {
                viewModel_Paper.ViewModel_Observation = (from item in viewModel_Paper.EditObservations
                                                         join p in db.UserInformations on item.UserAccount equals p.UserAccount into grpJoin_item
                                                         from grpJoin in grpJoin_item.DefaultIfEmpty()
                                                         select new ViewModel_Observation() {
                                                               Observation = item
                                                             , UserInformation = grpJoin    }
                                                        ).ToList(); };

            return viewModel_Paper;
        }

        public List<ViewModel_Paper> GetViewModels(int conferenceID, List<int> paperIDs)
        {
            List<ViewModel_Paper> viewModel_Papers = new List<ViewModel_Paper>();

            //  Nature of Payment
            List<string> qPaidAccounts = (db.Payments.Where(p => p.ConferenceID == conferenceID)
                                          .Where(p => p.ResultPayment == "Final" || p.MerchVPC != null)
                                          .Select(p => p.UserAccount)
                                         ).Union(
                                          db.Committees.Where(p => p.ConferenceID == conferenceID)
                                          .Where(p => p.PermissionGroup == "Chair" || p.PermissionGroup == "TPC" || p.PermissionGroup == "Speaker")
                                          .Select(p => p.UserAccount)
                                         ).GroupBy(p => p).FirstOrDefault().ToList();
            //  Paper
            List<Paper> papers = db.Papers.Where(p => p.ConferenceID == conferenceID).Where(p => paperIDs.Contains(p.ID)).ToList();

            //  Convert to view model
            viewModel_Papers = (from p in papers
                                select new ViewModel_Paper()    {
                                      Paper = p
                                    , NumPaid = (db.Authors.Where(a => a.PaperID == p.ID).Where(a => qPaidAccounts.Contains(a.UserAccount)).Count())
                                    , EditAuthors = (db.Authors.Where(a => a.PaperID == p.ID).ToList())
                                    , EditObservations = (db.Observations.Where(o => o.PaperID == p.ID).ToList())   }
                               ).ToList();

            return viewModel_Papers;
        }

        private List<SelectedListItem> GetDefaultItems(String iCategory)
        {
            int Index = 0;
            List<SelectedListItem> itemLists = new List<SelectedListItem>();
            if (iCategory == null) { return itemLists; }
            if (iCategory == "") { return itemLists; }

            var qLists = db.CommonDropLists.Where(p => p.Category == iCategory).OrderBy(p => p.SequenceNum).ToList();
            foreach (var qItem in qLists)
            {
                SelectedListItem itemList = new SelectedListItem();
                itemList.SequenceNumber = Index;
                itemList.ItemText = qItem.Text;
                itemList.ItemValue = qItem.Value;
                itemLists.Add(itemList);
                Index = Index + 1;
            }
            return itemLists;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
