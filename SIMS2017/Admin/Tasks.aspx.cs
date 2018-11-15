using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIMS2017.Admin
{
    public partial class Tasks : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        public Boolean HasEditAccess { get; set; }
        private Data.WSC currWSC;
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
            string office_id = Request.QueryString["office_id"];
            if (!string.IsNullOrEmpty(office_id)) OfficeID = Convert.ToInt32(office_id); else if (OfficeID == 0) Response.Redirect(Config.SIMSURL + "SIMSWSCHome.aspx");

            WSCID = Convert.ToInt32(db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id);
            currWSC = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID);

            ph1.Title = "Administration Tasks";
            ph1.SubTitle = currWSC.wsc_nm + " Water Science Center";
            ph1.ShowOfficeInfoPanel = true;

            if (!Page.IsPostBack)
            {
                //If the user belongs to this site's WSC (or has an exception to work in the WSC), or is a SuperUser, then allow them to edit the page
                if (user.WSCID.Contains(WSCID) && user.IsAdmin || user.IsSuperUser) HasEditAccess = true;

                InitialPageSetup();
            }
        }

        protected void InitialPageSetup()
        {
            if (!HasEditAccess)
            {
                pnlHasAccess.Visible = false;
                pnlNoAccess.Visible = true;
            }
            else
            {
                pnlHasAccess.Visible = true;
                pnlNoAccess.Visible = false;

                hlRegisterSite.NavigateUrl = String.Format("{0}Admin/RegisterSite.aspx?office_id={1}", Config.SIMSURL, OfficeID);
                hlManageOffices.NavigateUrl = String.Format("{0}Admin/Offices.aspx?office_id={1}", Config.SIMSURL, OfficeID);
                hlManagePersonnel.NavigateUrl = String.Format("{0}Admin/Personnel.aspx?office_id={1}", Config.SIMSURL, OfficeID);
                hlManageFieldTrips.NavigateUrl = String.Format("{0}Admin/FieldTrips.aspx?office_id={1}", Config.SIMSURL, OfficeID);
                hlEndangeredGages.NavigateUrl = String.Format("{0}Admin/EndangeredGages.aspx?office_id={1}", Config.SIMSURL, OfficeID);
                hlManageRecords.NavigateUrl = String.Format("{0}Admin/Records.aspx?office_id={1}", Config.RMSURL, OfficeID);
                hlManageRecordTypes.NavigateUrl = String.Format("{0}Admin/RecordTypes.aspx?office_id={1}", Config.RMSURL, OfficeID);
                hlPeriodDates.NavigateUrl = String.Format("{0}Admin/PeriodDate.aspx?office_id={1}", Config.RMSURL, OfficeID);
                hlPeriodStatus.NavigateUrl = String.Format("{0}Admin/PeriodStatus.aspx?office_id={1}", Config.RMSURL, OfficeID);
                hlSANAL.NavigateUrl = String.Format("{0}Admin/StationAnalyses.aspx?office_id={1}", Config.RMSURL, OfficeID);
                hlUnlock.NavigateUrl = String.Format("{0}Admin/Unlock.aspx?office_id={1}", Config.RMSURL, OfficeID);
            }
        }
    }
}