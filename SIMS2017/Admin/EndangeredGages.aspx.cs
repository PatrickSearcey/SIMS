using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SIMS2017.Admin
{
    public partial class EndangeredGages : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        public Boolean HasEditAccess { get; set; }
        private Data.WSC currWSC;
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
                SetupFilterControls();
            }
        }

        #region Page Load Events
        protected void UserControlSetup()
        {
            string wsc_nm = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).wsc_nm;
            ph1.Title = "Endangered Gages List";

            ph1.SubTitle = "For the " + wsc_nm + " WSC";
            ph1.ShowOfficeInfoPanel = true;
        }

        protected void SetupFilterControls()
        {
            rddlOffice.DataSource = db.Offices.Where(p => p.wsc_id == WSCID).OrderBy(p => p.office_nm).ToList();
            rddlOffice.DataBind();
            rddlOffice.Items.Insert(0, new DropDownListItem { Value = "0", Text = "All Offices" });
            rddlOffice.SelectedValue = OfficeID.ToString();

            selOfficeID = Convert.ToInt32(rddlOffice.SelectedValue);
        }
        #endregion

        #region Misc Events
        protected void ram_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            selOfficeID = Convert.ToInt32(rddlOffice.SelectedValue);
            rgEndangeredGages.Rebind();
            rgCurrentStatus.Rebind();
        }

        protected void UpdateDetails(object sender, EventArgs e)
        {
            selOfficeID = Convert.ToInt32(rddlOffice.SelectedValue);
            rgEndangeredGages.Rebind();
            rgCurrentStatus.Rebind();
        }
        #endregion

        #region rgEndangeredGages
        protected void rgEndangeredGages_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var data = db.SP_RMS_EndangeredGages_by_WSC_or_Office(selOfficeID, WSCID).ToList();
            rgEndangeredGages.DataSource = data;
        }

        protected void rgEndangeredGages_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgEndangeredGages.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" | menu.Items[i].Text == "Contains" | menu.Items[i].Text == "EqualTo" | menu.Items[i].Text == "DoesNotContain")
                    i = i + 1;
                else
                    menu.Items.RemoveAt(i);
            }
        }

        protected void rgEndangeredGages_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;

                int rms_record_id = Convert.ToInt32(item.GetDataKeyValue("rms_record_id"));
                var currRecord = db.Records.FirstOrDefault(p => p.rms_record_id == rms_record_id);
                
                LinkButton lbEdit = (LinkButton)item.FindControl("lbEdit");
                lbEdit.OnClientClick = String.Format("openWin('{0}','record', '{1}'); return false;", currRecord.rms_record_id, Config.SIMSURL);

                HyperLink hlSiteNo = (HyperLink)item.FindControl("hlSiteNo");
                hlSiteNo.NavigateUrl = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMSURL, currRecord.site_id);
            }
        }
        #endregion

        #region rgCurrentStatus
        protected void rgCurrentStatus_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var data = db.vEndangeredGagesCurrentStatus.Where(p => p.wsc_id == WSCID);

            if (selOfficeID != 0) rgCurrentStatus.DataSource = data.Where(p => p.office_id == selOfficeID).ToList();
            else rgCurrentStatus.DataSource = data.ToList();
        }

        protected void rgCurrentStatus_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e)
        {
            GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;
            int site_id = Convert.ToInt32(dataItem.GetDataKeyValue("site_id"));

            var data = db.vEndangeredGagesHistories.Where(p => p.site_id == site_id).OrderBy(p => p.rms_record_id).ThenBy(p => p.entered_dt).ToList();
            e.DetailTableView.DataSource = data;
        }

        protected void rgCurrentStatus_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgCurrentStatus.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" | menu.Items[i].Text == "Contains" | menu.Items[i].Text == "EqualTo" | menu.Items[i].Text == "DoesNotContain")
                    i = i + 1;
                else
                    menu.Items.RemoveAt(i);
            }
        }

        protected void rgCurrentStatus_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
            }
        }
        #endregion
    }
}