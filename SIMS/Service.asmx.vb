Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.Data.SqlClient

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class Service
    Inherits System.Web.Services.WebService

    <WebMethod(Description:="Gets Registered Site Information from TX SIMS using site_no")> _
    Public Function GetTXSiteInfo(ByVal site_no As String) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("SELECT ssm.site_id, CAST(ssm.nwisweb_site_id AS int) AS nwisweb_site_id, ssm.agency_cd, ssm.site_no, ssm.nwis_host, ssm.db_no, ssm.station_full_nm, ssm.office_id, ssm.alt_basin_nm, lo.wsc_id, ol.levels_dt, ol.levels_freq, ol.levels_closed FROM SIMS_Site_Master AS ssm INNER JOIN lut_Office AS lo ON ssm.office_id = lo.office_id LEFT OUTER JOIN Ops_Levels AS ol ON ssm.site_id = ol.site_id WHERE site_no = '" & site_no & "'", cn)
        Dim ds As New DataSet
        da.Fill(ds, "siteinfo")
        Return ds
    End Function

    <WebMethod(Description:="Gets Registered Site Information from TX SIMS using site_id")> _
    Public Function GetTXSiteInfoBySiteID(ByVal site_id As String) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("SELECT ssm.site_id, CAST(ssm.nwisweb_site_id AS int) AS nwisweb_site_id, ssm.agency_cd, ssm.site_no, ssm.nwis_host, ssm.db_no, ssm.station_full_nm, ssm.office_id, ssm.alt_basin_nm, lo.wsc_id, ol.levels_dt, ol.levels_freq, ol.levels_closed FROM SIMS_Site_Master AS ssm INNER JOIN lut_Office AS lo ON ssm.office_id = lo.office_id LEFT OUTER JOIN Ops_Levels AS ol ON ssm.site_id = ol.site_id WHERE ssm.site_id = " & site_id, cn)
        Dim ds As New DataSet
        da.Fill(ds, "siteinfo")
        Return ds
    End Function

    <WebMethod(Description:="Gets PASS Information for a site from TX SIMS")> _
    Public Function GetPASSsite(ByVal site_no As String) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("Select * from ops_DCP_id where site_no = '" & site_no & "'", cn)
        Dim ds As New DataSet
        da.Fill(ds, "siteinfo")
        Return ds
    End Function

    <WebMethod(Description:="Gets PASS table (ops_DCP_id) from sims.water.usgs.gov and copy to ops_DCP_id on TX")> _
    Public Function cpPASStabletoTX() As DataSet
        Dim cs As String = _
          "Persist Security Info=False;Integrated Security=SSPI;database=simsdb;server=igskahcwgscsims\SQLExpress"
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("Select * from ops_DCP_id ", cn)
        Dim ds As New DataSet
        da.Fill(ds, "PASStable")
        Return ds
    End Function

    <WebMethod(Description:="Gets ADR Status info for using stored procedure vSumBookingByState_site_tp_cd from TX SIMS")> _
    Public Function txADRStatus() As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("sp_Book_SumByState", cn)

        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.Add(New SqlParameter("@wy", SqlDbType.NVarChar, 4))
        da.SelectCommand.Parameters("@wy").Value = "2012"

        Dim ds As New DataSet
        da.Fill(ds, "bookRecord")
        Return ds
    End Function

    <WebMethod(Description:="Gets ADR Status info for all districts")> _
    Public Function allADRStatus() As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("sp_Book_SumByState", cn)

        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.Add(New SqlParameter("@wy", SqlDbType.NVarChar, 4))
        da.SelectCommand.Parameters("@wy").Value = "2012"

        Dim ds As New DataSet
        da.Fill(ds, "bookRecord")
        Return ds
    End Function

    <WebMethod(Description:="Gets a list of all the WSC's ID's and codes")> _
    Public Function allWSCs() As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("SELECT wsc_id, wsc_cd FROM lut_WSC ORDER BY wsc_cd", cn)

        Dim ds As New DataSet
        da.Fill(ds, "allWSCs")
        Return ds
    End Function

    <WebMethod(Description:="Gets CRP Status Chart data by Region Cd")> _
    Public Function CRPChartData(ByVal region_cd As String) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("SP_CRP_Cat_Charts_by_region_cd", cn)

        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.Add(New SqlParameter("@region_cd", SqlDbType.NVarChar, 3))
        da.SelectCommand.Parameters("@region_cd").Value = region_cd

        Dim ds As New DataSet
        da.Fill(ds, "ChartData")
        Return ds
    End Function

    <WebMethod(Description:="Gets sites from a specified field trip")> _
    Public Function fieldTripSites(ByVal trip_id As Integer, ByVal office_id As Integer) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("SP_Trip_Lists_using_trip_id", cn)

        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.Add(New SqlParameter("@trip_id", SqlDbType.Int))
        da.SelectCommand.Parameters("@trip_id").Value = trip_id
        da.SelectCommand.Parameters.Add(New SqlParameter("@office_id", SqlDbType.Int))
        da.SelectCommand.Parameters("@office_id").Value = office_id
        da.SelectCommand.Parameters.Add(New SqlParameter("@status", SqlDbType.NVarChar, 10))
        da.SelectCommand.Parameters("@status").Value = "assigned"

        Dim ds As New DataSet
        da.Fill(ds, "ftSites")
        Return ds
    End Function

    <WebMethod(Description:="Gets field trips by office")> _
    Public Function GetFieldTripsByOffice(ByVal office_id As Integer) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("SELECT tlt.trip_id, tlt.trip_nm + ' - ' + te.first_nm + ' ' + te.last_nm AS trip_name FROM Trip_Lut_Trip AS tlt INNER JOIN tblEmployees AS te ON tlt.user_id = te.user_id WHERE (tlt.office_id = " & office_id & ")", cn)

        Dim ds As New DataSet
        da.Fill(ds, "FieldTrips")
        Return ds
    End Function

    <WebMethod(Description:="Gets sites for a specified basin huc code")> _
    Public Function sitesByHUC(ByVal basin_cd As String) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim huc_codes As String = Nothing

        Select Case basin_cd
            Case "RG" 'Rio Grande
                huc_codes = "'13030102','13040100','13040201','13040202','13040203','13040204','13040205','13040206','13040207','13040208','13040209','13040210','13040211','13040212','13040301','13040302','13040303','13050003','13050004','13060011','13070001','13070002','13070003','13070004','13070005','13070006','13070007','13070008','13070009','13070010','13070011','13080002','13080003','13090001','12110202','12110203','12110204','12110205','12110206','12110207','12110208'"
            Case "Nueces" 'Nueces
                huc_codes = "'12100406','12100407','12110201','12110101','12110102','12110103','12110104','12110105','12110106','12110107','12110108','12110109','12110110','12110111','12110202','12110203','12110206','12110207'"
            Case "SA" 'San Antonio
                huc_codes = "'12100301','12100405','12100302','12100303','12100304'"
            Case "Guad" 'Guadalupe
                huc_codes = "'12100201','12100202','12100203','12100204','12100404','12100403'"
            Case "Lav" 'Lavaca
                huc_codes = "'12100101','12100102','12100402','12100401'"
            Case "SJ" 'San Jacinto
                huc_codes = "'12040101','12040102','12040103','12040104','12040204','12040203'"
            Case "Nech" 'Neches
                huc_codes = "'12020001','12020002','12020003','12020004','12020005','12020006','12020007','12040201','12040202'"
            Case "CB" 'Coastal Basins
                huc_codes = "'12090401','12090402','12100401','12100402','12100403','12040201','12040202','12110202','12110203','12110204','12110205','12110206','12110207','12110208','12040204','12040205','12040203','12100405','12100406','12100407'"
            Case "CO" 'Colorado
                huc_codes = "'12080001','12080002','12080003','12080004','12080005','12080006','12080007','12080008','12090101','12090102','12090103','12090104','12090105','12090106','12090107','12090108','12090109','12090110','12090201','12090202','12090203','12090204','12090205','12090206','12090301','12090302','12100401','12090401','12090402'"
            Case "Brazos" 'Brazos
                huc_codes = "'12050004','12050001','12050002','12050003','12050005','12050006','12050007','12060101','12060102','12060103','12060104','12060105','12060201','12060202','12060203','12060204','12070101','12070102','12070103','12070104','12070201','12070202','12070203','12070204','12070205','12090401','12090402','12040205'"
            Case "Trin" 'Trinity
                huc_codes = "'12030101','12030102','12030103','12030104','12030105','12030106','12030107','12030108','12030109','12030201','12030202','12030203','12040203','12040202'"
            Case "Sab" 'Sabine
                huc_codes = "'12010001','12010002','12010003','12010004','12010005','12010005'"
            Case "Cyp" 'Cypress
                huc_codes = "'11140305','11140304','11140306','11140307'"
            Case "Sul" 'Sulphur
                huc_codes = "'11140301','11140302','11140303'"
            Case "Red" 'Red
                huc_codes = "'11120101','11120102','11120103','11120104','11120105','11120201','11120202','11120301','11120302','11120304','11130101','11130102','11130103','11130104','11130105','11130201','11130204','11130205','11130206','11130207','11130209','11130210','11140101','11140106','11140201'"
            Case "Ark" 'Arkansas
                huc_codes = "'11080006','11090101','11090102','11090103','11090104','11090105','11090106','11090201','11100101','11100103','11100104','11100201','11100202','11100203','11130301'"
        End Select

        Dim da As New SqlDataAdapter("SELECT s.site_no, s.station_nm, s.huc_cd, s.agency_cd, s.dec_lat_va, s.dec_long_va, s.site_tp_cd, s.district_cd FROM nwisweb.dbo.SITEFILE AS s INNER JOIN nwisweb.dbo.rt_bol As b ON s.site_id = b.site_id WHERE s.district_cd = 48 AND s.huc_cd In(" & huc_codes & ")", cn)
        Dim ds As New DataSet
        da.Fill(ds, "hucSites")
        Return ds
    End Function

    <WebMethod(Description:="Gets list of active sites for a WSC or office from TX SIMS server")> _
    Public Function GetTXSitesByWSC(ByVal wsc_id As Integer, ByVal office_id As Integer, ByVal site_type As String) As DataSet
        Dim where_stmt As String = Nothing
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)

        If office_id <> 0 Then
            where_stmt = " AND (lo.office_id=" & office_id & ")"
        End If

        If site_type <> "0" Then
            where_stmt = where_stmt & " AND (lst.sims_site_tp='" & site_type & "')"
        End If

        Dim da As New SqlDataAdapter("SELECT DISTINCT s.site_no, s.agency_cd, s.station_nm, lo.office_cd," & _
            " lo.office_id, lo.wsc_id, s.site_tp_cd, s.agency_use_cd, ISNULL(tlt.trip_nm,'NA')," & _
            " ISNULL(tlt.user_id,'NA'), ISNULL(rst.type_ds,'NA')" & _
            " FROM RMS_Record_Master AS rsm INNER JOIN" & _
            " RMS_Record_Types AS rst ON rsm.record_type_id = rst.record_type_id RIGHT OUTER JOIN" & _
            " SIMS_Site_Master AS ssm INNER JOIN" & _
            " lut_Office AS lo ON ssm.office_id = lo.office_id INNER JOIN" & _
            " nwisweb.dbo.SITEFILE AS s ON ssm.nwisweb_site_id = s.site_id ON rsm.site_id = ssm.site_id LEFT OUTER JOIN" & _
            " Trip_Lut_Trip AS tlt INNER JOIN" & _
            " Trip_Site_Master AS tsm ON tlt.trip_id = tsm.trip_id ON ssm.site_id = tsm.site_id LEFT OUTER JOIN" & _
            " lut_site_tp AS lst ON s.site_tp_cd = lst.site_tp_cd" & _
            " WHERE (lo.wsc_id = " & wsc_id & ")" & where_stmt & _
            " ORDER BY lo.office_cd, s.site_no", cn)

        Dim ds As New DataSet
        da.Fill(ds, "sitelist")
        Return ds
    End Function

    <WebMethod(Description:="Gets list of sites for all WSCs from TX SIMS server")> _
    Public Function GetTXSitesInSIMS() As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)

        Dim da As New SqlDataAdapter("SELECT DISTINCT s.site_no, s.nwis_host, s.agency_cd, s.station_nm, lo.office_cd," & _
            " lo.office_id, lo.wsc_id, lw.wsc_cd, s.site_tp_cd, s.agency_use_cd" & _
            " FROM SIMS_Site_Master AS ssm INNER JOIN" & _
            " lut_Office AS lo ON ssm.office_id = lo.office_id INNER JOIN lut_WSC AS lw ON lo.wsc_id = lw.wsc_id" & _
            " INNER JOIN nwisweb.dbo.SITEFILE AS s ON ssm.nwisweb_site_id = s.site_id " & _
            " ORDER BY lw.wsc_cd, s.site_no", cn)

        Dim ds As New DataSet
        da.Fill(ds, "sitelist")
        Return ds
    End Function

    <WebMethod(Description:="Gets all fields from the stored procedure SP_Eval_MPLSumByConfigCat for Ed's scripts")> _
    Public Function GetMPLSumByConfigCat(ByVal nwis_host As String, ByVal wy As Integer) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("SP_Eval_MPLSumByConfigCat", cn)

        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.Add(New SqlParameter("@nwis_host", SqlDbType.VarChar, 12)).Value = nwis_host
        da.SelectCommand.Parameters.Add(New SqlParameter("@wy", SqlDbType.Int)).Value = wy

        Dim ds As New DataSet
        da.Fill(ds, "MPLSum")
        Return ds
    End Function

    <WebMethod(Description:="Gets information about an employee")> _
    Public Function GetTXEmployeeInfo(ByVal user_id As String) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("SP_Personnel_by_WSC_office_or_user_id", cn)

        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.Add("@wsc_id", SqlDbType.Int).Value = 0
        da.SelectCommand.Parameters.Add("@office_id", SqlDbType.Int).Value = 0
        da.SelectCommand.Parameters.Add("@user_id", SqlDbType.NVarChar, 30).Value = user_id
        da.SelectCommand.Parameters.Add("@show_reports", SqlDbType.NVarChar, 3).Value = "no"
        da.SelectCommand.Parameters.Add("@status", SqlDbType.NVarChar, 4).Value = "True"
        da.SelectCommand.Parameters.Add("@manage", SqlDbType.NVarChar, 3).Value = "no"

        Dim ds As New DataSet
        da.Fill(ds, "employeeinfo")
        Return ds
    End Function

    <WebMethod(Description:="Gets information about an employee from Active Directory")> _
    Public Function GetTXEmployeeInfoFromAD(ByVal user_id As String) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("spz_GetUserInfoFromAD", cn)

        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.Add("@user_id", SqlDbType.NVarChar, 50).Value = user_id

        Dim ds As New DataSet
        da.Fill(ds, "employeeinfo")
        Return ds
    End Function

    <WebMethod(Description:="Gets PASS users and access levels for a WSC")> _
    Public Function GetTXPASSUsers(ByVal wsc_id As Integer, ByVal access_level As String) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("SP_Personnel_PASSAccess_by_WSC", cn)

        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.Add("@wsc_id", SqlDbType.Int).Value = wsc_id
        da.SelectCommand.Parameters.Add("@access_level", SqlDbType.NVarChar, 10).Value = access_level

        Dim ds As New DataSet
        da.Fill(ds, "PASSaccessinfo")
        Return ds
    End Function

    <WebMethod(Description:="Gets all sites in PASS with DCP IDs")> _
    Public Function GetPASSSiteInfo() As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("SELECT site_no, dcp_id FROM simsdb_water.dbo.PASS_Site_Master", cn)

        Dim ds As New DataSet
        da.Fill(ds, "PASSSiteInfo")
        Return ds
    End Function

    <WebMethod(Description:="Gets operator user names by site number")> _
    Public Function GetTXOperators(ByVal site_no As String, ByVal agency_cd As String) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("SELECT rrm.operator_va FROM SIMS_Site_Master AS ssm INNER JOIN RMS_Record_Master AS rrm ON ssm.site_id = rrm.site_id WHERE (ssm.site_no = '" & site_no & "') AND (ssm.agency_cd = '" & agency_cd & "')", cn)

        Dim ds As New DataSet
        da.Fill(ds, "Operators")
        Return ds
    End Function

    <WebMethod(Description:="Gets field trip user names by site number")> _
    Public Function GetTXFieldTripUsers(ByVal site_no As String, ByVal agency_cd As String) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("SELECT tlt.user_id FROM SIMS_Site_Master AS ssm INNER JOIN Trip_Site_Master AS tsm ON ssm.site_id = tsm.site_id INNER JOIN Trip_Lut_Trip AS tlt ON tsm.trip_id = tlt.trip_id WHERE (ssm.site_no = '" & site_no & "') AND (ssm.agency_cd = '" & agency_cd & "')", cn)

        Dim ds As New DataSet
        da.Fill(ds, "FieldTripUsers")
        Return ds
    End Function

    <WebMethod(Description:="Gets site information for the KML downloads")> _
    Public Function GetTXSitesForKML(ByVal office_id As String, ByVal wsc_id As Integer) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("SP_Sites_By_WSC_KMZMapping", cn)

        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.Add("@wsc_id", SqlDbType.Int).Value = wsc_id
        da.SelectCommand.Parameters.Add("@office_id", SqlDbType.Int).Value = CInt(office_id)

        Dim ds As New DataSet
        da.Fill(ds, "siteinfo")
        Return ds
    End Function

    <WebMethod(Description:="Gets all the office info by wsc id")> _
    Public Function GetOfficeInfoByWSC(ByVal wsc_id As Integer) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("SELECT * FROM lut_Office AS lo WHERE lo.wsc_id = " + wsc_id.ToString(), cn)

        Dim ds As New DataSet
        da.Fill(ds, "OfficeList")
        Return ds
    End Function

    <WebMethod(Description:="Gets office information")> _
    Public Function GetTXOfficeInfo(ByVal office_id As String) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("SP_Office_Info", cn)

        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.Add("@office_id", SqlDbType.Int).Value = CInt(office_id)
        da.SelectCommand.Parameters.Add("@office_cd", SqlDbType.NVarChar).Value = "0"
        da.SelectCommand.Parameters.Add("@wsc_id", SqlDbType.Int).Value = 0
        da.SelectCommand.Parameters.Add("@site_id", SqlDbType.Int).Value = 0
        da.SelectCommand.Parameters.Add("@action", SqlDbType.NVarChar, 12).Value = "byofficeid"

        Dim ds As New DataSet
        da.Fill(ds, "officeinfo")
        Return ds
    End Function

    <WebMethod(Description:="Gets district information")> _
    Public Function GetTXDistrictInfo(ByVal district_cd As String, ByVal primaryOU As String) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("SP_District_Info", cn)

        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.Add("@district_cd", SqlDbType.NVarChar, 3).Value = district_cd
        da.SelectCommand.Parameters.Add("@primaryOU", SqlDbType.NVarChar, 255).Value = primaryOU
        da.SelectCommand.Parameters.Add("@region_cd", SqlDbType.NVarChar, 50).Value = "0"

        Dim ds As New DataSet
        da.Fill(ds, "districtinfo")
        Return ds
    End Function

    <WebMethod(Description:="Gets WSC information")> _
    Public Function GetTXWSCInfo(ByVal wsc_id As Integer, ByVal primaryOU As String) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("SP_WSC_Info", cn)

        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.Add("@wsc_id", SqlDbType.Int).Value = wsc_id
        da.SelectCommand.Parameters.Add("@primaryOU", SqlDbType.NVarChar, 255).Value = primaryOU
        da.SelectCommand.Parameters.Add("@region_cd", SqlDbType.NVarChar, 50).Value = "0"

        Dim ds As New DataSet
        da.Fill(ds, "wscinfo")
        Return ds
    End Function

    <WebMethod(Description:="Gets WSC information by WSC Code")> _
    Public Function GetWSCInfoByCode(ByVal wsc_cd As String) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("SELECT * FROM lut_WSC WHERE (wsc_cd = '" & wsc_cd & "')", cn)

        Dim ds As New DataSet
        da.Fill(ds, "wscinfo")
        Return ds
    End Function

    <WebMethod(Description:="Gets the elements for a site. Can return all elements, only SDESC, SANL, or MANU elements")> _
    Public Function GetElementsBySite(ByVal doc_type As String, ByVal site_no As String, ByVal agency_cd As String) As DataSet
        If agency_cd = "" Then
            agency_cd = "USGS"
        End If

        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("SP_Element_Info_by_site_id_and_type", cn)

        da.SelectCommand.CommandType = CommandType.StoredProcedure
        If doc_type = "" Then da.SelectCommand.Parameters.Add("@doc_type", SqlDbType.NVarChar, 5).Value = DBNull.Value Else da.SelectCommand.Parameters.Add("@doc_type", SqlDbType.NVarChar, 5).Value = doc_type
        da.SelectCommand.Parameters.Add("@site_no", SqlDbType.NVarChar, 15).Value = site_no
        da.SelectCommand.Parameters.Add("@agency_cd", SqlDbType.NVarChar, 5).Value = agency_cd

        Dim ds As New DataSet
        da.Fill(ds, "elementlist")
        Return ds
    End Function

    <WebMethod(Description:="Gets the single element information by site ID and element ID.")> _
    Public Function GetElementBySiteAndElement(ByVal element_id As Integer, ByVal site_id As Integer) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("SP_Element_Info_by_element_and_site_id", cn)

        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.Add("@element_id", SqlDbType.Int).Value = element_id
        da.SelectCommand.Parameters.Add("@site_id", SqlDbType.Int).Value = site_id

        Dim ds As New DataSet
        da.Fill(ds, "elementinfo")
        Return ds
    End Function

    <WebMethod(Description:="Generates the Records Management System Latency Reports.")> _
    Public Function GetRMSLatencyReport(ByVal wsc_cd As String, ByVal theme As String) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("SP_RMS_Latency_Old", cn)

        If wsc_cd = "" Then
            wsc_cd = "%"
        End If
        If theme = "" Then
            theme = "DETAILS"
        End If
        'If wpcat1 = 0 Then
        '    wpcat1 = 91
        'End If
        'If wpcat2 = 0 Then
        '    wpcat2 = 181
        'End If
        'If wpcat3 = 0 Then
        '    wpcat3 = 301
        'End If
        'If cpcat1 = 0 Then
        '    cpcat1 = 121
        'End If
        'If cpcat2 = 0 Then
        '    cpcat2 = 211
        'End If
        'If cpcat3 = 0 Then
        '    cpcat3 = 331
        'End If
        'If rpcat1 = 0 Then
        '    rpcat1 = 151
        'End If
        'If rpcat2 = 0 Then
        '    rpcat2 = 241
        'End If
        'If rpcat3 = 0 Then
        '    rpcat3 = 365
        'End If

        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.Add("@wsc_cd", SqlDbType.NVarChar, 15).Value = wsc_cd
        da.SelectCommand.Parameters.Add("@theme", SqlDbType.NVarChar, 7).Value = theme
        'da.SelectCommand.Parameters.Add("@wpcat1", SqlDbType.Int).Value = DBNull.Value
        'da.SelectCommand.Parameters.Add("@wpcat2", SqlDbType.Int).Value = DBNull.Value
        'da.SelectCommand.Parameters.Add("@wpcat3", SqlDbType.Int).Value = DBNull.Value
        'da.SelectCommand.Parameters.Add("@cpcat1", SqlDbType.Int).Value = DBNull.Value
        'da.SelectCommand.Parameters.Add("@cpcat2", SqlDbType.Int).Value = DBNull.Value
        'da.SelectCommand.Parameters.Add("@cpcat3", SqlDbType.Int).Value = DBNull.Value
        'da.SelectCommand.Parameters.Add("@rpcat1", SqlDbType.Int).Value = DBNull.Value
        'da.SelectCommand.Parameters.Add("@rpcat2", SqlDbType.Int).Value = DBNull.Value
        'da.SelectCommand.Parameters.Add("@rpcat3", SqlDbType.Int).Value = DBNull.Value

        Dim ds As New DataSet
        da.Fill(ds, "RMSlatencyreport")
        Return ds
    End Function

    <WebMethod(Description:="Gets site list for SDESC Repository within a certain amount of hours of last revised")> _
    Public Function GetSDSiteListByLastRevised(ByVal lu As String) As DataSet
        Dim secs As Integer = Convert.ToInt32(lu) * 3600
        Dim seconds As String = secs.ToString

        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("SELECT DISTINCT    lw.wsc_cd, lo.office_cd, ssm.site_no, ssm.agency_cd, s.station_nm, (CASE WHEN agency_use_cd In('A','M','L') THEN 'A' ELSE 'I' END) AS active_fg, ers.revised_dt" &
            " FROM lut_WSC AS lw INNER JOIN" &
            "   lut_Office AS lo ON lw.wsc_id = lo.wsc_id INNER JOIN" &
            "   SIMS_Site_Master AS ssm ON lo.office_id = ssm.office_id INNER JOIN" &
            "   Elem_Report_Sum AS ers ON ssm.site_id = ers.site_id INNER JOIN" &
            "   nwisweb.dbo.SITEFILE AS s ON s.site_id = ssm.nwisweb_site_id" &
            " WHERE DATEDIFF(second, ers.revised_dt, GETDATE()) <= " & seconds & " AND ers.report_type_cd = 'SDESC'", cn)

        Dim ds As New DataSet
        da.Fill(ds, "sitelist")
        Return ds
    End Function

    <WebMethod(Description:="Gets site list for SDESC Repository")> _
    Public Function GetSDSiteList() As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        Dim da As New SqlDataAdapter("SELECT DISTINCT lw.wsc_cd, lo.office_cd, ssm.site_no, ssm.agency_cd, s.station_nm, (CASE WHEN agency_use_cd In('A','M','L') THEN 'A' ELSE 'I' END) AS active_fg, ers.revised_dt" &
            " FROM lut_WSC AS lw INNER JOIN" &
            "   lut_Office AS lo ON lw.wsc_id = lo.wsc_id INNER JOIN" &
            "   SIMS_Site_Master AS ssm ON lo.office_id = ssm.office_id INNER JOIN" &
            "   Elem_Report_Sum AS ers ON ssm.site_id = ers.site_id INNER JOIN" &
            "   nwisweb.dbo.SITEFILE AS s ON s.site_id = ssm.nwisweb_site_id" &
            " WHERE ers.report_type_cd = 'SDESC'", cn)

        Dim ds As New DataSet
        da.Fill(ds, "sitelist")
        Return ds
    End Function

