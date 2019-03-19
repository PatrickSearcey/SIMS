using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Subgurim.Controles;
using Core;

namespace SIMS2017
{
    public partial class FieldTripMap : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        private Data.Trip currTrip;
        private int WSCID { get; set; }
        private int OfficeID { get; set; }
        private int TripID { get; set; }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            int TripID = Convert.ToInt32(Request.QueryString["trip_id"]);

            if (TripID > 0)
            {
                currTrip = db.Trips.FirstOrDefault(p => p.trip_id == TripID);
                OfficeID = (int)currTrip.office_id;
                WSCID = (int)currTrip.Office.wsc_id;

                lblTripName.Text = currTrip.trip_nm + " - " + currTrip.user_id;

                if (!Page.IsPostBack)
                {
                    SetupBasicGMapDisplay(OfficeID);
                    AddOfficeToMap(OfficeID);
                    AddSitesToMap(WSCID, OfficeID, TripID);
                }
            }
            else
            {
                //Show error message
            }
        }


        public void SetupBasicGMapDisplay(int office_id)
        {
            RepositionMap(office_id);
            GMap.enableHookMouseWheelToZoom = true;
            GMap.addControl(new GControl(GControl.preBuilt.LargeMapControl));
            GMap.addControl(new GControl(GControl.preBuilt.MapTypeControl));
            GMap.addControl(new GControl(GControl.preBuilt.GOverviewMapControl));
            GMap.addMapType(GMapType.GTypes.Physical);
            GMap.mapType = GMapType.GTypes.Physical;
            GMap.resetMarkers();
        }

        public void AddSitesToMap(int wsc_id, int office_id, int trip_id)
        {
            int rowCount = 0;
            double lat = 0;
            double latAvg = 0;
            double latMin = 0;
            double latMax = 0;
            double latPrev = 0;
            double lng = 0;
            double lngAvg = 0;
            double lngMin = 0;
            double lngMax = 0;
            double lngPrev = 0;
            int MapZoomLevel = 0;
            string site_tp_cd = null;
            GLatLng latlon = default(GLatLng);
            GMarker marker = default(GMarker);
            GInfoWindow window = default(GInfoWindow);
            string winText = "";
            string trip_nm = "";
            int i = 0;

            var SiteList = db.vFieldTripSites.Where(p => p.trip_id == trip_id).ToList();

            foreach (var site in SiteList)
            {
                string site_no = site.site_no.Trim();

                if ((!object.ReferenceEquals(site.dec_lat_va, DBNull.Value)))
                {
                    lat = (double)site.dec_lat_va;
                }
                if ((!object.ReferenceEquals(site.dec_long_va, DBNull.Value)))
                {
                    lng = (double)site.dec_long_va;
                }

                if (lat == 0 | lng == 0)
                {
                    i += 1;
                }
                else
                {
                    site_tp_cd = site.site_tp_cd;
                    trip_nm = site.trip_full_nm;

                    if (lat != latPrev & lng != lngPrev)
                    {
                        winText = "";
                    }
                    else
                    {
                        winText = winText + "<hr />";
                    }

                    winText = winText + "<b>" + site_no + " " + site.station_nm + "<br />" + 
                        trip_nm + "</b><br /><br />" + 
                        "<a href=\"https://sims.water.usgs.gov/SIMS/StationInfo.aspx?site_id=" + site.site_id.ToString() + "\" target=\"_blank\">SIMS Station Information Page</a><br />" + 
                        "<a href=\"http://waterdata.usgs.gov/nwis/nwisman/?site_no=" + site_no + "\" target=\"_blank\">NWIS Web</a>";

                    latAvg = latAvg + lat;
                    lngAvg = lngAvg + lng;
                    if (i == 0)
                    {
                        latMin = lat;
                        latMax = lat;
                        lngMin = lng;
                        lngMax = lng;
                    }
                    else
                    {
                        if (lat < latMin)
                            latMin = lat;
                        if (lat > latMax)
                            latMax = lat;
                        if (lng < lngMin)
                            lngMin = lng;
                        if (lng > lngMax)
                            lngMax = lng;
                    }

                    latlon = new GLatLng(lat, lng);
                    marker = new GMarker(latlon, GetMarkerOpts());
                    window = new GInfoWindow(marker, winText, false, GListener.Event.click);
                    GMap.addInfoWindow(window);

                    latPrev = lat;
                    lngPrev = lng;

                    i += 1;
                }
            }

            rowCount = SiteList.Count();

            if (rowCount == 0)
            {
                SetupBasicGMapDisplay(wsc_id);
            }
            else
            {
                //zoom/center map based on markers
                latAvg = (latAvg / i);
                lngAvg = (lngAvg / i);
                MapZoomLevel = GetMapZoomLevel(latMin, latMax, lngMin, lngMax);
                GMap.setCenter(new GLatLng(latAvg, lngAvg), MapZoomLevel);
            }
            lblResultsCount.Text = "Sites returned: " + rowCount.ToString();

        }

        public void AddOfficeToMap(int office_id)
        {
            var office = db.Offices.FirstOrDefault(p => p.office_id == office_id);

            GLatLng latlon = default(GLatLng);
            GMarker marker = default(GMarker);
            GInfoWindow window = default(GInfoWindow);
            string winText = null;

            winText = "<b>" + office.office_nm + "<br />" + office.street_addrs + "<br />" + office.city_st_zip + "<br />" + office.ph_no + "</b>";

            latlon = new GLatLng((double)office.dec_lat_va, (double)office.dec_long_va);
            marker = new GMarker(latlon);
            window = new GInfoWindow(marker, winText, false, GListener.Event.click);
            GMap.addInfoWindow(window);
        }

        public int GetMapZoomLevel(double latMin, double latMax, double lngMin, double lngMax)
        {
            int MapZoomLevel = 0;
            double latExtent = 0;
            double lngExtent = 0;

            latExtent = (latMax - latMin);
            lngExtent = (lngMax - lngMin);

            //simple lookup table for zoom levels
            if ((lngExtent > 100))
                MapZoomLevel = 3;
            if ((lngExtent > 40 & lngExtent < 100))
                MapZoomLevel = 3;
            if ((lngExtent > 15 & lngExtent < 40))
                MapZoomLevel = 4;
            if ((lngExtent > 4 & lngExtent < 15))
                MapZoomLevel = 5;
            if ((lngExtent > 1 & lngExtent < 4))
                MapZoomLevel = 6;
            if ((lngExtent > 0.5 & lngExtent < 1))
                MapZoomLevel = 7;
            if ((lngExtent > 0.25 & lngExtent < 0.5))
                MapZoomLevel = 8;
            if ((lngExtent > 0.125 & lngExtent < 0.25))
                MapZoomLevel = 9;
            if ((lngExtent < 0.125))
                MapZoomLevel = 10;

            return MapZoomLevel;
        }

        public GMarkerOptions GetMarkerOpts()
        {
            GIcon icon = new GIcon();
            
            //Create icon images for markers based on site type
            icon.image = "images/icons/8.png";
            icon.shadow = "http://wdr.water.usgs.gov/adrgmap/images/icons16x16/sw_onlys.png";

            icon.iconSize = new GSize(16, 16);
            icon.shadowSize = new GSize(26, 16);
            icon.iconAnchor = new GPoint(8, 8);
            icon.infoWindowAnchor = new GPoint(8, 0);

            GMarkerOptions mOpts = new GMarkerOptions();
            mOpts.clickable = true;
            mOpts.icon = icon;

            return mOpts;
        }
        
        public void RepositionMap(int office_id)
        {
            //Kept as a list only to retain legacy code for looping through all the offices in a WSC and finding the average lat/long
            var office = db.Offices.Where(p => p.office_id == office_id).ToList();

            double lat = 0;
            double latAvg = 0;
            double latMin = 0;
            double latMax = 0;
            double lng = 0;
            double lngAvg = 0;
            double lngMin = 0;
            double lngMax = 0;
            int MapZoomLevel = 0;

            int i = 0;
            foreach (var o in office)
            {
                lat = (double)o.dec_lat_va;
                lng = (double)o.dec_long_va;

                latAvg = latAvg + lat;
                lngAvg = lngAvg + lng;
                if (i == 0)
                {
                    latMin = lat;
                    latMax = lat;
                    lngMin = lng;
                    lngMax = lng;
                }
                else
                {
                    if (lat < latMin)
                        latMin = lat;
                    if (lat > latMax)
                        latMax = lat;
                    if (lng < lngMin)
                        lngMin = lng;
                    if (lng > lngMax)
                        lngMax = lng;
                }

                i += 1;
            }

            latAvg = (latAvg / i);
            lngAvg = (lngAvg / i);

            if (office_id > 0)
            {
                MapZoomLevel = 7;
            }
            else
            {
                MapZoomLevel = GetMapZoomLevel(latMin, latMax, lngMin, lngMax);
            }

            GMap.setCenter(new GLatLng(latAvg, lngAvg), MapZoomLevel);
        }

        public FieldTripMap()
        {
            Load += Page_Load;
        }
    }
}