using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIMS2017.Handler
{
    /// <summary>
    /// Summary description for FieldTrips
    /// </summary>
    public class FieldTrips : IHttpHandler
    {
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        private const string wsc_cd_qs = "wsc_cd";
        private const string office_id_qs = "office_id";
        private const string office_cd_qs = "office_cd";
        private const string type_qs = "type";
        private string wsc_cd;
        private int office_id;
        private string office_cd;
        private string type;
        private Data.Office office;
        private Data.WSC wsc;

        public void ProcessRequest(HttpContext context)
        {
            Setup(context);

            if (office != null) generateFieldTripListByOffice(context);
            else if (wsc != null) generateFieldTripListByWSC(type, context);

            //Send everything now and close the server response.        
            context.Response.Flush();
        }

        /// <summary>
        /// Set up our environment based on current session or query string info.
        /// </summary>
        /// <param name="context"></param>
        private void Setup(HttpContext context)
        {
            if (context.Request.QueryString[type_qs] != null)
                type = context.Request.QueryString[type_qs];

            if (context.Request.QueryString[wsc_cd_qs] != null)
                wsc_cd = context.Request.QueryString[wsc_cd_qs];
            if (context.Request.QueryString[office_id_qs] != null)
                office_id = Convert.ToInt32(context.Request.QueryString[office_id_qs]);
            if (context.Request.QueryString[office_cd_qs] != null)
                office_cd = context.Request.QueryString[office_cd_qs];

            if (office_id > 0) office = db.Offices.FirstOrDefault(p => p.office_id == office_id);
            else if (!string.IsNullOrEmpty(office_cd)) office = db.Offices.FirstOrDefault(p => p.office_cd == office_cd);
            else if (!string.IsNullOrEmpty(wsc_cd)) wsc = db.WSCs.FirstOrDefault(p => p.wsc_cd == wsc_cd);

        }

        public void generateFieldTripListByOffice(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("wsc_cd\toffice_cd\tfield_trip\tsite_tp\tsite_no\tstation_nm\r\n");

            var tripList = office.Trips.OrderBy(p => p.trip_nm);

            foreach (var trip in tripList)
            {
                var siteList = trip.TripSites.OrderBy(p => p.Site.site_no);

                foreach (var site in siteList)
                {
                    string site_tp = "NA";
                    try
                    {
                        site_tp = db.vSITEFILEs.FirstOrDefault(p => p.site_id == site.Site.nwisweb_site_id).site_tp_cd;
                    }
                    catch (Exception ex) { }

                    context.Response.Write(office.WSC.wsc_cd + "\t" + office.office_cd + "\t" + trip.trip_nm + " - " + trip.user_id + "\t" + site_tp + "\t" + site.Site.site_no + "\t" + db.vSITEFILEs.FirstOrDefault(s => s.site_id == site.Site.nwisweb_site_id).station_nm + "\r\n");
                }
            }
        }

        public void generateFieldTripListByWSC(string type, HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (type == "dev")
                context.Response.Write("wsc_cd\toffice_cd\toffice_id\tsite_no\tstation_nm\tsite_tp_cd\tagency_use_cd\trecord_type\tfield_trip\tuser_id\r\n");
            else
                context.Response.Write("district\toffice_cd\toffice_id\tsite_no\tstation_nm\tsite_tp_cd\tagency_use_cd\trecord_type\tfield_trip\tuser_id\r\n");

            var officeList = wsc.Offices.OrderBy(p => p.office_cd);

            foreach (var office in officeList)
            {
                var siteList = office.Sites.OrderBy(p => p.site_no);

                foreach (var site in siteList)
                {
                    string site_tp = "NA";
                    string agency_use_cd = "NA";
                    try
                    {
                        var nwissite = db.vSITEFILEs.FirstOrDefault(p => p.site_id == site.nwisweb_site_id);
                        if (nwissite != null)
                        {
                            site_tp = nwissite.site_tp_cd;
                            agency_use_cd = nwissite.agency_use_cd.ToString();
                        }
                    }
                    catch (Exception ex) { }

                    var records = site.Records.OrderBy(p => p.RecordType.type_cd).ToList();
                    if (records.Count > 0)
                    {
                        foreach (var rec in records)
                        {
                            var trips = site.TripSites.OrderBy(p => p.Trip.trip_nm).ToList();
                            if (trips.Count > 0)
                            {
                                foreach (var trip in trips)
                                {
                                    context.Response.Write(wsc.wsc_cd + "\t" + site.Office.office_cd + "\t" + site.office_id.ToString() + "\t" + site.site_no + "\t" + db.vSITEFILEs.FirstOrDefault(s => s.site_id == site.nwisweb_site_id).station_nm + "\t" + site_tp + "\t" + agency_use_cd + "\t");
                                    if (type == "dev")
                                    {
                                        context.Response.Write(rec.RecordType.type_ds + "\t" + trip.Trip.Office.office_cd + "-" + trip.Trip.trip_nm + "\t" + trip.Trip.user_id + "\r\n");
                                    }
                                    else
                                    {
                                        context.Response.Write(rec.RecordType.type_ds + "\t" + trip.Trip.trip_nm + "\t" + trip.Trip.user_id + "\r\n");
                                    }
                                }
                            }
                            else
                            {
                                context.Response.Write(wsc.wsc_cd + "\t" + site.Office.office_cd + "\t" + site.office_id.ToString() + "\t" + site.site_no + "\t" + db.vSITEFILEs.FirstOrDefault(s => s.site_id == site.nwisweb_site_id).station_nm + "\t" + site_tp + "\t" + agency_use_cd + "\t");
                                context.Response.Write(rec.RecordType.type_ds + "\tNA\tNA\r\n");
                            }
                            
                        }
                    }
                    else
                    {
                        context.Response.Write(wsc.wsc_cd + "\t" + site.Office.office_cd + "\t" + site.office_id.ToString() + "\t" + site.site_no + "\t" + db.vSITEFILEs.FirstOrDefault(s => s.site_id == site.nwisweb_site_id).station_nm + "\t" + site_tp + "\t" + agency_use_cd + "\t");
                        context.Response.Write("NA\tNA\tNA\r\n");
                    }
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