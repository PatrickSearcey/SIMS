using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Safety
{
    public partial class TCPView : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private Data.TCP currTCP;
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
            //If no TCPID was passed, then redirect back to the homepage
            string tcp_id = Request.QueryString["TCPID"];
            if (!string.IsNullOrEmpty(tcp_id)) TCPID = Convert.ToInt32(tcp_id); else Response.Redirect(Config.SIMS2017URL + "SIMSWSCHome.aspx");

            //Using the passed TCPID, setup the TCP data element, and reset the office and wsc to match that of the current site
            currTCP = db.TCPs.Where(p => p.TCPID == TCPID).FirstOrDefault();
            SiteID = (int)currTCP.site_id;
            OfficeID = (int)currTCP.TCPSite.Site.office_id;
            WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            //--BASIC PAGE SETUP--------------------------------------------------------------------
            ph1.Title = currTCP.TCPPlanDetail.Number + " - TCP, " + currTCP.TCPPlanDetail.SubName;
            ph1.SubTitle = currTCP.TCPSite.Site.site_no + " " + currTCP.TCPSite.Site.station_full_nm;

            PopulateReport();
        }

        /// <summary>
        /// Fill the page with data
        /// </summary>
        protected void PopulateReport()
        {
            var site = db.Sites.FirstOrDefault(p => p.site_id == currTCP.site_id);

            if (currTCP != null)
            {
                if (currTCP.TCPPlanDetail.Number == "0")
                {
                    pnlPlanVI.Visible = false;
                    pnlSiteInfo.Visible = false;
                    pnlLessData.Visible = false;
                    pnlIVbData.Visible = false;
                    pnlInstructions.Visible = false;
                    pnlImage.Visible = false;
                    pnlAllData.Visible = false;

                    ltlNotes0.Text = currTCP.TCPSite.Notes.ToStringSafe();
                    ltlUpdated0.Text = string.Format("{0:MM/dd/yyyy}", currTCP.UpdatedDt) + ", by " + currTCP.UpdatedBy.ToStringSafe();

                }
                else if (currTCP.TCPPlanDetail.Number == "VI")
                {
                    pnlPlan0.Visible = false;
                    pnlSiteInfo.Visible = false;
                    pnlLessData.Visible = false;
                    pnlIVbData.Visible = false;
                    pnlInstructions.Visible = false;
                    pnlImage.Visible = false;
                    pnlAllData.Visible = false;

                    ltlHighwayVI.Text = currTCP.TCPSite.RoadName.ToStringSafe();
                    ltlSpeedVI.Text = currTCP.TCPSite.SpeedLimit.ToString("unknown") + " mph";
                    ltlTrafficVI.Text = currTCP.TCPSite.TrafficVolume.ToString("unknown");
                    ltlWorkAreaActivityVI.Text = currTCP.WorkAreaActivity;
                    ltlRemarksVI.Text = currTCP.Remarks.ToStringSafe();
                    ltlUpdatedVI.Text = string.Format("{0:MM/dd/yyyy}", currTCP.UpdatedDt) + ", by " + currTCP.UpdatedBy.ToStringSafe();
                    ltlReviewedVI.Text = string.Format("{0:MM/dd/yyyy}", currTCP.UpdatedDt) + ", by " + currTCP.ReviewedBy.ToStringSafe();
                    ltlApprovedVI.Text = string.Format("{0:MM/dd/yyyy}", currTCP.UpdatedDt) + ", by " + currTCP.ApprovedBy.ToStringSafe();
                }
                else
                {
                    pnlPlan0.Visible = false;
                    pnlPlanVI.Visible = false;

                    ltlHighway.Text = currTCP.TCPSite.RoadName.ToStringSafe();
                    if (Convert.ToBoolean(currTCP.TCPSite.Expressway))
                        ltlFreeway.Text = "Yes";
                    else
                        ltlFreeway.Text = "No";
                    ltlWidth.Text = currTCP.TCPSite.BridgeWidth.ToString("unknown") + " feet";
                    ltlWorkZone.Text = currTCP.TCPSite.WorkZone.ToString("unknown") + " feet";
                    ltlLane.Text = currTCP.TCPSite.LaneWidth.ToString("unknown") + " feet";
                    if (currTCP.TCPSite.ShoulderWidth < 1)
                        ltlShoulder.Text = "< 1 foot";
                    else if (currTCP.TCPSite.ShoulderWidth <= 5)
                        ltlShoulder.Text = "<= 5 feet";
                    else
                        ltlShoulder.Text = "> 5 feet";
                    ltlSpeed.Text = currTCP.TCPSite.SpeedLimit.ToString("unknown") + " mph";
                    ltlTraffic.Text = currTCP.TCPSite.TrafficVolume.ToString("unknown");
                    if (Convert.ToBoolean(site.SHAs.FirstOrDefault().cell_service))
                        ltlCell.Text = "Yes";
                    else
                        ltlCell.Text = "No";
                    ltlUpdated.Text = string.Format("{0:MM/dd/yyyy}", currTCP.UpdatedDt) + ", by " + currTCP.UpdatedBy.ToStringSafe();
                    ltlReviewed.Text = string.Format("{0:MM/dd/yyyy}", currTCP.UpdatedDt) + ", by " + currTCP.ReviewedBy.ToStringSafe();
                    ltlApproved.Text = string.Format("{0:MM/dd/yyyy}", currTCP.UpdatedDt) + ", by " + currTCP.ApprovedBy.ToStringSafe();
                    ltlNotes.Text = currTCP.TCPSite.Notes.ToStringSafe();
                    ltlRemarks.Text = currTCP.Remarks.ToStringSafe();
                    ltlInstructions.Text = currTCP.TCPPlanDetail.Notes.ToStringSafe();

                    imgPlanImage.ImageUrl = String.Format("{0}images/TCPPlan{1}.png", Config.SafetyURL, currTCP.TCPPlanDetail.Number);
                    CalculatorData();
                }
            }
        }

        /// <summary>
        /// Determine which TCP Calculator Data panel to show based on plan number
        /// </summary>
        protected void CalculatorData()
        {
            switch (currTCP.TCPPlanDetail.Number)
            {
                case "Ia":
                case "Ib":
                case "II":
                case "III":
                    pnlAllData.Visible = true;
                    pnlLessData.Visible = false;
                    pnlIVbData.Visible = false;
                    CalculateData();
                    break;
                case "IVa":
                case "V":
                    pnlAllData.Visible = false;
                    pnlLessData.Visible = true;
                    pnlIVbData.Visible = false;
                    CalculateData();
                    break;
                case "IVb":
                    pnlAllData.Visible = false;
                    pnlLessData.Visible = false;
                    pnlIVbData.Visible = true;
                    CalculateData();
                    break;
                case "0":
                    break;
                case "VI":
                    break;
            }
        }

        protected void CalculateData()
        {
            int? WarningSignSpacing = null, MinTaperLength = null, OptBufferLength = null, FlaggerDistance = null;
            double? WZCones = null, TTCones = null, STMin = null, STCones = null, BZCones = null;
            int? WZConeSpacing = null, TTConeSpacing = null, BZConeSpacing = null;

            var calcData = db.TCPCalculations.FirstOrDefault(p => p.Speed == currTCP.TCPSite.SpeedLimit);

            if (calcData != null)
            {
                WarningSignSpacing = calcData.WarningSignSpacing;
                MinTaperLength = calcData.MinTaperLength;
                OptBufferLength = calcData.OptBufferLength;
                FlaggerDistance = calcData.FlaggerDistance;

                WZCones = Math.Ceiling(Convert.ToDouble(currTCP.TCPSite.WorkZone) / (Convert.ToDouble(currTCP.TCPSite.SpeedLimit) * 2));
                WZConeSpacing = currTCP.TCPSite.SpeedLimit * 2;
                TTCones = Math.Ceiling(Convert.ToDouble(MinTaperLength) / Convert.ToDouble(currTCP.TCPSite.SpeedLimit));
                TTConeSpacing = currTCP.TCPSite.SpeedLimit;
                STMin = Math.Ceiling(0.33 * Convert.ToDouble(MinTaperLength));
                STCones = Math.Ceiling(Convert.ToDouble(STMin) / Convert.ToDouble(currTCP.TCPSite.SpeedLimit));
                BZCones = Math.Ceiling(Convert.ToDouble(calcData.OptBufferLength) / Convert.ToDouble(currTCP.TCPSite.SpeedLimit) * 2);
                BZConeSpacing = currTCP.TCPSite.SpeedLimit * 2;
            }

            switch (currTCP.TCPPlanDetail.Number)
            {
                case "Ia":
                case "Ib":
                case "II":
                case "III":
                    ltlWS.Text = WarningSignSpacing.ToString("unknown");
                    ltlFlagger.Text = FlaggerDistance.ToString("unknown");
                    ltlWZLength.Text = currTCP.TCPSite.WorkZone.ToString("unknown");
                    ltlWZCones.Text = WZCones.ToString("unknown");
                    ltlWZConeSpacing.Text = WZConeSpacing.ToString("unknown");
                    ltlTTLength.Text = MinTaperLength.ToString("unknown");
                    ltlTTCones.Text = TTCones.ToString("unknown");
                    ltlTTConeSpacing.Text = TTConeSpacing.ToString("unknown");
                    ltlSTMin.Text = STMin.ToString("unknown");
                    ltlSTCones.Text = STCones.ToString("unknown");
                    ltlBZLength.Text = OptBufferLength.ToString("unknown");
                    ltlConesReq.Text = String.Format("{0}", WZCones + TTCones + STCones);
                    ltlBZCones.Text = BZCones.ToString("unknown");
                    ltlBZConeSpacing.Text = BZConeSpacing.ToString("unknown");
                    break;
                case "IVa":
                case "V":
                    ltlWS2.Text = WarningSignSpacing.ToString("unknown");
                    ltlWZLength2.Text = currTCP.TCPSite.WorkZone.ToString("unknown");
                    break;
                case "IVb":
                    ltlWS1.Text = WarningSignSpacing.ToString("unknown");
                    ltlWZLength1.Text = currTCP.TCPSite.WorkZone.ToString("unknown");
                    ltlWZCones1.Text = WZCones.ToString("unknown");
                    ltlWZConeSpacing1.Text = WZConeSpacing.ToString("unknown");
                    ltlSTMin1.Text = STMin.ToString("unknown");
                    ltlSTCones1.Text = STCones.ToString("unknown");
                    ltlBZLength1.Text = OptBufferLength.ToString("unknown");
                    ltlConesReq1.Text = String.Format("{0}", WZCones + STCones);
                    ltlBZCones1.Text = BZCones.ToString("unknown");
                    ltlBZConeSpacing1.Text = BZConeSpacing.ToString("unknown");
                    break;
            }
        }
    }
}
