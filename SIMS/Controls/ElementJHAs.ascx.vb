Imports Telerik.Web.UI
Imports System.Data
Imports System.Data.SqlClient

Public Class ElementJHAs
    Inherits System.Web.UI.UserControl

    Private s As Site
    Private o As Office
    Private w As WSC
    Private uid As String = HttpContext.Current.User.Identity.Name
    Private u As New User(uid)
    Private element_id As String
    Private site_id As String
    Private sha_site_id As Integer
    Private element_nm As String
    Private element_info As String
    Private revised_by As String
    Private revised_dt As String

#Region "Properties"
    Public Property ElementID() As String
        Get
            Return element_id
        End Get
        Set(value As String)
            element_id = value
        End Set
    End Property

    Public Property SiteID() As String
        Get
            Return site_id
        End Get
        Set(value As String)
            site_id = value
        End Set
    End Property

    Public Property SHASiteID() As Integer
        Get
            Return sha_site_id
        End Get
        Set(value As Integer)
            sha_site_id = value
        End Set
    End Property

    Public Property ElementName() As String
        Get
            Return element_nm
        End Get
        Set(value As String)
            element_nm = value
        End Set
    End Property

    Public Property ElementInfo() As String
        Get
            Return element_info
        End Get
        Set(value As String)
            element_info = value
        End Set
    End Property

    Public Property RevisedBy() As String
        Get
            Return revised_by
        End Get
        Set(value As String)
            revised_by = value
        End Set
    End Property

    Public Property RevisedDate() As String
        Get
            Return revised_dt
        End Get
        Set(value As String)
            revised_dt = value
        End Set
    End Property

    Private ReadOnly Property RefLevelTypeList() As DataTable
        Get
            Dim sql As String = "SELECT (reflevel_tp + ' - ' + notes) As reflevel_tp_desc, reflevel_id" & _
                " FROM SHA_Lut_RefLevel"
            Dim adapter As New SqlDataAdapter(sql, Config.ConnectionInfo)
            Dim dt As New DataTable()
            adapter.Fill(dt)
            RefLevelTypeList = dt
        End Get
    End Property

    Private ReadOnly Property JHAList() As DataTable
        Get
            Dim sql As String = "SELECT slej.elem_jha_id, slj.jha_description" & _
                " FROM SHA_Lut_JHA AS slj INNER JOIN" & _
                "      SHA_Lut_ElemJHA AS slej ON slj.jha_id = slej.jha_id LEFT OUTER JOIN" & _
                "      (SELECT DISTINCT slej.elem_jha_id" & _
                "       FROM SHA_Lut_JHA AS slj INNER JOIN" & _
                "       SHA_Lut_ElemJHA AS slej ON slj.jha_id = slej.jha_id INNER JOIN" & _
                "       SHA_Site_JHA AS ssj ON slej.elem_jha_id = ssj.elem_jha_id" & _
                "       WHERE (ssj.sha_site_id = " & s.SHASiteID & ")) AS dt ON dt.elem_jha_id = slej.elem_jha_id" & _
                " WHERE (slej.element_id = " & ElementID & ") And dt.elem_jha_id Is NULL And slej.elem_jha_id <> 1"
            Dim adapter As New SqlDataAdapter(sql, Config.ConnectionInfo)
            Dim dt As New DataTable()
            adapter.Fill(dt)
            JHAList = dt
        End Get
    End Property
