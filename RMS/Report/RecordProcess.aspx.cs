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
    public partial class RecordProcess : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private string processTask { get; set; }
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
            processTask = Request.QueryString["task"];
            string office_id = Request.QueryString["office_id"];

            if (!string.IsNullOrEmpty(office_id))
            {
                OfficeID = Convert.ToInt32(office_id);
                WSCID = Convert.ToInt32(db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id);
            }
            else
            {
                //If the office id and wsc id session variables are empty, set these values to the user's assigned office
                if (OfficeID == 0 && WSCID == 0)
                {
                    OfficeID = user.OfficeID;
                    WSCID = (int)db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id;
                }
            }

            if (!Page.IsPostBack)
            {
                UserControlSetup();
            }
        }

        #region Page Load Events
        protected void UserControlSetup()
        {
            string office_nm = db.Offices.FirstOrDefault(p => p.office_id == OfficeID).office_nm;

            ph1.Title = "Records Ready For Analyzing and Approving";
            ph1.SubTitle = "For " + office_nm;
        }
        #endregion

        #region Methods
        private int GetDaysSinceLastApprovedInAquarius(int rms_record_id)
        {
            int days = 0;

            return days;
        }
        #endregion

        #region Analyze Records RadGrid
        protected void rgAnalyzeRecords_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            rgAnalyzeRecords.DataSource = db.vRMSRecordsToBeAnalyzeds.Where(p => p.office_id == OfficeID).Select(p => new RecordDataItem
            {
                rms_record_id = p.rms_record_id.ToString(),
                lock_dt = String.Format("{0:MM/dd/yyyy}", p.lock_dt),
                lock_type = p.lock_type,
                lock_uid = p.lock_uid,
                site_no = p.site_no,
                site_id = p.site_id.ToString(),
                SIMS2017URL = Config.SIMS2017URL,
                station_nm = p.station_full_nm,
                category_no = p.category_no.ToString(),
                type_ds = p.type_ds,
                analyzer_uid = p.analyzer_uid,
                reanalyze_status = p.reanalyze_status,
                analyzed_dt = String.Format("{0:MM/dd/yyyy}", p.LastAnalyzedDate),
                aq_approved_dt = GetDaysSinceLastApprovedInAquarius(p.rms_record_id).ToString()
            }).ToList();
        }

        protected void rgAnalyzeRecords_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgAnalyzeRecords.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" | menu.Items[i].Text == "Contains" | menu.Items[i].Text == "EqualTo" | menu.Items[i].Text == "DoesNotContain")
                    i = i + 1;
                else
                    menu.Items.RemoveAt(i);
            }
        }

        protected void rgAnalyzeRecords_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;

                int rms_record_id = Convert.ToInt32(item.GetDataKeyValue("rms_record_id"));
                var record = db.vRMSRecordsToBeAnalyzeds.FirstOrDefault(p => p.rms_record_id == rms_record_id);
                Image imgLockIcon = (Image)item.FindControl("imgLockIcon");
                HyperLink hlRecordType = (HyperLink)item.FindControl("hlRecordType");

                if (record.lock_type == "Analyze" || record.lock_type == "Approve")
                {
                    imgLockIcon.ImageUrl = "../images/lock_sm.png";
                    imgLockIcon.AlternateText = String.Format("Locked by {0}, on {1:MM/dd/yyyy}", record.lock_uid, record.lock_dt);
                }
                else if (record.lock_type == "Analyzing" || record.lock_type == "Approving")
                {
                    imgLockIcon.ImageUrl = "../images/save_sm.png";
                    imgLockIcon.AlternateText = String.Format("Saved by {0}, on {1:MM/dd/yyyy}", record.lock_uid, record.lock_dt);
                }
                else
                {
                    imgLockIcon.Visible = false;
                }

                if (String.IsNullOrEmpty(record.reanalyze_status))
                {
                    hlRecordType.NavigateUrl = String.Format("../Task/RecordProcess.aspx?rms_record_id={0}&task=Analyze", record.rms_record_id);
                }
                else if (!String.IsNullOrEmpty(record.reanalyze_status))
                {
                    var period = db.RecordAnalysisPeriods.FirstOrDefault(p => p.rms_record_id == record.rms_record_id && p.status_va == "Reanalyze");
                    hlRecordType.NavigateUrl = String.Format("../Task/RecordProcess.aspx?period_id={0}&task=Reanalyze", period.period_id);
                }
            }
        }
        #endregion

        #region Approve Records RadGrid
        protected void rgApproveRecords_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {

        }

        protected void rgApproveRecords_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgApproveRecords.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" | menu.Items[i].Text == "Contains" | menu.Items[i].Text == "EqualTo" | menu.Items[i].Text == "DoesNotContain")
                    i = i + 1;
                else
                    menu.Items.RemoveAt(i);
            }
        }

        protected void rgApproveRecords_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.IsInEditMode)
            {
                GridEditableItem edititem = (GridEditableItem)e.Item;

                //int rms_audit_id = Convert.ToInt32(edititem.GetDataKeyValue("rms_audit_id"));
                //var audit = db.Audits.FirstOrDefault(p => p.rms_audit_id == rms_audit_id);

                //Literal ltlType = (Literal)edititem.FindControl("ltlAuditType");
                //Literal ltlResults = (Literal)edititem.FindControl("ltlAuditResults");
                //Literal ltlReason = (Literal)edititem.FindControl("ltlReason");
                //Literal ltlData = (Literal)edititem.FindControl("ltlData");
                //Literal ltlFindings = (Literal)edititem.FindControl("ltlFindings");
                //RadListView rlvAuditDocs = (RadListView)edititem.FindControl("rlvAuditDocs");

                //rlvAuditDocs.DataSource = audit.AuditDocuments.Select(p => new { rms_audit_document_id = p.rms_audit_document_id, document_nm = p.document_nm }).OrderBy(p => p.document_nm);
                //rlvAuditDocs.DataBind();

                //ltlType.Text = audit.AuditType.type + ": " + audit.AuditType.description;
                //ltlResults.Text = audit.AuditResult.result + audit.AuditResult.description;
                //ltlReason.Text = audit.audit_reason;
                //ltlData.Text = audit.audit_data;
                //ltlFindings.Text = audit.audit_findings;
            }
        }
        #endregion

        #region Internal Classes
        internal class RecordDataItem
        {
            private string _rms_record_id;
            private string _site_id;
            private string _site_no;
            private string _station_nm;
            private string _SIMS2017URL;
            private string _office_id;
            private string _office_cd;
            private string _record_office_id;
            private string _wsc_id;
            private string _agency_cd;
            private string _analyzer_uid;
            private string _analyzed_dt;
            private string _lock_type;
            private string _lock_uid;
            private string _lock_dt;
            private string _reanalyze_status;
            private string _type_cd;
            private string _type_ds;
            private string _category_no;
            private string _record_type_id;
            private string _approver_uid;
            private string _prev_approved_by;
            private string _aq_approved_dt;
            private string _period;

            public string rms_record_id
            {
                get { return _rms_record_id; }
                set { _rms_record_id = value; }
            }
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
            public string SIMS2017URL
            {
                get { return _SIMS2017URL; }
                set { _SIMS2017URL = value; }
            }
            public string office_id
            {
                get { return _office_id; }
                set { _office_id = value; }
            }
            public string office_cd
            {
                get { return _office_cd; }
                set { _office_cd = value; }
            }
            public string record_office_id
            {
                get { return _record_office_id; }
                set { _record_office_id = value; }
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
            public string analyzer_uid
            {
                get { return _analyzer_uid; }
                set { _analyzer_uid = value; }
            }
            public string analyzed_dt
            {
                get { return _analyzed_dt; }
                set { _analyzed_dt = value; }
            }
            public string lock_type
            {
                get { return _lock_type; }
                set { _lock_type = value; }
            }
            public string lock_uid
            {
                get { return _lock_uid; }
                set { _lock_uid = value; }
            }
            public string lock_dt
            {
                get { return _lock_dt; }
                set { _lock_dt = value; }
            }
            public string reanalyze_status
            {
                get { return _reanalyze_status; }
                set { _reanalyze_status = value; }
            }
            public string type_cd
            {
                get { return _type_cd; }
                set { _type_cd = value; }
            }
            public string type_ds
            {
                get { return _type_ds; }
                set { _type_ds = value; }
            }
            public string category_no
            {
                get { return _category_no; }
                set { _category_no = value; }
            }
            public string record_type_id
            {
                get { return _record_type_id; }
                set { _record_type_id = value; }
            }
            public string approver_uid
            {
                get { return _approver_uid; }
                set { _approver_uid = value; }
            }
            public string prev_approved_by
            {
                get { return _prev_approved_by; }
                set { _prev_approved_by = value; }
            }
            public string aq_approved_dt
            {
                get { return _aq_approved_dt; }
                set { _aq_approved_dt = value; }
            }
            public string period
            {
                get { return _period; }
                set { _period = value; }
            }
            
            public RecordDataItem()
            {
                _rms_record_id = rms_record_id;
                _site_id = site_id;
                _site_no = site_no;
                _station_nm = station_nm;
                _SIMS2017URL = SIMS2017URL;
                _office_id = office_id;
                _office_cd = office_cd;
                _record_office_id = record_office_id;
                _wsc_id = wsc_id;
                _agency_cd = agency_cd;
                _analyzer_uid = analyzer_uid;
                _analyzed_dt = analyzed_dt;
                _lock_type = lock_type;
                _lock_uid = lock_uid;
                _lock_dt = lock_dt;
                _reanalyze_status = reanalyze_status;
                _type_cd = type_cd;
                _type_ds = type_ds;
                _category_no = category_no;
                _record_type_id = record_type_id;
                _approver_uid = approver_uid;
                _prev_approved_by = prev_approved_by;
                _aq_approved_dt = aq_approved_dt;
                _period = period;
            }
        }
        #endregion
    }
}