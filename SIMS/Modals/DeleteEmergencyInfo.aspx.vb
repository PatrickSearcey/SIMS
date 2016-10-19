Imports System.Web.UI.WebControls
Imports System.Data.SqlClient
Imports System.Web.HttpContext
Imports System.Data

Public Class DeleteEmergencyInfo
    Inherits System.Web.UI.Page

    Private h As Hospital
    Private c As EmergContact

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not String.IsNullOrEmpty(Request.QueryString("hospital_id")) Then
            h = New Hospital(Request.QueryString("hospital_id"))
        ElseIf Not String.IsNullOrEmpty(Request.QueryString("contact_id")) Then
            c = New EmergContact(Request.QueryString("contact_id"))
        End If
    End Sub

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.Page.Title = "Deleting emergency info"
        hfStatus.Value = "go"
        If Not String.IsNullOrEmpty(Request.QueryString("hospital_id")) Then
            If Not String.IsNullOrEmpty(Request.QueryString("sha_site_id")) Then
                pnlHospitalForSite.Visible = True
                pnlContactForSite.Visible = False
                pnlHospital.Visible = False
                pnlContact.Visible = False
            Else
                pnlHospital.Visible = True
                pnlContact.Visible = False
                pnlHospitalForSite.Visible = False
                pnlContactForSite.Visible = False
            End If
        Else
            If Not String.IsNullOrEmpty(Request.QueryString("sha_site_id")) Then
                pnlHospitalForSite.Visible = False
                pnlContactForSite.Visible = True
                pnlHospital.Visible = False
                pnlContact.Visible = False
            Else
                pnlHospital.Visible = False
                pnlContact.Visible = True
                pnlHospitalForSite.Visible = False
                pnlContactForSite.Visible = False
            End If
        End If
    End Sub

    Public Sub DeleteInfo(ByVal sender As Object, ByVal e As CommandEventArgs)
        Dim sql1 As String = ""
        Dim sql2 As String = ""

        If hfStatus.Value = "go" Then
            If e.CommandArgument = "deletehospital" Then
                sql1 = "DELETE FROM SHA_Site_Hospital WHERE hospital_id = " & h.ID
                sql2 = "DELETE FROM lut_Hospital WHERE hospital_id = " & h.ID
            ElseIf e.CommandArgument = "deletecontact" Then
                sql1 = "DELETE FROM SHA_Site_Contact WHERE contact_id = " & c.ID
                sql2 = "DELETE FROM lut_Contact WHERE contact_id = " & c.ID
            ElseIf e.CommandArgument = "deletehospitalforsite" Then
                sql1 = "DELETE FROM SHA_Site_Hospital WHERE hospital_id = " & h.ID & " AND sha_site_id = " & Request.QueryString("sha_site_id")
                sql2 = ""
            ElseIf e.CommandArgument = "deletecontactforsite" Then
                sql1 = "DELETE FROM SHA_Site_Contact WHERE contact_id = " & c.ID & " AND sha_site_id = " & Request.QueryString("sha_site_id")
                sql2 = ""
            End If

            Using cnx As New SqlConnection(Config.ConnectionInfo)
                cnx.Open()

                Dim cmd1 As SqlCommand
                cmd1 = New SqlCommand(sql1, cnx)
                cmd1.ExecuteNonQuery()

                If sql2 <> "" Then
                    Dim cmd2 As SqlCommand
                    cmd2 = New SqlCommand(sql2, cnx)
                    cmd2.ExecuteNonQuery()
                End If

                hfStatus.Value = "done"

                cnx.Close()
            End Using
        End If

        ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "CloseAndRebind();", True)
    End Sub

End Class