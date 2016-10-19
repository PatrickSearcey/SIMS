Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.HttpContext
Imports System.Web.HttpRequest
Imports System.Net
Imports System.Web.UI.WebControls
Imports Telerik.Web.UI

Public Class RecordConfigList
    Inherits System.Web.UI.Page

    Private o As Office
    Private w As WSC
    Private uid As String = Current.User.Identity.Name
    Private u As New User(uid)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Master.PageTitle = "Modify Record Configurations"
        Page.Title = "Modify Record Configurations"

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

        If Not Page.IsPostBack Then
            '--PAGE ACCESS SECTION-------------------------------------------------------------
            Master.CheckAccessLevel(wsc_id, "None")
            If Master.NoAccessPanel Then
                pnlHasAccess.Visible = False
            Else
                pnlHasAccess.Visible = True
            End If
            '--END PAGE ACCESS SECTION---------------------------------------------------------

            rblOnlyActive.SelectedValue = "yes"
            PopulateWSCDDL()
        End If
    End Sub

#Region "Properties"
    Private ReadOnly Property RecordConfigs() As DataTable
        Get
            Using cnx As New SqlConnection(Config.ConnectionInfo)
                cnx.Open()
                Dim dt As New DataTable()
                Dim cmd As New SqlCommand("SP_Record_Details_by_WSC_or_office", cnx)
                cmd.CommandType = Data.CommandType.StoredProcedure
                cmd.Parameters.Add("@office_id", SqlDbType.Int).Value = ddlOffice.SelectedValue
                cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = ddlWSC.SelectedValue
                cmd.Parameters.Add("@onlyactive", SqlDbType.NVarChar, 3).Value = rblOnlyActive.SelectedValue

                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)

                RecordConfigs = dt
                cnx.Close()
            End Using
        End Get
    End Property
#End Region

    Public Sub PopulateWSCDDL()
        Dim sql As String = Nothing
        Dim dbcomm As SqlCommand
        Dim WSCList As SqlDataReader

        Using cnx As New SqlConnection(Config.ConnectionInfo)
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
        End Using

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
        If ddlWSC.SelectedValue.ToString = "0" Then
            PopulateWSCDDL()
        Else
            o = New Office(0)
            w = New WSC(ddlWSC.SelectedValue)
            PopulateOfficeDDL(ddlWSC.SelectedItem.Value.ToString)
            pnlOffice.Visible = True
            pnlOnlyActive.Visible = True
            rgRecs.Rebind()
        End If
    End Sub

    Protected Sub ddlOffice_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlOffice.SelectedIndexChanged
        If ddlOffice.SelectedItem.Value IsNot Nothing And ddlOffice.SelectedItem.Value <> "" Then
            o = New Office(ddlOffice.SelectedItem.Value)
        Else
            o = New Office(0)
        End If
        rgRecs.Rebind()
    End Sub

    Protected Sub rblOnlyActive_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rblOnlyActive.SelectedIndexChanged
        rgRecs.Rebind()
    End Sub

    Protected Sub rgRecs_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs) Handles rgRecs.NeedDataSource
        rgRecs.DataSource = RecordConfigs
    End Sub

    Protected Sub rgRecs_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles rgRecs.DataBound
        Dim recordCount As Integer = Nothing
        Dim gvRow As GridDataItem = Nothing

        If rgRecs.MasterTableView.Items.Count > 1 Then
            For rowIndex As Integer = rgRecs.MasterTableView.Items.Count - 2 To 0 Step -1
                gvRow = rgRecs.MasterTableView.Items(rowIndex)
                Dim gvPreviousRow As GridDataItem = rgRecs.MasterTableView.Items(rowIndex + 1)

                If gvRow.Cells(13).Text = gvPreviousRow.Cells(13).Text Then
                    If gvPreviousRow.Cells(3).RowSpan < 2 Then
                        gvRow.Cells(2).RowSpan = 2
                        gvRow.Cells(3).RowSpan = 2
                    Else
                        gvRow.Cells(2).RowSpan = gvPreviousRow.Cells(2).RowSpan + 1
                        gvRow.Cells(3).RowSpan = gvPreviousRow.Cells(3).RowSpan + 1
                    End If
                    gvPreviousRow.Cells(2).Visible = False
                    gvPreviousRow.Cells(3).Visible = False
                End If

                If gvRow.Cells(12).Text = "True" Then
                    gvRow.Cells(4).BackColor = Drawing.Color.PeachPuff
                    gvRow.Cells(5).BackColor = Drawing.Color.PeachPuff
                    gvRow.Cells(6).BackColor = Drawing.Color.PeachPuff
                    gvRow.Cells(7).BackColor = Drawing.Color.PeachPuff
                    gvRow.Cells(8).BackColor = Drawing.Color.PeachPuff
                    gvRow.Cells(9).BackColor = Drawing.Color.PeachPuff
                    gvRow.Cells(10).BackColor = Drawing.Color.PeachPuff
                    gvRow.Cells(11).BackColor = Drawing.Color.PeachPuff
                    gvRow.Cells(12).BackColor = Drawing.Color.PeachPuff
                End If
            Next
        End If

        lblRecordCount.Text = "Number of records returned: " & rgRecs.MasterTableView.Items.Count.ToString
        lblRecordCount.Visible = True
    End Sub

    Protected Sub rgRecs_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles rgRecs.PreRender
        Dim menu As GridFilterMenu = rgRecs.FilterMenu
        Dim i As Integer = 0
        While i < menu.Items.Count
            If menu.Items(i).Text = "NoFilter" Or _
               menu.Items(i).Text = "Contains" Or _
               menu.Items(i).Text = "EqualTo" Or _
               menu.Items(i).Text = "GreaterThan" Or _
               menu.Items(i).Text = "LessThan" Or _
               menu.Items(i).Text = "NotIsEmpty" Then
                i = i + 1
            Else
                menu.Items.RemoveAt(i)
            End If
        End While
    End Sub
End Class
