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
    public partial class SiteMAI : System.Web.UI.Page
    {
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private Data.Site site;
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

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            if (!Page.IsPostBack) Session.Clear();

            //If no site is passed, redirect to the homepage
            string site_id = Request.QueryString["site_id"];
            string site_no = Request.QueryString["site_no"];
            string agency_cd = Request.QueryString["agency_cd"];
            if (!string.IsNullOrEmpty(site_id) || !string.IsNullOrEmpty(site_no) && !string.IsNullOrEmpty(agency_cd))
            {
                if (!string.IsNullOrEmpty(site_id)) site = db.Sites.FirstOrDefault(p => p.site_id.ToString() == site_id);
                else site = db.Sites.FirstOrDefault(p => p.site_no == site_no && p.agency_cd == agency_cd);

                OfficeID = Convert.ToInt32(site.office_id);
            }
            else Response.Redirect(Config.SIMSURL + "SIMSWSCHome.aspx");

            //Set the the WSC ID based on the passed in Office ID
            WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            Page.Title = "SIMS - Site Manuscript Approval Interface";
            ph1.Title = "Site Manuscript Approval Interface";
            ph1.SubTitle = site.site_no + " " + site.station_full_nm;
            ph1.ShowOfficeInfoPanel = true;

            if (!Page.IsPostBack)
            {
                //If the user belongs to this site's WSC (or has an exception to work in the WSC) and is a WSC level users, or is a SuperUser, then allow them to approve MANUs
                if ((user.WSCID.Contains(WSCID) && user.IsAdmin) || user.IsSuperUser) HasApproveAccess = true; else HasApproveAccess = false;

                //Populate the site selection drop down and ApproveMANU control, only if first visit to page
                ucApproveMANU.ShowCloseButton = false;
                ucApproveMANU.SiteID = site.site_id;

                //For the site selection list, we only want sites that have time-series record types and have a record for MANU in the Elem_Report_Sum table
                
                var siteList = db.Records
                    .Where(p => p.RecordType.ts_fg == true && p.Site.Office.wsc_id == WSCID && p.Site.ElemReportSums.All(c => c.report_type_cd == "MANU"))
                    .Select(p => new { site_id = p.site_id, site_no_nm = p.Site.site_no + " " + p.Site.station_full_nm })
                    .OrderBy(p => p.site_no_nm);
                rcbSites.DataTextField = "site_no_nm";
                rcbSites.DataValueField = "site_id";
                rcbSites.DataSource = siteList.Distinct();
                rcbSites.DataBind();
                rcbSites.SelectedValue = site.site_id.ToString();

                //Populate the link boxes at the top of the page. Only populate the hlOriginalSite link if first visit to page
                hlCurrentSite.NavigateUrl = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMSURL, site.site_id);
                hlCurrentSite.Text = site.site_no + " " + site.station_full_nm;

                hlOriginalSite.NavigateUrl = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMSURL, site.site_id);
                hlOriginalSite.Text = site.site_no + " " + site.station_full_nm;

                hlFullReport.NavigateUrl = String.Format("{0}StationDoc/MAI.aspx?office_id={1}", Config.SIMSURL, OfficeID);
            }
        }

        protected void ucApproveMANU_SubmitEvent(object sender, CommandEventArgs e)
        {
            rgSiteDetails.DataSource = null;
            rgSiteDetails.Rebind();
        }

        #region Site Selection
        protected void btnSubmitSite_Command(object source, CommandEventArgs e)
        {
            string agency_cd = "USGS", site_no = null;
            int site_id;

            if (e.CommandArgument.ToString() == "choosesite")
            {
                if (!string.IsNullOrEmpty(rcbSites.SelectedValue.ToString()))
                {
                    site_id = Convert.ToInt32(rcbSites.SelectedValue);
                    site = db.Sites.FirstOrDefault(p => p.site_id == site_id);
                    RefreshPageData(site_id);
                    tbSiteNo.Text = "";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(tbSiteNo.Text))
                {
                    agency_cd = tbAgencyCd.Text;
                    site_no = tbSiteNo.Text;
                    site = db.Sites.FirstOrDefault(p => p.site_no == site_no && p.agency_cd == agency_cd);
                    RefreshPageData(site.site_id);
                    if (rcbSites.Items.Contains(new ListItem { Value = site.site_id.ToString(), Text = site.site_no + " " + site.station_full_nm }))
                        rcbSites.SelectedValue = site.site_id.ToString();
                }
            }
        }

        public void RefreshPageData(int site_id)
        {
            //Refresh the site details grid to show details for newly selected site
            rgSiteDetails.Rebind();

            //Update the Current Site link box at the top of the page
            hlCurrentSite.NavigateUrl = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMSURL, site.site_id);
            hlCurrentSite.Text = site.site_no + " " + site.station_full_nm;

            //Refresh the ApproveMANU user control to show the newly selected site's MANU
            ucApproveMANU.SiteID = site_id;
            ucApproveMANU.ShowCloseButton = false;
            ucApproveMANU.RefreshMANU();
        }
        #endregion

        #region Site Details RadGrid
        protected void rgSiteDetails_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            int site_id = 0;
            site_id = site.site_id;
            if (site_id == 0)
            {
                site_id = Convert.ToInt32(rcbSites.SelectedValue);
            }

            rgSiteDetails.DataSource = db.SP_Elem_Approval_Report("MANU", 0, WSCID, site_id, false);
        }

        //Setup in-grid links
        protected void rgSiteDetails_ItemDataBound(object sender, GridItemEventArgs e)
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
                header["needs_approval"].ToolTip = "A Manuscript requires approval when a manuscript element has been changed since the last approval date.";
            }
        }
        #endregion
    }
}