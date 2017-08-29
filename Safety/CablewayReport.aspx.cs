using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Safety
{
    public partial class CablewayReport : System.Web.UI.Page
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

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            string report_tp = Request.QueryString["tp"];

            if (report_tp == "nw")
            {
                ph1.Title = "Nationwide Cableway Information";
                OfficeID = 0;
                WSCID = 0;

                pnlNW.Visible = true;
                pnlStatus.Visible = false;
            }
            else
            {
                string office_id = Request.QueryString["office_id"];

                if (!string.IsNullOrEmpty(office_id))
                {
                    OfficeID = Convert.ToInt32(office_id);

                    //Using the passed office_id, reset the wsc to match that of the current office
                    WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;
                    var wsc = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID);

                    ph1.Title = "Cableway Status Report";
                    ph1.SubTitle = "For the " + wsc.wsc_nm + " WSC";
                }
                else
                {
                    ph1.Title = "Nationwide Cableway Status Report";

                    OfficeID = 0;
                    WSCID = 0;
                }

                pnlNW.Visible = false;
                pnlStatus.Visible = true;
            }


            if (!Page.IsPostBack)
            {
            }
            //--------------------------------------------------------------------------------------
        }


        #region Inspections RadGrid
        protected void rgInspections_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            rgInspections.DataSource = db.vCablewayInspectionSummaries.Select(p => new {
                site_no_nm = p.site_no + " " + p.station_nm,
                status = p.cableway_status_cd + " - " + p.cableway_status_desc,
                cableway_id = p.cableway_id,
                region_cd = p.region_cd,
                wsc_cd = p.wsc_cd,
                wsc_id = p.wsc_id,
                office_cd = p.office_cd,
                site_id = p.site_id,
                agency_cd = p.agency_cd,
                site_no = p.site_no,
                station_nm = p.station_nm,
                last_inspection_dt = p.last_inspection_dt,
                last_visit_dt = p.last_visit_dt,
                cableway_status_cd = p.cableway_status_cd,
                cableway_status_desc = p.cableway_status_desc,
                cableway_type_cd = p.cableway_type_cd,
                cableway_type_desc = p.cableway_type_desc,
                cableway_inspection_freq = p.cableway_inspection_freq,
                created_by = p.created_by,
                created_dt = p.created_dt,
                updated_by = p.updated_by,
                updated_dt = p.updated_dt,
                aerial_marker_inst = p.aerial_marker_inst,
                aerial_marker_req = p.aerial_marker_req
            }).OrderBy(p => p.region_cd).ThenBy(p => p.wsc_cd).ThenBy(p => p.site_no);
        }

        protected void rgInspections_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgInspections.FilterMenu;
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

        protected void rgInspections_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;

                int cableway_id = Convert.ToInt32(item.GetDataKeyValue("cableway_id"));
                var cw = db.vCablewayInspectionSummaries.FirstOrDefault(p => p.cableway_id == cableway_id);
                HyperLink hlSite = (HyperLink)item.FindControl("hlSite");
                string showr = "";

                if (cw.cableway_status_cd == "r")
                {
                    showr = "&showr=y";
                }

                hlSite.Attributes["target"] = "_blank";
                hlSite.Attributes["href"] = String.Format("Cableways.aspx?site_no={0}&agency_cd={1}{2}", cw.site_no, cw.agency_cd, showr);

            }
        }
        #endregion

        #region Status RadGrid
        protected void rgStatus_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            rgStatus.DataSource = db.vCablewayInspectionSummaries.Where(p => p.cableway_status_cd != "r" && p.cableway_status_cd != "ro" && p.cableway_status_cd != "d").Select(p => new {
                cableway_id = p.cableway_id,
                site_no = p.site_no,
                site_no_nm = p.site_no + " " + p.station_nm,
                last_inspection_dt = p.last_inspection_dt,
                next_inspection_dt = (p.last_inspection_dt != null) ? String.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(p.last_inspection_dt).AddYears(1)) : "",
                days_to_next = (p.last_inspection_dt != null) ? String.Format("{0}", (DateTime.Now - Convert.ToDateTime(p.last_inspection_dt)).Days) : "",
                cableway_inspection_freq = p.cableway_inspection_freq,
                status = p.cableway_status_cd + " - " + p.cableway_status_desc,
                wsc_id = p.wsc_id,
                office_cd = p.office_cd,
                region_cd = p.region_cd,
                wsc_cd = p.wsc_cd
            }).OrderBy(p => p.site_no);

            if ((!Page.IsPostBack))
            {
                try
                {
                    if (WSCID > 0)
                    {
                        var wsc_cd = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).wsc_cd;
                        rgStatus.MasterTableView.FilterExpression = "([wsc_cd] = '" + wsc_cd + "')";
                        GridColumn column = rgStatus.MasterTableView.GetColumnSafe("wsc_cd");
                        column.CurrentFilterFunction = GridKnownFunction.Contains;
                        column.CurrentFilterValue = wsc_cd;
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        protected void rgStatus_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgStatus.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" | menu.Items[i].Text == "Contains" | menu.Items[i].Text == "EqualTo" | menu.Items[i].Text == "DoesNotContain" | menu.Items[i].Text == "LessThan" | menu.Items[i].Text == "GreaterThan")
                {
                    i = i + 1;
                }
                else
                {
                    menu.Items.RemoveAt(i);
                }
            }
        }

        protected void rgStatus_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;

                int cableway_id = Convert.ToInt32(item.GetDataKeyValue("cableway_id"));
                var cw = db.vCablewayInspectionSummaries.FirstOrDefault(p => p.cableway_id == cableway_id);
                HyperLink hlSite = (HyperLink)item.FindControl("hlSite");
                string showr = "";

                if (cw.cableway_status_cd == "r")
                {
                    showr = "&showr=y";
                }

                hlSite.Attributes["target"] = "_blank";
                hlSite.Attributes["href"] = String.Format("Cableways.aspx?site_no={0}&agency_cd={1}{2}", cw.site_no, cw.agency_cd, showr);

                try
                {
                    int days_to_next = 0;
                    if (cw.last_inspection_dt != null)
                    {
                        days_to_next = (DateTime.Now - Convert.ToDateTime(cw.last_inspection_dt)).Days;
                    }

                    if (days_to_next < 0)
                    {
                        item["next_inspection_dt"].CssClass = "InspOverdue";
                    }
                    else if ((0 <= days_to_next) && (days_to_next <= 30))
                    {
                        item["next_inspection_dt"].CssClass = "InspWithin30days";
                    }
                    else if ((31 <= days_to_next) && (days_to_next <= 183))
                    {
                        item["next_inspection_dt"].CssClass = "InspWithin6mo";
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
        #endregion
    }
}