Imports Microsoft.VisualBasic
Imports System
Imports System.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports Google.KML
Imports System.Data.SqlClient

Public Class KML

    Private Sub New()
    End Sub

    Public Shared Function GetSites(ByVal office_id As String, ByVal wsc_id As Integer, ByVal trip_id As Integer) As DataTable
        Dim dt As New DataTable
        dt.Clear()

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand("SP_Sites_By_WSC_KMZMapping", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = wsc_id
            cmd.Parameters.Add("@office_id", SqlDbType.Int).Value = CInt(office_id)
            cmd.Parameters.Add("@trip_id", SqlDbType.Int).Value = trip_id

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            cnx.Close()
        End Using

        Return dt
    End Function

    Public Shared Function GetOfficeInfo(ByVal office_id As String, ByVal what_info As String, ByVal wsc_id As Integer) As String
        Dim pOut As String = Nothing
        Dim office_nm As String = Nothing
        Dim office_lat As String = Nothing
        Dim office_long As String = Nothing
        Dim office_addrs As String = Nothing

        Dim dt As New DataTable
        dt.Clear()

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand("SP_Office_Info", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@office_id", SqlDbType.Int).Value = CInt(office_id)
            cmd.Parameters.Add("@office_cd", SqlDbType.NVarChar, 3).Value = "0"
            cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@action", SqlDbType.NVarChar, 12).Value = "byofficeid"

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                office_nm = row("office_nm")
                Try
                    office_lat = row("dec_lat_va")
                    office_long = row("dec_long_va")
                Catch ex As Exception
                    office_lat = Nothing
                    office_long = Nothing
                End Try
                office_addrs = row("street_addrs") & ", " & row("city_st_zip")
            Next

            cnx.Close()
        End Using

        Select Case what_info
            Case "name"
                pOut = office_nm
            Case "geo"
                pOut = office_long & ", " & office_lat
            Case "address"
                pOut = office_addrs
        End Select

        Return pOut
    End Function

    Public Shared Function GetWSCInfo(ByVal wsc_id As Integer, ByVal what_info As String) As String
        Dim pOut As String = Nothing
        Dim cnxstrcol As ConnectionStringSettingsCollection = ConfigurationManager.ConnectionStrings
        Dim wsc_nm As String = Nothing
        Dim wsc_cd As String = Nothing

        Using cnx As New SqlConnection(cnxstrcol.Item("simsdbConnectionString").ConnectionString)
            cnx.Open()

            Dim cmd As New SqlCommand("SP_WSC_Info", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            Dim dt As New DataTable
            cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = wsc_id
            cmd.Parameters.Add("@primaryOU", SqlDbType.NVarChar, 12).Value = "0"
            cmd.Parameters.Add("@region_cd", SqlDbType.NVarChar, 12).Value = "0"

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                wsc_nm = row("wsc_nm")
                wsc_cd = row("wsc_cd")
            Next

            Select Case what_info
                Case "name"
                    pOut = wsc_nm
                Case "abbrev"
                    pOut = wsc_cd
            End Select

            cnx.Close()
        End Using

        Return pOut
    End Function

    Public Shared Function GetFieldTrips(ByVal office_id As Integer) As DataTable
        Dim dt As New DataTable
        dt.Clear()

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand("SELECT trip_id, trip_nm, user_id FROM Trip_Lut_Trip WHERE office_id = " & office_id.ToString(), cnx)
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            cnx.Close()
        End Using

        Return dt
    End Function

End Class
