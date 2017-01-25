using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SIMS2017.StationDoc
{
    public partial class EditDocs : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        private SIMSDevService.SIMSServiceClient svcSIMS = new SIMSDevService.SIMSServiceClient();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        public Boolean HasEditAccess { get; set; }
        private Data.Site currSite;
        private int element_id;
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

            ph1.Title = "Edit Station Documents";
            ph1.SubTitle = currSite.site_no + " " + currSite.station_full_nm;

            if (!Page.IsPostBack)
            {
                //If the user belongs to this site's WSC (or has an exception to work in the WSC), or is a SuperUser, then allow them to edit the page
                if (user.WSCID.Contains(WSCID) || user.IsSuperUser) HasEditAccess = true;

                InitialPageSetup();
            }
        }

        #region Page Methods
        protected void InitialPageSetup()
        {
            if (HasEditAccess)
            {
                string swr_url = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).swr_url;
                if (!string.IsNullOrEmpty(swr_url))
                {
                    hlAutoReview.NavigateUrl = String.Format("javascript:OpenSWR('{0}{1}/')", swr_url, currSite.site_no.Replace(" ", ""));
                }
                else hlAutoReview.Visible = false;

                ResetPanels();
            }
            else
            {
                pnlChooseAction.Visible = false;
                pnlChooseElement.Visible = false;
                pnlResult.Visible = false;
                pnlNoAccess.Visible = true;
            }
        }

        protected void ResetPanels()
        {
            foreach (ButtonListItem action in rrblAction.Items)
                action.Selected = false;

            rddlElements.SelectedIndex = 0;

            pnlResult.Visible = false;
            rddlElements.Enabled = false;
            pnlNoAccess.Visible = false;
        }

        protected void DisplayNote(string action)
        {
            switch (action)
            {
                case "Add":
                    ltlNote.Text = "The element was added.";
                    break;
                case "Edit":
                    ltlNote.Text = "The element was edited.";
                    break;
                case "Delete":
                    ltlNote.Text = "The element has been backed up and deleted.";
                    break;
            }
            pnlNote.Visible = true;
        }

        protected void DisplayError(string msg)
        {
            ltlError1.Text = msg;
            ltlError2.Text = msg;
        }
        #endregion
        
        #region Page Events
        protected void UpdateControls(object sender, EventArgs e)
        {
            switch (sender.GetType().Name)
            {
                case "RadRadioButtonList":
                    RadioButton_Logic((RadRadioButtonList)sender);
                    break;
                case "RadDropDownList":
                    DropDownList_Logic((RadDropDownList)sender);
                    break;
            }
        }

        protected void RadioButton_Logic(RadRadioButtonList rrbl)
        {
            UpdateElementPanel();
            pnlResult.Visible = false;
            pnlNote.Visible = false;
        }

        protected void DropDownList_Logic(RadDropDownList rddl)
        {
            if (!string.IsNullOrEmpty(rddl.SelectedValue))
                element_id = Convert.ToInt32(rddl.SelectedValue);

            if (element_id > 0)
            {
                pnlResult.Visible = true;
                var element = db.ElementDetails.FirstOrDefault(p => p.element_id == element_id);

                //Enable all fields that may possibily have been disabled during DOLL logic below
                rdpLastRunDt.Enabled = true;
                rntbFrequency.Enabled = true;
                cbCloseLevels.Enabled = true;
                rtbRevisedBy.Enabled = true;
                reElementInfo.Visible = true;
                rbSubmit.Visible = true;
                rbReset.Visible = true;
                rbCancel1.Visible = true; 
                ltlRevisedBy.Visible = true;
                hlArchives.Visible = true;

                //Handle Auto Generated Info
                //LOCATION, DRAINAGE AREA, WELL CHARACTERISTICS (MANU), DATUM, and DATE OF LAST LEVELS
                if (element_id == 28 || element_id == 17 || element_id == 106 || element_id == 104 || element_id == 9) 
                {
                    pnlAutoGenerated.Visible = true;
                    string ag_info = "&nbsp;";
                    ltlAutoGenerated.Text = ag_info.FormatElementInfo(element_id, currSite.site_id);
                    
                    //DATE OF LAST LEVELS
                    if (element_id == 9) pnlDOLL.Visible = true; else pnlDOLL.Visible = false;
                }
                else
                {
                    pnlAutoGenerated.Visible = false;
                    pnlDOLL.Visible = false;
                }

                switch (rrblAction.SelectedValue)
                {
                    case "Add":
                        //Populate fields
                        ltlResultHeading.Text = "Add element: " + element.element_nm;
                        rtbRevisedBy.Text = user.ID;
                        rtbRevisedDate.Text = String.Format("{0:MM/dd/yyyy}", DateTime.Now);
                        reElementInfo.Content = "&nbsp;";

                        //Setup control visibility 
                        pnlEditAddElementInfo.Visible = true;
                        pnlDeleteElement.Visible = false;
                        ltlRevisedBy.Visible = false;
                        hlArchives.Visible = false;
                        break;
                    case "Edit":
                        //Populate fields
                        ltlRevisedBy.Text = String.Format("Last Revised By: {0} &nbsp;&nbsp;Date Last Revised: {1:MM/dd/yyyy}", currSite.SiteElements.FirstOrDefault(p => p.element_id == element_id).revised_by, currSite.SiteElements.FirstOrDefault(p => p.element_id == element_id).revised_dt);
                        hlArchives.NavigateUrl = String.Format("Archive.aspx?element_id={0}&site_id={1}", element_id, currSite.site_id);
                        ltlResultHeading.Text = "Edit element: " + element.element_nm;
                        rtbRevisedBy.Text = user.ID;
                        rtbRevisedDate.Text = String.Format("{0:MM/dd/yyyy}", DateTime.Now);
                        reElementInfo.Content = currSite.SiteElements.FirstOrDefault(p => p.element_id == element_id).element_info.FormatParagraphEdit();
                        //DATE OF LAST LEVELS
                        if (element_id == 9)
                        {
                            Boolean slapauto = false;
                            var slap = db.vSLAPAutoSIMs.FirstOrDefault(p => p.sims_site_id == currSite.site_id);
                            if (slap != null)
                                if (Convert.ToBoolean(slap.sims_auto)) slapauto = true;
                            rdpLastRunDt.SelectedDate = currSite.OpsLevel.levels_dt;
                            rntbFrequency.Value = currSite.OpsLevel.levels_freq;
                            cbCloseLevels.Checked = Convert.ToBoolean(currSite.OpsLevel.levels_closed);
                            //If the auto-populate SIMS DATE OF LAST LEVELS with SLAP info has been turned on, then show the information but make the fields read-only
                            if (slapauto)
                            {
                                rdpLastRunDt.Enabled = false;
                                rntbFrequency.Enabled = false;
                                cbCloseLevels.Enabled = false;
                                rtbRevisedBy.Enabled = false;
                                reElementInfo.Visible = false;
                                rbSubmit.Visible = false;
                                rbReset.Visible = false;
                                rbCancel1.Visible = false;
                                //DOLL Specific
                                ltlDOLLInfo.Text = currSite.SiteElements.FirstOrDefault(p => p.element_id == element_id).element_info.FormatParagraphOut();
                                ltlSLAP.Text = "<p style='font-weight:bold;'><b>DATE OF LAST LEVELS is auto-generated from information managed in SLAP. ";
                                hlSLAP.Text = "Go to SLAP now!</p>";
                                hlSLAP.NavigateUrl = String.Format("{0}EventManager.aspx?sims_site_id={1}", Config.SLAPURL, currSite.site_id);
                            }
                            else
                            {
                                ltlDOLLInfo.Text = "";
                                ltlSLAP.Text = "";
                                hlSLAP.Text = "";
                            }
                        }

                        //Setup control visibility
                        pnlEditAddElementInfo.Visible = true;
                        pnlDeleteElement.Visible = false;
                        break;
                    case "Delete":
                        //Populate fields
                        ltlResultHeading.Text = "Delete element: " + element.element_nm;
                        rtbElementInfo.Text = currSite.SiteElements.FirstOrDefault(p => p.element_id == element_id).element_info.FormatParagraphTextBox();

                        //Setup control visibility
                        ltlRevisedBy.Visible = false;
                        hlArchives.Visible = false;
                        pnlEditAddElementInfo.Visible = false;
                        pnlDeleteElement.Visible = true;
                        break;
                }
            }
        }

        protected void ButtonCommand(object sender, CommandEventArgs e)
        {
            element_id = Convert.ToInt32(rddlElements.SelectedValue);

            switch (e.CommandArgument.ToString())
            {
                case "Submit":
                    if (element_id == 0) DisplayError("You must select an element from the drop-down list above.");
                    else if (string.IsNullOrEmpty(rtbRevisedBy.Text)) DisplayError("You must enter a user ID into the Revised By field.");
                    else
                    {
                        //Submit the modified or added element info to the database
                        if (rrblAction.SelectedValue == "Add")
                            AddElement();
                        else if (rrblAction.SelectedValue == "Edit")
                            EditElement();
                        DisplayNote(rrblAction.SelectedValue);
                        ResetPanels();
                    }
                    break;
                case "Reset":
                    //Replace the element info inside of the reElementInfo content with the element info in the database
                    reElementInfo.Content = currSite.SiteElements.FirstOrDefault(p => p.element_id == element_id).element_info;
                    break;
                case "Delete":
                    //Back up and delete the element from the database for this site
                    BackupElement();
                    DeleteElement();
                    UpdateElementReportSummary();
                    //Handle the confirmation note and panel settings
                    DisplayNote(rrblAction.SelectedValue);
                    ResetPanels();
                    break;
                case "Cancel":
                    ResetPanels();
                    break;
            }
        }
        #endregion

        #region Methods for Database Changes
        protected void AddElement()
        {
            string element_info = "&nbsp;";
            if (!string.IsNullOrEmpty(reElementInfo.Content))
                element_info = reElementInfo.Content;

            Data.SiteElement elem = new Data.SiteElement()
            {
                site_id = currSite.site_id,
                element_id = element_id,
                element_info = element_info.FormatParagraphIn(),
                revised_by = rtbRevisedBy.Text,
                revised_dt = DateTime.Now,
                entered_by = user.ID,
                entered_dt = DateTime.Now
            };

            db.SiteElements.InsertOnSubmit(elem);
            db.SubmitChanges();

            if (element_id == 9)
            {
                UpdateDateOfLastLevels();
            }

            UpdateMaxRevisedDate();
        }

        protected void EditElement()
        {
            //Grab the element object using the element_id from the rddlElement drop down
            var elem = currSite.SiteElements.FirstOrDefault(p => p.element_id == element_id);

            string element_info = "&nbsp;";
            if (!string.IsNullOrEmpty(reElementInfo.Content))
                element_info = reElementInfo.Content;

            elem.element_info = element_info.FormatParagraphIn();
            elem.revised_by = rtbRevisedBy.Text;
            elem.revised_dt = DateTime.Now;

            db.SubmitChanges();

            if (element_id == 9)
            {
                UpdateDateOfLastLevels();
            }

            UpdateMaxRevisedDate();
        }

        protected void UpdateDateOfLastLevels()
        {
            var doll = currSite.OpsLevel;

            DateTime? levels_dt = rdpLastRunDt.SelectedDate;
            short? levels_freq = Convert.ToInt16(rntbFrequency.Value);
            Boolean levels_closed = cbCloseLevels.Checked;

            if (doll == null)
            {
                Data.OpsLevel ol = new Data.OpsLevel()
                {
                    site_id = currSite.site_id,
                    levels_dt = levels_dt,
                    levels_freq = levels_freq,
                    levels_closed = levels_closed
                };

                db.OpsLevels.InsertOnSubmit(ol);
            }
            else
            {
                doll.levels_dt = levels_dt;
                doll.levels_freq = levels_freq;
                doll.levels_closed = levels_closed;
            }

            db.SubmitChanges();
        }

        /// <summary>
        /// Run the stored procedure for updating the max revised date of Elem_Report_Sum
        /// </summary>
        protected void UpdateMaxRevisedDate()
        {
            db.SP_Report_Update_Site_LastEdited(currSite.site_id);
        }

        /// <summary>
        /// Add a record to the Elem_Site_Element_Backup table
        /// </summary>
        protected void BackupElement()
        {
            var oldElem = currSite.SiteElements.FirstOrDefault(p => p.element_id == element_id);

            Data.SiteElementBackup backup = new Data.SiteElementBackup();

            backup.site_id = currSite.site_id;
            backup.element_id = element_id;
            if (element_id == 28 || element_id == 17)
                backup.element_info = oldElem.element_info.FormatElementInfo(element_id, currSite.site_id);
            else
                backup.element_info = oldElem.element_info;
            backup.revised_by = oldElem.revised_by;
            backup.revised_dt = oldElem.revised_dt;
            backup.entered_by = oldElem.entered_by;
            backup.entered_dt = oldElem.entered_dt;
            backup.backup_by = user.ID;
            backup.backup_dt = DateTime.Now;

            db.SiteElementBackups.InsertOnSubmit(backup);
            db.SubmitChanges();
        }

        /// <summary>
        /// Delete the element from the Elem_Site_Element table
        /// </summary>
        protected void DeleteElement()
        {
            //Delete the element from the Elem_Site_Element table
            db.SiteElements.DeleteOnSubmit(currSite.SiteElements.FirstOrDefault(p => p.element_id == element_id));

            //If this is the Date Of Last Levels element, make sure to remove the record from the Ops Levels table
            if (element_id == 9)
            {
                db.OpsLevels.DeleteOnSubmit(currSite.OpsLevel);
            }

            db.SubmitChanges();
        }

        /// <summary>
        /// Update the revised date in the Elem_Report_Sum table
        /// </summary>
        protected void UpdateElementReportSummary()
        {
            var reportTypes = db.ElementReportRefs.Where(p => p.element_id == element_id).ToList();
            foreach (var report in reportTypes)
            {
                var ers = currSite.ElemReportSums.FirstOrDefault(p => p.report_type_cd == report.report_type_cd);
                ers.revised_dt = DateTime.Now;
                db.SubmitChanges();
            }
        }
        #endregion

        #region Panel Methods
        protected void UpdateElementPanel()
        {
            if (!string.IsNullOrEmpty(rrblAction.SelectedValue))
            {
                //If deleting or editing, then just grab the list of currently assigned elements for the site
                if (rrblAction.SelectedValue == "Delete" || rrblAction.SelectedValue == "Edit")
                {
                    rddlElements.DataSource = currSite.SiteElements.Select(p => new { element_id = p.element_id, element_nm = p.ElementDetail.element_nm }).OrderBy(p => p.element_nm).ToList();
                    
                }
                else //If adding new elements, have to figure out which elements are available to add
                {
                    //Determine the site type
                    int site_type_id = Convert.ToInt32(db.SiteTypes.FirstOrDefault(p => p.site_tp_cd == db.vSITEFILEs.FirstOrDefault(s => s.site_id == currSite.nwisweb_site_id).site_tp_cd).sims_site_tp_id);
                    string site_type = GetSiteType(site_type_id);
                    //Grab all of the elements currently assigned to the site and put them into a list of ElementDetail objects
                    var currElems = currSite.SiteElements.Select(p => new Data.ElementDetail {
                            element_id = Convert.ToInt32(p.element_id),
                            element_nm = p.ElementDetail.element_nm,
                            priority = p.ElementDetail.priority,
                            remark = p.ElementDetail.remark,
                            stream = p.ElementDetail.stream,
                            lake = p.ElementDetail.lake,
                            estuary = p.ElementDetail.estuary,
                            specificsource = p.ElementDetail.specificsource,
                            spring = p.ElementDetail.spring,
                            groundwater = p.ElementDetail.groundwater,
                            meteorological = p.ElementDetail.meteorological,
                            outfall = p.ElementDetail.outfall,
                            diversion = p.ElementDetail.diversion,
                            landapplication = p.ElementDetail.landapplication,
                            aggregategroundwater = p.ElementDetail.aggregategroundwater,
                            aggregatesurfacewater = p.ElementDetail.aggregatesurfacewater,
                            wateruse_placeofuse = p.ElementDetail.wateruse_placeofuse,
                            coastalqw = p.ElementDetail.coastalqw,
                            active = p.ElementDetail.active
                        }).ToList();
                    //Now grab all of the elements possible to be assigned to the site
                    var addElems = db.ElementDetails.Where(p =>
                        ((site_type == "stream") ? p.stream == true : true) &&
                        ((site_type == "lake") ? p.lake == true : true) &&
                        ((site_type == "estuary") ? p.estuary == true : true) &&
                        ((site_type == "specificsource") ? p.specificsource == true : true) &&
                        ((site_type == "spring") ? p.spring == true : true) &&
                        ((site_type == "groundwater") ? p.groundwater == true : true) &&
                        ((site_type == "meteorological") ? p.meteorological == true : true) &&
                        ((site_type == "outfall") ? p.outfall == true : true) &&
                        ((site_type == "diversion") ? p.diversion == true : true) &&
                        ((site_type == "landapplication") ? p.landapplication == true : true) &&
                        ((site_type == "aggregategroundwater") ? p.aggregategroundwater == true : true) &&
                        ((site_type == "aggregatesurfacewater") ? p.aggregatesurfacewater == true : true) &&
                        ((site_type == "wateruse_placeofuse") ? p.wateruse_placeofuse == true : true) &&
                        ((site_type == "coastalQW") ? p.coastalqw == true : true) &&
                        !p.element_nm.Contains("FOOTNOTES") &&
                        !p.element_nm.Contains("EXTREMES FOR CURRENT YEAR") &&
                        p.active == true).OrderBy(p => p.priority).ToList();
                    //For the final list, remove the currently assigned elements from the full list of elements
                    foreach (var elem in currElems)
                        addElems.RemoveAll(p => p.element_id == elem.element_id);
                    rddlElements.DataSource = addElems.OrderBy(p => p.element_nm);
                }

                rddlElements.DataBind();
                rddlElements.Enabled = true;
                //Remove the LOCATION element from the list of elements possible to delete
                if (rrblAction.SelectedValue == "Delete") rddlElements.Items.RemoveAt(rddlElements.FindItemByValue("28").Index);
                //Add a blank item to the top of the drop down, and select it by default
                rddlElements.Items.Insert(0, new DropDownListItem { Value = "", Text = "" });
                rddlElements.SelectedIndex = 0;
            }
            else
            {
                rddlElements.Enabled = false;
            }
        }
        #endregion

        #region Functions
        /// <summary>
        /// Returns the site type based on the SIMS site type ID
        /// </summary>
        private string GetSiteType(int site_type_id)
        {
            string ret = "";

            switch (site_type_id)
            {
                case 1:
                    ret = "stream";
                    break;
                case 2:
                    ret = "lake";
                    break;
                case 3:
                    ret = "estuary";
                    break;
                case 4:
                    ret = "specificsource";
                    break;
                case 5:
                    ret = "spring";
                    break;
                case 6:
                    ret = "groundwater";
                    break;
                case 7:
                    ret = "meteorological";
                    break;
                case 8:
                    ret = "outfall";
                    break;
                case 9:
                    ret = "diversion";
                    break;
                case 10:
                    ret = "landapplication";
                    break;
                case 11:
                    ret = "aggregategroundwater";
                    break;
                case 12:
                    ret = "aggregatesurfacewater";
                    break;
                case 13:
                    ret = "wateruse_placeofuse";
                    break;
                case 14:
                    ret = "coastalQW";
                    break;
            }

            return ret;
        }
        #endregion
    }
}