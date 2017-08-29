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
    public partial class Offices : System.Web.UI.Page
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
            if (!string.IsNullOrEmpty(office_id)) OfficeID = Convert.ToInt32(office_id); else if (OfficeID == 0) Response.Redirect(Config.SIMSURL + "SIMSWSCHome.aspx");

            WSCID = Convert.ToInt32(db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id);
            currWSC = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID);

            ph1.Title = "Manage Office Information";
            ph1.SubTitle = currWSC.wsc_nm + " Water Science Center";
            ph1.ShowOfficeInfoPanel = true;

            if (!Page.IsPostBack)
            {
                //If the user belongs to this site's WSC (or has an exception to work in the WSC), or is a SuperUser, then allow them to edit the page
                if (user.WSCID.Contains(WSCID) && user.IsAdmin || user.IsSuperUser) HasEditAccess = true;

                if (HasEditAccess)
                {
                    pnlHasAccess.Visible = true;
                    pnlNotice.Visible = false;
                    pnlNoAccess.Visible = false;
                }
                else
                {
                    pnlHasAccess.Visible = false;
                    pnlNoAccess.Visible = true;
                }
            }
        }

        protected void ShowNotice()
        {
            pnlNotice.Visible = true;
            ltlNoticeHeading.Text = "Success!";
            ltlNotice.Text = "The office details have been modified.";
        }

        #region rgOffices Grid
        protected void rgOffices_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            rgOffices.DataSource = db.Offices.Where(p => p.wsc_id == WSCID).Select(p => new { 
                office_cd = p.office_cd, 
                office_nm = p.office_nm, 
                office_id = p.office_id }).OrderBy(p => p.office_nm).ToList();
        }

        //Specify which items appear in FilterMenu
        protected void rgOffices_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgOffices.FilterMenu;
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

        protected void rgOffices_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.IsInEditMode)
            {
                GridEditableItem item = (GridEditableItem)e.Item;

                int office_id = Convert.ToInt32(item.GetDataKeyValue("office_id"));
                var office = db.Offices.FirstOrDefault(p => p.office_id == office_id);

                Literal ltlOfficeCd = (Literal)item.FindControl("ltlOfficeCd");
                RadTextBox rtbOfficeName = (RadTextBox)item.FindControl("rtbOfficeName");
                RadTextBox rtbStreetAddress = (RadTextBox)item.FindControl("rtbStreetAddress");
                RadTextBox rtbCityStateZip = (RadTextBox)item.FindControl("rtbCityStateZip");
                RadNumericTextBox rntbLat = (RadNumericTextBox)item.FindControl("rntbLat");
                RadNumericTextBox rntbLong = (RadNumericTextBox)item.FindControl("rntbLong");
                RadTextBox rtbPhoneNo = (RadTextBox)item.FindControl("rtbPhoneNo");
                RadTextBox rtbDataChiefEmail = (RadTextBox)item.FindControl("rtbDataChiefEmail");
                RadTextBox rtbReviewerEmail = (RadTextBox)item.FindControl("rtbReviewerEmail");
                Literal ltlIPAddress = (Literal)item.FindControl("ltlIPAddress");
                RadDropDownList rddlTimeZone = (RadDropDownList)item.FindControl("rddlTimeZone");
                RadRadioButtonList rrblNewRecord = (RadRadioButtonList)item.FindControl("rrblNewRecord");

                ltlOfficeCd.Text = office.office_cd;
                rtbOfficeName.Text = office.office_nm;
                rtbStreetAddress.Text = office.street_addrs;
                rtbCityStateZip.Text = office.city_st_zip;
                rntbLat.Value = office.dec_lat_va;
                rntbLong.Value = office.dec_long_va;
                rtbPhoneNo.Text = office.ph_no;
                rtbDataChiefEmail.Text = office.data_chief_email;
                rtbReviewerEmail.Text = office.reviewer_email;
                ltlIPAddress.Text = office.office_subnet;
                rddlTimeZone.SelectedValue = office.tz_cd;
                if ((bool)office.start_new_rec)
                    rrblNewRecord.SelectedValue = "Yes";
                else
                    rrblNewRecord.SelectedValue = "No";

            }
        }

        protected void rgOffices_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            GridEditableItem item = e.Item as GridEditableItem;
            Literal ltlError = (Literal)item.FindControl("ltlError");

            if (Page.IsValid)
            {
                int office_id = Convert.ToInt32(item.GetDataKeyValue("office_id"));
                var office = db.Offices.FirstOrDefault(p => p.office_id == office_id);

                string office_nm = (item.FindControl("rtbOfficeName") as RadTextBox).Text;
                string street_address = (item.FindControl("rtbStreetAddress") as RadTextBox).Text;
                string citystatezip = (item.FindControl("rtbCityStateZip") as RadTextBox).Text;
                double latitude = (double)(item.FindControl("rntbLat") as RadNumericTextBox).Value;
                double longitude = (double)(item.FindControl("rntbLong") as RadNumericTextBox).Value;
                string phone_no = (item.FindControl("rtbPhoneNo") as RadTextBox).Text;
                string data_chief_email = (item.FindControl("rtbDataChiefEmail") as RadTextBox).Text;
                string reviewer_email = (item.FindControl("rtbReviewerEmail") as RadTextBox).Text;
                string timezone = (item.FindControl("rddlTimeZone") as RadDropDownList).SelectedValue.ToString();
                string new_rec = (item.FindControl("rrblNewRecord") as RadRadioButtonList).SelectedValue.ToString();
                
                office.office_nm = office_nm;
                office.street_addrs = street_address;
                office.city_st_zip = citystatezip;
                office.dec_lat_va = latitude;
                office.dec_long_va = longitude;
                office.ph_no = phone_no;
                office.data_chief_email = data_chief_email;
                office.reviewer_email = reviewer_email;
                office.tz_cd = timezone;
                if (new_rec == "Yes") office.start_new_rec = true; else office.start_new_rec = false;

                db.SubmitChanges();

                ShowNotice();
            }
            else
            {
                ltlError.Text = "<span style='color:red;font-weight:bold;'>You must populate the required fields (those marked with an *).</span>";
            }
        }
        #endregion
    }
}