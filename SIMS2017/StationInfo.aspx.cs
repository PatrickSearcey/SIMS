using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SIMS2017
{
    public partial class StationInfo : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        public Boolean HasEditAccess { get; set; }
        private Data.Site currSite;
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
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            //If no site_id was passed, then redirect back to the homepage
            string site_id = "3000336"; // Request.QueryString["site_id"];
            if (!string.IsNullOrEmpty(site_id)) SiteID = Convert.ToInt32(site_id); else Response.Redirect(Config.SIMS2017URL + "SIMSWSCHome.aspx");

            //Using the passed site_id, setup the site data element, and reset the office and wsc to match that of the current site
            currSite = db.Sites.Where(p => p.site_id == SiteID).FirstOrDefault();
            OfficeID = (int)currSite.office_id;
            WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            ph1.Title = "Station Information Page";
            ph1.SubTitle = currSite.site_no + " " + currSite.station_full_nm;

            if (!Page.IsPostBack)
            {
                //If the user belongs to this site's WSC (or has an exception to work in the WSC), or is a SuperUser, then allow them to edit the page
                if (user.WSCID.Contains(WSCID) || user.IsSuperUser) HasEditAccess = true; 

                SetupHyperlinks();
                PopulatePageData();
                SetupPermission();
            }
        }

        #region Page Load Methods
        protected void PopulatePageData()
        {
            //Station Details
            ltlPubName.Text = currSite.station_full_nm;
            rtbPubName.Text = currSite.station_full_nm;
            ltlOffice.Text = currSite.Office.office_nm;
            rddlOffice.DataSource = db.Offices.Where(p => p.wsc_id == WSCID).Select(p => new { office_id = p.office_id, office_nm = p.office_nm }).ToList();
            rddlOffice.DataBind();
            rddlOffice.SelectedValue = OfficeID.ToString();
            string fieldtrips = "";
            foreach (var trip in currSite.TripSites.ToList())
            {
                fieldtrips += trip.Trip.trip_nm + " - " + trip.Trip.user_id + ", ";
            }
            ltlFieldTrip.Text = fieldtrips.TrimEnd(' ').TrimEnd(',');
            lbEditFieldTrip.OnClientClick = String.Format("return EditFieldTrips('{0}');", currSite.site_id);

            //Station Documents

            //CRP

            //Safety

            //DCP/Realtime Ops
        }

        protected void SetupHyperlinks()
        {
            hlNWISWeb.NavigateUrl = String.Format("http://waterdata.usgs.gov/nwis/inventory/?site_no={0}&agency_cd={1}", currSite.site_no, currSite.agency_cd);
            hlNWISOpsRequest.NavigateUrl = String.Format("{0}NWISOpsRequest.aspx?office_id={1}&site_id={2}", Config.SIMS2017URL, OfficeID, SiteID);
        }

        protected void SetupPermission()
        {
            lbEditPubName.Visible = HasEditAccess;
            lbEditOffice.Visible = HasEditAccess;
            lbEditFieldTrip.Visible = HasEditAccess;
        }
        #endregion

        #region Edit Events
        protected void Edit_Command(object sender, CommandEventArgs e)
        {
            switch (e.CommandArgument.ToString())
            {
                case "PubName":
                    pnlPubNameEdit.Visible = true;
                    pnlPubNameView.Visible = false;
                    break;
                case "Office":
                    pnlOfficeEdit.Visible = true;
                    pnlOfficeView.Visible = false;
                    break;
            }
        }

        protected void Commit_Command(object sender, EventArgs e)
        {
            switch (sender.GetType().Name)
            {
                case "RadButton":
                    Button_Logic((RadButton)sender);
                    break;
                case "RadDropDownList":
                    DropDownList_Logic((RadDropDownList)sender);
                    break;
            }

            db.SubmitChanges();
        }

        protected void Button_Logic(RadButton rb)
        {
            switch (rb.ID)
            {
                case "rbPubName":
                    currSite.station_full_nm = rtbPubName.Text;
                    ltlPubName.Text = rtbPubName.Text;

                    //refresh the page header info
                    ph1.SubTitle = currSite.site_no + " " + rtbPubName.Text;
                    ph1.RefreshHeadingData();

                    pnlPubNameEdit.Visible = false;
                    pnlPubNameView.Visible = true;
                    break;
            }
        }

        protected void DropDownList_Logic(RadDropDownList rddl)
        {
            switch (rddl.ID)
            {
                case "rddlOffice":
                    currSite.office_id = Convert.ToInt32(rddl.SelectedValue);
                    OfficeID = Convert.ToInt32(rddl.SelectedValue);
                    ltlOffice.Text = db.Offices.Where(p => p.office_id == Convert.ToInt32(rddl.SelectedValue)).FirstOrDefault().office_nm;
                    //refresh the page header info
                    ph1.RefreshHeadingData();

                    pnlOfficeEdit.Visible = false;
                    pnlOfficeView.Visible = true;
                    break;
            }
        }
        #endregion

        #region Misc Events
        protected void rbJump_Click(object sender, EventArgs e)
        {
            string site_no = rtbSiteNo.Text;
            if (!string.IsNullOrEmpty(site_no))
            {
                int site_id = db.Sites.Where(p => p.site_no == site_no).FirstOrDefault().site_id;
                Response.Redirect(String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMS2017URL, site_id));
            }
        }
        #endregion
    }
}