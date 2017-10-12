using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.Data.Linq;
using System.Linq;
using Core;
using System.Data.SqlClient;

namespace SIMS2017
{
    /// <summary>
    /// Summary description for MANUDump_v2
    /// </summary>
    public class MANUDump_v2 : IHttpHandler
    {
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        private const string site_no_qs = "site_no";
        private const string agency_cd_qs = "agency_cd";
        private string site_no;
        private string agency_cd;
        private Data.Site site;

        public void ProcessRequest(HttpContext context)
        {
            Setup(context);

            createSiteMANUXML(site_no, agency_cd, context);

            //Send everything now and close the server response.        
            context.Response.Flush();
        }

        /// <summary>
        /// Set up our environment based on current session or query string info.
        /// </summary>
        /// <param name="context"></param>
        private void Setup(HttpContext context)
        {
            if (context.Request.QueryString[site_no_qs] != null)
                site_no = context.Request.QueryString[site_no_qs];
            if (context.Request.QueryString[agency_cd_qs] != null)
                agency_cd = context.Request.QueryString[agency_cd_qs];
            site = db.Sites.FirstOrDefault(p => p.site_no == site_no && p.agency_cd == agency_cd);
            //site = db.Sites.FirstOrDefault(p => p.site_no == "434307112382601" && p.agency_cd == "USGS");
        }

        /// <summary>
        /// Needed to implement IHttpHandler, we probably won't use it.
        /// </summary>
        public bool IsReusable
        {
            get { return false; }
        }

