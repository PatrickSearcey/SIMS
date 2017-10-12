using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Safety
{
    public partial class TCPEdit : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        public Boolean HasEditAccess { get; set; }
        public Boolean EnableControl { get; set; }
        private Data.Site currSite;
        private int ShoulderRule { get; set; }
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
        private int TCPID
        {
            get
            {
                if (Session["TCPID"] == null) return 0; else return (int)Session["TCPID"];
            }
            set
            {
                Session["TCPID"] = value;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            //If no site_id was passed, then redirect back to the homepage
            string site_id = Request.QueryString["site_id"];
            if (!string.IsNullOrEmpty(site_id)) SiteID = Convert.ToInt32(site_id); else Response.Redirect(Config.SIMSURL + "SIMSWSCHome.aspx");

            //Using the passed site_id, setup the site data element, and reset the office and wsc to match that of the current site
            currSite = db.Sites.Where(p => p.site_id == SiteID).FirstOrDefault();
            OfficeID = (int)currSite.office_id;
            WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            ph1.Title = "Manage Traffic Control Safety Plans";
            ph1.SubTitle = currSite.site_no + " " + db.vSITEFILEs.FirstOrDefault(s => s.site_no == currSite.site_no && s.agency_cd == currSite.agency_cd).station_nm;

            //Find and set the shoulder rule based on the physical (which state) location of the site
            ShoulderRule = Convert.ToInt32(db.TCPShoulderRules.FirstOrDefault(p => p.StateCode == db.vSITEFILEs.FirstOrDefault(s => s.site_id == currSite.nwisweb_site_id).state_cd).ShoulderRule);

            if (!Page.IsPostBack)
            {
                //If the user belongs to this site's WSC (or has an exception to work in the WSC), or is a SuperUser, then allow them to edit the page
                if (user.WSCID.Contains(WSCID) || user.IsSuperUser) HasEditAccess = true;

                //If a site is found in the TCP_Site_Master table, go ahead and populate the data in the form fields
                if (currSite.TCPSite != null) PopulatePageData();
                
                SetupPermission();
            }
        }

        #region Properties
        private IEnumerable<TCPDataItem> TCPDataSource()
        {
            var tcps = db.TCPs.Where(p => p.site_id == SiteID).Select(p => new TCPDataItem {
                    TCPID = p.TCPID,
                    TCPLink = String.Format("{0}TCPView.aspx?TCPID={1}", Config.SafetyURL, p.TCPID),
                    TCPName = String.Format("{0} - TCP, {1}", p.TCPPlanDetail.Number, p.TCPPlanDetail.SubName),
                    PlanRemarks = p.Remarks.FormatParagraphOut(),
                    WorkAreaActivity = p.WorkAreaActivity
                }).ToList();

            return tcps;
        }
        #endregion

        #region Internal Classes
        internal class TCPDataItem
        {
            private int _TCPID;
            private string _TCPLink;
            private string _TCPName;
            private string _PlanRemarks;
            private string _WorkAreaActivity;

            public int TCPID
            {
                get { return _TCPID; }
                set { _TCPID = value; }
            }
            public string TCPLink
            {
                get { return _TCPLink; }
                set { _TCPLink = value; }
            }
            public string TCPName
            {
                get { return _TCPName; }
                set { _TCPName = value; }
            }
            public string PlanRemarks
            {
                get { return _PlanRemarks; }
                set { _PlanRemarks = value; }
            }
            public string WorkAreaActivity
            {
                get { return _WorkAreaActivity; }
                set { _WorkAreaActivity = value; }
            }
            public TCPDataItem()
            {
                _TCPID = TCPID;
                _TCPLink = TCPLink;
                _TCPName = TCPName;
                _PlanRemarks = PlanRemarks;
                _WorkAreaActivity = WorkAreaActivity;
            }
        }
        #endregion

        #region Methods
        protected void PopulatePageData()
        {
            //Find out if there is a plan V already in the database
            var planV = currSite.TCPSite.TCPs.FirstOrDefault(p => p.PlanID == 8);

            //Site Specific Information
            ltlLastUpdated.Text = "This information was lasted updated on " + String.Format("{0:MM/dd/yyyy}", currSite.TCPSite.UpdatedDt) + " at " + String.Format("{0:h:mm tt}", currSite.TCPSite.UpdatedDt) + " by " + currSite.TCPSite.UpdatedBy + "."; 
            if (currSite.TCPSite.RemoteSite != null)
            {
                rddlRemote.SelectedValue = currSite.TCPSite.RemoteSite.ToString();
                if ((bool)currSite.TCPSite.RemoteSite)
                {
                    rddlFlaggers.Enabled = false;
                    rfvFlaggers.Enabled = false;
                    EnableControl = false;
                    SetControlStatus();
                    lblRemoteSiteNote.Visible = true;
                    rtbNotes.Text = currSite.TCPSite.Notes.FormatParagraphEdit();
                    //Plan Specific Information
                    dlTCPs.DataSource = TCPDataSource();
                    dlTCPs.DataBind();
                    //If there is already a plan VI in the database, do not show the link button to add it
                    if (planV != null) lbAddPlanV.Visible = false;
                    else
                    {
                        lbAddPlanV.PostBackUrl = String.Format("{0}TCPEdit.aspx?site_id={1}", Config.SafetyURL, currSite.site_id);
                        lbAddPlanV.Visible = true;
                    }
                    if (planV != null) imgBullet.Visible = false; else imgBullet.Visible = true;
                    //If there is a plan V in the database, display the Upload Plan V Document controls
                    if (planV != null) ltlPlanVDoc.Visible = true; else ltlPlanVDoc.Visible = false;
                    if (planV != null) pnlFileUpload.Visible = true; else pnlFileUpload.Visible = false;
                    if (planV != null) imgUploadDocHelp.Visible = true; else imgUploadDocHelp.Visible = false;
                    return;
                }
            }
            lblRemoteSiteNote.Visible = false;
            rtbRoadName.Text = currSite.TCPSite.RoadName;
            if (currSite.TCPSite.Expressway != null) rddlExpressway.SelectedValue = currSite.TCPSite.Expressway.ToString();
            rntbBridgeWidth.Text = currSite.TCPSite.BridgeWidth.ToString();
            rntbWorkZone.Text = currSite.TCPSite.WorkZone.ToString();
            rntbLaneWidth.Text = currSite.TCPSite.LaneWidth.ToString();
            rntbShoulderWidth.Text = currSite.TCPSite.ShoulderWidth.ToString();
            rntbSpeedLimit.Text = currSite.TCPSite.SpeedLimit.ToString();
            rntbLaneNumber.Text = currSite.TCPSite.LaneNumber.ToString();
            if (currSite.TCPSite.Flow2Way != null) rddlFlow2Way.SelectedValue = currSite.TCPSite.Flow2Way.ToString();
            if (currSite.TCPSite.TrafficVolume != null) rddlTrafficVolume.SelectedValue = currSite.TCPSite.TrafficVolume;
            if (currSite.TCPSite.DividedHighway != null) rddlDividedHighway.SelectedValue = currSite.TCPSite.DividedHighway.ToString();
            if (currSite.TCPSite.Median != null) rddlMedian.SelectedValue = currSite.TCPSite.Median.ToString();
            if (currSite.TCPSite.Flaggers != null) rddlFlaggers.SelectedValue = currSite.TCPSite.Flaggers.ToString();
            rtbNotes.Text = currSite.TCPSite.Notes.FormatParagraphEdit();

            //Set control status
            if (currSite.TCPSite.ShoulderWidth != null)
            {
                if (currSite.TCPSite.ShoulderWidth < ShoulderRule)
                {
                    if (currSite.TCPSite.LaneNumber == 2 && (bool)currSite.TCPSite.Flow2Way)
                    {
                        rddlFlaggers.Enabled = true;
                        rfvFlaggers.Enabled = true;
                    }
                    else
                    {
                        rddlFlaggers.Enabled = false;
                        rfvFlaggers.Enabled = false;
                    }
                }
                else
                {
                    rddlFlaggers.Enabled = false;
                    rfvFlaggers.Enabled = false;
                }
            }
            else
            {
                rddlFlaggers.Enabled = false;
                rfvFlaggers.Enabled = false;
            }

            //Plan Specific Information
            dlTCPs.DataSource = TCPDataSource();
            dlTCPs.DataBind();

            //If there is already a plan V in the database, do not show the link button to add it
            if (planV != null) lbAddPlanV.Visible = false;
            else
            {
                lbAddPlanV.PostBackUrl = String.Format("{0}TCPEdit.aspx?site_id={1}", Config.SafetyURL, currSite.site_id);
                lbAddPlanV.Visible = true;
            }
            if (planV != null) imgBullet.Visible = false; else imgBullet.Visible = true;
            //If there is a plan V in the database, display the Upload Plan V Document controls
            if (planV != null) ltlPlanVDoc.Visible = true; else ltlPlanVDoc.Visible = false;
            if (planV != null) pnlFileUpload.Visible = true; else pnlFileUpload.Visible = false;
            if (planV != null) imgUploadDocHelp.Visible = true; else imgUploadDocHelp.Visible = false;
        }

        protected void SetupPermission()
        {
            rbSubmit.Enabled = HasEditAccess;
            rbCancel.Enabled = HasEditAccess;
        }

        protected void SetControlStatus()
        {
            rtbRoadName.Enabled = EnableControl;
            rfvRoadName.Enabled = EnableControl;
            rddlExpressway.Enabled = EnableControl;
            rfvExpressway.Enabled = EnableControl;
            rntbBridgeWidth.Enabled = EnableControl;
            rfvBridgeWidth.Enabled = EnableControl;
            rntbWorkZone.Enabled = EnableControl;
            rfvWorkZone.Enabled = EnableControl;
            rntbLaneWidth.Enabled = EnableControl;
            rfvLaneWidth.Enabled = EnableControl;
            rntbShoulderWidth.Enabled = EnableControl;
            rfvShoulderWidth.Enabled = EnableControl;
            rntbSpeedLimit.Enabled = EnableControl;
            rfvSpeedLimit.Enabled = EnableControl;
            rntbLaneNumber.Enabled = EnableControl;
            rfvLaneNumber.Enabled = EnableControl;
            rddlFlow2Way.Enabled = EnableControl;
            rfvFlow2Way.Enabled = EnableControl;
            rddlTrafficVolume.Enabled = EnableControl;
            rfvTrafficVolume.Enabled = EnableControl;
            rddlDividedHighway.Enabled = EnableControl;
            rfvDividedHighway.Enabled = EnableControl;
            rddlMedian.Enabled = EnableControl;
            rfvMedian.Enabled = EnableControl;
            if (currSite.TCPSite != null)
            {
                var tcps = currSite.TCPSite.TCPs;
                if (tcps.FirstOrDefault(p => p.PlanID == 3) != null || tcps.FirstOrDefault(p => p.PlanID == 4) != null)
                {
                    rddlFlaggers.Enabled = EnableControl;
                    rfvFlaggers.Enabled = EnableControl;
                }
            }
            else
            {
                rddlFlaggers.Enabled = EnableControl;
                rfvFlaggers.Enabled = EnableControl;
            }
        }

        protected void ClearAllFields()
        {
            ltlLastUpdated.Text = "";
            rddlRemote.SelectedValue = "";
            rtbRoadName.Text = "";
            rddlExpressway.SelectedValue = "";
            rntbBridgeWidth.Text = "";
            rntbWorkZone.Text = "";
            rntbLaneWidth.Text = "";
            rntbShoulderWidth.Text = "";
            rntbSpeedLimit.Text = "";
            rntbLaneNumber.Text = "";
            rddlFlow2Way.SelectedValue = "";
            rddlTrafficVolume.SelectedValue = "";
            rddlDividedHighway.SelectedValue = "";
            rddlMedian.SelectedValue = "";
            rddlFlaggers.SelectedValue = "";
            rtbNotes.Text = "";
        }
        #endregion

        #region Page Events
        protected void rbSubmit_Command(object sender, CommandEventArgs e)
        {
            if (Page.IsValid)
            {
                string note = "";
                Data.TCPSite newSite;

                bool bflag;
                int iflag;

                //Setup all of the form field variables
                Boolean remotesite = Convert.ToBoolean(rddlRemote.SelectedValue);
                Boolean expressway = false;
                int bridgewidth = 0;
                int workzone = 0;
                int lanewidth = 0;
                int lanenumber = 0;
                int shoulderwidth = 0;
                int speedlimit = 0;
                Boolean flow2way = false;
                Boolean dividedhighway = false;
                Boolean median = false;
                Boolean flaggers = false;
                if (!remotesite)
                {
                    if (Boolean.TryParse(rddlExpressway.SelectedValue, out bflag)) expressway = Convert.ToBoolean(rddlExpressway.SelectedValue);
                    if (Int32.TryParse(rntbBridgeWidth.Text, out iflag)) bridgewidth = Convert.ToInt32(rntbBridgeWidth.Text);
                    if (Int32.TryParse(rntbWorkZone.Text, out iflag)) workzone = Convert.ToInt32(rntbWorkZone.Text);
                    if (Int32.TryParse(rntbLaneWidth.Text, out iflag)) lanewidth = Convert.ToInt32(rntbLaneWidth.Text);
                    if (Int32.TryParse(rntbLaneNumber.Text, out iflag)) lanenumber = Convert.ToInt32(rntbLaneNumber.Text);
                    if (Int32.TryParse(rntbShoulderWidth.Text, out iflag)) shoulderwidth = Convert.ToInt32(rntbShoulderWidth.Text);
                    if (Int32.TryParse(rntbSpeedLimit.Text, out iflag)) speedlimit = Convert.ToInt32(rntbSpeedLimit.Text);
                    if (Boolean.TryParse(rddlFlow2Way.SelectedValue, out bflag)) flow2way = Convert.ToBoolean(rddlFlow2Way.SelectedValue);
                    if (Boolean.TryParse(rddlDividedHighway.SelectedValue, out bflag)) dividedhighway = Convert.ToBoolean(rddlDividedHighway.SelectedValue);
                    if (Boolean.TryParse(rddlMedian.SelectedValue, out bflag)) median = Convert.ToBoolean(rddlMedian.SelectedValue);
                    if (Boolean.TryParse(rddlFlaggers.SelectedValue, out bflag)) flaggers = Convert.ToBoolean(rddlFlaggers.SelectedValue);
                }

                if (currSite.TCPSite == null) //If an entry for the site does not yet exist in the TCP Site table, add it
                {
                    if (Convert.ToBoolean(rddlRemote.SelectedValue))
                    {
                        newSite = new Data.TCPSite()
                            {
                                site_id = currSite.site_id,
                                RemoteSite = remotesite,
                                Notes = rtbNotes.Text.FormatParagraphIn(),
                                UpdatedBy = user.ID,
                                UpdatedDt = DateTime.Now
                            };
                    }
                    else
                    {
                        newSite = new Data.TCPSite()
                        {
                            site_id = currSite.site_id,
                            RemoteSite = remotesite,
                            RoadName = rtbRoadName.Text,
                            Expressway = expressway,
                            BridgeWidth = bridgewidth,
                            WorkZone = workzone,
                            LaneWidth = lanewidth,
                            LaneNumber = lanenumber,
                            ShoulderWidth = shoulderwidth,
                            SpeedLimit = speedlimit,
                            Flow2Way = flow2way,
                            TrafficVolume = rddlTrafficVolume.SelectedValue,
                            DividedHighway = dividedhighway,
                            Median = median,
                            Flaggers = flaggers,
                            Notes = rtbNotes.Text.FormatParagraphIn(),
                            UpdatedBy = user.ID,
                            UpdatedDt = DateTime.Now
                        };
                    }
                    db.TCPSites.InsertOnSubmit(newSite);
                }
                else //otherwise, update the record
                {
                    currSite.TCPSite.RemoteSite = remotesite;
                    currSite.TCPSite.RoadName = rtbRoadName.Text;
                    currSite.TCPSite.Expressway = expressway;
                    currSite.TCPSite.BridgeWidth = bridgewidth;
                    currSite.TCPSite.WorkZone = workzone;
                    currSite.TCPSite.LaneWidth = lanewidth;
                    currSite.TCPSite.LaneNumber = lanenumber;
                    currSite.TCPSite.ShoulderWidth = shoulderwidth;
                    currSite.TCPSite.SpeedLimit = speedlimit;
                    currSite.TCPSite.Flow2Way = flow2way;
                    currSite.TCPSite.TrafficVolume = rddlTrafficVolume.SelectedValue;
                    currSite.TCPSite.DividedHighway = dividedhighway;
                    currSite.TCPSite.Median = median;
                    currSite.TCPSite.Flaggers = flaggers;
                    currSite.TCPSite.Notes = rtbNotes.Text.FormatParagraphIn();
                    currSite.TCPSite.UpdatedBy = user.ID;
                    currSite.TCPSite.UpdatedDt = DateTime.Now;
                    newSite = currSite.TCPSite;
                }

                db.SubmitChanges();

                //Determine the plan ID to use based on the submitted site specific information
                int plan_id = PlanID(newSite);
                
                #region First, check to see if there is more than one TCP for the site
                if (newSite.TCPs.Count > 1)
                {
                    //Grab all TCPs assigned to this site, and loop through
                    var tcps = newSite.TCPs;
                    int i = 1;
                    int currPlanIndex = 0; //The index for the existing plan that matches the determined plan
                    int plan8Index = 0; //The index for plan V (too complicated)
                    //Setup the indexes
                    foreach (Data.TCP tcp in tcps)
                    {
                        if (tcp.PlanID == plan_id)
                        {
                            currPlanIndex = i;
                        }

                        if (tcp.PlanID == 8)
                        {
                            plan8Index = i;
                        }
                        i++;
                    }

                    int x = 1;
                    List<int> idsToDelete = new List<int>();
                    foreach (Data.TCP tcp in tcps)
                    {
                        //If there is no plan that matches the determined plan_id, then update the first plan that isn't plan V (too complicated), and delete the rest of the plans
                        if (currPlanIndex == 0)
                        {
                            if (x == 1 && x != plan8Index) //Update the first plan, unless it's plan V (too complicated)
                            {
                                if (plan_id == 5 && tcp.PlanID == 6) tcp.PlanID = tcp.PlanID; else tcp.PlanID = plan_id; //If the determined plan is IVa, then do not update the plan ID for IVb
                                tcp.UpdatedBy = user.ID;
                                tcp.UpdatedDt = DateTime.Now;
                                tcp.ApprovalReady = false;
                                db.SubmitChanges();
                                note = "<br /><b>ALERT:</b> Based on your submitted site specific information, the assigned TCP has been updated to " + tcp.TCPPlanDetail.Number + " - " + tcp.TCPPlanDetail.SubName + ".";
                            }
                            else if (x == 2 && plan8Index == 1) //If the first plan was plan V (too complicated), update the second plan
                            {
                                if (plan_id == 5 && tcp.PlanID == 6) tcp.PlanID = tcp.PlanID; else tcp.PlanID = plan_id; //If the determined plan is IVa, then do not update the plan ID for IVb
                                tcp.UpdatedBy = user.ID;
                                tcp.UpdatedDt = DateTime.Now;
                                tcp.ApprovalReady = false;
                                db.SubmitChanges();
                                note = "<br /><b>ALERT:</b> Based on your submitted site specific information, the assigned TCP has been updated to " + tcp.TCPPlanDetail.Number + " - " + tcp.TCPPlanDetail.SubName + ".";
                            }
                            else if (x > 1 && x != plan8Index) //If there is more than one plan assigned to the site, other than plan VI, delete from database
                            {
                                //But do not delete plan IVb if plan IVa is the determined plan!
                                if (plan_id == 5 && tcp.PlanID == 6)
                                {
                                    //skip
                                }
                                else idsToDelete.Add(tcp.TCPID);
                            }

                            //Now make sure to add plan IVb as an entirely new plan (plan IVa is already there and was updated above)
                            if (x == 1 && plan_id == 5)
                            {
                                Data.TCP newTCP = new Data.TCP()
                                {
                                    site_id = newSite.site_id,
                                    PlanID = 6,
                                    WorkAreaActivity = "",
                                    Remarks = "",
                                    UpdatedBy = user.ID,
                                    UpdatedDt = DateTime.Now,
                                    ApprovalReady = false
                                };
                                db.TCPs.InsertOnSubmit(newTCP);
                                db.SubmitChanges();
                            }
                        }
                        else //A plan was found to match the determined plan_id; update its information, and delete the rest of the plans (except plan VI)
                        {
                            if (x == currPlanIndex)
                            {
                                tcp.UpdatedBy = user.ID;
                                tcp.UpdatedDt = DateTime.Now;
                                tcp.ApprovalReady = false;
                                db.SubmitChanges();
                                note = "";
                            }
                            else if (plan_id == 5 && tcp.PlanID == 6) //Also make sure to update plan IVb if plan IVa was the determined plan
                            {
                                tcp.UpdatedBy = user.ID;
                                tcp.UpdatedDt = DateTime.Now;
                                tcp.ApprovalReady = false;
                                db.SubmitChanges();
                                note = "";
                            }
                            else if (x != currPlanIndex && x != plan8Index)
                            {
                                //But do not delete plan IVb if plan IVa is the determined plan!
                                if (plan_id == 5 && tcp.PlanID == 6)
                                {
                                    //skip
                                }
                                else idsToDelete.Add(tcp.TCPID);
                            }
                        }

                        x++;
                    }

                    //Finally, delete all the TCP collected in the idsToDelete list
                    foreach (int tcp_id in idsToDelete)
                    {
                        var deltcp = db.TCPs.FirstOrDefault(p => p.TCPID == tcp_id);
                        db.TCPs.DeleteOnSubmit(deltcp);
                        db.SubmitChanges();
                    }
                }
                #endregion
                #region If there is only one TCP
                else if (newSite.TCPs.Count == 1) 
                {
                    //Check to see if the one TCP is plan V (too complicated)
                    if (newSite.TCPs.FirstOrDefault().PlanID == 8)
                    {
                        //If the determined planID is 8, then update the plan VI
                        if (plan_id == 8)
                        {
                            var tcpVI = newSite.TCPs.FirstOrDefault();
                            tcpVI.UpdatedBy = user.ID;
                            tcpVI.UpdatedDt = DateTime.Now;
                            tcpVI.ApprovalReady = false;
                            db.SubmitChanges();
                            note = "";
                        }
                        else //Otherwise, create a new record in the TCP table for the determined plan
                        {
                            Data.TCP newTCP = new Data.TCP()
                            {
                                site_id = newSite.site_id,
                                PlanID = plan_id,
                                WorkAreaActivity = "",
                                Remarks = "",
                                UpdatedBy = user.ID,
                                UpdatedDt = DateTime.Now,
                                ApprovalReady = false
                            };
                            db.TCPs.InsertOnSubmit(newTCP);
                            db.SubmitChanges();

                            if (plan_id == 5) //If adding plan IVa, also add plan IVb
                            {
                                Data.TCP newTCPIVb = new Data.TCP()
                                {
                                    site_id = newSite.site_id,
                                    PlanID = 6,
                                    WorkAreaActivity = "",
                                    Remarks = "",
                                    UpdatedBy = user.ID,
                                    UpdatedDt = DateTime.Now,
                                    ApprovalReady = false
                                };
                                db.TCPs.InsertOnSubmit(newTCPIVb);
                                db.SubmitChanges();
                            }

                            note = "<br /><b>AlERT:</b> A new TCP was created for this site. You can view it by clicking the link under the Traffic Control Plan section below.";
                        }
                    }
                    else if (plan_id == 5) //Check to see if the determined plan is IVa
                    {
                        int pn = 6;
                        if (newSite.TCPs.FirstOrDefault().PlanID == 6) pn = 5;

                        //Update the one plan that's there, and then add the one that is missing
                        var updtTcp = newSite.TCPs.FirstOrDefault();
                        if (pn == 6) updtTcp.PlanID = 5;
                        updtTcp.UpdatedBy = user.ID;
                        updtTcp.UpdatedDt = DateTime.Now;
                        updtTcp.ApprovalReady = false;
                        db.SubmitChanges();

                        Data.TCP newTCPIV = new Data.TCP();
                        newTCPIV.site_id = newSite.site_id;
                        newTCPIV.PlanID = pn;
                        newTCPIV.UpdatedBy = user.ID;
                        newTCPIV.UpdatedDt = DateTime.Now;
                        newTCPIV.ApprovalReady = false;
                        db.TCPs.InsertOnSubmit(newTCPIV);
                        db.SubmitChanges();
                        note = "<br /><b>ALERT:</b> A new TCP was created for this site. You can view it by clicking the link under the Traffic Control Plan section below.";

                    }
                    else //Otherwise, just update the plan
                    {
                        var updtTcp = newSite.TCPs.FirstOrDefault();
                        updtTcp.PlanID = plan_id;
                        updtTcp.UpdatedBy = user.ID;
                        updtTcp.UpdatedDt = DateTime.Now;
                        updtTcp.ApprovalReady = false;
                        db.SubmitChanges();
                        note = "";
                    }
                }
                #endregion
                #region If no record is in the TCP Site Plan table yet, then add a new one
                else 
                {
                    Data.TCP newTCP = new Data.TCP()
                    {
                        site_id = newSite.site_id,
                        PlanID = plan_id,
                        WorkAreaActivity = "",
                        Remarks = "",
                        UpdatedBy = user.ID,
                        UpdatedDt = DateTime.Now,
                        ApprovalReady = false
                    };
                    db.TCPs.InsertOnSubmit(newTCP);
                    db.SubmitChanges();

                    //If they have entered information to return plan IVa or IVb (PlanIDs 5 & 6), then will need to add both (PlanID 5 already added above)
                    if (plan_id == 5)
                    {
                        newTCP = new Data.TCP()
                        {
                            site_id = newSite.site_id,
                            PlanID = 6,
                            WorkAreaActivity = "",
                            Remarks = "",
                            UpdatedBy = user.ID,
                            UpdatedDt = DateTime.Now,
                            ApprovalReady = false
                        };
                        db.TCPs.InsertOnSubmit(newTCP);
                        db.SubmitChanges();

                        note = "<br /><b>AlERT:</b> Two new TCPs were created for this site. You can view them by clicking the links under the Traffic Control Plan section below. <b>Refresh your browser if you do not see the new plans.</b>";
                    }
                    else
                    {
                        note = "<br /><b>AlERT:</b> A new TCP was created for this site. You can view it by clicking the link under the Traffic Control Plan section below. <b>Refreh your browser if you do not see the new plan.</b>";
                    }
                }
                #endregion

                #region Upload the plan document if one is present
                var planV = currSite.TCPSite.TCPs.FirstOrDefault(p => p.PlanID == 8);
                if (planV != null)
                {
                    if (ruFile != null)
                    {
                        if (ruFile.UploadedFiles.Count >= 1)
                        {
                            var uf = ruFile.UploadedFiles[0];
                            using (MemoryStream ms = new MemoryStream())
                            {
                                uf.InputStream.CopyTo(ms);
                                planV.PlanFile = ms.ToArray();
                            }
                            db.SubmitChanges();
                        }
                    }
                }
                #endregion

                ltlLastUpdated.Text = "This information was lasted updated on " + String.Format("{0:MM/dd/yyyy}", DateTime.Now) + " at " + String.Format("{0:h:mm tt}", DateTime.Now) + " by " + user.ID + "."; 
                ltlNote.Text = "<span style='color:green;'>Changes to the site specific information saved! " + note + "</span>";

                dlTCPs.DataSource = TCPDataSource();
                dlTCPs.DataBind();
            }
            else
            {
                ltlNote.Text = "<span style='color:red;'>Required field is empty!</span>";
            }
        }

        protected void rbCancel_Command(object sender, CommandEventArgs e)
        {
            //If a site is found in the TCP_Site_Master table, go ahead and populate the data in the form fields
            if (currSite.TCPSite != null) PopulatePageData(); else ClearAllFields();
        }

        protected void rddlRemote_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rddlRemote.SelectedValue == "True")
            {
                EnableControl = false;
                SetControlStatus();
                lblRemoteSiteNote.Visible = true;
            }
            else
            {
                EnableControl = true;
                SetControlStatus();
                lblRemoteSiteNote.Visible = false;
            }
        }

        protected void lbAddPlanV_Command(object sender, CommandEventArgs e)
        {
            Data.TCP newTCPV = new Data.TCP()
            {
                site_id = currSite.site_id,
                PlanID = 8,
                UpdatedBy = user.ID,
                UpdatedDt = DateTime.Now,
                ApprovalReady = false
            };
            db.TCPs.InsertOnSubmit(newTCPV);
            db.SubmitChanges();

            dlTCPs.DataSource = TCPDataSource();
            dlTCPs.DataBind();

            lbAddPlanV.Visible = false;
            imgBullet.Visible = false;

            ltlPlanVDoc.Visible = true;
            pnlFileUpload.Visible = true;
            imgUploadDocHelp.Visible = true;
        }

        protected void RunPlanLogic(object sender, EventArgs e)
        {
            switch (sender.GetType().Name)
            {
                case "RadNumericTextBox":
                    RadNumericTextBox rntb = (RadNumericTextBox)sender;

                    //If changing the shoulder width
                    if (rntb.ID == "rntbShoulderWidth")
                    {
                        if (!string.IsNullOrEmpty(rntbLaneNumber.Text) && !string.IsNullOrEmpty(rddlFlow2Way.SelectedValue))
                        {
                            if (Convert.ToInt32(rntb.Text) < ShoulderRule)
                            {
                                if (Convert.ToInt32(rntbLaneNumber.Text) == 2 && Convert.ToBoolean(rddlFlow2Way.SelectedValue))
                                {
                                    rddlFlaggers.Enabled = true;
                                    rfvFlaggers.Enabled = true;
                                }
                                else
                                {
                                    rddlFlaggers.Enabled = false;
                                    rfvFlaggers.Enabled = false;
                                }
                            }
                            else
                            {
                                rddlFlaggers.Enabled = false;
                                rfvFlaggers.Enabled = false;
                            }
                        }
                    }
                    else if (rntb.ID == "rntbLaneNumber") //If changing the lane number
                    {
                        if (!string.IsNullOrEmpty(rntbShoulderWidth.Text) && !string.IsNullOrEmpty(rddlFlow2Way.SelectedValue))
                        {
                            if (Convert.ToInt32(rntbShoulderWidth.Text) < ShoulderRule)
                            {
                                if (Convert.ToInt32(rntb.Text) == 2 && Convert.ToBoolean(rddlFlow2Way.SelectedValue))
                                {
                                    rddlFlaggers.Enabled = true;
                                    rfvFlaggers.Enabled = true;
                                }
                                else
                                {
                                    rddlFlaggers.Enabled = false;
                                    rfvFlaggers.Enabled = false;
                                }
                            }
                        }
                    }
                    break;
                case "RadDropDownList":
                    RadDropDownList rddl = (RadDropDownList)sender;

                    //If changing the traffic flow
                    if (!string.IsNullOrEmpty(rntbShoulderWidth.Text) && !string.IsNullOrEmpty(rntbLaneNumber.Text))
                    {
                        if (Convert.ToInt32(rntbShoulderWidth.Text) < ShoulderRule)
                        {
                            if (Convert.ToInt32(rntbLaneNumber.Text) == 2 && Convert.ToBoolean(rddl.SelectedValue))
                            {
                                rddlFlaggers.Enabled = true;
                                rfvFlaggers.Enabled = true;
                            }
                            else
                            {
                                rddlFlaggers.Enabled = false;
                                rfvFlaggers.Enabled = false;
                            }
                        }
                    }
                    break;
            }
        }
        #endregion

        #region Data List Events
        protected void dlTCPs_EditCommand(object sender, DataListCommandEventArgs e)
        {
            dlTCPs.EditItemIndex = e.Item.ItemIndex;
            dlTCPs.DataSource = TCPDataSource();
            dlTCPs.DataBind();
        }

        protected void dlTCPs_CancelCommand(object sender, DataListCommandEventArgs e)
        {
            dlTCPs.EditItemIndex = -1;
            dlTCPs.DataSource = TCPDataSource();
            dlTCPs.DataBind();
        }

        protected void dlTCPs_UpdateCommand(object sender, DataListCommandEventArgs e)
        {
            int TCPID = (int)dlTCPs.DataKeys[e.Item.ItemIndex];
            string plan_remarks = ((RadTextBox)e.Item.FindControl("rtbPlanRemarks")).Text;
            string waa = ((RadDropDownList)e.Item.FindControl("rddlWAA")).SelectedValue.ToString();

            var TCP = db.TCPs.FirstOrDefault(p => p.TCPID == TCPID);
            TCP.Remarks = plan_remarks.FormatParagraphIn();
            TCP.WorkAreaActivity = waa;
            TCP.UpdatedBy = user.ID;
            TCP.UpdatedDt = DateTime.Now;
            TCP.ApprovalReady = false;

            db.SubmitChanges();

            dlTCPs.EditItemIndex = -1;
            dlTCPs.DataSource = TCPDataSource();
            dlTCPs.DataBind();
        }

        protected void dlTCPs_DeleteCommand(object sender, DataListCommandEventArgs e)
        {
            int TCPID = (int)dlTCPs.DataKeys[e.Item.ItemIndex];
            var TCP = db.TCPs.FirstOrDefault(p => p.TCPID == TCPID);

            //If deleting Plan V, then remove the Upload Plan V Document controls from the Site Specific Info section
            if (TCP.PlanID == 8)
            {
                ltlPlanVDoc.Visible = false;
                pnlFileUpload.Visible = false;
                imgUploadDocHelp.Visible = false;
            }

            db.TCPs.DeleteOnSubmit(TCP);
            db.SubmitChanges();

            dlTCPs.EditItemIndex = -1;
            dlTCPs.DataSource = TCPDataSource();
            dlTCPs.DataBind();
        }

        protected void dlTCPs_ItemCreated(object sender, DataListItemEventArgs e)
        {
            switch (e.Item.ItemType)
            {
                case ListItemType.Item:
                case ListItemType.AlternatingItem:
                    try
                    {
                        LinkButton delBtn = (LinkButton)e.Item.FindControl("lbDelete");
                        delBtn.Attributes.Add("onclick",
                           "return confirm('WARNING: You will lose all plan related data, including review and approve information, if you delete this plan. " +
                           "After deleting, to re-add a TCP to the site, click the Submit Changes button under the Site Specific Information. " +
                           "Click Okay to delete, or Cancel.');");
                    }
                    catch { }
                    break;
            }
        }

        protected void dlTCPs_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            int TCPID = (int)dlTCPs.DataKeys[e.Item.ItemIndex];
            var TCP = db.TCPs.FirstOrDefault(p => p.TCPID == TCPID);

            if (e.Item.ItemType == ListItemType.EditItem)
            {
                RadDropDownList rddlWAA = (RadDropDownList)e.Item.FindControl("rddlWAA");
                Panel pnlUploadPlan = (Panel)e.Item.FindControl("pnlUploadPlan");

                if (!string.IsNullOrEmpty(TCP.WorkAreaActivity))
                    rddlWAA.SelectedValue = TCP.WorkAreaActivity;
            }

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Panel pnlDownloadPlan = (Panel)e.Item.FindControl("pnlDownloadPlan");
                HyperLink hlDownloadPlan = (HyperLink)e.Item.FindControl("hlDownloadPlan");
                Literal ltlUploadPlan = (Literal)e.Item.FindControl("ltlUploadPlan");

                if (TCP.PlanID == 8)
                {
                    pnlDownloadPlan.Visible = true;
                    if (TCP.PlanFile != null)
                    {
                        hlDownloadPlan.NavigateUrl = String.Format("{0}Handler/DocHandler.ashx?task=getTCP&TCPID={1}", Config.SIMSURL, TCP.TCPID);
                        hlDownloadPlan.Visible = true;
                        ltlUploadPlan.Visible = false;
                    }
                    else
                    {
                        hlDownloadPlan.Visible = false;
                        ltlUploadPlan.Visible = true;
                    }
                }
                else pnlDownloadPlan.Visible = false;
            }
        }
        #endregion

        #region Functions
        /// <summary>
        /// Returns the appropriate PlanID based on the submitted Site Specific Information
        /// </summary>
        /// <param name="ts">TCPSite data type</param>
        private int PlanID(Data.TCPSite ts)
        {
            int ret = 8;

            if ((bool)ts.RemoteSite) 
                ret = 1; //If remote site, then use plan 0
            else
                if (ts.ShoulderWidth < ShoulderRule)
                {
                    if (ts.LaneNumber == 4 && (bool)ts.Flow2Way) //If 4-lane, 2-way, use plan II
                    {
                        if ((bool)ts.DividedHighway) ret = 7; else ret = 2; //If multi-lane, divided highway, use plan III
                    }
                    else if (ts.LaneNumber == 2 && (bool)ts.Flow2Way) //If 2-lane, 2-way
                    {
                        if ((bool)ts.Flaggers) ret = 3; else ret = 4; //Determined by the presence of flaggers whether plan Ia or Ib is used
                    }
                    else if ((bool)ts.DividedHighway && ts.LaneNumber > 1) //If multi-lane, divided highway, use plan III
                    {
                        ret = 7;
                    }
                    else //Plan is too complicated, use plan V
                    {
                        ret = 8;
                    }
                }
                else //If shoulder is > ShoulderRule and short duration (time < 1 hr) is true, use plan IVa, else use plan IVb
                {
                    //Add both plan 5 and 6
                    ret = 5;
                }

            return ret;
        }
        #endregion
    }
}