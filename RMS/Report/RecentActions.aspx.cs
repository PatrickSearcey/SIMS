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
    public partial class RecentActions : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private int reportOfficeID { get; set; }
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
            string wsc_id = Request.QueryString["wsc_id"];

            //If an office was passed, then use this to determine the WSCID
            if (!string.IsNullOrEmpty(office_id))
            {
                OfficeID = Convert.ToInt32(office_id);
                WSCID = Convert.ToInt32(db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id);
            }
            //If a WSC was passed, then set the local variable - don't need to worry about setting the OfficeID (the default view will be to show all users for all offfices
            else if (!string.IsNullOrEmpty(wsc_id))
            {
                WSCID = Convert.ToInt32(wsc_id);
                OfficeID = (int)db.Offices.FirstOrDefault(p => p.wsc_id == WSCID).office_id;
            }
            else
            {
                //If the office id and wsc id session variables are empty, set these values to the user's assigned office
                if (OfficeID == 0 && WSCID == 0)
                {
                    OfficeID = user.OfficeID;
                    WSCID = (int)db.Offices.FirstOrDefault(p => p.office_id == user.OfficeID).wsc_id;
                }
                else if (OfficeID == 0 && WSCID > 0)
                    OfficeID = db.Offices.FirstOrDefault(p => p.wsc_id == WSCID).office_id;
                else if (OfficeID > 0 && WSCID == 0)
                    WSCID = (int)db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id;
            }

            string wsc_nm = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).wsc_nm;
            ph1.Title = "Recent Actions";
            ph1.SubTitle = "For the " + wsc_nm + " WSC";
            ph1.RecordType = "&nbsp;";

            if (!Page.IsPostBack)
            {
                //On initial load, narrow down the view to the responsible office
                reportOfficeID = OfficeID;
                InitialDataBind();
            }
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
        }

        #region rgRecentActions
        protected void rgRecentActions_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            DateTime startDt = DateTime.Now.AddDays(-1), endDt = DateTime.Now;
            if (rdpStartDt.SelectedDate != null) startDt = Convert.ToDateTime(rdpStartDt.SelectedDate);
            if (rdpEndDt.SelectedDate != null) endDt = Convert.ToDateTime(rdpEndDt.SelectedDate);

            if (rddlOffice.SelectedIndex > 0) reportOfficeID = Convert.ToInt32(rddlOffice.SelectedValue); else reportOfficeID = 0;

            var ActionsData = db.SP_RMS_RecentActions(reportOfficeID, WSCID, startDt, endDt).Select(p => new
            {
                origin_va = p.origin_va,
                status_set_to_va = p.status_set_to_va,
                dialog_by = p.dialog_by,
                dialog_dt = p.dialog_dt,
                period_beg_dt = p.period_beg_dt,
                period_end_dt = p.period_end_dt,
                office_id = p.record_office_id,
                station_nm = p.station_nm,
                site_no = p.site_no,
                site_id = p.site_id,
                type_cd = p.type_cd,
                type_ds = p.type_ds,
                SIMSURL = Config.SIMSURL
            }).OrderBy(p => p.dialog_dt).ToList();

            rgRecentActions.DataSource = ActionsData;
            ltlNumberOfRecords.Text = "Number of records returned: <b>" + ActionsData.Count.ToString() + "</b>";

        }

        protected void rgRecentActions_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                GridDataItem item = e.Item as GridDataItem;
                int site_id = Convert.ToInt32(item.GetDataKeyValue("site_id"));

            }
        }

        protected void rgRecentActions_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgRecentActions.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" | menu.Items[i].Text == "Contains" | menu.Items[i].Text == "EqualTo" | menu.Items[i].Text == "DoesNotContain")
                {
                    i = i + 1;
                }
                else
                {
                    menu.Items.RemoveAt(i);
                }
            }
        }
        #endregion

        protected void rrblDateRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(rrblDateRange.SelectedValue.ToString()))
            {
                rdpEndDt.SelectedDate = DateTime.Now;
                
                switch (rrblDateRange.SelectedValue.ToString())
                {
                    case "day":
                        rdpStartDt.SelectedDate = DateTime.Now.AddDays(-1);
                        break;
                    case "week":
                        rdpStartDt.SelectedDate = DateTime.Now.AddDays(-6);
                        break;
                    case "month":
                        rdpStartDt.SelectedDate = DateTime.Now.AddDays(-30);
                        break;
                }
            }
        }

        protected void UpdateGrid(object sender, CommandEventArgs e)
        {
            rgRecentActions.Rebind();
        }
    }
}