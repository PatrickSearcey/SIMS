Imports System.Data.SqlClient

Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim site_no As String = Request.QueryString("site_no")

        If Not String.IsNullOrEmpty(site_no) Then
            Response.Redirect("/SIMSClassic/StationInfo.asp?site_no=" & site_no & "&agency_cd=USGS")
        End If

        If Not Page.IsPostBack Then
            Dim dt As New DataTable()

            Using cnx As New SqlConnection(Config.ConnectionInfo)
                cnx.Open()
                Dim sql As String = "SELECT wsc_id, wsc_nm, wsc_cd" & _
                    " FROM lut_WSC" & _
                    " GROUP BY wsc_id, wsc_nm, wsc_cd" & _
                    " ORDER BY wsc_nm"
                Dim cmd As New SqlCommand(sql, cnx)
                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)

                ddlWSC.DataSource = dt
                ddlWSC.DataBind()

                ddlWSC.Items.Insert(0, New ListItem(String.Empty, String.Empty))
                ddlWSC.SelectedIndex = 0

                cnx.Close()
            End Using
        End If
    End Sub

    Protected Sub btnGo_OnCommand(ByVal sender As Object, ByVal e As CommandEventArgs)
        If Not tbSiteNo.Text = Nothing Then
            Response.Redirect("/SIMSClassic/StationInfo.asp?site_no=" & tbSiteNo.Text & "&agency_cd=" & tbAgencyCd.Text)
        End If
    End Sub

    Protected Sub ddlWSC_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Response.Redirect("/SIMSClassic/StationsRpts.asp?wsc_id=" & ddlWSC.SelectedValue.ToString())
    End Sub

End Class