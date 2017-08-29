using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIMS2017.Modal
{
    public partial class ConfirmDelete : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        private int ID;
        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            ID = Convert.ToInt32(Request.QueryString["ID"]);
        }

        protected void Page_Init(object sender, System.EventArgs e)
        {
            hfStatus.Value = "go";
            if (Request.QueryString["type"] == "fieldtrip")
                pnlFieldTrip.Visible = true;
        }

        public void DeleteInfo(object sender, CommandEventArgs e)
        {
            if (hfStatus.Value == "go")
            {
                if (e.CommandArgument == "deletetrip")
                {
                    var trip = db.Trips.FirstOrDefault(p => p.trip_id == ID);
                    db.TripSites.DeleteAllOnSubmit(trip.TripSites);
                    db.Trips.DeleteOnSubmit(trip);
                }

                db.SubmitChanges();
                hfStatus.Value = "done";
            }

            ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "CloseAndRebind();", true);
        }
    }
}