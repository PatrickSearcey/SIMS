using Core;
using System;
using System.Collections;
using Safety.Control;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.IO;

namespace Safety
{
    public partial class Cableways : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private string grid_tp;
        private int activeonly;
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
            string site_no = Request.QueryString["site_no"];
            string agency_cd = Request.QueryString["agency_cd"];
            string showr = Request.QueryString["showr"];
            string office_id = Request.QueryString["office_id"];

            if (!string.IsNullOrEmpty(office_id))
                OfficeID = Convert.ToInt32(office_id);
            else
                OfficeID = user.OfficeID;

            //Using the passed office_id, reset the wsc to match that of the current office
            WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            //If the site number was passed, then setup the SiteID and also reset the OfficeID and WSCID to that of the site
            if (!string.IsNullOrEmpty(site_no) && !string.IsNullOrEmpty(agency_cd))
            {
                var site = db.Sites.FirstOrDefault(p => p.site_no == site_no && p.agency_cd == agency_cd);
                if (site != null)
                {
                    SiteID = site.site_id;
                    OfficeID = Convert.ToInt32(site.office_id);
                    WSCID = Convert.ToInt32(site.Office.wsc_id);
                }
            }

            //--BASIC PAGE SETUP--------------------------------------------------------------------
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            var wsc = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID);
            ph1.Title = "Manage Cableways";
            ph1.SubTitle = "For the " + wsc.wsc_nm + " WSC";

            if (!Page.IsPostBack)
            {
                //Clear session state variables
                ResetSessionStateVariables();

                //--APPROVER ACCESS SECTION-------------------------------------------------------------
                if (user.IsSuperUser || user.WSCID.Contains(WSCID) && user.IsSafetyApprover)
                    Session["AllowEdits"] = "true";
                else
                    Session["AllowEdits"] = "false";
                //--END PAGE ACCESS SECTION---------------------------------------------------------

                if (string.IsNullOrEmpty(showr))
                {
                    lbToggleRStatus.Text = "Click to view removed or remediated cableways";
                    lbToggleRStatus.CommandArgument = "viewR";
                }
                else if (showr == "y")
                {
                    Session["ShowRStatusCableways"] = "true";
                    lbToggleRStatus.Text = "Click to view all cableways";
                    lbToggleRStatus.CommandArgument = "hideR";
                }
            }
        }

        protected void ResetSessionStateVariables()
        {
            Session["AllowEdits"] = "";
            Session["ShowRStatusCableways"] = "false";
        }

        #region Properties
        private List<Data.Site> SiteList
        {
            get
            {
                var sites = (from p in db.Sites
                        join v in db.vSITEFILEs on p.nwisweb_site_id equals v.site_id
                        join t in db.SiteTypes on v.site_tp_cd equals t.site_tp_cd
                        where p.Office.wsc_id == WSCID && t.sims_site_tp == "sw"
                        select new Data.Site()
                        {
                            site_id = p.site_id,
                            site_no = p.site_no,
                            agency_cd = p.agency_cd,
                            station_full_nm = p.station_full_nm,
                            office_id = p.office_id
                        }).ToList();
                return sites;
            }
        }
        #endregion

        public void lbToggleRStatus_Command(object source, CommandEventArgs e)
        {
            if (e.CommandArgument == "viewR")
            {
                Session["ShowRStatusCableways"] = "true";
                lbToggleRStatus.Text = "Click to view all cableways";
                lbToggleRStatus.CommandArgument = "hideR";
            }
            else if (e.CommandArgument == "hideR")
            {
                Session["ShowRStatusCableways"] = "false";
                lbToggleRStatus.Text = "Click to view removed or remediated cableways";
                lbToggleRStatus.CommandArgument = "viewR";
            }

            rgCableways.Rebind();
        }

        private void rgCableways_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if (!e.IsFromDetailTable)
            {
                var cableways = db.Cableways.Select(p => new {
                        cableway_id = p.cableway_id,
                        site_id = p.site_id,
                        site_no_nm = p.Site.site_no + " " + p.Site.station_full_nm,
                        status_cd_desc = p.cableway_status_cd + " - " + p.CablewayStatus.cableway_status_desc,
                        type_cd_desc = p.cableway_type_cd + " - " + p.CablewayType.cableway_type_desc,
                        cableway_inspection_freq = p.cableway_inspection_freq,
                        aerial_marker_req = p.aerial_marker_req,
                        aerial_marker_inst = p.aerial_marker_inst,
                        office_cd = p.Site.Office.office_cd,
                        wsc_id = p.Site.Office.wsc_id,
                        cableway_type_cd = p.cableway_type_cd
                    });

                if (Session["ShowRStatusCableways"] == "false")
                {
                    //We do not want any cableways to be shown with a status of "r - Full removal and site remediated"
                    rgCableways.DataSource = cableways.Where(p => p.wsc_id == WSCID && p.cableway_type_cd != "r").ToList();
                }
                else
                {
                    //Show only cableways with a status of "r - Full removal and site remediated"
                    rgCableways.DataSource = cableways.Where(p => p.wsc_id == WSCID && p.cableway_type_cd == "r").ToList();
                }

                if ((!Page.IsPostBack))
                {
                    try
                    {
                        if (SiteID > 0)
                        {
                            var site = db.Sites.FirstOrDefault(p => p.site_id == SiteID);
                            if (site != null)
                            {
                                rgCableways.MasterTableView.FilterExpression = "([site_no_nm] Like '%" + site.site_no + "%')";
                                GridColumn column = rgCableways.MasterTableView.GetColumnSafe("site_no_nm");
                                column.CurrentFilterFunction = GridKnownFunction.Contains;
                                column.CurrentFilterValue = site.site_no;
                            }
                        }
                        
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        private void rgCableways_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        {
            GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;
            switch (e.DetailTableView.Name)
            {
                case "Visits":
                    int cableway_id = Convert.ToInt32(dataItem.GetDataKeyValue("cableway_id"));
                    var cw = db.Cableways.FirstOrDefault(p => p.cableway_id == cableway_id);
                    e.DetailTableView.DataSource = cw.CablewayVisits.Select(p => new {
                        cableway_visit_id = p.cableway_visit_id,
                        visit_dt = p.visit_dt,
                        type_cd_desc = p.visit_type_cd + " - " + p.CablewayVisitType.visit_type_desc,
                        action_cd_desc = p.visit_action_cd + " - " + p.CablewayVisitAction.visit_action_desc,
                        visit_file_nm = p.visit_file_nm,
                        remarks = p.remarks
                    }).ToList();
                    break;
            }
        }

        protected void rgCableways_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgCableways.FilterMenu;
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

        protected void rgCableways_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if ("Visits".Equals(e.Item.OwnerTableView.Name))
            {
                if (e.Item.IsInEditMode)
                {
                    GridEditableItem item = (GridEditableItem)e.Item;

                    Literal ltlDetailsTitle = (Literal)item.FindControl("ltlDetailsEditFormTitle");
                    RadDatePicker rdpVisit = (RadDatePicker)item.FindControl("rdpVisit");
                    DropDownList ddlVisitType = (DropDownList)item.FindControl("ddlVisitType");
                    DropDownList ddlVisitAction = (DropDownList)item.FindControl("ddlVisitAction");
                    TextBox tbRemarks = (TextBox)item.FindControl("tbRemarks");
                    Button btnUpdate1 = (Button)item.FindControl("btnUpdate1");
                    Button btnInsert1 = (Button)item.FindControl("btnInsert1");
                    RadAsyncUpload upload = (RadAsyncUpload)item.FindControl("fuFile");
                    Label lblUploadDoc = (Label)item.FindControl("lblUploadDoc");
                    Image imgUploadDocHelp = (Image)item.FindControl("imgUploadDocHelp");

                    ddlVisitType.DataSource = db.CablewayVisitTypes.Select(p => new {
                        visit_type_cd = p.visit_type_cd,
                        type_cd_desc = p.visit_type_cd + " - " + p.visit_type_desc
                    }).ToList();
                    ddlVisitAction.DataSource = db.CablewayVisitActions.Select(p => new {
                        visit_action_cd = p.visit_action_cd,
                        action_cd_desc = p.visit_action_cd + " - " + p.visit_action_desc
                    }).ToList();

                    if (!(e.Item is IGridInsertItem))
                    {
                        ltlDetailsTitle.Text = "Edit Visit";
                        int cableway_visit_id = Convert.ToInt32(item.GetDataKeyValue("cableway_visit_id"));
                        var visit = db.CablewayVisits.FirstOrDefault(p => p.cableway_visit_id == cableway_visit_id);

                        rdpVisit.SelectedDate = visit.visit_dt;
                        ddlVisitType.SelectedValue = visit.visit_type_cd;
                        ddlVisitAction.SelectedValue = visit.visit_action_cd;
                        tbRemarks.Text = visit.remarks;

                        if (!string.IsNullOrEmpty(visit.visit_file_nm))
                        {
                            upload.Visible = false;
                            lblUploadDoc.Visible = false;
                            imgUploadDocHelp.Visible = false;
                        }
                        else
                        {
                            upload.Visible = true;
                            lblUploadDoc.Visible = true;
                            imgUploadDocHelp.Visible = true;
                        }

                        btnUpdate1.Visible = true;
                        btnInsert1.Visible = false;
                    }
                    else if ((e.Item is IGridInsertItem))
                    {
                        ltlDetailsTitle.Text = "Add New Visit";

                        btnUpdate1.Visible = false;
                        btnInsert1.Visible = true;
                    }

                    ddlVisitType.DataBind();
                    ddlVisitAction.DataBind();
                }
                else
                {
                    if (e.Item is GridDataItem)
                    {
                        GridDataItem item = (GridDataItem)e.Item;

                        int cableway_visit_id = Convert.ToInt32(item.GetDataKeyValue("cableway_visit_id"));
                        var visit = db.CablewayVisits.FirstOrDefault(p => p.cableway_visit_id == cableway_visit_id);
                        if (!string.IsNullOrEmpty(visit.visit_file_nm))
                        {
                            HyperLink hlDoc = (HyperLink)item.FindControl("hlDoc");
                            hlDoc.Attributes["href"] = String.Format("{0}Files/Cableways/{1}/{2}", visit.cableway_id, visit.visit_file_nm);
                        }
                    }
                }

                if (e.Item is GridDataItem)
                {
                    GridDataItem item = (GridDataItem)e.Item;
                    if (Session["AllowEdits"].ToString() == "false")
                    {
                        ImageButton ib1 = (ImageButton)item["EditCommandColumn2"].Controls[0];
                        ib1.Visible = false;
                        ImageButton ib2 = (ImageButton)item["VisitDeleteColumn"].Controls[0];
                        ib2.Visible = false;
                    }
                }
            }
            else
            {
                if (e.Item.IsInEditMode)
                {
                    GridEditableItem item = (GridEditableItem)e.Item;

                    Literal ltlTitle = (Literal)item.FindControl("ltlEditFormTitle");
                    DropDownList ddlSites = (DropDownList)item.FindControl("ddlSites");
                    TextBox tbNickname = (TextBox)item.FindControl("tbNickname");
                    DropDownList ddlCablewayStatus = (DropDownList)item.FindControl("ddlCablewayStatus");
                    DropDownList ddlCablewayType = (DropDownList)item.FindControl("ddlCablewayType");
                    RadNumericTextBox rntbFreq = (RadNumericTextBox)item.FindControl("rntbFreq");
                    RadioButtonList rblAerialMarkerReq = (RadioButtonList)item.FindControl("rblAerialMarkerReq");
                    RadioButtonList rblAerialMarkerInst = (RadioButtonList)item.FindControl("rblAerialMarkerInst");
                    Button btnUpdate2 = (Button)item.FindControl("btnUpdate2");
                    Button btnInsert2 = (Button)item.FindControl("btnInsert2");

                    ddlSites.DataSource = SiteList;
                    ddlCablewayStatus.DataSource = db.CablewayStatus.Select(p => new {
                        status_cd_desc = p.cableway_status_cd + " - " + p.cableway_status_desc,
                        cableway_status_cd = p.cableway_status_cd
                    });
                    ddlCablewayType.DataSource = db.CablewayTypes.Select(p => new {
                        type_cd_desc = p.cableway_type_cd + " - " + p.cableway_type_desc,
                        cableway_type_cd = p.cableway_type_cd
                    });

                    if (!(e.Item is IGridInsertItem))
                    {
                        ltlTitle.Text = "Edit Cableway";
                        int cableway_id = Convert.ToInt32(item.GetDataKeyValue("cableway_id"));
                        var cableway = db.Cableways.FirstOrDefault(p => p.cableway_id == cableway_id);
                        ddlSites.SelectedValue = cableway.site_id.ToString();
                        tbNickname.Text = cableway.cableway_nm;
                        ddlCablewayStatus.SelectedValue = cableway.cableway_status_cd;
                        ddlCablewayType.SelectedValue = cableway.cableway_type_cd;
                        rntbFreq.Text = cableway.cableway_inspection_freq.ToString();
                        rblAerialMarkerReq.SelectedValue = cableway.aerial_marker_req;
                        rblAerialMarkerInst.SelectedValue = cableway.aerial_marker_req;

                        btnUpdate2.Visible = true;
                        btnInsert2.Visible = false;
                    }
                    else if ((e.Item is IGridInsertItem))
                    {
                        ltlTitle.Text = "Add New Cableway";

                        rblAerialMarkerReq.SelectedValue = "false";
                        rblAerialMarkerInst.SelectedValue = "false";

                        btnUpdate2.Visible = false;
                        btnInsert2.Visible = true;
                    }

                    ddlSites.DataBind();
                    ddlCablewayStatus.DataBind();
                    ddlCablewayType.DataBind();
                }

                if (e.Item is GridDataItem)
                {
                    GridDataItem item = (GridDataItem)e.Item;
                    try
                    {
                        if (Session["AllowEdits"].ToString() == "false")
                        {
                            ImageButton ib3 = (ImageButton)item["EditCommandColumn1"].Controls[0];
                            ib3.Visible = false;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

        }

        protected void rgCableways_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            GridDataItem item = e.Item as GridDataItem;

            if (e.Item.OwnerTableView.Name == "Cableways")
            {
                //The ability to delete cableways was removed
                //!---This section of code not in use!!---!
                try
                {
                    int cableway_id = Convert.ToInt32(item.GetDataKeyValue("cableway_id"));
                    var deleteCW = db.Cableways.FirstOrDefault(p => p.cableway_id == cableway_id);
                    db.CablewayVisits.DeleteAllOnSubmit(deleteCW.CablewayVisits);
                    db.Cableways.DeleteOnSubmit(deleteCW);

                    db.SubmitChanges();
                    DisplayMessage(false, "The cableway was deleted!");

                }
                catch (Exception ex)
                {
                    DisplayMessage(true, "Unable to delete cableway. Reason: " + ex.Message);

                    e.Canceled = true;
                }
                //!---to here---!
            }
            else
            {
                try
                {
                    int cableway_visit_id = Convert.ToInt32(item.GetDataKeyValue("cableway_visit_id"));
                    var deleteVisit = db.CablewayVisits.FirstOrDefault(p => p.cableway_visit_id == cableway_visit_id);
                    db.CablewayVisits.DeleteOnSubmit(deleteVisit);

                    DumpMyFile(deleteVisit.cableway_id.ToString(), deleteVisit.visit_file_nm);

                    db.SubmitChanges();

                    DisplayMessage(false, "The cableway visit was deleted!");
                }
                catch (Exception ex)
                {
                    DisplayMessage(true, "Unable to delete cableway visit. Reason: " + ex.Message);

                    e.Canceled = true;
                }
            }
        }

        protected void DumpMyFile(string cableway_id, string file_nm)
        {
            try
            {
                FileInfo myFile = new FileInfo(Server.MapPath("~/Files/Cableways/") + "\\" + cableway_id + "\\" + file_nm);
                if (myFile.Exists)
                {
                    File.Delete(Server.MapPath("~/Files/Cableways/") + "\\" + cableway_id + "\\" + file_nm);
                }

            }
            catch (Exception ex)
            {
            }
        }

        protected void rgCableways_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            GridEditableItem item = e.Item as GridEditableItem;

            if (e.Item.OwnerTableView.Name == "Cableways")
            {
                int cableway_id = Convert.ToInt32(item.GetDataKeyValue("cableway_id"));
                var updateCW = db.Cableways.FirstOrDefault(p => p.cableway_id == cableway_id);

                Hashtable newValues = new Hashtable();

                updateCW.site_id = Convert.ToInt32((item.FindControl("ddlSites") as DropDownList).SelectedValue);
                updateCW.cableway_nm = (item.FindControl("tbNickname") as TextBox).Text;
                updateCW.cableway_status_cd = (item.FindControl("ddlCablewayStatus") as DropDownList).SelectedValue.ToString();
                updateCW.cableway_type_cd = (item.FindControl("ddlCablewayType") as DropDownList).SelectedValue.ToString();
                updateCW.cableway_inspection_freq = Convert.ToDouble((item.FindControl("rntbFreq") as RadNumericTextBox).Text);
                updateCW.aerial_marker_req = (item.FindControl("rblAerialMarkerReq") as RadioButtonList).SelectedValue.ToString();
                updateCW.aerial_marker_inst = (item.FindControl("rblAerialMarkerInst") as RadioButtonList).SelectedValue.ToString();
                updateCW.updated_by = user.ID;
                updateCW.updated_dt = DateTime.Now;

                try
                {
                    db.SubmitChanges();
                    DisplayMessage(false, "The cableway was updated!");

                }
                catch (Exception ex)
                {
                    DisplayMessage(true, "Unable to update cableway. Reason: " + ex.Message);

                    e.Canceled = true;
                }
            }
            else
            {
                int cableway_visit_id = Convert.ToInt32(item.GetDataKeyValue("cableway_visit_id"));
                var updateVisit = db.CablewayVisits.FirstOrDefault(p => p.cableway_visit_id == cableway_visit_id);

                updateVisit.visit_dt = (item.FindControl("rdpVisit") as RadDatePicker).SelectedDate;
                updateVisit.visit_type_cd = (item.FindControl("ddlVisitType") as DropDownList).SelectedValue.ToString();
                updateVisit.visit_action_cd = (item.FindControl("ddlVisitAction") as DropDownList).SelectedValue.ToString();
                updateVisit.remarks = (item.FindControl("tbRemarks") as TextBox).Text;
                updateVisit.updated_by = user.ID;
                updateVisit.updated_dt = DateTime.Now;

                try
                {
                    RadAsyncUpload uploader = (RadAsyncUpload)e.Item.FindControl("fuFile");
                    if (uploader.UploadedFiles.Count > 0)
                    {
                        string status_msg = SaveMyFile(uploader.UploadedFiles[0], updateVisit.cableway_id, Convert.ToDateTime(updateVisit.visit_dt));
                        if (!(status_msg == "fail"))
                        {
                            updateVisit.visit_file_nm = status_msg;
                        }
                    }

                    db.SubmitChanges();
                    DisplayMessage(false, "The cableway visit was updated!");

                }
                catch (Exception ex)
                {
                    DisplayMessage(true, "Unable to update cableway visit. Reason: " + ex.Message);

                    e.Canceled = true;
                }
            }
        }

        protected void rgCableways_InsertCommand(object sender, GridCommandEventArgs e)
        {
            GridEditableItem item = e.Item as GridEditableItem;

            if (e.Item.OwnerTableView.Name == "Cableways")
            {
                Data.Cableway newCW = new Data.Cableway();

                newCW.site_id = Convert.ToInt32((item.FindControl("ddlSites") as DropDownList).SelectedValue);
                newCW.cableway_nm = (item.FindControl("tbNickname") as TextBox).Text;
                newCW.cableway_status_cd = (item.FindControl("ddlCablewayStatus") as DropDownList).SelectedValue;
                newCW.cableway_type_cd = (item.FindControl("ddlCablewayType") as DropDownList).SelectedValue;
                newCW.cableway_inspection_freq = Convert.ToDouble((item.FindControl("rntbFreq") as RadNumericTextBox).Text);
                newCW.aerial_marker_req = (item.FindControl("rblAerialMarkerReq") as RadioButtonList).SelectedValue;
                newCW.aerial_marker_inst = (item.FindControl("rblAerialMarkerInst") as RadioButtonList).SelectedValue;
                newCW.created_by = user.ID;
                newCW.created_dt = DateTime.Now;
                newCW.updated_by = user.ID;
                newCW.updated_dt = DateTime.Now;

                try
                {
                    db.Cableways.InsertOnSubmit(newCW);
                    db.SubmitChanges();
                    DisplayMessage(false, "The cableway was added!");
                }
                catch (Exception ex)
                {
                    DisplayMessage(true, "Unable to insert cableway. Reason: " + ex.Message);
                    e.Canceled = true;
                }
            }
            else
            {
                Data.CablewayVisit newVisit = new Data.CablewayVisit();

                GridDataItem parentItem = (GridDataItem)e.Item.OwnerTableView.ParentItem;
                newVisit.cableway_id = Convert.ToInt32(parentItem.OwnerTableView.DataKeyValues[parentItem.ItemIndex]["cableway_id"]);
                newVisit.visit_dt = (item.FindControl("rdpVisit") as RadDatePicker).SelectedDate;
                newVisit.visit_type_cd = (item.FindControl("ddlVisitType") as DropDownList).SelectedValue;
                newVisit.visit_action_cd = (item.FindControl("ddlVisitAction") as DropDownList).SelectedValue;
                newVisit.remarks = (item.FindControl("tbRemarks") as TextBox).Text;
                newVisit.created_by = user.ID;
                newVisit.created_dt = DateTime.Now;
                newVisit.updated_by = user.ID;
                newVisit.updated_dt = DateTime.Now;

                RadAsyncUpload uploader = (RadAsyncUpload)e.Item.FindControl("fuFile");
                if (uploader.UploadedFiles.Count > 0)
                {
                    string status_msg = SaveMyFile(uploader.UploadedFiles[0], newVisit.cableway_id, Convert.ToDateTime(newVisit.visit_dt));
                    if (!(status_msg == "fail"))
                    {
                        newVisit.visit_file_nm = status_msg;
                    }
                }

                try
                {
                    db.CablewayVisits.InsertOnSubmit(newVisit);
                    db.SubmitChanges();
                    DisplayMessage(false, "The cableway visit was added!");
                }
                catch (Exception ex)
                {
                    DisplayMessage(true, "Unable to insert cableway visit. Reason: " + ex.Message);
                    e.Canceled = true;
                }
            }

        }

        private void DisplayMessage(bool isError, string text)
        {
            Label label = (isError) ? this.lblError : this.lblSuccess;
            label.Text = text;
        }

        private string SaveMyFile(UploadedFile uploadedFile, int cableway_id, DateTime visit_dt)
        {
            string status_msg = "fail";
            string savePath = Server.MapPath("~/Files/Cableways/");
            string fullfileName = string.Format("{0:yyyyMMdd}", visit_dt) + "_" + uploadedFile.GetName();
            string dirToCheck = savePath + cableway_id.ToString() + "\\";
            string pathToCheck = null;

            try
            {
                //Check to see if there's a pre-existing directory for this site ID
                if (!Directory.Exists(dirToCheck))
                {
                    //If it does not exist, create the site ID directory
                    Directory.CreateDirectory(dirToCheck);
                }

                pathToCheck = dirToCheck + fullfileName;

                if (File.Exists(pathToCheck))
                {
                    status_msg = "exists";
                }
                else
                {
                    uploadedFile.SaveAs(pathToCheck);
                    status_msg = fullfileName;
                }
            }
            catch (Exception ex)
            {
            }

            return status_msg;
        }
    }
}