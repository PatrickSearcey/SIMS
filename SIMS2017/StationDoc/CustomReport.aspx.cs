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
    public partial class CustomReport : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private string reportType;
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
            //Clear out the session state variables before using this page
            Session.Clear();

            //If no office_id was passed, then redirect back to the homepage
            string office_id = Request.QueryString["office_id"];
            if (!string.IsNullOrEmpty(office_id)) OfficeID = Convert.ToInt32(office_id); else Response.Redirect(Config.SIMS2017URL + "SIMSWSCHome.aspx");

            //If no report type was passed, then default to office
            reportType = Request.QueryString["type"];
            if (string.IsNullOrEmpty(reportType)) reportType = "office";

            //Using the passed office_id, reset the wsc to match that of the current office
            WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;
            var wsc = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID);

            ph1.Title = "Generate a Custom Report";
            ph1.SubTitle = wsc.wsc_nm + " WSC";
            ph1.ShowOfficeInfoPanel = true;

            if (!Page.IsPostBack)
            {
                rtsMain.Visible = true;
                rmp.Visible = true;

                PopulateReportOptions();
                PopulateElementList();
            }
        }

        #region Initial Setup
        protected void PopulateReportOptions()
        {
            rrblOffice.DataSource = db.Offices.Where(p => p.wsc_id == WSCID);
            rrblOffice.DataBind();

            if (reportType == "office")
            {
                rtsMain.SelectedIndex = 1;
                rmp.SelectedIndex = 1;

                rrblOffice.SelectedValue = OfficeID.ToString();
            }
            else
            {
                rtsMain.SelectedIndex = 0;
                rmp.SelectedIndex = 0;

                rrblWaterBodyType.SelectedValue = reportType;
            }
        }

        protected void PopulateElementList()
        {
            var elems = db.ElementDetails.Where(p => p.active == true).ToList();

            if (!string.IsNullOrEmpty(rrblWaterBodyType.SelectedValue))
            {
                switch (rrblWaterBodyType.SelectedValue)
                {
                    case "5": //Spring
                        elems = elems.Where(p => p.spring == true).ToList();
                        break;
                    case "3": //Estuary
                        elems = elems.Where(p => p.estuary == true).ToList();
                        break;
                    case "2": //Lake
                        elems = elems.Where(p => p.lake == true).ToList();
                        break;
                    case "1": //Stream
                        elems = elems.Where(p => p.stream == true).ToList();
                        break;
                    case "6": //Groundwater
                        elems = elems.Where(p => p.groundwater == true).ToList();
                        break;
                }
            }

            rcblSDESCElements.DataSource = elems.Where(p => p.priority < 200).Select(p => new { element_id = p.element_id, element_nm = p.element_nm, priority = p.priority }).OrderBy(p => p.priority).ToList();
            rcblSDESCElements.DataBind();

            rcblSANALElements.DataSource = elems.Where(p => p.priority < 300 && p.priority > 199).Select(p => new { element_id = p.element_id, element_nm = p.element_nm, priority = p.priority }).OrderBy(p => p.priority).ToList();
            rcblSANALElements.DataBind();

            rcblMANUElements.DataSource = elems.Where(p => p.priority > 299).Select(p => new { element_id = p.element_id, element_nm = p.element_nm, priority = p.priority }).OrderBy(p => p.priority).ToList();
            rcblMANUElements.DataBind();

            pnlElements.Visible = true;
            pnlCustomReport.Visible = false;
        }
        #endregion
        
        #region Control Binding and Events
        protected void UpdateElementList(object sender, EventArgs e)
        {
            RadRadioButtonList rrbl = (RadRadioButtonList)sender;
            if (rrbl.ID == "rrblWaterBodyType" && rtsMain.SelectedIndex == 0)
            {
                if (!string.IsNullOrEmpty(rrblOffice.SelectedValue))
                {
                    foreach (ButtonListItem item in rrblOffice.Items)
                        item.Selected = false;
                }
            }
            else if (rrbl.ID == "rrblOffice" && rtsMain.SelectedIndex == 1)
            {
                if (!string.IsNullOrEmpty(rrblWaterBodyType.SelectedValue))
                {
                    foreach (ButtonListItem item in rrblWaterBodyType.Items)
                        item.Selected = false;
                }
            }

            PopulateElementList();
        }

        protected void lvCustom_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                //Now get the list of element IDs to grab from the selected choices
                List<int> element_ids = new List<int>();
                foreach (ButtonListItem item in rcblSDESCElements.Items)
                    if (item.Selected) element_ids.Add(Convert.ToInt32(item.Value));
                foreach (ButtonListItem item in rcblSANALElements.Items)
                    if (item.Selected) element_ids.Add(Convert.ToInt32(item.Value));
                foreach (ButtonListItem item in rcblMANUElements.Items)
                    if (item.Selected) element_ids.Add(Convert.ToInt32(item.Value));

                SiteItem site = (SiteItem)e.Item.DataItem;
                DataList innerDL = (DataList)e.Item.FindControl("dlCustomElements");

                List<ElementItem> elements = new List<ElementItem>();
                foreach (int id in element_ids)
                {
                    var elem = db.SiteElements.Where(p => p.element_id == id && p.site_id == Convert.ToInt32(site.SiteID)).Select(p => new ElementItem
                    {
                        ElementID = p.element_id.ToString(),
                        SiteID = p.site_id.ToString(),
                        ElementName = p.ElementDetail.element_nm,
                        ElementInfo = p.element_info.FormatElementInfo(id, Convert.ToInt32(site.SiteID)),
                        RevisedBy = p.revised_by,
                        RevisedDate = p.revised_dt.ToString()
                    }).FirstOrDefault();

                    if (elem != null) elements.Add(elem);
                }

                if (elements.Count == 0)
                {
                    e.Item.Visible = false;
                }
                else
                {
                    e.Item.Visible = true;
                    innerDL.DataSource = elements;
                    innerDL.DataBind();
                }
            }
        }
        #endregion
        
        #region Button Events
        protected void rbCustom_Command(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(rtbReportTitle.Text)) ltlReportTitle.Text = "Custom Report"; else ltlReportTitle.Text = rtbReportTitle.Text;
            var sites = new List<SiteItem>();

            //Get the list of sites for which the report will be generated
            if (!string.IsNullOrEmpty(rrblOffice.SelectedValue))
            {
                sites = db.Sites.Where(p => p.office_id == Convert.ToInt32(rrblOffice.SelectedValue)).Select(p => new SiteItem
                {
                    SiteID = p.site_id.ToString(),
                    SiteNo = p.site_no,
                    StationName = p.station_full_nm,
                    AgencyCd = p.agency_cd
                }).ToList();
            }
            else if (!string.IsNullOrEmpty(rrblWaterBodyType.SelectedValue))
            {
                sites = (from p in db.Sites
                         join s in db.vSITEFILEs on p.nwisweb_site_id equals s.site_id
                         join o in db.Offices on p.office_id equals o.office_id
                         join t in db.SiteTypes on s.site_tp_cd equals t.site_tp_cd
                         where t.sims_site_tp_id == Convert.ToInt32(rrblWaterBodyType.SelectedValue) && o.wsc_id == WSCID
                         select new SiteItem
                         {
                             SiteID = p.site_id.ToString(),
                             SiteNo = p.site_no,
                             StationName = s.station_nm,
                             AgencyCd = s.agency_cd
                         }).ToList();
            }

            lvCustom.DataSource = sites;
            lvCustom.DataBind();

            pnlElements.Visible = false;
            pnlCustomReport.Visible = true;
            rtsMain.Visible = false;
            rmp.Visible = false;
        }

        protected void rbBack_Command(object sender, EventArgs e)
        {
            rtsMain.Visible = true;
            rmp.Visible = true;
            PopulateElementList();
        }
        #endregion

        #region Internal Classes
        internal class ElementItem
        {
            private string _ElementID;
            private string _SiteID;
            private string _ElementName;
            private string _ElementInfo;
            private string _RevisedBy;
            private string _RevisedDate;

            public string ElementID
            {
                get { return _ElementID; }
                set { _ElementID = value; }
            }
            public string SiteID
            {
                get { return _SiteID; }
                set { _SiteID = value; }
            }
            public string ElementName
            {
                get { return _ElementName; }
                set { _ElementName = value; }
            }
            public string ElementInfo
            {
                get { return _ElementInfo; }
                set { _ElementInfo = value; }
            }
            public string RevisedBy
            {
                get { return _RevisedBy; }
                set { _RevisedBy = value; }
            }
            public string RevisedDate
            {
                get { return _RevisedDate; }
                set { _RevisedDate = value; }
            }
            public ElementItem()
            {
                _ElementID = ElementID;
                _SiteID = SiteID;
                _ElementName = ElementName;
                _ElementInfo = ElementInfo;
                _RevisedBy = RevisedBy;
                _RevisedDate = RevisedDate;
            }
        }

        internal class SiteItem
        {
            private string _SiteID;
            private string _SiteNo;
            private string _StationName;
            private string _AgencyCd;

            public string SiteID
            {
                get { return _SiteID;  }
                set { _SiteID = value; }
            }
            public string SiteNo
            {
                get { return _SiteNo; }
                set { _SiteNo = value; }
            }
            public string StationName
            {
                get { return _StationName; }
                set { _StationName = value; }
            }
            public string AgencyCd
            {
                get { return _AgencyCd; }
                set { _AgencyCd = value; }
            }
            public SiteItem()
            {
                _SiteID = SiteID;
                _SiteNo = SiteNo;
                _StationName = StationName;
                _AgencyCd = AgencyCd;
            }
        }
        #endregion
    }
}