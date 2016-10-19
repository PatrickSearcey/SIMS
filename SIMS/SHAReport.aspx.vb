Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb
Imports System.Web.HttpContext
Imports Telerik.Web.UI

Public Class SHAReport
    Inherits System.Web.UI.Page

    Private s As Site
    Private o As Office
    Private w As WSC
    Private se As SiteElement
    Private uid As String = Current.User.Identity.Name
    Private u As New User(uid)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '--BASIC PAGE SETUP--------------------------------------------------------------------
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Master.ResponsibleOffice = False

        Page.Title = "SIMS - Site Hazard Analysis Report"

        Dim site_id As Integer = Request.QueryString("site_id") '3000336 '
        Dim site_no As String = Request.QueryString("site_no")
        Dim agency_cd As String = Request.QueryString("agency_cd")
        Dim office_id As Integer = Request.QueryString("office_id")
        Dim wsc_id As Integer = Request.QueryString("wsc_id")

        'Use site_id or site_no to initiate site class. 
        If Not site_id = Nothing Then
            s = New Site(site_id)
            site_no = s.Number
        ElseIf Not site_no = Nothing Then
            If agency_cd = Nothing Then
                agency_cd = "USGS"
            End If
            s = New Site(site_no, agency_cd)
            site_id = s.ID
        End If

        If Not office_id = Nothing Then
            'If office is available, use it to find wsc_id
            o = New Office(office_id)
            w = New WSC(o.WSCID)
        ElseIf site_id <> Nothing Then
            'If site is available, use it to find wsc_id
            office_id = s.OfficeID
            o = New Office(office_id)
            w = New WSC(o.WSCID)
        ElseIf Not wsc_id = Nothing Then
            'If office is unavailable, but district is not, use it to reference district class
            w = New WSC(wsc_id)
        End If

        'If no site, office, or WSC passed, then do not apply any filter to the grid, and show the nationwide grid - show all SHAs for all regions
        If site_id = Nothing And office_id = Nothing And wsc_id = Nothing Then
            Master.PageTitle = "Nationwide Site Hazard Analysis Status Report"
            pnlNWReport.Visible = True
            pnlWSCReport.Visible = False
        Else
            Master.PageTitle = "Site Hazard Analysis Status Report"
            pnlNWReport.Visible = False
            pnlWSCReport.Visible = True
        End If

        If Not Page.IsPostBack Then
            'Start with a clean session
            Session.Clear()

            '--PAGE ACCESS SECTION-------------------------------------------------------------
            Try
                Master.CheckAccessLevel(w.ID, "None")

                If Master.NoAccessPanel = False And u.ApproverValue Then
                    Session("showapprove") = "true"
                Else
                    Session("showapprove") = "false"
                End If
            Catch ex As Exception
                Session("showapprove") = "false"
            End Try
            '--END PAGE ACCESS SECTION---------------------------------------------------------
        End If
        '--------------------------------------------------------------------------------------
    End Sub

    Private ReadOnly Property StatusInfo(grid_tp As String, Optional activeonly As Integer = 0) As DataTable
        Get
            Dim wsc_spc_where_stmt As String = Nothing
            Dim where_stmt As String = Nothing
            Dim active_where_stmt As String = Nothing

            'If viewing the WSC specific report, then set the part of the where statement that narrows down results by wsc_id
            Try
                wsc_spc_where_stmt = " AND efsd.wsc_id = " & w.ID.ToString
            Catch ex As Exception
            End Try

            If activeonly = 2 Then
                active_where_stmt = " AND agency_use_cd Not In('M','A','L')"
            Else
                active_where_stmt = " AND agency_use_cd In('M','A','L')"
            End If

            Select Case grid_tp
                Case "currently approved"
                    where_stmt = "(shsm.updated_dt < shsm.approved_dt AND shsm.reviewed_dt < shsm.approved_dt AND shsm.approved_by <> 'transfer'" & wsc_spc_where_stmt & active_where_stmt & ") OR " & _
                        "(shsm.approved_dt IS NOT NULL AND shsm.approved_by <> 'transfer'" & wsc_spc_where_stmt & active_where_stmt & " AND (shsm.updated_dt IS NULL OR shsm.reviewed_dt IS NULL))"
                Case "need review"
                    where_stmt = "(shsm.updated_dt > shsm.reviewed_dt" & wsc_spc_where_stmt & active_where_stmt & ") OR " & _
                        "(shsm.reviewed_dt IS NULL" & wsc_spc_where_stmt & active_where_stmt & ") OR " & _
                        "(shsm.reviewed_by = 'transfer'" & wsc_spc_where_stmt & active_where_stmt & ")"
                Case "need approve"
                    where_stmt = "(shsm.updated_dt < shsm.reviewed_dt AND shsm.reviewed_dt > shsm.approved_dt" & wsc_spc_where_stmt & active_where_stmt & ") OR " & _
                        "(shsm.reviewed_dt > shsm.updated_dt AND shsm.approved_dt Is Null" & wsc_spc_where_stmt & active_where_stmt & ") OR " & _
                        "(shsm.reviewed_dt > shsm.approved_dt AND shsm.updated_dt IS Null" & wsc_spc_where_stmt & active_where_stmt & ") OR " & _
                        "(shsm.reviewed_dt Is Not Null AND shsm.updated_dt IS NULL AND shsm.approved_dt IS NULL" & wsc_spc_where_stmt & active_where_stmt & ")"
                    '" OR (shsm.approved_by = 'transfer'" & wsc_spc_where_stmt & ")" THIS WAS REMOVED SO THAT ONLY SHAs THAT HAVE BEEN REVIEWED CAN BE APPROVED
            End Select

            Dim sql As String = Nothing

            If Not grid_tp = "sites no SHA" Then
                sql = "SELECT efsd.site_no, efsd.site_no + ' ' + efsd.station_nm AS site_no_nm, efsd.office_cd, shsm.updated_by, shsm.updated_dt, shsm.reviewed_by, shsm.reviewed_dt," & _
                    " shsm.approved_by, shsm.approved_dt, shsm.reviewer_comments, lw.wsc_id, lw.wsc_cd, lw.region_cd, " & _
                    " (CASE WHEN shsm.updated_dt Is Not Null THEN (CASE WHEN shsm.updated_dt > shsm.reviewed_dt And shsm.updated_dt > shsm.approved_dt THEN 'Review' ELSE (CASE WHEN shsm.reviewed_dt > shsm.approved_dt THEN 'Approve' ELSE 'View' END) END) ELSE 'View' END) AS action," & _
                    " (CASE WHEN efsd.agency_use_cd In('M','A','L') THEN 'A' ELSE 'I' END) AS status, efsd.site_tp_cd" & _
                    " FROM simsdb_water.dbo.Eval_FullSiteDiagnostics AS efsd INNER JOIN SHA_Site_Master AS shsm ON efsd.site_id = shsm.site_id INNER JOIN" & _
                    " lut_WSC AS lw ON efsd.wsc_id = lw.wsc_id" & _
                    " WHERE " & where_stmt & _
                    " ORDER BY lw.region_cd, lw.wsc_id, efsd.office_cd, efsd.site_no"
            Else
                sql = "SELECT efsd.site_no, efsd.site_no + ' ' + efsd.station_nm AS site_no_nm, efsd.office_cd, lw.wsc_id, lw.wsc_cd, lw.region_cd, ('Create') AS action, efsd.site_tp_cd" & _
                    " FROM simsdb_water.dbo.Eval_FullSiteDiagnostics AS efsd INNER JOIN lut_WSC AS lw ON efsd.wsc_id = lw.wsc_id LEFT OUTER JOIN SHA_Site_Master AS shsm ON efsd.site_id = shsm.site_id" & _
                    " WHERE (shsm.sha_site_id IS NULL)" & active_where_stmt & wsc_spc_where_stmt & _
                    " ORDER BY lw.region_cd, lw.wsc_id, efsd.office_cd, efsd.site_no"
            End If

            Dim adapter As New SqlDataAdapter(sql, Config.ConnectionInfo)
            Dim dt As New DataTable()
            adapter.Fill(dt)
            StatusInfo = dt
        End Get
    End Property

