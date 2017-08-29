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
    public partial class SIMSSingleMenu : System.Web.UI.MasterPage
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
            ltlUserID.Text = user.ID;

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
            rmTop.Items.Add(new RadMenuItem { Text = "Contact", NavigateUrl = String.Format("{0}NWISOpsRequest.aspx?office_id={1}", Config.SIMSURL, OfficeID) });

            ClearLocks();
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