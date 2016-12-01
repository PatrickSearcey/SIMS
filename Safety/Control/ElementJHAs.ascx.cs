using Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Safety.Control
{
    public partial class ElementJHAs : System.Web.UI.UserControl
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        public Boolean HasEditAccess { get; set; }
        private Data.SiteElement currSiteElem;
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

        #region Public Properties
        public int ElementID { get; set; }
        #endregion

        #region Data Properties
        private List<Models.RefLevelModel> RefLevelTypeList {
            get {
                List<Models.RefLevelModel> rli = new List<Models.RefLevelModel>();

                rli = db.ReferenceLevels.Select(p => new Models.RefLevelModel()
                {
                    reflevel_id = p.reflevel_id,
                    reflevel_tp_desc = p.reflevel_tp + " - " + p.notes
                }).ToList();

                return rli;
            }
        }

        private List<Models.JHAModel> JHAList {
            get {
                List<Models.JHAModel> ehi = new List<Models.JHAModel>();
                
                //Get the full list of JHAs for the current element
                var jhas = db.ElementJHAs.Where(p => p.element_id == ElementID).ToList();

                //Now create an object with the elem_jha_ids currently assigned to this site
                var currJhas = db.SHAJHAs.Where(p => p.sha_site_id == db.SHAs.FirstOrDefault(s => s.site_id == SiteID).sha_site_id).Select(p => p.elem_jha_id).ToList();
                
                foreach (var jha in jhas)
                {
                    //If the elem_jha_id is not in the currently assigned list of elem_jha_ids, then add to the List
                    if (!currJhas.Contains(jha.elem_jha_id))
                    {
                        ehi.Add(new Models.JHAModel()
                        {
                            elem_jha_id = jha.elem_jha_id,
                            jha_description = jha.JHA.jha_description
                        });
                    }
                }
                return ehi;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            currSiteElem = db.SiteElements.FirstOrDefault(p => p.element_id == ElementID && p.site_id == SiteID);
            
		    ltlElemRevisedInfo.Text = "Revised by: " + currSiteElem.revised_by + " &nbsp;&nbsp;Date revised: " + currSiteElem.revised_dt.ToString();
            if (currSiteElem.element_info != null) ltlElemInfo.Text = currSiteElem.element_info.Replace("\n\n", "<br /><br />\n"); else ltlElemInfo.Text = "";
		    ltlElemName.Text = currSiteElem.ElementDetail.element_nm;
            ltlElemName2.Text = currSiteElem.ElementDetail.element_nm;

            hlRevisionHistory.NavigateUrl = String.Format("{0}StationDoc/Archive.aspx?element_id={1}&site+id={2}&begin_dt=1/1/1900&end_dt={3}", Config.SIMS2017URL, ElementID, SiteID, DateTime.Now);

            if (hfToggleElementEditMode.Value == "true")
            {
                lbToggleElementEditMode.Text = "leave element edit mode";
                pnlEditElemInfo.Visible = true;
                pnlStaticElemInfo.Visible = false;
            }
            else
            {
                lbToggleElementEditMode.Text = "enter element edit mode";
                pnlEditElemInfo.Visible = false;
                pnlStaticElemInfo.Visible = true;
            }

            if (hfToggleElementHazardEditMode.Value == "true")
            {
                lbToggleElementHazardEditMode.Text = "leave hazard edit mode";
            }
            else
            {
                lbToggleElementHazardEditMode.Text = "enter hazard edit mode";
            }
        }

        #region Page Events, Methods
        protected void lbToggleElementEditMode_Click(object sender, EventArgs e)
	    {
		    if (string.IsNullOrEmpty(hfToggleElementEditMode.Value)) {
			    hfToggleElementEditMode.Value = "true";
			    lbToggleElementEditMode.Text = "leave element edit mode";

			    reElemInfo.Content = currSiteElem.element_info;
			    pnlEditElemInfo.Visible = true;
			    pnlStaticElemInfo.Visible = false;
		    } else {
			    hfToggleElementEditMode.Value = "";
			    lbToggleElementEditMode.Text = "enter element edit mode";

			    pnlEditElemInfo.Visible = false;
			    pnlStaticElemInfo.Visible = true;
		    }
	    }

        protected void lbToggleElementHazardEditMode_Click(object sender, EventArgs e)
	    {
		    if (string.IsNullOrEmpty(hfToggleElementHazardEditMode.Value)) {
                hfToggleElementHazardEditMode.Value = "true";
			    lbToggleElementHazardEditMode.Text = "leave hazard edit mode";
		    } else {
                hfToggleElementHazardEditMode.Value = "";
			    lbToggleElementHazardEditMode.Text = "enter hazard edit mode";
			    lblError2.Visible = false;
			    lblSuccess2.Visible = false;
		    }
		    rgElementHazards.Rebind();
	    }

	    private void DisplayMessage(bool isError, string text, string type)
	    {
		    Label label = default(Label);
		    if (type == "element") {
			    label = (isError) ? this.lblError1 : this.lblSuccess1;
		    } else {
			    label = (isError) ? this.lblError2 : this.lblSuccess2;
		    }
		    label.Text = text;
	    }
        #endregion 

	    #region Element Editing Routines
	    protected void btnSubmitElemInfo_Command(object sender, CommandEventArgs e)
	    {
		    if (e.CommandArgument.ToString() == "editelement" & !string.IsNullOrEmpty(hfToggleElementEditMode.Value)) {
                try {
                    //First confirm that there is element information to back up
                    if (!string.IsNullOrEmpty(currSiteElem.element_info))
                    {
                        //If element_info is not blank, then back up before updating
                        Data.SiteElementBackup backup = new Data.SiteElementBackup() {
                            site_id = currSiteElem.site_id,
                            element_id = currSiteElem.element_id,
                            element_info = currSiteElem.element_info,
                            entered_by = currSiteElem.entered_by,
                            entered_dt = currSiteElem.entered_dt,
                            revised_by = currSiteElem.revised_by,
                            revised_dt = currSiteElem.revised_dt,
                            backup_by = user.ID,
                            backup_dt = DateTime.Now
                        };
                        db.SiteElementBackups.InsertOnSubmit(backup);
                    }

                    //Update the element info
                    currSiteElem.element_info = reElemInfo.Content;
                    currSiteElem.revised_by = user.ID;
                    currSiteElem.revised_dt = DateTime.Now;

                    //Run the stored procedure that updates the max revised date in Elem_Report_Sum
                    db.SP_Report_Update_Site_LastEdited(SiteID);

                    db.SubmitChanges();

                    DisplayMessage(false, "The element information was updated!", "element");
				    ltlElemRevisedInfo.Text = "Revised by: " + user.ID + " &nbsp;&nbsp;Date revised: " + System.DateTime.Now.ToString();
				    ltlElemInfo.Text = reElemInfo.Content;
                }
                catch (Exception ex)
                {
                    DisplayMessage(true, ex.Message, "element");
                }
		    }

		    hfToggleElementEditMode.Value = "";
		    lbToggleElementEditMode.Text = "enter element edit mode";

		    pnlEditElemInfo.Visible = false;
		    pnlStaticElemInfo.Visible = true;
	    }
	    #endregion

	    #region RadGrid Routines
	    public void rgElementHazards_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
	    {
		    if (!e.IsFromDetailTable) {
                var hazard = db.SHAJHAs.Where(p => p.SHA.site_id == SiteID && p.ElementJHA.element_id == ElementID).Select(p => new Models.JHAModel
                {
                    sha_site_id = Convert.ToInt32(p.sha_site_id),
                    site_jha_id = p.site_jha_id,
                    elem_jha_id = p.ElementJHA.elem_jha_id,
                    jha_id = Convert.ToInt32(p.ElementJHA.jha_id),
                    jha_description = p.ElementJHA.JHA.jha_description
                }).ToList();

                if (hfToggleElementHazardEditMode.Value == "true" && JHAList.Count > 0) {
				    rgElementHazards.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.Bottom;
			    } else {
				    rgElementHazards.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
				    rgElementHazards.MasterTableView.ClearEditItems();
			    }

			    rgElementHazards.DataSource = hazard;
		    }
	    }

	    public void rgElementHazards_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
	    {
		    GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;
		    string site_jha_id = dataItem.GetDataKeyValue("site_jha_id").ToString();
            var hazard = db.SHAJHAs.FirstOrDefault(p => p.site_jha_id == Convert.ToInt32(site_jha_id));

		    switch (e.DetailTableView.Name) {
			    case "Hazards":
                    var specificCond = hazard.SHASpecificConditions.OrderByDescending(p => p.priority).ToList();
                    e.DetailTableView.DataSource = specificCond;
				    break;
			    case "JobLimits":
                    var jobLimits = hazard.SHAReferenceLevels.Select(p => new Models.RefLevelModel
                    {
                        site_reflevel_id = p.site_reflevel_id,
                        site_jha_id = Convert.ToInt32(p.site_jha_id),
                        reflevel_id = Convert.ToInt32(p.reflevel_id),
                        reflevel_tp = p.ReferenceLevel.reflevel_tp,
                        reflevel_va = Convert.ToDouble(p.reflevel_va),
                        reflevel_units = p.reflevel_units,
                        remarks = p.remarks,
                        reflevel_desc = p.ReferenceLevel.reflevel_tp + " - " + p.ReferenceLevel.notes
                    }).ToList();
				    e.DetailTableView.DataSource = jobLimits;
				    break;
		    }

            if (hfToggleElementHazardEditMode.Value == "true"){
			    e.DetailTableView.CommandItemDisplay = GridCommandItemDisplay.Bottom;
		    } else {
			    e.DetailTableView.CommandItemDisplay = GridCommandItemDisplay.None;
			    e.DetailTableView.ClearEditItems();
		    }
	    }

	    /// <summary>
	    /// This routine hides the edit features of the grid while the enter edit mode link has not been selected
	    /// </summary>
	    protected void rgElementHazards_PreRender(object sender, EventArgs e)
	    {
            if (string.IsNullOrEmpty(hfToggleElementHazardEditMode.Value)) {
			    foreach (GridColumn col in rgElementHazards.MasterTableView.RenderColumns) {
				    if (col.UniqueName == "DeleteColumn") {
					    col.Visible = false;
				    }
			    }

			    foreach (GridDataItem item in rgElementHazards.Items) {
				    if ((item.OwnerTableView.Name == "JobLimits" && item is GridEditableItem)) {
					    GridEditableItem editableItem1 = item as GridEditableItem;
					    try {
						    ImageButton imgbtn1 = (ImageButton)editableItem1["EditCommandColumn2"].Controls[0];
						    imgbtn1.Visible = false;
						    ImageButton delbtn1 = (ImageButton)editableItem1["JobLimitDeleteColumn"].Controls[0];
						    delbtn1.Visible = false;
					    } catch (Exception ex) {
					    }
				    }

				    if ((item.OwnerTableView.Name == "Hazards" && item is GridEditableItem)) {
					    GridEditableItem editableItem2 = item as GridEditableItem;
					    try {
						    ImageButton imgbtn2 = (ImageButton)editableItem2["EditCommandColumn1"].Controls[0];
						    imgbtn2.Visible = false;
						    ImageButton delbtn2 = (ImageButton)editableItem2["HazardDeleteColumn"].Controls[0];
						    delbtn2.Visible = false;
					    } catch (Exception ex) {
					    }
				    }
			    }
		    } else {
			    foreach (GridColumn col in rgElementHazards.MasterTableView.RenderColumns) {
				    if (col.UniqueName == "DeleteColumn") {
					    col.Visible = true;
				    }
			    }
		    }
	    }

	    /// <summary>
	    /// This routine sets up the edit form templates for the RadGrid - the drop-down lists and headings for the forms are populated, and insert/update buttons displayed appropriately
	    /// </summary>
	    protected void rgElementHazards_ItemDataBound(object sender, GridItemEventArgs e)
	    {
		    if ("Hazards".Equals(e.Item.OwnerTableView.Name)) {
			    if (e.Item is GridDataItem) {
				    GridDataItem dataItem = (GridDataItem)e.Item;
				    foreach (GridColumn col in rgElementHazards.Columns) {
					    if (dataItem["priority"].Text == "True") {
						    dataItem["remarks"].Attributes.CssStyle.Add("font-weight", "bold");
					    }
				    }
			    }

			    if (e.Item.IsInEditMode) {
				    GridEditableItem item = (GridEditableItem)e.Item;

				    Literal ltlHazardsEditFormTitle = (Literal)item.FindControl("ltlHazardsEditFormTitle");
				    TextBox tbHazard = (TextBox)item.FindControl("tbHazard");
				    CheckBox cbPriority = (CheckBox)item.FindControl("cbPriority");
				    Button btnUpdate2 = (Button)item.FindControl("btnUpdate2");
				    Button btnInsert2 = (Button)item.FindControl("btnInsert2");

				    if (!(e.Item is IGridInsertItem)) {
					    ltlHazardsEditFormTitle.Text = "Edit Hazard";
                        tbHazard.Text = ((Data.SHASpecificCondition)e.Item.DataItem).remarks;
                        bool priority = Convert.ToBoolean(((Data.SHASpecificCondition)e.Item.DataItem).priority);
					    if (!priority) {
						    cbPriority.Checked = false;
					    } else {
						    cbPriority.Checked = true;
					    }

					    btnUpdate2.Visible = true;
					    btnInsert2.Visible = false;
				    } else if ((e.Item is IGridInsertItem)) {
					    ltlHazardsEditFormTitle.Text = "Add New Hazard";

					    btnUpdate2.Visible = false;
					    btnInsert2.Visible = true;
				    }
			    }
		    } else if ("JobLimits".Equals(e.Item.OwnerTableView.Name)) {
			    if (e.Item.IsInEditMode) {
				    GridEditableItem item = (GridEditableItem)e.Item;

				    Literal ltlJobLimitsEditFormTitle = (Literal)item.FindControl("ltlJobLimitsEditFormTitle");
                    RadDropDownList rcbJobLimitType = (RadDropDownList)item.FindControl("rddlJobLimitType");
				    RadNumericTextBox rntbJobLimitValue = (RadNumericTextBox)item.FindControl("rntbJobLimitValue");
				    TextBox tbUnits = (TextBox)item.FindControl("tbUnits");
				    TextBox tbRemarks = (TextBox)item.FindControl("tbRemarks");
				    Button btnUpdate3 = (Button)item.FindControl("btnUpdate3");
				    Button btnInsert3 = (Button)item.FindControl("btnInsert3");

				    rcbJobLimitType.DataSource = RefLevelTypeList;

				    if (!(e.Item is IGridInsertItem)) {
					    ltlJobLimitsEditFormTitle.Text = "Edit Job Limit";
                        rcbJobLimitType.SelectedValue = ((Models.RefLevelModel)e.Item.DataItem).reflevel_id.ToString();
                        rntbJobLimitValue.Text = ((Models.RefLevelModel)e.Item.DataItem).reflevel_va.ToString();
                        tbUnits.Text = ((Models.RefLevelModel)e.Item.DataItem).reflevel_units;
                        tbRemarks.Text = ((Models.RefLevelModel)e.Item.DataItem).remarks;

					    btnUpdate3.Visible = true;
					    btnInsert3.Visible = false;
				    } else if ((e.Item is IGridInsertItem)) {
					    ltlJobLimitsEditFormTitle.Text = "Add New Job Limit";

					    btnUpdate3.Visible = false;
					    btnInsert3.Visible = true;
				    }

				    rcbJobLimitType.DataBind();
			    }
		    } else {
			    if (e.Item.IsInEditMode) {
				    GridEditableItem item = (GridEditableItem)e.Item;

                    RadDropDownList ddlMeasType = (RadDropDownList)item.FindControl("rddlMeasType");

				    ddlMeasType.DataSource = JHAList;
				    ddlMeasType.DataBind();
			    }

			    if (e.Item is GridDataItem) {
				    GridDataItem dataItem = (GridDataItem)e.Item;
				    HyperLink hlFullReport = (HyperLink)dataItem.FindControl("hlFullReport");
				    int site_jha_id = Convert.ToInt32(dataItem.OwnerTableView.DataKeyValues[dataItem.ItemIndex]["site_jha_id"]);

				    var shj = db.SHAJHAs.FirstOrDefault(p => p.site_jha_id == site_jha_id);

				    switch (shj.elem_jha_id) {
					    case 1:
						    //Servicing Surface Water Field Sites
						    hlFullReport.NavigateUrl = "../Docs/JHAs/Servicing_Site.pdf";
						    break;
					    case 2:
						    //Wading Measurements
						    hlFullReport.NavigateUrl = "../Docs/JHAs/Wading_Measurements.pdf";
						    break;
					    case 3:
						    //Bridge Measurements
						    hlFullReport.NavigateUrl = "../Docs/JHAs/Bridge_Measurements.pdf";
						    break;
					    case 4:
						    //Cableway Measurements
						    hlFullReport.NavigateUrl = "../Docs/JHAs/Cableway_Measurements.pdf";
						    break;
					    case 5:
						    //Boat Measurements for Stream Sites
						    hlFullReport.NavigateUrl = "../Docs/JHAs/Boat_Measurements_Streams.pdf";
						    break;
					    case 6:
						    //Service and Measure Groundwater Field Site
						    hlFullReport.NavigateUrl = "../Docs/JHAs/Groundwater.pdf";
						    break;
					    case 7:
					    case 8:
						    //Stilling Well with Confined Space Hazard
						    hlFullReport.NavigateUrl = "../Docs/JHAs/StillingWell_ConfinedSpace.pdf";
						    break;
					    case 9:
						    //Water Quality Sampling
						    hlFullReport.NavigateUrl = "../Docs/JHAs/Water_Quality_Sampling.pdf";
						    break;
					    case 10:
						    //Boat Measurements for Lake Sites
						    hlFullReport.NavigateUrl = "../Docs/JHAs/Boat_Measurements_Lakes.pdf";
						    break;
				    }
			    }
		    }
	    }

	    /// <summary>
	    /// This routine fires when data is being inserted into the grid
	    /// </summary>
	    protected void rgElementHazards_InsertCommand(object sender, GridCommandEventArgs e)
	    {
		    GridEditableItem item = e.Item as GridEditableItem;

		    if (e.Item.OwnerTableView.Name == "Hazards") {
			    Data.SHASpecificCondition newSSC = new Data.SHASpecificCondition();

			    try {
				    GridDataItem parentItem = (GridDataItem)e.Item.OwnerTableView.ParentItem;
				    newSSC.site_jha_id = Convert.ToInt32(parentItem.OwnerTableView.DataKeyValues[parentItem.ItemIndex]["site_jha_id"]);
				    newSSC.remarks = (item.FindControl("tbHazard") as TextBox).Text;
				    newSSC.priority = (item.FindControl("cbPriority") as CheckBox).Checked;
			    } catch (Exception ex) {
			    }

			    try {
				    db.SHASpecificConditions.InsertOnSubmit(newSSC);
                    db.SubmitChanges();

				    DisplayMessage(false, "The site specific condition was added!", "hazards");
			    } catch (Exception ex) {
				    DisplayMessage(true, "Unable to insert site specific condition. Reason: " + ex.Message, "hazards");
				    e.Canceled = true;
			    }
		    } else if (e.Item.OwnerTableView.Name == "JobLimits") {
                Data.SHAReferenceLevel newJL = new Data.SHAReferenceLevel();

			    try {
				    GridDataItem parentItem = (GridDataItem)e.Item.OwnerTableView.ParentItem;
				    newJL.site_jha_id = Convert.ToInt32(parentItem.OwnerTableView.DataKeyValues[parentItem.ItemIndex]["site_jha_id"]);
				    newJL.reflevel_id = Convert.ToInt32((item.FindControl("rddlJobLimitType") as RadDropDownList).SelectedValue);
				    newJL.reflevel_va = Convert.ToDouble((item.FindControl("rntbJobLimitValue") as RadNumericTextBox).Text);
				    newJL.reflevel_units = (item.FindControl("tbUnits") as TextBox).Text;
				    newJL.remarks = (item.FindControl("tbRemarks") as TextBox).Text;
			    } catch (Exception ex) {
			    }

			    try {
				    db.SHAReferenceLevels.InsertOnSubmit(newJL);
                    db.SubmitChanges();

				    DisplayMessage(false, "The job operational limit was added!", "hazards");
			    } catch (Exception ex) {
				    DisplayMessage(true, "Unable to insert job operational limit. Reason: " + ex.Message, "hazards");
				    e.Canceled = true;
			    }
		    } else {
                Data.SHAJHA newJHA = new Data.SHAJHA();

			    try {
				    newJHA.elem_jha_id = Convert.ToInt32((item.FindControl("rddlMeasType") as RadDropDownList).SelectedValue);
				    newJHA.sha_site_id = db.SHAs.FirstOrDefault(p => p.site_id == SiteID).sha_site_id;
			    } catch (Exception ex) {
			    }

			    try {
				    db.SHAJHAs.InsertOnSubmit(newJHA);
                    db.SubmitChanges();

				    DisplayMessage(false, "The measurement type was added!", "hazards");
			    } catch (Exception ex) {
				    DisplayMessage(true, "Unable to insert measurement type. Reason: " + ex.Message, "hazards");
				    e.Canceled = true;
			    }
		    }

		    //Update the updated_by and updated_dt fields of SHA_Site_Master - the updated_by and updated_dt fields are legacy; not being used outside of this situation
            var sha = db.SHAs.FirstOrDefault(p => p.site_id == SiteID);
		    sha.updated_by = user.ID;
            sha.updated_dt = DateTime.Now;
            db.SubmitChanges();
	    }

	    /// <summary>
	    /// Fires when updating information in the grid
	    /// </summary>
	    protected void rgElementHazards_UpdateCommand(object sender, GridCommandEventArgs e)
	    {
		    GridEditableItem item = e.Item as GridEditableItem;

		    if (e.Item.OwnerTableView.Name == "Hazards") {
			    int site_specificcond_id = Convert.ToInt32(item.GetDataKeyValue("site_specificcond_id"));
			    var updateSSC = db.SHASpecificConditions.FirstOrDefault(p => p.site_specificcond_id == site_specificcond_id);

			    updateSSC.remarks = (item.FindControl("tbHazard") as TextBox).Text;
			    updateSSC.priority = (item.FindControl("cbPriority") as CheckBox).Checked;

			    try {
                    db.SubmitChanges();

				    DisplayMessage(false, "The site specific condition was updated!", "hazards");
			    } catch (Exception ex) {
				    DisplayMessage(true, "Unable to update site specific condition. Reason: " + ex.Message, "hazards");
				    e.Canceled = true;
			    }
		    } else if (e.Item.OwnerTableView.Name == "JobLimits") {
			    int site_reflevel_id = Convert.ToInt32(item.GetDataKeyValue("site_reflevel_id"));
			    var updateRL = db.SHAReferenceLevels.FirstOrDefault(p => p.site_reflevel_id == site_reflevel_id);

			    updateRL.reflevel_id = Convert.ToInt32((item.FindControl("rddlJobLimitType") as RadDropDownList).SelectedValue);
                updateRL.reflevel_va = Convert.ToDouble((item.FindControl("rntbJobLimitValue") as RadNumericTextBox).Text);
                updateRL.reflevel_units = (item.FindControl("tbUnits") as TextBox).Text;
                updateRL.remarks = (item.FindControl("tbRemarks") as TextBox).Text;

			    try {
                    db.SubmitChanges();

				    DisplayMessage(false, "The job operational limit was updated!", "hazards");
			    } catch (Exception ex) {
				    DisplayMessage(true, "Unable to update job operational limit. Reason: " + ex.Message, "hazards");
				    e.Canceled = true;
			    }
		    }

		    //Update the updated_by and updated_dt fields of SHA_Site_Master - the updated_by and updated_dt fields are legacy; not being used outside of this situation
            var sha = db.SHAs.FirstOrDefault(p => p.site_id == SiteID);
            sha.updated_by = user.ID;
            sha.updated_dt = DateTime.Now;
            db.SubmitChanges();
	    }

	    /// <summary>
	    /// Fires when deleting information from the grid
	    /// </summary>
	    protected void rgElementHazards_DeleteCommand(object sender, GridCommandEventArgs e)
	    {
		    GridDataItem item = e.Item as GridDataItem;

		    if (e.Item.OwnerTableView.Name == "Hazards") {
			    try {
				    int site_specificcond_id = Convert.ToInt32(item.GetDataKeyValue("site_specificcond_id"));
				    var deleteSSC = db.SHASpecificConditions.FirstOrDefault(p => p.site_specificcond_id == site_specificcond_id);

				    db.SHASpecificConditions.DeleteOnSubmit(deleteSSC);
				    db.SubmitChanges();
                    
                    DisplayMessage(false, "The site specific condition was deleted!", "hazards");
			    } catch (Exception ex) {
				    DisplayMessage(true, "Unable to delete site specific condition. Reason: " + ex.Message, "hazards");

				    e.Canceled = true;
			    }
		    } else if (e.Item.OwnerTableView.Name == "JobLimits") {
			    try {
				    int site_reflevel_id = Convert.ToInt32(item.GetDataKeyValue("site_reflevel_id"));
				    var deleteRL = db.SHAReferenceLevels.FirstOrDefault(p => p.site_reflevel_id == site_reflevel_id);

				    db.SHAReferenceLevels.DeleteOnSubmit(deleteRL);
                    db.SubmitChanges();
				    
                    DisplayMessage(false, "The job operational limit was deleted!", "hazards");
			    } catch (Exception ex) {
				    DisplayMessage(true, "Unable to delete job operational limit. Reason: " + ex.Message, "hazards");

				    e.Canceled = true;
			    }
		    } else {
			    try {
				    int site_jha_id = Convert.ToInt32(item.GetDataKeyValue("site_jha_id"));
				    var deleteSHAJHA = db.SHAJHAs.FirstOrDefault(p => p.site_jha_id == site_jha_id);

                    db.SHAJHAs.DeleteOnSubmit(deleteSHAJHA);
                    db.SubmitChanges();

				    DisplayMessage(false, "The SHA was deleted!", "hazards");
			    } catch (Exception ex) {
				    DisplayMessage(true, "Unable to delete SHA. Reason: " + ex.Message, "hazards");

				    e.Canceled = true;
			    }
		    }

		    //Update the updated_by and updated_dt fields of SHA_Site_Master - the updated_by and updated_dt fields are legacy; not being used outside of this situation
            var sha = db.SHAs.FirstOrDefault(p => p.site_id == SiteID);
            sha.updated_by = user.ID;
            sha.updated_dt = DateTime.Now;
            db.SubmitChanges();
	    }
	    
        public ElementJHAs()
	    {
		    Load += Page_Load;
	    }

	    #endregion
    }
}

