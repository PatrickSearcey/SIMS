using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SIMS2017
{
    public partial class StationInfo : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        public Boolean HasEditAccess { get; set; }
        private Data.Site currSite;
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
        private int SiteID
        {
            get
            {
                if (Session["SiteID"] == null) return 0; else return (int)Session["SiteID"];
            }
            set
            {
                Session["SiteID"] = value;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            //If no site_id or site_no was passed, then redirect back to the homepage
            string site_id = Request.QueryString["site_id"];
            string site_no = Request.QueryString["site_no"];
            if (!string.IsNullOrEmpty(site_id))
                SiteID = Convert.ToInt32(site_id);
            else if (!string.IsNullOrEmpty(site_no))
                SiteID = db.Sites.FirstOrDefault(p => p.site_no == site_no).site_id;
            else 
                Response.Redirect(Config.SIMSURL + "SIMSWSCHome.aspx");
            
            //Using the passed site_id, setup the site data element, and reset the office and wsc to match that of the current site
            currSite = db.Sites.Where(p => p.site_id == SiteID).FirstOrDefault();
            OfficeID = (int)currSite.office_id;
            WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            ph1.Title = "Station Information Page";
            ph1.SubTitle = currSite.site_no + " " + db.vSITEFILEs.FirstOrDefault(p => p.site_id == currSite.nwisweb_site_id).station_nm;
            ph1.ShowOfficeInfoPanel = true;

            if (!Page.IsPostBack)
            {
                //If the user belongs to this site's WSC (or has an exception to work in the WSC), or is a SuperUser, then allow them to edit the page
                if (user.WSCID.Contains(WSCID) || user.IsSuperUser) HasEditAccess = true; 

                PopulatePageData();
                SetupPermission();
            }
        }

        #region Page Load Methods
        protected void PopulatePageData()
        {
            //Page Top
            hlNWISWeb.NavigateUrl = String.Format("http://waterdata.usgs.gov/nwis/inventory/?site_no={0}&agency_cd={1}", currSite.site_no.Trim(), currSite.agency_cd);
            hlNWISOpsRequest.NavigateUrl = String.Format("{0}NWISOpsRequest.aspx?office_id={1}&site_id={2}", Config.SIMSURL, OfficeID, SiteID);

            //Station Details
            ltlPubName.Text = currSite.station_full_nm;
            rtbPubName.Text = currSite.station_full_nm;
            ltlOffice.Text = currSite.Office.office_nm;
            rddlOffice.DataSource = db.Offices.Where(p => p.wsc_id == WSCID).Select(p => new { office_id = p.office_id, office_nm = p.office_nm }).ToList();
            rddlOffice.DataBind();
            rddlOffice.SelectedValue = OfficeID.ToString();
            string fieldtrips = "";
            foreach (var trip in currSite.TripSites.ToList())
            {
                fieldtrips += trip.Trip.trip_nm + " - " + trip.Trip.user_id + ", ";
            }
            if (!string.IsNullOrEmpty(fieldtrips))
            {
                ltlFieldTrip.Text = fieldtrips.TrimEnd(' ').TrimEnd(',');
                //hlMapTrips.NavigateUrl = String.Format("{0}fieldtripmap.aspx?office_id={1}&trip_id={2}&wsc_id={3}", Config.SIMSURL, OfficeID, currSite.TripSites.FirstOrDefault().trip_id, WSCID);
                hlMapTrips.Visible = false;
            }
            else
            {
                ltlFieldTrip.Text = "<i>none assigned</i>";
                hlMapTrips.Visible = false;
            }
            lbEditFieldTrip.OnClientClick = String.Format("openWin('{0}','field trip'); return false;", currSite.site_id);
            hlSiFTA.NavigateUrl = "http://sifta.water.usgs.gov/NationalFunding/Site.aspx?SiteNumber=" + currSite.site_no.Trim();

            //Station Documents
            hlEditDocs.NavigateUrl = String.Format("{0}StationDoc/EditDocs.aspx?site_id={1}", Config.SIMSURL, currSite.site_id);
            hlSDESC.NavigateUrl = String.Format("{0}StationDoc/ViewDocs.aspx?site_id={1}&type=SDESC", Config.SIMSURL, currSite.site_id);
            hlMANU.NavigateUrl = String.Format("{0}StationDoc/ViewDocs.aspx?site_id={1}&type=MANU", Config.SIMSURL, currSite.site_id);
            hlSANAL.NavigateUrl = String.Format("{0}StationDoc/ViewDocs.aspx?site_id={1}&type=SANAL", Config.SIMSURL, currSite.site_id);
            hlCustomReport.NavigateUrl = String.Format("{0}StationDoc/ViewDocs.aspx?site_id={1}&type=Custom", Config.SIMSURL, currSite.site_id);
            hlArchives.NavigateUrl = String.Format("{0}StationDoc/Archive.aspx?site_id={1}", Config.SIMSURL, currSite.site_id);
            hlSLAP.NavigateUrl = String.Format("{0}HistoricObjectSum.aspx?site_no={1}&agency_cd={2}", Config.SLAPURL, currSite.site_no.Trim(), currSite.agency_cd);

            //Make the determination of what to show for the MANU approval link
            var manuSite = db.vSITEFILEs.FirstOrDefault(p => p.site_id == currSite.nwisweb_site_id && p.data_types_cd.EndsWith("NI") || p.site_id == currSite.nwisweb_site_id && p.data_types_cd.EndsWith("NA"));
            if (manuSite != null)
            {
                var manuRecord = db.Records.FirstOrDefault(p => p.site_id == currSite.site_id && p.RecordType.ts_fg == true);
                if (manuRecord != null)
                {
                    var manuApproved = db.ElemReportApproves.FirstOrDefault(p => p.site_id == currSite.site_id);
                    if (manuApproved != null)
                    {
                        DateTime approved_dt = Convert.ToDateTime(manuApproved.approved_dt);
                        DateTime? revised_dt = null;
                        if (db.ElemReportSums.FirstOrDefault(p => p.site_id == currSite.site_id && p.report_type_cd == "MANU") != null) 
                            revised_dt = db.ElemReportSums.FirstOrDefault(p => p.site_id == currSite.site_id && p.report_type_cd == "MANU").revised_dt;
                        if (revised_dt > approved_dt) ltlApproved.Text = String.Format("(<a href='{0}StationDoc/SiteMAI.aspx?site_id={1}' style='color:red;'>Needs Approval</a>)", Config.SIMSURL, currSite.site_id); else ltlApproved.Text = String.Format("(<a href='{0}StationDoc/SiteMAI.aspx?site_id={1}' style='color:green'>Approved</a>)", Config.SIMSURL, currSite.site_id);
                    }
                    else ltlApproved.Text = String.Format("(<a href='{0}StationDoc/SiteMAI.aspx?site_id={1}'>Needs Approval</a>)", Config.SIMSURL, currSite.site_id);
                }
                else ltlApproved.Visible = false;
            }
            else ltlApproved.Visible = false;

            //CRP
            var record = currSite.Records;
            dlRecords.DataSource = record.Select(p => new RecordItem
            {
                rms_record_id = p.rms_record_id,
                personnel = GetPersonnelAssignments(p.operator_uid, p.analyzer_uid, p.approver_uid, p.auditor_uid),
                published = GetPublished(Convert.ToBoolean(p.not_published_fg)),
                active = GetActive(Convert.ToBoolean(p.not_used_fg)),
                cat_no = GetCategory(Convert.ToInt32(p.category_no)),
                office_cd = GetOfficeCode(p),
                time_series = GetTimeSeries(p),
                type_ds = GetRecordType(p.RecordType.type_ds),
                RMSURL = Config.RMSURL
            }).ToList();
            dlRecords.DataBind();
            string swr_url = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).swr_url;
            if (!string.IsNullOrEmpty(swr_url))
            {
                hlAutoReview.NavigateUrl = String.Format("javascript:OpenSWR('{0}{1}/')", swr_url, currSite.site_no.Replace(" ", ""));
            }
            else hlAutoReview.Visible = false;
            lbNewRecordType.OnClientClick = String.Format("openWin('{0}','newrecord'); return false;", currSite.site_id);

            //Safety
            hlSHATutorial.NavigateUrl = String.Format("{0}SIMSShare/SIMS_SHA_Tutorial.pptx", Config.SIMSServerURL);
            hlTCPTutorial.NavigateUrl = String.Format("{0}SIMSShare/SIMS_TCP_Software_Training.pptx", Config.SIMSServerURL);
            var SHA = currSite.SHAs.FirstOrDefault();
            if (SHA != null)
            {
                pnlSHACreate.Visible = false;
                pnlSHAEdit.Visible = true;
                hlSHAEdit.NavigateUrl = String.Format("{0}SHAEdit.aspx?site_id={1}", Config.SafetyURL, currSite.site_id);
                hlSHAPrintVersion.NavigateUrl = String.Format("{0}SHAView.aspx?site_id={1}", Config.SafetyURL, currSite.site_id);
                string approved_dt = String.Format("{0:MM/dd/yyyy}", SHA.approved_dt);
                if (string.IsNullOrEmpty(approved_dt)) ltlSHAApproved.Text = "last approved: <i>never approved</i>";
                else
                {
                    ltlSHAApproved.Text = "last approved by: " + SHA.approved_by + ", date: " + approved_dt;
                }
                string reviewed_dt = String.Format("{0:MM/dd/yyyy}", SHA.reviewed_dt);
                if (string.IsNullOrEmpty(reviewed_dt)) ltlSHAReviewed.Text = "last reviewed: <i>never reviewed</i>";
                else
                {
                    ltlSHAReviewed.Text = "last reviewed by: " + SHA.reviewed_by + ", date: " + reviewed_dt;
                }
            }
            else
            {
                pnlSHACreate.Visible = true;
                pnlSHAEdit.Visible = false;
                hlSHACreate.NavigateUrl = String.Format("{0}Handler/CreateSHA.ashx?site_id={1}", Config.SafetyURL, currSite.site_id);
            }
            var TCPSite = currSite.TCPSite;
            if (TCPSite != null)
            {
                var TCP = TCPSite.TCPs.FirstOrDefault();
                if (TCP != null)
                {
                    pnlTCPCreate.Visible = false;
                    pnlTCPEdit.Visible = true;
                    hlTCPEdit.NavigateUrl = String.Format("{0}TCPEdit.aspx?site_id={1}", Config.SafetyURL, currSite.site_id);
                    dlTCPs.DataSource = currSite.TCPSite.TCPs.Select(p => new TCPItem
                    {
                        TCPID = p.TCPID,
                        TCPName = String.Format("{0} - TCP, {1}", p.TCPPlanDetail.Number, p.TCPPlanDetail.SubName),
                        TCPURL = String.Format("{0}TCPView.aspx?TCPID={1}", Config.SafetyURL, p.TCPID),
                        LastApprovedDt = TCPApprovedDate(p.ApprovedDt),
                        ApprovalReady = Convert.ToBoolean(p.ApprovalReady)
                    });
                    dlTCPs.DataBind();
                    hlTCPTrackStatus.NavigateUrl = String.Format("{0}TCPReport.aspx?office_id={1}", Config.SafetyURL, OfficeID);
                }
                else
                {
                    pnlTCPCreate.Visible = true;
                    pnlTCPEdit.Visible = false;
                    hlTCPCreate.NavigateUrl = String.Format("{0}TCPEdit.aspx?site_id={1}", Config.SafetyURL, currSite.site_id);
                }
            }
            else
            {
                pnlTCPCreate.Visible = true;
                pnlTCPEdit.Visible = false;
                hlTCPCreate.NavigateUrl = String.Format("{0}TCPEdit.aspx?site_id={1}", Config.SafetyURL, currSite.site_id);
            }

            //DCP/Realtime Ops
            var di = db.spz_GetDCPInfo(currSite.site_id).FirstOrDefault();
            var im = db.spz_GetIMEIInfo(currSite.site_id).FirstOrDefault();
            if (di != null || im != null)
            {
                rtsTelemetry.Visible = true;
                rmp.Visible = true;
                ltlNoTelemetry.Visible = false;

                if (di != null)
                {
                    pnlDCPTable.Visible = true;
                    ltlNoDCP.Visible = false;

                    ltlDCPOfficeTime.Text = String.Format("{0:dddd, MMMM dd, yyyy} ({1}), {0:h:mm:ss tt}", di.doffice, di.jdoffice.Substring(4));
                    ltlDCPSiteTime.Text = String.Format("{0:dddd, MMMM dd, yyyy} ({1}), {0:h:mm:ss tt}", di.dsite, di.jdsite.Substring(4));
                    ltlDCPGMTTime.Text = String.Format("{0:dddd, MMMM dd, yyyy} ({1}), {0:h:mm:ss tt}", di.dutc, di.jdutc.Substring(4));

                    dlDCPTable.DataSource = db.spz_GetDCPInfo(currSite.site_id).Select(p => new
                    {
                        MinutesToNext = NextTransmitTime(Convert.ToDateTime(p.doffice), p.officedcptimes, "next"),
                        dcp_id = p.dcp_id,
                        primary_ch = p.primary_ch,
                        random_ch = p.random_ch,
                        primary_bd = p.primary_bd,
                        random_bd = p.random_bd,
                        ant_azimuth = p.ant_azimuth,
                        satellite = p.satellite,
                        ant_elev = p.ant_elev,
                        assigned_time = p.assigned_time,
                        trans_interval = p.trans_interval,
                        window = p.window,
                        PASSURL = Config.PASSURL
                    });
                    dlDCPTable.DataBind();
                }
                else
                {
                    pnlDCPTable.Visible = false;
                    ltlNoDCP.Text = "No Package Contents";
                }

                if (im != null)
                {
                    pnlIMEITable.Visible = true;
                    ltlNoIMEI.Visible = false;

                    try
                    {
                        ltlDCPOfficeTime2.Text = String.Format("{0:dddd, MMMM dd, yyyy} ({1}), {0:h:mm:ss tt}", di.doffice, di.jdoffice.Substring(4));
                        ltlDCPSiteTime2.Text = String.Format("{0:dddd, MMMM dd, yyyy} ({1}), {0:h:mm:ss tt}", di.dsite, di.jdsite.Substring(4));
                        ltlDCPGMTTime2.Text = String.Format("{0:dddd, MMMM dd, yyyy} ({1}), {0:h:mm:ss tt}", di.dutc, di.jdutc.Substring(4));
                    }
                    catch (Exception ex)
                    {
                        ltlDCPOfficeTime2.Text = "<i>unknown</i>";
                        ltlDCPSiteTime2.Text = "<i>unknown</i>";
                        ltlDCPGMTTime2.Text = "<i>unknown</i>";
                    }

                    hlPASSURL.NavigateUrl = Config.PASSURL;

                    dlIMEITable.DataSource = db.spz_GetIMEIInfo(currSite.site_id).Select(p => new
                    {
                        IDType = p.IDType,
                        IMEI = p.IMEI,
                        TransmitInterval = p.TransmitInterval,
                        MobileNo = p.MobileNo == "--" ? "" : "<b>Mobile Number:</b><br />" + p.MobileNo
                    });
                    dlIMEITable.DataBind();
                }
                else
                {
                    pnlIMEITable.Visible = false;
                    ltlNoIMEI.Text = "No Iridium/Cellular Contents";
                }
                
            }
            else
            {
                rtsTelemetry.Visible = false;
                rmp.Visible = false;
                ltlNoTelemetry.Text = "No Telemetry Information";
            }
        }

        private string GetPersonnelAssignments(string operator_uid, string analyzer, string approver, string auditor)
        {
            string ret = "<i>unassigned</i>";

            if (!string.IsNullOrEmpty(operator_uid) || !string.IsNullOrEmpty(analyzer) || !string.IsNullOrEmpty(approver) || !string.IsNullOrEmpty(auditor))
            {
                string op;
                string az;
                string ap;
                string au;
                if (!string.IsNullOrEmpty(operator_uid)) op = operator_uid; else op = "<i>unassigned</i>";
                if (!string.IsNullOrEmpty(analyzer)) az = analyzer; else az = "<i>unassigned</i>";
                if (!string.IsNullOrEmpty(approver)) ap = approver; else ap = "<i>unassigned</i>";
                if (!string.IsNullOrEmpty(auditor)) au = auditor; else au = "<i>unassigned</i>";
                ret = op + "/" + az + "/" + ap + "/" + au;
            }

            return ret;
        }

        private string GetPublished(bool published)
        {
            string ret = "published";

            if (published) ret = "not published";

            return ret;
        }

        private string GetActive(bool active)
        {
            string ret = "active";

            if (active) ret = "inactive";

            return ret;
        }

        private string GetCategory(int catno)
        {
            string ret = "";

            if (catno > 0)
                ret = "CRP Category: <b>" + catno.ToString() + "</b>";

            return ret;
        }

        private string GetRecordType(string type_ds)
        {
            string ret = type_ds;

            if (!string.IsNullOrEmpty(type_ds))
            {
                if (type_ds.Length > 24)
                    ret = type_ds.Substring(0, 25) + "...";
            }
            
            return ret;
        }

        private string GetTimeSeries(Data.Record record)
        {
            string ret = "";
            
            if (Convert.ToBoolean(record.RecordType.ts_fg))
                ret = "yes";
            else
                ret = "no";

            var DDs = db.SP_RMS_Get_Record_DDs(record.rms_record_id).ToList();
            List<string> parm_cds = new List<string>();

            if (DDs.Count > 0)
            {
                foreach (var dd in DDs)
                {
                    if (dd.parameter_cd != null)
                    {
                        parm_cds.Add("<a href='http://ts.nwis.usgs.gov/AQUARIUS/Publish/v2/GetApprovalsTransactionList?TimeSeriesUniqueId=" + dd.gu_id +
                            "' target='_blank'>" + dd.parameter_cd.ToString() + "</a>");
                    }
                }

                if (parm_cds.Count > 0)
                {
                    string ts_ids = "";
                    foreach (string tsid in parm_cds)
                        ts_ids += tsid + ", ";
                    ret += " (ts parm cds: " + ts_ids.TrimEnd(' ').TrimEnd(',') + ")";
                }
            }

            return ret;
        }

        private string GetOfficeCode(Data.Record record)
        {
            string ret = currSite.Office.office_cd;

            var altoffice = record.RecordAltOffice;

            if (altoffice != null)
            {
                ret = db.Offices.FirstOrDefault(p => p.office_id == altoffice.alt_office_id).office_cd;
            }

            return ret;
        }

        private string TCPApprovedDate(DateTime? approved_dt)
        {
            string ret = "";

            if (approved_dt != null) ret = String.Format("{0:MM/dd/yyyy}", approved_dt); else ret = "<i>never approved</i>";

            return ret;
        }

        private string NextTransmitTime(DateTime dNow, string times, string type)
        {
            string ret = "";
            string[] time = times.Split(',');
            int idxNext = 0;
            DateTime dCur;
            int i;

            for (i = 0; i < time.Length; i++ )
            {
                dCur = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy} {1}", dNow, time[i]));
                if (dCur > dNow)
                {
                    idxNext = i;
                    break;
                }
            }

            if (i > time.GetUpperBound(0)) i = 0;

            if (type == "next")
            {
                TimeSpan span = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy} {1}", dNow, time[i])).Subtract(dNow);
                ret = span.Minutes.ToString();
            }

            return ret;
        }

        protected void SetupPermission()
        {
            lbEditPubName.Visible = HasEditAccess;
            lbEditOffice.Visible = HasEditAccess;
            lbEditFieldTrip.Visible = HasEditAccess;
            hlEditDocs.Visible = HasEditAccess;
        }
        #endregion

        #region Internal Classes
        internal class TCPItem
        {
            private int _TCPID;
            private string _TCPName;
            private string _TCPURL;
            private string _LastApprovedDt;
            private Boolean _ApprovalReady;

            public int TCPID
            {
                get { return _TCPID; }
                set { _TCPID = value; }
            }
            public string TCPName
            {
                get { return _TCPName; }
                set { _TCPName = value; }
            }
            public string TCPURL
            {
                get { return _TCPURL; }
                set { _TCPURL = value; }
            }
            public string LastApprovedDt
            {
                get { return _LastApprovedDt; }
                set { _LastApprovedDt = value; }
            }
            public Boolean ApprovalReady
            {
                get { return _ApprovalReady; }
                set { _ApprovalReady = value; }
            }

            public TCPItem()
            {
                _TCPID = TCPID;
                _TCPName = TCPName;
                _TCPURL = TCPURL;
                _LastApprovedDt = LastApprovedDt;
                _ApprovalReady = ApprovalReady;
            }
        }
        internal class RecordItem
        {
            private int _rms_record_id;
            private string _personnel;
            private string _published;
            private string _active;
            private string _cat_no;
            private string _time_series;
            private string _office_cd;
            private string _type_ds;
            private string _RMSURL;

            public int rms_record_id
            {
                get { return _rms_record_id; }
                set { _rms_record_id = value; }
            }
            public string personnel
            {
                get { return _personnel; }
                set { _personnel = value; }
            }
            public string published
            {
                get { return _published; }
                set { _published = value; }
            }
            public string active
            {
                get { return _active; }
                set { _active = value; }
            }
            public string cat_no
            {
                get { return _cat_no; }
                set { _cat_no = value; }
            }
            public string time_series
            {
                get { return _time_series; }
                set { _time_series = value; }
            }
            public string office_cd
            {
                get { return _office_cd; }
                set { _office_cd = value; }
            }
            public string type_ds
            {
                get { return _type_ds; }
                set { _type_ds = value; }
            }
            public string RMSURL
            {
                get { return _RMSURL; }
                set { _RMSURL = value; }
            }
            public RecordItem()
            {
                _rms_record_id = rms_record_id;
                _personnel = personnel;
                _published = published;
                _active = active;
                _cat_no = cat_no;
                _time_series = time_series;
                _office_cd = office_cd;
                _type_ds = type_ds;
                _RMSURL = RMSURL;
            }
        }

        internal class PeriodItem
        {
            private int _rms_record_id;
            private string _WY;
            private string _IconURL;
            private string _LockIconURL;

            public int rms_record_id
            {
                get { return _rms_record_id; }
                set { _rms_record_id = value; }
            }
            public string WY
            {
                get { return _WY; }
                set { _WY = value; }
            }
            public string IconURL
            {
                get { return _IconURL; }
                set { _IconURL = value; }
            }
            public string LockIconURL
            {
                get { return _LockIconURL; }
                set { _LockIconURL = value; }
            }
            public PeriodItem()
            {
                _rms_record_id = rms_record_id;
                _WY = WY;
                _IconURL = IconURL;
                _LockIconURL = LockIconURL;
            }
        }
        #endregion

        #region CRP DataList Methods and Events
        protected void dlRecords_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                RecordItem rec = e.Item.DataItem as RecordItem;
                HiddenField hf = (HiddenField)e.Item.FindControl("hfRMSRecordID");
                LinkButton lbEditRecord = (LinkButton)e.Item.FindControl("lbEditRecord");
                HyperLink hlAuditRecord = (HyperLink)e.Item.FindControl("hlAuditRecord");
                HyperLink hlNewPeriod = (HyperLink)e.Item.FindControl("hlNewPeriod");
                Image imgLock = (Image)e.Item.FindControl("imgLock");
                Literal ltlCurrentPeriods = (Literal)e.Item.FindControl("ltlCurrentPeriods");
                RadDropDownList rddlWYs = (RadDropDownList)e.Item.FindControl("rddlWYs");
                Panel pnlRecord = (Panel)e.Item.FindControl("pnlRecord");
                Panel pnlInactive = (Panel)e.Item.FindControl("pnlInactive");

                hf.Value = rec.rms_record_id.ToString();
                lbEditRecord.OnClientClick = String.Format("openWin('{0}','record'); return false;", rec.rms_record_id);

                //If the user belongs to this site's WSC (or has an exception to work in the WSC), or is a SuperUser, then allow them to edit the page
                if (user.WSCID.Contains(WSCID) && user.IsAdmin || user.IsSuperUser) lbEditRecord.Visible = true; else lbEditRecord.Visible = false;

                //If this is an active record, show the active panel, otherwise show the inactive
                if (rec.active == "active")
                {
                    pnlRecord.Visible = true;
                    pnlInactive.Visible = false;

                    hlAuditRecord.NavigateUrl = String.Format("{0}Task/Audit.aspx?rms_record_id={1}", Config.RMSURL, rec.rms_record_id);

                    //Grab all the years with a record period
                    var years = db.SP_RMS_WYs_with_periods(rec.rms_record_id).ToList();

                    rddlWYs.DataSource = years;
                    rddlWYs.DataBind();
                    rddlWYs.Items.Insert(0, new DropDownListItem { Value = "", Text = "" });
                    
                    List<string> currWYs = new List<string>();
                    string currPeriods = "";
                    ltlCurrentPeriods.Text = "";
                    foreach (var year in years)
                    {
                        //Check to see what WYs have periods that have not been approved yet, and show these by default
                        var currWY = db.SP_RMS_WYs_with_periods_to_work(rec.rms_record_id, year.WY.ToString()).ToList();

                        if (currWY.Count > 0)
                        {
                            DropDownListItem itemToRemove = rddlWYs.Items.FirstOrDefault(p => p.Value == year.WY.ToString());
                            rddlWYs.Items.Remove(itemToRemove);

                            int prevWY = (int)year.WY - 1;
                            string start_dt = "10/01/" + prevWY.ToString();
                            string end_dt = "10/01/" + year.WY.ToString();

                            //Grab the periods for the WY
                            var periods = db.SP_RMS_Periods_per_WY(rec.rms_record_id, Convert.ToDateTime(start_dt), Convert.ToDateTime(end_dt), "0").ToList();

                            //Set the reanalyze flag
                            var reanalyze = db.SP_RMS_Periods_per_WY(rec.rms_record_id, Convert.ToDateTime(start_dt), Convert.ToDateTime(end_dt), "Reanalyze").ToList();

                            //Loop through each period and setup the images and links
                            currPeriods = RenderRecordPeriods(rec.rms_record_id, periods, reanalyze);

                            ltlCurrentPeriods.Text += "<p style='font-weight:bold;'>" + year.WY.ToString() + "</p>" + currPeriods;
                        }
                    }

                    if (string.IsNullOrEmpty(ltlCurrentPeriods.Text)) ltlCurrentPeriods.Text = "";

                    //Setup the "analyze new period" link and associated lock icon
                    if (!string.IsNullOrEmpty(NewPeriodLink(rec.rms_record_id, "URL")))
                    {
                        hlNewPeriod.Visible = true;
                        hlNewPeriod.NavigateUrl = NewPeriodLink(rec.rms_record_id, "URL");
                        imgLock.AlternateText = NewPeriodLink(rec.rms_record_id, "alt");
                        if (!string.IsNullOrEmpty(imgLock.AlternateText))
                        {
                            imgLock.Visible = true;
                            imgLock.ImageUrl = String.Format("{0}images/lock_sm.png", Config.RMSURL);
                        }
                        else imgLock.Visible = false;
                    }
                    else
                    {
                        hlNewPeriod.Visible = false;
                        imgLock.Visible = false;
                    }
                    
                }
                else
                {
                    hlAuditRecord.Visible = false;
                    pnlRecord.Visible = false;
                    pnlInactive.Visible = true;
                }
            }
        }

        protected void rddlWYs_SelectedIndexChanged(object sender, DropDownListEventArgs e)
        {
            RadDropDownList rddl = sender as RadDropDownList;
            DataListItem item = rddl.NamingContainer as DataListItem;
            HiddenField hf = (HiddenField)item.FindControl("hfRMSRecordID");
            Literal ltl = (Literal)item.FindControl("ltlHistoricPeriod");
            RadDropDownList rddlWYs = (RadDropDownList)item.FindControl("rddlWYs");

            if (!string.IsNullOrEmpty(rddl.SelectedValue))
            {
                int prevWY = Convert.ToInt32(rddl.SelectedValue) - 1;
                string start_dt = "10/01/" + prevWY.ToString();
                string end_dt = "10/01/" + rddl.SelectedValue.ToString();

                //Grab the periods for the WY
                var periods = db.SP_RMS_Periods_per_WY(Convert.ToInt32(hf.Value), Convert.ToDateTime(start_dt), Convert.ToDateTime(end_dt), "0").ToList();

                //Set the reanalyze flag
                var reanalyze = db.SP_RMS_Periods_per_WY(Convert.ToInt32(hf.Value), Convert.ToDateTime(start_dt), Convert.ToDateTime(end_dt), "Reanalyze").ToList();

                //Loop through each period and setup the images and links
                string hisPeriods = RenderRecordPeriods(Convert.ToInt32(hf.Value), periods, reanalyze);

                ltl.Text = "<p style='font-weight:bold;'>" + rddl.SelectedValue.ToString() + "</p>" + hisPeriods;
            }
            else
            {
                ltl.Text = "";
            }
        }

        /// <summary>
        /// Look through each period and setup the images and links
        /// </summary>
        private string RenderRecordPeriods(int rms_record_id, IEnumerable<Data.SP_RMS_Periods_per_WYResult> periods, List<Data.SP_RMS_Periods_per_WYResult> reanalyze_flag)
        {
            string currPeriods = "";
            int? level = null;
            int? prev_level = null;
            string prev_status = "";
            DateTime? prev_pbd = null;

            string showlockpng = ShowLockPNG(rms_record_id);
            string showsavepng = ShowLocks("save", rms_record_id);

            //Check to see if there are any audits for this record
            var audits = db.Audits.Where(p => p.AuditRecords.Where(a => a.rms_record_id == rms_record_id ).ToList().Count > 0).ToList();

            foreach (var period in periods)
            {
                string imgApproved = "";
                string hlStart = "&nbsp;&nbsp;&nbsp;&nbsp;";
                string hlEnd = "</a>";
                string note = "";
                string showlock = "";
                string showsave = "";
                string showaudit = "";

                //If there is an audit for this record, check to see if any of them involve the current period, but only if the period has been approved
                if (audits != null && period.status_va == "Approved")
                {
                    foreach (var audit in audits)
                    {
                        if ((period.period_beg_dt == audit.audit_beg_dt || period.period_beg_dt > audit.audit_beg_dt) && (period.period_end_dt == audit.audit_end_dt || period.period_end_dt < audit.audit_end_dt))
                            showaudit = String.Format("<a href='javascript:ShowAuditPopup({0})'><img src='{1}images/viewdoc.png' alt='View audit details' /></a>", period.period_id, Config.RMSURL);
                    }
                }

                //A period with a status of 'Analyzing' means that it was saved to be finished later
                if (period.status_va == "Analyzing")
                {
                    hlStart = String.Format("<a href='{0}Task/RecordProcess.aspx?period_id={1}&task=Analyze' style='padding-left:14px;'>", Config.RMSURL, period.period_id);
                    level = 2;
                }
                //A period with a status of 'Reanalyze' means that it was sent back for reanalyzing by the approver
                else if (period.status_va == "Reanalyze")  
                {
                    hlStart = String.Format("<a href='{0}Task/RecordProcess.aspx?period_id={1}&task=Reanalyze' style='padding-left:14px;'>", Config.RMSURL, period.period_id);
                    note = " <i>(reanalyze)</i>";
                    level = 0;
                    if (!string.IsNullOrEmpty(showsavepng))
                    {
                        hlStart = "&nbsp;&nbsp;&nbsp;&nbsp;";
                        hlEnd = "";
                    }
                }
                //A period with a status of 'Analyzed' means that it's ready to be approved
                else if (period.status_va == "Analyzed")
                {
                    var doubles = db.SP_Site_Analyzed_Doubles(rms_record_id).FirstOrDefault();
                    if (period.period_beg_dt != doubles.period_beg_dt)
                    {
                        hlStart = "&nbsp;&nbsp;&nbsp;&nbsp;";
                        hlEnd = "";
                    }
                    else hlStart = String.Format("<a href='{0}Task/RecordProcess.aspx?period_id={1}&task=Approve' style='padding-left:14px;'>", Config.RMSURL, period.period_id);
                    level = 3;
                    if (!string.IsNullOrEmpty(showsavepng))
                    {
                        hlStart = "&nbsp;&nbsp;&nbsp;&nbsp;";
                        hlEnd = "";
                    }
                }
                //A period with a status of 'Approving' means that it was saved to be finished later
                else if (period.status_va == "Approving")
                {
                    hlStart = String.Format("<a href='{0}Task/RecordProcess.aspx?period_id={1}&task=Approve' style='padding-left:14px;'>", Config.RMSURL, period.period_id);
                    level = 4;
                }
                //A period with a status of 'SavePending' means that it was saved to be finished later
                else if (period.status_va == "SavePending")
                {
                    hlStart = String.Format("<a href='{0}Task/RecordProcess.aspx?period_id={1}&task=SavePending' style='padding-left:14px;'>", Config.RMSURL, period.period_id);
                    level = 4;
                }
                //A period with a status of 'Approved' means that it is done, and they can only view the station analysis in a pop-up
                else if (period.status_va == "Approved")
                {
                    imgApproved = String.Format("<img src='{0}images/approved.gif' alt='Approved' style='float:left;padding: 0 2px 2px 0;' />", Config.RMSURL);
                    hlStart = String.Format("<a href='javascript:ShowAnalysisPopup({0})'>", period.period_id);
                    level = 5;
                }
                //A period with a status of 'Pending' means that the approver made minor edits to the analysis, and the period is waiting for the analyst to concur
                else if (period.status_va == "Pending")
                {
                    imgApproved = String.Format("<img src='{0}images/pending.png' alt='Pending Analyst Concurrence' style='float:left;padding: 0 2px 2px 0;' />", Config.RMSURL);
                    hlStart = String.Format("<a href='{0}Task/RecordProcess.aspx?period_id={1}&task=Pending'>", Config.RMSURL, period.period_id);
                }
                //Otherwise, they are trying to start a new period for analyzing
                else
                {
                    hlStart = String.Format("<a href='{0}Task/RecordProcess.aspx?period_id={1}&task=Analyze' style='padding-left:14px;'>", Config.RMSURL, period.period_id);
                    if (!string.IsNullOrEmpty(showsavepng))
                    {
                        hlStart = "&nbsp;&nbsp;&nbsp;&nbsp;";
                        hlEnd = "";
                    }
                    level = 1;
                }

                if (prev_status == period.status_va && prev_pbd != period.period_beg_dt)
                {
                    if (period.status_va != "Approved")
                    {
                        hlStart = "&nbsp;&nbsp;&nbsp;&nbsp;";
                        hlEnd = "";
                    }
                }
                else if (prev_status == "Reanalyze")
                {
                    if (period.status_va != "Approved")
                    {
                        hlStart = "&nbsp;&nbsp;&nbsp;&nbsp;";
                        hlEnd = "";
                    }
                }
                else if (period.status_va == "Analyzing" || period.status_va == "Analyzed")
                {
                    if (reanalyze_flag.Count > 0)
                    {
                        hlStart = "&nbsp;&nbsp;&nbsp;&nbsp;";
                        hlEnd = "";
                    }
                    if (prev_status == "Approving")
                    {
                        hlStart = "&nbsp;&nbsp;&nbsp;&nbsp;";
                        hlEnd = "";
                    }
                }

                if (prev_level < level && prev_level != null)
                {
                    if (period.status_va != "Approved")
                    {
                        hlStart = "&nbsp;&nbsp;&nbsp;&nbsp;";
                        hlEnd = "";
                    }
                }

                if (period.status_va != "Approved")
                {
                    if (hlStart != "&nbsp;&nbsp;&nbsp;&nbsp;")
                    {
                        showlock = showlockpng;
                        showsave = showsavepng;
                    }
                    else
                    {
                        showlock = "";
                        showsave = "";
                    }
                }

                currPeriods += String.Format("<p class='periods'>{0} {1}{2:MM/dd/yyyy}-{3:MM/dd/yyyy}{4} {5}{6}{7}{8}</p>", imgApproved, hlStart, period.period_beg_dt, period.period_end_dt, hlEnd, note, showlock, showsave, showaudit);

                prev_level = level;
                prev_status = period.status_va;
                prev_pbd = period.period_beg_dt;
            }

            return currPeriods;
        }

        /// <summary>
        /// Returns the URL to be used in the new period link or the alternate text to be used in the lock icon
        /// </summary>
        /// <param name="type">Pass "URL" to get the new period link, or "alt" to get the alternate text for the lock icon</param>
        private string NewPeriodLink(int rms_record_id, string type)
        {
            string pOut = "";

            var rap = db.RecordAnalysisPeriods.OrderByDescending(p => p.period_beg_dt).FirstOrDefault(p => p.rms_record_id == rms_record_id);

            if (rap != null)
            {
                string showlockpng = ShowLockPNG(rms_record_id);
                string status_va = rap.status_va;
                string showsave = ShowLocks("save", rms_record_id);
                var reanalyze_flag = db.RecordAnalysisPeriods.FirstOrDefault(p => p.rms_record_id == rms_record_id && p.status_va == "Reanalyze");

                if (status_va != "Analyzing" && status_va != "Reanalyze" && string.IsNullOrEmpty(showsave))
                {
                    if (reanalyze_flag == null)
                    {
                        switch (type)
                        {
                            case "URL":
                                pOut = String.Format("{0}Task/RecordProcess.aspx?task=Analyze&rms_record_id={1}", Config.RMSURL, rms_record_id);
                                break;
                            case "alt":
                                if (!string.IsNullOrEmpty(showlockpng))
                                {
                                    var locks = db.RecordLocks.FirstOrDefault(p => p.rms_record_id == rms_record_id);
                                    pOut = String.Format("lock type is {0}, locked by {1} {2:MM/dd/yyyy}", locks.lock_type, locks.lock_uid, locks.lock_dt);
                                }
                                break;
                        }
                    }
                }
            }
            else
            {
                switch (type)
                {
                    case "URL":
                        pOut = String.Format("{0}Task/RecordProcess.aspx?task=Analyze&rms_record_id={1}", Config.RMSURL, rms_record_id);
                        break;
                    case "alt":
                        pOut = "";
                        break;
                }
            }
            return pOut;
        }

        /// <summary>
        /// Checks the RMS_Locks table to see if a lock exists for the rms_record_id and lock_type
        /// </summary>
        /// <param name="lock_type">Can be either Anaylze, Approve, or Reanalyze</param>
        private Boolean RecordIsLocked(string lock_type, int rms_record_id)
        {
            Boolean pOut = false;

            var locks = db.RecordLocks.FirstOrDefault(p => p.rms_record_id == rms_record_id && p.lock_type == lock_type);

            if (locks != null) pOut = true;

            return pOut;
        }

        /// <summary>
        /// Gets the image for the submitted lock_type and rms_record_id, but only if the lock_uid does not match the current authenticated user
        /// </summary>
        /// <param name="lock_type">A lock_type of "lock" returns the lock icon; a lock_type of "save" returns the save icon</param>
        private string ShowLocks(string lock_type, int rms_record_id)
        {
            string pOut = "";

            var locks = db.RecordLocks.FirstOrDefault(p => p.rms_record_id == rms_record_id);

            if (locks != null)
            {
                if (lock_type == "lock")
                {
                    if (locks.lock_uid != user.ID)
                    {
                        if (locks.lock_type == "Analyze" || locks.lock_type == "Approve")
                        {
                            pOut = String.Format(" <img border='0' src='{0}images/lock_sm.png' alt='lock type is {1}, locked by {2} {3:MM/dd/yyyy}' />", Config.RMSURL, locks.lock_type, locks.lock_uid, locks.lock_dt);
                        }
                    }
                }
                else if (lock_type == "save")
                {
                    if (locks.lock_type == "Analyzing" || locks.lock_type == "Approving")
                    {
                        pOut = String.Format(" <img border='0' src='{0}images/save_sm.png' alt='{1} in progress by {2} {3:MM/dd/yyyy}' />", Config.RMSURL, locks.lock_type, locks.lock_uid, locks.lock_dt);
                    }
                }
            }

            return pOut;
        }

        /// <summary>
        /// Returns a lock icon image of there is a lock for the record
        /// </summary>
        private string ShowLockPNG(int rms_record_id)
        {
            string pOut = "";
            Boolean islocked = RecordIsLocked("Analyze", rms_record_id);
            if (!islocked)
            {
                islocked = RecordIsLocked("Approve", rms_record_id);
            }

            if (islocked) pOut = ShowLocks("lock", rms_record_id);

            return pOut;
        }
        #endregion

        #region Safety DataList Methods and Events
        protected void dlTCPs_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                TCPItem tcp = e.Item.DataItem as TCPItem;
                LinkButton lbTCPReview = (LinkButton)e.Item.FindControl("lbTCPReview");
                Boolean ApprovalReady = tcp.ApprovalReady;
                int TCPID = tcp.TCPID;

                if (ApprovalReady)
                {
                    lbTCPReview.Text = "TCP pending approval";
                    lbTCPReview.Font.Italic = true;
                }
                else
                {
                    lbTCPReview.Text = "Submit for Approval";
                    lbTCPReview.Font.Bold = true;
                    lbTCPReview.OnClientClick = String.Format("openWin('{0}','TCP'); return false;", TCPID);
                }
            }
        }
        #endregion

        #region Edit Events
        protected void Edit_Command(object sender, CommandEventArgs e)
        {
            switch (e.CommandArgument.ToString())
            {
                case "PubName":
                    pnlPubNameEdit.Visible = true;
                    pnlPubNameView.Visible = false;
                    break;
                case "Office":
                    pnlOfficeEdit.Visible = true;
                    pnlOfficeView.Visible = false;
                    break;
            }
        }

        protected void Commit_Command(object sender, EventArgs e)
        {
            switch (sender.GetType().Name)
            {
                case "RadButton":
                    Button_Logic((RadButton)sender);
                    break;
                case "RadDropDownList":
                    DropDownList_Logic((RadDropDownList)sender);
                    break;
            }

            db.SubmitChanges();
        }

        protected void Button_Logic(RadButton rb)
        {
            switch (rb.ID)
            {
                case "rbPubName":
                    currSite.station_full_nm = rtbPubName.Text;
                    ltlPubName.Text = rtbPubName.Text;

                    //refresh the page header info
                    ph1.SubTitle = currSite.site_no + " " + rtbPubName.Text;
                    ph1.RefreshHeadingData();

                    pnlPubNameEdit.Visible = false;
                    pnlPubNameView.Visible = true;
                    break;
            }
        }

        protected void DropDownList_Logic(RadDropDownList rddl)
        {
            switch (rddl.ID)
            {
                case "rddlOffice":
                    currSite.office_id = Convert.ToInt32(rddl.SelectedValue);
                    OfficeID = Convert.ToInt32(rddl.SelectedValue);
                    ltlOffice.Text = db.Offices.Where(p => p.office_id == Convert.ToInt32(rddl.SelectedValue)).FirstOrDefault().office_nm;
                    //refresh the page header info
                    ph1.RefreshHeadingData();

                    pnlOfficeEdit.Visible = false;
                    pnlOfficeView.Visible = true;
                    break;
            }
        }
        #endregion

        #region Misc Events
        protected void rbJump_Click(object sender, EventArgs e)
        {
            string site_no = rtbSiteNo.Text;
            if (!string.IsNullOrEmpty(site_no))
            {
                int site_id = db.Sites.Where(p => p.site_no == site_no).FirstOrDefault().site_id;
                Response.Redirect(String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMSURL, site_id));
            }
        }

        protected void btnDCPViewData_Command(object sender, CommandEventArgs e)
        {
            Button btn = (Button)sender;
            DataListItem item = (DataListItem)btn.NamingContainer;
            TextBox txtSince = (TextBox)item.FindControl("tbDCPViewData");
            string since = txtSince.Text;
            if (!string.IsNullOrEmpty(since))
            {
                Response.Redirect(String.Format("http://lrgseddn3.cr.usgs.gov/cgi-bin/fieldtest.pl?DCPID={0}&SINCE={1}", e.CommandArgument.ToString(), since), "_blank", "menubar=0,scrollbars=1,width=780,height=900,top=10");
            }
        }

        protected void ram_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            if (e.Argument == "RebindFieldTrips")
            {
                //Delete all of the field trips assigned to this site
                db.TripSites.DeleteAllOnSubmit(currSite.TripSites);
                db.SubmitChanges();

                //Split out the hfFieldTripIDs string by ',' and place into an array
                string[] FieldTripIDs = hfFieldTripIDs.Value.Split(',');

                if (FieldTripIDs.Count() > 0)
                {
                    //Add each field trip from the listbox as a new record
                    foreach (var trip_id in FieldTripIDs)
                    {
                        if (!string.IsNullOrEmpty(trip_id))
                        {
                            if (currSite.TripSites.FirstOrDefault(p => p.trip_id.ToString() == trip_id) == null) currSite.TripSites.Add(new Data.TripSite { trip_id = Convert.ToInt32(trip_id), site_id = currSite.site_id });
                        }
                    }
                }
                //Commit changes to the database
                db.SubmitChanges();

                //Refresh the field trip text
                string fieldtrips = "";
                foreach (var trip in currSite.TripSites.ToList())
                {
                    fieldtrips += trip.Trip.trip_nm + " - " + trip.Trip.user_id + ", ";
                }
                if (!string.IsNullOrEmpty(fieldtrips)) ltlFieldTrip.Text = fieldtrips.TrimEnd(' ').TrimEnd(','); else ltlFieldTrip.Text = "<i>none assigned</i>";
                
            }
            else if (e.Argument == "RebindRecords")
            {
                var record = currSite.Records;
                dlRecords.DataSource = record.Select(p => new RecordItem
                {
                    rms_record_id = p.rms_record_id,
                    personnel = GetPersonnelAssignments(p.operator_uid, p.analyzer_uid, p.approver_uid, p.auditor_uid),
                    published = GetPublished(Convert.ToBoolean(p.not_published_fg)),
                    active = GetActive(Convert.ToBoolean(p.not_used_fg)),
                    cat_no = GetCategory(Convert.ToInt32(p.category_no)),
                    office_cd = GetOfficeCode(p),
                    time_series = GetTimeSeries(p),
                    type_ds = GetRecordType(p.RecordType.type_ds),
                    RMSURL = Config.RMSURL
                }).ToList();
                dlRecords.DataBind();
            }
            else if (e.Argument == "RebindSafety")
            {
                dlTCPs.DataSource = currSite.TCPSite.TCPs.Select(p => new TCPItem
                {
                    TCPID = p.TCPID,
                    TCPName = String.Format("{0} - TCP, {1}", p.TCPPlanDetail.Number, p.TCPPlanDetail.SubName),
                    TCPURL = String.Format("{0}TCPView.aspx?TCPID={1}", Config.SafetyURL, p.TCPID),
                    LastApprovedDt = TCPApprovedDate(p.ApprovedDt),
                    ApprovalReady = Convert.ToBoolean(p.ApprovalReady)
                });
                dlTCPs.DataBind();
            }
        }
        #endregion
    }
}