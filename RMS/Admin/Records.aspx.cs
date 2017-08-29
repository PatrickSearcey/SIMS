using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace RMS.Admin
{
    public partial class Records : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
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
        private string onlyactive;
        private int selOfficeID;
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
                UserControlSetup();
                SetupFilterControls();
            }
        }

        #region Page Load Events
        protected void UserControlSetup()
        {
            string wsc_nm = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).wsc_nm;
            ph1.Title = "Manage Record Configurations";

            ph1.SubTitle = "For the " + wsc_nm + " WSC";
            ph1.RecordType = "&nbsp;";
        }
        
        protected void SetupFilterControls()
        {
            rddlOffice.DataSource = db.Offices.Where(p => p.wsc_id == WSCID).OrderBy(p => p.office_nm).ToList();
            rddlOffice.DataBind();
            rddlOffice.Items.Insert(0, new DropDownListItem { Value = "0", Text = "All Offices" });
            rddlOffice.SelectedValue = OfficeID.ToString();

            selOfficeID = Convert.ToInt32(rddlOffice.SelectedValue);
            onlyactive = "yes";
        }
        #endregion

        #region Misc Events
        protected void ram_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            if (e.Argument == "RebindGrids")
            {
                rgRecords.Rebind();
                rgSites.Rebind();
            }
        }

        protected void UpdateDetails(object sender, EventArgs e)
        {
            if (rrblRecords.SelectedIndex == 0) onlyactive = "yes"; else onlyactive = "no";

            selOfficeID = Convert.ToInt32(rddlOffice.SelectedValue);

            rgRecords.Rebind();
        }
        #endregion

        #region rgSites
        protected void rgSites_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            rgSites.DataSource = db.Sites.Where(p => p.Office.wsc_id == WSCID && p.Records.FirstOrDefault() == null).Select(p => new
            {
                office_nm = p.Office.office_nm,
                site_no = p.site_no,
                station_nm = p.station_full_nm,
                site_id = p.site_id
            }).OrderBy(p => p.office_nm).ThenBy(p => p.site_no).ToList();
        }

        protected void rgSites_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgSites.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" | menu.Items[i].Text == "Contains" | menu.Items[i].Text == "EqualTo" | menu.Items[i].Text == "DoesNotContain")
                    i = i + 1;
                else
                    menu.Items.RemoveAt(i);
            }
        }

        protected void rgSites_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;

                int site_id = Convert.ToInt32(item.GetDataKeyValue("site_id"));
                var currSite = db.Sites.FirstOrDefault(p => p.site_id == site_id);

                LinkButton lbAssignRecord = (LinkButton)item.FindControl("lbAssignRecord");
                lbAssignRecord.OnClientClick = String.Format("openWin('{0}','newrecord', '{1}'); return false;", currSite.site_id, Config.SIMSURL);
            }

        }
        #endregion

        #region rgRecords
        protected void rgRecords_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var data = db.SP_RMS_Record_Details_by_WSC_or_office(selOfficeID, WSCID, onlyactive).ToList();
            rgRecords.DataSource = data;

            ltlNumberOfRecords.Text = "Number of records returned: <b>" + data.Count.ToString() + "</b>";
        }

        protected void rgRecords_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgRecords.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" | menu.Items[i].Text == "Contains" | menu.Items[i].Text == "EqualTo" | menu.Items[i].Text == "DoesNotContain")
                    i = i + 1;
                else
                    menu.Items.RemoveAt(i);
            }
        }

        protected void rgRecords_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;

                int rms_record_id = Convert.ToInt32(item.GetDataKeyValue("rms_record_id"));
                var currRecord = db.Records.FirstOrDefault(p => p.rms_record_id == rms_record_id);

                LinkButton lbAssignRecord = (LinkButton)item.FindControl("lbAssignRecord");
                LinkButton lbEditRecord = (LinkButton)item.FindControl("lbEditRecord");
                lbAssignRecord.OnClientClick = String.Format("openWin('{0}','newrecord', '{1}'); return false;", currRecord.site_id, Config.SIMSURL);
                lbEditRecord.OnClientClick = String.Format("openWin('{0}','record', '{1}'); return false;", currRecord.rms_record_id, Config.SIMSURL);

                HyperLink hlSiteNo = (HyperLink)item.FindControl("hlSiteNo");
                hlSiteNo.NavigateUrl = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMSURL, currRecord.site_id);

                if (Convert.ToBoolean(currRecord.not_used_fg))
                {
                    item["office_cd"].ForeColor = System.Drawing.Color.LightGray;
                    item["station_nm"].ForeColor = System.Drawing.Color.LightGray;
                    item["analyzer_uid"].ForeColor = System.Drawing.Color.LightGray;
                    item["approver_uid"].ForeColor = System.Drawing.Color.LightGray;
                    item["ts_fg"].ForeColor = System.Drawing.Color.LightGray;
                    item["category_no"].ForeColor = System.Drawing.Color.LightGray;
                    item["cat_reason"].ForeColor = System.Drawing.Color.LightGray;
                    item["ts_full_ds"].ForeColor = System.Drawing.Color.LightGray;
                }
            }
        }
        #endregion
    }
}