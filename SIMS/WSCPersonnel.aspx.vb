Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.HttpContext
Imports System.Web.HttpRequest
Imports System.Net
Imports System.Web.UI.WebControls
Imports Telerik.Web.UI
Imports System.IO

Public Class WSCPersonnel
    Inherits System.Web.UI.Page

    Private o As Office
    Private w As WSC
    Private uid As String = Current.User.Identity.Name
    Private u As New User(uid)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Page.Title = "Manage Personnel"
        Master.ResponsibleOffice = False

        Dim office_id As Integer = Request.QueryString("office_id")
        Dim wsc_id As Integer = Request.QueryString("wsc_id")
        Dim clientIP As String = Request.UserHostAddress

        '--REFERENCE OFFICE AND DISTRICT OBJECTS----------------------------------------------
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

        Master.PageTitle = "Manage Personnel for the " & w.Name & " Water Science Center"

        If Not Page.IsPostBack Then
            '--PAGE ACCESS SECTION-------------------------------------------------------------
            Master.CheckAccessLevel(wsc_id, "None")
            If Master.NoAccessPanel Then
                pnlHasAccess.Visible = False
            Else
                pnlHasAccess.Visible = True

                If u.AccessLevel = "None" Then
                    Session("CanEdit") = "false"
                    rgPersonnel.MasterTableView.CommandItemDisplay = False
                Else
                    Session("CanEdit") = "true"
                End If
            End If
            '--END PAGE ACCESS SECTION---------------------------------------------------------

            Session("ShowActiveOnly") = "True"

            If Session("ShowActiveOnly") = "False" Then
                Session("ShowActiveOnly") = "False"
                lbToggleActive.Text = "Click to view only active users"
                lbToggleActive.CommandArgument = "ActiveOnly"
            ElseIf Session("ShowActiveOnly") = "True" Then
                Session("ShowActiveOnly") = "True"
                lbToggleActive.Text = "Click to view all users"
                lbToggleActive.CommandArgument = "ShowAll"
            End If
        End If
    End Sub

#Region "Properties"
    Private ReadOnly Property Levels() As DataTable
        Get
            Dim where_stmt As String = " WHERE administrator_va <> 'SuperUser'"
            If u.AccessLevel = "SuperUser" Then
                where_stmt = ""
            End If
            Dim sql As String = "SELECT administrator_va AS level FROM tblEmployees" & where_stmt & " GROUP BY administrator_va ORDER BY level"
            Dim adapter As New SqlDataAdapter(sql, Config.ConnectionInfo)
            Dim dt As New DataTable()
            adapter.Fill(dt)
            Levels = dt
        End Get
    End Property
