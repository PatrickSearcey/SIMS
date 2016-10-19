Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb
Imports System.Web.HttpContext
Imports Telerik.Web.UI

Public Class SiteHazardAnalysis
    Inherits System.Web.UI.Page

    Private s As Site
    Private o As Office
    Private w As WSC
    Private se As SiteElement
    Private uid As String = Current.User.Identity.Name
    Private u As New User(uid)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '--BASIC PAGE SETUP--------------------------------------------------------------------
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Master.PageTitle = "Site Hazard Analysis"
        Master.ResponsibleOffice = True
        Page.Title = "SIMS - Site Hazard Analysis"

        Dim site_id As Integer = Request.QueryString("site_id") '3000336 '
        Dim site_no As String = Request.QueryString("site_no")
        Dim agency_cd As String = Request.QueryString("agency_cd")
        Dim office_id As Integer = Request.QueryString("office_id")

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

        If Not office_id = Nothing Then
            o = New Office(office_id)
        ElseIf site_id <> Nothing Then
            office_id = s.OfficeID
            o = New Office(office_id)
        End If

        w = New WSC(o.WSCID)

        If Not Page.IsPostBack Then
            '--PAGE ACCESS SECTION-------------------------------------------------------------
            Master.CheckAccessLevel(w.ID, "None")
            If Master.NoAccessPanel Then
                Response.Redirect("SHAPrint.aspx?site_id=" & s.ID)
                pnlHasAccess.Visible = False
            Else
                pnlHasAccess.Visible = True
            End If
            '--END PAGE ACCESS SECTION---------------------------------------------------------

            'Start with a clean session
            Session.Clear()

            hlPrintSHA.NavigateUrl = "SHAPrint.aspx?site_id=" & s.ID

            Call PopulatePagePanels()
        End If
        '--------------------------------------------------------------------------------------

        'Moved out of PopulatePagePanels routine for AJAX to work
        SetupMeasurementSpecificPanel()
    End Sub

    Protected Sub PopulatePagePanels()
        SetupWarningsPanel()

        SetupServicingSitePanel()

        SetupSiteHazardAnalysisElementPanel()

        SetupEmergencyInfoPanel()

        SetupAdminPanel()
    End Sub

    Private Sub DisplayMessage(isError As Boolean, text As String, type As String)
        Dim label As Label
        Select Case type
            Case "element"
                label = If((isError), Me.lblError1, Me.lblSuccess1)
            Case "sitehazard"
                label = If((isError), Me.lblError2, Me.lblSuccess2)
            Case Else
                label = Nothing
        End Select
        label.Text = text
    End Sub

    Protected Sub AddInfo_Command(ByVal sender As Object, ByVal e As CommandEventArgs)
        If e.CommandName = "ServicingSite" Then
            If e.CommandArgument = "AddHazard" Then
                AddServicingSiteHazard()
            ElseIf e.CommandArgument = "AddEquip" Then
                AddRecEquipment()
            End If
            SetupServicingSitePanel()
        End If
    End Sub

    Protected Sub ram_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs) Handles ram.AjaxRequest
        If e.Argument = "Rebind" Then
            rgHospitals.MasterTableView.SortExpressions.Clear()
            rgHospitals.MasterTableView.GroupByExpressions.Clear()
            rgHospitals.Rebind()
            rgContacts.MasterTableView.SortExpressions.Clear()
            rgContacts.MasterTableView.GroupByExpressions.Clear()
            rgContacts.Rebind()
            ltlNotice1.Text = ""
            ltlNotice2.Text = ""
        End If
    End Sub

#Region "Warnings Panel"
    Protected Sub SetupWarningsPanel()
        Try
            Dim has_warnings As Boolean = CreateWarningBullets()

            If has_warnings Then
                pnlWarnings.Visible = True
            Else
                pnlWarnings.Visible = False
            End If
        Catch ex As Exception
            pnlWarnings.Visible = False
        End Try
    End Sub

    Protected Function CreateWarningBullets() As Boolean
        Dim return_va As Boolean = False
        Dim bullets As New ArrayList
        Dim hospitals As DataTable = s.GetSiteHospitals(s.ID)
        Dim contacts As DataTable = s.GetSiteContacts(s.ID)

        If hospitals.Rows.Count = 0 Then
            bullets.Add("No hospitals have been assigned to this site.")
            return_va = True
        End If

        If contacts.Rows.Count = 0 Then
            bullets.Add("No contacts have been assigned to this site.")
            return_va = True
        End If

        blWarnings.DataSource = bullets
        blWarnings.DataBind()

        Return return_va
    End Function

    Protected Sub ibWarnings_Command(ByVal sender As Object, ByVal e As CommandEventArgs)
        If e.CommandArgument = "open" Then
            pnlWarningList.Visible = True
            ibWarningsClose.Visible = True
            ibWarningsOpen.Visible = False
        Else
            pnlWarningList.Visible = False
            ibWarningsClose.Visible = False
            ibWarningsOpen.Visible = True
        End If
    End Sub
