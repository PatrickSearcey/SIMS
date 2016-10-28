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
            if (WSCID == 0) WSCID = user.WSCID;
            if (OfficeID == 0) OfficeID = user.OfficeID;
        }

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