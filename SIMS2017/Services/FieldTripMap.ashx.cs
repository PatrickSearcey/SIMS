using Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace SIMS2017.Services
{
    /// <summary>
    /// Summary description for FieldTripMap
    /// </summary>
    public class FieldTripMap : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string wsc_cd = context.Request["wsc_cd"];
            string office_id = context.Request["office_id"];
            string service = context.Request["service"];

            string cs = Config.ConnectionInfo;
            using (SqlConnection con = new SqlConnection(cs))
            {
                if (service == "trips")
                {
                    List<tripMapInfo> listTripMapInfo = new List<tripMapInfo>();

                    SqlCommand cmd = new SqlCommand("SP_trip_map", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (string.IsNullOrEmpty(wsc_cd)) wsc_cd = "0";
                    if (string.IsNullOrEmpty(office_id)) office_id = "0";

                    SqlParameter WSCCD = new SqlParameter()
                    {
                        ParameterName = "@wsc_cd",
                        Value = wsc_cd
                    };
                    cmd.Parameters.Add(WSCCD);

                    SqlParameter OfficeID = new SqlParameter()
                    {
                        ParameterName = "@office_id",
                        Value = office_id
                    };
                    cmd.Parameters.Add(OfficeID);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        tripMapInfo trip = new tripMapInfo();
                        trip.wsc_id = rdr["wsc_id"].ToString();
                        trip.wsc_cd = rdr["wsc_cd"].ToString();
                        trip.wsc_name = rdr["wsc_nm"].ToString();
                        trip.wsc_office_id = rdr["wsc_office_id"].ToString();
                        trip.office_id = rdr["office_id"].ToString();
                        trip.office_nm = rdr["office_nm"].ToString();
                        trip.office_trip_id = rdr["office_trip_id"].ToString();
                        trip.trip_id = rdr["trip_id"].ToString();
                        trip.trip = rdr["trip"].ToString();
                        trip.site_id = rdr["site_id"].ToString();
                        trip.dec_lat_va = rdr["dec_lat_va"].ToString();
                        trip.dec_long_va = rdr["dec_long_va"].ToString();
                        trip.agency_cd = rdr["agency_cd"].ToString();
                        trip.site_no = rdr["site_no"].ToString();
                        trip.station_nm = rdr["station_nm"].ToString();
                        trip.recordTypeList = rdr["recordTypeList"].ToString();
                        listTripMapInfo.Add(trip);
                    }

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    js.MaxJsonLength = Int32.MaxValue;
                    context.Response.Write(js.Serialize(listTripMapInfo));
                }
                else
                {
                    List<tripOfficeMapInfo> listTripOfficeInfo = new List<tripOfficeMapInfo>();

                    SqlCommand cmd = new SqlCommand("SP_trips_offices_map", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        tripOfficeMapInfo office = new tripOfficeMapInfo();
                        office.wsc_id = rdr["wsc_id"].ToString();
                        office.wsc_cd = rdr["wsc_cd"].ToString();
                        office.wsc_office_id = rdr["wsc_office_id"].ToString();
                        office.office_id = rdr["office_id"].ToString();
                        office.office_nm = rdr["office_nm"].ToString();
                        office.latitude = rdr["latitude"].ToString();
                        office.longitude = rdr["longitude"].ToString();
                        listTripOfficeInfo.Add(office);
                    }

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    js.MaxJsonLength = Int32.MaxValue;
                    context.Response.Write(js.Serialize(listTripOfficeInfo));
                }
            }

                
        }

        // we create a tripMapInfo class to hold the JSON value
        public class tripMapInfo
        {
            public string wsc_id { get; set; }
            public string wsc_cd { get; set; }
            public string wsc_name { get; set; }
            public string wsc_office_id { get; set; }
            public string office_id { get; set; }
            public string office_nm { get; set; }
            public string office_trip_id { get; set; }
            public string trip_id { get; set; }
            public string trip { get; set; }
            public string site_id { get; set; }
            public string dec_lat_va { get; set; }
            public string dec_long_va { get; set; }
            public string agency_cd { get; set; }
            public string site_no { get; set; }
            public string station_nm { get; set; }
            public string recordTypeList { get; set; }
        }

        // we create a tripOfficeMapInfo class to hold the JSON value
        public class tripOfficeMapInfo
        {
            public string wsc_id { get; set; }
            public string wsc_cd { get; set; }
            public string wsc_office_id { get; set; }
            public string office_id { get; set; }
            public string office_nm { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
        }

        // Converts the specified JSON string to an object of type T
        public T Deserialize<T>(string context)
        {
            string jsonData = context;

            //cast to specified objectType
            var obj = (T)new JavaScriptSerializer().Deserialize<T>(jsonData);
            return obj;
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