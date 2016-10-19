Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

Public Class CablewayVisit
    Private _id As Integer
    Private _cableway_id As Integer
    Private _visit_dt As DateTime
    Private _type_cd As String
    Private _type_desc As String
    Private _action_cd As String
    Private _action_desc As String
    Private _file_nm As String
    Private _remarks As String
    Private _created_by As String
    Private _created_dt As DateTime
    Private _updated_by As String
    Private _updated_dt As DateTime

    Public Sub New(ByVal cableway_visit_id As Integer)
        _id = cableway_visit_id

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim sql As String = "SELECT scv.cableway_visit_id, scv.cableway_id, scv.visit_dt, scv.visit_type_cd, scv.visit_action_cd, " & _
                "scv.visit_file_nm, scv.remarks, scv.created_by, scv.created_dt, scv.updated_by, " & _
                "scv.updated_dt, lcvac.visit_action_desc, lcvtc.visit_type_desc " & _
                "FROM lut_cableway_visitaction_cd AS lcvac INNER JOIN " & _
                "Safety_Cableway_Visit AS scv ON lcvac.visit_action_cd = scv.visit_action_cd INNER JOIN " & _
                "lut_cableway_visittype_cd AS lcvtc ON scv.visit_type_cd = lcvtc.visit_type_cd " & _
                "WHERE (scv.cableway_visit_id = " & _id & ")"

            Dim cmd As SqlCommand = New SqlCommand(sql, cnx)
            Dim da As SqlDataAdapter
            Dim dt As DataTable = New DataTable
            da = New SqlDataAdapter(cmd)
            da.Fill(dt)

            If dt.Rows.Count = 0 Then
                _cableway_id = Nothing
                _visit_dt = Nothing
                _type_cd = Nothing
                _type_desc = Nothing
                _action_cd = Nothing
                _action_desc = Nothing
                _remarks = Nothing
                _created_by = Nothing
                _created_dt = Nothing
                _updated_by = Nothing
                _updated_dt = Nothing
            Else
                For Each row As DataRow In dt.Rows
                    _cableway_id = row("cableway_id")
                    If row("visit_dt") Is DBNull.Value Then
                        _visit_dt = Nothing
                    Else
                        _visit_dt = row("visit_dt")
                    End If
                    _type_cd = row("visit_type_cd").ToString
                    _type_desc = row("visit_type_desc").ToString
                    _action_cd = row("visit_action_cd").ToString
                    _action_desc = row("visit_action_desc").ToString
                    _remarks = row("remarks").ToString
                    _created_by = row("created_by").ToString
                    _file_nm = row("visit_file_nm").ToString
                    If row("created_dt") Is DBNull.Value Then
                        _created_dt = Nothing
                    Else
                        _created_dt = row("created_dt")
                    End If
                    _updated_by = row("updated_by").ToString
                    If row("updated_dt") Is DBNull.Value Then
                        _updated_dt = Nothing
                    Else
                        _updated_dt = row("updated_dt")
                    End If
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
    Public Property CablewayID() As Integer
        Get
            CablewayID = _cableway_id
        End Get
        Set(value As Integer)
            _cableway_id = value
        End Set
    End Property
    Public Property VisitDate() As DateTime
        Get
            VisitDate = _visit_dt
        End Get
        Set(value As DateTime)
            _visit_dt = value
        End Set
    End Property
    Public Property VisitTypeCode() As String
        Get
            VisitTypeCode = _type_cd
        End Get
        Set(value As String)
            _type_cd = value
        End Set
    End Property
    Public Property VisitType() As String
        Get
            VisitType = _type_desc
        End Get
        Set(value As String)
            _type_desc = value
        End Set
    End Property
    Public Property VisitActionCode() As String
        Get
            VisitActionCode = _action_cd
        End Get
        Set(value As String)
            _action_cd = value
        End Set
    End Property
    Public Property VisitAction() As String
        Get
            VisitAction = _action_desc
        End Get
        Set(value As String)
            _action_desc = value
        End Set
    End Property
    Public Property VisitFileName() As String
        Get
            VisitFileName = _file_nm
        End Get
        Set(value As String)
            _file_nm = value
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
    Public Property CreatedBy() As String
        Get
            CreatedBy = _created_by
        End Get
        Set(value As String)
            _created_by = value
        End Set
    End Property
    Public Property CreatedDate() As DateTime
        Get
            CreatedDate = _created_dt
        End Get
        Set(value As DateTime)
            _created_dt = value
        End Set
    End Property
    Public Property UpdatedBy() As String
        Get
            UpdatedBy = _updated_by
        End Get
        Set(value As String)
            _updated_by = value
        End Set
    End Property
    Public Property UpdatedDate() As DateTime
        Get
            UpdatedDate = _updated_dt
        End Get
        Set(value As DateTime)
            _updated_dt = value
        End Set
    End Property
#End Region

#Region "Class Methods"
    ''' <summary>
    ''' Updates the visit information in Safety_Cableway_Visit
    ''' </summary>
    Public Function UpdateVisitInfo(ByVal userid As String) As String
        Dim errmessage As String = "success"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim remark As String = Replace(Remarks, "'", """")

                Dim sql As String = "UPDATE Safety_Cableway_Visit" & _
                " SET visit_dt = '" & VisitDate & "', visit_type_cd = '" & VisitTypeCode & "', visit_action_cd = '" & VisitActionCode & _
                "', visit_file_nm = '" & VisitFileName & "', remarks = '" & remark & "', updated_by = '" & userid & "', updated_dt = GETDATE()" & _
                " WHERE cableway_visit_id = " & ID
                Dim cmd As New SqlCommand(sql, cnx)
                cmd.ExecuteNonQuery()
            Catch ex As Exception
                errmessage = ex.Message
            End Try

            cnx.Close()
        End Using

        Return errmessage
    End Function

    Public Function InsertVisitInfo(ByVal userid As String) As String
        Dim errmessage As String = "success"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim fileUpload_section1 As String = ""
                Dim fileUpload_section2 As String = ""
                If Not String.IsNullOrEmpty(VisitFileName) Then
                    fileUpload_section1 = "visit_file_nm, "
                    fileUpload_section2 = "'" & VisitFileName & "', "
                End If

                Dim remark As String = Replace(Remarks, "'", """")

                Dim sql As String = "INSERT INTO Safety_Cableway_Visit" & _
                " (cableway_id, visit_dt, visit_type_cd, visit_action_cd, " & fileUpload_section1 & "remarks, created_by, created_dt, updated_by, updated_dt)" & _
                " VALUES (" & CablewayID & ", '" & VisitDate & "', '" & VisitTypeCode & "', '" & VisitActionCode & "', " & fileUpload_section2 & "'" & _
                remark & "', '" & userid & "', GETDATE(), '" & userid & "', GETDATE())"
                Dim cmd As New SqlCommand(sql, cnx)
                cmd.ExecuteNonQuery()
            Catch ex As Exception
                errmessage = ex.Message
            End Try

            cnx.Close()
        End Using

        Return errmessage
    End Function

    Public Function DeleteVisit(ByVal userid As String) As String
        Dim errmessage As String = "success"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim sql As String = "DELETE FROM Safety_Cableway_Visit" & _
                " WHERE cableway_visit_id = " & ID
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
