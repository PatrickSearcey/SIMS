using Core;
using System;
using System.Collections.Generic;
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
            if (!string.IsNullOrEmpty(site_id)) SiteID = Convert.ToInt32(site_id); else Response.Redirect(Config.SIMS2017URL + "SIMSWSCHome.aspx");

            //Using the passed site_id, setup the site data element, and reset the office and wsc to match that of the current site
            currSite = db.Sites.Where(p => p.site_id == SiteID).FirstOrDefault();
            OfficeID = (int)currSite.office_id;
            WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            ph1.Title = "Manage Traffic Control Safety Plans";
            ph1.SubTitle = currSite.site_no + " " + currSite.station_full_nm;

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
            return currSite.TCPSite.TCPs.Select(p => new TCPDataItem
                {
                    TCPID = p.TCPID,
                    TCPLink = String.Format("{0}TCPView.aspx?TCPID={1}", Config.SafetyURL, p.TCPID),
                    TCPName = String.Format("{0} - TCP, {1}", p.TCPPlanDetail.Number, p.TCPPlanDetail.SubName),
                    PlanRemarks = p.Remarks,
                    WorkAreaActivity = p.WorkAreaActivity
                });
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
            //Site Specific Information
            ltlLastUpdated.Text = "This information was lasted updated on " + String.Format("{0:MM/dd/yyyy}", currSite.TCPSite.UpdatedDt) + " at " + String.Format("{0:h:mm tt}", currSite.TCPSite.UpdatedDt) + " by " + currSite.TCPSite.UpdatedBy + "."; 
            if (currSite.TCPSite.RemoteSite != null)
            {
                rddlRemote.SelectedValue = currSite.TCPSite.RemoteSite.ToString();
                if ((bool)currSite.TCPSite.RemoteSite)
                {
                    EnableControl = false;
                    SetControlStatus();
                    return;
                }
            }
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
            rtbNotes.Text = currSite.TCPSite.Notes;

            //Set control status
            if (currSite.TCPSite.ShoulderWidth != null)
            {
                if (currSite.TCPSite.ShoulderWidth < 5)
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

            //Plan Specific Information
            dlTCPs.DataSource = TCPDataSource();
            dlTCPs.DataBind();
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
            if (rddlFlaggers.Enabled)
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
                Data.TCPSite newSite = new Data.TCPSite();

                if (currSite.TCPSite == null) //If an entry for the site does not yet exist in the TCP Site table, add it
                {
                    newSite = new Data.TCPSite()
                        {
                            RemoteSite = Convert.ToBoolean(rddlRemote.SelectedValue),
                            RoadName = rtbRoadName.Text,
                            Expressway = Convert.ToBoolean(rddlExpressway.SelectedValue),
                            BridgeWidth = Convert.ToInt32(rntbBridgeWidth.Text),
                            WorkZone = Convert.ToInt32(rntbWorkZone.Text),
                            LaneWidth = Convert.ToInt32(rntbLaneWidth.Text),
                            LaneNumber = Convert.ToInt32(rntbLaneNumber.Text),
                            ShoulderWidth = Convert.ToInt32(rntbShoulderWidth.Text),
                            SpeedLimit = Convert.ToInt32(rntbSpeedLimit.Text),
                            Flow2Way = Convert.ToBoolean(rddlFlow2Way.SelectedValue),
                            TrafficVolume = rddlTrafficVolume.SelectedValue,
                            DividedHighway = Convert.ToBoolean(rddlDividedHighway.SelectedValue),
                            Median = Convert.ToBoolean(rddlMedian.SelectedValue),
                            Flaggers = Convert.ToBoolean(rddlFlaggers.SelectedValue),
                            Notes = rtbNotes.Text,
                            UpdatedBy = user.ID,
                            UpdatedDt = DateTime.Now
                        };
                    db.TCPSites.InsertOnSubmit(newSite);
                }
                else //otherwise, update the record
                {
                    currSite.TCPSite.RemoteSite = Convert.ToBoolean(rddlRemote.SelectedValue);
                    currSite.TCPSite.RoadName = rtbRoadName.Text;
                    currSite.TCPSite.Expressway = Convert.ToBoolean(rddlExpressway.SelectedValue);
                    currSite.TCPSite.BridgeWidth = Convert.ToInt32(rntbBridgeWidth.Text);
                    currSite.TCPSite.WorkZone = Convert.ToInt32(rntbWorkZone.Text);
                    currSite.TCPSite.LaneWidth = Convert.ToInt32(rntbLaneWidth.Text);
                    currSite.TCPSite.LaneNumber = Convert.ToInt32(rntbLaneNumber.Text);
                    currSite.TCPSite.ShoulderWidth = Convert.ToInt32(rntbShoulderWidth.Text);
                    currSite.TCPSite.SpeedLimit = Convert.ToInt32(rntbSpeedLimit.Text);
                    currSite.TCPSite.Flow2Way = Convert.ToBoolean(rddlFlow2Way.SelectedValue);
                    currSite.TCPSite.TrafficVolume = rddlTrafficVolume.SelectedValue;
                    currSite.TCPSite.DividedHighway = Convert.ToBoolean(rddlDividedHighway.SelectedValue);
                    currSite.TCPSite.Median = Convert.ToBoolean(rddlMedian.SelectedValue);
                    currSite.TCPSite.Flaggers = Convert.ToBoolean(rddlFlaggers.SelectedValue);
                    currSite.TCPSite.Notes = rtbNotes.Text;
                    currSite.TCPSite.UpdatedBy = user.ID;
                    currSite.TCPSite.UpdatedDt = DateTime.Now;
                    newSite = currSite.TCPSite;
                }

                db.SubmitChanges();

                //Determine the plan ID to use based on the submitted site specific information
                int plan_id = PlanID(newSite);
                //First, check to see if there is more than one TCP for the site
                if (newSite.TCPs.Count > 1)
                {
                    //Grab all TCPs assigned to this site, and loop through
                    var tcps = newSite.TCPs;
                    int i = 1;
                    int currPlanIndex = 0; //The index for the existing plan that matches the determined plan
                    int plan8Index = 0; //The index for plan IV (too complicated)
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
                        //If there is no plan that matches the determined plan_id, then update the first plan that isn't plan IV (too complicated), and delete the rest of the plans
                        if (currPlanIndex == 0)
                        {
                            if (x == 1 && x != plan8Index)
                            {
                                if (plan_id == 5 && tcp.PlanID == 6) tcp.PlanID = tcp.PlanID; else tcp.PlanID = plan_id; //If the determined plan is IVa, then do not update the plan ID for IVb
                                tcp.UpdatedBy = user.ID;
                                tcp.UpdatedDt = DateTime.Now;
                                tcp.ApprovalReady = false;
                                db.SubmitChanges();
                                note = "<br /><b>ALERT:</b> Based on your submitted site specific information, the assigned TCP has been updated to " + tcp.TCPPlanDetail.Number + " - " + tcp.TCPPlanDetail.SubName + ".";
                            }
                            else if (x == 2 && plan8Index == 1)
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
                                //Do not delete plan IVb if plan IVa is the determined plan!
                                idsToDelete.Add(tcp.TCPID);
                                if (plan_id == 5) idsToDelete.Remove(6);
                            }

                            //Now make sure to add plan IVa as an entirely new plan (plan IVb is already there and was updated above)
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
                        else //A plan was found to match the determined plan_id; update its information, and delete the rest of the plans (except plan IV)
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
                                //Do not delete plan IVb if plan IVa is the determined plan!
                                idsToDelete.Add(tcp.TCPID);
                                if (plan_id == 5) idsToDelete.Remove(6);
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
                else if (newSite.TCPs.Count == 1) //If there is only one TCP
                {
                    //Check to see if the one TCP is plan VI (too complicated)
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

                            note = "<br /><b>AlERT:</b> A new TCP was created for this site. You can view it by clicking the link under the Traffic Control Plan section below. Refresh your browser if you do not see the new plan.";
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
                        note = "<br /><b>ALERT:</b> A new TCP was created for this site. You can view it by clicking the link under the Traffic Control Plan section below.  Refresh your browser if you do not see the new plan.";

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
                else //If no record is in the TCP Site Plan table yet, then add a new one
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

                        note = "<br /><b>AlERT:</b> Two new TCPs were created for this site. You can view them by clicking the links under the Traffic Control Plan section below.";
                    }
                    else
                    {
                        note = "<br /><b>AlERT:</b> A new TCP was created for this site. You can view it by clicking the link under the Traffic Control Plan section below.";
                    }
                }
                
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
            }
            else
            {
                EnableControl = true;
                SetControlStatus();
            }
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
                        if (Convert.ToInt32(rntb.Text) < 5)
                        {
                            if (Convert.ToInt32(rntbLaneNumber.Text) == 2 && Convert.ToBoolean(rddlFlow2Way.SelectedValue))
                            {
                                rddlFlaggers.Enabled = true;
                                rfvFlaggers.Enabled = true;
                            }
                        }
                        else
                        {
                            rddlFlaggers.Enabled = false;
                            rfvFlaggers.Enabled = false;
                        }
                    }
                    else if (rntb.ID == "rntbLaneNumber") //If changing the lane number
                    {
                        if (Convert.ToInt32(rntbShoulderWidth.Text) < 5)
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
                    break;
                case "RadDropDownList":
                    RadDropDownList rddl = (RadDropDownList)sender;

                    //If changing the traffic flow
                    if (Convert.ToInt32(rntbShoulderWidth.Text) < 5)
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
            string waa = ((RadTextBox)e.Item.FindControl("rtbWAA")).Text;

            var TCP = db.TCPs.FirstOrDefault(p => p.TCPID == TCPID);
            TCP.Remarks = plan_remarks;
            TCP.WorkAreaActivity = waa;
            TCP.UpdatedBy = user.ID;
            TCP.UpdatedDt = DateTime.Now;
            TCP.ApprovalReady = false;

            db.SubmitChanges();

            dlTCPs.EditItemIndex = -1;
            dlTCPs.DataSource = TCPDataSource();
            dlTCPs.DataBind();
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

            if ((bool)ts.RemoteSite) ret = 1; //If remote site, then use plan 0
            if (ts.ShoulderWidth == 0) //If work is beyond shoulder, use plan V
            {
                ret = 9; 
            }
            else if (ts.ShoulderWidth < 5)
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
                else //Plan is too complicated, use plan VI
                {
                    ret = 8;
                }
            }
            else //If shoulder is > 5ft and short duration (time < 1 hr) is true, use plan IVa, else use plan IVb
            {
                //Add both plan 5 and 6
                ret = 5;
            }

            return ret;
        }
        #endregion
    }
}