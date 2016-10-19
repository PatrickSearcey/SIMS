Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.HttpContext
Imports System.Web.HttpRequest
Imports System.Net
Imports System.Web.UI.WebControls

Public Class RecordConfig
    Inherits System.Web.UI.Page

    Private s As Site
    Private r As Record
    Private o As Office
    Private w As WSC
    Private uid As String = Current.User.Identity.Name
    Private u As New User(uid)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Master.PageTitle = "RMS Record Configuration Interface"
        Page.Title = "RMS - Record Configuration Interface"

        Dim office_id As Integer = Request.QueryString("office_id")
        Dim wsc_id As Integer = Request.QueryString("wsc_id")
        Dim site_id As Integer = Request.QueryString("site_id")
        Dim rms_record_id As Integer = Request.QueryString("rms_record_id")
        Dim clientIP As String = Request.UserHostAddress

        '--REFERENCE SITE AND RECORD OBJECTS--------------------------------------------------
        'Set site_id based on rms_record_id; if no rms_record_id or site_id passed, then show error on page
        If Not rms_record_id = Nothing Or Not site_id = Nothing Then
            r = New Record(rms_record_id)

            If Not rms_record_id = Nothing Then
                site_id = r.SiteID
            End If
            s = New Site(site_id)

            pnlError.Visible = False
        Else
            pnlHasAccess.Visible = False
            pnlError.Visible = True
            Exit Sub
        End If
        '--END SECTION------------------------------------------------------------------------

        '--REFERENCE OFFICE AND DISTRICT OBJECTS----------------------------------------------
        If office_id = Nothing And wsc_id = Nothing Then
            'If neither office_id nor wsc_id were passed in querystring, find them out using site/user info
            If site_id = Nothing Then
                'No site_id passed, so use user info
                wsc_id = u.WSCID
                office_id = u.GetUserIPOfficeID(clientIP)
            Else
                'Use site_id to find related office
                wsc_id = s.WSCID
                office_id = s.OfficeID
            End If
        ElseIf Not office_id = Nothing And wsc_id = Nothing Then
            'If office_id was passed, but not wsc_id, use office_id to find wsc_id
            o = New Office(office_id)
            wsc_id = o.WSCID
        End If
        w = New WSC(wsc_id)
        o = New Office(office_id)
        '--END SECTION---------------------------------------------------------------------

        '--PAGE ACCESS SECTION-------------------------------------------------------------
        Master.CheckAccessLevel(wsc_id, "WSC")
        If Master.NoAccessPanel Then
            pnlHasAccess.Visible = False
        Else
            pnlHasAccess.Visible = True
        End If
        '--END PAGE ACCESS SECTION---------------------------------------------------------

        If Not Page.IsPostBack Then
            'This hidden field value should only be used for the time-series record settings. It fixes the issue of
            'the Record reference resetting to the originally passed querystring value when someone has chosen
            'to configure a different record from the GridView.
            hfRMSSiteID.Value = rms_record_id
            'Setting the cancel button redirect
            btnReset.PostBackUrl = "RecordConfigList.aspx?office_id=" & o.ID & "&wsc_id=" & w.ID.ToString

            '--BEGIN SETTING UP DETAILS UNIQUE TO THIS PAGE------------------------------------
            If rms_record_id = Nothing Then
                SetupContOrNoncontPanel()
                pnlRecordConfigForm.Visible = False
                Master.SiteNumberLabel = "Create Record for " & "<a href=""" & Config.SitePath & _
                    "StationInfo.asp?site_id=" & s.ID.ToString & """>" & s.NumberName & "</a>"
            Else
                RefreshConfigForm(rms_record_id, "initialload", "na")
                Master.SiteNumberLabel = r.FullTypeDS & " Record for " & "<a href=""" & Config.SitePath & _
                    "StationInfo.asp?site_id=" & s.ID.ToString & """>" & s.NumberName & "</a>"
            End If
        End If

    End Sub

    ''' <summary>
    ''' Used upon initial page load and when a new record has been chosen to configure; refreshes all form 
    ''' fields with values pertaining to chosen record
    ''' </summary>
    Public Sub RefreshConfigForm(ByVal rms_record_id As Integer, ByVal action As String, ByVal type As String)
        Dim record As New Record(rms_record_id)

        pnlContOrNoncont.Visible = False

        lblRecordTypeHelp.Text = record.RecordTypeHelp

        If rms_record_id = 0 Then
            btnSubmit.Text = "Add Record"
            btnSubmit.CommandArgument = "new"
            btnReset.Text = "Cancel"
            pnlCatReason.Visible = False
            pnlCatReasonNew.Visible = False
        Else
            btnSubmit.Text = "Submit Changes"
            btnSubmit.CommandArgument = "update"
            btnReset.Text = "Cancel"
        End If

        If type = "noncont" Or type = "noncontDD" Then
            record.TimeSeriesFlag = False
        ElseIf type = "cont" Then
            record.TimeSeriesFlag = True
        End If

        If record.NotUsedFlag And action <> "makecont" Then
            pnlRecordConfigForm.Visible = False
            pnlRecordNotUsed.Visible = True
            cbReactivate.Checked = False
        Else
            pnlRecordConfigForm.Visible = True
            pnlRecordNotUsed.Visible = False

            If record.AltOfficeID = Nothing Then
                lblResponsibleOffice.Text = o.Name
            Else
                Dim ao As New Office(record.AltOfficeID)
                lblResponsibleOffice.Text = ao.Name
            End If

            If Not o.ReviewerEmail = "" Then
                lblReviewerEmail.Text = "(<a href=""#"" class=""infobox""><span><strong>Explain this:</strong><br />" & _
                    "<!--introstart-->This is an optional, office-level setting which can be set by a WSC " & _
                    "level administrator by going to the Administration Tasks page, clicking Modify Offices, " & _
                    "and entering the editing interface for an office.</span><b>?</b></a>) upon checking and reviewing this record, an email will also be sent to " & _
                    "the following address: <b>" & o.ReviewerEmail & "</b>"
            End If

            If record.TimeSeriesFlag Then
                type = "cont"
                lblContRecord.Text = GetlblContRecordText("cont")
                PopulateDDCBL(rms_record_id)
                PopulateCategoryDDL(rms_record_id, type)
                If record.WSCID = 0 Then
                    PopulateRecordTypeDDL(o.WSCID, "cont", rms_record_id)
                Else
                    PopulateRecordTypeDDL(record.WSCID, "cont", rms_record_id)
                End If
            Else
                If type = "na" Then
                    Dim dt As DataTable = record.GetDDs(rms_record_id)
                    If dt.Rows.Count = 0 Then
                        type = "noncont"
                    Else
                        type = "noncontDD"
                    End If
                End If

                If type = "noncont" Then
                    lblDD.Visible = False
                    cblUnusedDDs.Visible = False
                    cbMultiDDs.Visible = False
                    cblUsedDDs.Visible = False
                    cvDDList.Enabled = False
                    lblDDHelp.Visible = False
                    lblCategoryNo.Visible = False
                    lblCategoryHelp.Visible = False
                    ddlCategoryNo.Visible = False
                    rfvCategoryNo.Enabled = False
                ElseIf type = "noncontDD" Then
                    PopulateDDCBL(rms_record_id)
                    PopulateCategoryDDL(rms_record_id, type)
                End If
                lblContRecord.Text = GetlblContRecordText("noncont")

                If record.WSCID = 0 Then
                    PopulateRecordTypeDDL(o.WSCID, "noncont", rms_record_id)
                Else
                    PopulateRecordTypeDDL(record.WSCID, "noncont", rms_record_id)
                End If
            End If

            If record.NotPublishedFlag Then
                cbNotPublished.Checked = True
            Else
                cbNotPublished.Checked = False
            End If

            If rms_record_id = 0 Then
                lblRecordType.Text = "[setup below]"
                PopulateWorkerCheckerReviewerDDLs(o.WSCID)
            Else
                lblRecordType.Text = record.FullTypeDS
                PopulateWorkerCheckerReviewerDDLs(record.WSCID)
            End If

            PopulateFieldTrips(s.ID)

            If s.GetRecordDetails(s.ID, rms_record_id).Rows.Count > 0 Then
                gvOtherRecords.DataSource = s.GetRecordDetails(s.ID, rms_record_id)
                gvOtherRecords.DataBind()
            Else
                gvOtherRecords.Visible = False
                lbOtherRecords.Visible = False
                imgOtherRecords.Visible = False
                cpe.Enabled = False
            End If
        End If

        hfTSType.Value = type
    End Sub

    Public Sub SetupContOrNoncontPanel()
        If s.GetUnusedDDs(s.ID).Rows.Count = 0 Then
            pnlOnlyNoncont.Visible = True
            pnlBoth.Visible = False
        Else
            pnlOnlyNoncont.Visible = False
            pnlBoth.Visible = True
        End If

        pnlContOrNoncont.Visible = True
    End Sub

    Public Sub RecordSelected(ByVal sender As Object, ByVal Args As CommandEventArgs)
        r = New Record(Args.CommandName)

        hfRMSSiteID.Value = r.RMSSiteID

        Master.SiteNumberLabel = r.FullTypeDS & " Record for " & "<a href=""" & Config.SitePath & _
                "StationInfo.asp?site_id=" & s.ID.ToString & """>" & s.NumberName & "</a>"
        Dim site_no_label As UpdatePanel = Master.FindControl("up1")
        site_no_label.Update()

        RefreshConfigForm(r.RMSSiteID, "diffrecord", "na")

        upMain.Update()
    End Sub

    Public Sub ResetForm(ByVal sender As Object, ByVal Args As CommandEventArgs)
        'Response.Redirect(Config.ServerURL & "/SIMS/RecordConfig.aspx?rms_record_id=" & Args.CommandName.ToString)
        hfRMSSiteID.Value = Args.CommandName.ToString
        r = New Record(Args.CommandName)
        RefreshConfigForm(CInt(Args.CommandName.ToString), "initialload", "na")
        Master.SiteNumberLabel = r.FullTypeDS & " Record for " & "<a href=""" & Config.SitePath & "StationInfo.asp?site_id=" & s.ID.ToString & """>" & s.NumberName & "</a>"
        pnlUpdateConfirm.Visible = False
    End Sub

    Public Sub PopulateFieldTrips(ByVal site_id As Integer)
        Dim fieldTrips As ArrayList = s.GetFieldTrips(site_id)
        If fieldTrips.Count > 0 Then
            blFieldTrips.DataSource = fieldTrips
            blFieldTrips.DataBind()
        Else
            pnlFieldTrips.Visible = False
        End If
    End Sub

    Public Sub PopulateWorkerCheckerReviewerDDLs(ByVal wsc_id As Integer)
        Dim personnelList As ArrayList = w.GetPersonnelList(wsc_id, "no")
        Dim worker As String = r.Worker
        Dim checker As String = r.Checker
        Dim reviewer As String = r.Reviewer

        ddlOperator.DataSource = personnelList
        ddlOperator.DataBind()
        ddlOperator.Items.Insert(0, "")
        Try
            ddlOperator.Items.FindByValue(worker).Selected = True
        Catch ex As Exception
            ddlOperator.SelectedIndex = 0
        End Try

        ddlChecker.DataSource = personnelList
        ddlChecker.DataBind()
        ddlChecker.Items.Insert(0, "")
        Try
            ddlChecker.Items.FindByValue(checker).Selected = True
        Catch ex As Exception
            ddlChecker.SelectedIndex = 0
        End Try

        ddlReviewer.DataSource = personnelList
        ddlReviewer.DataBind()
        ddlReviewer.Items.Insert(0, "")
        Try
            ddlReviewer.Items.FindByValue(reviewer).Selected = True
        Catch ex As Exception
            ddlReviewer.SelectedIndex = 0
        End Try
    End Sub

    Public Sub PopulateCategoryDDL(ByVal rms_record_id As Integer, ByVal type As String)
        Dim record As New Record(rms_record_id)

        If type = "cont" Then
            Dim categoryList As ArrayList = record.GetCategoryNumbers()
            Dim category_no As Integer = record.CategoryNo

            ddlCategoryNo.DataSource = categoryList
            ddlCategoryNo.DataBind()
            If category_no = 0 Then
                ddlCategoryNo.Items.Insert(0, "")
                ddlCategoryNo.SelectedIndex = 0
            Else
                ddlCategoryNo.Items.FindByValue(category_no).Selected = True
                If category_no = 1 Then
                    pnlCatReason.Visible = False
                    pnlCatReasonNew.Visible = False
                Else
                    lblCategoryReason.Text = "Reason: " & record.CategoryReasonFull & "<br />"
                    If record.CategoryReasonFull = "unavailable" Then
                        lblUpdateReason.Text = "Change your reason for choosing this category number:"
                        rfvUpdateReason.Enabled = True
                        pnlCatReason.Visible = False
                        pnlCatReasonNew.Visible = True
                    Else
                        lblUpdateReason.Text = "Change your reason for choosing this category number (optional):"
                        rfvUpdateReason.Enabled = False
                        pnlCatReason.Visible = True
                        pnlCatReasonNew.Visible = False
                    End If
                    PopulateCategoryReasonsDDL()
                End If
            End If

            lblCategoryHelp.Text = record.GetCategoryHelp("ts")

            ddlCategoryNo.Visible = True
            rfvCategoryNo.Enabled = True
        ElseIf type = "noncontDD" Then
            lblCategoryReason.Text = "<b>3</b>; Reason: Record is non-time-series<br />"
            lblUpdateReason.Visible = False
            rfvUpdateReason.Enabled = False
            pnlCatReason.Visible = True
            ddlUpdateReason.Visible = False
            pnlCatReasonNew.Visible = False

            lblCategoryHelp.Text = record.GetCategoryHelp("nts")

            ddlCategoryNo.Visible = False
            rfvCategoryNo.Enabled = False
        End If

        lblCategoryNo.Visible = True
        lblCategoryHelp.Visible = True
    End Sub

    Public Sub PopulateCategoryReasonsDDL()
        Dim reasonList As ArrayList = r.GetCategoryReasons(ddlCategoryNo.SelectedValue)

        ddlNewReason.DataSource = reasonList
        ddlNewReason.DataBind()

        ddlNewReason.Items.Insert(0, "")
        ddlNewReason.SelectedIndex = 0

        ddlUpdateReason.DataSource = reasonList
        ddlUpdateReason.DataBind()

        ddlUpdateReason.Items.Insert(0, "")
        ddlUpdateReason.SelectedIndex = 0
    End Sub

    Public Sub PopulateRecordTypeDDL(ByVal wsc_id As Integer, ByVal type As String, ByVal rms_record_id As Integer)
        Dim record As New Record(rms_record_id)
        Dim dtRecordType As DataTable = w.GetRecordTypeList(wsc_id, type, "")
        Dim recordType As Integer = record.TypeID

        ddlRecordType.DataValueField = dtRecordType.Columns(0).ColumnName.ToString
        ddlRecordType.DataTextField = dtRecordType.Columns(2).ColumnName.ToString
        ddlRecordType.DataSource = dtRecordType
        ddlRecordType.DataBind()

        Try
            ddlRecordType.Items.FindByValue(recordType).Selected = True
        Catch ex As Exception
            ddlRecordType.SelectedIndex = 0
        End Try

    End Sub

    Public Sub PopulateDDCBL(ByVal rms_record_id As Integer)
        Dim record As New Record(rms_record_id)
        Dim dtUsedDDList As DataTable = record.GetDDs(record.RMSSiteID)

        cblUsedDDs.DataValueField = dtUsedDDList.Columns(1).ColumnName.ToString
        cblUsedDDs.DataTextField = dtUsedDDList.Columns(4).ColumnName.ToString
        cblUsedDDs.DataSource = dtUsedDDList
        cblUsedDDs.DataBind()
        For Each li As ListItem In cblUsedDDs.Items
            li.Selected = True
        Next

        'If a record is using all the possible DDs for a site, then don't show the cbMultiDDs checkbox
        If s.GetUnusedDDs(s.ID).Rows.Count > 0 Then
            If Not rms_record_id = 0 Then
                If record.MultiDD Then
                    cbMultiDDs.Text = "Add a new DD"
                Else
                    cbMultiDDs.Text = "This is a multi-parameter record, or change the DD"
                End If
                cbMultiDDs.Visible = True
                cbMultiDDs.Checked = False
                cblUnusedDDs.Visible = False
                cblUsedDDs.Visible = True
                cvDDList.ClientValidationFunction = "ValidateDDList"
            Else
                cbMultiDDs.Visible = False
                ShowUnusedDDCBL()
                cblUnusedDDs.Visible = True
                cblUsedDDs.Visible = False
                cvDDList.ClientValidationFunction = "ValidateDDListWithOnlyUnusedDDs"
                cvDDList.Enabled = True
            End If
            lblDD.Visible = True
            lblDDHelp.Visible = True
            lblDDHelp.Text = record.DDHelp
        ElseIf s.GetUnusedDDs(record.SiteID).Rows.Count = 0 And rms_record_id <> 0 Then
            cbMultiDDs.Visible = False
            cblUnusedDDs.Visible = False
            cblUsedDDs.Visible = True
            cvDDList.ClientValidationFunction = "ValidateDDList"
            lblDD.Visible = True
            lblDDHelp.Visible = True
            lblDDHelp.Text = record.DDHelp
        Else
            cbMultiDDs.Visible = False
            cblUnusedDDs.Visible = False
            cblUsedDDs.Visible = False
            cvDDList.Enabled = False
            lblDD.Visible = False
            lblDDHelp.Visible = False
        End If
    End Sub

    Protected Sub cbReactivate_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbReactivate.CheckedChanged
        pnlRecordConfigForm.Visible = True
        pnlRecordNotUsed.Visible = False

        RefreshConfigForm(hfRMSSiteID.Value, "makecont", "na")

        upMain.Update()
    End Sub

    Protected Sub cbCont_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbCont.CheckedChanged
        RefreshConfigForm(0, "initialload", "cont")
    End Sub

    Protected Sub cbNonContDD_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbNonContDD.CheckedChanged
        RefreshConfigForm(0, "initialload", "noncontDD")
    End Sub

    Protected Sub cbNoncont_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbNoncont.CheckedChanged
        RefreshConfigForm(0, "initialload", "noncont")
    End Sub

    Protected Sub cbOnlyNoncont_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbOnlyNoncont.CheckedChanged
        RefreshConfigForm(0, "initialload", "noncont")
    End Sub

    Public Function GetlblContRecordText(ByVal type As String) As String
        Dim pOut As String = "Undetermined"
        Dim dtDDForSite As DataTable = s.GetUnusedDDs(s.ID)
        Dim dtDDForRecord As DataTable = r.GetDDs(hfRMSSiteID.Value)

        If type = "noncont" Then
            pOut = "This is a <b>non-time-series</b> record."
        Else
            pOut = "This is a <b>time-series</b> record."
        End If

        Return pOut
    End Function

    Public Sub Expand_pnlOtherRecords(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbOtherRecords.Click
        If pnlOtherRecords.Visible = False Then
            pnlOtherRecords.Visible = True
        End If
    End Sub

    ''' <summary>
    ''' Runs when the make a multi-parameter record checkbox has been checked; hides this checkbox and shows the
    ''' unused DDs for the site available to add to the record
    ''' </summary>
    Protected Sub cbMultiDDs_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbMultiDDs.CheckedChanged
        ShowUnusedDDCBL()

        upMain.Update()
    End Sub

    Public Sub ShowUnusedDDCBL()
        Dim dtUnusedDDList As DataTable = s.GetUnusedDDs(s.ID)

        cblUnusedDDs.DataValueField = dtUnusedDDList.Columns(2).ColumnName.ToString
        cblUnusedDDs.DataTextField = dtUnusedDDList.Columns(5).ColumnName.ToString
        cblUnusedDDs.DataSource = dtUnusedDDList
        cblUnusedDDs.DataBind()

        lblDD.Visible = True
        lblDDHelp.Visible = True
        lblDDHelp.Text = r.DDHelp
        cblUnusedDDs.Visible = True
        cvDDList.ClientValidationFunction = "ValidateDDListWithUnusedDDs"
        cblUsedDDs.Visible = True
        cbMultiDDs.Visible = False
    End Sub

    ''' <summary>
    ''' The routine that runs when the submit changes button is clicked; updates an existing record or adds a new one
    ''' </summary>
    Protected Sub btnSubmit_Command(ByVal sender As Object, ByVal e As CommandEventArgs) Handles btnSubmit.Command
        Dim rms_record_id As Integer
        Dim record As New Record(hfRMSSiteID.Value)
        record.NotUsedFlag = cbNotUsed.Checked
        record.Worker = ddlOperator.SelectedValue
        record.Checker = ddlChecker.SelectedValue
        record.Reviewer = ddlReviewer.SelectedValue
        record.NotPublishedFlag = cbNotPublished.Checked
        record.TypeID = ddlRecordType.SelectedValue
        record.SiteID = s.ID
        If record.RecordTypeIsCont(record.TypeID) Then
            record.TimeSeriesFlag = True
            record.CategoryNo = ddlCategoryNo.SelectedValue

            If record.CategoryNo = 1 Then
                record.CategoryReason = "not needed"
            Else
                Dim updating As Boolean = False
                If Not record.CategoryReason Is Nothing And Not record.CategoryReason = "unavailable" Then
                    If ddlUpdateReason.SelectedIndex > 0 Then
                        record.CategoryReason = ddlUpdateReason.SelectedValue
                        updating = True
                    End If
                Else
                    record.CategoryReason = ddlNewReason.SelectedValue
                End If

                If tbUpdateOtherReason.Visible = True Then
                    record.CategoryReasonRemarks = tbUpdateOtherReason.Text
                ElseIf tbNewOtherReason.Visible = True Then
                    record.CategoryReasonRemarks = tbNewOtherReason.Text
                Else
                    If Not (record.CategoryReasonRemarks = tbUpdateOtherReason.Text Or record.CategoryReasonRemarks = tbNewOtherReason.Text) Then
                        If updating Then
                            record.CategoryReasonRemarks = ""
                        End If
                    End If
                End If
            End If
        Else
            record.TimeSeriesFlag = False
            record.CategoryNo = 3
            record.CategoryReason = "Record is non-time-series"
            record.CategoryReasonRemarks = "none"
        End If

        If e.CommandArgument = "update" Then
            ManageDDs(record.RMSSiteID)

            record.UpdateRecordDetails()

            Master.SiteNumberLabel = record.FullTypeDS & " Record for " & "<a href=""" & Config.SitePath & "StationInfo.asp?site_id=" & s.ID.ToString & """>" & s.NumberName & "</a>"
            Dim site_no_label As UpdatePanel = Master.FindControl("up1")
            site_no_label.Update()

            lblRecordTypeAU.Text = record.FullTypeDS
            hlStationInfo.NavigateUrl = Config.SitePath & "StationInfo.asp?site_id=" & s.ID.ToString

            If s.GetRecordDetails(s.ID, 0).Rows.Count > 0 Then
                gvOtherRecordsAU.DataSource = s.GetRecordDetails(s.ID, 0)
                gvOtherRecordsAU.DataBind()
            Else
                gvOtherRecordsAU.Visible = False
            End If

            pnlRecordConfigForm.Visible = False
            pnlUpdateConfirm.Visible = True
        Else
            rms_record_id = record.AddRecordDetails()

            ManageDDs(rms_record_id)

            Dim newRecord As New Record(rms_record_id)

            Master.SiteNumberLabel = newRecord.FullTypeDS & " Record for " & "<a href=""" & Config.SitePath & "StationInfo.asp?site_id=" & s.ID.ToString & """>" & s.NumberName & "</a>"
            Dim site_no_label As UpdatePanel = Master.FindControl("up1")
            site_no_label.Update()

            lblRecordTypeAU.Text = newRecord.FullTypeDS
            hlStationInfo.NavigateUrl = Config.SitePath & "StationInfo.asp?site_id=" & s.ID.ToString

            If s.GetRecordDetails(s.ID, 0).Rows.Count > 0 Then
                gvOtherRecordsAU.DataSource = s.GetRecordDetails(s.ID, 0)
                gvOtherRecordsAU.DataBind()
            Else
                gvOtherRecordsAU.Visible = False
            End If

            pnlRecordConfigForm.Visible = False
            pnlUpdateConfirm.Visible = True
        End If
    End Sub

    ''' <summary>
    ''' Clears out existing DDs for a record, and adds them and any new ones again
    ''' </summary>
    Public Sub ManageDDs(ByVal rms_record_id As Integer)
        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd1 As New SqlCommand("SP_RMS_Modify_Record_DDs", cnx)
            cmd1.CommandType = Data.CommandType.StoredProcedure
            cmd1.Parameters.Add("@action", SqlDbType.NVarChar, 5).Value = "clear"
            cmd1.Parameters.Add("@rms_record_id", SqlDbType.Int).Value = rms_record_id
            cmd1.Parameters.Add("@dd_nu", SqlDbType.Int).Value = 0
            cmd1.ExecuteNonQuery()

            If hfTSType.Value = "cont" Or hfTSType.Value = "noncontDD" Then
                Dim cmd3 As New SqlCommand("SP_RMS_Modify_Record_DDs", cnx)
                cmd3.CommandType = Data.CommandType.StoredProcedure

                For i As Integer = 0 To cblUsedDDs.Items.Count - 1
                    If cblUsedDDs.Items(i).Selected Then
                        cmd3.Parameters.Add("@action", SqlDbType.NVarChar, 5).Value = "add"
                        cmd3.Parameters.Add("@rms_record_id", SqlDbType.Int).Value = rms_record_id
                        cmd3.Parameters.AddWithValue("@dd_nu", cblUsedDDs.Items(i).Value)
                        cmd3.ExecuteNonQuery()
                        cmd3.Parameters.Clear()
                    End If
                Next

                For i As Integer = 0 To cblUnusedDDs.Items.Count - 1
                    If cblUnusedDDs.Items(i).Selected Then
                        cmd3.Parameters.Add("@action", SqlDbType.NVarChar, 5).Value = "add"
                        cmd3.Parameters.Add("@rms_record_id", SqlDbType.Int).Value = rms_record_id
                        cmd3.Parameters.AddWithValue("@dd_nu", cblUnusedDDs.Items(i).Value)
                        cmd3.ExecuteNonQuery()
                        cmd3.Parameters.Clear()
                    End If
                Next
            End If

            cnx.Close()
        End Using

        Dim rtemp As New Record(rms_record_id)
        rtemp.CleanUpDuplicateDDs(rms_record_id)

    End Sub

    Protected Sub ddlCategoryNo_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlCategoryNo.SelectedIndexChanged
        Dim record As New Record(hfRMSSiteID.Value)

        If ddlCategoryNo.SelectedValue = 1 Then
            pnlCatReason.Visible = False
            pnlCatReasonNew.Visible = False
            rfvNewReason.Enabled = False
            rfvUpdateReason.Enabled = False
        Else
            PopulateCategoryReasonsDDL()

            If record.CategoryNo > 1 Then
                If record.CategoryNo = ddlCategoryNo.SelectedValue Then
                    lblUpdateReason.Text = "Change your reason for choosing this category number (optional):"
                    rfvUpdateReason.Enabled = False
                Else
                    lblUpdateReason.Text = "You must update your reason for choosing this category number:"
                    rfvUpdateReason.Enabled = True
                End If
                pnlCatReason.Visible = True
                rfvNewReason.Enabled = False
                lblUpdateOtherReason.Visible = False
                tbUpdateOtherReason.Visible = False
            Else
                pnlCatReasonNew.Visible = True
                rfvNewReason.Enabled = True
                rfvUpdateReason.Enabled = False
                lblNewOtherReason.Visible = False
                tbNewOtherReason.Visible = False
            End If
        End If
    End Sub

    Protected Sub ddlNewReason_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlNewReason.SelectedIndexChanged
        If ddlNewReason.SelectedValue.ToString = "Other" Then
            lblNewOtherReason.Visible = True
            tbNewOtherReason.Visible = True
        Else
            lblNewOtherReason.Visible = False
            tbNewOtherReason.Visible = False
        End If
    End Sub

    Protected Sub ddlUpdateReason_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlUpdateReason.SelectedIndexChanged
        If ddlUpdateReason.SelectedValue.ToString = "Other" Then
            lblUpdateOtherReason.Visible = True
            tbUpdateOtherReason.Visible = True
        Else
            lblUpdateOtherReason.Visible = False
            tbUpdateOtherReason.Visible = False
        End If
    End Sub
End Class
