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
    public partial class SIMS : System.Web.UI.MasterPage
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
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            ltlUserID.Text = user.ID;
            if (OfficeID == 0) OfficeID = Convert.ToInt32(Request.QueryString["office_id"]);
            if (OfficeID == 0) OfficeID = user.OfficeID;
            if (WSCID == 0) WSCID = Convert.ToInt32(Request.QueryString["wsc_id"]);
            if (WSCID == 0) WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            SetupPageHyperlinks();
            ClearLocks();
        }

        //For setting up hyperlinks in the side and top menus
        protected void SetupPageHyperlinks()
        {
            rmTop.Items.Add(new RadMenuItem { Text = "SIMS National Home", NavigateUrl = String.Format("{0}SIMSHome.aspx", Config.SIMS2017URL) });
            rmTop.Items.Add(new RadMenuItem { IsSeparator = true });
            rmTop.Items.Add(new RadMenuItem { Text = "SIMS WSC Home", NavigateUrl = String.Format("{0}SIMSWSCHome.aspx?wsc_id={1}", Config.SIMS2017URL, WSCID) });
            rmTop.Items.Add(new RadMenuItem { IsSeparator = true });
            rmTop.Items.Add(new RadMenuItem { Text = "RMS WSC Home", NavigateUrl = String.Format("{0}RMSWSCHome.aspx?wsc_id={1}", Config.RMSURL, WSCID) });
            rmTop.Items.Add(new RadMenuItem { IsSeparator = true });
            rmTop.Items.Add(new RadMenuItem { Text = "Admin Tasks", NavigateUrl = String.Format("{0}Admin/Tasks.aspx?wsc_id={1}", Config.SIMS2017URL, WSCID) });
            rmTop.Items.Add(new RadMenuItem { IsSeparator = true });
            rmTop.Items.Add(new RadMenuItem { Text = "Latest News", NavigateUrl = "https://collaboration.usgs.gov/wg/owi/specialprojects/SIMS/Shared%20Documents/updates.html" });
            rmTop.Items.Add(new RadMenuItem { IsSeparator = true });
            rmTop.Items.Add(new RadMenuItem { Text = "Contact", NavigateUrl = String.Format("{0}NWISOpsRequest.aspx", Config.SIMS2017URL) });

            rmSide.FindItemByText("RMS").NavigateUrl = String.Format("{0}RMSWSCHome.aspx?wsc_id={1}", Config.RMSURL, WSCID);

            hlDCPIDInfo.NavigateUrl = String.Format("{0}DCPIDReport.aspx?office_id={1}", Config.SIMS2017URL, OfficeID);
            hlPASS.NavigateUrl = Config.PASSURL;
            hlStationLevels.NavigateUrl = String.Format("{0}StationLevels.aspx?wsc_id={1}", Config.SLAPURL, WSCID);
            hlOfficeReport.NavigateUrl = String.Format("{0}StationDoc/CustomReport.aspx?type={1}&office_id={2}", Config.SIMS2017URL, "office", OfficeID);
            hlCableway.NavigateUrl = String.Format("{0}CablewayReport.aspx?tp=status&office_id={1}", Config.SafetyURL, OfficeID);
            hlEditCableway.NavigateUrl = String.Format("{0}Cableways.aspx?office_id={1}", Config.SafetyURL, OfficeID);
            hlSHAReport.NavigateUrl = String.Format("{0}SHAReport.aspx?office_id={1}", Config.SafetyURL, OfficeID, WSCID);
            hlTCPReport.NavigateUrl = String.Format("{0}TCPReport.aspx?office_id={1}", Config.SafetyURL, OfficeID, WSCID);
            hlEmergencyInfo.NavigateUrl = String.Format("{0}EmergencyInfo.aspx?office_id={1}", Config.SafetyURL, OfficeID);
            hlMAI.NavigateUrl = String.Format("{0}StationDoc/MAI.aspx?office_id={1}", Config.SIMS2017URL, OfficeID);
            hlWYSummaryReport.NavigateUrl = String.Format("{0}SIMSReports/WY/wys_details.html", Config.SIMSServerURL);
            hlMapFieldTrips.NavigateUrl = String.Format("{0}fieldtripmap.aspx?office_id={1}&trip_id=0&wsc_id={2}", Config.SIMSURL, OfficeID, WSCID);
            hlKMLWSC.NavigateUrl = String.Format("{0}KMLHandler.ashx?wsc_id={1}", Config.SIMSURL, WSCID);
            hlKMLOffice.NavigateUrl = String.Format("{0}KMLHandler.ashx?office_id={1}", Config.SIMSURL, OfficeID);
            hlKMLInstructions.NavigateUrl = String.Format("{0}Docs/KMLDownloadInstructions.pdf", Config.SIMSURL);
            hlEvalMaps.NavigateUrl = String.Format("{0}EvalMaps.aspx?wsc_id={1}", Config.SIMSURL, WSCID);
        }

        /// <summary>
        /// Clear any record period locks that the user might have in place
        /// </summary>
        protected void ClearLocks()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("{0}Handler/ClearLock.ashx?user_id={1}", Config.RMSURL, user.ID));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        }

        //Handles Info By Site link redirects
        protected void SiteInfo_Command(object sender, CommandEventArgs e)
        {
            int waterbodytype = 1;
            switch (e.CommandArgument.ToString())
            {
                case "Spring":
                    waterbodytype = 5;
                    break;
                case "Estuary":
                    waterbodytype = 3;
                    break;
                case "Lake":
                    waterbodytype = 2;
                    break;
                case "Stream":
                    waterbodytype = 1;
                    break;
                case "Groundwater":
                    waterbodytype = 6;
                    break;
            }

            Response.Redirect(String.Format("{0}StationDoc/CustomReport.aspx?type={1}&office_id={2}", Config.SIMS2017URL, waterbodytype, OfficeID));
        }
    }
}