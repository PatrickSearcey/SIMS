using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SIMS2017.Admin
{
    public partial class Personnel : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        public Boolean HasEditAccess { get; set; }
        private Data.WSC currWSC;
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
            string office_id = Request.QueryString["office_id"];
            if (!string.IsNullOrEmpty(office_id)) OfficeID = Convert.ToInt32(office_id); else if (OfficeID == 0) Response.Redirect(Config.SIMS2017URL + "SIMSWSCHome.aspx");

            WSCID = Convert.ToInt32(db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id);
            currWSC = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID);

            ph1.Title = "Manage Personnel";
            ph1.SubTitle = currWSC.wsc_nm + " Water Science Center";
            ph1.ShowOfficeInfoPanel = true;

            if (!Page.IsPostBack)
            {
                //If the user belongs to this site's WSC (or has an exception to work in the WSC), or is a SuperUser, then allow them to edit the page
                if (user.WSCID.Contains(WSCID) && user.IsAdmin || user.IsSuperUser) HasEditAccess = true;
                
                if (!HasEditAccess)
                {
                    pnlNoAccess.Visible = true;
                    pnlHasAccess.Visible = false;
                }
                else
                {
                    pnlNoAccess.Visible = false;
                    pnlHasAccess.Visible = true;

                    if (!user.IsAdmin && !user.IsSuperUser)
                    {
                        Session["CanEdit"] = "false";
                        rgPersonnel.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
                    }
                    else
                        Session["CanEdit"] = "true";
                }

                Session["ShowActiveOnly"] = "True";

                if (Session["ShowActiveOnly"].ToString() == "False")
                {
                    Session["ShowActiveOnly"] = "False";
                    lbToggleActive.Text = "Click to view only active users";
                    lbToggleActive.CommandArgument = "ActiveOnly";
                }
                else if (Session["ShowActiveOnly"].ToString() == "True")
                {
                    Session["ShowActiveOnly"] = "True";
                    lbToggleActive.Text = "Click to view all users";
                    lbToggleActive.CommandArgument = "ShowAll";
                }

            }
        }

        #region Internal Classes
        internal class LevelItem
        {
            private string _level;

            public string level
            {
                get { return _level; }
                set { _level = value; }
            }

            public LevelItem()
            {
                _level = level;
            }
        }
        #endregion

        #region Properties
        private List<LevelItem> Levels
        {
            get
            {
                List<LevelItem> _levels = new List<LevelItem>();
                if (user.IsSuperUser)
                    _levels = db.Employees.Select(p => new LevelItem() { level = p.administrator_va }).Distinct().OrderBy(p => p.level).ToList();
                else
                    _levels = db.Employees.Where(p => p.administrator_va != "SuperUser").Select(p => new LevelItem() { level = p.administrator_va }).Distinct().OrderBy(p => p.level).ToList();
                return _levels;
            }
        }
        #endregion

        #region Page Methods and Events
        public void lbToggleActive_Command(object source, CommandEventArgs e)
        {
            if (e.CommandArgument.ToString() == "ActiveOnly")
            {
                Session["ShowActiveOnly"] = "True";
                lbToggleActive.Text = "Click to view all users";
                lbToggleActive.CommandArgument = "ShowAll";
            }
            else if (e.CommandArgument.ToString() == "ShowAll")
            {
                Session["ShowActiveOnly"] = "False";
                lbToggleActive.Text = "Click to view only active users";
                lbToggleActive.CommandArgument = "ActiveOnly";
            }

            rgPersonnel.Rebind();
        }

        private void DisplayMessage(bool isError, string text)
        {
            Label label = (isError) ? this.lblError : this.lblSuccess;
            label.Text = text;
        }
        #endregion

        #region Personnel RadGrid
        protected void rgPersonnel_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var personnel = db.Employees.Where(p => p.Office.wsc_id == WSCID).Select(p => new
            {
                    user_id = p.user_id,
                    office_nm = p.Office.office_nm,
                    user_nm = p.first_nm + " " + p.last_nm,
                    administrator_va = p.administrator_va,
                    pass_access = p.pass_access,
                    approver_va = p.approver_va,
                    active = p.active,
                    show_reports = p.show_reports
                }).OrderBy(p => p.office_nm).ThenBy(p => p.user_id).ToList();

            if (Session["ShowActiveOnly"].ToString() == "False")
                rgPersonnel.DataSource = personnel;
            else
                rgPersonnel.DataSource = personnel.Where(p => p.active.Equals(true));

        }

        protected void rgPersonnel_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgPersonnel.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" | menu.Items[i].Text == "Contains" | menu.Items[i].Text == "EqualTo" | menu.Items[i].Text == "DoesNotContain")
                    i = i + 1;
                else
                    menu.Items.RemoveAt(i);
            }
        }

        protected void rgPersonnel_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.IsInEditMode)
            {
                GridEditableItem item = (GridEditableItem)e.Item;

                Panel pnlUpdate = (Panel)item.FindControl("pnlUpdate");
                Panel pnlInsert1 = (Panel)item.FindControl("pnlInsert1");
                Panel pnlInsert2 = (Panel)item.FindControl("pnlInsert2");
                RadTextBox rtbUserID = (RadTextBox)item.FindControl("rtbUserID");
                RadTextBox rtbFirstNm = (RadTextBox)item.FindControl("rtbFirstNm");
                RadTextBox rtbLastNm = (RadTextBox)item.FindControl("rtbLastNm");
                RadDropDownList rddlAdminLevel = (RadDropDownList)item.FindControl("rddlAdminLevel");
                RadDropDownList rddlPASSLevel = (RadDropDownList)item.FindControl("rddlPASSLevel");
                RadRadioButtonList rrblSafetyApprover = (RadRadioButtonList)item.FindControl("rrblSafetyApprover");
                RadDropDownList rddlOffice = (RadDropDownList)item.FindControl("rddlOffice");
                RadRadioButtonList rrblStatus = (RadRadioButtonList)item.FindControl("rrblStatus");
                RadCheckBox rcbReports = (RadCheckBox)item.FindControl("rcbReports");
                RadDropDownList rddlAdminLevel2 = (RadDropDownList)item.FindControl("rddlAdminLevel2");
                RadDropDownList rddlPASSLevel2 = (RadDropDownList)item.FindControl("rddlPASSLevel2");
                RadDropDownList rddlOffice2 = (RadDropDownList)item.FindControl("rddlOffice2");
                RadRadioButtonList rrblSafetyApprover2 = (RadRadioButtonList)item.FindControl("rrblSafetyApprover2");
                RadRadioButtonList rrblStatus2 = (RadRadioButtonList)item.FindControl("rrblStatus2");

                if (!(e.Item is IGridInsertItem))
                {
                    string user_id = item.GetDataKeyValue("user_id").ToString();
                    var currUser = db.Employees.FirstOrDefault(p => p.user_id == user_id);

                    rddlAdminLevel.DataSource = Levels;
                    rddlPASSLevel.DataSource = Levels;
                    rddlOffice.DataSource = db.Offices.Where(p => p.wsc_id == WSCID).Select(p => new { office_id = p.office_id, office_nm = p.office_nm }).ToList();
                    rddlAdminLevel.DataBind();
                    rddlPASSLevel.DataBind();
                    rddlOffice.DataBind();

                    rtbUserID.Text = currUser.user_id;
                    rtbFirstNm.Text = currUser.first_nm;
                    rtbLastNm.Text = currUser.last_nm;
                    rddlAdminLevel.SelectedValue = currUser.administrator_va;
                    rddlPASSLevel.SelectedValue = currUser.pass_access;
                    if (currUser.approver_va.ToString() != null)
                        rrblSafetyApprover.SelectedValue = currUser.approver_va.ToString().ToLower();
                    else
                        rrblSafetyApprover.SelectedValue = "false";
                    rddlOffice.SelectedValue = currUser.Office.office_id.ToString();
                    rrblStatus.SelectedValue = currUser.active.ToString().ToLower();
                    rcbReports.Checked = currUser.show_reports;

                    pnlUpdate.Visible = true;
                    pnlInsert1.Visible = false;
                    pnlInsert2.Visible = false;
                }
                else if ((e.Item is IGridInsertItem))
                {
                    rddlAdminLevel2.DataSource = Levels;
                    rddlPASSLevel2.DataSource = Levels;
                    rddlOffice2.DataSource = db.Offices.Where(p => p.wsc_id == WSCID).Select(p => new { office_id = p.office_id, office_nm = p.office_nm }).ToList();
                    rddlAdminLevel2.DataBind();
                    rddlPASSLevel2.DataBind();
                    rddlOffice2.DataBind();
                    rrblSafetyApprover2.SelectedValue = "false";
                    rrblStatus2.SelectedValue = "true";

                    pnlUpdate.Visible = false;
                    pnlInsert1.Visible = true;
                    pnlInsert2.Visible = false;
                }
            }

            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                try
                {
                    if (Session["CanEdit"].ToString()== "false")
                    {
                        ImageButton ib3 = (ImageButton)item["EditCommandColumn"].Controls[0];
                        ib3.Visible = false;
                    }
                }
                catch (Exception ex) { }
            }

        }

        protected void rgPersonnel_ItemCommand(object sender, GridCommandEventArgs e)
        {
            GridEditableItem item = e.Item as GridEditableItem;

            if (e.CommandName == "ValidateUser")
            {
                Panel pnlInsert1 = (Panel)item.FindControl("pnlInsert1");
                Panel pnlInsert2 = (Panel)item.FindControl("pnlInsert2");
                RadTextBox rtbUserID2 = (RadTextBox)item.FindControl("rtbUserID2");
                RadTextBox rtbFirstNm = (RadTextBox)item.FindControl("rtbFirstNm2");
                RadTextBox rtbLastNm = (RadTextBox)item.FindControl("rtbLastNm2");
                RadTextBox rtbUserID3 = (RadTextBox)item.FindControl("rtbUserID3");

                var currUser = db.Employees.FirstOrDefault(p => p.user_id == rtbUserID2.Text);

                if (currUser != null)
                {
                    DisplayMessage(true, "This user is already in the SIMS database. If the user you are trying to add is moving from a different WSC, their PrimaryOU information in Active Directory must first be changed. Then, an email should be sent to GS-W Help SIMS for help with adding them to your WSC's personnel list.");
                    pnlInsert1.Visible = true;
                    pnlInsert2.Visible = false;
                    rtbUserID2.Text = "";
                }
                else
                {
                    var ADuser = db.spz_GetUserInfoFromAD(rtbUserID2.Text).Select(p => new Data.ADUserInfo { primaryOU = p.primaryOU, SN = p.SN, givenName = p.givenName }).FirstOrDefault();
                    if (ADuser != null)
                    {
                        var currUserWSC = db.spz_GetUserWSCID(ADuser.primaryOU).FirstOrDefault();
                        if (currUserWSC.wsc_id != WSCID)
                        {
                            DisplayMessage(true, "This user belongs to another WSC. Please contact GS-W Help SIMS@usgs.gov for assistance.");
                            pnlInsert1.Visible = true;
                            pnlInsert2.Visible = false;
                            rtbUserID2.Text = "";
                        }
                        else
                        {
                            rtbFirstNm.Text = ADuser.givenName;
                            rtbLastNm.Text = ADuser.SN;
                            rtbUserID3.Text = rtbUserID2.Text;

                            pnlInsert1.Visible = false;
                            pnlInsert2.Visible = true;
                        }
                    }
                    else
                    {
                        DisplayMessage(true, "This user is not authorized. Please confirm that you are entering the user's Active Directory user ID, and not their email alias.");
                        pnlInsert1.Visible = true;
                        pnlInsert2.Visible = false;
                        rtbUserID2.Text = "";
                    }
                }
            }

        }

        protected void rgPersonnel_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            GridEditableItem item = e.Item as GridEditableItem;

            string user_id = item.GetDataKeyValue("user_id").ToString();
            var currUser = db.Employees.FirstOrDefault(p => p.user_id == user_id);

            int office_id = Convert.ToInt32((item.FindControl("rddlOffice") as RadDropDownList).SelectedValue);
            string first_nm = (item.FindControl("rtbFirstNm") as RadTextBox).Text;
            string last_nm = (item.FindControl("rtbLastNm") as RadTextBox).Text;
            string administrator_va = (item.FindControl("rddlAdminLevel") as RadDropDownList).SelectedValue.ToString();
            string pass_access = (item.FindControl("rddlPASSLevel") as RadDropDownList).SelectedValue.ToString();
            Boolean approver_va = Convert.ToBoolean((item.FindControl("rrblSafetyApprover") as RadRadioButtonList).SelectedValue);
            Boolean active = Convert.ToBoolean((item.FindControl("rrblStatus") as RadRadioButtonList).SelectedValue);
            Boolean show_reports = Convert.ToBoolean((item.FindControl("rcbReports") as RadCheckBox).Checked);

            currUser.first_nm = first_nm;
            currUser.last_nm = last_nm;
            currUser.office_id = office_id;
            currUser.administrator_va = administrator_va;
            currUser.pass_access = pass_access;
            currUser.approver_va = approver_va;
            currUser.active = active;
            currUser.show_reports = show_reports;

            db.SubmitChanges();

            DisplayMessage(false, "The user's info was updated!");
        }

        protected void rgPersonnel_InsertCommand(object sender, GridCommandEventArgs e)
        {
            GridEditableItem item = e.Item as GridEditableItem;

            Data.Employee newUsr = new Data.Employee();

            newUsr.office_id = Convert.ToInt32((item.FindControl("rddlOffice2") as RadDropDownList).SelectedValue);
            newUsr.user_id = (item.FindControl("rtbUserID3") as RadTextBox).Text;
            newUsr.first_nm = (item.FindControl("rtbFirstNm2") as RadTextBox).Text;
            newUsr.last_nm = (item.FindControl("rtbLastNm2") as RadTextBox).Text;
            newUsr.administrator_va = (item.FindControl("rddlAdminLevel2") as RadDropDownList).SelectedValue.ToString();
            newUsr.pass_access = (item.FindControl("rddlPASSLevel2") as RadDropDownList).SelectedValue.ToString();
            newUsr.approver_va = Convert.ToBoolean((item.FindControl("rrblSafetyApprover2") as RadRadioButtonList).SelectedValue);
            newUsr.active = Convert.ToBoolean((item.FindControl("rrblStatus2") as RadRadioButtonList).SelectedValue);
            newUsr.show_reports = Convert.ToBoolean((item.FindControl("rcbReports2") as RadCheckBox).Checked);

            db.Employees.InsertOnSubmit(newUsr);
            db.SubmitChanges();

            DisplayMessage(false, "The user was added!");
        }
        #endregion

    }
}