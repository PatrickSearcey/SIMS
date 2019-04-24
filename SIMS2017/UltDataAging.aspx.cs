using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIMS2017
{
    public partial class UltDataAging : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        private int WSCID { get; set; }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            WSCID = Convert.ToInt32(Request.QueryString["wsc_id"]);

            List<Data.SP_CRP_Ult_Data_Aging_Table_LegacyResult> data = db.SP_CRP_Ult_Data_Aging_Table_Legacy(WSCID)
                .Select(p => new Data.SP_CRP_Ult_Data_Aging_Table_LegacyResult
                {
                    office_cd = p.office_cd,
                    site_no = p.site_no,
                    station_nm = p.station_nm,
                    parm_cd = p.parm_cd,
                    type_cd = p.type_cd,
                    category_no = p.category_no,
                    work_period_dt = p.work_period_dt,
                    check_period_dt = p.check_period_dt,
                    rev_period_dt = p.rev_period_dt,
                    last_aging_dt = p.last_aging_dt,
                    DaysSinceAging = p.DaysSinceAging
                }).ToList();

            gvCRP.DataSource = data;
            gvCRP.DataBind();
        }

    }
}