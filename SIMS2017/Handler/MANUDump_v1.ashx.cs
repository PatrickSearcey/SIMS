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

namespace SIMS2017.Handler
{
    /// <summary>
    /// Summary description for MANUDump_v1
    /// </summary>
    public class MANUDump_v1 : System.Web.IHttpHandler
    {
        private Data.SIMSDataContext db = new Data.SIMSDataContext();
        private const string site_no_qs = "site_no";
        private const string agency_cd_qs = "agency_cd";
        private const string wsc_id_qs = "wsc_id";
        private int wsc_id;
        private string site_no;
        private string agency_cd;
        private Data.Site site;
        private Data.WSC wsc;

        public void ProcessRequest(HttpContext context)
        {
            Setup(context);

            if (wsc_id > 0)
                dumpMANUsForWSC(wsc_id, context);
            else
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
            if (context.Request.QueryString[wsc_id_qs] != null)
                wsc_id = Convert.ToInt32(context.Request.QueryString[wsc_id_qs]);

            if (wsc_id > 0)
                wsc = db.WSCs.FirstOrDefault(p => p.wsc_id == wsc_id);
            else
                site = db.Sites.FirstOrDefault(p => p.site_no == site_no && p.agency_cd == agency_cd); //site = db.Sites.FirstOrDefault(p => p.site_no == "434307112382601" && p.agency_cd == "USGS");
        }

        /// <summary>
        /// Needed to implement IHttpHandler, we probably won't use it.
        /// </summary>
        public bool IsReusable
        {
            get { return false; }
        }

        protected void dumpMANUsForWSC(int wsc_id, HttpContext context)
        {
            context.Response.ContentType = "text/xml";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            XmlTextWriter XMLwrite = new XmlTextWriter(stream, System.Text.Encoding.UTF8);

            XMLwrite.WriteStartDocument();

            XMLwrite.WriteWhitespace(Environment.NewLine);
            XMLwrite.WriteStartElement("USGS");

            List<Data.Office> offices = wsc.Offices.ToList();

            foreach (var o in offices)
            {
                XMLwrite.WriteWhitespace(Environment.NewLine);
                XMLwrite.WriteStartElement("Office");

                XMLwrite.WriteWhitespace(Environment.NewLine);
                XMLwrite.WriteElementString("office_nm", o.office_nm);

                foreach (var site in o.Sites)
                {
                    try
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

                        XMLwrite.WriteWhitespace(Environment.NewLine);
                        XMLwrite.WriteStartElement("Manuscript");

                        XMLwrite.WriteWhitespace(Environment.NewLine);
                        XMLwrite.WriteElementString("Description", "Manuscript for site " + site.site_no + " " + db.vSITEFILEs.FirstOrDefault(s => s.site_id == site.nwisweb_site_id).station_nm);
                        XMLwrite.WriteWhitespace(Environment.NewLine);
                        XMLwrite.WriteElementString("site_no", site.site_no);
                        XMLwrite.WriteWhitespace(Environment.NewLine);
                        XMLwrite.WriteElementString("agency_cd", site.agency_cd);
                        XMLwrite.WriteWhitespace(Environment.NewLine);
                        XMLwrite.WriteElementString("station_full_nm", site.station_full_nm);

                        foreach (var x in lstElems.Where(p => p.ReportType == "MANU"))
                        {
                            strElementName = x.ElementName.ToString().Replace(" (MANU)", "");
                            strElementInfo = x.ElementInfo;

                            //Don't send EXTREMES FOR CURRENT YEAR element to NWIS
                            if (strElementName != "EXTREMES FOR CURRENT YEAR" && strElementName != "PEAK DISCHARGES FOR CURRENT YEAR")
                            {
                                XMLwrite.WriteWhitespace(Environment.NewLine);
                                XMLwrite.WriteStartElement("Element");

                                XMLwrite.WriteWhitespace(Environment.NewLine);
                                XMLwrite.WriteElementString("name", strElementName);

                                XMLwrite.WriteWhitespace(Environment.NewLine);
                                XMLwrite.WriteElementString("info", strElementInfo);

                                XMLwrite.WriteWhitespace(Environment.NewLine);
                                XMLwrite.WriteEndElement();
                            }
                            //End Element
                        }

                        XMLwrite.WriteWhitespace(Environment.NewLine);
                        XMLwrite.WriteEndElement();
                        //End Manuscript
                    }
                    catch (Exception ex) { }
                }

                XMLwrite.WriteWhitespace(Environment.NewLine);
                XMLwrite.WriteEndElement();
                //End Office
            }

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
            XMLwrite.WriteElementString("Description", "Manuscript for site " + site.site_no + " " + db.vSITEFILEs.FirstOrDefault(s => s.site_id == site.nwisweb_site_id).station_nm);
		    XMLwrite.WriteWhitespace(Environment.NewLine);
		    XMLwrite.WriteElementString("site_no", site.site_no);
		    XMLwrite.WriteWhitespace(Environment.NewLine);
		    XMLwrite.WriteElementString("agency_cd", site.agency_cd);
		    XMLwrite.WriteWhitespace(Environment.NewLine);
		    XMLwrite.WriteElementString("station_full_nm", site.station_full_nm);

		    foreach (var x in lstElems.Where(p => p.ReportType == "MANU")) 
            {
			    strElementName = x.ElementName.ToString().Replace(" (MANU)", "");
			    strElementInfo = x.ElementInfo;

			    //Don't send EXTREMES FOR CURRENT YEAR element to NWIS
			    if (strElementName != "EXTREMES FOR CURRENT YEAR" && strElementName != "PEAK DISCHARGES FOR CURRENT YEAR") 
                {
				    XMLwrite.WriteWhitespace(Environment.NewLine);
				    XMLwrite.WriteStartElement("Element");

				    XMLwrite.WriteWhitespace(Environment.NewLine);
				    XMLwrite.WriteElementString("name", strElementName);

				    XMLwrite.WriteWhitespace(Environment.NewLine);
				    XMLwrite.WriteElementString("info", strElementInfo);

				    XMLwrite.WriteWhitespace(Environment.NewLine);
				    XMLwrite.WriteEndElement();
			    }
			    //End Element
		    }

		    XMLwrite.WriteWhitespace(Environment.NewLine);
		    XMLwrite.WriteEndElement();
		    //End Manuscript

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