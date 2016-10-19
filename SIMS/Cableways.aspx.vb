Imports Telerik.Web.UI
Imports System.Web.HttpContext
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Public Class Cableways
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
        Page.Title = "SIMS - Cableway Inspections"

        Dim office_id As Integer = Request.QueryString("office_id")
        Dim wsc_id As Integer = Request.QueryString("wsc_id")
        Dim site_no As String = Request.QueryString("site_no")
        Dim agency_cd As String = Request.QueryString("agency_cd")
        Dim showr As String = Request.QueryString("showr")

        If Not office_id = Nothing Then
            'If office is available, use it to find wsc_id
            o = New Office(office_id)
            w = New WSC(o.WSCID)
        ElseIf Not wsc_id = Nothing Then
            'If office is unavailable, but WSC is not, use it to reference WSC class
            w = New WSC(wsc_id)
        Else
            'If neither wsc_id or office_id passed, use user's wsc_id to reference district class
            w = New WSC(u.WSCID)
        End If

        'If a site_no has been included in the querystring, setup reference to Site object
        If Not site_no = Nothing Then
            If Not agency_cd = Nothing Then
                s = New Site(site_no, agency_cd)
            Else
                s = New Site(site_no, "USGS")
            End If
        End If

        Master.PageTitle = "Cableways for " & w.Name

        If Not Page.IsPostBack Then
            '--PAGE ACCESS SECTION-------------------------------------------------------------
            'Get the referring page; if it's from CablewayReport.aspx, allow to see but not to edit no matter the district code
            Dim ref_page As String = "none"
            Try
                ref_page = HttpContext.Current.Request.UrlReferrer.ToString()
            Catch ex As Exception
            End Try

            Dim x As Integer = ref_page.IndexOf("CablewayReport.aspx")

            Master.CheckAccessLevel(w.ID, "None")
            If Master.NoAccessPanel And x < 0 Then
                Session("AllowEdits") = "false"
                pnlHasAccess.Visible = False
            ElseIf Master.NoAccessPanel And x > 0 Then
                Session("AllowEdits") = "false"
                pnlHasAccess.Visible = True
                Master.NoAccessPanel = False
            Else
                Session("AllowEdits") = "true"
                pnlHasAccess.Visible = True
            End If
            '--END PAGE ACCESS SECTION---------------------------------------------------------

            Session("ShowRStatusCableways") = "false"

            If Session("ShowRStatusCableways") = "false" And showr = "" Then
                lbToggleRStatus.Text = "Click to view removed or remediated cableways"
                lbToggleRStatus.CommandArgument = "viewR"
            ElseIf Session("ShowRStatusCableways") = "true" Or showr = "y" Then
                Session("ShowRStatusCableways") = "true"
                lbToggleRStatus.Text = "Click to view all cableways"
                lbToggleRStatus.CommandArgument = "hideR"
            End If
        End If
        '--------------------------------------------------------------------------------------

    End Sub

