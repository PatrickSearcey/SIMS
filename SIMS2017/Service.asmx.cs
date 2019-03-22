using Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace SIMS2017
{
    /// <summary>
    /// Summary description for Service
    /// </summary>
    [WebService(Namespace = "http://simsdev.usgs.gov/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Service : System.Web.Services.WebService
    {
        private Data.SIMSDataContext db = new Data.SIMSDataContext();

        [WebMethod(Description = "Gets Registered Site Information from SIMS using site_no and agency code")]
        public SiteInfo GetSiteInfo(string site_no, string agency_cd)
        {
            var site = db.Sites.Where(p => p.site_no == site_no && p.agency_cd == agency_cd).Select(p => new SiteInfo() {
                SiteID = p.site_id,
                NWISWebSiteID = Convert.ToInt32(p.nwisweb_site_id),
                AgencyCd = p.agency_cd,
                SiteNo = p.site_no,
                NWISHost = p.nwis_host,
                DBNO = p.db_no,
                StationName = p.station_full_nm,
                OfficeID = Convert.ToInt32(p.office_id),
                AltBasinName = p.alt_basin_nm,
                WSCID = Convert.ToInt32(p.Office.wsc_id),
                LevelsDt = (p.OpsLevel != null) ? p.OpsLevel.levels_dt : null,
                LevelsFreq = (p.OpsLevel != null) ? p.OpsLevel.levels_freq : null,
                LevelsClosed = (p.OpsLevel != null) ? p.OpsLevel.levels_closed : false
            }).FirstOrDefault();

            return site;
        }

        [WebMethod(Description = "Returns a list of site numbers and site ids by WSC")]
        public DataSet GetSiteNoSiteID(string wsc_id)
        {
            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);
            SqlDataAdapter da = new SqlDataAdapter("SELECT site_no, site_id FROM SIMS_Site_Master AS ssm INNER JOIN lut_Office AS lo ON lo.office_id = ssm.office_id WHERE lo.wsc_id = " + wsc_id + " ORDER BY lo.wsc_id, site_no", cn);

            DataSet ds = new DataSet();
            da.Fill(ds, "SiteNoSiteID");
            return ds;
        }

        [WebMethod(Description = "Gets Registered Site Information from SIMS using site_id")]
        public SiteInfo GetSiteInfoBySiteID(int site_id)
        {
            var site = db.Sites.Where(p => p.site_id == site_id).Select(p => new SiteInfo()
            {
                SiteID = p.site_id,
                NWISWebSiteID = Convert.ToInt32(p.nwisweb_site_id),
                AgencyCd = p.agency_cd,
                SiteNo = p.site_no,
                NWISHost = p.nwis_host,
                DBNO = p.db_no,
                StationName = p.station_full_nm,
                OfficeID = Convert.ToInt32(p.office_id),
                AltBasinName = p.alt_basin_nm,
                WSCID = Convert.ToInt32(p.Office.wsc_id),
                LevelsDt = (p.OpsLevel != null) ? p.OpsLevel.levels_dt : null,
                LevelsFreq = (p.OpsLevel != null) ? p.OpsLevel.levels_freq : null,
                LevelsClosed = (p.OpsLevel != null) ? p.OpsLevel.levels_closed : false
            }).FirstOrDefault();

            return site;
        }

        [WebMethod(Description = "Gets PASS Information for a site from SIMS")]
        public List<Data.DCPID> GetPASSInfoForSite(string site_no)
        {
            List<Data.DCPID> dcpids = new List<Data.DCPID>();
            dcpids = db.DCPIDs.Where(p => p.site_no == site_no).ToList();

            return dcpids;
        }

        [WebMethod(Description = "Gets a list of all the WSC's IDs and codes")]
        public DataSet GetAllWSCs()
        {
            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);
            SqlDataAdapter da = new SqlDataAdapter("SELECT wsc_id, wsc_cd FROM lut_WSC ORDER BY wsc_cd", cn);

            DataSet ds = new DataSet();
            da.Fill(ds, "allWSCs");
            return ds;
        }

        [WebMethod(Description = "Gets sites from a specified field trip")]
        public List<SiteTripInfo> GetFieldTripSites(int trip_id)
        {
            var tripsites = db.TripSites.Where(p => p.trip_id == trip_id).Select(p => new SiteTripInfo()
            {
                SiteID = Convert.ToInt32(p.site_id),
                SiteNo = p.Site.site_no,
                StationName = p.Site.station_full_nm,
                DecLatVa = db.vSITEFILEs.FirstOrDefault(s => s.site_id == p.Site.nwisweb_site_id).dec_lat_va,
                DecLongVa = db.vSITEFILEs.FirstOrDefault(s => s.site_id == p.Site.nwisweb_site_id).dec_long_va,
                AgencyCd = p.Site.agency_cd,
                SiteTypeCd = db.vSITEFILEs.FirstOrDefault(s => s.site_id == p.Site.nwisweb_site_id).site_tp_cd,
                TripName = p.Trip.trip_nm + " - " + p.Trip.user_id,
                TripID = p.Trip.trip_id
            }).ToList();
            return tripsites;
        }

        [WebMethod(Description = "Gets field trips by office")]
        public DataSet GetFieldTripsByOffice(int office_id)
        {
            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);
            SqlDataAdapter da = new SqlDataAdapter("SELECT tlt.trip_id, tlt.trip_nm + ' - ' + te.first_nm + ' ' + te.last_nm AS trip_name FROM Trip_Lut_Trip AS tlt INNER JOIN tblEmployees AS te ON tlt.user_id = te.user_id WHERE (tlt.office_id = " + office_id + ")", cn);

            DataSet ds = new DataSet();
            da.Fill(ds, "FieldTrips");
            return ds;
        }

        [WebMethod(Description = "Gets sites for a specified basin huc code")]
        public DataSet GetSitesByHUC(string basin_cd)
        {
            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);
            string huc_codes = null;

            switch (basin_cd)
            {
                case "RG": //Rio Grande
                    huc_codes = "'13030102','13040100','13040201','13040202','13040203','13040204','13040205','13040206','13040207','13040208','13040209','13040210','13040211','13040212','13040301','13040302','13040303','13050003','13050004','13060011','13070001','13070002','13070003','13070004','13070005','13070006','13070007','13070008','13070009','13070010','13070011','13080002','13080003','13090001','12110202','12110203','12110204','12110205','12110206','12110207','12110208'";
                    break;
                case "Nueces": //Nueces
                    huc_codes = "'12100406','12100407','12110201','12110101','12110102','12110103','12110104','12110105','12110106','12110107','12110108','12110109','12110110','12110111','12110202','12110203','12110206','12110207'";
                    break;
                case "SA": //San Antonio
                    huc_codes = "'12100301','12100405','12100302','12100303','12100304'";
                    break;
                case "Guad": //Guadalupe
                    huc_codes = "'12100201','12100202','12100203','12100204','12100404','12100403'";
                    break;
                case "Lav": //Lavaca
                    huc_codes = "'12100101','12100102','12100402','12100401'";
                    break;
                case "SJ": //San Jacinto
                    huc_codes = "'12040101','12040102','12040103','12040104','12040204','12040203'";
                    break;
                case "Nech": //Neches
                    huc_codes = "'12020001','12020002','12020003','12020004','12020005','12020006','12020007','12040201','12040202'";
                    break;
                case "CB": //Coastal Basins
                    huc_codes = "'12090401','12090402','12100401','12100402','12100403','12040201','12040202','12110202','12110203','12110204','12110205','12110206','12110207','12110208','12040204','12040205','12040203','12100405','12100406','12100407'";
                    break;
                case "CO": //Colorado
                    huc_codes = "'12080001','12080002','12080003','12080004','12080005','12080006','12080007','12080008','12090101','12090102','12090103','12090104','12090105','12090106','12090107','12090108','12090109','12090110','12090201','12090202','12090203','12090204','12090205','12090206','12090301','12090302','12100401','12090401','12090402'";
                    break;
                case "Brazos": //Brazos
                    huc_codes = "'12050004','12050001','12050002','12050003','12050005','12050006','12050007','12060101','12060102','12060103','12060104','12060105','12060201','12060202','12060203','12060204','12070101','12070102','12070103','12070104','12070201','12070202','12070203','12070204','12070205','12090401','12090402','12040205'";
                    break;
                case "Trin": //Trinity
                    huc_codes = "'12030101','12030102','12030103','12030104','12030105','12030106','12030107','12030108','12030109','12030201','12030202','12030203','12040203','12040202'";
                    break;
                case "Sab": //Sabine
                    huc_codes = "'12010001','12010002','12010003','12010004','12010005','12010005'";
                    break;
                case "Cyp": //Cypress
                    huc_codes = "'11140305','11140304','11140306','11140307'";
                    break;
                case "Sul": //Sulphur
                    huc_codes = "'11140301','11140302','11140303'";
                    break;
                case "Red": //Red
                    huc_codes = "'11120101','11120102','11120103','11120104','11120105','11120201','11120202','11120301','11120302','11120304','11130101','11130102','11130103','11130104','11130105','11130201','11130204','11130205','11130206','11130207','11130209','11130210','11140101','11140106','11140201'";
                    break;
                case "Ark": //Arkansas
                    huc_codes = "'11080006','11090101','11090102','11090103','11090104','11090105','11090106','11090201','11100101','11100103','11100104','11100201','11100202','11100203','11130301'";
                    break;
            }

            SqlDataAdapter da = new SqlDataAdapter("SELECT s.site_no, s.station_nm, s.huc_cd, s.agency_cd, s.dec_lat_va, s.dec_long_va, s.site_tp_cd, s.district_cd FROM nwisweb.dbo.SITEFILE AS s INNER JOIN nwisweb.dbo.rt_bol As b ON s.site_id = b.site_id WHERE s.district_cd = 48 AND s.huc_cd In(" + huc_codes + ")", cn);
            DataSet ds = new DataSet();
            da.Fill(ds, "hucSites");
            return ds;
        }

        [WebMethod(Description = "Gets list of active sites for a WSC or office from SIMS server")]
        public DataSet GetSitesByWSC(int wsc_id, int office_id, string site_type)
        {
            string where_stmt = null;
            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);

            if (office_id != 0)
            {
                where_stmt = " AND (lo.office_id=" + office_id + ")";
            }

            if (site_type != "0")
            {
                where_stmt = where_stmt + " AND (lst.sims_site_tp='" + site_type + "')";
            }

            SqlDataAdapter da = new SqlDataAdapter("SELECT DISTINCT s.site_no, s.agency_cd, s.station_nm, lo.office_cd," + 
                " lo.office_id, lo.wsc_id, s.site_tp_cd, s.agency_use_cd, ISNULL(tlt.trip_nm,'NA')," + 
                " ISNULL(tlt.user_id,'NA'), ISNULL(rst.type_ds,'NA')" + 
                " FROM RMS_Record_Master AS rsm INNER JOIN" + 
                " RMS_Record_Types AS rst ON rsm.record_type_id = rst.record_type_id RIGHT OUTER JOIN" + 
                " SIMS_Site_Master AS ssm INNER JOIN" + 
                " lut_Office AS lo ON ssm.office_id = lo.office_id INNER JOIN" + 
                " nwisweb.dbo.SITEFILE AS s ON ssm.nwisweb_site_id = s.site_id ON rsm.site_id = ssm.site_id LEFT OUTER JOIN" + 
                " Trip_Lut_Trip AS tlt INNER JOIN" + 
                " Trip_Site_Master AS tsm ON tlt.trip_id = tsm.trip_id ON ssm.site_id = tsm.site_id LEFT OUTER JOIN" + 
                " lut_site_tp AS lst ON s.site_tp_cd = lst.site_tp_cd" + 
                " WHERE (lo.wsc_id = " + wsc_id + ")" + where_stmt + 
                " ORDER BY lo.office_cd, s.site_no", cn);

            DataSet ds = new DataSet();
            da.Fill(ds, "sitelist");
            return ds;
        }

        [WebMethod(Description = "Gets list of sites for all WSCs from SIMS server")]
        public DataSet GetSitesInSIMS()
        {
            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);

            SqlDataAdapter da = new SqlDataAdapter("SELECT DISTINCT s.site_no, s.nwis_host, s.agency_cd, s.station_nm, lo.office_cd," +
                " lo.office_id, lo.wsc_id, lw.wsc_cd, s.site_tp_cd, s.agency_use_cd" + 
                " FROM SIMS_Site_Master AS ssm INNER JOIN" + 
                " lut_Office AS lo ON ssm.office_id = lo.office_id INNER JOIN lut_WSC AS lw ON lo.wsc_id = lw.wsc_id" + 
                " INNER JOIN nwisweb.dbo.SITEFILE AS s ON ssm.nwisweb_site_id = s.site_id " + 
                " ORDER BY lw.wsc_cd, s.site_no", cn);

            DataSet ds = new DataSet();
            da.Fill(ds, "sitelist");
            return ds;
        }

        [WebMethod(Description = "Gets information about an employee")]
        public DataSet GetEmployeeInfo(string user_id)
        {
            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);
            SqlDataAdapter da = new SqlDataAdapter("SP_Personnel_by_WSC_office_or_user_id", cn);

            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@wsc_id", SqlDbType.Int).Value = 0;
            da.SelectCommand.Parameters.Add("@office_id", SqlDbType.Int).Value = 0;
            da.SelectCommand.Parameters.Add("@user_id", SqlDbType.NVarChar, 30).Value = user_id;
            da.SelectCommand.Parameters.Add("@show_reports", SqlDbType.NVarChar, 3).Value = "no";
            da.SelectCommand.Parameters.Add("@status", SqlDbType.NVarChar, 4).Value = "True";
            da.SelectCommand.Parameters.Add("@manage", SqlDbType.NVarChar, 3).Value = "no";

            DataSet ds = new DataSet();
            da.Fill(ds, "employeeinfo");
            return ds;
        }

        [WebMethod(Description = "Gets information about an employee from Active Directory")]
        public DataSet GetEmployeeInfoFromAD(string user_id)
        {
            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);
            SqlDataAdapter da = new SqlDataAdapter("spz_GetUserInfoFromAD", cn);

            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@user_id", SqlDbType.NVarChar, 50).Value = user_id;

            DataSet ds = new DataSet();
            da.Fill(ds, "employeeinfo");
            return ds;
        }

        [WebMethod(Description = "Gets PASS users and access levels for a WSC")]
        public DataSet GetPASSUsers(int wsc_id, string access_level)
        {
            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);

            SqlDataAdapter da = new SqlDataAdapter("SELECT DISTINCT user_id, user_nm, wsc_id, active, approver_va, administrator_va, pass_access" +
                " FROM tblEmployees AS te INNER JOIN" +
                " lut_Office AS lo ON te.office_id = lo.office_id " +
                " WHERE lo.wsc_id = " + wsc_id.ToString() + " and pass_access = '" + access_level + "'" +
                " ORDER BY lw.wsc_cd, s.site_no", cn);

            DataSet ds = new DataSet();
            da.Fill(ds, "PASSaccessinfo");
            return ds;
        }

        [WebMethod(Description = "Gets all sites in PASS with DCP IDs")]
        public DataSet GetPASSSiteInfo()
        {
            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);
            SqlDataAdapter da = new SqlDataAdapter("SELECT site_no, dcp_id FROM simsdb_water.dbo.PASS_Site_Master", cn);

            DataSet ds = new DataSet();
            da.Fill(ds, "PASSSiteInfo");
            return ds;
        }

        [WebMethod(Description = "Gets operator user names by site number")]
        public DataSet GetRecordOperators(string site_no, string agency_cd)
        {
            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);
            SqlDataAdapter da = new SqlDataAdapter("SELECT rrm.operator_uid FROM SIMS_Site_Master AS ssm INNER JOIN RMS_Record_Master AS rrm ON ssm.site_id = rrm.site_id WHERE (ssm.site_no = '" + site_no + "') AND (ssm.agency_cd = '" + agency_cd + "')", cn);

            DataSet ds = new DataSet();
            da.Fill(ds, "Operators");
            return ds;
        }

        [WebMethod(Description = "Gets field trip user names by site number")]
        public DataSet GetFieldTripUsers(string site_no, string agency_cd)
        {
            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);
            SqlDataAdapter da = new SqlDataAdapter("SELECT tlt.user_id FROM SIMS_Site_Master AS ssm INNER JOIN Trip_Site_Master AS tsm ON ssm.site_id = tsm.site_id INNER JOIN Trip_Lut_Trip AS tlt ON tsm.trip_id = tlt.trip_id WHERE (ssm.site_no = '" + site_no + "') AND (ssm.agency_cd = '" + agency_cd + "')", cn);

            DataSet ds = new DataSet();
            da.Fill(ds, "FieldTripUsers");
            return ds;
        }

        [WebMethod(Description = "Gets site information for the KML downloads")]
        public DataSet GetSitesForKML(string office_id, int wsc_id)
        {
            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);
            SqlDataAdapter da = new SqlDataAdapter("SP_Sites_By_WSC_KMZMapping", cn);

            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@wsc_id", SqlDbType.Int).Value = wsc_id;
            da.SelectCommand.Parameters.Add("@office_id", SqlDbType.Int).Value = Convert.ToInt32(office_id);

            DataSet ds = new DataSet();
            da.Fill(ds, "siteinfo");
            return ds;
        }

        [WebMethod(Description = "Gets all the office info by wsc id")]
        public DataSet GetOfficeInfoByWSC(int wsc_id)
        {
            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM lut_Office AS lo WHERE lo.wsc_id = " + wsc_id.ToString(), cn);

            DataSet ds = new DataSet();
            da.Fill(ds, "OfficeList");
            return ds;
        }

        [WebMethod(Description = "Gets all the office info by office id")]
        public DataSet GetOfficeInfo(int office_id)
        {
            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM lut_Office AS lo WHERE lo.office_id = " + office_id.ToString(), cn);

            DataSet ds = new DataSet();
            da.Fill(ds, "OfficeList");
            return ds;
        }

        [WebMethod(Description = "Gets WSC information")]
        public DataSet GetWSCInfo(int wsc_id, string primaryOU)
        {
            string sql;
            if (wsc_id == 0)
            {
                sql = "SELECT * FROM lut_WSC WHERE AD_OU Like('%" + primaryOU + "%')";
            }
            else
            {
                sql = "SELECT * FROM lut_WSC WHERE wsc_id = " + wsc_id.ToString();
            }

            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);
            SqlDataAdapter da = new SqlDataAdapter(sql, cn);

            DataSet ds = new DataSet();
            da.Fill(ds, "wscinfo");
            return ds;
        }

        [WebMethod(Description = "Gets WSC information using the wsc_cd")]
        public DataSet GetWSCInfoByCode(string wsc_cd)
        {
            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM lut_WSC WHERE (wsc_cd = '" + wsc_cd + "')", cn);

            DataSet ds = new DataSet();
            da.Fill(ds, "wscinfo");
            return ds;
        }

        [WebMethod(Description = "Gets the elements for a site. Can return all elements, only SDESC, SANL, or MANU elements")]
        public DataSet GetElementsBySite(string doc_type, string site_no, string agency_cd)
        {
            if (string.IsNullOrEmpty(agency_cd))
                agency_cd = "USGS";

            string where_stmt = " ssm.site_no = '" + site_no + "' AND ssm.agency_Cd = '" + agency_cd + "'";

            if (!string.IsNullOrEmpty(doc_type))
                where_stmt += " AND report_type_cd = '" + doc_type + "'";

            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);
            SqlDataAdapter da = new SqlDataAdapter("SELECT DISTINCT ese.site_id, ese.element_id, eled.element_nm, (CASE WHEN ese.element_info IS NULL THEN '' ELSE ese.element_info END) AS element_info, ese.revised_by, ese.revised_dt, eled.remark, elr.report_type_cd, priority" +
                " FROM Elem_Lut_ReportRef AS elr INNER JOIN Elem_Lut_ElemDetail AS eled INNER JOIN SIMS_Site_Master AS ssm INNER JOIN" +
                " Elem_Site_Element AS ese ON ssm.site_id = ese.site_id ON eled.element_id = ese.element_id ON elr.element_id = eled.element_id" + 
                " WHERE " + where_stmt + 
                " ORDER BY priority", cn);

            DataSet ds = new DataSet();
            da.Fill(ds, "elementlist");

            DataTable tblElements;
            tblElements = ds.Tables["elementlist"];

            foreach (DataRow drCurr in tblElements.Rows)
            {
                string element_info = drCurr["element_info"].ToString();
                int site_id = Convert.ToInt32(drCurr["site_id"].ToString());
                int element_id = Convert.ToInt32(drCurr["element_id"].ToString());

                drCurr["element_info"] = Extensions.FormatElementInfo(element_info, element_id, site_id);
            }
            ds.AcceptChanges();
            return ds;
        }

        [WebMethod(Description = "Gets the single element information by site ID and element ID.")]
        public DataSet GetElementBySiteAndElement(int element_id, int site_id)
        {
            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Elem_Site_Element WHERE (element_id=" + element_id.ToString() + ") AND (site_id=" + site_id.ToString() + ")", cn);

            DataSet ds = new DataSet();
            da.Fill(ds, "elementinfo");

            DataTable tblElement;
            tblElement = ds.Tables["elementinfo"];

            foreach (DataRow drCurr in tblElement.Rows)
            {
                string element_info = drCurr["element_info"].ToString();

                drCurr["element_info"] = Extensions.FormatElementInfo(element_info, element_id, site_id);
            }
            ds.AcceptChanges();
            return ds;
        }

        [WebMethod(Description = "Generates the Records Management System Latency Reports.")]
        public DataSet GetRMSLatencyReport(string wsc_cd, string theme)
        {
	        string cs = Config.ConnectionInfo;
	        SqlConnection cn = new SqlConnection(cs);
	        SqlDataAdapter da = new SqlDataAdapter("SP_RMS_Latency_Old", cn);

	        if (string.IsNullOrEmpty(wsc_cd)) {
		        wsc_cd = "%";
	        }
	        if (string.IsNullOrEmpty(theme)) {
		        theme = "DETAILS";
	        }

	        da.SelectCommand.CommandType = CommandType.StoredProcedure;
	        da.SelectCommand.Parameters.Add("@wsc_cd", SqlDbType.NVarChar, 15).Value = wsc_cd;
	        da.SelectCommand.Parameters.Add("@theme", SqlDbType.NVarChar, 7).Value = theme;

	        DataSet ds = new DataSet();
	        da.Fill(ds, "RMSlatencyreport");
	        return ds;
        }

        [WebMethod(Description = "Gets site list for SDESC Repository within a certain amount of hours of last revised")]
        public DataSet GetSDSiteListByLastRevised(string lu)
        {
            int secs = Convert.ToInt32(lu) * 3600;
            string seconds = secs.ToString();

            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);
            SqlDataAdapter da = new SqlDataAdapter("SELECT DISTINCT lw.wsc_cd, lo.office_cd, ssm.site_no, ssm.agency_cd, s.station_nm, (CASE WHEN agency_use_cd In('A','M','L') THEN 'A' ELSE 'I' END) AS active_fg, ers.revised_dt" +
                " FROM lut_WSC AS lw INNER JOIN" +
                "   lut_Office AS lo ON lw.wsc_id = lo.wsc_id INNER JOIN" +
                "   SIMS_Site_Master AS ssm ON lo.office_id = ssm.office_id INNER JOIN" +
                "   Elem_Report_Sum AS ers ON ssm.site_id = ers.site_id INNER JOIN" +
                "   nwisweb.dbo.SITEFILE AS s ON s.site_id = ssm.nwisweb_site_id" +
                " WHERE DATEDIFF(second, ers.revised_dt, GETDATE()) <= " + seconds + " AND ers.report_type_cd = 'SDESC'", cn);

            DataSet ds = new DataSet();
            da.Fill(ds, "sitelist");
            return ds;
        }

        [WebMethod(Description = "Gets site list for MANU Repository within a certain amount of hours of last revised")]
        public DataSet GetMANUSiteListByLastRevised(string lu)
        {
            int secs = Convert.ToInt32(lu) * 3600;
            string seconds = secs.ToString();

            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);
            SqlDataAdapter da = new SqlDataAdapter("SELECT DISTINCT lw.wsc_cd, lo.office_cd, ssm.site_no, ssm.agency_cd, s.station_nm, (CASE WHEN agency_use_cd In('A','M','L') THEN 'A' ELSE 'I' END) AS active_fg, ers.revised_dt" +
                " FROM lut_WSC AS lw INNER JOIN" +
                "   lut_Office AS lo ON lw.wsc_id = lo.wsc_id INNER JOIN" +
                "   SIMS_Site_Master AS ssm ON lo.office_id = ssm.office_id INNER JOIN" +
                "   Elem_Report_Sum AS ers ON ssm.site_id = ers.site_id INNER JOIN" +
                "   nwisweb.dbo.SITEFILE AS s ON s.site_id = ssm.nwisweb_site_id" +
                " WHERE DATEDIFF(second, ers.revised_dt, GETDATE()) <= " + seconds + " AND ers.report_type_cd = 'MANU'", cn);

            DataSet ds = new DataSet();
            da.Fill(ds, "sitelist");
            return ds;
        }

        [WebMethod(Description = "Gets site list for SDESC Repository")]
        public DataSet GetSDSiteList()
        {
            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);
            SqlDataAdapter da = new SqlDataAdapter("SELECT DISTINCT lw.wsc_cd, lo.office_cd, ssm.site_no, ssm.agency_cd, s.station_nm, (CASE WHEN agency_use_cd In('A','M','L') THEN 'A' ELSE 'I' END) AS active_fg, ers.revised_dt" +
                " FROM lut_WSC AS lw INNER JOIN" +
                "   lut_Office AS lo ON lw.wsc_id = lo.wsc_id INNER JOIN" +
                "   SIMS_Site_Master AS ssm ON lo.office_id = ssm.office_id INNER JOIN" +
                "   Elem_Report_Sum AS ers ON ssm.site_id = ers.site_id INNER JOIN" +
                "   nwisweb.dbo.SITEFILE AS s ON s.site_id = ssm.nwisweb_site_id" +
                " WHERE ers.report_type_cd = 'SDESC'", cn);

            DataSet ds = new DataSet();
            da.Fill(ds, "sitelist");
            return ds;
        }

        [WebMethod(Description = "Gets RMS total status information from stored proc SP_RMS_Progress_Report_by_region_or_WSC from SIMS")]
        public DataSet GetProgressReportByRegionWSC(DateTime querydate, string region_cd, int wsc_id)
        {
            if (string.IsNullOrEmpty(region_cd)) region_cd = "NADA";

            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);
            //Create a Data Adapter, then provide the name of the stored procedure
            SqlDataAdapter da = new SqlDataAdapter("SP_RMS_Progress_Report_by_region_or_WSC", cn);

            //Set the command type as Stored Procedure
            da.SelectCommand.CommandType = CommandType.StoredProcedure;

            //Create and add parameters to Parameters collection for stored procedure
            da.SelectCommand.Parameters.Add(new SqlParameter("@region_cd", SqlDbType.NVarChar, 50));
            da.SelectCommand.Parameters["@region_cd"].Value = region_cd;

            da.SelectCommand.Parameters.Add(new SqlParameter("@wsc_id", SqlDbType.Int));
            da.SelectCommand.Parameters["@wsc_id"].Value = wsc_id;

            da.SelectCommand.Parameters.Add(new SqlParameter("@querydate", SqlDbType.DateTime));
            da.SelectCommand.Parameters["@querydate"].Value = querydate;

            da.SelectCommand.Parameters.Add(new SqlParameter("@ts", SqlDbType.Bit));
            da.SelectCommand.Parameters["@ts"].Value = 0;
            
            da.SelectCommand.Parameters.Add(new SqlParameter("@use_ts", SqlDbType.NVarChar, 3));
            da.SelectCommand.Parameters["@use_ts"].Value = "no";

            //Create a DataSet to hold the records
            DataSet ds = new DataSet();
            //Fill the DataSet with the rows returned
            da.Fill(ds, "Total");
            return ds;
        }

        [WebMethod(Description = "Gets RMS total status information from stored proc SP_RMS_Progress_Report_by_region_or_WSC from SIMS - Continuous records only")]
        public DataSet GetContinuousProgressReportByRegionWSC(DateTime querydate, string region_cd, int wsc_id)
        {
            if (string.IsNullOrEmpty(region_cd)) region_cd = "NADA";

            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);
            //Create a Data Adapter, then provide the name of the stored procedure
            SqlDataAdapter da = new SqlDataAdapter("SP_RMS_Progress_Report_by_region_or_WSC", cn);

            //Set the command type as Stored Procedure
            da.SelectCommand.CommandType = CommandType.StoredProcedure;

            //Create and add parameters to Parameters collection for stored procedure
            da.SelectCommand.Parameters.Add(new SqlParameter("@region_cd", SqlDbType.NVarChar, 50));
            da.SelectCommand.Parameters["@region_cd"].Value = region_cd;

            da.SelectCommand.Parameters.Add(new SqlParameter("@wsc_id", SqlDbType.Int));
            da.SelectCommand.Parameters["@wsc_id"].Value = wsc_id;

            da.SelectCommand.Parameters.Add(new SqlParameter("@querydate", SqlDbType.DateTime));
            da.SelectCommand.Parameters["@querydate"].Value = querydate;

            da.SelectCommand.Parameters.Add(new SqlParameter("@ts", SqlDbType.Bit));
            da.SelectCommand.Parameters["@ts"].Value = 1;

            da.SelectCommand.Parameters.Add(new SqlParameter("@use_ts", SqlDbType.NVarChar, 3));
            da.SelectCommand.Parameters["@use_ts"].Value = "yes";

            //Create a DataSet to hold the records
            DataSet ds = new DataSet();
            //Fill the DataSet with the rows returned
            da.Fill(ds, "Total");
            return ds;
        }

        [WebMethod(Description = "Gets RMS total status information from stored proc SP_RMS_Progress_Report_by_region_or_WSC from SIMS - Non-continuous records only")]
        public DataSet GetNonContinuousProgressReportByRegionWSC(DateTime querydate, string region_cd, int wsc_id)
        {
            if (string.IsNullOrEmpty(region_cd)) region_cd = "NADA";

            string cs = Config.ConnectionInfo;
            SqlConnection cn = new SqlConnection(cs);
            //Create a Data Adapter, then provide the name of the stored procedure
            SqlDataAdapter da = new SqlDataAdapter("SP_RMS_Progress_Report_by_region_or_WSC", cn);

            //Set the command type as Stored Procedure
            da.SelectCommand.CommandType = CommandType.StoredProcedure;

            //Create and add parameters to Parameters collection for stored procedure
            da.SelectCommand.Parameters.Add(new SqlParameter("@region_cd", SqlDbType.NVarChar, 50));
            da.SelectCommand.Parameters["@region_cd"].Value = region_cd;

            da.SelectCommand.Parameters.Add(new SqlParameter("@wsc_id", SqlDbType.Int));
            da.SelectCommand.Parameters["@wsc_id"].Value = wsc_id;

            da.SelectCommand.Parameters.Add(new SqlParameter("@querydate", SqlDbType.DateTime));
            da.SelectCommand.Parameters["@querydate"].Value = querydate;

            da.SelectCommand.Parameters.Add(new SqlParameter("@ts", SqlDbType.Bit));
            da.SelectCommand.Parameters["@ts"].Value = 0;

            da.SelectCommand.Parameters.Add(new SqlParameter("@use_ts", SqlDbType.NVarChar, 3));
            da.SelectCommand.Parameters["@use_ts"].Value = "yes";

            //Create a DataSet to hold the records
            DataSet ds = new DataSet();
            //Fill the DataSet with the rows returned
            da.Fill(ds, "Total");
            return ds;
        }
    }

    public class SiteInfo
    {
        public int SiteID { get; set; }
        public int NWISWebSiteID { get; set; }
        public string AgencyCd { get; set; }
        public string SiteNo { get; set; }
        public string NWISHost { get; set; }
        public string DBNO { get; set; }
        public string StationName { get; set; }
        public int OfficeID { get; set; }
        public string AltBasinName { get; set; }
        public int WSCID { get; set; }
        public DateTime? LevelsDt { get; set; }
        public short? LevelsFreq { get; set; }
        public Boolean? LevelsClosed { get; set; }

        public SiteInfo() { }

        public SiteInfo(int _SiteID, int _NWISWebSiteID, string _AgencyCd, string _SiteNo, string _NWISHost, string _DBNO, string _StationName, int _OfficeID, string _AltBasinName, int _WSCID, DateTime _LevelsDt, short _LevelsFreq, Boolean _LevelsClosed)
        {
            SiteID = _SiteID;
            NWISWebSiteID = _NWISWebSiteID;
            AgencyCd = _AgencyCd;
            SiteNo = _SiteNo;
            NWISHost = _NWISHost;
            DBNO = _DBNO;
            StationName = _StationName;
            OfficeID = _OfficeID;
            AltBasinName = _AltBasinName;
            WSCID = _WSCID;
            LevelsDt = _LevelsDt;
            LevelsFreq = _LevelsFreq;
            LevelsClosed = _LevelsClosed;
        }
    }

    public class SiteTripInfo
    {
        public int SiteID { get; set; }
        public string SiteNo { get; set; }
        public string StationName { get; set; }
        public double? DecLatVa { get; set; }
        public double? DecLongVa { get; set; }
        public string AgencyCd { get; set; }
        public string SiteTypeCd { get; set; }
        public string TripName { get; set; }
        public int TripID { get; set; }

        public SiteTripInfo() { }

        public SiteTripInfo(int _SiteID, string _SiteNo, string _StationName, double _DecLatVa, double _DecLongVa, string _AgencyCd, string _SiteTypeCd, string _TripName, int _TripID)
        {
            SiteID = _SiteID;
            SiteNo = _SiteNo;
            StationName = _StationName;
            DecLatVa = _DecLatVa;
            DecLongVa = _DecLongVa;
            AgencyCd = _AgencyCd;
            SiteTypeCd = _SiteTypeCd;
            TripName = _TripName;
            TripID = _TripID;
        }
    }
}
