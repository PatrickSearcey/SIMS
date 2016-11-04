using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SIMS2017.Modal
{
    public partial class FieldTripEdit : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            int site_id = Convert.ToInt32(Request.QueryString["site_id"]);
            Data.Site site = db.Sites.Where(p => p.site_id == site_id).FirstOrDefault();

            rlbFieldTripsStart.DataSource = db.Trips.Where(p => p.office_id == site.office_id).Select(p => new { TripName = p.trip_nm + " (" + p.user_id + ")", trip_id = p.trip_id }).ToList();
            rlbFieldTripsStart.DataBind();

            List<TripItem> trips = new List<TripItem>();
            foreach (var trip in site.TripSites)
            {
                //Remove the trips that are already assigned to the site from the start list box
                RadListBoxItem itemToRemove = rlbFieldTripsStart.FindItemByValue(trip.trip_id.ToString());
                rlbFieldTripsStart.Items.Remove(itemToRemove);

                //Add the trips that are already assigned to the site to the trips list
                trips.Add(new TripItem { trip_id = trip.trip_id, TripName = trip.Trip.trip_nm + " ( " + trip.Trip.user_id + ")" });
            }

            rlbFieldTripsEnd.DataSource = trips;
            rlbFieldTripsEnd.DataBind();
        }

        #region Internal Classes
        internal class TripItem
        {
            private string _trip_nm;
            private int? _trip_id;

            public string TripName
            {
                get { return _trip_nm; }
                set { _trip_nm = value; }
            }
            public int? trip_id
            {
                get { return _trip_id; }
                set { _trip_id = value; }
            }
            public TripItem()
            {
                _trip_id = trip_id;
                _trip_nm = TripName;
            }
        }
        #endregion
    }
}