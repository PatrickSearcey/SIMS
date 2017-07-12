using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace RMS.Admin
{
    public partial class RecordTypes : System.Web.UI.Page
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

        }

        #region rgRecordTypes
        protected void rgRecordTypes_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            rgRecordTypes.DataSource = db.RecordTypes.Where(p => p.wsc_id == WSCID).OrderBy(p => p.type_ds).ToList();
        }

        protected void rgRecordTypes_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!user.IsAdmin) //If the user has no access, do not allow to edit
                {
                    rgRecordTypes.Columns.FindByUniqueName("EditCommandColumn").Visible = false;
                    rgRecordTypes.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
                }
                else if (user.IsAdmin && !user.IsSuperUser) //If the user is an Admin, but not a SuperUser
                {
                    //Then check to make sure they belong to the WSC for which they are attempting to modify record-types
                    if (user.WSCID.Contains(WSCID))
                    {
                        rgRecordTypes.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.Top;
                    }
                    else
                    {
                        rgRecordTypes.Columns.FindByUniqueName("EditCommandColumn").Visible = false;
                        rgRecordTypes.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
                    }
                }
                else //If the user is a SuperUser, give access
                {
                    rgRecordTypes.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.Top;
                }
            }
        }

        private void rgRecordTypes_UpdateCommand(object source, GridCommandEventArgs e)
        {
            GridEditableItem editedItem = (GridEditableItem)e.Item;
            UserControl MyUserControl = (UserControl)e.Item.FindControl(GridEditFormItem.EditFormUserControlID);

            int record_type_id = Convert.ToInt32(editedItem.GetDataKeyValue("record_type_id"));
            var rec_type = db.RecordTypes.FirstOrDefault(p => p.record_type_id == record_type_id);
            rec_type.type_cd = ((TextBox)MyUserControl.FindControl("tbCode")).Text;
            rec_type.type_ds = ((TextBox)MyUserControl.FindControl("tbDescription")).Text;

            try
            {
                rec_type.UpdateRecordTypeDetails();
                this.Session["RecordTypes"] = null;
                rgRecordTypes.Rebind();
            }
            catch (Exception ex)
            {
                Label lblError = new Label();
                lblError.Text = "Unable to update record-type. Reason: " + ex.Message;
                lblError.ForeColor = System.Drawing.Color.Red;
                rgRecordTypes.Controls.Add(lblError);

                e.Canceled = true;
            }
        }

        protected void rgRecordTypes_InsertCommand(object source, GridCommandEventArgs e)
        {
            UserControl userControl = (UserControl)e.Item.FindControl(GridEditFormItem.EditFormUserControlID);

            RecordType rec_type = new RecordType(0);
            rec_type.Code = Strings.Trim(((TextBox)userControl.FindControl("tbCode")).Text);
            rec_type.Description = ((TextBox)userControl.FindControl("tbDescription")).Text;
            rec_type.WorkerInstructions = ((TextBox)userControl.FindControl("tbWorkInst")).Text;
            rec_type.CheckerInstructions = ((TextBox)userControl.FindControl("tbCheckInst")).Text;
            rec_type.ReviewerInstructions = ((TextBox)userControl.FindControl("tbReviewInst")).Text;
            rec_type.WSCID = w.ID;
            string cont_va = null;

            try
            {
                cont_va = ((RadioButtonList)userControl.FindControl("rblContorNoncont")).SelectedItem.Value;
                if (cont_va == "cont")
                {
                    rec_type.TimeSeriesFlag = true;
                }
                else if (cont_va == "noncont")
                {
                    rec_type.TimeSeriesFlag = false;
                }
            }
            catch (Exception ex)
            {
                Label lblError = new Label();
                lblError.Text = "Unable to insert record-type. Reason: you must select either the time-series or non-time-series box.";
                lblError.ForeColor = System.Drawing.Color.Red;
                rgRecordTypes.Controls.Add(lblError);

                return;
            }


            try
            {
                RecordType rec_type_exist_test = new RecordType(rec_type.Code, rec_type.WSCID);
                if (rec_type_exist_test.ID == 0)
                {
                    rec_type.AddRecordType();
                    this.Session["RecordTypes"] = null;
                    rgRecordTypes.Rebind();
                }
                else
                {
                    ((Label)userControl.FindControl("lblError")).ForeColor = System.Drawing.Color.Red;
                    ((Label)userControl.FindControl("lblError")).Text = "Unable to insert record-type. Reason: a record-type with this code already exists";

                    e.Canceled = true;
                }

            }
            catch (Exception ex)
            {
                Label lblError = new Label();
                lblError.Text = "Unable to insert record-type. Reason: " + ex.Message;
                lblError.ForeColor = System.Drawing.Color.Red;
                rgRecordTypes.Controls.Add(lblError);

                e.Canceled = true;
            }
        }
        #endregion
    }
}