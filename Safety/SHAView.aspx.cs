using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Safety
{
    public partial class SHAView : System.Web.UI.Page
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
            string site_id = Request.QueryString["site_id"];
            if (!string.IsNullOrEmpty(site_id)) SiteID = Convert.ToInt32(site_id); else Response.Redirect(Config.SIMS2017URL + "SIMSWSCHome.aspx");

            //Using the passed site_id, setup the SHA data element, and reset the office and wsc to match that of the current site
            currSHA = db.SHAs.Where(p => p.site_id == SiteID).FirstOrDefault();
            OfficeID = (int)currSHA.Site.office_id;
            WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            //--BASIC PAGE SETUP--------------------------------------------------------------------
            ph1.Title = "Site Hazard Analysis";
            ph1.SubTitle = currSHA.Site.site_no + " " + currSHA.Site.station_full_nm;

            //Fills out the header information
            PopulateHeader();
            //Fill the page with data
            PopulateReport();
        }


        #region Data Properties
        private List<Models.JHAModel> DischargeMeasJHA
        {
            get
            {
                List<Models.JHAModel> jha = new List<Models.JHAModel>();

                jha = currSHA.SHAJHAs.Where(p => p.ElementJHA.element_id == Config.DischargeMeasElem).Select(p => new Models.JHAModel()
                {
                    sha_site_id = p.sha_site_id,
                    site_jha_id = p.site_jha_id,
                    elem_jha_id = p.elem_jha_id,
                    jha_id = p.ElementJHA.jha_id,
                    jha_description = p.ElementJHA.JHA.jha_description
                }).ToList();

                return jha;
            }
        }
        private List<Models.JHAModel> QWMeasJHA
        {
            get
            {
                List<Models.JHAModel> jha = new List<Models.JHAModel>();

                jha = currSHA.SHAJHAs.Where(p => p.ElementJHA.element_id == Config.QWMeasElem).Select(p => new Models.JHAModel()
                {
                    sha_site_id = p.sha_site_id,
                    site_jha_id = p.site_jha_id,
                    elem_jha_id = p.elem_jha_id,
                    jha_id = p.ElementJHA.jha_id,
                    jha_description = p.ElementJHA.JHA.jha_description
                }).ToList();

                return jha;
            }
        }
        private List<Models.JHAModel> GWMeasJHA
        {
            get
            {
                List<Models.JHAModel> jha = new List<Models.JHAModel>();

                jha = currSHA.SHAJHAs.Where(p => p.ElementJHA.element_id == Config.GWMeasElem).Select(p => new Models.JHAModel()
                {
                    sha_site_id = p.sha_site_id,
                    site_jha_id = p.site_jha_id,
                    elem_jha_id = p.elem_jha_id,
                    jha_id = p.ElementJHA.jha_id,
                    jha_description = p.ElementJHA.JHA.jha_description
                }).ToList();

                return jha;
            }
        }
        private List<Models.JHAModel> LakeMeasJHA
        {
            get
            {
                List<Models.JHAModel> jha = new List<Models.JHAModel>();

                jha = currSHA.SHAJHAs.Where(p => p.ElementJHA.element_id == Config.LakeMeasElem).Select(p => new Models.JHAModel()
                {
                    sha_site_id = p.sha_site_id,
                    site_jha_id = p.site_jha_id,
                    elem_jha_id = p.elem_jha_id,
                    jha_id = p.ElementJHA.jha_id,
                    jha_description = p.ElementJHA.JHA.jha_description
                }).ToList();

                return jha;
            }
        }
        private List<Models.JHAModel> EcoMeasJHA
        {
            get
            {
                List<Models.JHAModel> jha = new List<Models.JHAModel>();

                jha = currSHA.SHAJHAs.Where(p => p.ElementJHA.element_id == Config.EcoMeasElem).Select(p => new Models.JHAModel()
                {
                    sha_site_id = p.sha_site_id,
                    site_jha_id = p.site_jha_id,
                    elem_jha_id = p.elem_jha_id,
                    jha_id = p.ElementJHA.jha_id,
                    jha_description = p.ElementJHA.JHA.jha_description
                }).ToList();

                return jha;
            }
        }
        private List<Models.JHAModel> AtmMeasJHA
        {
            get
            {
                List<Models.JHAModel> jha = new List<Models.JHAModel>();

                jha = currSHA.SHAJHAs.Where(p => p.ElementJHA.element_id == Config.AtmMeasElem).Select(p => new Models.JHAModel()
                {
                    sha_site_id = p.sha_site_id,
                    site_jha_id = p.site_jha_id,
                    elem_jha_id = p.elem_jha_id,
                    jha_id = p.ElementJHA.jha_id,
                    jha_description = p.ElementJHA.JHA.jha_description
                }).ToList();

                return jha;
            }
        }
        #endregion

        protected void PopulateHeader()
        {
            ltlDate.Text = DateTime.Now.ToString();
        }

        protected void PopulateReport()
        {
            lvServicingSiteSpecificCond.DataSource = currSHA.SHAServicings.ToList();
            lvServicingSiteSpecificCond.DataBind();

            lvServicingSiteRecEquip.DataSource = currSHA.SHAEquips.ToList();
            lvServicingSiteRecEquip.DataBind();

            var elements = db.SiteElements.Where(p => p.site_id == SiteID).ToList();

            foreach (var elem in elements)
            {
                int element_id = Convert.ToInt32(elem.element_id);
                if (element_id == Config.DischargeMeasElem) PopulateDischargeMeasJHA();
                if (element_id == Config.QWMeasElem) PopulateQWMeasJHA();
                if (element_id == Config.GWMeasElem) PopulateGWMeasJHA();
                if (element_id == Config.LakeMeasElem) PopulateLakeMeasJHA();
                if (element_id == Config.EcoMeasElem) PopulateEcoMeasJHA();
                if (element_id == Config.AtmMeasElem) PopulateAtmMeasJHA();
            }

            bool emerg_service = Convert.ToBoolean(currSHA.emerg_service);

            if (emerg_service)
            {
                ltlEmergService.Text = "911 emergency service is available at this site.<br />";
            }
            else
            {
                ltlEmergService.Text = "911 emergency service is NOT available at this site.<br />";
            }

            bool cell_service = Convert.ToBoolean(currSHA.cell_service);

            if (cell_service)
            {
                ltlCellService.Text = "Cell service is available at this site.<br /><br />";
            }
            else
            {
                ltlCellService.Text = "Cell service is NOT available at this site.<br /><br />";
            }

            lvEmergContacts.DataSource = currSHA.SHAContacts.Select(p => new Data.Contact {
                contact_nm = p.Contact.contact_nm,
                street_addrs = p.Contact.street_addrs,
                state = p.Contact.state,
                city = p.Contact.city,
                zip = p.Contact.zip,
                ph_no = p.Contact.ph_no
            }).ToList();
            lvEmergContacts.DataBind();

            lvHospitals.DataSource = currSHA.SHAHospitals.Select(p => new Data.Hospital { 
                hospital_nm = p.Hospital.hospital_nm,
                street_addrs = p.Hospital.street_addrs,
                state = p.Hospital.state,
                city = p.Hospital.city,
                zip = p.Hospital.zip,
                ph_no = p.Hospital.ph_no,
                dec_lat_va = p.Hospital.dec_lat_va,
                dec_long_va = p.Hospital.dec_long_va
            }).ToList();
            lvHospitals.DataBind();

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
        }

        protected void PopulateDischargeMeasJHA()
        {
            var element = currSHA.Site.SiteElements.FirstOrDefault(p => p.element_id == Config.DischargeMeasElem);

            ltlDMRevisedBy.Text = element.revised_by;
            ltlDMRevisedDate.Text = String.Format("{0:MM/dd/yyyy}", element.revised_dt);
            ltlDischargeMeas.Text = element.element_info.Replace("\n\n", "<br /><br />\n");

            lvDischargeMeasJHA.DataSource = DischargeMeasJHA;
            lvDischargeMeasJHA.DataBind();

            pnlDischargeMeas.Visible = true;
        }

        protected void PopulateQWMeasJHA()
        {
            var element = currSHA.Site.SiteElements.FirstOrDefault(p => p.element_id == Config.QWMeasElem);

            ltlQWRevisedBy.Text = element.revised_by;
            ltlQWRevisedDate.Text = String.Format("{0:MM/dd/yyyy}", element.revised_dt);
            ltlQWMeas.Text = element.element_info.Replace("\n\n", "<br /><br />\n");

            lvQWMeasJHA.DataSource = QWMeasJHA;
            lvQWMeasJHA.DataBind();

            pnlQWMeas.Visible = true;
        }

        protected void PopulateGWMeasJHA()
        {
            var element = currSHA.Site.SiteElements.FirstOrDefault(p => p.element_id == Config.GWMeasElem);

            ltlGWRevisedBy.Text = element.revised_by;
            ltlGWRevisedDate.Text = String.Format("{0:MM/dd/yyyy}", element.revised_dt);
            ltlGWMeas.Text = element.element_info.Replace("\n\n", "<br /><br />\n");

            lvGWMeasJHA.DataSource = GWMeasJHA;
            lvGWMeasJHA.DataBind();

            pnlGWMeas.Visible = true;
        }

        protected void PopulateLakeMeasJHA()
        {
            var element = currSHA.Site.SiteElements.FirstOrDefault(p => p.element_id == Config.LakeMeasElem);

            ltlLakeRevisedBy.Text = element.revised_by;
            ltlLakeRevisedDate.Text = String.Format("{0:MM/dd/yyyy}", element.revised_dt);
            ltlLakeMeas.Text = element.element_info.Replace("\n\n", "<br /><br />\n");

            lvLakeMeasJHA.DataSource = LakeMeasJHA;
            lvLakeMeasJHA.DataBind();

            pnlLakeMeas.Visible = true;
        }

        protected void PopulateEcoMeasJHA()
        {
            var element = currSHA.Site.SiteElements.FirstOrDefault(p => p.element_id == Config.EcoMeasElem);

            ltlEcoRevisedBy.Text = element.revised_by;
            ltlEcoRevisedDate.Text = String.Format("{0:MM/dd/yyyy}", element.revised_dt);
            ltlEcoMeas.Text = element.element_info.Replace("\n\n", "<br /><br />\n");

            lvEcoMeasJHA.DataSource = EcoMeasJHA;
            lvEcoMeasJHA.DataBind();

            pnlEcoMeas.Visible = true;
        }

        protected void PopulateAtmMeasJHA()
        {
            var element = currSHA.Site.SiteElements.FirstOrDefault(p => p.element_id == Config.AtmMeasElem);

            ltlAtmRevisedBy.Text = element.revised_by;
            ltlAtmRevisedDate.Text = String.Format("{0:MM/dd/yyyy}", element.revised_dt);
            ltlAtmMeas.Text = element.element_info.Replace("\n\n", "<br /><br />\n");

            lvAtmMeasJHA.DataSource = AtmMeasJHA;
            lvAtmMeasJHA.DataBind();

            pnlAtmMeas.Visible = true;
        }

        protected void lvServicingSiteSpecificCond_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataitem = (ListViewDataItem)e.Item;
                Label lbl = (Label)e.Item.FindControl("lblServicingSiteSpecificCond");

                try
                {
                    int priority = Convert.ToInt32(DataBinder.Eval(dataitem.DataItem, "priority"));
                    if (priority < 0)
                    {
                        lbl.Font.Bold = true;
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
        protected List<Data.SHASpecificCondition> getRemarksForJHA(string site_jha_id)
        {
            var spc = db.SHASpecificConditions.Where(p => p.site_jha_id == Convert.ToInt32(site_jha_id)).OrderBy(p => p.priority).ToList();

            return spc;
        }

        protected List<Models.RefLevelModel> getJobOpLimitsForJHA(string site_jha_id)
        {
            var rlm = db.SHAReferenceLevels.Where(p => p.site_jha_id == Convert.ToInt32(site_jha_id)).Select(p => new Models.RefLevelModel {
                site_reflevel_id = p.site_reflevel_id,
                reflevel_id = p.reflevel_id,
                reflevel_units = p.reflevel_units,
                reflevel_va = Convert.ToDouble(p.reflevel_va),
                remarks = p.remarks,
                reflevel_tp = p.ReferenceLevel.reflevel_tp
            }).ToList();

            return rlm;
        }
    }
}