#End Region

#Region "Site Specific Panel"
    Protected Sub SetupServicingSitePanel()
        lvServicingSiteSpecificCond.DataSource = s.GetServicingSiteSpecificConditions(s.ID)
        lvServicingSiteSpecificCond.DataBind()
        lvServicingSiteRecEquip.DataSource = s.GetRecommendedEquipment(s.ID)
        lvServicingSiteRecEquip.DataBind()

        If s.GetServicingSiteSpecificConditions(s.ID).Rows.Count > 0 Then
            If hfToggleHazardEditMode.Value = "true" Then
                lbToggleHazardEditMode.Text = "leave delete/change priority mode"
            Else
                lbToggleHazardEditMode.Text = "enter delete/change priority mode"
            End If
            lbToggleHazardEditMode.Visible = True
        Else
            lbToggleHazardEditMode.Visible = False
        End If

        If s.GetRecommendedEquipment(s.ID).Rows.Count > 0 Then
            If hfToggleEquipEditMode.Value = "true" Then
                lbToggleEquipEditMode.Text = "leave delete mode"
            Else
                lbToggleEquipEditMode.Text = "enter delete mode"
            End If
            lbToggleEquipEditMode.Visible = True
        Else
            lbToggleEquipEditMode.Visible = False
        End If

        If hfAddSiteSpecificInfo.Value = "true" Then
            lbAddSiteSpecificInfo.Visible = False
            imgArrow1.Visible = False
            pnlAddSiteSpecificInfo.Visible = True
        Else
            lbAddSiteSpecificInfo.Visible = True
            imgArrow1.Visible = True
            pnlAddSiteSpecificInfo.Visible = False
        End If
    End Sub

    Protected Sub lvServicingSiteSpecificCond_ItemDataBound(ByVal sender As Object, ByVal e As ListViewItemEventArgs)
        If e.Item.ItemType = ListViewItemType.DataItem Then
            Dim dataitem As ListViewDataItem = DirectCast(e.Item, ListViewDataItem)
            Dim lbl As Label = DirectCast(e.Item.FindControl("lblServicingSiteSpecificCond"), Label)
            Dim tb As TextBox = DirectCast(e.Item.FindControl("tbServicingSiteSpecificCond"), TextBox)
            Dim ibEdit As ImageButton = DirectCast(e.Item.FindControl("ibEditHazard"), ImageButton)
            Dim ibDelete As ImageButton = DirectCast(e.Item.FindControl("ibDeleteHazard"), ImageButton)
            Dim ibPriority As ImageButton = DirectCast(e.Item.FindControl("ibChangePriority"), ImageButton)

            Try
                Dim priority As Integer = CInt(DataBinder.Eval(dataitem.DataItem, "priority"))
                If priority < 0 Then
                    lbl.Font.Bold = True
                End If
            Catch ex As Exception
            End Try

            'EDIT CURRENTLY NOT BEING USED - ALL SET TO FALSE
            Try
                If hfToggleHazardEditMode.Value = "true" Then
                    ibEdit.Visible = False
                    ibDelete.Visible = True
                    ibPriority.Visible = True
                Else
                    ibEdit.Visible = False
                    ibDelete.Visible = False
                    ibPriority.Visible = False
                End If
            Catch ex As Exception
                ibEdit.Visible = False
                ibDelete.Visible = False
                ibPriority.Visible = False
            End Try

            'FOR CONTROLLING EDITING; CURRENTLY NOT BEING USED
            Try
                If String.IsNullOrEmpty(Session("EditHazardID").ToString()) Then
                    tb.Visible = False
                    lbl.Visible = True
                Else
                    If Session("EditHazardID").ToString() = DataBinder.Eval(dataitem.DataItem, "site_servicing_id").ToString() Then
                        tb.Visible = True
                        lbl.Visible = False
                    Else
                        tb.Visible = False
                        lbl.Visible = True
                    End If
                End If
            Catch ex As Exception
                tb.Visible = False
                lbl.Visible = True
            End Try
        End If
    End Sub

    Protected Sub lvServicingSiteRecEquip_ItemDataBound(ByVal sender As Object, ByVal e As ListViewItemEventArgs)
        If e.Item.ItemType = ListViewItemType.DataItem Then
            Dim dataitem As ListViewDataItem = DirectCast(e.Item, ListViewDataItem)
            Dim ibDelete As ImageButton = DirectCast(e.Item.FindControl("ibDeleteEquip"), ImageButton)

            Try
                If hfToggleEquipEditMode.Value = "true" Then
                    ibDelete.Visible = True
                Else
                    ibDelete.Visible = False
                End If
            Catch ex As Exception
                ibDelete.Visible = False
            End Try
        End If
    End Sub

    Protected Sub lbToggleHazardEditMode_Click(ByVal sender As Object, ByVal e As EventArgs)
        If hfToggleHazardEditMode.Value = "" Then
            hfToggleHazardEditMode.Value = "true"
        Else
            hfToggleHazardEditMode.Value = ""
        End If
        SetupServicingSitePanel()
    End Sub

    Protected Sub lbToggleEquipEditMode_Click(ByVal sender As Object, ByVal e As EventArgs)
        If hfToggleEquipEditMode.Value = "" Then
            hfToggleEquipEditMode.Value = "true"
        Else
            hfToggleEquipEditMode.Value = ""
        End If
        SetupServicingSitePanel()
    End Sub

    Protected Sub lbAddSiteSpecificInfo_Command(ByVal sender As Object, ByVal e As CommandEventArgs)
        If hfAddSiteSpecificInfo.Value = "" Then
            hfAddSiteSpecificInfo.Value = "true"
        Else
            hfAddSiteSpecificInfo.Value = ""
        End If
        SetupServicingSitePanel()
    End Sub

    Protected Sub ibHazard_Command(ByVal sender As Object, ByVal e As CommandEventArgs)
        Dim site_servicing_id As String = e.CommandArgument
        Dim sql As String = Nothing

        If e.CommandName = "DeleteHazard" Then
            sql = "DELETE FROM SHA_Site_Servicing" & _
                " WHERE (site_servicing_id = " & site_servicing_id & ")"

            'Update the updated_by and updated_dt fields of SHA_Site_Master
            s.UpdateSHA(u.ID)
        ElseIf e.CommandName = "EditHazard" Then
            'NOT BEING USED AT THIS TIME
        Else
            Dim priority As String = Right(site_servicing_id, 1)
            If priority = "_" Then
                priority = "1"
            Else
                priority = Right(site_servicing_id, 4)
                If priority = "alse" Then
                    priority = "1"
                Else
                    priority = "0"
                End If
            End If

            sql = "UPDATE SHA_Site_Servicing SET priority = " & priority & " WHERE site_servicing_id = " & Mid(site_servicing_id, 1, InStr(site_servicing_id, "_") - 1)
        End If

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As SqlCommand = New SqlCommand(sql, cnx)
            cmd.ExecuteNonQuery()

            cnx.Close()
        End Using

        'Refresh the servicing site panel
        SetupServicingSitePanel()
    End Sub

    Protected Sub ibEquip_Command(ByVal sender As Object, ByVal e As CommandEventArgs)
        Dim site_equip_id As String = e.CommandArgument
        Dim sql As String = Nothing

        If e.CommandName = "DeleteEquip" Then
            Using cnx As New SqlConnection(Config.ConnectionInfo)
                cnx.Open()

                sql = "DELETE FROM SHA_Site_Equip" & _
                " WHERE (site_equip_id = " & site_equip_id & ")"

                Dim cmd As SqlCommand = New SqlCommand(sql, cnx)
                cmd.ExecuteNonQuery()

                cnx.Close()
            End Using
        End If

        'Update the updated_by and updated_dt fields of SHA_Site_Master
        s.UpdateSHA(u.ID)

        'Refresh the servicing site panel
        SetupServicingSitePanel()
    End Sub

    Sub AddServicingSiteHazard()
        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim sql As String = Nothing
            Dim dt As DataTable = New DataTable()
            Dim hazards As New List(Of String)

            sql = "SELECT servicing_va FROM SHA_Site_Servicing WHERE sha_site_id = " & s.SHASiteID
            Dim cmd As New SqlCommand(sql, cnx)
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                hazards.Add(row("servicing_va"))
            Next

            For Each item As RadListBoxItem In rlbHazards.CheckedItems
                If hazards.IndexOf(item.Text) = -1 Then
                    sql = "INSERT INTO SHA_Site_Servicing" & _
                        " (sha_site_id, servicing_va)" & _
                        " VALUES (" & s.SHASiteID & ", '" & item.Text & "')"

                    Dim cmd1 As New SqlCommand(sql, cnx)
                    cmd1.ExecuteNonQuery()
                End If
            Next

            If Not tbOtherHazard.Text = "" Then
                If hazards.IndexOf(tbOtherHazard.Text) = -1 Then
                    sql = "INSERT INTO SHA_Site_Servicing" & _
                        " (sha_site_id, servicing_va)" & _
                        " VALUES (" & s.SHASiteID & ", '" & tbOtherHazard.Text & "')"

                    Dim cmd1 As New SqlCommand(sql, cnx)
                    cmd1.ExecuteNonQuery()
                End If
            End If

            cnx.Close()
        End Using

        'Clear the selections
        tbOtherHazard.Text = ""
        rlbHazards.ClearChecked()

        'Update the updated_by and updated_dt fields of SHA_Site_Master
        s.UpdateSHA(u.ID)
    End Sub

    Sub AddRecEquipment()
        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim sql As String = Nothing
            Dim dt As DataTable = New DataTable()
            Dim equip As New List(Of String)

            sql = "SELECT recom_equip FROM SHA_Site_Equip WHERE sha_site_id = " & s.SHASiteID
            Dim cmd As New SqlCommand(sql, cnx)
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                equip.Add(row("recom_equip"))
            Next

            For Each item As RadListBoxItem In rlbEquip.CheckedItems
                If equip.IndexOf(item.Text) = -1 Then
                    sql = "INSERT INTO SHA_Site_Equip" & _
                        " (sha_site_id, recom_equip)" & _
                        " VALUES (" & s.SHASiteID & ", '" & item.Text & "')"

                    Dim cmd1 As SqlCommand = New SqlCommand(sql, cnx)
                    cmd1.ExecuteNonQuery()
                End If
            Next

            If Not tbOtherEquip.Text = "" Then
                If equip.IndexOf(tbOtherEquip.Text) = -1 Then
                    sql = "INSERT INTO SHA_Site_Equip" & _
                        " (sha_site_id, recom_equip)" & _
                        " VALUES (" & s.SHASiteID & ", '" & tbOtherEquip.Text & "')"

                    Dim cmd1 As SqlCommand = New SqlCommand(sql, cnx)
                    cmd1.ExecuteNonQuery()
                End If
                
            End If

            cnx.Close()
        End Using

        'Clear the selections
        tbOtherEquip.Text = ""
        rlbEquip.ClearChecked()

        'Update the updated_by and updated_dt fields of SHA_Site_Master
        s.UpdateSHA(u.ID)
    End Sub
