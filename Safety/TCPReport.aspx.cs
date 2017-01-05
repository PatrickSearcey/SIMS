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
    public partial class TCPReport : System.Web.UI.Page
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

            //If no site, office, or WSC passed, then do not apply any filter to the grid, and show the nationwide grid - show all TCPs for all regions
            if (OfficeID == 0)
            {
                ph1.Title = "Nationwide Traffic Control Plan Status Report";
                pnlNWReport.Visible = true;
                pnlWSCReport.Visible = false;
            }
            else
            {
                var wsc = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID);
                ph1.Title = "Traffic Control Plan Status Report";
                ph1.SubTitle = "Viewing TCPs for the " + wsc.wsc_nm + " WSC";
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
        internal class TCPDataItem
        {
            private string _site_no;
            private string _plan_no;
            private string _site_no_nm;
            private string _office_cd;
            private string _updated_by;
            private string _updated_dt;
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
            public string plan_no
            {
                get { return _plan_no; }
                set { _plan_no = value; }
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
            public string updated_by
            {
                get { return _updated_by; }
                set { _updated_by = value; }
            }
            public string updated_dt
            {
                get { return _updated_dt; }
                set { _updated_dt = value; }
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

            public TCPDataItem()
            {
                _site_no = site_no;
                _plan_no = plan_no;
                _site_no_nm = site_no_nm;
                _office_cd = office_cd;
                _updated_by = updated_by;
                _updated_dt = updated_dt;
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
        private List<TCPDataItem> StatusInfo
        {
            get
            {
                List<TCPDataItem> data = new List<TCPDataItem>();

                switch (grid_tp)
                {
                    case "currently approved":
                        data = (from p in db.vEval_FullSiteDiagnostics
                                join t in db.TCPs on p.site_id equals t.site_id
                                join w in db.WSCs on p.wsc_id equals w.wsc_id
                                where (t.UpdatedDt < t.ApprovedDt && t.ReviewedDt < t.ApprovedDt && t.ApprovedBy != "transfer") ||
                                (t.ApprovedDt != null && t.ApprovedBy != "transfer" && (t.UpdatedDt == null || t.ReviewedDt == null))
                                select new TCPDataItem()
                                {
                                    site_no = p.site_no,
                                    plan_no = t.TCPPlanDetail.Number,
                                    site_no_nm = p.site_no + " " + p.station_nm,
                                    office_cd = p.office_cd,
                                    updated_by = t.UpdatedBy,
                                    updated_dt = String.Format("{0:MM/dd/yyyy}", t.UpdatedDt),
                                    reviewed_by = t.ReviewedBy,
                                    reviewed_dt = String.Format("{0:MM/dd/yyyy}", t.ReviewedDt),
                                    approved_by = t.ApprovedBy,
                                    approved_dt = String.Format("{0:MM/dd/yyyy}", t.ApprovedDt),
                                    reviewer_comments = t.ReviewerComments.FormatParagraphOut(),
                                    wsc_id = w.wsc_id.ToString(),
                                    wsc_cd = w.wsc_cd,
                                    region_cd = w.region_cd,
                                    action = GetAction(t.UpdatedDt, t.ReviewedDt, t.ApprovedDt),
                                    site_tp_cd = p.site_tp_cd,
                                    agency_use_cd = p.agency_use_cd.ToString()
                                }).OrderBy(p => p.region_cd).ThenBy(p => p.wsc_id).ThenBy(p => p.office_cd).ThenBy(p => p.site_no).ToList();
                        break;
                    case "need review":
                        data = (from p in db.vEval_FullSiteDiagnostics
                                join t in db.TCPs on p.site_id equals t.site_id
                                join w in db.WSCs on p.wsc_id equals w.wsc_id
                                where (t.UpdatedDt > t.ReviewedDt) || (t.ReviewedDt == null) || (t.ReviewedBy.Equals("transfer"))
                                select new TCPDataItem()
                                {
                                    site_no = p.site_no,
                                    plan_no = t.TCPPlanDetail.Number,
                                    site_no_nm = p.site_no + " " + p.station_nm,
                                    office_cd = p.office_cd,
                                    updated_by = t.UpdatedBy,
                                    updated_dt = String.Format("{0:MM/dd/yyyy}", t.UpdatedDt),
                                    reviewed_by = t.ReviewedBy,
                                    reviewed_dt = String.Format("{0:MM/dd/yyyy}", t.ReviewedDt),
                                    approved_by = t.ApprovedBy,
                                    approved_dt = String.Format("{0:MM/dd/yyyy}", t.ApprovedDt),
                                    reviewer_comments = t.ReviewerComments.FormatParagraphOut(),
                                    wsc_id = w.wsc_id.ToString(),
                                    wsc_cd = w.wsc_cd,
                                    region_cd = w.region_cd,
                                    action = GetAction(t.UpdatedDt, t.ReviewedDt, t.ApprovedDt),
                                    site_tp_cd = p.site_tp_cd,
                                    agency_use_cd = p.agency_use_cd.ToString()
                                }).OrderBy(p => p.region_cd).ThenBy(p => p.wsc_id).ThenBy(p => p.office_cd).ThenBy(p => p.site_no).ToList();
                        break;
                    case "need approve":
                        data = (from p in db.vEval_FullSiteDiagnostics
                                join t in db.TCPs on p.site_id equals t.site_id
                                join w in db.WSCs on p.wsc_id equals w.wsc_id
                                where (t.ApprovalReady == true)
                                select new TCPDataItem()
                                {
                                    site_no = p.site_no,
                                    plan_no = t.TCPPlanDetail.Number,
                                    site_no_nm = p.site_no + " " + p.station_nm,
                                    office_cd = p.office_cd,
                                    updated_by = t.UpdatedBy,
                                    updated_dt = String.Format("{0:MM/dd/yyyy}", t.UpdatedDt),
                                    reviewed_by = t.ReviewedBy,
                                    reviewed_dt = String.Format("{0:MM/dd/yyyy}", t.ReviewedDt),
                                    approved_by = t.ApprovedBy,
                                    approved_dt = String.Format("{0:MM/dd/yyyy}", t.ApprovedDt),
                                    reviewer_comments = t.ReviewerComments.FormatParagraphOut(),
                                    wsc_id = w.wsc_id.ToString(),
                                    wsc_cd = w.wsc_cd,
                                    region_cd = w.region_cd,
                                    action = GetAction(t.UpdatedDt, t.ReviewedDt, t.ApprovedDt),
                                    site_tp_cd = p.site_tp_cd,
                                    agency_use_cd = p.agency_use_cd.ToString()
                                }).OrderBy(p => p.region_cd).ThenBy(p => p.wsc_id).ThenBy(p => p.office_cd).ThenBy(p => p.site_no).ToList();
                        break;
                    case "sites no TCP":
                        data = (from p in db.vEval_FullSiteDiagnostics
                                join w in db.WSCs on p.wsc_id equals w.wsc_id
                                join t in db.TCPs on p.site_id equals t.site_id into sr
                                from x in sr.DefaultIfEmpty()
                                where (x.TCPID == null)
                                select new TCPDataItem()
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

                List<TCPDataItem> ret = new List<TCPDataItem>();

                if (activeonly == 2)
                    ret = data.Where(p => !p.agency_use_cd.ToString().Equals("M") && !p.agency_use_cd.ToString().Equals("A") && !p.agency_use_cd.ToString().Equals("L")).ToList();
                else
                    ret = data.Where(p => p.agency_use_cd.ToString().Equals("M") || p.agency_use_cd.ToString().Equals("A") || p.agency_use_cd.ToString().Equals("L")).ToList();

                if (WSCID > 0)
                    ret = ret.Where(p => p.wsc_id == WSCID.ToString()).ToList();

                return ret;
            }
        }

        private string GetAction(DateTime? updated_dt, DateTime? reviewed_dt, DateTime? approved_dt)
        {
            string ret = "";

            if (updated_dt != null)
            {
                if (updated_dt > reviewed_dt && updated_dt > approved_dt)
                    ret = "Review";
                else
                    if (reviewed_dt > approved_dt) ret = "Approve"; else ret = "View";
            }
            else
                ret = "View";

            return ret;
        }
        #endregion

        #region Nationwide Currently Approved TCP RadGrid
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
                hlSite.Attributes["href"] = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMS2017URL, site.site_id);

                HyperLink hlAction = (HyperLink)item.FindControl("hlAction");

                hlAction.Attributes["target"] = "_blank";
                hlAction.Attributes["href"] = String.Format("{0}TCPEdit.aspx?site_id={1}", Config.SafetyURL, site.site_id);

                hlAction.Text = "View";
            }
        }
        #endregion

        #region Nationwide TCPs Requiring Review RadGrid
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
                hlSite.Attributes["href"] = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMS2017URL, site.site_id);

                HyperLink hlAction = (HyperLink)item.FindControl("hlAction");

                hlAction.Attributes["target"] = "_blank";
                hlAction.Attributes["href"] = String.Format("{0}TCPEdit.aspx?site_id={1}", Config.SafetyURL, site.site_id);

                hlAction.Text = "View";
            }
        }
        #endregion

        #region Nationwide TCPs Requiring Approval RadGrid
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
                hlSite.Attributes["href"] = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMS2017URL, site.site_id);

                HyperLink hlAction = (HyperLink)item.FindControl("hlAction");

                hlAction.Attributes["target"] = "_blank";
                hlAction.Attributes["href"] = String.Format("{0}TCPEdit.aspx?site_id={1}", Config.SafetyURL, site.site_id);

                hlAction.Text = "View";
            }
        }
        #endregion

        #region Nationwide Sites with no TCP RadGrid
        protected void rgNWSitesNoTCP_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grid_tp = "sites no TCP";
            activeonly = Convert.ToInt32(Session["activeonlyNW4"]);

            rgNWSitesNoTCP.DataSource = StatusInfo;
        }

        //Specify which items appear in FilterMenu
        protected void rgNWSitesNoTCP_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgNWSitesNoTCP.FilterMenu;
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
        protected void rgNWSitesNoTCP_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;

                string site_no = DataBinder.Eval(item.DataItem, "site_no").ToString();
                var site = db.Sites.FirstOrDefault(p => p.site_no == site_no);

                HyperLink hlSite = (HyperLink)item.FindControl("hlSite");

                hlSite.Attributes["target"] = "_blank";
                hlSite.Attributes["href"] = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMS2017URL, site.site_id);
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
                hlSite.Attributes["href"] = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMS2017URL, site.site_id);

                HyperLink hlAction = (HyperLink)item.FindControl("hlAction");

                hlAction.Attributes["target"] = "_blank";
                hlAction.Attributes["href"] = String.Format("{0}TCPEdit.aspx?site_id={1}", Config.SafetyURL, site.site_id);

                if (Session["showapprove"].ToString() == "false" && hlAction.Text == "Approve")
                {
                    hlAction.Text = "View";
                }
            }
        }
        #endregion

        #region TCPs Requiring Review RadGrid
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
                hlSite.Attributes["href"] = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMS2017URL, site.site_id);

                HyperLink hlAction = (HyperLink)item.FindControl("hlAction");

                hlAction.Attributes["target"] = "_blank";
                hlAction.Attributes["href"] = String.Format("{0}TCPEdit.aspx?site_id={1}", Config.SafetyURL, site.site_id);

                hlAction.Text = "Review";

            }
        }
        #endregion

        #region TCPs Requiring Approval RadGrid
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
                hlSite.Attributes["href"] = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMS2017URL, site.site_id);

                HyperLink hlAction = (HyperLink)item.FindControl("hlAction");

                hlAction.Attributes["target"] = "_blank";
                hlAction.Attributes["href"] = String.Format("{0}TCPEdit.aspx?site_id={1}", Config.SafetyURL, site.site_id);

                if (Session["showapprove"].ToString() == "false")
                    hlAction.Text = "View";
                else
                    hlAction.Text = "Approve";
            }
        }
        #endregion

        #region Sites with no TCP RadGrid
        protected void rgSitesNoTCP_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grid_tp = "sites no TCP";
            activeonly = Convert.ToInt32(Session["activeonly4"]);

            rgSitesNoTCP.DataSource = StatusInfo;

            if ((!Page.IsPostBack))
            {
                try
                {
                    string office_cd = db.Offices.FirstOrDefault(p => p.office_id == OfficeID).office_cd;
                    rgSitesNoTCP.MasterTableView.FilterExpression = "([office_cd] Like '%" + office_cd + "%')";
                    GridColumn column = rgSitesNoTCP.MasterTableView.GetColumnSafe("office_cd");
                    column.CurrentFilterFunction = GridKnownFunction.Contains;
                    column.CurrentFilterValue = office_cd;
                }
                catch (Exception ex) { }
            }
        }

        //Specify which items appear in FilterMenu
        protected void rgSitesNoTCP_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgSitesNoTCP.FilterMenu;
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
        protected void rgSitesNoTCP_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;

                string site_no = DataBinder.Eval(item.DataItem, "site_no").ToString();
                var site = db.Sites.FirstOrDefault(p => p.site_no == site_no);

                HyperLink hlSite = (HyperLink)item.FindControl("hlSite");

                hlSite.Attributes["target"] = "_blank";
                hlSite.Attributes["href"] = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMS2017URL, site.site_id);

                HyperLink hlAction = (HyperLink)item.FindControl("hlAction");

                hlAction.Attributes["target"] = "_blank";
                hlAction.Attributes["href"] = String.Format("{0}TCPEdit.aspx?site_id={1}", Config.SafetyURL, site.site_id);
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
                    rgNWSitesNoTCP.Rebind();
                    break;
                case "viewActiveNW4":
                    Session["activeonlyNW4"] = 1;
                    lbActiveSiteToggleNW4.CommandArgument = "viewInactiveNW4";
                    lbActiveSiteToggleNW4.Text = "Click to view inactive sites";
                    ltlActiveSiteToggleNW4.Text = "Viewing active sites only";
                    rgNWSitesNoTCP.Rebind();
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
                    rgSitesNoTCP.Rebind();
                    break;
                case "viewActive4":
                    Session["activeonly4"] = 1;
                    lbActiveSiteToggle4.CommandArgument = "viewInactive4";
                    lbActiveSiteToggle4.Text = "Click to view inactive sites";
                    ltlActiveSiteToggle4.Text = "Viewing active sites only";
                    rgSitesNoTCP.Rebind();
                    break;
            }
        }
        #endregion
    }
}