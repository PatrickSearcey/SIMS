using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

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

        #region Page Events
        protected void Page_Load(object sender, EventArgs e)
        {
            string wsc_id = Request.QueryString["wsc_id"];

            if (!string.IsNullOrEmpty(wsc_id))
            {
                WSCID = Convert.ToInt32(wsc_id);
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
                var wsc = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID);
                ltlWSCName.Text = wsc.wsc_nm + " Water Science Center";

                ltlOfficeName.Text = "Status of Records for " + db.Offices.Where(p => p.office_id == OfficeID).Select(p => p.office_nm).First();

                pnlFieldTrip.Visible = false;
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ltlRecordCount.Text = rgRecords.Items.Count.ToString() + " records returned";
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
        }
        #endregion

        #region Misc Events
        private void osHome_SelectorChanged(object sender, EventArgs e)
        {
            //If the office was the only thing changed, reset to the office site list
            if (TripID == 0 && SiteID == 0)
            {
                ltlOfficeName.Text = "Status of Records for " + db.Offices.Where(p => p.office_id == OfficeID).Select(p => p.office_nm).First();
                pnlFieldTrip.Visible = false;
            }
            else if (SiteID > 0) //Else if a site number was entered, show for a site
            {
                var site = db.Sites.FirstOrDefault(p => p.site_id == SiteID);
                ltlOfficeName.Text = "Status of Records for " + site.site_no + " " + site.station_full_nm;
                pnlFieldTrip.Visible = false;
            }
            else //Switch to the field trip site list
            {
                ltlOfficeName.Text = "Status of Records by Field Trip";
                pnlFieldTrip.Visible = true;
                SetupFieldTripPanel();
            }

            rgRecords.Rebind();
        }
        #endregion

        #region Data
        private IEnumerable<RecordDataItem> GetData()
        {
            var ret = db.vRMSStatusOfRecords.Where(p => p.record_office_id == OfficeID).Select(p => new RecordDataItem()
            {
                site_id = p.site_id.ToString(),
                site_no = p.site_no,
                station_nm = p.station_full_nm,
                SIMSURL = Config.SIMSURL,
                office_id = p.record_office_id.ToString(),
                wsc_id = p.wsc_id.ToString(),
                agency_cd = p.agency_cd,
                RecordType = p.type_cd,
                Active = (!p.not_used_fg).ToString(),
                Analyzer = p.analyzer_uid,
                AnalyzedDt = String.Format("{0:MM/dd/yyyy}", p.analyzed_period_dt),
                AnalyzedBy = p.analyzed_period_by,
                Approver = p.approver_uid,
                ApprovedDt = String.Format("{0:MM/dd/yyyy}", p.approved_period_dt),
                ApprovedBy = p.approved_period_by,
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
        #endregion

        #region Records Grid
        protected void rgRecords_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (TripID == 0 && SiteID == 0)
            {
                rgRecords.DataSource = GetData().Where(p => p.Active == "True");
            }
            else if (SiteID > 0)
            {
                rgRecords.DataSource = GetData().Where(p => p.site_id == SiteID.ToString());
            }
            else
            {
                rgRecords.DataSource = GetData().Where(p => p.trip_ids.Contains(TripID) && p.Active == "True");
            }
        }

        protected void rgRecords_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem dataItem = e.Item as GridDataItem;
            }
        }

        protected void rgRecords_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgRecords.FilterMenu;
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

        #region Internal Classes
        internal class RecordDataItem
        {
            private string _site_id;
            private string _site_no;
            private string _station_nm;
            private string _SIMSURL;
            private string _office_id;
            private string _wsc_id;
            private string _agency_cd;
            private string _Analyzer;
            private string _AnalyzedDt;
            private string _AnalyzedBy;
            private string _Approver;
            private string _ApprovedDt;
            private string _ApprovedBy;
            private string _Active;
            private string _RecordType;
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
            public string RecordType
            {
                get { return _RecordType; }
                set { _RecordType = value; }
            }
            public string Active
            {
                get { return _Active; }
                set { _Active = value; }
            }
            public string Analyzer
            {
                get { return _Analyzer; }
                set { _Analyzer = value; }
            }
            public string AnalyzedDt
            {
                get { return _AnalyzedDt; }
                set { _AnalyzedDt = value; }
            }
            public string AnalyzedBy
            {
                get { return _AnalyzedBy; }
                set { _AnalyzedBy = value; }
            }
            public string Approver
            {
                get { return _Approver; }
                set { _Approver = value; }
            }
            public string ApprovedDt
            {
                get { return _ApprovedDt; }
                set { _ApprovedDt = value; }
            }
            public string ApprovedBy
            {
                get { return _ApprovedBy; }
                set { _ApprovedBy = value; }
            }
            public List<int?> trip_ids
            {
                get { return _trip_ids; }
                set { _trip_ids = value; }
            }
            public RecordDataItem()
            {
                _site_id = site_id;
                _site_no = site_no;
                _station_nm = station_nm;
                _SIMSURL = SIMSURL;
                _office_id = office_id;
                _wsc_id = wsc_id;
                _agency_cd = agency_cd;
                _RecordType = RecordType;
                _Active = Active;
                _Analyzer = Analyzer;
                _AnalyzedDt = AnalyzedDt;
                _AnalyzedBy = AnalyzedBy;
                _Approver = Approver;
                _ApprovedDt = ApprovedDt;
                _ApprovedBy = ApprovedBy;
                _trip_ids = trip_ids;
            }
        }
        #endregion
    }
}