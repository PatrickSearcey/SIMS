using System;
using System.IO;
using System.Web;
using System.Xml;
using System.Net;
using System.Data;
using System.Data.SqlClient;
using Core;
using System.Configuration;

/// <summary>
/// This class will handle returning all of the dynamic KML/KMZ requests coming from Google Earth
/// Usually from a NetworkLink.
/// </summary>
public class KMLHandler : IHttpHandler
{
    private const string office_id_qs = "office_id";
    private const string wsc_id_qs = "wsc_id";

    private int wsc_id = 22;
    private string office_id = "0";

    /// <summary>
    ///  This is your main, or page_load.  It's where we're gonna start.
    /// </summary>
    /// <param name="context"></param>
    public void ProcessRequest(HttpContext context)
    {
        Setup(context);

        createSiteKML(office_id, wsc_id, context);

        // Send everything now and close the server response.        
        context.Response.Flush();
    }

    /// <summary>
    ///  Set up our environment based on current session or query string info.
    /// </summary>
    /// <param name="context"></param>
    private void Setup(HttpContext context)
    {
        if (context.Request.QueryString[wsc_id_qs] != null)
            wsc_id = Convert.ToInt32(context.Request.QueryString[wsc_id_qs]);

        if (context.Request.QueryString[office_id_qs] != null)
            office_id = context.Request.QueryString[office_id_qs];
    }

