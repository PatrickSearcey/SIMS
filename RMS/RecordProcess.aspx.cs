﻿using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RMS
{
    public partial class RecordProcess : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private Data.Record currRecord;
        private string task { get; set; }
        private Boolean HasEditAccess { get; set; }
        private int? RecordID
        {
            get
            {
                if (Session["RecordID"] == null) return 0; else return (int)Session["RecordID"];
            }
            set
            {
                Session["RecordID"] = value;
            }
        }
        private int? PeriodID
        {
            get
            {
                if (Session["PeriodID"] == null) return 0; else return (int)Session["PeriodID"];
            }
            set
            {
                Session["PeriodID"] = value;
            }
        }
        private int WSCID
        {
            get
            {
                if (Session["WSCID"] == null) return 0; else return (int)Session["WSCID"];
            }
            set
            {
                Session["WSCID"] = value;
            }
        }
        private int OfficeID
        {
            get
            {
                if (Session["OfficeID"] == null) return 0; else return (int)Session["OfficeID"];
            }
            set
            {
                Session["OfficeID"] = value;
            }
        }
        private Boolean Locked
        {
            get
            {
                Boolean ret = false;

                var locks = currRecord.RecordLock;
                //If there is a record in the Record Lock table, then go ahead and compare the lock_uid to the user.ID
                if (locks != null)
                {
                    if (user.ID != locks.lock_uid) ret = true;
                }

                return ret;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            string period_id =  Request.QueryString["period_id"];
            string rms_record_id = Request.QueryString["rms_record_id"];
            task = Request.QueryString["task"];

            //If no rms_record_id or period_id was passed, then redirect back to the homepage
            if (!string.IsNullOrEmpty(period_id) || !string.IsNullOrEmpty(rms_record_id))
            {
                if (!string.IsNullOrEmpty(period_id)) PeriodID = Convert.ToInt32(period_id);
                if (!string.IsNullOrEmpty(rms_record_id)) RecordID = Convert.ToInt32(rms_record_id);

                if (RecordID == 0) RecordID = Convert.ToInt32(db.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID).rms_record_id);
            }
            else Response.Redirect(Config.SIMS2017URL + "SIMSWSCHome.aspx");

            //Using the passed rms_record_id, setup the record data element, and reset the office and wsc to match that of the current record
            currRecord = db.Records.Where(p => p.rms_record_id == RecordID).FirstOrDefault();
            if (currRecord.RecordAltOffice == null) OfficeID = (int)currRecord.Site.office_id; else OfficeID = (int)currRecord.RecordAltOffice.alt_office_id;
            WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            UserControlSetup();
            
            if (!Page.IsPostBack)
            {
                //If the user belongs to this site's WSC (or has an exception to work in the WSC), or is a SuperUser, then allow them to edit the page
                if (user.WSCID.Contains(WSCID) || user.IsSuperUser) HasEditAccess = true; else HasEditAccess = false;

                //Start with a cleared out PeriodID session state variable
                if (string.IsNullOrEmpty(period_id)) PeriodID = 0;

                PopulatePageData();
                if (!Locked)
                {
                    CreateLock(task);
                    SetupPermission();
                }
            }
        }

        #region Page Load Methods
        protected void UserControlSetup()
        {
            ph1.Title = task + " Record";
            ph1.SubTitle = currRecord.Site.site_no + " " + currRecord.Site.station_full_nm;
            ph1.RecordType = currRecord.RecordType.type_ds + " Record for";
        }

        protected void CreateLock(string lock_type)
        {
            if (currRecord.RecordLock == null)
            {
                db.RecordLocks.InsertOnSubmit(new Data.RecordLock
                {
                    rms_record_id = currRecord.rms_record_id,
                    period_id = PeriodID,
                    lock_type = lock_type,
                    lock_dt = DateTime.Now,
                    lock_uid = user.ID
                });
            }
            else
            {
                currRecord.RecordLock.period_id = PeriodID;
                currRecord.RecordLock.lock_type = lock_type;
                currRecord.RecordLock.lock_dt = DateTime.Now;
                currRecord.RecordLock.lock_uid = user.ID;
            }
            db.SubmitChanges();
        }

        protected void PopulatePageData()
        {
            if (Locked)
            {
                pnlLocked.Visible = true;
                pnlAnalyze.Visible = false;
                pnlApprove.Visible = false;
                SetupLockedPanel();
            }
            else
            {
                if (task == "Analyze")
                {
                    pnlLocked.Visible = false;
                    pnlAnalyze.Visible = true;
                    pnlApprove.Visible = false;
                    SetupAnalyzePanel();
                }
                else
                {
                    pnlLocked.Visible = false;
                    pnlAnalyze.Visible = false;
                    pnlApprove.Visible = true;
                    SetupApprovePanel();
                }
            }
        }

        protected void SetupLockedPanel()
        {
            var locks = currRecord.RecordLock;

            ltlLockType.Text = locks.lock_type;
            ltlLockBy.Text = locks.lock_uid;
            ltlLockDt.Text = String.Format("{0:MM/dd/yyyy}", locks.lock_dt);

            if (user.IsAdmin || user.IsSuperUser)
            {
                if (locks.lock_type == "Analyzing" || locks.lock_type == "Approving") pnlUnlock.Visible = true; else pnlUnlock.Visible = false;
            }
            else pnlUnlock.Visible = false;
        }

        protected void SetupAnalyzePanel()
        {
            ltlHydrographer.Text = "<b>" + user.ID + "</b>";
            hlWYAnalysisNotes.NavigateUrl = String.Format("javascript:OpenPopup('Modal/ReportPopup.aspx?view=wyanalysisnotes&rms_record_id={0}')", RecordID);
            hlInstructions.NavigateUrl = "javascript:OpenPopup('Modal/Instructions.aspx?type=Analyze')";
            string swr_url = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).swr_url;
            if (!string.IsNullOrEmpty(swr_url))
            {
                hlAutoReview.NavigateUrl = String.Format("javascript:OpenPopup('{0}{1}/')", swr_url, currRecord.Site.site_no.Replace(" ", ""));
            }
            else hlAutoReview.Visible = false;

            if (PeriodID > 0)
            {
                var period = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID);

                rdpBeginDateAnalyze.SelectedDate = period.period_beg_dt;
                rdpEndDateAnalyze.SelectedDate = period.period_end_dt;

                //If analyzing the only period in the Analysis Periods table for this record, then allow the begin date to be edited
                if (currRecord.RecordAnalysisPeriods.Count == 1) rdpBeginDateAnalyze.Enabled = true; else rdpBeginDateAnalyze.Enabled = false;

                //Use the current period's end date, and if there is no period found with a matching begin date, then allow the end date to be edited
                if (currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_beg_dt == period.period_end_dt) == null) rdpEndDateAnalyze.Enabled = true; else rdpEndDateAnalyze.Enabled = false;
                
                //If a previous period is found, then populate the analysis notes from previous period
                var prevPeriod = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_end_dt == period.period_beg_dt);
                if (prevPeriod != null) rtbPrevAnalysisNotes.Text = Format(prevPeriod.analysis_notes_va); else rtbPrevAnalysisNotes.Text = "No previous period found.";

                reAnalysisNotes.Content = period.analysis_notes_va;
            }
            else
            {
                //Analyzing a new period, but check to see if there have been other periods started for this record
                var analysis_status = currRecord.RecordAnalysisStatus;
                if (analysis_status != null)
                {
                    rdpBeginDateAnalyze.SelectedDate = analysis_status.analyzed_period_dt;
                    rdpBeginDateAnalyze.Enabled = false;
                    rtbPrevAnalysisNotes.Text = Format(currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_end_dt == analysis_status.analyzed_period_dt).analysis_notes_va);
                }
                else //Totally new, adding first period ever to record
                {
                    rtbPrevAnalysisNotes.Text = "No previous period found.";
                }
            }

        }

        /// <summary>
        /// The Approve panel is used for both approving and reanalyzing
        /// </summary>
        protected void SetupApprovePanel()
        {
            var currPeriod = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID);

            if (currPeriod == null) 
                Response.Redirect(Config.SIMS2017URL + "SIMSWSCHome.aspx");
            else
            {
                ltlAnalyzedBy.Text = "<b>" + currPeriod.analyzed_by + "</b>";
                if (!string.IsNullOrEmpty(currPeriod.approved_by)) ltlApprover.Text = "<b>" + currPeriod.approved_by + "</b>"; else ltlApprover.Text = "<b>" + user.ID + "</b>";
                ltlTimePeriod.Text = String.Format("<b>{0:MM/dd/yyyy} - {1:MM/dd/yyyy}</b>", currPeriod.period_beg_dt, currPeriod.period_end_dt);
                hlWYAnalysisNotes2.NavigateUrl = String.Format("javascript:OpenPopup('Modal/ReportPopup.aspx?view=wyanalysisnotes&rms_record_id={0}')", RecordID);
                hlDialog.NavigateUrl = String.Format("javascript:OpenPopup('Modal/ReportPopup.aspx?view=dialog&period_id={0}')", PeriodID);
                hlChangeLog.NavigateUrl = String.Format("javascript:OpenPopup('Modal/ReportPopup.aspx?view=changelog&period_id={0}')", PeriodID);
                hlApproveInst.NavigateUrl = "javascript:OpenPopup('Modal/Instructions.aspx?type=Analyze')";
                string swr_url = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).swr_url;
                if (!string.IsNullOrEmpty(swr_url))
                {
                    hlAutoReview2.NavigateUrl = String.Format("javascript:OpenPopup('{0}{1}/')", swr_url, currRecord.Site.site_no.Replace(" ", ""));
                }
                else hlAutoReview2.Visible = false;
                pnlAnalysisNotesEdit.Visible = false;
                pnlAnalysisNotesReadOnly.Visible = true;
                rtbAnalysisNotes.Text = Format(currPeriod.analysis_notes_va);
                reAnalysisNotes2.Content = currPeriod.analysis_notes_va;
                if (currPeriod.PeriodDialogs.FirstOrDefault(p => p.status_set_to_va == "Approving") != null)
                    reComments.Content = currPeriod.PeriodDialogs.Where(p => p.status_set_to_va == "Approving").OrderByDescending(p => p.dialog_dt).FirstOrDefault().comments_va;
                if (task == "Reanalyze")
                {
                    if (currPeriod.PeriodDialogs.FirstOrDefault(p => p.status_set_to_va == "Reanalyze") != null)
                        rtbApproverComments.Text = Format(currPeriod.PeriodDialogs.Where(p => p.status_set_to_va == "Reanalyze").OrderByDescending(p => p.dialog_dt).FirstOrDefault().comments_va);
                    pnlApproverComments.Visible = true;
                    rbFinish.Text = "Finish Reanalyzing";
                    rbFinish.CommandName = "Reanalyze";
                    rbSave.Visible = false;
                    rbReanalyze.Visible = false;
                }
                else
                {
                    rbFinish.Text = "Finish Approving";
                    pnlApproverComments.Visible = false;
                    rbFinish.CommandName = "Approve";
                    rbSave.CommandName = "Approve";
                }
            }
        }

        protected void SetupPermission()
        {
            if (!HasEditAccess) Response.Redirect(Config.SIMS2017URL + "SIMSWSCHome.aspx");
        }
        
        private string Format(string text)
        {
            text = text.Replace("<br />", "\n").Replace("</p>", "\n").Replace("<p>", "").Replace("<strong>", "").Replace("</strong>", "");

            return text;
        }
        #endregion

        #region Page Events
        /// <summary>
        /// Allows a SuperUser or WSC Admin the ability to unloack a record, but only if the lock is a "save" type - in other words, 
        /// if the lock is in place because a user is currently analyzing/approving the record, the record cannot be unlocked
        /// </summary>
        protected void lbUnlockPeriod_Command(object sender, CommandEventArgs e)
        {
            db.RecordLocks.DeleteOnSubmit(currRecord.RecordLock);
            db.SubmitChanges();

            PopulatePageData();
            CreateLock(task);
        }

        protected void Button_Commands(object sender, CommandEventArgs e)
        {
            switch (e.CommandName.ToString())
            {
                case "Analyze":
                    if (e.CommandArgument.ToString() == "Finish") FinishAnalyzingPeriod();
                    else if (e.CommandArgument.ToString() == "Save") SaveAnalyzingPeriod();
                    break;
                case "Reanalyze":
                    FinishReanalyzingPeriod();
                    break;
                case "Approve":
                    if (e.CommandArgument.ToString() == "Finish") FinishApprovingPeriod();
                    else if (e.CommandArgument.ToString() == "Save") SaveApprovingPeriod();
                    else if (e.CommandArgument.ToString() == "Reanalyze") SendBackToReanalyze();
                    break;
                case "Cancel":
                    CloseOutPage(false);
                    break;
            }
        }

        protected void EditAnalysisNotes(object sender, CommandEventArgs e)
        {
            switch (e.CommandArgument.ToString())
            {
                case "Toggle":
                    pnlAnalysisNotesEdit.Visible = true;
                    pnlAnalysisNotesReadOnly.Visible = false;
                    ltlNote.Visible = false;
                    break;
                case "Save":
                    //Save the analysis notes
                    var currPeriod = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID);
                    currPeriod.analysis_notes_va = reAnalysisNotes2.Content;

                    //Add an entry to the change log table
                    Data.PeriodChangeLog pcl = new Data.PeriodChangeLog
                    {
                        period_id = currPeriod.period_id,
                        edited_by_uid = user.ID,
                        edited_dt = DateTime.Now,
                        new_va = reAnalysisNotes2.Content
                    };
                    db.PeriodChangeLogs.InsertOnSubmit(pcl);
                    db.SubmitChanges();

                    rtbAnalysisNotes.Text = Format(reAnalysisNotes2.Content);
                    ltlNote.Visible = true;
                    pnlAnalysisNotesEdit.Visible = false;
                    pnlAnalysisNotesReadOnly.Visible = true;
                    break;
                case "Cancel":
                    pnlAnalysisNotesEdit.Visible = false;
                    pnlAnalysisNotesReadOnly.Visible = true;
                    break;
            }
        }
        #endregion

        #region Button Methods
        protected void CloseOutPage(bool finished)
        {
            string all = "true";
            if (!finished) all = "false";

            //Clear Locks
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("{0}Handler/ClearLock.ashx?user_id={1}&all={2}", Config.RMSURL, user.ID, all));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //Redirect back to Station Info page
            Response.Redirect(String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMS2017URL, currRecord.Site.site_id));
        }

        protected void FinishAnalyzingPeriod()
        {
            //First, do some validation
            Boolean valid = ValidateDates(rdpBeginDateAnalyze.SelectedDate, rdpEndDateAnalyze.SelectedDate);

            if (valid)
            {
                //If analyzing a period that already exists, just update
                if (PeriodID > 0)
                {
                    var period = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID);

                    period.period_end_dt = rdpEndDateAnalyze.SelectedDate;
                    period.status_va = "Analyzed";
                    period.status_set_by = user.ID;
                    period.status_set_by_role_va = "Analyzer";
                    period.analyzed_by = user.ID;
                    period.analyzed_dt = DateTime.Now;
                    period.analysis_notes_va = reAnalysisNotes.Content;

                    db.SubmitChanges();
                    AddDialog(period, "Analyzed", "Analyzer", "The period is finished being analyzed.");
                    AddChangeLog(period.period_id, reAnalysisNotes.Content);
                }
                else //Insert new period
                {
                    Data.RecordAnalysisPeriod new_period = new Data.RecordAnalysisPeriod
                    {
                        rms_record_id = currRecord.rms_record_id,
                        period_beg_dt = rdpBeginDateAnalyze.SelectedDate,
                        period_end_dt = rdpEndDateAnalyze.SelectedDate,
                        status_va = "Analyzed",
                        status_set_by = user.ID,
                        status_set_by_role_va = "Analyzer",
                        analyzed_by = user.ID,
                        analyzed_dt = DateTime.Now,
                        analysis_notes_va = reAnalysisNotes.Content
                    };
                    db.RecordAnalysisPeriods.InsertOnSubmit(new_period);
                    db.SubmitChanges();
                    PeriodID = new_period.period_id;
                    AddDialog(new_period, "Analyzed", "Analyzer", "The period is finished being Analyzed.");
                    AddChangeLog(new_period.period_id, reAnalysisNotes.Content);
                }

                SendEmails("Analyzed", "", currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID));
                CloseOutPage(true);
            }
            else
            {
                ErrorMessage("The period start and/or end dates are not valid!");
            }
        }
        
        protected void SaveAnalyzingPeriod()
        {
            //First, do some validation
            Boolean valid = ValidateDates(rdpBeginDateAnalyze.SelectedDate, rdpEndDateAnalyze.SelectedDate);

            if (valid)
            {
                //If analyzing a period that already exists, just update
                if (PeriodID > 0)
                {
                    var period = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID);

                    period.period_end_dt = rdpEndDateAnalyze.SelectedDate;
                    period.status_va = "Analyzing";
                    period.status_set_by = user.ID;
                    period.status_set_by_role_va = "Analyzer";
                    period.analyzed_by = user.ID;
                    period.analyzed_dt = DateTime.Now;
                    period.analysis_notes_va = reAnalysisNotes.Content;

                    db.SubmitChanges();
                    AddDialog(period, "Analyzing", "Analyzer", "The period was saved by the analyzer.");
                }
                else //Insert new period
                {
                    Data.RecordAnalysisPeriod new_period = new Data.RecordAnalysisPeriod
                    {
                        rms_record_id = currRecord.rms_record_id,
                        period_beg_dt = rdpBeginDateAnalyze.SelectedDate,
                        period_end_dt = rdpEndDateAnalyze.SelectedDate,
                        status_va = "Analyzing",
                        status_set_by = user.ID,
                        status_set_by_role_va = "Analyzer",
                        analyzed_by = user.ID,
                        analyzed_dt = DateTime.Now,
                        analysis_notes_va = reAnalysisNotes.Content
                    };
                    db.RecordAnalysisPeriods.InsertOnSubmit(new_period);
                    db.SubmitChanges();
                    PeriodID = new_period.period_id;
                    AddDialog(new_period, "Analyzing", "Analyzer", "The period was saved by the analyzer.");
                }

                //Change the lock to a save type
                CreateLock("Analyzing");
                ltlSaved.Visible = true;
                ErrorMessage("hide");
            }
            else
            {
                ErrorMessage("The period start and/or end dates are not valid!");
            }

        }

        protected void FinishReanalyzingPeriod()
        {
            //First, do some validation
            Boolean valid = false;
            if (!string.IsNullOrEmpty(reComments.Content)) valid = true;

            if (valid)
            {
                var period = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID);

                period.status_va = "Analyzed";
                period.status_set_by = user.ID;
                period.status_set_by_role_va = "Analyzer";
                period.analyzed_by = user.ID;
                period.analyzed_dt = DateTime.Now;

                db.SubmitChanges();
                AddDialog(period, "", "Admin", "The period was reanalyzed and is ready for approval.");
                AddDialog(period, "Analyzed", "Analyzer", reComments.Content);
                
                SendEmails("Reanalyzed", reComments.Content, period);
                CloseOutPage(true);
            }
            else
            {
                ErrorMessage("You must enter some comments before you can finish reanalyzing!");
            }
        }

        protected void FinishApprovingPeriod()
        {
            //First, do some validation
            Boolean valid = false;
            if (!string.IsNullOrEmpty(reComments.Content)) valid = true;

            if (valid)
            {
                var period = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID);

                period.status_va = "Approved";
                period.status_set_by = user.ID;
                period.status_set_by_role_va = "Approver";
                period.approved_by = user.ID;
                period.approved_dt = DateTime.Now;

                db.SubmitChanges();
                AddDialog(period, "", "Admin", "The period was set to approved by the approver.");
                AddDialog(period, "Approved", "Approver", reComments.Content);

                SendEmails("Approved", reComments.Content, period);
                CloseOutPage(true);
            }
            else
            {
                ErrorMessage("You must enter some comments when approving!");
            }
        }

        protected void SaveApprovingPeriod()
        {
            //First, do some validation
            Boolean valid = false;
            if (!string.IsNullOrEmpty(reComments.Content)) valid = true;

            if (valid)
            {
                var period = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID);

                period.status_va = "Approving";
                period.status_set_by = user.ID;
                period.status_set_by_role_va = "Approver";
                period.approved_by = user.ID;
                period.approved_dt = DateTime.Now;

                db.SubmitChanges();
                AddDialog(period, "Approving", "Approver", reComments.Content);

                //Change the lock to a save type
                CreateLock("Approving");
                ltlSaved.Visible = true;
                ErrorMessage("hide");
            }
            else
            {
                ErrorMessage("You must enter some comments before saving!");
            }
        }

        protected void SendBackToReanalyze()
        {
            //First, do some validation
            Boolean valid = false;
            if (!string.IsNullOrEmpty(reComments.Content)) valid = true;

            if (valid)
            {
                var period = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID);

                period.status_va = "Reanalyze";
                period.status_set_by = user.ID;
                period.status_set_by_role_va = "Approver";
                period.approved_by = user.ID;
                period.approved_dt = DateTime.Now;

                db.SubmitChanges();
                AddDialog(period, "", "Admin", "The period was sent back for reanalyzing.");
                AddDialog(period, "Reanalyze", "Approver", reComments.Content);

                SendEmails("Reanalyze", reComments.Content, period);
                CloseOutPage(true);
            }
            else
            {
                ErrorMessage("You must enter a comment for the analyzer!");
            }
        }

        protected void ErrorMessage(string error_msg)
        {
            if (error_msg == "hide")
            {
                ltlError.Text = "";
                pnlErrors.Visible = false;
            }
            else
            {
                ltlError.Text = error_msg;
                pnlErrors.Visible = true;
            }
        }

        private Boolean ValidateDates(DateTime? begin_dt, DateTime? end_dt)
        {
            Boolean ret = true;

            if (begin_dt == null || end_dt == null)
            {
                ret = false;
            }
            else
            {
                if (begin_dt > end_dt) ret = false;
            }

            return ret;
        }
        
        /// <summary>
        /// Adds a record to the RMS_Dialog table.
        /// </summary>
        /// <param name="period">The period object</param>
        /// <param name="status">The status the period was set to</param>
        /// <param name="origin_va">The role of the user setting the status</param>
        /// <param name="comments">Comments about the status change/reanalyzing notes</param>
        protected void AddDialog(Data.RecordAnalysisPeriod period, string status, string origin_va, string comments)
        {
            var dialog = period.PeriodDialogs.FirstOrDefault(p => p.status_set_to_va == status && p.status_set_to_va == "Approving" || p.status_set_to_va == status && p.status_set_to_va == "Analayzing");

            if (dialog != null)
            {
                dialog.dialog_dt = DateTime.Now;
                dialog.dialog_by = user.ID;
                dialog.comments_va = comments;
            }
            else
            {
                Data.PeriodDialog new_dialog = new Data.PeriodDialog
                {
                    period_id = period.period_id,
                    dialog_dt = DateTime.Now,
                    dialog_by = user.ID,
                    status_set_to_va = status,
                    origin_va = origin_va,
                    comments_va = comments,
                    period_end_dt = period.period_end_dt
                };

                db.PeriodDialogs.InsertOnSubmit(new_dialog);
            }

            db.SubmitChanges();
        }

        /// <summary>
        /// Adds a record to the RMS_Change_Log table
        /// </summary>
        /// <param name="period_id">The period ID for the analysis notes being tracked</param>
        /// <param name="analysis_notes">The new analysis notes</param>
        protected void AddChangeLog(int period_id, string analysis_notes)
        {
            Data.PeriodChangeLog changelog = new Data.PeriodChangeLog
            {
                period_id = period_id,
                edited_by_uid = user.ID,
                edited_dt = DateTime.Now,
                new_va = analysis_notes
            };

            db.PeriodChangeLogs.InsertOnSubmit(changelog);
            db.SubmitChanges();
        }
        #endregion

        #region Email Properties and Routines
        protected void SendEmails(string action, string comments, Data.RecordAnalysisPeriod period)
        {
            string timespan = String.Format("{0:MM/dd/yyyy} to {1:MM/dd/yyyy}", period.period_beg_dt, period.period_end_dt);
            var cc = new List<String>();
            var to = new List<String>();

            using (var smtp = new SmtpClient() { Host = "gscamnlh01.wr.usgs.gov" })
            {
                var message = new MailMessage("rmsonline@usgs.gov", "rmsonline@usgs.gov");
                message.IsBodyHtml = true;

                switch (action)
                {
                    case "Analyzed":
                        //TO the assigned approver
                        to.Add(EmailAddress(period.Record.approver_uid));
                        //CC the analyzer
                        cc.Add(user.Email); 

                        message.Subject = "Record for " + period.Record.Site.site_no.Trim() + " ready for approving";
                        message.Body = "A record assigned to you for approving has been analyzed:<br /><br />" +
                            "The record period of " + timespan + " for station " + period.Record.Site.site_no.Trim() + " " + period.Record.Site.station_full_nm +
                            " (" + period.Record.RecordType.type_ds + ") has been analyzed by " + user.ID + ".";
                        break;
                    case "Approved":
                        //TO the assigned analyzer
                        to.Add(EmailAddress(period.Record.analyzer_uid));  
                        //If the assigned analyzer is different from the user who analyzed the record, CC to the user who analyzed the record
                        if (period.Record.analyzer_uid != period.analyzed_by) cc.Add(EmailAddress(period.analyzed_by));

                        //If one has been setup, CC the office's designated approver
                        var office = db.Offices.FirstOrDefault(p => p.office_id == OfficeID);
                        if (!string.IsNullOrEmpty(office.reviewer_email)) cc.Add(office.reviewer_email);

                        //Add the approver to the CC list
                        cc.Add(user.Email);

                        message.Subject = "Your record for " + period.Record.Site.site_no.Trim() + " has been approved by " + user.ID;
                        message.Body = "The record period of " + timespan + " for station " + period.Record.Site.site_no.Trim() + " " + period.Record.Site.station_full_nm +
                            " (" + period.Record.RecordType.type_ds + ") has been approved by " + user.ID + ". The status has been set to Approved. The following comments" +
                            " were made by the approver:<br /><br />" + comments;

                        break;
                    case "Reanalyze":
                        //To the assigned analyzer
                        to.Add(EmailAddress(period.Record.analyzer_uid));
                        //If the assigned analyzer is different from the user who analyzed the record, CC to the user who analyzed the record
                        if (period.Record.analyzer_uid != period.analyzed_by) cc.Add(EmailAddress(period.analyzed_by));

                        //Add the approver to the CC list
                        cc.Add(user.Email);

                        message.Subject = "Your record for " + period.Record.Site.site_no.Trim() + " needs to be reanalyzed";
                        message.Body = "The record period of " + timespan + " for station " + period.Record.Site.site_no.Trim() + " " + period.Record.Site.station_full_nm +
                            " (" + period.Record.RecordType.type_ds + ") has been sent back for reanalyzing.  The following comments were made by the approver:<br /><br />" +
                            comments;

                        break;
                    case "Reanalyzed":
                        //To the assigned approver
                        to.Add(EmailAddress(period.Record.approver_uid));
                        //If the assigned approver is different from the user who approved the record, CC to the user who approved the record
                        if (period.Record.approver_uid != period.approved_by) cc.Add(EmailAddress(period.approved_by));

                        //Add the analyzer to the CC list
                        cc.Add(user.Email);

                        message.Subject = "Record for " + period.Record.Site.site_no.Trim() + " has been reanalyzed by " + user.ID;
                        message.Body = "The record period of " + timespan + " for station " + period.Record.Site.site_no.Trim() + " " + period.Record.Site.station_full_nm +
                            " (" + period.Record.RecordType.type_ds + ") has been reanalyzed by " + user.ID + ". The status has been set to Analyzed. The following comments" +
                            " were made by the analyzer:<br /><br />" + comments;

                        break;
                }

#if DEBUG
                string to_string = "";
                string cc_string = "";
                foreach (string email in to)
                    to_string += email + ", ";
                foreach (string email in cc)
                    cc_string += email + ", ";
                to_string.TrimEnd(' ').TrimEnd(',');
                cc_string.TrimEnd(' ').TrimEnd(',');
                message.Body += "<br /><br />To Recipients: " + to_string + "<br />CC Recipients: " + cc_string;

                message.To.Add("dterry@usgs.gov");
                message.CC.Add("slvasque@usgs.gov");
#else
                foreach (string email in to)
                    message.To.Add(email);
                foreach (string email in cc)
                    message.CC.Add(email);
#endif

                smtp.Send(message);
            }
        }

        private string EmailAddress(string user_id)
        {
            return user_id + "@usgs.gov";
        }
        #endregion
    }
}