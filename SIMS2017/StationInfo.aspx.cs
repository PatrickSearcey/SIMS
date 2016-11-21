using Core;
using System;
using System.Collections.Generic;
using System.Linq;
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
            //If no site_id was passed, then redirect back to the homepage
            string site_id = Request.QueryString["site_id"];
            if (!string.IsNullOrEmpty(site_id)) SiteID = Convert.ToInt32(site_id); else Response.Redirect(Config.SIMS2017URL + "SIMSWSCHome.aspx");
            
            //Using the passed site_id, setup the site data element, and reset the office and wsc to match that of the current site
            currSite = db.Sites.Where(p => p.site_id == SiteID).FirstOrDefault();
            OfficeID = (int)currSite.office_id;
            WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            ph1.Title = "Station Information Page";
            ph1.SubTitle = currSite.site_no + " " + currSite.station_full_nm;

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
            hlNWISWeb.NavigateUrl = String.Format("http://waterdata.usgs.gov/nwis/inventory/?site_no={0}&agency_cd={1}", currSite.site_no, currSite.agency_cd);
            hlNWISOpsRequest.NavigateUrl = String.Format("{0}NWISOpsRequest.aspx?office_id={1}&site_id={2}", Config.SIMS2017URL, OfficeID, SiteID);

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
            if (!string.IsNullOrEmpty(fieldtrips)) ltlFieldTrip.Text = fieldtrips.TrimEnd(' ').TrimEnd(','); else ltlFieldTrip.Text = "<i>none assigned</i>";
            hlMapTrips.NavigateUrl = String.Format("{0}fieldtripmap.aspx?office_id={1}&trip_id={2}&wsc_id={3}", Config.SIMSURL, OfficeID, currSite.TripSites.FirstOrDefault().trip_id, WSCID);
            lbEditFieldTrip.OnClientClick = String.Format("openWin('{0}'); return false;", currSite.site_id);
            hlSiFTA.NavigateUrl = "http://sifta.water.usgs.gov/NationalFunding/Site.aspx?SiteNumber=" + currSite.site_no.Trim();

            //Station Documents
            hlEditDocs.NavigateUrl = String.Format("{0}StationDoc/EditDocs.aspx?site_id={1}", Config.SIMS2017URL, currSite.site_id);
            hlSDESC.NavigateUrl = String.Format("{0}StationDoc/ViewDocs.aspx?site_id={1}&type=SDESC", Config.SIMS2017URL, currSite.site_id);
            hlMANU.NavigateUrl = String.Format("{0}StationDoc/ViewDocs.aspx?site_id={1}&type=MANU", Config.SIMS2017URL, currSite.site_id);
            hlSANAL.NavigateUrl = String.Format("{0}StationDoc/ViewDocs.aspx?site_id={1}&type=SANAL", Config.SIMS2017URL, currSite.site_id);
            hlCustomReport.NavigateUrl = String.Format("{0}StationDoc/ViewDocs.aspx?site_id={1}&type=Custom", Config.SIMS2017URL, currSite.site_id);
            hlArchives.NavigateUrl = String.Format("{0}StationDoc/Archives.aspx?site_id={1}", Config.SIMS2017URL, currSite.site_id);
            hlSLAP.NavigateUrl = String.Format("{0}HistoricObjectSum.aspx?site_no={1}&agency_cd={2}", Config.SLAPURL, currSite.site_no.Trim(), currSite.agency_cd);
            if (!Config.IsSLAPWSC.Contains(WSCID)) hlSLAP.Visible = false;
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
                        DateTime revised_dt = Convert.ToDateTime(db.ElemReportSums.FirstOrDefault(p => p.site_id == currSite.site_id && p.report_type_cd == "MANU").revised_dt);
                        if (revised_dt > approved_dt) ltlApproved.Text = String.Format("(<a href='{0}StationDoc/MAISite.aspx?site_id={1}' style='color:red;'>Needs Approval</a>)", Config.SIMS2017URL, currSite.site_id); else ltlApproved.Text = String.Format("(<a href='{0}StationDoc/MAISite.aspx?site_id={1}' style='color:green'>Approved</a>)", Config.SIMS2017URL, currSite.site_id);
                    }
                    else ltlApproved.Text = String.Format("(<a href='{0}StationDoc/MAISite.aspx?site_id={1}'>Needs Approval</a>)", Config.SIMS2017URL, currSite.site_id);
                }
                else ltlApproved.Visible = false;
            }
            else ltlApproved.Visible = false;

            //CRP


            //Safety
            hlSHATutorial.NavigateUrl = String.Format("{0}SIMSShare/SIMS_SHA_Tutorial.pptx", Config.SIMSURL);
            hlTCPTutorial.NavigateUrl = String.Format("{0}SIMSShare/SIMS_TCP_Software_Training.pptx", Config.SIMSURL);
            var SHA = currSite.SHAs.FirstOrDefault();
            if (SHA != null)
            {
                pnlSHACreate.Visible = false;
                pnlSHAEdit.Visible = true;
                hlSHAEdit.NavigateUrl = String.Format("{0}SiteHazardAnalysis.aspx?site_id={1}", Config.SafetyURL, currSite.site_id);
                hlSHAPrintVersion.NavigateUrl = String.Format("{0}SHAPrint.aspx?site_id={1}", Config.SafetyURL, currSite.site_id);
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
                hlSHACreate.NavigateUrl = String.Format("{0}Helper/CreateSHA.ashx?site_id={1}", Config.SIMS2017URL, currSite.site_id);
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
                    dlTCPs.DataSource = currSite.TCPSite.TCPs.Select(p => new
                    {
                        TCPName = p.TCPPlanDetail.Name,
                        TCPURL = String.Format("{0}TCPView.aspx?tcp_id={1}", Config.SafetyURL, p.TCPID),
                        LastApprovedDt = TCPApprovedDate(p.ApprovedDt),
                        TCPApprovalStatus = TCPApprovalStatus(p.ApprovalReady, p.TCPID)
                    });
                    dlTCPs.DataBind();
                    hlTCPTrackStatus.NavigateUrl = String.Format("{0}TCPReport.aspx", Config.SafetyURL);
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
            if (di != null)
            {
                pnlDCPTable.Visible = true;
                ltlNoDCP.Visible = false;

                ltlDCPOfficeTime.Text = String.Format("{0:dddd, MMMM dd, yyyy} ({1}), {0:h:mm:ss tt}", di.doffice, di.jdoffice.Substring(4,3));
                ltlDCPSiteTime.Text = String.Format("{0:dddd, MMMM dd, yyyy} ({1}), {0:h:mm:ss tt}", di.dsite, di.jdsite.Substring(4, 3));
                ltlDCPGMTTime.Text = String.Format("{0:dddd, MMMM dd, yyyy} ({1}), {0:h:mm:ss tt}", di.dutc, di.jdutc.Substring(4, 3));

                dlDCPTable.DataSource = db.spz_GetDCPInfo(currSite.site_id).Select(p => new
                    {
                        LocalTransmitTime = NextTransmitTime(Convert.ToDateTime(p.doffice), p.officedcptimes, "local"),
                        GMTTransmitTime = NextTransmitTime(Convert.ToDateTime(p.dutc), p.utcdcptimes, "GMT"),
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
            
            
        }

        private string TCPApprovalStatus(bool? approval_ready, int TCPID)
        {
            string ret = "";

            if (approval_ready != null)
            {
                if ((bool)approval_ready) ret = "<i>TCP pending approval</i>"; else ret = String.Format("<a href='{0}TCPEdit.aspx?tcp_id={1}&action=approve'><b>Submit for Approval</b></a>", Config.SafetyURL, TCPID);
            }
            else
            {
                ret = String.Format("<a href='{0}TCPEdit.aspx?tcp_id={1}&action=approve'><b>Submit for Approval</b></a>", Config.SafetyURL, TCPID);
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
            else if (type == "local")
            {
                ret = String.Format("{0:hh:mm}", time[i]);
            }
            else if (type == "GMT")
            {
                ret = String.Format("{0:hh:mm}", time[idxNext]);
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
                Response.Redirect(String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMS2017URL, site_id));
            }
        }

        protected void ram_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            if (e.Argument == "Rebind")
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
        }
        #endregion
    }
}