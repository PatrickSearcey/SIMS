Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb
Imports System.Web.HttpContext
Imports Telerik.Web.UI

Public Class ElemReport
    Inherits System.Web.UI.Page

    Private s As Site
    Private o As Office
    Private w As WSC
    Private se As SiteElement
    Private uid As String = Current.User.Identity.Name
    Private u As New User(uid)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Master.ResponsibleOffice = False
        Page.Title = "SIMS - Site Manuscript Approval Interface"
        Master.PageTitle = "Site Manuscript Approval Interface"

        Dim office_id As Integer = Request.QueryString("office_id")
        Dim wsc_id As Integer = Request.QueryString("wsc_id")

        'Initializing the Office and District classes
        If Not office_id = Nothing Then
            'If office is available, use it to find wsc_id
            o = New Office(office_id)
            w = New WSC(o.WSCID)
        ElseIf Not wsc_id = Nothing Then
            'If office is unavailable, but district is not, use it to reference district class
            w = New WSC(wsc_id)
        End If

        'If this is the first time coming to the page, get values from querystring
        If Not Page.IsPostBack Then
            'Start with a clear session state
            Session.Clear()

            '--PAGE ACCESS SECTION-------------------------------------------------------------
            Try
                Master.CheckAccessLevel(w.ID, "None")

                If Master.NoAccessPanel = False And (u.AccessLevel = "WSC" Or u.AccessLevel = "SuperUser") Then
                    Session("showapprove") = "true"
                Else
                    Session("showapprove") = "false"
                End If
            Catch ex As Exception
                Session("showapprove") = "false"
            End Try
            '--END PAGE ACCESS SECTION---------------------------------------------------------
        End If
    End Sub

