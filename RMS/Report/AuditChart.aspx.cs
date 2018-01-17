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
    public partial class AuditChart : System.Web.UI.Page
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
                else if (OfficeID == 0 && WSCID > 0)
                    OfficeID = db.Offices.FirstOrDefault(p => p.wsc_id == WSCID).office_id;
                else if (OfficeID > 0 && WSCID == 0)
                    WSCID = (int)db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id;
            }

            string wsc_nm = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).wsc_nm;
            ph1.Title = "Current Audit Progress by Record Type";
            ph1.SubTitle = "For the " + wsc_nm + " WSC";
            ph1.RecordType = "&nbsp;";

            if (!Page.IsPostBack)
            {

            }
        }

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
                DateTime date15MonthsAgo = DateTime.Now.AddMonths(-15);

                RecordProgressItem rpi = new RecordProgressItem();
                var countsByRecordType = db.SP_RMS_Audit_Progress_by_recordtype(record_type_id, date15MonthsAgo).FirstOrDefault();

                //Progress Chart Data
                rpi.TotalRecords = (int)countsByRecordType.total_sites;
                rpi.Audited = (int)countsByRecordType.audited;
                rpi.PercentAudited = rpi.TotalRecords > 0 ? Decimal.Divide((decimal)rpi.Audited, (decimal)rpi.TotalRecords) * 100 : 0;
                rpi.PercentAuditedString = rpi.TotalRecords > 0 ? String.Format("{0:###.##}%", Decimal.Divide((decimal)rpi.Audited, (decimal)rpi.TotalRecords) * 100) : "0%";

                List<RecordProgressItem> chartData = new List<RecordProgressItem>();
                chartData.Add(rpi);

                progressChart.DataSource = chartData;
                progressChart.DataBind();
            }
        }
        #endregion

        #region Internal Classes
        internal class RecordProgressItem
        {
            private int _totalRecords;
            private int _audited;
            private decimal _percentAudited;
            private string _percentAuditedString;

            public int TotalRecords
            {
                get { return _totalRecords; }
                set { _totalRecords = value; }
            }
            public int Audited
            {
                get { return _audited; }
                set { _audited = value; }
            }
            public decimal PercentAudited
            {
                get { return _percentAudited; }
                set { _percentAudited = value; }
            }
            public string PercentAuditedString
            {
                get { return _percentAuditedString; }
                set { _percentAuditedString = value; }
            }

            public RecordProgressItem()
            {
                _totalRecords = TotalRecords;
                _audited = Audited;
                _percentAudited = PercentAudited;
                _percentAuditedString = PercentAuditedString;
            }
        }
        #endregion
    }
}