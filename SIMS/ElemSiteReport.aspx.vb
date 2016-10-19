Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb
Imports System.Web.HttpContext
Imports Telerik.Web.UI

Public Class ElemSiteReport
    Inherits System.Web.UI.Page

    Private s As Site
    Private w As WSC
    Private se As SiteElement
    Private uid As String = Current.User.Identity.Name
    Private u As New User(uid)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Master.ResponsibleOffice = False
        Page.Title = "SIMS - Site Manuscript Approval"
        Master.PageTitle = "Site Manuscript Approval"
        Master.ShowSiteLink = False

        Dim site_id As Integer = Nothing
        Dim site_no As String = Nothing
        Dim agency_cd As String = Nothing

        'If this is the first time coming to the page, get values from querystring
        If Not Page.IsPostBack Then
            site_id = Request.QueryString("site_id") '3000339 '
            site_no = Request.QueryString("site_no")
            agency_cd = Request.QueryString("agency_cd")

            Session.Clear()

            'Once session variables have been setup, initialize Site class
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

            'Initializing the District classes
            If site_id <> Nothing Then
                'If site is available, use it to find WSC ID
                w = New WSC(s.WSCID)
            End If

            'Ppopulate the site selection drop down and ApproveMANU control, only if first visit to page
            ucApproveMANU.ShowCloseButton = False
            ucApproveMANU.SiteID = site_id

            Dim dt As DataTable = s.GetSiteNameFromSearch("forMAI", w.ID)

            rcbSites.DataTextField = "site_no_nm"
            rcbSites.DataValueField = "site_id"
            rcbSites.DataSource = dt
            rcbSites.DataBind()

            rcbSites.SelectedValue = site_id

            'Populate the link boxes at the top of the page. Only populate the hlOriginalSite link if first visit to page
            hlCurrentSite.NavigateUrl = Config.SitePath & "StationInfo.asp?site_no=" & site_no & "&agency_cd=USGS"
            hlCurrentSite.Text = s.NumberName

            hlOriginalSite.NavigateUrl = Config.SitePath & "StationInfo.asp?site_no=" & site_no & "&agency_cd=USGS"
            hlOriginalSite.Text = s.NumberName

            hlFullReport.NavigateUrl = "ElemReport.aspx?wsc_id=" & s.WSCID

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
                cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = 0
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

    Protected Sub ucApproveMANU_SubmitEvent(sender As Object, e As CommandEventArgs)
        rgSiteDetails.DataSource = Nothing
        rgSiteDetails.Rebind()
    End Sub

#Region "Site Selection"
    Protected Sub btnSubmitSite_Command(source As Object, e As CommandEventArgs)
        Dim agency_cd As String = "USGS"
        Dim site_id As Integer = Nothing
        Dim site_no As String = Nothing

        If e.CommandArgument.ToString() = "choosesite" Then
            If Not String.IsNullOrEmpty(rcbSites.SelectedValue.ToString()) Then
                site_id = CInt(rcbSites.SelectedValue)

                s = New Site(site_id)

                RefreshPageData(site_id)

                tbSiteNo.Text = ""
            End If
        Else
            If Not String.IsNullOrEmpty(tbSiteNo.Text) Then
                agency_cd = tbAgencyCd.Text
                site_no = tbSiteNo.Text

                s = New Site(site_no, agency_cd)

                RefreshPageData(s.ID)

                rcbSites.SelectedValue = s.ID.ToString()
            End If
        End If
    End Sub

    Public Sub RefreshPageData(site_id As Integer)
        'Refresh the site details grid to show details for newly selected site
        rgSiteDetails.Rebind()

        'Update the Current Site link box at the top of the page
        Dim stemp As New Site(site_id)
        hlCurrentSite.NavigateUrl = Config.SitePath & "StationInfo.asp?site_no=" & stemp.Number & "&agency_cd=USGS"
        hlCurrentSite.Text = stemp.NumberName

        'Set the session variable so that the user control will use it for the site_id
        Session("site_id") = site_id

        'Refresh the ApproveMANU user control to show the newly selected site's MANU
        ucApproveMANU.SiteID = site_id
        ucApproveMANU.ShowCloseButton = False
        ucApproveMANU.RefreshMANU()
    End Sub
#End Region

#Region "Site Details RadGrid"
    Protected Sub rgSiteDetails_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs) Handles rgSiteDetails.NeedDataSource
        Dim site_id As Integer
        Try
            site_id = s.ID
            If site_id = Nothing Then
                site_id = CInt(rcbSites.SelectedValue)
            End If
        Catch ex As Exception
            site_id = CInt(rcbSites.SelectedValue)
        End Try

        rgSiteDetails.DataSource = MANUStatus(0, 0, site_id)
    End Sub

    'Setup in-grid links
    Protected Sub rgSiteDetails_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs)
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
        End If
    End Sub
#End Region

End Class