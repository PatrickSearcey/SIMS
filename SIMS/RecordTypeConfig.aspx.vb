Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.HttpContext
Imports System.Web.HttpRequest
Imports System.Net
Imports System.Web.UI.WebControls
Imports Telerik.Web.UI


Public Class RecordTypeConfig
    Inherits System.Web.UI.Page

    Private o As Office
    Private w As WSC
    Private uid As String = Current.User.Identity.Name
    Private u As New User(uid)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Master.PageTitle = "Record-Type Maintenance Interface"
        Page.Title = "Record-Type Maintenance Interface"

        Dim office_id As Integer = Request.QueryString("office_id")
        Dim wsc_id As Integer = Request.QueryString("wsc_id")
        Dim clientIP As String = Request.UserHostAddress

        '--REFERENCE OFFICE AND DISTRICT OBJECTS----------------------------------------------
        If office_id = Nothing And wsc_id = Nothing Then
            'If neither office_id nor wsc_id were passed in querystring, base them on user info
            wsc_id = u.WSCID
            office_id = u.GetUserIPOfficeID(clientIP)
        ElseIf Not office_id = Nothing Then
            'If office_id was passed, use office_id to find wsc_id
            o = New Office(office_id)
            wsc_id = o.WSCID
        End If
        w = New WSC(wsc_id)
        '--END SECTION---------------------------------------------------------------------

        Master.SiteNumberLabel = "Record-Types for the " & w.Name & " WSC"

        If Not Page.IsPostBack Then
            '--PAGE ACCESS SECTION-------------------------------------------------------------
            Master.CheckAccessLevel(wsc_id, "None")
            If Master.NoAccessPanel Then
                pnlHasAccess.Visible = False
            Else
                pnlHasAccess.Visible = True
            End If

            '--END PAGE ACCESS SECTION---------------------------------------------------------
            Me.Session("RecordTypes") = Nothing
        End If
    End Sub

    Public ReadOnly Property RecordTypes() As DataTable
        Get
            Dim obj As Object = Me.Session("RecordTypes")
            If (Not obj Is Nothing) Then
                Return CType(obj, DataTable)
            End If

            Dim myDataTable As DataTable = New DataTable
            myDataTable = w.GetRecordTypeList(w.ID, "all", "")
            Me.Session("RecordTypes") = myDataTable
            Return myDataTable
        End Get
    End Property

    Protected Sub rgRecordTypes_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles rgRecordTypes.Load
        If Not IsPostBack Then
            Dim access_level As String = u.AccessLevel
            Dim user_wsc_id As Integer = u.WSCID

            If access_level = "None" Then
                rgRecordTypes.Columns.FindByUniqueName("EditCommandColumn").Visible = False

                rgRecordTypes.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None
            ElseIf access_level = "WSC" Then
                Dim strpred As New Helpers(w.ID)

                Try
                    If user_wsc_id = w.ID Or Array.Exists(Of String)(u.GetUserExceptions(u.ID), AddressOf strpred.CompareTo) Then
                        rgRecordTypes.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.Top
                    Else
                        rgRecordTypes.Columns.FindByUniqueName("EditCommandColumn").Visible = False

                        rgRecordTypes.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None
                    End If
                Catch ex As Exception
                    Try
                        If Array.Exists(Of String)(u.GetWSCExceptions(u.PrimaryOU), AddressOf strpred.CompareTo) Then
                            rgRecordTypes.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.Top
                        Else
                            rgRecordTypes.Columns.FindByUniqueName("EditCommandColumn").Visible = False

                            rgRecordTypes.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None
                        End If
                    Catch ex1 As Exception
                        If user_wsc_id = w.ID Then
                            rgRecordTypes.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.Top
                        Else
                            rgRecordTypes.Columns.FindByUniqueName("EditCommandColumn").Visible = False

                            rgRecordTypes.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None
                        End If
                    End Try
                End Try

            Else
                rgRecordTypes.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.Top
            End If
        End If
    End Sub

    Protected Sub rgRecordTypes_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs) Handles rgRecordTypes.NeedDataSource
        Me.rgRecordTypes.DataSource = Me.RecordTypes
        Me.RecordTypes.PrimaryKey = New DataColumn() {Me.RecordTypes.Columns("record_type_id")}
    End Sub

    Private Sub rgRecordTypes_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles rgRecordTypes.PreRender
        If Not IsPostBack Then

        End If
    End Sub

    Private Sub rgRecordTypes_UpdateCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles rgRecordTypes.UpdateCommand
        Dim editedItem As GridEditableItem = CType(e.Item, GridEditableItem)
        Dim MyUserControl As UserControl = CType(e.Item.FindControl(GridEditFormItem.EditFormUserControlID), UserControl)

        Dim rec_type As New RecordType(editedItem.OwnerTableView.DataKeyValues(editedItem.ItemIndex)("record_type_id"))
        rec_type.Code = CType(MyUserControl.FindControl("tbCode"), TextBox).Text
        rec_type.Description = CType(MyUserControl.FindControl("tbDescription"), TextBox).Text
        rec_type.WorkerInstructions = CType(MyUserControl.FindControl("tbWorkInst"), TextBox).Text
        rec_type.CheckerInstructions = CType(MyUserControl.FindControl("tbCheckInst"), TextBox).Text
        rec_type.ReviewerInstructions = CType(MyUserControl.FindControl("tbReviewInst"), TextBox).Text

        Try
            rec_type.UpdateRecordTypeDetails()
            Me.Session("RecordTypes") = Nothing
            rgRecordTypes.Rebind()
        Catch ex As Exception
            Dim lblError As Label = New Label()
            lblError.Text = "Unable to update record-type. Reason: " + ex.Message
            lblError.ForeColor = System.Drawing.Color.Red
            rgRecordTypes.Controls.Add(lblError)

            e.Canceled = True
        End Try
    End Sub

    Protected Sub rgRecordTypes_InsertCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles rgRecordTypes.InsertCommand
        Dim userControl As UserControl = CType(e.Item.FindControl(GridEditFormItem.EditFormUserControlID), UserControl)

        Dim rec_type As New RecordType(0)
        rec_type.Code = Trim(CType(userControl.FindControl("tbCode"), TextBox).Text)
        rec_type.Description = CType(userControl.FindControl("tbDescription"), TextBox).Text
        rec_type.WorkerInstructions = CType(userControl.FindControl("tbWorkInst"), TextBox).Text
        rec_type.CheckerInstructions = CType(userControl.FindControl("tbCheckInst"), TextBox).Text
        rec_type.ReviewerInstructions = CType(userControl.FindControl("tbReviewInst"), TextBox).Text
        rec_type.WSCID = w.ID
        Dim cont_va As String = Nothing

        Try
            cont_va = CType(userControl.FindControl("rblContorNoncont"), RadioButtonList).SelectedItem.Value
            If cont_va = "cont" Then
                rec_type.TimeSeriesFlag = True
            ElseIf cont_va = "noncont" Then
                rec_type.TimeSeriesFlag = False
            End If
        Catch ex As Exception
            Dim lblError As Label = New Label()
            lblError.Text = "Unable to insert record-type. Reason: you must select either the time-series or non-time-series box."
            lblError.ForeColor = System.Drawing.Color.Red
            rgRecordTypes.Controls.Add(lblError)

            Exit Sub
        End Try


        Try
            Dim rec_type_exist_test As New RecordType(rec_type.Code, rec_type.WSCID)
            If rec_type_exist_test.ID = 0 Then
                rec_type.AddRecordType()
                Me.Session("RecordTypes") = Nothing
                rgRecordTypes.Rebind()
            Else
                CType(userControl.FindControl("lblError"), Label).ForeColor = System.Drawing.Color.Red
                CType(userControl.FindControl("lblError"), Label).Text = "Unable to insert record-type. Reason: a record-type with this code already exists"

                e.Canceled = True
            End If
        Catch ex As Exception

            Dim lblError As Label = New Label()
            lblError.Text = "Unable to insert record-type. Reason: " + ex.Message
            lblError.ForeColor = System.Drawing.Color.Red
            rgRecordTypes.Controls.Add(lblError)

            e.Canceled = True
        End Try
    End Sub

End Class
