using Core;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace RMS
{
    public partial class MasterPage : System.Web.UI.MasterPage
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
            rmTop.Items.Add(new RadMenuItem { Text = "SIMS National Home", NavigateUrl = String.Format("{0}SIMSHome.aspx", Config.SIMSURL) });
            rmTop.Items.Add(new RadMenuItem { IsSeparator = true });
            rmTop.Items.Add(new RadMenuItem { Text = "SIMS WSC Home", NavigateUrl = String.Format("{0}SIMSWSCHome.aspx?wsc_id={1}&office_id={2}", Config.SIMSURL, WSCID, OfficeID) });
            rmTop.Items.Add(new RadMenuItem { IsSeparator = true });
            rmTop.Items.Add(new RadMenuItem { Text = "RMS WSC Home", NavigateUrl = String.Format("{0}RMSWSCHome.aspx?wsc_id={1}&office_id={2}", Config.RMSURL, WSCID, OfficeID) });
            rmTop.Items.Add(new RadMenuItem { IsSeparator = true });
            rmTop.Items.Add(new RadMenuItem { Text = "Admin Tasks", NavigateUrl = String.Format("{0}Admin/Tasks.aspx?wsc_id={1}&office_id={2}", Config.SIMSURL, WSCID, OfficeID) });
            rmTop.Items.Add(new RadMenuItem { IsSeparator = true });
            rmTop.Items.Add(new RadMenuItem { Text = "Latest News", NavigateUrl = "https://collaboration.usgs.gov/wg/owi/specialprojects/SIMS/Shared%20Documents/updates.html" });
            rmTop.Items.Add(new RadMenuItem { IsSeparator = true });
            rmTop.Items.Add(new RadMenuItem { Text = "Contact", NavigateUrl = String.Format("{0}NWISOpsRequest.aspx?office_id={1}", Config.SIMSURL, OfficeID) });


            hlAnalyzeRecordsList.NavigateUrl = String.Format("{0}Report/RecordProcess.aspx?task=analyze", Config.RMSURL);
            hlApproveRecordsList.NavigateUrl = String.Format("{0}Report/RecordProcess.aspx?task=approve", Config.RMSURL);
            hlCRPStatus.NavigateUrl = String.Format("{0}Report/CRPStatus.aspx", Config.RMSURL);
            hlCRPStatusChart.NavigateUrl = String.Format("{0}Report/CRPStatusChart.aspx", Config.RMSURL);
            hlRecordProgressWSC.NavigateUrl = String.Format("{0}Report/RecordProgressWSC.aspx", Config.RMSURL);
            hlRecordProgress.NavigateUrl = String.Format("{0}Report/RecordProgress.aspx", Config.RMSURL);
            hlPubStatus.NavigateUrl = String.Format("{0}Report/PubStatus.aspx", Config.RMSURL);
            hlAudit.NavigateUrl = String.Format("{0}Report/Audit.aspx", Config.RMSURL);
            hlAudit.NavigateUrl = String.Format("{0}Report/AuditChart.aspx", Config.RMSURL);
            hlRecentActions.NavigateUrl = String.Format("{0}Report/RecentActions.aspx", Config.RMSURL);

            rmSide.FindItemByText("Period Details").NavigateUrl = String.Format("{0}Report/PeriodDetails.aspx?type=wsc", Config.RMSURL);
        }

        /// <summary>
        /// Clear any record period locks that the user might have in place
        /// </summary>
        protected void ClearLocks()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("{0}Handler/ClearLock.ashx?user_id={1}", Config.RMSURL, user.ID));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        }
    }
}