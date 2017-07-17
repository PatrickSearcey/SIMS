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
        private Data.SIMSDataContext db = new Data.SIMSDataContext();

        public object DataItem
        {
            get { return this._dataItem; }
            set { this._dataItem = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            DataBinding += this.RecordTypeConfig_DataBinding;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void RecordTypeConfig_DataBinding(object sender, System.EventArgs e)
        {
            object contValue = DataBinder.Eval(DataItem, "ts_fg");
            object record_type_id = DataBinder.Eval(DataItem, "record_type_id");
            object TemplateID = DataBinder.Eval(DataItem, "TemplateID");
            object analyzeInstructions = DataBinder.Eval(DataItem, "analyze_html_va");
            object approveInstructions = DataBinder.Eval(DataItem, "approve_html_va");

            if (record_type_id.Equals(DBNull.Value))
                lblHeading.Text = "Add New Record-Type";
            else
                lblHeading.Text = "Edit Record-Type";

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

            List<Data.RecordTemplate> temps = new List<Data.RecordTemplate>();
            temps.Add(new Data.RecordTemplate { TemplateID = 0, TemplateName = "" });
            foreach (var temp in db.RecordTemplates)
            {
                temps.Add(temp);
            }
            rddlTemplates.DataSource = temps;
            rddlTemplates.DataBind();

            dlTemplates.DataSource = db.RecordTemplates.ToList();
            dlTemplates.DataBind();

            if (TemplateID != null && !TemplateID.Equals(DBNull.Value))
            {
                rddlTemplates.SelectedValue = TemplateID.ToString();
                lblRequired.Visible = false;
            }

            if (analyzeInstructions != null)
                rtbAnalyzeInstructions.Text = analyzeInstructions.ToString();

            if (approveInstructions != null)
                rtbApproveInstructions.Text = approveInstructions.ToString();

        }
    }
}