Imports System.Web
Imports System.Web.Services
Imports System.Data.SqlClient

Public Class CreateSHA
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim site_id As String = context.Request.QueryString("site_id")
        Dim ref_page As String = context.Request.QueryString("ref")

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim sql As String = Nothing
            Dim dt As DataTable = New DataTable()

            sql = "INSERT INTO SHA_Site_Master" & _
                  " (site_id)" & _
                  " VALUES (" & site_id & ")"

            Dim cmd As New SqlCommand(sql, cnx)
            cmd.ExecuteNonQuery()

            cnx.Close()
        End Using

        'If coming from the SHA Status Report page, take them directly to the newly created SHA
        If ref_page = "" Then
            context.Response.Redirect("/SIMSClassic/StationInfo.asp?site_id=" & site_id)
        Else
            context.Response.Redirect("SiteHazardAnalysis.aspx?site_id=" & site_id)
        End If

    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class