#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        s = New Site(SiteID)

        ltlElemRevisedInfo.Text = "Revised by: " & RevisedBy & " &nbsp;&nbsp;Date revised: " & RevisedDate
        ltlElemInfo.Text = ElementInfo
        ltlElemName.Text = ElementName
        ltlElemName2.Text = ElementName

        hlRevisionHistory.NavigateUrl = "../Archives.aspx?element_id=" & ElementID & "&site_id=" & s.ID & "&office_id=" & s.OfficeID & "&begin_dt=1/1/1900&end_dt=" & DateTime.Now

        If hfToggleElementEditMode.Value = "true" Then
            lbToggleElementEditMode.Text = "leave element edit mode"
            pnlEditElemInfo.Visible = True
            pnlStaticElemInfo.Visible = False
        Else
            lbToggleElementEditMode.Text = "enter element edit mode"
            pnlEditElemInfo.Visible = False
            pnlStaticElemInfo.Visible = True
        End If

        If hfToggleElementHazardEditMode.Value = "true" Then
            lbToggleElementHazardEditMode.Text = "leave hazard edit mode"
        Else
            lbToggleElementHazardEditMode.Text = "enter hazard edit mode"
        End If
    End Sub

    Protected Sub lbToggleElementEditMode_Click(ByVal sender As Object, ByVal e As EventArgs)
        If hfToggleElementEditMode.Value = "" Then
            hfToggleElementEditMode.Value = "true"
            lbToggleElementEditMode.Text = "leave element edit mode"

            reElemInfo.Content = ElementInfo
            pnlEditElemInfo.Visible = True
            pnlStaticElemInfo.Visible = False
        Else
            hfToggleElementEditMode.Value = ""
            lbToggleElementEditMode.Text = "enter element edit mode"

            pnlEditElemInfo.Visible = False
            pnlStaticElemInfo.Visible = True
        End If
    End Sub

    Protected Sub lbToggleElementHazardEditMode_Click(ByVal sender As Object, ByVal e As EventArgs)
        If hfToggleElementHazardEditMode.Value = "" Then
            hfToggleElementHazardEditMode.Value = "true"
            lbToggleElementHazardEditMode.Text = "leave hazard edit mode"
        Else
            hfToggleElementHazardEditMode.Value = ""
            lbToggleElementHazardEditMode.Text = "enter hazard edit mode"
            lblError2.Visible = False
            lblSuccess2.Visible = False
        End If
        rgElementHazards.Rebind()
    End Sub

    Private Sub DisplayMessage(isError As Boolean, text As String, type As String)
        Dim label As Label
        If type = "element" Then
            label = If((isError), Me.lblError1, Me.lblSuccess1)
        Else
            label = If((isError), Me.lblError2, Me.lblSuccess2)
        End If
        label.Text = text
    End Sub

#Region "Element Editing Routines"
    Protected Sub btnSubmitElemInfo_Command(sender As Object, e As CommandEventArgs) Handles btnSubmitElemInfo.Command
        If e.CommandArgument = "editelement" And hfToggleElementEditMode.Value <> "" Then
            Dim elem_temp As New SiteElement(ElementID, SiteID)

            elem_temp.ElementInfo = reElemInfo.Content

            Dim update_status As String = elem_temp.UpdateSiteElementInfo(u.ID)

            If update_status = "success" Then
                DisplayMessage(False, "The element information was updated!", "element")
                ltlElemRevisedInfo.Text = "Revised by: " & u.ID & " &nbsp;&nbsp;Date revised: " & Date.Now.ToString()
                ltlElemInfo.Text = reElemInfo.Content
            Else
                DisplayMessage(True, update_status, "element")
            End If
        End If

        hfToggleElementEditMode.Value = ""
        lbToggleElementEditMode.Text = "enter element edit mode"

        pnlEditElemInfo.Visible = False
        pnlStaticElemInfo.Visible = True
    End Sub
#End Region