        protected void createSiteMANUXML(string site_no, string agency_cd, HttpContext context)
        {
	        List<ElementItem> lstElems = site.SiteElements.Select(p => new ElementItem
                {
                    ElementID = p.element_id.ToString(),
                    SiteID = p.site_id.ToString(),
                    ElementName = p.ElementDetail.element_nm,
                    ElementInfo = p.element_info.FormatElementInfo(Convert.ToInt32(p.element_id), site.site_id),
                    ReportType = db.ElementReportRefs.OrderBy(r => r.report_type_cd).FirstOrDefault(r => r.element_id == p.element_id).report_type_cd,
                    RevisedBy = p.revised_by,
                    RevisedDate = p.revised_dt.ToString(),
                    Priority = Convert.ToInt32(p.ElementDetail.priority)
                }).OrderBy(p => p.Priority).ToList();

	        string strElementName = "";
	        string strElementInfo = "";
	        string strRevisedDate = "";
	        string strStatus = "";
	        string pf5 = "";
	        string pf6 = "";

	        context.Response.ContentType = "text/xml";
	        context.Response.ContentEncoding = System.Text.Encoding.UTF8;
	        System.IO.MemoryStream stream = new System.IO.MemoryStream();
	        XmlTextWriter XMLwrite = new XmlTextWriter(stream, System.Text.Encoding.UTF8);

	        XMLwrite.WriteStartDocument();

	        XMLwrite.WriteWhitespace(Environment.NewLine);
	        XMLwrite.WriteStartElement("USGS");

	        XMLwrite.WriteWhitespace(Environment.NewLine);
	        XMLwrite.WriteStartElement("Manuscript");

	        XMLwrite.WriteWhitespace(Environment.NewLine);
            XMLwrite.WriteElementString("Description", "Manuscript for site " + site.site_no + " " + db.vSITEFILEs.FirstOrDefault(s => s.site_no == site.site_no && s.agency_cd == site.agency_cd).station_nm);
	        XMLwrite.WriteWhitespace(Environment.NewLine);
		    XMLwrite.WriteElementString("site_no", site.site_no);
		    XMLwrite.WriteWhitespace(Environment.NewLine);
		    XMLwrite.WriteElementString("agency_cd", site.agency_cd);
		    XMLwrite.WriteWhitespace(Environment.NewLine);
		    XMLwrite.WriteElementString("station_full_nm", site.station_full_nm);

            var reportApproved = site.ElemReportApproves.FirstOrDefault(p => p.report_type_cd == "MANU");
            var reportRevised = site.ElemReportSums.FirstOrDefault(p => p.report_type_cd == "MANU");

            //SqlCommand cmd = new SqlCommand("SELECT era.approved_dt, ers.revised_dt" + 
            //    " FROM Elem_Report_Approve AS era LEFT OUTER JOIN" + 
            //    " Elem_Report_Sum AS ers ON era.site_id = ers.site_id" + 
            //    " WHERE (ers.report_type_cd = 'MANU') AND era.site_id = " + s.ID.ToString(), cnx);
            //DataTable dt = new DataTable();

            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //da.Fill(dt);

		    if (reportApproved == null) {
			    XMLwrite.WriteWhitespace(Environment.NewLine);
			    XMLwrite.WriteElementString("status", "provisional");
		    } else {
			    DateTime approved_dt = default(DateTime);
			    DateTime revised_dt = default(DateTime);

                approved_dt = Convert.ToDateTime(reportApproved.approved_dt);
                if (reportRevised != null)
                {
                    revised_dt = Convert.ToDateTime(reportRevised.revised_dt);

                    if (revised_dt < approved_dt) strStatus = "approved";
                    else strStatus = "in review";
                }
                else strStatus = "approved";

				XMLwrite.WriteWhitespace(Environment.NewLine);
				XMLwrite.WriteElementString("status", strStatus);
		    }


            foreach (var x in lstElems.Where(p => p.ReportType == "MANU"))
            {
                strElementName = x.ElementName.ToString().Replace(" (MANU)", "");
                strElementInfo = x.ElementInfo;
		        strRevisedDate = string.Format("{0:MM/dd/yyyy}", x.RevisedDate);

                //Don't send EXTREMES FOR CURRENT YEAR element to NWIS
                if (strElementName != "EXTREMES FOR CURRENT YEAR" && strElementName != "PEAK DISCHARGES FOR CURRENT YEAR")
                {
                    XMLwrite.WriteWhitespace(Environment.NewLine);
                    XMLwrite.WriteStartElement("Element");

                    XMLwrite.WriteWhitespace(Environment.NewLine);
                    XMLwrite.WriteElementString("name", strElementName);

			        XMLwrite.WriteWhitespace(Environment.NewLine);
			        XMLwrite.WriteElementString("revised_dt", strRevisedDate);

			        XMLwrite.WriteWhitespace(Environment.NewLine);
			        XMLwrite.WriteElementString("info", strElementInfo);

			        XMLwrite.WriteWhitespace(Environment.NewLine);
			        XMLwrite.WriteEndElement();
		        }
		        //End Element
	        }

            foreach (var x in lstElems.Where(p => p.ElementID == "39" || p.ElementID == "40"))
            {
                strElementName = x.ElementName.ToString().Replace(" (MANU)", "");
                strElementInfo = x.ElementInfo;
                strRevisedDate = string.Format("{0:MM/dd/yyyy}", x.RevisedDate);

                XMLwrite.WriteWhitespace(Environment.NewLine);
                XMLwrite.WriteStartElement("Element");

                XMLwrite.WriteWhitespace(Environment.NewLine);
                XMLwrite.WriteElementString("name", strElementName);

                XMLwrite.WriteWhitespace(Environment.NewLine);
                XMLwrite.WriteElementString("revised_dt", strRevisedDate);

                XMLwrite.WriteWhitespace(Environment.NewLine);
                XMLwrite.WriteElementString("info", strElementInfo);

                XMLwrite.WriteWhitespace(Environment.NewLine);
                XMLwrite.WriteEndElement();
            }

	        XMLwrite.WriteWhitespace(Environment.NewLine);
	        XMLwrite.WriteEndElement();
	        //End Manuscript

	        XMLwrite.WriteWhitespace(Environment.NewLine);
	        XMLwrite.WriteStartElement("PeakFlowFile");

	        using (SqlConnection cnx = new SqlConnection(Config.ConnectionInfo)) {
		        cnx.Open();
		        SqlCommand cmd2 = new SqlCommand("SELECT pvt.site_no, pvt.pf5, pvt.pf6" + 
                    " FROM (SELECT ssm.site_no, 'pf5' AS data_in, 'X' AS data_va" + 
                    "   FROM OPENQUERY(a, 'select DISTINCT site_id from PEAK where (peak_cd like ''5'')') AS t INNER JOIN" + 
                    "   simsdb.dbo.SIMS_Site_Master AS ssm ON ssm.nwisweb_site_id = t.site_id" + 
                    "   UNION" + 
                    "   SELECT ssm.site_no, 'pf6' AS data_in, 'X' AS data_va" + 
                    "   FROM OPENQUERY(a, 'select DISTINCT site_id from PEAK where (peak_cd like ''6'')') AS t INNER JOIN" + 
                    "   simsdb.dbo.SIMS_Site_Master AS ssm ON ssm.nwisweb_site_id = t.site_id) as t1 PIVOT (MAX(data_va) FOR data_in IN ([pf5],[pf6])) AS pvt" + 
                    " WHERE site_no = '" + site.site_no + "'", cnx);
		        DataTable dt2 = new DataTable();

		        SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
		        da2.Fill(dt2);

		        foreach (DataRow row in dt2.Rows) {
			        if ((!object.ReferenceEquals(row["pf5"], DBNull.Value))) {
				        pf5 = row["pf5"].ToString();
			        } else {
				        pf5 = "false";
			        }
			        if ((!object.ReferenceEquals(row["pf6"], DBNull.Value))) {
				        pf6 = row["pf6"].ToString();
			        } else {
				        pf6 = "false";
			        }

			        if (pf5 == "X") {
				        pf5 = "true";
			        }
			        if (pf6 == "X") {
				        pf6 = "true";
			        }

			        XMLwrite.WriteWhitespace(Environment.NewLine);
			        XMLwrite.WriteElementString("pf5", pf5);

			        XMLwrite.WriteWhitespace(Environment.NewLine);
			        XMLwrite.WriteElementString("pf6", pf6);
		        }

		        cnx.Close();
	        }

	        XMLwrite.WriteWhitespace(Environment.NewLine);
	        XMLwrite.WriteEndElement();
	        //End PeakFlowFile

	        XMLwrite.WriteWhitespace(Environment.NewLine);
	        XMLwrite.WriteEndDocument();
	        //End USGS

	        XMLwrite.Flush();
	        System.IO.StreamReader reader = default(System.IO.StreamReader);
	        stream.Position = 0;
	        reader = new System.IO.StreamReader(stream);
	        Byte[] bytes = System.Text.Encoding.UTF8.GetBytes(reader.ReadToEnd());
	        context.Response.BinaryWrite(bytes);
	        context.Response.End();
        }