#Region "Nationwide Currently Approved SHA RadGrid"
    Protected Sub rgNWStatus_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs) Handles rgNWStatus.NeedDataSource
        rgNWStatus.DataSource = StatusInfo("currently approved", Session("activeonlyNW1"))
    End Sub

    'Specify which items appear in FilterMenu
    Protected Sub rgNWStatus_PreRender(sender As Object, e As EventArgs)
        Dim menu As GridFilterMenu = rgNWStatus.FilterMenu
        Dim i As Integer = 0
        While i < menu.Items.Count
            If menu.Items(i).Text = "NoFilter" Or menu.Items(i).Text = "Contains" Or menu.Items(i).Text = "EqualTo" Or menu.Items(i).Text = "DoesNotContain" Then
                i = i + 1
            Else
                menu.Items.RemoveAt(i)
            End If
        End While
    End Sub

    'Setup row background colors and in-grid links
    Protected Sub rgNWStatus_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs)
        If TypeOf e.Item Is GridDataItem Then
            Dim item As GridDataItem = DirectCast(e.Item, GridDataItem)

            Dim site_no As String = DirectCast(item.DataItem, DataRowView)("site_no").ToString().Trim()
            Dim hlSite As HyperLink = DirectCast(item.FindControl("hlSite"), HyperLink)

            hlSite.Attributes("target") = "_blank"
            hlSite.Attributes("href") = Config.SitePath & "StationInfo.asp?site_no=" & site_no & "&agency_cd=USGS"

            Dim hlAction As HyperLink = DirectCast(item.FindControl("hlAction"), HyperLink)

            hlAction.Attributes("target") = "_blank"
            hlAction.Attributes("href") = "SiteHazardAnalysis.aspx?site_no=" & site_no & "&agency_cd=USGS"

            hlAction.Text = "View"
        End If
    End Sub
