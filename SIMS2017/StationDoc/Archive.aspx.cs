using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

namespace SIMS2017.StationDoc
{
    public partial class Archive : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
#if DEBUG
        private SIMSDevService.SIMSServiceClient svcSIMS = new SIMSDevService.SIMSServiceClient();
#else
        private SIMSService.SIMSServiceClient svcSIMS = new SIMSService.SIMSServiceClient();
#endif
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        public Boolean HasEditAccess { get; set; }
        private Data.Site currSite;
        private int element_id { get; set; }
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
            string site_id = Request.QueryString["site_id"];
            if (!string.IsNullOrEmpty(site_id)) SiteID = Convert.ToInt32(site_id); else if (SiteID == 0) Response.Redirect(Config.SIMSURL + "SIMSWSCHome.aspx");

            //Set the element_id if it was passed from the browser
            if (!string.IsNullOrEmpty(Request.QueryString["element_id"])) element_id = Convert.ToInt32(Request.QueryString["element_id"]); 

            //Using the passed site_id, setup the site data element, and reset the office and wsc to match that of the current site
            currSite = db.Sites.Where(p => p.site_id == SiteID).FirstOrDefault();
            OfficeID = (int)currSite.office_id;
            WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            if (element_id == 0) ph1.Title = "Retrieve Archived Elements"; 
            else ph1.Title = "Archives for " + db.ElementDetails.FirstOrDefault(p => p.element_id == element_id).element_nm;

            ph1.SubTitle = currSite.site_no + " " + db.vSITEFILEs.FirstOrDefault(s => s.site_id == currSite.nwisweb_site_id).station_nm;
            ph1.ShowOfficeInfoPanel = true;

            if (!Page.IsPostBack)
            {
                //If the user belongs to this site's WSC (or has an exception to work in the WSC), or is a SuperUser, then allow them to edit the page
                if (user.WSCID.Contains(WSCID) || user.IsSuperUser) HasEditAccess = true;

                if (element_id == 0)
                {
                    gvElementList.DataSource = db.SP_Element_Info_Archives(SiteID).ToList();
                    gvElementList.DataBind();

                    pnlStep1.Visible = true;
                    pnlStep2.Visible = false;
                    pnlStep3.Visible = false;
                }
                else
                {
                    ShowArchivedInfo();
                    pnlStep1.Visible = false;
                    pnlStep2.Visible = false;
                    pnlStep3.Visible = true;
                    lbBack2.Visible = false;
                    lbBack3.Visible = false;
                }
            }
        }

        protected void ElementSelected(object sender, CommandEventArgs e)
        {
            element_id = Convert.ToInt32(e.CommandName);
            string element_nm = db.ElementDetails.FirstOrDefault(p => p.element_id == element_id).element_nm;

            var BU_elems = db.SiteElementBackups.Where(p => p.site_id == currSite.site_id && p.element_id == element_id && p.element_info != null).ToList();

            DateTime? first_revised_dt = BU_elems.OrderBy(p => p.backup_dt).FirstOrDefault().revised_dt;
            DateTime? last_revised_dt = BU_elems.OrderByDescending(p => p.backup_dt).FirstOrDefault().revised_dt;
            DateTime begin_dt, end_dt;

            if (first_revised_dt != null) begin_dt = Convert.ToDateTime(first_revised_dt); else begin_dt = Convert.ToDateTime(BU_elems.OrderBy(p => p.backup_dt).First().backup_dt);
            if (last_revised_dt != null) end_dt = Convert.ToDateTime(last_revised_dt); else end_dt = Convert.ToDateTime(BU_elems.OrderByDescending(p => p.backup_dt).FirstOrDefault().backup_dt);

            lblElementName.Text = element_nm;
            tbBeginDate.SelectedDate = begin_dt;
            tbEndDate.SelectedDate = end_dt;
            btnRetrieve.CommandName = element_id.ToString();

            pnlStep1.Visible = false;
            pnlStep2.Visible = true;
            pnlStep3.Visible = false;
        }

        protected void RetrieveArchivedInfo(object sender, CommandEventArgs e)
        {
            if (Page.IsValid)
            {
                lblElementName2.Text = lblElementName.Text;

                var archived_info = db.SP_Element_Info_Archives_by_element_id(currSite.site_id, Convert.ToInt32(e.CommandName), tbBeginDate.SelectedDate, tbEndDate.SelectedDate).ToList();

                if (archived_info.Count() > 0)
                {
                    dlElementInfo.DataSource = archived_info;
                    dlElementInfo.DataBind();
                    lblNothingReturned.Visible = false;
                    dlElementInfo.Visible = true;
                }
                else
                {
                    dlElementInfo.Visible = false;
                    lblNothingReturned.Visible = true;
                }

                pnlStep1.Visible = false;
                pnlStep2.Visible = false;
                pnlStep3.Visible = true;
            }
        }

        protected void Back_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName.ToString() == "back2")
            {
                pnlStep1.Visible = false;
                pnlStep2.Visible = true;
                pnlStep3.Visible = false;
            }
            else
            {
                gvElementList.DataSource = db.SP_Element_Info_Archives(SiteID).ToList();
                gvElementList.DataBind();

                pnlStep1.Visible = true;
                pnlStep2.Visible = false;
                pnlStep3.Visible = false;
            }
        }

        protected void ShowArchivedInfo()
        {
            lblElementName2.Text = "";

            var archived_info = db.SP_Element_Info_Archives_by_element_id(currSite.site_id, element_id, Convert.ToDateTime("01/01/1990"), DateTime.Now).ToList();
            if (archived_info.Count() > 0)
            {
                dlElementInfo.DataSource = archived_info;
                dlElementInfo.DataBind();
                lblNothingReturned.Visible = false;
                dlElementInfo.Visible = true;
            }
            else
            {
                dlElementInfo.Visible = false;
                lblNothingReturned.Text = "No archive information exists for this element";
                lblNothingReturned.Visible = true;
            }
        }

        protected void gvElementList_Bound(object sender, EventArgs e)
        {
            GridViewRow gvRow = null;

            if (gvElementList.Rows.Count > 1)
            {
                for (int rowIndex = gvElementList.Rows.Count - 1; rowIndex >= 0; rowIndex += -1)
                {
                    gvRow = gvElementList.Rows[rowIndex];

                    string priority = gvRow.Cells[5].Text;
                    Color bgc = default(Color);

                    if (Convert.ToInt32(priority) != 6 & Convert.ToInt32(priority) != 2)
                    {
                        if (Convert.ToInt32(priority) < 200)
                        {
                            bgc = Color.White;
                            //bgc = "bgcolor=""#FFFFFF"""
                        }
                        else if (Convert.ToInt32(priority) > 199 & Convert.ToInt32(priority) < 300)
                        {
                            bgc = Color.LightGray;
                            //bgc = "bgcolor=""#CCCCCC"""
                        }
                        else if (Convert.ToInt32(priority) > 299)
                        {
                            bgc = Color.LightSteelBlue;
                            //bgc = "bgcolor=""#97b2dc"""
                        }
                    }
                    else
                    {
                        bgc = Color.White;
                    }

                    gvRow.Cells[0].BackColor = bgc;
                    gvRow.Cells[1].BackColor = bgc;
                    gvRow.Cells[2].BackColor = bgc;
                    gvRow.Cells[3].BackColor = bgc;
                    gvRow.Cells[4].BackColor = bgc;
                }
            }
        }

    }
}