        #region Internal Classes
        internal class ElementItem
        {
            private string _ElementID;
            private string _SiteID;
            private string _ElementName;
            private string _ElementInfo;
            private string _ReportType;
            private string _RevisedBy;
            private string _RevisedDate;
            private int _Priority;

            public string ElementID
            {
                get { return _ElementID; }
                set { _ElementID = value; }
            }
            public string SiteID
            {
                get { return _SiteID; }
                set { _SiteID = value; }
            }
            public string ElementName
            {
                get { return _ElementName; }
                set { _ElementName = value; }
            }
            public string ElementInfo
            {
                get { return _ElementInfo; }
                set { _ElementInfo = value; }
            }
            public string ReportType
            {
                get { return _ReportType; }
                set { _ReportType = value; }
            }
            public string RevisedBy
            {
                get { return _RevisedBy; }
                set { _RevisedBy = value; }
            }
            public string RevisedDate
            {
                get { return _RevisedDate; }
                set { _RevisedDate = value; }
            }
            public int Priority
            {
                get { return _Priority; }
                set { _Priority = value; }
            }
            public ElementItem()
            {
                _ElementID = ElementID;
                _SiteID = SiteID;
                _ElementName = ElementName;
                _ElementInfo = ElementInfo;
                _RevisedBy = RevisedBy;
                _RevisedDate = RevisedDate;
                _Priority = Priority;
            }
        }
        #endregion
    }
}