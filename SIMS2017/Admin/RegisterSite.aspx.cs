using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIMS2017.Admin
{
    public partial class RegisterSite : System.Web.UI.Page
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

            ph1.Title = "Initial Site Registration";
            ph1.SubTitle = currWSC.wsc_nm + " Water Science Center";
            ph1.ShowOfficeInfoPanel = true;

            //If the user belongs to this site's WSC (or has an exception to work in the WSC), or is a SuperUser, then allow them to edit the page
            if (user.WSCID.Contains(WSCID) && user.IsAdmin || user.IsSuperUser) HasEditAccess = true;

            if (!Page.IsPostBack)
            {
                InitialPageSetup();
            }
        }

        protected void InitialPageSetup()
        {
            if (!HasEditAccess)
            {
                pnlHasAccess.Visible = false;
                pnlNoAccess.Visible = true;
            }
            else
            {
                pnlHasAccess.Visible = true;
                pnlNoAccess.Visible = false;

                pnlEnterSite.Visible = true;
                pnlConfirmSite.Visible = false;
                pnlError.Visible = false;
                pnlFinal.Visible = false;

                rddlOffice.DataSource = db.Offices.Where(p => p.wsc_id == WSCID).OrderBy(p => p.office_nm).ToList();
                rddlOffice.DataBind();

                rddlOffice.SelectedValue = OfficeID.ToString();

                rtbSiteNo.Text = "";
            }
        }

        protected void SubmitEvent(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument.ToString() == "ConfirmSite")
            {
                if (string.IsNullOrEmpty(rtbSiteNo.Text.ToString()) || string.IsNullOrEmpty(rtbAgencyCode.Text))
                {
                    pnlError.Visible = true;
                    ltlError.Text = "You must enter a site number and agency code.";
                }
                else
                {
                    var sims_site = db.Sites.FirstOrDefault(p => p.site_no.Trim() == rtbSiteNo.Text.ToString() && p.agency_cd == rtbAgencyCode.Text);
                    if (sims_site != null)
                    {
                        pnlError.Visible = true;
                        ltlError.Text = "The site number you entered is already registered in SIMS.";
                        return;
                    }

                    var site = db.vSITEFILEUNUSEDs.FirstOrDefault(p => p.site_no.Trim() == rtbSiteNo.Text.ToString() && p.agency_cd == rtbAgencyCode.Text);
                    if (site == null)
                    {
                        pnlError.Visible = true;
                        ltlError.Text = "The site number you entered was not found in the NWISWeb SITEFILE.";
                        return;
                    }

                    pnlError.Visible = false;
                    pnlEnterSite.Visible = false;
                    pnlConfirmSite.Visible = true;

                    ltlSiteNo.Text = rtbSiteNo.Text;
                    ltlOffice.Text = rddlOffice.SelectedText;
                    ltlSiteName.Text = site.station_nm;
                    rtbSiteName.Text = site.station_nm;
                }
            }
            else if (e.CommandArgument.ToString() == "AddSite")
            {
                var site = db.vSITEFILEUNUSEDs.FirstOrDefault(p => p.site_no.Trim() == rtbSiteNo.Text.ToString() && p.agency_cd == rtbAgencyCode.Text);
                var site_type = db.SiteTypes.FirstOrDefault(p => p.site_tp_cd == site.site_tp_cd).sims_site_tp;

                Data.Site new_site = new Data.Site()
                {
                    site_no = rtbSiteNo.Text,
                    station_full_nm = rtbSiteName.Text,
                    office_id = Convert.ToInt32(rddlOffice.SelectedValue),
                    agency_cd = rtbAgencyCode.Text,
                    nwisweb_site_id = site.site_id,
                    nwis_host = site.nwis_host,
                    db_no = site.db_no
                };
                db.Sites.InsertOnSubmit(new_site);
                db.SubmitChanges();

                Data.SiteElement elem = new Data.SiteElement()
                {
                    site_id = new_site.site_id,
                    element_id = 28,
                    entered_by = "Admin",
                    entered_dt = DateTime.Now,
                    revised_by = "Admin",
                    revised_dt = DateTime.Now,
                };
                db.SiteElements.InsertOnSubmit(elem);
                db.SubmitChanges();

                //If GW site, then also add the WELL CHARACTERISTICS (104) and DATUM (106) elements
                if (site_type == "gw")
                {
                    elem = new Data.SiteElement()
                    {
                        site_id = new_site.site_id,
                        element_id = 104,
                        entered_by = "Admin",
                        entered_dt = DateTime.Now,
                        revised_by = "Admin",
                        revised_dt = DateTime.Now,
                    };
                    db.SiteElements.InsertOnSubmit(elem);
                    db.SubmitChanges();

                    elem = new Data.SiteElement()
                    {
                        site_id = new_site.site_id,
                        element_id = 106,
                        entered_by = "Admin",
                        entered_dt = DateTime.Now,
                        revised_by = "Admin",
                        revised_dt = DateTime.Now,
                    };
                    db.SiteElements.InsertOnSubmit(elem);
                    db.SubmitChanges();
                }
                
                //Add record to the Elem_Report_Approve table for the MANU
                Data.ElemReportApprove elem_approve = new Data.ElemReportApprove()
                {
                    site_id = new_site.site_id,
                    report_type_cd = "MANU",
                    publish_complete = "N"
                };
                db.ElemReportApproves.InsertOnSubmit(elem_approve);
                db.SubmitChanges();

                //Add three records to the Eleme_Report_Sum table for each report type
                Data.ElemReportSum elem_report = new Data.ElemReportSum()
                {
                    site_id = new_site.site_id,
                    report_type_cd = "MANU",
                    revised_dt = DateTime.Now
                };
                db.ElemReportSums.InsertOnSubmit(elem_report);
                db.SubmitChanges();

                elem_report = new Data.ElemReportSum()
                {
                    site_id = new_site.site_id,
                    report_type_cd = "SDESC",
                    revised_dt = DateTime.Now
                };
                db.ElemReportSums.InsertOnSubmit(elem_report);
                db.SubmitChanges();

                elem_report = new Data.ElemReportSum()
                {
                    site_id = new_site.site_id,
                    report_type_cd = "SANAL",
                    revised_dt = DateTime.Now
                };
                db.ElemReportSums.InsertOnSubmit(elem_report);
                db.SubmitChanges();

                pnlEnterSite.Visible = false;
                pnlConfirmSite.Visible = false;
                pnlFinal.Visible = true;

                hlStationInfo.NavigateUrl = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMS2017URL, new_site.site_id);
            }
            else
                InitialPageSetup();
        }

        protected void CancelEvent(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument.ToString() == "Cancel")
                Response.Redirect(String.Format("{0}Admin/Tasks.aspx?office_id={1}", Config.SIMS2017URL, OfficeID));
            else
                InitialPageSetup();

        }
    }
}