    /// <summary>
    ///  Needed to implement IHttpHandler, we probably won't use it.
    /// </summary>
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    protected void createSiteKML(string office_id, int wsc_id, HttpContext context)
    {
        DataTable dt = GetSites(office_id, wsc_id, 0);
        DataTable dt1 = GetFieldTrips(office_id);
        string strOfficeName;
        string strWSCName;
        string strDocName;
        string strFieldTrip;
        string strGeocodes;
        string strSiteType;
        string strDescription;
        string strName;
        string strNumber;
        string[] coordinates;
        string dGeoLatitude;
        string dGeoLongitude;
        string strIconFileName;

        if (office_id != "0")
        {
            strOfficeName = GetOfficeInfo(office_id, "name", wsc_id);
            strDocName = "Sites for " + strOfficeName;
        }
        else
        {
            strWSCName = GetWSCInfo(wsc_id, "name");
            strDocName = "Sites for " + strWSCName;
        }

        context.Response.ContentType = "application/vnd.google-earth.kml+xml";
        context.Response.ContentEncoding = System.Text.Encoding.UTF8;
        System.IO.MemoryStream stream = new System.IO.MemoryStream();
        XmlTextWriter XMLwrite = new XmlTextWriter(stream, System.Text.Encoding.UTF8);

        XMLwrite.WriteStartDocument();
        XMLwrite.WriteWhitespace(Environment.NewLine);
        XMLwrite.WriteStartElement("kml");
        XMLwrite.WriteAttributeString("xmlns", "http://www.opengis.net/kml/2.2");
        XMLwrite.WriteWhitespace(Environment.NewLine);

        XMLwrite.WriteWhitespace(Environment.NewLine);
        XMLwrite.WriteStartElement("Document");
        XMLwrite.WriteWhitespace(Environment.NewLine);
        XMLwrite.WriteElementString("name", strDocName);
        XMLwrite.WriteWhitespace(Environment.NewLine);
        XMLwrite.WriteStartElement("Folder");
        XMLwrite.WriteAttributeString("id", strDocName);
        XMLwrite.WriteWhitespace(Environment.NewLine);
        XMLwrite.WriteElementString("name", strDocName);

        foreach (DataRow row in dt.Rows)
        {
            strGeocodes = Convert.ToString(row["dec_long_va"]) + ", " + Convert.ToString(row["dec_lat_va"]) + ", 0.000000";
            strSiteType = (string)row["site_tp_cd"];

            switch (strSiteType)
            {
                case "LK":
                case "OC":
                case "ST":
                case "ES":
                case "GL":
                case "OC-CO":
                case "ST-CA":
                case "ST-DCH":
                case "ST-TS":
                case "WE":
                    {
                        strIconFileName = "https://sims.water.usgs.gov/SIMS/images/SiteIcons/SW.png";
                        break;
                    }
                case "GW":
                case "GW-CR":
                case "GW-EX":
                case "GW-HZ":
                case "GW-IE":
                case "GW-MW":
                case "GW-TH":
                case "SB":
                case "SB-CV":
                case "SB-GWD":
                case "SB-TSM":
                case "SB-UZ":
                    {
                        strIconFileName = "https://sims.water.usgs.gov/SIMS/images/SiteIcons/GW.png";
                        break;
                    }
                case "AT":
                    {
                        strIconFileName = "https://sims.water.usgs.gov/SIMS/images/SiteIcons/CL.png";
                        break;
                    }
                case "SP":
                    {
                        strIconFileName = "https://sims.water.usgs.gov/SIMS/images/SiteIcons/SP.png";
                        break;
                    }
                default:
                    {
                        strIconFileName = "https://sims.water.usgs.gov/SIMS/images/SiteIcons/OT.png";
                        break;
                    }
            }

            strDescription = "<![CDATA[<html xmlns:fo='http://www.w3.org/1999/XSL/Format' xmlns:msxsl='urn:schemas-microsoft-com:xslt'> <head> <META http-equiv='Content-Type' content='text/html'> </head> <body style='margin:0px 0px 0px 0px;overflow:auto;background:#FFFFFF;'> <table style='font-family:Arial,Verdana,Times;font-size:12px;text-align:left;width:300;border-collapse:collapse;padding:3px 3px 3px 3px'> <tr style='text-align:center;font-weight:bold;background:#9CBCE2'> <td>USGS - US GEOLOGICAL SURVEY</td> </tr> <tr> <td> <table style='font-family:Arial,Verdana,Times;font-size:12px;text-align:left;width:300;border-spacing:0px; padding:3px 3px 3px 3px'> <tr> <td>USGS site identifier</td> <td>" + Convert.ToString(row["site_no"]) + "</td> </tr> <tr bgcolor='#D4E4F3'> <td>Station name</td> <td>" + Convert.ToString(row["station_nm"]) + "</td> </tr> <tr> <td>Latitude, DD</td> <td>" + Convert.ToString(row["dec_lat_va"]) + "</td> </tr> <tr bgcolor='#D4E4F3'> <td>Longitude, DD</td> <td>" + Convert.ToString(row["dec_long_va"]) + "</td> </tr> <tr> <td>Site type</td> <td>" + Convert.ToString(row["site_tp_cd"]) + "</td> </tr> <tr bgcolor='#D4E4F3'> <td>Office code</td> <td>" + Convert.ToString(row["office_cd"]) + "</td> </tr> <tr> <td>Field trip</td> <td>" + Convert.ToString(row["trip_full_nm"]) + "</td> </tr> </table> </td> </tr> </table> </body> </html>]]>";
            strName = (string)row["station_nm"];
            strNumber = (string)row["site_no"];

            coordinates = strGeocodes.Split(',');
            dGeoLatitude = coordinates[1];
            dGeoLongitude = coordinates[0];

            XMLwrite.WriteWhitespace(Environment.NewLine);
            XMLwrite.WriteStartElement("Placemark");
            XMLwrite.WriteWhitespace(Environment.NewLine);
            XMLwrite.WriteElementString("name", strNumber);
            XMLwrite.WriteWhitespace(Environment.NewLine);
            XMLwrite.WriteElementString("Snippet", strNumber + " " + strName);
            XMLwrite.WriteWhitespace(Environment.NewLine);
            XMLwrite.WriteElementString("description", strDescription);
            XMLwrite.WriteWhitespace(Environment.NewLine);
            XMLwrite.WriteStartElement("Style");
            XMLwrite.WriteWhitespace(Environment.NewLine);
            XMLwrite.WriteStartElement("IconStyle");
            XMLwrite.WriteWhitespace(Environment.NewLine);
            XMLwrite.WriteStartElement("Icon");
            XMLwrite.WriteElementString("href", strIconFileName);
            XMLwrite.WriteEndElement();
            // End Icon
            XMLwrite.WriteWhitespace(Environment.NewLine);
            XMLwrite.WriteElementString("scale", "0.500");
            XMLwrite.WriteWhitespace(Environment.NewLine);
            XMLwrite.WriteEndElement();
            // End IconStyle
            XMLwrite.WriteWhitespace(Environment.NewLine);
            XMLwrite.WriteStartElement("LabelStyle");
            XMLwrite.WriteWhitespace(Environment.NewLine);
            XMLwrite.WriteElementString("scale", "0.667");
            XMLwrite.WriteWhitespace(Environment.NewLine);
            XMLwrite.WriteEndElement();
            // End LabelStyle
            XMLwrite.WriteWhitespace(Environment.NewLine);
            XMLwrite.WriteEndElement();
            // End Style
            XMLwrite.WriteWhitespace(Environment.NewLine);

            XMLwrite.WriteStartElement("Point");
            XMLwrite.WriteWhitespace(Environment.NewLine);
            XMLwrite.WriteElementString("extrude", "0");
            XMLwrite.WriteWhitespace(Environment.NewLine);
            XMLwrite.WriteElementString("altitudeMode", "clampToGround");
            XMLwrite.WriteWhitespace(Environment.NewLine);
            XMLwrite.WriteElementString("coordinates", dGeoLongitude + ", " + dGeoLatitude + ", 0");
            XMLwrite.WriteWhitespace(Environment.NewLine);
            XMLwrite.WriteEndElement();
            // End Point
            XMLwrite.WriteWhitespace(Environment.NewLine);
            // End Placemark
            XMLwrite.WriteEndElement();
        }

        XMLwrite.WriteWhitespace(Environment.NewLine);
        XMLwrite.WriteEndElement();
        // End Folder

        if (office_id != "0")
        {
            foreach (DataRow row in dt1.Rows)
            {
                DataTable dt2 = GetSites("0", 0, Convert.ToInt32(row["trip_id"]));
                strFieldTrip = row["trip_nm"] + " - " + row["user_id"];

                XMLwrite.WriteStartElement("Folder");
                XMLwrite.WriteAttributeString("id", strFieldTrip);
                XMLwrite.WriteWhitespace(Environment.NewLine);
                XMLwrite.WriteElementString("name", strFieldTrip);

                foreach (DataRow row1 in dt2.Rows)
                {
                    strGeocodes = Convert.ToString(row1["dec_long_va"]) + ", " + Convert.ToString(row1["dec_lat_va"]) + ", 0.000000";
                    strSiteType = (string)row1["site_tp_cd"];

                    switch (strSiteType)
                    {
                        case "LK":
                        case "OC":
                        case "ST":
                        case "ES":
                        case "GL":
                        case "OC-CO":
                        case "ST-CA":
                        case "ST-DCH":
                        case "ST-TS":
                        case "WE":
                            {
                                strIconFileName = "https://sims.water.usgs.gov/SIMS/images/SiteIcons/SW.png";
                                break;
                            }
                        case "GW":
                        case "GW-CR":
                        case "GW-EX":
                        case "GW-HZ":
                        case "GW-IE":
                        case "GW-MW":
                        case "GW-TH":
                        case "SB":
                        case "SB-CV":
                        case "SB-GWD":
                        case "SB-TSM":
                        case "SB-UZ":
                            {
                                strIconFileName = "https://sims.water.usgs.gov/SIMS/images/SiteIcons/GW.png";
                                break;
                            }
                        case "AT":
                            {
                                strIconFileName = "https://sims.water.usgs.gov/SIMS/images/SiteIcons/CL.png";
                                break;
                            }
                        case "SP":
                            {
                                strIconFileName = "https://sims.water.usgs.gov/SIMS/images/SiteIcons/SP.png";
                                break;
                            }
                        default:
                            {
                                strIconFileName = "https://sims.water.usgs.gov/SIMS/images/SiteIcons/OT.png";
                                break;
                            }
                    }

                    strDescription = "<![CDATA[<html xmlns:fo='http://www.w3.org/1999/XSL/Format' xmlns:msxsl='urn:schemas-microsoft-com:xslt'> <head> <META http-equiv='Content-Type' content='text/html'> </head> <body style='margin:0px 0px 0px 0px;overflow:auto;background:#FFFFFF;'> <table style='font-family:Arial,Verdana,Times;font-size:12px;text-align:left;width:300;border-collapse:collapse;padding:3px 3px 3px 3px'> <tr style='text-align:center;font-weight:bold;background:#9CBCE2'> <td>USGS - US GEOLOGICAL SURVEY</td> </tr> <tr> <td> <table style='font-family:Arial,Verdana,Times;font-size:12px;text-align:left;width:300;border-spacing:0px; padding:3px 3px 3px 3px'> <tr> <td>USGS site identifier</td> <td>" + Convert.ToString(row1["site_no"]) + "</td> </tr> <tr bgcolor='#D4E4F3'> <td>Station name</td> <td>" + Convert.ToString(row1["station_nm"]) + "</td> </tr> <tr> <td>Latitude, DD</td> <td>" + Convert.ToString(row1["dec_lat_va"]) + "</td> </tr> <tr bgcolor='#D4E4F3'> <td>Longitude, DD</td> <td>" + Convert.ToString(row1["dec_long_va"]) + "</td> </tr> <tr> <td>Site type</td> <td>" + Convert.ToString(row1["site_tp_cd"]) + "</td> </tr> <tr bgcolor='#D4E4F3'> <td>Office code</td> <td>" + Convert.ToString(row1["office_cd"]) + "</td> </tr> <tr> <td>Field trip</td> <td>" + Convert.ToString(row1["trip_full_nm"]) + "</td> </tr> </table> </td> </tr> </table> </body> </html>]]>";
                    strName = (string)row1["station_nm"];
                    strNumber = (string)row1["site_no"];

                    coordinates = strGeocodes.Split(',');
                    dGeoLatitude = coordinates[1];
                    dGeoLongitude = coordinates[0];

                    XMLwrite.WriteWhitespace(Environment.NewLine);
                    XMLwrite.WriteStartElement("Placemark");
                    XMLwrite.WriteWhitespace(Environment.NewLine);
                    XMLwrite.WriteElementString("name", strNumber);
                    XMLwrite.WriteWhitespace(Environment.NewLine);
                    XMLwrite.WriteElementString("Snippet", strNumber + " " + strName);
                    XMLwrite.WriteWhitespace(Environment.NewLine);
                    XMLwrite.WriteElementString("description", strDescription);
                    XMLwrite.WriteWhitespace(Environment.NewLine);
                    XMLwrite.WriteStartElement("Style");
                    XMLwrite.WriteWhitespace(Environment.NewLine);
                    XMLwrite.WriteStartElement("IconStyle");
                    XMLwrite.WriteWhitespace(Environment.NewLine);
                    XMLwrite.WriteStartElement("Icon");
                    XMLwrite.WriteElementString("href", strIconFileName);
                    XMLwrite.WriteEndElement();
                    // End Icon
                    XMLwrite.WriteWhitespace(Environment.NewLine);
                    XMLwrite.WriteElementString("scale", "0.500");
                    XMLwrite.WriteWhitespace(Environment.NewLine);
                    XMLwrite.WriteEndElement();
                    // End IconStyle
                    XMLwrite.WriteWhitespace(Environment.NewLine);
                    XMLwrite.WriteStartElement("LabelStyle");
                    XMLwrite.WriteWhitespace(Environment.NewLine);
                    XMLwrite.WriteElementString("scale", "0.667");
                    XMLwrite.WriteWhitespace(Environment.NewLine);
                    XMLwrite.WriteEndElement();
                    // End LabelStyle
                    XMLwrite.WriteWhitespace(Environment.NewLine);
                    XMLwrite.WriteEndElement();
                    // End Style
                    XMLwrite.WriteWhitespace(Environment.NewLine);

                    XMLwrite.WriteStartElement("Point");
                    XMLwrite.WriteWhitespace(Environment.NewLine);
                    XMLwrite.WriteElementString("extrude", "0");
                    XMLwrite.WriteWhitespace(Environment.NewLine);
                    XMLwrite.WriteElementString("altitudeMode", "clampToGround");
                    XMLwrite.WriteWhitespace(Environment.NewLine);
                    XMLwrite.WriteElementString("coordinates", dGeoLongitude + ", " + dGeoLatitude + ", 0");
                    XMLwrite.WriteWhitespace(Environment.NewLine);
                    XMLwrite.WriteEndElement();
                    // End Point
                    XMLwrite.WriteWhitespace(Environment.NewLine);
                    // End Placemark
                    XMLwrite.WriteEndElement();
                }

                XMLwrite.WriteWhitespace(Environment.NewLine);
                XMLwrite.WriteEndElement();
            }
        }

        XMLwrite.WriteWhitespace(Environment.NewLine);
        XMLwrite.WriteEndElement();
        // End Document
        XMLwrite.WriteWhitespace(Environment.NewLine);
        XMLwrite.WriteEndDocument();
        // End kml
        XMLwrite.Flush();
        System.IO.StreamReader reader;
        stream.Position = 0;
        reader = new System.IO.StreamReader(stream);
        Byte[] bytes = System.Text.Encoding.UTF8.GetBytes(reader.ReadToEnd());
        context.Response.BinaryWrite(bytes);
        context.Response.End();
    }

