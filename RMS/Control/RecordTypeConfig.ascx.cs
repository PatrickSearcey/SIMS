using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace RMS.Control
{
    public partial class RecordTypeConfig : System.Web.UI.UserControl
    {
        private object _dataItem = null;

        public object DataItem
        {
            get { return this._dataItem; }
            set { this._dataItem = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (DataItem is GridInsertionObject)
            {
                btnInsert.Visible = true;
                btnUpdate.Visible = false;
            }
            else
            {
                btnInsert.Visible = false;
                btnUpdate.Visible = true;
            }
        }

        protected void RecordTypeConfigDetails_DataBinding(object sender, System.EventArgs e)
        {
            object contValue = DataBinder.Eval(DataItem, "ts_fg");
            object record_type_id = DataBinder.Eval(DataItem, "record_type_id");

            if (record_type_id.Equals(DBNull.Value))
            {
                lblHeading.Text = "Add New Record-Type";
            }
            else
            {
                lblHeading.Text = "Edit Record-Type";
            }

            if (contValue.ToString() == "True")
            {
                lblContorNoncont.Text = "Has been set to:";
                lblCONStatus.Text = "Time-series";
                lblCONStatus.Visible = true;
                rblContorNoncont.Visible = false;
                rfvContorNoncont.Visible = false;
            }
            else if (contValue.ToString() == "False")
            {
                lblContorNoncont.Text = "Has been set to:";
                lblCONStatus.Text = "Non-time-series";
                lblCONStatus.Visible = true;
                rblContorNoncont.Visible = false;
                rfvContorNoncont.Visible = false;
            }
            else if (contValue.Equals(DBNull.Value))
            {
                lblContorNoncont.Text = "Choose one:";
                rblContorNoncont.Visible = true;
                lblCONStatus.Visible = false;
                rfvContorNoncont.Visible = true;
            }
        }
    }
}