#End Region

#Region "Nationwide SHAs Requiring Review RadGrid"
    Protected Sub rgNWReview_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs) Handles rgNWReview.NeedDataSource
        rgNWReview.DataSource = StatusInfo("need review", Session("activeonlyNW2"))
    End Sub

    'Specify which items appear in FilterMenu
    Protected Sub rgNWReview_PreRender(sender As Object, e As EventArgs)
        Dim menu As GridFilterMenu = rgNWReview.FilterMenu
        Dim i As Integer = 0
        While i < menu.Items.Count
            If menu.Items(i).Text = "NoFilter" Or menu.Items(i).Text = "Contains" Or menu.Items(i).Text = "EqualTo" Or menu.Items(i).Text = "DoesNotContain" Then
                i = i + 1
            Else
                menu.Items.RemoveAt(i)
            End If
        End While
    End Sub

    'Setup row background colors and in-grid links
    Protected Sub rgNWReview_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs)
        If TypeOf e.Item Is GridDataItem Then
            Dim item As GridDataItem = DirectCast(e.Item, GridDataItem)

            Dim site_no As String = DirectCast(item.DataItem, DataRowView)("site_no").ToString().Trim()
            Dim hlSite As HyperLink = DirectCast(item.FindControl("hlSite"), HyperLink)

            hlSite.Attributes("target") = "_blank"
            hlSite.Attributes("href") = Config.SitePath & "StationInfo.asp?site_no=" & site_no & "&agency_cd=USGS"

            Dim hlAction As HyperLink = DirectCast(item.FindControl("hlAction"), HyperLink)

            hlAction.Attributes("target") = "_blank"
            hlAction.Attributes("href") = "SiteHazardAnalysis.aspx?site_no=" & site_no & "&agency_cd=USGS"

            hlAction.Text = "View"
        End If
    End Sub
#End Region

#Region "Nationwide SHAs Requiring Approval RadGrid"
    Protected Sub rgNWApprove_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs) Handles rgNWApprove.NeedDataSource
        rgNWApprove.DataSource = StatusInfo("need approve", Session("activeonlyNW3"))
    End Sub

    'Specify which items appear in FilterMenu
    Protected Sub rgNWApprove_PreRender(sender As Object, e As EventArgs)
        Dim menu As GridFilterMenu = rgNWApprove.FilterMenu
        Dim i As Integer = 0
        While i < menu.Items.Count
            If menu.Items(i).Text = "NoFilter" Or menu.Items(i).Text = "Contains" Or menu.Items(i).Text = "EqualTo" Or menu.Items(i).Text = "DoesNotContain" Then
                i = i + 1
            Else
                menu.Items.RemoveAt(i)
            End If
        End While
    End Sub

    'Setup row background colors and in-grid links
    Protected Sub rgNWApprove_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs)
        If TypeOf e.Item Is GridDataItem Then
            Dim item As GridDataItem = DirectCast(e.Item, GridDataItem)

            Dim site_no As String = DirectCast(item.DataItem, DataRowView)("site_no").ToString().Trim()
            Dim hlSite As HyperLink = DirectCast(item.FindControl("hlSite"), HyperLink)

            hlSite.Attributes("target") = "_blank"
            hlSite.Attributes("href") = Config.SitePath & "StationInfo.asp?site_no=" & site_no & "&agency_cd=USGS"

            Dim hlAction As HyperLink = DirectCast(item.FindControl("hlAction"), HyperLink)

            hlAction.Attributes("target") = "_blank"
            hlAction.Attributes("href") = "SiteHazardAnalysis.aspx?site_no=" & site_no & "&agency_cd=USGS"

            hlAction.Text = "View"
        End If
    End Sub
