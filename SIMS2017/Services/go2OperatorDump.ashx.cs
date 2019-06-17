using Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SIMS2017.Services
{
    /// <summary>
    /// Summary description for go2OperatorDump
    /// </summary>
    public class go2OperatorDump : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string wsc_id = "0";
            string district_cd = "0";
            string cs = Config.ConnectionInfo;

            if (!string.IsNullOrEmpty(context.Request.QueryString["wsc_id"])) wsc_id = context.Request.QueryString["wsc_id"];
            if (!string.IsNullOrEmpty(context.Request.QueryString["district_cd"])) district_cd = context.Request.QueryString["district_cd"];

            SqlConnection cn = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("spz_Go2OperatorDump", cn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = Convert.ToInt32(wsc_id);
            cmd.Parameters.Add("@district_cd", SqlDbType.NVarChar, 15).Value = district_cd;

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            context.Response.ContentType = "text/plain";

            if (dt.Rows.Count == 0)
            {
                context.Response.Write("No records found");
            }
            else
            {
                foreach (DataRow row in dt.Rows)
                {
                    context.Response.Write(row["site_operator"].ToString() + "\n");
                }
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