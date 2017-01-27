using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Data;

namespace SIMS2017.Control
{
    public partial class ApproveMANU : System.Web.UI.UserControl
    {
        #region Local Variables
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        private SIMSDevService.SIMSServiceClient svcSIMS = new SIMSDevService.SIMSServiceClient();
        public WindowsAuthenticationUser user = new WindowsAuthenticationUser();
        private Data.Site currSite;
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
        public object DataItem { get; set; }
        public bool ShowCloseButton 
        { 
            get
            {
                if (Session["ShowCloseButton"] == null) return true; else return Convert.ToBoolean(Session["ShowCloseButton"]);
            }
            set
            {
                Session["ShowCloseButton"] = value;
            }
        }
        public delegate void ApproveButtonClickEventHandler(object sender, CommandEventArgs e);
        public event ApproveButtonClickEventHandler SubmitEvent;
        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            object site_id = DataBinder.Eval(DataItem, "site_id");
            if (site_id != null)
            {
                Session.Clear();
                SiteID = Convert.ToInt32(site_id);
            }

            currSite = db.Sites.FirstOrDefault(p => p.site_id == SiteID);

            if (!ShowCloseButton) btnCancel.Visible = false;

            RefreshMANU();
        }

        #region Public Methods
        public void RefreshMANU()
        {
            //currSite = db.Sites.FirstOrDefault(p => p.site_id == SiteID);
            var ers = currSite.ElemReportSums.FirstOrDefault(p => p.report_type_cd == "MANU");
            var era = currSite.ElemReportApproves.FirstOrDefault(p => p.report_type_cd == "MANU");
            string needs_approval = "YES";

            if (era.approved_dt != null)
                if (ers.revised_dt > era.approved_dt) needs_approval = "YES"; else needs_approval = "NO";

            //If the user is not a WSC level admin or a SuperUser, or if the MANU does not need approval, then hide the approve button and approver comments textbox
            if (user.IsSuperUser == false && user.IsAdmin == false || needs_approval == "NO")
            {
                lblComments.Visible = false;
                rtbComments.Visible = false;
                btnApprove.Visible = false;
            }
            else
            {
                lblComments.Visible = true;
                rtbComments.Visible = true;
                btnApprove.Visible = true;
            }

            rlvElem.DataSource = null;
            rlvElem.Rebind();
        }
        #endregion

        #region RadListView Events
        protected void rlvElem_NeedDataSource(object source, RadListViewNeedDataSourceEventArgs e)
        {
#if DEBUG
            SIMSDevService.SIMSServiceClient svcSIMS = new SIMSDevService.SIMSServiceClient();
            List<SIMSDevService.Element> lstElems = new List<SIMSDevService.Element>();

            lstElems = svcSIMS.GetElementsBySiteAndReport(currSite.site_no, currSite.agency_cd, "MANU").Select(p => new SIMSDevService.Element
            {
                ElementID = p.ElementID,
                ElementName = p.ElementName.Replace(" (MANU)", ""),
                ElementInfo = p.ElementInfo,
                RevisedBy = p.RevisedBy,
                RevisedDate = p.RevisedDate,
                Remark = p.Remark,
                ReportTypeCd = p.ReportTypeCd,
                Priority = p.Priority
            }).ToList();
#else
            SIMSService.SIMSServiceClient svcSIMS = new SIMSService.SIMSServiceClient();
            List<SIMSService.Element> lstElems = new List<SIMSService.Element>();
            
            lstElems = svcSIMS.GetElementsBySiteAndReport(currSite.site_no, currSite.agency_cd, "MANU").Select(p => new SIMSService.Element {
                ElementID = p.ElementID,
                ElementName = p.ElementName.Replace(" (MANU)", ""),
                ElementInfo = p.ElementInfo,
                RevisedBy = p.RevisedBy,
                RevisedDate = p.RevisedDate,
                Remark = p.Remark,
                ReportTypeCd = p.ReportTypeCd,
                Priority = p.Priority
            }).ToList();

#endif

            rlvElem.DataSource = lstElems;
        }

