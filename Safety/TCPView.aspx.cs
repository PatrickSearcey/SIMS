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
            string tcp_id = "2";// Request.QueryString["TCPID"];
            if (!string.IsNullOrEmpty(tcp_id)) TCPID = Convert.ToInt32(tcp_id); else Response.Redirect(Config.SIMS2017URL + "SIMSWSCHome.aspx");

            //Using the passed TCPID, setup the TCP data element, and reset the office and wsc to match that of the current site
            currTCP = db.TCPs.Where(p => p.TCPID == TCPID).FirstOrDefault();
            SiteID = (int)currTCP.site_id;
            OfficeID = (int)currTCP.TCPSite.Site.office_id;
            WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            //--BASIC PAGE SETUP--------------------------------------------------------------------
            ph1.Title = currTCP.TCPPlanDetail.Number + " - TCP, " + currTCP.TCPPlanDetail.SubName;
            ph1.SubTitle = currTCP.TCPSite.Site.site_no + " " + currTCP.TCPSite.Site.station_full_nm;

            //Fill the page with data
            PopulateReport();
        }

        protected void PopulateReport()
        {
            var site = db.Sites.FirstOrDefault(p => p.site_id == currTCP.site_id);
            if (currTCP != null)
            {

                ltlTCPtitle.Text = currTCP.TCPPlanDetail.Number + " - " + currTCP.TCPPlanDetail.Name;
                ltlName.Text = site.station_full_nm;
                ltlNumber.Text = site.site_no;
                ltlHighway.Text = currTCP.TCPSite.RoadName.ToStringSafe();
                if (Convert.ToBoolean(currTCP.TCPSite.Expressway))
                {
                    ltlFreeway.Text = "Yes";
                }
                else
                {
                    ltlFreeway.Text = "No";
                }
                ltlWidth.Text = currTCP.TCPSite.BridgeWidth.ToString("N/A");
                ltlWorkZone.Text = currTCP.TCPSite.WorkZone.ToString("N/A");
                ltlLane.Text = currTCP.TCPSite.LaneWidth.ToString("N/A");
                if (currTCP.TCPSite.ShoulderWidth < 1)
                {
                    ltlShoulder.Text = "<1";
                }
                else if (currTCP.TCPSite.ShoulderWidth <= 5)
                {
                    ltlShoulder.Text = "<=5";
                }
                else
                {
                    ltlShoulder.Text = ">5";
                }
                ltlSpeed.Text = currTCP.TCPSite.SpeedLimit.ToString("N/A");
                ltlTraffic.Text = currTCP.TCPSite.TrafficVolume.ToString("N/A");
                ltlCell.Text = site.SHAs.FirstOrDefault().cell_service.ToString();
                ltlUpdated.Text = string.Format("{0:MM/dd/yyyy}", currTCP.UpdatedDt) + ", by " + currTCP.UpdatedBy.ToStringSafe();
                ltlReviewed.Text = string.Format("{0:MM/dd/yyyy}", currTCP.UpdatedDt) + ", by " + currTCP.ReviewedBy.ToStringSafe();
                ltlApproved.Text = string.Format("{0:MM/dd/yyyy}", currTCP.UpdatedDt) + ", by " + currTCP.ApprovedBy.ToStringSafe();
                ltlNotes.Text = currTCP.TCPSite.Notes.ToStringSafe();
                ltlRemarks.Text = currTCP.TCPPlanDetail.Notes.ToStringSafe();

            }
        }
    }
}
