Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb
Imports Subgurim.Controles
Imports System.Drawing
Imports System.Collections.Generic

Public Class FieldTripMap
    Inherits System.Web.UI.Page

    Private o As Office

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim wsc_id As Integer = 0
        Dim office_id As Integer = Request.QueryString("office_id")
        Dim trip_id As Integer = Request.QueryString("trip_id")

        o = New Office(office_id)

        If Request.QueryString("wsc_id") <> Nothing Then
            wsc_id = Request.QueryString("wsc_id")
            Session("wsc_id") = wsc_id
        ElseIf Not office_id = 0 Then
            Session("wsc_id") = o.WSCID
            wsc_id = Session("wsc_id")
        End If

        If Not Page.IsPostBack Then
            SetupBasicGMapDisplay(wsc_id)
            PopulateOfficeDDL(wsc_id, office_id)
            PopulateFieldTripList(trip_id, office_id)
            AddOfficeToMap(office_id)
        End If
    End Sub

    Public Sub SetupBasicGMapDisplay(ByVal wsc_id As Integer)

        RepositionMap(wsc_id, 0, 0)
        GMap.enableHookMouseWheelToZoom = True
        GMap.addControl(New GControl(GControl.preBuilt.LargeMapControl))
        GMap.addControl(New GControl(GControl.preBuilt.MapTypeControl))
        GMap.addControl(New GControl(GControl.preBuilt.GOverviewMapControl))
        GMap.addMapType(GMapType.GTypes.Physical)
        GMap.mapType = GMapType.GTypes.Physical
        GMap.resetMarkers()

    End Sub

    Public Sub PopulateOfficeDDL(ByVal wsc_id As Integer, ByVal office_id As Integer)
        Dim sql As String = Nothing
        Dim dbcomm As SqlCommand
        Dim OfficeList As SqlDataReader
        Dim OfficeName As SqlDataReader

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            sql = "SELECT office_nm FROM lut_Office WHERE office_id=" & office_id
            dbcomm = New SqlCommand(sql, cnx)
            OfficeName = dbcomm.ExecuteReader

            OfficeName.Read()
            Dim office_nm As String = OfficeName.Item(0)
            OfficeName.Close()

            sql = "SELECT DISTINCT office_nm, office_id" & _
                " FROM lut_Office" & _
                " WHERE (wsc_id = " & wsc_id & ") And office_id <> " & office_id & _
                " ORDER BY office_nm"
            dbcomm = New SqlCommand(sql, cnx)
            OfficeList = dbcomm.ExecuteReader()

            If OfficeList.HasRows Then
                ddlOffice.DataSource = OfficeList
                ddlOffice.DataBind()
                ddlOffice.Items.Insert(0, New ListItem(office_nm, office_id.ToString))
            End If

            OfficeList.Close()
            cnx.Close()
        End Using
    End Sub

    Public Sub PopulateFieldTripList(ByVal trip_id As Integer, ByVal office_id As Integer)
        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As SqlCommand = New SqlCommand("SP_Trip_Info", cnx)
            Dim dr As SqlDataReader

            cmd.CommandType = Data.CommandType.StoredProcedure

            cmd.Parameters.Add("@office_id", SqlDbType.Int).Value = office_id
            cmd.Parameters.Add("@trip_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@trip_nm", SqlDbType.NVarChar, 255).Value = "0"
            cmd.Parameters.Add("@user_id", SqlDbType.NVarChar, 50).Value = "0"

            dr = cmd.ExecuteReader

            cblFieldTrips.DataSource = dr
            cblFieldTrips.DataBind()
            cblFieldTrips.Items.Insert(0, New ListItem("<span style=""font-size:8pt;"">R-T sites not belonging to any trip <img src=""images/icons/0.png"" /></span>", "0"))
            cnx.Close()
        End Using

        If trip_id > 0 Then
            Try
                cblFieldTrips.Items.FindByValue(trip_id.ToString).Selected = True
            Catch ex As Exception

            End Try
            AddSitesToMap(Session("wsc_id"), office_id, trip_id, cblFieldTrips.SelectedIndex)
        End If
    End Sub

    Public Sub AddSitesToMap(ByVal wsc_id As Integer, ByVal office_id As Integer, ByVal trip_id As Integer, ByVal SelectedIndex As Integer)
        Dim rowCount As Integer = 0
        Dim lat As Double = 0
        Dim latAvg As Double = 0
        Dim latMin As Double = 0
        Dim latMax As Double = 0
        Dim latPrev As Double = 0
        Dim lng As Double = 0
        Dim lngAvg As Double = 0
        Dim lngMin As Double = 0
        Dim lngMax As Double = 0
        Dim lngPrev As Double = 0
        Dim MapZoomLevel As Integer = 0
        Dim site_tp_cd As String
        Dim latlon As GLatLng
        Dim marker As GMarker
        Dim window As GInfoWindow
        Dim winText As String = ""
        Dim trip_nm As String = ""
        Dim i As Integer = 0

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As SqlCommand = New SqlCommand("SP_Trip_Lists_using_trip_id", cnx)
            Dim dt As New DataTable

            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@trip_id", SqlDbType.Int).Value = trip_id
            cmd.Parameters.Add("@office_id", SqlDbType.Int).Value = office_id
            cmd.Parameters.Add("@status", SqlDbType.NVarChar, 10).Value = "assigned"

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                Dim site_no As String = row("site_no")

                If Not row("dec_lat_va") Is DBNull.Value Then
                    lat = row("dec_lat_va")
                End If
                If Not row("dec_long_va") Is DBNull.Value Then
                    lng = row("dec_long_va")
                End If

                If lat = 0 Or lng = 0 Then
                    i += 1
                Else
                    site_tp_cd = row("site_tp_cd").ToString
                    trip_nm = row("trip_full_nm").ToString

                    If lat <> latPrev And lng <> lngPrev Then
                        winText = ""
                    Else
                        winText = winText & "<hr />"
                    End If

                    winText = winText & "<b>" & row("site_no").ToString & " " & row("station_nm").ToString & "<br />" & _
                        trip_nm & "</b><br /><br />" & _
                        "<a href=""https://sims.water.usgs.gov/SIMSClassic/StationInfo.asp?site_no=" & row("site_no").ToString & _
                        """ target=""_blank"">SIMS Station Information Page</a><br />" & _
                        "<a href=""http://waterdata.usgs.gov/nwis/nwisman/?site_no=" & row("site_no").ToString & _
                        """ target=""_blank"">NWIS Web</a>"

                    latAvg = latAvg + lat
                    lngAvg = lngAvg + lng
                    If i = 0 Then
                        latMin = lat
                        latMax = lat
                        lngMin = lng
                        lngMax = lng
                    Else
                        If lat < latMin Then latMin = lat
                        If lat > latMax Then latMax = lat
                        If lng < lngMin Then lngMin = lng
                        If lng > lngMax Then lngMax = lng
                    End If

                    latlon = New GLatLng(lat, lng)
                    marker = New GMarker(latlon, GetMarkerOpts(SelectedIndex))
                    window = New GInfoWindow(marker, winText, False, GListener.Event.click)
                    GMap.addInfoWindow(window)

                    latPrev = lat
                    lngPrev = lng

                    i += 1
                End If
            Next

            rowCount = dt.Rows.Count

            dt.Clear()
            cnx.Close()
        End Using

        If rowCount = 0 Then
            SetupBasicGMapDisplay(Session("wsc_id"))
        Else
            'zoom/center map based on markers

            latAvg = (latAvg / i)
            lngAvg = (lngAvg / i)
            MapZoomLevel = GetMapZoomLevel(latMin, latMax, lngMin, lngMax)
            GMap.setCenter(New GLatLng(latAvg, lngAvg), MapZoomLevel)
        End If
        lblResultsCount.Text = "Sites returned: " & rowCount.ToString

    End Sub

    Public Sub AddOfficeToMap(ByVal office_id As Integer)
        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim sql As String = "SELECT dec_lat_va, dec_long_va, office_nm, street_addrs, city_st_zip, ph_no" & _
                " FROM lut_Office WHERE office_id=" & office_id & " AND dec_lat_va Is Not Null"
            Dim cmd As SqlCommand = New SqlCommand(sql, cnx)
            Dim dr As SqlDataReader = cmd.ExecuteReader

            Dim latlon As GLatLng
            Dim marker As GMarker
            Dim window As GInfoWindow
            Dim winText As String

            Do While dr.Read
                winText = "<b>" & dr.Item(2) & "<br />" & _
                    dr.Item(3) & "<br />" & _
                    dr.Item(4) & "<br />" & _
                    dr.Item(5) & "</b>"

                latlon = New GLatLng(dr.Item(0), dr.Item(1))
                marker = New GMarker(latlon)
                window = New GInfoWindow(marker, winText, False, GListener.Event.click)
                GMap.addInfoWindow(window)
            Loop

            dr.Close()
            cnx.Close()
        End Using
    End Sub

    Public Function GetMapZoomLevel(ByVal latMin As Double, ByVal latMax As Double, ByVal lngMin As Double, ByVal lngMax As Double) As Integer
        Dim MapZoomLevel As Integer = 0
        Dim latExtent As Double
        Dim lngExtent As Double

        latExtent = (latMax - latMin)
        lngExtent = (lngMax - lngMin)

        'simple lookup table for zoom levels
        If (lngExtent > 100) Then MapZoomLevel = 3
        If (lngExtent > 40 And lngExtent < 100) Then MapZoomLevel = 3
        If (lngExtent > 15 And lngExtent < 40) Then MapZoomLevel = 4
        If (lngExtent > 4 And lngExtent < 15) Then MapZoomLevel = 5
        If (lngExtent > 1 And lngExtent < 4) Then MapZoomLevel = 6
        If (lngExtent > 0.5 And lngExtent < 1) Then MapZoomLevel = 7
        If (lngExtent > 0.25 And lngExtent < 0.5) Then MapZoomLevel = 8
        If (lngExtent > 0.125 And lngExtent < 0.25) Then MapZoomLevel = 9
        If (lngExtent < 0.125) Then MapZoomLevel = 10

        Return MapZoomLevel
    End Function

    Public Function GetMarkerOpts(ByVal SelectedIndex As Integer) As GMarkerOptions
        Dim icon As GIcon = New GIcon()
        Dim x As Integer = SelectedIndex

        If x > 37 Then
            x = x - 37
        End If

        'Create icon images for markers based on site type
        icon.image = "images/icons/" & x.ToString & ".png"
        icon.shadow = "http://wdr.water.usgs.gov/adrgmap/images/icons16x16/sw_onlys.png"

        icon.iconSize = New GSize(16, 16)
        icon.shadowSize = New GSize(26, 16)
        icon.iconAnchor = New GPoint(8, 8)
        icon.infoWindowAnchor = New GPoint(8, 0)

        Dim mOpts As GMarkerOptions = New GMarkerOptions()
        mOpts.clickable = True
        mOpts.icon = icon

        Return mOpts
    End Function

    Protected Sub ddlOffice_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlOffice.SelectedIndexChanged
        GMap.resetMarkers()
        GMap.resetInfoWindows()

        AddOfficeToMap(ddlOffice.SelectedItem.Value)
        PopulateFieldTripList(0, ddlOffice.SelectedItem.Value)
        RepositionMap("0", ddlOffice.SelectedItem.Value, 0)
        lblResultsCount.Text = "Sites returned: 0"

    End Sub

    Protected Sub cblFieldTrips_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cblFieldTrips.SelectedIndexChanged
        GMap.resetMarkers()
        GMap.resetInfoWindows()

        Dim MyItem As ListItem
        Dim SiteCount As Integer = 0
        Dim CurrentOffice As Integer = 0

        Try
            CurrentOffice = ddlOffice.SelectedItem.Value
        Catch ex As Exception
            CurrentOffice = o.ID
        End Try

        For Each MyItem In cblFieldTrips.Items
            If MyItem.Selected Then
                AddSitesToMap(Session("wsc_id"), CurrentOffice, CInt(MyItem.Value), cblFieldTrips.Items.IndexOf(MyItem))
                SiteCount = SiteCount + GetTripSiteCount(CInt(MyItem.Value), CurrentOffice)
            End If
        Next

        lblResultsCount.Text = "Sites returned: " & SiteCount.ToString

        AddOfficeToMap(CurrentOffice)
        RepositionMap("0", CurrentOffice, 0)

    End Sub

    Function GetTripSiteCount(ByVal trip_id As Integer, ByVal office_id As Integer) As Integer
        Dim SiteCount As Integer = 0

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As SqlCommand = New SqlCommand("SP_Trip_Lists_using_trip_id", cnx)
            Dim da As SqlDataAdapter
            Dim dt As DataTable = New DataTable

            cmd.CommandType = Data.CommandType.StoredProcedure

            cmd.Parameters.Add("@trip_id", SqlDbType.Int).Value = trip_id
            cmd.Parameters.Add("@office_id", SqlDbType.Int).Value = office_id
            cmd.Parameters.Add("@status", SqlDbType.NVarChar, 10).Value = "assigned"

            da = New SqlDataAdapter(cmd)
            da.Fill(dt)

            SiteCount = dt.Rows.Count
            cnx.Close()
        End Using

        Return SiteCount
    End Function

    Public Sub RepositionMap(ByVal wsc_id As Integer, ByVal office_id As Integer, ByVal trip_id As Integer)
        Dim where_stmt As String = "dec_lat_va Is Not Null"
        If wsc_id <> 0 Then
            where_stmt = where_stmt & " AND wsc_id = " & wsc_id
        End If
        If office_id > 0 Then
            where_stmt = where_stmt & " AND office_id = " & office_id
        End If

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim sql As String = "SELECT dec_lat_va, dec_long_va" & _
                " FROM lut_Office" & _
                " WHERE " & where_stmt
            Dim cmd As SqlCommand = New SqlCommand(sql, cnx)
            Dim dr As SqlDataReader = cmd.ExecuteReader

            Dim lat As Double = 0
            Dim latAvg As Double = 0
            Dim latMin As Double = 0
            Dim latMax As Double = 0
            Dim lng As Double = 0
            Dim lngAvg As Double = 0
            Dim lngMin As Double = 0
            Dim lngMax As Double = 0
            Dim MapZoomLevel As Integer = 0

            Dim i As Integer = 0
            Do While dr.Read
                lat = dr.Item(0)
                lng = dr.Item(1)

                latAvg = latAvg + lat
                lngAvg = lngAvg + lng
                If i = 0 Then
                    latMin = lat
                    latMax = lat
                    lngMin = lng
                    lngMax = lng
                Else
                    If lat < latMin Then latMin = lat
                    If lat > latMax Then latMax = lat
                    If lng < lngMin Then lngMin = lng
                    If lng > lngMax Then lngMax = lng
                End If

                i += 1
            Loop

            latAvg = (latAvg / i)
            lngAvg = (lngAvg / i)

            If office_id > 0 Then
                MapZoomLevel = 7
            Else
                MapZoomLevel = GetMapZoomLevel(latMin, latMax, lngMin, lngMax)
            End If

            GMap.setCenter(New GLatLng(latAvg, lngAvg), MapZoomLevel)

            dr.Close()
            cnx.Close()
        End Using
    End Sub
End Class
