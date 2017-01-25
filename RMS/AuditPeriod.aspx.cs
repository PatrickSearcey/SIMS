using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace RMS
{
    public partial class AuditPeriod : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private Data.Record currRecord;
        private int RecordID
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
            string rms_record_id = Request.QueryString["rms_record_id"];

            //If rms_record_id was passed, then set the RecordID variable and make sure the WSCID and OfficeID are changed to match the RecordIDs office and WSC
            if (!string.IsNullOrEmpty(rms_record_id))
            {
                RecordID = Convert.ToInt32(rms_record_id);
                currRecord = db.Records.Where(p => p.rms_record_id == RecordID).FirstOrDefault();
                if (currRecord.RecordAltOffice == null) OfficeID = (int)currRecord.Site.office_id; else OfficeID = (int)currRecord.RecordAltOffice.alt_office_id;
                WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;
            }
            else RecordID = 0;

            UserControlSetup();

            if (!Page.IsPostBack)
            {
                //If the user belongs to this site's WSC (or has an exception to work in the WSC), or is a SuperUser, then allow them to edit the page
                if (user.WSCID.Contains(WSCID) || user.IsSuperUser)
                {
                    pnlHasAccess.Visible = true;
                    pnlNoAccess.Visible = false;

                    InitialView();
                }
                else
                {
                    pnlHasAccess.Visible = false;
                    pnlNoAccess.Visible = true;
                }
            }
        }

        #region Page Load Events
        protected void UserControlSetup()
        {
            string wsc_nm = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).wsc_nm;
            ph1.Title = "Create a New Audit Period";
            if (RecordID == 0)
            {
                ph1.SubTitle = "For the " + wsc_nm + " WSC";
                ph1.RecordType = "&nbsp;";
            }
            else
            {
                ph1.SubTitle = currRecord.Site.site_no + " " + currRecord.Site.station_full_nm;
                ph1.RecordType = currRecord.RecordType.type_ds + " Record for";
            }
        }

        protected void InitialView()
        {
            pnlSetupAuditPeriod.Visible = true;
            pnlAuditPeriod.Visible = false;
            rdpBeginDt.SelectedDate = null;
            rdpEndDt.SelectedDate = null;
            rtbAuditReason.Text = "";
            rtbDataAudited.Text = "";
            rtbAuditFindings.Text = "";
            rtbSANAL.Text = "";

            LoadOfficeList();
            LoadFieldTripList();
            LoadRecordList();
        }

        protected void LoadRecordList()
        {
            if (!string.IsNullOrEmpty(rddlOffice.SelectedValue))
            {
                if (!string.IsNullOrEmpty(rddlFieldTrip.SelectedValue))
                {
                    rlbRecords.DataSource = db.Records
                    .Where(p => p.Site.TripSites.Select(t => t.trip_id).Contains(Convert.ToInt32(rddlFieldTrip.SelectedValue)))
                    .Select(p => new
                    {
                        rms_record_id = p.rms_record_id,
                        record_nm = p.Site.site_no + " - " + p.RecordType.type_ds
                    }).ToList();
                }
                else
                {
                    rlbRecords.DataSource = db.Records
                    .Where(p => p.RecordAltOffice.alt_office_id == Convert.ToInt32(rddlOffice.SelectedValue) || p.Site.office_id == Convert.ToInt32(rddlOffice.SelectedValue))
                    .Select(p => new
                    {
                        rms_record_id = p.rms_record_id,
                        record_nm = p.Site.site_no + " - " + p.RecordType.type_ds
                    }).ToList();
                }
            }
            else
            {
                rlbRecords.DataSource = db.Records
                .Where(p => p.Site.Office.wsc_id == WSCID)
                .Select(p => new
                {
                    rms_record_id = p.rms_record_id,
                    record_nm = p.Site.site_no + " - " + p.RecordType.type_ds
                }).ToList();
            }

            rlbRecords.DataBind();

            if (RecordID > 0)
            {
                RadListBoxItem item = rlbRecords.Items.Where(p => p.Value == RecordID.ToString()).FirstOrDefault();

                if (item != null)
                {
                    item.Checked = true;
                    rlbRecords.Items.Remove(item);
                    rlbRecords.Items.Insert(0, item);
                    rlbRecords.SelectedIndex = 0;
                }
            }
        }

        protected void LoadOfficeList()
        {
            rddlOffice.DataSource = db.Offices.Where(p => p.wsc_id == WSCID).ToList();
            rddlOffice.DataBind();
            rddlOffice.Items.Insert(0, new DropDownListItem { Value = "", Text = "" });
        }

        protected void LoadFieldTripList()
        {
            if (!string.IsNullOrEmpty(rddlOffice.SelectedValue))
            {
                rddlFieldTrip.Enabled = true;
                rddlFieldTrip.DataSource = db.Trips.Where(p => p.office_id == Convert.ToInt32(rddlOffice.SelectedValue)).Select(p => new
                {
                    trip_id = p.trip_id,
                    trip_nm = p.trip_nm + " (" + p.user_id + ")"
                }).ToList();
                rddlFieldTrip.DataBind();
                rddlFieldTrip.Items.Insert(0, new DropDownListItem { Value = "", Text = "" });
            }
            else rddlFieldTrip.Enabled = false;
        }
        #endregion

        #region In Page Events
        protected void FilterRecordList(object sender, EventArgs e)
        {
            RadDropDownList rddl = (RadDropDownList)sender;
            if (rddl.ID == "rddlOffice")
            {
                LoadFieldTripList();
                LoadRecordList();
            }
            else if (rddl.ID == "rddlFieldTrip")
            {
                LoadRecordList();
            }
        }

        protected void rbSubmitRecords_Command(object sender, CommandEventArgs e)
        {
            if (rlbRecords.CheckedItems.Count > 0)
            {
                if (rdpBeginDt.SelectedDate != null && rdpEndDt.SelectedDate != null && rdpBeginDt.SelectedDate < rdpEndDt.SelectedDate)
                {
                    pnlNotice.Visible = false;
                    pnlSetupAuditPeriod.Visible = false;
                    pnlAuditPeriod.Visible = true;
                    SetupAuditForm();
                }
                else
                {
                    pnlNotice.Visible = true;
                    ltlNotice.Text = "You must enter both a begin and end date for the audit period date range, and the begin date must be before the end date.";
                }
            }
            else
            {
                pnlNotice.Visible = true;
                ltlNotice.Text = "You must check the box next to at least one record!";
            }
        }

        protected void rlbViewRecords_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(rlbViewRecords.SelectedValue))
            {
                string pOut = "", edited_dt, edited_by_uid;
                var record = db.Records.FirstOrDefault(p => p.rms_record_id == Convert.ToInt32(rlbViewRecords.SelectedValue));
                var periods = record.RecordAnalysisPeriods.Where(p => p.period_end_dt >= rdpBeginDt.SelectedDate && p.period_end_dt <= rdpEndDt.SelectedDate).OrderByDescending(p => p.period_beg_dt).ToList();

                pOut = "Station Analyses for " + record.Site.site_no.Trim() + " " + record.Site.station_full_nm + "\n" + record.RecordType.type_ds + "\n" +
                        "------------------------------------------------------------------------------------------------------------------------------------------------------------------\n" +
                        "------------------------------------------------------------------------------------------------------------------------------------------------------------------\n\n";

                foreach (var period in periods)
                {
                    edited_dt = period.PeriodChangeLogs.Count() > 0 ? String.Format("{0}", period.PeriodChangeLogs.OrderByDescending(b => b.edited_dt).FirstOrDefault().edited_dt) : "unavailable";
                    edited_by_uid = period.PeriodChangeLogs.Count() > 0 ? period.PeriodChangeLogs.OrderByDescending(b => b.edited_dt).FirstOrDefault().edited_by_uid : "unavailable";

                    pOut += "Analysis Period: " + String.Format("{0:MM/dd/yyyy} to {1:MM/dd/yyyy}", period.period_beg_dt, period.period_end_dt) + "\n" +
                        "Analysis Notes:\n\n" +
                        period.analysis_notes_va.FormatParagraphTextBox() +
                        "Analysis notes for this period last updated " + edited_dt + " by " + edited_by_uid + "\n" +
                        "Analyzed By: " + period.analyzed_by + " Date: " + String.Format("{0:MM/dd/yyyy}", period.analyzed_dt) + "\n" +
                        "Approved By: " + period.approved_by + " Date: " + String.Format("{0:MM/dd/yyyy}", period.approved_dt) + "\n" +
                        "------------------------------------------------------------------------------------------------------------------------------------------------------------------\n" +
                        "------------------------------------------------------------------------------------------------------------------------------------------------------------------\n";
                }

                rtbSANAL.Text = pOut;
            }
        }

        protected void CreateAudit(object sender, CommandEventArgs e)
        {
            int audit_type_id, audit_results_id;
            if (!string.IsNullOrEmpty(rddlAuditType.SelectedValue)) audit_type_id = Convert.ToInt32(rddlAuditType.SelectedValue); else audit_type_id = 0;
            if (!string.IsNullOrEmpty(rddlAuditResults.SelectedValue)) audit_results_id = Convert.ToInt32(rddlAuditResults.SelectedValue); else audit_results_id = 0;
            string audit_reason = rtbAuditReason.Text;
            string audit_data = rtbDataAudited.Text;
            string audit_findings = rtbAuditFindings.Text;

            if (audit_type_id == 0 || audit_results_id == 0 || string.IsNullOrEmpty(audit_reason) || string.IsNullOrEmpty(audit_data) || string.IsNullOrEmpty(audit_findings))
            {
                pnlError.Visible = true;
                ltlError.Text = "You must complete all form fields under the 'Audit the Period' section!";
            }
            else
            {
                Data.Audit new_audit = new Data.Audit()
                {
                    audit_beg_dt = rdpBeginDt.SelectedDate,
                    audit_end_dt = rdpEndDt.SelectedDate,
                    audit_by = user.ID,
                    audit_dt = DateTime.Now,
                    audit_type_id = audit_type_id,
                    audit_results_id = audit_results_id,
                    audit_reason = audit_reason,
                    audit_data = audit_data,
                    audit_findings = audit_findings
                };
                db.Audits.InsertOnSubmit(new_audit);
                db.SubmitChanges();

                foreach (RadListBoxItem item in rlbViewRecords.Items)
                {
                    Data.AuditRecord new_audit_rec = new Data.AuditRecord()
                    {
                        rms_audit_id = new_audit.rms_audit_id,
                        rms_record_id = Convert.ToInt32(item.Value)
                    };
                    db.AuditRecords.InsertOnSubmit(new_audit_rec);
                    db.SubmitChanges();
                }

                pnlError.Visible = false;
                pnlNotice.Visible = true;
                ltlNotice.Text = "The audit was saved.  To view audited periods, visit the <a href='AuditReport.aspx'>Audit Report</a>.";
                InitialView();
            }
        }

        protected void StartOver(object sender, CommandEventArgs e)
        {
            pnlNotice.Visible = false;
            pnlError.Visible = false;
            InitialView();
        }
        #endregion

        #region Page Methods
        protected void SetupAuditForm()
        {
            ltlAuditDateRange.Text = String.Format("{0:MM/dd/yyyy} - {1:MM/dd/yyyy}", rdpBeginDt.SelectedDate, rdpEndDt.SelectedDate);
            ltlAuditBy.Text = user.ID;
            rlbViewRecords.DataSource = rlbRecords.CheckedItems.Select(p => new { rms_record_id = p.Value, record_nm = p.Text });
            rlbViewRecords.DataBind();

            rddlAuditType.DataSource = db.AuditTypes.ToList();
            rddlAuditType.DataBind();
            rddlAuditType.Items.Insert(0, new DropDownListItem { Value = "", Text = "" });
            rddlAuditType.SelectedIndex = 0;

            rddlAuditResults.DataSource = db.AuditResults.ToList();
            rddlAuditResults.DataBind();
            rddlAuditResults.Items.Insert(0, new DropDownListItem { Value = "", Text = "" });
            rddlAuditResults.SelectedIndex = 0;
        }
        #endregion
    }
}