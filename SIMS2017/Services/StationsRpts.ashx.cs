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
    /// Summary description for StationsRpts
    /// </summary>
    public class StationsRpts : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string wsc_id = "0";
            string office_id = "0";
            string cs = Config.ConnectionInfo;
            string where_stmt = "";

            if (!string.IsNullOrEmpty(context.Request.QueryString["wsc_id"])) wsc_id = context.Request.QueryString["wsc_id"];
            if (!string.IsNullOrEmpty(context.Request.QueryString["office_id"])) office_id = context.Request.QueryString["office_id"];

            if (office_id != "0")
            {
                where_stmt = " AND lo.office_id = " + office_id;
            }

            SqlConnection cn = new SqlConnection(cs);
            string sql = "SELECT s.site_no, s.agency_cd, s.station_nm, lo.office_cd" +
                " FROM SIMS_Site_Master AS ssm INNER JOIN lut_Office AS lo ON ssm.office_id = lo.office_id INNER JOIN" +
                " nwisweb.dbo.SITEFILE AS s ON ssm.nwisweb_site_id = s.site_id" +
                " WHERE ((agency_use_cd In('A','L','M')) OR (agency_use_cd Is Null) OR (agency_use_cd = '')) AND" +
                " (lo.wsc_id=" + wsc_id + ")" + where_stmt + " ORDER BY lo.office_cd, s.site_no";
            SqlCommand cmd = new SqlCommand(sql, cn);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            context.Response.ContentType = "text/plain";

            if (dt.Rows.Count == 0)
            {
                context.Response.Write("no sites returned");
            }
            else
            {
                foreach (DataRow row in dt.Rows)
                {
                    string site_no = "";
                    string agency_cd = "";
                    string station_nm = "";
                    string office_cd = "";

                    site_no = row["site_no"].ToString().Trim();
                    agency_cd = row["agency_cd"].ToString();
                    station_nm = row["station_nm"].ToString();
                    office_cd = row["office_cd"].ToString();

                    context.Response.Write("  " + office_cd + "   " + site_no + "   " + station_nm + "\n");
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