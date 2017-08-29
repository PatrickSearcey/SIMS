using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RMS.Modal
{
    public partial class Instructions : System.Web.UI.Page
    {
        private Data.SIMSDataContext db = new Data.SIMSDataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            string id = Request.QueryString["id"];
            string type = Request.QueryString["type"];

            int record_type_id = Convert.ToInt32(id);

            if (record_type_id > 0)
            {
                var recordType = db.RecordTypes.FirstOrDefault(p => p.record_type_id == record_type_id);
                var wsc = db.WSCs.FirstOrDefault(p => p.wsc_id == recordType.wsc_id);
                if (type == "Analyze")
                {
                    ltlSubTitle.Text = "For Analyzing " + recordType.type_ds + " Records for " + wsc.wsc_nm;
                    ltlInstructions.Text = recordType.analyze_html_va;
                }
                else
                {
                    ltlSubTitle.Text = "For Approving " + recordType.type_ds + " Records for " + wsc.wsc_nm;
                    ltlInstructions.Text = recordType.approve_html_va;
                }

            }
        }
    }
}