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
    public partial class CRPStatus : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private int reportOfficeID { get; set; }
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
            string wsc_id = Request.QueryString["wsc_id"];

            //If an office was passed, then use this to determine the WSCID
            if (!string.IsNullOrEmpty(office_id))
            {
                OfficeID = Convert.ToInt32(office_id);
                WSCID = Convert.ToInt32(db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id);
            }
            //If a WSC was passed, then set the local variable - don't need to worry about setting the OfficeID (the default view will be to show all users for all offfices
            else if (!string.IsNullOrEmpty(wsc_id))
            {
                WSCID = Convert.ToInt32(wsc_id);
                OfficeID = (int)db.Offices.FirstOrDefault(p => p.wsc_id == WSCID).office_id;
            }
            else
            {
                //If the office id and wsc id session variables are empty, set these values to the user's assigned office
                if (OfficeID == 0 && WSCID == 0)
                {
                    OfficeID = user.OfficeID;
                    WSCID = (int)db.Offices.FirstOrDefault(p => p.office_id == user.OfficeID).wsc_id;
                }
            }

            string wsc_nm = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).wsc_nm;
            ph1.Title = "Continuous Records Processing Status";
            ph1.SubTitle = "For the " + wsc_nm + " WSC";
            ph1.RecordType = "&nbsp;";

            if (!Page.IsPostBack)
            {
                //On initial load, narrow down the view to the responsible office
                reportOfficeID = OfficeID;
                InitialDataBind();
            }
        }

        /// <summary>
        /// When binding the chart data initially, pull data for the entire WSC
        /// </summary>
        protected void InitialDataBind()
        {
            //Filter controls
            rddlOffice.DataSource = db.Offices.Where(p => p.wsc_id == WSCID).OrderBy(p => p.office_nm).ToList();
            rddlOffice.DataBind();
            rddlOffice.Items.Insert(0, "All Offices");
            rddlOffice.SelectedValue = reportOfficeID.ToString();

            if (rrblRecords.SelectedValue == null) rrblRecords.SelectedValue = "active";

            ltl150DaysAgo.Text = String.Format("The date 150 days ago: <b>{0:MM/dd/yyyy}</b>", DateTime.Now.AddDays(-150));
            ltl240DaysAgo.Text = String.Format("The date 240 days ago: <b>{0:MM/dd/yyyy}</b>", DateTime.Now.AddDays(-240));
        }

        #region rgCRPStatus
        protected void rgCRPStatus_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            string onlyactive = "yes";
            if (rrblRecords.SelectedValue != "active")
                onlyactive = "no";

            if (rddlOffice.SelectedIndex > 0) reportOfficeID = Convert.ToInt32(rddlOffice.SelectedValue); else reportOfficeID = 0;

            var CRPdata = db.SP_CRP_Ult_Data_Aging_Table(reportOfficeID, WSCID, onlyactive).Select(p => new
            {
                office_cd = p.office_cd,
                office_id = p.office_id,
                rms_record_id = p.rms_record_id,
                site_no = p.site_no,
                station_nm = p.station_nm,
                category_no = p.category_no,
                parm_cd = p.parm_cd,
                type_cd = p.type_cd,
                type_ds = p.type_ds,
                analyzed_period_beg_dt = p.analyzed_period_beg_dt,
                analyzed_period_dt = p.analyzed_period_dt,
                approved_period_beg_dt = p.approved_period_beg_dt,
                approved_period_dt = p.approved_period_dt,
                last_aging_dt = p.last_aging_dt,
                DaysSinceAging = p.DaysSinceAging,
                SIMS2017URL = Config.SIMS2017URL,
                site_id = db.Sites.FirstOrDefault(s => s.site_no == p.site_no).site_id
            }).OrderBy(p => p.office_cd).ThenBy(p => p.site_no).ThenBy(p => p.type_cd).ToList();

            rgCRPStatus.DataSource = CRPdata;
            ltlNumberOfRecords.Text = "Number of records returned: <b>" + CRPdata.Count.ToString() + "</b>";
            
        }

        protected void rgCRPStatus_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                GridDataItem item = e.Item as GridDataItem;
                int rms_record_id = Convert.ToInt32(item.GetDataKeyValue("rms_record_id"));

                var ras = db.RecordApprovalStatus.FirstOrDefault(p => p.rms_record_id == rms_record_id);

                if (ras != null)
                {
                    //If this is a category 1 record, then compare against the 150 days ago date; otherwise compare against 240 days ago date; if category 3, exit - no color changes necessary
                    DateTime daysAgo;
                    if (ras.category_no == 1) daysAgo = DateTime.Now.AddDays(-150); else if (ras.category_no == 2) daysAgo = DateTime.Now.AddDays(-240); else return;

                    //If all analyzed periods have been approved, then set the background colors of both the analyzed and approved columns to blanched almond
                    if (ras.analyzed_period_end_dt == ras.approved_period_end_dt)
                    {
                        item["analyzed_period_dt"].BackColor = System.Drawing.Color.BurlyWood;
                        item["approved_period_dt"].BackColor = System.Drawing.Color.BurlyWood;
                    }

                    //If the analyzed period date is within range
                    if (ras.analyzed_period_end_dt > daysAgo)
                    {
                        //If all analyzed periods have been approved and are within range, then set the background colors of both the analyzed and approved columns to pale goldenrod
                        if (ras.analyzed_period_end_dt == ras.approved_period_end_dt)
                        {
                            item["analyzed_period_dt"].BackColor = System.Drawing.Color.DarkKhaki;
                            item["approved_period_dt"].BackColor = System.Drawing.Color.DarkKhaki;
                        }
                        //Otherwise, set only the background of the analyzed cell to peach puff
                        else
                        {
                            item["analyzed_period_dt"].BackColor = System.Drawing.Color.PaleGoldenrod;
                        }
                    }

                    //If the approved period date is within range
                    if (ras.approved_period_end_dt > daysAgo)
                    {
                        //If all analyzed periods have been approved and are within range, then set the background colors of both the analyzed and approved columns to pale goldenrod
                        if (ras.approved_period_end_dt == ras.analyzed_period_end_dt)
                        {
                            item["analyzed_period_dt"].BackColor = System.Drawing.Color.DarkKhaki;
                            item["approved_period_dt"].BackColor = System.Drawing.Color.DarkKhaki;
                        }
                        //Otherwise, set only the background of the approved cell to peach puff
                        else
                        {
                            item["approved_period_dt"].BackColor = System.Drawing.Color.PaleGoldenrod;
                        }
                    }

                    if (ras.last_aging_dt > daysAgo) item["last_aging_dt"].BackColor = System.Drawing.Color.RosyBrown;
                }

            }
        }

        protected void rgCRPStatus_PreRender(object sender, EventArgs e)
        {
            GridFilterMenu menu = rgCRPStatus.FilterMenu;
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
        #endregion

        protected void UpdateDetails(object sender, CommandEventArgs e)
        {
            if (rddlOffice.SelectedIndex > 0) reportOfficeID = Convert.ToInt32(rddlOffice.SelectedValue); else reportOfficeID = 0;
            InitialDataBind();
            rgCRPStatus.Rebind();
        }
    }
}