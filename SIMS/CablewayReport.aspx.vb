Imports Telerik.Web.UI
Imports System.Web.HttpContext
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Public Class CablewayReport
    Inherits System.Web.UI.Page

    Private o As Office
    Private w As WSC
    Private s As Site
    Private cw As Cableway
    Private uid As String = Current.User.Identity.Name
    Private u As New User(uid)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '--BASIC PAGE SETUP--------------------------------------------------------------------
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Master.ResponsibleOffice = False

        Dim report_tp As String = Request.QueryString("tp") '"status"'

        If report_tp = "nw" Then
            Page.Title = "SIMS - Nationwide Cableway Information"
            Master.PageTitle = "Nationwide Cableway Information"

            pnlNW.Visible = True
            pnlStatus.Visible = False
        Else
            Dim office_id As Integer = Request.QueryString("office_id")
            Dim wsc_id As Integer = Request.QueryString("wsc_id")

            If Not office_id = Nothing Then
                'If office is available, use it to find wsc_id
                o = New Office(office_id)
                w = New WSC(o.WSCID)
            ElseIf Not wsc_id = Nothing Then
                'If office is unavailable, but WSC is not, use it to reference WSC class
                w = New WSC(wsc_id)
            End If

            Page.Title = "SIMS - Cableway Status Report"
            Master.PageTitle = "Cableway Inspection Status Report"

            pnlNW.Visible = False
            pnlStatus.Visible = True
        End If

        If Not Page.IsPostBack Then

        End If
        '--------------------------------------------------------------------------------------
    End Sub

    Private ReadOnly Property InspectionInfo() As DataTable
        Get
            Dim sql As String = "SELECT (site_no + ' ' + station_nm) AS site_no_nm, cableway_status_cd + ' - ' + cableway_status_desc AS status, * FROM simsdb.dbo.vCablewayInspectionSummary ORDER BY region_cd, wsc_cd, site_no"
            Dim adapter As New SqlDataAdapter(sql, Config.ConnectionInfo)
            Dim dt As New DataTable()
            adapter.Fill(dt)
            InspectionInfo = dt
        End Get
    End Property

    Private ReadOnly Property StatusInfo() As DataTable
        Get
            Dim sql As String = "SELECT site_no, (site_no + ' ' + station_nm) AS site_no_nm, last_inspection_dt, DATEADD(year, 1, last_inspection_dt) AS next_inspection_dt," & _
                " DATEDIFF(d, GETDATE(), DATEADD(year, 1, last_inspection_dt) ) AS days_to_next, cableway_inspection_freq, cableway_status_cd + ' - ' + cableway_status_desc AS status, wsc_id, office_cd, region_cd, wsc_cd" & _
                " FROM simsdb.dbo.vCablewayInspectionSummary" & _
                " WHERE cableway_status_cd not in ('r','ro','d')" & _
                " ORDER BY site_no"
            Dim adapter As New SqlDataAdapter(sql, Config.ConnectionInfo)
            Dim dt As New DataTable()
            adapter.Fill(dt)
            StatusInfo = dt
        End Get
    End Property

#Region "Inspections RadGrid"
    Protected Sub rgInspections_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs) Handles rgInspections.NeedDataSource
        rgInspections.DataSource = InspectionInfo()
    End Sub

    Protected Sub rgInspections_PreRender(sender As Object, e As EventArgs)
        Dim menu As GridFilterMenu = rgInspections.FilterMenu
        Dim i As Integer = 0
        While i < menu.Items.Count
            If menu.Items(i).Text = "NoFilter" Or menu.Items(i).Text = "Contains" Or menu.Items(i).Text = "EqualTo" Or menu.Items(i).Text = "DoesNotContain" Then
                i = i + 1
            Else
                menu.Items.RemoveAt(i)
            End If
        End While
    End Sub

    Protected Sub rgInspections_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs)
        If TypeOf e.Item Is GridDataItem Then
            Dim item As GridDataItem = DirectCast(e.Item, GridDataItem)

            Dim site_no As String = DirectCast(item.DataItem, DataRowView)("site_no").ToString().Trim()
            Dim wsc_id As Integer = DirectCast(item.DataItem, DataRowView)("wsc_id")
            Dim status As String = DirectCast(item.DataItem, DataRowView)("status").ToString()
            Dim hlSite As HyperLink = DirectCast(item.FindControl("hlSite"), HyperLink)
            Dim showr As String = ""

            If status = "r - Full removal and site remediated" Then
                showr = "&showr=y"
            End If

            hlSite.Attributes("target") = "_blank"
            hlSite.Attributes("href") = "Cableways.aspx?wsc_id=" & wsc_id.ToString & "&site_no=" & site_no & "&agency_cd=USGS" & showr

        End If
    End Sub
#End Region

#Region "Status RadGrid"
    Protected Sub rgStatus_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs) Handles rgStatus.NeedDataSource
        rgStatus.DataSource = StatusInfo()

        If (Not Page.IsPostBack) Then
            Try
                rgStatus.MasterTableView.FilterExpression = "([wsc_cd] = '" & w.Code & "')"
                Dim column As GridColumn = rgStatus.MasterTableView.GetColumnSafe("wsc_cd")
                column.CurrentFilterFunction = GridKnownFunction.Contains
                column.CurrentFilterValue = w.Code
            Catch ex As Exception
            End Try
        End If
    End Sub

    Protected Sub rgStatus_PreRender(sender As Object, e As EventArgs)
        Dim menu As GridFilterMenu = rgStatus.FilterMenu
        Dim i As Integer = 0
        While i < menu.Items.Count
            If menu.Items(i).Text = "NoFilter" Or menu.Items(i).Text = "Contains" Or menu.Items(i).Text = "EqualTo" Or menu.Items(i).Text = "DoesNotContain" Or menu.Items(i).Text = "LessThan" Or menu.Items(i).Text = "GreaterThan" Then
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
            Dim wsc_id As Integer = DirectCast(item.DataItem, DataRowView)("wsc_id")
            Dim hlSite As HyperLink = DirectCast(item.FindControl("hlSite"), HyperLink)

            hlSite.Attributes("target") = "_blank"
            hlSite.Attributes("href") = "Cableways.aspx?wsc_id=" & wsc_id.ToString & "&site_no=" & site_no & "&agency_cd=USGS"

            Try
                Dim days_to_next As Integer = Convert.ToInt32(item("days_to_next").Text)

                If days_to_next < 0 Then
                    item("next_inspection_dt").CssClass = "InspOverdue"
                ElseIf (0 <= days_to_next) AndAlso (days_to_next <= 30) Then
                    item("next_inspection_dt").CssClass = "InspWithin30days"
                ElseIf (31 <= days_to_next) AndAlso (days_to_next <= 183) Then
                    item("next_inspection_dt").CssClass = "InspWithin6mo"
                End If
            Catch ex As Exception
            End Try
        End If
    End Sub
#End Region
End Class
