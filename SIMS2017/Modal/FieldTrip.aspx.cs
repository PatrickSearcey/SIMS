using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIMS2017.Modal
{
    public partial class FieldTrip : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            int trip_id = Convert.ToInt32(Request.QueryString["ID"]);
            Data.Trip trip = db.Trips.Where(p => p.trip_id == trip_id).FirstOrDefault();

            ltlTripName.Text = trip.trip_nm;
            dlSites.DataSource = trip.TripSites.Select(p => new { site_no = p.Site.site_no, station_nm = db.vSITEFILEs.FirstOrDefault(s => s.site_no == p.Site.site_no && s.agency_cd == p.Site.agency_cd).station_nm }).OrderBy(p => p.site_no);
            dlSites.DataBind();
        }
    }
}