using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace RMS.Report
{
    public partial class PubStatus : System.Web.UI.Page
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
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            string office_id = Request.QueryString["office_id"];

            if (!string.IsNullOrEmpty(office_id))
                OfficeID = Convert.ToInt32(office_id);
            else
                OfficeID = user.OfficeID;

            WSCID = (int)db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id;

            if (!Page.IsPostBack)
            {
                UserControlSetup();
            }
        }

        #region Page Load Events
        protected void UserControlSetup()
        {
            string wsc_nm = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).wsc_nm;
            ph1.Title = "Current Publication Status";

            ph1.SubTitle = "For the " + wsc_nm + " WSC";
            ph1.RecordType = "&nbsp;";
        }
        #endregion

        #region Time-Series Status RadGrid
        protected void rgTSStatus_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            rgTSStatus.DataSource = db.SP_Publication_Status(WSCID, 0).OrderBy(p => p.office_cd).ThenBy(p => p.site_no);
        }

        protected void rgTSStatus_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgTSStatus.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" | menu.Items[i].Text == "Contains" | menu.Items[i].Text == "EqualTo" | menu.Items[i].Text == "DoesNotContain")
                    i = i + 1;
                else
                    menu.Items.RemoveAt(i);
            }
        }

        protected void rgTSStatus_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                int rms_record_id = Convert.ToInt32(item.GetDataKeyValue("rms_record_id"));
            }
        }
        #endregion

        #region Non-Time-Series Status RadGrid
        protected void rgNTSStatus_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var ras = db.RecordApprovalStatus.Select(p => new { 
                wsc_id = db.Sites.FirstOrDefault(s => s.site_id == p.site_id).Office.wsc_id, 
                office_cd = p.office_cd,
                site_no = p.site_no, 
                rms_record_id = p.rms_record_id,
                type_cd = p.type_cd, 
                parm_cd = p.parm_cd,
                station_full_nm = db.Sites.FirstOrDefault(s => s.site_id == p.site_id).station_full_nm,
                last_aging_dt = p.last_aging_dt
            });

            rgNTSStatus.DataSource = ras.Where(p => p.wsc_id == WSCID).OrderBy(p => p.office_cd).ThenBy(p => p.site_no);
        }

        protected void rgNTSStatus_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgNTSStatus.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" | menu.Items[i].Text == "Contains" | menu.Items[i].Text == "EqualTo" | menu.Items[i].Text == "DoesNotContain")
                    i = i + 1;
                else
                    menu.Items.RemoveAt(i);
            }
        }

        protected void rgNTSStatus_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                int rms_record_id = Convert.ToInt32(item.GetDataKeyValue("rms_record_id"));
            }
        }
        #endregion
    }
}