#End Region

#Region "Nationwide Sites with no SHA RadGrid"
    Protected Sub rgNWSitesNoSHA_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs) Handles rgNWSitesNoSHA.NeedDataSource
        rgNWSitesNoSHA.DataSource = StatusInfo("sites no SHA", Session("activeonlyNW4"))
    End Sub

    'Specify which items appear in FilterMenu
    Protected Sub rgNWSitesNoSHA_PreRender(sender As Object, e As EventArgs)
        Dim menu As GridFilterMenu = rgNWSitesNoSHA.FilterMenu
        Dim i As Integer = 0
        While i < menu.Items.Count
            If menu.Items(i).Text = "NoFilter" Or menu.Items(i).Text = "Contains" Or menu.Items(i).Text = "EqualTo" Or menu.Items(i).Text = "DoesNotContain" Then
                i = i + 1
            Else
                menu.Items.RemoveAt(i)
            End If
        End While
    End Sub

    'Setup row background colors and in-grid links
    Protected Sub rgNWSitesNoSHA_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs)
        If TypeOf e.Item Is GridDataItem Then
            Dim item As GridDataItem = DirectCast(e.Item, GridDataItem)

            Dim site_no As String = DirectCast(item.DataItem, DataRowView)("site_no").ToString().Trim()
            Dim hlSite As HyperLink = DirectCast(item.FindControl("hlSite"), HyperLink)

            hlSite.Attributes("target") = "_blank"
            hlSite.Attributes("href") = Config.SitePath & "StationInfo.asp?site_no=" & site_no & "&agency_cd=USGS"
        End If
    End Sub
#End Region

#Region "Status RadGrid"
    Protected Sub rgStatus_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs) Handles rgStatus.NeedDataSource
        rgStatus.DataSource = StatusInfo("currently approved", Session("activeonly1"))

        If (Not Page.IsPostBack) Then
            Try
                rgStatus.MasterTableView.FilterExpression = "([office_cd] Like '%" & o.Code & "%')"
                Dim column As GridColumn = rgStatus.MasterTableView.GetColumnSafe("office_cd")
                column.CurrentFilterFunction = GridKnownFunction.Contains
                column.CurrentFilterValue = o.Code
            Catch ex As Exception
            End Try
        End If
    End Sub

    Protected Sub rgStatus_PreRender(sender As Object, e As EventArgs)
        Dim menu As GridFilterMenu = rgStatus.FilterMenu
        Dim i As Integer = 0
        While i < menu.Items.Count
            If menu.Items(i).Text = "NoFilter" Or menu.Items(i).Text = "Contains" Or menu.Items(i).Text = "EqualTo" Or menu.Items(i).Text = "DoesNotContain" Then
                i = i + 1
            Else
                menu.Items.RemoveAt(i)
            End If
        End While
    End Sub

    Protected Sub rgStatus_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs)
        If TypeOf e.Item Is GridDataItem Then
            Dim item As GridDataItem = DirectCast(e.Item, GridDataItem)

            Dim site_no As String = DirectCast(item.DataItem, DataRowView)("site_no").ToString().Trim()
            Dim hlSite As HyperLink = DirectCast(item.FindControl("hlSite"), HyperLink)

            hlSite.Attributes("target") = "_blank"
            hlSite.Attributes("href") = Config.SitePath & "StationInfo.asp?site_no=" & site_no & "&agency_cd=USGS"

            Dim hlAction As HyperLink = DirectCast(item.FindControl("hlAction"), HyperLink)

            hlAction.Attributes("target") = "_blank"
            hlAction.Attributes("href") = "SiteHazardAnalysis.aspx?site_no=" & site_no & "&agency_cd=USGS"

            If Session("showapprove").ToString = "false" And hlAction.Text = "Approve" Then
                hlAction.Text = "View"
            End If
        End If
    End Sub
