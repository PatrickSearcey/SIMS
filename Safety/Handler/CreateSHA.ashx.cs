using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Safety.Helper
{
    /// <summary>
    /// Summary description for CreateSHA
    /// </summary>
    public class CreateSHA : IHttpHandler
    {
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();

        public void ProcessRequest(HttpContext context)
        {
            int site_id = Convert.ToInt32(context.Request.QueryString["site_id"]);

            Data.SHA sha = new Data.SHA() {
                site_id = site_id,
                reviewed_by = user.ID,
                reviewed_dt = DateTime.Now
            };

            db.SHAs.InsertOnSubmit(sha);
            db.SubmitChanges();

            context.Response.Redirect(String.Format("{0}SHAView.aspx?site_id={1}", Config.SafetyURL, site_id));
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