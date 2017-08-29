using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RMS.Modal
{
    public partial class ReportPopup : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private string View { get; set; }
        private Data.Record currRecord { get; set; }
        private Data.RecordAnalysisPeriod currPeriod { get; set; }
        private DateTime? beg_dt, end_dt;
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
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            View = Request.QueryString["view"];
            string rms_record_id = Request.QueryString["rms_record_id"];
            string period_id = Request.QueryString["period_id"];
            beg_dt = Convert.ToDateTime(Request.QueryString["beg_dt"]);
            end_dt = Convert.ToDateTime(Request.QueryString["end_dt"]);

            if (!string.IsNullOrEmpty(rms_record_id) || !string.IsNullOrEmpty(period_id))
            {
                if (!string.IsNullOrEmpty(period_id)) PeriodID = Convert.ToInt32(period_id); else PeriodID = 0;
                if (!string.IsNullOrEmpty(rms_record_id)) RecordID = Convert.ToInt32(rms_record_id); else RecordID = 0;

                if (RecordID == 0) RecordID = Convert.ToInt32(db.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID).rms_record_id);
                currRecord = db.Records.FirstOrDefault(p => p.rms_record_id == RecordID);

                ltlSite.Text = currRecord.Site.site_no + " " + currRecord.Site.station_full_nm;
                ltlRecord.Text = currRecord.RecordType.type_ds + " Record";

                switch (View)
                {
                    case "changelog":
                        SetupChangeLogPanel();
                        break;
                    case "dialog":
                        SetupDialogPanel();
                        break;
                    case "wyanalysisnotes":
                    case "analysisbyperiod":
                    case "analysisnotesbydaterange":
                        SetupWYAnalysisNotesPanel();
                        break;
                }
            }
            else
            {
                ltlReportTitle.Text = "Report Error";
                pnlNoRecordID.Visible = true;
            }

        }

        protected void SetupChangeLogPanel()
        {
            currPeriod = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID);

            ltlReportTitle.Text = "Change Log";
            pnlChangeLog.Visible = true;
            ltlPeriod1.Text = String.Format("{0:MM/dd/yyyy} - {1:MM/dd/yyyy}", currPeriod.period_beg_dt, currPeriod.period_end_dt);
            dgChangeLog.DataSource = currPeriod.PeriodChangeLogs.Select(p => new Data.PeriodChangeLog {
                edited_dt = p.edited_dt,
                edited_by_uid = p.edited_by_uid,
                new_va = p.new_va.FormatParagraphOut()
            }).OrderByDescending(p => p.edited_dt).ToList();
            dgChangeLog.DataBind();
        }

        protected void SetupDialogPanel()
        {
            currPeriod = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID);

            ltlReportTitle.Text = "Dialog";
            pnlDialog.Visible = true;
            ltlPeriod2.Text = String.Format("{0:MM/dd/yyyy} - {1:MM/dd/yyyy}", currPeriod.period_beg_dt, currPeriod.period_end_dt);
            dgDialog.DataSource = currPeriod.PeriodDialogs.Select(p => new Data.PeriodDialog {
                dialog_dt = p.dialog_dt,
                origin_va = p.origin_va,
                dialog_by = p.dialog_by,
                status_set_to_va = p.status_set_to_va,
                comments_va = p.comments_va.FormatParagraphOut()
            }).OrderByDescending(p => p.dialog_dt).ToList();
            dgDialog.DataBind();
        }
        
        protected void SetupWYAnalysisNotesPanel()
        {
            List<AnalysisNotesItem> lani = new List<AnalysisNotesItem>();

            if (PeriodID == 0)
            {
                //Figure out the begin and end dates to use for the WY timespan
                if (beg_dt == null && end_dt == null)
                {
                    DateTime currDate = DateTime.Now;
                    int currWY;
                    if (currDate.Month < 4) currWY = DateTime.Now.Year - 2; else currWY = DateTime.Now.Year - 1;
                    beg_dt = Convert.ToDateTime("10/1/" + currWY.ToString());
                    end_dt = DateTime.Now;
                }

                //Grab all analysis periods within this timespan
                var periods = currRecord.RecordAnalysisPeriods.Where(p => p.period_end_dt >= beg_dt && p.period_beg_dt <= end_dt).OrderByDescending(p => p.period_beg_dt).ToList();

                foreach (var period in periods)
                {
                    AnalysisNotesItem ani = new AnalysisNotesItem
                    {
                        timespan = String.Format("{0:MM/dd/yyyy} to {1:MM/dd/yyyy}", period.period_beg_dt, period.period_end_dt),
                        analysis_notes_va = period.analysis_notes_va.FormatParagraphOut(),
                        edited_dt = period.PeriodChangeLogs.Count() > 0 ? String.Format("{0}", period.PeriodChangeLogs.OrderByDescending(b => b.edited_dt).FirstOrDefault().edited_dt) : "unavailable",
                        edited_by_uid = period.PeriodChangeLogs.Count() > 0 ? period.PeriodChangeLogs.OrderByDescending(b => b.edited_dt).FirstOrDefault().edited_by_uid : "unavailable",
                        analyzed_by = period.analyzed_by,
                        analyzed_dt = String.Format("{0:MM/dd/yyyy}", period.analyzed_dt),
                        approved_by = period.approved_by,
                        approved_dt = String.Format("{0:MM/dd/yyyy}", period.approved_dt)
                    };
                    lani.Add(ani);
                }

                ltlReportTitle.Text = "WY Analysis";
            }
            else
            {
                var period = currRecord.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID);

                AnalysisNotesItem ani = new AnalysisNotesItem
                {
                    timespan = String.Format("{0:MM/dd/yyyy} to {1:MM/dd/yyyy}", period.period_beg_dt, period.period_end_dt),
                    analysis_notes_va = period.analysis_notes_va.FormatParagraphOut(),
                    edited_dt = String.Format("{0}", period.PeriodChangeLogs.OrderByDescending(b => b.edited_dt).FirstOrDefault().edited_dt),
                    edited_by_uid = period.PeriodChangeLogs.OrderByDescending(b => b.edited_dt).FirstOrDefault().edited_by_uid,
                    analyzed_by = period.analyzed_by,
                    analyzed_dt = String.Format("{0:MM/dd/yyyy}", period.analyzed_dt),
                    approved_by = period.approved_by,
                    approved_dt = String.Format("{0:MM/dd/yyyy}", period.approved_dt)
                };
                lani.Add(ani);

                ltlReportTitle.Text = "Station Analysis";
            }

            if (lani.Count == 0)
            {
                ltlNoPeriods.Visible = true;
            }
            pnlWYAnalysisNotes.Visible = true;
            dlWYAnalysisNotes.DataSource = lani;
            dlWYAnalysisNotes.DataBind();
        }

        #region Internal Classes
        internal class AnalysisNotesItem
        {
            private string _timespan;
            private string _analysis_notes_va;
            private string _edited_dt;
            private string _edited_by_uid;
            private string _analyzed_by;
            private string _analyzed_dt;
            private string _approved_by;
            private string _approved_dt;

            public string timespan
            {
                get { return _timespan; }
                set { _timespan = value; }
            }
            public string analysis_notes_va
            {
                get { return _analysis_notes_va; }
                set { _analysis_notes_va = value; }
            }
            public string edited_dt
            {
                get { return _edited_dt; }
                set { _edited_dt = value; }
            }
            public string edited_by_uid
            {
                get { return _edited_by_uid; }
                set { _edited_by_uid = value; }
            }
            public string analyzed_by
            {
                get { return _analyzed_by; }
                set { _analyzed_by = value; }
            }
            public string analyzed_dt
            {
                get { return _analyzed_dt; }
                set { _analyzed_dt = value; }
            }
            public string approved_by
            {
                get { return _approved_by; }
                set { _approved_by = value; }
            }
            public string approved_dt
            {
                get { return _approved_dt; }
                set { _approved_dt = value; }
            }
            public AnalysisNotesItem()
            {
                _timespan = timespan;
                _analysis_notes_va = analysis_notes_va;
                _edited_dt = edited_dt;
                _edited_by_uid = edited_by_uid;
                _analyzed_by = analyzed_by;
                _analyzed_dt = analyzed_dt;
                _approved_by = approved_by;
                _approved_dt = approved_dt;
            }
        }
        #endregion
    }
}