using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIMS2._0
{
    public partial class RMSWSCHome : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            var wsc = db.WSCs.FirstOrDefault(p => p.wsc_id == user.WSCID);
            ltlWSCName.Text = wsc.wsc_nm + " Water Science Center";
        }
    }
}