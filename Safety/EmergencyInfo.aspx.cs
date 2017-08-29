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

namespace Safety
{
    public partial class EmergencyInfo : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
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
            string office_id = Request.QueryString["office_id"];

            if (!string.IsNullOrEmpty(office_id))
                OfficeID = Convert.ToInt32(office_id);
            else
                OfficeID = user.OfficeID;

            //Using the passed office_id, reset the wsc to match that of the current office
            WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            var wsc = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID);
            ph1.Title = "Manage Emergency Information";
            ph1.SubTitle = "For the " + wsc.wsc_nm + " WSC";

            if (!Page.IsPostBack)
            {
                //--APPROVER ACCESS SECTION-------------------------------------------------------------
                if (user.IsSuperUser || user.WSCID.Contains(WSCID) && user.IsSafetyApprover)
                {
                    pnlHasAccess.Visible = true;
                    pnlNoAccess.Visible = false;
                }
                else
                {
                    pnlHasAccess.Visible = false;
                    pnlNoAccess.Visible = true;
                }
                //--END PAGE ACCESS SECTION---------------------------------------------------------
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

        #region Hospitals
        protected void rgHospitals_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            rgHospitals.DataSource = db.Hospitals.Where(p => p.wsc_id == WSCID);
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
                HyperLink deleteLink = (HyperLink)e.Item.FindControl("hlDelete");
                deleteLink.Attributes["href"] = "#";
                deleteLink.Attributes["onclick"] = String.Format("return ShowDeleteHospitalForm('{0}','{1}');", e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["hospital_id"], e.Item.ItemIndex);
            }
        }

        protected void rgHospitals_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName == RadGrid.InitInsertCommandName)
            {
                //"Add new" button clicked
                GridEditCommandColumn editColumn = (GridEditCommandColumn)rgHospitals.MasterTableView.GetColumn("EditCommandColumn");
                editColumn.Visible = false;
            }
            else if (e.CommandName == RadGrid.RebindGridCommandName && e.Item.OwnerTableView.IsItemInserted)
            {
                e.Canceled = true;
            }
            else
            {
                GridEditCommandColumn editColumn = (GridEditCommandColumn)rgHospitals.MasterTableView.GetColumn("EditCommandColumn");
                if (!editColumn.Visible)
                {
                    editColumn.Visible = true;
                }
            }
        }

        protected void rgHospitals_InsertCommand(object sender, GridCommandEventArgs e)
        {
            GridEditableItem item = e.Item as GridEditableItem;
            Data.Hospital h = new Data.Hospital();

            try
            {
                h.hospital_nm = (item.FindControl("tbHospitalNm") as TextBox).Text;
                h.street_addrs = (item.FindControl("tbStreetAddrs1") as TextBox).Text;
                h.city = (item.FindControl("tbCity1") as TextBox).Text;
                h.state = (item.FindControl("tbState1") as TextBox).Text;
                h.zip = (item.FindControl("tbZip1") as TextBox).Text;
                h.ph_no = (item.FindControl("tbPhoneNo1") as TextBox).Text;
                h.dec_lat_va = Convert.ToDouble((item.FindControl("tbLat") as TextBox).Text);
                h.dec_long_va = Convert.ToDouble((item.FindControl("tbLong") as TextBox).Text);
                h.wsc_id = WSCID;
            }
            catch (Exception ex)
            {
                DisplayHospitalMessage(true, "You must complete all form fields!");
                e.Canceled = true;
                return;
            }

            try
            {
                db.Hospitals.InsertOnSubmit(h);
                db.SubmitChanges();

                DisplayHospitalMessage(false, "The hospital was added!");
            }
            catch (Exception ex)
            {
                DisplayHospitalMessage(true, "Unable to add hospital. Reason: " + ex.Message);
                e.Canceled = true;
            }
        }

        protected void rgHospitals_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            GridEditableItem item = e.Item as GridEditableItem;
            int hospital_id = Convert.ToInt32(item.GetDataKeyValue("hospital_id"));
            var h = db.Hospitals.FirstOrDefault(p => p.hospital_id == hospital_id);

            try
            {
                h.hospital_nm = (item.FindControl("tbHospitalNm") as TextBox).Text;
                h.street_addrs = (item.FindControl("tbStreetAddrs1") as TextBox).Text;
                h.city = (item.FindControl("tbCity1") as TextBox).Text;
                h.state = (item.FindControl("tbState1") as TextBox).Text;
                h.zip = (item.FindControl("tbZip1") as TextBox).Text;
                h.ph_no = (item.FindControl("tbPhoneNo1") as TextBox).Text;
                h.dec_lat_va = Convert.ToDouble((item.FindControl("tbLat") as TextBox).Text);
                h.dec_long_va = Convert.ToDouble((item.FindControl("tbLong") as TextBox).Text);
            }
            catch (Exception ex)
            {
                DisplayHospitalMessage(true, "You must complete all form fields!");
                e.Canceled = true;
                return;
            }

            try
            {
                db.SubmitChanges();
                DisplayHospitalMessage(false, "The hospital was updated!");
            }
            catch (Exception ex)
            {
                DisplayHospitalMessage(true, "Unable to update hospital. Reason: " + ex.Message);

                e.Canceled = true;
            }

        }

        private void DisplayHospitalMessage(bool isError, string text)
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

        #region "Contacts"
        protected void rgContacts_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            rgContacts.DataSource = db.Contacts.Where(p => p.wsc_id == WSCID);
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
                HyperLink deleteLink = (HyperLink)e.Item.FindControl("hlDelete");
                deleteLink.Attributes["href"] = "#";
                deleteLink.Attributes["onclick"] = String.Format("return ShowDeleteContactForm('{0}','{1}');", e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["contact_id"], e.Item.ItemIndex);
            }
        }

        protected void rgContacts_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName == RadGrid.InitInsertCommandName)
            {
                //"Add new" button clicked
                GridEditCommandColumn editColumn = (GridEditCommandColumn)rgContacts.MasterTableView.GetColumn("EditCommandColumn");
                editColumn.Visible = false;
            }
            else if (e.CommandName == RadGrid.RebindGridCommandName && e.Item.OwnerTableView.IsItemInserted)
            {
                e.Canceled = true;
            }
            else
            {
                GridEditCommandColumn editColumn = (GridEditCommandColumn)rgContacts.MasterTableView.GetColumn("EditCommandColumn");
                if (!editColumn.Visible)
                {
                    editColumn.Visible = true;
                }
            }
        }

        protected void rgContacts_InsertCommand(object sender, GridCommandEventArgs e)
        {
            GridEditableItem item = e.Item as GridEditableItem;
            Data.Contact c = new Data.Contact();

            try
            {
                c.contact_nm = (item.FindControl("tbContactNm") as TextBox).Text;
                c.street_addrs = (item.FindControl("tbStreetAddrs2") as TextBox).Text;
                c.city = (item.FindControl("tbCity2") as TextBox).Text;
                c.state = (item.FindControl("tbState2") as TextBox).Text;
                c.zip = (item.FindControl("tbZip2") as TextBox).Text;
                c.ph_no = (item.FindControl("tbPhoneNo2") as TextBox).Text;
                c.wsc_id = WSCID;
            }
            catch (Exception ex)
            {
                DisplayHospitalMessage(true, "You must complete all required form fields!");
                e.Canceled = true;
                return;
            }

            try
            {
                db.Contacts.InsertOnSubmit(c);
                db.SubmitChanges();

                DisplayContactMessage(false, "The emergency contact was added!");
            }
            catch (Exception ex)
            {
                DisplayContactMessage(true, "Unable to add contact. Reason: " + ex.Message);
                e.Canceled = true;
            }
        }

        protected void rgContacts_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            GridEditableItem item = e.Item as GridEditableItem;
            int contact_id = Convert.ToInt32(item.GetDataKeyValue("contact_id"));
            var c = db.Contacts.FirstOrDefault(p => p.contact_id == contact_id);

            try
            {
                c.contact_nm = (item.FindControl("tbContactNm") as TextBox).Text;
                c.street_addrs = (item.FindControl("tbStreetAddrs2") as TextBox).Text;
                c.city = (item.FindControl("tbCity2") as TextBox).Text;
                c.state = (item.FindControl("tbState2") as TextBox).Text;
                c.zip = (item.FindControl("tbZip2") as TextBox).Text;
                c.ph_no = (item.FindControl("tbPhoneNo2") as TextBox).Text;
            }
            catch (Exception ex)
            {
                DisplayContactMessage(true, "You must complete all required form fields!");
                e.Canceled = true;
                return;
            }

            try
            {
                db.SubmitChanges();

                DisplayContactMessage(false, "The contact was updated!");
            }
            catch (Exception ex)
            {
                DisplayContactMessage(true, "Unable to update contact. Reason: " + ex.Message);

                e.Canceled = true;
            }

        }

        private void DisplayContactMessage(bool isError, string text)
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
    }
}