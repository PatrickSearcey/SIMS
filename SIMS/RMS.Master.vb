Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.HttpContext

Public Class RMS
    Inherits System.Web.UI.MasterPage

    Dim uid As String = Current.User.Identity.Name
    Dim u As New User(uid)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim office_id As Integer = Request.QueryString("office_id")
        Dim wsc_id As Integer = Request.QueryString("wsc_id")
        Dim rms_record_id As Integer = Request.QueryString("rms_record_id")
        Dim site_id As Integer = Request.QueryString("site_id")
        Dim period_id As Integer = Request.QueryString("period_id")
        Dim Referrer As String

        ToolkitScriptManager1.AsyncPostBackTimeout = 36000

        If Request.UrlReferrer Is Nothing Or Request.UrlReferrer = Request.Url Then
            Referrer = ""
            hlPrevPage.NavigateUrl = Nothing
            hlPrevPage.Visible = False
        Else
            Referrer = Request.UrlReferrer.ToString()
            'If InStr(Referrer, "?") > 0 Then
            '    Referrer = Mid(Referrer, 1, InStr(Referrer, "?") - 1)
            'End If
            If Not Page.IsPostBack Then
                ViewState("PrevPageURL") = Referrer
            End If
            hlPrevPage.NavigateUrl = ViewState("PrevPageURL")
        End If

        If Not site_id = Nothing Then
            Dim s As New Site(site_id)

            If Not rms_record_id = Nothing And SiteNumberLabel Is Nothing Then
                Dim r As New Record(rms_record_id)
                SiteNumberLabel = r.FullTypeDS & " Record for " & "<a href=""" & Config.SitePath & _
                    "StationInfo.asp?site_id=" & s.ID.ToString & """>" & s.NumberName & "</a>"
            ElseIf SiteNumberLabel Is Nothing Then
                SiteNumberLabel = "<a href=""" & Config.SitePath & "StationInfo.asp?site_id=" & _
                    s.ID.ToString & """>" & s.NumberName & "</a>"
            End If
        Else
            If Not rms_record_id = Nothing And SiteNumberLabel Is Nothing Then
                Dim r As New Record(rms_record_id)
                site_id = r.SiteID
                Dim s As New Site(site_id)
                SiteNumberLabel = r.FullTypeDS & " Record for " & "<a href=""" & Config.SitePath & _
                    "StationInfo.asp?site_id=" & s.ID.ToString & """>" & s.NumberName & "</a>"
            Else
                'lblSiteNumber.Text = ""
            End If
        End If

        If office_id = Nothing Then
            If site_id = Nothing And Not period_id = Nothing Then
                Dim p As New RecordPeriod(period_id)
                Dim r As New Record(p.RMSRecordID)
                Dim s As New Site(r.SiteID)
                office_id = s.OfficeID
            Else
                Dim s As New Site(site_id)
                office_id = s.OfficeID
            End If
        End If

        If office_id <> Nothing Then
            hlSIMSHome.NavigateUrl = Config.SitePath & "StationsRpts.asp?office_id=" & office_id.ToString
            hlRMSHome.NavigateUrl = Config.SitePath & "start.asp?office_id=" & office_id.ToString
            hlAdminTasks.NavigateUrl = Config.SitePath & "admin/admintasks.asp?office_id=" & office_id.ToString
        ElseIf wsc_id <> Nothing Then
            hlSIMSHome.NavigateUrl = Config.SitePath & "StationsRpts.asp?wsc_id=" & wsc_id.ToString
            hlRMSHome.NavigateUrl = Config.SitePath & "start.asp?wsc_id=" & wsc_id.ToString
            hlAdminTasks.NavigateUrl = Config.SitePath & "admin/admintasks.asp?wsc_id=" & wsc_id.ToString
        Else
            hlSIMSHome.NavigateUrl = Config.SitePath & "StationsRpts.asp"
            hlRMSHome.NavigateUrl = Config.SitePath & "start.asp"
            hlAdminTasks.NavigateUrl = Config.SitePath & "admin/admintasks.asp"
        End If

        lblLogonLink.Text = "<a href=""" & Config.SitePath & "integrated_logon/logon.asp?office_id=" & office_id.ToString & "&action=Logoff"">Logoff</a>"
        lblLogonUID.Text = "logged on as: " & u.ID
        hlSiteURL.NavigateUrl = Config.ServerURL & Config.SitePath
        hlSiteURL.Text = Config.ServerURL & Config.SitePath
        hlTitle.NavigateUrl = Config.ServerURL & Config.SitePath
    End Sub

    ''' <summary>
    ''' Gets or sets the title of the page displayed under the body content section
    ''' </summary>
    Public Property PageTitle() As String
        Get
            Return lblPageTitle.Text
        End Get
        Set(ByVal value As String)
            lblPageTitle.Text = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the site number label text at the top of the page
    ''' </summary>
    Public Property SiteNumberLabel() As String
        Get
            Return lblSiteNumber.Text
        End Get
        Set(ByVal value As String)
            lblSiteNumber.Text = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the URL for the previous page link in the page top navigation
    ''' </summary>
    Public Property PrevPageLink() As String
        Get
            Return hlPrevPage.NavigateUrl
        End Get
        Set(ByVal value As String)
            hlPrevPage.NavigateUrl = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets if the previous page link is visible on the page
    ''' </summary>
    Public Property ShowPrevPageLink() As Boolean
        Get
            Return hlPrevPage.Visible
        End Get
        Set(ByVal value As Boolean)
            hlPrevPage.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the visibilty of the "no access" panel
    ''' </summary>
    Public Property NoAccessPanel() As Boolean
        Get
            Return pnlNoAccess.Visible
        End Get
        Set(ByVal value As Boolean)
            pnlNoAccess.Visible = value
        End Set
    End Property

    Public Property HorizontalLine() As Boolean
        Get
            Return pnlHR.Visible
        End Get
        Set(ByVal value As Boolean)
            pnlHR.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Checks the access level of the current user, and displays a "no access" panel if insufficient access 
    ''' determined
    ''' </summary>
    ''' <param name="wsc_id">The district code determined by the page, using either querystring parameter, 
    ''' site_id, or user (as a last resort)</param>
    ''' <param name="level">The level of security to check for: "WSC" level means page accessed only by users 
    ''' with "WSC" level access, "None" level means page accessed by all users belonging to the passed wsc_id,
    ''' "All" level means no restrictions placed on the page.</param>
    ''' <remarks></remarks>
    Public Sub CheckAccessLevel(ByVal wsc_id As Integer, ByVal level As String)
        Dim user_access_level As String = u.AccessLevel
        Dim user_exception As Array = u.UserException
        Dim user_wsc_exception As Array = u.WSCException
        Dim user_office_id As Integer = u.OfficeID
        Dim user_wsc_id As Integer = u.WSCID

        lblNoAccess.Text = "User access level not sufficient enough to work with this interface. Please " & _
            "contact your local SIMS/RMS administrator for help.<br /><br /><hr />" & _
            "Error diagnostics:" & _
            "<ul><li>user access level: " & user_access_level & "</li>" & _
            "<li>user office ID: " & user_office_id & "</li>" & _
            "<li>user WSC ID: " & user_wsc_id.ToString & "</li>" & _
            "<li>page WSC ID: " & wsc_id.ToString & "</li></ul>"

        Try
            If user_access_level <> "SuperUser" Then
                Dim strpred As New Helpers(wsc_id.ToString)

                Select Case level
                    Case "WSC"
                        If user_access_level = "None" Then
                            pnlNoAccess.Visible = True
                        ElseIf user_access_level = "WSC" And user_wsc_id <> wsc_id Then
                            Try
                                If user_exception.Length = 0 Then
                                    pnlNoAccess.Visible = True
                                Else
                                    'If the wsc_id doesn't exist in the exception for user array, do not give access
                                    If Array.Exists(Of String)(user_exception, AddressOf strpred.CompareTo) = False Then
                                        pnlNoAccess.Visible = True
                                    Else
                                        pnlNoAccess.Visible = False
                                    End If
                                End If
                            Catch ex As Exception
                            End Try
                            Try
                                If user_wsc_exception.Length = 0 Then
                                    pnlNoAccess.Visible = True
                                Else
                                    'If the wsc_id doesn't exist in the exception for WSC array, do not give access
                                    If Array.Exists(Of String)(user_wsc_exception, AddressOf strpred.CompareTo) = False Then
                                        pnlNoAccess.Visible = True
                                    Else
                                        pnlNoAccess.Visible = False
                                    End If
                                End If
                            Catch ex As Exception
                            End Try
                        Else
                            pnlNoAccess.Visible = False
                        End If
                    Case "None"
                        If user_wsc_id <> wsc_id Then
                            Try
                                If user_exception.Length = 0 Then
                                    pnlNoAccess.Visible = True
                                Else
                                    'If the wsc_id doesn't exist in the exception for user array, do not give access
                                    If Array.Exists(Of String)(user_exception, AddressOf strpred.CompareTo) = False Then
                                        pnlNoAccess.Visible = True
                                    Else
                                        pnlNoAccess.Visible = False
                                    End If
                                End If
                            Catch ex As Exception
                            End Try
                            Try
                                If user_wsc_exception.Length = 0 Then
                                    pnlNoAccess.Visible = True
                                Else
                                    'If the wsc_id doesn't exist in the exception for WSC array, do not give access
                                    If Array.Exists(Of String)(user_wsc_exception, AddressOf strpred.CompareTo) = False Then
                                        pnlNoAccess.Visible = True
                                    Else
                                        pnlNoAccess.Visible = False
                                    End If
                                End If
                            Catch ex As Exception
                            End Try
                        Else
                            pnlNoAccess.Visible = False
                        End If
                    Case Else
                        pnlNoAccess.Visible = False
                End Select
            Else
                pnlNoAccess.Visible = False
            End If
        Catch ex As Exception
            lblNoAccess.Text = "There has been an error in the program.  Please send the following information to GS-W_Help_SIMS@usgs.gov.<br /><br /><hr />" & _
            "Error diagnostics:" & _
            "<ul><li>error message: " & ex.Message & "</li>" & _
            "<li>user access level: " & user_access_level & "</li>" & _
            "<li>user office ID: " & user_office_id & "</li>" & _
            "<li>user WSC ID: " & user_wsc_id.ToString & "</li>" & _
            "<li>page WSC ID: " & wsc_id.ToString & "</li></ul>"
        End Try

    End Sub

End Class