#End Region

#Region "Measurement Specific Panel"
    Protected Sub SetupMeasurementSpecificPanel()
        Dim elementids As New List(Of Integer)
        Dim elements As New DataTable
        phElements.Controls.Clear()

        elements = s.GetMeasurementElements(s.ID, 0)

        For Each row As DataRow In elements.Rows
            Dim elementJHA As ElementJHAs = CType(LoadControl("~\Controls\ElementJHAs.ascx"), ElementJHAs)

            elementJHA.SiteID = s.ID
            elementJHA.SHASiteID = s.SHASiteID
            elementJHA.ID = "ucElement" & row("element_id").ToString()
            elementJHA.ElementID = row("element_id").ToString()
            elementJHA.ElementName = row("element_nm").ToString()
            Try
                elementJHA.ElementInfo = Replace(row("element_info"), vbCrLf & vbCrLf, "<br /><br />" & vbCrLf)
            Catch ex As Exception
                elementJHA.ElementInfo = ""
            End Try
            Try
                elementJHA.RevisedBy = row("revised_by").ToString()
                elementJHA.RevisedDate = String.Format("{0:MM/dd/yyyy}", row("revised_dt"))
            Catch ex As Exception
                elementJHA.RevisedBy = ""
                elementJHA.RevisedDate = ""
            End Try
            phElements.Controls.Add(elementJHA)

            Dim spacer As LiteralControl = New LiteralControl("<br />")
            phElements.Controls.Add(spacer)

            elementids.Add(row("element_id"))
        Next

        PopulateAddMeasTypeDropDown(elementids)
    End Sub

    Protected Sub PopulateAddMeasTypeDropDown(elementids As List(Of Integer))
        If s.SIMSSiteType = "sw" And elementids.Count < 5 And (ddlNewMeasType.SelectedValue = "" Or ddlNewMeasType.SelectedValue = "0") Then '
            RemoveDropDownItems()

            ddlNewMeasType.Items.Add(New ListItem("", "0"))

            ddlNewMeasType.Items.Add(New ListItem("DISCHARGE MEASUREMENTS", Config.DischargeMeasElem.ToString()))
            ddlNewMeasType.Items.Add(New ListItem("LAKE / RESERVOIR MEASUREMENTS", Config.LakeMeasElem.ToString()))
            ddlNewMeasType.Items.Add(New ListItem("WATER QUALITY MEASUREMENT", Config.QWMeasElem.ToString()))
            ddlNewMeasType.Items.Add(New ListItem("ECOLOGICAL MEASUREMENTS", Config.EcoMeasElem.ToString()))
            ddlNewMeasType.Items.Add(New ListItem("ATMOSPHERIC MEASUREMENTS", Config.AtmMeasElem.ToString()))

            If elementids.Count > 0 Then
                For Each id As Integer In elementids
                    Select Case id
                        Case Config.DischargeMeasElem
                            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.DischargeMeasElem.ToString()))
                        Case Config.QWMeasElem
                            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.QWMeasElem.ToString()))
                        Case Config.LakeMeasElem
                            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.LakeMeasElem.ToString()))
                        Case Config.EcoMeasElem
                            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.EcoMeasElem.ToString()))
                        Case Config.AtmMeasElem
                            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.AtmMeasElem.ToString()))
                    End Select
                Next
            End If

            ddlNewMeasType.Visible = True
            btnNewMeasType.Visible = True
            ltlNewMeasType.Text = "Select a measurement element and click the button to add. By adding an element here, it will also appear in this site's Station Description."
        ElseIf s.SIMSSiteType = "gw" And elementids.Count < 4 And (ddlNewMeasType.SelectedValue = "" Or ddlNewMeasType.SelectedValue = "0") Then
            RemoveDropDownItems()

            ddlNewMeasType.Items.Add(New ListItem("", "0"))

            ddlNewMeasType.Items.Add(New ListItem("GROUNDWATER MEASUREMENTS", Config.GWMeasElem.ToString()))
            ddlNewMeasType.Items.Add(New ListItem("WATER QUALITY MEASUREMENT", Config.QWMeasElem.ToString()))
            ddlNewMeasType.Items.Add(New ListItem("ECOLOGICAL MEASUREMENTS", Config.EcoMeasElem.ToString()))
            ddlNewMeasType.Items.Add(New ListItem("ATMOSPHERIC MEASUREMENTS", Config.AtmMeasElem.ToString()))

            If elementids.Count > 0 Then
                For Each id As Integer In elementids
                    Select Case id
                        Case Config.GWMeasElem
                            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.GWMeasElem.ToString()))
                        Case Config.QWMeasElem
                            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.QWMeasElem.ToString()))
                        Case Config.EcoMeasElem
                            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.EcoMeasElem.ToString()))
                        Case Config.AtmMeasElem
                            ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.AtmMeasElem.ToString()))
                    End Select
                Next
            End If

            ddlNewMeasType.Visible = True
            btnNewMeasType.Visible = True
            ltlNewMeasType.Text = "Select a measurement element and click the button to add. By adding an element here, it will also appear in this site's Station Description."
        ElseIf ddlNewMeasType.SelectedValue = "" Or ddlNewMeasType.SelectedValue = "0" Then
            ddlNewMeasType.Visible = False
            btnNewMeasType.Visible = False
            ltlNewMeasType.Text = "All possible measurement elements have been added to this site."
        End If
    End Sub

    Protected Sub RemoveDropDownItems()
        ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue("0"))
        ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.DischargeMeasElem.ToString()))
        ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.QWMeasElem.ToString()))
        ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.LakeMeasElem.ToString()))
        ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.EcoMeasElem.ToString()))
        ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.AtmMeasElem.ToString()))
        ddlNewMeasType.Items.Remove(ddlNewMeasType.Items.FindByValue(Config.GWMeasElem.ToString()))
    End Sub

    Protected Sub btnNewMeasType_Command(sender As Object, e As System.Web.UI.WebControls.CommandEventArgs) Handles btnNewMeasType.Command
        If Session("btnpressed") = "" Or Session("btnpressed") = "true" Then
            Dim element_id As Integer = CInt(ddlNewMeasType.SelectedValue)
            Dim elem_temp As New SiteElement(element_id, s.ID)

            elem_temp.ElementID = element_id

            Dim add_status As String = elem_temp.AddNewElementToSite(u.ID)

            If add_status = "success" Then
                DisplayMessage(False, "The element was added!", "element")
            Else
                Session("btnpressed") = ""
                DisplayMessage(True, add_status, "element")
            End If

            ddlNewMeasType.SelectedValue = Nothing
            SetupMeasurementSpecificPanel()
        End If
        Session("btnpressed") = "true"
    End Sub
