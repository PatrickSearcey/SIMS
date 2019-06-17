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
    /// Summary description for go2PASSDump
    /// </summary>
    public class go2PASSDump : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string wsc_id = "0";
            string cs = Config.ConnectionInfo;

            if (!string.IsNullOrEmpty(context.Request.QueryString["wsc_id"])) wsc_id = context.Request.QueryString["wsc_id"];

            SqlConnection cn = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("spz_Go2PASSDump", cn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = Convert.ToInt32(wsc_id);

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
                context.Response.Write("dcp_id\tsite_no\tstation_nm\tassigned_time\ttrans_interval\twindow\tprimary_ch\tprimary_bd\trandom_ch\trandom_bd\tsatellite\twsc_id\tcontact_id\tcontact_nm\tpass_station_nm\tant_azimuth\tant_elev\n");

                foreach (DataRow row in dt.Rows)
                {
                    wsc_id = "";
                    string dcp_id = "";
                    string site_no = "";
                    string station_nm = "";
                    string assigned_time = "";
                    string trans_interval = "";
                    string window = "";
                    string primary_ch = "";
                    string primary_bd = "";
                    string random_ch = "";
                    string random_bd = "";
                    string satellite = "";
                    string contact_id = "";
                    string contact_nm = "";
                    string pass_station_nm = "";
                    string ant_azimuth = "";
                    string ant_elev = "";

                    wsc_id = row["wsc_id"].ToString();
                    dcp_id = row["dcp_id"].ToString();
                    site_no = row["site_no"].ToString();
                    station_nm = row["station_nm"].ToString();
                    assigned_time = row["assigned_time"].ToString();
                    trans_interval = row["trans_interval"].ToString();
                    window = row["window"].ToString();
                    primary_ch = row["primary_ch"].ToString();
                    primary_bd = row["primary_bd"].ToString();
                    random_bd = row["random_bd"].ToString();
                    random_ch = row["random_ch"].ToString();
                    satellite = row["satellite"].ToString();
                    contact_id = row["contact_id"].ToString();
                    contact_nm = row["contact_nm"].ToString();
                    pass_station_nm = row["pass_station_nm"].ToString();
                    ant_azimuth = row["ant_azimuth"].ToString();
                    ant_elev = row["ant_elev"].ToString();

                    context.Response.Write(dcp_id + "\t" + site_no + "\t" + station_nm + "\t" + assigned_time + "\t" + trans_interval + "\t" + window + "\t" + primary_ch + "\t" + 
                        primary_bd + "\t" + random_ch + "\t" + random_bd + "\t" + satellite + "\t" + wsc_id + "\t" + contact_id + "\t" + contact_nm + "\t" + pass_station_nm + "\t" + 
                        ant_azimuth + "\t" + ant_elev + "\n");
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