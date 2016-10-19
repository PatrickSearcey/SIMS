Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb
Imports System.Web.HttpContext
Imports Telerik.Web.UI

Public Class EmergencyInfo
    Inherits System.Web.UI.Page

    Private w As WSC
    Private o As Office
    Private uid As String = Current.User.Identity.Name
    Private u As New User(uid)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim wsc_id As Integer = 0
        Dim office_id As Integer = Nothing

        If Request.QueryString("wsc_id") <> "" Then
            wsc_id = Request.QueryString("wsc_id")
        ElseIf Request.QueryString("office_id") <> "" Then
            office_id = Request.QueryString("office_id")
            o = New Office(office_id)
            wsc_id = o.WSCID
        End If

        w = New WSC(wsc_id)

        Master.PageTitle = "SIMS Emergency Information Management Interface for the " & w.Name & " WSC"
        Master.ResponsibleOffice = False
        Page.Title = "SIMS - Emergency Information Management Interface"

        If Not Page.IsPostBack Then
            '--PAGE ACCESS SECTION-------------------------------------------------------------
            Master.CheckAccessLevel(wsc_id, "WSC")
            If Master.NoAccessPanel Then
                pnlHasAccess.Visible = False
            Else
                pnlHasAccess.Visible = True
            End If
            '--END PAGE ACCESS SECTION---------------------------------------------------------
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

#Region "Hospitals"
    Private ReadOnly Property Hospitals() As DataTable
        Get
            Dim dt As New DataTable()
            dt = w.GetHospitals(w.ID)
            Return dt
        End Get
    End Property

    Protected Sub rgHospitals_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs)
        rgHospitals.DataSource = Me.Hospitals
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
            Dim deleteLink As HyperLink = DirectCast(e.Item.FindControl("hlDelete"), HyperLink)
            deleteLink.Attributes("href") = "#"
            deleteLink.Attributes("onclick") = [String].Format("return ShowDeleteHospitalForm('{0}','{1}');", e.Item.OwnerTableView.DataKeyValues(e.Item.ItemIndex)("hospital_id"), e.Item.ItemIndex)
        End If
    End Sub

    Protected Sub rgHospitals_ItemCommand(source As Object, e As GridCommandEventArgs)
        If e.CommandName = RadGrid.InitInsertCommandName Then
            '"Add new" button clicked
            Dim editColumn As GridEditCommandColumn = DirectCast(rgHospitals.MasterTableView.GetColumn("EditCommandColumn"), GridEditCommandColumn)
            editColumn.Visible = False
        ElseIf e.CommandName = RadGrid.RebindGridCommandName AndAlso e.Item.OwnerTableView.IsItemInserted Then
            e.Canceled = True
        Else
            Dim editColumn As GridEditCommandColumn = DirectCast(rgHospitals.MasterTableView.GetColumn("EditCommandColumn"), GridEditCommandColumn)
            If Not editColumn.Visible Then
                editColumn.Visible = True
            End If
        End If
    End Sub

    Protected Sub rgHospitals_InsertCommand(sender As Object, e As GridCommandEventArgs)
        Dim item As GridEditableItem = TryCast(e.Item, GridEditableItem)
        Dim h As Hospital = New Hospital(0)

        Try
            h.Name = TryCast(item.FindControl("tbHospitalNm"), TextBox).Text
            h.StreetAddress = TryCast(item.FindControl("tbStreetAddrs1"), TextBox).Text
            h.City = TryCast(item.FindControl("tbCity1"), TextBox).Text
            h.State = TryCast(item.FindControl("tbState1"), TextBox).Text
            h.Zip = TryCast(item.FindControl("tbZip1"), TextBox).Text
            h.PhoneNo = TryCast(item.FindControl("tbPhoneNo1"), TextBox).Text
            h.Latitude = Convert.ToDouble(TryCast(item.FindControl("tbLat"), TextBox).Text)
            h.Longitude = Convert.ToDouble(TryCast(item.FindControl("tbLong"), TextBox).Text)
            h.WSCID = w.ID
        Catch ex As Exception
            DisplayHospitalMessage(True, "You must complete all form fields!")
            e.Canceled = True
            Exit Sub
        End Try

        Try
            Dim insert_results As String = h.AddHospital()

            If insert_results <> "success" Then
                DisplayHospitalMessage(True, insert_results)
            Else
                DisplayHospitalMessage(False, "The hospital was added!")
            End If
        Catch ex As Exception
            DisplayHospitalMessage(True, "Unable to add hospital. Reason: " + ex.Message)
            e.Canceled = True
        End Try
    End Sub

    Protected Sub rgHospitals_UpdateCommand(sender As Object, e As GridCommandEventArgs)
        Dim item As GridEditableItem = TryCast(e.Item, GridEditableItem)
        Dim hospital_id As String = item.GetDataKeyValue("hospital_id").ToString()
        Dim h As Hospital = New Hospital(hospital_id)

        Try
            h.Name = TryCast(item.FindControl("tbHospitalNm"), TextBox).Text
            h.StreetAddress = TryCast(item.FindControl("tbStreetAddrs1"), TextBox).Text
            h.City = TryCast(item.FindControl("tbCity1"), TextBox).Text
            h.State = TryCast(item.FindControl("tbState1"), TextBox).Text
            h.Zip = TryCast(item.FindControl("tbZip1"), TextBox).Text
            h.PhoneNo = TryCast(item.FindControl("tbPhoneNo1"), TextBox).Text
            h.Latitude = Convert.ToDouble(TryCast(item.FindControl("tbLat"), TextBox).Text)
            h.Longitude = Convert.ToDouble(TryCast(item.FindControl("tbLong"), TextBox).Text)
        Catch ex As Exception
            DisplayHospitalMessage(True, "You must complete all form fields!")
            e.Canceled = True
            Exit Sub
        End Try

        Try
            Dim update_results As String = h.UpdateHospitalDetails()

            If update_results <> "success" Then
                DisplayHospitalMessage(True, update_results)
            Else
                DisplayHospitalMessage(False, "The hospital was updated!")
            End If
        Catch ex As Exception
            DisplayHospitalMessage(True, "Unable to update hospital. Reason: " + ex.Message)

            e.Canceled = True
        End Try

    End Sub

    Private Sub DisplayHospitalMessage(isError As Boolean, text As String)
        If isError Then
            ltlNotice1.Text = "<p style='color:red;font-weight:bold;'>" & text & "</p>"
        Else
            ltlNotice1.Text = "<p style='color:green;font-weight:bold;'>" & text & "</p>"
        End If
    End Sub
