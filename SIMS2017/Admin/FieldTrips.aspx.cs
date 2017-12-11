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
    public partial class FieldTrips : System.Web.UI.Page
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

            ph1.Title = "Manage Field Trips";
            ph1.SubTitle = currWSC.wsc_nm + " Water Science Center";
            ph1.ShowOfficeInfoPanel = true;

            if (!Page.IsPostBack)
            {
                //If the user belongs to this site's WSC (or has an exception to work in the WSC), or is a SuperUser, then allow them to edit the page
                if (user.WSCID.Contains(WSCID) && user.IsAdmin || user.IsSuperUser) HasEditAccess = true;

                if (!HasEditAccess)
                    Session["CanEdit"] = "false";
                else
                    Session["CanEdit"] = "true";
            }
        }

        #region Internal Classes
        internal class EmployeeItem
        {
            private string _user_nm;
            private string _user_id;

            public string user_nm
            {
                get { return _user_nm; }
                set { _user_nm = value; }
            }
            public string user_id
            {
                get { return _user_id; }
                set { _user_id = value; }
            }

            public EmployeeItem()
            {
                _user_nm = user_nm;
                _user_id = user_id;
            }
        }
        internal class SiteItem
        {
            private int? _site_id;
            private string _site_no_nm;
            private int? _office_id;

            public int? site_id
            {
                get { return _site_id; }
                set { _site_id = value; }
            }
            public int? office_id
            {
                get { return _office_id; }
                set { _office_id = value; }
            }
            public string site_no_nm
            {
                get { return _site_no_nm; }
                set { _site_no_nm = value; }
            }

            public SiteItem()
            {
                _site_no_nm = site_no_nm;
                _site_id = site_id;
                _office_id = office_id;
            }
        }
        #endregion

        #region Properties
        private List<EmployeeItem> Employees
        {
            get
            {
                List<EmployeeItem> _employees = new List<EmployeeItem>();
                _employees = db.SP_Personnel_by_WSC_office_or_user_id(WSCID, 0, "", "no", "True", "no")
                    .Select(p => new EmployeeItem { user_nm = p.first_nm + " " + p.last_nm, user_id = p.user_id })
                    .OrderBy(p => p.user_id).ToList();

                return _employees;
            }
        }
        private List<SiteItem> Sites
        {
            get
            {
                List<SiteItem> _sites = new List<SiteItem>();
                _sites = db.Sites
                    .Where(p => p.Office.wsc_id == WSCID)
                    .Select(p => new SiteItem { site_id = p.site_id, site_no_nm = p.site_no.Trim() + " " + p.station_full_nm, office_id = p.office_id })
                    .OrderBy(p => p.site_no_nm).ToList();
                return _sites;
            }
        }
        #endregion

        private void DisplayMessage(bool isError, string text)
        {
            Label label = (isError) ? this.lblError : this.lblSuccess;
            label.Text = text;
        }

        //NO LONGER BEING USED - So that all sites, regardless of office, are available to be assigned to a field trip
        protected void rddlOffice_SelectedIndexChanged(object source, EventArgs e)
        {
            RadDropDownList rddl = (RadDropDownList)source;
            GridEditFormInsertItem item = (GridEditFormInsertItem)rddl.NamingContainer;
            RadListBox rlb = (RadListBox)item.FindControl("rlbSitesStart2");

            int office_id = Convert.ToInt32(rddl.SelectedValue);
            rlb.DataSource = Sites.Where(p => p.office_id == office_id);
            rlb.DataBind();
        }

        protected void ram_AjaxRequest(object source, AjaxRequestEventArgs e)
        {
            rgFieldTrips.Rebind();
        }

        #region Field Trip RadGrid
        protected void rgFieldTrips_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            rgFieldTrips.DataSource = db.Trips.Where(p => p.Office.wsc_id == WSCID).Select(p => new
            {
                office_nm = p.Office.office_nm,
                trip_nm = p.trip_nm,
                user_id = p.user_id,
                trip_id = p.trip_id
            }).OrderBy(p => p.office_nm).ThenBy(p => p.trip_nm).ToList();
        }

        protected void rgFieldTrips_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgFieldTrips.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" | menu.Items[i].Text == "Contains" | menu.Items[i].Text == "EqualTo" | menu.Items[i].Text == "DoesNotContain")
                    i = i + 1;
                else
                    menu.Items.RemoveAt(i);
            }
        }

        protected void rgFieldTrips_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.IsInEditMode)
            {
                GridEditableItem item = (GridEditableItem)e.Item;

                Panel pnlUpdate = (Panel)item.FindControl("pnlUpdate");
                Panel pnlInsert = (Panel)item.FindControl("pnlInsert");
                RadTextBox rtbTripName = (RadTextBox)item.FindControl("rtbTripName");
                RadDropDownList rddlAssignedTo = (RadDropDownList)item.FindControl("rddlAssignedTo");
                RadListBox rlbSitesStart = (RadListBox)item.FindControl("rlbSitesStart");
                RadListBox rlbSitesEnd = (RadListBox)item.FindControl("rlbSitesEnd");
                RadTextBox rtbTripName2 = (RadTextBox)item.FindControl("rtbTripName2");
                RadDropDownList rddlOffice = (RadDropDownList)item.FindControl("rddlOffice");
                RadDropDownList rddlAssignedTo2 = (RadDropDownList)item.FindControl("rddlAssignedTo2");
                RadListBox rlbSitesStart2 = (RadListBox)item.FindControl("rlbSitesStart2");
                RadListBox rlbSitesEnd2 = (RadListBox)item.FindControl("rlbSitesEnd2");

                if (!(e.Item is IGridInsertItem))
                {
                    int trip_id = Convert.ToInt32(item.GetDataKeyValue("trip_id"));
                    var currTrip = db.Trips.FirstOrDefault(p => p.trip_id == trip_id);

                    rtbTripName.Text = currTrip.trip_nm;
                    rddlAssignedTo.DataSource = Employees;
                    rddlAssignedTo.DataBind();
                    rddlAssignedTo.SelectedValue = currTrip.user_id;
                    rlbSitesStart.DataSource = Sites.Where(p => !currTrip.TripSites.Select(a => a.site_id).Contains(p.site_id));
                    rlbSitesStart.DataBind();
                    rlbSitesEnd.DataSource = currTrip.TripSites.Select(p => new { site_id = p.site_id, site_no_nm = p.Site.site_no + " " + p.Site.station_full_nm }).OrderBy(p => p.site_no_nm);
                    rlbSitesEnd.DataBind();

                    pnlUpdate.Visible = true;
                    pnlInsert.Visible = false;
                }
                else if ((e.Item is IGridInsertItem))
                {
                    rddlAssignedTo2.DataSource = Employees;
                    rddlAssignedTo2.DataBind();
                    rddlOffice.DataSource = db.Offices.Where(p => p.wsc_id == WSCID).Select(p => new { office_id = p.office_id, office_nm = p.office_nm }).ToList();
                    rddlOffice.DataBind();
                    rlbSitesStart2.DataSource = Sites;
                    rlbSitesStart2.DataBind();

                    pnlUpdate.Visible = false;
                    pnlInsert.Visible = true;
                }
            }

            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                int trip_id = Convert.ToInt32(item.GetDataKeyValue("trip_id"));
                LinkButton lbDelete = (LinkButton)item.FindControl("lbDelete");
                LinkButton lbViewSites = (LinkButton)item.FindControl("lbViewSites");

                lbViewSites.OnClientClick = String.Format("openWin('{0}'); return false;", trip_id);
                lbDelete.OnClientClick = String.Format("ShowDeleteForm('{0}'); return false;", trip_id);

                if (Session["CanEdit"].ToString() == "false")
                {
                    ImageButton ib = (ImageButton)item["EditCommandColumn"].Controls[0];
                    ib.Visible = false;
                    LiteralControl lb = (LiteralControl)item["TemplateDeleteColumn"].Controls[0];
                    lb.Visible = false;
                }
            }

        }

        protected void rgFieldTrips_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            GridEditableItem item = e.Item as GridEditableItem;

            int trip_id = Convert.ToInt32(item.GetDataKeyValue("trip_id"));
            var currTrip = db.Trips.FirstOrDefault(p => p.trip_id == trip_id);

            string trip_nm = (item.FindControl("rtbTripName") as RadTextBox).Text;
            string assigned_to = (item.FindControl("rddlAssignedTo") as RadDropDownList).SelectedValue.ToString();
            RadListBox assigned_sites = (item.FindControl("rlbSitesEnd") as RadListBox);

            currTrip.trip_nm = trip_nm;
            currTrip.user_id = assigned_to;
            db.TripSites.DeleteAllOnSubmit(currTrip.TripSites);
            db.SubmitChanges();

            List<Data.TripSite> tripSites = new List<Data.TripSite>();
            foreach (RadListBoxItem site in assigned_sites.Items)
                tripSites.Add(new Data.TripSite() { trip_id = currTrip.trip_id, site_id = Convert.ToInt32(site.Value) });
            db.TripSites.InsertAllOnSubmit(tripSites);
            db.SubmitChanges();

            DisplayMessage(false, "The field trip's info was updated!");
        }

        protected void rgFieldTrips_InsertCommand(object sender, GridCommandEventArgs e)
        {
            GridEditableItem item = e.Item as GridEditableItem;

            Data.Trip newTrip = new Data.Trip();

            newTrip.office_id = Convert.ToInt32((item.FindControl("rddlOffice") as RadDropDownList).SelectedValue);
            newTrip.user_id = (item.FindControl("rddlAssignedTo2") as RadDropDownList).SelectedValue.ToString();
            newTrip.trip_nm = (item.FindControl("rtbTripName2") as RadTextBox).Text;
            RadListBox assigned_sites = (item.FindControl("rlbSitesEnd2") as RadListBox);
            
            db.Trips.InsertOnSubmit(newTrip);
            db.SubmitChanges();

            List<Data.TripSite> tripSites = new List<Data.TripSite>();
            foreach (RadListBoxItem site in assigned_sites.Items)
                tripSites.Add(new Data.TripSite() { trip_id = newTrip.trip_id, site_id = Convert.ToInt32(site.Value) });
            db.TripSites.InsertAllOnSubmit(tripSites);
            db.SubmitChanges();

            DisplayMessage(false, "The field trip was added!");
        }
        #endregion
    }
}