    public static DataTable GetSites(string office_id, int wsc_id, int trip_id)
    {
        DataTable dt = new DataTable();
        dt.Clear();

        using (SqlConnection cnx = new SqlConnection(Config.ConnectionInfo))
        {
            cnx.Open();

            SqlCommand cmd = new SqlCommand("SP_Sites_By_WSC_KMZMapping", cnx);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = wsc_id;
            cmd.Parameters.Add("@office_id", SqlDbType.Int).Value = System.Convert.ToInt32(office_id);
            cmd.Parameters.Add("@trip_id", SqlDbType.Int).Value = trip_id;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            cnx.Close();
        }

        return dt;
    }

    public static string GetOfficeInfo(string office_id, string what_info, int wsc_id)
    {
        string pOut = null;
        string office_nm = null;
        string office_lat = null;
        string office_long = null;
        string office_addrs = null;

        DataTable dt = new DataTable();
        dt.Clear();

        using (SqlConnection cnx = new SqlConnection(Config.ConnectionInfo))
        {
            cnx.Open();

            SqlCommand cmd = new SqlCommand("SP_Office_Info", cnx);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add("@office_id", SqlDbType.Int).Value = System.Convert.ToInt32(office_id);
            cmd.Parameters.Add("@office_cd", SqlDbType.NVarChar, 3).Value = "0";
            cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@action", SqlDbType.NVarChar, 12).Value = "byofficeid";

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                office_nm = row["office_nm"].ToString();
                try
                {
                    office_lat = row["dec_lat_va"].ToString();
                    office_long = row["dec_long_va"].ToString();
                }
                catch (Exception ex)
                {
                    office_lat = null;
                    office_long = null;
                }
                office_addrs = row["street_addrs"].ToString() + ", " + row["city_st_zip"].ToString();
            }

            cnx.Close();
        }

