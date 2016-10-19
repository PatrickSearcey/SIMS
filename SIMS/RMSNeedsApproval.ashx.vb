Imports System.Web
Imports System.Web.Services
Imports System.Data.SqlClient

Public Class RMSNeedsApproval
    Implements System.Web.IHttpHandler

    Private Const wsc_id_qs As String = "wsc"
    Private Const district_cd_qs As String = "district_cd"

    Private wsc_id As Integer
    Private district_cd As String

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Setup(context)

        createTextDump(wsc_id, context)

    End Sub

    ''' <summary>
    ''' Set up our environment based on current session or query string info.
    ''' </summary>
    ''' <param name="context"></param>
    Private Sub Setup(context As HttpContext)
        If context.Request.QueryString(wsc_id_qs) IsNot Nothing Then
            wsc_id = Convert.ToInt32(context.Request.QueryString(wsc_id_qs))
        End If
        If context.Request.QueryString(district_cd_qs) IsNot Nothing Then
            district_cd = context.Request.QueryString(district_cd_qs)
        End If
    End Sub

    Protected Sub createTextDump(wsc_id As String, context As HttpContext)
        context.Response.ContentType = "text/plain"
        context.Response.Write("wsc_id" & vbTab)
        context.Response.Write("district_cd" & vbTab)
        context.Response.Write("office_id" & vbTab)
        context.Response.Write("office_cd" & vbTab)
        context.Response.Write("site_no" & vbTab)
        context.Response.Write("dd_nu" & vbTab)
        context.Response.Write("type_cd" & vbTab)
        context.Response.Write("type_ds" & vbTab)
        context.Response.Write("operator_va" & vbTab)
        context.Response.Write("assigned_reviewer_uid" & vbTab)
        context.Response.Write("assigned_checker_uid" & vbTab)
        context.Response.Write("last_aging_dt" & vbTab)
        context.Response.Write("rev_period_end_dt" & vbTab)
        context.Response.Write(vbNewLine)

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim where_stmt = ""
            If Not String.IsNullOrEmpty(wsc_id) Then
                where_stmt = " WHERE wsc_id = " & wsc_id
            End If
            If Not String.IsNullOrEmpty(district_cd) Then
                where_stmt = " WHERE district_cd = '" & district_cd & "'"
            End If

            Dim cmd As New SqlCommand("SELECT * FROM vRMS_Needs_Approval" & where_stmt, cnx)
            Dim dt As New DataTable

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                context.Response.Write(row("wsc_id").ToString() & vbTab)
                context.Response.Write(row("district_cd").ToString() & vbTab)
                context.Response.Write(row("office_id").ToString() & vbTab)
                context.Response.Write(row("office_cd").ToString() & vbTab)
                context.Response.Write(row("site_no").ToString() & vbTab)
                context.Response.Write(row("dd_nu").ToString() & vbTab)
                context.Response.Write(row("type_cd").ToString() & vbTab)
                context.Response.Write(row("type_ds").ToString() & vbTab)
                context.Response.Write(row("operator_va").ToString() & vbTab)
                context.Response.Write(row("assigned_reviewer_uid").ToString() & vbTab)
                context.Response.Write(row("assigned_checker_uid").ToString() & vbTab)
                context.Response.Write(row("last_aging_dt").ToString() & vbTab)
                context.Response.Write(row("rev_period_end_dt").ToString() & vbTab)
                context.Response.Write(vbNewLine)
            Next
            cnx.Close()
        End Using

        context.Response.Flush()
        context.Response.End()
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class