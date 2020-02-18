using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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

        #region Page Load Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            string office_id = Request.QueryString["office_id"];
            string site_id = Request.QueryString["site_id"];

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
            var userAD = db.spz_GetUserInfoFromAD(user.ID).ToList();
            foreach (var item in userAD)
            {
                lblName.Text = item.Name;
                lblEmail.Text = item.mail;
                lblPhone.Text = item.TelephoneNumber;
                hfEmail.Value = item.mail;
            }

            if (SiteID > 0)
            {
                var site = db.Sites.FirstOrDefault(p => p.site_id == SiteID);
                lblSiteNo.Text = site.site_no + " " + db.vSITEFILEs.FirstOrDefault(p => p.site_id == site.nwisweb_site_id).station_nm;
                tbSiteNo.Visible = false;
                lblOptional.Visible = false;
                hfSiteID.Value = SiteID.ToString();
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
        #endregion

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

        /// <summary>
        /// Gets the request summary for the review part of the process
        /// </summary>
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
                    site_no = site.site_no + " " + db.vSITEFILEs.FirstOrDefault(p => p.site_id == site.nwisweb_site_id).station_nm;
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

            string request = request_type + " - " + request_subtype;
            hfRequestType.Value = request;

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
                "      <td colspan='2'><b>Request Subject</b> " + request + "</td>" +
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

        #region Button Events
        protected void rbNext_Command(object sender, CommandEventArgs e)
        {
            if (rrblRequestType.SelectedValue == "" || tbRequest.Text == "")
            {
                ltlError.Text = "<span style='color:red;'>You must select the type of request and enter text into the 'Enter your request' text box!</span>";
                return;
            }

            if (!string.IsNullOrEmpty(tbSiteNo.Text))
            {
                var site = db.Sites.FirstOrDefault(p => p.site_no == tbSiteNo.Text);
                if (site == null)
                {
                    ltlError.Text = "<span style='color:red;'>The site number you entered is not valid!</span>";
                    return;
                }
                else
                {
                    hfSiteID.Value = site.site_id.ToString();
                }
            }

            pnlSetup.Visible = false;
            pnlStepReview.Visible = true;

            if (!string.IsNullOrEmpty(hfSiteID.Value)) ltlEmails.Text = fcnMailToNWISOps("email", Convert.ToInt32(hfSiteID.Value));
            else ltlEmails.Text = fcnMailToNWISOps("email", 0);

            ltlCompleteRequest.Text = GetRequestText("review");
        }

        protected void rbSend_Command(object sender, CommandEventArgs e)
        {
            pnlStepReview.Visible = false;
            pnlConfirmSend.Visible = true;
        
	        MailMessage message = new MailMessage();
	        string requestID = fcnMailToNWISOps("reqID", 0);
	        string site_id = hfSiteID.Value.ToString(), site_information = "";
	        List<string> assigned_operators = new List<string>();
	        List<string> trip_userids = new List<string>();
	        var office = db.Offices.FirstOrDefault(p => p.office_id == OfficeID);

	        if (!string.IsNullOrEmpty(site_id)) {
		        site_information = GetSiteMailInfo(Convert.ToInt32(site_id));
                var site = db.Sites.FirstOrDefault(p => p.site_id.ToString() == site_id);
		        site_id = " for " + site.site_no;

		        var records = site.Records;
                foreach (var record in records)
                {
                    assigned_operators.Add(record.operator_uid);
                }
                var trips = site.TripSites;
                foreach (var trip in trips)
                {
                    trip_userids.Add(trip.Trip.user_id);
                }

		        foreach (string ao in assigned_operators) {
			        if (!trip_userids.Contains(ao) & !string.IsNullOrEmpty(ao) & ao != null) {
                        string aoEmail = GetUserEmailAliasFromAD(ao);
                        if (!string.IsNullOrEmpty(aoEmail)) message.CC.Add(aoEmail);
			        }
		        }

		        foreach (string tu in trip_userids) {
			        if (!string.IsNullOrEmpty(tu) & tu != null) {
                        string tuEmail = GetUserEmailAliasFromAD(tu);
                        if (!string.IsNullOrEmpty(tuEmail)) message.CC.Add(tuEmail);
			        }
		        }
	        } 
            else site_id = "";

	        string subject = "NWIS Ops Request " + requestID + site_id + ": " + hfRequestType.Value.ToString();
	        string CCTo = tbCCEmail.Text, x;
	        if (!string.IsNullOrEmpty(CCTo)) {
                List<string> a = new List<string>(CCTo.Split(','));
		        foreach (var x_loopVariable in a) {
			        x = x_loopVariable;
			        if (x.IndexOf('@', 0) > 0) {
				        message.CC.Add(x);
			        } else {
				        message.CC.Add(GetUserEmailAliasFromAD(x));
			        }
		        }
	        }
	        string ToAddress1 = fcnMailToNWISOps("toemail1", 0);
	        if (!string.IsNullOrEmpty(ToAddress1)) {
                List<string> a1 = new List<string>(ToAddress1.Split(','));
		        foreach (var x_loopVariable in a1) {
			        x = x_loopVariable;
			        message.To.Add(x);
		        }
	        }
	        string ToAddress2 = fcnMailToNWISOps("toemail2", 0);
	        if (!string.IsNullOrEmpty(ToAddress2)) {
                List<string> a2 = new List<string>(ToAddress2.Split(','));
		        foreach (var x_loopVariable in a2) {
			        x = x_loopVariable;
			        string y = x;
                    if (y.IndexOf('<', 0) > 0)
                    {
                        y = y.Substring(y.IndexOf('<', 0) + 1);
                        y = y.Replace(">", "").Trim();
                    }
			        message.To.Add(y);
		        }
	        }

	        message.CC.Add(hfEmail.Value.ToString());
	        message.From = new MailAddress(hfEmail.Value.ToString());
	        message.Subject = subject;
            message.Body = "E-Mail: " + hfEmail.Value.ToString() + "\r\n\r\n" +
                "Request CC'ed to: " + tbCCEmail.Text + "\r\n\r\n" +
                "Office: " + office.office_nm + "\r\n\r\n" +
                "Request Type: " + hfRequestType.Value.ToString() + "\r\n\r\n" +
                site_information + "Request:" + "\r\n" + 
                tbRequest.Text;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "gscamnlh01.wr.usgs.gov";
#if !DEBUG
	        smtp.Send(message);
            IncreaseRequestID(requestID);
#endif

            pnlConfirmSend.Visible = true;
            pnlStepReview.Visible = false;
        }

        protected void lbReset_Command(object sender, CommandEventArgs e)
        {
            pnlSetup.Visible = true;
            pnlStepReview.Visible = false;
            SetupPageControls();
        }
        #endregion

        /// <summary>
        /// Returns the full email of the user from AD using their passed user ID
        /// </summary>
        private string GetUserEmailAliasFromAD(string userid)
        {
            string email = "";
            var userAD = db.spz_GetUserInfoFromAD(userid).ToList();

            foreach (var u in userAD)
            {
                email = u.mail;
            }

            return email;
        }

        /// <summary>
        /// Increases the reqID field in the lut_WSC table by one
        /// </summary>
        public void IncreaseRequestID(string requestid)
        {
            int req_id = Convert.ToInt32(requestid) + 1;
            var wsc = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID);
            wsc.reqID = req_id;
            db.SubmitChanges();
        }

        public string fcnMailToNWISOps(string options, int site_id)
        {
	        string functionReturnValue = null, x, reqID = null, ToAddress1 = null, ToAddress2 = null, pOut = "<ul>";

            var office = db.Offices.FirstOrDefault(p => p.office_id == OfficeID);
            if (options == "email")
            {
                string groupemail = office.WSC.nwisops_email;
                string dcemail = office.data_chief_email;
                if (!string.IsNullOrEmpty(groupemail))
                {
                    List<string> a = new List<string>(groupemail.Split(','));
                    foreach (var x_loopvariable in a)
                    {
                        x = x_loopvariable;
                        pOut += "<li>" + x + "</li>" + "\r\n";
                    }
                }
                if (!string.IsNullOrEmpty(dcemail))
                {
                    List<string> a1 = new List<string>(dcemail.Split(','));
                    foreach (var x_loopVariable in a1)
                    {
                        x = x_loopVariable;
                        string y = x.Replace("<", "&lt;");
                        y = y.Replace(">", "&gt;");
                        pOut += "<li>" + y + "</li>" + "\r\n";
                    }
                }

                if (site_id > 0) 
                {
					var site = db.Sites.FirstOrDefault(p => p.site_id == site_id);
					List<string> assigned_operators = new List<string>();
	                List<string> trip_userids = new List<string>();

                    var records = site.Records;
                    foreach (var record in records)
                    {
                        assigned_operators.Add(record.operator_uid);
                    }
                    var trips = site.TripSites;
                    foreach (var trip in trips)
                    {
                        trip_userids.Add(trip.Trip.user_id);
                    }

		            foreach (string ao in assigned_operators) {
			            if (!trip_userids.Contains(ao) & !string.IsNullOrEmpty(ao) & ao != null) {
                            string aoEmail = GetUserEmailAliasFromAD(ao);
                            if (!string.IsNullOrEmpty(aoEmail)) pOut += "<li>" + aoEmail + "</li>" + "\r\n";
			            }
		            }

		            foreach (string tu in trip_userids) {
			            if (!string.IsNullOrEmpty(tu) & tu != null) {
                            string tuEmail = GetUserEmailAliasFromAD(tu);
                            if (!string.IsNullOrEmpty(tuEmail)) pOut += "<li>" + GetUserEmailAliasFromAD(tu) + "</li>" + "\r\n";
			            }
		            }
				}
            }
            else if (options == "toemail1") ToAddress1 = office.WSC.nwisops_email;
            else if (options == "toemail2") ToAddress2 = office.data_chief_email;
            else reqID = office.WSC.reqID.ToString();

	        if (options == "email") {
		        functionReturnValue = pOut;
	        } else if (options == "toemail1") {
		        functionReturnValue = ToAddress1;
	        } else if (options == "toemail2") {
		        functionReturnValue = ToAddress2;
	        } else {
		        functionReturnValue = reqID;
	        }

	        return functionReturnValue;
        }

        public string GetSiteMailInfo(int site_id)
        {
            string dcp_id = null;
            var site = db.Sites.FirstOrDefault(p => p.site_id == site_id);
            var dcpInfo = db.spz_GetDCPInfo(site_id);

            foreach (var di in dcpInfo)
            {
                dcp_id = di.dcp_id;
            }

            string site_no_nm = site.site_no + " " + db.vSITEFILEs.FirstOrDefault(p => p.site_id == site.nwisweb_site_id).station_nm;
            string pOut = "Site ID: " + site_no_nm + "\r\n\r\n" +
                "DCPID: " + dcp_id + "\r\n\r\n" +
                "SIMS Station Info Page: " + Config.SIMSURL + "StationInfo.aspx?office_id=" + site.office_id.ToString() + "&site_id=" + site.site_id.ToString() + "\r\n\r\n" +
                "NWIS Web: http://waterdata.usgs.gov/nwis/nwisman/?site_no=" + site.site_no + "\r\n\r\n";

            return pOut;
        }
    }
}