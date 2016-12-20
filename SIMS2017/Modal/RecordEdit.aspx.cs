using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SIMS2017.Modal
{
    public partial class RecordEdit : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        private Data.Record record;
        private Data.Site site;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            string type = Request.QueryString["type"];
            int rms_record_id = Convert.ToInt32(Request.QueryString["rms_record_id"]);
            int site_id = Convert.ToInt32(Request.QueryString["site_id"]);

            if (rms_record_id > 0) record = db.Records.FirstOrDefault(p => p.rms_record_id == rms_record_id);
            if (site_id > 0) site = db.Sites.FirstOrDefault(p => p.site_id == site_id); else site = db.Sites.FirstOrDefault(p => p.site_id == record.site_id);

            if (!Page.IsPostBack)
            {
                if (type == "newrecord")
                {
                    ltlTitle.Text = "Add New Record";
                    ltlRecordType.Visible = false;
                    ltlSite.Text = site.site_no.Trim() + " " + site.station_full_nm;
                    pnlNewRecord.Visible = true;
                    pnlRecordNotUsed.Visible = false;
                    pnlEditRecord.Visible = false;
                    PopulateNewRecordPanel();
                }
                else if (type == "record")
                {
                    ltlTitle.Text = "Modify Record";
                    var dds = db.SP_RMS_Get_Record_DDs(record.rms_record_id);
                    if (dds.Count() > 1) ltlRecordType.Text = "Multi-Parameter, " + record.RecordType.type_ds + " Record for";
                    else
                    {
                        try { ltlRecordType.Text = db.SP_RMS_Get_Record_DDs(record.rms_record_id).FirstOrDefault().parm_nm + ", " + record.RecordType.type_ds + " Record for"; }
                        catch (Exception ex) { ltlRecordType.Text = record.RecordType.type_ds + " Record for"; }
                    }
                    ltlSite.Text = record.Site.site_no.Trim() + " " + record.Site.station_full_nm;
                    pnlNewRecord.Visible = false;
                    if ((bool)record.not_used_fg)
                    {
                        pnlRecordNotUsed.Visible = true;
                        pnlEditRecord.Visible = false;
                    }
                    else
                    {
                        pnlRecordNotUsed.Visible = false;
                        pnlEditRecord.Visible = true;
                    }
                    PopulateEditRecordPanel("editcurrentrecord");
                }
            }
        }

        #region Internal Classes
        internal class OptionItem
        {
            private string _option;
            private string _description;

            public string option
            {
                get { return _option; }
                set { _option = value; }
            }
            public string description
            {
                get { return _description; }
                set { _description = value; }
            }
            public OptionItem()
            {
                _option = option;
                _description = description;
            }
        }
        #endregion

        #region Page Load Events
        protected void PopulateNewRecordPanel()
        {
            List<OptionItem> options = new List<OptionItem>();
            var available_ids = db.SP_RMS_Get_Site_DDs(site.site_id, "");
            if (available_ids.Count() == 0)
            {
                ltlTopNote.Text = "There are no available time-series IDs registered in Aquarius for this site, so the record must be non-time-series.  If this is correct," +
                    " please check the box below to continue creating a non-time-series record.";
                ltlBottomNote.Text = "<b>Note:</b> If you are trying to create a time-series record but do not see the ID, you must register the daily value ID in nw_edit," +
                    " and manually push the data to NWISWeb. Your registered ID will be available for creating a record within an hour.";

                options.Add(new OptionItem() { option = "nonts_noid", description = "Non-Time-Series Record, no ID" });
                rcblOptions.DataSource = options;
                rcblOptions.DataBind();
            }
            else
            {
                ltlTopNote.Text = "Choose a classification for this new record:";
                ltlBottomNote.Visible = false;

                options.Add(new OptionItem() { option = "ts_id", description = "Time-Series Record, with ID" });
                options.Add(new OptionItem() { option = "nonts_id", description = "Non-Time-Series Record, with ID" });
                options.Add(new OptionItem() { option = "nonts_noid", description = "Non-Time-Series Record, no ID" });
                rcblOptions.DataSource = options;
                rcblOptions.DataBind();
            }
        }

        protected void PopulateEditRecordPanel(string option)
        {
            //Time-series IDs
            if (option == "nonts_noid") //Creating a non-ts record with no IDs
            {
                ltlTimeSeriesLabel.Visible = false;
                pnlAssignedIDs.Visible = false;
            }
            else //Creating or editing a non-ts or ts record with IDs
            {
                if (option == "editcurrentrecord") //editing a current record
                {
                    if (record.RecordDDs.Count() > 0)
                    {
                        ltlTimeSeriesLabel.Visible = true;
                        pnlAssignedIDs.Visible = true;
                        ltlAssignedIDs.Text = "";
                        var ts_ids = db.SP_RMS_Get_Record_DDs(record.rms_record_id);
                        foreach (var id in ts_ids) ltlAssignedIDs.Text += id.dd_ts_ds + ", ";
                        ltlAssignedIDs.Text.TrimEnd(' ').TrimEnd(',');
                    }
                    else
                    {
                        ltlTimeSeriesLabel.Visible = false;
                        pnlAssignedIDs.Visible = false;
                    }
                }
                else //creating a new record with IDs
                {
                    ltlTimeSeriesLabel.Visible = true;
                    pnlAssignedIDs.Visible = false;
                    pnlEditIDs.Visible = true;
                    SetupIDCheckBoxList();
                }
            }

            //Record-type drop down
            var ts_record_types = db.RecordTypes.Where(p => p.wsc_id == site.Office.wsc_id && p.ts_fg == true);
            var nonts_record_types = db.RecordTypes.Where(p => p.wsc_id == site.Office.wsc_id && p.ts_fg == false);
            if (option == "editcurrentrecord") //Editing record
            {
                if ((bool)record.RecordType.ts_fg)
                    rddlRecordTypes.DataSource = ts_record_types;
                else
                    rddlRecordTypes.DataSource = nonts_record_types;

                rddlRecordTypes.SelectedValue = record.record_type_id.ToString();
            }
            else if (option == "ts_id") //Creating a time-series record, so populate record-type drop down with time-series record types
                rddlRecordTypes.DataSource = ts_record_types;
            else //Creating a non-time-series record, so populate record-type drop down with non-time-series record types
                rddlRecordTypes.DataSource = nonts_record_types;

            rddlRecordTypes.DataBind();

            //Category Number
            if (option == "editcurrentrecord") //Show, and populate
            {
                if (record.RecordDDs.Count() > 0) //Show the category number stuff if there are IDs tied to the record
                {
                    rddlCatNumber.SelectedValue = record.category_no.ToString();
                    if (record.category_no > 1)
                    {
                        rtbCatReason.Visible = true;
                        rtbCatReason.Text = record.cat_reason;
                    }
                    else rtbCatReason.Visible = false;
                }
                else //Do not show if there are no IDs tied to the record - this means it's a non-time-series record with no IDs
                {
                    ltlCatNumberLabel.Visible = false;
                    rddlCatNumber.Visible = false;
                    rtbCatReason.Visible = false;
                }
            }
            else if (option == "ts_id") //Show, and enable
            {
                rtbCatReason.Visible = false;
            }
            else if (option == "nonts_id") //Show, but only allow them to add category number 3, and the canned reason
            {
                rddlCatNumber.SelectedValue = "3";
                rddlCatNumber.Enabled = false;
                rtbCatReason.Visible = true;
                rtbCatReason.Enabled = false;
                rtbCatReason.Text = "Record is non-time-series";
            }
            else //Do not show at all if adding a non-time-series record with no ID
            {
                ltlCatNumberLabel.Visible = false;
                rddlCatNumber.Visible = false;
                rtbCatReason.Visible = false;
            }

            //Analyzer & Approver
            var personnel = db.SP_Personnel_by_WSC_office_or_user_id(site.Office.wsc_id, 0, "", "no", "True", "no");
            rddlAnalyzer.DataSource = personnel;
            rddlAnalyzer.DataBind();
            rddlApprover.DataSource = personnel;
            rddlApprover.DataBind();
            if (option == "editcurrentrecord")
            {
                if (!string.IsNullOrEmpty(record.analyzer_uid)) rddlAnalyzer.SelectedValue = record.analyzer_uid; else rddlAnalyzer.Items.Insert(0, new DropDownListItem { Value = "", Text = "" });
                if (!string.IsNullOrEmpty(record.approver_uid)) rddlApprover.SelectedValue = record.approver_uid; else rddlApprover.Items.Insert(0, new DropDownListItem { Value = "", Text = "" });
            }
            
            //Approver email
            int office_id = Convert.ToInt32(site.office_id);
            if (option == "editcurrentrecord")
                if (record.RecordAltOffice != null) office_id = Convert.ToInt32(record.RecordAltOffice.alt_office_id);
            ltlApproverEmail.Text = db.Offices.FirstOrDefault(p => p.office_id == office_id).reviewer_email;

            //Field Trips
            string fieldtrips = "";
            foreach (var trip in site.TripSites.ToList()) fieldtrips += trip.Trip.trip_nm + " - " + trip.Trip.user_id + ", ";
            if (!string.IsNullOrEmpty(fieldtrips)) ltlFieldTrips.Text = fieldtrips.TrimEnd(' ').TrimEnd(','); else ltlFieldTrips.Text = "<i>none assigned</i>";

            //Responsible Office
            rddlResponsibleOffice.DataSource = db.Offices.Where(p => p.wsc_id == site.Office.wsc_id).ToList();
            rddlResponsibleOffice.DataBind();
            rddlResponsibleOffice.SelectedValue = office_id.ToString();

            //Checkboxesif 
            if (option == "editcurrentrecord")
            {
                rcbNotPublished.Checked = record.not_published_fg;
                rcbRecordInactive.Checked = record.not_used_fg;
            }

            //Other records Panel
            if (option == "editcurrentrecord")
                pnlOtherRecords.Visible = false;
            else
            {
                pnlOtherRecords.Visible = true;
                PopulateOtherRecordsPanel();
            }
        }

        protected void PopulateOtherRecordsPanel()
        {
            dgOtherRecords.DataSource = site.Records.Select(p => new
            {
                type_ds = p.RecordType.type_ds,
                analyzer_uid = p.analyzer_uid,
                approver_uid = p.approver_uid,
                status = GetActive((bool)p.not_used_fg)
            });
            dgOtherRecords.DataBind();
        }
        #endregion

        #region Time Series IDs
        protected void EditIDs(object sender, EventArgs e)
        {
            pnlAssignedIDs.Visible = false;
            pnlEditIDs.Visible = true;
            SetupIDCheckBoxList();
        }

        protected void SetupIDCheckBoxList()
        {
            hfEditIDs.Value = "true";
            var unused_ids = db.SP_RMS_Get_Site_DDs(site.site_id, "").Select(p => new { iv_ts_id = p.iv_ts_id, dd_ts_ds = p.dd_ts_ds }).ToList();

            rcblIDs.DataSource = unused_ids;
            rcblIDs.DataBind();

            if (record != null)
            {
                var assigned_ids = db.SP_RMS_Get_Record_DDs(record.rms_record_id).ToList();
                var ids = record.RecordDDs.Select(p => p.iv_ts_id).ToList();

                foreach (var id in assigned_ids)
                    rcblIDs.Items.Add(new ButtonListItem { Value = id.iv_ts_id.ToString(), Text = id.dd_ts_ds });

                foreach (ButtonListItem item in rcblIDs.Items)
                    if (ids.Contains(Convert.ToInt32(item.Value))) item.Selected = true;
            }
        }
        #endregion

        #region Misc Page Events
        protected void CreateNewRecord(object sender, EventArgs e)
        {
            pnlNewRecord.Visible = false;
            pnlEditRecord.Visible = true;

            PopulateEditRecordPanel(rcblOptions.SelectedValue);
        }
        
        protected void cbReactivate_CheckedChanged(object sender, EventArgs e)
        {
            rcbRecordInactive.Checked = false;
            pnlEditRecord.Visible = true;
            pnlRecordNotUsed.Visible = false;
        }

        protected void CategoryNumberChanged(object sender, DropDownListEventArgs e)
        {
            if (rddlCatNumber.SelectedValue.ToString() == "2" || rddlCatNumber.SelectedValue.ToString() == "3")
                rtbCatReason.Visible = true;
            else
                rtbCatReason.Visible = false;

        }
        #endregion

        #region Button Events
        protected void EditRecord(object sender, EventArgs e)
        {
            string error = GetErrorText();
            
            if (string.IsNullOrEmpty(error))
            {
                //If updating a current record
                if (record != null)
                {
                    //Update the time-series ID
                    if (hfEditIDs.Value == "true")
                    {
                        db.RecordDDs.DeleteAllOnSubmit(record.RecordDDs);
                        db.SubmitChanges();

                        foreach (ButtonListItem item in rcblIDs.Items)
                        {
                            if (item.Selected)
                            {
                                //Using the selected IDs iv_ts_id value, find the full details for the ID in the TS_ID_CACHE, and create a new entry in the Record DD table
                                var ts_id = db.vTS_ID_CACHEs.FirstOrDefault(p => p.iv_ts_id == Convert.ToInt32(item.Value));
                                Data.RecordDD id = new Data.RecordDD()
                                {
                                    rms_record_id = record.rms_record_id,
                                    iv_ts_id = ts_id.iv_ts_id,
                                    dd_nu = ts_id.adaps_dd_nu,
                                    gu_id = ts_id.gu_id
                                };
                                db.RecordDDs.InsertOnSubmit(id);
                                db.SubmitChanges();
                            }
                        }
                    }

                    //Update the record-type
                    record.record_type_id = Convert.ToInt32(rddlRecordTypes.SelectedValue);
                    //Update the category number and reason, but only for records with time-series DDs
                    if (record.RecordDDs.Count() > 0)
                    {
                        record.category_no = Convert.ToInt32(rddlCatNumber.SelectedValue);
                        record.cat_reason = rtbCatReason.Text;
                    }
                    //Update the analyzer and approver
                    record.analyzer_uid = rddlAnalyzer.SelectedValue;
                    record.approver_uid = rddlApprover.SelectedValue;
                    //Update the responsible office
                    if (record.RecordAltOffice != null)
                    {
                        //First delete the alt office that might be there
                        db.RecordAltOffices.DeleteOnSubmit(record.RecordAltOffice);
                        db.SubmitChanges();
                    }
                    //If an alt office was selected other than the site assigned office, add it to the alt office table
                    if (rddlResponsibleOffice.SelectedValue != record.Site.office_id.ToString())
                    {
                        db.RecordAltOffices.InsertOnSubmit(new Data.RecordAltOffice()
                        {
                            rms_record_id = record.rms_record_id,
                            site_id = record.site_id,
                            alt_office_id = Convert.ToInt32(rddlResponsibleOffice.SelectedValue)
                        });
                        db.SubmitChanges();
                    }
                    //Checkboxes
                    record.not_published_fg = rcbNotPublished.Checked;
                    record.not_used_fg = rcbRecordInactive.Checked;

                    db.SubmitChanges();
                }
                else //Create a new record
                {
                    Data.Record new_record = new Data.Record();

                    new_record.site_id = site.site_id;
                    new_record.analyzer_uid = rddlAnalyzer.SelectedValue;
                    new_record.approver_uid = rddlApprover.SelectedValue;
                    new_record.not_published_fg = rcbNotPublished.Checked;
                    new_record.not_used_fg = rcbRecordInactive.Checked;
                    new_record.record_type_id = Convert.ToInt32(rddlRecordTypes.SelectedValue);
                    if (hfEditIDs.Value == "true")
                    {
                        new_record.category_no = Convert.ToInt32(rddlCatNumber.SelectedValue);
                        new_record.cat_reason = rtbCatReason.Text;
                    }

                    db.Records.InsertOnSubmit(new_record);
                    db.SubmitChanges();

                    //Add the time-series ID
                    if (hfEditIDs.Value == "true")
                    {
                        foreach (ButtonListItem item in rcblIDs.Items)
                        {
                            if (item.Selected)
                            {
                                //Using the selected IDs iv_ts_id value, find the full details for the ID in the TS_ID_CACHE, and create a new entry in the Record DD table
                                var ts_id = db.vTS_ID_CACHEs.FirstOrDefault(p => p.iv_ts_id == Convert.ToInt32(item.Value));
                                Data.RecordDD id = new Data.RecordDD()
                                {
                                    rms_record_id = new_record.rms_record_id,
                                    iv_ts_id = ts_id.iv_ts_id,
                                    dd_nu = ts_id.adaps_dd_nu,
                                    gu_id = ts_id.gu_id
                                };
                                db.RecordDDs.InsertOnSubmit(id);
                                db.SubmitChanges();
                            }
                        }
                    }

                    //If an alt office was selected other than the site assigned office, add it to the alt office table
                    if (rddlResponsibleOffice.SelectedValue != site.office_id.ToString())
                    {
                        db.RecordAltOffices.InsertOnSubmit(new Data.RecordAltOffice()
                        {
                            rms_record_id = new_record.rms_record_id,
                            site_id = site.site_id,
                            alt_office_id = Convert.ToInt32(rddlResponsibleOffice.SelectedValue)
                        });
                        db.SubmitChanges();
                    }
                }
                

                ScriptManager.RegisterStartupScript(this, GetType(), "close", "CloseModal();", true);
            }
            else
            {
                ltlError.Visible = true;
                ltlError.Text = error;
            }
        }

        protected void Cancel(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "close", "CloseModal();", true);
        }
        #endregion

        #region Functions
        private string GetErrorText()
        {
            string ret = "";

            if (rcblIDs.SelectedValue.Count() == 0 && hfEditIDs.Value == "true") ret = "You must select at least one time-series ID!";
            if (rddlCatNumber.SelectedValue == "2" || rddlCatNumber.SelectedValue == "3")
                if (string.IsNullOrEmpty(rtbCatReason.Text)) ret += " You must enter a category reason!";

            return ret;
        }

        private string GetActive(bool active)
        {
            string ret = "active";

            if (active) ret = "inactive";

            return ret;
        }
        #endregion
    }
}