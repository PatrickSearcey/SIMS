using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace RMS.Task
{
    public partial class Audit : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private Data.Record currRecord;
        private Data.Audit currAudit;
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
        private int AuditID
        {
            get
            {
                if (Session["AuditID"] == null) return 0; else return (int)Session["AuditID"];
            }
            set
            {
                Session["AuditID"] = value;
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

        protected void Page_Load(object sender, EventArgs e)
        {
            string rms_record_id = Request.QueryString["rms_record_id"];
            string rms_audit_id = Request.QueryString["rms_audit_id"];

            //If rms_record_id was passed, then set the RecordID variable and make sure the WSCID and OfficeID are changed to match the RecordIDs office and WSC
            if (!string.IsNullOrEmpty(rms_record_id))
            {
                RecordID = Convert.ToInt32(rms_record_id);
                currRecord = db.Records.Where(p => p.rms_record_id == RecordID).FirstOrDefault();
                if (currRecord.RecordAltOffice == null) OfficeID = (int)currRecord.Site.office_id; else OfficeID = (int)currRecord.RecordAltOffice.alt_office_id;
                WSCID = (int)db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault().wsc_id;
            }
            else RecordID = 0;

            if (!string.IsNullOrEmpty(rms_audit_id))
            {
                AuditID = Convert.ToInt32(rms_audit_id);
                currAudit = db.Audits.Where(p => p.rms_audit_id == AuditID).FirstOrDefault();
                WSCID = (int)currAudit.wsc_id;
                OfficeID = db.Offices.FirstOrDefault(p => p.wsc_id == WSCID).office_id;
            }
            else AuditID = 0;

            UserControlSetup();

            if (!Page.IsPostBack)
            {
                //If the user belongs to this site's WSC (or has an exception to work in the WSC), or is a SuperUser, then allow them to edit the page
                if (user.WSCID.Contains(WSCID) || user.IsSuperUser)
                {
                    pnlHasAccess.Visible = true;
                    pnlNoAccess.Visible = false;

                    InitialView();
                }
                else
                {
                    pnlHasAccess.Visible = false;
                    pnlNoAccess.Visible = true;
                }
            }
        }

        #region Internal Classes
        internal class RecordItem
        {
            private string _rms_record_id;
            private string _record_nm;

            public string rms_record_id
            {
                get { return _rms_record_id; }
                set { _rms_record_id = value; }
            }
            public string record_nm
            {
                get { return _record_nm; }
                set { _record_nm = value; }
            }
            public RecordItem()
            {
                _rms_record_id = rms_record_id;
                _record_nm = record_nm;
            }
        }
        #endregion

        #region Page Load Events
        protected void UserControlSetup()
        {
            string wsc_nm = db.WSCs.FirstOrDefault(p => p.wsc_id == WSCID).wsc_nm;
            if (AuditID == 0) ph1.Title = "Create a New Audit Period"; else ph1.Title = "Modify an Audit Period";

            if (RecordID == 0)
            {
                ph1.SubTitle = "For the " + wsc_nm + " WSC";
                ph1.RecordType = "&nbsp;";
            }
            else
            {
                ph1.SubTitle = currRecord.Site.site_no + " " + db.vSITEFILEs.FirstOrDefault(s => s.site_id == currRecord.Site.nwisweb_site_id).station_nm;
                ph1.RecordType = currRecord.RecordType.type_ds + " Record for";
            }
        }

        protected void InitialView()
        {
            pnlSetupAuditPeriod.Visible = true;
            pnlSetupAuditPeriodForMultiples.Visible = false;
            pnlAuditPeriod.Visible = false;
            pnlUploadDocs.Visible = false;

            if (currAudit == null)
            {
                rdpBeginDt1.SelectedDate = null;
                rdpEndDt1.SelectedDate = null;
                rdpBeginDt2.SelectedDate = null;
                rdpEndDt2.SelectedDate = null;
                rtbAuditReason.Text = "";
                rtbDataAudited.Text = "";
                rtbAuditFindings.Text = "";
                reSANAL.Content = "";
            }
            else
            {
                rdpBeginDt1.SelectedDate = currAudit.audit_beg_dt;
                rdpEndDt1.SelectedDate = currAudit.audit_end_dt;
                rdpBeginDt2.SelectedDate = currAudit.audit_beg_dt;
                rdpEndDt2.SelectedDate = currAudit.audit_end_dt;
                rtbAuditReason.Text = currAudit.audit_reason;
                rtbDataAudited.Text = currAudit.audit_data;
                rtbAuditFindings.Text = currAudit.audit_findings;
                reSANAL.Content = "";
            }

            LoadPeriodList();
            LoadOfficeList();
            LoadFieldTripList();
            LoadRecordList();
        }

        protected void LoadPeriodList()
        {
            if (currRecord == null)
            {
                //If coming to this interface without passing a rms_record_id, then we have no way of knowing the single record for which the audit period is being created
                //So do not allow the option for selecting record periods to include in the audit
                pnlSetupAuditPeriodForMultiples.Visible = true;
                pnlSetupAuditPeriod.Visible = false;
                rbReturnToSingle.Visible = false;
            }
            else
            {
                //Grab all of the approved periods for the record
                rlbRecordPeriods.DataSource = currRecord.RecordAnalysisPeriods
                    .Where(p => p.status_va == "Approved")
                    .Select(p => new { period_id = p.period_id, PeriodDates = String.Format("{0:MM/dd/yyyy} - {1:MM/dd/yyyy}", p.period_beg_dt, p.period_end_dt), period_beg_dt = p.period_beg_dt })
                    .OrderBy(p => p.period_beg_dt).ToList();
                rlbRecordPeriods.DataBind();
            }
        }

        protected void LoadRecordList()
        {
            if (!string.IsNullOrEmpty(rddlOffice.SelectedValue))
            {
                if (!string.IsNullOrEmpty(rddlFieldTrip.SelectedValue))
                {
                    rlbRecords.DataSource = db.Records
                    .Where(p => p.Site.TripSites.Select(t => t.trip_id).Contains(Convert.ToInt32(rddlFieldTrip.SelectedValue)))
                    .Select(p => new
                    {
                        rms_record_id = p.rms_record_id,
                        record_nm = p.Site.site_no + " - " + p.RecordType.type_ds
                    }).ToList();
                }
                else
                {
                    rlbRecords.DataSource = db.Records
                    .Where(p => p.RecordAltOffice.alt_office_id == Convert.ToInt32(rddlOffice.SelectedValue) || p.Site.office_id == Convert.ToInt32(rddlOffice.SelectedValue))
                    .Select(p => new
                    {
                        rms_record_id = p.rms_record_id,
                        record_nm = p.Site.site_no + " - " + p.RecordType.type_ds
                    }).ToList();
                }
            }
            else
            {
                rlbRecords.DataSource = db.Records
                .Where(p => p.Site.Office.wsc_id == WSCID)
                .Select(p => new
                {
                    rms_record_id = p.rms_record_id,
                    record_nm = p.Site.site_no + " - " + p.RecordType.type_ds
                }).ToList();
            }

            rlbRecords.DataBind();

            if (RecordID > 0)
            {
                RadListBoxItem item1 = rlbRecords.Items.Where(p => p.Value == RecordID.ToString()).FirstOrDefault();

                if (item1 != null)
                {
                    item1.Checked = true;
                    rlbRecords.Items.Remove(item1);
                    rlbRecords.Items.Insert(0, item1);
                }
                else
                {
                    RadListBoxItem item2 = new RadListBoxItem() { Value = RecordID.ToString(), Text = currRecord.Site.site_no + " - " + currRecord.RecordType.type_ds };
                    item2.Checked = true;
                    rlbRecords.Items.Insert(0, item2);
                }
                rlbRecords.SelectedIndex = 0;
            }

            if (currAudit != null)
            {
                int x = 0;
                foreach (var rec in currAudit.AuditRecords)
                {
                    RadListBoxItem item3 = rlbRecords.Items.Where(p => p.Value == rec.rms_record_id.ToString()).FirstOrDefault();

                    if (item3 != null)
                    {
                        item3.Checked = true;
                        rlbRecords.Items.Remove(item3);
                        rlbRecords.Items.Insert(x, item3);
                    }
                    else
                    {
                        var record = db.Records.FirstOrDefault(p => p.rms_record_id == rec.rms_record_id);
                        RadListBoxItem item4 = new RadListBoxItem() { Value = rec.rms_record_id.ToString(), Text = record.Site.site_no + " - " + record.RecordType.type_ds };
                        item4.Checked = true;
                        rlbRecords.Items.Insert(x, item4);
                    }
                    x += 1;
                }
            }
        }

        protected void LoadOfficeList()
        {
            rddlOffice.DataSource = db.Offices.Where(p => p.wsc_id == WSCID).ToList();
            rddlOffice.DataBind();
            rddlOffice.Items.Insert(0, new DropDownListItem { Value = "", Text = "" });
        }

        protected void LoadFieldTripList()
        {
            if (!string.IsNullOrEmpty(rddlOffice.SelectedValue))
            {
                rddlFieldTrip.Enabled = true;
                rddlFieldTrip.DataSource = db.Trips.Where(p => p.office_id == Convert.ToInt32(rddlOffice.SelectedValue)).Select(p => new
                {
                    trip_id = p.trip_id,
                    trip_nm = p.trip_nm + " (" + p.user_id + ")"
                }).ToList();
                rddlFieldTrip.DataBind();
                rddlFieldTrip.Items.Insert(0, new DropDownListItem { Value = "", Text = "" });
            }
            else rddlFieldTrip.Enabled = false;
        }
        #endregion

        #region In Page Events
        protected void lbMultiple_Command(object sender, CommandEventArgs e)
        {
            pnlSetupAuditPeriod.Visible = false;
            pnlSetupAuditPeriodForMultiples.Visible = true;
        }

        protected void rbReturnToSingle_Command(object sender, CommandEventArgs e)
        {
            pnlSetupAuditPeriod.Visible = true;
            pnlSetupAuditPeriodForMultiples.Visible = false;
        }

        protected void FilterRecordList(object sender, EventArgs e)
        {
            RadDropDownList rddl = (RadDropDownList)sender;
            if (rddl.ID == "rddlOffice")
            {
                LoadFieldTripList();
                LoadRecordList();
            }
            else if (rddl.ID == "rddlFieldTrip")
            {
                LoadRecordList();
            }
        }

        protected void rbSubmitRecords_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument.ToString() == "multiple")
            {
                if (rlbRecords.CheckedItems.Count > 0)
                {
                    if (rdpBeginDt2.SelectedDate != null && rdpEndDt2.SelectedDate != null && rdpBeginDt2.SelectedDate < rdpEndDt2.SelectedDate)
                    {
                        pnlNotice.Visible = false;
                        pnlSetupAuditPeriodForMultiples.Visible = false;
                        pnlAuditPeriod.Visible = true;
                        SetupAuditForm("multiple");
                    }
                    else
                    {
                        pnlNotice.Visible = true;
                        ltlNotice.Text = "You must enter both a begin and end date for the audit period date range, and the begin date must be before the end date.";
                    }
                }
                else
                {
                    pnlNotice.Visible = true;
                    ltlNotice.Text = "You must check the box next to at least one record!";
                }
            }
            else
            {
                if ((rdpBeginDt1.SelectedDate != null && rdpEndDt1.SelectedDate != null && rdpBeginDt1.SelectedDate < rdpEndDt1.SelectedDate) || rlbRecordPeriods.CheckedItems.Count > 0)
                {
                    pnlNotice.Visible = false;
                    pnlSetupAuditPeriod.Visible = false;
                    pnlAuditPeriod.Visible = true;
                    SetupAuditForm("single");
                }
                else
                {
                    pnlNotice.Visible = true;
                    ltlNotice.Text = "You must either select a record period, or enter a valid begin and end date for the date range!";
                }
            }
        }

        protected void rlbViewRecords_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(rlbViewRecords.SelectedValue))
            {
                string pOut = "", edited_dt, edited_by_uid;
                var record = db.Records.FirstOrDefault(p => p.rms_record_id == Convert.ToInt32(rlbViewRecords.SelectedValue));

                DateTime? beg_dt, end_dt;
                if (rdpBeginDt1.SelectedDate != null)
                {
                    beg_dt = rdpBeginDt1.SelectedDate;
                    end_dt = rdpEndDt1.SelectedDate;
                }
                else if (rdpBeginDt2.SelectedDate != null)
                {
                    beg_dt = rdpBeginDt2.SelectedDate;
                    end_dt = rdpEndDt2.SelectedDate;
                }
                else
                {
                    beg_dt = PeriodDate("begin");
                    end_dt = PeriodDate("end");
                }
                var periods = record.RecordAnalysisPeriods.Where(p => p.period_end_dt >= beg_dt && p.period_end_dt <= end_dt).OrderByDescending(p => p.period_beg_dt).ToList();

                pOut = "Station Analyses for " + record.Site.site_no.Trim() + " " + db.vSITEFILEs.FirstOrDefault(s => s.site_id == record.Site.nwisweb_site_id).station_nm + "<br />" +
                        record.RecordType.type_ds + "<br />" +
                        "----------------------------------------------------------------------------------------------------------------------------------------<br />" +
                        "----------------------------------------------------------------------------------------------------------------------------------------<br /><br />";

                foreach (var period in periods)
                {
                    edited_dt = period.PeriodChangeLogs.Count() > 0 ? String.Format("{0}", period.PeriodChangeLogs.OrderByDescending(b => b.edited_dt).FirstOrDefault().edited_dt) : "unavailable";
                    edited_by_uid = period.PeriodChangeLogs.Count() > 0 ? period.PeriodChangeLogs.OrderByDescending(b => b.edited_dt).FirstOrDefault().edited_by_uid : "unavailable";

                    pOut += "Analysis Period: " + String.Format("{0:MM/dd/yyyy} to {1:MM/dd/yyyy}", period.period_beg_dt, period.period_end_dt) + "<br />" +
                        "Analysis:<br /><br />" +
                        period.analysis_notes_va.FormatParagraphOut() +
                        "Analysis for this period last updated " + edited_dt + " by " + edited_by_uid + "<br />" +
                        "Analyzed By: " + period.analyzed_by + " Date: " + String.Format("{0:MM/dd/yyyy}", period.analyzed_dt) + "<br />" +
                        "Approved By: " + period.approved_by + " Date: " + String.Format("{0:MM/dd/yyyy}", period.approved_dt) + "<br />" +
                        "----------------------------------------------------------------------------------------------------------------------------------------<br />" +
                        "----------------------------------------------------------------------------------------------------------------------------------------<br />";
                }

                reSANAL.Content = pOut;
            }
        }

        protected void CreateEditAudit(object sender, CommandEventArgs e)
        {
            int audit_type_id, audit_results_id;
            if (!string.IsNullOrEmpty(rddlAuditType.SelectedValue)) audit_type_id = Convert.ToInt32(rddlAuditType.SelectedValue); else audit_type_id = 0;
            if (!string.IsNullOrEmpty(rddlAuditResults.SelectedValue)) audit_results_id = Convert.ToInt32(rddlAuditResults.SelectedValue); else audit_results_id = 0;
            string audit_reason = rtbAuditReason.Text;
            string audit_data = rtbDataAudited.Text;
            string audit_findings = rtbAuditFindings.Text;

            if (audit_type_id == 0 || audit_results_id == 0 || string.IsNullOrEmpty(audit_reason) || string.IsNullOrEmpty(audit_data) || string.IsNullOrEmpty(audit_findings))
            {
                pnlError.Visible = true;
                ltlError.Text = "You must complete all form fields under the 'Audit the Period' section!";
            }
            else
            {
                DateTime? beg_dt, end_dt;
                if (rdpBeginDt1.SelectedDate != null)
                {
                    beg_dt = rdpBeginDt1.SelectedDate;
                    end_dt = rdpEndDt1.SelectedDate;
                }
                else if (rdpBeginDt2.SelectedDate != null)
                {
                    beg_dt = rdpBeginDt2.SelectedDate;
                    end_dt = rdpEndDt2.SelectedDate;
                }
                else
                {
                    beg_dt = PeriodDate("begin");
                    end_dt = PeriodDate("end");
                }

                if (e.CommandArgument.ToString() == "Add")
                {
                    Data.Audit new_audit = new Data.Audit()
                    {
                        audit_beg_dt = beg_dt,
                        audit_end_dt = end_dt,
                        audit_by = user.ID,
                        audit_dt = DateTime.Now,
                        audit_type_id = audit_type_id,
                        audit_results_id = audit_results_id,
                        audit_reason = audit_reason,
                        audit_data = audit_data,
                        audit_findings = audit_findings,
                        wsc_id = Convert.ToInt32(WSCID)
                    };
                    db.Audits.InsertOnSubmit(new_audit);
                    db.SubmitChanges();

                    foreach (RadListBoxItem item in rlbViewRecords.Items)
                    {
                        Data.AuditRecord new_audit_rec = new Data.AuditRecord()
                        {
                            rms_audit_id = new_audit.rms_audit_id,
                            rms_record_id = Convert.ToInt32(item.Value)
                        };
                        db.AuditRecords.InsertOnSubmit(new_audit_rec);
                        db.SubmitChanges();
                    }

                    rbSubmit.CommandArgument = new_audit.rms_audit_id.ToString();

                    //Send out emails to analysts and approvers
                    SendEmail("new", new_audit.rms_audit_id);
                    //Setup the rest of the page
                    pnlError.Visible = false;
                    pnlNotice.Visible = false;
                    pnlAuditPeriod.Visible = false;
                    ltlConfirm.Text = "Audit Period Created!";
                    ltlDone.Text = "To return and create a new audit period, click the 'Done' button at the bottom of the page.";
                    rbDone.CommandArgument = "stay";
                    pnlUploadDocs.Visible = true;
                    rlvAuditDocs.Visible = false;
                    ltlAlert.Text = "";
                }
                else
                {
                    currAudit.audit_beg_dt = beg_dt;
                    currAudit.audit_end_dt = end_dt;
                    currAudit.audit_by = user.ID;
                    currAudit.audit_dt = DateTime.Now;
                    currAudit.audit_type_id = audit_type_id;
                    currAudit.audit_results_id = audit_results_id;
                    currAudit.audit_reason = audit_reason;
                    currAudit.audit_data = audit_data;
                    currAudit.audit_findings = audit_findings;
                    currAudit.wsc_id = Convert.ToInt32(WSCID);

                    db.AuditRecords.DeleteAllOnSubmit(currAudit.AuditRecords);
                    db.SubmitChanges();

                    foreach (RadListBoxItem item in rlbViewRecords.Items)
                    {
                        Data.AuditRecord new_audit_rec = new Data.AuditRecord()
                        {
                            rms_audit_id = currAudit.rms_audit_id,
                            rms_record_id = Convert.ToInt32(item.Value)
                        };
                        db.AuditRecords.InsertOnSubmit(new_audit_rec);
                        db.SubmitChanges();
                    }

                    rbSubmit.CommandArgument = currAudit.rms_audit_id.ToString();

                    //Send out emails to analysts and approvers
                    SendEmail("edit", currAudit.rms_audit_id);
                    //Setup the rest of the page
                    pnlError.Visible = false;
                    pnlNotice.Visible = false;
                    pnlAuditPeriod.Visible = false;
                    ltlConfirm.Text = "Audit Period Updated!";
                    ltlDone.Text = "When finished uploading documents, clicking the 'Done' button below will return you to the Audit Report.";
                    rbDone.CommandArgument = "leave";
                    pnlUploadDocs.Visible = true;
                    //Create the list of uploaded documents
                    rlvAuditDocs.Visible = true;
                    rlvAuditDocs.DataSource = currAudit.AuditDocuments.Select(p => new { rms_audit_document_id = p.rms_audit_document_id, document_nm = p.document_nm }).OrderBy(p => p.document_nm);
                    rlvAuditDocs.DataBind();
                    ltlAlert.Text = "";
                }
            }
        }

        protected void StartOver(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument.ToString() == "leave")
                Response.Redirect(String.Format("{0}Report/Audit.aspx", Config.RMSURL));
            else
            {
                pnlNotice.Visible = false;
                pnlError.Visible = false;
                pnlUploadDocs.Visible = false;
                InitialView();
            }
        }

        protected void UploadDocument(object sender, CommandEventArgs e)
        {
            if (e.CommandName.ToString() == "UploadDoc")
            {
                var audit = db.Audits.FirstOrDefault(p => p.rms_audit_id == Convert.ToInt32(e.CommandArgument));

                if (rauAuditDoc.UploadedFiles.Count >= 1)
                {
                    var doc = new Data.AuditDocument();
                    var ud = rauAuditDoc.UploadedFiles[0];
                    doc.rms_audit_id = audit.rms_audit_id;
                    if (!string.IsNullOrEmpty(rtbName.Text)) doc.document_nm = rtbName.Text; else doc.document_nm = ud.FileName;
                    doc.document_desc = rtbDescription.Text;
                    doc.entered_dt = DateTime.Now;
                    doc.entered_by = user.ID;
                    doc.document_content_tp = ud.GetExtension().ToLower().Replace(".", "");
                    var file = doc.AuditDocumentFiles.FirstOrDefault();
                    //It's being added for the first time
                    if (file == null)
                    {
                        file = new Data.AuditDocumentFile();
                        using (MemoryStream ms = new MemoryStream())
                        {
                            ud.InputStream.CopyTo(ms);
                            file.document_file = ms.ToArray();
                        }
                        doc.AuditDocumentFiles.Add(file);
                    }
                    db.AuditDocuments.InsertOnSubmit(doc);
                    db.SubmitChanges();

                    ltlAlert.Text = "<span style=\"color:green;font-weight:bold;\">The document was successfully uploaded!</span>";
                    //Update the list of uploaded documents
                    rlvAuditDocs.Visible = true;
                    rlvAuditDocs.DataSource = audit.AuditDocuments.Select(p => new { rms_audit_document_id = p.rms_audit_document_id, document_nm = p.document_nm }).OrderBy(p => p.document_nm);
                    rlvAuditDocs.DataBind();
                    ClearFormFields();
                }
                else
                {
                    ltlAlert.Text = "<span style=\"color:red;font-weight:bold;\">A document is required!</span>";
                }
            }
        }
        #endregion

        #region Page Methods
        protected void SetupAuditForm(string type)
        {
            if (type == "single")
            {
                if (rlbRecordPeriods.CheckedItems.Count == 0)
                    ltlAuditDateRange.Text = String.Format("{0:MM/dd/yyyy} - {1:MM/dd/yyyy}", rdpBeginDt1.SelectedDate, rdpEndDt1.SelectedDate);
                else
                {
                    ltlAuditDateRange.Text = String.Format("{0:MM/dd/yyyy} - {1:MM/dd/yyyy}", PeriodDate("begin"), PeriodDate("end"));

                    bool consecutive = false;
                    bool theend = false;
                    foreach (RadListBoxItem item in rlbRecordPeriods.Items)
                    {
                        if (item.Checked && !consecutive) //will enter this part of the statement upon reaching the first checked item
                            consecutive = true;
                        else if (!item.Checked && consecutive) //will enter this part of the statement upon reaching the first unchecked item after a checked item
                            theend = true;
                        else if (item.Checked && consecutive && theend) //will enter this part of the statement if another checked period is found after a series of checked and unchecked - non-consecutive periods were selected!!
                        {
                            consecutive = false;
                            break;
                        }
                    }

                    if (!consecutive)
                    {
                        pnlNotice.Visible = true;
                        ltlNotice.Text = "<b>You did not select consecutive periods.</b> The audit date range is determined by the earliest selected period begin date and " +
                            "latest selected period end date, and this audit includes all periods within the audit date range.";
                    }
                }
                List<RecordItem> rec = new List<RecordItem>();
                rec.Add(new RecordItem() { rms_record_id = RecordID.ToString(), record_nm = currRecord.Site.site_no + " - " + currRecord.RecordType.type_ds });
                rlbViewRecords.DataSource = rec;
                rlbViewRecords.DataBind();
            }
            else
            {
                ltlAuditDateRange.Text = String.Format("{0:MM/dd/yyyy} - {1:MM/dd/yyyy}", rdpBeginDt2.SelectedDate, rdpEndDt2.SelectedDate);
                rlbViewRecords.DataSource = rlbRecords.CheckedItems.Select(p => new { rms_record_id = p.Value, record_nm = p.Text });
                rlbViewRecords.DataBind();
            }
            
            ltlAuditBy.Text = user.ID;
            
            rddlAuditType.DataSource = db.AuditTypes.Select(p => new { audit_type_id = p.audit_type_id, description = p.type + ": " + p.description }).ToList();
            rddlAuditType.DataBind();
            rddlAuditType.Items.Insert(0, new DropDownListItem { Value = "", Text = "" });

            rddlAuditResults.DataSource = db.AuditResults.Select(p => new { audit_results_id = p.audit_results_id, description = p.result + p.description }).ToList();
            rddlAuditResults.DataBind();
            rddlAuditResults.Items.Insert(0, new DropDownListItem { Value = "", Text = "" });

            if (currAudit != null)
            {
                rddlAuditResults.SelectedValue = currAudit.audit_results_id.ToString();
                rddlAuditType.SelectedValue = currAudit.audit_type_id.ToString();

                rbCreateEditAudit.Text = "Edit Audit Period";
                rbCreateEditAudit.CommandArgument = "Edit";
            }
            else
            {
                rddlAuditType.SelectedIndex = 0;
                rddlAuditResults.SelectedIndex = 0;

                rbCreateEditAudit.Text = "Create Audit";
                rbCreateEditAudit.CommandArgument = "Add";
            }
        }

        private DateTime? PeriodDate(string type)
        {
            DateTime? ret_dt;

            List<Data.RecordAnalysisPeriod> periods = new List<Data.RecordAnalysisPeriod>();
            foreach (RadListBoxItem item in rlbRecordPeriods.CheckedItems)
            {
                periods.Add(new Data.RecordAnalysisPeriod()
                {
                    period_id = Convert.ToInt32(item.Value),
                    period_beg_dt = db.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == Convert.ToInt32(item.Value)).period_beg_dt,
                    period_end_dt = db.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == Convert.ToInt32(item.Value)).period_end_dt
                });
            }

            if (type == "begin")
                ret_dt = periods.OrderBy(p => p.period_beg_dt).First().period_beg_dt;
            else
                ret_dt = periods.OrderByDescending(p => p.period_end_dt).First().period_end_dt;

            return ret_dt;
        }

        protected void ClearFormFields()
        {
            rtbName.Text = "";
            rtbDescription.Text = "";
        }
        
        protected void SendEmail(string action, int audit_id)
        {
            var audit = db.Audits.FirstOrDefault(p => p.rms_audit_id == audit_id);
            string recordList = "";
            var cc = new List<String>();
            var to = new List<String>();

            string timespan = String.Format("{0:MM/dd/yyyy} to {1:MM/dd/yyyy}", audit.audit_beg_dt, audit.audit_end_dt);
            //Create the text list of records involved with this audit
            var recordsInAudit = db.AuditRecords.Where(p => p.rms_audit_id == audit.rms_audit_id);
            foreach (var record in recordsInAudit)
            {
                var Record = db.Records.FirstOrDefault(p => p.rms_record_id == record.rms_record_id);
                recordList += Record.Site.site_no + " " + Record.Site.station_full_nm + ", " + Record.RecordType.type_ds + "<br />";

                //Create the to email list - send to analysts and approvers of all periods involved with this audit
                foreach (var period in Record.RecordAnalysisPeriods.Where(p => p.period_beg_dt >= audit.audit_beg_dt && p.period_end_dt <= audit.audit_end_dt))
                {
                    string analyst_email = EmailAddress(period.analyzed_by);
                    string approver_email = EmailAddress(period.approved_by);

                    if (!to.Contains(analyst_email)) to.Add(analyst_email);
                    if (!to.Contains(approver_email)) to.Add(approver_email);
                }

                if (to.Contains("@usgs.gov")) to.Remove("@usgs.gov");
            }

            using (var smtp = new SmtpClient() { Host = "smtp.usgs.gov" })
            {
                var message = new MailMessage("rmsonline@usgs.gov", "rmsonline@usgs.gov");
                message.IsBodyHtml = true;

                //Setup the office's approver email list
                var office = db.Offices.FirstOrDefault(p => p.office_id == OfficeID);
                string[] appEmails;
                List<string> appEmailList = new List<string>();
                if (!string.IsNullOrEmpty(office.reviewer_email))
                {
                    //If the approver email field contains multiple email addresses, split them up and put them into a List
                    if (office.reviewer_email.IndexOf(',') > 0 || office.reviewer_email.IndexOf(';') > 0)
                    {
                        char[] delimiterChars = { ',', ';' };
                        appEmails = office.reviewer_email.Split(delimiterChars);
                        foreach (string s in appEmails)
                        {
                            //Make sure to grab just the email address if formatted like so: "Cary Carman <cdcarman@usgs.gov>"
                            if (s.IndexOf('<') > 0 && s.IndexOf('>') > 0)
                                appEmailList.Add(s.Substring(s.IndexOf('<') + 1, s.IndexOf('>') - 1));
                            else
                                appEmailList.Add(s);
                        }
                    }
                    else
                    {
                        appEmailList.Add(office.reviewer_email);
                    }

                    //If one has been setup, CC the office's designated approver
                    if (appEmailList.Count > 0)
                    {
                        foreach (string email in appEmailList)
                            cc.Add(email);
                    }
                }
                
                switch (action)
                {
                    case "new":
                        message.Subject = "RMS: A new audit period was created";
                        message.Body = "A record period that you either analyzed or approved has been audited.<br /><br />" +
                            "The audit was performed for the time period of " + timespan + " and included the following records:<br />" + recordList;
                        break;
                    case "edit":
                        message.Subject = "RMS: An audit period was edited";
                        message.Body = "An audit that involves a record period that you either analyzed or approved has been edited.<br /><br />" +
                            "The audit encompasses time period of " + timespan + " and included the following records:<br />" + recordList;
                        break;
                }

#if DEBUG
                string to_string = "";
                string cc_string = "";
                foreach (string email in to)
                    to_string += email + ", ";
                foreach (string email in cc)
                    cc_string += email + ", ";
                to_string.TrimEnd(' ').TrimEnd(',');
                cc_string.TrimEnd(' ').TrimEnd(',');
                message.Body += "<br /><br />To Recipients: " + to_string + "<br />CC Recipients: " + cc_string;

                message.To.Add("dterry@usgs.gov");
                message.CC.Add("slvasque@usgs.gov");
#else
                foreach (string email in to)
                    message.To.Add(email);
                foreach (string email in cc)
                    message.CC.Add(email);
#endif

                smtp.Send(message);
            }
        }

        private string EmailAddress(string user_id)
        {
            //If possible, get the email address for the user from AD
            string email = user_id + "@usgs.gov";
            var reg_user = db.spz_GetUserInfoFromAD(user_id).ToList();
            foreach (var result in reg_user)
                email = result.mail;

            return email;
        }
        #endregion
    }
}