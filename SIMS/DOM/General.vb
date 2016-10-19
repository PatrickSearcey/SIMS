Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient

Public Class General

#Region "Class Methods"
    Public Function GetWSCList() As DataTable
        Dim dt As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As SqlCommand = New SqlCommand("SELECT lw.wsc_id, lw.wsc_nm, lw.wsc_cd" & _
                " FROM lut_Office AS lo RIGHT OUTER JOIN lut_WSC AS lw ON lo.wsc_id = lw.wsc_id" & _
                " GROUP BY lw.wsc_id, lw.wsc_nm, lw.wsc_cd" & _
                " ORDER BY lw.wsc_nm", cnx)
            Dim da As SqlDataAdapter

            da = New SqlDataAdapter(cmd)
            da.Fill(dt)

            cnx.Close()
        End Using

        Return dt
    End Function
#End Region
End Class