#Region "Properties"
    Private ReadOnly Property SiteList() As DataTable
        Get
            Dim s As New Site("", "")
            SiteList = s.GetSiteNameFromSearch("swonly", w.ID)
        End Get
    End Property

    Private ReadOnly Property CablewayStatusList() As DataTable
        Get
            Dim sql As String = "SELECT (cableway_status_cd + ' - ' + cableway_status_desc) AS status_cd_desc, cableway_status_cd," & _
                " cableway_status_desc FROM lut_cableway_status_cd"
            Dim adapter As New SqlDataAdapter(sql, Config.ConnectionInfo)
            Dim dt As New DataTable()
            adapter.Fill(dt)
            CablewayStatusList = dt
        End Get
    End Property

    Private ReadOnly Property CablewayTypeList() As DataTable
        Get
            Dim sql As String = "SELECT (cableway_type_cd + ' - ' + cableway_type_desc) AS type_cd_desc, cableway_type_cd," & _
                " cableway_type_desc FROM lut_cableway_type_cd"
            Dim adapter As New SqlDataAdapter(sql, Config.ConnectionInfo)
            Dim dt As New DataTable()
            adapter.Fill(dt)
            CablewayTypeList = dt
        End Get
    End Property

    Private ReadOnly Property VisitTypeList() As DataTable
        Get
            Dim sql As String = "SELECT visit_type_cd, visit_type_desc, visit_type_cd + ' - ' + visit_type_desc AS type_cd_desc" & _
                " FROM lut_cableway_visittype_cd"
            Dim adapter As New SqlDataAdapter(sql, Config.ConnectionInfo)
            Dim dt As New DataTable()
            adapter.Fill(dt)
            VisitTypeList = dt
        End Get
    End Property

    Private ReadOnly Property VisitActionList() As DataTable
        Get
            Dim sql As String = "SELECT visit_action_cd, visit_action_desc, visit_action_cd + ' - ' + visit_action_desc AS" & _
                " action_cd_desc FROM lut_cableway_visitaction_cd"
            Dim adapter As New SqlDataAdapter(sql, Config.ConnectionInfo)
            Dim dt As New DataTable()
            adapter.Fill(dt)
            VisitActionList = dt
        End Get
    End Property
