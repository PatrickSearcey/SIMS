using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RMS.Admin
{
    public partial class PeriodDate : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        public Boolean HasEditAccess { get; set; }
        private Data.Site site;
        private Data.RecordAnalysisPeriod period;
        private string date_type;
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
            date_type = Request.QueryString["dt"];

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
                        PopulateRecordPeriods("edit", "");
                    }
                    else
                    {
                        pnlEnterSite.Visible = true;
                        pnlEditDates.Visible = false;
                        rbCancel.PostBackUrl = String.Format("{0}Admin/Tasks.asp?office_id={1}", Config.SIMS2017URL, OfficeID);
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
            if (pnlEditDates.Visible == false)
            {
                site = db.Sites.FirstOrDefault(p => p.site_no == rtbSiteNo.Text && p.agency_cd == rtbAgencyCd.Text);
                rpPeriods.Collapsed = false;
                PopulateRecordPeriods("view", "");
            }
        }

        protected void lbReturn_Command(object sender, CommandEventArgs e)
        {
            pnlEnterSite.Visible = true;
            rtbSiteNo.Text = "";
            pnlEditDates.Visible = false;
        }

        public void PopulateRecordPeriods(string showing, string new_dt)
        {
            if (DateTime.Now.Month > 9) WY = DateTime.Now.AddYears(-1).Year; else WY = DateTime.Now.Year;
            DateTime start_dt = Convert.ToDateTime("1/1/1900");
            DateTime end_dt = Convert.ToDateTime("1/1/1900");

            if (site.Records != null)
            {
                foreach (var rec in site.Records)
                {
                    var dsPeriods = db.SP_RMS_Periods_per_WY(rec.rms_record_id, start_dt, end_dt, "0");
                    int period_count = dsPeriods.Count();

                    if (rec.RecordAnalysisPeriods != null)
                    {
                        DataList dlPeriods = ConstructDL(rec.RecordType.type_ds, period_count, WY.ToString());

                        dlPeriods.DataSource = db.SP_RMS_Periods_per_WY(rec.rms_record_id, start_dt, end_dt, "0");
                        dlPeriods.DataBind();

                        phPeriods.Controls.Add(dlPeriods);
                    }
                    else
                    {
                        lblNoRecs.Visible = true;
                        pnlEnterSite.Visible = true;
                        pnlEditDates.Visible = false;

                        return;
                    }
                }

                lblSiteNo.Text = "Records for <a href=\"" + Config.SIMS2017URL + "StationInfo.asp?site_id=" + site.site_id + "\" target=\"_blank\">" + site.site_no + " " + site.station_full_nm + "</a>";
                lblNoRecs.Visible = false;
                pnlEnterSite.Visible = false;
                pnlEditDates.Visible = true;

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
                    PopulateEditDatesArea(new_dt);
                }
            }
            else
            {
                lblNoRecs.Visible = true;
                pnlEnterSite.Visible = true;
                pnlEditDates.Visible = false;
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
            itemTemplate.WhichView = "dates";
            itemTemplate.NoOfPeriods = Count.ToString();
            dlPeriods.ItemTemplate = itemTemplate;

            DataListTemplate footerTemplate = new DataListTemplate(ListItemType.Footer);
            footerTemplate.WY = WY;
            footerTemplate.WhichView = "dates";
            footerTemplate.NoOfPeriods = Count.ToString();
            dlPeriods.FooterTemplate = footerTemplate;

            return dlPeriods;
        }

        public void PopulateEditDatesArea(string new_dt)
        {
            ltlRecordType.Text = period.Record.RecordType.type_ds;

            //Details for the most recent record period
            var mrp = db.RecordAnalysisPeriods.OrderByDescending(p => p.period_beg_dt).FirstOrDefault(p => p.rms_record_id == period.rms_record_id);
            //Details for the second most recent record period
            var smrp = db.RecordAnalysisPeriods.OrderByDescending(p => p.period_beg_dt).Where(p => p.rms_record_id == period.rms_record_id).Skip(1).Take(1).FirstOrDefault();

            string periodid1 = null;
            string begdate1 = null;
            string enddate1 = null;
            string status1 = null;
            string periodid2 = null;
            string begdate2 = null;
            string enddate2 = null;
            string status2 = null;
            DateTime begdate2_minusms = default(DateTime);
            string begdate2_minustime = null;

            if (mrp != null)
            {
                periodid2 = mrp.period_id.ToString();
                begdate2 = String.Format("{0:MM/dd/yyyy}", mrp.period_beg_dt);
                enddate2 = String.Format("{0:MM/dd/yyyy}", mrp.period_end_dt);
                status2 = mrp.status_va;

                begdate2_minusms = Convert.ToDateTime(mrp.period_beg_dt).AddMonths(-16);
                begdate2_minustime = String.Format("{0:MM/dd/yyyy}", begdate2_minusms);
            }

            if (smrp != null)
            {
                periodid1 = smrp.period_id.ToString();
                begdate1 = String.Format("{0:MM/dd/yyyy}", smrp.period_beg_dt);
                enddate1 = String.Format("{0:MM/dd/yyyy}", smrp.period_end_dt);
                status1 = smrp.status_va;
            }

            if (periodid2 == periodid1)
            {
                ltlPeriodID1.Visible = false;
                ltlBegDate1.Visible = false;
                lblEndDate1.Visible = false;
                ltlStatus1.Visible = false;
            }
            else
            {
                ltlPeriodID1.Text = periodid1;
                ltlBegDate1.Text = begdate1;
                lblEndDate1.Text = enddate1;
                ltlStatus1.Text = status1;
            }

            ltlPeriodID2.Text = periodid2;
            lblBegDate2.Text = begdate2;
            lblEndDate2.Text = enddate2;
            ltlStatus2.Text = status2;

            if (date_type == "end1" | date_type == "beg2")
            {
                lblEndDate1.Font.Bold = true;
                lblEndDate1.ForeColor = System.Drawing.Color.Red;
                lblBegDate2.Font.Bold = true;
                lblBegDate2.ForeColor = System.Drawing.Color.Red;
                lblEndDate2.Font.Bold = false;
                lblEndDate2.ForeColor = System.Drawing.Color.Black;

                pnlEndBeginDates.Visible = true;
                pnlEndDate.Visible = false;

                if (periodid2 == periodid1)
                {
                    InitDates(begdate2_minustime, enddate2, begdate2);
                }
                else
                {
                    InitDates(begdate1, enddate2, begdate2);
                }

            }
            else
            {
                lblEndDate1.Font.Bold = false;
                lblEndDate1.ForeColor = System.Drawing.Color.Black;
                lblBegDate2.Font.Bold = false;
                lblBegDate2.ForeColor = System.Drawing.Color.Black;
                lblEndDate2.Font.Bold = true;
                lblEndDate2.ForeColor = System.Drawing.Color.Red;

                pnlEndBeginDates.Visible = false;
                pnlEndDate.Visible = true;

                InitDates(begdate2, DateTime.Now.ToString(), enddate2);
            }

            pnlEdit.Visible = true;
        }

        private void InitDates(string start_date, string end_date, string change_date)
        {
            DateTime dtStart = Convert.ToDateTime(start_date);
            DateTime dtEnd = Convert.ToDateTime(end_date);
            DateTime dtChange = Convert.ToDateTime(change_date);
            int dtdiff = Convert.ToInt32(dtEnd.ToOADate() - dtStart.ToOADate());

            if (date_type == "end1" | date_type == "beg2")
            {
                lblSlider1StartDate.Text = dtStart.ToShortDateString();
                lblSlider1EndDate.Text = dtEnd.ToShortDateString();
                rsEndBeginDates.MinimumValue = Convert.ToInt32(dtStart.ToOADate()) + 1;
                rsEndBeginDates.MaximumValue = Convert.ToInt32(dtEnd.ToOADate()) - 1;
                rsEndBeginDates.Value = Convert.ToInt32(dtChange.ToOADate());
                rsEndBeginDates.SmallChange = 1;

                if (dtdiff < 200)
                {
                    rsEndBeginDates.Width = 370;
                }
                else if (dtdiff > 199 & dtdiff < 365)
                {
                    rsEndBeginDates.Width = 550;
                }
                else if (dtdiff > 364 & dtdiff < 730)
                {
                    rsEndBeginDates.Width = 650;
                }
                else
                {
                    rsEndBeginDates.Width = 900;
                    rpPeriods.Collapsed = true;
                }
            }
            else
            {
                lblSlider2StartDate.Text = dtStart.ToShortDateString();
                lblSlider2EndDate.Text = dtEnd.ToShortDateString();
                rsEndDate.MinimumValue = Convert.ToInt32(dtStart.ToOADate()) + 1;
                rsEndDate.MaximumValue = Convert.ToInt32(dtEnd.ToOADate());
                rsEndDate.Value = Convert.ToInt32(dtChange.ToOADate());
                rsEndDate.SmallChange = 1;

                if (dtdiff < 200)
                {
                    rsEndDate.Width = 370;
                }
                else if (dtdiff > 199 & dtdiff < 365)
                {
                    rsEndDate.Width = 550;
                }
                else if (dtdiff > 364 & dtdiff < 730)
                {
                    rsEndDate.Width = 650;
                }
                else
                {
                    rsEndDate.Width = 900;
                    rpPeriods.Collapsed = true;
                }
            }

        }

        protected void rbEditDates_Command(object sender, CommandEventArgs e)
        {
            if (hfDatesEdited.Value == "no")
            {
                DateTime dtNewDate = default(DateTime);

                if (date_type == "end1" | date_type == "beg2")
                {
                    dtNewDate = DateTime.FromOADate(Convert.ToDouble(rsEndBeginDates.Value));
                    if (ltlPeriodID1.Visible == false)
                    {
                        period = db.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == Convert.ToInt32(ltlPeriodID2.Text));
                        period.period_beg_dt = dtNewDate;

                        Data.PeriodDialog dialog = new Data.PeriodDialog()
                        {
                            period_id = Convert.ToInt32(ltlPeriodID2.Text),
                            dialog_dt = DateTime.Now,
                            dialog_by = "admin",
                            origin_va = "Admin",
                            comments_va = "Period date modified using Modify Record Period Dates interface."
                        };
                        db.PeriodDialogs.InsertOnSubmit(dialog);
                        site = period.Record.Site;
                    }
                    else
                    {
                        var period1 = db.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == Convert.ToInt32(ltlPeriodID1.Text));
                        var period2 = db.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == Convert.ToInt32(ltlPeriodID2.Text));

                        period1.period_end_dt = dtNewDate;
                        period2.period_beg_dt = dtNewDate;

                        Data.PeriodDialog dialog1 = new Data.PeriodDialog()
                        {
                            period_id = Convert.ToInt32(ltlPeriodID1.Text),
                            dialog_dt = DateTime.Now,
                            dialog_by = "admin",
                            origin_va = "Admin",
                            comments_va = "Period date modified using Modify Record Period Dates interface."
                        };
                        db.PeriodDialogs.InsertOnSubmit(dialog1);
                        Data.PeriodDialog dialog2 = new Data.PeriodDialog()
                        {
                            period_id = Convert.ToInt32(ltlPeriodID2.Text),
                            dialog_dt = DateTime.Now,
                            dialog_by = "admin",
                            origin_va = "Admin",
                            comments_va = "Period date modified using Modify Record Period Dates interface."
                        };
                        db.PeriodDialogs.InsertOnSubmit(dialog2);
                        site = period1.Record.Site;
                    }
                }
                else
                {
                    dtNewDate = DateTime.FromOADate(Convert.ToDouble(rsEndDate.Value));
                    period = db.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == Convert.ToInt32(ltlPeriodID2.Text));
                    period.period_end_dt = dtNewDate;

                    Data.PeriodDialog dialog = new Data.PeriodDialog()
                    {
                        period_id = Convert.ToInt32(ltlPeriodID2.Text),
                        dialog_dt = DateTime.Now,
                        dialog_by = "admin",
                        origin_va = "Admin",
                        comments_va = "Period date modified using Modify Record Period Dates interface."
                    };
                    db.PeriodDialogs.InsertOnSubmit(dialog);
                    site = period.Record.Site;
                }

                db.SubmitChanges();

                pnlEditDate.Visible = false;
                pnlConfirm.Visible = true;

                PopulateRecordPeriods("confirm", "");

                hfDatesEdited.Value = "yes";
            }
        }
    }
}