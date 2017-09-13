using Core;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SIMS2017.Control
{
    public partial class OfficeSelector : System.Web.UI.UserControl
    {
        #region Local Variables
        private SIMSDataContext db = new SIMSDataContext();
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
        private int TripID
        {
            get
            {
                if (Session["TripID"] == null) return 0; else return (int)Session["TripID"];
            }
            set
            {
                Session["TripID"] = value;
            }
        }
        private WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        #endregion

        #region Public Properties
        public Boolean HideSiteFields { get; set; }
        public String SelSiteNo { get; set; }
        public Office SelOffice { get; set; }
        public Trip SelTrip { get; set; }
        public event EventHandler SelectorChanged;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                SetupDropDownLists();
                SetupResponsibleOfficeInfo();

                if (HideSiteFields)
                {
                    ltlSiteNo.Visible = false;
                    tbSiteNo.Visible = false;
                    tbAgencyCd.Visible = false;
                    btnSiteNo.Visible = false;
                }
            }
        }

        private void OnSelectorChanged()
        {
            if (SelectorChanged != null)
            {
                SelectorChanged(this, EventArgs.Empty);
            }
        }

        protected void SetupDropDownLists()
        {
            rddlOffice.DataSource = db.Offices.Where(p => p.wsc_id == WSCID).ToList();
            rddlOffice.DataBind();
            rddlOffice.SelectedValue = OfficeID.ToString();
            SelOffice = db.Offices.Where(p => p.office_id == Convert.ToInt32(rddlOffice.SelectedValue)).FirstOrDefault();

            rddlFieldTrip.DataSource = db.Trips.Where(p => p.office_id == OfficeID).Select(p => new { trip_id = p.trip_id, TripName = p.trip_nm + " (" + p.user_id + ")" }).OrderBy(p => p.TripName).ToList();
            rddlFieldTrip.DataBind();
            rddlFieldTrip.Items.Add(new DropDownListItem { Value = "", Text = "" });
            if (TripID > 0) rddlFieldTrip.SelectedValue = TripID.ToString(); else rddlFieldTrip.SelectedValue = "";
            if (TripID > 0) SelTrip = db.Trips.Where(p => p.trip_id == Convert.ToInt32(rddlFieldTrip.SelectedValue)).FirstOrDefault();
        }

        protected void SetupResponsibleOfficeInfo()
        {
            ltlOfficeInfo.Text = String.Format("<a href='{0}SIMSWSCHome.aspx?wsc_id={1}&office_id={2}'>{3}</a><br />{4}<br />{5}<br />{6}", Config.SIMSURL, WSCID, SelOffice.office_id, SelOffice.office_nm, SelOffice.street_addrs, SelOffice.city_st_zip, SelOffice.ph_no);
        }

        protected void Filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            RadDropDownList ddl = (RadDropDownList)sender;
            switch (ddl.ID)
            {
                case "rddlOffice":
                    OfficeID = Convert.ToInt32(rddlOffice.SelectedValue);
                    TripID = 0;
                    break;
                case "rddlFieldTrip":
                    OfficeID = Convert.ToInt32(rddlOffice.SelectedValue);
                    if (!string.IsNullOrEmpty(rddlFieldTrip.SelectedValue)) TripID = Convert.ToInt32(rddlFieldTrip.SelectedValue); else TripID = 0;
                    break;
            }
            SetupDropDownLists();
            SetupResponsibleOfficeInfo();
            OnSelectorChanged();
        }

        protected void btnSiteNo_Command(object sender, CommandEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbSiteNo.Text))
            {
                var site = db.Sites.FirstOrDefault(p => p.site_no == tbSiteNo.Text && p.agency_cd == tbAgencyCd.Text);
                if (site != null)
                {
                    Response.Redirect(String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMSURL, site.site_id));
                }
            }
        }
    }
}