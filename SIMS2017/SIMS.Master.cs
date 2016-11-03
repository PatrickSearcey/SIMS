using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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
        }

        //For setting up hyperlinks in the side menu
        protected void SetupPageHyperlinks()
        {
            hlDCPIDInfo.NavigateUrl = String.Format("{0}NessInfo3.asp?office_id={1}&wsc_id={2}", Config.SIMSClassicURL, OfficeID, WSCID);
            hlPASS.NavigateUrl = Config.PASSURL;
            hlStationLevels.NavigateUrl = String.Format("{0}StationLevels.aspx?wsc_id={1}", Config.SLAPURL, WSCID);
            hlOfficeReport.NavigateUrl = String.Format("{0}StationElemRpt.asp?office_id={1}&wsc_id={2}", Config.SIMSClassicURL, OfficeID, WSCID);
            hlCableway.NavigateUrl = String.Format("{0}CablewayReport.aspx?tp=status&wsc_id={1}", Config.SIMSURL, WSCID);
            hlEditCableway.NavigateUrl = String.Format("{0}Cableways.aspx?wsc_id={1}", Config.SIMSURL, WSCID);
            hlSHAReport.NavigateUrl = String.Format("{0}SHAReport.aspx?office_id={1}&wsc_id={2}", Config.SIMSURL, OfficeID, WSCID);
            hlTCPReport.NavigateUrl = String.Format("{0}admin/TCPadmin.asp?view=show&office_id={1}&wsc_id={2}", Config.SIMSClassicURL, OfficeID, WSCID);
            hlEmergencyInfo.NavigateUrl = String.Format("{0}EmergencyInfo.aspx?office_id={1}", Config.SIMSURL, OfficeID);
            hlMAI.NavigateUrl = String.Format("{0}ElemReport.aspx?wsc_id={1}", Config.SIMSURL, WSCID);
            hlWYSummaryReport.NavigateUrl = String.Format("{0}WY/wys_details.html", Config.SIMSServerURL);
            hlPubsList.NavigateUrl = String.Format("{0}StationsList.asp?office_id={1}&wy=2013", Config.SIMSClassicURL, OfficeID);
            hlADRStatus.NavigateUrl = String.Format("{0}StationsCount.asp?wsc_id={1}", Config.SIMSClassicURL, WSCID);
            hlMapFieldTrips.NavigateUrl = String.Format("{0}fieldtripmap.aspx?office_id={1}&trip_id=0&wsc_id={2}", Config.SIMSURL, OfficeID, WSCID);
            hlKMLWSC.NavigateUrl = String.Format("{0}KMLHandler.ashx?wsc_id={1}", Config.SIMSURL, WSCID);
            hlKMLOffice.NavigateUrl = String.Format("{0}KMLHandler.ashx?office_id={1}", Config.SIMSURL, OfficeID);
            hlKMLInstructions.NavigateUrl = String.Format("{0}Docs/KMLDownloadInstructions.pdf", Config.SIMSURL);
            hlEvalMaps.NavigateUrl = String.Format("{0}EvalMaps.aspx?wsc_id={1}", Config.SIMSURL, WSCID);
        }

        //Handles Info By Site link redirects
        protected void SiteInfo_Command(object sender, CommandEventArgs e)
        {
            switch (e.CommandArgument.ToString())
            {
                case "Spring":
                    Response.Redirect(Config.SIMSClassicURL + "StationElemRpt.asp?waterbody_type=5&wsc_id=" + WSCID.ToString());
                    break;
                case "Estuary":
                    Response.Redirect(Config.SIMSClassicURL + "StationElemRpt.asp?waterbody_type=3&wsc_id=" + WSCID.ToString());
                    break;
                case "Lake":
                    Response.Redirect(Config.SIMSClassicURL + "StationElemRpt.asp?waterbody_type=2&wsc_id=" + WSCID.ToString());
                    break;
                case "Stream":
                    Response.Redirect(Config.SIMSClassicURL + "StationElemRpt.asp?waterbody_type=1&wsc_id=" + WSCID.ToString());
                    break;
                case "Groundwater":
                    Response.Redirect(Config.SIMSClassicURL + "StationElemRpt.asp?waterbody_type=6&wsc_id=" + WSCID.ToString());
                    break;
            }
        }
    }
}