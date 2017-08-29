using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace RMS.Report
{
    public partial class PubStatus : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private int reportOfficeID { get; set; }
        private string siteTypeCd { get; set; }
        private string recordTypeCd { get; set; }
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

            WSCID = (int)db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id;

            if (!Page.IsPostBack)
            {
                UserControlSetup();
                //On initial load, narrow down the view to the responsible office
                reportOfficeID = OfficeID;
                InitialDataBind();
            }
        }

        #region Page Load Events
        protected void UserControlSetup()
        {
            string wsc_nm = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).wsc_nm;
            ph1.Title = "Current Publication Status";

            ph1.SubTitle = "For the " + wsc_nm + " WSC";
            ph1.RecordType = "&nbsp;";
        }

        /// <summary>
        /// When binding the chart data initially, pull data for the entire WSC
        /// </summary>
        protected void InitialDataBind()
        {
            //Filter controls
            rddlOffice.DataSource = db.Offices.Where(p => p.wsc_id == WSCID).OrderBy(p => p.office_nm).ToList();
            rddlOffice.DataBind();
            rddlOffice.Items.Insert(0, "All Offices");
            rddlOffice.SelectedValue = reportOfficeID.ToString();

            rddlSiteType.DataSource = db.vSiteTypesForWSCs.Where(p => p.wsc_id == WSCID).OrderBy(p => p.site_tp_cd).ToList();
            rddlSiteType.DataBind();
            rddlSiteType.Items.Insert(0, "All Site Types");
            rddlSiteType.SelectedValue = siteTypeCd;

            rddlRecordType.DataSource = db.RecordTypes.Where(p => p.wsc_id == WSCID).OrderBy(p => p.type_cd).ToList();
            rddlRecordType.DataBind();
            rddlRecordType.Items.Insert(0, "All Record Types");
            rddlRecordType.SelectedValue = recordTypeCd;
        }
        #endregion

        #region Misc Events
        protected void UpdateDetails(object sender, CommandEventArgs e)
        {
            if (rddlOffice.SelectedIndex > 0) reportOfficeID = Convert.ToInt32(rddlOffice.SelectedValue); else reportOfficeID = 0;
            if (rddlSiteType.SelectedIndex > 0) siteTypeCd = rddlSiteType.SelectedValue; else siteTypeCd = "";
            if (rddlRecordType.SelectedIndex > 0) recordTypeCd = rddlRecordType.SelectedValue; else recordTypeCd = "";
            InitialDataBind();
            rgTSStatus.Rebind();
        }
        #endregion 

        #region Time-Series Status RadGrid
        protected void rgTSStatus_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var reportData = db.SP_Publication_Status(WSCID, reportOfficeID, siteTypeCd, recordTypeCd).OrderBy(p => p.office_cd).ThenBy(p => p.site_no).ToList();

            rgTSStatus.DataSource = reportData;

            ltlNumberOfRecords.Text = "Number of records returned: <b>" + reportData.Count.ToString() + "</b>";
        }

        protected void rgTSStatus_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgTSStatus.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" | menu.Items[i].Text == "Contains" | menu.Items[i].Text == "EqualTo" | menu.Items[i].Text == "DoesNotContain" | menu.Items[i].Text == "GreaterThan" | menu.Items[i].Text == "LessThan")
                    i = i + 1;
                else
                    menu.Items.RemoveAt(i);
            }
        }

        protected void rgTSStatus_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                int rms_record_id = Convert.ToInt32(item.GetDataKeyValue("rms_record_id"));
            }
        }
        #endregion

        #region Non-Time-Series Status RadGrid
        protected void rgNTSStatus_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var ras = db.RecordApprovalStatus.Select(p => new { 
                wsc_id = db.Sites.FirstOrDefault(s => s.site_id == p.site_id).Office.wsc_id, 
                office_cd = p.office_cd,
                site_no = p.site_no, 
                rms_record_id = p.rms_record_id,
                site_tp_cd = db.vSITEFILEs.FirstOrDefault(s => s.site_no == s.site_no && s.agency_cd == s.agency_cd).site_tp_cd,
                type_cd = p.type_cd, 
                parm_cd = p.parm_cd,
                ts = db.RecordTypes.FirstOrDefault(r => r.record_type_id == p.record_type_id).ts_fg,
                category_no = p.category_no,
                station_full_nm = db.Sites.FirstOrDefault(s => s.site_id == p.site_id).station_full_nm,
                last_aging_dt = p.last_aging_dt
            }).Where(p => p.ts == false);

            rgNTSStatus.DataSource = ras.Where(p => p.wsc_id == WSCID).OrderBy(p => p.office_cd).ThenBy(p => p.site_no);
        }

        protected void rgNTSStatus_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgNTSStatus.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" | menu.Items[i].Text == "Contains" | menu.Items[i].Text == "EqualTo" | menu.Items[i].Text == "DoesNotContain")
                    i = i + 1;
                else
                    menu.Items.RemoveAt(i);
            }
        }

        protected void rgNTSStatus_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                int rms_record_id = Convert.ToInt32(item.GetDataKeyValue("rms_record_id"));
            }
        }
        #endregion
    }
}