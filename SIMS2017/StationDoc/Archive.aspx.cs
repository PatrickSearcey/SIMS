using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIMS2017.StationDoc
{
    public partial class Archive : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        private SIMSDevService.SIMSServiceClient svcSIMS = new SIMSDevService.SIMSServiceClient();
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
            string site_id = Request.QueryString["site_id"];
            if (!string.IsNullOrEmpty(site_id)) SiteID = Convert.ToInt32(site_id); else Response.Redirect(Config.SIMS2017URL + "SIMSWSCHome.aspx");

            //Using the passed site_id, setup the site data element, and reset the office and wsc to match that of the current site
            currSite = db.Sites.Where(p => p.site_id == SiteID).FirstOrDefault();
            OfficeID = (int)currSite.office_id;
            WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            ph1.Title = "Retrieve Archived Elements";
            ph1.SubTitle = currSite.site_no + " " + currSite.station_full_nm;

            if (!Page.IsPostBack)
            {
                //If the user belongs to this site's WSC (or has an exception to work in the WSC), or is a SuperUser, then allow them to edit the page
                if (user.WSCID.Contains(WSCID) || user.IsSuperUser) HasEditAccess = true;

                gvElementList.DataSource = db.SP_Element_Info_Archives(SiteID).ToList();
                gvElementList.DataBind();

                pnlStep1.Visible = true;
                pnlStep2.Visible = false;
                pnlStep3.Visible = false;
            }
        }

        protected void ElementSelected(object sender, CommandEventArgs e)
        {
            pnlStep1.Visible = false;
            pnlStep2.Visible = true;
            pnlStep3.Visible = false;
        }

        protected void RetrieveArchivedInfo(object sender, CommandEventArgs e)
        {
            pnlStep1.Visible = false;
            pnlStep2.Visible = false;
            pnlStep3.Visible = true;
        }

        protected void Back_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName.ToString() == "back2")
            {
                pnlStep1.Visible = false;
                pnlStep2.Visible = true;
                pnlStep3.Visible = false;
            }
            else
            {
                gvElementList.DataSource = db.SP_Element_Info_Archives(SiteID).ToList();
                gvElementList.DataBind();

                pnlStep1.Visible = true;
                pnlStep2.Visible = false;
                pnlStep3.Visible = false;
            }
        }

        protected void ShowArchivedInfo()
        {
            lblElementName2.Text = "";

            var archived_info = db.SP_Element_Info_Archives_by_element_id(currSite.site_id, 34, Convert.ToDateTime("01/01/1990"), DateTime.Now).ToList();
            if (archived_info != null)
            {
                dlElementInfo.DataSource = archived_info;
                dlElementInfo.DataBind();
                lblNothingReturned.Visible = false;
                dlElementInfo.Visible = true;
            }
            else
            {
                dlElementInfo.Visible = false;
                lblNothingReturned.Text = "No archive information exists for this element";
                lblNothingReturned.Visible = true;
            }
        }

        protected void gvElementList_Bound(object sender, EventArgs e)
        {

        }

    }
}