#Region "RMS Status Reports"
    <WebMethod(Description:="Gets RMS 120 day region status information from stored proc SP_TOTALS_PART_by_region_or_district_Q_sites from SIMS")> _
    Public Function PartRegDistQ(ByVal querydate As DateTime, ByVal region_cd As String, ByVal wsc_id As Integer, ByVal partenddate As DateTime) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        'Create a Data Adapter, then provide the name of the stored procedure
        Dim da As New SqlDataAdapter("SP_TOTALS_PART_by_region_or_WSC_Q_sites", cn)

        'Set the command type as Stored Procedure
        da.SelectCommand.CommandType = CommandType.StoredProcedure

        'Create and add parameters to Parameters collection for stored procedure

        da.SelectCommand.Parameters.Add(New SqlParameter("@region_cd", SqlDbType.NVarChar, 50))
        da.SelectCommand.Parameters("@region_cd").Value = region_cd
        da.SelectCommand.Parameters.Add(New SqlParameter("@wsc_id", SqlDbType.Int))
        da.SelectCommand.Parameters("@wsc_id").Value = wsc_id

        da.SelectCommand.Parameters.Add(New SqlParameter("@querydate", SqlDbType.DateTime))
        da.SelectCommand.Parameters("@querydate").Value = querydate
        da.SelectCommand.Parameters.Add(New SqlParameter("@partenddate", SqlDbType.DateTime))
        da.SelectCommand.Parameters("@partenddate").Value = partenddate

        'Create a DataSet to hold the records
        Dim ds As New DataSet
        'Fill the DataSet with the rows returned
        da.Fill(ds, "Total")
        Return ds
    End Function

    <WebMethod(Description:="Gets RMS total status information from stored proc SP_TOTALS_Current_by_region_or_district from SIMS")> _
    Public Function CurRegDist(ByVal querydate As DateTime, ByVal region_cd As String, ByVal wsc_id As Integer) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        'Create a Data Adapter, then provide the name of the stored procedure
        Dim da As New SqlDataAdapter("SP_TOTALS_Current_by_region_or_WSC", cn)

        'Set the command type as Stored Procedure
        da.SelectCommand.CommandType = CommandType.StoredProcedure

        'Create and add parameters to Parameters collection for stored procedure

        da.SelectCommand.Parameters.Add(New SqlParameter("@region_cd", SqlDbType.NVarChar, 50))
        da.SelectCommand.Parameters("@region_cd").Value = region_cd

        da.SelectCommand.Parameters.Add(New SqlParameter("@wsc_id", SqlDbType.Int))
        da.SelectCommand.Parameters("@wsc_id").Value = wsc_id

        da.SelectCommand.Parameters.Add(New SqlParameter("@querydate", SqlDbType.DateTime))
        da.SelectCommand.Parameters("@querydate").Value = querydate


        'Create a DataSet to hold the records
        Dim ds As New DataSet
        'Fill the DataSet with the rows returned
        da.Fill(ds, "Total")
        Return ds
    End Function
    <WebMethod(Description:="Gets RMS total status information from stored proc SP_TOTALS_Current_Continuous_by_region_or_district from SIMS")> _
    Public Function CurContRegDist(ByVal querydate As DateTime, ByVal region_cd As String, ByVal wsc_id As Integer) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        'Create a Data Adapter, then provide the name of the stored procedure
        Dim da As New SqlDataAdapter("SP_TOTALS_Current_Continuous_by_region_or_WSC", cn)

        'Set the command type as Stored Procedure
        da.SelectCommand.CommandType = CommandType.StoredProcedure

        'Create and add parameters to Parameters collection for stored procedure

        da.SelectCommand.Parameters.Add(New SqlParameter("@region_cd", SqlDbType.NVarChar, 50))
        da.SelectCommand.Parameters("@region_cd").Value = region_cd

        da.SelectCommand.Parameters.Add(New SqlParameter("@wsc_id", SqlDbType.Int))
        da.SelectCommand.Parameters("@wsc_id").Value = wsc_id

        da.SelectCommand.Parameters.Add(New SqlParameter("@querydate", SqlDbType.DateTime))
        da.SelectCommand.Parameters("@querydate").Value = querydate


        'Create a DataSet to hold the records
        Dim ds As New DataSet
        'Fill the DataSet with the rows returned
        da.Fill(ds, "Total")
        Return ds
    End Function
    <WebMethod(Description:="Gets RMS total status information from stored proc SP_TOTALS_Current_by_region_or_district_Q_Sites from SIMS")> _
    Public Function CurRegDistQ(ByVal querydate As DateTime, ByVal region_cd As String, ByVal wsc_id As Integer) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        'Create a Data Adapter, then provide the name of the stored procedure
        Dim da As New SqlDataAdapter("SP_TOTALS_Current_by_region_or_district_Q_sites", cn)

        'Set the command type as Stored Procedure
        da.SelectCommand.CommandType = CommandType.StoredProcedure

        'Create and add parameters to Parameters collection for stored procedure

        da.SelectCommand.Parameters.Add(New SqlParameter("@region_cd", SqlDbType.NVarChar, 50))
        da.SelectCommand.Parameters("@region_cd").Value = region_cd

        da.SelectCommand.Parameters.Add(New SqlParameter("@wsc_id", SqlDbType.Int))
        da.SelectCommand.Parameters("@wsc_id").Value = wsc_id

        da.SelectCommand.Parameters.Add(New SqlParameter("@querydate", SqlDbType.DateTime))
        da.SelectCommand.Parameters("@querydate").Value = querydate


        'Create a DataSet to hold the records
        Dim ds As New DataSet
        'Fill the DataSet with the rows returned
        da.Fill(ds, "Total")
        Return ds
    End Function

    <WebMethod(Description:="Gets RMS total status information from stored proc SP_TOTALS_Current_Non_Continuous_by_region_or_district from SIMS")> _
    Public Function CurNonContRegDist(ByVal querydate As DateTime, ByVal region_cd As String, ByVal wsc_id As Integer) As DataSet
        Dim cs As String = Config.ConnectionInfo
        Dim cn As New SqlConnection(cs)
        'Create a Data Adapter, then provide the name of the stored procedure
        Dim da As New SqlDataAdapter("SP_TOTALS_Current_Non_Continuous_by_region_or_district", cn)

        'Set the command type as Stored Procedure
        da.SelectCommand.CommandType = CommandType.StoredProcedure

        'Create and add parameters to Parameters collection for stored procedure

        da.SelectCommand.Parameters.Add(New SqlParameter("@region_cd", SqlDbType.NVarChar, 50))
        da.SelectCommand.Parameters("@region_cd").Value = region_cd

        da.SelectCommand.Parameters.Add(New SqlParameter("@wsc_id", SqlDbType.Int))
        da.SelectCommand.Parameters("@wsc_id").Value = wsc_id

        da.SelectCommand.Parameters.Add(New SqlParameter("@querydate", SqlDbType.DateTime))
        da.SelectCommand.Parameters("@querydate").Value = querydate


        'Create a DataSet to hold the records
        Dim ds As New DataSet
        'Fill the DataSet with the rows returned
        da.Fill(ds, "Total")
        Return ds
    End Function
#End Region

End Class