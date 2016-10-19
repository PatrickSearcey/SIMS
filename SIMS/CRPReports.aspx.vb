Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.HttpContext
Imports System.Web.HttpRequest
Imports System.Net
Imports System.Web.UI.WebControls
Imports Telerik.Web.UI
Imports Telerik.Charting

Public Class CRPReports
    Inherits System.Web.UI.Page

    Private o As Office
    Private w As WSC
    Private uid As String = Current.User.Identity.Name
    Private u As New User(uid)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Master.PageTitle = "Continuous Records Processing Chart"
        Page.Title = "Continuous Records Processing Chart"

        Dim office_id As Integer = Request.QueryString("office_id")
        Dim wsc_id As Integer = Request.QueryString("wsc_id")
        Dim clientIP As String = Request.UserHostAddress

        '--REFERENCE OFFICE AND DISTRICT OBJECTS----------------------------------------------
        If office_id = Nothing And wsc_id = Nothing Then
            'If neither office_id nor wsc_id were passed in querystring, base them on user info
            wsc_id = u.WSCID
            office_id = u.GetUserIPOfficeID(clientIP)
            o = New Office(office_id)
        ElseIf Not office_id = Nothing Then
            'If office_id was passed, use office_id to find wsc_id
            o = New Office(office_id)
            wsc_id = o.WSCID
        Else
            o = New Office(0)
        End If
        w = New WSC(wsc_id)
        '--END SECTION---------------------------------------------------------------------

        Dim lad_150 As Date = DateAdd(DateInterval.DayOfYear, -150, Now())
        Dim lad_240 As Date = DateAdd(DateInterval.DayOfYear, -240, Now())

        lblDateCount.Text = "The date 150 days ago: <b>" & Format(lad_150, "MM/dd/yy") & _
                "</b>, and 240 days ago: <b>" & Format(lad_240, "MM/dd/yy") & "</b>"
        lblDate.Text = "Current data as of: <b>" & Now.ToString & "</b>"

        If Not Page.IsPostBack Then
            '--SETUP RADCHART------------------------------------------------------------------
            If Not office_id = Nothing Then
                hfSeriesName.Value = "records"
                hfOfficeID.Value = o.ID
                hfWSCID.Value = w.ID
                hlGraph.NavigateUrl = "UltDataAging.aspx?office_id=" & o.ID
                imgProgress.ImageUrl = "images/RecordLevel.png"
                ' Detach Click event handler for the innermost chart
                RemoveHandler rcCat1.Click, AddressOf rcCat1_Click
            Else
                hlGraph.NavigateUrl = "UltDataAging.aspx?wsc_id=" & w.ID
                imgProgress.ImageUrl = "images/WSCLevel.png"
                hfSeriesName.Value = "forWSC"
                hfOfficeID.Value = "0"
                hfWSCID.Value = w.ID
            End If
            PopulateWSCDDL()
            ResetChart()
        End If
    End Sub

    Public ReadOnly Property ChartData() As DataTable
        Get
            Dim myDataTable As DataTable = New DataTable
            myDataTable = GetChartDataTable(hfWSCID.Value, hfOfficeID.Value, hfSeriesName.Value)

            Return myDataTable
        End Get
    End Property

    Public Function GetChartDataTable(ByVal wsc_id As Integer, ByVal office_id As Integer, ByVal type As String) As DataTable
        Dim cdt As New DataTable
        cdt.Clear()

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            If office_id > 0 Then
                wsc_id = 0
            End If

            Dim cmd As New SqlCommand("SP_CRP_Cat_Charts", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@office_id", SqlDbType.Int).Value = office_id
            cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = wsc_id
            cmd.Parameters.Add("@type", SqlDbType.NVarChar, 10).Value = type

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(cdt)

            cnx.Close()
        End Using

        Return cdt
    End Function

    Public Sub ResetChart()

        rcCat1.Clear()
        rcCat1.PlotArea.XAxis.AutoScale = False
        rcCat1.PlotArea.XAxis.Clear()
        rcCat1.Series.ClearItems()

        If hfSeriesName.Value = "records" Or hfSeriesName.Value = "userid" Then
            Dim office As New Office(hfOfficeID.Value)
            rcCat1.ChartTitle.TextBlock.Text = "Progress for the " & office.Name
            rcCat1.ChartTitle.Visible = True
        Else
            rcCat1.ChartTitle.Visible = False
        End If

        Dim cs1 As New ChartSeries()
        cs1.Clear()
        cs1.Name = "Category 1"
        cs1.Type = ChartSeriesType.Bar

        Dim x As Integer = 0
        For Each row As DataRow In ChartData.Rows
            cs1.AddItem(row("percent_of_cat1_done"))
            cs1.SetItemLabel(x, row("ApprovedInLast150Days").ToString & "/" & row("TotalCat1Records").ToString)

            If hfSeriesName.Value = "forWSC" Then
                rcCat1.PlotArea.XAxis.AddItem(row("wsc_nm"))
                rcCat1.AutoTextWrap = True
            ElseIf hfSeriesName.Value = "alloffices" Then
                rcCat1.PlotArea.XAxis.AddItem(row("office_cd"))
                cs1.Item(x).Name = row("office_id").ToString
                rcCat1.AutoTextWrap = True
            ElseIf hfSeriesName.Value = "records" Then
                rcCat1.PlotArea.XAxis.AddItem(row("type_ds"))
                rcCat1.AutoTextWrap = True
            ElseIf hfSeriesName.Value = "userid" Then
                rcCat1.PlotArea.XAxis.AddItem(row("operator_va"))
                rcCat1.PlotArea.XAxis.Item(x).Appearance.RotationAngle = 70
                rcCat1.AutoTextWrap = False
            End If

            x = x + 1
        Next

        Dim cs2 As New ChartSeries()
        cs2.Clear()
        cs2.Name = "Category 2"
        cs2.Type = ChartSeriesType.Bar

        Dim y As Integer = 0
        For Each row As DataRow In ChartData.Rows
            cs2.AddItem(row("percent_of_cat2_done"))
            cs2.SetItemLabel(y, row("ApprovedInLast240Days").ToString & "/" & row("TotalCat2Records").ToString)

            If hfSeriesName.Value = "alloffices" Then
                cs2.Item(y).Name = row("office_id").ToString
            End If

            y = y + 1
        Next

        rcCat1.PlotArea.YAxis.Appearance.CustomFormat = "0\%"
        rcCat1.Legend.TextBlock.Text = "Explanation"

        ' Add the series to the chart.
        rcCat1.Series.Add(cs1)
        rcCat1.Series.Add(cs2)

        rcCat1.Series(0).Appearance.Shadow.Blur = 5
        rcCat1.Series(0).Appearance.Shadow.Distance = 3
        rcCat1.Series(1).Appearance.Shadow.Blur = 5
        rcCat1.Series(1).Appearance.Shadow.Distance = 3

    End Sub

    Protected Sub rcCat1_Click(ByVal sender As Object, ByVal args As ChartClickEventArgs)
        If args.SeriesItem IsNot Nothing Then
            If hfSeriesName.Value = "forWSC" Then
                hfSeriesName.Value = "alloffices"
                imgProgress.ImageUrl = "images/OfficeLevel.png"
                ResetChart()
            ElseIf hfSeriesName.Value = "alloffices" Then
                Dim office_id As Integer = Convert.ToInt32(args.SeriesItem.Name)
                'ddlFinalChartData.AutoPostBack = True
                hfOfficeID.Value = office_id
                imgProgress.ImageUrl = "images/RecordLevel.png"
                hfSeriesName.Value = "records" 'ddlFinalChartData.SelectedValue
                hlGraph.NavigateUrl = "UltDataAging.aspx?office_id=" & office_id
                ddlOffice.SelectedValue = office_id
                ' Detach Click event handler for the innermost chart
                RemoveHandler rcCat1.Click, AddressOf rcCat1_Click
                ResetChart()
            End If
        End If
    End Sub

    Protected Sub Reset_Command(ByVal sender As Object, ByVal args As CommandEventArgs)
        If args.CommandArgument = "ResettoWSC" Then
            If hfSeriesName.Value IsNot "forWSC" Then
                hfSeriesName.Value = "forWSC"
                imgProgress.ImageUrl = "images/WSCLevel.png"
                hfOfficeID.Value = "0"
                hlGraph.NavigateUrl = "UltDataAging.aspx?wsc_id=" & hfWSCID.Value.ToString
                ddlOffice.SelectedIndex = 0
                ResetChart()
            End If
        Else
            If hfSeriesName.Value IsNot "alloffices" Then
                hfSeriesName.Value = "alloffices"
                imgProgress.ImageUrl = "images/OfficeLevel.png"
                hfOfficeID.Value = "0"
                hlGraph.NavigateUrl = "UltDataAging.aspx?wsc_id=" & hfWSCID.Value.ToString
                ddlOffice.SelectedIndex = 0
                ResetChart()
            End If
        End If
    End Sub

    Public Sub PopulateWSCDDL()
        Dim sql As String = Nothing
        Dim dbcomm As SqlCommand
        Dim WSCList As SqlDataReader

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            sql = "SELECT DISTINCT lw.wsc_nm, lw.wsc_cd, lw.wsc_id" & _
            " FROM lut_WSC AS lw INNER JOIN lut_Office As lo ON lw.wsc_id = lo.wsc_id" & _
            " ORDER BY wsc_nm"
            dbcomm = New SqlCommand(sql, cnx)
            WSCList = dbcomm.ExecuteReader()

            ddlWSC.DataSource = WSCList
            ddlWSC.DataBind()

            cnx.Close()
        End Using

        If w.ID <> Nothing And w.ID <> 0 Then
            ddlWSC.SelectedValue = w.ID
            PopulateOfficeDDL(w.ID)
            pnlOffice.Visible = True
        End If
    End Sub

    Public Sub PopulateOfficeDDL(ByVal wsc_id As Integer)
        Dim sql As String = Nothing
        Dim dbcomm As SqlCommand
        Dim OfficeList As SqlDataReader

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            sql = "SELECT DISTINCT office_nm, office_id" & _
                " FROM lut_Office" & _
                " WHERE wsc_id = " & wsc_id & _
                " ORDER BY office_nm"
            dbcomm = New SqlCommand(sql, cnx)
            OfficeList = dbcomm.ExecuteReader()

            If OfficeList.HasRows Then
                ddlOffice.DataSource = OfficeList
                ddlOffice.DataBind()
                ddlOffice.Items.Insert(0, New ListItem(String.Empty, String.Empty))
            End If

            If Not o.ID = 0 Then
                ddlOffice.SelectedValue = o.ID
            End If

            OfficeList.Close()
            cnx.Close()
        End Using
    End Sub

    Protected Sub ddlWSC_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlWSC.SelectedIndexChanged
        o = New Office(0)
        w = New WSC(ddlWSC.SelectedValue)
        PopulateOfficeDDL(ddlWSC.SelectedItem.Value.ToString)
        pnlOffice.Visible = True
        hlGraph.NavigateUrl = "UltDataAging.aspx?wsc_id=" & w.ID.ToString
        hfSeriesName.Value = "forWSC"
        imgProgress.ImageUrl = "images/WSCLevel.png"
        hfOfficeID.Value = "0"
        hfWSCID.Value = w.ID
        ResetChart()
    End Sub

    Protected Sub ddlOffice_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlOffice.SelectedIndexChanged
        If ddlOffice.SelectedItem.Value IsNot Nothing And ddlOffice.SelectedItem.Value <> "" Then
            o = New Office(Convert.ToInt32(ddlOffice.SelectedItem.Value))
            hlGraph.NavigateUrl = "UltDataAging.aspx?office_id=" & o.ID
            hfOfficeID.Value = o.ID
            imgProgress.ImageUrl = "images/RecordLevel.png"
            hfSeriesName.Value = "records" 'ddlFinalChartData.SelectedValue
            ' Detach Click event handler for the innermost chart
            RemoveHandler rcCat1.Click, AddressOf rcCat1_Click
            ResetChart()
        Else
            o = New Office(0)
            hfSeriesName.Value = "forWSC"
            imgProgress.ImageUrl = "images/WSCLevel.png"
            hlGraph.NavigateUrl = "UltDataAging.aspx?wsc_id=" & hfWSCID.Value.ToString
            hfOfficeID.Value = "0"
            ResetChart()
        End If
    End Sub
End Class