#End Region

#Region "Site Hazard Analysis Element Panel"
    Protected Sub SetupSiteHazardAnalysisElementPanel()
        se = New SiteElement(Config.SiteHazardElem, s.ID)

        If se.ElementInfo <> "" Then
            pnlNoSiteHazardElem.Visible = False
            pnlSiteHazardElem.Visible = True

            ltlElemInfo.Text = se.ElementInfo
            ltlElemRevisedInfo.Text = "Revised by: " & se.RevisedBy & " &nbsp;&nbsp;Date revised: " & se.RevisedDate.ToString()

            hlRevisionHistory.NavigateUrl = "Archives.aspx?element_id=" & Config.SiteHazardElem & "&site_id=" & s.ID & "&office_id=" & s.OfficeID & "&begin_dt=1/1/1900&end_dt=" & DateTime.Now

            If hfToggleElementEditMode.Value = "true" Then
                lbToggleElementEditMode.Text = "leave element edit mode"
                pnlEditElemInfo.Visible = True
                pnlStaticElemInfo.Visible = False
            Else
                lbToggleElementEditMode.Text = "enter element edit mode"
                pnlEditElemInfo.Visible = False
                pnlStaticElemInfo.Visible = True
            End If
        Else
            pnlNoSiteHazardElem.Visible = True
            pnlSiteHazardElem.Visible = False
        End If
    End Sub

    Protected Sub lbToggleElementEditMode_Click(ByVal sender As Object, ByVal e As EventArgs)
        If hfToggleElementEditMode.Value = "" Then
            hfToggleElementEditMode.Value = "true"
            lbToggleElementEditMode.Text = "leave element edit mode"

            se = New SiteElement(Config.SiteHazardElem, s.ID)
            reElemInfo.Content = se.ElementInfo
            pnlEditElemInfo.Visible = True
            pnlStaticElemInfo.Visible = False
        Else
            hfToggleElementEditMode.Value = ""
            lbToggleElementEditMode.Text = "enter element edit mode"

            pnlEditElemInfo.Visible = False
            pnlStaticElemInfo.Visible = True
        End If
    End Sub

    Protected Sub btnSubmitElemInfo_Command(sender As Object, e As CommandEventArgs) Handles btnSubmitElemInfo.Command
        If e.CommandArgument = "editelement" And hfToggleElementEditMode.Value <> "" Then
            Dim elem_temp As New SiteElement(Config.SiteHazardElem, s.ID)

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

