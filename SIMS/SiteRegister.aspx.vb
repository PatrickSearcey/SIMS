Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.HttpContext
Imports System.Web.HttpRequest
Imports System.Net
Imports System.Web.UI.WebControls
Imports Telerik.Web.UI
Imports System.IO

Public Class SiteRegister
    Inherits System.Web.UI.Page

    Private o As Office
    Private w As WSC
    Private s As Site
    Private uid As String = Current.User.Identity.Name
    Private u As New User(uid)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Page.Title = "Register a New Site in SIMS"
        Master.ResponsibleOffice = False

        Dim office_id As Integer = Request.QueryString("office_id")
        Dim wsc_id As Integer = Request.QueryString("wsc_id")
        Dim clientIP As String = Request.UserHostAddress

        '--REFERENCE OFFICE AND WSC OBJECTS----------------------------------------------
        If office_id = Nothing And wsc_id = Nothing Then
            'If neither office_id nor wsc_id were passed in querystring, base on user info.
            wsc_id = u.WSCID
            office_id = u.GetUserIPOfficeID(clientIP)
        ElseIf Not office_id = Nothing Then
            'If office_id was passed, use office_id to find wsc_id
            o = New Office(office_id)
            wsc_id = o.WSCID
        End If
        w = New WSC(wsc_id)
        '--END SECTION---------------------------------------------------------------------

        Master.PageTitle = "Register a New Site in SIMS for the " & w.Name & " Water Science Center"

        If Not Page.IsPostBack Then
            '--PAGE ACCESS SECTION-------------------------------------------------------------
            Master.CheckAccessLevel(wsc_id, "WSC")
            If Master.NoAccessPanel Then
                pnlHasAccess.Visible = False
            Else
                pnlHasAccess.Visible = True
            End If
            '--END PAGE ACCESS SECTION---------------------------------------------------------

            pnlEnterSite.Visible = True
            pnlConfirmSite.Visible = False

            ddlOffice.DataSource = w.GetOfficeList(w.ID)
            ddlOffice.DataBind()

            If Not office_id = Nothing Then
                ddlOffice.SelectedValue = office_id
            End If

            btnCancel.PostBackUrl = Config.ServerURL & Config.SitePath & "admin/admintasks.asp?office_id=" & o.ID
            btnCancel2.PostBackUrl = Config.ServerURL & Config.SitePath & "admin/admintasks.asp?office_id=" & o.ID
        End If
    End Sub

    Private Sub DisplayMessage(isError As Boolean, text As String)
        Dim label As Label = If((isError), Me.lblError, Me.lblSuccess)
        label.Text = text
    End Sub

    Protected Sub btnAdd_Command(ByVal sender As Object, ByVal e As CommandEventArgs)
        If e.CommandArgument = "ConfirmSite" Then
            GetSiteName()
        Else
            AddSiteToSIMS()
        End If
    End Sub

    Protected Sub GetSiteName()
        s = New Site(tbSiteNo.Text, tbAgencyCd.Text)

        If s.ID = 0 Then
            Dim nwisweb_site_info As ArrayList = s.GetNWISWEBSiteID(tbSiteNo.Text, tbAgencyCd.Text)

            If nwisweb_site_info.Count = 0 Then
                DisplayMessage(True, "This site (" & tbSiteNo.Text & ", " & tbAgencyCd.Text & ") could not be found in the NWIS Sitefile.<br />Add it to NWISWeb or enter a different site.")
            Else
                hfNWISSiteID.Value = nwisweb_site_info(0).ToString()
                ltlOffice.Text = ddlOffice.SelectedItem.Text
                ltlSiteNo.Text = tbSiteNo.Text & ", " & tbAgencyCd.Text
                ltlSiteNm.Text = nwisweb_site_info(1).ToString()
                tbSiteFullNm.Text = nwisweb_site_info(1).ToString()

                pnlEnterSite.Visible = False
                pnlConfirmSite.Visible = True
            End If
        Else
            o = New Office(s.OfficeID)
            DisplayMessage(True, "This site is already in SIMS (registered under the " & o.Name & " office).")
        End If
    End Sub

    Protected Sub AddSiteToSIMS()
        s = New Site(tbSiteNo.Text, tbAgencyCd.Text)
        Dim insert_site As String = s.AddSiteToSIMS(hfNWISSiteID.Value.ToString(), tbSiteFullNm.Text, ddlOffice.SelectedValue.ToString())

        If insert_site = "error" Then
            DisplayMessage(True, "The site could not be added.  Please contact GS-W Help SIMS@usgs.gov for assistance.")
        Else
            tbSiteNo.Text = ""
            tbSiteFullNm.Text = ""
            pnlEnterSite.Visible = True
            pnlConfirmSite.Visible = False
            DisplayMessage(False, "The site was registered!  Visit the <a href='/SIMSClassic/StationInfo.asp?site_id=" & insert_site & "'>Station Information</a> page now, " & _
                           "or return to the <a href='/SIMSClassic/admin/admintasks.asp?office_id=" & o.ID & "'>Admin Tasks page</a>.")
        End If
    End Sub
End Class