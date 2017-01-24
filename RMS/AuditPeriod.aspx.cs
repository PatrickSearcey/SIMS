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
        private int RecordID { get; set; }
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
            string rms_record_id = "32045";// Request.QueryString["rms_record_id"];

            //If rms_record_id was passed, then set the RecordID variable and make sure the WSCID and OfficeID are changed to match the RecordIDs office and WSC
            if (!string.IsNullOrEmpty(rms_record_id))
            {
                RecordID = Convert.ToInt32(rms_record_id);
                var Record = db.Records.Where(p => p.rms_record_id == RecordID).FirstOrDefault();
                if (Record.RecordAltOffice == null) OfficeID = (int)Record.Site.office_id; else OfficeID = (int)Record.RecordAltOffice.alt_office_id;
                WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;
            }

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
            ph1.SubTitle = "For the " + wsc_nm + " WSC";
            ph1.RecordType = "&nbsp;";
        }

        protected void InitialView()
        {
            pnlSetupAuditPeriod.Visible = true;
            pnlAuditPeriod.Visible = false;
            rdpBeginDt.SelectedDate = null;
            rdpEndDt.SelectedDate = null;

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

        }

        protected void rlbViewRecords_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void CreateAudit(object sender, CommandEventArgs e)
        {
            pnlNotice.Visible = true;
            ltlNotice.Text = "The audit was saved.  To view audited periods, visit the <a href='AuditReport.aspx'>Audit Report</a>.";
            InitialView();
        }

        protected void StartOver(object sender, CommandEventArgs e)
        {
            pnlNotice.Visible = false;
            InitialView();
        }
        #endregion
    }
}