#Region "Emergency Info Panel"
    Private ReadOnly Property EmergInfoLink() As String
        Get
            Return "EmergencyInfo.aspx?office_id=" & o.ID
        End Get
    End Property

    Protected Sub SetupEmergencyInfoPanel()
        Dim emerg_service As Boolean = s.EmergencyService

        If emerg_service Then
            lbl911Service.Text = "911 emergency service is available at this site.<br />"
        Else
            lbl911Service.Text = "911 emergency service is NOT available at this site.<br />"
        End If

        Dim cell_service As Boolean = s.CellService

        If cell_service Then
            lblCellService.Text = "Cell service is available at this site.<br /><br />"
        Else
            lblCellService.Text = "Cell service is NOT available at this site.<br /><br />"
        End If

        hlEmergInfo1.NavigateUrl = EmergInfoLink
        hlEmergInfo4.NavigateUrl = EmergInfoLink
    End Sub

    Protected Sub lbEmergService_Command(sender As Object, e As CommandEventArgs)
        Dim es As String = Nothing
        Dim sql As String = Nothing

        If e.CommandArgument = "911" Then
            If s.EmergencyService Then
                es = "0"
                lbl911Service.Text = "911 emergency service is NOT available at this site.<br />"
            Else
                es = "1"
                lbl911Service.Text = "911 emergency service is available at this site.<br />"
            End If

            sql = "UPDATE SHA_Site_Master SET emerg_service = " & es & " WHERE sha_site_id = " & s.SHASiteID
        Else
            If s.CellService Then
                es = "0"
                lblCellService.Text = "Cell service is NOT available at this site.<br /><br />"
            Else
                es = "1"
                lblCellService.Text = "Cell service is available at this site.<br /><br />"
            End If

            sql = "UPDATE SHA_Site_Master SET cell_service = " & es & " WHERE sha_site_id = " & s.SHASiteID
        End If

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As SqlCommand = New SqlCommand(sql, cnx)
            cmd.ExecuteNonQuery()
            cnx.Close()
        End Using
    End Sub

