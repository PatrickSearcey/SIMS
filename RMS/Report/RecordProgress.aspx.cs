using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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
            string wsc_id = "31";// Request.QueryString["wsc_id"];

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
                //If an office_id was not passed to the report, then this will cause the report to show all offices' personnel initially
                if (string.IsNullOrEmpty(office_id)) reportOfficeID = 0;
                InitialDataBind();
            }
        }

        protected void InitialDataBind()
        {
            //Filter controls
            rddlOffice.DataSource = db.Offices.Where(p => p.wsc_id == WSCID).OrderBy(p => p.office_nm).ToList();
            rddlOffice.DataBind();
            rddlOffice.Items.Insert(0, "All Offices");

            rdpEndDt.SelectedDate = DateTime.Now.AddDays(-180);

            //For the All Records Chart
            var allRecs = db.SP_TOTALS_Current_by_region_or_WSC("NADA", WSCID, rdpEndDt.SelectedDate)
                .Select(p => new { 
                    TotalSites = p.TotalSites, 
                    Analyzed = p.Analyzed, 
                    Approved = p.Approved, 
                    AnalyzedPercent = Decimal.Divide((decimal)p.Analyzed, (decimal)p.TotalSites) * 100,
                    ApprovedPercent = Decimal.Divide((decimal)p.Approved, (decimal)p.TotalSites) * 100,
                    AnalyzedPercentString = String.Format("{0:###.##}%", Decimal.Divide((decimal)p.Analyzed, (decimal)p.TotalSites) * 100),
                    ApprovedPercentString = String.Format("{0:###.##}%", Decimal.Divide((decimal)p.Approved, (decimal)p.TotalSites) * 100)
                });

            ltlAllRecordsTR.Text = db.SP_TOTALS_Current_by_region_or_WSC("NADA", WSCID, rdpEndDt.SelectedDate).FirstOrDefault().TotalSites.ToString();
            
            rhcAllRecords.DataSource = allRecs;
            rhcAllRecords.DataBind();

            //For the Time-Series Records Chart

        }

        protected void UpdateDetails(object sender, CommandEventArgs e)
        {

        }
    }
}