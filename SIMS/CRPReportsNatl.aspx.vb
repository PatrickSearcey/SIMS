Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb
Imports System.Drawing
Imports System.Collections.Generic
Imports Telerik.Charting

Public Class CRPReportsNatl
    Inherits System.Web.UI.Page

    Private region_cd As String

    Dim cnxstrcol As ConnectionStringSettingsCollection = ConfigurationManager.ConnectionStrings

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "SIMS - Continuous Records Processing Chart"

        region_cd = UCase(Request.QueryString("region_cd"))

        lblDate1.Text = "Current data as of: " & Now.ToString
        lblDate2.Text = "Current data as of: " & Now.ToString
        lblDate3.Text = "Current data as of: " & Now.ToString

        If Not Page.IsPostBack Then
            Select Case region_cd
                Case "CR"
                    ltlPageSubTitle.Text = "Central Region"
                    ltlRC1Title.Text = "Rocky Mountain (CRM)"
                    ltlRC2Title.Text = "South Central (CSC)"
                    ltlRC3Title.Visible = False
                    lblDate3.Visible = False

                Case "ER"
                    ltlPageSubTitle.Text = "Eastern Region"
                    ltlRC1Title.Text = "Midwest (EMW)"
                    ltlRC2Title.Text = "Northeast (ENE)"
                    ltlRC3Title.Text = "Southeast (ESE)"
                Case "WR"
                    ltlPageSubTitle.Text = "Western Region"
                    ltlRC1Title.Text = "Alaska (WAK)"
                    ltlRC2Title.Text = "Northwest (WNW)"
                    ltlRC3Title.Text = "Southwest (WSW)"
            End Select

            ResetCharts()
        End If
    End Sub

    Public ReadOnly Property Chart1Data() As DataTable
        Get
            Dim myDataTable As DataTable = New DataTable
            myDataTable = GetChartDataTable(region_cd, 1)

            Return myDataTable
        End Get
    End Property

    Public ReadOnly Property Chart2Data() As DataTable
        Get
            Dim myDataTable As DataTable = New DataTable
            myDataTable = GetChartDataTable(region_cd, 2)

            Return myDataTable
        End Get
    End Property

    Public ReadOnly Property Chart3Data() As DataTable
        Get
            Dim myDataTable As DataTable = New DataTable
            myDataTable = GetChartDataTable(region_cd, 3)

            Return myDataTable
        End Get
    End Property

    Public Function GetChartDataTable(ByVal region_cd As String, ByVal chart_no As Integer) As DataTable
        Dim dt As New DataTable
        dt.Clear()

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand("SP_CRP_Cat_Charts_by_region_cd", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add(New SqlParameter("@region_cd", SqlDbType.NVarChar, 3))
            cmd.Parameters("@region_cd").Value = region_cd

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            cnx.Close()
        End Using

        Return dt
    End Function

    Public Sub ResetCharts()

        rcCat1.Clear()
        rcCat1.PlotArea.XAxis.AutoScale = False
        rcCat1.PlotArea.XAxis.Clear()
        rcCat1.Series.ClearItems()
        rcCat1.ChartTitle.Visible = False

        rcCat2.Clear()
        rcCat2.PlotArea.XAxis.AutoScale = False
        rcCat2.PlotArea.XAxis.Clear()
        rcCat2.Series.ClearItems()
        rcCat2.ChartTitle.Visible = False

        If region_cd = "CR" Then
            rcCat3.Visible = False
        Else
            rcCat3.Clear()
            rcCat3.PlotArea.XAxis.AutoScale = False
            rcCat3.PlotArea.XAxis.Clear()
            rcCat3.Series.ClearItems()
            rcCat3.ChartTitle.Visible = False
        End If

        '---POPULATE THE FIRST CHART-------------------------------------------------------------------
        Dim cs1 As New ChartSeries()
        cs1.Clear()
        cs1.Name = "Category 1"
        cs1.Type = ChartSeriesType.Bar

        Dim x As Integer = 0
        For Each row As DataRow In Chart1Data.Rows
            cs1.AddItem(row("percent_of_cat1_done"))

            If row("ApprovedInLast150Days") > 999 Or row("TotalCat1Records") > 999 Then
                cs1.SetItemLabel(x, row("ApprovedInLast150Days").ToString & "/" & vbCrLf & row("TotalCat1Records").ToString)
            Else
                cs1.SetItemLabel(x, row("ApprovedInLast150Days").ToString & "/" & row("TotalCat1Records").ToString)
            End If

            rcCat1.PlotArea.XAxis.AddItem(row("wsc_nm"))
            rcCat1.AutoTextWrap = True

            x = x + 1
        Next

        Dim cs2 As New ChartSeries()
        cs2.Clear()
        cs2.Name = "Category 2"
        cs2.Type = ChartSeriesType.Bar

        Dim y As Integer = 0
        For Each row As DataRow In Chart1Data.Rows
            cs2.AddItem(row("percent_of_cat2_done"))

            If row("ApprovedInLast240Days") > 999 Or row("TotalCat2Records") > 999 Then
                cs2.SetItemLabel(y, row("ApprovedInLast240Days").ToString & "/" & vbCrLf & row("TotalCat2Records").ToString)
            Else
                cs2.SetItemLabel(y, row("ApprovedInLast240Days").ToString & "/" & row("TotalCat2Records").ToString)
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

        '---POPULATE THE SECOND CHART-------------------------------------------------------------------
        Dim cs3 As New ChartSeries()
        cs3.Clear()
        cs3.Name = "Category 1"
        cs3.Type = ChartSeriesType.Bar

        x = 0
        For Each row As DataRow In Chart2Data.Rows
            cs3.AddItem(row("percent_of_cat1_done"))

            If row("ApprovedInLast150Days") > 999 Or row("TotalCat1Records") > 999 Then
                cs3.SetItemLabel(x, row("ApprovedInLast150Days").ToString & "/" & vbCrLf & row("TotalCat1Records").ToString)
            Else
                cs3.SetItemLabel(x, row("ApprovedInLast150Days").ToString & "/" & row("TotalCat1Records").ToString)
            End If

            rcCat2.PlotArea.XAxis.AddItem(row("wsc_nm"))
            rcCat2.AutoTextWrap = True

            x = x + 1
        Next

        Dim cs4 As New ChartSeries()
        cs4.Clear()
        cs4.Name = "Category 2"
        cs4.Type = ChartSeriesType.Bar

        y = 0
        For Each row As DataRow In Chart2Data.Rows
            cs4.AddItem(row("percent_of_cat2_done"))

            If row("ApprovedInLast240Days") > 999 Or row("TotalCat2Records") > 999 Then
                cs4.SetItemLabel(y, row("ApprovedInLast240Days").ToString & "/" & vbCrLf & row("TotalCat2Records").ToString)
            Else
                cs4.SetItemLabel(y, row("ApprovedInLast240Days").ToString & "/" & row("TotalCat2Records").ToString)
            End If

            y = y + 1
        Next

        rcCat2.PlotArea.YAxis.Appearance.CustomFormat = "0\%"
        rcCat2.Legend.TextBlock.Text = "Explanation"

        ' Add the series to the chart.
        rcCat2.Series.Add(cs3)
        rcCat2.Series.Add(cs4)

        rcCat2.Series(0).Appearance.Shadow.Blur = 5
        rcCat2.Series(0).Appearance.Shadow.Distance = 3
        rcCat2.Series(1).Appearance.Shadow.Blur = 5
        rcCat2.Series(1).Appearance.Shadow.Distance = 3

        '---POPULATE THE THIRD CHART-------------------------------------------------------------------
        If Not region_cd = "CR" Then
            Dim cs5 As New ChartSeries()
            cs5.Clear()
            cs5.Name = "Category 1"
            cs5.Type = ChartSeriesType.Bar

            x = 0
            For Each row As DataRow In Chart3Data.Rows
                cs5.AddItem(row("percent_of_cat1_done"))

                If row("ApprovedInLast150Days") > 999 Or row("TotalCat1Records") > 999 Then
                    cs5.SetItemLabel(x, row("ApprovedInLast150Days").ToString & "/" & vbCrLf & row("TotalCat1Records").ToString)
                Else
                    cs5.SetItemLabel(x, row("ApprovedInLast150Days").ToString & "/" & row("TotalCat1Records").ToString)
                End If

                rcCat3.PlotArea.XAxis.AddItem(row("wsc_nm"))
                rcCat3.AutoTextWrap = True

                x = x + 1
            Next

            Dim cs6 As New ChartSeries()
            cs6.Clear()
            cs6.Name = "Category 2"
            cs6.Type = ChartSeriesType.Bar

            y = 0
            For Each row As DataRow In Chart3Data.Rows
                cs6.AddItem(row("percent_of_cat2_done"))

                If row("ApprovedInLast240Days") > 999 Or row("TotalCat2Records") > 999 Then
                    cs6.SetItemLabel(y, row("ApprovedInLast240Days").ToString & "/" & vbCrLf & row("TotalCat2Records").ToString)
                Else
                    cs6.SetItemLabel(y, row("ApprovedInLast240Days").ToString & "/" & row("TotalCat2Records").ToString)
                End If

                y = y + 1
            Next

            rcCat3.PlotArea.YAxis.Appearance.CustomFormat = "0\%"
            rcCat3.Legend.TextBlock.Text = "Explanation"

            ' Add the series to the chart.
            rcCat3.Series.Add(cs5)
            rcCat3.Series.Add(cs6)

            rcCat3.Series(0).Appearance.Shadow.Blur = 5
            rcCat3.Series(0).Appearance.Shadow.Distance = 3
            rcCat3.Series(1).Appearance.Shadow.Blur = 5
            rcCat3.Series(1).Appearance.Shadow.Distance = 3
        End If

    End Sub

End Class