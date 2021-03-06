﻿using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace RMS.Modal
{
    public partial class ViewAudit : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private Data.RecordAnalysisPeriod currPeriod { get; set; }
        private Data.Record currRecord { get; set; }
        private Data.Audit currAudit { get; set; }
        private int PeriodID { get; set; }
        private int AuditID { get; set; }
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
            string period_id = Request.QueryString["period_id"];
            string rms_audit_id = Request.QueryString["rms_audit_id"];
            string rms_record_id = Request.QueryString["rms_record_id"];

            if (!string.IsNullOrEmpty(period_id))
            {
                PeriodID = Convert.ToInt32(period_id);
                currPeriod = db.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == PeriodID);
                currRecord = db.Records.FirstOrDefault(p => p.rms_record_id == currPeriod.rms_record_id);

                ltlSite.Text = currRecord.Site.site_no + " " + db.vSITEFILEs.FirstOrDefault(s => s.site_id == currRecord.Site.nwisweb_site_id).station_nm;
                ltlRecord.Text = currRecord.RecordType.type_ds + " Record";

                pnlAudits.Visible = true;
                pnlError.Visible = false;
                PopulateAuditDetails();
            }
            else if (!string.IsNullOrEmpty(rms_audit_id) && !string.IsNullOrEmpty(rms_record_id))
            {
                AuditID = Convert.ToInt32(rms_audit_id);
                currAudit = db.Audits.FirstOrDefault(p => p.rms_audit_id == AuditID);
                currRecord = db.Records.FirstOrDefault(p => p.rms_record_id == Convert.ToInt32(rms_record_id));

                ltlSite.Text = currRecord.Site.site_no + " " + db.vSITEFILEs.FirstOrDefault(s => s.site_id == currRecord.Site.nwisweb_site_id).station_nm;
                ltlRecord.Text = currRecord.RecordType.type_ds + " Record";

                pnlAudits.Visible = true;
                pnlError.Visible = false;
                PopulateAuditDetails();
            }
            else
            {
                pnlAudits.Visible = false;
                pnlError.Visible = true;
            }
        }

        protected void PopulateAuditDetails()
        {
            if (currPeriod != null)
            {
                var auditsByPeriodID = db.Audits.Where(p =>
                (p.audit_beg_dt == currPeriod.period_beg_dt || p.audit_beg_dt < currPeriod.period_beg_dt) &&
                (p.audit_end_dt == currPeriod.period_end_dt || p.audit_end_dt > currPeriod.period_end_dt) &&
                (p.AuditRecords.Select(a => a.rms_record_id).Contains(currRecord.rms_record_id)))
                .Select(p => new
                {
                    DateRange = String.Format("{0:MM/dd/yyyy} - {1:MM/dd/yyyy}", p.audit_beg_dt, p.audit_end_dt),
                    audit_by = p.audit_by,
                    audit_dt = String.Format("{0:MM/dd/yyyy}", p.audit_dt),
                    AuditType = p.AuditType.type + ": " + p.AuditType.description,
                    AuditResults = p.AuditResult.result + p.AuditResult.description,
                    audit_reason = p.audit_reason,
                    audit_data = p.audit_data,
                    audit_findings = p.audit_findings,
                    rms_audit_id = p.rms_audit_id
                }).ToList();

                dlAudits.DataSource = auditsByPeriodID;
                dlAudits.DataBind();
            }
            else if (currAudit != null)
            {
                var auditsByAuditID = db.Audits.Where(p => p.rms_audit_id == currAudit.rms_audit_id)
                .Select(p => new
                {
                    DateRange = String.Format("{0:MM/dd/yyyy} - {1:MM/dd/yyyy}", p.audit_beg_dt, p.audit_end_dt),
                    audit_by = p.audit_by,
                    audit_dt = String.Format("{0:MM/dd/yyyy}", p.audit_dt),
                    AuditType = p.AuditType.type + ": " + p.AuditType.description,
                    AuditResults = p.AuditResult.result + p.AuditResult.description,
                    audit_reason = p.audit_reason,
                    audit_data = p.audit_data,
                    audit_findings = p.audit_findings,
                    rms_audit_id = p.rms_audit_id
                }).ToList();

                dlAudits.DataSource = auditsByAuditID;
                dlAudits.DataBind();
            }
            
        }

        protected void dlAudits_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                int rms_audit_id = Convert.ToInt32(dlAudits.DataKeys[e.Item.ItemIndex]);
                RadListView rlv = (RadListView)e.Item.FindControl("rlvAuditDocs");

                rlv.DataSource = db.AuditDocuments.Where(p => p.rms_audit_id == rms_audit_id).Select(p => new { rms_audit_document_id = p.rms_audit_document_id, document_nm = p.document_nm }).ToList();
                rlv.DataBind();
            }
        }
    }
}