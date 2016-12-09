using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            string period_id = "368644";// Request.QueryString["period_id"];
            string rms_record_id = Request.QueryString["rms_record_id"];
            task = "Analyze"; // Request.QueryString["task"];

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
                if (user.WSCID.Contains(WSCID) || user.IsSuperUser) HasEditAccess = true;

                CreateLock(task);
                PopulatePageData();
                SetupPermission();
            }
        }


        #region Page Load Methods
        protected void UserControlSetup()
        {
            if (task == "Analyze") ph1.Title = "Analyze Record";
            else if (task == "Reanalyze") ph1.Title = "Reanalyze Record";
            else ph1.Title = "Approve Record";
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
                pnlReanalyze.Visible = false;
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
                    pnlReanalyze.Visible = false;
                    SetupAnalyzePanel();
                }
                else if (task == "Reanalyze")
                {
                    pnlLocked.Visible = false;
                    pnlAnalyze.Visible = false;
                    pnlApprove.Visible = false;
                    pnlReanalyze.Visible = true;
                    SetupReanalyzePanel();
                }
                else if (task == "Approve")
                {
                    pnlLocked.Visible = false;
                    pnlAnalyze.Visible = false;
                    pnlApprove.Visible = true;
                    pnlReanalyze.Visible = false;
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
                if (prevPeriod != null) rtbPrevAnalysisNotes.Text = prevPeriod.analysis_notes_va; else rtbPrevAnalysisNotes.Text = "No previous period found.";

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
                    rtbPrevAnalysisNotes.Text = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_end_dt == analysis_status.analyzed_period_dt).analysis_notes_va;
                }
                else //Totally new, adding first period ever to record
                {
                    rtbPrevAnalysisNotes.Text = "No previous period found.";
                }
            }

        }

        protected void SetupReanalyzePanel()
        {

        }

        protected void SetupApprovePanel()
        {

        }

        protected void SetupPermission()
        {

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
                    break;
                case "Approve":
                    if (e.CommandArgument.ToString() == "Finish") FinishApprovingPeriod();
                    else if (e.CommandArgument.ToString() == "Save") SaveApprovingPeriod();
                    else if (e.CommandArgument.ToString() == "Reanalyze") SendBackToReanalyze();
                    break;
                case "Cancel":
                    CloseOutPage();
                    break;
            }
        }
        #endregion

        #region Button Methods
        protected void CloseOutPage()
        {
            //Clear Locks
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("{0}Handler/ClearLock.ashx?user_id={1}", Config.RMSURL, user.ID));
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
                    period.analysis_notes_va = reAnalysisNotes.Text;

                    db.SubmitChanges();
                    AddDialog(period.period_id, "Analyzed", "Analyzer", "The period is finished being Analyzed.", Convert.ToDateTime(rdpEndDateAnalyze.SelectedDate), "");
                    AddChangeLog(period.period_id, reAnalysisNotes.Text);
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
                        analysis_notes_va = reAnalysisNotes.Text
                    };
                    db.RecordAnalysisPeriods.InsertOnSubmit(new_period);
                    db.SubmitChanges();
                    PeriodID = new_period.period_id;
                    AddDialog(new_period.period_id, "Analyzed", "Analyzer", "The period is finished being Analyzed.", Convert.ToDateTime(rdpEndDateAnalyze.SelectedDate), "");
                    AddChangeLog(new_period.period_id, reAnalysisNotes.Text);
                }

                SendEmails();
                CloseOutPage();
            }
            else
            {
                ShowErrorMessage("The period start and/or end dates are not valid!");
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
                    period.analysis_notes_va = reAnalysisNotes.Text;

                    db.SubmitChanges();
                    AddDialog(period.period_id, "Analyzing", "Analyzer", "The period was saved by the analyzer.", Convert.ToDateTime(rdpEndDateAnalyze.SelectedDate), "");
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
                        analysis_notes_va = reAnalysisNotes.Text
                    };
                    db.RecordAnalysisPeriods.InsertOnSubmit(new_period);
                    db.SubmitChanges();
                    AddDialog(new_period.period_id, "Analyzing", "Analyzer", "The period was saved by the analyzer.", Convert.ToDateTime(rdpEndDateAnalyze.SelectedDate), "");
                }

                //Change the lock to a save type
                CreateLock("Analyzing");
                SendEmails();
                ltlSaved.Visible = true;
            }
            else
            {
                ShowErrorMessage("The period start and/or end dates are not valid!");
            }

        }

        protected void FinishReanalyzingPeriod()
        {

        }

        protected void SaveReanalyingPeriod()
        {

        }

        protected void FinishApprovingPeriod()
        {

        }

        protected void SaveApprovingPeriod()
        {

        }

        protected void SendBackToReanalyze()
        {

        }

        protected void ShowErrorMessage(string error_msg)
        {
            ltlError.Text = error_msg;
            pnlErrors.Visible = true;
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
        /// <param name="period_id">The period ID for the period</param>
        /// <param name="status">The status the period was set to</param>
        /// <param name="origin_va">The role of the user setting the status</param>
        /// <param name="comments">Comments about the status change/reanalyzing notes</param>
        /// <param name="end_dt">The end date of the period</param>
        /// <param name="severity">The severity of the reason to Reanalyze</param>
        protected void AddDialog(int period_id, string status, string origin_va, string comments, DateTime end_dt, string severity)
        {
            Data.PeriodDialog dialog = new Data.PeriodDialog
            {
                period_id = period_id,
                dialog_dt = DateTime.Now,
                dialog_by = user.ID,
                status_set_to_va = status,
                origin_va = origin_va,
                comments_va = comments,
                period_end_dt = end_dt,
                reanalyze_severity = severity
            };

            db.PeriodDialogs.InsertOnSubmit(dialog);
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

        protected void SendEmails()
        {

        }
        #endregion
    }
}