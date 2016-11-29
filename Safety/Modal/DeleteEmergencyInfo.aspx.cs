using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Safety
{
    public partial class DeleteEmergencyInfo : System.Web.UI.Page
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private Data.Contact Contact;
        private Data.Hospital Hospital;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["hospital_id"]))
                Hospital = db.Hospitals.FirstOrDefault(p => p.hospital_id == Convert.ToInt32(Request.QueryString["hospital_id"]));
            else if (!string.IsNullOrEmpty(Request.QueryString["contact_id"]))
                Contact = db.Contacts.FirstOrDefault(p => p.contact_id == Convert.ToInt32(Request.QueryString["contact_id"]));
        }

        protected void Page_Init(object sender, System.EventArgs e)
        {
            this.Page.Title = "Deleting emergency info";
            hfStatus.Value = "go";
            if (!string.IsNullOrEmpty(Request.QueryString["hospital_id"]))
            {
                if (!string.IsNullOrEmpty(Request.QueryString["sha_site_id"]))
                {
                    pnlHospitalForSite.Visible = true;
                    pnlContactForSite.Visible = false;
                    pnlHospital.Visible = false;
                    pnlContact.Visible = false;
                }
                else
                {
                    pnlHospital.Visible = true;
                    pnlContact.Visible = false;
                    pnlHospitalForSite.Visible = false;
                    pnlContactForSite.Visible = false;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Request.QueryString["sha_site_id"]))
                {
                    pnlHospitalForSite.Visible = false;
                    pnlContactForSite.Visible = true;
                    pnlHospital.Visible = false;
                    pnlContact.Visible = false;
                }
                else
                {
                    pnlHospital.Visible = false;
                    pnlContact.Visible = true;
                    pnlHospitalForSite.Visible = false;
                    pnlContactForSite.Visible = false;
                }
            }
        }

        public void DeleteInfo(object sender, CommandEventArgs e)
        {
            if (hfStatus.Value == "go")
            {
                if (e.CommandArgument == "deletehospital")
                {
                    db.SHAHospitals.DeleteAllOnSubmit(db.SHAHospitals.Where(p => p.hospital_id == Hospital.hospital_id));
                    db.Hospitals.DeleteOnSubmit(Hospital);
                }
                else if (e.CommandArgument == "deletecontact")
                {
                    db.SHAContacts.DeleteAllOnSubmit(db.SHAContacts.Where(p => p.contact_id == Contact.contact_id));
                    db.Contacts.DeleteOnSubmit(Contact);
                }
                else if (e.CommandArgument == "deletehospitalforsite")
                {
                    db.SHAHospitals.DeleteOnSubmit(db.SHAHospitals.Where(p => p.hospital_id == Hospital.hospital_id && p.sha_site_id == Convert.ToInt32(Request.QueryString["sha_site_id"])).FirstOrDefault());
                }
                else if (e.CommandArgument == "deletecontactforsite")
                {
                    db.SHAContacts.DeleteOnSubmit(db.SHAContacts.Where(p => p.contact_id == Contact.contact_id && p.sha_site_id == Convert.ToInt32(Request.QueryString["sha_site_id"])).FirstOrDefault());
                }

                db.SubmitChanges();
                hfStatus.Value = "done";
            }

            ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "CloseAndRebind();", true);
        }
    }
}