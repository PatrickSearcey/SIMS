using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RMS.Task
{
    public partial class RecordProcess : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private Data.Record currRecord;
        private string task { get; set; }
        private Boolean HasEditAccess { get; set; }
        private int? RecordID { get; set; }
        private int? PeriodID { get; set; }
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
                    //The lock was set by the current user, so they can access the period to process
                    if (user.ID != locks.lock_uid) ret = true;
                }

                return ret;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            string period_id = Request.QueryString["period_id"];
            string rms_record_id = Request.QueryString["rms_record_id"];
            task = Request.QueryString["task"];

            //If no rms_record_id or period_id was passed, then show the error diagnostics panel
            if (!string.IsNullOrEmpty(period_id) || !string.IsNullOrEmpty(rms_record_id))
            {
                if (!string.IsNullOrEmpty(period_id)) PeriodID = Convert.ToInt32(period_id);
                else
                {
                    //If no period_id was passed, then try to figure it out based on the rms_record_id and the task
                    //Only really necessary when trying to analyze a record coming from the Report/RecordProcess.aspx page - because we don't get a period_id from that page
                    period_id = GetPeriodID(rms_record_id);
                    if (!string.IsNullOrEmpty(period_id)) PeriodID = Convert.ToInt32(period_id);
                }
                if (!string.IsNullOrEmpty(rms_record_id)) RecordID = Convert.ToInt32(rms_record_id);
                else RecordID = Convert.ToInt32(db.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID).rms_record_id);
            }
            else
            {
                PopulateErrorDiagnostics("No Period ID or Record ID was passed");
                return;
                //Response.Redirect(Config.SIMSURL + "SIMSWSCHome.aspx");
            }

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
                    SetupPermission();
                    CreateLock(task);
                }
            }
        }

        #region Page Load Methods
        protected void UserControlSetup()
        {
            if (task == "Pending") ph1.Title = "Pending Analyst Concurrence";
            else if (task == "SavePending") ph1.Title = "Approving Record";
            else ph1.Title = task + " Record";
            ph1.SubTitle = currRecord.Site.site_no + " " + db.vSITEFILEs.FirstOrDefault(s => s.site_id == currRecord.Site.nwisweb_site_id).station_nm;
            ph1.RecordType = currRecord.RecordType.type_ds + " Record for";
        }

        /// <summary>
        /// Only necessary when coming from the Record Process Report, and trying to analyze - this is because it's too difficult to get the period_id to put in the URL on the report page
        /// </summary>
        private string GetPeriodID(string rms_record_id)
        {
            string ret = "";

            if (task == "Analyze")
            {
                var period = db.RecordAnalysisPeriods.FirstOrDefault(p => p.rms_record_id.ToString() == rms_record_id && p.status_va == "Analyzing");
                if (period != null)
                {
                    ret = period.period_id.ToString();
                }
            }

            return ret;
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
                pnlPending.Visible = false;
                SetupLockedPanel();
            }
            else
            {
                if (task == "Analyze")
                {
                    pnlLocked.Visible = false;
                    pnlAnalyze.Visible = true;
                    pnlApprove.Visible = false;
                    pnlPending.Visible = false;
                    SetupAnalyzePanel();
                }
                else if (task == "Pending")
                {
                    pnlLocked.Visible = false;
                    pnlAnalyze.Visible = false;
                    pnlApprove.Visible = false;
                    pnlPending.Visible = true;
                    SetupPendingPanel();
                }
                else
                {
                    pnlLocked.Visible = false;
                    pnlAnalyze.Visible = false;
                    pnlApprove.Visible = true;
                    pnlPending.Visible = false;
                    SetupApprovePanel();
                }
            }
        }

        protected void PopulateErrorDiagnostics(string error_msg)
        {
            pnlDiagnostics.Visible = true;
            pnlLocked.Visible = false;
            pnlAnalyze.Visible = false;
            pnlApprove.Visible = false;

            ltlErrorMsg.Text = error_msg;
            ltlUserID.Text = user.ID;
            if (currRecord != null)
            {
                ltlSite.Text = currRecord.site_id.ToString();
                ltlRecord.Text = currRecord.rms_record_id.ToString();
                ltlSiteWSC.Text = currRecord.Site.Office.wsc_id.ToString();
            }
            else
            {
                ltlSite.Text = "record not referenced";
                ltlRecord.Text = Request.QueryString["rms_record_id"];
            }
            ltlPeriod.Text = Request.QueryString["period_id"];
            ltlTask.Text = Request.QueryString["task"];
            ltlUserWSC.Text = "";
            foreach (int wsc in user.WSCID)
                ltlUserWSC.Text += wsc.ToString() + ", ";

            ltlAccess.Text = HasEditAccess.ToString();
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
            hlWYAnalysisNotes.NavigateUrl = String.Format("javascript:OpenPopup('../Modal/ReportPopup.aspx?view=wyanalysisnotes&rms_record_id={0}')", RecordID);
            hlInstructions.NavigateUrl = String.Format("javascript:OpenPopup('../Modal/Instructions.aspx?type=Analyze&id={0}')", currRecord.record_type_id);
            string swr_url = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).swr_url;
            if (!string.IsNullOrEmpty(swr_url))
            {
                hlAutoReview.NavigateUrl = String.Format("javascript:OpenPopup('{0}{1}/')", swr_url, currRecord.Site.site_no.Replace(" ", ""));
            }
            else hlAutoReview.Enabled = false;

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
                if (prevPeriod != null) ltlPrevAnalysisNotes.Text = prevPeriod.analysis_notes_va.FormatParagraphOut(); else ltlPrevAnalysisNotes.Text = "No previous period found.";

                reAnalysisNotes.Content = period.analysis_notes_va.FormatParagraphEdit();
            }
            else
            {
                //Analyzing a new period, but check to see if there have been other periods started for this record
                var analysis_status = currRecord.RecordAnalysisStatus;
                if (analysis_status != null)
                {
                    rdpBeginDateAnalyze.SelectedDate = analysis_status.analyzed_period_dt;
                    rdpBeginDateAnalyze.Enabled = false;
                    ltlPrevAnalysisNotes.Text = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_end_dt == analysis_status.analyzed_period_dt).analysis_notes_va.FormatParagraphOut();
                }
                else //Totally new, adding first period ever to record
                {
                    ltlPrevAnalysisNotes.Text = "No previous period found.";
                }

                //If a template has been setup for this record-type, then pre-populate the Station Analysis textbox with it
                if (currRecord.RecordType.RecordTemplate != null)
                {
                    var template = currRecord.RecordType.RecordTemplate;
                    reAnalysisNotes.Content = template.AnalyzeTemplateEdit;

                    pnlTemplateLink.Visible = true;
                    hlTemplate.NavigateUrl = String.Format("{0}Handler/TemplateViewer.ashx?TemplateID={1}&type={2}", Config.RMSURL, template.TemplateID, "analyze");
                    hlAAADoc1.NavigateUrl = String.Format("{0}Doc/AAA/{1}", Config.RMSURL, template.AAADocFileName);
                }
            }

        }
        
        /// <summary>
        /// The Pending panel is used for the analyst to concur with the approver's minor changes to the analysis notes
        /// </summary>
        protected void SetupPendingPanel()
        {
            var currPeriod = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID);

            if (currPeriod == null) PopulateErrorDiagnostics("The Period could not be referenced.");
            else
            {
                ltlAnalyzedBy1.Text = "<b>" + currPeriod.analyzed_by + "</b>";
                if (!string.IsNullOrEmpty(currPeriod.approved_by)) ltlApprover1.Text = "<b>" + currPeriod.approved_by + "</b>"; else ltlApprover1.Text = "<b>" + user.ID + "</b>";
                ltlTimePeriod1.Text = String.Format("<b>{0:MM/dd/yyyy} - {1:MM/dd/yyyy}</b>", currPeriod.period_beg_dt, currPeriod.period_end_dt);
                hlWYAnalysisNotes2.NavigateUrl = String.Format("javascript:OpenPopup('../Modal/ReportPopup.aspx?view=wyanalysisnotes&rms_record_id={0}')", RecordID);
                hlDialog1.NavigateUrl = String.Format("javascript:OpenPopup('../Modal/ReportPopup.aspx?view=dialog&period_id={0}')", PeriodID);
                hlChangeLog1.NavigateUrl = String.Format("javascript:OpenPopup('../Modal/ReportPopup.aspx?view=changelog&period_id={0}')", PeriodID);

                string swr_url = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).swr_url;
                if (!string.IsNullOrEmpty(swr_url))
                {
                    hlAutoReview2.NavigateUrl = String.Format("javascript:OpenPopup('{0}{1}/')", swr_url, currRecord.Site.site_no.Replace(" ", ""));
                }
                else hlAutoReview2.Enabled = false;
                pnlAnalysisNotesReadOnly1.Visible = true;
                ltlAnalysisNotes1.Text = currPeriod.analysis_notes_va.FormatParagraphOut();
                ltlApproverComments1.Text = currPeriod.PeriodDialogs.Where(p => p.status_set_to_va == "Pending").OrderByDescending(p => p.dialog_dt).FirstOrDefault().comments_va.FormatParagraphOut();
            }
        }

        /// <summary>
        /// The Approve panel is used for both approving and reanalyzing
        /// </summary>
        protected void SetupApprovePanel()
        {
            var currPeriod = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID);

            if (currPeriod == null) PopulateErrorDiagnostics("The Period could not be referenced.");
            else
            {
                ltlAnalyzedBy2.Text = "<b>" + currPeriod.analyzed_by + "</b>";
                if (!string.IsNullOrEmpty(currPeriod.approved_by)) ltlApprover2.Text = "<b>" + currPeriod.approved_by + "</b>"; else ltlApprover2.Text = "<b>" + user.ID + "</b>";
                ltlTimePeriod2.Text = String.Format("<b>{0:MM/dd/yyyy} - {1:MM/dd/yyyy}</b>", currPeriod.period_beg_dt, currPeriod.period_end_dt);
                hlWYAnalysisNotes3.NavigateUrl = String.Format("javascript:OpenPopup('../Modal/ReportPopup.aspx?view=wyanalysisnotes&rms_record_id={0}')", RecordID);
                hlDialog2.NavigateUrl = String.Format("javascript:OpenPopup('../Modal/ReportPopup.aspx?view=dialog&period_id={0}')", PeriodID);
                hlChangeLog2.NavigateUrl = String.Format("javascript:OpenPopup('../Modal/ReportPopup.aspx?view=changelog&period_id={0}')", PeriodID);
                
                string swr_url = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).swr_url;
                if (!string.IsNullOrEmpty(swr_url))
                {
                    hlAutoReview3.NavigateUrl = String.Format("javascript:OpenPopup('{0}{1}/')", swr_url, currRecord.Site.site_no.Replace(" ", ""));
                }
                else hlAutoReview3.Enabled = false;
                pnlAnalysisNotesEdit.Visible = false;
                pnlAnalysisNotesReadOnly2.Visible = true;
                pnlAnalystComments.Visible = false;
                ltlAnalysisNotes2.Text = currPeriod.analysis_notes_va.FormatParagraphOut();
                reAnalysisNotes2.Content = currPeriod.analysis_notes_va.FormatParagraphEdit();
                
                if (task == "Reanalyze")
                {
                    if (currPeriod.PeriodDialogs.FirstOrDefault(p => p.status_set_to_va == "Reanalyze") != null)
                        ltlApproverComments2.Text = currPeriod.PeriodDialogs.Where(p => p.status_set_to_va == "Reanalyze").OrderByDescending(p => p.dialog_dt).FirstOrDefault().comments_va.FormatParagraphOut();
                    if (currPeriod.PeriodDialogs.FirstOrDefault(p => p.status_set_to_va == "Reanalyzing") != null)
                        reComments.Content = currPeriod.PeriodDialogs.Where(p => p.status_set_to_va == "Reanalyzing").OrderByDescending(p => p.dialog_dt).FirstOrDefault().comments_va.FormatParagraphEdit();
                    pnlApproverComments2.Visible = true;
                    hlApproveInst.NavigateUrl = String.Format("javascript:OpenPopup('../Modal/Instructions.aspx?type=Analyze&id={0}')", currRecord.record_type_id);
                    hlApproveInst.Text = "WSC Analyzing Instructions";
                    pnlAnalysisNotesReadOnly2.Visible = false;
                    pnlAnalysisNotesEdit.Visible = true;
                    rbSaveAnalysisNotes.Visible = false;
                    rbCancelAnalysisNotes.Visible = false;
                    ltlReanalyzeNote.Visible = false;
                    ltlApproveNote.Visible = false;
                    rbFinish2.Text = "Finish Reanalyzing";
                    rbFinish2.CommandName = "Reanalyze";
                    rbSave.CommandName = "Reanalyze";
                    rrblReanalyze.Visible = false;
                    rbReanalyze2.Visible = false;
                }
                else
                {
                    //If coming back in to approve after previously saving, populate the comments with the previously saved approver comments
                    if (currPeriod.PeriodDialogs.FirstOrDefault(p => p.status_set_to_va == "Approving") != null)
                    {
                        //If coming back in to the approve after previously sending back for reanalyzing, check the order - has this been saved by the approver since reanalyzing happened?
                        if (currPeriod.PeriodDialogs.FirstOrDefault(p => p.status_set_to_va == "Reanalyze" && p.origin_va == "Approver") != null)
                        {
                            //Check to see if the reanalyze dialog was entered before the approving dialog; if so, then populate the comments with the previously saved approver comments
                            if (currPeriod.PeriodDialogs.FirstOrDefault(p => p.status_set_to_va == "Reanalyze" && p.origin_va == "Approver").dialog_dt < currPeriod.PeriodDialogs.FirstOrDefault(p => p.status_set_to_va == "Approving").dialog_dt)
                            {
                                reComments.Content = currPeriod.PeriodDialogs.Where(p => p.status_set_to_va == "Approving").OrderByDescending(p => p.dialog_dt).FirstOrDefault().comments_va.FormatParagraphEdit();
                            }
                            else //else, populate the comments with the previously saved approver comments from the reanalyze dialog
                            {
                                reComments.Content = currPeriod.PeriodDialogs.Where(p => p.status_set_to_va == "Reanalyze" && p.origin_va == "Approver").OrderByDescending(p => p.dialog_dt).FirstOrDefault().comments_va.FormatParagraphEdit();
                            }
                        }
                        else reComments.Content = currPeriod.PeriodDialogs.Where(p => p.status_set_to_va == "Approving").OrderByDescending(p => p.dialog_dt).FirstOrDefault().comments_va.FormatParagraphEdit();
                    }
                    else
                    {
                        //If coming back in to the approve after previously sending back for reanalyzing, populate the comments with the previously saved approver comments
                        if (currPeriod.PeriodDialogs.FirstOrDefault(p => p.status_set_to_va == "Reanalyze" && p.origin_va == "Approver") != null)
                            reComments.Content = currPeriod.PeriodDialogs.Where(p => p.status_set_to_va == "Reanalyze" && p.origin_va == "Approver").OrderByDescending(p => p.dialog_dt).FirstOrDefault().comments_va.FormatParagraphEdit();
                        //If coming back into the approve after previously making minor edits to the station analysis, and then having the analyst choose to open for reanalyzing, populate the comments 
                        //with the previously made approver comments
                        if (currPeriod.PeriodDialogs.FirstOrDefault(p => p.status_set_to_va == "Reanalyze" && p.origin_va == "Analyst") != null)
                            reComments.Content = currPeriod.PeriodDialogs.Where(p => p.status_set_to_va == "Reanalyze" && p.origin_va == "Analyst").OrderByDescending(p => p.dialog_dt).FirstOrDefault().comments_va.FormatParagraphEdit();
                    }
                    
                    hlApproveInst.NavigateUrl = String.Format("javascript:OpenPopup('../Modal/Instructions.aspx?type=Approve&id={0}')", currRecord.record_type_id);
                    rbFinish2.Text = "Finish Approving";
                    pnlApproverComments2.Visible = false;
                    rbFinish2.CommandName = "Approve";
                    rbSave.CommandName = "Approve";

                    if (task == "SavePending")
                    {
                        rbFinish2.Text = "Finish approving with minor edits";
                        rbFinish2.CommandArgument = "Pending";
                        rbSave.CommandArgument = "SavePending";
                        rrblReanalyze.Enabled = false;
                    }

                    //We need to find out if this period had been previously reanalyzed, and if so, then we show the analyst comments panel
                    var reanalyzedPeriod = currPeriod.PeriodDialogs.OrderByDescending(p => p.dialog_dt).FirstOrDefault(p => p.status_set_to_va == "Reanalyzed");
                    if (reanalyzedPeriod != null)
                    {
                        pnlAnalystComments.Visible = true;
                        ltlAnalystComments.Text = reanalyzedPeriod.comments_va.FormatParagraphOut();
                    }

                    //If a template has been setup for this record-type, then show the template panel
                    if (currRecord.RecordType.RecordTemplate != null)
                    {
                        var template = currRecord.RecordType.RecordTemplate;

                        pnlApproveTemplateLink.Visible = true;
                        hlApproverTemplate.NavigateUrl = String.Format("{0}Handler/TemplateViewer.ashx?TemplateID={1}&type={2}", Config.RMSURL, template.TemplateID, "approve");
                        hlAAADoc2.NavigateUrl = String.Format("{0}Doc/AAA/{1}", Config.RMSURL, template.AAADocFileName);
                    }
                }
            }
        }

        protected void SetupPermission()
        {
            if (!HasEditAccess)
                PopulateErrorDiagnostics("The user does not have permission to access this page.");
                //Response.Redirect(Config.SIMSURL + "SIMSWSCHome.aspx");
        }
        #endregion

        #region Page Methods
        protected void UpdateSavedText()
        {
            string currTime = String.Format("{0:hh:mm}", DateTime.Now);
            ltlSaved.Text = "<div style='width:100%;text-align:center;color:#ec562c;font-weight:bold;'>The period data was saved at " + currTime + "!</div>";
            ltlSaved.Visible = true;
        }
        #endregion

        #region Page Events
        protected void rrblReanalyze_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rrblReanalyze.SelectedValue != null)
            {
                rbReanalyze2.Enabled = true;
            }
        }

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

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            switch (task)
            {
                case "Analyze":
                    SaveAnalyzingPeriod();
                    break;
                case "Reanalyze":
                    SaveReanalyzingPeriod();
                    break;
                case "Approve":
                    SaveApprovingPeriod("Approving");
                    break;
            }
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
                    if (e.CommandArgument.ToString() == "Finish") FinishReanalyzingPeriod();
                    else if (e.CommandArgument.ToString() == "Save") SaveReanalyzingPeriod();
                    break;
                case "Approve":
                    if (e.CommandArgument.ToString() == "Finish") FinishApprovingPeriod();
                    else if (e.CommandArgument.ToString() == "Pending") PendingApproval();
                    else if (e.CommandArgument.ToString() == "Save") SaveApprovingPeriod("Approving");
                    else if (e.CommandArgument.ToString() == "Reanalyze") SendBackToReanalyze();
                    else if (e.CommandArgument.ToString() == "SavePending") SaveApprovingPeriod("SavePending");
                    break;
                case "Pending":
                    if (e.CommandArgument.ToString() == "Reanalyze") OpenToReanalyze();
                    else if (e.CommandArgument.ToString() == "Finish") AcceptApproverEdits();
                    break;
                case "Cancel":
                    CloseOutPage(false);
                    break;
            }
        }

        protected void EditAnalysisNotes(object sender, CommandEventArgs e)
        {
            var currPeriod = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID);

            switch (e.CommandArgument.ToString())
            {
                case "Toggle":
                    pnlAnalysisNotesEdit.Visible = true;
                    pnlAnalysisNotesReadOnly2.Visible = false;
                    ltlNote.Visible = false;
                    break;
                case "Save":
                    //Save the analysis notes
                    currPeriod.analysis_notes_va = reAnalysisNotes2.Content.FormatParagraphIn();

                    //Add an entry to the change log table
                    Data.PeriodChangeLog pcl = new Data.PeriodChangeLog
                    {
                        period_id = currPeriod.period_id,
                        edited_by_uid = user.ID,
                        edited_dt = DateTime.Now,
                        new_va = reAnalysisNotes2.Content.FormatParagraphIn()
                    };
                    db.PeriodChangeLogs.InsertOnSubmit(pcl);
                    db.SubmitChanges();

                    ltlAnalysisNotes2.Text = reAnalysisNotes2.Content.FormatParagraphOut();
                    rbFinish2.Text = "Finish approving with minor edits";
                    rbFinish2.CommandArgument = "Pending";
                    rbSave.CommandArgument = "SavePending";
                    ltlNote.Visible = true;
                    rrblReanalyze.Enabled = false;
                    pnlAnalysisNotesEdit.Visible = false;
                    pnlAnalysisNotesReadOnly2.Visible = true;
                    break;
                case "Cancel":
                    //Reset the analysis notes
                    reAnalysisNotes2.Content = currPeriod.analysis_notes_va.FormatParagraphEdit();

                    pnlAnalysisNotesEdit.Visible = false;
                    pnlAnalysisNotesReadOnly2.Visible = true;
                    break;
            }
        }
        #endregion

        #region Button Methods
        protected void CloseOutPage(bool finished)
        {
            string all = "true";
            if (!finished) all = "false";

            //Clear Locks, but only if no period exists with a status of "Approving" or "Analyzing".  If this is the case, then need to update the lock to be "Approving" or "Analyzing"
            var period = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID);
            if (period != null)
            {
                //Update the lock from "Approve"/"Analyze" to "Approving"/"Analyzing"
                if (period.status_va == "Approving" || period.status_va == "Analyzing")
                {
                    CreateLock(period.status_va);
                }
                else //Otherwise, clear the lock
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("{0}Handler/ClearLock.ashx?user_id={1}&all={2}", Config.RMSURL, user.ID, all));
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                }
            }
            else //Otherwise, clear the lock
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("{0}Handler/ClearLock.ashx?user_id={1}&all={2}", Config.RMSURL, user.ID, all));
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            }
            //Redirect back to Station Info page
            Response.Redirect(String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMSURL, currRecord.Site.site_id));
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
                    period.status_set_by_role_va = "Analyst";
                    period.analyzed_by = user.ID;
                    period.analyzed_dt = DateTime.Now;
                    period.analysis_notes_va = reAnalysisNotes.Content.FormatParagraphIn();

                    db.SubmitChanges();
                    AddDialog(period, "Analyzed", "Analyst", "The period is finished being analyzed.");
                    AddChangeLog(period.period_id, reAnalysisNotes.Content.FormatParagraphIn());
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
                        status_set_by_role_va = "Analyst",
                        analyzed_by = user.ID,
                        analyzed_dt = DateTime.Now,
                        analysis_notes_va = reAnalysisNotes.Content.FormatParagraphIn()
                    };
                    db.RecordAnalysisPeriods.InsertOnSubmit(new_period);
                    db.SubmitChanges();
                    PeriodID = new_period.period_id;
                    AddDialog(new_period, "Analyzed", "Analyst", "The period is finished being Analyzed.");
                    AddChangeLog(new_period.period_id, reAnalysisNotes.Content.FormatParagraphIn());
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
                    period.status_set_by_role_va = "Analyst";
                    period.analyzed_by = user.ID;
                    period.analyzed_dt = DateTime.Now;
                    period.analysis_notes_va = reAnalysisNotes.Content.FormatParagraphIn();

                    db.SubmitChanges();
                    AddDialog(period, "Analyzing", "Analyst", "The period was saved by the analyst.");
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
                        status_set_by_role_va = "Analyst",
                        analyzed_by = user.ID,
                        analyzed_dt = DateTime.Now,
                        analysis_notes_va = reAnalysisNotes.Content.FormatParagraphIn()
                    };
                    db.RecordAnalysisPeriods.InsertOnSubmit(new_period);
                    db.SubmitChanges();
                    PeriodID = new_period.period_id;
                    AddDialog(new_period, "Analyzing", "Analyst", "The period was saved by the analyst.");
                }

                //Change the lock to a save type
                CreateLock("Analyzing");
                UpdateSavedText();
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
                period.status_set_by_role_va = "Analyst";
                period.analyzed_by = user.ID;
                period.analyzed_dt = DateTime.Now;
                period.analysis_notes_va = reAnalysisNotes2.Content.FormatParagraphIn();

                db.SubmitChanges();
                AddDialog(period, "", "Admin", "The period was reanalyzed and is ready for approval.");
                AddDialog(period, "Reanalyzed", "Analyst", reComments.Content.FormatParagraphIn());

                SendEmails("Reanalyzed", reComments.Content.FormatParagraphIn(), period);
                CloseOutPage(true);
            }
            else
            {
                ErrorMessage("You must enter some comments before you can finish reanalyzing!");
            }
        }

        protected void SaveReanalyzingPeriod()
        {
            //First, do some validation
            Boolean valid = false;
            if (!string.IsNullOrEmpty(reComments.Content)) valid = true;

            if (valid)
            {
                //Since reanalyzing a period that already exists, just update
                if (PeriodID > 0)
                {
                    var period = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID);

                    period.status_va = "Reanalyze";
                    period.status_set_by = user.ID;
                    period.status_set_by_role_va = "Analyst";
                    period.analyzed_by = user.ID;
                    period.analyzed_dt = DateTime.Now;
                    period.analysis_notes_va = reAnalysisNotes2.Content.FormatParagraphIn();

                    db.SubmitChanges();
                    AddDialog(period, "Reanalyzing", "Analyst", reComments.Content.FormatParagraphIn());
                }

                UpdateSavedText();
                ErrorMessage("hide");
            }
            else
            {
                ErrorMessage("You must enter some comments before saving!");
            }
        }

        protected void AcceptApproverEdits()
        {
            var period = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID);

            period.status_va = "Approved";
            period.status_set_by = user.ID;
            period.status_set_by_role_va = "Analyst";
            period.approved_by = user.ID;
            period.approved_dt = DateTime.Now;

            string comments = "<p style='font-weight:bold;'>" + user.ID + " has accepted the approver's minor edits to the analysis, and the period is now marked as approved.</p>" + ltlApproverComments1.Text.FormatParagraphIn();

            db.SubmitChanges();
            AddDialog(period, "", "Admin", "The period was set to approved after the analyst accepted the approver's minor edits to the analysis.");
            AddDialog(period, "Approved", "Analyst", comments);

            SendEmails("Approved", comments, period);
            CloseOutPage(true);
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

                string comments = "<p style='font-weight:bold;'>" + user.ID + " has followed current approval guidance and has determined that the record has been properly analyzed and has approved the record period.</p>" + reComments.Content.FormatParagraphIn();

                db.SubmitChanges();
                AddDialog(period, "", "Admin", "The period was set to approved by the approver.");
                AddDialog(period, "Approved", "Approver", comments);

                SendEmails("Approved", comments, period);
                CloseOutPage(true);
            }
            else
            {
                ErrorMessage("You must enter some comments when approving!");
            }
        }

        protected void PendingApproval()
        {
            //First, do some validation
            Boolean valid = false;
            if (!string.IsNullOrEmpty(reComments.Content)) valid = true;

            if (valid)
            {
                var period = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID);

                period.status_va = "Pending";
                period.status_set_by = user.ID;
                period.status_set_by_role_va = "Approver";
                period.approved_by = user.ID;
                period.approved_dt = DateTime.Now;

                string comments = "<p style='font-weight:bold;'>" + user.ID + " has made minor edits to the analysis notes. The analyst must concur before the period is considered approved.</p>" + reComments.Content.FormatParagraphIn();

                db.SubmitChanges();
                AddDialog(period, "", "Admin", "The period was set to pending analyst concurrence by the approver.");
                AddDialog(period, "Pending", "Approver", comments);

                SendEmails("Pending", comments, period);
                CloseOutPage(true);
            }
            else
            {
                ErrorMessage("You must enter some comments when approving!");
            }
        }
        

        protected void SaveApprovingPeriod(string status_va)
        {
            //First, do some validation
            Boolean valid = false;
            if (!string.IsNullOrEmpty(reComments.Content)) valid = true;

            if (valid)
            {
                var period = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID);
                
                period.status_va = status_va;
                period.status_set_by = user.ID;
                period.status_set_by_role_va = "Approver";
                period.approved_by = user.ID;
                period.approved_dt = DateTime.Now;

                db.SubmitChanges();
                AddDialog(period, "Approving", "Approver", reComments.Content.FormatParagraphIn());

                //Change the lock to a save type
                CreateLock("Approving");
                UpdateSavedText();
                ErrorMessage("hide");
            }
            else
            {
                ErrorMessage("You must enter some comments before saving!");
            }
        }
        
        protected void OpenToReanalyze()
        {
            var period = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID);

            period.status_va = "Reanalyze";
            period.status_set_by = user.ID;
            period.status_set_by_role_va = "Analyst";
            period.approved_by = user.ID;
            period.approved_dt = DateTime.Now;

            string comments = "<p style='font-weight:bold;'>" + user.ID + " has reviewed the minor edits made by the approver to the analysis, and determined that some aspects of the record need to be reanalyzed. " +
                "The original approver comments are below.</p>" + ltlApproverComments1.Text.FormatParagraphIn();

            db.SubmitChanges();
            AddDialog(period, "", "Admin", "The period was opened for reanalyzing. The severity was deemed MINOR.");
            AddDialog(period, "Reanalyze", "Analyst", comments);

            SendEmails("OpenToReanalyze", comments, period);
            CloseOutPage(true);
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

                string comments = "<p style='font-weight:bold;'>" + user.ID + " has followed current approval guidance and has determined that some aspects of the record need to be reanalyzed. The severity was deemed " + 
                    rrblReanalyze.SelectedValue.ToUpper() + ". These aspects have been listed below.</p>" + reComments.Content.FormatParagraphIn();

                db.SubmitChanges();
                AddDialog(period, "", "Admin", "The period was sent back for reanalyzing. The severity was deemed " + rrblReanalyze.SelectedValue.ToUpper() + ".");
                AddDialog(period, "Reanalyze", "Approver", comments);

                SendEmails("Reanalyze", comments, period);
                CloseOutPage(true);
            }
            else
            {
                ErrorMessage("You must enter a comment for the analyst!");
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
                if (begin_dt == end_dt) ret = false;
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
            var dialog = period.PeriodDialogs.FirstOrDefault(p => p.status_set_to_va == status && p.status_set_to_va == "Approving" || p.status_set_to_va == status && p.status_set_to_va == "Analayzing" || p.status_set_to_va == status && p.status_set_to_va == "Reanalyzing");

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

                //Setup the office's approver email list
                var office = db.Offices.FirstOrDefault(p => p.office_id == OfficeID);
                string[] appEmails;
                List<string> appEmailList = new List<string>();
                if (!string.IsNullOrEmpty(office.reviewer_email))
                {
                    //If the approver email field contains multiple email addresses, split them up and put them into a List
                    if (office.reviewer_email.IndexOf(',') > 0 || office.reviewer_email.IndexOf(';') > 0)
                    {
                        char[] delimiterChars = { ',', ';' };
                        appEmails = office.reviewer_email.Split(delimiterChars);
                        foreach (string s in appEmails)
                        {
                            //Make sure to grab just the email address if formatted like so: "Cary Carman <cdcarman@usgs.gov>"
                            if (s.IndexOf('<') > 0 && s.IndexOf('>') > 0)
                                appEmailList.Add(s.Substring(s.IndexOf('<') + 1, s.IndexOf('>') - 1));
                            else
                                appEmailList.Add(s);
                        }
                    }
                    else
                    {
                        appEmailList.Add(office.reviewer_email);
                    }
                }

                switch (action)
                {
                    case "Analyzed":
                        //TO the assigned approver
                        if (!string.IsNullOrEmpty(period.Record.approver_uid)) to.Add(EmailAddress(period.Record.approver_uid));
                        //CC the analyzer
                        cc.Add(user.Email);

                        //If one has been setup, CC the office's designated approver
                        if (appEmailList.Count > 0)
                        {
                            foreach (string email in appEmailList)
                                cc.Add(email);
                        }

                        message.Subject = "Record for " + period.Record.Site.site_no.Trim() + " ready for approving";
                        message.Body = "A record assigned to you for approving has been analyzed:<br /><br />" +
                            "The record period of " + timespan + " for station " + period.Record.Site.site_no.Trim() + " " + db.vSITEFILEs.FirstOrDefault(s => s.site_id == period.Record.Site.nwisweb_site_id).station_nm +
                            " (" + period.Record.RecordType.type_ds + ") has been analyzed by " + user.ID + ".";
                        break;
                    case "Approved":
                    case "Pending":
                    case "Accepted":
                        //To the assigned analyzer (or if no assigned analyzer, the person who analyzed the record
                        if (!string.IsNullOrEmpty(period.Record.analyzer_uid)) to.Add(EmailAddress(period.Record.analyzer_uid)); else to.Add(EmailAddress(period.analyzed_by));
                        //If the assigned analyzer is different from the user who analyzed the record, CC to the user who analyzed the record
                        if (period.Record.analyzer_uid != period.analyzed_by) cc.Add(EmailAddress(period.analyzed_by));

                        //If one has been setup, CC the office's designated approver
                        if (appEmailList.Count > 0)
                        {
                            foreach (string email in appEmailList)
                                cc.Add(email);
                        }

                        //Add the approver to the CC list
                        cc.Add(user.Email);

                        if (action == "Approved")
                        {
                            message.Subject = "Your record for " + period.Record.Site.site_no.Trim() + " has been approved by " + user.ID;
                            message.Body = "The record period of " + timespan + " for station " + period.Record.Site.site_no.Trim() + " " + db.vSITEFILEs.FirstOrDefault(s => s.site_id == period.Record.Site.nwisweb_site_id).station_nm +
                                " (" + period.Record.RecordType.type_ds + ") has been approved by " + user.ID + ". The status has been set to Approved. The following comments" +
                                " were made by the approver:<br /><br />" + comments;
                        }
                        else if (action == "Accepted")
                        {
                            message.Subject = "Your record for " + period.Record.Site.site_no.Trim() + " has been approved by " + period.Record.approver_uid;
                            message.Body = "The record period of " + timespan + " for station " + period.Record.Site.site_no.Trim() + " " + db.vSITEFILEs.FirstOrDefault(s => s.site_id == period.Record.Site.nwisweb_site_id).station_nm +
                                " (" + period.Record.RecordType.type_ds + ") has been approved by " + period.Record.approver_uid + ". The status has been set to Approved. The following comments" +
                                " were made by the approver:<br /><br />" + comments;
                        }
                        else
                        {
                            message.Subject = "Your record for " + period.Record.Site.site_no.Trim() + " has been set to pending by " + user.ID;
                            message.Body = "The record period of " + timespan + " for station " + period.Record.Site.site_no.Trim() + " " + db.vSITEFILEs.FirstOrDefault(s => s.site_id == period.Record.Site.nwisweb_site_id).station_nm +
                                " (" + period.Record.RecordType.type_ds + ") has been approved by " + user.ID + ". Minor edits were made to the analysis notes, and the status has been set to Pending. Please review and concur the changes to finalize the period as Approved." +
                                " The following comments were made by the approver:<br /><br />" + comments;
                        }
                        break;
                    case "Reanalyze":
                    case "OpenToReanalyze":
                        //To the assigned analyzer (or if no assigned analyzer, the person who analyzed the record
                        if (!string.IsNullOrEmpty(period.Record.analyzer_uid)) to.Add(EmailAddress(period.Record.analyzer_uid)); else to.Add(EmailAddress(period.analyzed_by));
                        //If the assigned analyzer is different from the user who analyzed the record, CC to the user who analyzed the record
                        if (period.Record.analyzer_uid != period.analyzed_by) cc.Add(EmailAddress(period.analyzed_by));

                        //If one has been setup, CC the office's designated approver
                        if (appEmailList.Count > 0)
                        {
                            foreach (string email in appEmailList)
                                cc.Add(email);
                        }

                        //Add the approver to the CC list
                        cc.Add(user.Email);

                        if (action == "Reanalyze")
                        {
                            message.Subject = "Your record for " + period.Record.Site.site_no.Trim() + " needs to be reanalyzed";
                            message.Body = "The record period of " + timespan + " for station " + period.Record.Site.site_no.Trim() + " " + db.vSITEFILEs.FirstOrDefault(s => s.site_id == period.Record.Site.nwisweb_site_id).station_nm +
                                " (" + period.Record.RecordType.type_ds + ") has been sent back for reanalyzing. The severity was deemed " + rrblReanalyze.SelectedValue.ToUpper() + ". The following comments were made by the approver:<br /><br />" +
                                comments;
                        }
                        else
                        {
                            message.Subject = "Your record for " + period.Record.Site.site_no.Trim() + " needs to be reanalyzed";
                            message.Body = "The record period of " + timespan + " for station " + period.Record.Site.site_no.Trim() + " " + db.vSITEFILEs.FirstOrDefault(s => s.site_id == period.Record.Site.nwisweb_site_id).station_nm +
                                " (" + period.Record.RecordType.type_ds + ") has been sent back for reanalyzing. The severity was deemed MINOR. The following comments were made by the approver:<br /><br />" +
                                comments;
                        }

                        break;
                    case "Reanalyzed":
                        //To the assigned approver (or if no assigned approver, the person who approved the record
                        if (!string.IsNullOrEmpty(period.Record.approver_uid)) to.Add(EmailAddress(period.Record.approver_uid)); else to.Add(EmailAddress(period.approved_by));
                        //If the assigned approver is different from the user who approved the record, CC to the user who approved the record
                        if (period.Record.approver_uid != period.approved_by) cc.Add(EmailAddress(period.approved_by));

                        //If one has been setup, CC the office's designated approver
                        if (appEmailList.Count > 0)
                        {
                            foreach (string email in appEmailList)
                                cc.Add(email);
                        }

                        //Add the analyzer to the CC list
                        cc.Add(user.Email);

                        message.Subject = "Record for " + period.Record.Site.site_no.Trim() + " has been reanalyzed by " + user.ID;
                        message.Body = "The record period of " + timespan + " for station " + period.Record.Site.site_no.Trim() + " " + db.vSITEFILEs.FirstOrDefault(s => s.site_id == period.Record.Site.nwisweb_site_id).station_nm +
                            " (" + period.Record.RecordType.type_ds + ") has been reanalyzed by " + user.ID + ". The status has been set to Analyzed. The following comments" +
                            " were made by the analyst:<br /><br />" + comments;

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
            //If possible, get the email address for the user from AD
            string email = user_id + "@usgs.gov";
            var reg_user = db.spz_GetUserInfoFromAD(user_id).ToList();
            foreach (var result in reg_user)
                email = result.mail;

            return email;
        }
        #endregion
    }
}