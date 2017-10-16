using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS.Handler
{
    /// <summary>
    /// Summary description for DialogsHandler
    /// </summary>
    public class DialogsHandler : IHttpHandler
    {
        private Data.SIMSDataContext db = new Data.SIMSDataContext();

        public void ProcessRequest(HttpContext context)
        {
            int period_id = Convert.ToInt32(context.Request.QueryString["period_id"]);

            var period = db.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == period_id);
            var site = period.Record.Site;

            string status = period.status_va;

            var period_dialogs = period.PeriodDialogs;
            List<Data.PeriodDialog> dialogs = new List<Data.PeriodDialog>();
            if (status == "Analyzed") dialogs = period_dialogs.Where(p => p.status_set_to_va == "Analyzed").ToList();
            else if (status == "Approved") dialogs = period_dialogs.Where(p => p.origin_va == "Approver").ToList();

            string pOut = null;

            pOut = "Dialogs for " + site.site_no + " " + db.vSITEFILEs.FirstOrDefault(s => s.site_id == site.nwisweb_site_id).station_nm + " - " + period.Record.RecordType.type_ds + "\r\nFile created on " + 
                DateTime.Now.ToShortDateString() + "\r\n\r\n";

            foreach (var item in dialogs)
            {
                pOut += "Dialog Date: " + String.Format("{0:MM/dd/yyyy}", item.dialog_dt) + "\r\n" + "Dialog Created by: " + item.dialog_by + "\r\n" +
                    "Status of Record at creation: " + item.status_set_to_va + "\r\n" + "Comments: " + item.comments_va + "\r\n\r\n";
            }

            context.Response.ContentType = "text/plain";
            context.Response.AddHeader("content-disposition", "attachment; filename=Dialogs_" + site.site_no.Trim() + ".txt");
            context.Response.Write(pOut);
            context.Response.Flush();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}