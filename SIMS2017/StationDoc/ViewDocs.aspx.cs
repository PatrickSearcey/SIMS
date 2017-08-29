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
    public partial class ViewDocs : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        private SIMSService.SIMSServiceClient svcSIMS = new SIMSService.SIMSServiceClient();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        public Boolean HasEditAccess { get; set; }
        private Data.Site currSite;
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
            string site_id = "6001334";// Request.QueryString["site_id"];
            if (!string.IsNullOrEmpty(site_id)) SiteID = Convert.ToInt32(site_id); else Response.Redirect(Config.SIMSURL + "SIMSWSCHome.aspx");

            reportType = Request.QueryString["type"];

            //Using the passed site_id, setup the site data element, and reset the office and wsc to match that of the current site
            currSite = db.Sites.Where(p => p.site_id == SiteID).FirstOrDefault();
            OfficeID = (int)currSite.office_id;
            WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            ph1.Title = "Station Documents";
            ph1.SubTitle = currSite.site_no + " " + currSite.station_full_nm;
            ph1.ShowOfficeInfoPanel = true;

            if (!Page.IsPostBack)
            {
                //If the user belongs to this site's WSC (or has an exception to work in the WSC), or is a SuperUser, then allow them to edit the page
                if (user.WSCID.Contains(WSCID) || user.IsSuperUser) HasEditAccess = true;

                PopulateSDESCView();
                PopulateSANALView();
                PopulateMANUView();
                PopulateCustomView();

                switch (reportType)
                {
                    case "SDESC":
                        rtsMain.SelectedIndex = 0;
                        rpvSDESC.Selected = true;
                        break;
                    case "SANAL":
                        rtsMain.SelectedIndex = 1;
                        rpvSANAL.Selected = true;
                        break;
                    case "MANU":
                        rtsMain.SelectedIndex = 2;
                        rpvMANU.Selected = true;
                        break;
                    case "Custom":
                        rtsMain.SelectedIndex = 3;
                        rpvCustom.Selected = true;
                        break;
                }
            }
        }

        protected void PopulateSDESCView()
        {
            var SDESCelems = currSite.SiteElements.Where(p => p.ElementDetail.priority < 200);
            if (SDESCelems.Count() > 0)
            {
                DateTime last_revised = Convert.ToDateTime(SDESCelems.OrderByDescending(p => p.revised_dt).FirstOrDefault().revised_dt);
                ltlSDESCRevisedDt.Text = String.Format("{0:MM/dd/yyyy}", last_revised);
                ltlSDESCRevisedBy.Text = SDESCelems.OrderByDescending(p => p.revised_dt).FirstOrDefault().revised_by;
            }
            else
            {
                ltlSDESCRevisedDt.Text = "<i>N/A</i>";
                ltlSDESCRevisedBy.Text = "<i>N/A</i>";
            }

            dlSDESC.DataSource = svcSIMS.GetElementsBySiteAndReport(currSite.site_no, currSite.agency_cd, "SDESC");
            dlSDESC.DataBind();
        }

        protected void PopulateSANALView()
        {
            var SANALelems = currSite.SiteElements.Where(p => p.ElementDetail.priority > 199 && p.ElementDetail.priority < 300);
            if (SANALelems.Count() > 0)
            {
                DateTime last_revised = Convert.ToDateTime(SANALelems.OrderByDescending(p => p.revised_dt).FirstOrDefault().revised_dt);
                ltlSANALRevisedDt.Text = String.Format("{0:MM/dd/yyyy}", last_revised);
                ltlSANALRevisedBy.Text = SANALelems.OrderByDescending(p => p.revised_dt).FirstOrDefault().revised_by;
            }
            else
            {
                ltlSANALRevisedDt.Text = "<i>N/A</i>";
                ltlSANALRevisedBy.Text = "<i>N/A</i>";
            }

            dlSANAL.DataSource = svcSIMS.GetElementsBySiteAndReport(currSite.site_no, currSite.agency_cd, "SANAL");
            dlSANAL.DataBind();
        }

        protected void PopulateMANUView()
        {
            var MANUelems = currSite.SiteElements.Where(p => p.ElementDetail.priority > 299);
            if (MANUelems.Count() > 0)
            {
                DateTime last_revised = Convert.ToDateTime(MANUelems.OrderByDescending(p => p.revised_dt).FirstOrDefault().revised_dt);
                ltlMANURevisedDt.Text = String.Format("{0:MM/dd/yyyy}", last_revised);
                ltlMANURevisedBy.Text = MANUelems.OrderByDescending(p => p.revised_dt).FirstOrDefault().revised_by;
            }
            else
            {
                ltlMANURevisedDt.Text = "<i>N/A</i>";
                ltlMANURevisedBy.Text = "<i>N/A</i>";
            }

            if (!string.IsNullOrEmpty(currSite.alt_basin_nm)) 
                ltlPublishedBasin.Text = currSite.alt_basin_nm;
            else
                ltlPublishedBasin.Text = db.HUCs.FirstOrDefault(p => p.huc_cd == db.vSITEFILEs.FirstOrDefault(n => n.site_id == currSite.nwisweb_site_id).huc_cd).basin;
            if (!string.IsNullOrEmpty(currSite.station_full_nm))
                ltlPublishedName.Text = currSite.station_full_nm;
            else
                ltlPublishedName.Text = db.vSITEFILEs.FirstOrDefault(p => p.site_id == currSite.nwisweb_site_id).station_nm;

            dlMANU.DataSource = svcSIMS.GetElementsBySiteAndReport(currSite.site_no, currSite.agency_cd, "MANU");
            dlMANU.DataBind();
        }

        protected void PopulateCustomView()
        {
            rcblElements.DataSource = currSite.SiteElements.Select(p => new { element_id = p.element_id, element_nm = p.ElementDetail.element_nm, priority = p.ElementDetail.priority }).OrderBy(p => p.priority).ToList();
            rcblElements.DataBind();

            pnlElements.Visible = true;
            pnlCustomReport.Visible = false;
        }

        protected void rcblElements_ItemDataBound(object sender, ButtonListEventArgs e)
        {
            ButtonListItem item = (ButtonListItem)e.Item;

            int element_id = Convert.ToInt32(item.Value);
            string report_type = db.ElementReportRefs.FirstOrDefault(p => p.element_id == element_id).report_type_cd;

            switch (report_type)
            {
                case "SDESC":
                    break;
                case "MANU":
                    break;
                case "SANAL":
                    break;
            }
        }

        protected void rbCustom_Command(object sender, EventArgs e)
        {
            List<int> element_ids = new List<int>();

            ltlReportTitle.Text = rtbReportTitle.Text;
            foreach (ButtonListItem item in rcblElements.Items)
            {
                if (item.Selected) element_ids.Add(Convert.ToInt32(item.Value));
            }
            List<ElementItem> elements = new List<ElementItem>();
            foreach (int id in element_ids)
            {
                elements.Add(currSite.SiteElements.Where(p => p.element_id == id).Select(p => new ElementItem
                {
                    ElementID = p.element_id.ToString(),
                    SiteID = p.site_id.ToString(),
                    ElementName = p.ElementDetail.element_nm,
                    ElementInfo = p.element_info.FormatElementInfo(id, currSite.site_id),
                    RevisedBy = p.revised_by,
                    RevisedDate = p.revised_dt.ToString()
                }).FirstOrDefault());
            }

            dlCustom.DataSource = elements;
            dlCustom.DataBind();

            pnlElements.Visible = false;
            pnlCustomReport.Visible = true;
        }

        protected void rbBack_Command(object sender, EventArgs e)
        {
            PopulateCustomView();
        }

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
        #endregion

    }
}