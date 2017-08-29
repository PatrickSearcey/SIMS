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
    public partial class DCPIDReport : System.Web.UI.Page
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
        private string SiteNo
        {
            get
            {
                if (Session["SiteNo"] == null) return "0"; else return (string)Session["SiteNo"];
            }
            set
            {
                Session["SiteNo"] = value;
            }
        }
        private Boolean ShowAssignedIDs;
        #endregion 

        protected void Page_Load(object sender, EventArgs e)
        {
            string office_id = Request.QueryString["office_id"];

            if (!string.IsNullOrEmpty(office_id)) OfficeID = Convert.ToInt32(office_id);
            else if (OfficeID == 0) OfficeID = user.OfficeID;
            WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            phDCPReport.Title = "DCPID Information";
            phDCPReport.SubTitle = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).wsc_nm + " WSC";
            phDCPReport.ShowOfficeInfoPanel = false;

            osDCPReport.SelectorChanged += new EventHandler(osDCPReport_SelectorChanged);
            osDCPReport.HideSiteFields = true;

            if (!Page.IsPostBack)
            {
                ltlOfficeName.Text = "DCPID Information for " + db.Offices.Where(p => p.office_id == OfficeID).Select(p => p.office_nm).First();

                pnlFieldTrip.Visible = false;

                ShowAssignedIDs = true;
                lbNotAssignedToggle.Text = "Click to view DCPIDs not assigned";
                lbNotAssignedToggle.CommandArgument = "unassigned";
            }
        }

        #region Misc Events
        private void osDCPReport_SelectorChanged(object sender, EventArgs e)
        {
            //If the office was the only thing changed, reset to the office site list
            if (TripID == 0)
            {
                ltlOfficeName.Text = "DCPID Information for " + db.Offices.Where(p => p.office_id == OfficeID).Select(p => p.office_nm).First();
                pnlFieldTrip.Visible = false;
            }
            else //Switch to the field trip site list
            {
                ltlOfficeName.Text = "DCPID Information by Field Trip";
                pnlFieldTrip.Visible = true;
                SetupFieldTripPanel();
            }

            //Reset the show assigned ID toggle to show assigned DCPIDs
            ShowAssignedIDs = true;
            lbNotAssignedToggle.Text = "Click to view DCPIDs not assigned";
            lbNotAssignedToggle.CommandArgument = "unassigned";

            rgDCPIDs.Rebind();
        }

        protected void ChangeView(object sender, CommandEventArgs e)
        {
            if (e.CommandName.ToString() == "NotAssigned")
            {
                if (e.CommandArgument.ToString() == "unassigned")
                {
                    ShowAssignedIDs = false;
                    lbNotAssignedToggle.Text = "Click to view assigned DCPIDs";
                    lbNotAssignedToggle.CommandArgument = "assigned";
                    ltlOfficeName.Text = "Viewing Unassigned DCPIDs";
                }
                else
                {
                    ShowAssignedIDs = true;
                    lbNotAssignedToggle.Text = "Click to view DCPIDs not assigned";
                    lbNotAssignedToggle.CommandArgument = "unassigned";
                    ltlOfficeName.Text = "Viewing Assigned DCPIDs by Office";
                }
            }
            rgDCPIDs.Rebind();
        }
        #endregion

        #region Methods
        protected void SetupFieldTripPanel()
        {
            var trip = db.Trips.Where(p => p.trip_id == TripID);

            ltlFieldTrip.Text = "Field Trip: " + trip.FirstOrDefault().trip_nm + " - " + db.Employees.Where(p => p.user_id == trip.FirstOrDefault().user_id).FirstOrDefault().first_nm + " " + db.Employees.Where(p => p.user_id == trip.FirstOrDefault().user_id).FirstOrDefault().last_nm;
        }
        #endregion

        #region Internal Classes
        internal class DCPIDDataItem
        {
            private string _site_id;
            private string _site_no;
            private string _station_nm;
            private string _DCPID;
            private string _channel;
            private string _baud;
            private string _elevation;
            private string _transmission;

            public string site_id
            {
                get { return _site_id; }
                set { _site_id = value; }
            }
            public string site_no
            {
                get { return _site_no; }
                set { _site_no = value; }
            }
            public string station_nm
            {
                get { return _station_nm; }
                set { _station_nm = value; }
            }
            public string DCPID
            {
                get { return _DCPID; }
                set { _DCPID = value; }
            }
            public string channel
            {
                get { return _channel; }
                set { _channel = value; }
            }
            public string baud
            {
                get { return _baud; }
                set { _baud = value; }
            }
            public string elevation
            {
                get { return _elevation; }
                set { _elevation = value; }
            }
            public string transmission
            {
                get { return _transmission; }
                set { _transmission = value; }
            }
            public DCPIDDataItem()
            {
                _site_id = site_id;
                _site_no = site_no;
                _station_nm = station_nm;
                _DCPID = DCPID;
                _channel = channel;
                _baud = baud;
                _elevation = elevation;
                _transmission = transmission;
            }
        }
        #endregion

        #region Data
        private IEnumerable<DCPIDDataItem> GetData()
        {
            string status = "assigned";
            int wsc_id = WSCID;
            int office_id = OfficeID;
            int trip_id = TripID;

            if (!ShowAssignedIDs)
            {
                status = "unassigned";
                office_id = 0;
                trip_id = 0;
            }

            var ret = db.SP_DCPID_Report(wsc_id, office_id, trip_id, SiteNo, status).Select(p => new DCPIDDataItem()
            {
                site_id = p.site_id.ToString(),
                site_no = p.site_no,
                station_nm = p.station_nm,
                DCPID = p.dcp_id,
                channel = p.primary_ch.ToString() + "/" + p.random_ch.ToString(),
                baud = p.primary_bd.ToString() + "/" + p.random_bd.ToString(),
                elevation = FormatElevation(p.ant_azimuth.ToString(), p.satellite.ToString(), p.ant_elev.ToString()),
                transmission = p.assigned_time + "/" + p.trans_interval + "/" + p.window
            }).ToList();

            return ret;
        }

        private string FormatElevation(string ant_azimuth, string satellite, string ant_elev)
        {
            string ret = "";

            if (string.IsNullOrEmpty(ant_azimuth))
                ret = satellite + " ---/---";
            else
                ret = satellite + " " + ant_azimuth + "/" + ant_elev;

            return ret;
        }
        #endregion

        #region DCPIDs Grid
        protected void rgDCPIDs_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            rgDCPIDs.DataSource = GetData();
        }

        protected void rgDCPIDs_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgDCPIDs.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" | menu.Items[i].Text == "Contains" | menu.Items[i].Text == "EqualTo" | menu.Items[i].Text == "DoesNotContain")
                {
                    i = i + 1;
                }
                else
                {
                    menu.Items.RemoveAt(i);
                }
            }
        }
        #endregion
    }
}