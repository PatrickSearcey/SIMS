Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

Public Class SHAJHA
    Private _id As Integer
    Private _sha_site_id As Integer
    Private _elem_jha_id As Integer
    Private _description As String

    Public Sub New(ByVal site_jha_id As Integer)
        _id = site_jha_id

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim sql As String = "SELECT ssj.site_jha_id, ssj.sha_site_id, ssj.elem_jha_id, slj.jha_description" & _
                " FROM SHA_Lut_ElemJHA AS slej INNER JOIN" & _
                " SHA_Lut_JHA AS slj ON slej.jha_id = slj.jha_id INNER JOIN" & _
                " SHA_Site_JHA AS ssj ON slej.elem_jha_id = ssj.elem_jha_id" & _
                " WHERE (site_jha_id = " & _id & ")"

            Dim cmd As SqlCommand = New SqlCommand(sql, cnx)
            Dim da As SqlDataAdapter
            Dim dt As DataTable = New DataTable
            da = New SqlDataAdapter(cmd)
            da.Fill(dt)

            If dt.Rows.Count = 0 Then
                _sha_site_id = Nothing
                _elem_jha_id = Nothing
                _description = Nothing
            Else
                For Each row As DataRow In dt.Rows
                    _sha_site_id = row("sha_site_id")
                    _elem_jha_id = row("elem_jha_id")
                    _description = row("jha_description").ToString
                Next
            End If

            cnx.Close()
        End Using
    End Sub

#Region "Class Properties"
    Public Property ID() As Integer
        Get
            ID = _id
        End Get
        Set(value As Integer)
            _id = value
        End Set
    End Property
    Public Property SHASiteID() As Integer
        Get
            SHASiteID = _sha_site_id
        End Get
        Set(value As Integer)
            _sha_site_id = value
        End Set
    End Property
    Public Property ElementJHAID() As Integer
        Get
            ElementJHAID = _elem_jha_id
        End Get
        Set(value As Integer)
            _elem_jha_id = value
        End Set
    End Property
    Public Property Description() As String
        Get
            Description = _description
        End Get
        Set(value As String)
            _description = value
        End Set
    End Property
#End Region

#Region "Class Methods"
    Public Function InsertSiteJHA(ByVal userid As String) As String
        Dim errmessage As String = "success"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim sql As String = "INSERT INTO SHA_Site_JHA" & _
                " (sha_site_id, elem_jha_id)" & _
                " VALUES (" & SHASiteID & ", " & ElementJHAID & ")"
                Dim cmd As New SqlCommand(sql, cnx)
                cmd.ExecuteNonQuery()
            Catch ex As Exception
                errmessage = ex.Message
            End Try

            cnx.Close()
        End Using

        Return errmessage
    End Function

    Public Function DeleteJHA(ByVal userid As String) As String
        Dim errmessage As String = "success"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim cmd As SqlCommand = New SqlCommand("SP_Safety_Info_Delete", cnx)
                cmd.CommandType = Data.CommandType.StoredProcedure

                cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = 0
                cmd.Parameters.Add("@type", SqlDbType.Int).Value = 0
                cmd.Parameters.Add("@site_jha_id", SqlDbType.Int).Value = ID
                cmd.Parameters.Add("@backup_by", SqlDbType.NVarChar, 15).Value = userid
                cmd.Parameters.Add("@backup_dt", SqlDbType.DateTime).Value = Now()
                cmd.Parameters.Add("@action", SqlDbType.NVarChar, 15).Value = "deleteJHA"

                cmd.ExecuteNonQuery()
            Catch ex As Exception
                errmessage = ex.Message
            End Try

            cnx.Close()
        End Using

        Return errmessage
    End Function
#End Region
End Class
