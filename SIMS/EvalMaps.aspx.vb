Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb
Imports Subgurim.Controles
Imports System.Drawing
Imports System.Collections.Generic

Public Class EvalMaps
    Inherits System.Web.UI.Page

    Dim cnxstrcol As ConnectionStringSettingsCollection = ConfigurationManager.ConnectionStrings
    Dim cnx As SqlConnection = New SqlConnection(cnxstrcol.Item("simsdbConnectionString").ConnectionString)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "SIMS - National Evaluation Maps"
        Dim wsc_id As Integer = Request.QueryString("wsc_id")

        If Not Page.IsPostBack Then
            If wsc_id = Nothing Then
                SetupBasicGMapDisplay()
                PopulateWSCDDL()

                lblResultsCount.Text = "Sites returned: 0"
                lblResultsSum.Text = ""
            Else
                Dim w As New WSC(wsc_id)

                SetupBasicGMapDisplay()
                PopulateWSCDDL()
                AddSitesToMap(1, w.Code, "0")
                ddlWSC.SelectedValue = w.Code
            End If
        End If
    End Sub

    Public Sub SetupBasicGMapDisplay()

        GMap.setCenter(New GLatLng(36.0, -96.0), 4)
        GMap.enableHookMouseWheelToZoom = True
        GMap.addControl(New GControl(GControl.preBuilt.LargeMapControl))
        GMap.addControl(New GControl(GControl.preBuilt.MapTypeControl))
        GMap.addControl(New GControl(GControl.preBuilt.GOverviewMapControl))
        GMap.addMapType(GMapType.GTypes.Physical)
        GMap.mapType = GMapType.GTypes.Physical
        GMap.resetMarkers()

    End Sub

    Public Sub PopulateWSCDDL()
        Dim sql As String = Nothing
        Dim dbcomm As SqlCommand
        Dim WSCList As SqlDataReader

        cnx.Open()
        sql = "SELECT DISTINCT lw.wsc_nm, lw.wsc_cd" & _
            " FROM lut_WSC AS lw INNER JOIN" & _
            " simsdb_water.dbo.Eval_FullSiteDiagnostics AS efsd ON lw.wsc_id = efsd.wsc_id" & _
            " ORDER BY lw.wsc_nm"
        dbcomm = New SqlCommand(sql, cnx)
        WSCList = dbcomm.ExecuteReader()

        ddlWSC.DataSource = WSCList
        ddlWSC.DataBind()

        ddlWSC.Items.Insert(0, New ListItem("United States", "USA"))
        ddlWSC.Items.Insert(1, New ListItem("Headquarters Water", "HW"))
        ddlWSC.Items.Insert(2, New ListItem("Northeast Region", "NE"))
        ddlWSC.Items.Insert(3, New ListItem("Southeast Region", "SE"))
        ddlWSC.Items.Insert(4, New ListItem("Pacific Region", "P"))
        ddlWSC.Items.Insert(5, New ListItem("Southwest Region", "SW"))
        ddlWSC.Items.Insert(6, New ListItem("Northwest Region", "NW"))
        ddlWSC.Items.Insert(7, New ListItem("Midwest Region", "MW"))
        ddlWSC.Items.Insert(8, New ListItem("Alaska Region", "AK"))
        cnx.Close()
    End Sub

    Public Sub AddSitesToMap(ByVal eval_option As Integer, ByVal wsc_cd As String, ByVal office_cd As String)
        GMap.resetMarkers()
        GMap.resetInfoWindows()
        Dim where_stmt As String = ""
        Dim results_sum As String = Nothing
        Dim from_stmt As String = ""

        results_sum = GetEvalDetails(eval_option, "results_sum")

        If eval_option = 5 Then
            from_stmt = " LEFT OUTER JOIN Ops_DCP_ID AS od ON efsd.site_no = od.site_no"
        End If

        If wsc_cd <> "0" And wsc_cd <> "USA" Then
            Select Case wsc_cd
                Case "AK", "HW", "NE", "NW", "P", "SE", "SW", "MW"
                    where_stmt = " And lw.region_cd = '" & wsc_cd & "'"
                Case Else
                    where_stmt = " And (lw.wsc_cd = '" & wsc_cd & "')"
            End Select
        End If
        If office_cd <> "0" Then
            where_stmt = " AND efsd.office_cd = '" & office_cd & "'"
        End If

        If eval_option <> 1 Then
            where_stmt = where_stmt & GetEvalDetails(eval_option, "where_stmt")
        End If

        cnx.Open()
        Dim sql As String
        If eval_option = 1 Then
            sql = "SELECT DISTINCT s.dec_lat_va, s.dec_long_va, s.site_tp_cd, s.site_no, s.station_nm" & _
                " FROM lut_WSC AS lw INNER JOIN lut_District AS ld ON ld.wsc_id = lw.wsc_id RIGHT OUTER JOIN" & _
                " nwisweb.dbo.SITEFILE AS s ON ld.district_cd = s.district_cd LEFT OUTER JOIN" & _
                " simsdb_water.dbo.Eval_FullSiteDiagnostics AS efsd ON s.nwis_host = efsd.nwis_host AND s.site_no = efsd.site_no AND" & _
                " s.agency_cd = efsd.agency_cd RIGHT OUTER JOIN" & _
                " nwisweb.dbo.rt_bol AS rb ON s.site_id = rb.site_id" & _
                " WHERE s.dec_lat_va Is Not Null And s.dec_long_va Is Not Null And (efsd.site_no Is NULL)" & where_stmt & _
                " ORDER BY dec_lat_va"
        Else
            sql = "SELECT efsd.dec_lat_va, efsd.dec_long_va, efsd.site_tp_cd, efsd.site_id," & _
                " efsd.agency_cd, efsd.site_no, efsd.nwis_host, efsd.db_no, efsd.station_nm," & _
                " efsd.station_full_nm, efsd.alt_basin_nm, efsd.wsc_id, efsd.office_cd," & _
                " efsd.office_nm, efsd.state_cd, efsd.agency_use_cd, efsd.levels_dt, efsd.levels_freq," & _
                " efsd.levels_closed, efsd.NextLevels, efsd.DaysToNext, efsd.rt_bol_byinstruments," & _
                " efsd.rt_bol_nwisweb, efsd.hasELEM, efsd.hasSHA, efsd.hasEMF, efsd.hasEC, efsd.hasTCP," & _
                " efsd.TCP_0, efsd.TCP_1, efsd.TCP_2, efsd.TCP_3, efsd.TCP_4, efsd.TCP_5, efsd.TCP_6," & _
                " efsd.inRMS, efsd.rms_last_worked, efsd.date_summarized" & _
                " FROM simsdb_water.dbo.Eval_FullSiteDiagnostics AS efsd INNER JOIN" & _
                " lut_WSC AS lw ON lw.wsc_id = efsd.wsc_id" & from_stmt & _
                " WHERE dec_lat_va Is Not Null And dec_long_va Is Not Null" & where_stmt & _
                " ORDER BY dec_lat_va"
        End If
        Dim cmd As SqlCommand = New SqlCommand(sql, cnx)
        Dim dr As SqlDataReader
        dr = cmd.ExecuteReader

        Dim rowcount As Integer = 0
        Do While dr.Read()
            rowcount = rowcount + 1
        Loop
        dr.Close()

        Dim dr2 As SqlDataReader = cmd.ExecuteReader

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

        If rowcount < 350 Then
            Dim i As Integer = 0
            Do While dr2.Read()
                lat = dr2.Item(0)
                lng = dr2.Item(1)
                site_tp_cd = dr2.Item(2)

                If eval_option = 1 Then
                    winText = winText & "<b>" & dr2.Item(3) & " " & dr2.Item(4) & "</b><br /><br />" & _
                        "<a href=""http://waterdata.usgs.gov/nwis/nwisman/?site_no=" & dr2.Item(3) & _
                        """ target=""_blank"">NWIS Web</a><hr />"
                Else
                    winText = winText & "<b>" & dr2.Item(5) & " " & dr2.Item(8) & "<br />" & _
                        dr2.Item(13) & "</b><br /><br />" & _
                        "<a href=""https://sims.water.usgs.gov/SIMSClassic/StationInfo.asp?site_no=" & dr2.Item(5) & _
                        """ target=""_blank"">SIMS Station Information Page</a><br />" & _
                        "<a href=""http://waterdata.usgs.gov/nwis/nwisman/?site_no=" & dr2.Item(5) & _
                        """ target=""_blank"">NWIS Web</a><hr />"
                End If

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

                If lat <> latPrev And lng <> lngPrev Then
                    winText = Mid(winText, 1, Len(winText) - 6)

                    latlon = New GLatLng(lat, lng)
                    marker = New GMarker(latlon, GetMarkerOpts(site_tp_cd))
                    window = New GInfoWindow(marker, winText, False, GListener.Event.click)
                    GMap.addInfoWindow(window)

                    winText = ""
                End If

                latPrev = lat
                lngPrev = lng
                i += 1
            Loop

            pnlNotice.Visible = False
            If rowcount = 0 And office_cd = "0" Then
                'If not using office filter and no records returned, remove results summary text,
                'and reset map center/zoom
                lblResultsSum.Text = ""
                hlResultsList.Text = ""
                GMap.setCenter(New GLatLng(36.0, -96.0), 4)
            ElseIf rowcount = 0 And office_cd <> "0" Then
                'If using office filter and no records returned, remove results summary text, 
                'but don't reset map center/zoom
                lblResultsSum.Text = ""
                hlResultsList.Text = ""
            Else
                'Add results summary text and zoom/center map based on markers
                If rowcount > 100 Then
                    lblWarning.Text = "<br />&raquo; map may take a minute to load"
                Else
                    lblWarning.Text = ""
                End If
                lblResultsSum.Text = results_sum
                hlResultsList.NavigateUrl = "EvalSiteList.aspx?region_cd=" & wsc_cd & "&eval_option=" & _
                    eval_option & "&office_cd=" & office_cd
                hlResultsList.Text = "(view list of sites)"

                latAvg = (latAvg / i)
                lngAvg = (lngAvg / i)
                MapZoomLevel = GetMapZoomLevel(latMin, latMax, lngMin, lngMax)
                GMap.setCenter(New GLatLng(latAvg, lngAvg), MapZoomLevel)
            End If
            lblResultsCount.Text = "Sites returned: " & rowcount.ToString
        Else
            'Too many sites returned to display, so reset map to initial display & show notice
            SetupBasicGMapDisplay()

            Select Case wsc_cd
                Case "USA", "HW", "NE", "SE", "P", "SW", "NW", "MW", "AK"
                    lblNotice.Text = "The chosen evaluation returned more than 350 sites. Please narrow " & _
                        "down the geographic area by choosing a WSC from the drop-down list."
                Case Else
                    If eval_option <> 1 Then
                        lblNotice.Text = "The chosen evaluation returned more than 350 sites.  Please use the " & _
                            "filters below to narrow down your search."
                    Else
                        lblNotice.Text = "The chosen evaluation returned more than 350 sites. Please narrow " & _
                            "down the geographic area by choosing a WSC from the drop-down list."
                    End If
            End Select
            pnlNotice.Visible = True
            lblWarning.Text = ""
            lblResultsSum.Text = ""
            lblResultsCount.Text = "Sites returned: " & rowcount.ToString
            hlResultsList.NavigateUrl = "EvalSiteList.aspx?region_cd=" & wsc_cd & "&eval_option=" & _
                eval_option & "&office_cd=" & office_cd
            hlResultsList.Text = "(view list of sites)"
        End If

        dr2.Close()
        cnx.Close()
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

    Public Function GetMarkerOpts(ByVal site_tp_cd As String) As GMarkerOptions
        Dim icon As GIcon = New GIcon()

        'Create icon images for markers based on site type
        Select Case site_tp_cd
            Case "ES", "GL", "LK", "OC", "OC-CO", "ST", "ST-CA", "ST-DCH", "ST-TS", "WE"
                icon.image = "images/SiteIcons/SW.png"
                icon.shadow = "images/SiteIcons/SWS.png"
            Case "GW", "GW-CR", "GW-EX", "GW-HZ", "GW-IW", "GW-MW", "GW-TH", "SB", "SB-CV", "SB-GWD", "SB-TSM", "SB-UZ"
                icon.image = "images/SiteIcons/GW.png"
                icon.shadow = "images/SiteIcons/GWS.png"
            Case "SP"
                icon.image = "images/SiteIcons/SP.png"
                icon.shadow = "images/SiteIcons/SWS.png"
            Case "AT"
                icon.image = "images/SiteIcons/CL.png"
                icon.shadow = "images/SiteIcons/CLS.png"
            Case "na"
                icon.image = "images/blankicon.png"
                icon.shadow = "images/blankicons.png"
            Case Else
                icon.image = "images/SiteIcons/OT.png"
                icon.shadow = "images/SiteIcons/SWS.png"
        End Select

        icon.iconSize = New GSize(16, 16)
        icon.shadowSize = New GSize(26, 16)
        icon.iconAnchor = New GPoint(8, 8)
        icon.infoWindowAnchor = New GPoint(8, 0)

        Dim mOpts As GMarkerOptions = New GMarkerOptions()
        mOpts.clickable = True
        mOpts.icon = icon

        Return mOpts
    End Function

    'NOT BEING USED---------------------------------------------------------------------------------------
    Protected Function Marker_Clicked(ByVal s As Object, ByVal e As GAjaxServerEventArgs) As String
        Dim winText As String = Nothing
        Dim latlon As String = e.point.ToString
        Dim lat As String = Mid(latlon, 1, InStr(latlon, ",") - 1)
        Dim lon As String = Mid(latlon, InStr(latlon, ",") + 1)

        cnx.Open()
        Dim sql As String = "SELECT site_id, agency_cd, site_no, nwis_host, db_no, station_nm," & _
            " station_full_nm, alt_basin_nm, wsc_id, office_cd, office_nm, dec_lat_va," & _
            " dec_long_va, state_cd, site_tp_cd, agency_use_cd, levels_dt, levels_freq, levels_closed," & _
            " NextLevels, DaysToNext, rt_bol_byinstruments, rt_bol_nwisweb, hasELEM, hasSHA, hasEMF," & _
            " hasEC, hasTCP, TCP_0, TCP_1, TCP_2, TCP_3, TCP_4, TCP_5, TCP_6, inRMS, rms_last_worked," & _
            " date_summarized" & _
            " FROM simsdb_water.dbo.Eval_FullSiteDiagnostics AS efsd" & _
            " WHERE (dec_lat_va = " & lat & " And dec_long_va = " & lon & ")"
        Dim cmd As SqlCommand = New SqlCommand(sql, cnx)
        Dim da As SqlDataAdapter
        Dim dt As DataTable = New DataTable
        da = New SqlDataAdapter(cmd)
        da.Fill(dt)

        For Each row As DataRow In dt.Rows
            winText = winText & "<b>" & row("site_no") & " " & row("station_nm") & "<br />" & _
            row("office_cd") & "</b><br /><br />" & _
            "<a href=""SIMSHome.asp?site_no=" & row("site_no") & _
            """ target=""_blank"">SIMS Station Information Page</a><br />" & _
            "<a href=""http://waterdata.usgs.gov/nwis/nwisman/?site_no=" & row("site_no") & _
            """ target=""_blank"">NWIS Web</a><hr />"
        Next

        If winText <> Nothing Then
            winText = Mid(winText, 1, Len(winText) - 6)
        Else
            winText = "<b>Not in SIMS</b>"
        End If

        cnx.Close()

        Dim marker As New GMarker(e.point, GetMarkerOpts("na"))
        Dim window As New GInfoWindow(marker, winText, True)

        Return window.ToString(e.map)
    End Function
    '-----------------------------------------------------------------------------------------------------

    'Triggered by clicking Go! button 
    Public Sub TopFiltersUsed(ByVal sender As Object, ByVal Args As CommandEventArgs)
        Dim wsc_cd As String = ddlWSC.SelectedItem.Value.ToString
        Dim office_cd As String = "0"
        Dim eval_options As Integer = ddlEvalOptions.SelectedItem.Value

        AddSitesToMap(eval_options, wsc_cd, office_cd)

        'If searching by USGS area, do not populate or show office drop-down list
        Select Case wsc_cd
            Case "USA", "HW", "NE", "SE", "P", "SW", "NW", "MW", "AK"
                pnlFilters.Visible = False
            Case Else
                If eval_options <> 1 Then
                    PopulateOfficeDDL(wsc_cd)
                Else
                    pnlFilters.Visible = False
                End If
        End Select
    End Sub

    'Triggered by changing the USGS area drop-down selection
    Public Sub WSC_Selected(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlWSC.SelectedIndexChanged
        Dim wsc_cd As String = ddlWSC.SelectedItem.Value.ToString
        Dim office_cd As String = "0"
        Dim eval_options As Integer = ddlEvalOptions.SelectedItem.Value

        AddSitesToMap(eval_options, wsc_cd, office_cd)

        'If searching by USGS area, do not populate or show office drop-down list
        Select Case wsc_cd
            Case "USA", "HW", "NE", "SE", "P", "SW", "NW", "MW", "AK"
                pnlFilters.Visible = False
            Case Else
                If eval_options <> 1 Then
                    PopulateOfficeDDL(wsc_cd)
                Else
                    pnlFilters.Visible = False
                End If
        End Select
    End Sub

    Public Sub PopulateOfficeDDL(ByVal wsc_cd As String)
        Dim sql As String = Nothing
        Dim dbcomm As SqlCommand
        Dim OfficeList As SqlDataReader

        cnx.Open()
        sql = "SELECT DISTINCT office_nm, office_cd" & _
            " FROM lut_Office AS lo INNER JOIN lut_WSC AS lw ON lw.wsc_id = lo.wsc_id" & _
            " WHERE (lw.wsc_cd = '" & wsc_cd & "')" & _
            " ORDER BY office_nm"
        dbcomm = New SqlCommand(sql, cnx)
        OfficeList = dbcomm.ExecuteReader()

        If OfficeList.HasRows Then
            ddlOffice.DataSource = OfficeList
            ddlOffice.DataBind()
            ddlOffice.Items.Insert(0, New ListItem("All", "0"))

            pnlFilters.Visible = True
        Else
            pnlFilters.Visible = False
        End If

        cnx.Close()
    End Sub

    Public Sub Office_Selected(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlOffice.SelectedIndexChanged
        Dim office_cd As String
        If ddlOffice.SelectedItem.Value = Nothing Then
            office_cd = "0"
        Else
            office_cd = ddlOffice.SelectedItem.Value.ToString
        End If
        AddSitesToMap(ddlEvalOptions.SelectedItem.Value, ddlWSC.SelectedItem.Value.ToString, office_cd)
    End Sub

    Public Function GetEvalDetails(ByVal eval_option As Integer, ByVal type As String) As String
        Dim where_stmt As String
        Dim results_sum As String

        Select Case eval_option
            Case 3
                where_stmt = " AND rt_bol_byinstruments = 'Y' AND rt_bol_nwisweb = 'N'"
                results_sum = "Sites displayed are non-realtime based on the data displayed on NWISWeb, but " & _
                    "have telemetry defined as Y in the one of the 3rd-5th positions of the Instruments field " & _
                    "(C805) of the sitefile. To fix this, set to N and run GWCHECK. See Section 1.26 of the " & _
                    "<a href=""http://nwis.usgs.gov/currentdocs/gw/gwcoding_Sect2-1.pdf"" target=""_blank"">GWSI " & _
                    "Users Manual</a>.<br /><br />"
            Case 4
                where_stmt = " AND rt_bol_byinstruments = 'N' AND rt_bol_nwisweb = 'Y'"
                results_sum = "Sites displayed are real-time based on the data displayed on NWISWeb, but " & _
                    "have telemetry defined as N for the 3rd-5th positions of the Instruments field (C805) " & _
                    "of the sitefile. To fix this, set one of those positions to Y and run GWCHECK. " & _
                    "See Section 1.26 of the <a href=""http://nwis.usgs.gov/currentdocs/gw/gwcoding_Sect2-1.pdf"" target=""_blank"">GWSI " & _
                    "Users Manual</a>.<br /><br />"
            Case 2
                where_stmt = " AND agency_use_cd Not In('A','L','M') AND rt_bol_nwisweb = 'Y'"
                results_sum = "Sites displayed are real-time based on the data displayed on NWISWeb, but " & _
                    "Agency Use code is set to inactive (I, R, D, O). To fix, set Agency Use Code to " & _
                    "active (A, L, M) and run GWCHECK.  See Section 1.33 of the <a href=""http://nwis.usgs.gov/currentdocs/gw/gwcoding_Sect2-1.pdf"" target=""_blank"">GWSI " & _
                    "Users Manual</a>.<br /><br />"
            Case 6
                where_stmt = " AND (rt_bol_nwisweb = 'Y') AND (hasSHA IS NULL) AND (hasTCP IS NULL)"
                results_sum = "Sites displayed are real-time based on the data displayed on NWISWeb Sites" & _
                    " but have no assigned Site Hazard Analysis nor assigned Traffic Control Plan." & _
                    " All sites should have safety information in SIMS. <a href=""EvalHelpGuide.html#assignSafety"" target=""_blank"">View this page for help.</a><br /><br />"
            Case 8
                where_stmt = " AND (rt_bol_nwisweb = 'Y') AND (hasSHA IS NULL) AND (hasTCP IS NOT NULL)"
                results_sum = "Sites displayed are real-time based on the data displayed on NWISWeb" & _
                    " and have an assigned Traffic Control Plan, but do not have a Site Hazard Analysis. " & _
                    "All sites should be assigned a detailed SHA. <a href=""EvalHelpGuide.html#assignSafety"" target=""_blank"">View this page for help.</a><br /><br />"
            Case 7
                where_stmt = " AND (rt_bol_nwisweb = 'Y') AND (hasSHA IS NOT NULL) AND (hasTCP IS NULL)"
                results_sum = "Sites displayed are real-time based on the data displayed on NWISWeb and have" & _
                    " an assigned Site Hazard Analysis, but do not have a Traffic Control Plan. " & _
                    " All sites should be assigned a TCP. <a href=""EvalHelpGuide.html#assignSafety"" target=""_blank"">View this page for help.</a><br /><br />"
            Case 9
                where_stmt = " AND ((rt_bol_nwisweb = 'Y') AND (hasEMF IS NULL) OR (rt_bol_nwisweb = 'Y') AND (hasEC IS NULL))"
                results_sum = "Sites displayed are real-time based on the data displayed on NWISWeb, " & _
                    "but do not have either an assigned emergency medical facility or an assigned" & _
                    " emergency contact. Sites should be assigned both an emergency medical facility" & _
                    " as well as an emergency contact. <a href=""EvalHelpGuide.html#assignEmergency"" target=""_blank"">View this page for help.</a><br /><br />"
            Case 10
                where_stmt = " AND (rt_bol_nwisweb = 'Y') AND (hasSHA IS NOT NULL) AND (hasELEM IS NOT NULL)"
                results_sum = "Sites displayed are real-time based on the data displayed on NWISWeb and" & _
                    " have safety information stored in both the Site Hazard Analysis element and an identical" & _
                    " Site Hazard Analysis. Having safety information in both places can be confusing and means" & _
                    " information must be maintained in two places. To fix, delete the element. " & _
                    "<a href=""EvalHelpGuide.html#deleteElement"" target=""_blank"">View this page for help.</a><br /><br />"
            Case 5
                where_stmt = " AND od.primary_bd = 100"
                results_sum = "Sites shown are currently assigned to 100 baud DCP IDS in PASS. Sites may be" & _
                    " cooperator sites or sites waiting to have 300 baud IDS assigned to them.<br /><br />"
            Case 1
                where_stmt = ""
                results_sum = "Sites displayed are real-time based on the data displayed on NWISWeb," & _
                    " but are not yet registered in SIMS. <a href=""EvalHelpGuide.html#registerSite"" target=""_blank"">View this page for help.</a><br /><br />"
            Case Else
                where_stmt = ""
                results_sum = ""
        End Select

        If type = "where_stmt" Then
            Return where_stmt
        Else
            Return results_sum
        End If
    End Function
End Class
