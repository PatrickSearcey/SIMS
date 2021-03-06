﻿using Core;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SIMS2017.Control
{
    public partial class SitePageHeading : System.Web.UI.UserControl
    {
        #region Local Variables
        private SIMSDataContext db = new SIMSDataContext();
        private WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private int WSCID
        {
            get
            {
                if (Session["WSCID"] == null) return 0; else return (int)Session["WSCID"];
            }
            set
            {
                Session["WSCID"] = value;
            }
        }
        private int OfficeID
        {
            get
            {
                if (Session["OfficeID"] == null) return 0; else return (int)Session["OfficeID"];
            }
            set
            {
                Session["OfficeID"] = value;
            }
        }
        private int TripID
        {
            get
            {
                if (Session["TripID"] == null) return 0; else return (int)Session["TripID"];
            }
            set
            {
                Session["TripID"] = value;
            }
        }
        private int SiteID
        {
            get
            {
                if (Session["SiteID"] == null) return 0; else return (int)Session["SiteID"];
            }
            set
            {
                Session["SiteID"] = value;
            }
        }
        #endregion

        #region Public Properties
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public Boolean ShowOfficeInfoPanel { get; set; }
        public void RefreshHeadingData()
        {
            hlPageSubTitle.Text = SubTitle;
            hlPageSubTitle.NavigateUrl = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMSURL, SiteID);
            SetupResponsibleOfficeInfo();
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ltlPageTitle.Text = Title;
                hlPageSubTitle.Text = SubTitle;
                if (SiteID == 0) hlPageSubTitle.NavigateUrl = String.Format("{0}SIMSWSCHome.aspx", Config.SIMSURL);
                else hlPageSubTitle.NavigateUrl = String.Format("{0}StationInfo.aspx?site_id={1}", Config.SIMSURL, SiteID);

                SetupResponsibleOfficeInfo();
            }
        }

        protected void SetupResponsibleOfficeInfo()
        {
            if (ShowOfficeInfoPanel)
            {
                pnlOfficeInfo.Visible = true;
                Office office = db.Offices.Where(p => p.office_id == OfficeID).FirstOrDefault();
                ltlOfficeInfo.Text = String.Format("<a href='{0}SIMSWSCHome.aspx?wsc_id={1}&office_id={2}'>{3}</a><br />{4}<br />{5}<br />{6}", Config.SIMSURL, WSCID, OfficeID, office.office_nm, office.street_addrs, office.city_st_zip, office.ph_no);
            }
            else pnlOfficeInfo.Visible = false;
        }
    }
}