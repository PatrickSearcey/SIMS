Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.HttpContext

Public Class SIMSSite
    Inherits System.Web.UI.MasterPage

    Private u As User

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim office_id As Integer = Request.QueryString("office_id")
        Dim site_id As Integer = Request.QueryString("site_id")
        Dim site_no As String = Request.QueryString("site_no")
        Dim agency_cd As String = Request.QueryString("agency_cd")
        Dim wsc_id As Integer = Request.QueryString("wsc_id")
        Dim s As Site
        Dim o As Office
        Dim Referrer As String
        Dim uid As String = ""

        'Try to get the userid based on windows authentication - not necessary for some pages, so continue on error
        Try
            uid = Current.User.Identity.Name
        Catch ex As Exception
        End Try

        u = New User(uid)

        If Request.UrlReferrer Is Nothing Then
            Referrer = ""
            hlPrevPage.Visible = False
        Else
            Referrer = Request.UrlReferrer.ToString()
            If Not Page.IsPostBack Then
                ViewState("PrevPageURL") = Referrer
            End If
            hlPrevPage.NavigateUrl = ViewState("PrevPageURL")
            hlPrevPage.Visible = True
        End If

        If agency_cd = "" And site_id <> Nothing Then
            s = New Site(site_id)
            agency_cd = s.AgencyCode
        Else
            agency_cd = "USGS"
        End If

        If site_id <> Nothing Or site_no <> Nothing Then
            If site_id = Nothing And site_no <> Nothing Then
                s = New Site(site_no, agency_cd)
                site_id = s.ID
            ElseIf site_id <> Nothing And site_no = Nothing Then
                s = New Site(site_id)
                site_no = s.Number
            End If

            Dim site_no_nm As String = s.NumberName
            lblSiteNumber.Text = "<a href=""" & Config.SitePath & "StationInfo.asp?site_id=" & site_id.ToString & """>" & site_no_nm & "</a>"
        End If

        If office_id <> Nothing Or site_id <> Nothing Or wsc_id <> Nothing Then
            If wsc_id = Nothing Then
                If office_id = Nothing Then
                    office_id = s.OfficeID
                End If

                o = New Office(office_id)
                Dim office_nm As String = o.Name
                Dim office_address As String = o.StreetAddress
                Dim office_city As String = o.CityStateZip
                Dim office_number As String = o.PhoneNo

                lblOfficeName.Text = "<a href=""" & Config.SitePath & "StationsRpts.asp?office_id=" & office_id.ToString & """>" & office_nm & "</a>"
                lblOfficeAddress.Text = office_address
                lblOfficeCity.Text = office_city
                lblOfficeNumber.Text = office_number

                hlSIMSHome.NavigateUrl = Config.SitePath & "StationsRpts.asp?office_id=" & office_id.ToString
                hlRMSHome.NavigateUrl = Config.SitePath & "start.asp?office_id=" & office_id.ToString
                hlAdminTasks.NavigateUrl = Config.SitePath & "admin/admintasks.asp?office_id=" & office_id.ToString
            Else
                hlSIMSHome.NavigateUrl = Config.SitePath & "StationsRpts.asp?wsc_id=" & wsc_id.ToString
                hlRMSHome.NavigateUrl = Config.SitePath & "start.asp?wsc_id=" & wsc_id.ToString
                hlAdminTasks.NavigateUrl = Config.SitePath & "admin/admintasks.asp?wsc_id=" & wsc_id.ToString
            End If
        Else
            hlSIMSHome.NavigateUrl = Config.SitePath & "StationsRpts.asp"
            hlRMSHome.NavigateUrl = Config.SitePath & "start.asp"
            hlAdminTasks.NavigateUrl = Config.SitePath & "admin/admintasks.asp"
        End If

        lblLogonLink.Text = "<a href=""" & Config.SitePath & "integrated_logon/logon.asp?office_id=" & office_id.ToString & "&action=Logoff"">Logoff</a>"
        lblLogonUID.Text = "logged on as: " & u.ID
        hlSiteURL.NavigateUrl = Config.ServerURL & Config.SitePath
        hlSiteURL.Text = Config.ServerURL & Config.SitePath
        hlTitle.NavigateUrl = "/SIMS/"
    End Sub

    Public Property SiteNumberLabel() As String
        Get
            Return lblSiteNumber.Text
        End Get
        Set(ByVal value As String)
            lblSiteNumber.Text = value
        End Set
    End Property

    Public Property ResponsibleOffice() As Boolean
        Get
            Return pnlResponsibleOffice.Visible
        End Get
        Set(ByVal value As Boolean)
            pnlResponsibleOffice.Visible = value
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

    Public Property LogonInfo() As Boolean
        Get
            Return pnlLogon.Visible
        End Get
        Set(ByVal value As Boolean)
            pnlLogon.Visible = value
        End Set
    End Property

    Public Property PageTitle() As String
        Get
            Return lblPageTitle.Text
        End Get
        Set(ByVal value As String)
            lblPageTitle.Text = value
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

    ''' <summary>
    ''' Checks the access level of the current user, and displays a "no access" panel if insufficient access 
    ''' determined
    ''' </summary>
    ''' <param name="wsc_id">The wsc_id determined by the page, using either querystring parameter, 
    ''' site_id, or user (as a last resort)</param>
    ''' <param name="level">The level of security to check for: "WSC" level means page accessed only by users 
    ''' with "WSC" level access, "None" level means page accessed by all users belonging to the passed wsc_id,
    ''' "All" level means no restrictions placed on the page.</param>
    ''' <remarks></remarks>
    Public Sub CheckAccessLevel(ByVal wsc_id As Integer, ByVal level As String)
        Dim uid As String = ""
        Try
            uid = Current.User.Identity.Name
        Catch ex As Exception
        End Try

        u = New User(uid)

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
                Dim strpred As New Helpers(wsc_id)

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