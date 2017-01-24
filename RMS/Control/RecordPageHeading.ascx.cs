using Core;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RMS.Control
{
    public partial class RecordPageHeading : System.Web.UI.UserControl
    {
        #region Local Variables
        private SIMSDataContext db = new SIMSDataContext();
        private WindowsAuthenticationUser user = new WindowsAuthenticationUser();
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
        private int RecordID
        {
            get
            {
                if (Session["RecordID"] == null) return 0; else return (int)Session["RecordID"];
            }
            set
            {
                Session["RecordID"] = value;
            }
        }
        #endregion

        #region Public Properties
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string RecordType { get; set; }
        public void RefreshHeadingData()
        {
            hlPageSubTitle.Text = SubTitle;
            int SiteID = Convert.ToInt32(db.Records.FirstOrDefault(p => p.rms_record_id == RecordID).site_id);
            hlPageSubTitle.NavigateUrl = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMS2017URL, SiteID);
            SetupResponsibleOfficeInfo();
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ltlPageTitle.Text = Title;
                hlPageSubTitle.Text = SubTitle;
                ltlRecordType.Text = RecordType;

                if (RecordID > 0)
                {
                    int SiteID = Convert.ToInt32(db.Records.FirstOrDefault(p => p.rms_record_id == RecordID).site_id);
                    hlPageSubTitle.NavigateUrl = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMS2017URL, SiteID);
                }
                else
                {
                    hlPageSubTitle.NavigateUrl = String.Format("{0}RMSWSCHome.aspx", Config.RMSURL);
                }

                SetupResponsibleOfficeInfo();
            }
        }

        protected void SetupResponsibleOfficeInfo()
        {
            Office office = db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault();
            ltlOfficeInfo.Text = String.Format("<a href='{0}SIMSWSCHome.aspx?wsc_id={1}&office_id={2}'>{3}</a><br />{4}<br />{5}<br />{6}", Config.SIMS2017URL, WSCID, OfficeID, office.office_nm, office.street_addrs, office.city_st_zip, office.ph_no);
        }
    }
}