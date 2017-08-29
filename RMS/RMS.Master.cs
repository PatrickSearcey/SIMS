using Core;
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
            rmTop.Items.Add(new RadMenuItem { Text = "SIMS WSC Home", NavigateUrl = String.Format("{0}SIMSWSCHome.aspx?wsc_id={1}", Config.SIMSURL, WSCID) });
            rmTop.Items.Add(new RadMenuItem { IsSeparator = true });
            rmTop.Items.Add(new RadMenuItem { Text = "RMS WSC Home", NavigateUrl = String.Format("{0}RMSWSCHome.aspx?wsc_id={1}", Config.RMSURL, WSCID) });
            rmTop.Items.Add(new RadMenuItem { IsSeparator = true });
            rmTop.Items.Add(new RadMenuItem { Text = "Admin Tasks", NavigateUrl = String.Format("{0}Admin/Tasks.aspx?wsc_id={1}", Config.SIMSURL, WSCID) });
            rmTop.Items.Add(new RadMenuItem { IsSeparator = true });
            rmTop.Items.Add(new RadMenuItem { Text = "Latest News", NavigateUrl = "https://collaboration.usgs.gov/wg/owi/specialprojects/SIMS/Shared%20Documents/updates.html" });
            rmTop.Items.Add(new RadMenuItem { IsSeparator = true });
            rmTop.Items.Add(new RadMenuItem { Text = "Contact", NavigateUrl = String.Format("{0}NWISOpsRequest.aspx", Config.SIMSURL) });


            hlAnalyzeRecordsList.NavigateUrl = String.Format("{0}Report/RecordProcess.aspx?task=analyze&office_id={1}", Config.RMSURL, OfficeID);
            hlApproveRecordsList.NavigateUrl = String.Format("{0}Report/RecordProcess.aspx?task=approve&office_id={1}", Config.RMSURL, OfficeID);
            hlCRPStatus.NavigateUrl = String.Format("{0}Report/CRPStatus.aspx?office_id={1}", Config.RMSURL, OfficeID);
            hlCRPStatusChart.NavigateUrl = String.Format("{0}Report/CRPStatusChart.aspx?office_id={1}", Config.RMSURL, OfficeID);
            hlRecordProgressWSC.NavigateUrl = String.Format("{0}Report/RecordProgressWSC.aspx?wsc_id={1}", Config.RMSURL, WSCID);
            hlRecordProgress.NavigateUrl = String.Format("{0}Report/RecordProgress.aspx?wsc_id={1}", Config.RMSURL, WSCID);
            hlPubStatus.NavigateUrl = String.Format("{0}Report/PubStatus.aspx?office_id={1}", Config.RMSURL, OfficeID);
            hlAudit.NavigateUrl = String.Format("{0}Report/Audit.aspx?office_id={1}", Config.RMSURL, OfficeID);

            rmSide.FindItemByText("Period Details").NavigateUrl = String.Format("{0}Report/PeriodDetails.aspx?wsc_id={1}&office_id={2}", Config.RMSURL, WSCID, OfficeID);
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