using Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SIMS2017
{
    public partial class SIMSWSCHome : System.Web.UI.Page
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
        private Boolean ShowActiveSites
        {
            get
            {
                if (Session["ShowActiveSites"] == null) return true; else return (Boolean)Session["ShowActiveSites"];
            }
            set
            {
                Session["ShowActiveSites"] = value;
            }
        }
        #endregion 

        #region Page Events
        protected void Page_Load(object sender, EventArgs e)
        {
            string wsc_id = Request.QueryString["wsc_id"];
            string office_id = Request.QueryString["office_id"];

            if (!string.IsNullOrEmpty(office_id))
            {
                OfficeID = Convert.ToInt32(office_id);
                if (WSCID == 0 || db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id != WSCID) 
                    WSCID = Convert.ToInt32(db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id);
            }
            else if (!string.IsNullOrEmpty(wsc_id))
            {
                WSCID = Convert.ToInt32(wsc_id);
                if (OfficeID == 0 || db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id != WSCID)
                    OfficeID = db.Offices.FirstOrDefault(p => p.wsc_id == WSCID).office_id;
            }
            else
            {
                if (OfficeID == 0 && WSCID == 0)
                {
                    OfficeID = user.OfficeID;
                    WSCID = (int)db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id;
                }
                else if (OfficeID == 0 && WSCID > 0)
                    OfficeID = db.Offices.FirstOrDefault(p => p.wsc_id == WSCID).office_id;
                else if (OfficeID > 0 && WSCID == 0)
                    WSCID = (int)db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id;
            }

            osHome.SelectorChanged += new EventHandler(osHome_SelectorChanged);

            if (!Page.IsPostBack)
            {
                //Reset the field trip ID to 0 when first coming to the page
                TripID = 0;

                var wsc = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID);
                ltlWSCName.Text = wsc.wsc_nm + " Water Science Center";

                lbFAQ.NavigateUrl = String.Format("{0}SiteListsFAQ.html", Config.SIMSURL);

                ltlOfficeName.Text = "USGS Master Station List for " + db.Offices.Where(p => p.office_id == OfficeID).Select(p => p.office_nm).First();

                pnlFieldTrip.Visible = false;

                ShowActiveSites = true;
                lbActiveToggle.Text = "Click to view inactive sites";
                lbActiveToggle.CommandArgument = "inactive";
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ltlSiteCount.Text = rgSites.Items.Count.ToString() + " sites returned";
        }
        #endregion

        #region Methods
        protected void SetupFieldTripPanel()
        {
            var trip = db.Trips.Where(p => p.trip_id == TripID);

            ltlFieldTrip.Text = "Field Trip: " + trip.FirstOrDefault().trip_nm + " - " + db.Employees.Where(p => p.user_id == trip.FirstOrDefault().user_id).FirstOrDefault().first_nm + " " + db.Employees.Where(p => p.user_id == trip.FirstOrDefault().user_id).FirstOrDefault().last_nm;
            //hlMapTrip.NavigateUrl = String.Format("{0}fieldtripmap.aspx?office_id={1}&trip_id={2}&wsc_id={3}", Config.SIMSURL, OfficeID, TripID, WSCID);
            hlMapTrip.Visible = false;

            string site_no_list = "";
            var trip_sites = db.TripSites.Where(p => p.trip_id == TripID).Select(p => p.Site.site_no).ToList();
            foreach (string site in trip_sites)
            {
                site_no_list += site.Trim() + ",";
            }
            hlRealtimeGraphs.NavigateUrl = "http://waterdata.usgs.gov/nwis/uv?multiple_site_no=" + site_no_list + "&sort_key=site_no&group_key=NONE&sitefile_output_format=html_table&column_name=agency_cd&column_name=site_no&column_name=station_nm&range_selection=days&period=7&format=gif_default&date_format=YYYY-MM-DD&rdb_compression=file&list_of_search_criteria=multiple_site_no%2Crealtime_parameter_selection";
            lbActiveToggle.Visible = false;
        }
        #endregion

        #region Misc Events
        private void osHome_SelectorChanged(object sender, EventArgs e)
        {
            //If the office was the only thing changed, reset to the office site list
            if (TripID == 0)
            {
                lbActiveToggle.Visible = true;
                ltlOfficeName.Text = "USGS Master Station List for " + db.Offices.Where(p => p.office_id == OfficeID).Select(p => p.office_nm).First();
                pnlFieldTrip.Visible = false;
            }
            else //Switch to the field trip site list
            {
                ltlOfficeName.Text = "USGS Field Trip Station List";
                pnlFieldTrip.Visible = true;
                SetupFieldTripPanel();
            }

            //Reset the active site toggle to show active sites
            ShowActiveSites = true;
            lbActiveToggle.Text = "Click to view inactive sites";
            lbActiveToggle.CommandArgument = "inactive";

            rgSites.Rebind();
        }
        
        //Event that fires when a user has selected to see inactive or active sites
        protected void lbActiveToggle_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument.ToString() == "inactive")
            {
                ShowActiveSites = false;
                lbActiveToggle.Text = "Click to view active sites";
                lbActiveToggle.CommandArgument = "active";
            }
            else
            {
                ShowActiveSites = true;
                lbActiveToggle.Text = "Click to view inactive sites";
                lbActiveToggle.CommandArgument = "inactive";
            }
            rgSites.Rebind();
        }
        #endregion

        #region Internal Classes
        internal class SiteDataItem
        {
            private string _site_id;
            private string _site_no;
            private string _station_nm;
            private string _SIMSURL;
            private string _office_id;
            private string _wsc_id;
            private string _agency_cd;
            private string _SiteType;
            private string _Active;
            private string _tel_flag;
            private List<int?> _trip_ids;

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
            public string SIMSURL
            {
                get { return _SIMSURL; }
                set { _SIMSURL = value; }
            }
            public string office_id
            {
                get { return _office_id; }
                set { _office_id = value; }
            }
            public string wsc_id
            {
                get { return _wsc_id; }
                set { _wsc_id = value; }
            }
            public string agency_cd
            {
                get { return _agency_cd; }
                set { _agency_cd = value; }
            }
            public string SiteType
            {
                get { return _SiteType; }
                set { _SiteType = value; }
            }
            public string Active
            {
                get { return _Active; }
                set { _Active = value; }
            }
            public string TelFlag
            {
                get { return _tel_flag; }
                set { _tel_flag = value; }
            }
            public List<int?> trip_ids
            {
                get { return _trip_ids; }
                set { _trip_ids = value; }
            }
            public SiteDataItem()
            {
                _site_id = site_id;
                _site_no = site_no;
                _station_nm = station_nm;
                _SIMSURL = SIMSURL;
                _office_id = office_id;
                _wsc_id = wsc_id;
                _agency_cd = agency_cd;
                _SiteType = SiteType;
                _Active = Active;
                _tel_flag = TelFlag;
                _trip_ids = trip_ids;
            }
        }
        #endregion

        #region Data
        private IEnumerable<SiteDataItem> GetData()
        {
            var ret = db.vSiteMasterLists.Where(p => p.office_id == OfficeID).Select(p => new SiteDataItem()
            {
                site_id = p.site_id.ToString(),
                site_no = p.site_no,
                station_nm = p.station_nm,
                SIMSURL = Config.SIMSURL,
                office_id = p.office_id.ToString(),
                wsc_id = p.wsc_id.ToString(),
                agency_cd = p.agency_cd,
                SiteType = p.sims_site_tp,
                Active = GetActiveData(p.agency_use_cd.ToString()),
                TelFlag = p.tel_fg
            }).OrderBy(p => p.site_no).ToList();

            return ret;
        }

        private IEnumerable<SiteDataItem> GetTripData()
        {
            var ret = db.vSiteMasterLists.Where(p => p.wsc_id == WSCID).Select(p => new SiteDataItem()
            {
                site_id = p.site_id.ToString(),
                site_no = p.site_no,
                station_nm = p.station_nm,
                SIMSURL = Config.SIMSURL,
                office_id = p.office_id.ToString(),
                wsc_id = p.wsc_id.ToString(),
                agency_cd = p.agency_cd,
                SiteType = p.sims_site_tp,
                TelFlag = p.tel_fg,
                trip_ids = GetTripIDs(p.site_id)
            }).OrderBy(p => p.site_no).ToList();

            return ret;
        }

        private List<int?> GetTripIDs(int site_id)
        {
            List<int?> ret = new List<int?>();

            var trips = db.TripSites.Where(p => p.site_id == site_id).ToList();
            foreach (var trip in trips)
                ret.Add(trip.trip_id);

            return ret;
        }

        private string GetActiveData(string agency_use_cd)
        {
            string ret;
            switch (agency_use_cd)
            {
                case "L":
                case "A":
                case "M":
                    ret = "Active";
                    break;
                case "I":
                case "R":
                case "D":
                case "O":
                    ret = "Inactive";
                    break;
                default:
                    ret = "unknown";
                    break;
            }
            return ret;
        }
        #endregion

        #region Sites Grid
        protected void rgSites_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (TripID == 0)
            {
                if (ShowActiveSites) rgSites.DataSource = GetData().Where(p => p.Active == "Active"); else rgSites.DataSource = GetData().Where(p => p.Active == "Inactive");
            }
            else
            {
                rgSites.DataSource = GetTripData().Where(p => p.trip_ids.Contains(TripID));
            }
        }

        protected void rgSites_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem dataItem = e.Item as GridDataItem;
                if (dataItem["TelFlag"].Text != "NNN")
                    dataItem.BackColor = Color.AliceBlue;
            }
        }

        protected void rgSites_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgSites.FilterMenu;
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