using Core;
using System;
using System.Collections;
using Safety.Control;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Safety
{
    public partial class SHAReport : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private string grid_tp;
        private int activeonly;
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
            //Grab the office_id - if none was passed, then display the Nationwide Report
            string office_id = Request.QueryString["office_id"];
            if (!string.IsNullOrEmpty(office_id))
            {
                OfficeID = Convert.ToInt32(office_id);

                //Using the passed office_id, reset the wsc to match that of the current office
                WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;
            }
            else
            {
                OfficeID = 0;
                WSCID = 0;
            }

            //--BASIC PAGE SETUP--------------------------------------------------------------------
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //If no site, office, or WSC passed, then do not apply any filter to the grid, and show the nationwide grid - show all SHAs for all regions
            if (OfficeID == 0)
            {
                ph1.Title = "Nationwide Site Hazard Analysis Status Report";
                pnlNWReport.Visible = true;
                pnlWSCReport.Visible = false;
            }
            else
            {
                var wsc = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID);
                ph1.Title = "Site Hazard Analysis Status Report";
                ph1.SubTitle = "Viewing SHAs for the " + wsc.wsc_nm + " WSC";
                pnlNWReport.Visible = false;
                pnlWSCReport.Visible = true;
            }

            if (!Page.IsPostBack)
            {
                //Clear session state variables
                ResetSessionStateVariables();

                //--APPROVER ACCESS SECTION-------------------------------------------------------------
                if (user.IsSuperUser || user.WSCID.Contains(WSCID) && user.IsSafetyApprover) 
                    Session["showapprove"] = "true";
                else
                    Session["showapprove"] = "false";
                //--END PAGE ACCESS SECTION---------------------------------------------------------
            }
        }

        #region Internal Classes
        internal class SHADataItem
        {
            private string _site_no;
            private string _site_no_nm;
            private string _office_cd;
            private string _reviewed_by;
            private string _reviewed_dt;
            private string _approved_by;
            private string _approved_dt;
            private string _reviewer_comments;
            private string _wsc_id;
            private string _wsc_cd;
            private string _region_cd;
            private string _action;
            private string _site_tp_cd;
            private string _agency_use_cd;

            public string site_no
            {
                get { return _site_no; }
                set { _site_no = value; }
            }
            public string site_no_nm
            {
                get { return _site_no_nm; }
                set { _site_no_nm = value; }
            }
            public string office_cd
            {
                get { return _office_cd; }
                set { _office_cd = value; }
            }
            public string reviewed_by
            {
                get { return _reviewed_by; }
                set { _reviewed_by = value; }
            }
            public string reviewed_dt
            {
                get { return _reviewed_dt; }
                set { _reviewed_dt = value; }
            }
            public string approved_by
            {
                get { return _approved_by; }
                set { _approved_by = value; }
            }
            public string approved_dt
            {
                get { return _approved_dt; }
                set { _approved_dt = value; }
            }
            public string reviewer_comments
            {
                get { return _reviewer_comments; }
                set { _reviewer_comments = value; }
            }
            public string wsc_id
            {
                get { return _wsc_id; }
                set { _wsc_id = value; }
            }
            public string wsc_cd
            {
                get { return _wsc_cd; }
                set { _wsc_cd = value; }
            }
            public string region_cd
            {
                get { return _region_cd; }
                set { _region_cd = value; }
            }
            public string action
            {
                get { return _action; }
                set { _action = value; }
            }
            public string site_tp_cd
            {
                get { return _site_tp_cd; }
                set { _site_tp_cd = value; }
            }
            public string agency_use_cd
            {
                get { return _agency_use_cd; }
                set { _agency_use_cd = value; }
            }
            
            public SHADataItem()
            {
                _site_no = site_no;
                _site_no_nm = site_no_nm;
                _office_cd = office_cd;
                _reviewed_by = reviewed_by;
                _reviewed_dt = reviewed_dt;
                _approved_by = approved_by;
                _approved_dt = approved_dt;
                _reviewer_comments = reviewer_comments;
                _wsc_id = wsc_id;
                _wsc_cd = wsc_cd;
                _region_cd = region_cd;
                _action = action;
                _site_tp_cd = site_tp_cd;
                _agency_use_cd = agency_use_cd;
            }
        }
        #endregion

        #region Data
        private List<SHADataItem> StatusInfo
        {
            get
            {
                List<SHADataItem> data = new List<SHADataItem>();
                DateTime oneYearAgo = DateTime.Now.AddDays(-365);

                switch (grid_tp)
                {
                    case "currently approved":
                        data = (from p in db.vEval_FullSiteDiagnostics
                                join s in db.SHAs on p.site_id equals s.site_id
                                join w in db.WSCs on p.wsc_id equals w.wsc_id
                                where (s.reviewed_dt > oneYearAgo && s.reviewed_dt < s.approved_dt && s.approved_by != "transfer") ||
                                (s.approved_dt != null && s.approved_by != "transfer" && (s.reviewed_dt == null))
                                select new SHADataItem()
                                {
                                    site_no = p.site_no,
                                    site_no_nm = p.site_no + " " + p.station_nm,
                                    office_cd = p.office_cd,
                                    reviewed_by = s.reviewed_by,
                                    reviewed_dt = String.Format("{0:MM/dd/yyyy}", s.reviewed_dt),
                                    approved_by = s.approved_by,
                                    approved_dt = String.Format("{0:MM/dd/yyyy}", s.approved_dt),
                                    reviewer_comments = s.reviewer_comments.FormatParagraphOut(),
                                    wsc_id = w.wsc_id.ToString(),
                                    wsc_cd = w.wsc_cd,
                                    region_cd = w.region_cd,
                                    action = GetAction(s.reviewed_dt, s.approved_dt),
                                    site_tp_cd = p.site_tp_cd,
                                    agency_use_cd = p.agency_use_cd.ToString()
                                }).OrderBy(p => p.region_cd).ThenBy(p => p.wsc_id).ThenBy(p => p.office_cd).ThenBy(p => p.site_no).ToList();
                        break;
                    case "need review":
                        data = (from p in db.vEval_FullSiteDiagnostics
                                join s in db.SHAs on p.site_id equals s.site_id
                                join w in db.WSCs on p.wsc_id equals w.wsc_id
                                where (s.reviewed_dt == null) || (s.reviewed_by.Equals("transfer") || (s.reviewed_dt < oneYearAgo))
                                select new SHADataItem()
                                {
                                    site_no = p.site_no,
                                    site_no_nm = p.site_no + " " + p.station_nm,
                                    office_cd = p.office_cd,
                                    reviewed_by = s.reviewed_by,
                                    reviewed_dt = String.Format("{0:MM/dd/yyyy}", s.reviewed_dt),
                                    approved_by = s.approved_by,
                                    approved_dt = String.Format("{0:MM/dd/yyyy}", s.approved_dt),
                                    reviewer_comments = s.reviewer_comments.FormatParagraphOut(),
                                    wsc_id = w.wsc_id.ToString(),
                                    wsc_cd = w.wsc_cd,
                                    region_cd = w.region_cd,
                                    action = GetAction(s.reviewed_dt, s.approved_dt),
                                    site_tp_cd = p.site_tp_cd,
                                    agency_use_cd = p.agency_use_cd.ToString()
                                }).OrderBy(p => p.region_cd).ThenBy(p => p.wsc_id).ThenBy(p => p.office_cd).ThenBy(p => p.site_no).ToList();
                        break;
                    case "need approve":
                        data = (from p in db.vEval_FullSiteDiagnostics
                                join s in db.SHAs on p.site_id equals s.site_id
                                join w in db.WSCs on p.wsc_id equals w.wsc_id
                                where (s.reviewed_dt > s.approved_dt) || (s.approved_dt == null) ||
                                (s.reviewed_dt > s.approved_dt) || (s.reviewed_dt != null && s.approved_dt == null)
                                select new SHADataItem()
                                {
                                    site_no = p.site_no,
                                    site_no_nm = p.site_no + " " + p.station_nm,
                                    office_cd = p.office_cd,
                                    reviewed_by = s.reviewed_by,
                                    reviewed_dt = String.Format("{0:MM/dd/yyyy}", s.reviewed_dt),
                                    approved_by = s.approved_by,
                                    approved_dt = String.Format("{0:MM/dd/yyyy}", s.approved_dt),
                                    reviewer_comments = s.reviewer_comments.FormatParagraphOut(),
                                    wsc_id = w.wsc_id.ToString(),
                                    wsc_cd = w.wsc_cd,
                                    region_cd = w.region_cd,
                                    action = GetAction(s.reviewed_dt, s.approved_dt),
                                    site_tp_cd = p.site_tp_cd,
                                    agency_use_cd = p.agency_use_cd.ToString()
                                }).OrderBy(p => p.region_cd).ThenBy(p => p.wsc_id).ThenBy(p => p.office_cd).ThenBy(p => p.site_no).ToList();
                        break;
                    case "sites no SHA":
                        data = (from p in db.vEval_FullSiteDiagnostics
                                join w in db.WSCs on p.wsc_id equals w.wsc_id
                                join s in db.SHAs on p.site_id equals s.site_id into sr
                                from x in sr.DefaultIfEmpty()
                                where (x.sha_site_id == null)
                                select new SHADataItem()
                                {
                                    site_no = p.site_no,
                                    site_no_nm = p.site_no + " " + p.station_nm,
                                    office_cd = p.office_cd,
                                    wsc_id = w.wsc_id.ToString(),
                                    wsc_cd = w.wsc_cd,
                                    region_cd = w.region_cd,
                                    action = "Create",
                                    site_tp_cd = p.site_tp_cd,
                                    agency_use_cd = p.agency_use_cd.ToString()
                                }).OrderBy(p => p.region_cd).ThenBy(p => p.wsc_id).ThenBy(p => p.office_cd).ThenBy(p => p.site_no).ToList();
                        break;
                }

                List<SHADataItem> ret = new List<SHADataItem>();

                if (activeonly == 2)
                    ret = data.Where(p => !p.agency_use_cd.ToString().Equals("M") && !p.agency_use_cd.ToString().Equals("A") && !p.agency_use_cd.ToString().Equals("L")).ToList();
                else
                    ret = data.Where(p => p.agency_use_cd.ToString().Equals("M") || p.agency_use_cd.ToString().Equals("A") || p.agency_use_cd.ToString().Equals("L")).ToList();

                if (WSCID > 0)
                    ret = ret.Where(p => p.wsc_id == WSCID.ToString()).ToList();

                return ret;
            }
        }

        private string GetAction(DateTime? reviewed_dt, DateTime? approved_dt)
        {
            string ret = "";

            if (reviewed_dt != null)
            {
                if (reviewed_dt > approved_dt) ret = "Approve"; else ret = "Review";
            }
            else
                ret = "Review";

            return ret;
        }
        #endregion

        #region Nationwide Currently Approved SHA RadGrid
        protected void rgNWStatus_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grid_tp = "currently approved";
            activeonly = Convert.ToInt32(Session["activeonlyNW1"]);

            rgNWStatus.DataSource = StatusInfo;
        }

        //Specify which items appear in FilterMenu
        protected void rgNWStatus_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgNWStatus.FilterMenu;
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

        //Setup row background colors and in-grid links
        protected void rgNWStatus_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;

                string site_no = DataBinder.Eval(item.DataItem, "site_no").ToString();
                var site = db.Sites.FirstOrDefault(p => p.site_no == site_no);

                HyperLink hlSite = (HyperLink)item.FindControl("hlSite");

                hlSite.Attributes["target"] = "_blank";
                hlSite.Attributes["href"] = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMSURL, site.site_id);

                HyperLink hlAction = (HyperLink)item.FindControl("hlAction");

                hlAction.Attributes["target"] = "_blank";
                hlAction.Attributes["href"] = String.Format("{0}SHAEdit.aspx?site_id={1}", Config.SafetyURL, site.site_id);

                hlAction.Text = "View";
            }
        }
        #endregion

        #region Nationwide SHAs Requiring Review RadGrid
        protected void rgNWReview_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grid_tp = "need review";
            activeonly = Convert.ToInt32(Session["activeonlyNW2"]);

            rgNWReview.DataSource = StatusInfo;
        }

        //Specify which items appear in FilterMenu
        protected void rgNWReview_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgNWReview.FilterMenu;
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

        //Setup row background colors and in-grid links
        protected void rgNWReview_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;

                string site_no = DataBinder.Eval(item.DataItem, "site_no").ToString();
                var site = db.Sites.FirstOrDefault(p => p.site_no == site_no);

                HyperLink hlSite = (HyperLink)item.FindControl("hlSite");

                hlSite.Attributes["target"] = "_blank";
                hlSite.Attributes["href"] = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMSURL, site.site_id);

                HyperLink hlAction = (HyperLink)item.FindControl("hlAction");

                hlAction.Attributes["target"] = "_blank";
                hlAction.Attributes["href"] = String.Format("{0}SHAEdit.aspx?site_id={1}", Config.SafetyURL, site.site_id);

                hlAction.Text = "View";
            }
        }
        #endregion

        #region Nationwide SHAs Requiring Approval RadGrid
        protected void rgNWApprove_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grid_tp = "need approve";
            activeonly = Convert.ToInt32(Session["activeonlyNW3"]);

            rgNWApprove.DataSource = StatusInfo;
        }

        //Specify which items appear in FilterMenu
        protected void rgNWApprove_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgNWApprove.FilterMenu;
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

        //Setup row background colors and in-grid links
        protected void rgNWApprove_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;

                string site_no = DataBinder.Eval(item.DataItem, "site_no").ToString();
                var site = db.Sites.FirstOrDefault(p => p.site_no == site_no);

                HyperLink hlSite = (HyperLink)item.FindControl("hlSite");

                hlSite.Attributes["target"] = "_blank";
                hlSite.Attributes["href"] = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMSURL, site.site_id);

                HyperLink hlAction = (HyperLink)item.FindControl("hlAction");

                hlAction.Attributes["target"] = "_blank";
                hlAction.Attributes["href"] = String.Format("{0}SHAEdit.aspx?site_id={1}", Config.SafetyURL, site.site_id);

                hlAction.Text = "View";
            }
        }
        #endregion

        #region Nationwide Sites with no SHA RadGrid
        protected void rgNWSitesNoSHA_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grid_tp = "sites no SHA";
            activeonly = Convert.ToInt32(Session["activeonlyNW4"]);

            rgNWSitesNoSHA.DataSource = StatusInfo;
        }

        //Specify which items appear in FilterMenu
        protected void rgNWSitesNoSHA_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgNWSitesNoSHA.FilterMenu;
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

        //Setup row background colors and in-grid links
        protected void rgNWSitesNoSHA_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;

                string site_no = DataBinder.Eval(item.DataItem, "site_no").ToString();
                var site = db.Sites.FirstOrDefault(p => p.site_no == site_no);

                HyperLink hlSite = (HyperLink)item.FindControl("hlSite");

                hlSite.Attributes["target"] = "_blank";
                hlSite.Attributes["href"] = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMSURL, site.site_id);
            }
        }
        #endregion

        #region Status RadGrid
        protected void rgStatus_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grid_tp = "currently approved";
            activeonly = Convert.ToInt32(Session["activeonly1"]);

            rgStatus.DataSource = StatusInfo;

            if ((!Page.IsPostBack))
            {
                try
                {
                    string office_cd = db.Offices.FirstOrDefault(p => p.office_id == OfficeID).office_cd;
                    rgStatus.MasterTableView.FilterExpression = "([office_cd] Like '%" + office_cd + "%')";
                    GridColumn column = rgStatus.MasterTableView.GetColumnSafe("office_cd");
                    column.CurrentFilterFunction = GridKnownFunction.Contains;
                    column.CurrentFilterValue = office_cd;
                }
                catch (Exception ex) { }
            }
        }

        protected void rgStatus_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgStatus.FilterMenu;
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

        protected void rgStatus_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;

                string site_no = DataBinder.Eval(item.DataItem, "site_no").ToString();
                var site = db.Sites.FirstOrDefault(p => p.site_no == site_no);

                HyperLink hlSite = (HyperLink)item.FindControl("hlSite");

                hlSite.Attributes["target"] = "_blank";
                hlSite.Attributes["href"] = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMSURL, site.site_id);

                HyperLink hlAction = (HyperLink)item.FindControl("hlAction");

                hlAction.Attributes["target"] = "_blank";
                hlAction.Attributes["href"] = String.Format("{0}SHAEdit.aspx?site_id={1}", Config.SafetyURL, site.site_id);

                if (Session["showapprove"].ToString() == "false" && hlAction.Text == "Approve")
                {
                    hlAction.Text = "View";
                }
            }
        }
        #endregion

        #region SHAs Requiring Review RadGrid
        protected void rgReview_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grid_tp = "need review";
            activeonly = Convert.ToInt32(Session["activeonly2"]);

            rgReview.DataSource = StatusInfo;

            if ((!Page.IsPostBack))
            {
                try
                {
                    string office_cd = db.Offices.FirstOrDefault(p => p.office_id == OfficeID).office_cd;
                    rgReview.MasterTableView.FilterExpression = "([office_cd] Like '%" + office_cd + "%')";
                    GridColumn column = rgReview.MasterTableView.GetColumnSafe("office_cd");
                    column.CurrentFilterFunction = GridKnownFunction.Contains;
                    column.CurrentFilterValue = office_cd;
                }
                catch (Exception ex)
                {
                }
            }
        }

        //Specify which items appear in FilterMenu
        protected void rgReview_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgReview.FilterMenu;
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

        //Setup row background colors and in-grid links
        protected void rgReview_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;

                string site_no = DataBinder.Eval(item.DataItem, "site_no").ToString();
                var site = db.Sites.FirstOrDefault(p => p.site_no == site_no);

                HyperLink hlSite = (HyperLink)item.FindControl("hlSite");

                hlSite.Attributes["target"] = "_blank";
                hlSite.Attributes["href"] = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMSURL, site.site_id);

                HyperLink hlAction = (HyperLink)item.FindControl("hlAction");

                hlAction.Attributes["target"] = "_blank";
                hlAction.Attributes["href"] = String.Format("{0}SHAEdit.aspx?site_id={1}", Config.SafetyURL, site.site_id);

                hlAction.Text = "Review";

            }
        }
        #endregion

        #region SHAs Requiring Approval RadGrid
        protected void rgApprove_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grid_tp = "need approve";
            activeonly = Convert.ToInt32(Session["activeonly3"]);

            rgApprove.DataSource = StatusInfo;

            if ((!Page.IsPostBack))
            {
                try
                {
                    string office_cd = db.Offices.FirstOrDefault(p => p.office_id == OfficeID).office_cd;
                    rgApprove.MasterTableView.FilterExpression = "([office_cd] Like '%" + office_cd + "%')";
                    GridColumn column = rgApprove.MasterTableView.GetColumnSafe("office_cd");
                    column.CurrentFilterFunction = GridKnownFunction.Contains;
                    column.CurrentFilterValue = office_cd;
                }
                catch (Exception ex)
                {
                }
            }
        }

        //Specify which items appear in FilterMenu
        protected void rgApprove_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgApprove.FilterMenu;
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

        //Setup row background colors and in-grid links
        protected void rgApprove_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;

                string site_no = DataBinder.Eval(item.DataItem, "site_no").ToString();
                var site = db.Sites.FirstOrDefault(p => p.site_no == site_no);

                HyperLink hlSite = (HyperLink)item.FindControl("hlSite");

                hlSite.Attributes["target"] = "_blank";
                hlSite.Attributes["href"] = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMSURL, site.site_id);

                HyperLink hlAction = (HyperLink)item.FindControl("hlAction");

                hlAction.Attributes["target"] = "_blank";
                hlAction.Attributes["href"] = String.Format("{0}SHAEdit.aspx?site_id={1}", Config.SafetyURL, site.site_id);

                if (Session["showapprove"].ToString() == "false")
                    hlAction.Text = "View";
                else
                    hlAction.Text = "Approve";
            }
        }
        #endregion

        #region Sites with no SHA RadGrid
        protected void rgSitesNoSHA_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grid_tp = "sites no SHA";
            activeonly = Convert.ToInt32(Session["activeonly4"]);

            rgSitesNoSHA.DataSource = StatusInfo;

            if ((!Page.IsPostBack))
            {
                try
                {
                    string office_cd = db.Offices.FirstOrDefault(p => p.office_id == OfficeID).office_cd;
                    rgSitesNoSHA.MasterTableView.FilterExpression = "([office_cd] Like '%" + office_cd + "%')";
                    GridColumn column = rgSitesNoSHA.MasterTableView.GetColumnSafe("office_cd");
                    column.CurrentFilterFunction = GridKnownFunction.Contains;
                    column.CurrentFilterValue = office_cd;
                }
                catch (Exception ex) { }
            }
        }

        //Specify which items appear in FilterMenu
        protected void rgSitesNoSHA_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgSitesNoSHA.FilterMenu;
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

        //Setup in-grid links
        protected void rgSitesNoSHA_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;

                string site_no = DataBinder.Eval(item.DataItem, "site_no").ToString();
                var site = db.Sites.FirstOrDefault(p => p.site_no == site_no);

                HyperLink hlSite = (HyperLink)item.FindControl("hlSite");

                hlSite.Attributes["target"] = "_blank";
                hlSite.Attributes["href"] = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMSURL, site.site_id);

                HyperLink hlAction = (HyperLink)item.FindControl("hlAction");

                hlAction.Attributes["target"] = "_blank";
                hlAction.Attributes["href"] = String.Format("{0}Handler/CreateSHA.ashx?site_id={1}", Config.SafetyURL, site.site_id);
            }
        }
        #endregion

        #region Session Variable Methods
        protected void ResetSessionStateVariables()
        {
            Session["showapprove"] = "";
            Session["activeonlyNW1"] = "1";
            Session["activeonlyNW2"] = "1";
            Session["activeonlyNW3"] = "1";
            Session["activeonlyNW4"] = "1";
            Session["activeonly1"] = "1";
            Session["activeonly2"] = "1";
            Session["activeonly3"] = "1";
            Session["activeonly4"] = "1";
        }

        public void lbActiveSiteToggle_Command(object sender, CommandEventArgs e)
        {
            switch (e.CommandArgument.ToString())
            {
                case "viewInactiveNW1":
                    Session["activeonlyNW1"] = 2;
                    lbActiveSiteToggleNW1.CommandArgument = "viewActiveNW1";
                    lbActiveSiteToggleNW1.Text = "Click to view active sites";
                    ltlActiveSiteToggleNW1.Text = "Viewing inactive sites only";
                    rgNWStatus.Rebind();
                    break;
                case "viewActiveNW1":
                    Session["activeonlyNW1"] = 1;
                    lbActiveSiteToggleNW1.CommandArgument = "viewInactiveNW1";
                    lbActiveSiteToggleNW1.Text = "Click to view inactive sites";
                    ltlActiveSiteToggleNW1.Text = "Viewing active sites only";
                    rgNWStatus.Rebind();
                    break;
                case "viewInactiveNW2":
                    Session["activeonlyNW2"] = 2;
                    lbActiveSiteToggleNW2.CommandArgument = "viewActiveNW2";
                    lbActiveSiteToggleNW2.Text = "Click to view active sites";
                    ltlActiveSiteToggleNW2.Text = "Viewing inactive sites only";
                    rgNWReview.Rebind();
                    break;
                case "viewActiveNW2":
                    Session["activeonlyNW2"] = 1;
                    lbActiveSiteToggleNW2.CommandArgument = "viewInactiveNW2";
                    lbActiveSiteToggleNW2.Text = "Click to view inactive sites";
                    ltlActiveSiteToggleNW2.Text = "Viewing active sites only";
                    rgNWReview.Rebind();
                    break;
                case "viewInactiveNW3":
                    Session["activeonlyNW3"] = 2;
                    lbActiveSiteToggleNW3.CommandArgument = "viewActiveNW3";
                    lbActiveSiteToggleNW3.Text = "Click to view active sites";
                    ltlActiveSiteToggleNW3.Text = "Viewing inactive sites only";
                    rgNWApprove.Rebind();
                    break;
                case "viewActiveNW3":
                    Session["activeonlyNW3"] = 1;
                    lbActiveSiteToggleNW3.CommandArgument = "viewInactiveNW3";
                    lbActiveSiteToggleNW3.Text = "Click to view inactive sites";
                    ltlActiveSiteToggleNW3.Text = "Viewing active sites only";
                    rgNWApprove.Rebind();
                    break;
                case "viewInactiveNW4":
                    Session["activeonlyNW4"] = 2;
                    lbActiveSiteToggleNW4.CommandArgument = "viewActiveNW4";
                    lbActiveSiteToggleNW4.Text = "Click to view active sites";
                    ltlActiveSiteToggleNW4.Text = "Viewing inactive sites only";
                    rgNWSitesNoSHA.Rebind();
                    break;
                case "viewActiveNW4":
                    Session["activeonlyNW4"] = 1;
                    lbActiveSiteToggleNW4.CommandArgument = "viewInactiveNW4";
                    lbActiveSiteToggleNW4.Text = "Click to view inactive sites";
                    ltlActiveSiteToggleNW4.Text = "Viewing active sites only";
                    rgNWSitesNoSHA.Rebind();
                    break;
                case "viewInactive1":
                    Session["activeonly1"] = 2;
                    lbActiveSiteToggle1.CommandArgument = "viewActive1";
                    lbActiveSiteToggle1.Text = "Click to view active sites";
                    ltlActiveSiteToggle1.Text = "Viewing inactive sites only";
                    rgStatus.Rebind();
                    break;
                case "viewActive1":
                    Session["activeonly1"] = 1;
                    lbActiveSiteToggle1.CommandArgument = "viewInactive1";
                    lbActiveSiteToggle1.Text = "Click to view inactive sites";
                    ltlActiveSiteToggle1.Text = "Viewing active sites only";
                    rgStatus.Rebind();
                    break;
                case "viewInactive2":
                    Session["activeonly2"] = 2;
                    lbActiveSiteToggle2.CommandArgument = "viewActive2";
                    lbActiveSiteToggle2.Text = "Click to view active sites";
                    ltlActiveSiteToggle2.Text = "Viewing inactive sites only";
                    rgReview.Rebind();
                    break;
                case "viewActive2":
                    Session["activeonly2"] = 1;
                    lbActiveSiteToggle2.CommandArgument = "viewInactive2";
                    lbActiveSiteToggle2.Text = "Click to view inactive sites";
                    ltlActiveSiteToggle2.Text = "Viewing active sites only";
                    rgReview.Rebind();
                    break;
                case "viewInactive3":
                    Session["activeonly3"] = 2;
                    lbActiveSiteToggle3.CommandArgument = "viewActive3";
                    lbActiveSiteToggle3.Text = "Click to view active sites";
                    ltlActiveSiteToggle3.Text = "Viewing inactive sites only";
                    rgApprove.Rebind();
                    break;
                case "viewActive3":
                    Session["activeonly3"] = 1;
                    lbActiveSiteToggle3.CommandArgument = "viewInactive3";
                    lbActiveSiteToggle3.Text = "Click to view inactive sites";
                    ltlActiveSiteToggle3.Text = "Viewing active sites only";
                    rgApprove.Rebind();
                    break;
                case "viewInactive4":
                    Session["activeonly4"] = 2;
                    lbActiveSiteToggle4.CommandArgument = "viewActive4";
                    lbActiveSiteToggle4.Text = "Click to view active sites";
                    ltlActiveSiteToggle4.Text = "Viewing inactive sites only";
                    rgSitesNoSHA.Rebind();
                    break;
                case "viewActive4":
                    Session["activeonly4"] = 1;
                    lbActiveSiteToggle4.CommandArgument = "viewInactive4";
                    lbActiveSiteToggle4.Text = "Click to view inactive sites";
                    ltlActiveSiteToggle4.Text = "Viewing active sites only";
                    rgSitesNoSHA.Rebind();
                    break;
            }
        }
        #endregion
    }
}
