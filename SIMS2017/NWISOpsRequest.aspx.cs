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
    public partial class NWISOpsRequest : System.Web.UI.Page
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
            string office_id = Request.QueryString["office_id"];
            string site_id = "3000337";// Request.QueryString["site_id"];

            if (!string.IsNullOrEmpty(office_id)) OfficeID = Convert.ToInt32(office_id);
            else if (OfficeID == 0) OfficeID = user.OfficeID;
            WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            if (!string.IsNullOrEmpty(site_id)) SiteID = Convert.ToInt32(site_id);
            else SiteID = 0;
            //If a site ID was passed, then reset the OfficeID to the assigned site's office
            if (SiteID > 0)
            {
                OfficeID = Convert.ToInt32(db.Sites.FirstOrDefault(p => p.site_id == SiteID).office_id);
                WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;
            }

            ph1.Title = "NWIS Ops Help Request";
            ph1.SubTitle = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).wsc_nm + " WSC";
            ph1.ShowOfficeInfoPanel = false;

            if (!Page.IsPostBack)
            {
                SetupPageControls();
            }
        }

        protected void SetupPageControls()
        {
            var userAD = db.spz_GetUserInfoFromAD(user.ID);
            foreach (var item in userAD)
            {
                lblName.Text = item.Name;
                lblEmail.Text = item.mail;
                lblPhone.Text = item.TelephoneNumber;
            }

            if (SiteID > 0)
            {
                var site = db.Sites.FirstOrDefault(p => p.site_id == SiteID);
                lblSiteNo.Text = site.site_no + " " + site.station_full_nm;
                tbSiteNo.Visible = false;
                lblOptional.Visible = false;
            }
            else
            {
                lblSiteNo.Visible = false;
            }

            lblReqTypesHeading.Visible = false;
            rrblRTReqTypes.Visible = false;
            rrblNWISReqTypes.Visible = false;
            rrblMANUReqTypes.Visible = false;
            rrblOtherReqTypes.Visible = false;
        }

        protected void formatSubRequestType(object sender, EventArgs e)
        {
            lblReqTypesHeading.Visible = true;
            rrblRTReqTypes.Visible = false;
            rrblNWISReqTypes.Visible = false;
            rrblMANUReqTypes.Visible = false;
            rrblOtherReqTypes.Visible = false;

            switch (rrblRequestType.SelectedValue)
            {
                case "Realtime":
                    rrblRTReqTypes.Visible = true;
                    break;
                case "GeneralNWIS":
                    rrblNWISReqTypes.Visible = true;
                    break;
                case "MANU":
                    rrblMANUReqTypes.Visible = true;
                    break;
                case "Other":
                    rrblOtherReqTypes.Visible = true;
                    break;
            }
        }

        protected void rbNext_Command(object sender, CommandEventArgs e)
        {
            if (rrblRequestType.SelectedValue == "" || tbRequest.Text == "")
            {
                ltlError.Text = "<span style='color:red;'>You must select the type of request and enter text into the 'Enter your request' text box!</span>";
                return;
            }

            pnlSetup.Visible = false;
            pnlStepReview.Visible = true;

            ltlCompleteRequest.Text = GetRequestText("review");
        }

        protected void rbSend_Command(object sender, CommandEventArgs e)
        {
            pnlStepReview.Visible = false;
            pnlConfirmSend.Visible = true;
        }

        protected void lbReset_Command(object sender, CommandEventArgs e)
        {
            pnlSetup.Visible = true;
            pnlStepReview.Visible = false;
            SetupPageControls();
        }

        private string GetRequestText(string forWhat)
        {
            string pOut = "";
            string site_no = "<i>not referenced</i>";
            string request_type = rrblRequestType.SelectedValue;
            string request_subtype = "";

            if (!string.IsNullOrEmpty(lblSiteNo.Text))
            {
                site_no = lblSiteNo.Text;
            }
            else if (!string.IsNullOrEmpty(tbSiteNo.Text))
            {
                var site = db.Sites.FirstOrDefault(p => p.site_no == tbSiteNo.Text);
                if (site != null)
                {
                    site_no = site.site_no + " " + site.station_full_nm;
                }
            }

            switch (request_type)
            {
                case "Realtime":
                    request_type = "Real-time";
                    request_subtype = rrblRTReqTypes.SelectedValue;
                    break;
                case "GeneralNWIS":
                    request_type = "General NWIS";
                    request_subtype = rrblNWISReqTypes.SelectedValue;
                    break;
                case "MANU":
                    request_type = "Manuscript Request";
                    request_subtype = rrblMANUReqTypes.SelectedValue;
                    break;
                case "Other":
                    request_type = "Other Support";
                    request_subtype = rrblOtherReqTypes.SelectedValue;
                    break;
            }

            if (forWhat == "review")
            {
                pOut = "<div class='EmailReview'>" +
                "  <table cellpadding='10'>" +
                "    <tr>" +
                "      <td><b>Name:</b> " + lblName.Text + "</td>" +
                "      <td><b>Email:</b> " + lblEmail.Text + "</td>" +
                "      <td><b>Phone:</b> " + lblPhone.Text + "</td>" +
                "    </tr>" +
                "    <tr>" +
                "      <td><b>Site Number:</b> " + site_no + "</td>" +
                "      <td colspan='2'><b>Request Subject</b> " + request_type + " - " + request_subtype + "</td>" +
                "    </tr>" +
                "    <tr>" +
                "      <td colspan='3'><b>Request:</b><br />" +
                "      " + tbRequest.Text +
                "      </td>" +
                "    </tr>" +
                "  </table>" +
                "</div>";
            }

            return pOut;
        }
    }
}