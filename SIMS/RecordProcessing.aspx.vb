Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.HttpContext
Imports System.Web.HttpRequest
Imports System.Net
Imports System.Web.UI.WebControls
Imports Telerik.Web.UI

Public Class RecordProcessing
    Inherits System.Web.UI.Page

    Private o As Office
    Private w As WSC
    Private r As Record
    Private s As Site
    Private uid As String = Current.User.Identity.Name
    Private u As New User(uid)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Master.PageTitle = "Records Processing Interface"
        Page.Title = "Records Processing Interface"
        Master.ShowPrevPageLink = False

        Dim office_id As Integer = Request.QueryString("office_id")
        Dim listtype As String = Request.QueryString("listtype")
        Dim lt As String = Nothing
        Dim clientIP As String = Request.UserHostAddress
        Dim wsc_id As Integer = 0
        Dim rms_record_id As Integer = Request.QueryString("rms_record_id")

        '--REFERENCE OFFICE AND DISTRICT OBJECTS----------------------------------------------
        If office_id = Nothing Then
            'If office_id was not passed in querystring, base them on user info
            wsc_id = u.WSCID
            office_id = u.GetUserIPOfficeID(clientIP)
        ElseIf Not office_id = Nothing Then
            'If office_id was passed, use office_id to find wsc_id
            o = New Office(office_id)
            wsc_id = o.WSCID
        End If
        w = New WSC(wsc_id)
        o = New Office(office_id)
        '--END SECTION---------------------------------------------------------------------

        If listtype = "work" Then
            lt = "worked"
        ElseIf listtype = "check" Then
            lt = "checked"
        ElseIf listtype = "review" Then
            lt = "reviewed"
        End If

        lblListType.Text = listtype

        If rms_record_id = Nothing Then
            r = New Record(0)

            PopulateQuickLinks(listtype)

            Master.SiteNumberLabel = "Records to be " & lt & " for " & o.Name
        Else
            r = New Record(rms_record_id)

            PopulateQuickLinks("all")

            Master.SiteNumberLabel = "Work Record"
        End If

        If Not Page.IsPostBack Then
            '--PAGE ACCESS SECTION-------------------------------------------------------------
            Master.CheckAccessLevel(wsc_id, "None")
            If Master.NoAccessPanel Then
                pnlHasAccess.Visible = False
            Else
                pnlHasAccess.Visible = True
            End If

            '--END PAGE ACCESS SECTION---------------------------------------------------------
            Me.Session("RecordsToWork") = Nothing
            Me.Session("RecordsToCheck") = Nothing
            Me.Session("RecordsToReview") = Nothing
            hfListType.Value = listtype

        End If
    End Sub

    Private Sub PopulateQuickLinks(ByVal listtype As String)
        Dim pOut As String = Nothing

        Select Case listtype
            Case "work"
                pOut = "<label>View records available for:</label><br />" & _
                    "<a href=""RecordLists.aspx?listtype=check&office_id=" & o.ID & """>checking</a> | " & _
                    "<a href=""RecordLists.aspx?listtype=review&office_id=" & o.ID & """>reviewing</a><br /><br />" & _
                    "<label><a href=""/doc/RecordsProcessingInterface.doc"">Learn more about this page</a></label>"
            Case "check"
                pOut = "<label>View records available for:</label><br />" & _
                    "<a href=""RecordLists.aspx?listtype=work&office_id=" & o.ID & """>working</a> | " & _
                    "<a href=""RecordLists.aspx?listtype=review&office_id=" & o.ID & """>reviewing</a><br /><br />" & _
                    "<label><a href=""/doc/RecordsProcessingInterface.doc"">Learn more about this page</a></label>"
            Case "review"
                pOut = "<label>View records available for:</label><br />" & _
                    "<a href=""RecordLists.aspx?listtype=work&office_id=" & o.ID & """>working</a> | " & _
                    "<a href=""RecordLists.aspx?listtype=check&office_id=" & o.ID & """>checking</a><br /><br />" & _
                    "<label><a href=""/doc/RecordsProcessingInterface.doc"">Learn more about this page</a></label>"
            Case "all"
                pOut = "<label>View all records available for:</label><br />" & _
                    "<a href=""RecordLists.aspx?listtype=work&office_id=" & o.ID & """>working</a> | " & _
                    "<a href=""RecordLists.aspx?listtype=check&office_id=" & o.ID & """>checking</a> | " & _
                    "<a href=""RecordLists.aspx?listtype=review&office_id=" & o.ID & """>reviewing</a><br /><br />" & _
                    "<label><a href=""/doc/RecordsProcessingInterface.doc"">Learn more about this page</a></label>"
        End Select

        ltlQuickLinks.Text = pOut
    End Sub

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
        Dim listtype As String = Request.QueryString("listtype")
        Dim office_id As Integer = Request.QueryString("office_id")
        Dim rms_record_id As Integer = Request.QueryString("rms_record_id")
        Dim src As String = Request.QueryString("src")

        r = New Record(rms_record_id)
        o = New Office(office_id)

        Dim grid As New GridView
        Dim dbHandler As EventHandler = New EventHandler(AddressOf gv_DataBound)

        Select Case listtype
            Case "work"
                grid.DataSource = RecordsToWork()
                grid.ID = "rgWork"
            Case "check"
                grid.DataSource = RecordsToCheck()
                grid.ID = "rgCheck"
            Case "review"
                grid.DataSource = RecordsToReview()
                grid.ID = "rgReview"
            Case Else
                grid.DataSource = RecordsToWork()
                grid.ID = "rgWork"
        End Select

        AddHandler grid.DataBound, dbHandler

        grid.AutoGenerateColumns = False

        '---CREATE THE SITE TOP LEVEL TABLE-------------------------------------------------------
        Dim templateColumn2 As New TemplateField()
        templateColumn2.ItemTemplate = New GridViewTemplate("lcks", u.ID, listtype)
        templateColumn2.HeaderText = "lcks"

        Dim templateColumn3 As New TemplateField()
        templateColumn3.ItemTemplate = New GridViewTemplate("site no", u.ID, listtype)
        templateColumn3.HeaderText = "site no"
        templateColumn3.SortExpression = "site_no"

        Dim boundColumn2 As New BoundField()
        boundColumn2.DataField = "station_nm"
        boundColumn2.HeaderText = "station nm"

        Dim templateColumn4 As New TemplateField()
        templateColumn4.ItemTemplate = New GridViewTemplate("record-type", u.ID, listtype & src)
        templateColumn4.HeaderText = "record-type"

        Dim boundColumn4 As New BoundField()
        boundColumn4.DataField = "category_no"
        boundColumn4.HeaderText = "cat no"

        Dim boundColumn9 As New BoundField()
        boundColumn9.DataField = "DaysSinceAging"
        boundColumn9.HeaderText = "days since last approved in ADAPS"

        Dim boundColumn6 As New BoundField()
        Dim boundColumn7 As New BoundField()
        Dim boundColumn8 As New BoundField()

        Select Case listtype
            Case "work"
                boundColumn6.DataField = "operator_va"
                boundColumn6.HeaderText = "assigned to"

                boundColumn7.DataField = "rework_status"
                boundColumn7.HeaderText = "rework status"

                boundColumn8.DataField = "last_worked_dt"
                boundColumn8.HeaderText = "worked through"
            Case "check"
                boundColumn6.DataField = "worked_by_uid"
                boundColumn6.HeaderText = "worked by"

                boundColumn7.DataField = "checkerassigned"
                boundColumn7.HeaderText = "assigned to"

                boundColumn8.DataField = "period_dt"
                boundColumn8.HeaderText = "period dt"
            Case "review"
                boundColumn6.DataField = "reviewerassigned"
                boundColumn6.HeaderText = "assigned to"

                boundColumn7.DataField = "reviewed_by_uid"
                boundColumn7.HeaderText = "prev. review by"

                boundColumn8.DataField = "period_dt"
                boundColumn8.HeaderText = "period dt"
        End Select

        templateColumn2.HeaderStyle.HorizontalAlign = HorizontalAlign.Center

        templateColumn3.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        templateColumn3.ItemStyle.Wrap = False

        templateColumn4.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        templateColumn4.ItemStyle.Wrap = False

        boundColumn2.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        boundColumn2.ItemStyle.Wrap = False

        boundColumn4.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        boundColumn4.ItemStyle.HorizontalAlign = HorizontalAlign.Center

        boundColumn6.HeaderStyle.HorizontalAlign = HorizontalAlign.Center

        boundColumn7.HeaderStyle.HorizontalAlign = HorizontalAlign.Center

        boundColumn8.ItemStyle.Wrap = False
        boundColumn8.HeaderStyle.HorizontalAlign = HorizontalAlign.Center

        boundColumn9.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        boundColumn9.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        boundColumn9.ItemStyle.Width = "110"

        grid.Columns.Add(templateColumn2)
        grid.Columns.Add(templateColumn3)
        grid.Columns.Add(boundColumn2)
        grid.Columns.Add(templateColumn4)
        grid.Columns.Add(boundColumn4)
        grid.Columns.Add(boundColumn6)
        grid.Columns.Add(boundColumn7)
        grid.Columns.Add(boundColumn8)
        grid.Columns.Add(boundColumn9)

        grid.DataKeyNames = New String() {"rms_record_id"}
        grid.AllowSorting = True
        grid.EnableViewState = True
        grid.AllowPaging = False

        grid.DataBind()

        phRecordsToProcess.Controls.Add(grid)
    End Sub

    Protected Sub gv_DataBound(ByVal sender As Object, ByVal e As EventArgs)
        'Dim recordCount As Integer = Nothing
        'Dim gvRow As GridDataItem = Nothing

        'If sender.MasterTableView.Items.Count > 1 Then
        '    For rowIndex As Integer = sender.MasterTableView.Items.Count - 2 To 0 Step -1
        '        gvRow = sender.MasterTableView.Items(rowIndex)
        '        Dim gvPreviousRow As GridDataItem = sender.MasterTableView.Items(rowIndex + 1)

        '        If gvRow.Cells(4).Text = gvPreviousRow.Cells(4).Text Then
        '            If gvPreviousRow.Cells(4).RowSpan < 2 Then
        '                gvRow.Cells(3).RowSpan = 2
        '                gvRow.Cells(4).RowSpan = 2
        '            Else
        '                gvRow.Cells(3).RowSpan = gvPreviousRow.Cells(3).RowSpan + 1
        '                gvRow.Cells(4).RowSpan = gvPreviousRow.Cells(4).RowSpan + 1
        '            End If
        '            gvPreviousRow.Cells(3).Visible = False
        '            gvPreviousRow.Cells(4).Visible = False
        '        End If
        '    Next
        'End If
    End Sub

    Public ReadOnly Property RecordsToWork() As DataTable
        Get
            Dim obj As Object = Me.Session("RecordsToWork")
            If (Not obj Is Nothing) Then
                Return CType(obj, DataTable)
            End If

            Dim myDataTable As DataTable = New DataTable
            myDataTable = o.GetRecordsToBeProcessed("work", o.ID, "min", "max", r.RMSSiteID)
            Me.Session("RecordsToWork") = myDataTable
            Return myDataTable
        End Get
    End Property

    Public ReadOnly Property RecordsToCheck() As DataTable
        Get
            Dim obj As Object = Me.Session("RecordsToCheck")
            If (Not obj Is Nothing) Then
                Return CType(obj, DataTable)
            End If

            Dim myDataTable As DataTable = New DataTable
            myDataTable = o.GetRecordsToBeProcessed("check", o.ID, "min", "max", r.RMSSiteID)
            Me.Session("RecordsToCheck") = myDataTable
            Return myDataTable
        End Get
    End Property

    Public ReadOnly Property RecordsToReview() As DataTable
        Get
            Dim obj As Object = Me.Session("RecordsToReview")
            If (Not obj Is Nothing) Then
                Return CType(obj, DataTable)
            End If

            Dim myDataTable As DataTable = New DataTable
            myDataTable = o.GetRecordsToBeProcessed("review", o.ID, "min", "max", r.RMSSiteID)
            Me.Session("RecordsToReview") = myDataTable
            Return myDataTable
        End Get
    End Property

End Class
