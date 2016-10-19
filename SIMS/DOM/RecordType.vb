Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

''' <summary>
''' This is the class that contains properties and methods for defining Record-Types
''' </summary>
Public Class RecordType
    Private _id As Integer
    Private _cd As String
    Private _ds As String
    Private _ts_fg As Boolean
    Private _wsc_id As Integer
    Private _worker_inst As String
    Private _checker_inst As String
    Private _reviewer_inst As String

    ''' <summary>
    ''' Initializes the Record-Type class and sets all the properties associated with a record-type
    ''' </summary>
    Public Sub New(ByVal record_type_id As Integer)
        _id = record_type_id

        If _id = 0 Then
            _cd = Nothing
            _ds = Nothing
            _ts_fg = True
            _wsc_id = 0
            _worker_inst = Nothing
            _checker_inst = Nothing
            _reviewer_inst = Nothing
        Else
            Using cnx As New SqlConnection(Config.ConnectionInfo)
                cnx.Open()
                Dim cmd As New SqlCommand("SP_Record_Type_by_record", cnx)
                Dim dt As New DataTable

                cmd.CommandType = Data.CommandType.StoredProcedure
                cmd.Parameters.Add("@record_type_id", SqlDbType.Int).Value = _id
                cmd.Parameters.Add("@type_ds", SqlDbType.NVarChar, 10).Value = "0"
                cmd.Parameters.Add("@type_cd", SqlDbType.NVarChar, 10).Value = "0"
                cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = 0

                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)

                For Each row As DataRow In dt.Rows
                    _cd = row("type_cd").ToString
                    _ds = row("type_ds").ToString
                    _ts_fg = row("ts_fg")
                    _wsc_id = row("wsc_id")
                    If row("work_html_va") Is DBNull.Value Or row("work_html_va").ToString = "" Then
                        _worker_inst = "No instructions have been entered"
                    Else
                        _worker_inst = row("work_html_va").ToString
                    End If
                    If row("check_html_va") Is DBNull.Value Or row("check_html_va").ToString = "" Then
                        _checker_inst = "No instructions have been entered"
                    Else
                        _checker_inst = row("check_html_va").ToString
                    End If
                    If row("review_html_va") Is DBNull.Value Or row("review_html_va").ToString = "" Then
                        _reviewer_inst = "No instructions have been entered"
                    Else
                        _reviewer_inst = row("review_html_va").ToString
                    End If
                Next

                cnx.Close()
            End Using
        End If
    End Sub

    ''' <summary>
    ''' Initializes the Record-Type class and sets all the properties associated with a record-type
    ''' </summary>
    Public Sub New(ByVal type_cd As String, ByVal wsc_id As Integer)
        _cd = type_cd
        _wsc_id = wsc_id

        If _cd = "0" Then
            _id = 0
            _ds = Nothing
            _ts_fg = True
            _wsc_id = 0
            _worker_inst = Nothing
            _checker_inst = Nothing
            _reviewer_inst = Nothing
        Else
            Using cnx As New SqlConnection(Config.ConnectionInfo)
                cnx.Open()
                Dim cmd As New SqlCommand("SP_Record_Type_by_record", cnx)
                Dim dt As New DataTable

                cmd.CommandType = Data.CommandType.StoredProcedure
                cmd.Parameters.Add("@record_type_id", SqlDbType.Int).Value = 0
                cmd.Parameters.Add("@type_ds", SqlDbType.NVarChar, 10).Value = "0"
                cmd.Parameters.Add("@type_cd", SqlDbType.NVarChar, 10).Value = type_cd
                cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = wsc_id

                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)

                For Each row As DataRow In dt.Rows
                    _id = row("record_type_id").ToString
                    _ds = row("type_ds").ToString
                    _ts_fg = row("ts_fg")
                    If row("work_html_va") Is DBNull.Value Or row("work_html_va").ToString = "" Then
                        _worker_inst = "No instructions have been entered"
                    Else
                        _worker_inst = row("work_html_va").ToString
                    End If
                    If row("check_html_va") Is DBNull.Value Or row("check_html_va").ToString = "" Then
                        _checker_inst = "No instructions have been entered"
                    Else
                        _checker_inst = row("check_html_va").ToString
                    End If
                    If row("review_html_va") Is DBNull.Value Or row("review_html_va").ToString = "" Then
                        _reviewer_inst = "No instructions have been entered"
                    Else
                        _reviewer_inst = row("review_html_va").ToString
                    End If
                Next

                cnx.Close()
            End Using
        End If
    End Sub

#Region "Class Properties"
    ''' <summary>
    ''' Gets the record_type_id for this record-type
    ''' </summary>
    Public ReadOnly Property ID() As Integer
        Get
            ID = _id
        End Get
    End Property
    ''' <summary>
    ''' Gets or sets the type code of the record-type
    ''' </summary>
    Public Property Code() As String
        Get
            Code = _cd
        End Get
        Set(ByVal value As String)
            _cd = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the description of the record-type
    ''' </summary>
    Public Property Description() As String
        Get
            Description = _ds
        End Get
        Set(ByVal value As String)
            _ds = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the instructions for working this record-type
    ''' </summary>
    Public Property WorkerInstructions() As String
        Get
            WorkerInstructions = _worker_inst
        End Get
        Set(ByVal value As String)
            _worker_inst = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the instructions for checking this record-type
    ''' </summary>
    Public Property CheckerInstructions() As String
        Get
            CheckerInstructions = _checker_inst
        End Get
        Set(ByVal value As String)
            _checker_inst = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the instructions for reviewing this record-type
    ''' </summary>
    Public Property ReviewerInstructions() As String
        Get
            ReviewerInstructions = _reviewer_inst
        End Get
        Set(ByVal value As String)
            _reviewer_inst = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the district code that the record type belongs to
    ''' </summary>
    Public Property WSCID() As Integer
        Get
            WSCID = _wsc_id
        End Get
        Set(ByVal value As Integer)
            _wsc_id = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the time-series flag for this record-type
    ''' </summary>
    Public Property TimeSeriesFlag() As Boolean
        Get
            TimeSeriesFlag = _ts_fg
        End Get
        Set(ByVal value As Boolean)
            _ts_fg = value
        End Set
    End Property
#End Region

#Region "Class Methods"
    ''' <summary>
    ''' Updates the record-type details. Used when modifying the HTML instructions.
    ''' </summary>
    Public Sub UpdateRecordTypeDetails()
        Dim record_type_id As Integer = ID
        Dim type_cd As String = Code
        Dim type_ds As String = Replace(Description, "'", "''")
        Dim work_html As String = Replace(WorkerInstructions, "'", "''")
        Dim check_html As String = Replace(CheckerInstructions, "'", "''")
        Dim review_html As String = Replace(ReviewerInstructions, "'", "''")
        Dim wsc_id As Integer = WSCID
        Dim ts_fg As Boolean = TimeSeriesFlag
        Dim cf As Integer

        If ts_fg Then
            cf = 1
        Else
            cf = 0
        End If

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim sql As String = "UPDATE RMS_Record_Types SET type_cd='" & type_cd & "', type_ds='" & _
                type_ds & "', work_html_va='" & work_html & "', check_html_va='" & check_html & "', review_html_va='" & _
                review_html & "', wsc_id=" & wsc_id.ToString & ", ts_fg=" & cf.ToString & _
                " WHERE record_type_id=" & record_type_id.ToString
            Dim dbcomm As New SqlCommand(sql, cnx)
            dbcomm.ExecuteNonQuery()

            cnx.Close()
        End Using
    End Sub
    ''' <summary>
    ''' Inserts a new record-type into the RMS_Record_Types table.
    ''' </summary>
    Public Sub AddRecordType()
        Dim type_cd As String = Code
        Dim type_ds As String = Replace(Description, "'", "''")
        Dim work_html As String = Replace(WorkerInstructions, "'", "''")
        Dim check_html As String = Replace(CheckerInstructions, "'", "''")
        Dim review_html As String = Replace(ReviewerInstructions, "'", "''")
        Dim wsc_id As Integer = WSCID
        Dim ts_fg As Boolean = TimeSeriesFlag
        Dim cf As Integer

        If ts_fg Then
            cf = 1
        Else
            cf = 0
        End If

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim sql1 As String = "SELECT record_type_id FROM RMS_Record_Types WHERE type_cd = '" & type_cd & _
                "' AND wsc_id = " & wsc_id.ToString
            Dim cmd1 As New SqlCommand(sql1, cnx)
            Dim dt1 As New DataTable

            Dim da1 As New SqlDataAdapter(cmd1)
            da1.Fill(dt1)

            If dt1.Rows.Count = 0 Then
                Dim sql As String = "INSERT INTO RMS_Record_Types " & _
                    "(type_cd, type_ds, work_html_va, check_html_va, review_html_va, wsc_id, ts_fg) " & _
                    "VALUES('" & type_cd & "','" & type_ds & "','" & work_html & "','" & check_html & "','" & _
                    review_html & "'," & wsc_id.ToString & "," & cf.ToString & ")"

                Dim dbcomm As New SqlCommand(sql, cnx)
                dbcomm.ExecuteNonQuery()
            End If

            cnx.Close()
        End Using
    End Sub
#End Region
End Class