#End Region

#Region "SHAs Requiring Review RadGrid"
    Protected Sub rgReview_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs) Handles rgReview.NeedDataSource
        rgReview.DataSource = StatusInfo("need review", Session("activeonly2"))

        If (Not Page.IsPostBack) Then
            Try
                rgReview.MasterTableView.FilterExpression = "([office_cd] Like '%" & o.Code & "%')"
                Dim column As GridColumn = rgReview.MasterTableView.GetColumnSafe("office_cd")
                column.CurrentFilterFunction = GridKnownFunction.Contains
                column.CurrentFilterValue = o.Code
            Catch ex As Exception
            End Try
        End If
    End Sub

    'Specify which items appear in FilterMenu
    Protected Sub rgReview_PreRender(sender As Object, e As EventArgs)
        Dim menu As GridFilterMenu = rgReview.FilterMenu
        Dim i As Integer = 0
        While i < menu.Items.Count
            If menu.Items(i).Text = "NoFilter" Or menu.Items(i).Text = "Contains" Or menu.Items(i).Text = "EqualTo" Or menu.Items(i).Text = "DoesNotContain" Then
                i = i + 1
            Else
                menu.Items.RemoveAt(i)
            End If
        End While
    End Sub

    'Setup row background colors and in-grid links
    Protected Sub rgReview_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs)
        If TypeOf e.Item Is GridDataItem Then
            Dim item As GridDataItem = DirectCast(e.Item, GridDataItem)

            Dim site_no As String = DirectCast(item.DataItem, DataRowView)("site_no").ToString().Trim()
            Dim hlSite As HyperLink = DirectCast(item.FindControl("hlSite"), HyperLink)

            hlSite.Attributes("target") = "_blank"
            hlSite.Attributes("href") = Config.SitePath & "StationInfo.asp?site_no=" & site_no & "&agency_cd=USGS"

            Dim hlAction As HyperLink = DirectCast(item.FindControl("hlAction"), HyperLink)

            hlAction.Attributes("target") = "_blank"
            hlAction.Attributes("href") = "SiteHazardAnalysis.aspx?site_no=" & site_no & "&agency_cd=USGS"

            hlAction.Text = "Review"

        End If
    End Sub
#End Region

#Region "SHAs Requiring Approval RadGrid"
    Protected Sub rgApprove_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs) Handles rgApprove.NeedDataSource
        rgApprove.DataSource = StatusInfo("need approve", Session("activeonly3"))

        If (Not Page.IsPostBack) Then
            Try
                rgApprove.MasterTableView.FilterExpression = "([office_cd] Like '%" & o.Code & "%')"
                Dim column As GridColumn = rgApprove.MasterTableView.GetColumnSafe("office_cd")
                column.CurrentFilterFunction = GridKnownFunction.Contains
                column.CurrentFilterValue = o.Code
            Catch ex As Exception
            End Try
        End If
    End Sub

    'Specify which items appear in FilterMenu
    Protected Sub rgApprove_PreRender(sender As Object, e As EventArgs)
        Dim menu As GridFilterMenu = rgApprove.FilterMenu
        Dim i As Integer = 0
        While i < menu.Items.Count
            If menu.Items(i).Text = "NoFilter" Or menu.Items(i).Text = "Contains" Or menu.Items(i).Text = "EqualTo" Or menu.Items(i).Text = "DoesNotContain" Then
                i = i + 1
            Else
                menu.Items.RemoveAt(i)
            End If
        End While
    End Sub

    'Setup row background colors and in-grid links
    Protected Sub rgApprove_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs)
        If TypeOf e.Item Is GridDataItem Then
            Dim item As GridDataItem = DirectCast(e.Item, GridDataItem)

            Dim site_no As String = DirectCast(item.DataItem, DataRowView)("site_no").ToString().Trim()
            Dim hlSite As HyperLink = DirectCast(item.FindControl("hlSite"), HyperLink)

            hlSite.Attributes("target") = "_blank"
            hlSite.Attributes("href") = Config.SitePath & "StationInfo.asp?site_no=" & site_no & "&agency_cd=USGS"

            Dim hlAction As HyperLink = DirectCast(item.FindControl("hlAction"), HyperLink)

            hlAction.Attributes("target") = "_blank"
            hlAction.Attributes("href") = "SiteHazardAnalysis.aspx?site_no=" & site_no & "&agency_cd=USGS"

            If Session("showapprove").ToString = "false" Then
                hlAction.Text = "View"
            Else
                hlAction.Text = "Approve"
            End If
        End If
    End Sub
