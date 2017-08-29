using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Data;

namespace SIMS2017.StationDoc
{
    public partial class MAI : System.Web.UI.Page
    {
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private int SiteID;
        private Boolean ApproveOnly = false;
        private Boolean HasApproveAccess
        {
            get
            {
                if (Session["HasApproveAccess"] == null) return false; else return Convert.ToBoolean(Session["HasApproveAccess"]);
            }
            set
            {
                Session["HasApproveAccess"] = value;
            }
        }
        private int ActiveOnly
        {
            get
            {
                if (Session["ActiveOnly"] == null) return 1; else return (int)Session["ActiveOnly"];
            }
            set
            {
                Session["ActiveOnly"] = value;
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

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            if (!Page.IsPostBack) Session.Clear();
            
            //If no office is passed, redirect to the homepage
            string office_id = Request.QueryString["office_id"];
            if (!string.IsNullOrEmpty(office_id)) OfficeID = Convert.ToInt32(office_id); else Response.Redirect(Config.SIMSURL + "SIMSWSCHome.aspx");

            //Set the the WSC ID based on the passed in Office ID
            WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            Page.Title = "SIMS - Site Manuscript Approval Interface";
            ph1.Title = "Site Manuscript Approval Interface";
            ph1.SubTitle = "Viewing Manuscripts for the " + db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).wsc_nm + " WSC";
            ph1.ShowOfficeInfoPanel = true;

            if (!Page.IsPostBack)
            {
                //If the user belongs to this site's WSC (or has an exception to work in the WSC) and is a WSC level users, or is a SuperUser, then allow them to approve MANUs
                if ((user.WSCID.Contains(WSCID) && user.IsAdmin) || user.IsSuperUser) HasApproveAccess = true; else HasApproveAccess = false;
            }
        }

        #region Properties
        /// <summary>
        /// Gets the list of data used for the RadGrids on the page
        /// </summary>
        private List<Data.ElementApprovalItem> MANUStatus
        {
            get
            {
                List<Data.ElementApprovalItem> dt = new List<Data.ElementApprovalItem>();

                //ActiveOnly: Send 0 for all sites, 1 for active only, and 2 for inactive only
                //ApproveOnly: Send true for MANUs needing approval only, and false for status of all
                dt = db.SP_Elem_Approval_Report("MANU", ActiveOnly, WSCID, SiteID, ApproveOnly).ToList();

                return dt;
            }
        }
        #endregion

        #region Approve MANU RadGrid
        protected void rgApprove_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            ApproveOnly = true;
            ActiveOnly = 1;
            SiteID = 0;

            rgApprove.DataSource = MANUStatus;
        }

        //Specify which items appear in FilterMenu
        protected void rgApprove_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgApprove.FilterMenu;
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

        //Setup in-grid links
        protected void rgApprove_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;

                int site_id = Convert.ToInt32(item.GetDataKeyValue("site_id"));
                HyperLink hlSiteNo = (HyperLink)item.FindControl("hlSiteNo");

                hlSiteNo.Attributes["target"] = "_blank";
                hlSiteNo.Attributes["href"] = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMSURL, site_id);
            }

            //Set custom column header tooltips
            if (e.Item is GridHeaderItem)
            {
                GridHeaderItem header = (GridHeaderItem)e.Item;

                header["sitefile_md"].ToolTip = "The date when data in the NWISWeb SITEFILE was last modified.";
                header["revised_dt"].ToolTip = "The date when an element in the manuscript was last modified.";
                header["approved_dt"].ToolTip = "The date when the manuscript was last approved.";
            }
        }
        #endregion

        #region All Sites Status RadGrid
        protected void rgAllSites_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            ApproveOnly = false;
            SiteID = 0;

            rgAllSites.DataSource = MANUStatus;
        }

        //Specify which items appear in FilterMenu
        protected void rgAllSites_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgAllSites.FilterMenu;
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

        //Setup in-grid links
        protected void rgAllSites_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;

                int site_id = Convert.ToInt32(item.GetDataKeyValue("site_id"));
                HyperLink hlSiteNo = (HyperLink)item.FindControl("hlSiteNo");
                Button btnNWISWebSend = (Button)item.FindControl("btnNWISWebSend");

                hlSiteNo.Attributes["target"] = "_blank";
                hlSiteNo.Attributes["href"] = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMSURL, site_id);
            }

            //Set custom column header tooltips
            if (e.Item is GridHeaderItem)
            {
                GridHeaderItem header = (GridHeaderItem)e.Item;

                header["sitefile_md"].ToolTip = "The date when data in the NWISWeb SITEFILE was last modified.";
                header["revised_dt"].ToolTip = "The date when an element in the manuscript was last modified.";
                header["approved_dt"].ToolTip = "The date when the manuscript was last approved.";
                header["needs_approval"].ToolTip = "A Manuscript requires approval when a manuscript element has been changed since the last approval date.";
                header["SendToNWISWeb"].ToolTip = "The Go! button allows a manual push of an approved manuscript to NWISWeb. A user may want this if the manuscript is not showing on NWISWeb or if autogenerated fields from NWIS have been changed and need to be updated.";
            }
        }

        protected void btnNWISWebSend_Command(object sender, CommandEventArgs e)
        {
            SiteID = Convert.ToInt32(e.CommandArgument);
            var era = db.ElemReportApproves.FirstOrDefault(p => p.site_id == SiteID && p.report_type_cd == "MANU");
            era.publish_complete = "N";
            db.SubmitChanges();

            rgAllSites.Rebind();
        }

        public bool GetVisibleValue(string publish_complete)
        {
            bool visible_va = true;

            if (publish_complete == "N")
            {
                visible_va = false;
            }

            return visible_va;
        }
        #endregion

        #region "Tab Strip Events"
        /// <summary>
        /// Refreshes the active sites and inactive sites status grids when the tab is clicked - to show just approved MANUs
        /// </summary>
        protected void rts1_TabClick(object sender, RadTabStripEventArgs e)
        {
            //Reset the initial view of the grid to show active sites
            ActiveOnly = 1;
            lbActiveSiteToggle.CommandArgument = "viewInactive";
            lbActiveSiteToggle.Text = "Click to view inactive sites";
            ltlActiveSiteToggle.Text = "Viewing active sites only";

            rgAllSites.Rebind();
            rgApprove.Rebind();
        }
        #endregion

        public void lbActiveSiteToggle_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument.ToString() == "viewInactive")
            {
                ActiveOnly = 2;
                lbActiveSiteToggle.CommandArgument = "viewActive";
                lbActiveSiteToggle.Text = "Click to view active sites";
                ltlActiveSiteToggle.Text = "Viewing inactive sites only";
            }
            else
            {
                ActiveOnly = 1;
                lbActiveSiteToggle.CommandArgument = "viewInactive";
                lbActiveSiteToggle.Text = "Click to view inactive sites";
                ltlActiveSiteToggle.Text = "Viewing active sites only";
            }

            rgAllSites.Rebind();
        }

    }
}