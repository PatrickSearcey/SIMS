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
    public partial class CRPStatusChart : System.Web.UI.Page
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

            if (reportOfficeID == 0) BindChartByWSC();
            else BindChartByOffice();

            ltl150DaysAgo.Text = String.Format("The date 150 days ago: <b>{0:MM/dd/yyyy}</b>", DateTime.Now.AddDays(-150));
            ltl240DaysAgo.Text = String.Format("The date 240 days ago: <b>{0:MM/dd/yyyy}</b>", DateTime.Now.AddDays(-240));
            ltlCurrentData.Text = String.Format("Current data as of: <b>{0}</b>", DateTime.Now);
        }

        protected void BindChartByWSC()
        {
            rhcCRPStatus.DataSource = db.SP_CRP_Cat_Charts(0, WSCID, "alloffices").Select(p => new CRPItem
            {
                OfficeCode = p.office_cd,
                Cat1Percent = p.percent_of_cat1_done,
                RecordsThatMeetCat1Criteria = p.ApprovedInLast150Days,
                TotalCat1Records = p.TotalCat1Records,
                Cat2Percent = p.percent_of_cat2_done,
                RecordsThatMeetCat2Criteria = p.ApprovedInLast240Days,
                TotalCat2Records = p.TotalCat2Records
            });
            rhcCRPStatus.DataBind();
        }

        protected void BindChartByOffice()
        {
            rhcCRPStatus.DataSource = db.SP_CRP_Cat_Charts(reportOfficeID, 0, "records").Select(p => new CRPItem {
                RecordType = p.type_ds,
                Cat1Percent = p.percent_of_cat1_done,
                RecordsThatMeetCat1Criteria = p.ApprovedInLast150Days,
                TotalCat1Records = p.TotalCat1Records,
                Cat2Percent = p.percent_of_cat2_done,
                RecordsThatMeetCat2Criteria = p.ApprovedInLast240Days,
                TotalCat2Records = p.TotalCat2Records
            });
            rhcCRPStatus.DataBind();
        }

        protected void UpdateDetails(object sender, CommandEventArgs e)
        {
            if (rddlOffice.SelectedIndex == 0)
            {
                reportOfficeID = 0;
                BindChartByWSC();
            }
            else
            {
                reportOfficeID = Convert.ToInt32(rddlOffice.SelectedValue);
                BindChartByOffice();
            }
        }

        #region Internal Classes
        internal class CRPItem
        {
            private string _office_cd;
            private string _type_ds;
            private decimal _cat1Percent;
            private int _recordsThatMeetCat1Criteria;
            private int _totalCat1Records;
            private decimal _cat2Percent;
            private int _recordsThatMeetCat2Criteria;
            private int _totalCat2Records;

            public string OfficeCode
            {
                get { return _office_cd; }
                set { _office_cd = value; }
            }
            public string RecordType
            {
                get { return _type_ds; }
                set { _type_ds = value; }
            }
            public decimal Cat1Percent
            {
                get { return _cat1Percent; }
                set { _cat1Percent = value; }
            }
            public int RecordsThatMeetCat1Criteria
            {
                get { return _recordsThatMeetCat1Criteria; }
                set { _recordsThatMeetCat1Criteria = value; }
            }
            public int TotalCat1Records
            {
                get { return _totalCat1Records; }
                set { _totalCat1Records = value; }
            }
            public decimal Cat2Percent
            {
                get { return _cat2Percent; }
                set { _cat2Percent = value; }
            }
            public int RecordsThatMeetCat2Criteria
            {
                get { return _recordsThatMeetCat2Criteria; }
                set { _recordsThatMeetCat2Criteria = value; }
            }
            public int TotalCat2Records
            {
                get { return _totalCat2Records; }
                set { _totalCat2Records = value; }
            }

            public CRPItem()
            {
                _office_cd = OfficeCode;
                _type_ds = RecordType;
                _cat1Percent = Cat1Percent;
                _recordsThatMeetCat1Criteria = RecordsThatMeetCat1Criteria;
                _totalCat1Records = TotalCat1Records;
                _cat2Percent = Cat2Percent;
                _recordsThatMeetCat2Criteria = RecordsThatMeetCat2Criteria;
                _totalCat2Records = TotalCat2Records;
            }
        }
        #endregion
    }
}