#Region "Properties"
    ''' <summary>
    ''' Gets the DataTable used for the RadGrids on the page
    ''' </summary>
    ''' <param name="approveonly">Send "1" for MANUs needing approval only, and "0" for status of all</param>
    ''' <param name="activeonly">Send 0 for all sites, 1 for active only, and 2 for inactive only</param>
    Private ReadOnly Property MANUStatus(approveonly As Integer, activeonly As Integer, site_id As Integer) As DataTable
        Get
            Dim dt As New DataTable()

            Using cnx As New SqlConnection(Config.ConnectionInfo)
                cnx.Open()
                Dim cmd As New SqlCommand("SP_Elem_Approval_Report", cnx)
                cmd.CommandType = Data.CommandType.StoredProcedure
                cmd.Parameters.Add("@report_type", SqlDbType.NVarChar).Value = "MANU"
                cmd.Parameters.Add("@activeonly", SqlDbType.Int).Value = activeonly
                cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = w.ID
                cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = site_id
                cmd.Parameters.Add("@approveonly", SqlDbType.Bit).Value = approveonly

                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)

                cnx.Close()
            End Using
            MANUStatus = dt
        End Get
    End Property
#End Region

#Region "Approve MANU RadGrid"
    Protected Sub rgApprove_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs) Handles rgApprove.NeedDataSource
        rgApprove.DataSource = MANUStatus(1, 1, 0)
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

    'Setup in-grid links
    Protected Sub rgApprove_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs)
        If TypeOf e.Item Is GridDataItem Then
            Dim item As GridDataItem = DirectCast(e.Item, GridDataItem)

            Dim site_no As String = DirectCast(item.DataItem, DataRowView)("site_no").ToString().Trim()
            Dim hlSiteNo As HyperLink = DirectCast(item.FindControl("hlSiteNo"), HyperLink)

            hlSiteNo.Attributes("target") = "_blank"
            hlSiteNo.Attributes("href") = Config.SitePath & "StationInfo.asp?site_no=" & site_no & "&agency_cd=USGS"
        End If

        'Set custom column header tooltips
        If TypeOf e.Item Is GridHeaderItem Then
            Dim header As GridHeaderItem = DirectCast(e.Item, GridHeaderItem)

            header("sitefile_md").ToolTip = "The date when data in the NWISWeb SITEFILE was last modified."
            header("revised_dt").ToolTip = "The date when an element in the manuscript was last modified."
            header("approved_dt").ToolTip = "The date when the manuscript was last approved."
        End If
    End Sub
#End Region

#Region "All Sites Status RadGrid"
    Protected Sub rgAllSites_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs) Handles rgAllSites.NeedDataSource
        rgAllSites.DataSource = MANUStatus(0, Session("activeonly"), 0)
    End Sub

    'Specify which items appear in FilterMenu
    Protected Sub rgAllSites_PreRender(sender As Object, e As EventArgs)
        Dim menu As GridFilterMenu = rgAllSites.FilterMenu
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
    Protected Sub rgAllSites_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs)
        If TypeOf e.Item Is GridDataItem Then
            Dim item As GridDataItem = DirectCast(e.Item, GridDataItem)

            Dim site_no As String = DirectCast(item.DataItem, DataRowView)("site_no").ToString().Trim()
            Dim hlSiteNo As HyperLink = DirectCast(item.FindControl("hlSiteNo"), HyperLink)

            hlSiteNo.Attributes("target") = "_blank"
            hlSiteNo.Attributes("href") = Config.SitePath & "StationInfo.asp?site_no=" & site_no & "&agency_cd=USGS"
        End If

        'Set custom column header tooltips
        If TypeOf e.Item Is GridHeaderItem Then
            Dim header As GridHeaderItem = DirectCast(e.Item, GridHeaderItem)

            header("sitefile_md").ToolTip = "The date when data in the NWISWeb SITEFILE was last modified."
            header("revised_dt").ToolTip = "The date when an element in the manuscript was last modified."
            header("approved_dt").ToolTip = "The date when the manuscript was last approved."
            header("needs_approval").ToolTip = "A Manuscript requires approval when a manuscript element has been changed since the last approval date."
            header("SendToNWISWeb").ToolTip = "The Go! button allows a manual push of an approved manuscript to NWISWeb. A user may want this if the manuscript is not showing on NWISWeb or if autogenerated fields from NWIS have been changed and need to be updated."
        End If
    End Sub

    Protected Sub btnNWISWebSend_Command(ByVal sender As Object, ByVal e As CommandEventArgs)
        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim sql As String = "UPDATE Elem_Report_Approve" & _
                    " SET publish_complete = 'N'" & _
                    " WHERE site_id = " & e.CommandArgument.ToString() & " And report_type_cd = 'MANU'"
                Dim cmd As New SqlCommand(sql, cnx)
                cmd.ExecuteNonQuery()

                rgAllSites.Rebind()
            Catch ex As Exception
            End Try

            cnx.Close()
        End Using
    End Sub

    Public Function GetVisibleValue(ByVal publish_complete As String) As String
        Dim visible_va As String = "True"

        If publish_complete = "N" Then
            visible_va = "False"
        End If

        Return visible_va
    End Function
#End Region

#Region "Tab Strip Events"
    ''' <summary>
    ''' Refreshes the active sites and inactive sites status grids when the tab is clicked - to show just approved MANUs
    ''' </summary>
    Protected Sub rts1_TabClick(sender As Object, e As RadTabStripEventArgs) Handles rts1.TabClick
        'Reset the initial view of the grid to show active sites
        Session("activeonly") = 1
        lbActiveSiteToggle.CommandArgument = "viewInactive"
        lbActiveSiteToggle.Text = "Click to view inactive sites"
        ltlActiveSiteToggle.Text = "Viewing active sites only"

        rgAllSites.Rebind()
        rgApprove.Rebind()
    End Sub
#End Region

    Public Sub lbActiveSiteToggle_Command(sender As Object, e As CommandEventArgs) Handles lbActiveSiteToggle.Command
        If e.CommandArgument = "viewInactive" Then
            Session("activeonly") = 2
            lbActiveSiteToggle.CommandArgument = "viewActive"
            lbActiveSiteToggle.Text = "Click to view active sites"
            ltlActiveSiteToggle.Text = "Viewing inactive sites only"
        Else
            Session("activeonly") = 1
            lbActiveSiteToggle.CommandArgument = "viewInactive"
            lbActiveSiteToggle.Text = "Click to view inactive sites"
            ltlActiveSiteToggle.Text = "Viewing active sites only"
        End If

        rgAllSites.Rebind()
    End Sub
End Class