using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Safety
{
    public partial class TCPEdit : System.Web.UI.Page
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
        private int TCPID
        {
            get
            {
                if (Session["TCPID"] == null) return 0; else return (int)Session["TCPID"];
            }
            set
            {
                Session["TCPID"] = value;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            //If no site_id was passed, then redirect back to the homepage
            string site_id = "3000336";// Request.QueryString["site_id"];
            if (!string.IsNullOrEmpty(site_id)) SiteID = Convert.ToInt32(site_id); else Response.Redirect(Config.SIMS2017URL + "SIMSWSCHome.aspx");

            //Using the passed site_id, setup the site data element, and reset the office and wsc to match that of the current site
            currSite = db.Sites.Where(p => p.site_id == SiteID).FirstOrDefault();
            OfficeID = (int)currSite.office_id;
            WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            ph1.Title = "Manage Traffic Control Safety Plans";
            ph1.SubTitle = currSite.site_no + " " + currSite.station_full_nm;

            if (!Page.IsPostBack)
            {
                //If the user belongs to this site's WSC (or has an exception to work in the WSC), or is a SuperUser, then allow them to edit the page
                if (user.WSCID.Contains(WSCID) || user.IsSuperUser) HasEditAccess = true;

                if (currSite.TCPSite != null) PopulatePageData();
                
                SetupPermission();
            }
        }

        protected void PopulatePageData()
        {
            //Site Specific Information
            if (currSite.TCPSite.RemoteSite != null)
            {
                rddlRemote.SelectedValue = currSite.TCPSite.RemoteSite.ToString();
                if ((bool)currSite.TCPSite.RemoteSite)
                {
                    DisableFields();
                    return;
                }
            }
            rtbRoadName.Text = currSite.TCPSite.RoadName;
            if (currSite.TCPSite.Expressway != null)
            {
                rddlExpressway.SelectedValue = currSite.TCPSite.Expressway.ToString();
            }
            
        }

        protected void SetupPermission()
        {
            rbSubmit.Enabled = HasEditAccess;
            rbCancel.Enabled = HasEditAccess;
        }

        protected void DisableFields()
        {
            rtbRoadName.Enabled = false;
            rfvRoadName.Enabled = false;
        }

        #region Page Events
        protected void rbSubmit_Command(object sender, CommandEventArgs e)
        {
            
        }

        protected void rbCancel_Command(object sender, CommandEventArgs e)
        {

        }

        protected void rddlRemote_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        #endregion
    }
}