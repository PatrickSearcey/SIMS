using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace SIMS2017.Services
{
    /// <summary>
    /// Summary description for AuditHistory
    /// </summary>
    public class AuditHistory : IHttpHandler
    {
        private Data.SIMSDataContext db = new Data.SIMSDataContext();

        public void ProcessRequest(HttpContext context)
        {
            string wsc_cd = context.Request["wsc_cd"];
            string site_no = context.Request["site_no"];

            string json = GetAuditHistoryJSON(wsc_cd, site_no);
            context.Response.ContentType = "text/json";
            context.Response.Write(json);
        }

        private string GetAuditHistoryJSON(string wsc_cd, string site_no)
        {
            List<AuditItem> audits = new List<AuditItem>();

            if (string.IsNullOrEmpty(wsc_cd) && string.IsNullOrEmpty(site_no))
            {
                audits = db.vAuditHistories.Select(p => new AuditItem
                {
                    WSCID = p.wsc_id,
                    WSCCode = p.wsc_cd,
                    SiteWSC = p.site_wsc,
                    OfficeCode = p.office_cd,
                    OfficeName = p.office_nm,
                    AgencyCode = p.agency_cd,
                    SiteNo = p.site_no,
                    StationName = p.station_nm,
                    SiteTypeCode = p.site_tp_cd,
                    TypeCode = p.type_cd,
                    TypeDesc = p.type_ds,
                    CategoryNo = p.category_no,
                    Timeseries = p.Timeseries,
                    ActiveRecordType = p.active_record_type,
                    AssignedAuditor = p.assigned_auditor,
                    AuditDate = p.audit_dt,
                    AuditBy = p.audit_by,
                    AuditBegDate = p.audit_beg_dt,
                    AuditEndDate = p.audit_end_dt,
                    Type = p.type,
                    Description = p.description,
                    Result = p.result,
                    AuditURL = string.Format("{0}Modal/ViewAudit.aspx?rms_audit_id={1}", Core.Config.RMSURL, p.rms_audit_id)
                }).OrderBy(p => p.SiteWSC).ThenBy(p => p.OfficeName).ThenBy(p => p.SiteNo).ThenBy(p => p.TypeCode).ThenBy(p => p.AuditDate).ToList();
            }
            else if (!string.IsNullOrEmpty(wsc_cd))
            {
                audits = db.vAuditHistories.Where(p => p.wsc_cd == wsc_cd).Select(p => new AuditItem
                {
                    WSCID = p.wsc_id,
                    WSCCode = p.wsc_cd,
                    SiteWSC = p.site_wsc,
                    OfficeCode = p.office_cd,
                    OfficeName = p.office_nm,
                    AgencyCode = p.agency_cd,
                    SiteNo = p.site_no,
                    StationName = p.station_nm,
                    SiteTypeCode = p.site_tp_cd,
                    TypeCode = p.type_cd,
                    TypeDesc = p.type_ds,
                    CategoryNo = p.category_no,
                    Timeseries = p.Timeseries,
                    ActiveRecordType = p.active_record_type,
                    AssignedAuditor = p.assigned_auditor,
                    AuditDate = p.audit_dt,
                    AuditBy = p.audit_by,
                    AuditBegDate = p.audit_beg_dt,
                    AuditEndDate = p.audit_end_dt,
                    Type = p.type,
                    Description = p.description,
                    Result = p.result,
                    AuditURL = string.Format("{0}Modal/ViewAudit.aspx?rms_audit_id={1}", Core.Config.RMSURL, p.rms_audit_id)
                }).OrderBy(p => p.SiteWSC).ThenBy(p => p.OfficeName).ThenBy(p => p.SiteNo).ThenBy(p => p.TypeCode).ThenBy(p => p.AuditDate).ToList();
            }
            else if (!string.IsNullOrEmpty(site_no))
            {
                audits = db.vAuditHistories.Where(p => p.site_no == site_no).Select(p => new AuditItem
                {
                    WSCID = p.wsc_id,
                    WSCCode = p.wsc_cd,
                    SiteWSC = p.site_wsc,
                    OfficeCode = p.office_cd,
                    OfficeName = p.office_nm,
                    AgencyCode = p.agency_cd,
                    SiteNo = p.site_no,
                    StationName = p.station_nm,
                    SiteTypeCode = p.site_tp_cd,
                    TypeCode = p.type_cd,
                    TypeDesc = p.type_ds,
                    CategoryNo = p.category_no,
                    Timeseries = p.Timeseries,
                    ActiveRecordType = p.active_record_type,
                    AssignedAuditor = p.assigned_auditor,
                    AuditDate = p.audit_dt,
                    AuditBy = p.audit_by,
                    AuditBegDate = p.audit_beg_dt,
                    AuditEndDate = p.audit_end_dt,
                    Type = p.type,
                    Description = p.description,
                    Result = p.result,
                    AuditURL = string.Format("{0}Modal/ViewAudit.aspx?rms_audit_id={1}", Core.Config.RMSURL, p.rms_audit_id)
                }).OrderBy(p => p.SiteWSC).ThenBy(p => p.OfficeName).ThenBy(p => p.SiteNo).ThenBy(p => p.TypeCode).ThenBy(p => p.AuditDate).ToList();
            }

            
            return (new JavaScriptSerializer().Serialize(audits));
        }

        internal class AuditItem
        {
            private int? _wsc_id;
            private string _wsc_cd;
            private string _site_wsc;
            private string _office_cd;
            private string _office_nm;
            private string _agency_cd;
            private string _site_no;
            private string _station_nm;
            private string _site_tp_cd;
            private string _type_cd;
            private string _type_ds;
            private int? _category_no;
            private string _timeseries;
            private string _active_record_type;
            private string _assigned_auditor;
            private string _audit_dt;
            private string _audit_by;
            private string _audit_beg_dt;
            private string _audit_end_dt;
            private string _type;
            private string _description;
            private string _result;
            private string _auditURL;

            public int? WSCID
            {
                get { return _wsc_id; }
                set { _wsc_id = value; }
            }
            public string WSCCode
            {
                get { return _wsc_cd; }
                set { _wsc_cd = value; }
            }
            public string SiteWSC
            {
                get { return _site_wsc; }
                set { _site_wsc = value; }
            }
            public string OfficeCode
            {
                get { return _office_cd; }
                set { _office_cd = value; }
            }
            public string OfficeName
            {
                get { return _office_nm; }
                set { _office_nm = value; }
            }
            public string AgencyCode
            {
                get { return _agency_cd; }
                set { _agency_cd = value; }
            }
            public string SiteNo
            {
                get { return _site_no; }
                set { _site_no = value; }
            }
            public string StationName
            {
                get { return _station_nm; }
                set { _station_nm = value; }
            }
            public string SiteTypeCode
            {
                get { return _site_tp_cd; }
                set { _site_tp_cd = value; }
            }
            public string TypeCode
            {
                get { return _type_cd; }
                set { _type_cd = value; }
            }
            public string TypeDesc
            {
                get { return _type_ds; }
                set { _type_ds = value; }
            }
            public int? CategoryNo
            {
                get { return _category_no; }
                set { _category_no = value; }
            }
            public string Timeseries
            {
                get { return _timeseries; }
                set { _timeseries = value; }
            }
            public string ActiveRecordType
            {
                get { return _active_record_type; }
                set { _active_record_type = value; }
            }
            public string AssignedAuditor
            {
                get { return _assigned_auditor; }
                set { _assigned_auditor = value; }
            }
            public string AuditDate
            {
                get { return _audit_dt; }
                set { _audit_dt = value; }
            }
            public string AuditBy
            {
                get { return _audit_by; }
                set { _audit_by = value; }
            }
            public string AuditBegDate
            {
                get { return _audit_beg_dt; }
                set { _audit_beg_dt = value; }
            }
            public string AuditEndDate
            {
                get { return _audit_end_dt; }
                set { _audit_end_dt = value; }
            }
            public string Type
            {
                get { return _type; }
                set { _type = value; }
            }
            public string Description
            {
                get { return _description; }
                set { _description = value; }
            }
            public string Result
            {
                get { return _result; }
                set { _result = value; }
            }
            public string AuditURL
            {
                get { return _auditURL; }
                set { _auditURL = value; }
            }
            public AuditItem()
            {
                _wsc_id = WSCID;
                _wsc_cd = WSCCode;
                _site_wsc = SiteWSC;
                _office_cd = OfficeCode;
                _agency_cd = AgencyCode;
                _site_no = SiteNo;
                _station_nm = StationName;
                _site_tp_cd = SiteTypeCode;
                _type_cd = TypeCode;
                _type_ds = TypeDesc;
                _category_no = CategoryNo;
                _timeseries = Timeseries;
                _active_record_type = ActiveRecordType;
                _assigned_auditor = AssignedAuditor;
                _audit_dt = AuditDate;
                _audit_by = AuditBy;
                _audit_beg_dt = AuditBegDate;
                _audit_end_dt = AuditEndDate;
                _type = Type;
                _description = Description;
                _result = Result;
                _auditURL = AuditURL;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}