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
                else if (OfficeID == 0 && WSCID > 0)
                    OfficeID = db.Offices.FirstOrDefault(p => p.wsc_id == WSCID).office_id;
                else if (OfficeID > 0 && WSCID == 0)
                    WSCID = (int)db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id;
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

            if (processTask == "approve")
            {
                rmp1.SelectedIndex = 1;
                rts1.SelectedIndex = 1; 
            }
            else if (processTask == "audit")
            {
                rmp1.SelectedIndex = 2;
                rts1.SelectedIndex = 2;
            }
        }
        #endregion

        #region Methods
        private int GetDaysSinceLastApprovedInAquarius(int rms_record_id)
        {
            int days = 0;

            return days;
        }
        private int CalculateMonthsSinceLast(DateTime? audit_end_dt)
        {
            int monthsSinceLast = -1;

            if (audit_end_dt != null)
            {
                monthsSinceLast = ((DateTime.Now.Year - Convert.ToDateTime(audit_end_dt).Year) * 12) + DateTime.Now.Month - Convert.ToDateTime(audit_end_dt).Month;
            }

            return monthsSinceLast;
        }
        #endregion

        #region Analyze Records RadGrid
        protected void rgAnalyzeRecords_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            rgAnalyzeRecords.DataSource = db.SP_RMS_Record_List_for_analyze_approve("analyze", user.ID, OfficeID).Select(p => new Data.RecordProcessDataItem
            {
                rms_record_id = p.rms_record_id,
                lock_dt = p.lock_dt,
                lock_type = p.lock_type,
                lock_uid = p.lock_uid,
                site_no = p.site_no,
                site_id = p.site_id,
                SIMSURL = Config.SIMSURL,
                station_nm = p.station_nm,
                category_no = p.category_no,
                type_ds = p.type_ds,
                type_cd = p.type_cd,
                analyzer_uid = p.analyzer_uid,
                reanalyze_status = p.reanalyze_status,
                LastAnalyzedDate = p.LastAnalyzedDate,
                DaysSinceAging = p.DaysSinceAging
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
                string lock_type = item["lock_type"].Text;
                string lock_uid = item["lock_uid"].Text;
                string lock_dt = item["lock_dt"].Text;
                string reanalyze_status = item["reanalyze_status"].Text;
                Image imgLockIcon = (Image)item.FindControl("imgLockIcon");
                HyperLink hlRecordType = (HyperLink)item.FindControl("hlRecordType");

                if (lock_type == "Analyze" || lock_type == "Approve" || lock_type == "Reanalyze")
                {
                    imgLockIcon.ImageUrl = "../images/lock_sm.png";
                    imgLockIcon.AlternateText = String.Format("Locked by {0}, on {1:MM/dd/yyyy}", lock_uid, lock_dt);
                }
                else if (lock_type == "Analyzing" || lock_type == "Approving")
                {
                    imgLockIcon.ImageUrl = "../images/save_sm.png";
                    imgLockIcon.AlternateText = String.Format("Saved by {0}, on {1:MM/dd/yyyy}", lock_uid, lock_dt);
                }
                else
                {
                    imgLockIcon.Visible = false;
                }

                if (reanalyze_status == "&nbsp;")
                {
                    hlRecordType.NavigateUrl = String.Format("../Task/RecordProcess.aspx?rms_record_id={0}&task=Analyze", rms_record_id);
                }
                else
                {
                    e.Item.Style.Add(0, "LightYellow !important");
                    var period = db.RecordAnalysisPeriods.FirstOrDefault(p => p.rms_record_id == rms_record_id && p.status_va == "Reanalyze");
                    hlRecordType.NavigateUrl = String.Format("../Task/RecordProcess.aspx?period_id={0}&task=Reanalyze", period.period_id);
                }
            }
        }
        #endregion

        #region Analyze My Records RadGrid
        protected void rgAnalyzeMyRecords_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            rgAnalyzeMyRecords.DataSource = db.SP_RMS_Record_List_for_analyze_approve("analyzeByUser", user.ID, OfficeID).Select(p => new Data.RecordProcessDataItem
            {
                rms_record_id = p.rms_record_id,
                lock_dt = p.lock_dt,
                lock_type = p.lock_type,
                lock_uid = p.lock_uid,
                site_no = p.site_no,
                site_id = p.site_id,
                SIMSURL = Config.SIMSURL,
                station_nm = p.station_nm,
                category_no = p.category_no,
                type_ds = p.type_ds,
                type_cd = p.type_cd,
                analyzer_uid = p.analyzer_uid,
                reanalyze_status = p.reanalyze_status,
                LastAnalyzedDate = p.LastAnalyzedDate,
                DaysSinceAging = p.DaysSinceAging
            }).ToList();
        }

        protected void rgAnalyzeMyRecords_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgAnalyzeMyRecords.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" | menu.Items[i].Text == "Contains" | menu.Items[i].Text == "EqualTo" | menu.Items[i].Text == "DoesNotContain")
                    i = i + 1;
                else
                    menu.Items.RemoveAt(i);
            }
        }

        protected void rgAnalyzeMyRecords_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;

                int rms_record_id = Convert.ToInt32(item.GetDataKeyValue("rms_record_id"));
                string lock_type = item["lock_type"].Text;
                string lock_uid = item["lock_uid"].Text;
                string lock_dt = item["lock_dt"].Text;
                string reanalyze_status = item["reanalyze_status"].Text;
                Image imgLockIcon = (Image)item.FindControl("imgLockIcon");
                HyperLink hlRecordType = (HyperLink)item.FindControl("hlRecordType");

                if (lock_type == "Analyze" || lock_type == "Approve" || lock_type == "Reanalyze")
                {
                    imgLockIcon.ImageUrl = "../images/lock_sm.png";
                    imgLockIcon.AlternateText = String.Format("Locked by {0}, on {1:MM/dd/yyyy}", lock_uid, lock_dt);
                }
                else if (lock_type == "Analyzing" || lock_type == "Approving")
                {
                    imgLockIcon.ImageUrl = "../images/save_sm.png";
                    imgLockIcon.AlternateText = String.Format("Saved by {0}, on {1:MM/dd/yyyy}", lock_uid, lock_dt);
                }
                else
                {
                    imgLockIcon.Visible = false;
                }

                if (reanalyze_status == "&nbsp;")
                {
                    hlRecordType.NavigateUrl = String.Format("../Task/RecordProcess.aspx?rms_record_id={0}&task=Analyze", rms_record_id);
                }
                else
                {
                    e.Item.BackColor = System.Drawing.Color.LightYellow;
                    var period = db.RecordAnalysisPeriods.FirstOrDefault(p => p.rms_record_id == rms_record_id && p.status_va == "Reanalyze");
                    hlRecordType.NavigateUrl = String.Format("../Task/RecordProcess.aspx?period_id={0}&task=Reanalyze", period.period_id);
                }
            }
        }
        #endregion

        #region Approve Records RadGrid
        protected void rgApproveRecords_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            rgApproveRecords.DataSource = db.SP_RMS_Record_List_for_analyze_approve("approve", user.ID, OfficeID).Select(p => new Data.RecordProcessDataItem
            {
                rms_record_id = p.rms_record_id,
                lock_dt = p.lock_dt,
                lock_type = p.lock_type,
                lock_uid = p.lock_uid,
                site_no = p.site_no,
                site_id = p.site_id,
                SIMSURL = Config.SIMSURL,
                station_nm = p.station_nm,
                category_no = p.category_no,
                type_ds = p.type_ds,
                type_cd = p.type_cd,
                approver_uid = p.approver_uid,
                approved_by = p.approved_by,
                period_id = p.period_id,
                period = String.Format("{0:MM/dd/yyyy} - {1:MM/dd/yyyy}", p.period_beg_dt, p.period_end_dt),
                DaysSinceAging = p.DaysSinceAging
            }).ToList();
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
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;

                int rms_record_id = Convert.ToInt32(item.GetDataKeyValue("rms_record_id"));
                string lock_type = item["lock_type"].Text;
                string lock_uid = item["lock_uid"].Text;
                string lock_dt = item["lock_dt"].Text;
                string period_id = item["period_id"].Text;
                Image imgLockIcon = (Image)item.FindControl("imgLockIcon");
                HyperLink hlRecordType = (HyperLink)item.FindControl("hlRecordType");

                if (lock_type == "Analyze" || lock_type == "Approve")
                {
                    imgLockIcon.ImageUrl = "../images/lock_sm.png";
                    imgLockIcon.AlternateText = String.Format("Locked by {0}, on {1:MM/dd/yyyy}", lock_uid, lock_dt);
                }
                else if (lock_type == "Analyzing" || lock_type == "Approving")
                {
                    imgLockIcon.ImageUrl = "../images/save_sm.png";
                    imgLockIcon.AlternateText = String.Format("Saved by {0}, on {1:MM/dd/yyyy}", lock_uid, lock_dt);
                }
                else
                {
                    imgLockIcon.Visible = false;
                }

                if (lock_type == "Analyze" || lock_type == "Analyzing")
                    hlRecordType.Enabled = false;
                else
                    hlRecordType.NavigateUrl = String.Format("../Task/RecordProcess.aspx?period_id={0}&task=Approve", period_id);
            }
        }
        #endregion

        #region Approve Records RadGrid
        protected void rgApproveMyRecords_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            rgApproveMyRecords.DataSource = db.SP_RMS_Record_List_for_analyze_approve("approveByUser", user.ID, OfficeID).Select(p => new Data.RecordProcessDataItem
            {
                rms_record_id = p.rms_record_id,
                lock_dt = p.lock_dt,
                lock_type = p.lock_type,
                lock_uid = p.lock_uid,
                site_no = p.site_no,
                site_id = p.site_id,
                SIMSURL = Config.SIMSURL,
                station_nm = p.station_nm,
                category_no = p.category_no,
                type_ds = p.type_ds,
                type_cd = p.type_cd,
                approver_uid = p.approver_uid,
                approved_by = p.approved_by,
                period_id = p.period_id,
                period = String.Format("{0:MM/dd/yyyy} - {1:MM/dd/yyyy}", p.period_beg_dt, p.period_end_dt),
                DaysSinceAging = p.DaysSinceAging
            }).ToList();
        }

        protected void rgApproveMyRecords_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgApproveMyRecords.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" | menu.Items[i].Text == "Contains" | menu.Items[i].Text == "EqualTo" | menu.Items[i].Text == "DoesNotContain")
                    i = i + 1;
                else
                    menu.Items.RemoveAt(i);
            }
        }

        protected void rgApproveMyRecords_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                int rms_record_id = Convert.ToInt32(item.GetDataKeyValue("rms_record_id"));
                string lock_type = item["lock_type"].Text;
                string lock_uid = item["lock_uid"].Text;
                string lock_dt = item["lock_dt"].Text;
                string period_id = item["period_id"].Text;
                Image imgLockIcon = (Image)item.FindControl("imgLockIcon");
                HyperLink hlRecordType = (HyperLink)item.FindControl("hlRecordType");

                if (lock_type == "Analyze" || lock_type == "Approve")
                {
                    imgLockIcon.ImageUrl = "../images/lock_sm.png";
                    imgLockIcon.AlternateText = String.Format("Locked by {0}, on {1:MM/dd/yyyy}", lock_uid, lock_dt);
                }
                else if (lock_type == "Analyzing" || lock_type == "Approving")
                {
                    imgLockIcon.ImageUrl = "../images/save_sm.png";
                    imgLockIcon.AlternateText = String.Format("Saved by {0}, on {1:MM/dd/yyyy}", lock_uid, lock_dt);
                }
                else
                {
                    imgLockIcon.Visible = false;
                }

                if (lock_type == "Analyze" || lock_type == "Analyzing")
                    hlRecordType.Enabled = false;
                else
                    hlRecordType.NavigateUrl = String.Format("../Task/RecordProcess.aspx?period_id={0}&task=Approve", period_id);
            }
        }
        #endregion

        #region Audits Due RadGrid
        protected void rgAuditsDue_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            rgAuditsDue.DataSource = db.vRMSMostRecentAuditPeriods.Where(p => p.wsc_id == WSCID).Select(p => new
            {
                office_cd = p.office_cd,
                site_no = p.site_no,
                station_nm = p.station_nm,
                type_cd = p.type_cd,
                auditor_uid = p.auditor_uid,
                approved_dt = p.approved_dt,
                last_approved_period = p.last_approved_period,
                audit_beg_dt = p.audit_beg_dt,
                audit_end_dt = p.audit_end_dt,
                months_since_last = CalculateMonthsSinceLast(p.audit_end_dt),
                rms_record_id = p.rms_record_id
            }).ToList();
        }

        protected void rgAuditsDue_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgAuditsDue.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" | menu.Items[i].Text == "Contains" | menu.Items[i].Text == "EqualTo" | menu.Items[i].Text == "DoesNotContain")
                    i = i + 1;
                else
                    menu.Items.RemoveAt(i);
            }
        }

        protected void rgAuditsDue_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if ("Records".Equals(e.Item.OwnerTableView.Name))
            {
                if (e.Item is GridDataItem)
                {
                    GridDataItem item = (GridDataItem)e.Item;

                    int rms_record_id = Convert.ToInt32(item.GetDataKeyValue("rms_record_id"));
                    var recAudit = db.vRMSMostRecentAuditPeriods.FirstOrDefault(p => p.rms_record_id == rms_record_id);

                    HyperLink hlAuditPeriod = (HyperLink)item.FindControl("hlAuditPeriod");
                    Literal ltlMonthsSinceLast = (Literal)item.FindControl("ltlMonthsSinceLast");

                    if (recAudit.audit_beg_dt != null && recAudit.audit_end_dt != null)
                    {
                        hlAuditPeriod.Text = String.Format("{0:MM/dd/yyyy} - {1:MM/dd/yyyy}", recAudit.audit_beg_dt, recAudit.audit_end_dt);
                        hlAuditPeriod.NavigateUrl = String.Format("javascript:OpenPopup('../Modal/ViewAudit.aspx?rms_audit_id={0}&rms_record_id={1}')", recAudit.rms_audit_id, rms_record_id);

                        int monthsSinceLast = CalculateMonthsSinceLast(recAudit.audit_end_dt);
                        if (monthsSinceLast < 10)
                        {
                            ltlMonthsSinceLast.Text = String.Format("<span style='color:#196F3D;'>{0}</span>", monthsSinceLast);
                        }
                        else if (monthsSinceLast > 9 && monthsSinceLast < 13)
                        {
                            ltlMonthsSinceLast.Text = String.Format("<span style='color:#7DCEA0;'>{0}</span>", monthsSinceLast);
                        }
                        else if (monthsSinceLast > 12 && monthsSinceLast < 16)
                        {
                            ltlMonthsSinceLast.Text = String.Format("<span style='color:#CA6F1E;'>{0}</span>", monthsSinceLast);
                        }
                        else
                        {
                            ltlMonthsSinceLast.Text = String.Format("<span style='color:#E33813;'>{0}</span>", monthsSinceLast);
                        }
                    }
                    else
                    {
                        hlAuditPeriod.Text = "";
                        hlAuditPeriod.Enabled = false;

                        ltlMonthsSinceLast.Text = "N/A";
                    }
                }
            }
        }
        #endregion

    }
}