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
        private SIMSDevService.SIMSServiceClient svcSIMS = new SIMSDevService.SIMSServiceClient();
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
            string site_id = "3000337";// Request.QueryString["site_id"];
            if (!string.IsNullOrEmpty(site_id)) SiteID = Convert.ToInt32(site_id); else Response.Redirect(Config.SIMS2017URL + "SIMSWSCHome.aspx");

            reportType = "SDESC";// Request.QueryString["type"];

            //Using the passed site_id, setup the site data element, and reset the office and wsc to match that of the current site
            currSite = db.Sites.Where(p => p.site_id == SiteID).FirstOrDefault();
            OfficeID = (int)currSite.office_id;
            WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;

            ph1.Title = "Station Documents";
            ph1.SubTitle = currSite.site_no + " " + currSite.station_full_nm;

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
            var SDESCelems = currSite.ElementSite.SiteElements.Where(p => p.ElementDetail.priority < 200);
            if (SDESCelems != null)
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
            var SANALelems = currSite.ElementSite.SiteElements.Where(p => p.ElementDetail.priority > 199 && p.ElementDetail.priority < 300);
            if (SANALelems != null)
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
            var MANUelems = currSite.ElementSite.SiteElements.Where(p => p.ElementDetail.priority > 299);
            if (MANUelems != null)
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
            rcblElements.DataSource = currSite.ElementSite.SiteElements.Select(p => new { element_id = p.element_id, element_nm = p.ElementDetail.element_nm, priority = p.ElementDetail.priority }).OrderBy(p => p.priority).ToList();
            rcblElements.DataBind();

            pnlElements.Visible = true;
            pnlCustomReport.Visible = false;
        }

        protected void rcblElements_DataBound(object sender, EventArgs e)
        {

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
                elements.Add(currSite.ElementSite.SiteElements.Where(p => p.element_id == id).Select(p => new ElementItem
                {
                    ElementID = p.element_id.ToString(),
                    SiteID = p.site_id.ToString(),
                    ElementName = p.ElementDetail.element_nm,
                    ElementInfo = FormatElementInfo(currSite.site_no, p.element_info, p.ElementDetail.element_nm),
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

        #region Helper Methods
        private string FormatElementInfo(string site_no, string element_info, string element_nm)
        {
            string elem_info = element_info;
            string pOut = "";

            try
            {
                var lstNWISInfo = (from n in db.vNWISSiteInfoForManus 
                                    where n.site_no.Equals(site_no)
                                    select new
                                    {
                                        SiteUseCode = n.site_use_cd,
                                        WellDepth = n.well_depth_va,
                                        AqfBookName = n.aqfr_book_nm,
                                        AltDatumCode = n.alt_datum_cd,
                                        Altitude = n.alt_va,
                                        DistrictAbbrev = n.district_abbrev,
                                        CountyName = n.county_nm,
                                        LocationDatumDescription = n.datum_desc,
                                        DatumDescription = n.description,
                                        DrainageArea = n.drain_area_va,
                                        ContributingDrainageArea = n.contrib_drain_area_va,
                                        Latitude = n.lat_va,
                                        Longitude = n.long_va,
                                        LandNetDesc = n.land_net_ds,
                                        HUCCode = n.huc_cd,
                                        UpperCasingDia = n.upperCasingDia,
                                        OpenTopVa = n.openTopVa,
                                        OpenBottomVa = n.openBottomVa,
                                        MPString = n.mpString,
                                        MPAltString = n.mpAltString,
                                        UseMpntAlt = n.use_mpntalt,
                                        WellDepthVa = n.wellDepthVa
                                    });

                switch (element_nm)
                {
                    case "LOCATION":
                        #region "Set Variables for Location element"
                        string lat_va = "";
                        string long_va = "";
                        string land_net_ds = "";
                        string huc_cd = "is unknown";
                        string county_nm = "UNSPECIFIED";
                        string district_abbrev = "";
                        string dat_desc = "";

                        foreach (var l in lstNWISInfo)
                        {
                            lat_va = l.Latitude;
                            long_va = l.Longitude;
                            land_net_ds = l.LandNetDesc;
                            try
                            {
                                if (string.IsNullOrEmpty(l.HUCCode))
                                {
                                    huc_cd = "is unknown";
                                }
                                else
                                {
                                    huc_cd = l.HUCCode;
                                }
                            }
                            catch (Exception ex) { huc_cd = "is unknown"; }
                            try
                            {
                                if (string.IsNullOrEmpty(l.CountyName))
                                {
                                    county_nm = "UNSPECIFIED";
                                }
                                else
                                {
                                    county_nm = l.CountyName + ", ";
                                }
                            }
                            catch (Exception ex) { county_nm = "UNSPECIFIED"; }
                            try
                            {
                                if (string.IsNullOrEmpty(l.DistrictAbbrev))
                                {
                                    district_abbrev = "";
                                }
                                else
                                {
                                    district_abbrev = l.DistrictAbbrev + ", ";
                                }
                            }
                            catch (Exception ex) { district_abbrev = ""; }
                            try
                            {
                                if (string.IsNullOrEmpty(l.LocationDatumDescription))
                                {
                                    dat_desc = "";
                                }
                                else
                                {
                                    dat_desc = " referenced to " + l.LocationDatumDescription;
                                }
                            }
                            catch (Exception ex) { dat_desc = ""; }
                        }
                        #endregion

                        //Format the Land Net Description (Township and Range)
                        //Correct format in NWIS: 'SESESES16 T14S  R65E  M'
                        #region "Land Net Description"
                        try
                        {
                            if (!string.IsNullOrEmpty(land_net_ds))
                            {
                                if (land_net_ds.Substring(6, 1) != "S")
                                {
                                    land_net_ds = " " + land_net_ds.Trim() + ",";
                                }
                                else
                                {
                                    string lnd = land_net_ds;
                                    string lnd1 = lnd.Substring(0, 2);
                                    if (lnd1 != "  ")
                                    {
                                        lnd1 = lnd1 + " 1/4 ";
                                    }
                                    else
                                    {
                                        lnd1 = "";
                                    }

                                    string lnd2 = lnd.Substring(2, 2);
                                    if (lnd2 != "  ")
                                    {
                                        lnd2 = lnd2 + " 1/4 ";
                                    }
                                    else
                                    {
                                        lnd2 = "";
                                    }

                                    string lnd3 = lnd.Substring(4, 2);
                                    if (lnd3 != "  ")
                                    {
                                        lnd3 = lnd3 + " 1/4 ";
                                    }
                                    else
                                    {
                                        lnd3 = "";
                                    }

                                    string lnd4 = lnd.Substring(7, 3);
                                    if (lnd4 != "   ")
                                    {
                                        try
                                        {
                                            lnd4 = "sec." + lnd4.Trim() + ", ";
                                        }
                                        catch (Exception ex)
                                        {
                                            lnd4 = "";
                                        }
                                    }
                                    else
                                    {
                                        lnd4 = "";
                                    }

                                    string lnd5 = "";
                                    try
                                    {
                                        //Check to see which template is being used for parsing out the rest
                                        lnd5 = lnd.Substring(11, 5);

                                        if (lnd5 != "     ")
                                        {
                                            lnd5 = lnd.Substring(11, 5).Trim();
                                            if (lnd5.Length > 1)
                                            {
                                                if (IsNumeric(lnd5.Substring(0, lnd5.Length - 1)) | IsNumeric(lnd5.Substring(0, lnd5.Length - 2)))
                                                {
                                                    if (lnd5.Substring(lnd5.Length - 2, 1) == "H")
                                                    {
                                                        lnd5 = "T." + Convert.ToInt64(lnd5.Substring(0, lnd5.Length - 2)).ToString() + " 1/2 " + lnd5.Substring(lnd5.Length - 1, 1) + "., ";
                                                    }
                                                    else
                                                    {
                                                        lnd5 = "T." + Convert.ToInt64(lnd5.Substring(0, lnd5.Length - 1)).ToString() + " " + lnd5.Substring(lnd5.Length - 1, 1) + "., ";
                                                    }
                                                }
                                                else
                                                {
                                                    lnd5 = "";
                                                }
                                            }
                                            else
                                            {
                                                lnd5 = "";
                                            }
                                        }
                                        else
                                        {
                                            lnd5 = "";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        lnd5 = "";
                                    }

                                    string lnd6 = "";
                                    try
                                    {
                                        if (lnd.Length > 17)
                                        {
                                            try
                                            {
                                                lnd6 = lnd.Substring(17, 5).Trim();
                                            }
                                            catch (Exception ex)
                                            {
                                                lnd6 = lnd.Substring(17).Trim();
                                            }
                                            if (!string.IsNullOrEmpty(lnd6) && lnd6.Length > 1)
                                            {
                                                if (IsNumeric(lnd6.Substring(0, lnd6.Length - 1)) | IsNumeric(lnd6.Substring(0, lnd6.Length - 2)))
                                                {
                                                    if (lnd6.Substring(lnd6.Length - 2, 1) == "H")
                                                    {
                                                        lnd6 = "R." + Convert.ToInt64(lnd6.Substring(0, lnd6.Length - 2)).ToString() + " 1/2 " + lnd6.Substring(lnd6.Length - 1, 1) + "., ";
                                                    }
                                                    else
                                                    {
                                                        lnd6 = "R." + Convert.ToInt64(lnd6.Substring(0, lnd6.Length - 1)).ToString() + " " + lnd6.Substring(lnd6.Length - 1, 1) + "., ";
                                                    }
                                                }
                                                else
                                                {
                                                    lnd6 = "";
                                                }
                                            }
                                            else
                                            {
                                                lnd6 = "";
                                            }
                                        }
                                        else
                                        {
                                            lnd6 = "";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        lnd6 = "";
                                    }


                                    land_net_ds = " in " + lnd1 + lnd2 + lnd3 + lnd4 + lnd5 + lnd6;

                                    if (land_net_ds == " in " | lnd5 == "" | lnd6 == "")
                                    {
                                        land_net_ds = "";
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            land_net_ds = "Error: " + ex.Message;
                        }
                        #endregion

                        //Modify the Lat and Long for appropriate text output
                        #region "Latitude and Longitude"
                        if (string.IsNullOrEmpty(lat_va))
                        {
                            lat_va = "<i>unavailable</i>";
                        }
                        else
                        {
                            if (lat_va.Substring(0, 1) == "-")
                            {
                                lat_va = lat_va.Substring(1, 2) + "&deg;" + lat_va.Substring(3, 2) + "'" + lat_va.Substring(5) + "\"";
                            }
                            else
                            {
                                lat_va = lat_va.Substring(0, 2) + "&deg;" + lat_va.Substring(2, 2) + "'" + lat_va.Substring(4) + "\"";
                            }
                        }
                        if (string.IsNullOrEmpty(long_va))
                        {
                            long_va = "<i>unavailable</i>";
                        }
                        else
                        {
                            if (long_va.Substring(0, 1) == "-")
                            {
                                if (long_va.Substring(1, 1) == "0")
                                {
                                    long_va = long_va.Substring(2, 2) + "&deg;" + long_va.Substring(4, 2) + "'" + long_va.Substring(6) + "\"";
                                }
                                else
                                {
                                    if (district_abbrev == "GM, ")
                                    {
                                        long_va = long_va.Substring(0, 4) + "&deg;" + long_va.Substring(5, 2) + "'" + long_va.Substring(7) + "\"";
                                    }
                                    else
                                    {
                                        long_va = long_va.Substring(1, 3) + "&deg;" + long_va.Substring(4, 2) + "'" + long_va.Substring(6) + "\"";
                                    }
                                }
                            }
                            else if (long_va.Substring(0, 1) == "0")
                            {
                                long_va = long_va.Substring(1, 2) + "&deg;" + long_va.Substring(3, 2) + "'" + long_va.Substring(5) + "\"";
                            }
                            else
                            {
                                long_va = long_va.Substring(0, 3) + "&deg;" + long_va.Substring(3, 2) + "'" + long_va.Substring(5) + "\"";
                            }
                        }
                        #endregion

                        elem_info = "Lat " + lat_va + ", long " + long_va + dat_desc + ", " + land_net_ds + " " + county_nm + district_abbrev + " Hydrologic Unit " + huc_cd + ", " + element_info;

                        break;
                    case "DRAINAGE AREA":
                        #region "Define and assign variables"
                        string da = "";
                        string cda = "";
                        string nca = "";
                        string error_spot = "";

                        foreach (var d in lstNWISInfo)
                        {
                            try
                            {
                                da = d.DrainageArea;
                            }
                            catch (Exception ex)
                            {
                                da = "";
                            }
                            try
                            {
                                cda = d.ContributingDrainageArea;
                            }
                            catch (Exception ex)
                            {
                                cda = "";
                            }
                        }
                        #endregion

                        #region "Compute non-contributing area"
                        if (IsNumeric(da) && IsNumeric(cda))
                        {
                            try
                            {
                                //Calculate the non-contributing drainage area
                                try { nca = (Convert.ToDouble(da) - Convert.ToDouble(cda)).ToString(); }
                                catch (Exception ex) { error_spot = "first spot"; }

                                try
                                {
                                    if (nca == "0")
                                    {
                                        if (!string.IsNullOrEmpty(element_info))
                                        {
                                            pOut = FormatNWISNumber(Convert.ToDecimal(da)) + " mi&#178;, ";
                                        }
                                        else
                                        {
                                            pOut = FormatNWISNumber(Convert.ToDecimal(da)) + " mi&#178;.";
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(element_info))
                                        {
                                            pOut = FormatNWISNumber(Convert.ToDecimal(da)) + " mi&#178; of which " + FormatNWISNumber(Convert.ToDecimal(nca)) + " mi&#178; probably is noncontributing, ";
                                        }
                                        else
                                        {
                                            pOut = FormatNWISNumber(Convert.ToDecimal(da)) + " mi&#178; of which " + FormatNWISNumber(Convert.ToDecimal(nca)) + " mi&#178; probably is noncontributing.";
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    error_spot = "third spot";
                                }

                            }
                            catch (Exception ex)
                            {
                                pOut = "Error with non-contributing: " + ex.Message + ", " + error_spot;
                            }
                        }
                        #endregion
                        #region "No noncontributing area referenced"
                        else if (IsNumeric(da))
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(element_info))
                                {
                                    pOut = FormatNWISNumber(Convert.ToDecimal(da)) + " mi&#178;, ";
                                }
                                else
                                {
                                    pOut = FormatNWISNumber(Convert.ToDecimal(da)) + " mi&#178;.";
                                }
                            }
                            catch (Exception ex)
                            {
                                pOut = "Error with contributing: " + ex.Message;
                            }

                        }
                        #endregion
                        #region "Can't determine a drainage area"
                        else
                        {
                            if (string.IsNullOrEmpty(element_info))
                            {
                                pOut = "unknown";
                            }
                            else
                            {
                                pOut = "";
                            }
                        }
                        #endregion

                        elem_info = pOut + error_spot + element_info;

                        break;
                    case "WELL CHARACTERISTICS (MANU)":
                        #region "Define and assign variables"
                        decimal wd = 0;
                        string ucd = "UPPERCASINGDIAMETER";
                        string otv = "OPENTOPVALUE";
                        string obv = "OPENBOTTOMVALUE";
                        error_spot = "";
                        pOut = "Undefined.";

                        foreach (var w in lstNWISInfo)
                        {
                            try
                            {
                                wd = Convert.ToDecimal(w.WellDepth);
                            }
                            catch (Exception ex)
                            { }
                            try
                            {
                                if (string.IsNullOrEmpty(w.UpperCasingDia))
                                {
                                    ucd = "UPPERCASINGDIAMETER";
                                }
                                else
                                {
                                    ucd = w.UpperCasingDia;
                                }
                            }
                            catch (Exception ex) { ucd = "UPPERCASINGDIAMETER"; }
                            try
                            {
                                if (string.IsNullOrEmpty(w.OpenTopVa))
                                {
                                    otv = "OPENTOPVALUE";
                                }
                                else
                                {
                                    otv = w.OpenTopVa;
                                }
                            }
                            catch (Exception ex) { otv = "OPENTOPVALUE"; }
                            try
                            {
                                if (string.IsNullOrEmpty(w.OpenBottomVa))
                                {
                                    obv = "OPENBOTTOMVALUE";
                                }
                                else
                                {
                                    obv = w.OpenBottomVa;
                                }
                            }
                            catch (Exception ex) { obv = "OPENBOTTOMVALUE"; }
                        }
                        #endregion

                        #region "Format the well depth"
                        if (wd == -9999 | wd == 0)
                        {
                            pOut = "Depth undefined.";
                        }
                        else
                        {
                            pOut = "Depth " + FormatNWISNumber(wd) + " ft.";
                        }
                        #endregion

                        elem_info = pOut + " Upper casing diameter " + ucd + "; top of first opening " + otv + ", bottom of last opening " + obv + ". " + element_info;

                        break;
                    case "DATUM":
                        #region "Define and assign variables"
                        elem_info = "Land-surface datum is undefined. Measuring point: MEASURINGPOINT. ";
                        string vd = "";
                        string mp = "MEASURINGPOINT";
                        decimal alt = 0;
                        string alt_formatted;
                        string uma;

                        foreach (var d in lstNWISInfo)
                        {
                            vd = d.DatumDescription;
                            alt = Convert.ToDecimal(d.Altitude);
                            uma = Convert.ToString(d.UseMpntAlt);
                            try
                            {
                                //If an office has set the use alternate measureing point flag to true, then use the value contained in the MPAltString field
                                if (!String.IsNullOrEmpty(uma))
                                {
                                    if (uma == "True" || uma == "true")
                                    {
                                        if (string.IsNullOrEmpty(d.MPAltString))
                                        {
                                            mp = "MEASURINGPOINT";
                                        }
                                        else
                                        {
                                            mp = d.MPAltString;
                                        }
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(d.MPString))
                                        {
                                            mp = "MEASURINGPOINT";
                                        }
                                        else
                                        {
                                            mp = d.MPString;
                                        }
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(d.MPString))
                                    {
                                        mp = "MEASURINGPOINT";
                                    }
                                    else
                                    {
                                        mp = d.MPString;
                                    }
                                }
                            }
                            catch (Exception ex) { mp = "MEASURINGPOINT"; }
                        }
                        #endregion

                        #region "Format the datum and altitude"
                        try
                        {
                            if (string.IsNullOrEmpty(vd))
                            {
                                vd = "undefined vertical datum";
                            }

                            alt_formatted = FormatNWISNumber(alt);

                            if (alt_formatted == "-9,999")
                            {
                                elem_info = "Land-surface datum is undefined. Measuring point: " + mp + ". " + element_info;
                            }
                            else
                            {
                                elem_info = "Land-surface datum is " + alt_formatted + " ft above " + vd + ". Measuring point: " + mp + ". " + element_info;
                            }
                        }
                        catch (Exception ex) { elem_info = elem_info + "ERROR"; }
                        #endregion

                        break;
                    default:
                        elem_info = element_info;
                        break;
                }

                //Handle special characters
                elem_info = elem_info.Replace(System.Environment.NewLine, "<br />");
                elem_info = elem_info.Replace("`", "&#0176;"); //Degree symbol
                elem_info = elem_info.Replace("mi2", "mi&#178;"); //Superscript 2
                elem_info = elem_info.Replace("ft3", "ft&#179;"); //Superscript 3
            }
            catch (Exception ex)
            {
                elem_info = ex.Message;
            }

            return elem_info;
        }

        private static bool IsNumeric(object obj)
        {
            string strNum = obj.ToString();
            try
            {
                double num;
                bool isNum = double.TryParse(strNum, out num);
                if (isNum)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        private static string FormatNWISNumber(decimal pIn)
        {
            string pOut = pIn.ToString();

            try
            {
                string x;
                string y;
                decimal r = pIn - Math.Round(pIn);
                var hasNoFractionalPart = (r == 0);

                if (hasNoFractionalPart)
                {
                    pOut = pIn.ToString("f0");

                    if (pOut.Length == 4)
                    {
                        x = pOut.Substring(0, 1);
                        y = pOut.Substring(1);
                        pOut = x + "," + y;
                    }
                    else if (pOut.Length == 5)
                    {
                        x = pOut.Substring(0, 2);
                        y = pOut.Substring(2);
                        pOut = x + "," + y;
                    }
                    else if (pOut.Length == 6)
                    {
                        x = pOut.Substring(0, 3);
                        y = pOut.Substring(3);
                        pOut = x + "," + y;
                    }

                    pOut = pOut.Replace(",,", ",");
                }
                else
                {
                    pOut = pIn.ToString("f2");

                    if (pOut.Substring(pOut.Length - 1, 1) == "0")
                    {
                        if (pOut.Length > 4)
                        {
                            pOut = pIn.ToString("f1");
                        }
                    }

                    if (pOut.Substring(0, pOut.IndexOf(".")).Length == 4)
                    {
                        x = pOut.Substring(0, 1);
                        y = pOut.Substring(1);
                        pOut = x + "," + y;
                    }
                    else if (pOut.Substring(0, pOut.IndexOf(".")).Length == 5)
                    {
                        x = pOut.Substring(0, 2);
                        y = pOut.Substring(2);
                        pOut = x + "," + y;
                    }
                    else if (pOut.Substring(0, pOut.IndexOf(".")).Length == 6)
                    {
                        x = pOut.Substring(0, 3);
                        y = pOut.Substring(3);
                        pOut = x + "," + y;
                    }

                    pOut = pOut.Replace(",,", ",");
                }
            }
            catch (Exception ex)
            {
                return pOut;
            }

            return pOut;
        }
        #endregion
    }
}