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
    public class ConferencesController : Controller
    {
        private IEEEConferenceDBContext db = new IEEEConferenceDBContext();

        // GET: Conferences
        public ActionResult Index()
        {
            Cmn cmn = new Cmn();
            cmn.UserName = "User Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }

            //  Get user information
            ViewModel_ConferenceProfileSearch viewModel_ConferenceProfileSearch = new ViewModel_ConferenceProfileSearch();
            viewModel_ConferenceProfileSearch.UserInformation = db.UserInformations.Where(p => p.UserAccount==cmn.UserName).FirstOrDefault();
            viewModel_ConferenceProfileSearch.ViewModel_ConferenceProfiles = (db.Conferences
                                        .OrderByDescending(p => p.ID)
                                        .Select(p => new ViewModel_ConferenceProfile()  {
                                              Confernece = p
                                            , ControlDates = (db.ControlDates.Where(c => c.ConferenceID == p.ID)
                                                              .Where(c => c.PeriodFrom <= cmn.CurrentDatetime && c.PeriodTo <= cmn.CurrentDatetime)
                                                              .ToList())    }   )
                                       ).ToList();
             return View(viewModel_ConferenceProfileSearch);
            //  View Menu:
            //  1. Enabled Manage Paper when selected conference:                       Papers/Index
            //  2. Enabled Manage conference as TPC selected conference:                Conferences/Manage/5
            //  3. Enabled Create new conference as system administrator:               Conferences/Create
            //  4. Enabled Manage conferences as system administator:                   Conferences/IndexAdmin
            //  List Confernece
            //  5. Selected conference as Not null of controlDate and user information: Conferences/Select/5
            //  6. Linked to Details                                                    Conferences/Details/5
        }

        public ActionResult IndexAdmin()
        {
            Cmn cmn = new Cmn();
            cmn.UserName = "User Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            UserInformation userInformation = db.UserInformations.Where(p => p.UserAccount == cmn.UserName)
                                              .Where(p => p.PermissionEnabled == "Enabled")
                                              .Where(p => p.PermissionGroup== "Administrator")
                                              .FirstOrDefault();
            if (userInformation == null) { return RedirectToAction("", "Home", null); }

            //  Get user information
            ViewModel_ConferenceProfileSearch viewModel_ConferenceProfileSearch = new ViewModel_ConferenceProfileSearch();
            viewModel_ConferenceProfileSearch.UserInformation = userInformation;
            viewModel_ConferenceProfileSearch.ViewModel_ConferenceProfiles = (db.Conferences
                                       .OrderByDescending(p => p.ID)
                                       .Select(p => new ViewModel_ConferenceProfile()   {
                                             Confernece = p
                                           , ControlDates = (db.ControlDates.Where(c => c.ConferenceID == p.ID)
                                                             .Where(c => c.PeriodFrom <= cmn.CurrentDatetime && c.PeriodTo <= cmn.CurrentDatetime
                                                            ).ToList()) })
                                      ).ToList();
            return View(viewModel_ConferenceProfileSearch);
            //  View Menu:
            //  1. Enabled Return:                                          Conferences/Index
            //  2. Enabled Create new conference:                           Conferences/Create
            //  List Confernece
            //  3. Selected conference as Not null of controlDate:          Conferences/Manage/5
        }

        // GET: Conferences/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            Conference conference = db.Conferences.Find(id);
            if (conference == null) { return HttpNotFound(); };

            ViewModel_Conference viewModel_Conference = new ViewModel_Conference();
            viewModel_Conference = GetViewModel(id);

            return View(viewModel_Conference);
            //  View Menu:
            //  1. Enabled Return:                                          Conferences/Index
            //  List all
        }

        // GET: Conferences/Manage/5
        public ActionResult Manage(int? id)
        {
            Cmn cmn = new Cmn();
            cmn.UserName = "User Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            UserInformation userInformation = db.UserInformations.Where(p => p.UserAccount == cmn.UserName)
                                              .Where(p => p.PermissionEnabled == "Enabled")
                                              .FirstOrDefault();
            if (userInformation == null) { return RedirectToAction("", "Home", null); }

            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            Conference conference = db.Conferences.Find(id);
            if (conference == null) { return HttpNotFound(); };
            ViewModel_Conference viewModel_Conference = new ViewModel_Conference();
            viewModel_Conference = GetViewModel(id);

            //  System administator
            if (userInformation.PermissionGroup == "Administrator") { return View(viewModel_Conference); }
            //  Return to home as not selected conference
            if (userInformation.SelectedConference != viewModel_Conference.Confernece.ID) { return RedirectToAction("", "Home", null); }
            
            Committee committee = db.Committees.Where(p => p.ConferenceID == viewModel_Conference.Confernece.ID)
                                  .Where(p => p.UserAccount == userInformation.UserAccount)
                                  .FirstOrDefault();
            //  Chair account
            if (committee.PermissionGroup == "Chair") { return View(viewModel_Conference); }
            //  TPC account
            if (committee.PermissionGroup == "Chair") { return View(viewModel_Conference); }

            return RedirectToAction("", "Home", null);
            //  View Menu:
            //  1. Enabled Return:                                          Conferences/Index
            //  2. Enabled Edit:                                            Conferences/Edit/5
            //  4. Enabled Delete conference as Not included below items:   Conferences/Delete/5
            //  5. Enabled Import committee list:                           Conferences/Committee/5
            //  6. Enabled Price item list:                                 Conferences/PricesPresetting/5
            //  7. Enabled Configure Virtual Payment Client (VPC):          Conferences/VPCConfigure/5
            //  8. Enabled Upload mdia:                                     Conferences/UploadMedia/5
            //  9. Enabled Close conference:                                Conferences/Close/5
            //  List All
        }

        // GET: Conferences/Create
        public ActionResult Create()
        {
            Conference conference = new Conference();
            return View(conference);
        }

        // POST: Conferences/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Created([Bind(Include = "ID,Code,Category,ConferenceYear")] Conference conference)
        {
            Cmn cmn = new Cmn();
            cmn.UserName = "Create Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            UserInformation userInformation = db.UserInformations.Where(p => p.UserAccount == cmn.UserName)
                                              .Where(p => p.PermissionEnabled == "Enabled")
                                              .FirstOrDefault();
              if (userInformation == null) { return RedirectToAction("", "Home", null); }
              if (userInformation.PermissionGroup == null) { return RedirectToAction("", "Home", null); }
              if (userInformation.PermissionGroup != "Administrator") { return RedirectToAction("", "Home", null); }

            if (conference == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (ModelState.IsValid == false) { return View(conference); }

            //  Either one time or in series
            if (conference.Code == null && conference.Category == null && conference.ConferenceYear == null) { return View(conference); };
            if (conference.Code == null && conference.Category == null && conference.ConferenceYear != null) { return View(conference); };
            if (conference.Code == null && conference.Category != null && conference.ConferenceYear == null) { return View(conference); };
            //  if (conference.Code == null && conference.Category != null && conference.ConferenceYear != null) { return View(conference); };
            //  if (conference.Code != null && conference.Category == null && conference.ConferenceYear == null) { return View(conference); };
            if (conference.Code != null && conference.Category == null && conference.ConferenceYear != null) { return View(conference); };
            if (conference.Code != null && conference.Category != null && conference.ConferenceYear == null) { return View(conference); };
            if (conference.Code != null && conference.Category != null && conference.ConferenceYear != null) { return View(conference); };

            //  Validate conference code as unique
            string conferneceCode = conference.Code;
            if (conference.ConferenceYear != null) {
                if (cmn.IsInteger(conference.ConferenceYear) == false)  { return View(conference);  }   
                conferneceCode = conference.Category + cmn.CnvString(conference.ConferenceYear.ToString(), 2);
            };
            int numRecord = db.Conferences.Count(p => p.Code == conferneceCode);
            if (numRecord != 0) { return View(conference); }
            //  Set system log
            conference.DateCreate = cmn.CurrentDatetime;
            conference.UserCreate = cmn.UserName;
            conference.DateEdit = cmn.CurrentDatetime;
            conference.UserEdit = cmn.UserName;
            db.Conferences.Add(conference);
            db.SaveChanges();

            //  Generate control date
            List<CommonDropList> commonDropLists = db.CommonDropLists.Where(p => p.Category == "ImportantDateOfConference").OrderBy(p => p.SequenceNum).ToList();
            if (commonDropLists != null)  {
                foreach (CommonDropList commonDropList in commonDropLists)
                {
                    ControlDate controlDate = new ControlDate();
                    controlDate.ConferenceID = conference.ID;
                    controlDate.Category = commonDropList.Value;
                    controlDate.Explanation = cmn.CnvString(commonDropList.SequenceNum.ToString(), 2) + " " + commonDropList.Text;
                    controlDate.DateCreate = cmn.CurrentDatetime;
                    controlDate.UserCreate = cmn.UserName;
                    controlDate.DateEdit = cmn.CurrentDatetime;
                    controlDate.UserEdit = cmn.UserName;
                    db.ControlDates.Add(controlDate);
                }
            }

            return RedirectToAction("Edit", "Conferences", new { id = conference.ID });
        }

        // GET: Conferences/Edit/5
        public ActionResult Edit(int? id)
        {
            Cmn cmn = new Cmn();
            cmn.UserName = "User Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            UserInformation userInformation = db.UserInformations.Where(p => p.UserAccount == cmn.UserName)
                                              .Where(p => p.PermissionEnabled == "Enabled")
                                              .FirstOrDefault();
            if (userInformation == null) { return RedirectToAction("", "Home", null); }

            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); } 
            Conference conference = db.Conferences.Find(id);
            if (conference == null) { return HttpNotFound(); };
            if (conference.DateClose != null) { return RedirectToAction("Details", "Conferences", new { id = conference.ID }); }

            ViewModel_Conference viewModel_Conference = new ViewModel_Conference();
            viewModel_Conference = GetViewModel(id);

            //  Select persentation type
            List<SelectedListItem> selectStyles = new List<SelectedListItem>();
            List<CommonDropList> commonDropLists = db.CommonDropLists.Where(p => p.Category == "StyleType").OrderBy(p => p.SequenceNum).ToList();
            if (commonDropLists != null) {
                foreach(CommonDropList commonDropList in commonDropLists) {
                    SelectedListItem selectStyle = new SelectedListItem();
                    selectStyle.SequenceNumber = commonDropList.SequenceNum;
                    selectStyle.ItemText = commonDropList.Text;
                    selectStyle.ItemValue = commonDropList.Value;
                    selectStyles.Add(selectStyle);
                }
            }
            viewModel_Conference.SelectStyles = selectStyles;

            //  System administator
            if (userInformation.PermissionGroup == "Administrator") { return View(viewModel_Conference); }
            //  Return to home as not selected conference
            if (userInformation.SelectedConference != viewModel_Conference.Confernece.ID) { return RedirectToAction("", "Home", null); }

            Committee committee = db.Committees.Where(p => p.ConferenceID == viewModel_Conference.Confernece.ID)
                                  .Where(p => p.UserAccount == userInformation.UserAccount)
                                  .FirstOrDefault();
            //  Chair account
            if (committee.PermissionGroup == "Chair") { return View(viewModel_Conference); }
            //  TPC account
            if (committee.PermissionGroup == "Chair") { return View(viewModel_Conference); }

            return RedirectToAction("", "Home", null);
        }

        // POST: Conferences/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edited(ViewModel_Conference viewModel_Conference, HttpPostedFileBase docFile)
        {
            if (viewModel_Conference == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_Conference.Confernece == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_Conference.Confernece.ID == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (!ModelState.IsValid)
            {
                ViewData["error"] = "Please check below item(s), again";
                return View(viewModel_Conference);
            }
            //  Save
            //  Procedure of form edition
            Cmn cmn = new Cmn();
            cmn.UserName = "Edit Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            Conference conference = db.Conferences.Find(viewModel_Conference.Confernece.ID);
            //  Save as Fields / Technical / Area
            if (viewModel_Conference.Fields != null)
            {
                foreach (Field item in viewModel_Conference.Fields)
                {
                    Field field = db.Fields.Find(item.ID);
                    // ***  Non-system attribute (Begin)    *** //
                    field.Code = item.Code;
                    field.Description = item.Description;
                    // ***  Non-system attribute (Finish)   *** //
                    //  Set system log
                    field.DateEdit = cmn.CurrentDatetime;
                    field.UserEdit = cmn.UserName;
                    db.Entry(field).State = EntityState.Modified;
                }
            }
            //  Save as control date
            if (viewModel_Conference.ControlDates != null)
            {
                foreach (ControlDate item in viewModel_Conference.ControlDates)
                {
                    ControlDate controlDate = db.ControlDates.Find(item.ID);
                    // ***  Non-system attribute (Begin)    *** //
                    controlDate.Description = item.Description;
                    controlDate.PeriodFrom = item.PeriodFrom;
                    controlDate.PeriodTo = item.PeriodTo;
                    // ***  Non-system attribute (Finish)   *** //
                    //  Set system log
                    controlDate.DateEdit = cmn.CurrentDatetime;
                    controlDate.UserEdit = cmn.UserName;
                    db.Entry(controlDate).State = EntityState.Modified;
                }
            }
            // ***  Non-system attribute (Begin)    *** //
            conference.Title = viewModel_Conference.Confernece.Title;
            conference.PeriodFrom = viewModel_Conference.Confernece.PeriodFrom;
            conference.PeriodTo = viewModel_Conference.Confernece.PeriodTo;
            conference.Venue = viewModel_Conference.Confernece.Venue;
            conference.City = viewModel_Conference.Confernece.City;
            conference.Country = viewModel_Conference.Confernece.Country;
            conference.HomeUrl = viewModel_Conference.Confernece.HomeUrl;
            conference.Contact = viewModel_Conference.Confernece.Contact;
            conference.Style = viewModel_Conference.Confernece.Style;
            if (docFile != null)
            {
                try
                {
                    var fileName = cmn.GetFileName("Conference", "Poster", conference.ID.ToString(), "", conference.Code.ToString(), Path.GetExtension(docFile.FileName).ToString());
                    var filePath = Path.Combine(Server.MapPath(cmn.GetStoredFilePath()), fileName);
                    docFile.SaveAs(filePath);
                    conference.FileNamePoster = Path.GetFileName(docFile.FileName);
                    conference.FilePathPoster = filePath.ToString();
                }
                catch
                {
                    ViewData["uploadMsg"] = "Upload failed";
                    return View(viewModel_Conference);
                }
            }
            // ***  Non-system attribute (Finish)   *** //
            //  Set system log
            conference.DateEdit = cmn.CurrentDatetime;
            conference.UserEdit = cmn.UserName;
            db.Entry(conference).State = EntityState.Modified;
            //  Update database
            db.SaveChanges();
            String ctrlBtn = Request.Form["ctrlBtn"];
            if (ctrlBtn == "Refresh")
            { return RedirectToAction("Edit", "Conferences", new { id = conference.ID }); }
            if (ctrlBtn == "Add Item")
            {
                //  Set row number
                String numRecord = db.Fields.Count(p => p.ConferenceID == conference.ID).ToString();
                String maxVal = db.Fields.Where(p => p.ConferenceID == conference.ID).Max(p => p.RowNum).ToString();
                Field newItem = new Field();
                newItem.ConferenceID = conference.ID;
                newItem.RowNum = cmn.CnvInteger(cmn.GetVersionID(numRecord.ToString(), maxVal, 2));
                newItem.DateCreate = cmn.CurrentDatetime;
                newItem.UserCreate = cmn.UserName;
                newItem.DateEdit = cmn.CurrentDatetime;
                newItem.UserEdit = cmn.UserName;
                db.Fields.Add(newItem);
                db.SaveChanges();
                return RedirectToAction("Edit", "Conferences", new { id = conference.ID });
            }
            if (ctrlBtn == "Submit")
            {
                List<Field> removeItems = db.Fields.Where(p => p.ConferenceID == conference.ID)
                                              .Where(p => p.Code == null && p.Description == null)
                                              .ToList();
                if (removeItems == null) { return RedirectToAction("Details", "Conferences", new { id = conference.ID }); };
                foreach (Field removeItem in removeItems)
                {
                    db.Fields.Remove(removeItem);
                }
                db.SaveChanges();
                return RedirectToAction("Details", "Conferences", new { id = conference.ID });
            }
            return RedirectToAction("", "Home", null);
        }

        // GET: Conferences/VPCConfigure/5
        //  Virtual Payment Client (VPC) on credit card
        public ActionResult VPCConfigure(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            Conference conference = db.Conferences.Find(id);
            if (conference == null) { return HttpNotFound(); };
            if (conference.DateClose != null) { return RedirectToAction("Details", "Conferences", new { id = conference.ID }); }

            ViewModel_ConferenceVPC viewModel_ConferenceVPC = new ViewModel_ConferenceVPC();
            viewModel_ConferenceVPC.Confernece = conference;

            //  Select persentation type
            List<ConfigureVPC> ConfigureVPCs = db.ConfigureVPCs.Where(p => p.ConferenceID == conference.ID).OrderBy(p => p.Explanation).ToList();
            viewModel_ConferenceVPC.ConfigureVPCs = ConfigureVPCs;

            return View(viewModel_ConferenceVPC);
        }

        // POST: Conferences/ConfigureVPC/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("VPCConfigure")]
        [ValidateAntiForgeryToken]
        public ActionResult VPCConfirmed(ViewModel_ConferenceVPC viewModel_ConferenceVPC)
        {
            if (viewModel_ConferenceVPC == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_ConferenceVPC.Confernece == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_ConferenceVPC.Confernece.ID == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (!ModelState.IsValid)
            {
                ViewData["error"] = "Please check below item(s), again";
                return View(viewModel_ConferenceVPC);
            }
            //  Save
            //  Procedure of form edition
            Cmn cmn = new Cmn();
            cmn.UserName = "VPC Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            Conference conference = db.Conferences.Find(viewModel_ConferenceVPC.Confernece.ID);
            //  Save as VPC
            if (viewModel_ConferenceVPC.ConfigureVPCs != null)
            {
                foreach (ConfigureVPC item in viewModel_ConferenceVPC.ConfigureVPCs)
                {
                    ConfigureVPC configureVPC = db.ConfigureVPCs.Find(item.ID);
                    // ***  Non-system attribute (Begin)    *** //
                    configureVPC.Unit = item.Unit;
                    configureVPC.Title = item.Title;
                    configureVPC.Explanation = item.Explanation;
                    configureVPC.Name = item.Name;
                    configureVPC.Value = item.Value;
                    // ***  Non-system attribute (Finish)   *** //
                    //  Set system log
                    configureVPC.DateEdit = cmn.CurrentDatetime;
                    configureVPC.UserEdit = cmn.UserName;
                    db.Entry(configureVPC).State = EntityState.Modified;
                }
            }
            //  Update database
            db.SaveChanges();
            String ctrlBtn = Request.Form["ctrlBtn"];
            if (ctrlBtn == "Refresh")
            { return RedirectToAction("VPCConfigure", "Conferences", new { id = conference.ID }); }
            if (ctrlBtn == "Submit")
            { return RedirectToAction("Details", "Conferences", new { id = conference.ID }); }
            return RedirectToAction("", "Home", null);
        }

        // GET: Conferences/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            Conference conference = db.Conferences.Find(id);
            if (conference == null) { return HttpNotFound(); };

            ViewModel_Conference viewModel_Conference = new ViewModel_Conference();
            viewModel_Conference = GetViewModel(id);

            return View(viewModel_Conference);
        }

        // POST: Conferences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

            Cmn cmn = new Cmn();
            cmn.UserName = "Confirm Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            Conference conference = db.Conferences.Find(id);
            if (conference == null) { return RedirectToAction("", "Home", null); }

            //  Return as has paper and/or payment
            int numRecord = 0;
            numRecord = db.Papers.Count(p => p.ConferenceID == conference.ID);
            if (numRecord > 0) { return RedirectToAction("Delete", "Conferences", new { id = conference.ID }); }
            numRecord = db.Payments.Count(p => p.ConferenceID == conference.ID);
            if (numRecord > 0) { return RedirectToAction("Delete", "Conferences", new { id = conference.ID }); }

            //  Remove conference details
            //  Remove committees
            foreach (Committee removeItem in db.Committees.Where(p => p.ConferenceID == conference.ID))
            { db.Committees.Remove(removeItem); };
            //  Remove ConfigureVPCs
            foreach (ConfigureVPC removeItem in db.ConfigureVPCs.Where(p => p.ConferenceID == conference.ID))
            { db.ConfigureVPCs.Remove(removeItem); };
            //  Remove PriceItems
            foreach (PriceItem removeItem in db.PriceItems.Where(p => p.ConferenceID == conference.ID))
            { db.PriceItems.Remove(removeItem); };
            //  Remove s
            foreach (Field removeItem in db.Fields.Where(p => p.ConferenceID == conference.ID))
            { db.Fields.Remove(removeItem); };
            //  Remove ControlDates
            foreach (ControlDate removeItem in db.ControlDates.Where(p => p.ConferenceID == conference.ID))
            { db.ControlDates.Remove(removeItem); };
            //  Remove Conferences
            foreach (Conference removeItem in db.Conferences.Where(p => p.ID == conference.ID))
            { db.Conferences.Remove(removeItem); };
            db.SaveChanges();

            return RedirectToAction("Details", "Conferences", new { id = conference.ID });
        }

        // GET: Conferences/Select/5
        public ActionResult Select(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            Conference conference = db.Conferences.Find(id);
            if (conference == null) { return HttpNotFound(); };

            ViewModel_Conference viewModel_Conference = new ViewModel_Conference();
            viewModel_Conference = GetViewModel(id);

            return View(viewModel_Conference);
        }

        // POST: Conferences/Select/5
        [HttpPost, ActionName("Select")]
        [ValidateAntiForgeryToken]
        public ActionResult Selected(int id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

            Cmn cmn = new Cmn();
            cmn.UserName = "Confirm Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            Conference conference = db.Conferences.Find(id);
            if (conference == null) { return RedirectToAction("", "Home", null); }

            UserInformation userInformation = db.UserInformations.Where(p =>p.UserAccount == cmn.UserName && p.PermissionEnabled == "Enabled").FirstOrDefault();
            if (userInformation == null) { return RedirectToAction("", "Home", null); }
            userInformation.SelectedConference = id;
            db.Entry(userInformation).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "Papers", new { id = conference.ID });
        }

        // GET: Conferences/UploadMedia/5
        public ActionResult UploadMedia(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            Conference conference = db.Conferences.Find(id);
            if (conference == null) { return HttpNotFound(); };

            ViewModel_Conference viewModel_Conference = new ViewModel_Conference();
            viewModel_Conference = GetViewModel(id);

            return View(viewModel_Conference);
        }

        // POST: Conferences/UploadMedia/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadMedia(ViewModel_Conference viewModel_Conference, HttpPostedFileBase docFile)
        {
            if (viewModel_Conference == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_Conference.Confernece == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_Conference.Confernece.ID == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (!ModelState.IsValid)
            {
                ViewData["error"] = "Please check below item(s), again";
                return View(viewModel_Conference);
            }
            //  Save
            //  Procedure of form edition
            Cmn cmn = new Cmn();
            cmn.UserName = "Edit Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            Conference conference = db.Conferences.Find(viewModel_Conference.Confernece.ID);
            if (docFile != null)
            {
                try
                {
                    var fileName = cmn.GetFileName("Conference", "Media", conference.ID.ToString(), "", conference.Code.ToString(), Path.GetExtension(docFile.FileName).ToString());
                    var filePath = Path.Combine(Server.MapPath(cmn.GetStoredFilePath()), fileName);
                    docFile.SaveAs(filePath);
                    conference.FileNameMedia = Path.GetFileName(docFile.FileName);
                    conference.FilePathMedia = filePath.ToString();
                }
                catch
                {
                    ViewData["uploadMsg"] = "Upload failed";
                    viewModel_Conference = GetViewModel(viewModel_Conference.Confernece.ID);
                    return View(viewModel_Conference);

                }
            }
            // ***  Non-system attribute (Finish)   *** //
            //  Set system log
            conference.DateEdit = cmn.CurrentDatetime;
            conference.UserEdit = cmn.UserName;
            db.Entry(conference).State = EntityState.Modified;
            //  Update database
            db.SaveChanges();
            String ctrlBtn = Request.Form["ctrlBtn"];
            if (ctrlBtn == "Refresh")
            { return RedirectToAction("UploadMedia", "Conferences", new { id = conference.ID }); }
            if (ctrlBtn == "Submit")
            { return RedirectToAction("Details", "Conferences", new { id = conference.ID }); }
            return RedirectToAction("", "Home", null);
        }

        // GET: Conferences/Close/5
        public ActionResult Close(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            Conference conference = db.Conferences.Find(id);
            if (conference == null) { return HttpNotFound(); };

            ViewModel_Conference viewModel_Conference = new ViewModel_Conference();
            viewModel_Conference = GetViewModel(id);

            return View(viewModel_Conference);
        }

        // POST: Conferences/Close/5
        [HttpPost, ActionName("Close")]
        [ValidateAntiForgeryToken]
        public ActionResult CloseConfirmed(ViewModel_Conference viewModel_Conference)
        {
            if (viewModel_Conference == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_Conference.Confernece == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            if (viewModel_Conference.Confernece.ID == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

            Cmn cmn = new Cmn();
            cmn.UserName = "Revoke Account";
            if (User.Identity.IsAuthenticated) { cmn.UserName = User.Identity.Name; }
            Conference conference = db.Conferences.Find(viewModel_Conference.Confernece.ID);
            if (conference == null) { return RedirectToAction("", "Home", null); }

            //  Close conference
            conference.RevokeReason = viewModel_Conference.Confernece.RevokeReason;
            conference.DateClose = cmn.CurrentDatetime;
            conference.UserClose = cmn.UserName;

            db.SaveChanges();

            return RedirectToAction("Details", "Conferences", new { id = conference.ID });
        }


        public ViewModel_Conference GetViewModel(int? id)
        {
            ViewModel_Conference viewModel_Conference = new ViewModel_Conference();
            //  Return as record null
            if (id == null) { return viewModel_Conference; }
            Conference conference = db.Conferences.Find(id);
            if (conference == null) { return viewModel_Conference; }
            Cmn cmn = new Cmn();

            viewModel_Conference.Confernece = conference;
            List<ControlDate> controlDates = db.ControlDates.Where(p => p.ConferenceID == conference.ID).OrderBy(p => p.Description).ToList();
            viewModel_Conference.ControlDates = controlDates;
            List<Field> Fields = db.Fields.Where(p => p.ConferenceID == conference.ID).OrderBy(p => p.RowNum).ToList();
            viewModel_Conference.Fields = Fields;
            //  Disabled
            //  List<ConfigureVPC> ConfigureVPCs = db.ConfigureVPCs.Where(p => p.ConferenceID == conference.ID).OrderBy(p => p.Explanation).ToList();
            //  viewModel_Conference.ConfigureVPCs = ConfigureVPCs;
            viewModel_Conference.NumberOfSubmittedPaper = db.Papers.Count(p => p.ConferenceID == conference.ID);
            viewModel_Conference.NumberOfSubmittedPayment = db.Payments.Count(p => p.ConferenceID == conference.ID);

            return viewModel_Conference;
        }

        //  Do it
        // GET: Conferences/Committee/5
        // POST: Conferences/Committee/5
        // GET: Conferences/PricesPresetting/5
        // POST: Conferences/PricesPresetting/5


        protected override void Dispose(bool disposing)
        {
            if (disposing)  {   db.Dispose();   }
            base.Dispose(disposing);
        }


        
    }
}
