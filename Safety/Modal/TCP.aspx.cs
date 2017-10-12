using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Safety
{
    public partial class TCP : System.Web.UI.Page
    {
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private Data.TCP tcp;

        protected void Page_Load(object sender, EventArgs e)
        {
            string type = Request.QueryString["type"];
            int TCPID = Convert.ToInt32(Request.QueryString["TCPID"]);

            tcp = db.TCPs.FirstOrDefault(p => p.TCPID == TCPID);

            ltlSiteNoName.Text = tcp.TCPSite.Site.site_no.Trim() + " " + db.vSITEFILEs.FirstOrDefault(s => s.site_no == tcp.TCPSite.Site.site_no && s.agency_cd == tcp.TCPSite.Site.agency_cd).station_nm;

            SetupTCPPanel();

            if (type == "approve")
            {
                pnlApprove.Visible = true;
                pnlReview.Visible = false;
                SetupApprovePanel();
            }
            else
            {
                pnlApprove.Visible = false;
                pnlReview.Visible = true;
                ltlApproved2.Text = String.Format("{0:MM/dd/yyyy} ({1})", tcp.ApprovedDt, tcp.ApprovedBy);
            }
        }

        protected void SetupTCPPanel()
        {
            ltlPlanTitle.Text = tcp.TCPPlanDetail.Number + " - " + tcp.TCPPlanDetail.Name;
            ltlPlanSubTitle.Text = tcp.TCPPlanDetail.SubName;
            ltlUpdated.Text = String.Format("{0:MM/dd/yyyy} ({1})", tcp.UpdatedDt, tcp.UpdatedBy);
            ltlReviewed.Text = String.Format("{0:MM/dd/yyyy} ({1})", tcp.ReviewedDt, tcp.ReviewedBy);
            ltlApproved1.Text = String.Format("{0:MM/dd/yyyy} ({1})", tcp.ApprovedDt, tcp.ApprovedBy);
            ltlRemoteSite.Text = tcp.TCPSite.RemoteSite.ProcessBoolean();
            ltlRoadName.Text = tcp.TCPSite.RoadName;

            if (tcp.TCPPlanDetail.Number == "0")
                pnlPlanInfo.Visible = false;
            else
            {
                pnlPlanInfo.Visible = true;
                ltlExpressway.Text = tcp.TCPSite.Expressway.ProcessBoolean();
                ltlBridgeWidth.Text = tcp.TCPSite.BridgeWidth.ToString() + " feet";
                ltlWorkZone.Text = tcp.TCPSite.WorkZone.ToString() + " feet";
                ltlLaneWidth.Text = tcp.TCPSite.LaneWidth.ToString() + " feet";
                ltlShoulderWidth.Text = tcp.TCPSite.ShoulderWidth.ToString() + " feet";
                ltlSpeedLimit.Text = tcp.TCPSite.SpeedLimit.ToString() + " mph";
                ltlLaneNumber.Text = tcp.TCPSite.LaneNumber.ToString();
                ltlFlow2Way.Text = tcp.TCPSite.Flow2Way.ProcessBoolean();
                ltlDividedHighway.Text = tcp.TCPSite.DividedHighway.ProcessBoolean();
                ltlMedian.Text = tcp.TCPSite.Median.ProcessBoolean();
                ltlFlaggers.Text = tcp.TCPSite.Flaggers.ProcessBoolean();
            }
            
            ltlTrafficVolume.Text = tcp.TCPSite.TrafficVolume;
            ltlSiteSpecificNotes.Text = tcp.TCPSite.Notes;
            if (!string.IsNullOrEmpty(tcp.WorkAreaActivity)) ltlPlanSpecificNotes.Text = tcp.WorkAreaActivity + "<br />" + tcp.Remarks;
            else ltlPlanSpecificNotes.Text = tcp.Remarks;
        }

        protected void SetupApprovePanel()
        {
            if (Convert.ToBoolean(tcp.NoChanges))
                ltlNoChanges.Text = "<p style='font-weight:bold;'>No changes were made to the TCP since it was last approved.</p>";
            else
                ltlNoChanges.Text = "<p style='font-weight:bold;color: red;'>Changes were made to the TCP since it was last approved.</p>";

            ltlReviewerComments.Text = tcp.ReviewerComments;
            ltlApprovedDt.Text = String.Format("<b>{0:MM/dd/yyyy}</b>", DateTime.Now);
            ltlApprovedBy.Text = "<b>" + user.ID + "</b>";

            int WSCID = Convert.ToInt32(tcp.TCPSite.Site.Office.wsc_id);
            if (user.IsSuperUser || user.WSCID.Contains(WSCID) && user.IsSafetyApprover)
            {
                rbSubmit2.Visible = true;
                ltlNotApprover.Visible = false;
            }
            else
            {
                rbSubmit2.Visible = false;
                ltlNotApprover.Visible = true;
            }
        }

        protected void ReviewClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbReviewerComments.Text))
            {
                ltlError.Text = "<p style='font-weight:bold;color:red;'>You must enter a comment for the approver!</p>";
            }
            else
            {
                tcp.ApprovalReady = true;
                tcp.NoChanges = cbNoChanges.Checked;
                tcp.ReviewerComments = tbReviewerComments.Text;
                tcp.ReviewedBy = user.ID;
                tcp.ReviewedDt = DateTime.Now;
                db.SubmitChanges();

                ScriptManager.RegisterStartupScript(this, GetType(), "close", "CloseModal();", true);
            }
        }

        protected void ApproveClicked(object sender, EventArgs e)
        {
            tcp.ApprovedBy = user.ID;
            tcp.ApprovedDt = DateTime.Now;
            tcp.ApprovalReady = false;
            db.SubmitChanges();

            ScriptManager.RegisterStartupScript(this, GetType(), "close", "CloseModal();", true);
        }

        protected void Cancel(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "close", "CloseModal();", true);
        }
    }
}