#End Region

#Region "Sites with no SHA RadGrid"
    Protected Sub rgSitesNoSHA_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs) Handles rgSitesNoSHA.NeedDataSource
        rgSitesNoSHA.DataSource = StatusInfo("sites no SHA", Session("activeonly4"))

        If (Not Page.IsPostBack) Then
            Try
                rgSitesNoSHA.MasterTableView.FilterExpression = "([office_cd] Like '%" & o.Code & "%')"
                Dim column As GridColumn = rgSitesNoSHA.MasterTableView.GetColumnSafe("office_cd")
                column.CurrentFilterFunction = GridKnownFunction.Contains
                column.CurrentFilterValue = o.Code
            Catch ex As Exception
            End Try
        End If
    End Sub

    'Specify which items appear in FilterMenu
    Protected Sub rgSitesNoSHA_PreRender(sender As Object, e As EventArgs)
        Dim menu As GridFilterMenu = rgSitesNoSHA.FilterMenu
        Dim i As Integer = 0
        While i < menu.Items.Count
            If menu.Items(i).Text = "NoFilter" Or menu.Items(i).Text = "Contains" Or menu.Items(i).Text = "EqualTo" Or menu.Items(i).Text = "DoesNotContain" Then
                i = i + 1
            Else
                menu.Items.RemoveAt(i)
            End If
        End While
    End Sub

    'Setup in-grid links
    Protected Sub rgSitesNoSHA_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs)
        If TypeOf e.Item Is GridDataItem Then
            Dim item As GridDataItem = DirectCast(e.Item, GridDataItem)

            Dim site_no As String = DirectCast(item.DataItem, DataRowView)("site_no").ToString().Trim()
            s = New Site(site_no, "USGS")
            Dim hlSite As HyperLink = DirectCast(item.FindControl("hlSite"), HyperLink)

            hlSite.Attributes("target") = "_blank"
            hlSite.Attributes("href") = Config.SitePath & "StationInfo.asp?site_no=" & site_no & "&agency_cd=USGS"

            Dim hlAction As HyperLink = DirectCast(item.FindControl("hlAction"), HyperLink)

            hlAction.Attributes("target") = "_blank"
            hlAction.Attributes("href") = "CreateSHA.ashx?site_id=" & s.ID & "&ref=rep"
        End If
    End Sub
