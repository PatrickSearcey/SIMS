Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.HttpContext
Imports System.Web.HttpRequest
Imports System.Net
Imports System.Web.UI.WebControls
Imports Telerik.Web.UI

Public Class RecordLists
    Inherits System.Web.UI.Page

    Private o As Office
    Private w As WSC
    Private r As Record
    Private s As Site
    Private uid As String = Current.User.Identity.Name
    Private u As New User(uid)

    Dim _pers As PageStatePersister

    Protected Overrides ReadOnly Property PageStatePersister() As PageStatePersister
        Get
            If _pers Is Nothing Then
                _pers = New SessionPageStatePersister(Me)
            End If
            Return _pers
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Master.PageTitle = "Records Processing Interface"
        Page.Title = "Records Processing Interface"
        Master.ShowPrevPageLink = False

        Dim office_id As Integer = Request.QueryString("office_id")
        Dim listtype As String = Request.QueryString("listtype")
        Dim lt As String = Nothing
        Dim ltCaps As String = Nothing
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
            ltCaps = "Work"
        ElseIf listtype = "check" Then
            lt = "checked"
            ltCaps = "Check"
        ElseIf listtype = "review" Then
            lt = "reviewed"
            ltCaps = "Review"
        End If

        lblListType.Text = listtype

        If rms_record_id = Nothing Then
            r = New Record(0)

            PopulateQuickLinks(listtype)

            Master.SiteNumberLabel = "Records to be " & lt & " for " & o.Name
        Else
            r = New Record(rms_record_id)

            PopulateQuickLinks("all")

            Master.SiteNumberLabel = ltCaps & " Record"
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
            hfRangeStart.Value = "min"
            hfRangeEnd.Value = "max"
            hfListType.Value = listtype

        End If
    End Sub

    Private Sub PopulateQuickLinks(ByVal listtype As String)
        Dim pOut As String = Nothing

        Select Case listtype
            Case "work"
                pOut = "<label>View records available for:</label><br />" & _
                    "<a href=""" & Config.SitePath & "sitelists.asp?listtype=check&office_id=" & o.ID & """>checking</a> | " & _
                    "<a href=""" & Config.SitePath & "sitelists.asp?listtype=review&office_id=" & o.ID & """>reviewing</a><br /><br />" & _
                    "<label><a href=""/doc/RecordsProcessingInterface.doc"">Learn more about this page</a></label>"
            Case "check"
                pOut = "<label>View records available for:</label><br />" & _
                    "<a href=""" & Config.SitePath & "sitelists.asp?listtype=work&office_id=" & o.ID & """>working</a> | " & _
                    "<a href=""" & Config.SitePath & "sitelists.asp?listtype=review&office_id=" & o.ID & """>reviewing</a><br /><br />" & _
                    "<label><a href=""/doc/RecordsProcessingInterface.doc"">Learn more about this page</a></label>"
            Case "review"
                pOut = "<label>View records available for:</label><br />" & _
                    "<a href=""" & Config.SitePath & "sitelists.asp?listtype=work&office_id=" & o.ID & """>working</a> | " & _
                    "<a href=""" & Config.SitePath & "sitelists.asp?listtype=check&office_id=" & o.ID & """>checking</a><br /><br />" & _
                    "<label><a href=""/doc/RecordsProcessingInterface.doc"">Learn more about this page</a></label>"
            Case "all"
                pOut = "<label>View all records available for:</label><br />" & _
                    "<a href=""" & Config.SitePath & "sitelists.asp?listtype=work&office_id=" & o.ID & """>working</a> | " & _
                    "<a href=""" & Config.SitePath & "sitelists.asp?listtype=check&office_id=" & o.ID & """>checking</a> | " & _
                    "<a href=""" & Config.SitePath & "sitelists.asp?listtype=review&office_id=" & o.ID & """>reviewing</a><br /><br />" & _
                    "<label><a href=""/doc/RecordsProcessingInterface.doc"">Learn more about this page</a></label>"
        End Select

        ltlQuickLinks.Text = pOut
    End Sub

    Protected Sub rgWork_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs)
        If Not e.IsFromDetailTable Then
            source.DataSource = Me.RecordsToWork
        End If
    End Sub

    Protected Sub rgCheck_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs)
        If Not e.IsFromDetailTable Then
            source.DataSource = Me.RecordsToCheck
        End If
    End Sub

    Protected Sub rgReview_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs)
        If Not e.IsFromDetailTable Then
            source.DataSource = Me.RecordsToReview
        End If
    End Sub

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
        Dim listtype As String = Request.QueryString("listtype")
        Dim rms_record_id As Integer = Request.QueryString("rms_record_id")
        Dim src As String = Request.QueryString("src")
        Dim grid As New RadGrid
        Dim ndsHandler As GridNeedDataSourceEventHandler
        Dim dbHandler As EventHandler = New EventHandler(AddressOf rg_DataBound)
        Dim prHandler As EventHandler = New EventHandler(AddressOf rg_PreRender)
        'Dim dtbHandler As GridDetailTableDataBindEventHandler
        'dtbHandler = New GridDetailTableDataBindEventHandler(AddressOf rg_DetailTableDataBind)

        Select Case listtype
            Case "work"
                ndsHandler = New GridNeedDataSourceEventHandler(AddressOf rgWork_NeedDataSource)
                grid.ID = "rgWork"
            Case "check"
                ndsHandler = New GridNeedDataSourceEventHandler(AddressOf rgCheck_NeedDataSource)
                grid.ID = "rgCheck"
            Case "review"
                ndsHandler = New GridNeedDataSourceEventHandler(AddressOf rgReview_NeedDataSource)
                grid.ID = "rgReview"
            Case Else
                ndsHandler = New GridNeedDataSourceEventHandler(AddressOf rgWork_NeedDataSource)
                grid.ID = "rgWork"
        End Select

        AddHandler grid.NeedDataSource, ndsHandler
        AddHandler grid.DataBound, dbHandler
        AddHandler grid.PreRender, prHandler
        'AddHandler grid.DetailTableDataBind, dtbHandler

        grid.AutoGenerateColumns = False

        '---CREATE THE SITE TOP LEVEL TABLE-------------------------------------------------------
        Dim templateColumn2 As New GridTemplateColumn()
        templateColumn2.ItemTemplate = New RadGridTemplate("lcks", u.ID, listtype)
        templateColumn2.HeaderText = "lcks"

        Dim templateColumn3 As New GridTemplateColumn()
        templateColumn3.ItemTemplate = New RadGridTemplate("site no", u.ID, listtype)
        templateColumn3.HeaderText = "site no"
        templateColumn3.DataField = "site_no"
        templateColumn3.UniqueName = "SiteNumber"
        templateColumn3.SortExpression = "site_no"

        Dim boundColumn2 As New GridBoundColumn()
        boundColumn2.DataField = "station_nm"
        boundColumn2.UniqueName = "StationName"
        boundColumn2.HeaderText = "station nm"

        Dim templateColumn4 As New GridTemplateColumn()
        templateColumn4.ItemTemplate = New RadGridTemplate("record-type", u.ID, listtype & src)
        templateColumn4.HeaderText = "record-type"
        templateColumn4.DataField = "type_ds"
        templateColumn4.UniqueName = "RecordType"

        Dim boundColumn4 As New GridBoundColumn()
        boundColumn4.DataField = "category_no"
        boundColumn4.UniqueName = "CategoryNumber"
        boundColumn4.HeaderText = "cat no"

        Dim boundColumn9 As New GridBoundColumn()
        boundColumn9.DataField = "DaysSinceAging"
        boundColumn9.UniqueName = "DaysSinceLastApproved"
        boundColumn9.HeaderText = "days since last approved in ADAPS"

        Dim boundColumn6 As New GridBoundColumn()
        Dim boundColumn7 As New GridBoundColumn()
        Dim boundColumn8 As New GridBoundColumn()

        Select Case listtype
            Case "work"
                boundColumn6.DataField = "operator_va"
                boundColumn6.UniqueName = "Operator"
                boundColumn6.HeaderText = "assigned to"

                boundColumn7.DataField = "rework_status"
                boundColumn7.UniqueName = "ReworkStatus"
                boundColumn7.HeaderText = "rework status"
                boundColumn7.AllowFiltering = False

                boundColumn8.DataField = "last_worked_dt"
                boundColumn8.UniqueName = "LastWorkedDate"
                boundColumn8.HeaderText = "worked through"
            Case "check"
                boundColumn6.DataField = "worked_by_uid"
                boundColumn6.UniqueName = "WorkedBy"
                boundColumn6.HeaderText = "worked by"

                boundColumn7.DataField = "checkerassigned"
                boundColumn7.UniqueName = "CheckerAssigned"
                boundColumn7.HeaderText = "assigned to"
                boundColumn7.FilterControlWidth = 40

                boundColumn8.DataField = "period_dt"
                boundColumn8.UniqueName = "PeriodDate"
                boundColumn8.HeaderText = "period dt"
            Case "review"
                boundColumn6.DataField = "reviewerassigned"
                boundColumn6.UniqueName = "ReviewerAssigned"
                boundColumn6.HeaderText = "assigned to"

                boundColumn7.DataField = "reviewed_by_uid"
                boundColumn7.UniqueName = "ReviewedBy"
                boundColumn7.HeaderText = "prev. review by"
                boundColumn7.AllowFiltering = False

                boundColumn8.DataField = "period_dt"
                boundColumn8.UniqueName = "PeriodDate"
                boundColumn8.HeaderText = "period dt"
        End Select

        templateColumn2.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        templateColumn2.AllowFiltering = False

        templateColumn3.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        templateColumn3.ItemStyle.Wrap = False
        templateColumn3.FilterControlWidth = 50

        templateColumn4.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        templateColumn4.ItemStyle.Wrap = False

        boundColumn2.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        boundColumn2.ItemStyle.Wrap = False
        boundColumn2.FilterControlWidth = 150

        boundColumn4.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        boundColumn4.FilterControlWidth = 10
        boundColumn4.ItemStyle.HorizontalAlign = HorizontalAlign.Center

        boundColumn6.FilterControlWidth = 40
        boundColumn6.HeaderStyle.HorizontalAlign = HorizontalAlign.Center

        boundColumn7.HeaderStyle.HorizontalAlign = HorizontalAlign.Center

        boundColumn8.AllowFiltering = False
        boundColumn8.ItemStyle.Wrap = False
        boundColumn8.HeaderStyle.HorizontalAlign = HorizontalAlign.Center

        boundColumn9.FilterControlWidth = 30
        boundColumn9.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        boundColumn9.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        boundColumn9.ItemStyle.Width = "110"

        grid.MasterTableView.Columns.Add(templateColumn2)
        grid.MasterTableView.Columns.Add(templateColumn3)
        grid.MasterTableView.Columns.Add(boundColumn2)
        grid.MasterTableView.Columns.Add(templateColumn4)
        grid.MasterTableView.Columns.Add(boundColumn4)
        grid.MasterTableView.Columns.Add(boundColumn6)
        grid.MasterTableView.Columns.Add(boundColumn7)
        grid.MasterTableView.Columns.Add(boundColumn8)
        grid.MasterTableView.Columns.Add(boundColumn9)

        grid.MasterTableView.DataKeyNames = New String() {"rms_record_id"}
        grid.MasterTableView.Name = "Records"
        grid.MasterTableView.AllowMultiColumnSorting = True
        grid.EnableViewState = True

        If rms_record_id = Nothing Then
            grid.AllowFilteringByColumn = True
        Else
            grid.AllowFilteringByColumn = False
        End If

        Dim menu As GridFilterMenu = grid.FilterMenu
        Dim i As Integer = 0
        While i < menu.Items.Count
            If menu.Items(i).Text = "NoFilter" Or _
               menu.Items(i).Text = "Contains" Or _
               menu.Items(i).Text = "EqualTo" Or _
               menu.Items(i).Text = "GreaterThan" Or _
               menu.Items(i).Text = "LessThan" Or _
               menu.Items(i).Text = "IsEmpty" Or _
               menu.Items(i).Text = "NotIsEmpty" Then
                i = i + 1
            Else
                menu.Items.RemoveAt(i)
            End If
        End While

        grid.ClientSettings.AllowExpandCollapse = True
        grid.AllowPaging = False
        grid.Skin = "Sunset"
        grid.AllowSorting = True
        grid.ShowStatusBar = True
        grid.EnableLinqExpressions = False

        phRecordsToProcess.Controls.Add(grid)
    End Sub

    Protected Sub rg_PreRender(ByVal sender As Object, ByVal e As System.EventArgs)
        If (Not Page.IsPostBack) Then
            Dim rms_record_id As Integer = Request.QueryString("rms_record_id")
            r = New Record(rms_record_id)
            s = New Site(r.SiteID)

            If s.ID > 0 Then
                sender.MasterTableView.FilterExpression = "([site_no] LIKE '%" & s.Number & "%') "
                Dim column As GridColumn = sender.MasterTableView.GetColumnSafe("SiteNumber")
                column.CurrentFilterFunction = GridKnownFunction.Contains
                column.CurrentFilterValue = s.Number
                sender.MasterTableView.Rebind()
            End If
        End If
    End Sub

    Protected Sub rg_DataBound(ByVal sender As Object, ByVal e As EventArgs)
        Dim recordCount As Integer = Nothing
        Dim gvRow As GridDataItem = Nothing

        If sender.MasterTableView.Items.Count > 1 Then
            For rowIndex As Integer = sender.MasterTableView.Items.Count - 2 To 0 Step -1
                gvRow = sender.MasterTableView.Items(rowIndex)
                Dim gvPreviousRow As GridDataItem = sender.MasterTableView.Items(rowIndex + 1)

                If gvRow.Cells(4).Text = gvPreviousRow.Cells(4).Text Then
                    If gvPreviousRow.Cells(4).RowSpan < 2 Then
                        gvRow.Cells(3).RowSpan = 2
                        gvRow.Cells(4).RowSpan = 2
                    Else
                        gvRow.Cells(3).RowSpan = gvPreviousRow.Cells(3).RowSpan + 1
                        gvRow.Cells(4).RowSpan = gvPreviousRow.Cells(4).RowSpan + 1
                    End If
                    gvPreviousRow.Cells(3).Visible = False
                    gvPreviousRow.Cells(4).Visible = False
                End If
            Next
        End If
    End Sub

    Protected Sub rsDaysAgo_ValueChanged(ByVal sender As Object, ByVal e As EventArgs)
        lblSelectionStart.Text = rsDaysAgo.SelectionStart.ToString()
        lblSelectionEnd.Text = rsDaysAgo.SelectionEnd.ToString()
        hfRangeStart.Value = rsDaysAgo.SelectionStart
        hfRangeEnd.Value = rsDaysAgo.SelectionEnd
        Me.Session("RecordsToWork") = Nothing
        Me.Session("RecordsToCheck") = Nothing
        Me.Session("RecordsToReview") = Nothing

        Dim grid As RadGrid = DirectCast(phRecordsToProcess.Controls.Item(0), RadGrid)

        grid.Rebind()

    End Sub

    Public ReadOnly Property RecordsToWork() As DataTable
        Get
            Dim obj As Object = Me.Session("RecordsToWork")
            If (Not obj Is Nothing) Then
                Return CType(obj, DataTable)
            End If

            Dim myDataTable As DataTable = New DataTable
            myDataTable = o.GetRecordsToBeProcessed("work", o.ID, hfRangeStart.Value, hfRangeEnd.Value, r.RMSSiteID)
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
            myDataTable = o.GetRecordsToBeProcessed("check", o.ID, hfRangeStart.Value, hfRangeEnd.Value, r.RMSSiteID)
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
            myDataTable = o.GetRecordsToBeProcessed("review", o.ID, hfRangeStart.Value, hfRangeEnd.Value, r.RMSSiteID)
            Me.Session("RecordsToReview") = myDataTable
            Return myDataTable
        End Get
    End Property


End Class