#End Region

    Public Sub lbToggleActive_Command(ByVal source As Object, ByVal e As CommandEventArgs) Handles lbToggleActive.Command
        If e.CommandArgument = "ActiveOnly" Then
            Session("ShowActiveOnly") = "True"
            lbToggleActive.Text = "Click to view all users"
            lbToggleActive.CommandArgument = "ShowAll"
        ElseIf e.CommandArgument = "ShowAll" Then
            Session("ShowActiveOnly") = "False"
            lbToggleActive.Text = "Click to view only active users"
            lbToggleActive.CommandArgument = "ActiveOnly"
        End If

        rgPersonnel.Rebind()
    End Sub

    Private Sub rgPersonnel_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs) Handles rgPersonnel.NeedDataSource
        If Session("ShowActiveOnly") = "False" Then
            rgPersonnel.DataSource = w.GetPersonnel(w.ID, "False", "yes")
        Else
            rgPersonnel.DataSource = w.GetPersonnel(w.ID, "True", "yes")
        End If

        If (Not Page.IsPostBack) Then
            Try
                rgPersonnel.MasterTableView.FilterExpression = "([office_nm] Like '%" & o.Name & "%')"
                Dim column As GridColumn = rgPersonnel.MasterTableView.GetColumnSafe("office_nm")
                column.CurrentFilterFunction = GridKnownFunction.Contains
                column.CurrentFilterValue = o.Name
            Catch ex As Exception
            End Try
        End If
    End Sub

    Protected Sub rgPersonnel_PreRender(sender As Object, e As EventArgs)
        Dim menu As GridFilterMenu = rgPersonnel.FilterMenu
        Dim i As Integer = 0
        While i < menu.Items.Count
            If menu.Items(i).Text = "NoFilter" Or menu.Items(i).Text = "Contains" Or menu.Items(i).Text = "EqualTo" Or menu.Items(i).Text = "DoesNotContain" Then
                i = i + 1
            Else
                menu.Items.RemoveAt(i)
            End If
        End While
    End Sub

    Protected Sub rgPersonnel_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs)
        If e.Item.IsInEditMode Then
            Dim item As GridEditableItem = DirectCast(e.Item, GridEditableItem)

            Dim pnlUpdate As Panel = DirectCast(item.FindControl("pnlUpdate"), Panel)
            Dim pnlInsert1 As Panel = DirectCast(item.FindControl("pnlInsert1"), Panel)
            Dim pnlInsert2 As Panel = DirectCast(item.FindControl("pnlInsert2"), Panel)
            Dim tbUserID As TextBox = DirectCast(item.FindControl("tbUserID"), TextBox)
            Dim tbFirstNm As TextBox = DirectCast(item.FindControl("tbFirstNm"), TextBox)
            Dim tbLastNm As TextBox = DirectCast(item.FindControl("tbLastNm"), TextBox)
            Dim ddlAdminLevel As DropDownList = DirectCast(item.FindControl("ddlAdminLevel"), DropDownList)
            Dim ddlPASSLevel As DropDownList = DirectCast(item.FindControl("ddlPASSLevel"), DropDownList)
            Dim rblSafetyApprover As RadioButtonList = DirectCast(item.FindControl("rblSafetyApprover"), RadioButtonList)
            Dim ddlOffice As DropDownList = DirectCast(item.FindControl("ddlOffice"), DropDownList)
            Dim rblStatus As RadioButtonList = DirectCast(item.FindControl("rblStatus"), RadioButtonList)
            Dim cbReports As CheckBox = DirectCast(item.FindControl("cbReports"), CheckBox)
            Dim ddlAdminLevel2 As DropDownList = DirectCast(item.FindControl("ddlAdminLevel2"), DropDownList)
            Dim ddlPASSLevel2 As DropDownList = DirectCast(item.FindControl("ddlPASSLevel2"), DropDownList)
            Dim ddlOffice2 As DropDownList = DirectCast(item.FindControl("ddlOffice2"), DropDownList)
            Dim rblSafetyApprover2 As RadioButtonList = DirectCast(item.FindControl("rblSafetyApprover2"), RadioButtonList)
            Dim rblStatus2 As RadioButtonList = DirectCast(item.FindControl("rblStatus2"), RadioButtonList)

            If Not (TypeOf e.Item Is IGridInsertItem) Then
                ddlAdminLevel.DataSource = Levels
                ddlPASSLevel.DataSource = Levels
                ddlOffice.DataSource = w.GetOfficeList(w.ID)
                ddlAdminLevel.DataBind()
                ddlPASSLevel.DataBind()
                ddlOffice.DataBind()

                tbUserID.Text = DirectCast(e.Item.DataItem, DataRowView)("user_id").ToString()
                tbFirstNm.Text = DirectCast(e.Item.DataItem, DataRowView)("first_nm").ToString()
                tbLastNm.Text = DirectCast(e.Item.DataItem, DataRowView)("last_nm").ToString()
                ddlAdminLevel.SelectedValue = DirectCast(e.Item.DataItem, DataRowView)("administrator_va").ToString()
                ddlPASSLevel.SelectedValue = DirectCast(e.Item.DataItem, DataRowView)("pass_access").ToString()
                Dim sa As String = DirectCast(e.Item.DataItem, DataRowView)("approver_va").ToString()
                Select Case sa
                    Case "True"
                        rblSafetyApprover.SelectedValue = "yes"
                    Case "False"
                        rblSafetyApprover.SelectedValue = "no"
                End Select
                ddlOffice.SelectedValue = DirectCast(e.Item.DataItem, DataRowView)("office_cd").ToString()
                Dim st As String = DirectCast(e.Item.DataItem, DataRowView)("active").ToString()
                Select Case st
                    Case "True"
                        rblStatus.SelectedValue = "true"
                    Case "False"
                        rblStatus.SelectedValue = "false"
                End Select
                cbReports.Checked = DirectCast(e.Item.DataItem, DataRowView)("show_reports").ToString()

                pnlUpdate.Visible = True
                pnlInsert1.Visible = False
                pnlInsert2.Visible = False
            ElseIf (TypeOf e.Item Is IGridInsertItem) Then
                ddlAdminLevel2.DataSource = Levels
                ddlPASSLevel2.DataSource = Levels
                ddlOffice2.DataSource = w.GetOfficeList(w.ID)
                ddlAdminLevel2.DataBind()
                ddlPASSLevel2.DataBind()
                ddlOffice2.DataBind()
                rblSafetyApprover2.SelectedValue = "no"
                rblStatus2.SelectedValue = "true"

                pnlUpdate.Visible = False
                pnlInsert1.Visible = True
                pnlInsert2.Visible = False
            End If

            
        End If

        If TypeOf e.Item Is GridDataItem Then
            Dim item As GridDataItem = DirectCast(e.Item, GridDataItem)
            Try
                If Session("CanEdit").ToString = "false" Then
                    Dim ib3 As ImageButton = DirectCast(item("EditCommandColumn").Controls(0), ImageButton)
                    ib3.Visible = False
                End If
            Catch ex As Exception
            End Try
        End If

    End Sub

    Protected Sub rgPersonnel_ItemCommand(sender As Object, e As GridCommandEventArgs)
        Dim item As GridEditableItem = TryCast(e.Item, GridEditableItem)

        If e.CommandName = "ValidateUser" Then
            Dim pnlInsert1 As Panel = DirectCast(item.FindControl("pnlInsert1"), Panel)
            Dim pnlInsert2 As Panel = DirectCast(item.FindControl("pnlInsert2"), Panel)
            Dim tbUserID2 As TextBox = DirectCast(item.FindControl("tbUserID2"), TextBox)
            Dim tbFirstNm As TextBox = DirectCast(item.FindControl("tbFirstNm2"), TextBox)
            Dim tbLastNm As TextBox = DirectCast(item.FindControl("tbLastNm2"), TextBox)
            Dim tbUserID3 As TextBox = DirectCast(item.FindControl("tbUserID3"), TextBox)

            Dim WSC As String = u.GetUserWSCID(tbUserID2.Text)

            Dim sql As String = "SELECT user_id FROM tblEmployees WHERE user_id = '" & tbUserID2.Text & "'"
            Dim adapter As New SqlDataAdapter(sql, Config.ConnectionInfo)
            Dim dt As New DataTable()
            adapter.Fill(dt)

            If CInt(WSC) <> w.ID Then
                DisplayMessage(True, "This user belongs to another WSC. Please contact GS-W Help SIMS@usgs.gov for assistance.")
                pnlInsert1.Visible = True
                pnlInsert2.Visible = False
                tbUserID2.Text = ""
            ElseIf dt.Rows.Count > 0 Then
                DisplayMessage(True, "This user is already in the SIMS database. If the user you are trying to add is moving from a different WSC, their PrimaryOU information in Active Directory must first be changed. Then, an email should be sent to GS-W Help SIMS for help with adding them to your WSC's personnel list.")
                pnlInsert1.Visible = True
                pnlInsert2.Visible = False
                tbUserID2.Text = ""
            Else
                Dim name As String = u.GetUserNameFromAD(tbUserID2.Text)
                tbFirstNm.Text = Mid(name, 1, InStr(name, ",") - 1)
                tbLastNm.Text = Mid(name, InStr(name, ",") + 1)
                tbUserID3.Text = tbUserID2.Text

                pnlInsert1.Visible = False
                pnlInsert2.Visible = True
            End If
        End If

    End Sub

    Protected Sub rgPersonnel_UpdateCommand(sender As Object, e As GridCommandEventArgs)
        Dim item As GridEditableItem = TryCast(e.Item, GridEditableItem)

        Dim user_id As String = item.GetDataKeyValue("user_id")
        Dim updateUsr As New User(user_id)

        Dim newValues As New Hashtable()

        Dim office_cd As String = TryCast(item.FindControl("ddlOffice"), DropDownList).SelectedValue.ToString()
        Dim newOffice As Office = New Office(office_cd)

        newValues("first_nm") = TryCast(item.FindControl("tbFirstNm"), TextBox).Text
        newValues("last_nm") = TryCast(item.FindControl("tbLastNm"), TextBox).Text
        newValues("office_id") = newOffice.ID
        newValues("administrator_va") = TryCast(item.FindControl("ddlAdminLevel"), DropDownList).SelectedValue.ToString()
        newValues("pass_access") = TryCast(item.FindControl("ddlPASSLevel"), DropDownList).SelectedValue.ToString()
        newValues("approver_va") = TryCast(item.FindControl("rblSafetyApprover"), RadioButtonList).SelectedValue.ToString()
        newValues("active") = TryCast(item.FindControl("rblStatus"), RadioButtonList).SelectedValue.ToString()
        newValues("show_reports") = TryCast(item.FindControl("cbReports"), CheckBox).Checked.ToString()

        Try
            For Each entry As DictionaryEntry In newValues
                Select Case DirectCast(entry.Key, String)
                    Case "first_nm"
                        updateUsr.FirstName = DirectCast(entry.Value, String)
                        Exit Select
                    Case "last_nm"
                        updateUsr.LastName = DirectCast(entry.Value, String)
                        Exit Select
                    Case "office_id"
                        updateUsr.OfficeID = DirectCast(entry.Value, Integer)
                        Exit Select
                    Case "administrator_va"
                        updateUsr.AccessLevel = DirectCast(entry.Value, String)
                        Exit Select
                    Case "pass_access"
                        updateUsr.PASSAccess = DirectCast(entry.Value, String)
                        Exit Select
                    Case "approver_va"
                        If entry.Value = "yes" Then
                            updateUsr.ApproverValue = True
                        Else
                            updateUsr.ApproverValue = False
                        End If
                        Exit Select
                    Case "active"
                        If entry.Value = "true" Then
                            updateUsr.Active = True
                        Else
                            updateUsr.Active = False
                        End If
                        Exit Select
                    Case "show_reports"
                        updateUsr.ShowReports = entry.Value
                        Exit Select
                End Select
            Next

            Dim updateStatus As String = updateUsr.UpdateUserInfo(user_id)
            If updateStatus = "success" Then
                DisplayMessage(False, "The user's info was updated!")
            Else
                DisplayMessage(True, "Unable to update user's info. Reason: " + updateStatus)
            End If

        Catch ex As Exception
            DisplayMessage(True, "Unable to update user's info. Reason: " + ex.Message)

            e.Canceled = True
        End Try

    End Sub

    Protected Sub rgPersonnel_InsertCommand(sender As Object, e As GridCommandEventArgs)
        Dim item As GridEditableItem = TryCast(e.Item, GridEditableItem)

        Dim newUsr As New User(0)

        Try
            Dim office_cd As String = TryCast(item.FindControl("ddlOffice2"), DropDownList).SelectedValue.ToString()
            Dim newOffice As Office = New Office(office_cd)

            newUsr.ID = TryCast(item.FindControl("tbUserID3"), TextBox).Text
            newUsr.FirstName = TryCast(item.FindControl("tbFirstNm2"), TextBox).Text
            newUsr.LastName = TryCast(item.FindControl("tbLastNm2"), TextBox).Text
            newUsr.OfficeID = newOffice.ID
            newUsr.AccessLevel = TryCast(item.FindControl("ddlAdminLevel2"), DropDownList).SelectedValue.ToString()
            newUsr.PASSAccess = TryCast(item.FindControl("ddlPASSLevel2"), DropDownList).SelectedValue.ToString()
            Dim approver_va As String = TryCast(item.FindControl("rblSafetyApprover2"), RadioButtonList).SelectedValue.ToString()
            If approver_va = "yes" Then
                newUsr.ApproverValue = True
            Else
                newUsr.ApproverValue = False
            End If
            Dim active As String = TryCast(item.FindControl("rblStatus2"), RadioButtonList).SelectedValue.ToString()
            If active = "true" Then
                newUsr.Active = True
            Else
                newUsr.Active = False
            End If
            newUsr.ShowReports = TryCast(item.FindControl("cbReports2"), CheckBox).Checked
        Catch ex As Exception
        End Try

        Try
            Dim insert_results As String = newUsr.InsertUserInfo(newUsr.ID)

            If insert_results <> "success" Then
                DisplayMessage(True, insert_results)
            Else
                DisplayMessage(False, "The user was added!")
            End If
        Catch ex As Exception
            DisplayMessage(True, "Unable to insert user. Reason: " + ex.Message)
            e.Canceled = True
        End Try

    End Sub

    Private Sub DisplayMessage(isError As Boolean, text As String)
        Dim label As Label = If((isError), Me.lblError, Me.lblSuccess)
        label.Text = text
    End Sub
End Class