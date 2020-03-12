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
    public partial class Audit : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private Data.Record currRecord;
        private Data.Audit currAudit;
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
        private int AuditID
        {
            get
            {
                if (Session["AuditID"] == null) return 0; else return (int)Session["AuditID"];
            }
            set
            {
                Session["AuditID"] = value;
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
            string rms_record_id = Request.QueryString["rms_record_id"];
            string rms_audit_id = Request.QueryString["rms_audit_id"];
            string office_id = Request.QueryString["office_id"];

            if (!string.IsNullOrEmpty(office_id))
            {
                OfficeID = Convert.ToInt32(office_id);
                WSCID = Convert.ToInt32(db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id);
            }
            else
            {
                //If rms_record_id was passed, then set the RecordID variable and make sure the WSCID and OfficeID are changed to match the RecordIDs office and WSC
                if (!string.IsNullOrEmpty(rms_record_id))
                {
                    RecordID = Convert.ToInt32(rms_record_id);
                    currRecord = db.Records.Where(p => p.rms_record_id == RecordID).FirstOrDefault();
                    if (currRecord.RecordAltOffice == null) OfficeID = (int)currRecord.Site.office_id; else OfficeID = (int)currRecord.RecordAltOffice.alt_office_id;
                    WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;
                }
                else RecordID = 0;

                if (!string.IsNullOrEmpty(rms_audit_id))
                {
                    AuditID = Convert.ToInt32(rms_audit_id);
                    currAudit = db.Audits.Where(p => p.rms_audit_id == AuditID).FirstOrDefault();
                    WSCID = (int)currAudit.wsc_id;
                    OfficeID = db.Offices.FirstOrDefault(p => p.wsc_id == WSCID).office_id;
                }
                else AuditID = 0;
            }

            if (OfficeID == 0 || WSCID == 0)
            {
                OfficeID = user.OfficeID;
                WSCID = (int)db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id;
            }
            else if (OfficeID == 0 && WSCID > 0)
                OfficeID = db.Offices.FirstOrDefault(p => p.wsc_id == WSCID).office_id;
            else if (OfficeID > 0 && WSCID == 0)
                WSCID = (int)db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id;
              
            if (!Page.IsPostBack)
            {
                UserControlSetup();
            }
        }

        #region Page Load Events
        protected void UserControlSetup()
        {
            string wsc_nm = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).wsc_nm;
            //string wsc_nm = WSCID.ToString();
            ph1.Title = "Audit Periods Report";

            if (RecordID == 0)
            {
                ph1.SubTitle = "For the " + wsc_nm + " WSC";
                ph1.RecordType = "&nbsp;";
            }
            else
            {
                ph1.SubTitle = currRecord.Site.site_no + " " + db.vSITEFILEs.FirstOrDefault(s => s.site_id == currRecord.Site.nwisweb_site_id).station_nm;
                ph1.RecordType = currRecord.RecordType.type_ds + " Record for";
            }
        }
        #endregion

        #region Audits by Date RadGrid
        protected void rgAudits_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if (!e.IsFromDetailTable)
            {
                rgAudits.DataSource = db.Audits.Where(p => p.wsc_id == WSCID).Select(p => new {
                    rms_audit_id = p.rms_audit_id,
                    date_range = String.Format("{0:MM/dd/yyyy} - {1:MM/dd/yyyy}", p.audit_beg_dt, p.audit_end_dt),
                    audit_by = p.audit_by,
                    audit_type = p.AuditType.type,
                    audit_results = p.AuditResult.result
                }).ToList();
            }
        }

        protected void rgAudits_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        {
            GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;
            switch (e.DetailTableView.Name)
            {
                case "Records":
                    int rms_audit_id = Convert.ToInt32(dataItem.GetDataKeyValue("rms_audit_id"));
                    e.DetailTableView.DataSource = db.AuditRecords.Where(p => p.rms_audit_id == rms_audit_id).Select(p => new {
                        rms_audit_record_id = p.rms_audit_record_id,
                        rms_audit_id = p.rms_audit_id,
                        site_no = p.Record.Site.site_no,
                        station_nm = p.Record.Site.station_full_nm,
                        type_ds = p.Record.RecordType.type_ds
                    }).ToList();
                    break;
            }
        }

        protected void rgAudits_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgAudits.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" | menu.Items[i].Text == "Contains" | menu.Items[i].Text == "EqualTo" | menu.Items[i].Text == "DoesNotContain")
                    i = i + 1;
                else
                    menu.Items.RemoveAt(i);
            }
        }

        protected void rgAudits_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if ("Records".Equals(e.Item.OwnerTableView.Name))
            {
                if (e.Item is GridDataItem)
                {
                    GridDataItem item = (GridDataItem)e.Item;

                    int rms_audit_record_id = Convert.ToInt32(item.GetDataKeyValue("rms_audit_record_id"));
                    var audit_rec = db.AuditRecords.FirstOrDefault(p => p.rms_audit_record_id == rms_audit_record_id);

                    HyperLink hlAnalysisNotes = (HyperLink)item.FindControl("hlAnalysisNotes");
                    hlAnalysisNotes.NavigateUrl = String.Format("javascript:OpenPopup('Modal/ReportPopup.aspx?view=analysisnotesbydaterange&rms_record_id={0}&beg_dt={1:MM/dd/yyyy}&end_dt={2:MM/dd/yyyy}')", audit_rec.rms_record_id, audit_rec.Audit.audit_beg_dt, audit_rec.Audit.audit_end_dt);
                }
            }
            else
            {
                if (e.Item is GridDataItem)
                {
                    GridDataItem item = (GridDataItem)e.Item;

                    int rms_audit_id = Convert.ToInt32(item.GetDataKeyValue("rms_audit_id"));
                    var audit = db.Audits.FirstOrDefault(p => p.rms_audit_id == rms_audit_id);
                    HyperLink hlEditAudit = (HyperLink)item.FindControl("hlEditAudit");

                    hlEditAudit.NavigateUrl = String.Format("../Task/Audit.aspx?rms_audit_id={0}", rms_audit_id);
                }

                if (e.Item.IsInEditMode)
                {
                    GridEditableItem edititem = (GridEditableItem)e.Item;

                    int rms_audit_id = Convert.ToInt32(edititem.GetDataKeyValue("rms_audit_id"));
                    var audit = db.Audits.FirstOrDefault(p => p.rms_audit_id == rms_audit_id);

                    Literal ltlType = (Literal)edititem.FindControl("ltlAuditType");
                    Literal ltlResults = (Literal)edititem.FindControl("ltlAuditResults");
                    Literal ltlReason = (Literal)edititem.FindControl("ltlReason");
                    Literal ltlData = (Literal)edititem.FindControl("ltlData");
                    Literal ltlFindings = (Literal)edititem.FindControl("ltlFindings");
                    RadListView rlvAuditDocs = (RadListView)edititem.FindControl("rlvAuditDocs");

                    rlvAuditDocs.DataSource = audit.AuditDocuments.Select(p => new { rms_audit_document_id = p.rms_audit_document_id, document_nm = p.document_nm }).OrderBy(p => p.document_nm);
                    rlvAuditDocs.DataBind();

                    ltlType.Text = audit.AuditType.type + ": " + audit.AuditType.description;
                    ltlResults.Text = audit.AuditResult.result + audit.AuditResult.description;
                    ltlReason.Text = audit.audit_reason;
                    ltlData.Text = audit.audit_data;
                    ltlFindings.Text = audit.audit_findings;
                }
            }
        }
        #endregion

        #region Audits By Record RadGrid
        protected void rgAuditByRecord_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if (!e.IsFromDetailTable)
            {
                rgAuditByRecord.DataSource = db.vRMSAuditByRecords.Where(p => p.wsc_id == WSCID).Select(p => new
                {
                    rms_record_id = p.rms_record_id,
                    office_cd = p.office_cd,
                    site_no = p.site_no,
                    station_nm = p.station_full_nm,
                    type_ds = p.type_ds
                }).Distinct().ToList();
            }
        }

        protected void rgAuditByRecord_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        {
            GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;
            switch (e.DetailTableView.Name)
            {
                case "Audits":
                    int rms_record_id = Convert.ToInt32(dataItem.GetDataKeyValue("rms_record_id"));
                    e.DetailTableView.DataSource = db.AuditRecords.Where(p => p.rms_record_id == rms_record_id).Select(p => new {
                        rms_record_id = rms_record_id,
                        rms_audit_id = p.rms_audit_id,
                        date_range = String.Format("{0:MM/dd/yyyy} - {1:MM/dd/yyyy}", p.Audit.audit_beg_dt, p.Audit.audit_end_dt),
                        audit_by = p.Audit.audit_by,
                        audit_type = p.Audit.AuditType.type,
                        audit_results = p.Audit.AuditResult.result
                    }).ToList();
                    break;
            }
        }

        protected void rgAuditByRecord_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgAuditByRecord.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" | menu.Items[i].Text == "Contains" | menu.Items[i].Text == "EqualTo" | menu.Items[i].Text == "DoesNotContain")
                    i = i + 1;
                else
                    menu.Items.RemoveAt(i);
            }
        }

        protected void rgAuditByRecord_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if ("Audits".Equals(e.Item.OwnerTableView.Name))
            {
                if (e.Item is GridDataItem)
                {
                    GridDataItem item = (GridDataItem)e.Item;
                    int rms_record_id = Convert.ToInt32(item.OwnerTableView.DataKeyValues[item.ItemIndex]["rms_record_id"]);
                    int rms_audit_id = Convert.ToInt32(item.GetDataKeyValue("rms_audit_id"));
                    var audit = db.Audits.FirstOrDefault(p => p.rms_audit_id == rms_audit_id);

                    HyperLink hlAnalysisNotes = (HyperLink)item.FindControl("hlAnalysisNotes");
                    hlAnalysisNotes.NavigateUrl = String.Format("javascript:OpenPopup('../Modal/ReportPopup.aspx?view=analysisnotesbydaterange&rms_record_id={0}&beg_dt={1:MM/dd/yyyy}&end_dt={2:MM/dd/yyyy}')", rms_record_id, audit.audit_beg_dt, audit.audit_end_dt);

                    HyperLink hlEditAudit = (HyperLink)item.FindControl("hlEditAudit");
                    hlEditAudit.NavigateUrl = String.Format("../Task/Audit.aspx?rms_audit_id={0}", rms_audit_id);
                }

                if (e.Item.IsInEditMode)
                {
                    GridEditableItem edititem = (GridEditableItem)e.Item;

                    int rms_audit_id = Convert.ToInt32(edititem.GetDataKeyValue("rms_audit_id"));
                    var audit = db.Audits.FirstOrDefault(p => p.rms_audit_id == rms_audit_id);

                    Literal ltlType = (Literal)edititem.FindControl("ltlAuditType");
                    Literal ltlResults = (Literal)edititem.FindControl("ltlAuditResults");
                    Literal ltlReason = (Literal)edititem.FindControl("ltlReason");
                    Literal ltlData = (Literal)edititem.FindControl("ltlData");
                    Literal ltlFindings = (Literal)edititem.FindControl("ltlFindings");
                    RadListView rlvAuditDocs = (RadListView)edititem.FindControl("rlvAuditDocs");

                    rlvAuditDocs.DataSource = audit.AuditDocuments.Select(p => new { rms_audit_document_id = p.rms_audit_document_id, document_nm = p.document_nm }).OrderBy(p => p.document_nm);
                    rlvAuditDocs.DataBind();

                    ltlType.Text = audit.AuditType.type + ": " + audit.AuditType.description;
                    ltlResults.Text = audit.AuditResult.result + audit.AuditResult.description;
                    ltlReason.Text = audit.audit_reason;
                    ltlData.Text = audit.audit_data;
                    ltlFindings.Text = audit.audit_findings;
                }
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
                audit_beg_dt = p.audit_beg_dt,
                audit_end_dt = p.audit_end_dt,
                months_since_last = CalculateMonthsSinceLast(p.audit_end_dt),
                rms_record_id = p.rms_record_id,
                audit_by = p.audit_by,
                audit_dt = p.audit_dt,
                audit_type = p.type,
                result = p.result,
                description = p.description
            }).ToList();
        }

        protected void rgAuditsDue_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgAuditsDue.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" | menu.Items[i].Text == "Contains" | menu.Items[i].Text == "EqualTo" | menu.Items[i].Text == "DoesNotContain" | menu.Items[i].Text == "LessThan" | menu.Items[i].Text == "GreaterThan")
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
                        hlAuditPeriod.Text = String.Format("{0:MM/dd/yy} - {1:MM/dd/yy}", recAudit.audit_beg_dt, recAudit.audit_end_dt);
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

        #region Page Methods
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

    }
}