#End Region

    Public Sub lbActiveSiteToggle_Command(sender As Object, e As CommandEventArgs)
        Select Case e.CommandArgument.ToString()
            Case "viewInactiveNW1"
                Session("activeonlyNW1") = 2
                lbActiveSiteToggleNW1.CommandArgument = "viewActiveNW1"
                lbActiveSiteToggleNW1.Text = "Click to view active sites"
                ltlActiveSiteToggleNW1.Text = "Viewing inactive sites only"
                rgNWStatus.Rebind()
            Case "viewActiveNW1"
                Session("activeonlyNW1") = 1
                lbActiveSiteToggleNW1.CommandArgument = "viewInactiveNW1"
                lbActiveSiteToggleNW1.Text = "Click to view inactive sites"
                ltlActiveSiteToggleNW1.Text = "Viewing active sites only"
                rgNWStatus.Rebind()
            Case "viewInactiveNW2"
                Session("activeonlyNW2") = 2
                lbActiveSiteToggleNW2.CommandArgument = "viewActiveNW2"
                lbActiveSiteToggleNW2.Text = "Click to view active sites"
                ltlActiveSiteToggleNW2.Text = "Viewing inactive sites only"
                rgNWReview.Rebind()
            Case "viewActiveNW2"
                Session("activeonlyNW2") = 1
                lbActiveSiteToggleNW2.CommandArgument = "viewInactiveNW2"
                lbActiveSiteToggleNW2.Text = "Click to view inactive sites"
                ltlActiveSiteToggleNW2.Text = "Viewing active sites only"
                rgNWReview.Rebind()
            Case "viewInactiveNW3"
                Session("activeonlyNW3") = 2
                lbActiveSiteToggleNW3.CommandArgument = "viewActiveNW3"
                lbActiveSiteToggleNW3.Text = "Click to view active sites"
                ltlActiveSiteToggleNW3.Text = "Viewing inactive sites only"
                rgNWApprove.Rebind()
            Case "viewActiveNW3"
                Session("activeonlyNW3") = 1
                lbActiveSiteToggleNW3.CommandArgument = "viewInactiveNW3"
                lbActiveSiteToggleNW3.Text = "Click to view inactive sites"
                ltlActiveSiteToggleNW3.Text = "Viewing active sites only"
                rgNWApprove.Rebind()
            Case "viewInactiveNW4"
                Session("activeonlyNW4") = 2
                lbActiveSiteToggleNW4.CommandArgument = "viewActiveNW4"
                lbActiveSiteToggleNW4.Text = "Click to view active sites"
                ltlActiveSiteToggleNW4.Text = "Viewing inactive sites only"
                rgNWSitesNoSHA.Rebind()
            Case "viewActiveNW4"
                Session("activeonlyNW4") = 1
                lbActiveSiteToggleNW4.CommandArgument = "viewInactiveNW4"
                lbActiveSiteToggleNW4.Text = "Click to view inactive sites"
                ltlActiveSiteToggleNW4.Text = "Viewing active sites only"
                rgNWSitesNoSHA.Rebind()
            Case "viewInactive1"
                Session("activeonly1") = 2
                lbActiveSiteToggle1.CommandArgument = "viewActive1"
                lbActiveSiteToggle1.Text = "Click to view active sites"
                ltlActiveSiteToggle1.Text = "Viewing inactive sites only"
                rgStatus.Rebind()
            Case "viewActive1"
                Session("activeonly1") = 1
                lbActiveSiteToggle1.CommandArgument = "viewInactive1"
                lbActiveSiteToggle1.Text = "Click to view inactive sites"
                ltlActiveSiteToggle1.Text = "Viewing active sites only"
                rgStatus.Rebind()
            Case "viewInactive2"
                Session("activeonly2") = 2
                lbActiveSiteToggle2.CommandArgument = "viewActive2"
                lbActiveSiteToggle2.Text = "Click to view active sites"
                ltlActiveSiteToggle2.Text = "Viewing inactive sites only"
                rgReview.Rebind()
            Case "viewActive2"
                Session("activeonly2") = 1
                lbActiveSiteToggle2.CommandArgument = "viewInactive2"
                lbActiveSiteToggle2.Text = "Click to view inactive sites"
                ltlActiveSiteToggle2.Text = "Viewing active sites only"
                rgReview.Rebind()
            Case "viewInactive3"
                Session("activeonly3") = 2
                lbActiveSiteToggle3.CommandArgument = "viewActive3"
                lbActiveSiteToggle3.Text = "Click to view active sites"
                ltlActiveSiteToggle3.Text = "Viewing inactive sites only"
                rgApprove.Rebind()
            Case "viewActive3"
                Session("activeonly3") = 1
                lbActiveSiteToggle3.CommandArgument = "viewInactive3"
                lbActiveSiteToggle3.Text = "Click to view inactive sites"
                ltlActiveSiteToggle3.Text = "Viewing active sites only"
                rgApprove.Rebind()
            Case "viewInactive4"
                Session("activeonly4") = 2
                lbActiveSiteToggle4.CommandArgument = "viewActive4"
                lbActiveSiteToggle4.Text = "Click to view active sites"
                ltlActiveSiteToggle4.Text = "Viewing inactive sites only"
                rgSitesNoSHA.Rebind()
            Case "viewActive4"
                Session("activeonlyN") = 1
                lbActiveSiteToggle4.CommandArgument = "viewInactive4"
                lbActiveSiteToggle4.Text = "Click to view inactive sites"
                ltlActiveSiteToggle4.Text = "Viewing active sites only"
                rgSitesNoSHA.Rebind()
        End Select
    End Sub
End Class