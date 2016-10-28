﻿using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIMS2017
{
    public partial class RMS : System.Web.UI.MasterPage
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            ltlUserID.Text = user.ID;
        }
    }
}