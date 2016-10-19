Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

Public Class Cableway
    Private _id As Integer
    Private _site_id As Integer
    Private _status_cd As String
    Private _status_desc As String
    Private _type_cd As String
    Private _type_desc As String
    Private _inspection_freq As Double
    Private _nickname As String
    Private _aerial_marker_req As String
    Private _aerial_marker_inst As String
    Private _created_by As String
    Private _created_dt As DateTime
    Private _updated_by As String
    Private _updated_dt As DateTime

    Public Sub New(ByVal cableway_id As Integer)
        _id = cableway_id

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim sql As String = "SELECT scm.cableway_id, scm.site_id, scm.cableway_status_cd, scm.cableway_type_cd, " & _
                "scm.cableway_inspection_freq, scm.cableway_nm, scm.aerial_marker_req, scm.aerial_marker_inst, scm.created_by, scm.created_dt, scm.updated_by, scm.updated_dt, " & _
                "lcsc.cableway_status_desc, lctc.cableway_type_desc " & _
                "FROM lut_cableway_status_cd AS lcsc INNER JOIN " & _
                "Safety_Cableway_Master AS scm ON lcsc.cableway_status_cd = scm.cableway_status_cd INNER JOIN " & _
                "lut_cableway_type_cd AS lctc ON scm.cableway_type_cd = lctc.cableway_type_cd " & _
                "WHERE (scm.cableway_id = " & _id & ")"

            Dim cmd As SqlCommand = New SqlCommand(sql, cnx)
            Dim da As SqlDataAdapter
            Dim dt As DataTable = New DataTable
            da = New SqlDataAdapter(cmd)
            da.Fill(dt)

            If dt.Rows.Count = 0 Then
                _site_id = Nothing
                _status_cd = Nothing
                _status_desc = Nothing
                _type_cd = Nothing
                _type_desc = Nothing
                _inspection_freq = Nothing
                _nickname = Nothing
                _aerial_marker_req = Nothing
                _aerial_marker_inst = Nothing
                _created_by = Nothing
                _created_dt = Nothing
                _updated_by = Nothing
                _updated_dt = Nothing
            Else
                For Each row As DataRow In dt.Rows
                    _site_id = row("site_id")
                    _status_cd = row("cableway_status_cd").ToString
                    _status_desc = row("cableway_status_desc").ToString
                    _type_cd = row("cableway_type_cd").ToString
                    _type_desc = row("cableway_type_desc").ToString
                    If row("cableway_inspection_freq") Is DBNull.Value Then
                        _inspection_freq = Nothing
                    Else
                        _inspection_freq = row("cableway_inspection_freq")
                    End If
                    If row("cableway_nm") Is DBNull.Value Then
                        _nickname = Nothing
                    Else
                        _nickname = row("cableway_nm")
                    End If
                    If row("aerial_marker_req") Is DBNull.Value Then
                        _aerial_marker_req = "U"
                    Else
                        _aerial_marker_req = row("aerial_marker_req")
                    End If
                    If row("aerial_marker_inst") Is DBNull.Value Then
                        _aerial_marker_inst = "U"
                    Else
                        _aerial_marker_inst = row("aerial_marker_inst")
                    End If
                    _created_by = row("created_by").ToString
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
    Public Property SiteID() As Integer
        Get
            SiteID = _site_id
        End Get
        Set(value As Integer)
            _site_id = value
        End Set
    End Property
    Public Property CablewayStatusCode() As String
        Get
            CablewayStatusCode = _status_cd
        End Get
        Set(value As String)
            _status_cd = value
        End Set
    End Property
    Public Property CablewayStatus() As String
        Get
            CablewayStatus = _status_desc
        End Get
        Set(value As String)
            _status_desc = value
        End Set
    End Property
    Public Property CablewayTypeCode() As String
        Get
            CablewayTypeCode = _type_cd
        End Get
        Set(value As String)
            _type_cd = value
        End Set
    End Property
    Public Property CablewayType() As String
        Get
            CablewayType = _type_desc
        End Get
        Set(value As String)
            _type_desc = value
        End Set
    End Property
    Public Property InspectionFrequency() As Double
        Get
            InspectionFrequency = _inspection_freq
        End Get
        Set(value As Double)
            _inspection_freq = value
        End Set
    End Property
    Public Property Nickname() As String
        Get
            Nickname = _nickname
        End Get
        Set(value As String)
            _nickname = value
        End Set
    End Property
    Public Property AerialMarkerReq() As String
        Get
            AerialMarkerReq = _aerial_marker_req
        End Get
        Set(value As String)
            _aerial_marker_req = value
        End Set
    End Property
    Public Property AerialMarkerInst() As String
        Get
            AerialMarkerInst = _aerial_marker_inst
        End Get
        Set(value As String)
            _aerial_marker_inst = value
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
    ''' Gets a datatable of visits for a cableway
    ''' </summary>
    Public Function GetVisits(ByVal cableway_id As Integer) As DataTable
        Dim visitlist As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim sql As String = "SELECT scv.cableway_visit_id, scv.cableway_id, scv.visit_dt, scv.visit_type_cd, scv.visit_action_cd," & _
                " scv.remarks, scv.created_by, scv.created_dt, scv.updated_by, scv.updated_dt, lcvac.visit_action_desc," & _
                " lcvtc.visit_type_desc, (scv.visit_type_cd + ' - ' + lcvtc.visit_type_desc) AS type_cd_desc, (scv.visit_action_cd +" & _
                " ' - ' + lcvac.visit_action_desc) AS action_cd_desc, scv.visit_file_nm" & _
                " FROM lut_cableway_visitaction_cd AS lcvac INNER JOIN" & _
                " Safety_Cableway_Visit AS scv ON lcvac.visit_action_cd = scv.visit_action_cd INNER JOIN" & _
                " lut_cableway_visittype_cd AS lcvtc ON lcvtc.visit_type_cd = scv.visit_type_cd" & _
                " WHERE (scv.cableway_id = " & cableway_id.ToString & ")" & _
                " ORDER BY scv.visit_dt"

            Dim cmd As New SqlCommand(sql, cnx)
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(visitlist)

            cnx.Close()
        End Using

        Return visitlist
    End Function

    ''' <summary>
    ''' Updates the Safety_Cableway_Master table with general cableway information
    ''' </summary>
    ''' <returns>A string "success" if no errors, or errors if found.</returns>
    Public Function UpdateCablewayInfo(ByVal userid As String) As String
        Dim errmessage As String = "success"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim amr As String
                If String.IsNullOrEmpty(AerialMarkerReq) Then
                    amr = "U"
                Else
                    amr = AerialMarkerReq
                End If

                Dim ami As String
                If String.IsNullOrEmpty(AerialMarkerInst) Then
                    ami = "U"
                Else
                    ami = AerialMarkerInst
                End If

                Dim sql As String = "UPDATE Safety_Cableway_Master" & _
                " SET site_id = " & SiteID & ", cableway_status_cd = '" & CablewayStatusCode & "', cableway_type_cd = '" & CablewayTypeCode & _
                "', cableway_inspection_freq = " & InspectionFrequency & ", cableway_nm = '" & Nickname & "', aerial_marker_req = '" & amr & "', aerial_marker_inst = '" & ami & "',  updated_by = '" & userid & "', updated_dt = GETDATE()" & _
                " WHERE cableway_id = " & ID
                Dim cmd As New SqlCommand(sql, cnx)
                cmd.ExecuteNonQuery()
            Catch ex As Exception
                errmessage = ex.Message
            End Try

            cnx.Close()
        End Using

        Return errmessage
    End Function

    ''' <summary>
    ''' Adds a new cableway for a site into the Safety_Cableway_Master table.
    ''' </summary>
    ''' <returns>A string "success" if no errors, or errors if found.</returns>
    Public Function InsertCablewayInfo(ByVal userid As String) As String
        Dim errmessage As String = "success"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim amr As String
                If String.IsNullOrEmpty(AerialMarkerReq) Then
                    amr = "U"
                Else
                    amr = AerialMarkerReq
                End If

                Dim ami As String
                If String.IsNullOrEmpty(AerialMarkerInst) Then
                    ami = "U"
                Else
                    ami = AerialMarkerInst
                End If

                Dim sql As String = "INSERT INTO Safety_Cableway_Master" & _
                " (site_id, cableway_status_cd, cableway_type_cd, cableway_inspection_freq, cableway_nm, aerial_marker_req, aerial_marker_inst, created_by, created_dt, updated_by, updated_dt)" & _
                " VALUES (" & SiteID & ", '" & CablewayStatusCode & "', '" & CablewayTypeCode & "', " & InspectionFrequency & ", '" & Nickname & "', '" & amr & "', '" & ami & "', '" & _
                userid & "', GETDATE(), '" & userid & "', GETDATE())"
                Dim cmd As New SqlCommand(sql, cnx)
                cmd.ExecuteNonQuery()
            Catch ex As Exception
                errmessage = ex.Message
            End Try

            cnx.Close()
        End Using

        Return errmessage
    End Function

    ''' <summary>
    ''' Deletes a cableway from the Safety_Cableway_Master table.
    ''' </summary>
    Public Function DeleteCableway(ByVal userid As String) As String
        Dim errmessage As String = "success"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim sql As String = "DELETE FROM Safety_Cableway_Master" & _
                " WHERE cableway_id = " & ID
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