#Region "RadGrid Routines"
    Public Sub rgElementHazards_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs) Handles rgElementHazards.NeedDataSource
        If Not e.IsFromDetailTable Then
            Dim dt As New DataTable

            Using cnx As New SqlConnection(Config.ConnectionInfo)
                cnx.Open()

                Dim sql As String = "SELECT shsm.sha_site_id, ssj.site_jha_id, slej.elem_jha_id, slj.jha_id, slj.jha_description" & _
                    " FROM SHA_Lut_ElemJHA AS slej INNER JOIN" & _
                    " SHA_Lut_JHA AS slj ON slej.jha_id = slj.jha_id INNER JOIN" & _
                    " SHA_Site_JHA AS ssj ON slej.elem_jha_id = ssj.elem_jha_id INNER JOIN" & _
                    " SHA_Site_Master AS shsm ON ssj.sha_site_id = shsm.sha_site_id" & _
                    " WHERE (shsm.site_id = " & SiteID & ") And (slej.element_id = " & ElementID & ")"

                Dim cmd As New SqlCommand(sql, cnx)
                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)

                cnx.Close()
            End Using

            If hfToggleElementHazardEditMode.Value = "true" And JHAList.Rows.Count > 0 Then
                rgElementHazards.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.Bottom
            Else
                rgElementHazards.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None
                rgElementHazards.MasterTableView.ClearEditItems()
            End If

            rgElementHazards.DataSource = dt
        End If
    End Sub

    Public Sub rgElementHazards_DetailTableDataBind(ByVal source As Object, ByVal e As GridDetailTableDataBindEventArgs) Handles rgElementHazards.DetailTableDataBind
        Dim dataItem As GridDataItem = CType(e.DetailTableView.ParentItem, GridDataItem)
        Dim sql As String = Nothing
        Dim dt As New DataTable

        Dim site_jha_id As String = dataItem.GetDataKeyValue("site_jha_id").ToString()

        Select Case e.DetailTableView.Name
            Case "Hazards"
                sql = "SELECT site_jha_id, site_specificcond_id, remarks, priority" & _
                    " FROM SHA_Site_SpecificCond AS sssc" & _
                    " WHERE (site_jha_id = " & dataItem.GetDataKeyValue("site_jha_id") & ")" & _
                    " ORDER BY priority DESC"
            Case "JobLimits"
                sql = "SELECT ssrl.site_reflevel_id, ssrl.site_jha_id, slrl.reflevel_id, slrl.reflevel_tp, ssrl.reflevel_va, ssrl.reflevel_units, ssrl.remarks, (slrl.reflevel_tp + ' - ' + slrl.notes) As reflevel_desc" & _
                    " FROM SHA_Lut_RefLevel AS slrl INNER JOIN" & _
                    " SHA_Site_RefLevel AS ssrl ON slrl.reflevel_id = ssrl.reflevel_id" & _
                    " WHERE (ssrl.site_jha_id = " & dataItem.GetDataKeyValue("site_jha_id") & ")"
        End Select

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand(sql, cnx)
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            cnx.Close()
        End Using

        If hfToggleElementHazardEditMode.Value = "true" Then
            e.DetailTableView.CommandItemDisplay = GridCommandItemDisplay.Bottom
        Else
            e.DetailTableView.CommandItemDisplay = GridCommandItemDisplay.None
            e.DetailTableView.ClearEditItems()
        End If

        e.DetailTableView.DataSource = dt
    End Sub

    ''' <summary>
    ''' This routine hides the edit features of the grid while the enter edit mode link has not been selected
    ''' </summary>
    Protected Sub rgElementHazards_PreRender(sender As Object, e As EventArgs)
        If hfToggleElementHazardEditMode.Value = "" Then
            For Each col As GridColumn In rgElementHazards.MasterTableView.RenderColumns
                If col.UniqueName = "DeleteColumn" Then
                    col.Visible = False
                End If
            Next

            For Each item As GridDataItem In rgElementHazards.Items
                If (item.OwnerTableView.Name = "JobLimits" AndAlso TypeOf item Is GridEditableItem) Then
                    Dim editableItem1 As GridEditableItem = TryCast(item, GridEditableItem)
                    Try
                        Dim imgbtn1 As ImageButton = DirectCast(editableItem1("EditCommandColumn2").Controls(0), ImageButton)
                        imgbtn1.Visible = False
                        Dim delbtn1 As ImageButton = DirectCast(editableItem1("JobLimitDeleteColumn").Controls(0), ImageButton)
                        delbtn1.Visible = False
                    Catch ex As Exception
                    End Try
                End If

                If (item.OwnerTableView.Name = "Hazards" AndAlso TypeOf item Is GridEditableItem) Then
                    Dim editableItem2 As GridEditableItem = TryCast(item, GridEditableItem)
                    Try
                        Dim imgbtn2 As ImageButton = DirectCast(editableItem2("EditCommandColumn1").Controls(0), ImageButton)
                        imgbtn2.Visible = False
                        Dim delbtn2 As ImageButton = DirectCast(editableItem2("HazardDeleteColumn").Controls(0), ImageButton)
                        delbtn2.Visible = False
                    Catch ex As Exception
                    End Try
                End If
            Next
        Else
            For Each col As GridColumn In rgElementHazards.MasterTableView.RenderColumns
                If col.UniqueName = "DeleteColumn" Then
                    col.Visible = True
                End If
            Next
        End If
    End Sub

    ''' <summary>
    ''' This routine sets up the edit form templates for the RadGrid - the drop-down lists and headings for the forms are populated, and insert/update buttons displayed appropriately
    ''' </summary>
    Protected Sub rgElementHazards_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs)
        If "Hazards".Equals(e.Item.OwnerTableView.Name) Then
            If TypeOf e.Item Is GridDataItem Then
                Dim dataItem As GridDataItem = DirectCast(e.Item, GridDataItem)
                For Each col As GridColumn In rgElementHazards.Columns
                    If dataItem("priority").Text = "True" Then
                        dataItem("remarks").Attributes.CssStyle.Add("font-weight", "bold")
                    End If
                Next
            End If

            If e.Item.IsInEditMode Then
                Dim item As GridEditableItem = DirectCast(e.Item, GridEditableItem)

                Dim ltlHazardsEditFormTitle As Literal = DirectCast(item.FindControl("ltlHazardsEditFormTitle"), Literal)
                Dim tbHazard As TextBox = DirectCast(item.FindControl("tbHazard"), TextBox)
                Dim cbPriority As CheckBox = DirectCast(item.FindControl("cbPriority"), CheckBox)
                Dim btnUpdate2 As Button = DirectCast(item.FindControl("btnUpdate2"), Button)
                Dim btnInsert2 As Button = DirectCast(item.FindControl("btnInsert2"), Button)

                If Not (TypeOf e.Item Is IGridInsertItem) Then
                    ltlHazardsEditFormTitle.Text = "Edit Hazard"
                    tbHazard.Text = DirectCast(e.Item.DataItem, DataRowView)("remarks").ToString()
                    If DirectCast(e.Item.DataItem, DataRowView)("priority").ToString() = "False" Then
                        cbPriority.Checked = False
                    Else
                        cbPriority.Checked = True
                    End If

                    btnUpdate2.Visible = True
                    btnInsert2.Visible = False
                ElseIf (TypeOf e.Item Is IGridInsertItem) Then
                    ltlHazardsEditFormTitle.Text = "Add New Hazard"

                    btnUpdate2.Visible = False
                    btnInsert2.Visible = True
                End If
            End If
        ElseIf "JobLimits".Equals(e.Item.OwnerTableView.Name) Then
            If e.Item.IsInEditMode Then
                Dim item As GridEditableItem = DirectCast(e.Item, GridEditableItem)

                Dim ltlJobLimitsEditFormTitle As Literal = DirectCast(item.FindControl("ltlJobLimitsEditFormTitle"), Literal)
                Dim rcbJobLimitType As RadComboBox = DirectCast(item.FindControl("rcbJobLimitType"), RadComboBox)
                Dim rntbJobLimitValue As RadNumericTextBox = DirectCast(item.FindControl("rntbJobLimitValue"), RadNumericTextBox)
                Dim tbUnits As TextBox = DirectCast(item.FindControl("tbUnits"), TextBox)
                Dim tbRemarks As TextBox = DirectCast(item.FindControl("tbRemarks"), TextBox)
                Dim btnUpdate3 As Button = DirectCast(item.FindControl("btnUpdate3"), Button)
                Dim btnInsert3 As Button = DirectCast(item.FindControl("btnInsert3"), Button)

                rcbJobLimitType.DataSource = RefLevelTypeList

                If Not (TypeOf e.Item Is IGridInsertItem) Then
                    ltlJobLimitsEditFormTitle.Text = "Edit Job Limit"
                    rcbJobLimitType.SelectedValue = DirectCast(e.Item.DataItem, DataRowView)("reflevel_id").ToString()
                    rntbJobLimitValue.Text = DirectCast(e.Item.DataItem, DataRowView)("reflevel_va").ToString()
                    tbUnits.Text = DirectCast(e.Item.DataItem, DataRowView)("reflevel_units").ToString()
                    tbRemarks.Text = DirectCast(e.Item.DataItem, DataRowView)("remarks").ToString()

                    btnUpdate3.Visible = True
                    btnInsert3.Visible = False
                ElseIf (TypeOf e.Item Is IGridInsertItem) Then
                    ltlJobLimitsEditFormTitle.Text = "Add New Job Limit"

                    btnUpdate3.Visible = False
                    btnInsert3.Visible = True
                End If

                rcbJobLimitType.DataBind()
            End If
        Else
            If e.Item.IsInEditMode Then
                Dim item As GridEditableItem = DirectCast(e.Item, GridEditableItem)

                Dim ddlMeasType As DropDownList = DirectCast(item.FindControl("ddlMeasType"), DropDownList)

                ddlMeasType.DataSource = JHAList
                ddlMeasType.DataBind()
            End If

            If TypeOf e.Item Is GridDataItem Then
                Dim dataItem As GridDataItem = DirectCast(e.Item, GridDataItem)
                Dim hlFullReport As HyperLink = DirectCast(dataItem.FindControl("hlFullReport"), HyperLink)
                Dim site_jha_id As Integer = Convert.ToInt32(dataItem.OwnerTableView.DataKeyValues(dataItem.ItemIndex)("site_jha_id"))

                Dim shj As New SHAJHA(site_jha_id)

                Select Case shj.ElementJHAID
                    Case 1 'Servicing Surface Water Field Sites
                        hlFullReport.NavigateUrl = "../Docs/JHAs/Servicing_Site.pdf"
                    Case 2 'Wading Measurements
                        hlFullReport.NavigateUrl = "../Docs/JHAs/Wading_Measurements.pdf"
                    Case 3 'Bridge Measurements
                        hlFullReport.NavigateUrl = "../Docs/JHAs/Bridge_Measurements.pdf"
                    Case 4 'Cableway Measurements
                        hlFullReport.NavigateUrl = "../Docs/JHAs/Cableway_Measurements.pdf"
                    Case 5 'Boat Measurements for Stream Sites
                        hlFullReport.NavigateUrl = "../Docs/JHAs/Boat_Measurements_Streams.pdf"
                    Case 6 'Service and Measure Groundwater Field Site
                        hlFullReport.NavigateUrl = "../Docs/JHAs/Groundwater.pdf"
                    Case 7, 8 'Stilling Well with Confined Space Hazard
                        hlFullReport.NavigateUrl = "../Docs/JHAs/StillingWell_ConfinedSpace.pdf"
                    Case 9 'Water Quality Sampling
                        hlFullReport.NavigateUrl = "../Docs/JHAs/Water_Quality_Sampling.pdf"
                    Case 10 'Boat Measurements for Lake Sites
                        hlFullReport.NavigateUrl = "../Docs/JHAs/Boat_Measurements_Lakes.pdf"
                End Select
            End If
        End If
    End Sub

    ''' <summary>
    ''' This routine fires when data is being inserted into the grid
    ''' </summary>
    Protected Sub rgElementHazards_InsertCommand(sender As Object, e As GridCommandEventArgs)
        Dim item As GridEditableItem = TryCast(e.Item, GridEditableItem)

        If e.Item.OwnerTableView.Name = "Hazards" Then
            Dim newSSC As New SHASpecificCond(0)

            Try
                Dim parentItem As GridDataItem = DirectCast(e.Item.OwnerTableView.ParentItem, GridDataItem)
                newSSC.SiteJHAID = Convert.ToInt32(parentItem.OwnerTableView.DataKeyValues(parentItem.ItemIndex)("site_jha_id"))
                newSSC.Remarks = TryCast(item.FindControl("tbHazard"), TextBox).Text
                newSSC.Priority = TryCast(item.FindControl("cbPriority"), CheckBox).Checked
            Catch ex As Exception
            End Try

            Try
                Dim insert_results As String = newSSC.InsertSpecificCondInfo(u.ID)

                If insert_results <> "success" Then
                    DisplayMessage(True, insert_results, "hazards")
                Else
                    DisplayMessage(False, "The site specific condition was added!", "hazards")
                End If
            Catch ex As Exception
                DisplayMessage(True, "Unable to insert site specific condition. Reason: " + ex.Message, "hazards")
                e.Canceled = True
            End Try
        ElseIf e.Item.OwnerTableView.Name = "JobLimits" Then
            Dim newJL As New SHARefLevel(0)

            Try
                Dim parentItem As GridDataItem = DirectCast(e.Item.OwnerTableView.ParentItem, GridDataItem)
                newJL.SiteJHAID = Convert.ToInt32(parentItem.OwnerTableView.DataKeyValues(parentItem.ItemIndex)("site_jha_id"))
                newJL.RefLevelID = TryCast(item.FindControl("rcbJobLimitType"), RadComboBox).SelectedValue
                newJL.RefLevelValue = TryCast(item.FindControl("rntbJobLimitValue"), RadNumericTextBox).Text
                newJL.RefLevelUnits = TryCast(item.FindControl("tbUnits"), TextBox).Text
                newJL.Remarks = TryCast(item.FindControl("tbRemarks"), TextBox).Text
            Catch ex As Exception
            End Try

            Try
                Dim insert_results As String = newJL.InsertRefLevel(u.ID)

                If insert_results <> "success" Then
                    DisplayMessage(True, insert_results, "hazards")
                Else
                    DisplayMessage(False, "The job operational limit was added!", "hazards")
                End If
            Catch ex As Exception
                DisplayMessage(True, "Unable to insert job operational limit. Reason: " + ex.Message, "hazards")
                e.Canceled = True
            End Try
        Else
            Dim newJHA As New SHAJHA(0)

            Try
                newJHA.ElementJHAID = TryCast(item.FindControl("ddlMeasType"), DropDownList).SelectedValue
                newJHA.SHASiteID = SHASiteID
            Catch ex As Exception
            End Try

            Try
                Dim insert_results As String = newJHA.InsertSiteJHA(u.ID)

                If insert_results <> "success" Then
                    DisplayMessage(True, insert_results, "hazards")
                Else
                    DisplayMessage(False, "The measurement type was added!", "hazards")
                End If
            Catch ex As Exception
                DisplayMessage(True, "Unable to insert measurement type. Reason: " + ex.Message, "hazards")
                e.Canceled = True
            End Try
        End If

        'Update the updated_by and updated_dt fields of SHA_Site_Master - the updated_by and updated_dt fields are legacy; not being used outside of this situation
        s.UpdateSHA(u.ID)
    End Sub

    ''' <summary>
    ''' Fires when updating information in the grid
    ''' </summary>
    Protected Sub rgElementHazards_UpdateCommand(sender As Object, e As GridCommandEventArgs)
        Dim item As GridEditableItem = TryCast(e.Item, GridEditableItem)

        If e.Item.OwnerTableView.Name = "Hazards" Then
            Dim site_specificcond_id As Integer = Convert.ToInt32(item.GetDataKeyValue("site_specificcond_id"))
            Dim updateSSC As New SHASpecificCond(site_specificcond_id)

            Dim newValues As New Hashtable()

            newValues("remarks") = TryCast(item.FindControl("tbHazard"), TextBox).Text
            newValues("priority") = TryCast(item.FindControl("cbPriority"), CheckBox).Checked.ToString()

            Try
                For Each entry As DictionaryEntry In newValues
                    Select Case DirectCast(entry.Key, String)
                        Case "remarks"
                            updateSSC.Remarks = DirectCast(entry.Value, String)
                            Exit Select
                        Case "priority"
                            updateSSC.Priority = DirectCast(entry.Value, String)
                            Exit Select
                    End Select
                Next

                Dim updateStatus As String = updateSSC.UpdateSpecificCondInfo(u.ID)
                If updateStatus = "success" Then
                    DisplayMessage(False, "The site specific condition was updated!", "hazards")
                Else
                    DisplayMessage(True, "Unable to update site specific condition. Reason: " + updateStatus, "hazards")
                End If
            Catch ex As Exception
                DisplayMessage(True, "Unable to update site specific condition. Reason: " + ex.Message, "hazards")
                e.Canceled = True
            End Try
        ElseIf e.Item.OwnerTableView.Name = "JobLimits" Then
            Dim site_reflevel_id As Integer = Convert.ToInt32(item.GetDataKeyValue("site_reflevel_id"))
            Dim updateRL As New SHARefLevel(site_reflevel_id)

            Dim newValues As New Hashtable()

            Try
                newValues("reflevel_id") = Convert.ToInt32(TryCast(item.FindControl("rcbJobLimitType"), RadComboBox).SelectedValue)
                newValues("reflevel_va") = Convert.ToDouble(TryCast(item.FindControl("rntbJobLimitValue"), RadNumericTextBox).Text)
                newValues("reflevel_units") = TryCast(item.FindControl("tbUnits"), TextBox).Text
                newValues("remarks") = TryCast(item.FindControl("tbRemarks"), TextBox).Text

                For Each entry As DictionaryEntry In newValues
                    Select Case DirectCast(entry.Key, String)
                        Case "reflevel_id"
                            updateRL.RefLevelID = DirectCast(entry.Value, Integer)
                            Exit Select
                        Case "reflevel_va"
                            updateRL.RefLevelValue = DirectCast(entry.Value, Double)
                            Exit Select
                        Case "reflevel_units"
                            updateRL.RefLevelUnits = DirectCast(entry.Value, String)
                            Exit Select
                        Case "remarks"
                            updateRL.Remarks = DirectCast(entry.Value, String)
                            Exit Select
                    End Select
                Next

                Dim updateStatus As String = updateRL.UpdateRefLevel(u.ID)
                If updateStatus = "success" Then
                    DisplayMessage(False, "The job operational limit was updated!", "hazards")
                Else
                    DisplayMessage(True, "Unable to update job operational limit. Reason: " + updateStatus, "hazards")
                End If
            Catch ex As Exception
                DisplayMessage(True, "Unable to update job operational limit. Reason: " + ex.Message, "hazards")
                e.Canceled = True
            End Try
        End If

        'Update the updated_by and updated_dt fields of SHA_Site_Master - the updated_by and updated_dt fields are legacy; not being used outside of this situation
        s.UpdateSHA(u.ID)
    End Sub

    ''' <summary>
    ''' Fires when deleting information from the grid
    ''' </summary>
    Protected Sub rgElementHazards_DeleteCommand(sender As Object, e As GridCommandEventArgs)
        Dim item As GridDataItem = TryCast(e.Item, GridDataItem)
        Dim deleteStatus As String = Nothing

        If e.Item.OwnerTableView.Name = "Hazards" Then
            Try
                Dim site_specificcond_id As Integer = Convert.ToInt32(item.GetDataKeyValue("site_specificcond_id"))
                Dim deleteSSC As New SHASpecificCond(site_specificcond_id)

                deleteStatus = deleteSSC.DeleteSpecificCond(u.ID)
                If deleteStatus = "success" Then
                    DisplayMessage(False, "The site specific condition was deleted!", "hazards")
                Else
                    DisplayMessage(True, "Unable to delete site specific condition. Reason: " + deleteStatus, "hazards")
                End If
            Catch ex As Exception
                DisplayMessage(True, "Unable to delete site specific condition. Reason: " + ex.Message, "hazards")

                e.Canceled = True
            End Try
        ElseIf e.Item.OwnerTableView.Name = "JobLimits" Then
            Try
                Dim site_reflevel_id As Integer = Convert.ToInt32(item.GetDataKeyValue("site_reflevel_id"))
                Dim deleteRL As New SHARefLevel(site_reflevel_id)

                deleteStatus = deleteRL.DeleteRefLevel(u.ID)
                If deleteStatus = "success" Then
                    DisplayMessage(False, "The job operational limit was deleted!", "hazards")
                Else
                    DisplayMessage(True, "Unable to delete job operational limit. Reason: " + deleteStatus, "hazards")
                End If
            Catch ex As Exception
                DisplayMessage(True, "Unable to delete job operational limit. Reason: " + ex.Message, "hazards")

                e.Canceled = True
            End Try
        Else
            Try
                Dim site_jha_id As Integer = Convert.ToInt32(item.GetDataKeyValue("site_jha_id"))
                Dim deleteJHA As New SHAJHA(site_jha_id)

                deleteStatus = deleteJHA.DeleteJHA(u.ID)
                If deleteStatus = "success" Then
                    DisplayMessage(False, "The SHA was deleted!", "hazards")
                Else
                    DisplayMessage(True, "Unable to delete SHA. Reason: " + deleteStatus, "hazards")
                End If
            Catch ex As Exception
                DisplayMessage(True, "Unable to delete SHA. Reason: " + ex.Message, "hazards")

                e.Canceled = True
            End Try
        End If

        'Update the updated_by and updated_dt fields of SHA_Site_Master - the updated_by and updated_dt fields are legacy; not being used outside of this situation
        s.UpdateSHA(u.ID)
    End Sub

#End Region


End Class