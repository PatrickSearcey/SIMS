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
    public partial class PeriodDetails : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private Data.Record currRecord;
        private int RecordID
        {
            get
            {
                if (Session["RecordID"] == null) return 0; else return (int)Session["RecordID"];
            }
            set
            {
                Session["RecordID"] = value;
            }
        }
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

        #region Page Load Events
        protected void Page_Load(object sender, EventArgs e)
        {
            string office_id = Request.QueryString["office_id"];

            if (!string.IsNullOrEmpty(office_id))
            {
                OfficeID = Convert.ToInt32(office_id);
                WSCID = Convert.ToInt32(db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id);
            }
            else
            {
                //If the office id and wsc id session variables are empty, set these values to the user's assigned office
                if (OfficeID == 0 && WSCID == 0)
                {
                    OfficeID = user.OfficeID;
                    WSCID = (int)db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id;
                }
                else if (OfficeID == 0 && WSCID > 0)
                    OfficeID = db.Offices.FirstOrDefault(p => p.wsc_id == WSCID).office_id;
                else if (OfficeID > 0 && WSCID == 0)
                    WSCID = (int)db.Offices.FirstOrDefault(p => p.office_id == OfficeID).wsc_id;
            }

            string wsc_nm = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).wsc_nm;
            ph1.Title = "Retrieve Record Period Details";

            if (RecordID == 0 || Request.QueryString["type"] == "wsc")
            {
                ph1.SubTitle = "For the " + wsc_nm + " WSC";
                ph1.RecordType = "&nbsp;";
            }
            else
            {
                currRecord = db.Records.FirstOrDefault(p => p.rms_record_id == RecordID);
                ph1.SubTitle = currRecord.Site.site_no + " " + db.vSITEFILEs.FirstOrDefault(s => s.site_id == currRecord.Site.nwisweb_site_id).station_nm;
                ph1.RecordType = currRecord.RecordType.type_ds + " Record for";
            }

            if (!Page.IsPostBack)
            {
                InitialDataBind();
            }
        }

        protected void InitialDataBind()
        {
            rddlOffice.DataSource = db.Offices
                .Where(p => p.wsc_id == WSCID)
                .Select(p => new { office_id = p.office_id, office_nm = p.office_nm })
                .OrderBy(p => p.office_nm).ToList();
            rddlOffice.DataBind();
            rddlOffice.Items.Insert(0, "");

            rddlRecords.DataSource = db.Records
                .Where(p => p.Site.Office.wsc_id == WSCID && p.not_used_fg == false)
                .Select(p => new { rms_record_id = p.rms_record_id, SiteRecord = p.Site.site_no + " " + p.Site.station_full_nm + " (" + p.RecordType.type_cd + ")" })
                .OrderBy(p => p.SiteRecord).ToList();
            rddlRecords.DataBind();
            rddlRecords.Items.Insert(0, "");

            if (RecordID > 0) rddlRecords.SelectedValue = RecordID.ToString();

            rdpBeginDt.SelectedDate = Convert.ToDateTime("10/1/" + (DateTime.Now.Year - 1).ToString());
            rdpEndDt.SelectedDate = DateTime.Now;

            ltlNotice1.Visible = true;
            ltlNotice2.Visible = true;
            ltlNotice3.Visible = true;
            ltlError.Visible = false;
        }
        #endregion

        #region Filter Control Event
        protected void UpdateRecords(object sender, EventArgs e)
        {
            //If an office was selected, then narrow down the record list by record_office_id
            if (rddlOffice.SelectedIndex > 0)
            {
                List<int> records = db.vRMSRecordOfficeIDs.Where(p => p.record_office_id == Convert.ToInt32(rddlOffice.SelectedValue)).Select(p => p.rms_record_id).ToList();
                if (records.Count() > 0)
                {
                    var recList = db.Records
                        .Where(p => records.Contains(p.rms_record_id) && p.not_used_fg == false)
                        .Select(p => new { rms_record_id = p.rms_record_id, SiteRecord = p.Site.site_no + " " + p.Site.station_full_nm + " (" + p.RecordType.type_cd + ")" })
                        .OrderBy(p => p.SiteRecord).ToList();
                    if (recList.Count() > 0)
                    {
                        rddlRecords.DataSource = recList;
                        rddlRecords.DataBind();
                        rddlRecords.Items.Insert(0, "");
                        rddlRecords.Items.Insert(1, "All Records");

                        rbSubmit.Enabled = true;
                    }
                    else
                    {
                        rddlRecords.Items.Clear();
                        rddlRecords.Items.Insert(0, "No Records Found");
                        rbSubmit.Enabled = false;
                    }
                }
                else
                {
                    rddlRecords.Items.Clear();
                    rddlRecords.Items.Insert(0, "No Records Found");
                    rbSubmit.Enabled = false;
                }
            }
            else //If office selection was reset back to "nothing", then reset the record list to be the full, WSC-wide list
            {
                rddlRecords.DataSource = db.Records
                    .Where(p => p.Site.Office.wsc_id == WSCID && p.not_used_fg == false)
                    .Select(p => new { rms_record_id = p.rms_record_id, SiteRecord = p.Site.site_no + " " + p.Site.station_full_nm + " (" + p.RecordType.type_cd + ")" })
                    .OrderBy(p => p.SiteRecord).ToList();
                rddlRecords.DataBind();
                rddlRecords.Items.Insert(0, "");
                rbSubmit.Enabled = true;
            }
        }
        #endregion

        #region Submit Event
        protected void UpdateDetails(object sender, CommandEventArgs e)
        {
            ltlError.Visible = false;
            List<int> records = new List<int>();
            //If all records were chosen, or no records were chosen, then add all the rms_record_ids in the dropdown list to the records list
            if (rddlRecords.SelectedText == "All Records" || rddlRecords.SelectedText == "" && rddlOffice.SelectedIndex > 0)
            {
                foreach (DropDownListItem record in rddlRecords.Items)
                {
                    if (record.Text != "" && record.Text != "All Records") records.Add(Convert.ToInt32(record.Value));
                }
            }
            //Otherwise, add just the one, chosen rms_record_id
            else if (rddlRecords.SelectedText != "")
            {
                records.Add(Convert.ToInt32(rddlRecords.SelectedValue));
            }
            else
            {
                ltlError.Text = "You must select at least one record!";
                ltlError.Visible = true;
                return;
            }

            var recordData = db.Records
                    .Where(p => records.Contains(p.rms_record_id))
                    .Select(p => new { rms_record_id = p.rms_record_id, site_no = p.Site.site_no, station_nm = p.Site.station_full_nm, type_ds = p.RecordType.type_ds })
                    .OrderBy(p => p.site_no).ThenBy(p => p.type_ds).ToList();

            dlOuterSANAL.DataSource = recordData;
            dlOuterSANAL.DataBind();

            dlOuterChangeLogs.DataSource = recordData;
            dlOuterChangeLogs.DataBind();

            dlOuterDialogs.DataSource = recordData;
            dlOuterDialogs.DataBind();

            ltlNotice1.Visible = false;
            ltlNotice2.Visible = false;
            ltlNotice3.Visible = false;
        }
        #endregion

        #region SANALs
        protected void dlOuterSANAL_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataList dlInnerSANAL = e.Item.FindControl("dlInnerSANAL") as DataList;
                HiddenField hfRecordID = e.Item.FindControl("hfRecordID") as HiddenField;

                //Make sure a record is found from the current row's rms_record_id
                var tRecord = db.Records.FirstOrDefault(p => p.rms_record_id.ToString() == hfRecordID.Value.ToString());

                if (tRecord != null)
                {
                    DateTime period_beg_dt, period_end_dt;
                    if (rdpBeginDt.SelectedDate == null) period_beg_dt = Convert.ToDateTime(String.Format("10/01/{0}", DateTime.Now.Year - 1)); else period_beg_dt = Convert.ToDateTime(rdpBeginDt.SelectedDate);
                    if (rdpEndDt.SelectedDate == null) period_end_dt = DateTime.Now; else period_end_dt = Convert.ToDateTime(rdpEndDt.SelectedDate);

                    List<AnalysisNotesItem> lani = new List<AnalysisNotesItem>();
                    //Grab all analysis periods within this timespan
                    var periods = tRecord.RecordAnalysisPeriods
                        .Where(p => p.period_end_dt >= period_beg_dt && p.period_beg_dt <= period_end_dt)
                        .OrderByDescending(p => p.period_beg_dt).ToList();

                    foreach (var period in periods)
                    {
                        AnalysisNotesItem ani = new AnalysisNotesItem
                        {
                            timespan = String.Format("{0:MM/dd/yyyy} to {1:MM/dd/yyyy}", period.period_beg_dt, period.period_end_dt),
                            analysis_notes_va = period.analysis_notes_va.FormatParagraphOut(),
                            edited_dt = period.PeriodChangeLogs.Count() > 0 ? String.Format("{0}", period.PeriodChangeLogs.OrderByDescending(b => b.edited_dt).FirstOrDefault().edited_dt) : "unavailable",
                            edited_by_uid = period.PeriodChangeLogs.Count() > 0 ? period.PeriodChangeLogs.OrderByDescending(b => b.edited_dt).FirstOrDefault().edited_by_uid : "unavailable",
                            analyzed_by = period.analyzed_by,
                            analyzed_dt = String.Format("{0:MM/dd/yyyy}", period.analyzed_dt),
                            approved_by = period.approved_by,
                            approved_dt = String.Format("{0:MM/dd/yyyy}", period.approved_dt)
                        };
                        lani.Add(ani);
                    }

                    dlInnerSANAL.DataSource = lani;
                    dlInnerSANAL.DataBind();
                }
            }
        }
        #endregion

        #region ChangeLogs
        protected void dlOuterChangeLogs_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataList dlInnerChangeLogs = e.Item.FindControl("dlInnerChangeLogs") as DataList;
                HiddenField hfRecordID = e.Item.FindControl("hfRecordID") as HiddenField;

                //Make sure a record is found from the current row's rms_record_id
                var tRecord = db.Records.FirstOrDefault(p => p.rms_record_id.ToString() == hfRecordID.Value.ToString());

                if (tRecord != null)
                {
                    DateTime period_beg_dt, period_end_dt;
                    if (rdpBeginDt.SelectedDate == null) period_beg_dt = Convert.ToDateTime(String.Format("10/01/{0}", DateTime.Now.Year - 1)); else period_beg_dt = Convert.ToDateTime(rdpBeginDt.SelectedDate);
                    if (rdpEndDt.SelectedDate == null) period_end_dt = DateTime.Now; else period_end_dt = Convert.ToDateTime(rdpEndDt.SelectedDate);

                    //Grab all analysis periods within this timespan
                    var periods = tRecord.RecordAnalysisPeriods
                        .Where(p => p.period_end_dt >= period_beg_dt && p.period_beg_dt <= period_end_dt)
                        .Select(p => new { period_id = p.period_id, timespan = String.Format("{0:MM/dd/yyyy} to {1:MM/dd/yyyy}", p.period_beg_dt, p.period_end_dt), period_beg_dt = p.period_beg_dt })
                        .OrderByDescending(p => p.period_beg_dt).ToList();

                    dlInnerChangeLogs.DataSource = periods;
                    dlInnerChangeLogs.DataBind();
                }
            }
        }

        protected void dlInnerChangeLogs_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataGrid dgChangeLog = e.Item.FindControl("dgChangeLog") as DataGrid;
                HiddenField hfPeriodID = e.Item.FindControl("hfPeriodID") as HiddenField;

                //Make sure a period is found from the current row's period_id
                var currPeriod = db.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == Convert.ToInt32(hfPeriodID.Value));

                if (currPeriod != null)
                {
                    dgChangeLog.DataSource = currPeriod.PeriodChangeLogs.Select(p => new Data.PeriodChangeLog
                    {
                        edited_dt = p.edited_dt,
                        edited_by_uid = p.edited_by_uid,
                        new_va = p.new_va.FormatParagraphOut()
                    }).OrderByDescending(p => p.edited_dt).ToList();
                    dgChangeLog.DataBind();
                }
            }
        }
        #endregion

        #region Dialogs
        protected void dlOuterDialogs_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataList dlInnerDialogs = e.Item.FindControl("dlInnerDialogs") as DataList;
                HiddenField hfRecordID = e.Item.FindControl("hfRecordID") as HiddenField;

                //Make sure a record is found from the current row's rms_record_id
                var tRecord = db.Records.FirstOrDefault(p => p.rms_record_id.ToString() == hfRecordID.Value.ToString());

                if (tRecord != null)
                {
                    DateTime period_beg_dt, period_end_dt;
                    if (rdpBeginDt.SelectedDate == null) period_beg_dt = Convert.ToDateTime(String.Format("10/01/{0}", DateTime.Now.Year - 1)); else period_beg_dt = Convert.ToDateTime(rdpBeginDt.SelectedDate);
                    if (rdpEndDt.SelectedDate == null) period_end_dt = DateTime.Now; else period_end_dt = Convert.ToDateTime(rdpEndDt.SelectedDate);

                    //Grab all analysis periods within this timespan
                    var periods = tRecord.RecordAnalysisPeriods
                        .Where(p => p.period_end_dt >= period_beg_dt && p.period_beg_dt <= period_end_dt)
                        .Select(p => new { period_id = p.period_id, timespan = String.Format("{0:MM/dd/yyyy} to {1:MM/dd/yyyy}", p.period_beg_dt, p.period_end_dt), period_beg_dt = p.period_beg_dt })
                        .OrderByDescending(p => p.period_beg_dt).ToList();

                    dlInnerDialogs.DataSource = periods;
                    dlInnerDialogs.DataBind();
                }
            }
        }

        protected void dlInnerDialogs_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataGrid dgDialog = e.Item.FindControl("dgDialog") as DataGrid;
                HiddenField hfPeriodID = e.Item.FindControl("hfPeriodID") as HiddenField;

                //Make sure a period is found from the current row's period_id
                var currPeriod = db.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == Convert.ToInt32(hfPeriodID.Value));

                if (currPeriod != null)
                {
                    dgDialog.DataSource = currPeriod.PeriodDialogs.Select(p => new Data.PeriodDialog
                    {
                        dialog_dt = p.dialog_dt,
                        origin_va = p.origin_va,
                        dialog_by = p.dialog_by,
                        status_set_to_va = p.status_set_to_va,
                        comments_va = p.comments_va.FormatParagraphOut()
                    }).OrderByDescending(p => p.dialog_dt).ToList();
                    dgDialog.DataBind();
                }
            }
        }
        #endregion

        #region Internal Classes
        internal class AnalysisNotesItem
        {
            private string _timespan;
            private string _analysis_notes_va;
            private string _edited_dt;
            private string _edited_by_uid;
            private string _analyzed_by;
            private string _analyzed_dt;
            private string _approved_by;
            private string _approved_dt;

            public string timespan
            {
                get { return _timespan; }
                set { _timespan = value; }
            }
            public string analysis_notes_va
            {
                get { return _analysis_notes_va; }
                set { _analysis_notes_va = value; }
            }
            public string edited_dt
            {
                get { return _edited_dt; }
                set { _edited_dt = value; }
            }
            public string edited_by_uid
            {
                get { return _edited_by_uid; }
                set { _edited_by_uid = value; }
            }
            public string analyzed_by
            {
                get { return _analyzed_by; }
                set { _analyzed_by = value; }
            }
            public string analyzed_dt
            {
                get { return _analyzed_dt; }
                set { _analyzed_dt = value; }
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
            public AnalysisNotesItem()
            {
                _timespan = timespan;
                _analysis_notes_va = analysis_notes_va;
                _edited_dt = edited_dt;
                _edited_by_uid = edited_by_uid;
                _analyzed_by = analyzed_by;
                _analyzed_dt = analyzed_dt;
                _approved_by = approved_by;
                _approved_dt = approved_dt;
            }
        }
        #endregion
    }
}