#End Region

#Region "Contacts"
    Private ReadOnly Property Contacts() As DataTable
        Get
            Dim dt As New DataTable()
            dt = w.GetContacts(w.ID)
            Return dt
        End Get
    End Property

    Protected Sub rgContacts_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs)
        rgContacts.DataSource = Me.Contacts
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
            Dim deleteLink As HyperLink = DirectCast(e.Item.FindControl("hlDelete"), HyperLink)
            deleteLink.Attributes("href") = "#"
            deleteLink.Attributes("onclick") = [String].Format("return ShowDeleteContactForm('{0}','{1}');", e.Item.OwnerTableView.DataKeyValues(e.Item.ItemIndex)("contact_id"), e.Item.ItemIndex)
        End If
    End Sub

    Protected Sub rgContacts_ItemCommand(source As Object, e As GridCommandEventArgs)
        If e.CommandName = RadGrid.InitInsertCommandName Then
            '"Add new" button clicked
            Dim editColumn As GridEditCommandColumn = DirectCast(rgContacts.MasterTableView.GetColumn("EditCommandColumn"), GridEditCommandColumn)
            editColumn.Visible = False
        ElseIf e.CommandName = RadGrid.RebindGridCommandName AndAlso e.Item.OwnerTableView.IsItemInserted Then
            e.Canceled = True
        Else
            Dim editColumn As GridEditCommandColumn = DirectCast(rgContacts.MasterTableView.GetColumn("EditCommandColumn"), GridEditCommandColumn)
            If Not editColumn.Visible Then
                editColumn.Visible = True
            End If
        End If
    End Sub

    Protected Sub rgContacts_InsertCommand(sender As Object, e As GridCommandEventArgs)
        Dim item As GridEditableItem = TryCast(e.Item, GridEditableItem)
        Dim c As EmergContact = New EmergContact(0)

        Try
            c.Name = TryCast(item.FindControl("tbContactNm"), TextBox).Text
            c.StreetAddress = TryCast(item.FindControl("tbStreetAddrs2"), TextBox).Text
            c.City = TryCast(item.FindControl("tbCity2"), TextBox).Text
            c.State = TryCast(item.FindControl("tbState2"), TextBox).Text
            c.Zip = TryCast(item.FindControl("tbZip2"), TextBox).Text
            c.PhoneNo = TryCast(item.FindControl("tbPhoneNo2"), TextBox).Text
            c.WSCID = w.ID
        Catch ex As Exception
            DisplayHospitalMessage(True, "You must complete all required form fields!")
            e.Canceled = True
            Exit Sub
        End Try

        Try
            Dim insert_results As String = c.AddContact()

            If insert_results <> "success" Then
                DisplayContactMessage(True, insert_results)
            Else
                DisplayContactMessage(False, "The emergency contact was added!")
            End If
        Catch ex As Exception
            DisplayContactMessage(True, "Unable to add contact. Reason: " + ex.Message)
            e.Canceled = True
        End Try
    End Sub

    Protected Sub rgContacts_UpdateCommand(sender As Object, e As GridCommandEventArgs)
        Dim item As GridEditableItem = TryCast(e.Item, GridEditableItem)
        Dim contact_id As String = item.GetDataKeyValue("contact_id").ToString()
        Dim c As EmergContact = New EmergContact(contact_id)

        Try
            c.Name = TryCast(item.FindControl("tbContactNm"), TextBox).Text
            c.StreetAddress = TryCast(item.FindControl("tbStreetAddrs2"), TextBox).Text
            c.City = TryCast(item.FindControl("tbCity2"), TextBox).Text
            c.State = TryCast(item.FindControl("tbState2"), TextBox).Text
            c.Zip = TryCast(item.FindControl("tbZip2"), TextBox).Text
            c.PhoneNo = TryCast(item.FindControl("tbPhoneNo2"), TextBox).Text
        Catch ex As Exception
            DisplayContactMessage(True, "You must complete all required form fields!")
            e.Canceled = True
            Exit Sub
        End Try

        Try
            Dim update_results As String = c.UpdateContactDetails()

            If update_results <> "success" Then
                DisplayContactMessage(True, update_results)
            Else
                DisplayContactMessage(False, "The contact was updated!")
            End If
        Catch ex As Exception
            DisplayContactMessage(True, "Unable to update contact. Reason: " + ex.Message)

            e.Canceled = True
        End Try

    End Sub

    Private Sub DisplayContactMessage(isError As Boolean, text As String)
        If isError Then
            ltlNotice2.Text = "<p style='color:red;font-weight:bold;'>" & text & "</p>"
        Else
            ltlNotice2.Text = "<p style='color:green;font-weight:bold;'>" & text & "</p>"
        End If
    End Sub
#End Region
End Class