        protected void rlvElem_ItemDataBound(object sender, RadListViewItemEventArgs e)
        {
            if (e.Item is RadListViewDataItem)
            {
                RadListViewDataItem item = (RadListViewDataItem)e.Item;
                
                int element_id = Convert.ToInt32(item.GetDataKeyValue("ElementID"));
                Label lblRevisedDt = (Label)item.FindControl("lblRevisedDt");
                Label lblRevisedBy = (Label)item.FindControl("lblRevisedBy");
                //Hide the EXTREMES FOR CURRENT YEAR, PEAK DISCHARGES FOR CURRENT YEAR and FOOTNOTE element by setting panel visibility to false
                //Change panel background color if WELL CHARACTERISTICS or DATUM element and add icon with tooltip
                Panel pnlElement = (Panel)item.FindControl("pnlElement");
                Image ibIcon = (Image)item.FindControl("ibIcon");
                //Create the Revision History link
                HyperLink hlRevisionHistory = (HyperLink)item.FindControl("hlRevisionHistory");

                var elem = currSite.SiteElements.FirstOrDefault(p => p.element_id == element_id);

                if (elem != null)
                {
                    lblRevisedDt.Text = String.Format("{0:MM/dd/yyyy}", elem.revised_dt);
                    lblRevisedBy.Text = elem.revised_by.ToString("N/A");

                    hlRevisionHistory.NavigateUrl = String.Format("{0}StationDoc/Archive.aspx?element_id={1}&site_id={2}", Config.SIMS2017URL, element_id, currSite.site_id);

                    //Don't show EXTREMES FOR CURRENT YEAR, PEAK DISCHARGES FOR CURRENT YEAR, or FOOTNOTES elements in MANU
                    if (elem.ElementDetail.element_nm.Contains("EXTREMES FOR CURRENT YEAR") || elem.ElementDetail.element_nm.Contains("PEAK DISCHARGES FOR CURRENT YEAR") || elem.ElementDetail.element_nm.Contains("FOOTNOTE"))
                        pnlElement.Visible = false;
                    else
                        pnlElement.Visible = true;

                    if (elem.ElementDetail.element_nm.Contains("WELL CHARACTERISTICS") || elem.ElementDetail.element_nm.Contains("DATUM"))
                    {
                        pnlElement.CssClass = "remarksElemAlert";
                        ibIcon.Visible = true;
                        ibIcon.ImageUrl = "../images/cleanicon.png";
                    }
                    else
                    {
                        pnlElement.CssClass = "";
                        ibIcon.Visible = false;
                    }

                    //For modal pop-ups to work
                    switch (elem.ElementDetail.element_nm)
                    {
                        case "WELL CHARACTERISTICS":
                            rwWC.OpenerElementID = ibIcon.ClientID;
                            break;
                        case "DATUM":
                            rwDatum.OpenerElementID = ibIcon.ClientID;
                            break;
                    }
                }
                else
                {
                    pnlElement.CssClass = "";
                    ibIcon.Visible = false;
                }
            }
        }
        #endregion

        protected void btnApprove_Command(object sender, CommandEventArgs e)
        {
            //currSite = db.Sites.FirstOrDefault(p => p.site_id == SiteID);
            var era = currSite.ElemReportApproves.FirstOrDefault(p => p.report_type_cd == "MANU");

            Data.ElementReportApproveBackup erab = new Data.ElementReportApproveBackup()
            {
                site_id = currSite.site_id,
                report_type_cd = "MANU",
                approved_by = era.approved_by,
                approved_dt = era.approved_dt,
                approver_comments = era.approver_comments,
                publish_complete = era.publish_complete
            };
            db.ElementReportApproveBackups.InsertOnSubmit(erab);

            era.approved_by = user.ID;
            era.approved_dt = DateTime.Now;
            era.approver_comments = rtbComments.Text;
            era.publish_complete = "N";

            db.SubmitChanges();

            lblComments.Visible = false;
            rtbComments.Text = "";
            rtbComments.Visible = false;
            btnApprove.Visible = false;

            if (SubmitEvent != null) SubmitEvent(this, e);
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("{0}StationDoc/EditDocs.aspx?site_id={1}", Config.SIMS2017URL, SiteID), "_blank", "");
        }

        protected void ibFAQ_Click(object sender, EventArgs e)
        {
            Response.Redirect("https://collaboration.usgs.gov/wg/owi/specialprojects/SIMS/Shared%20Documents/PADRE/FAQ.html", "_blank", "");
        }

        protected void ibRefresh_Command(object sender, CommandEventArgs e)
        {
            rlvElem.Rebind();

            if (SubmitEvent != null) SubmitEvent(this, e);
        }
    }
}