#Region "Contacts"
    Protected Sub rgContacts_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs)
        rgContacts.DataSource = s.GetSiteContacts(s.ID)
    End Sub

    Protected Sub rgContacts_PreRender(sender As Object, e As EventArgs)
        Dim menu As GridFilterMenu = rgContacts.FilterMenu
        Dim i As Integer = 0
        While i < menu.Items.Count
            If menu.Items(i).Text = "NoFilter" Or menu.Items(i).Text = "Contains" Or menu.Items(i).Text = "EqualTo" Or menu.Items(i).Text = "DoesNotContain" Then
                i = i + 1
            Else
                menu.Items.RemoveAt(i)
            End If
        End While
    End Sub

    Protected Sub rgContacts_ItemCreated(ByVal sender As Object, ByVal e As GridItemEventArgs)
        If TypeOf e.Item Is GridDataItem Then
            Try
                Dim deleteLink As HyperLink = DirectCast(e.Item.FindControl("hlDelete"), HyperLink)
                deleteLink.Attributes("href") = "#"
                deleteLink.Attributes("onclick") = [String].Format("return ShowDeleteContactForm('{0}','{1}','{2}');", e.Item.OwnerTableView.DataKeyValues(e.Item.ItemIndex)("contact_id"), e.Item.ItemIndex, s.SHASiteID)
            Catch ex As Exception
            End Try
        End If
    End Sub

    Protected Sub rgContacts_ItemDataBound(source As Object, e As GridItemEventArgs)
        If e.Item.IsInEditMode Then
            Dim item As GridEditableItem = DirectCast(e.Item, GridEditableItem)

            Dim ddlEmergContacts As DropDownList = DirectCast(item.FindControl("ddlEmergContacts"), DropDownList)
            ddlEmergContacts.DataSource = w.GetContacts(w.ID)
            ddlEmergContacts.DataBind()

            Dim hlEmergInfoLink As HyperLink = DirectCast(item.FindControl("hlEmergInfo2"), HyperLink)
            hlEmergInfoLink.NavigateUrl = EmergInfoLink
        End If
    End Sub

    Protected Sub rgContacts_InsertCommand(sender As Object, e As GridCommandEventArgs)
        Dim item As GridEditableItem = TryCast(e.Item, GridEditableItem)
        Dim c As EmergContact = New EmergContact(0)

        Try
            c.ID = TryCast(item.FindControl("ddlEmergContacts"), DropDownList).SelectedValue
        Catch ex As Exception
            DisplayHospitalMessage(True, "You must select a contact!")
            e.Canceled = True
            Exit Sub
        End Try

        Try
            Dim insert_results As String = c.AddContactToSite(s.SHASiteID)

            If insert_results <> "success" Then
                DisplayContactMessage(True, insert_results)
            Else
                DisplayContactMessage(False, "The emergency contact was added!")
            End If
        Catch ex As Exception
            DisplayContactMessage(True, "Unable to add contact. Reason: " + ex.Message)
            e.Canceled = True
        End Try

        ltlNotice2.Text = ""
    End Sub

    Private Sub DisplayContactMessage(isError As Boolean, text As String)
        If isError Then
            ltlNotice1.Text = "<p style='color:red;font-weight:bold;'>" & text & "</p>"
        Else
            ltlNotice1.Text = "<p style='color:green;font-weight:bold;'>" & text & "</p>"
        End If
    End Sub
