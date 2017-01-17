using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class Extensions
    {
        public static string FormatElementInfo(this string element_info, int element_id, int site_id)
        {
            Data.SIMSDataContext db = new Data.SIMSDataContext();

            string elem_info = element_info;
            string element_nm = db.ElementDetails.FirstOrDefault(p => p.element_id == element_id).element_nm;
            string site_no = db.Sites.FirstOrDefault(p => p.site_id == site_id).site_no;
            string pOut = "";

            try
            {
                var lstNWISInfo = (from n in db.vNWISSiteInfoForManus
                                   where n.site_no.Equals(site_no)
                                   select new
                                   {
                                       SiteUseCode = n.site_use_cd,
                                       WellDepth = n.well_depth_va,
                                       AqfBookName = n.aqfr_book_nm,
                                       AltDatumCode = n.alt_datum_cd,
                                       Altitude = n.alt_va,
                                       DistrictAbbrev = n.district_abbrev,
                                       CountyName = n.county_nm,
                                       LocationDatumDescription = n.datum_desc,
                                       DatumDescription = n.description,
                                       DrainageArea = n.drain_area_va,
                                       ContributingDrainageArea = n.contrib_drain_area_va,
                                       Latitude = n.lat_va,
                                       Longitude = n.long_va,
                                       LandNetDesc = n.land_net_ds,
                                       HUCCode = n.huc_cd,
                                       UpperCasingDia = n.upperCasingDia,
                                       OpenTopVa = n.openTopVa,
                                       OpenBottomVa = n.openBottomVa,
                                       MPString = n.mpString,
                                       MPAltString = n.mpAltString,
                                       UseMpntAlt = n.use_mpntalt,
                                       WellDepthVa = n.wellDepthVa
                                   });

                switch (element_nm)
                {
                    case "LOCATION":
                        #region "Set Variables for Location element"
                        string lat_va = "";
                        string long_va = "";
                        string land_net_ds = "";
                        string huc_cd = "is unknown";
                        string county_nm = "UNSPECIFIED";
                        string district_abbrev = "";
                        string dat_desc = "";

                        foreach (var l in lstNWISInfo)
                        {
                            lat_va = l.Latitude;
                            long_va = l.Longitude;
                            land_net_ds = l.LandNetDesc;
                            try
                            {
                                if (string.IsNullOrEmpty(l.HUCCode))
                                {
                                    huc_cd = "is unknown";
                                }
                                else
                                {
                                    huc_cd = l.HUCCode;
                                }
                            }
                            catch (Exception ex) { huc_cd = "is unknown"; }
                            try
                            {
                                if (string.IsNullOrEmpty(l.CountyName))
                                {
                                    county_nm = "UNSPECIFIED";
                                }
                                else
                                {
                                    county_nm = l.CountyName + ", ";
                                }
                            }
                            catch (Exception ex) { county_nm = "UNSPECIFIED"; }
                            try
                            {
                                if (string.IsNullOrEmpty(l.DistrictAbbrev))
                                {
                                    district_abbrev = "";
                                }
                                else
                                {
                                    district_abbrev = l.DistrictAbbrev + ", ";
                                }
                            }
                            catch (Exception ex) { district_abbrev = ""; }
                            try
                            {
                                if (string.IsNullOrEmpty(l.LocationDatumDescription))
                                {
                                    dat_desc = "";
                                }
                                else
                                {
                                    dat_desc = " referenced to " + l.LocationDatumDescription;
                                }
                            }
                            catch (Exception ex) { dat_desc = ""; }
                        }
                        #endregion

                        //Format the Land Net Description (Township and Range)
                        //Correct format in NWIS: 'SESESES16 T14S  R65E  M'
                        #region "Land Net Description"
                        try
                        {
                            if (!string.IsNullOrEmpty(land_net_ds))
                            {
                                if (land_net_ds.Substring(6, 1) != "S")
                                {
                                    land_net_ds = " " + land_net_ds.Trim() + ",";
                                }
                                else
                                {
                                    string lnd = land_net_ds;
                                    string lnd1 = lnd.Substring(0, 2);
                                    if (lnd1 != "  ")
                                    {
                                        lnd1 = lnd1 + " 1/4 ";
                                    }
                                    else
                                    {
                                        lnd1 = "";
                                    }

                                    string lnd2 = lnd.Substring(2, 2);
                                    if (lnd2 != "  ")
                                    {
                                        lnd2 = lnd2 + " 1/4 ";
                                    }
                                    else
                                    {
                                        lnd2 = "";
                                    }

                                    string lnd3 = lnd.Substring(4, 2);
                                    if (lnd3 != "  ")
                                    {
                                        lnd3 = lnd3 + " 1/4 ";
                                    }
                                    else
                                    {
                                        lnd3 = "";
                                    }

                                    string lnd4 = lnd.Substring(7, 3);
                                    if (lnd4 != "   ")
                                    {
                                        try
                                        {
                                            lnd4 = "sec." + lnd4.Trim() + ", ";
                                        }
                                        catch (Exception ex)
                                        {
                                            lnd4 = "";
                                        }
                                    }
                                    else
                                    {
                                        lnd4 = "";
                                    }

                                    string lnd5 = "";
                                    try
                                    {
                                        //Check to see which template is being used for parsing out the rest
                                        lnd5 = lnd.Substring(11, 5);

                                        if (lnd5 != "     ")
                                        {
                                            lnd5 = lnd.Substring(11, 5).Trim();
                                            if (lnd5.Length > 1)
                                            {
                                                if (lnd5.Substring(0, lnd5.Length - 1).IsNumeric() | lnd5.Substring(0, lnd5.Length - 2).IsNumeric())
                                                {
                                                    if (lnd5.Substring(lnd5.Length - 2, 1) == "H")
                                                    {
                                                        lnd5 = "T." + Convert.ToInt64(lnd5.Substring(0, lnd5.Length - 2)).ToString() + " 1/2 " + lnd5.Substring(lnd5.Length - 1, 1) + "., ";
                                                    }
                                                    else
                                                    {
                                                        lnd5 = "T." + Convert.ToInt64(lnd5.Substring(0, lnd5.Length - 1)).ToString() + " " + lnd5.Substring(lnd5.Length - 1, 1) + "., ";
                                                    }
                                                }
                                                else
                                                {
                                                    lnd5 = "";
                                                }
                                            }
                                            else
                                            {
                                                lnd5 = "";
                                            }
                                        }
                                        else
                                        {
                                            lnd5 = "";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        lnd5 = "";
                                    }

                                    string lnd6 = "";
                                    try
                                    {
                                        if (lnd.Length > 17)
                                        {
                                            try
                                            {
                                                lnd6 = lnd.Substring(17, 5).Trim();
                                            }
                                            catch (Exception ex)
                                            {
                                                lnd6 = lnd.Substring(17).Trim();
                                            }
                                            if (!string.IsNullOrEmpty(lnd6) && lnd6.Length > 1)
                                            {
                                                if (lnd6.Substring(0, lnd6.Length - 1).IsNumeric() | lnd6.Substring(0, lnd6.Length - 2).IsNumeric())
                                                {
                                                    if (lnd6.Substring(lnd6.Length - 2, 1) == "H")
                                                    {
                                                        lnd6 = "R." + Convert.ToInt64(lnd6.Substring(0, lnd6.Length - 2)).ToString() + " 1/2 " + lnd6.Substring(lnd6.Length - 1, 1) + "., ";
                                                    }
                                                    else
                                                    {
                                                        lnd6 = "R." + Convert.ToInt64(lnd6.Substring(0, lnd6.Length - 1)).ToString() + " " + lnd6.Substring(lnd6.Length - 1, 1) + "., ";
                                                    }
                                                }
                                                else
                                                {
                                                    lnd6 = "";
                                                }
                                            }
                                            else
                                            {
                                                lnd6 = "";
                                            }
                                        }
                                        else
                                        {
                                            lnd6 = "";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        lnd6 = "";
                                    }


                                    land_net_ds = " in " + lnd1 + lnd2 + lnd3 + lnd4 + lnd5 + lnd6;

                                    if (land_net_ds == " in " | lnd5 == "" | lnd6 == "")
                                    {
                                        land_net_ds = "";
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            land_net_ds = "Error: " + ex.Message;
                        }
                        #endregion

                        //Modify the Lat and Long for appropriate text output
                        #region "Latitude and Longitude"
                        if (string.IsNullOrEmpty(lat_va))
                        {
                            lat_va = "<i>unavailable</i>";
                        }
                        else
                        {
                            if (lat_va.Substring(0, 1) == "-")
                            {
                                lat_va = lat_va.Substring(1, 2) + "&deg;" + lat_va.Substring(3, 2) + "'" + lat_va.Substring(5) + "\"";
                            }
                            else
                            {
                                lat_va = lat_va.Substring(0, 2) + "&deg;" + lat_va.Substring(2, 2) + "'" + lat_va.Substring(4) + "\"";
                            }
                        }
                        if (string.IsNullOrEmpty(long_va))
                        {
                            long_va = "<i>unavailable</i>";
                        }
                        else
                        {
                            if (long_va.Substring(0, 1) == "-")
                            {
                                if (long_va.Substring(1, 1) == "0")
                                {
                                    long_va = long_va.Substring(2, 2) + "&deg;" + long_va.Substring(4, 2) + "'" + long_va.Substring(6) + "\"";
                                }
                                else
                                {
                                    if (district_abbrev == "GM, ")
                                    {
                                        long_va = long_va.Substring(0, 4) + "&deg;" + long_va.Substring(5, 2) + "'" + long_va.Substring(7) + "\"";
                                    }
                                    else
                                    {
                                        long_va = long_va.Substring(1, 3) + "&deg;" + long_va.Substring(4, 2) + "'" + long_va.Substring(6) + "\"";
                                    }
                                }
                            }
                            else if (long_va.Substring(0, 1) == "0")
                            {
                                long_va = long_va.Substring(1, 2) + "&deg;" + long_va.Substring(3, 2) + "'" + long_va.Substring(5) + "\"";
                            }
                            else
                            {
                                long_va = long_va.Substring(0, 3) + "&deg;" + long_va.Substring(3, 2) + "'" + long_va.Substring(5) + "\"";
                            }
                        }
                        #endregion

                        elem_info = "Lat " + lat_va + ", long " + long_va + dat_desc + ", " + land_net_ds + " " + county_nm + district_abbrev + " Hydrologic Unit " + huc_cd + ", " + element_info;

                        break;
                    case "DRAINAGE AREA":
                        #region "Define and assign variables"
                        string da = "";
                        string cda = "";
                        string nca = "";
                        string error_spot = "";

                        foreach (var d in lstNWISInfo)
                        {
                            try
                            {
                                da = d.DrainageArea;
                            }
                            catch (Exception ex)
                            {
                                da = "";
                            }
                            try
                            {
                                cda = d.ContributingDrainageArea;
                            }
                            catch (Exception ex)
                            {
                                cda = "";
                            }
                        }
                        #endregion

                        #region "Compute non-contributing area"
                        if (da.IsNumeric() && cda.IsNumeric())
                        {
                            try
                            {
                                //Calculate the non-contributing drainage area
                                try { nca = (Convert.ToDouble(da) - Convert.ToDouble(cda)).ToString(); }
                                catch (Exception ex) { error_spot = "first spot"; }

                                try
                                {
                                    if (nca == "0")
                                    {
                                        if (!string.IsNullOrEmpty(element_info))
                                        {
                                            pOut = Convert.ToDecimal(da).FormatNWISNumber() + " mi&#178;, ";
                                        }
                                        else
                                        {
                                            pOut = Convert.ToDecimal(da).FormatNWISNumber() + " mi&#178;.";
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(element_info))
                                        {
                                            pOut = Convert.ToDecimal(da).FormatNWISNumber() + " mi&#178; of which " + Convert.ToDecimal(nca).FormatNWISNumber() + " mi&#178; probably is noncontributing, ";
                                        }
                                        else
                                        {
                                            pOut = Convert.ToDecimal(da).FormatNWISNumber() + " mi&#178; of which " + Convert.ToDecimal(nca).FormatNWISNumber() + " mi&#178; probably is noncontributing.";
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    error_spot = "third spot";
                                }

                            }
                            catch (Exception ex)
                            {
                                pOut = "Error with non-contributing: " + ex.Message + ", " + error_spot;
                            }
                        }
                        #endregion
                        #region "No noncontributing area referenced"
                        else if (da.IsNumeric())
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(element_info))
                                {
                                    pOut = Convert.ToDecimal(da).FormatNWISNumber() + " mi&#178;, ";
                                }
                                else
                                {
                                    pOut = Convert.ToDecimal(da).FormatNWISNumber() + " mi&#178;.";
                                }
                            }
                            catch (Exception ex)
                            {
                                pOut = "Error with contributing: " + ex.Message;
                            }

                        }
                        #endregion
                        #region "Can't determine a drainage area"
                        else
                        {
                            if (string.IsNullOrEmpty(element_info))
                            {
                                pOut = "unknown";
                            }
                            else
                            {
                                pOut = "";
                            }
                        }
                        #endregion

                        elem_info = pOut + error_spot + element_info;

                        break;
                    case "WELL CHARACTERISTICS (MANU)":
                        #region "Define and assign variables"
                        decimal wd = 0;
                        string ucd = "UPPERCASINGDIAMETER";
                        string otv = "OPENTOPVALUE";
                        string obv = "OPENBOTTOMVALUE";
                        error_spot = "";
                        pOut = "Undefined.";

                        foreach (var w in lstNWISInfo)
                        {
                            try
                            {
                                wd = Convert.ToDecimal(w.WellDepth);
                            }
                            catch (Exception ex)
                            { }
                            try
                            {
                                if (string.IsNullOrEmpty(w.UpperCasingDia))
                                {
                                    ucd = "UPPERCASINGDIAMETER";
                                }
                                else
                                {
                                    ucd = w.UpperCasingDia;
                                }
                            }
                            catch (Exception ex) { ucd = "UPPERCASINGDIAMETER"; }
                            try
                            {
                                if (string.IsNullOrEmpty(w.OpenTopVa))
                                {
                                    otv = "OPENTOPVALUE";
                                }
                                else
                                {
                                    otv = w.OpenTopVa;
                                }
                            }
                            catch (Exception ex) { otv = "OPENTOPVALUE"; }
                            try
                            {
                                if (string.IsNullOrEmpty(w.OpenBottomVa))
                                {
                                    obv = "OPENBOTTOMVALUE";
                                }
                                else
                                {
                                    obv = w.OpenBottomVa;
                                }
                            }
                            catch (Exception ex) { obv = "OPENBOTTOMVALUE"; }
                        }
                        #endregion

                        #region "Format the well depth"
                        if (wd == -9999 | wd == 0)
                        {
                            pOut = "Depth undefined.";
                        }
                        else
                        {
                            pOut = "Depth " + wd.FormatNWISNumber() + " ft.";
                        }
                        #endregion

                        elem_info = pOut + " Upper casing diameter " + ucd + "; top of first opening " + otv + ", bottom of last opening " + obv + ". " + element_info;

                        break;
                    case "DATUM":
                        #region "Define and assign variables"
                        elem_info = "Land-surface datum is undefined. Measuring point: MEASURINGPOINT. ";
                        string vd = "";
                        string mp = "MEASURINGPOINT";
                        decimal alt = 0;
                        string alt_formatted;
                        string uma;

                        foreach (var d in lstNWISInfo)
                        {
                            vd = d.DatumDescription;
                            alt = Convert.ToDecimal(d.Altitude);
                            uma = Convert.ToString(d.UseMpntAlt);
                            try
                            {
                                //If an office has set the use alternate measureing point flag to true, then use the value contained in the MPAltString field
                                if (!String.IsNullOrEmpty(uma))
                                {
                                    if (uma == "True" || uma == "true")
                                    {
                                        if (string.IsNullOrEmpty(d.MPAltString))
                                        {
                                            mp = "MEASURINGPOINT";
                                        }
                                        else
                                        {
                                            mp = d.MPAltString;
                                        }
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(d.MPString))
                                        {
                                            mp = "MEASURINGPOINT";
                                        }
                                        else
                                        {
                                            mp = d.MPString;
                                        }
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(d.MPString))
                                    {
                                        mp = "MEASURINGPOINT";
                                    }
                                    else
                                    {
                                        mp = d.MPString;
                                    }
                                }
                            }
                            catch (Exception ex) { mp = "MEASURINGPOINT"; }
                        }
                        #endregion

                        #region "Format the datum and altitude"
                        try
                        {
                            if (string.IsNullOrEmpty(vd))
                            {
                                vd = "undefined vertical datum";
                            }

                            alt_formatted = alt.FormatNWISNumber();

                            if (alt_formatted == "-9,999")
                            {
                                elem_info = "Land-surface datum is undefined. Measuring point: " + mp + ". " + element_info;
                            }
                            else
                            {
                                elem_info = "Land-surface datum is " + alt_formatted + " ft above " + vd + ". Measuring point: " + mp + ". " + element_info;
                            }
                        }
                        catch (Exception ex) { elem_info = elem_info + "ERROR"; }
                        #endregion

                        break;
                    case "DATE OF LAST LEVELS":
                        var doll = db.OpsLevels.FirstOrDefault(p => p.site_id == site_id);
                        if (doll != null)
                        {
                            DateTime? levels_dt = doll.levels_dt;

                            string lastrun = "UNKNOWN";
                            string nextrun = "UNKNOWN";
                            string frequency = "UNKNOWN";
                            string status = "OPEN";

                            if (levels_dt != null)
                            {
                                lastrun = String.Format("{0:MM/dd/yyyy}", levels_dt);
                                if (doll.levels_freq != null)
                                    nextrun = String.Format("{0:MM/dd}/{1}", levels_dt, Convert.ToDateTime(levels_dt).Year + doll.levels_freq);
                                else
                                    nextrun = "unknown frequency, cannot be calculated";
                            }
                            if (doll.levels_freq != null) frequency = doll.levels_freq + " year(s)";
                            if (Convert.ToBoolean(doll.levels_closed)) status = "CLOSED";

                            elem_info = "Last run: " + lastrun + "; Next run: " + nextrun + "; Frequency: " + frequency + "; Status: " + status;
                        }
                        break;
                    default:
                        elem_info = element_info;
                        break;
                }

                //Handle special characters
                elem_info = elem_info.FormatParagraphOut();
            }
            catch (Exception ex)
            {
                elem_info = ex.Message;
            }

            return elem_info;
        }

        /// <summary>
        /// Formats the text for rendering onto a webpage
        /// </summary>
        public static string FormatParagraphOut(this string text)
        {
            if (text != null)
            {
                text = text.Replace("\n", "<br />");
                text = text.Replace(System.Environment.NewLine, "<br />");
                text = text.Replace("`", "&#0176;"); //Degree symbol
                text = text.Replace("mi2", "mi&#178;"); //Superscript 2
                text = text.Replace("ft3", "ft&#179;"); //Superscript 3
            }

            return text;
        }

        /// <summary>
        /// Formats the text to be used inside of a text editor
        /// </summary>
        public static string FormatParagraphEdit(this string text)
        {
            if (text != null)
            {
                text = text.Replace(System.Environment.NewLine, "<br />");
                text = text.Replace("\n", "<br />");
            }

            return text;
        }

        /// <summary>
        /// Formats the text to be used inside of a read only text box
        /// </summary>
        public static string FormatParagraphTextBox(this string text)
        {
            if (text != null)
            {
                text = text.Replace("&nbsp;", " ");
                text = text.Replace("</p>", "\n");
                text = text.Replace("<p>", "");
                text = text.Replace("<br />", "\n");
                text = text.Replace("<strong>", "");
                text = text.Replace("</strong>", "");
            }

            return text;
        }

        /// <summary>
        /// Formats the text for inserting into the database
        /// </summary>
        public static string FormatParagraphIn(this string text)
        {
            if (text != null)
            {
                text = text.Replace("<br />\n", "\n");
                text = text.Replace("<br />", "\n");
                text = text.Replace("<p>", "");
                text = text.Replace("</p>", "\n");
            }

            return text;
        }

        /// <summary>
        /// Returns a yes for true or a no for false
        /// </summary>
        public static string ProcessBoolean(this Boolean? bit)
        {
            string ret;

            if (bit == null)
                ret = "<i>unknown</i>";
            else if (Convert.ToBoolean(bit)) 
                ret = "Yes"; 
            else 
                ret = "No";

            return ret;
        }

        public static bool IsNumeric(this object obj)
        {
            string strNum = obj.ToString();
            try
            {
                double num;
                bool isNum = double.TryParse(strNum, out num);
                if (isNum)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static string FormatNWISNumber(this decimal pIn)
        {
            string pOut = pIn.ToString();

            try
            {
                string x;
                string y;
                decimal r = pIn - Math.Round(pIn);
                var hasNoFractionalPart = (r == 0);

                if (hasNoFractionalPart)
                {
                    pOut = pIn.ToString("f0");

                    if (pOut.Length == 4)
                    {
                        x = pOut.Substring(0, 1);
                        y = pOut.Substring(1);
                        pOut = x + "," + y;
                    }
                    else if (pOut.Length == 5)
                    {
                        x = pOut.Substring(0, 2);
                        y = pOut.Substring(2);
                        pOut = x + "," + y;
                    }
                    else if (pOut.Length == 6)
                    {
                        x = pOut.Substring(0, 3);
                        y = pOut.Substring(3);
                        pOut = x + "," + y;
                    }

                    pOut = pOut.Replace(",,", ",");
                }
                else
                {
                    pOut = pIn.ToString("f2");

                    if (pOut.Substring(pOut.Length - 1, 1) == "0")
                    {
                        if (pOut.Length > 4)
                        {
                            pOut = pIn.ToString("f1");
                        }
                    }

                    if (pOut.Substring(0, pOut.IndexOf(".")).Length == 4)
                    {
                        x = pOut.Substring(0, 1);
                        y = pOut.Substring(1);
                        pOut = x + "," + y;
                    }
                    else if (pOut.Substring(0, pOut.IndexOf(".")).Length == 5)
                    {
                        x = pOut.Substring(0, 2);
                        y = pOut.Substring(2);
                        pOut = x + "," + y;
                    }
                    else if (pOut.Substring(0, pOut.IndexOf(".")).Length == 6)
                    {
                        x = pOut.Substring(0, 3);
                        y = pOut.Substring(3);
                        pOut = x + "," + y;
                    }

                    pOut = pOut.Replace(",,", ",");
                }
            }
            catch (Exception ex)
            {
                return pOut;
            }

            return pOut;
        }
        public static String ToStringSafe(this object s)
        {
            if (s == null) return ""; else return s.ToString();
        }
        public static String ToString(this object s, String option)
        {
            if (s == null) return option; else return s.ToString();
        }
    }
}
