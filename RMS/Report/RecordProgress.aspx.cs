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
    public partial class RecordProgress : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private int reportOfficeID { get; set; }
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
                else if (OfficeID == 0 && WSCID > 0)
                    OfficeID = db.Offices.FirstOrDefault(p => p.wsc_id == WSCID).office_id;
                else if (OfficeID > 0 && WSCID == 0)
                    WSCID = (int)db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id;
            }

            string wsc_nm = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).wsc_nm;
            ph1.Title = "Current Record Progress by Office";
            ph1.SubTitle = "For the " + wsc_nm + " WSC";
            ph1.RecordType = "&nbsp;";

            if (!Page.IsPostBack)
            {
                //On initial load, narrow down the view to the responsible office
                reportOfficeID = OfficeID;
                InitialDataBind();
            }
        }

        /// <summary>
        /// When binding the chart data initially, pull data for the entire WSC
        /// </summary>
        protected void InitialDataBind()
        {
            //Filter controls
            rddlOffice.DataSource = db.Offices.Where(p => p.wsc_id == WSCID).OrderBy(p => p.office_nm).ToList();
            rddlOffice.DataBind();
            rddlOffice.Items.Insert(0, "All Offices");
            rddlOffice.SelectedValue = reportOfficeID.ToString();

            if (rdpEndDt.SelectedDate == null) rdpEndDt.SelectedDate = DateTime.Now.AddDays(-90);

            if (reportOfficeID == 0) BindChartsByWSC();
            else BindChartsByOffice();
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

        protected void BindChartsByOffice()
        {
            //For the All Records Chart
            var allRecs = db.SP_RMS_Progress_Report_by_office_id(reportOfficeID, rdpEndDt.SelectedDate, false, "no")
                .Select(p => new
                {
                    TotalSites = p.TotalSites,
                    Analyzed = p.Analyzed,
                    Approved = p.Approved,
                    AnalyzedPercent = p.TotalSites > 0 ? Decimal.Divide((decimal)p.Analyzed, (decimal)p.TotalSites) * 100 : 0,
                    ApprovedPercent = p.TotalSites > 0 ? Decimal.Divide((decimal)p.Approved, (decimal)p.TotalSites) * 100 : 0,
                    AnalyzedPercentString = p.TotalSites > 0 ? String.Format("{0:###.##}%", Decimal.Divide((decimal)p.Analyzed, (decimal)p.TotalSites) * 100) : "0%",
                    ApprovedPercentString = p.TotalSites > 0 ? String.Format("{0:###.##}%", Decimal.Divide((decimal)p.Approved, (decimal)p.TotalSites) * 100) : "0%"
                });

            ltlAllRecordsTR.Text = "Total Records = " + db.SP_RMS_Progress_Report_by_office_id(reportOfficeID, rdpEndDt.SelectedDate, false, "no").FirstOrDefault().TotalSites.ToString();

            rhcAllRecords.DataSource = allRecs;
            rhcAllRecords.DataBind();

            //For the Time-Series Records Chart
            var tsRecs = db.SP_RMS_Progress_Report_by_office_id(reportOfficeID, rdpEndDt.SelectedDate, true, "yes")
                .Select(p => new
                {
                    TotalSites = p.TotalSites,
                    Analyzed = p.Analyzed,
                    Approved = p.Approved,
                    AnalyzedPercent = p.TotalSites > 0 ? Decimal.Divide((decimal)p.Analyzed, (decimal)p.TotalSites) * 100 : 0,
                    ApprovedPercent = p.TotalSites > 0 ? Decimal.Divide((decimal)p.Approved, (decimal)p.TotalSites) * 100 : 0,
                    AnalyzedPercentString = p.TotalSites > 0 ? String.Format("{0:###.##}%", Decimal.Divide((decimal)p.Analyzed, (decimal)p.TotalSites) * 100) : "0%",
                    ApprovedPercentString = p.TotalSites > 0 ? String.Format("{0:###.##}%", Decimal.Divide((decimal)p.Approved, (decimal)p.TotalSites) * 100) : "0%"
                });

            ltlTSRecordsTR.Text = "Total Records = " + db.SP_RMS_Progress_Report_by_office_id(reportOfficeID, rdpEndDt.SelectedDate, true, "yes").FirstOrDefault().TotalSites.ToString();

            rhcTSRecords.DataSource = tsRecs;
            rhcTSRecords.DataBind();

            //For the Non-Time-Series Records Chart
            var ntsRecs = db.SP_RMS_Progress_Report_by_office_id(reportOfficeID, rdpEndDt.SelectedDate, false, "yes")
                .Select(p => new
                {
                    TotalSites = p.TotalSites,
                    Analyzed = p.Analyzed,
                    Approved = p.Approved,
                    AnalyzedPercent = p.TotalSites > 0 ? Decimal.Divide((decimal)p.Analyzed, (decimal)p.TotalSites) * 100 : 0,
                    ApprovedPercent = p.TotalSites > 0 ? Decimal.Divide((decimal)p.Approved, (decimal)p.TotalSites) * 100 : 0,
                    AnalyzedPercentString = p.TotalSites > 0 ? String.Format("{0:###.##}%", Decimal.Divide((decimal)p.Analyzed, (decimal)p.TotalSites) * 100) : "0%",
                    ApprovedPercentString = p.TotalSites > 0 ? String.Format("{0:###.##}%", Decimal.Divide((decimal)p.Approved, (decimal)p.TotalSites) * 100) : "0%"
                });

            ltlNTSRecordsTR.Text = "Total Records = " + db.SP_RMS_Progress_Report_by_office_id(reportOfficeID, rdpEndDt.SelectedDate, false, "yes").FirstOrDefault().TotalSites.ToString();

            rhcNTSRecords.DataSource = ntsRecs;
            rhcNTSRecords.DataBind();
        }

        #region rgEmployees
        protected void rgEmployees_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if (reportOfficeID == 0) rgEmployees.DataSource = db.Employees.Where(p => p.Office.wsc_id == WSCID && p.show_reports == true && p.active == true).Select(p => new { user_id = p.user_id, office_cd = p.Office.office_cd }).OrderBy(p => p.office_cd).ThenBy(p => p.user_id);
            else rgEmployees.DataSource = db.Employees.Where(p => p.office_id == reportOfficeID && p.show_reports == true && p.active == true).Select(p => new { user_id = p.user_id, office_cd = p.Office.office_cd }).OrderBy(p => p.office_cd).ThenBy(p => p.user_id);
        }

        protected void rgEmployees_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                GridDataItem item = e.Item as GridDataItem;
                RadHtmlChart totalProgressChart = item["TotalProgressChartColumn"].FindControl("rhcTotalProgress") as RadHtmlChart;
                RadHtmlChart assignedProgressChart = item["AssignedProgressChartColumn"].FindControl("rhcAssignedProgress") as RadHtmlChart;
                RadHtmlChart assignedForAnalyzingChart = item["AssignedForAnalyzingChartColumn"].FindControl("rhcAssignedForAnalyzing") as RadHtmlChart;
                RadHtmlChart assignedForApprovingChart = item["AssignedForApprovingChartColumn"].FindControl("rhcAssignedForApproving") as RadHtmlChart;
                string user_id = item.GetDataKeyValue("user_id").ToString();
                
                RecordProgressItem rpi = new RecordProgressItem();
                var totalCountsForAnalyzer = db.SP_RMS_Progress_Counts_for_analyzer(user_id, rdpEndDt.SelectedDate, "total").FirstOrDefault();
                var totalCountsForApprover = db.SP_RMS_Progress_Counts_for_approver(user_id, rdpEndDt.SelectedDate, "total").FirstOrDefault();
                var assignedCountsForAnalyzer = db.SP_RMS_Progress_Counts_for_analyzer(user_id, rdpEndDt.SelectedDate, "assigned").FirstOrDefault();
                var assignedCountsForApprover = db.SP_RMS_Progress_Counts_for_approver(user_id, rdpEndDt.SelectedDate, "assigned").FirstOrDefault();

                //Total Progress Chart Data
                rpi.TotalRecordsAssignedToAnalyze = (int)totalCountsForAnalyzer.total_sites;
                rpi.TotalRecordsAssignedToApprove = (int)totalCountsForApprover.total_sites;
                rpi.TotalActuallyAnalyzed = (int)totalCountsForAnalyzer.analyzed;
                rpi.TotalActuallyApproved = (int)totalCountsForApprover.approved;
                rpi.PercentActuallyAnalyzed = rpi.TotalRecordsAssignedToAnalyze > 0 ? Decimal.Divide((decimal)rpi.TotalActuallyAnalyzed, (decimal)rpi.TotalRecordsAssignedToAnalyze) * 100 : 0;
                rpi.PercentActuallyApproved = rpi.TotalRecordsAssignedToApprove > 0 ? Decimal.Divide((decimal)rpi.TotalActuallyApproved, (decimal)rpi.TotalRecordsAssignedToApprove) * 100 : 0;
                rpi.PercentActuallyAnalyzedString = rpi.TotalRecordsAssignedToAnalyze > 0 ? String.Format("{0:###.##}%", Decimal.Divide((decimal)rpi.TotalActuallyAnalyzed, (decimal)rpi.TotalRecordsAssignedToAnalyze) * 100) : "0%";
                rpi.PercentActuallyApprovedString = rpi.TotalRecordsAssignedToApprove > 0 ? String.Format("{0:###.##}%", Decimal.Divide((decimal)rpi.TotalActuallyApproved, (decimal)rpi.TotalRecordsAssignedToApprove) * 100) : "0%";

                //Assigned Progress Chart Data
                rpi.TotalAssignedAnalyzed = (int)assignedCountsForAnalyzer.analyzed;
                rpi.TotalAssignedApproved = (int)assignedCountsForApprover.approved;
                rpi.PercentAssignedAnalyzed = rpi.TotalRecordsAssignedToAnalyze > 0 ? Decimal.Divide((decimal)rpi.TotalAssignedAnalyzed, (decimal)rpi.TotalRecordsAssignedToAnalyze) * 100 : 0;
                rpi.PercentAssignedApproved = rpi.TotalRecordsAssignedToApprove > 0 ? Decimal.Divide((decimal)rpi.TotalAssignedApproved, (decimal)rpi.TotalRecordsAssignedToApprove) * 100 : 0;
                rpi.PercentAssignedAnalyzedString = rpi.TotalRecordsAssignedToAnalyze > 0 ? String.Format("{0:###.##}%", Decimal.Divide((decimal)rpi.TotalAssignedAnalyzed, (decimal)rpi.TotalRecordsAssignedToAnalyze) * 100) : "0%";
                rpi.PercentAssignedApprovedString = rpi.TotalRecordsAssignedToApprove > 0 ? String.Format("{0:###.##}%", Decimal.Divide((decimal)rpi.TotalAssignedApproved, (decimal)rpi.TotalRecordsAssignedToApprove) * 100) : "0%";

                //Records Assigned For Analyzing Chart Data
                rpi.RecordsAssignedToAnalyzeAnalyzed = (int)assignedCountsForAnalyzer.analyzed;
                rpi.RecordsAssignedToAnalyzeApproved = (int)assignedCountsForAnalyzer.approved;
                rpi.PercentRecordsAssignedToAnalyzeAnalyzed = rpi.TotalRecordsAssignedToAnalyze > 0 ? Decimal.Divide((decimal)rpi.RecordsAssignedToAnalyzeAnalyzed, (decimal)rpi.TotalRecordsAssignedToAnalyze) * 100 : 0;
                rpi.PercentRecordsAssignedToAnalyzeApproved = rpi.TotalRecordsAssignedToAnalyze > 0 ? Decimal.Divide((decimal)rpi.RecordsAssignedToAnalyzeApproved, (decimal)rpi.TotalRecordsAssignedToAnalyze) * 100 : 0;
                rpi.PercentRecordsAssignedToAnalyzeAnalyzedString = rpi.TotalRecordsAssignedToAnalyze > 0 ? String.Format("{0:###.##}%", Decimal.Divide((decimal)rpi.RecordsAssignedToAnalyzeAnalyzed, (decimal)rpi.TotalRecordsAssignedToAnalyze) * 100) : "0%";
                rpi.PercentRecordsAssignedToAnalyzeApprovedString = rpi.TotalRecordsAssignedToAnalyze > 0 ? String.Format("{0:###.##}%", Decimal.Divide((decimal)rpi.RecordsAssignedToAnalyzeApproved, (decimal)rpi.TotalRecordsAssignedToAnalyze) * 100) : "0%";

                //Records Assigned For Approving Chart Data
                rpi.RecordsAssignedToApproveAnalyzed = (int)assignedCountsForApprover.analyzed;
                rpi.RecordsAssignedToApproveApproved = (int)assignedCountsForApprover.approved;
                rpi.PercentRecordsAssignedToApproveAnalyzed = rpi.TotalRecordsAssignedToApprove > 0 ? Decimal.Divide((decimal)rpi.RecordsAssignedToApproveAnalyzed, (decimal)rpi.TotalRecordsAssignedToApprove) * 100 : 0;
                rpi.PercentRecordsAssignedToApproveApproved = rpi.TotalRecordsAssignedToApprove > 0 ? Decimal.Divide((decimal)rpi.RecordsAssignedToApproveApproved, (decimal)rpi.TotalRecordsAssignedToApprove) * 100 : 0;
                rpi.PercentRecordsAssignedToApproveAnalyzedString = rpi.TotalRecordsAssignedToApprove > 0 ? String.Format("{0:###.##}%", Decimal.Divide((decimal)rpi.RecordsAssignedToApproveAnalyzed, (decimal)rpi.TotalRecordsAssignedToApprove) * 100) : "0%";
                rpi.PercentRecordsAssignedToApproveApprovedString = rpi.TotalRecordsAssignedToApprove > 0 ? String.Format("{0:###.##}%", Decimal.Divide((decimal)rpi.RecordsAssignedToApproveApproved, (decimal)rpi.TotalRecordsAssignedToApprove) * 100) : "0%";

                List<RecordProgressItem> chartData = new List<RecordProgressItem>();
                chartData.Add(rpi);

                totalProgressChart.DataSource = chartData;
                totalProgressChart.DataBind();

                assignedProgressChart.DataSource = chartData;
                assignedProgressChart.DataBind();

                assignedForAnalyzingChart.DataSource = chartData;
                assignedForAnalyzingChart.DataBind();

                assignedForApprovingChart.DataSource = chartData;
                assignedForApprovingChart.DataBind();
            }
        }
        #endregion

        #region rgRecordTypes
        protected void rgRecordTypes_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            rgRecordTypes.DataSource = db.RecordTypes.Where(p => p.wsc_id == WSCID).Select(p => new { record_type_id = p.record_type_id, RecordType = p.type_ds + " (" + p.type_cd + ")", type_ds = p.type_ds }).OrderBy(p => p.type_ds);
        }

        protected void rgRecordTypes_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                GridDataItem item = e.Item as GridDataItem;
                RadHtmlChart progressChart = item["ProgressChartColumn"].FindControl("rhcProgress") as RadHtmlChart;
                int record_type_id = Convert.ToInt32(item.GetDataKeyValue("record_type_id"));

                RecordProgressItem rpi = new RecordProgressItem();
                var countsByRecordType = db.SP_RMS_Progress_Counts_by_recordtype(record_type_id, reportOfficeID, rdpEndDt.SelectedDate).FirstOrDefault();

                //Progress Chart Data
                rpi.TotalRecords = (int)countsByRecordType.total_sites;
                rpi.Analyzed = (int)countsByRecordType.analyzed;
                rpi.Approved = (int)countsByRecordType.approved;
                rpi.PercentAnalyzed = rpi.TotalRecords > 0 ? Decimal.Divide((decimal)rpi.Analyzed, (decimal)rpi.TotalRecords) * 100 : 0;
                rpi.PercentApproved = rpi.TotalRecords > 0 ? Decimal.Divide((decimal)rpi.Approved, (decimal)rpi.TotalRecords) * 100 : 0;
                rpi.PercentAnalyzedString = rpi.TotalRecords > 0 ? String.Format("{0:###.##}%", Decimal.Divide((decimal)rpi.Analyzed, (decimal)rpi.TotalRecords) * 100) : "0%";
                rpi.PercentApprovedString = rpi.TotalRecords > 0 ? String.Format("{0:###.##}%", Decimal.Divide((decimal)rpi.Approved, (decimal)rpi.TotalRecords) * 100) : "0%";

                List<RecordProgressItem> chartData = new List<RecordProgressItem>();
                chartData.Add(rpi);

                progressChart.DataSource = chartData;
                progressChart.DataBind();
            }
        }
        #endregion

        protected void UpdateDetails(object sender, CommandEventArgs e)
        {
            reportOfficeID = Convert.ToInt32(rddlOffice.SelectedValue);
            InitialDataBind();
            rgEmployees.Rebind();
            rgRecordTypes.Rebind();
        }

        #region Internal Classes
        internal class RecordProgressItem
        {
            private int _totalRecords;
            private int _analyzed;
            private int _approved;
            private decimal _percentAnalyzed;
            private decimal _percentApproved;
            private string _percentAnalyzedString;
            private string _percentApprovedString;
            private int _totalRecordsAssignedToAnalyze;
            private int _totalRecordsAssignedToApprove;
            private int _totalActuallyAnalyzed;
            private int _totalActuallyApproved;
            private decimal _percentActuallyAnalyzed;
            private decimal _percentActuallyApproved;
            private string _percentActuallyAnalyzedString;
            private string _percentActuallyApprovedString;
            private int _totalAssignedAnalyzed;
            private int _totalAssignedApproved;
            private decimal _percentAssignedAnalyzed;
            private decimal _percentAssignedApproved;
            private string _percentAssignedAnalyzedString;
            private string _percentAssignedApprovedString;
            private int _recordsAssignedToAnalyzeAnalyzed;
            private int _recordsAssignedToAnalyzeApproved;
            private decimal _percentRecordsAssignedToAnalyzeAnalyzed;
            private decimal _percentRecordsAssignedToAnalyzeApproved;
            private string _percentRecordsAssignedToAnalyzeAnalyzedString;
            private string _percentRecordsAssignedToAnalyzeApprovedString;
            private int _recordsAssignedToApproveAnalyzed;
            private int _recordsAssignedToApproveApproved;
            private decimal _percentRecordsAssignedToApproveAnalyzed;
            private decimal _percentRecordsAssignedToApproveApproved;
            private string _percentRecordsAssignedToApproveAnalyzedString;
            private string _percentRecordsAssignedToApproveApprovedString;

            public int TotalRecords
            {
                get { return _totalRecords; }
                set { _totalRecords = value; }
            }
            public int Analyzed
            {
                get { return _analyzed; }
                set { _analyzed = value; }
            }
            public int Approved
            {
                get { return _approved; }
                set { _approved = value; }
            }
            public decimal PercentAnalyzed
            {
                get { return _percentAnalyzed; }
                set { _percentAnalyzed = value; }
            }
            public decimal PercentApproved
            {
                get { return _percentApproved; }
                set { _percentApproved = value; }
            }
            public string PercentAnalyzedString
            {
                get { return _percentAnalyzedString; }
                set { _percentAnalyzedString = value; }
            }
            public string PercentApprovedString
            {
                get { return _percentApprovedString; }
                set { _percentApprovedString = value; }
            }
            public int TotalRecordsAssignedToAnalyze
            {
                get { return _totalRecordsAssignedToAnalyze; }
                set { _totalRecordsAssignedToAnalyze = value; }
            }
            public int TotalRecordsAssignedToApprove
            {
                get { return _totalRecordsAssignedToApprove; }
                set { _totalRecordsAssignedToApprove = value; }
            }
            public int TotalActuallyAnalyzed
            {
                get { return _totalActuallyAnalyzed; }
                set { _totalActuallyAnalyzed = value; }
            }
            public int TotalActuallyApproved
            {
                get { return _totalActuallyApproved; }
                set { _totalActuallyApproved = value; }
            }
            public decimal PercentActuallyAnalyzed
            {
                get { return _percentActuallyAnalyzed; }
                set { _percentActuallyAnalyzed = value; }
            }
            public decimal PercentActuallyApproved
            {
                get { return _percentActuallyApproved; }
                set { _percentActuallyApproved = value; }
            }
            public string PercentActuallyAnalyzedString
            {
                get { return _percentActuallyAnalyzedString; }
                set { _percentActuallyAnalyzedString = value; }
            }
            public string PercentActuallyApprovedString
            {
                get { return _percentActuallyApprovedString; }
                set { _percentActuallyApprovedString = value; }
            }
            public int TotalAssignedAnalyzed
            {
                get { return _totalAssignedAnalyzed; }
                set { _totalAssignedAnalyzed = value; }
            }
            public int TotalAssignedApproved
            {
                get { return _totalAssignedApproved; }
                set { _totalAssignedApproved = value; }
            }
            public decimal PercentAssignedAnalyzed
            {
                get { return _percentAssignedAnalyzed; }
                set { _percentAssignedAnalyzed = value; }
            }
            public decimal PercentAssignedApproved
            {
                get { return _percentAssignedApproved; }
                set { _percentAssignedApproved = value; }
            }
            public string PercentAssignedAnalyzedString
            {
                get { return _percentAssignedAnalyzedString; }
                set { _percentAssignedAnalyzedString = value; }
            }
            public string PercentAssignedApprovedString
            {
                get { return _percentAssignedApprovedString; }
                set { _percentAssignedApprovedString = value; }
            }
            public int RecordsAssignedToAnalyzeAnalyzed
            {
                get { return _recordsAssignedToAnalyzeAnalyzed; }
                set { _recordsAssignedToAnalyzeAnalyzed = value; }
            }
            public int RecordsAssignedToAnalyzeApproved
            {
                get { return _recordsAssignedToAnalyzeApproved; }
                set { _recordsAssignedToAnalyzeApproved = value; }
            }
            public decimal PercentRecordsAssignedToAnalyzeAnalyzed
            {
                get { return _percentRecordsAssignedToAnalyzeAnalyzed; }
                set { _percentRecordsAssignedToAnalyzeAnalyzed = value; }
            }
            public decimal PercentRecordsAssignedToAnalyzeApproved
            {
                get { return _percentRecordsAssignedToAnalyzeApproved; }
                set { _percentRecordsAssignedToAnalyzeApproved = value; }
            }
            public string PercentRecordsAssignedToAnalyzeAnalyzedString
            {
                get { return _percentRecordsAssignedToAnalyzeAnalyzedString; }
                set { _percentRecordsAssignedToAnalyzeAnalyzedString = value; }
            }
            public string PercentRecordsAssignedToAnalyzeApprovedString
            {
                get { return _percentRecordsAssignedToAnalyzeApprovedString; }
                set { _percentRecordsAssignedToAnalyzeApprovedString = value; }
            }
            public int RecordsAssignedToApproveAnalyzed
            {
                get { return _recordsAssignedToApproveAnalyzed; }
                set { _recordsAssignedToApproveAnalyzed = value; }
            }
            public int RecordsAssignedToApproveApproved
            {
                get { return _recordsAssignedToApproveApproved; }
                set { _recordsAssignedToApproveApproved = value; }
            }
            public decimal PercentRecordsAssignedToApproveAnalyzed
            {
                get { return _percentRecordsAssignedToApproveAnalyzed; }
                set { _percentRecordsAssignedToApproveAnalyzed = value; }
            }
            public decimal PercentRecordsAssignedToApproveApproved
            {
                get { return _percentRecordsAssignedToApproveApproved; }
                set { _percentRecordsAssignedToApproveApproved = value; }
            }
            public string PercentRecordsAssignedToApproveAnalyzedString
            {
                get { return _percentRecordsAssignedToApproveAnalyzedString; }
                set { _percentRecordsAssignedToApproveAnalyzedString = value; }
            }
            public string PercentRecordsAssignedToApproveApprovedString
            {
                get { return _percentRecordsAssignedToApproveApprovedString; }
                set { _percentRecordsAssignedToApproveApprovedString = value; }
            }
            
            public RecordProgressItem()
            {
                _totalRecords = TotalRecords;
                _analyzed = Analyzed;
                _approved = Approved;
                _percentAnalyzed = PercentAnalyzed;
                _percentApproved = PercentApproved;
                _percentAnalyzedString = PercentAnalyzedString;
                _percentApprovedString = PercentApprovedString;
                _totalRecordsAssignedToAnalyze = TotalRecordsAssignedToAnalyze;
                _totalRecordsAssignedToApprove = TotalRecordsAssignedToApprove;
                _totalActuallyAnalyzed = TotalActuallyAnalyzed;
                _totalActuallyApproved = TotalActuallyApproved;
                _percentActuallyAnalyzed = PercentActuallyAnalyzed;
                _percentActuallyApproved = PercentActuallyApproved;
                _percentActuallyAnalyzedString = PercentActuallyAnalyzedString;
                _percentActuallyApprovedString = PercentActuallyApprovedString;
                _totalAssignedAnalyzed = TotalAssignedAnalyzed;
                _totalAssignedApproved = TotalAssignedApproved;
                _percentAssignedAnalyzed = PercentAssignedAnalyzed;
                _percentAssignedApproved = PercentAssignedApproved;
                _percentAssignedAnalyzedString = PercentAssignedAnalyzedString;
                _percentAssignedApprovedString = PercentAssignedApprovedString;
                _recordsAssignedToAnalyzeAnalyzed = RecordsAssignedToAnalyzeAnalyzed;
                _recordsAssignedToAnalyzeApproved = RecordsAssignedToAnalyzeApproved;
                _percentRecordsAssignedToAnalyzeAnalyzed = PercentRecordsAssignedToAnalyzeAnalyzed;
                _percentRecordsAssignedToAnalyzeApproved = PercentRecordsAssignedToAnalyzeApproved;
                _percentRecordsAssignedToAnalyzeAnalyzedString = PercentRecordsAssignedToAnalyzeAnalyzedString;
                _percentRecordsAssignedToAnalyzeApprovedString = PercentRecordsAssignedToAnalyzeApprovedString;
                _recordsAssignedToApproveAnalyzed = RecordsAssignedToApproveAnalyzed;
                _recordsAssignedToApproveApproved = RecordsAssignedToApproveApproved;
                _percentRecordsAssignedToApproveAnalyzed = PercentRecordsAssignedToApproveAnalyzed;
                _percentRecordsAssignedToApproveApproved = PercentRecordsAssignedToApproveApproved;
                _percentRecordsAssignedToApproveAnalyzedString = PercentRecordsAssignedToApproveAnalyzedString;
                _percentRecordsAssignedToApproveApprovedString = PercentRecordsAssignedToApproveApprovedString;
            }
        }
        #endregion
    }
}