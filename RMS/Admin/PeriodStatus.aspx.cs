using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;

namespace RMS.Admin
{
    public partial class PeriodStatus : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        public Boolean HasEditAccess { get; set; }
        private Data.Site site;
        private Data.RecordAnalysisPeriod period;
        private int WY;
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
            string period_id = Request.QueryString["period_id"];

            if (!string.IsNullOrEmpty(office_id))
                OfficeID = Convert.ToInt32(office_id);
            else
                OfficeID = user.OfficeID;

            WSCID = Convert.ToInt32(db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id);

            if (!Page.IsPostBack)
            {
                //If the user belongs to this site's WSC (or has an exception to work in the WSC), or is a SuperUser, then allow them to edit the page
                if (user.WSCID.Contains(WSCID) && user.IsAdmin || user.IsSuperUser) HasEditAccess = true;

                if (HasEditAccess)
                {
                    pnlHasAccess.Visible = true;
                    pnlNotice.Visible = false;
                    pnlNoAccess.Visible = false;
                    if (!string.IsNullOrEmpty(period_id))
                    {
                        period = db.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == Convert.ToInt32(period_id));
                        site = period.Record.Site;
                        PopulateRecordPeriods("edit");
                    }
                    else
                    {
                        pnlEnterSite.Visible = true;
                        pnlEditStatus.Visible = false;
                        rbCancel.PostBackUrl = String.Format("{0}Admin/Tasks.asp?office_id={1}", Config.SIMSURL, OfficeID);
                    }
                }
                else
                {
                    pnlHasAccess.Visible = false;
                    pnlNoAccess.Visible = true;
                }

