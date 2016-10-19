Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

Public Class SHARefLevel
    Private _id As Integer
    Private _site_jha_id As Integer
    Private _reflevel_id As Integer
    Private _reflevel_va As Double
    Private _reflevel_units As String
    Private _remarks As String

    Public Sub New(ByVal site_reflevel_id As Integer)
        _id = site_reflevel_id

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim sql As String = "SELECT site_jha_id, reflevel_id, reflevel_va, reflevel_units, remarks" & _
                " FROM SHA_Site_RefLevel" & _
                " WHERE (site_reflevel_id = " & _id & ")"

            Dim cmd As SqlCommand = New SqlCommand(sql, cnx)
            Dim da As SqlDataAdapter
            Dim dt As DataTable = New DataTable
            da = New SqlDataAdapter(cmd)
            da.Fill(dt)

            If dt.Rows.Count = 0 Then
                _site_jha_id = Nothing
                _reflevel_id = Nothing
                _reflevel_va = Nothing
                _reflevel_units = Nothing
                _remarks = Nothing
            Else
                For Each row As DataRow In dt.Rows
                    _site_jha_id = row("site_jha_id")
                    _reflevel_id = row("reflevel_id")
                    _reflevel_va = row("reflevel_va")
                    _reflevel_units = row("reflevel_units").ToString
                    _remarks = row("remarks").ToString
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
    Public Property RefLevelID() As Integer
        Get
            RefLevelID = _reflevel_id
        End Get
        Set(value As Integer)
            _reflevel_id = value
        End Set
    End Property
    Public Property RefLevelValue() As Double
        Get
            RefLevelValue = _reflevel_va
        End Get
        Set(value As Double)
            _reflevel_va = value
        End Set
    End Property
    Public Property RefLevelUnits() As String
        Get
            RefLevelUnits = _reflevel_units
        End Get
        Set(value As String)
            _reflevel_units = value
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
#End Region

#Region "Class Methods"
    ''' <summary>
    ''' Updates the JHA reference level (job operational limit) in SHA_Site_RefLevel
    ''' </summary>
    Public Function UpdateRefLevel(ByVal userid As String) As String
        Dim errmessage As String = "success"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim sql As String = "UPDATE SHA_Site_RefLevel" & _
                " SET reflevel_id = " & RefLevelID & ", remarks = '" & Remarks & "', reflevel_va = '" & RefLevelValue & "', reflevel_units = '" & RefLevelUnits & "'" & _
                " WHERE site_reflevel_id = " & ID
                Dim cmd As New SqlCommand(sql, cnx)
                cmd.ExecuteNonQuery()
            Catch ex As Exception
                errmessage = ex.Message
            End Try

            cnx.Close()
        End Using

        Return errmessage
    End Function

    Public Function InsertRefLevel(ByVal userid As String) As String
        Dim errmessage As String = "success"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim sql As String = "INSERT INTO SHA_Site_RefLevel" & _
                " (site_jha_id, reflevel_id, reflevel_va, reflevel_units, remarks)" & _
                " VALUES (" & SiteJHAID & ", " & RefLevelID & ", " & RefLevelValue & ", '" & RefLevelUnits & "', '" & Remarks & "')"
                Dim cmd As New SqlCommand(sql, cnx)
                cmd.ExecuteNonQuery()
            Catch ex As Exception
                errmessage = ex.Message
            End Try

            cnx.Close()
        End Using

        Return errmessage
    End Function

    Public Function DeleteRefLevel(ByVal userid As String) As String
        Dim errmessage As String = "success"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim sql As String
                sql = "DELETE FROM SHA_Site_RefLevel" & _
                    " WHERE site_reflevel_id = " & ID
                Dim cmd2 As New SqlCommand(sql, cnx)
                cmd2.ExecuteNonQuery()
            Catch ex As Exception
                errmessage = ex.Message
            End Try

            cnx.Close()
        End Using

        Return errmessage
    End Function
#End Region
End Class