#End Region

#Region "Hospitals"
    Protected Sub rgHospitals_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs)
        rgHospitals.DataSource = s.GetSiteHospitals(s.ID)
    End Sub

    Protected Sub rgHospitals_PreRender(sender As Object, e As EventArgs)
        Dim menu As GridFilterMenu = rgHospitals.FilterMenu
        Dim i As Integer = 0
        While i < menu.Items.Count
            If menu.Items(i).Text = "NoFilter" Or menu.Items(i).Text = "Contains" Or menu.Items(i).Text = "EqualTo" Or menu.Items(i).Text = "DoesNotContain" Then
                i = i + 1
            Else
                menu.Items.RemoveAt(i)
            End If
        End While
    End Sub

    Protected Sub rgHospitals_ItemCreated(ByVal sender As Object, ByVal e As GridItemEventArgs)
        If TypeOf e.Item Is GridDataItem Then
            Try
                Dim deleteLink As HyperLink = DirectCast(e.Item.FindControl("hlDelete"), HyperLink)
                deleteLink.Attributes("href") = "#"
                deleteLink.Attributes("onclick") = [String].Format("return ShowDeleteHospitalForm('{0}','{1}','{2}');", e.Item.OwnerTableView.DataKeyValues(e.Item.ItemIndex)("hospital_id"), e.Item.ItemIndex, s.SHASiteID)
            Catch ex As Exception
            End Try
        End If
    End Sub

    Protected Sub rgHospitals_ItemDataBound(source As Object, e As GridItemEventArgs)
        If e.Item.IsInEditMode Then
            Dim item As GridEditableItem = DirectCast(e.Item, GridEditableItem)

            Dim ddlHospitals As DropDownList = DirectCast(item.FindControl("ddlHospitals"), DropDownList)
            ddlHospitals.DataSource = w.GetHospitals(w.ID)
            ddlHospitals.DataBind()

            Dim hlEmergInfoLink As HyperLink = DirectCast(item.FindControl("hlEmergInfo3"), HyperLink)
            hlEmergInfoLink.NavigateUrl = EmergInfoLink
        End If
    End Sub

    Protected Sub rgHospitals_InsertCommand(sender As Object, e As GridCommandEventArgs)
        Dim item As GridEditableItem = TryCast(e.Item, GridEditableItem)
        Dim h As Hospital = New Hospital(0)

        Try
            h.ID = TryCast(item.FindControl("ddlHospitals"), DropDownList).SelectedValue
        Catch ex As Exception
            DisplayHospitalMessage(True, "You must select a hospital!")
            e.Canceled = True
            Exit Sub
        End Try

        Try
            Dim insert_results As String = h.AddHospitalToSite(s.SHASiteID)

            If insert_results <> "success" Then
                DisplayHospitalMessage(True, insert_results)
            Else
                DisplayHospitalMessage(False, "The hospital was added!")
            End If
        Catch ex As Exception
            DisplayHospitalMessage(True, "Unable to add hospital. Reason: " + ex.Message)
            e.Canceled = True
        End Try

        ltlNotice1.Text = ""
    End Sub

    Private Sub DisplayHospitalMessage(isError As Boolean, text As String)
        If isError Then
            ltlNotice2.Text = "<p style='color:red;font-weight:bold;'>" & text & "</p>"
        Else
            ltlNotice2.Text = "<p style='color:green;font-weight:bold;'>" & text & "</p>"
        End If
    End Sub