#End Region

    Public Sub lbToggleRStatus_Command(ByVal source As Object, ByVal e As CommandEventArgs) Handles lbToggleRStatus.Command
        If e.CommandArgument = "viewR" Then
            Session("ShowRStatusCableways") = "true"
            lbToggleRStatus.Text = "Click to view all cableways"
            lbToggleRStatus.CommandArgument = "hideR"
        ElseIf e.CommandArgument = "hideR" Then
            Session("ShowRStatusCableways") = "false"
            lbToggleRStatus.Text = "Click to view removed or remediated cableways"
            lbToggleRStatus.CommandArgument = "viewR"
        End If

        rgCableways.Rebind()
    End Sub

    Private Sub rgCableways_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs) Handles rgCableways.NeedDataSource
        If Not e.IsFromDetailTable Then
            If Session("ShowRStatusCableways") = "false" Then
                'We do not want any cableways to be shown with a status of "r - Full removal and site remediated"
                rgCableways.DataSource = w.GetCableways(w.ID, "norstatus")
            Else
                'Show only cableways with a status of "r - Full removal and site remediated"
                rgCableways.DataSource = w.GetCableways(w.ID, "rstatus")
            End If

            If (Not Page.IsPostBack) Then
                Try
                    rgCableways.MasterTableView.FilterExpression = "([site_no_nm] Like '%" & s.Number & "%')"
                    Dim column As GridColumn = rgCableways.MasterTableView.GetColumnSafe("site_no_nm")
                    column.CurrentFilterFunction = GridKnownFunction.Contains
                    column.CurrentFilterValue = s.Number
                Catch ex As Exception
                End Try
            End If
        End If
    End Sub

    Private Sub rgCableways_DetailTableDataBind(ByVal source As Object, ByVal e As GridDetailTableDataBindEventArgs) Handles rgCableways.DetailTableDataBind
        Dim dataItem As GridDataItem = CType(e.DetailTableView.ParentItem, GridDataItem)
        Select Case e.DetailTableView.Name
            Case "Visits"
                Dim cableway_id As Integer = Convert.ToInt32(dataItem.GetDataKeyValue("cableway_id"))
                cw = New Cableway(cableway_id)
                e.DetailTableView.DataSource = cw.GetVisits(cableway_id)
        End Select
    End Sub

    Protected Sub rgCableways_PreRender(sender As Object, e As EventArgs)
        Dim menu As GridFilterMenu = rgCableways.FilterMenu
        Dim i As Integer = 0
        While i < menu.Items.Count
            If menu.Items(i).Text = "NoFilter" Or menu.Items(i).Text = "Contains" Or menu.Items(i).Text = "EqualTo" Or menu.Items(i).Text = "DoesNotContain" Then
                i = i + 1
            Else
                menu.Items.RemoveAt(i)
            End If
        End While
    End Sub

    Protected Sub rgCableways_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs)
        If "Visits".Equals(e.Item.OwnerTableView.Name) Then
            If e.Item.IsInEditMode Then
                Dim item As GridEditableItem = DirectCast(e.Item, GridEditableItem)

                Dim ltlDetailsTitle As Literal = DirectCast(item.FindControl("ltlDetailsEditFormTitle"), Literal)
                Dim rdpVisit As RadDatePicker = DirectCast(item.FindControl("rdpVisit"), RadDatePicker)
                Dim ddlVisitType As DropDownList = DirectCast(item.FindControl("ddlVisitType"), DropDownList)
                Dim ddlVisitAction As DropDownList = DirectCast(item.FindControl("ddlVisitAction"), DropDownList)
                Dim tbRemarks As TextBox = DirectCast(item.FindControl("tbRemarks"), TextBox)
                Dim btnUpdate1 As Button = DirectCast(item.FindControl("btnUpdate1"), Button)
                Dim btnInsert1 As Button = DirectCast(item.FindControl("btnInsert1"), Button)
                Dim upload As RadUpload = DirectCast(item.FindControl("fuFile"), RadUpload)
                Dim lblUploadDoc As Label = DirectCast(item.FindControl("lblUploadDoc"), Label)
                Dim imgUploadDocHelp As Image = DirectCast(item.FindControl("imgUploadDocHelp"), Image)

                ddlVisitType.DataSource = VisitTypeList
                ddlVisitAction.DataSource = VisitActionList

                If Not (TypeOf e.Item Is IGridInsertItem) Then
                    ltlDetailsTitle.Text = "Edit Visit"
                    rdpVisit.SelectedDate = DirectCast(e.Item.DataItem, DataRowView)("visit_dt").ToString()
                    ddlVisitType.SelectedValue = DirectCast(e.Item.DataItem, DataRowView)("visit_type_cd").ToString()
                    ddlVisitAction.SelectedValue = DirectCast(e.Item.DataItem, DataRowView)("visit_action_cd").ToString()
                    tbRemarks.Text = DirectCast(e.Item.DataItem, DataRowView)("remarks").ToString()

                    If Not String.IsNullOrEmpty(DirectCast(e.Item.DataItem, DataRowView)("visit_file_nm").ToString()) Then
                        upload.Visible = False
                        lblUploadDoc.Visible = False
                        imgUploadDocHelp.Visible = False
                    Else
                        upload.Visible = True
                        lblUploadDoc.Visible = True
                        imgUploadDocHelp.Visible = True
                    End If

                    btnUpdate1.Visible = True
                    btnInsert1.Visible = False
                ElseIf (TypeOf e.Item Is IGridInsertItem) Then
                    ltlDetailsTitle.Text = "Add New Visit"

                    btnUpdate1.Visible = False
                    btnInsert1.Visible = True
                End If

                ddlVisitType.DataBind()
                ddlVisitAction.DataBind()
            Else
                If TypeOf e.Item Is GridDataItem Then
                    Dim item As GridDataItem = DirectCast(e.Item, GridDataItem)

                    Dim visit_file_nm As String = DirectCast(item.DataItem, DataRowView)("visit_file_nm").ToString()
                    Dim parentItem As GridDataItem = DirectCast(e.Item.OwnerTableView.ParentItem, GridDataItem)
                    Dim cableway_id As String = parentItem.OwnerTableView.DataKeyValues(parentItem.ItemIndex)("cableway_id").ToString
                    Dim hlDoc As HyperLink = DirectCast(item.FindControl("hlDoc"), HyperLink)
                    hlDoc.Attributes("href") = "Files/Cableways/" & cableway_id & "/" & visit_file_nm
                End If
            End If

            If TypeOf e.Item Is GridDataItem Then
                Dim item As GridDataItem = DirectCast(e.Item, GridDataItem)
                If Session("AllowEdits").ToString = "false" Then
                    Dim ib1 As ImageButton = DirectCast(item("EditCommandColumn2").Controls(0), ImageButton)
                    ib1.Visible = False
                    Dim ib2 As ImageButton = DirectCast(item("VisitDeleteColumn").Controls(0), ImageButton)
                    ib2.Visible = False
                End If
            End If
        Else
            If e.Item.IsInEditMode Then
                Dim item As GridEditableItem = DirectCast(e.Item, GridEditableItem)

                Dim ltlTitle As Literal = DirectCast(item.FindControl("ltlEditFormTitle"), Literal)
                Dim ddlSites As DropDownList = DirectCast(item.FindControl("ddlSites"), DropDownList)
                Dim tbNickname As TextBox = DirectCast(item.FindControl("tbNickname"), TextBox)
                Dim ddlCablewayStatus As DropDownList = DirectCast(item.FindControl("ddlCablewayStatus"), DropDownList)
                Dim ddlCablewayType As DropDownList = DirectCast(item.FindControl("ddlCablewayType"), DropDownList)
                Dim rntbFreq As RadNumericTextBox = DirectCast(item.FindControl("rntbFreq"), RadNumericTextBox)
                Dim rblAerialMarkerReq As RadioButtonList = DirectCast(item.FindControl("rblAerialMarkerReq"), RadioButtonList)
                Dim rblAerialMarkerInst As RadioButtonList = DirectCast(item.FindControl("rblAerialMarkerInst"), RadioButtonList)
                Dim btnUpdate2 As Button = DirectCast(item.FindControl("btnUpdate2"), Button)
                Dim btnInsert2 As Button = DirectCast(item.FindControl("btnInsert2"), Button)

                ddlSites.DataSource = SiteList
                ddlCablewayStatus.DataSource = CablewayStatusList
                ddlCablewayType.DataSource = CablewayTypeList

                If Not (TypeOf e.Item Is IGridInsertItem) Then
                    ltlTitle.Text = "Edit Cableway"
                    ddlSites.SelectedValue = DirectCast(e.Item.DataItem, DataRowView)("site_id").ToString()
                    tbNickname.Text = DirectCast(e.Item.DataItem, DataRowView)("cableway_nm").ToString()
                    ddlCablewayStatus.SelectedValue = DirectCast(e.Item.DataItem, DataRowView)("cableway_status_cd").ToString()
                    ddlCablewayType.SelectedValue = DirectCast(e.Item.DataItem, DataRowView)("cableway_type_cd").ToString()
                    rntbFreq.Text = DirectCast(e.Item.DataItem, DataRowView)("cableway_inspection_freq").ToString()
                    rblAerialMarkerReq.SelectedValue = DirectCast(e.Item.DataItem, DataRowView)("aerial_marker_req").ToString()
                    rblAerialMarkerInst.SelectedValue = DirectCast(e.Item.DataItem, DataRowView)("aerial_marker_inst").ToString()

                    btnUpdate2.Visible = True
                    btnInsert2.Visible = False
                ElseIf (TypeOf e.Item Is IGridInsertItem) Then
                    ltlTitle.Text = "Add New Cableway"

                    rblAerialMarkerReq.SelectedValue = "false"
                    rblAerialMarkerInst.SelectedValue = "false"

                    btnUpdate2.Visible = False
                    btnInsert2.Visible = True
                End If

                ddlSites.DataBind()
                ddlCablewayStatus.DataBind()
                ddlCablewayType.DataBind()
            End If

            If TypeOf e.Item Is GridDataItem Then
                Dim item As GridDataItem = DirectCast(e.Item, GridDataItem)
                Try
                    If Session("AllowEdits").ToString = "false" Then
                        Dim ib3 As ImageButton = DirectCast(item("EditCommandColumn1").Controls(0), ImageButton)
                        ib3.Visible = False
                    End If
                Catch ex As Exception
                End Try
            End If
        End If

    End Sub

    Protected Sub rgCableways_DeleteCommand(sender As Object, e As GridCommandEventArgs)
        Dim item As GridDataItem = TryCast(e.Item, GridDataItem)

        If e.Item.OwnerTableView.Name = "Cableways" Then
            'The ability to delete cableways was removed
            '!---This section of code not in use!!---!
            Try
                Dim cableway_id As Integer = Convert.ToInt32(item.GetDataKeyValue("cableway_id"))
                Dim deleteCW As New Cableway(cableway_id)

                Dim deleteStatus As String = deleteCW.DeleteCableway(u.ID)
                If deleteStatus = "success" Then
                    DisplayMessage(False, "The cableway was deleted!")
                Else
                    DisplayMessage(True, "Unable to delete cableway. Reason: " + deleteStatus)
                End If

            Catch ex As Exception
                DisplayMessage(True, "Unable to delete cableway. Reason: " + ex.Message)

                e.Canceled = True
            End Try
            '!---to here---!
        Else
            Try
                Dim cableway_visit_id As Integer = Convert.ToInt32(item.GetDataKeyValue("cableway_visit_id"))
                Dim deleteVisit As New CablewayVisit(cableway_visit_id)

                Dim deleteStatus As String = deleteVisit.DeleteVisit(u.ID)
                If deleteStatus = "success" Then
                    DumpMyFile(deleteVisit.CablewayID, deleteVisit.VisitFileName)
                    DisplayMessage(False, "The cableway visit was deleted!")
                Else
                    DisplayMessage(True, "Unable to delete cableway visit. Reason: " + deleteStatus)
                End If

            Catch ex As Exception
                DisplayMessage(True, "Unable to delete cableway visit. Reason: " + ex.Message)

                e.Canceled = True
            End Try
        End If
    End Sub

    Protected Sub DumpMyFile(cableway_id As String, file_nm As String)
        Try
            Dim myFile As New FileInfo(Server.MapPath("~/Files/Cableways/") & "\" & cableway_id & "\" & file_nm)
            If myFile.Exists Then
                File.Delete(Server.MapPath("~/Files/Cableways/") & "\" & cableway_id & "\" & file_nm)
            End If

        Catch ex As Exception
        End Try
    End Sub

    Protected Sub rgCableways_UpdateCommand(sender As Object, e As GridCommandEventArgs)
        Dim item As GridEditableItem = TryCast(e.Item, GridEditableItem)

        If e.Item.OwnerTableView.Name = "Cableways" Then
            Dim cableway_id As Integer = Convert.ToInt32(item.GetDataKeyValue("cableway_id"))
            Dim updateCW As New Cableway(cableway_id)

            Dim newValues As New Hashtable()

            newValues("site_id") = Convert.ToInt32(TryCast(item.FindControl("ddlSites"), DropDownList).SelectedValue)
            newValues("cableway_nm") = TryCast(item.FindControl("tbNickname"), TextBox).Text
            newValues("cableway_status_cd") = TryCast(item.FindControl("ddlCablewayStatus"), DropDownList).SelectedValue.ToString()
            newValues("cableway_type_cd") = TryCast(item.FindControl("ddlCablewayType"), DropDownList).SelectedValue.ToString()
            newValues("cableway_inspection_freq") = Convert.ToDouble(TryCast(item.FindControl("rntbFreq"), RadNumericTextBox).Text)
            newValues("aerial_marker_req") = TryCast(item.FindControl("rblAerialMarkerReq"), RadioButtonList).SelectedValue.ToString()
            newValues("aerial_marker_inst") = TryCast(item.FindControl("rblAerialMarkerInst"), RadioButtonList).SelectedValue.ToString()

            Try
                For Each entry As DictionaryEntry In newValues
                    Select Case DirectCast(entry.Key, String)
                        Case "site_id"
                            updateCW.SiteID = DirectCast(entry.Value, Integer)
                            Exit Select
                        Case "cableway_nm"
                            updateCW.Nickname = DirectCast(entry.Value, String)
                            Exit Select
                        Case "cableway_status_cd"
                            updateCW.CablewayStatusCode = DirectCast(entry.Value, String)
                            Exit Select
                        Case "cableway_type_cd"
                            updateCW.CablewayTypeCode = DirectCast(entry.Value, String)
                            Exit Select
                        Case "cableway_inspection_freq"
                            updateCW.InspectionFrequency = DirectCast(entry.Value, Double)
                            Exit Select
                        Case "aerial_marker_req"
                            updateCW.AerialMarkerReq = DirectCast(entry.Value, String)
                            Exit Select
                        Case "aerial_marker_inst"
                            updateCW.AerialMarkerInst = DirectCast(entry.Value, String)
                            Exit Select
                    End Select
                Next

                Dim updateStatus As String = updateCW.UpdateCablewayInfo(u.ID)
                If updateStatus = "success" Then
                    DisplayMessage(False, "The cableway was updated!")
                Else
                    DisplayMessage(True, "Unable to update cableway. Reason: " + updateStatus)
                End If

            Catch ex As Exception
                DisplayMessage(True, "Unable to update cableway. Reason: " + ex.Message)

                e.Canceled = True
            End Try
        Else
            Dim cableway_visit_id As Integer = Convert.ToInt32(item.GetDataKeyValue("cableway_visit_id"))
            Dim updateVisit As New CablewayVisit(cableway_visit_id)

            Dim newValues As New Hashtable()

            newValues("visit_dt") = TryCast(item.FindControl("rdpVisit"), RadDatePicker).SelectedDate.ToString()
            newValues("visit_type_cd") = TryCast(item.FindControl("ddlVisitType"), DropDownList).SelectedValue.ToString()
            newValues("visit_action_cd") = TryCast(item.FindControl("ddlVisitAction"), DropDownList).SelectedValue.ToString()
            newValues("remarks") = TryCast(item.FindControl("tbRemarks"), TextBox).Text

            Try
                For Each entry As DictionaryEntry In newValues
                    Select Case DirectCast(entry.Key, String)
                        Case "visit_dt"
                            updateVisit.VisitDate = Convert.ToDateTime(entry.Value)
                            Exit Select
                        Case "visit_type_cd"
                            updateVisit.VisitTypeCode = DirectCast(entry.Value, String)
                            Exit Select
                        Case "visit_action_cd"
                            updateVisit.VisitActionCode = DirectCast(entry.Value, String)
                            Exit Select
                        Case "remarks"
                            updateVisit.Remarks = DirectCast(entry.Value, String)
                            Exit Select
                    End Select
                Next

                Dim uploader As RadUpload = e.Item.FindControl("fuFile")
                Dim imageFile As UploadedFile = Nothing
                If uploader.UploadedFiles.Count > 0 Then
                    Dim status_msg As String = SaveMyFile(uploader.UploadedFiles(0), updateVisit.CablewayID, updateVisit.VisitDate)
                    If Not status_msg = "fail" Then
                        updateVisit.VisitFileName = status_msg
                    End If
                End If

                Dim updateStatus As String = updateVisit.UpdateVisitInfo(u.ID)
                If updateStatus = "success" Then
                    DisplayMessage(False, "The cableway visit was updated!")
                Else
                    DisplayMessage(True, "Unable to update cableway visit. Reason: " + updateStatus)
                End If

            Catch ex As Exception
                DisplayMessage(True, "Unable to update cableway visit. Reason: " + ex.Message)

                e.Canceled = True
            End Try
        End If
    End Sub

    Protected Sub rgCableways_InsertCommand(sender As Object, e As GridCommandEventArgs)
        Dim item As GridEditableItem = TryCast(e.Item, GridEditableItem)

        If e.Item.OwnerTableView.Name = "Cableways" Then
            Dim newCW As New Cableway(0)

            Try
                newCW.SiteID = Convert.ToInt32(TryCast(item.FindControl("ddlSites"), DropDownList).SelectedValue)
                newCW.Nickname = TryCast(item.FindControl("tbNickname"), TextBox).Text
                newCW.CablewayStatusCode = TryCast(item.FindControl("ddlCablewayStatus"), DropDownList).SelectedValue
                newCW.CablewayTypeCode = TryCast(item.FindControl("ddlCablewayType"), DropDownList).SelectedValue
                newCW.InspectionFrequency = Convert.ToDouble(TryCast(item.FindControl("rntbFreq"), RadNumericTextBox).Text)
                newCW.AerialMarkerReq = TryCast(item.FindControl("rblAerialMarkerReq"), RadioButtonList).SelectedValue
                newCW.AerialMarkerInst = TryCast(item.FindControl("rblAerialMarkerInst"), RadioButtonList).SelectedValue
            Catch ex As Exception
            End Try

            Try
                Dim insert_results As String = newCW.InsertCablewayInfo(u.ID)

                If insert_results <> "success" Then
                    DisplayMessage(True, insert_results)
                Else
                    DisplayMessage(False, "The cableway was added!")
                End If
            Catch ex As Exception
                DisplayMessage(True, "Unable to insert cableway. Reason: " + ex.Message)
                e.Canceled = True
            End Try
        Else
            Dim newVisit As New CablewayVisit(0)

            Try
                Dim parentItem As GridDataItem = DirectCast(e.Item.OwnerTableView.ParentItem, GridDataItem)
                newVisit.CablewayID = Convert.ToInt32(parentItem.OwnerTableView.DataKeyValues(parentItem.ItemIndex)("cableway_id"))
                newVisit.VisitDate = TryCast(item.FindControl("rdpVisit"), RadDatePicker).SelectedDate
                newVisit.VisitTypeCode = TryCast(item.FindControl("ddlVisitType"), DropDownList).SelectedValue
                newVisit.VisitActionCode = TryCast(item.FindControl("ddlVisitAction"), DropDownList).SelectedValue
                newVisit.Remarks = TryCast(item.FindControl("tbRemarks"), TextBox).Text
            Catch ex As Exception
            End Try

            Dim uploader As RadUpload = e.Item.FindControl("fuFile")
            Dim imageFile As UploadedFile = Nothing
            If uploader.UploadedFiles.Count > 0 Then
                Dim status_msg As String = SaveMyFile(uploader.UploadedFiles(0), newVisit.CablewayID, newVisit.VisitDate)
                If Not status_msg = "fail" Then
                    newVisit.VisitFileName = status_msg
                End If
            End If

            Try
                Dim insert_results As String = newVisit.InsertVisitInfo(u.ID)

                If insert_results <> "success" Then
                    DisplayMessage(True, insert_results)
                Else
                    DisplayMessage(False, "The cableway visit was added!")
                End If
            Catch ex As Exception
                DisplayMessage(True, "Unable to insert cableway visit. Reason: " + ex.Message)
                e.Canceled = True
            End Try
        End If

    End Sub

    Private Sub DisplayMessage(isError As Boolean, text As String)
        Dim label As Label = If((isError), Me.lblError, Me.lblSuccess)
        label.Text = text
    End Sub

    Private Function SaveMyFile(uploadedFile As UploadedFile, cableway_id As Integer, visit_dt As DateTime) As String
        Dim status_msg As String = "fail"
        Dim savePath As String = Server.MapPath("~/Files/Cableways/")
        Dim fullfileName As String = String.Format("{0:yyyyMMdd}", visit_dt) & "_" & uploadedFile.GetName()
        Dim dirToCheck As String = savePath + cableway_id.ToString & "\"
        Dim pathToCheck As String = Nothing

        Try
            'Check to see if there's a pre-existing directory for this site ID
            If Not Directory.Exists(dirToCheck) Then
                'If it does not exist, create the site ID directory
                Directory.CreateDirectory(dirToCheck)
            End If

            pathToCheck = dirToCheck & fullfileName

            If File.Exists(pathToCheck) Then
                status_msg = "exists"
            Else
                uploadedFile.SaveAs(pathToCheck)
                status_msg = fullfileName
            End If
        Catch ex As Exception
        End Try

        Return status_msg
    End Function


End Class
