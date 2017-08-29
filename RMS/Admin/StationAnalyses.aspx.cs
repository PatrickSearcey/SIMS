using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace RMS.Admin
{
    public partial class StationAnalyses : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        public Boolean HasEditAccess { get; set; }
        private Data.Record record;
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
            string office_id = Request.QueryString["office_id"];
            if (!string.IsNullOrEmpty(office_id))
                OfficeID = Convert.ToInt32(office_id);
            else
                OfficeID = user.OfficeID;

            WSCID = Convert.ToInt32(db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id);

            if (!Page.IsPostBack)
            {
                //If the user belongs to this site's WSC (or has an exception to work in the WSC), or is a SuperUser, then allow them to edit the page
                if (user.WSCID.Contains(WSCID) && user.IsAdmin || user.IsSuperUser) HasEditAccess = true;

                if (HasEditAccess)
                {
                    pnlHasAccess.Visible = true;
                    pnlNotice.Visible = false;
                    pnlNoAccess.Visible = false;
                }
                else
                {
                    pnlHasAccess.Visible = false;
                    pnlNoAccess.Visible = true;
                }

                UserControlSetup();
            }
        }

        protected void UserControlSetup()
        {
            string wsc_nm = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).wsc_nm;
            ph1.Title = "Manage Station Analyses";

            ph1.SubTitle = "For the " + wsc_nm + " WSC";
            ph1.RecordType = "&nbsp;";
        }

        protected void UpdateRecordList(object sender, CommandEventArgs e)
        {
            if (!string.IsNullOrEmpty(rtbSiteNo.Text) && !string.IsNullOrEmpty(rtbAgencyCd.Text))
            {
                var site = db.Sites.FirstOrDefault(p => p.site_no == rtbSiteNo.Text && p.agency_cd == rtbAgencyCd.Text);
                if (site != null)
                {
                    rddlRecords.DataSource = site.Records.Where(p => p.not_used_fg == false).Select(p => new { rms_record_id = p.rms_record_id, type_ds = p.RecordType.type_ds });
                    rddlRecords.DataBind();
                    rddlRecords.Items.Insert(0, new DropDownListItem { Value = "", Text = "" });

                    ltlError.Text = "";
                }
                else
                {
                    ltlError.Text = "<span style='color:red;'>No records were found for this site</span>";
                }
            }
            else
            {
                ltlError.Text = "<span style='color:red;'>Invalid site submitted</span>";
            }
            rgSANAL.Rebind();
        }

        protected void UpdateSANALGrid(object sender, EventArgs e)
        {
            if (rddlRecords.SelectedIndex > 0)
            {
                rgSANAL.Rebind();
            }
        }

        #region rgSANAL
        protected void rgSANAL_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (rddlRecords.SelectedIndex > 0)
            {
                record = db.Records.FirstOrDefault(p => p.rms_record_id == Convert.ToInt32(rddlRecords.SelectedValue));
                rgSANAL.DataSource = record.RecordAnalysisPeriods.Where(p => p.status_va == "Approved").Select(p => new {
                    period_id = p.period_id,
                    rms_record_id = p.rms_record_id,
                    period_beg_dt = p.period_beg_dt,
                    period_end_dt = p.period_end_dt,
                    status_va = p.status_va,
                    site_no = p.Record.Site.site_no,
                    station_full_nm = p.Record.Site.station_full_nm,
                    type_ds = p.Record.RecordType.type_ds,
                    status_set_by_role_va = p.status_set_by_role_va
                }).OrderBy(p => p.period_beg_dt).ToList();
            }
        }

        protected void rgSANAL_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.IsInEditMode)
            {
                GridEditableItem item = (GridEditableItem)e.Item;

                int period_id = Convert.ToInt32(item.GetDataKeyValue("period_id"));
                var period = db.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == period_id);

                RadDropDownList rddlAnalyzedBy = (RadDropDownList)item.FindControl("rddlAnalyzedBy");
                RadDatePicker rdpAnalyzedDt = (RadDatePicker)item.FindControl("rdpAnalyzedDt");
                RadDropDownList rddlApprovedBy = (RadDropDownList)item.FindControl("rddlApprovedBy");
                RadDatePicker rdpApprovedDt = (RadDatePicker)item.FindControl("rdpApprovedDt");
                RadEditor reSANAL = (RadEditor)item.FindControl("reSANAL");

                //Analyzer & Approver
                var personnel = db.SP_Personnel_by_WSC_office_or_user_id(period.Record.Site.Office.wsc_id, 0, "", "no", "All", "no");
                rddlAnalyzedBy.DataSource = personnel;
                rddlAnalyzedBy.DataBind();
                rddlApprovedBy.DataSource = personnel;
                rddlApprovedBy.DataBind();
                if (!string.IsNullOrEmpty(period.analyzed_by))
                {
                    rddlAnalyzedBy.SelectedValue = period.analyzed_by;
                    rdpAnalyzedDt.SelectedDate = period.analyzed_dt;
                }
                else rddlAnalyzedBy.Items.Insert(0, new DropDownListItem { Value = "", Text = "" });
                if (!string.IsNullOrEmpty(period.approved_by))
                {
                    rddlApprovedBy.SelectedValue = period.approved_by;
                    rdpApprovedDt.SelectedDate = period.approved_dt;
                }
                else rddlApprovedBy.Items.Insert(0, new DropDownListItem { Value = "", Text = "" });

                reSANAL.Content = period.analysis_notes_va.FormatParagraphEdit();
            }
        }

        protected void rgSANAL_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            GridEditableItem item = e.Item as GridEditableItem;

            int period_id = Convert.ToInt32(item.GetDataKeyValue("period_id"));
            var period = db.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == period_id);

            period.analyzed_by = (item.FindControl("rddlAnalyzedBy") as RadDropDownList).SelectedValue.ToString();
            period.analyzed_dt = (item.FindControl("rdpAnalyzedDt") as RadDatePicker).SelectedDate;
            period.approved_by = (item.FindControl("rddlApprovedBy") as RadDropDownList).SelectedValue.ToString();
            period.approved_dt = (item.FindControl("rdpApprovedDt") as RadDatePicker).SelectedDate;
            period.analysis_notes_va = (item.FindControl("reSANAL") as RadEditor).Content.FormatParagraphIn();

            Data.PeriodChangeLog changeLog = new Data.PeriodChangeLog() {
                period_id = period.period_id,
                edited_by_uid = user.ID,
                edited_dt = DateTime.Now,
                new_va = (item.FindControl("reSANAL") as RadEditor).Text.FormatParagraphIn()
            };
            db.PeriodChangeLogs.InsertOnSubmit(changeLog);
            Data.PeriodDialog dialog = new Data.PeriodDialog() {
                period_id = period.period_id,
                dialog_dt = DateTime.Now,
                dialog_by = user.ID,
                origin_va = "Admin",
                comments_va = "The analysis was modified using the Manage Analyses interface."
            };
            db.PeriodDialogs.InsertOnSubmit(dialog);

            db.SubmitChanges();
            
        }
        #endregion
    }
}