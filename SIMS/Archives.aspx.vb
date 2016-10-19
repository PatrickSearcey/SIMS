Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb

Public Class Archives
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Master.PageTitle = "Retrieve Archived Elements"
        Master.ResponsibleOffice = True
        Page.Title = "SIMS - Archives Search Interface"
        Master.LogonInfo = False

        Dim office_id As Integer = Nothing
        Dim site_id As Integer = Nothing
        Dim site_no As String = Nothing
        Dim agency_cd As String = Nothing
        Dim element_id As Integer = Request.QueryString("element_id")
        Dim element_nm As String = Request.QueryString("element_nm")
        Dim s As Site

        If Request.QueryString("office_id") <> "" Then
            office_id = Request.QueryString("office_id")
            Session("office_id") = office_id
        Else
            office_id = Session("office_id")
        End If

        If Request.QueryString("site_id") <> "" Then
            site_id = Request.QueryString("site_id")
            Session("site_id") = site_id
        ElseIf Request.QueryString("site_no") <> "" And Request.QueryString("agency_cd") <> "" Then
            s = New Site(Request.QueryString("site_no"), Request.QueryString("agency_cd"))
            site_id = s.ID
            Session("site_id") = site_id
        Else
            site_id = Session("site_id")
        End If

        If Request.QueryString("site_no") <> "" Then
            site_no = Request.QueryString("site_no")
            Session("site_no") = site_no
        ElseIf Request.QueryString("site_id") <> "" Then
            s = New Site(Request.QueryString("site_id"))
            site_no = s.Number
            Session("site_no") = site_no
        Else
            site_no = Session("site_no")
        End If

        If Request.QueryString("agency_cd") <> "" Then
            agency_cd = Request.QueryString("agency_cd")
            Session("agency_cd") = agency_cd
        ElseIf Request.QueryString("site_id") <> "" Then
            s = New Site(Request.QueryString("site_id"))
            agency_cd = s.AgencyCode
            Session("agency_cd") = agency_cd
        Else
            agency_cd = Session("agency_cd")
        End If

        If element_id <> Nothing Then
            ShowArchivedInfo(element_id, element_nm)
            pnlStep1.Visible = False
            pnlStep2.Visible = False
            pnlStep3.Visible = True
            lbBack2.Visible = False
            lbBack3.Visible = False
        End If

    End Sub

    Public Sub ElementSelected(ByVal sender As Object, ByVal Args As CommandEventArgs)
        Dim element_id As Integer = Args.CommandName

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim sql As String = "SELECT eled.element_nm," & _
                " (CASE WHEN MIN(eseb.revised_dt) IS NULL THEN CONVERT(nvarchar,MIN(eseb.backup_dt),101) ELSE CONVERT(nvarchar,MIN(eseb.revised_dt),101) END) As first_revised," & _
                " (CASE WHEN MAX(eseb.revised_dt) IS NULL THEN CONVERT(nvarchar,MAX(eseb.backup_dt),101) ELSE CONVERT(nvarchar,MAX(eseb.revised_dt),101) END) As last_revised," & _
                " COUNT(*) AS revisions" & _
                " FROM Elem_Site_Element_Backup AS eseb INNER JOIN" & _
                " Elem_Lut_ElemDetail AS eled ON eseb.element_id = eled.element_id" & _
                " WHERE (eled.element_id = " & element_id & ") And (eseb.site_id = " & Session("site_id") & ")" & _
                " And (eseb.element_info Is Not Null)" & _
                " GROUP BY eled.priority, eled.element_nm"
            Dim cmd As SqlCommand = New SqlCommand(sql, cnx)
            Dim dr As SqlDataReader
            dr = cmd.ExecuteReader
            dr.Read()

            lblElementName.Text = dr.GetString(0)
            tbBeginDate.Text = dr.GetString(1)
            tbEndDate.Text = dr.GetString(2)
            btnRetrieve.CommandName = element_id

            dr.Close()
            cnx.Close()
        End Using

        pnlStep1.Visible = False
        pnlStep2.Visible = True
        pnlStep3.Visible = False
    End Sub

    Public Sub RetrieveArchivedInfo(ByVal sender As Object, ByVal Args As CommandEventArgs)
        If Page.IsValid Then
            lblElementName2.Text = lblElementName.Text

            Using cnx As New SqlConnection(Config.ConnectionInfo)
                cnx.Open()
                Dim cmd As SqlCommand = New SqlCommand("SP_Element_Info_Archives_by_element_id", cnx)
                Dim dr As SqlDataReader
                cmd.CommandType = Data.CommandType.StoredProcedure
                cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = Session("site_id")
                cmd.Parameters.Add("@element_id", SqlDbType.Int).Value = Args.CommandName
                cmd.Parameters.Add("@begin_dt", SqlDbType.DateTime).Value = tbBeginDate.Text
                cmd.Parameters.Add("@end_dt", SqlDbType.DateTime).Value = tbEndDate.Text
                dr = cmd.ExecuteReader

                If dr.HasRows Then
                    rptElementInfo.DataSource = dr
                    rptElementInfo.DataBind()
                    lblNothingReturned.Visible = False
                    rptElementInfo.Visible = True
                Else
                    rptElementInfo.Visible = False
                    lblNothingReturned.Visible = True
                End If

                dr.Close()
                cnx.Close()
            End Using

            pnlStep1.Visible = False
            pnlStep2.Visible = False
            pnlStep3.Visible = True
        End If
    End Sub

    Public Sub ShowArchivedInfo(ByVal element_id As Integer, ByVal element_nm As String)
        lblElementName2.Text = element_nm

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As SqlCommand = New SqlCommand("SP_Element_Info_Archives_by_element_id", cnx)
            Dim dr As SqlDataReader
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = Session("site_id")
            cmd.Parameters.Add("@element_id", SqlDbType.Int).Value = element_id
            cmd.Parameters.Add("@begin_dt", SqlDbType.DateTime).Value = "1/1/1990"
            cmd.Parameters.Add("@end_dt", SqlDbType.DateTime).Value = Now()
            dr = cmd.ExecuteReader

            If dr.HasRows Then
                rptElementInfo.DataSource = dr
                rptElementInfo.DataBind()
                lblNothingReturned.Visible = False
                rptElementInfo.Visible = True
            Else
                rptElementInfo.Visible = False
                lblNothingReturned.Text = "No archive information exists for this element"
                lblNothingReturned.Visible = True
            End If

            dr.Close()
            cnx.Close()
        End Using

    End Sub

    Public Sub Back_Command(ByVal sender As Object, ByVal Args As CommandEventArgs)
        If Args.CommandName = "back2" Then
            pnlStep1.Visible = False
            pnlStep2.Visible = True
            pnlStep3.Visible = False
        Else
            pnlStep1.Visible = True
            pnlStep2.Visible = False
            pnlStep3.Visible = False
        End If
    End Sub

    Protected Sub gvElementList_Bound(ByVal sender As Object, ByVal e As EventArgs)
        Dim gvRow As GridViewRow = Nothing

        If gvElementList.Rows.Count > 1 Then
            For rowIndex As Integer = gvElementList.Rows.Count - 2 To 0 Step -1
                gvRow = gvElementList.Rows(rowIndex)

                Dim priority As String = gvRow.Cells(5).Text
                Dim bgc As Drawing.Color

                If CInt(priority) <> 6 And CInt(priority) <> 2 Then
                    If CInt(priority) < 200 Then
                        bgc = Drawing.Color.White
                        'bgc = "bgcolor=""#FFFFFF"""
                    ElseIf CInt(priority) > 199 And CInt(priority) < 300 Then
                        bgc = Drawing.Color.LightGray
                        'bgc = "bgcolor=""#CCCCCC"""
                    ElseIf CInt(priority) > 299 Then
                        bgc = Nothing
                        'bgc = "bgcolor=""#97b2dc"""
                    End If
                Else
                    bgc = Drawing.Color.White
                End If

                gvRow.Cells(0).BackColor = bgc
                gvRow.Cells(1).BackColor = bgc
                gvRow.Cells(2).BackColor = bgc
                gvRow.Cells(3).BackColor = bgc
                gvRow.Cells(4).BackColor = bgc
            Next

        End If

    End Sub

End Class