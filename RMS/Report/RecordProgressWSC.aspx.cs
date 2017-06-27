using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace RMS.Report
{
    public partial class RecordProgressWSC : System.Web.UI.Page
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
            string office_id = Request.QueryString["office_id"];
            string wsc_id = Request.QueryString["wsc_id"];

            //If an office was passed, then use this to determine the WSCID
            if (!string.IsNullOrEmpty(office_id))
            {
                OfficeID = Convert.ToInt32(office_id);
                WSCID = Convert.ToInt32(db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id);
            }
            //If a WSC was passed, then set the local variable - don't need to worry about setting the OfficeID (the default view will be to show all users for all offfices
            else if (!string.IsNullOrEmpty(wsc_id))
            {
                WSCID = Convert.ToInt32(wsc_id);
                OfficeID = (int)db.Offices.FirstOrDefault(p => p.wsc_id == WSCID).office_id;
            }
            else
            {
                //If the office id and wsc id session variables are empty, set these values to the user's assigned office
                if (OfficeID == 0 && WSCID == 0)
                {
                    OfficeID = user.OfficeID;
                    WSCID = (int)db.Offices.FirstOrDefault(p => p.office_id == user.OfficeID).wsc_id;
                }
            }

            string wsc_nm = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).wsc_nm;
            ph1.Title = "Current Record Progress by Office";
            ph1.SubTitle = "For the " + wsc_nm + " WSC";
            ph1.RecordType = "&nbsp;";

            if (!Page.IsPostBack)
            {
                InitialDataBind();
            }
        }

        /// <summary>
        /// When binding the chart data initially, pull data for the entire WSC
        /// </summary>
        protected void InitialDataBind()
        {
            if (rdpEndDt.SelectedDate == null) rdpEndDt.SelectedDate = DateTime.Now.AddDays(-90);

            BindChartsByWSC();
        }

        protected void BindChartsByWSC()
        {
            //For the All Records Chart
            var allRecs = db.SP_RMS_Progress_Report_by_region_or_WSC("NADA", WSCID, rdpEndDt.SelectedDate, false, "no")
                .Select(p => new
                {
                    TotalSites = p.TotalSites,
                    Analyzed = p.Analyzed,
                    Approved = p.Approved,
                    AnalyzedPercent = Decimal.Divide((decimal)p.Analyzed, (decimal)p.TotalSites) * 100,
                    ApprovedPercent = Decimal.Divide((decimal)p.Approved, (decimal)p.TotalSites) * 100,
                    AnalyzedPercentString = String.Format("{0:###.##}%", Decimal.Divide((decimal)p.Analyzed, (decimal)p.TotalSites) * 100),
                    ApprovedPercentString = String.Format("{0:###.##}%", Decimal.Divide((decimal)p.Approved, (decimal)p.TotalSites) * 100)
                });

            ltlAllRecordsTR.Text = "Total Records = " + db.SP_RMS_Progress_Report_by_region_or_WSC("NADA", WSCID, rdpEndDt.SelectedDate, false, "no").FirstOrDefault().TotalSites.ToString();

            rhcAllRecords.DataSource = allRecs;
            rhcAllRecords.DataBind();

            //For the Time-Series Records Chart
            var tsRecs = db.SP_RMS_Progress_Report_by_region_or_WSC("NADA", WSCID, rdpEndDt.SelectedDate, true, "yes")
                .Select(p => new
                {
                    TotalSites = p.TotalSites,
                    Analyzed = p.Analyzed,
                    Approved = p.Approved,
                    AnalyzedPercent = Decimal.Divide((decimal)p.Analyzed, (decimal)p.TotalSites) * 100,
                    ApprovedPercent = Decimal.Divide((decimal)p.Approved, (decimal)p.TotalSites) * 100,
                    AnalyzedPercentString = String.Format("{0:###.##}%", Decimal.Divide((decimal)p.Analyzed, (decimal)p.TotalSites) * 100),
                    ApprovedPercentString = String.Format("{0:###.##}%", Decimal.Divide((decimal)p.Approved, (decimal)p.TotalSites) * 100)
                });

            ltlTSRecordsTR.Text = "Total Records = " + db.SP_RMS_Progress_Report_by_region_or_WSC("NADA", WSCID, rdpEndDt.SelectedDate, true, "yes").FirstOrDefault().TotalSites.ToString();

            rhcTSRecords.DataSource = tsRecs;
            rhcTSRecords.DataBind();

            //For the Non-Time-Series Records Chart
            var ntsRecs = db.SP_RMS_Progress_Report_by_region_or_WSC("NADA", WSCID, rdpEndDt.SelectedDate, false, "yes")
                .Select(p => new
                {
                    TotalSites = p.TotalSites,
                    Analyzed = p.Analyzed,
                    Approved = p.Approved,
                    AnalyzedPercent = Decimal.Divide((decimal)p.Analyzed, (decimal)p.TotalSites) * 100,
                    ApprovedPercent = Decimal.Divide((decimal)p.Approved, (decimal)p.TotalSites) * 100,
                    AnalyzedPercentString = String.Format("{0:###.##}%", Decimal.Divide((decimal)p.Analyzed, (decimal)p.TotalSites) * 100),
                    ApprovedPercentString = String.Format("{0:###.##}%", Decimal.Divide((decimal)p.Approved, (decimal)p.TotalSites) * 100)
                });

            ltlNTSRecordsTR.Text = "Total Records = " + db.SP_RMS_Progress_Report_by_region_or_WSC("NADA", WSCID, rdpEndDt.SelectedDate, false, "yes").FirstOrDefault().TotalSites.ToString();

            rhcNTSRecords.DataSource = ntsRecs;
            rhcNTSRecords.DataBind();
        }

        #region rgOffice
        protected void rgOffice_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            rgOffice.DataSource = db.Offices.Where(p => p.wsc_id == WSCID).Select(p => new { office_id = p.office_id, office_nm = p.office_nm }).OrderBy(p => p.office_nm);
        }

        protected void rgOffice_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                GridDataItem item = e.Item as GridDataItem;
                RadHtmlChart allProgressChart = item["AllRecordsProgressChartColumn"].FindControl("rhcAllProgress") as RadHtmlChart;
                RadHtmlChart tsProgressChart = item["TSRecordsProgressChartColumn"].FindControl("rhcTSProgress") as RadHtmlChart;
                RadHtmlChart ntsProgressChart = item["NTSRecordsProgressChartColumn"].FindControl("rhcNTSProgress") as RadHtmlChart;
                int office_id = Convert.ToInt32(item.GetDataKeyValue("office_id"));

                RecordProgressItem rpi = new RecordProgressItem();
                var allProgressChartData = db.SP_RMS_Progress_Report_by_office_id(office_id, rdpEndDt.SelectedDate, false, "no").FirstOrDefault();
                var tsProgressChartData = db.SP_RMS_Progress_Report_by_office_id(office_id, rdpEndDt.SelectedDate, true, "yes").FirstOrDefault();
                var ntsProgressChartData = db.SP_RMS_Progress_Report_by_office_id(office_id, rdpEndDt.SelectedDate, false, "yes").FirstOrDefault();

                //All Records Progress Chart Data
                rpi.AllRecords = (int)allProgressChartData.TotalSites;
                rpi.AllAnalyzed = (int)allProgressChartData.Analyzed;
                rpi.AllApproved = (int)allProgressChartData.Approved;
                rpi.PercentAllAnalyzed = rpi.AllRecords > 0 ? Decimal.Divide((decimal)rpi.AllAnalyzed, (decimal)rpi.AllRecords) * 100 : 0;
                rpi.PercentAllApproved = rpi.AllRecords > 0 ? Decimal.Divide((decimal)rpi.AllApproved, (decimal)rpi.AllRecords) * 100 : 0;
                rpi.PercentAllAnalyzedString = rpi.AllRecords > 0 ? String.Format("{0:###.##}%", Decimal.Divide((decimal)rpi.AllAnalyzed, (decimal)rpi.AllRecords) * 100) : "0%";
                rpi.PercentAllApprovedString = rpi.AllRecords > 0 ? String.Format("{0:###.##}%", Decimal.Divide((decimal)rpi.AllApproved, (decimal)rpi.AllRecords) * 100) : "0%";

                //Time-Series Records Progress Chart Data
                rpi.TSRecords = (int)tsProgressChartData.TotalSites;
                rpi.TSAnalyzed = (int)tsProgressChartData.Analyzed;
                rpi.TSApproved = (int)tsProgressChartData.Approved;
                rpi.PercentTSAnalyzed = rpi.TSRecords > 0 ? Decimal.Divide((decimal)rpi.TSAnalyzed, (decimal)rpi.TSRecords) * 100 : 0;
                rpi.PercentTSApproved = rpi.TSRecords > 0 ? Decimal.Divide((decimal)rpi.TSApproved, (decimal)rpi.TSRecords) * 100 : 0;
                rpi.PercentTSAnalyzedString = rpi.TSRecords > 0 ? String.Format("{0:###.##}%", Decimal.Divide((decimal)rpi.TSAnalyzed, (decimal)rpi.TSRecords) * 100) : "0%";
                rpi.PercentTSApprovedString = rpi.TSRecords > 0 ? String.Format("{0:###.##}%", Decimal.Divide((decimal)rpi.TSApproved, (decimal)rpi.TSRecords) * 100) : "0%";

                //Non-Time-Series Records Progress Chart Data
                rpi.NTSRecords = (int)ntsProgressChartData.TotalSites;
                rpi.NTSAnalyzed = (int)ntsProgressChartData.Analyzed;
                rpi.NTSApproved = (int)ntsProgressChartData.Approved;
                rpi.PercentNTSAnalyzed = rpi.NTSRecords > 0 ? Decimal.Divide((decimal)rpi.NTSAnalyzed, (decimal)rpi.NTSRecords) * 100 : 0;
                rpi.PercentNTSApproved = rpi.NTSRecords > 0 ? Decimal.Divide((decimal)rpi.NTSApproved, (decimal)rpi.NTSRecords) * 100 : 0;
                rpi.PercentNTSAnalyzedString = rpi.NTSRecords > 0 ? String.Format("{0:###.##}%", Decimal.Divide((decimal)rpi.NTSAnalyzed, (decimal)rpi.NTSRecords) * 100) : "0%";
                rpi.PercentNTSApprovedString = rpi.NTSRecords > 0 ? String.Format("{0:###.##}%", Decimal.Divide((decimal)rpi.NTSApproved, (decimal)rpi.NTSRecords) * 100) : "0%";
                
                List<RecordProgressItem> chartData = new List<RecordProgressItem>();
                chartData.Add(rpi);

                allProgressChart.DataSource = chartData;
                allProgressChart.DataBind();

                tsProgressChart.DataSource = chartData;
                tsProgressChart.DataBind();

                ntsProgressChart.DataSource = chartData;
                ntsProgressChart.DataBind();
            }
        }
        #endregion


        protected void UpdateDetails(object sender, CommandEventArgs e)
        {
            InitialDataBind();
            rgOffice.Rebind();
        }

        #region Internal Classes
        internal class RecordProgressItem
        {
            private int _allRecords;
            private int _allAnalyzed;
            private int _allApproved;
            private decimal _percentAllAnalyzed;
            private decimal _percentAllApproved;
            private string _percentAllAnalyzedString;
            private string _percentAllApprovedString;
            private int _tsRecords;
            private int _tsAnalyzed;
            private int _tsApproved;
            private decimal _percentTSAnalyzed;
            private decimal _percentTSApproved;
            private string _percentTSAnalyzedString;
            private string _percentTSApprovedString;
            private int _ntsRecords;
            private int _ntsAnalyzed;
            private int _ntsApproved;
            private decimal _percentNTSAnalyzed;
            private decimal _percentNTSApproved;
            private string _percentNTSAnalyzedString;
            private string _percentNTSApprovedString;

            public int AllRecords
            {
                get { return _allRecords; }
                set { _allRecords = value; }
            }
            public int AllAnalyzed
            {
                get { return _allAnalyzed; }
                set { _allAnalyzed = value; }
            }
            public int AllApproved
            {
                get { return _allApproved; }
                set { _allApproved = value; }
            }
            public decimal PercentAllAnalyzed
            {
                get { return _percentAllAnalyzed; }
                set { _percentAllAnalyzed = value; }
            }
            public decimal PercentAllApproved
            {
                get { return _percentAllApproved; }
                set { _percentAllApproved = value; }
            }
            public string PercentAllAnalyzedString
            {
                get { return _percentAllAnalyzedString; }
                set { _percentAllAnalyzedString = value; }
            }
            public string PercentAllApprovedString
            {
                get { return _percentAllApprovedString; }
                set { _percentAllApprovedString = value; }
            }
            public int TSRecords
            {
                get { return _tsRecords; }
                set { _tsRecords = value; }
            }
            public int TSAnalyzed
            {
                get { return _tsAnalyzed; }
                set { _tsAnalyzed = value; }
            }
            public int TSApproved
            {
                get { return _tsApproved; }
                set { _tsApproved = value; }
            }
            public decimal PercentTSAnalyzed
            {
                get { return _percentTSAnalyzed; }
                set { _percentTSAnalyzed = value; }
            }
            public decimal PercentTSApproved
            {
                get { return _percentTSApproved; }
                set { _percentTSApproved = value; }
            }
            public string PercentTSAnalyzedString
            {
                get { return _percentTSAnalyzedString; }
                set { _percentTSAnalyzedString = value; }
            }
            public string PercentTSApprovedString
            {
                get { return _percentTSApprovedString; }
                set { _percentTSApprovedString = value; }
            }
            public int NTSRecords
            {
                get { return _ntsRecords; }
                set { _ntsRecords = value; }
            }
            public int NTSAnalyzed
            {
                get { return _ntsAnalyzed; }
                set { _ntsAnalyzed = value; }
            }
            public int NTSApproved
            {
                get { return _ntsApproved; }
                set { _ntsApproved = value; }
            }
            public decimal PercentNTSAnalyzed
            {
                get { return _percentNTSAnalyzed; }
                set { _percentNTSAnalyzed = value; }
            }
            public decimal PercentNTSApproved
            {
                get { return _percentNTSApproved; }
                set { _percentNTSApproved = value; }
            }
            public string PercentNTSAnalyzedString
            {
                get { return _percentNTSAnalyzedString; }
                set { _percentNTSAnalyzedString = value; }
            }
            public string PercentNTSApprovedString
            {
                get { return _percentNTSApprovedString; }
                set { _percentNTSApprovedString = value; }
            }

            public RecordProgressItem()
            {
                _allRecords = AllRecords;
                _allAnalyzed = AllAnalyzed;
                _allApproved = AllApproved;
                _percentAllAnalyzed = PercentAllAnalyzed;
                _percentAllApproved = PercentAllApproved;
                _percentAllAnalyzedString = PercentAllAnalyzedString;
                _percentAllApprovedString = PercentAllApprovedString;
                _tsRecords = TSRecords;
                _tsAnalyzed = TSAnalyzed;
                _tsApproved = TSApproved;
                _percentTSAnalyzed = PercentTSAnalyzed;
                _percentTSApproved = PercentTSApproved;
                _percentTSAnalyzedString = PercentTSAnalyzedString;
                _percentTSApprovedString = PercentTSApprovedString;
                _ntsRecords = NTSRecords;
                _ntsAnalyzed = NTSAnalyzed;
                _ntsApproved = NTSApproved;
                _percentNTSAnalyzed = PercentNTSAnalyzed;
                _percentNTSApproved = PercentNTSApproved;
                _percentNTSAnalyzedString = PercentNTSAnalyzedString;
                _percentNTSApprovedString = PercentNTSApprovedString;
            }
        }
        #endregion
    }
}