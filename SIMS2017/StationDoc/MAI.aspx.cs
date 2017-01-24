using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SIMS2017.StationDoc
{
    public partial class MAI : System.Web.UI.Page
    {
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        private SIMSDevService.SIMSServiceClient svcSIMS = new SIMSDevService.SIMSServiceClient();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        public Boolean HasApproveAccess { get; set; }
        private Data.SiteElement currSiteElement;
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
        private int SiteID
        {
            get
            {
                if (Session["SiteID"] == null) return 0; else return (int)Session["SiteID"];
            }
            set
            {
                Session["SiteID"] = value;
            }
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            if (!Page.IsPostBack) Session.Clear();
            
            //If no office is passed, redirect to the homepage
            string office_id = Request.QueryString["office_id"];
            if (!string.IsNullOrEmpty(office_id)) OfficeID = Convert.ToInt32(office_id); else Response.Redirect(Config.SIMS2017URL + "SIMSWSCHome.aspx");

            //Set the the WSC ID based on the passed in Office ID
            WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            Page.Title = "SIMS - Site Manuscript Approval Interface";
            ph1.Title = "Site Manuscript Approval Interface";
            ph1.SubTitle = "Viewing Manuscripts for the " + db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).wsc_nm + " WSC";

            if (!Page.IsPostBack)
            {
                //--PAGE ACCESS SECTION-------------------------------------------------------------
                try
                {
                    Master.CheckAccessLevel(w.ID, "None");

                    if (Master.NoAccessPanel == false & (u.AccessLevel == "WSC" | u.AccessLevel == "SuperUser"))
                    {
                        Session["showapprove"] = "true";
                    }
                    else
                    {
                        Session["showapprove"] = "false";
                    }
                }
                catch (Exception ex)
                {
                    Session["showapprove"] = "false";
                }
                //--END PAGE ACCESS SECTION---------------------------------------------------------
            }
        }

        #region "Properties"
        /// <summary>
        /// Gets the DataTable used for the RadGrids on the page
        /// </summary>
        /// <param name="approveonly">Send "1" for MANUs needing approval only, and "0" for status of all</param>
        /// <param name="activeonly">Send 0 for all sites, 1 for active only, and 2 for inactive only</param>
        private DataTable MANUStatus
        {
            get
            {
                DataTable dt = new DataTable();

                using (SqlConnection cnx = new SqlConnection(Config.ConnectionInfo))
                {
                    cnx.Open();
                    SqlCommand cmd = new SqlCommand("SP_Elem_Approval_Report", cnx);
                    cmd.CommandType = Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@report_type", SqlDbType.NVarChar).Value = "MANU";
                    cmd.Parameters.Add("@activeonly", SqlDbType.Int).Value = activeonly;
                    cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = w.ID;
                    cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = site_id;
                    cmd.Parameters.Add("@approveonly", SqlDbType.Bit).Value = approveonly;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    cnx.Close();
                }
                return dt;
            }
        }
        #endregion

        #region "Approve MANU RadGrid"
        protected void rgApprove_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            rgApprove.DataSource = MANUStatus[1, 1, 0];
        }

        //Specify which items appear in FilterMenu
        protected void rgApprove_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgApprove.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items(i).Text == "NoFilter" | menu.Items(i).Text == "Contains" | menu.Items(i).Text == "EqualTo" | menu.Items(i).Text == "DoesNotContain")
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

                string site_no = (DataRowView)item.DataItem("site_no").ToString().Trim();
                HyperLink hlSiteNo = (HyperLink)item.FindControl("hlSiteNo");

                hlSiteNo.Attributes("target") = "_blank";
                hlSiteNo.Attributes("href") = Config.SitePath + "StationInfo.asp?site_no=" + site_no + "&agency_cd=USGS";
            }

            //Set custom column header tooltips
            if (e.Item is GridHeaderItem)
            {
                GridHeaderItem header = (GridHeaderItem)e.Item;

                header("sitefile_md").ToolTip = "The date when data in the NWISWeb SITEFILE was last modified.";
                header("revised_dt").ToolTip = "The date when an element in the manuscript was last modified.";
                header("approved_dt").ToolTip = "The date when the manuscript was last approved.";
            }
        }
        #endregion

        #region "All Sites Status RadGrid"
        protected void rgAllSites_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            rgAllSites.DataSource = MANUStatus[0, Session["activeonly"], 0];
        }

        //Specify which items appear in FilterMenu
        protected void rgAllSites_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgAllSites.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items(i).Text == "NoFilter" | menu.Items(i).Text == "Contains" | menu.Items(i).Text == "EqualTo" | menu.Items(i).Text == "DoesNotContain")
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

                string site_no = (DataRowView)item.DataItem("site_no").ToString().Trim();
                HyperLink hlSiteNo = (HyperLink)item.FindControl("hlSiteNo");

                hlSiteNo.Attributes("target") = "_blank";
                hlSiteNo.Attributes("href") = Config.SitePath + "StationInfo.asp?site_no=" + site_no + "&agency_cd=USGS";
            }

            //Set custom column header tooltips
            if (e.Item is GridHeaderItem)
            {
                GridHeaderItem header = (GridHeaderItem)e.Item;

                header("sitefile_md").ToolTip = "The date when data in the NWISWeb SITEFILE was last modified.";
                header("revised_dt").ToolTip = "The date when an element in the manuscript was last modified.";
                header("approved_dt").ToolTip = "The date when the manuscript was last approved.";
                header("needs_approval").ToolTip = "A Manuscript requires approval when a manuscript element has been changed since the last approval date.";
                header("SendToNWISWeb").ToolTip = "The Go! button allows a manual push of an approved manuscript to NWISWeb. A user may want this if the manuscript is not showing on NWISWeb or if autogenerated fields from NWIS have been changed and need to be updated.";
            }
        }

        protected void btnNWISWebSend_Command(object sender, CommandEventArgs e)
        {
            using (SqlConnection cnx = new SqlConnection(Config.ConnectionInfo))
            {
                cnx.Open();

                try
                {
                    string sql = "UPDATE Elem_Report_Approve" + " SET publish_complete = 'N'" + " WHERE site_id = " + e.CommandArgument.ToString() + " And report_type_cd = 'MANU'";
                    SqlCommand cmd = new SqlCommand(sql, cnx);
                    cmd.ExecuteNonQuery();

                    rgAllSites.Rebind();
                }
                catch (Exception ex)
                {
                }

                cnx.Close();
            }
        }

        public string GetVisibleValue(string publish_complete)
        {
            string visible_va = "True";

            if (publish_complete == "N")
            {
                visible_va = "False";
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
            Session["activeonly"] = 1;
            lbActiveSiteToggle.CommandArgument = "viewInactive";
            lbActiveSiteToggle.Text = "Click to view inactive sites";
            ltlActiveSiteToggle.Text = "Viewing active sites only";

            rgAllSites.Rebind();
            rgApprove.Rebind();
        }
        #endregion

        public void lbActiveSiteToggle_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument == "viewInactive")
            {
                Session["activeonly"] = 2;
                lbActiveSiteToggle.CommandArgument = "viewActive";
                lbActiveSiteToggle.Text = "Click to view active sites";
                ltlActiveSiteToggle.Text = "Viewing inactive sites only";
            }
            else
            {
                Session["activeonly"] = 1;
                lbActiveSiteToggle.CommandArgument = "viewInactive";
                lbActiveSiteToggle.Text = "Click to view inactive sites";
                ltlActiveSiteToggle.Text = "Viewing active sites only";
            }

            rgAllSites.Rebind();
        }

    }
}