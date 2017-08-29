using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SIMS2017
{
    public partial class SIMSHome : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
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
        private int TripID
        {
            get
            {
                if (Session["TripID"] == null) return 0; else return (int)Session["TripID"];
            }
            set
            {
                Session["TripID"] = value;
            }
        }
        #endregion 

        protected void Page_Load(object sender, EventArgs e)
        {
            ltlUserID.Text = user.ID;
            if (OfficeID == 0) OfficeID = Convert.ToInt32(Request.QueryString["office_id"]);
            if (OfficeID == 0) OfficeID = user.OfficeID;
            if (WSCID == 0) WSCID = Convert.ToInt32(Request.QueryString["wsc_id"]);
            if (WSCID == 0) WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            if (!Page.IsPostBack)
            {
                rddlWSC.DataSource = db.WSCs.Select(p => new { wsc_nm = p.wsc_nm, wsc_id = p.wsc_id }).OrderBy(p => p.wsc_nm).ToList();
                rddlWSC.DataBind();
                rddlWSC.Items.Insert(0, new DropDownListItem { Value = "", Text = "" });
            }

            SetupPageHyperlinks();
            ClearLocks();
        }

        //For setting up hyperlinks in the side and top menus
        protected void SetupPageHyperlinks()
        {
            rmTop.Items.Add(new RadMenuItem { Text = "SIMS National Home" });
            rmTop.Items.Add(new RadMenuItem { IsSeparator = true });
            rmTop.Items.Add(new RadMenuItem { Text = "Your SIMS WSC Home", NavigateUrl = String.Format("{0}SIMSWSCHome.aspx", Config.SIMS2017URL) });
            rmTop.Items.Add(new RadMenuItem { IsSeparator = true });
            rmTop.Items.Add(new RadMenuItem { Text = "Your RMS WSC Home", NavigateUrl = String.Format("{0}RMSWSCHome.aspx", Config.RMSURL) });
            rmTop.Items.Add(new RadMenuItem { IsSeparator = true });
            rmTop.Items.Add(new RadMenuItem { Text = "Latest News", NavigateUrl = "https://collaboration.usgs.gov/wg/owi/specialprojects/SIMS/Shared%20Documents/updates.html" });
            rmTop.Items.Add(new RadMenuItem { IsSeparator = true });
            rmTop.Items.Add(new RadMenuItem { Text = "Contact", NavigateUrl = String.Format("{0}NWISOpsRequest.aspx", Config.SIMS2017URL) });

            hlCRPCharts.NavigateUrl = String.Format("{0}SIMSReports/CRP/", Config.SIMSServerURL);
            hlCablewayInfo.NavigateUrl = String.Format("{0}CablewayReport.aspx?tp=nw", Config.SafetyURL);
            hlCablewayStatus.NavigateUrl = String.Format("{0}CablewayReport.aspx?tp=status", Config.SafetyURL);
            hlCablewayReports.NavigateUrl = String.Format("{0}SIMSReports/Cableways/", Config.SIMSServerURL);
            hlSHA.NavigateUrl = String.Format("{0}SHAReport.aspx", Config.SafetyURL);
            hlWYSummary.NavigateUrl = String.Format("{0}SIMSReports/WYS/wys_details.html", Config.SIMSServerURL);

            hlPASSHome.NavigateUrl = Config.PASSURL;
            hlSLAPHome.NavigateUrl = Config.SLAPURL;
        }

        /// <summary>
        /// Clear any record period locks that the user might have in place
        /// </summary>
        protected void ClearLocks()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("{0}Handler/ClearLock.ashx?user_id={1}", Config.RMSURL, user.ID));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        }

        protected void rddlWSC_SelectedIndexChanged(object sender, EventArgs e)
        {
            Response.Redirect("SIMSWSCHome.aspx?wsc_id=" + rddlWSC.SelectedValue.ToString());
        }

        protected void btnGo_OnCommand(object sender, CommandEventArgs e)
        {
            string URL = String.Format("StationInfo.aspx?site_id={0}", db.Sites.FirstOrDefault(p => p.site_no == tbSiteNo.Text && p.agency_cd == tbAgencyCd.Text).site_id);
            Response.Redirect(URL);
        }
    }
}