Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

Public Class SHASpecificCond
    Private _id As Integer
    Private _site_jha_id As Integer
    Private _remarks As String
    Private _priority As Boolean

    Public Sub New(ByVal site_specificcond_id As Integer)
        _id = site_specificcond_id

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim sql As String = "SELECT site_jha_id, remarks, priority" & _
                " FROM SHA_Site_SpecificCond" & _
                " WHERE (site_specificcond_id = " & _id & ")"

            Dim cmd As SqlCommand = New SqlCommand(sql, cnx)
            Dim da As SqlDataAdapter
            Dim dt As DataTable = New DataTable
            da = New SqlDataAdapter(cmd)
            da.Fill(dt)

            If dt.Rows.Count = 0 Then
                _site_jha_id = Nothing
                _remarks = Nothing
                _priority = Nothing
            Else
                For Each row As DataRow In dt.Rows
                    _site_jha_id = row("site_jha_id")
                    _remarks = row("remarks").ToString
                    _priority = row("priority").ToString
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
    Public Property SiteJHAID() As Integer
        Get
            SiteJHAID = _site_jha_id
        End Get
        Set(value As Integer)
            _site_jha_id = value
        End Set
    End Property
    Public Property Remarks() As String
        Get
            Remarks = _remarks
        End Get
        Set(value As String)
            _remarks = value
        End Set
    End Property
    Public Property Priority() As Boolean
        Get
            Priority = _priority
        End Get
        Set(value As Boolean)
            _priority = value
        End Set
    End Property
#End Region

#Region "Class Methods"
    ''' <summary>
    ''' Updates the JHA site specific condition information in SHA_Site_SpecificCond
    ''' </summary>
    Public Function UpdateSpecificCondInfo(ByVal userid As String) As String
        Dim errmessage As String = "success"
        Dim priority_temp As String = "0"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            If Priority Then
                priority_temp = "1"
            End If

            Try
                Dim sql As String = "UPDATE SHA_Site_SpecificCond" & _
                " SET remarks = '" & Remarks & "', priority = '" & priority_temp & "'" & _
                " WHERE site_specificcond_id = " & ID
                Dim cmd As New SqlCommand(sql, cnx)
                cmd.ExecuteNonQuery()
            Catch ex As Exception
                errmessage = ex.Message
            End Try

            cnx.Close()
        End Using

        Return errmessage
    End Function

    Public Function InsertSpecificCondInfo(ByVal userid As String) As String
        Dim errmessage As String = "success"
        Dim priority_temp As String = "0"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            If Priority Then
                priority_temp = "1"
            End If

            Try
                Dim sql As String = "INSERT INTO SHA_Site_SpecificCond" & _
                " (site_jha_id, remarks, priority)" & _
                " VALUES (" & SiteJHAID & ", '" & Remarks & "', '" & priority_temp & "')"
                Dim cmd As New SqlCommand(sql, cnx)
                cmd.ExecuteNonQuery()
            Catch ex As Exception
                errmessage = ex.Message
            End Try

            cnx.Close()
        End Using

        Return errmessage
    End Function

    Public Function DeleteSpecificCond(ByVal userid As String) As String
        Dim errmessage As String = "success"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim sql As String = "DELETE FROM SHA_Site_SpecificCond" & _
                " WHERE site_specificcond_id = " & ID
                Dim cmd As New SqlCommand(sql, cnx)
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
