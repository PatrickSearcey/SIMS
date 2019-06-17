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
    /// Summary description for SIMSHome
    /// </summary>
    public class SIMSHome : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string wsc_cd = "0";
            string district_cd = "0";
            string cs = Config.ConnectionInfo;

            if (!string.IsNullOrEmpty(context.Request.QueryString["wsc_cd"])) wsc_cd = context.Request.QueryString["wsc_cd"];
            if (!string.IsNullOrEmpty(context.Request.QueryString["district_cd"])) district_cd = context.Request.QueryString["district_cd"];

            SqlConnection cn = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("SP_SIMSHome", cn);
            
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@wsc_cd", SqlDbType.NVarChar, 15).Value = wsc_cd;
            cmd.Parameters.Add("@district_cd", SqlDbType.NVarChar, 15).Value = district_cd;

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
                context.Response.Write("district\toffice_cd\toffice_id\tsite_no\tstation_nm\tsite_tp_cd\tagency_use_cd\trecord_type\tfield_trip\tuser_id\tSLAP_auto\t\n");

                foreach (DataRow row in dt.Rows)
                {
                    string site_no = "";
                    string agency_cd = "";
                    string station_nm = "";
                    string office_cd = "";
                    string office_id = "";
                    string wsc_id = "";
                    wsc_cd = "";
                    string district_abbrev = "";
                    string site_tp_cd = "";
                    string agency_use_cd = "";
                    string trip_nm = "";
                    string user_id = "";
                    string type_ds = "";
                    string slap_auto = "";

                    site_no = row["site_no"].ToString();
                    agency_cd = row["agency_cd"].ToString();
                    station_nm = row["station_nm"].ToString();
                    office_cd = row["office_cd"].ToString();
                    office_id = row["office_id"].ToString();
                    wsc_id = row["wsc_id"].ToString();
                    wsc_cd = row["wsc_cd"].ToString();
                    district_abbrev = row["district_abbrev"].ToString();
                    site_tp_cd = row["site_tp_cd"].ToString();
                    agency_use_cd = row["agency_use_cd"].ToString();
                    trip_nm = row["trip_nm"].ToString();
                    user_id = row["user_id"].ToString();
                    type_ds = row["type_ds"].ToString();
                    slap_auto = row["sims_auto_rm"].ToString();

                    if (slap_auto == "True")
                        slap_auto = "Y";
                    else
                        slap_auto = "N";

                    context.Response.Write(district_abbrev + "\t" + office_cd + "\t" + office_id + "\t" + site_no + "\t" + station_nm + "\t" + site_tp_cd + "\t" + agency_use_cd + "\t" +
                        type_ds + "\t" + trip_nm + "\t" + user_id + "\t" + slap_auto + "\n");
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