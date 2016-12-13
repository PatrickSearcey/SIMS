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
            dgChangeLog.DataSource = currPeriod.PeriodChangeLogs.OrderByDescending(p => p.edited_dt).ToList();
            dgChangeLog.DataBind();
        }

        protected void SetupDialogPanel()
        {
            ltlReportTitle.Text = "Dialog";
            pnlDialog.Visible = true;
        }
        
        protected void SetupWYAnalysisNotesPanel()
        {
            ltlReportTitle.Text = "WY Analysis Notes";
            pnlWYAnalysisNotes.Visible = true;
        }
    }
}