                UserControlSetup();
            }
        }

        protected void UserControlSetup()
        {
            string wsc_nm = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).wsc_nm;
            ph1.Title = "Manage Record Period Status";

            ph1.SubTitle = "For the " + wsc_nm + " WSC";
            ph1.RecordType = "&nbsp;";
        }

        protected void btnSubmitSite_Command(object sender, CommandEventArgs e)
        {
            if (pnlEditStatus.Visible == false)
            {
                site = db.Sites.FirstOrDefault(p => p.site_no == rtbSiteNo.Text && p.agency_cd == rtbAgencyCd.Text);
                PopulateRecordPeriods("view");
            }
        }

        protected void lbReturn_Command(object sender, CommandEventArgs e)
        {
            pnlEnterSite.Visible = true;
            rtbSiteNo.Text = "";
            pnlEditStatus.Visible = false;
        }

        public void PopulateRecordPeriods(string showing)
        {
            if (site.Records != null)
            {
                foreach (var rec in site.Records)
                {
                    if (DateTime.Now.Month > 9) WY = DateTime.Now.AddYears(-1).Year; else WY = DateTime.Now.Year;
                    DateTime start_dt = Convert.ToDateTime(String.Format("10/1/{0}", WY - 1));
                    DateTime end_dt = Convert.ToDateTime(String.Format("9/30/{0}", DateTime.Now.Year));

                    var dsPeriods = db.SP_RMS_Periods_per_WY(rec.rms_record_id, start_dt, end_dt, "0").ToList();
                    int period_count = dsPeriods.Count();

                    if (rec.RecordAnalysisPeriods != null)
                    {
                        if (period_count > 0)
                        {
                            DataList dlPeriods = ConstructDL(rec.RecordType.type_ds, period_count, WY.ToString());

                            dlPeriods.DataSource = db.SP_RMS_Periods_per_WY(rec.rms_record_id, start_dt, end_dt, "0").ToList();
                            dlPeriods.DataBind();

                            phPeriods.Controls.Add(dlPeriods);
                        }
                        else
                        {
                            int lastWY = WY - 1;
                            start_dt = Convert.ToDateTime(String.Format("10/1/{0}", lastWY - 1));
                            end_dt = Convert.ToDateTime(String.Format("9/30/{0}", lastWY));
                            var dsPeriods_lastWY = db.SP_RMS_Periods_per_WY(rec.rms_record_id, start_dt, end_dt, "0").ToList();
                            int period_count_lastWY = dsPeriods_lastWY.Count();

                            if (period_count_lastWY > 0)
                            {
                                DataList dlPeriods = ConstructDL(rec.RecordType.type_ds, period_count_lastWY, lastWY.ToString());

                                dlPeriods.DataSource = dsPeriods_lastWY;
                                dlPeriods.DataBind();

                                phPeriods.Controls.Add(dlPeriods);
                            }
                            else
                            {
                                int last2WY = lastWY - 1;
                                start_dt = Convert.ToDateTime(String.Format("10/1/{0}", last2WY - 1));
                                end_dt = Convert.ToDateTime(String.Format("9/30/{0}", last2WY));
                                var dsPeriods_last2WY = db.SP_RMS_Periods_per_WY(rec.rms_record_id, start_dt, end_dt, "0").ToList();
                                int period_count_last2WY = dsPeriods_last2WY.Count();

                                if (period_count_last2WY > 0)
                                {
                                    DataList dlPeriods = ConstructDL(rec.RecordType.type_ds, period_count_last2WY, last2WY.ToString());

                                    dlPeriods.DataSource = dsPeriods_last2WY;
                                    dlPeriods.DataBind();

                                    phPeriods.Controls.Add(dlPeriods);
                                }
                            }
                        }
                    }
                    else
                    {
                        lblNoRecs.Visible = true;
                        pnlEnterSite.Visible = true;
                        pnlEditStatus.Visible = false;

                        return;
                    }
                }

                lblSiteNo.Text = "Records for <a href=\"" + Config.SIMSURL + "StationInfo.aspx?site_id=" + site.site_id.ToString() + "\" target=\"_blank\">" + site.site_no + " " + site.station_full_nm + "</a>";
                lblNoRecs.Visible = false;
                pnlEnterSite.Visible = false;
                pnlEditStatus.Visible = true;

                if (showing == "view")
                {
                    pnlInstructions.Visible = true;
                    pnlEdit.Visible = false;
                }
                else if (showing == "confirm")
                {
                    pnlInstructions.Visible = false;
                }
                else
                {
                    pnlInstructions.Visible = false;
                    PopulateEditStatusArea();
                }
            }
            else
            {
                lblNoRecs.Visible = true;
                pnlEnterSite.Visible = true;
                pnlEditStatus.Visible = false;
            }

        }

        public DataList ConstructDL(string RecordType, int Count, string WY)
        {
            DataList dlPeriods = new DataList();

            DataListTemplate headTemplate = new DataListTemplate(ListItemType.Header);
            headTemplate.RecordType = RecordType;
            dlPeriods.HeaderTemplate = headTemplate;

            dlPeriods.Width = Unit.Percentage(100);

            DataListTemplate itemTemplate = new DataListTemplate(ListItemType.Item);
            itemTemplate.NoOfPeriods = Count.ToString();
            dlPeriods.ItemTemplate = itemTemplate;

            DataListTemplate footerTemplate = new DataListTemplate(ListItemType.Footer);
            footerTemplate.WY = WY;
            footerTemplate.NoOfPeriods = Count.ToString();
            dlPeriods.FooterTemplate = footerTemplate;

            return dlPeriods;
        }

        public void PopulateEditStatusArea()
        {
            ltlPeriodID.Text = period.period_id.ToString();
            ltlBeginDt.Text = String.Format("{0:MM/dd/yyyy}", period.period_beg_dt);
            ltlEndDt.Text = String.Format("{0:MM/dd/yyyy}", period.period_end_dt);
            ltlStatus.Text = period.status_va;

            var period_dialogs = period.PeriodDialogs;
            List<Data.PeriodDialog> dialogs = new List<Data.PeriodDialog>();

            switch (period.status_va)
            {
                case "Analyzed":
                    rbEditStatus.Text = "Set the status to Analyzing";
                    dialogs = period_dialogs.Where(p => p.status_set_to_va == "Analyzed").ToList();
                    break;
                case "Approved":
                    rbEditStatus.Text = "Set the status to Analyzed";
                    dialogs = period_dialogs.Where(p => p.origin_va == "Approver").ToList();
                    break;
            }

            dlDialogs.DataSource = dialogs;
            dlDialogs.DataBind();

            hlDialogs.NavigateUrl = "javascript:EnableButton('../Handler/DialogsHandler.ashx?period_id=" + period.period_id.ToString() + "')";

            pnlEdit.Visible = true;
        }

        protected void rbEditStatus_Command(object sender, CommandEventArgs e)
        {
            if (hfStatusEdited.Value == "no")
            {
                period = db.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == Convert.ToInt32(Request.QueryString["period_id"]));
                List<Data.PeriodDialog> dialogs = new List<Data.PeriodDialog>();
                switch (period.status_va)
                {
                    case "Analyzed":
                        dialogs = period.PeriodDialogs.Where(p => p.status_set_to_va == "Analyzed").ToList();
                        period.status_va = "Analyzing";
                        period.status_set_by_role_va = "Analyst";
                        break;
                    case "Approved":
                        dialogs = period.PeriodDialogs.Where(p => p.origin_va == "Approver").ToList();
                        period.status_va = "Analyzed";
                        period.status_set_by_role_va = "Analyst";
                        break;
                }
                db.PeriodDialogs.DeleteAllOnSubmit(dialogs);
                
                pnlSetBackStatus.Visible = false;
                pnlDialogs.Visible = false;
                pnlConfirm.Visible = true;

                db.SubmitChanges();

                site = period.Record.Site;
                PopulateRecordPeriods("confirm");

                hfStatusEdited.Value = "yes";
            }
        }
    }
}