#End Region

#End Region

#Region "Admin Panel"
    Protected Sub SetupAdminPanel()
        ltlReviewedBy.Text = s.SHAReviewedBy
        ltlReviewerComments.Text = s.ReviewerComments
        If s.SHAReviewedDate.ToString("MM/dd/yyyy") = "01/01/1900" Then
            ltlReviewedDate.Text = "never reviewed"
        Else
            ltlReviewedDate.Text = s.SHAReviewedDate.ToString("MM/dd/yyyy")
        End If

        ltlApprovedBy.Text = s.SHAApprovedBy
        If s.SHAApprovedDate.ToString("MM/dd/yyyy") = "01/01/1900" Then
            ltlApprovedDate.Text = "never approved"
        Else
            ltlApprovedDate.Text = s.SHAApprovedDate.ToString("MM/dd/yyyy")
        End If

        If u.ApproverValue And s.SHAReviewedDate > s.SHAApprovedDate Then
            pnlApprove.Visible = True
        End If
    End Sub

    Protected Sub lbReview_Command(ByVal sender As Object, ByVal e As CommandEventArgs)
        pnlReview.Visible = False
        pnlReviewSubmit.Visible = True
    End Sub

    Protected Sub lbApprove_Command(ByVal sender As Object, ByVal e As CommandEventArgs)
        pnlApprovePreSubmit.Visible = False
        pnlApproveSubmit.Visible = True
    End Sub

    Protected Sub btnAdmin_Command(ByVal sender As Object, ByVal e As CommandEventArgs)
        If e.CommandArgument = "Reviewed" Then
            If Page.IsValid Then
                s.ReviewSHA(u.ID, tbReviewerComments.Text)

                ltlReviewedBy.Text = u.ID
                ltlReviewedDate.Text = DateTime.Now
                ltlReviewerComments.Text = tbReviewerComments.Text

                pnlReview.Visible = True
                pnlReviewSubmit.Visible = False
                If u.ApproverValue Then
                    pnlApprove.Visible = True
                End If
            End If
        ElseIf e.CommandArgument = "Approved" Then
            s.ApproveSHA(u.ID)

            ltlApprovedBy.Text = u.ID
            ltlApprovedDate.Text = DateTime.Now

            pnlApprovePreSubmit.Visible = True
            pnlApproveSubmit.Visible = False
        ElseIf e.CommandArgument = "CancelReview" Then
            pnlReview.Visible = True
            pnlReviewSubmit.Visible = False
        ElseIf e.CommandArgument = "CancelApprove" Then
            pnlApprovePreSubmit.Visible = True
            pnlApproveSubmit.Visible = False
        End If
    End Sub
#End Region

End Class