Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb
Imports System.Net.Mail

Public Class UltDataAging
    Inherits System.Web.UI.Page

    Private w As WSC
    Private d As District
    Private o As Office
    Private r As Record

    Dim cnxstrcol As ConnectionStringSettingsCollection = ConfigurationManager.ConnectionStrings
    Dim cnx As SqlConnection = New SqlConnection(cnxstrcol.Item("simsdbConnectionString").ConnectionString)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Master.PageTitle = "Continuous Records Processing Status"
        Page.Title = "Continuous Records Processing Status"

        Dim wsc_id As Integer = Request.QueryString("wsc_id")
        Dim office_id As Integer = Request.QueryString("office_id")
        Dim rms_record_id As Integer = Request.QueryString("rms_record_id")
        Dim district_cd As String = Request.QueryString("district_cd")

        If Not String.IsNullOrEmpty(district_cd) Then
            d = New District(district_cd)
            w = New WSC(d.WSCID)
            hlGraph.NavigateUrl = "CRPReports.aspx?wsc_id=" & w.ID.ToString
        Else
            If Not wsc_id = Nothing Then
                w = New WSC(wsc_id)
                hlGraph.NavigateUrl = "CRPReports.aspx?wsc_id=" & wsc_id.ToString
            Else
                w = New WSC(0)
            End If
        End If

        If Not office_id = Nothing Then
            o = New Office(office_id)
            w = New WSC(o.WSCID)
            hlGraph.NavigateUrl = "CRPReports.aspx?office_id=" & office_id
        Else
            o = New Office(0)
            If wsc_id = Nothing And w.ID = 0 Then
                w = New WSC(0)
            End If
        End If

        If Not rms_record_id = Nothing Then
            r = New Record(rms_record_id)
            o = New Office(r.AltOfficeID)
            w = New WSC(r.WSCID)
            hlGraph.NavigateUrl = "CRPReports.aspx?office_id=" & o.ID
        Else
            r = New Record(0)
        End If

        If Not Page.IsPostBack Then
            rblOnlyActive.SelectedValue = "yes"
            PopulateWSCDDL()
            gvCRP.Visible = False
        End If

    End Sub

    Public Sub PopulateWSCDDL()
        Dim sql As String = Nothing
        Dim dbcomm As SqlCommand
        Dim WSCList As SqlDataReader

        cnx.Open()
        sql = "SELECT DISTINCT lw.wsc_nm, lw.wsc_id" & _
            " FROM lut_WSC AS lw INNER JOIN lut_Office As lo ON lw.wsc_id = lo.wsc_id" & _
            " ORDER BY wsc_nm"
        dbcomm = New SqlCommand(sql, cnx)
        WSCList = dbcomm.ExecuteReader()

        ddlWSC.DataSource = WSCList
        ddlWSC.DataBind()
        ddlWSC.Items.Insert(0, New ListItem(String.Empty, String.Empty))

        cnx.Close()

        If w.ID <> Nothing And w.ID <> 0 Then
            ddlWSC.SelectedValue = w.ID
            PopulateOfficeDDL(w.ID)
            pnlOffice.Visible = True
            pnlOnlyActive.Visible = True
        End If
    End Sub

    Public Sub PopulateOfficeDDL(ByVal wsc_id As Integer)
        Dim sql As String = Nothing
        Dim dbcomm As SqlCommand
        Dim OfficeList As SqlDataReader

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
    End Sub

    Protected Sub ddlWSC_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlWSC.SelectedIndexChanged
        If ddlWSC.SelectedValue.ToString = "0" Then
            PopulateWSCDDL()
            gvCRP.Visible = False
        Else
            o = New Office(0)
            w = New WSC(ddlWSC.SelectedValue)
            PopulateOfficeDDL(ddlWSC.SelectedItem.Value)
            pnlOffice.Visible = True
            pnlOnlyActive.Visible = True
            hlGraph.NavigateUrl = "CRPReports.aspx?wsc_id=" & w.ID
            gvCRP.DataBind()
            gvCRP.Visible = True
        End If
    End Sub

    Protected Sub ddlOffice_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlOffice.SelectedIndexChanged
        If ddlOffice.SelectedItem.Value IsNot Nothing And ddlOffice.SelectedItem.Value <> "" Then
            o = New Office(ddlOffice.SelectedItem.Value)
            hlGraph.NavigateUrl = "CRPReports.aspx?office_id=" & o.ID
        Else
            o = New Office(0)
        End If
        gvCRP.DataBind()
        gvCRP.Visible = True
    End Sub

    Protected Sub gvCRP_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvCRP.DataBound
        Dim gvRow As GridViewRow = Nothing
        Dim lad_150 As Date = DateAdd(DateInterval.DayOfYear, -150, Now())
        Dim lad_240 As Date = DateAdd(DateInterval.DayOfYear, -240, Now())
        Dim lad_cell_va As Date = Nothing
        Dim cat_no As String = Nothing
        Dim work_period_dt As String = Nothing
        Dim rev_period_dt As String = Nothing
        Dim work_period_end_dt As String = Nothing
        Dim check_period_end_dt As String = Nothing
        Dim rev_period_end_dt As String = Nothing

        If gvCRP.Rows.Count = 1 Then
            gvRow = gvCRP.Rows(0)

            work_period_dt = gvRow.Cells(13).Text
            rev_period_dt = gvRow.Cells(14).Text
            work_period_end_dt = gvRow.Cells(15).Text
            check_period_end_dt = gvRow.Cells(16).Text
            rev_period_end_dt = gvRow.Cells(17).Text
            cat_no = gvRow.Cells(7).Text

            Try
                lad_cell_va = gvRow.Cells(11).Text

                If lad_cell_va > lad_150 And cat_no = "1" Then
                    gvRow.Cells(11).BackColor = Drawing.Color.PaleGoldenrod
                ElseIf lad_cell_va > lad_240 And cat_no = "2" Then
                    gvRow.Cells(11).BackColor = Drawing.Color.PaleGoldenrod
                Else
                    gvRow.Cells(11).BackColor = Drawing.Color.White
                End If
            Catch ex As Exception
                gvRow.Cells(11).BackColor = Drawing.Color.White
            End Try

            If work_period_dt = rev_period_dt Then
                Select Case cat_no
                    Case "1"

                        Try
                            If work_period_end_dt > lad_150 Then
                                gvRow.Cells(8).BackColor = Drawing.Color.Olive
                                gvRow.Cells(9).BackColor = Drawing.Color.Olive
                                gvRow.Cells(10).BackColor = Drawing.Color.Olive
                            Else
                                gvRow.Cells(8).BackColor = Drawing.Color.DarkKhaki
                                gvRow.Cells(9).BackColor = Drawing.Color.DarkKhaki
                                gvRow.Cells(10).BackColor = Drawing.Color.DarkKhaki
                            End If
                        Catch ex As Exception
                            gvRow.Cells(8).BackColor = Drawing.Color.DarkKhaki
                            gvRow.Cells(9).BackColor = Drawing.Color.DarkKhaki
                            gvRow.Cells(10).BackColor = Drawing.Color.DarkKhaki
                        End Try

                    Case "2"

                        Try
                            If work_period_end_dt > lad_240 Then
                                gvRow.Cells(8).BackColor = Drawing.Color.Olive
                                gvRow.Cells(9).BackColor = Drawing.Color.Olive
                                gvRow.Cells(10).BackColor = Drawing.Color.Olive
                            Else
                                gvRow.Cells(8).BackColor = Drawing.Color.DarkKhaki
                                gvRow.Cells(9).BackColor = Drawing.Color.DarkKhaki
                                gvRow.Cells(10).BackColor = Drawing.Color.DarkKhaki
                            End If
                        Catch ex As Exception
                            gvRow.Cells(8).BackColor = Drawing.Color.DarkKhaki
                            gvRow.Cells(9).BackColor = Drawing.Color.DarkKhaki
                            gvRow.Cells(10).BackColor = Drawing.Color.DarkKhaki
                        End Try

                    Case Else
                        gvRow.Cells(8).BackColor = Drawing.Color.DarkKhaki
                        gvRow.Cells(9).BackColor = Drawing.Color.DarkKhaki
                        gvRow.Cells(10).BackColor = Drawing.Color.DarkKhaki
                End Select
            Else
                Select Case cat_no
                    Case "1"

                        Try
                            If work_period_end_dt > lad_150 Then
                                gvRow.Cells(8).BackColor = Drawing.Color.Moccasin
                            Else
                                gvRow.Cells(8).BackColor = Drawing.Color.White
                            End If
                        Catch ex As Exception
                            gvRow.Cells(8).BackColor = Drawing.Color.White
                        End Try

                        Try
                            If check_period_end_dt > lad_150 Then
                                gvRow.Cells(9).BackColor = Drawing.Color.Moccasin
                            Else
                                gvRow.Cells(9).BackColor = Drawing.Color.White
                            End If
                        Catch ex As Exception
                            gvRow.Cells(9).BackColor = Drawing.Color.White
                        End Try

                        Try
                            If rev_period_end_dt > lad_150 Then
                                gvRow.Cells(10).BackColor = Drawing.Color.Moccasin
                            Else
                                gvRow.Cells(10).BackColor = Drawing.Color.White
                            End If
                        Catch ex As Exception
                            gvRow.Cells(10).BackColor = Drawing.Color.White
                        End Try

                    Case "2"

                        Try
                            If work_period_end_dt > lad_240 Then
                                gvRow.Cells(8).BackColor = Drawing.Color.Moccasin
                            Else
                                gvRow.Cells(8).BackColor = Drawing.Color.White
                            End If
                        Catch ex As Exception
                            gvRow.Cells(8).BackColor = Drawing.Color.White
                        End Try

                        Try
                            If check_period_end_dt > lad_240 Then
                                gvRow.Cells(9).BackColor = Drawing.Color.Moccasin
                            Else
                                gvRow.Cells(9).BackColor = Drawing.Color.White
                            End If
                        Catch ex As Exception
                            gvRow.Cells(9).BackColor = Drawing.Color.White
                        End Try

                        Try
                            If rev_period_end_dt > lad_240 Then
                                gvRow.Cells(10).BackColor = Drawing.Color.Moccasin
                            Else
                                gvRow.Cells(10).BackColor = Drawing.Color.White
                            End If
                        Catch ex As Exception
                            gvRow.Cells(10).BackColor = Drawing.Color.White
                        End Try

                    Case Else
                        gvRow.Cells(8).BackColor = Drawing.Color.White
                        gvRow.Cells(9).BackColor = Drawing.Color.White
                        gvRow.Cells(10).BackColor = Drawing.Color.White
                End Select
            End If

            gvRow.Cells(13).Visible = False
            gvRow.Cells(14).Visible = False
            gvRow.Cells(15).Visible = False
            gvRow.Cells(16).Visible = False
            gvRow.Cells(17).Visible = False
            gvCRP.Visible = True
            pnlExplanation.Visible = True
            pnlGraph.Visible = True
            lblNoResults.Visible = False
            lblDateCount.Text = "The date 150 days ago: <b>" & Format(lad_150, "MM/dd/yy") & _
                "</b>, and 240 days ago: <b>" & Format(lad_240, "MM/dd/yy") & "</b>"
            lblDateCount.Visible = True
            lblRecordCount.Text = "Number of records returned: <b>" & gvCRP.Rows.Count.ToString & "</b>"
            lblRecordCount.Visible = True
        ElseIf gvCRP.Rows.Count > 1 Then
            For rowIndex As Integer = gvCRP.Rows.Count - 2 To 0 Step -1
                gvRow = gvCRP.Rows(rowIndex)
                Dim gvPreviousRow As GridViewRow = gvCRP.Rows(rowIndex + 1)

                If gvRow.Cells(0).Text = gvPreviousRow.Cells(0).Text Then
                    If gvPreviousRow.Cells(0).RowSpan < 2 Then
                        gvRow.Cells(0).RowSpan = 2
                        gvRow.Cells(1).RowSpan = 2
                        gvRow.Cells(2).RowSpan = 2
                        gvRow.Cells(3).RowSpan = 2
                    Else
                        gvRow.Cells(0).RowSpan = gvPreviousRow.Cells(0).RowSpan + 1
                        gvRow.Cells(1).RowSpan = gvPreviousRow.Cells(1).RowSpan + 1
                        gvRow.Cells(2).RowSpan = gvPreviousRow.Cells(2).RowSpan + 1
                        gvRow.Cells(3).RowSpan = gvPreviousRow.Cells(2).RowSpan + 1
                    End If
                    gvPreviousRow.Cells(0).Visible = False
                    gvPreviousRow.Cells(1).Visible = False
                    gvPreviousRow.Cells(2).Visible = False
                    gvPreviousRow.Cells(3).Visible = False
                End If

                cat_no = gvRow.Cells(7).Text
                work_period_dt = gvRow.Cells(13).Text
                rev_period_dt = gvRow.Cells(14).Text
                work_period_end_dt = gvRow.Cells(15).Text
                check_period_end_dt = gvRow.Cells(16).Text
                rev_period_end_dt = gvRow.Cells(17).Text

                Try
                    lad_cell_va = gvRow.Cells(11).Text

                    If lad_cell_va > lad_150 And cat_no = "1" Then
                        gvRow.Cells(11).BackColor = Drawing.Color.PaleGoldenrod
                    ElseIf lad_cell_va > lad_240 And cat_no = "2" Then
                        gvRow.Cells(11).BackColor = Drawing.Color.PaleGoldenrod
                    Else
                        gvRow.Cells(11).BackColor = Drawing.Color.White
                    End If
                Catch ex As Exception
                    gvRow.Cells(11).BackColor = Drawing.Color.White
                End Try

                If work_period_dt = rev_period_dt Then
                    Select Case cat_no
                        Case "1"

                            Try
                                If work_period_end_dt > lad_150 Then
                                    gvRow.Cells(8).BackColor = Drawing.Color.Olive
                                    gvRow.Cells(9).BackColor = Drawing.Color.Olive
                                    gvRow.Cells(10).BackColor = Drawing.Color.Olive
                                Else
                                    gvRow.Cells(8).BackColor = Drawing.Color.DarkKhaki
                                    gvRow.Cells(9).BackColor = Drawing.Color.DarkKhaki
                                    gvRow.Cells(10).BackColor = Drawing.Color.DarkKhaki
                                End If
                            Catch ex As Exception
                                gvRow.Cells(8).BackColor = Drawing.Color.DarkKhaki
                                gvRow.Cells(9).BackColor = Drawing.Color.DarkKhaki
                                gvRow.Cells(10).BackColor = Drawing.Color.DarkKhaki
                            End Try

                        Case "2"

                            Try
                                If work_period_end_dt > lad_240 Then
                                    gvRow.Cells(8).BackColor = Drawing.Color.Olive
                                    gvRow.Cells(9).BackColor = Drawing.Color.Olive
                                    gvRow.Cells(10).BackColor = Drawing.Color.Olive
                                Else
                                    gvRow.Cells(8).BackColor = Drawing.Color.DarkKhaki
                                    gvRow.Cells(9).BackColor = Drawing.Color.DarkKhaki
                                    gvRow.Cells(10).BackColor = Drawing.Color.DarkKhaki
                                End If
                            Catch ex As Exception
                                gvRow.Cells(8).BackColor = Drawing.Color.DarkKhaki
                                gvRow.Cells(9).BackColor = Drawing.Color.DarkKhaki
                                gvRow.Cells(10).BackColor = Drawing.Color.DarkKhaki
                            End Try

                        Case Else
                            gvRow.Cells(8).BackColor = Drawing.Color.DarkKhaki
                            gvRow.Cells(9).BackColor = Drawing.Color.DarkKhaki
                            gvRow.Cells(10).BackColor = Drawing.Color.DarkKhaki
                    End Select
                Else
                    Select Case cat_no
                        Case "1"

                            Try
                                If work_period_end_dt > lad_150 Then
                                    gvRow.Cells(8).BackColor = Drawing.Color.Wheat
                                Else
                                    gvRow.Cells(8).BackColor = Drawing.Color.White
                                End If
                            Catch ex As Exception
                                gvRow.Cells(8).BackColor = Drawing.Color.White
                            End Try

                            Try
                                If check_period_end_dt > lad_150 Then
                                    gvRow.Cells(9).BackColor = Drawing.Color.Wheat
                                Else
                                    gvRow.Cells(9).BackColor = Drawing.Color.White
                                End If
                            Catch ex As Exception
                                gvRow.Cells(9).BackColor = Drawing.Color.White
                            End Try

                            Try
                                If rev_period_end_dt > lad_150 Then
                                    gvRow.Cells(10).BackColor = Drawing.Color.Wheat
                                Else
                                    gvRow.Cells(10).BackColor = Drawing.Color.White
                                End If
                            Catch ex As Exception
                                gvRow.Cells(10).BackColor = Drawing.Color.White
                            End Try

                        Case "2"

                            Try
                                If work_period_end_dt > lad_240 Then
                                    gvRow.Cells(8).BackColor = Drawing.Color.Wheat
                                Else
                                    gvRow.Cells(8).BackColor = Drawing.Color.White
                                End If
                            Catch ex As Exception
                                gvRow.Cells(8).BackColor = Drawing.Color.White
                            End Try

                            Try
                                If check_period_end_dt > lad_240 Then
                                    gvRow.Cells(9).BackColor = Drawing.Color.Wheat
                                Else
                                    gvRow.Cells(9).BackColor = Drawing.Color.White
                                End If
                            Catch ex As Exception
                                gvRow.Cells(9).BackColor = Drawing.Color.White
                            End Try

                            Try
                                If rev_period_end_dt > lad_240 Then
                                    gvRow.Cells(10).BackColor = Drawing.Color.Wheat
                                Else
                                    gvRow.Cells(10).BackColor = Drawing.Color.White
                                End If
                            Catch ex As Exception
                                gvRow.Cells(10).BackColor = Drawing.Color.White
                            End Try

                        Case Else
                            gvRow.Cells(8).BackColor = Drawing.Color.White
                            gvRow.Cells(9).BackColor = Drawing.Color.White
                            gvRow.Cells(10).BackColor = Drawing.Color.White
                    End Select
                End If

            Next

            gvRow = gvCRP.Rows(gvCRP.Rows.Count - 1)

            cat_no = gvRow.Cells(7).Text

            Try
                lad_cell_va = gvRow.Cells(11).Text

                If lad_cell_va > lad_150 And cat_no = "1" Then
                    gvRow.Cells(11).BackColor = Drawing.Color.PaleGoldenrod
                ElseIf lad_cell_va > lad_240 And cat_no = "2" Then
                    gvRow.Cells(11).BackColor = Drawing.Color.PaleGoldenrod
                Else
                    gvRow.Cells(11).BackColor = Drawing.Color.White
                End If
            Catch ex As Exception
                gvRow.Cells(11).BackColor = Drawing.Color.White
            End Try

            work_period_dt = gvRow.Cells(13).Text
            rev_period_dt = gvRow.Cells(14).Text
            work_period_end_dt = gvRow.Cells(15).Text
            check_period_end_dt = gvRow.Cells(16).Text
            rev_period_end_dt = gvRow.Cells(17).Text

            If work_period_dt = rev_period_dt Then
                Select Case cat_no
                    Case "1"

                        Try
                            If work_period_end_dt > lad_150 Then
                                gvRow.Cells(8).BackColor = Drawing.Color.Olive
                                gvRow.Cells(9).BackColor = Drawing.Color.Olive
                                gvRow.Cells(10).BackColor = Drawing.Color.Olive
                            Else
                                gvRow.Cells(8).BackColor = Drawing.Color.DarkKhaki
                                gvRow.Cells(9).BackColor = Drawing.Color.DarkKhaki
                                gvRow.Cells(10).BackColor = Drawing.Color.DarkKhaki
                            End If
                        Catch ex As Exception
                            gvRow.Cells(8).BackColor = Drawing.Color.DarkKhaki
                            gvRow.Cells(9).BackColor = Drawing.Color.DarkKhaki
                            gvRow.Cells(10).BackColor = Drawing.Color.DarkKhaki
                        End Try

                    Case "2"

                        Try
                            If work_period_end_dt > lad_240 Then
                                gvRow.Cells(8).BackColor = Drawing.Color.Olive
                                gvRow.Cells(9).BackColor = Drawing.Color.Olive
                                gvRow.Cells(10).BackColor = Drawing.Color.Olive
                            Else
                                gvRow.Cells(8).BackColor = Drawing.Color.DarkKhaki
                                gvRow.Cells(9).BackColor = Drawing.Color.DarkKhaki
                                gvRow.Cells(10).BackColor = Drawing.Color.DarkKhaki
                            End If
                        Catch ex As Exception
                            gvRow.Cells(8).BackColor = Drawing.Color.DarkKhaki
                            gvRow.Cells(9).BackColor = Drawing.Color.DarkKhaki
                            gvRow.Cells(10).BackColor = Drawing.Color.DarkKhaki
                        End Try

                    Case Else
                        gvRow.Cells(8).BackColor = Drawing.Color.DarkKhaki
                        gvRow.Cells(9).BackColor = Drawing.Color.DarkKhaki
                        gvRow.Cells(10).BackColor = Drawing.Color.DarkKhaki
                End Select
            Else
                Select Case cat_no
                    Case "1"

                        Try
                            If work_period_end_dt > lad_150 Then
                                gvRow.Cells(8).BackColor = Drawing.Color.Wheat
                            Else
                                gvRow.Cells(8).BackColor = Drawing.Color.White
                            End If
                        Catch ex As Exception
                            gvRow.Cells(8).BackColor = Drawing.Color.White
                        End Try

                        Try
                            If check_period_end_dt > lad_150 Then
                                gvRow.Cells(9).BackColor = Drawing.Color.Wheat
                            Else
                                gvRow.Cells(9).BackColor = Drawing.Color.White
                            End If
                        Catch ex As Exception
                            gvRow.Cells(9).BackColor = Drawing.Color.White
                        End Try

                        Try
                            If rev_period_end_dt > lad_150 Then
                                gvRow.Cells(10).BackColor = Drawing.Color.Wheat
                            Else
                                gvRow.Cells(10).BackColor = Drawing.Color.White
                            End If
                        Catch ex As Exception
                            gvRow.Cells(10).BackColor = Drawing.Color.White
                        End Try

                    Case "2"

                        Try
                            If work_period_end_dt > lad_240 Then
                                gvRow.Cells(8).BackColor = Drawing.Color.Wheat
                            Else
                                gvRow.Cells(8).BackColor = Drawing.Color.White
                            End If
                        Catch ex As Exception
                            gvRow.Cells(8).BackColor = Drawing.Color.White
                        End Try

                        Try
                            If check_period_end_dt > lad_240 Then
                                gvRow.Cells(9).BackColor = Drawing.Color.Wheat
                            Else
                                gvRow.Cells(9).BackColor = Drawing.Color.White
                            End If
                        Catch ex As Exception
                            gvRow.Cells(9).BackColor = Drawing.Color.White
                        End Try

                        Try
                            If rev_period_end_dt > lad_240 Then
                                gvRow.Cells(10).BackColor = Drawing.Color.Wheat
                            Else
                                gvRow.Cells(10).BackColor = Drawing.Color.White
                            End If
                        Catch ex As Exception
                            gvRow.Cells(10).BackColor = Drawing.Color.White
                        End Try

                    Case Else
                        gvRow.Cells(8).BackColor = Drawing.Color.White
                        gvRow.Cells(9).BackColor = Drawing.Color.White
                        gvRow.Cells(10).BackColor = Drawing.Color.White
                End Select
            End If

            gvRow.Cells(13).Visible = False
            gvRow.Cells(14).Visible = False
            gvRow.Cells(15).Visible = False
            gvRow.Cells(16).Visible = False
            gvRow.Cells(17).Visible = False
            gvCRP.Visible = True
            pnlExplanation.Visible = True
            pnlGraph.Visible = True
            lblDateCount.Text = "The date 150 days ago: <b>" & Format(lad_150, "MM/dd/yy") & _
                "</b>, and 240 days ago: <b>" & Format(lad_240, "MM/dd/yy") & "</b>"
            lblDateCount.Visible = True
            lblRecordCount.Text = "Number of records returned: <b>" & gvCRP.Rows.Count.ToString & "</b>"
            lblRecordCount.Visible = True
            lblNoResults.Visible = False
        Else
            gvCRP.Visible = False
            pnlExplanation.Visible = False
            pnlGraph.Visible = False
            lblNoResults.Visible = True
            lblDateCount.Visible = False
            lblRecordCount.Visible = False
            If ddlWSC.SelectedIndex > 0 Then
                lblNoResults.Text = "No sites found for this WSC/office"
            Else
                lblNoResults.Text = "Please choose a WSC to begin"
            End If
        End If
    End Sub

    Protected Sub rblOnlyActive_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rblOnlyActive.SelectedIndexChanged
        gvCRP.DataBind()
    End Sub
End Class
