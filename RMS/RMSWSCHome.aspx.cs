using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RMS
{
    public partial class RMSWSCHome : System.Web.UI.Page
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
        private int TripID
        {
            get
            {
                if (Session["TripID"] == null) return 0; else return (int)Session["TripID"];
            }
            set
            {
                Session["TripID"] = value;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (OfficeID == 0) OfficeID = user.OfficeID;
            if (WSCID == 0) WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;
            osHome.SelectorChanged += new EventHandler(osHome_SelectorChanged);

            if (!Page.IsPostBack)
            {
                var wsc = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID);
                ltlWSCName.Text = wsc.wsc_nm + " Water Science Center";
            }
        }

        private void osHome_SelectorChanged(object sender, EventArgs e)
        {
            //If the office was the only thing changed, reset to the office site list
            if (TripID == 0)
            {
                //ltlOfficeName.Text = "USGS Master Station List for " + db.Offices.Where(p => p.office_id == OfficeID).Select(p => p.office_nm).First();
                //pnlFieldTrip.Visible = false;
            }
            else //Switch to the field trip site list
            {
                //ltlOfficeName.Text = "USGS Field Trip Station List";
                //pnlFieldTrip.Visible = true;
                //SetupFieldTripPanel();
            }

            //rgSites.Rebind();
        }
    }
}