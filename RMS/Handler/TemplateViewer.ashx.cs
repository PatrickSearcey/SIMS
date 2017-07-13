using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS.Handler
{
    /// <summary>
    /// Summary description for TemplateViewer
    /// </summary>
    public class TemplateViewer : IHttpHandler
    {
        private Data.SIMSDataContext db = new Data.SIMSDataContext();

        public void ProcessRequest(HttpContext context)
        {
            int TemplateID;
            string temp_ID = context.Request.QueryString["TemplateID"];

            if (!string.IsNullOrEmpty(temp_ID))
            {
                TemplateID = Convert.ToInt32(temp_ID);
                var template = db.RecordTemplates.FirstOrDefault(p => p.TemplateID == TemplateID);

                context.Response.ContentType = "text/html";
                context.Response.Write(template.TemplateText);
            }
            else 
            {
                context.Response.ContentType = "text/text";
                context.Response.Write("ERROR: Invalid TemplateID submitted.");
            }
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