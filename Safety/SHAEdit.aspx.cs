using Core;
using Safety.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Safety
{
    public partial class SHAEdit : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private Data.SHA currSHA;
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
        private int SHAID
        {
            get
            {
                if (Session["SHAID"] == null) return 0; else return (int)Session["SHAID"];
            }
            set
            {
                Session["SHAID"] = value;
            }
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            //If no site_id was passed, then redirect back to the homepage
            string site_id = "3000002";// Request.QueryString["site_id"];
            if (!string.IsNullOrEmpty(site_id)) SiteID = Convert.ToInt32(site_id); else Response.Redirect(Config.SIMS2017URL + "SIMSWSCHome.aspx");

            //Using the passed site_id, setup the site data element, and reset the office and wsc to match that of the current site
            currSHA = db.SHAs.Where(p => p.site_id == SiteID).FirstOrDefault();
            OfficeID = (int)currSHA.Site.office_id;
            WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            //--BASIC PAGE SETUP--------------------------------------------------------------------
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            ph1.Title = "Manage Site Hazard Analysis";
            ph1.SubTitle = currSHA.Site.site_no + " " + currSHA.Site.station_full_nm;

            if (!Page.IsPostBack)
            {
                //--PAGE ACCESS SECTION-------------------------------------------------------------
                if (user.IsSuperUser || user.WSCID.Contains(WSCID))
                    pnlHasAccess.Visible = true;
                else
                    Response.Redirect("SHAView.aspx?site_id=" + SiteID.ToString());
                    pnlHasAccess.Visible = false;
                //--END PAGE ACCESS SECTION---------------------------------------------------------

                //Start with a clean session
                Session.Clear();

                hlPrintSHA.NavigateUrl = "SHAView.aspx?site_id=" + SiteID.ToString();

                PopulatePagePanels();
            }
            //--------------------------------------------------------------------------------------

            //Moved out of PopulatePagePanels routine for AJAX to work
            SetupMeasurementSpecificPanel();
        }

        #region Misc Methods and Events
        protected void PopulatePagePanels()
        {
            SetupWarningsPanel();

            SetupServicingSitePanel();

            SetupSiteHazardAnalysisElementPanel();

            SetupEmergencyInfoPanel();

            SetupAdminPanel();
        }

        private void DisplayMessage(bool isError, string text, string type)
        {
            Label label = default(Label);
            switch (type)
            {
                case "element":
                    label = (isError) ? this.lblError1 : this.lblSuccess1;
                    break;
                case "sitehazard":
                    label = (isError) ? this.lblError2 : this.lblSuccess2;
                    break;
                default:
                    label = null;
                    break;
            }
            label.Text = text;
        }

        protected void AddInfo_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "ServicingSite")
            {
                if (e.CommandArgument == "AddHazard")
                {
                    AddServicingSiteHazard();
                }
                else if (e.CommandArgument == "AddEquip")
                {
                    AddRecEquipment();
                }
                SetupServicingSitePanel();
            }
        }

        protected void ram_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            if (e.Argument == "Rebind")
            {
                rgHospitals.MasterTableView.SortExpressions.Clear();
                rgHospitals.MasterTableView.GroupByExpressions.Clear();
                rgHospitals.Rebind();
                rgContacts.MasterTableView.SortExpressions.Clear();
                rgContacts.MasterTableView.GroupByExpressions.Clear();
                rgContacts.Rebind();
                ltlNotice1.Text = "";
                ltlNotice2.Text = "";
            }
        }

        protected void UpdateSHA()
        {
            //Update the updated_by and updated_dt fields of SHA_Site_Master - the updated_by and updated_dt fields are legacy; not being used outside of this situation
            currSHA.updated_by = user.ID;
            currSHA.updated_dt = DateTime.Now;
            db.SubmitChanges();
        }
        #endregion

        #region Warnings Panel
        protected void SetupWarningsPanel()
        {
            try
            {
                bool has_warnings = CreateWarningBullets();

                if (has_warnings)
                {
                    pnlWarnings.Visible = true;
                }
                else
                {
                    pnlWarnings.Visible = false;
                }
            }
            catch (Exception ex)
            {
                pnlWarnings.Visible = false;
            }
        }

        protected bool CreateWarningBullets()
        {
            bool return_va = false;
            List<string> bullets = new List<string>();
            var hospitals = currSHA.SHAHospitals;
            var contacts = currSHA.SHAContacts;

            if (hospitals == null)
            {
                bullets.Add("No hospitals have been assigned to this site.");
                return_va = true;
            }

            if (contacts == null)
            {
                bullets.Add("No contacts have been assigned to this site.");
                return_va = true;
            }

            blWarnings.DataSource = bullets;
            blWarnings.DataBind();

            return return_va;
        }

        protected void ibWarnings_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument == "open")
            {
                pnlWarningList.Visible = true;
                ibWarningsClose.Visible = true;
                ibWarningsOpen.Visible = false;
            }
            else
            {
                pnlWarningList.Visible = false;
                ibWarningsClose.Visible = false;
                ibWarningsOpen.Visible = true;
            }
        }
        #endregion

        #region Site Specific Panel
        protected void SetupServicingSitePanel()
        {
            var sssc = db.SP_Safety_Info(0, currSHA.Site.site_id, 0, "sssc").ToList();
            var equip = db.SP_Safety_Info(0, currSHA.Site.site_id, 0, "equip").ToList();

            lvServicingSiteSpecificCond.DataSource = sssc;
            lvServicingSiteSpecificCond.DataBind();
            lvServicingSiteRecEquip.DataSource = equip;
            lvServicingSiteRecEquip.DataBind();

            if (sssc.Count > 0)
            {
                if (hfToggleHazardEditMode.Value == "true")
                {
                    lbToggleHazardEditMode.Text = "leave delete/change priority mode";
                }
                else
                {
                    lbToggleHazardEditMode.Text = "enter delete/change priority mode";
                }
                lbToggleHazardEditMode.Visible = true;
            }
            else
            {
                lbToggleHazardEditMode.Visible = false;
            }

            if (equip.Count > 0)
            {
                if (hfToggleEquipEditMode.Value == "true")
                {
                    lbToggleEquipEditMode.Text = "leave delete mode";
                }
                else
                {
                    lbToggleEquipEditMode.Text = "enter delete mode";
                }
                lbToggleEquipEditMode.Visible = true;
            }
            else
            {
                lbToggleEquipEditMode.Visible = false;
            }

            if (hfAddSiteSpecificInfo.Value == "true")
            {
                lbAddSiteSpecificInfo.Visible = false;
                imgArrow1.Visible = false;
                pnlAddSiteSpecificInfo.Visible = true;
            }
            else
            {
                lbAddSiteSpecificInfo.Visible = true;
                imgArrow1.Visible = true;
                pnlAddSiteSpecificInfo.Visible = false;
            }
        }

        protected void lvServicingSiteSpecificCond_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataitem = (ListViewDataItem)e.Item;
                Label lbl = (Label)e.Item.FindControl("lblServicingSiteSpecificCond");
                TextBox tb = (TextBox)e.Item.FindControl("tbServicingSiteSpecificCond");
                ImageButton ibEdit = (ImageButton)e.Item.FindControl("ibEditHazard");
                ImageButton ibDelete = (ImageButton)e.Item.FindControl("ibDeleteHazard");
                ImageButton ibPriority = (ImageButton)e.Item.FindControl("ibChangePriority");

                try
                {
                    int priority = Convert.ToInt32(DataBinder.Eval(dataitem.DataItem, "priority"));
                    if (priority < 0)
                    {
                        lbl.Font.Bold = true;
                    }
                }
                catch (Exception ex) { }

                //EDIT CURRENTLY NOT BEING USED - ALL SET TO FALSE
                try
                {
                    if (hfToggleHazardEditMode.Value == "true")
                    {
                        ibEdit.Visible = false;
                        ibDelete.Visible = true;
                        ibPriority.Visible = true;
                    }
                    else
                    {
                        ibEdit.Visible = false;
                        ibDelete.Visible = false;
                        ibPriority.Visible = false;
                    }
                }
                catch (Exception ex)
                {
                    ibEdit.Visible = false;
                    ibDelete.Visible = false;
                    ibPriority.Visible = false;
                }

                //FOR CONTROLLING EDITING; CURRENTLY NOT BEING USED
                try
                {
                    if (string.IsNullOrEmpty(Session["EditHazardID"].ToString()))
                    {
                        tb.Visible = false;
                        lbl.Visible = true;
                    }
                    else
                    {
                        if (Session["EditHazardID"].ToString() == DataBinder.Eval(dataitem.DataItem, "site_servicing_id").ToString())
                        {
                            tb.Visible = true;
                            lbl.Visible = false;
                        }
                        else
                        {
                            tb.Visible = false;
                            lbl.Visible = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    tb.Visible = false;
                    lbl.Visible = true;
                }
            }
        }

        protected void lvServicingSiteRecEquip_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataitem = (ListViewDataItem)e.Item;
                ImageButton ibDelete = (ImageButton)e.Item.FindControl("ibDeleteEquip");

                try
                {
                    if (hfToggleEquipEditMode.Value == "true")
                    {
                        ibDelete.Visible = true;
                    }
                    else
                    {
                        ibDelete.Visible = false;
                    }
                }
                catch (Exception ex)
                {
                    ibDelete.Visible = false;
                }
            }
        }

        protected void lbToggleHazardEditMode_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfToggleHazardEditMode.Value))
            {
                hfToggleHazardEditMode.Value = "true";
            }
            else
            {
                hfToggleHazardEditMode.Value = "";
            }
            SetupServicingSitePanel();
        }

        protected void lbToggleEquipEditMode_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfToggleEquipEditMode.Value))
            {
                hfToggleEquipEditMode.Value = "true";
            }
            else
            {
                hfToggleEquipEditMode.Value = "";
            }
            SetupServicingSitePanel();
        }

        protected void lbAddSiteSpecificInfo_Command(object sender, CommandEventArgs e)
        {
            if (string.IsNullOrEmpty(hfAddSiteSpecificInfo.Value))
            {
                hfAddSiteSpecificInfo.Value = "true";
            }
            else
            {
                hfAddSiteSpecificInfo.Value = "";
            }
            SetupServicingSitePanel();
        }

        protected void ibHazard_Command(object sender, CommandEventArgs e)
        {
            string site_servicing_id = e.CommandArgument.ToString();

            if (e.CommandName == "DeleteHazard")
            {
                db.SHAServicings.DeleteOnSubmit(db.SHAServicings.FirstOrDefault(p => p.site_servicing_id == Convert.ToInt32(site_servicing_id)));
            }
            else if (e.CommandName == "EditHazard")
            {
                //NOT BEING USED AT THIS TIME
            }
            else
            {
                string strPriority = site_servicing_id.Substring(site_servicing_id.Length - 1, 1);
                bool priority = false;
                if (strPriority == "_")
                {
                    priority = true;
                }
                else
                {
                    strPriority = site_servicing_id.Substring(site_servicing_id.Length - 4, 4);
                    if (strPriority == "alse")
                    {
                        priority = true;
                    }
                    else
                    {
                        priority = false;
                    }
                }

                int ssid = Convert.ToInt32(site_servicing_id.Substring(0, site_servicing_id.IndexOf('_')));
                var ss = db.SHAServicings.FirstOrDefault(p => p.site_servicing_id == ssid);
                
                ss.priority = priority;
            }

            db.SubmitChanges();

            //Update the updated_by and updated_dt fields of SHA_Site_Master
            UpdateSHA();

            //Refresh the servicing site panel
            SetupServicingSitePanel();
        }

        protected void ibEquip_Command(object sender, CommandEventArgs e)
        {
            int site_equip_id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "DeleteEquip")
            {

                var equip = db.SHAEquips.FirstOrDefault(p => p.site_equip_id == site_equip_id);
                db.SHAEquips.DeleteOnSubmit(equip);
            }

            db.SubmitChanges();

            //Update the updated_by and updated_dt fields of SHA_Site_Master
            UpdateSHA();

            //Refresh the servicing site panel
            SetupServicingSitePanel();
        }

        public void AddServicingSiteHazard()
        {
            var servicingList = currSHA.SHAServicings.Select(p => p.servicing_va).ToList();

            foreach (RadListBoxItem item in rlbHazards.CheckedItems)
            {
                if (!servicingList.Contains(item.Text))
                {
                    Data.SHAServicing ser = new Data.SHAServicing()
                    {
                        sha_site_id = currSHA.sha_site_id,
                        servicing_va = item.Text
                    };
                    db.SHAServicings.InsertOnSubmit(ser);
                    db.SubmitChanges();
                }
            }

            if (!string.IsNullOrEmpty(tbOtherHazard.Text))
            {
                if (!servicingList.Contains(tbOtherHazard.Text))
                {
                    Data.SHAServicing ser = new Data.SHAServicing()
                    {
                        sha_site_id = currSHA.sha_site_id,
                        servicing_va = tbOtherHazard.Text
                    };
                    db.SHAServicings.InsertOnSubmit(ser);
                    db.SubmitChanges();
                }
            }

            //Clear the selections
            tbOtherHazard.Text = "";
            rlbHazards.ClearChecked();

            //Update the updated_by and updated_dt fields of SHA_Site_Master
            UpdateSHA();
        }

        public void AddRecEquipment()
        {
            var equipList = currSHA.SHAEquips.Select(p => p.recom_equip).ToList();

            foreach (RadListBoxItem item in rlbHazards.CheckedItems)
            {
                if (!equipList.Contains(item.Text))
                {
                    Data.SHAEquip equip = new Data.SHAEquip()
                    {
                        sha_site_id = currSHA.sha_site_id,
                        recom_equip = item.Text
                    };
                    db.SHAEquips.InsertOnSubmit(equip);
                    db.SubmitChanges();
                }
            }

            if (!string.IsNullOrEmpty(tbOtherEquip.Text))
            {
                if (!equipList.Contains(tbOtherEquip.Text))
                {
                    Data.SHAEquip equip = new Data.SHAEquip()
                    {
                        sha_site_id = currSHA.sha_site_id,
                        recom_equip = tbOtherEquip.Text
                    };
                    db.SHAEquips.InsertOnSubmit(equip);
                    db.SubmitChanges();
                }
            }

            //Clear the selections
            tbOtherEquip.Text = "";
            rlbEquip.ClearChecked();

            //Update the updated_by and updated_dt fields of SHA_Site_Master
            UpdateSHA();
        }
        #endregion

        #region Measurement Specific Panel
        protected void SetupMeasurementSpecificPanel()
        {
            List<int> elementids = new List<int>();
            phElements.Controls.Clear();

            var elements = currSHA.Site.ElementSite.SiteElements.Where(p => p.element_id == 13 || p.element_id == 57 || p.element_id == 124 || p.element_id == 1002 || p.element_id == 1003 || p.element_id == 1004).ToList();

            foreach (var rec in elements)
            {
                ElementJHAs elementJHA = (ElementJHAs)LoadControl("~\\Control\\ElementJHAs.ascx");

                elementJHA.ElementID = Convert.ToInt32(rec.element_id);

                phElements.Controls.Add(elementJHA);

                LiteralControl spacer = new LiteralControl("<br />");
                phElements.Controls.Add(spacer);

                elementids.Add(Convert.ToInt32(rec.element_id));
            }

            PopulateAddMeasTypeDropDown(elementids);
        }

        protected void PopulateAddMeasTypeDropDown(List<int> elementids)
        {
            string site_tp_cd = db.vSITEFILEs.FirstOrDefault(p => p.site_id == currSHA.Site.nwisweb_site_id).site_tp_cd;
            string sims_site_tp = db.SiteTypes.FirstOrDefault(p => p.site_tp_cd == site_tp_cd).sims_site_tp;

            if (sims_site_tp == "sw" & elementids.Count < 5 & (string.IsNullOrEmpty(ddlNewMeasType.SelectedValue) | ddlNewMeasType.SelectedValue == "0"))
            {
                RemoveDropDownItems();

                ddlNewMeasType.Items.Add(new ListItem("", "0"));

                ddlNewMeasType.Items.Add(new ListItem("DISCHARGE MEASUREMENTS", Config.DischargeMeasElem.ToString()));
                ddlNewMeasType.Items.Add(new ListItem("LAKE / RESERVOIR MEASUREMENTS", Config.LakeMeasElem.ToString()));
                ddlNewMeasType.Items.Add(new ListItem("WATER QUALITY MEASUREMENT", Config.QWMeasElem.ToString()));
                ddlNewMeasType.Items.Add(new ListItem("ECOLOGICAL MEASUREMENTS", Config.EcoMeasElem.ToString()));
                ddlNewMeasType.Items.Add(new ListItem("ATMOSPHERIC MEASUREMENTS", Config.AtmMeasElem.ToString()));

                if (elementids.Count > 0)
                {
                    foreach (int id in elementids)
                    {
                        if (id == Config.DischargeMeasElem)
                        {
                            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.DischargeMeasElem.ToString()));
                            break;
                        }
                        if (id == Config.QWMeasElem)
                        {
                            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.QWMeasElem.ToString()));
                            break;
                        }
                        if (id == Config.LakeMeasElem)
                        {
                            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.LakeMeasElem.ToString()));
                            break;
                        }
                        if (id == Config.EcoMeasElem)
                        {
                            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.EcoMeasElem.ToString()));
                            break;
                        }
                        if (id == Config.AtmMeasElem)
                        {
                            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.AtmMeasElem.ToString()));
                            break;
                        }
                    }
                }

                ddlNewMeasType.Visible = true;
                btnNewMeasType.Visible = true;
                ltlNewMeasType.Text = "Select a measurement element and click the button to add. By adding an element here, it will also appear in this site's Station Description.";
            }
            else if (sims_site_tp == "gw" & elementids.Count < 4 & (string.IsNullOrEmpty(ddlNewMeasType.SelectedValue) | ddlNewMeasType.SelectedValue == "0"))
            {
                RemoveDropDownItems();

                ddlNewMeasType.Items.Add(new ListItem("", "0"));

                ddlNewMeasType.Items.Add(new ListItem("GROUNDWATER MEASUREMENTS", Config.GWMeasElem.ToString()));
                ddlNewMeasType.Items.Add(new ListItem("WATER QUALITY MEASUREMENT", Config.QWMeasElem.ToString()));
                ddlNewMeasType.Items.Add(new ListItem("ECOLOGICAL MEASUREMENTS", Config.EcoMeasElem.ToString()));
                ddlNewMeasType.Items.Add(new ListItem("ATMOSPHERIC MEASUREMENTS", Config.AtmMeasElem.ToString()));

                if (elementids.Count > 0)
                {
                    foreach (int id in elementids)
                    {
                        if (id == Config.GWMeasElem)
                        {
                            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.GWMeasElem.ToString()));
                            break;
                        }
                        if (id == Config.QWMeasElem)
                        {
                            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.QWMeasElem.ToString()));
                            break;
                        }
                        if (id == Config.EcoMeasElem)
                        {
                            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.EcoMeasElem.ToString()));
                            break;
                        }
                        if (id == Config.AtmMeasElem)
                        {
                            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.AtmMeasElem.ToString()));
                            break;
                        }
                    }
                }

                ddlNewMeasType.Visible = true;
                btnNewMeasType.Visible = true;
                ltlNewMeasType.Text = "Select a measurement element and click the button to add. By adding an element here, it will also appear in this site's Station Description.";
            }
            else if (string.IsNullOrEmpty(ddlNewMeasType.SelectedValue) | ddlNewMeasType.SelectedValue == "0")
            {
                ddlNewMeasType.Visible = false;
                btnNewMeasType.Visible = false;
                ltlNewMeasType.Text = "All possible measurement elements have been added to this site.";
            }
        }

        protected void RemoveDropDownItems()
        {
            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue("0"));
            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.DischargeMeasElem.ToString()));
            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.QWMeasElem.ToString()));
            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.LakeMeasElem.ToString()));
            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.EcoMeasElem.ToString()));
            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.AtmMeasElem.ToString()));
            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.GWMeasElem.ToString()));
        }

        protected void btnNewMeasType_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
        {
            if (string.IsNullOrEmpty(Session["btnpressed"].ToString()) | Session["btnpressed"] == "true")
            {
                int element_id = Convert.ToInt32(ddlNewMeasType.SelectedValue);
                Data.SiteElement elem_temp = new Data.SiteElement();

                try
                {
                    elem_temp.site_id = SiteID;
                    elem_temp.element_id = element_id;
                    elem_temp.entered_by = user.ID;
                    elem_temp.entered_dt = DateTime.Now;
                    elem_temp.revised_by = user.ID;
                    elem_temp.revised_dt = DateTime.Now;

                    db.SiteElements.InsertOnSubmit(elem_temp);
                    db.SP_Report_Update_Site_LastEdited(SiteID);
                    db.SubmitChanges();

                    DisplayMessage(false, "The element was added!", "element");
                }
                catch (Exception ex)
                {
                    Session["btnpressed"] = "";
                    DisplayMessage(true, ex.Message, "element");
                }

                ddlNewMeasType.SelectedValue = null;
                SetupMeasurementSpecificPanel();
            }
            Session["btnpressed"] = "true";
        }
        #endregion

        #region Site Hazard Analysis Element Panel
        protected void SetupSiteHazardAnalysisElementPanel()
        {
            var se = db.SiteElements.FirstOrDefault(p => p.element_id == Config.SiteHazardElem && p.site_id == SiteID);

            if (se != null)
            {
                pnlNoSiteHazardElem.Visible = false;
                pnlSiteHazardElem.Visible = true;

                ltlElemInfo.Text = se.element_info;
                ltlElemRevisedInfo.Text = String.Format("Revised by: {0} &nbsp;&nbsp;Date revised: {1:MM/dd/yyyy}", se.revised_by, se.revised_dt);

                hlRevisionHistory.NavigateUrl = String.Format("{0}StationDoc/Archive.aspx?element_id={1}&site_id={2}&begin_dt=1/1/1900&end_dt={3}", Config.SIMS2017URL, Config.SiteHazardElem, SiteID, DateTime.Now);

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
            }
            else
            {
                pnlNoSiteHazardElem.Visible = true;
                pnlSiteHazardElem.Visible = false;
            }
        }

        protected void lbToggleElementEditMode_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfToggleElementEditMode.Value))
            {
                hfToggleElementEditMode.Value = "true";
                lbToggleElementEditMode.Text = "leave element edit mode";

                var se = db.SiteElements.FirstOrDefault(p => p.element_id == Config.SiteHazardElem && p.site_id == SiteID);
                reElemInfo.Content = se.element_info;
                pnlEditElemInfo.Visible = true;
                pnlStaticElemInfo.Visible = false;
            }
            else
            {
                hfToggleElementEditMode.Value = "";
                lbToggleElementEditMode.Text = "enter element edit mode";

                pnlEditElemInfo.Visible = false;
                pnlStaticElemInfo.Visible = true;
            }
        }

        protected void btnSubmitElemInfo_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument == "editelement" & !string.IsNullOrEmpty(hfToggleElementEditMode.Value))
            {
                var elem_temp = db.SiteElements.FirstOrDefault(p => p.element_id == Config.SiteHazardElem && p.site_id == SiteID);

                try
                {
                    //First confirm that there is element information to back up
                    if (!string.IsNullOrEmpty(elem_temp.element_info))
                    {
                        //If element_info is not blank, then back up before updating
                        Data.SiteElementBackup backup = new Data.SiteElementBackup()
                        {
                            site_id = elem_temp.site_id,
                            element_id = elem_temp.element_id,
                            element_info = elem_temp.element_info,
                            entered_by = elem_temp.entered_by,
                            entered_dt = elem_temp.entered_dt,
                            revised_by = elem_temp.revised_by,
                            revised_dt = elem_temp.revised_dt,
                            backup_by = user.ID,
                            backup_dt = DateTime.Now
                        };
                        db.SiteElementBackups.InsertOnSubmit(backup);
                    }

                    //Update the element info
                    elem_temp.element_info = reElemInfo.Content;
                    elem_temp.revised_by = user.ID;
                    elem_temp.revised_dt = DateTime.Now;

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

        #region Emergency Info Panel
        private string EmergInfoLink
        {
            get { return String.Format("{0}EmergencyInfo.aspx?office_id={1}", Config.SafetyURL, currSHA.Site.office_id); }
        }

        protected void SetupEmergencyInfoPanel()
        {
            if ((bool)currSHA.emerg_service)
                lbl911Service.Text = "911 emergency service is available at this site.<br />";
            else
                lbl911Service.Text = "911 emergency service is NOT available at this site.<br />";

            if ((bool)currSHA.cell_service)
                lblCellService.Text = "Cell service is available at this site.<br /><br />";
            else
                lblCellService.Text = "Cell service is NOT available at this site.<br /><br />";

            hlEmergInfo1.NavigateUrl = EmergInfoLink;
            hlEmergInfo4.NavigateUrl = EmergInfoLink;
        }

        protected void lbEmergService_Command(object sender, CommandEventArgs e)
        {
            bool es = false;

            if (e.CommandArgument == "911")
            {
                if ((bool)currSHA.emerg_service)
                {
                    es = false;
                    lbl911Service.Text = "911 emergency service is NOT available at this site.<br />";
                }
                else
                {
                    es = true;
                    lbl911Service.Text = "911 emergency service is available at this site.<br />";
                }

                currSHA.emerg_service = es;
            }
            else
            {
                if ((bool)currSHA.cell_service)
                {
                    es = false;
                    lblCellService.Text = "Cell service is NOT available at this site.<br /><br />";
                }
                else
                {
                    es = true;
                    lblCellService.Text = "Cell service is available at this site.<br /><br />";
                }

                currSHA.cell_service = es;
            }

            currSHA.updated_by = user.ID;
            currSHA.updated_dt = DateTime.Now;
            db.SubmitChanges();
        }

        #region "Contacts"
        protected void rgContacts_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            rgContacts.DataSource = currSHA.SHAContacts.Select(p => new Data.Contact()).ToList();
        }

        protected void rgContacts_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgContacts.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" | menu.Items[i].Text == "Contains" | menu.Items[i].Text == "EqualTo" | menu.Items[i].Text == "DoesNotContain")
                {
                    i = i + 1;
                }
                else
                {
                    menu.Items.RemoveAt(i);
                }
            }
        }

        protected void rgContacts_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                try
                {
                    HyperLink deleteLink = (HyperLink)e.Item.FindControl("hlDelete");
                    deleteLink.Attributes["href"] = "#";
                    deleteLink.Attributes["onclick"] = String.Format("return ShowDeleteContactForm('{0}','{1}','{2}');", e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["contact_id"], e.Item.ItemIndex, currSHA.sha_site_id);
                }
                catch (Exception ex) { }
            }
        }

        protected void rgContacts_ItemDataBound(object source, GridItemEventArgs e)
        {
            if (e.Item.IsInEditMode)
            {
                GridEditableItem item = (GridEditableItem)e.Item;

                DropDownList ddlEmergContacts = (DropDownList)item.FindControl("ddlEmergContacts");
                ddlEmergContacts.DataSource = db.Contacts.Where(p => p.wsc_id == currSHA.Site.Office.wsc_id).ToList();
                ddlEmergContacts.DataBind();

                HyperLink hlEmergInfoLink = (HyperLink)item.FindControl("hlEmergInfo2");
                hlEmergInfoLink.NavigateUrl = EmergInfoLink;
            }
        }

        protected void rgContacts_InsertCommand(object sender, GridCommandEventArgs e)
        {
            GridEditableItem item = e.Item as GridEditableItem;
            int contact_id;

            try
            {
                contact_id = Convert.ToInt32((item.FindControl("ddlEmergContacts") as DropDownList).SelectedValue);
            }
            catch (Exception ex)
            {
                DisplayContactMessage(true, "You must select a contact!");
                e.Canceled = true;
                return;
            }

            var c = db.Contacts.FirstOrDefault(p => p.contact_id == contact_id);

            try
            {
                var sha_contact = currSHA.SHAContacts.FirstOrDefault(p => p.contact_id == c.contact_id);
                if (sha_contact == null)
                {
                    Data.SHAContact newSHAContact = new Data.SHAContact() {
                        sha_site_id = currSHA.sha_site_id,
                        contact_id = c.contact_id
                    };

                    db.SHAContacts.InsertOnSubmit(newSHAContact);
                    db.SubmitChanges();

                    DisplayContactMessage(false, "The emergency contact was added!");
                }
                else
                {
                    DisplayContactMessage(false, "The emergency contact is assigned to the site.");
                }
            }
            catch (Exception ex)
            {
                DisplayContactMessage(true, "Unable to add contact. Reason: " + ex.Message);
                e.Canceled = true;
            }

            ltlNotice2.Text = "";
        }

        private void DisplayContactMessage(bool isError, string text)
        {
            if (isError)
            {
                ltlNotice1.Text = "<p style='color:red;font-weight:bold;'>" + text + "</p>";
            }
            else
            {
                ltlNotice1.Text = "<p style='color:green;font-weight:bold;'>" + text + "</p>";
            }
        }
        #endregion

        #region "Hospitals"
        protected void rgHospitals_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            rgHospitals.DataSource = currSHA.SHAHospitals.Select(p => new Data.Hospital()).ToList();
        }

        protected void rgHospitals_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgHospitals.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" | menu.Items[i].Text == "Contains" | menu.Items[i].Text == "EqualTo" | menu.Items[i].Text == "DoesNotContain")
                {
                    i = i + 1;
                }
                else
                {
                    menu.Items.RemoveAt(i);
                }
            }
        }

        protected void rgHospitals_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                try
                {
                    HyperLink deleteLink = (HyperLink)e.Item.FindControl("hlDelete");
                    deleteLink.Attributes["href"] = "#";
                    deleteLink.Attributes["onclick"] = String.Format("return ShowDeleteHospitalForm('{0}','{1}','{2}');", e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["hospital_id"], e.Item.ItemIndex, currSHA.sha_site_id);
                }
                catch (Exception ex)
                {
                }
            }
        }

        protected void rgHospitals_ItemDataBound(object source, GridItemEventArgs e)
        {
            if (e.Item.IsInEditMode)
            {
                GridEditableItem item = (GridEditableItem)e.Item;

                DropDownList ddlHospitals = (DropDownList)item.FindControl("ddlHospitals");
                ddlHospitals.DataSource = db.Hospitals.Where(p => p.wsc_id == currSHA.Site.Office.wsc_id).ToList();
                ddlHospitals.DataBind();

                HyperLink hlEmergInfoLink = (HyperLink)item.FindControl("hlEmergInfo3");
                hlEmergInfoLink.NavigateUrl = EmergInfoLink;
            }
        }

        protected void rgHospitals_InsertCommand(object sender, GridCommandEventArgs e)
        {
            GridEditableItem item = e.Item as GridEditableItem;
            int hospital_id;

            try
            {
                hospital_id = Convert.ToInt32((item.FindControl("ddlHospitals") as DropDownList).SelectedValue);
            }
            catch (Exception ex)
            {
                DisplayHospitalMessage(true, "You must select a hospital!");
                e.Canceled = true;
                return;
            }

            var h = db.Hospitals.FirstOrDefault(p => p.hospital_id == hospital_id);

            try
            {
                var sha_hospital = currSHA.SHAHospitals.FirstOrDefault(p => p.hospital_id == h.hospital_id);
                if (sha_hospital == null)
                {
                    Data.SHAHospital newSHAHospital = new Data.SHAHospital() {
                        sha_site_id = currSHA.sha_site_id,
                        hospital_id = h.hospital_id
                    };

                    db.SHAHospitals.InsertOnSubmit(newSHAHospital);
                    db.SubmitChanges();
                    
                    DisplayHospitalMessage(false, "The hospital was added!");
                }
                else
                {
                    DisplayHospitalMessage(false, "The hospital is assigned to the site.");
                }
            }
            catch (Exception ex)
            {
                DisplayHospitalMessage(true, "Unable to add hospital. Reason: " + ex.Message);
                e.Canceled = true;
            }

            ltlNotice1.Text = "";
        }

        private void DisplayHospitalMessage(bool isError, string text)
        {
            if (isError)
            {
                ltlNotice2.Text = "<p style='color:red;font-weight:bold;'>" + text + "</p>";
            }
            else
            {
                ltlNotice2.Text = "<p style='color:green;font-weight:bold;'>" + text + "</p>";
            }
        }
        #endregion

        #endregion

        #region "Admin Panel"
        protected void SetupAdminPanel()
        {
            ltlReviewedBy.Text = currSHA.reviewed_by;
            ltlReviewerComments.Text = currSHA.reviewer_comments;
            if (String.Format("{0:MM/dd/yyyy}", currSHA.reviewed_dt) == "01/01/1900")
            {
                ltlReviewedDate.Text = "never reviewed";
            }
            else
            {
                ltlReviewedDate.Text = String.Format("{0:MM/dd/yyyy}", currSHA.reviewed_dt);
            }

            ltlApprovedBy.Text = currSHA.approved_by;
            if (String.Format("{0:MM/dd/yyyy}", currSHA.approved_dt) == "01/01/1900")
            {
                ltlApprovedDate.Text = "never approved";
            }
            else
            {
                ltlApprovedDate.Text = String.Format("{0:MM/dd/yyyy}", currSHA.approved_dt);
            }

            if (user.IsSafetyApprover & currSHA.reviewed_dt > currSHA.approved_dt)
            {
                pnlApprove.Visible = true;
            }
        }

        protected void lbReview_Command(object sender, CommandEventArgs e)
        {
            pnlReview.Visible = false;
            pnlReviewSubmit.Visible = true;
        }

        protected void lbApprove_Command(object sender, CommandEventArgs e)
        {
            pnlApprovePreSubmit.Visible = false;
            pnlApproveSubmit.Visible = true;
        }

        protected void btnAdmin_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument == "Reviewed")
            {
                if (Page.IsValid)
                {
                    currSHA.reviewed_by = user.ID;
                    currSHA.reviewed_dt = DateTime.Now;
                    currSHA.reviewer_comments = tbReviewerComments.Text;

                    ltlReviewedBy.Text = user.ID;
                    ltlReviewedDate.Text = DateTime.Now.ToString();
                    ltlReviewerComments.Text = tbReviewerComments.Text;

                    pnlReview.Visible = true;
                    pnlReviewSubmit.Visible = false;
                    if (user.IsSafetyApprover)
                    {
                        pnlApprove.Visible = true;
                    }
                }
            }
            else if (e.CommandArgument == "Approved")
            {
                currSHA.approved_by = user.ID;
                currSHA.approved_dt = DateTime.Now;

                ltlApprovedBy.Text = user.ID;
                ltlApprovedDate.Text = DateTime.Now.ToString();

                pnlApprovePreSubmit.Visible = true;
                pnlApproveSubmit.Visible = false;
            }
            else if (e.CommandArgument == "CancelReview")
            {
                pnlReview.Visible = true;
                pnlReviewSubmit.Visible = false;
            }
            else if (e.CommandArgument == "CancelApprove")
            {
                pnlApprovePreSubmit.Visible = true;
                pnlApproveSubmit.Visible = false;
            }
            db.SubmitChanges();
        }
        #endregion

    }
}