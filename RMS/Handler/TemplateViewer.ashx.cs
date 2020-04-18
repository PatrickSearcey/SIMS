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
            string type = context.Request.QueryString["type"];

            if (!string.IsNullOrEmpty(temp_ID))
            {
                TemplateID = Convert.ToInt32(temp_ID);
                var template = db.RecordTemplates.FirstOrDefault(p => p.TemplateID == TemplateID);

                context.Response.ContentType = "text/html";
                if (TemplateID == 10) context.Response.Write(template.TemplateName);
                else
                {
                    if (type == "analyze")
                    {
                        context.Response.Write(template.AnalyzeTemplateText);
                    }
                    else
                    {
                        context.Response.Write(template.ApproveTemplateText);
                    }
                }
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