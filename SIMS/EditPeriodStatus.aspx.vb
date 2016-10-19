Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.HttpContext
Imports System.Web.HttpRequest
Imports System.Net
Imports System.Web.UI.WebControls
Imports Telerik.Web.UI
Imports System.IO

Public Class EditPeriodStatus
    Inherits System.Web.UI.Page

    Private o As Office
    Private w As WSC
    Private s As Site
    Private r As Record
    Private p As RecordPeriod
    Private uid As String = Current.User.Identity.Name
    Private u As New User(uid)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Master.PageTitle = "Modify Record Period Status"
        Page.Title = "Modify Record Period Status"

        Dim office_id As Integer = Request.QueryString("office_id")
        Dim wsc_id As Integer = Request.QueryString("wsc_id")
        Dim period_id As Integer = Request.QueryString("period_id")
        Dim clientIP As String = Request.UserHostAddress

        '--REFERENCE OFFICE AND DISTRICT OBJECTS----------------------------------------------
        If office_id = Nothing And wsc_id = Nothing Then
            'If neither office_id nor wsc_id were passed in querystring, base them on period_id. If period_id was not passed, base on user info.
            If period_id = Nothing Then
                wsc_id = u.WSCID
                office_id = u.GetUserIPOfficeID(clientIP)
            Else
                p = New RecordPeriod(period_id)
                r = New Record(p.RMSRecordID)
                s = New Site(r.SiteID)
                wsc_id = r.WSCID
                office_id = r.AltOfficeID
                o = New Office(office_id)
                btnCancel.Visible = False
            End If
        ElseIf Not office_id = Nothing Then
            'If office_id was passed, use office_id to find wsc_id
            o = New Office(office_id)
            wsc_id = o.WSCID
        End If
        w = New WSC(wsc_id)
        '--END SECTION---------------------------------------------------------------------

        If Not Page.IsPostBack Then
            '--PAGE ACCESS SECTION-------------------------------------------------------------
            Master.CheckAccessLevel(wsc_id, "WSC")
            If Master.NoAccessPanel Then
                pnlHasAccess.Visible = False
            Else
                pnlHasAccess.Visible = True
            End If
            '--END PAGE ACCESS SECTION---------------------------------------------------------

            If period_id = Nothing Then
                pnlEnterSite.Visible = True
                pnlEditStatus.Visible = False
                btnCancel.PostBackUrl = Config.ServerURL & Config.SitePath & "admin/admintasks.asp?office_id=" & o.ID
            Else
                PopulateRecordPeriods(s.Number, s.AgencyCode, "edit")
            End If
        End If
    End Sub

    Protected Sub btnSubmitSite_Command(ByVal sender As Object, ByVal e As CommandEventArgs) Handles btnSubmitSite.Command
        If pnlEditStatus.Visible = False Then
            PopulateRecordPeriods(tbSiteNo.Text, tbAgencyCd.Text, "view")
        End If
    End Sub

    Protected Sub lbReturn_Command(ByVal sender As Object, ByVal e As CommandEventArgs) Handles lbReturn.Command
        pnlEnterSite.Visible = True
        tbSiteNo.Text = ""
        pnlEditStatus.Visible = False
    End Sub

    Public Sub PopulateRecordPeriods(ByVal site_no As String, ByVal agency_cd As String, ByVal showing As String)
        Dim pOut As String = Nothing

        s = New Site(site_no, agency_cd)
        Dim record_ids As ArrayList = s.GetRecordIDs(s.ID)

        If record_ids.Count > 0 Then
            For Each rec As Integer In record_ids
                r = New Record(rec)

                Dim dsPeriods As DataSet = r.GetPeriodsByWY(rec, Config.WY)

                If Not dsPeriods.Tables.Count = 0 Then
                    If dsPeriods.Tables(0).Rows.Count > 0 Then
                        Dim dlPeriods As DataList = ConstructDL(r.TypeDS, dsPeriods.Tables(0).Rows.Count, Config.WY)

                        dlPeriods.DataSource = dsPeriods.Tables(0)
                        dlPeriods.DataBind()

                        phPeriods.Controls.Add(dlPeriods)
                    Else
                        Dim last_wy As Integer = CInt(Config.WY) - 1
                        Dim lastWY As String = CStr(last_wy)
                        Dim dsPeriods_lastWY As DataSet = r.GetPeriodsByWY(rec, lastWY)

                        If dsPeriods_lastWY.Tables(0).Rows.Count > 0 Then
                            Dim dlPeriods As DataList = ConstructDL(r.TypeDS, dsPeriods_lastWY.Tables(0).Rows.Count, lastWY)

                            dlPeriods.DataSource = dsPeriods_lastWY.Tables(0)
                            dlPeriods.DataBind()

                            phPeriods.Controls.Add(dlPeriods)
                        Else
                            Dim last2_wy As Integer = CInt(last_wy) - 1
                            Dim last2WY As String = CStr(last2_wy)
                            Dim dsPeriods_last2WY As DataSet = r.GetPeriodsByWY(rec, last2WY)

                            If dsPeriods_last2WY.Tables(0).Rows.Count > 0 Then
                                Dim dlPeriods As DataList = ConstructDL(r.TypeDS, dsPeriods_last2WY.Tables(0).Rows.Count, last2WY)

                                dlPeriods.DataSource = dsPeriods_last2WY.Tables(0)
                                dlPeriods.DataBind()

                                phPeriods.Controls.Add(dlPeriods)
                            End If
                        End If
                    End If
                Else
                    lblNoRecs.Visible = True
                    pnlEnterSite.Visible = True
                    pnlEditStatus.Visible = False

                    Exit Sub
                End If
            Next

            lblSiteNo.Text = "Records for <a href=""" & Config.SitePath & "StationInfo.asp?site_id=" & s.ID & """ target=""_blank"">" & s.NumberName & "</a>"
            lblNoRecs.Visible = False
            pnlEnterSite.Visible = False
            pnlEditStatus.Visible = True

            If showing = "view" Then
                pnlInstructions.Visible = True
                pnlEdit.Visible = False
            ElseIf showing = "confirm" Then
                pnlInstructions.Visible = False
            Else
                pnlInstructions.Visible = False
                PopulateEditStatusArea()
            End If
        Else
            lblNoRecs.Visible = True
            pnlEnterSite.Visible = True
            pnlEditStatus.Visible = False
        End If

    End Sub

    Public Function ConstructDL(ByVal RecordType As String, ByVal Count As Integer, ByVal WY As String) As DataList
        Dim dlPeriods As DataList = New DataList()

        Dim headTemplate As DataListTemplate = New DataListTemplate(ListItemType.Header)
        headTemplate.RecordType = RecordType
        dlPeriods.HeaderTemplate = headTemplate

        dlPeriods.Width = Unit.Percentage(100)

        Dim itemTemplate As DataListTemplate = New DataListTemplate(ListItemType.Item)
        itemTemplate.NoOfPeriods = Count.ToString()
        dlPeriods.ItemTemplate = itemTemplate

        Dim footerTemplate As DataListTemplate = New DataListTemplate(ListItemType.Footer)
        footerTemplate.WY = WY
        footerTemplate.NoOfPeriods = Count.ToString()
        dlPeriods.FooterTemplate = footerTemplate

        Return dlPeriods
    End Function

    Public Sub PopulateEditStatusArea()
        ltlPeriodID.Text = p.ID.ToString
        ltlBeginDt.Text = p.BeginDate.ToShortDateString
        ltlEndDt.Text = p.EndDate.ToShortDateString
        ltlStatus.Text = p.Status

        Select Case p.Status
            Case "Worked"
                btnEditStatus.Text = "Set the status to Working"
            Case "Checked"
                btnEditStatus.Text = "Set the status to Worked"
            Case "Reviewed"
                btnEditStatus.Text = "Set the status to Checked"
        End Select

        dlDialogs.DataSource = p.GetDialogs(p.ID, p.Status)
        dlDialogs.DataBind()

        hlDialogs.NavigateUrl = "javascript:EnableButton('DialogsHandler.ashx?period_id=" & p.ID.ToString & "')"

        pnlEdit.Visible = True
    End Sub

    Protected Sub btnEditStatus_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs) Handles btnEditStatus.Command
        If hfStatusEdited.Value = "no" Then
            p.DeleteDialogs(p.ID, p.Status)
            p.EditPeriodStatus(p.ID, p.Status)

            pnlSetBackStatus.Visible = False
            pnlDialogs.Visible = False
            pnlConfirm.Visible = True

            PopulateRecordPeriods(s.Number, s.AgencyCode, "confirm")

            hfStatusEdited.Value = "yes"
        End If
    End Sub

End Class