        switch (what_info)
        {
            case "name":
            {
                pOut = office_nm;
                break;
            }
            case "geo":
            {
                pOut = office_long + ", " + office_lat;
                break;
            }
            case "address":
            {
                pOut = office_addrs;
                break;
            }
        }

        return pOut;
    }

    public static string GetWSCInfo(int wsc_id, string what_info)
    {
        string pOut = null;
        string wsc_nm = null;
        string wsc_cd = null;

        using (SqlConnection cnx = new SqlConnection(Config.ConnectionInfo))
        {
            cnx.Open();

            SqlCommand cmd = new SqlCommand("SP_WSC_Info", cnx);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            DataTable dt = new DataTable();
            cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = wsc_id;
            cmd.Parameters.Add("@primaryOU", SqlDbType.NVarChar, 12).Value = "0";
            cmd.Parameters.Add("@region_cd", SqlDbType.NVarChar, 12).Value = "0";

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                wsc_nm = row["wsc_nm"].ToString();
                wsc_cd = row["wsc_cd"].ToString();
            }

            switch (what_info)
            {
                case "name":
                {
                    pOut = wsc_nm;
                    break;
                }
                case "abbrev":
                {
                    pOut = wsc_cd;
                    break;
                }
            }

            cnx.Close();
        }

        return pOut;
    }

    public static DataTable GetFieldTrips(string office_id)
    {
        DataTable dt = new DataTable();
        dt.Clear();

        using (SqlConnection cnx = new SqlConnection(Config.ConnectionInfo))
        {
            cnx.Open();

            SqlCommand cmd = new SqlCommand("SELECT trip_id, trip_nm, user_id FROM Trip_Lut_Trip WHERE office_id = " + office_id, cnx);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            cnx.Close();
        }

        return dt;
    }
}
