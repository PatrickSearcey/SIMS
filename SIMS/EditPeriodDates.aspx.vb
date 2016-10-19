Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.HttpContext
Imports System.Web.HttpRequest
Imports System.Net
Imports System.Web.UI.WebControls
Imports Telerik.Web.UI
Imports System.IO

Public Class EditPeriodDates
    Inherits System.Web.UI.Page

    Dim date_type As String
    Private o As Office
    Private w As WSC
    Private s As Site
    Private r As Record
    Private record As Record
    Private p As RecordPeriod
    Private uid As String = Current.User.Identity.Name
    Private u As New User(uid)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Master.PageTitle = "Modify Record Period Dates"
        Page.Title = "Modify Record Period Dates"

        Dim office_id As Integer = Request.QueryString("office_id")
        Dim wsc_id As Integer = Request.QueryString("wsc_id")
        Dim period_id As Integer = Request.QueryString("period_id")
        date_type = Request.QueryString("dt")
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
                pnlEditDates.Visible = False
                btnCancel.PostBackUrl = Config.ServerURL & Config.SitePath & "admin/admintasks.asp?office_id=" & o.ID
            Else
                PopulateRecordPeriods(s.Number, s.AgencyCode, "edit", "")
            End If
        End If
    End Sub

    Protected Sub btnSubmitSite_Command(ByVal sender As Object, ByVal e As CommandEventArgs) Handles btnSubmitSite.Command
        If pnlEditDates.Visible = False Then
            rpPeriods.Collapsed = False
            PopulateRecordPeriods(tbSiteNo.Text, tbAgencyCd.Text, "view", "")
        End If
    End Sub

    Protected Sub lbReturn_Command(ByVal sender As Object, ByVal e As CommandEventArgs) Handles lbReturn.Command
        pnlEnterSite.Visible = True
        tbSiteNo.Text = ""
        pnlEditDates.Visible = False
    End Sub

    Public Sub PopulateRecordPeriods(ByVal site_no As String, ByVal agency_cd As String, ByVal showing As String, ByVal new_dt As String)
        Dim pOut As String = Nothing

        s = New Site(site_no, agency_cd)
        Dim record_ids As ArrayList = s.GetRecordIDs(s.ID)

        If record_ids.Count > 0 Then
            For Each rec As Integer In record_ids
                record = New Record(rec)

                Dim dsPeriods As DataSet = record.GetPeriodsByWY(rec, "last2")

                If Not dsPeriods.Tables.Count = 0 Then
                    Dim dlPeriods As DataList = ConstructDL(record.TypeDS, dsPeriods.Tables(0).Rows.Count, Config.WY)

                    dlPeriods.DataSource = dsPeriods.Tables(0)
                    dlPeriods.DataBind()

                    phPeriods.Controls.Add(dlPeriods)
                Else
                    lblNoRecs.Visible = True
                    pnlEnterSite.Visible = True
                    pnlEditDates.Visible = False

                    Exit Sub
                End If
            Next

            lblSiteNo.Text = "Records for <a href=""" & Config.SitePath & "StationInfo.asp?site_id=" & s.ID & """ target=""_blank"">" & s.NumberName & "</a>"
            lblNoRecs.Visible = False
            pnlEnterSite.Visible = False
            pnlEditDates.Visible = True

            If showing = "view" Then
                pnlInstructions.Visible = True
                pnlEdit.Visible = False
            ElseIf showing = "confirm" Then
                pnlInstructions.Visible = False
            Else
                pnlInstructions.Visible = False
                PopulateEditDatesArea(new_dt)
            End If
        Else
            lblNoRecs.Visible = True
            pnlEnterSite.Visible = True
            pnlEditDates.Visible = False
        End If

    End Sub

    Public Function ConstructDL(ByVal RecordType As String, ByVal Count As Integer, ByVal WY As String) As DataList
        Dim dlPeriods As DataList = New DataList()

        Dim headTemplate As DataListTemplate = New DataListTemplate(ListItemType.Header)
        headTemplate.RecordType = RecordType
        dlPeriods.HeaderTemplate = headTemplate

        dlPeriods.Width = Unit.Percentage(100)

        Dim itemTemplate As DataListTemplate = New DataListTemplate(ListItemType.Item)
        itemTemplate.WhichView = "dates"
        itemTemplate.NoOfPeriods = Count.ToString()
        dlPeriods.ItemTemplate = itemTemplate

        Dim footerTemplate As DataListTemplate = New DataListTemplate(ListItemType.Footer)
        footerTemplate.WY = WY
        footerTemplate.WhichView = "dates"
        footerTemplate.NoOfPeriods = Count.ToString()
        dlPeriods.FooterTemplate = footerTemplate

        Return dlPeriods
    End Function

    Public Sub PopulateEditDatesArea(ByVal new_dt As String)
        ltlRecordType.Text = r.TypeDS

        'Details for the most recent record period
        Dim mrp As DataSet = r.Get2MostRecentPeriods(p.RMSRecordID, 1)
        'Details for the second most recent record period
        Dim smrp As DataSet = r.Get2MostRecentPeriods(p.RMSRecordID, 2)

        Dim periodid1 As String = Nothing
        Dim begdate1 As String = Nothing
        Dim enddate1 As String = Nothing
        Dim status1 As String = Nothing
        Dim periodid2 As String = Nothing
        Dim begdate2 As String = Nothing
        Dim enddate2 As String = Nothing
        Dim status2 As String = Nothing
        Dim begdate2_minusms As Date
        Dim begdate2_minustime As String = Nothing

        For Each row As DataRow In mrp.Tables(0).Rows
            periodid2 = row("period_id").ToString
            begdate2 = row("period_beg_dt").ToShortDateString
            enddate2 = row("period_end_dt").ToShortDateString
            status2 = row("status_va").ToString

            begdate2_minusms = DateAdd(DateInterval.Month, -16, row("period_beg_dt"))
            begdate2_minustime = begdate2_minusms.ToShortDateString
        Next

        For Each row As DataRow In smrp.Tables(0).Rows
            periodid1 = row("period_id").ToString
            begdate1 = row("period_beg_dt").ToShortDateString
            enddate1 = row("period_end_dt").ToShortDateString
            status1 = row("status_va").ToString
        Next

        If periodid2 = periodid1 Then
            ltlPeriodID1.Visible = False
            ltlBegDate1.Visible = False
            lblEndDate1.Visible = False
            ltlStatus1.Visible = False
        Else
            ltlPeriodID1.Text = periodid1
            ltlBegDate1.Text = begdate1
            lblEndDate1.Text = enddate1
            ltlStatus1.Text = status1
        End If

        ltlPeriodID2.Text = periodid2
        lblBegDate2.Text = begdate2
        lblEndDate2.Text = enddate2
        ltlStatus2.Text = status2

        If date_type = "end1" Or date_type = "beg2" Then
            lblEndDate1.Font.Bold = True
            lblEndDate1.ForeColor = Drawing.Color.Red
            lblBegDate2.Font.Bold = True
            lblBegDate2.ForeColor = Drawing.Color.Red
            lblEndDate2.Font.Bold = False
            lblEndDate2.ForeColor = Drawing.Color.Black

            pnlEndBeginDates.Visible = True
            pnlEndDate.Visible = False

            If periodid2 = periodid1 Then
                InitDates(begdate2_minustime, enddate2, begdate2)
            Else
                InitDates(begdate1, enddate2, begdate2)
            End If

        Else
            lblEndDate1.Font.Bold = False
            lblEndDate1.ForeColor = Drawing.Color.Black
            lblBegDate2.Font.Bold = False
            lblBegDate2.ForeColor = Drawing.Color.Black
            lblEndDate2.Font.Bold = True
            lblEndDate2.ForeColor = Drawing.Color.Red

            pnlEndBeginDates.Visible = False
            pnlEndDate.Visible = True

            InitDates(begdate2, Now(), enddate2)
        End If

        pnlEdit.Visible = True
    End Sub

    Private Sub InitDates(ByVal start_date As String, ByVal end_date As String, ByVal change_date As String)
        Dim dtStart As Date = CDate(start_date).ToShortDateString
        Dim dtEnd As Date = CDate(end_date).ToShortDateString
        Dim dtChange As Date = CDate(change_date).ToShortDateString
        Dim dtdiff As Integer = CInt(dtEnd.ToOADate) - CInt(dtStart.ToOADate)

        If date_type = "end1" Or date_type = "beg2" Then
            lblSlider1StartDate.Text = dtStart.ToShortDateString
            lblSlider1EndDate.Text = dtEnd.ToShortDateString
            rsEndBeginDates.MinimumValue = CInt(dtStart.ToOADate) + 1
            rsEndBeginDates.MaximumValue = CInt(dtEnd.ToOADate) - 1
            rsEndBeginDates.Value = CInt(dtChange.ToOADate)
            rsEndBeginDates.SmallChange = 1

            If dtdiff < 200 Then
                rsEndBeginDates.Width = 370
            ElseIf dtdiff > 199 And dtdiff < 365 Then
                rsEndBeginDates.Width = 550
            ElseIf dtdiff > 364 And dtdiff < 730 Then
                rsEndBeginDates.Width = 650
            Else
                rsEndBeginDates.Width = 900
                rpPeriods.Collapsed = True
            End If
        Else
            lblSlider2StartDate.Text = dtStart.ToShortDateString
            lblSlider2EndDate.Text = dtEnd.ToShortDateString
            rsEndDate.MinimumValue = CInt(dtStart.ToOADate) + 1
            rsEndDate.MaximumValue = CInt(dtEnd.ToOADate)
            rsEndDate.Value = CInt(dtChange.ToOADate)
            rsEndDate.SmallChange = 1

            If dtdiff < 200 Then
                rsEndDate.Width = 370
            ElseIf dtdiff > 199 And dtdiff < 365 Then
                rsEndDate.Width = 550
            ElseIf dtdiff > 364 And dtdiff < 730 Then
                rsEndDate.Width = 650
            Else
                rsEndDate.Width = 900
                rpPeriods.Collapsed = True
            End If
        End If

    End Sub

    Protected Sub btnEditDates_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs)
        If hfDatesEdited.Value = "no" Then
            Dim dtNewDate As Date

            If date_type = "end1" Or date_type = "beg2" Then
                dtNewDate = Date.FromOADate(rsEndBeginDates.Value.ToString)
                If ltlPeriodID1.Visible = False Then
                    p.EditPeriodDates(1, ltlPeriodID2.Text, dtNewDate)
                    p.AddDialogForPeriod(1, ltlPeriodID2.Text, "admin", "Admin", "Period date modified using Modify Record Period Dates interface.")
                Else
                    p.EditPeriodDates(ltlPeriodID1.Text, ltlPeriodID2.Text, dtNewDate)
                    p.AddDialogForPeriod(ltlPeriodID1.Text, ltlPeriodID2.Text, "admin", "Admin", "Period date modified using Modify Record Period Dates interface.")
                End If
            Else
                dtNewDate = Date.FromOADate(rsEndDate.Value.ToString)
                p.EditPeriodDates(0, ltlPeriodID2.Text, dtNewDate)
                p.AddDialogForPeriod(0, ltlPeriodID2.Text, "admin", "Admin", "Period date modified using Modify Record Period Dates interface.")
            End If

            pnlEditDate.Visible = False
            pnlConfirm.Visible = True

            PopulateRecordPeriods(s.Number, s.AgencyCode, "confirm", "")

            hfDatesEdited.Value = "yes"
        End If
    End Sub

End Class
