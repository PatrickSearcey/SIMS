Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb

Public Class EvalSiteList
    Inherits System.Web.UI.Page

    Dim cnxstrcol As ConnectionStringSettingsCollection = ConfigurationManager.ConnectionStrings
    Dim cnx As SqlConnection = New SqlConnection(cnxstrcol.Item("simsdbConnectionString").ConnectionString)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim eval_option As Integer = Request.QueryString("eval_option")
        Dim region_cd As String = Request.QueryString("region_cd")
        Dim office_cd As String = Request.QueryString("office_cd")

        Dim from_stmt As String = ""
        Dim where_stmt As String = ""

        If eval_option = 5 Then
            from_stmt = " LEFT OUTER JOIN Ops_DCP_ID AS od ON efsd.site_no = od.site_no"
        End If

        If region_cd <> "0" And region_cd <> "USA" Then
            Select Case region_cd
                Case "AK", "MW", "NE", "NW", "P", "SE", "SW"
                    where_stmt = " And lw.region_cd = '" & region_cd & "'"
                Case Else
                    where_stmt = " And (lw.wsc_cd = '" & region_cd & "')"
            End Select
        End If
        If office_cd <> "0" Then
            where_stmt = " AND efsd.office_cd = '" & office_cd & "'"
        End If

        If eval_option <> 1 Then
            where_stmt = where_stmt & GetEvalDetails(eval_option)
        End If

        cnx.Open()
        Dim sql As String = Nothing
        If eval_option = 1 Then
            sql = "SELECT DISTINCT s.site_no, s.station_nm" & _
                " FROM lut_WSC AS lw INNER JOIN lut_District AS ld ON ld.wsc_id = lw.wsc_id RIGHT OUTER JOIN" & _
                " nwisweb.dbo.SITEFILE AS s ON ld.district_cd = s.district_cd LEFT OUTER JOIN" & _
                " simsdb_water.dbo.Eval_FullSiteDiagnostics AS efsd ON s.nwis_host = efsd.nwis_host AND s.site_no = efsd.site_no AND" & _
                " s.agency_cd = efsd.agency_cd RIGHT OUTER JOIN" & _
                " nwisweb.dbo.rt_bol AS rb ON s.site_id = rb.site_id" & _
                " WHERE s.dec_lat_va Is Not Null And s.dec_long_va Is Not Null And (efsd.site_no Is NULL)" & where_stmt & _
                " ORDER BY s.site_no"
        Else
            sql = "SELECT efsd.site_no, efsd.station_nm" & _
                " FROM simsdb_water.dbo.Eval_FullSiteDiagnostics AS efsd INNER JOIN" & _
                " lut_WSC AS lw ON efsd.wsc_id = lw.wsc_id" & from_stmt & _
                " WHERE dec_lat_va Is Not Null And dec_long_va Is Not Null" & where_stmt & _
                " ORDER BY efsd.site_no"
        End If
        Dim cmd As SqlCommand = New SqlCommand(sql, cnx)
        Dim dr As SqlDataReader
        dr = cmd.ExecuteReader

        If dr.HasRows Then
            rptSiteList.DataSource = dr
            rptSiteList.DataBind()
        End If

        dr.Close()
        cnx.Close()
    End Sub

    Public Function GetEvalDetails(ByVal eval_option As Integer) As String
        Dim where_stmt As String

        Select Case eval_option
            Case 3
                where_stmt = " AND rt_bol_byinstruments = 'Y' AND rt_bol_nwisweb = 'N'"
            Case 4
                where_stmt = " AND rt_bol_byinstruments = 'N' AND rt_bol_nwisweb = 'Y'"
            Case 2
                where_stmt = " AND agency_use_cd Not In('A','L','M') AND rt_bol_nwisweb = 'Y'"
            Case 6
                where_stmt = " AND (rt_bol_nwisweb = 'Y') AND (hasSHA IS NULL) AND (hasTCP IS NULL)"
            Case 8
                where_stmt = " AND (rt_bol_nwisweb = 'Y') AND (hasSHA IS NULL) AND (hasTCP IS NOT NULL)"
            Case 7
                where_stmt = " AND (rt_bol_nwisweb = 'Y') AND (hasSHA IS NOT NULL) AND (hasTCP IS NULL)"
            Case 9
                where_stmt = " AND ((rt_bol_nwisweb = 'Y') AND (hasEMF IS NULL) OR (rt_bol_nwisweb = 'Y') AND (hasEC IS NULL))"
            Case 10
                where_stmt = " AND (rt_bol_nwisweb = 'Y') AND (hasSHA IS NOT NULL) AND (hasELEM IS NOT NULL)"
            Case 5
                where_stmt = " AND od.primary_bd = 100"
            Case 1
                where_stmt = ""
            Case Else
                where_stmt = ""
        End Select

        Return where_stmt
    End Function
End Class
