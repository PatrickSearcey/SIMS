Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

Public Class RecordPeriod
    Private _id As Integer
    Private _rms_record_id As Integer
    Private _beg_dt As DateTime
    Private _end_dt As DateTime
    Private _status As String
    Private _status_set_by As String
    Private _status_set_by_role As String
    Private _status_after_rework As String
    Private _rework_status_set_by_role As String
    Private _worked_by As String
    Private _worked_dt As DateTime
    Private _checked_by As String
    Private _checked_dt As DateTime
    Private _reviewed_by As String
    Private _reviewed_dt As DateTime
    Private _analysis_notes As String
    Private _orig_period_id As Integer
    Private _locked As Boolean
    Private _locked_by As String
    Private _locked_dt As DateTime

    ''' <summary>
    ''' Initializes the RecordPeriod class and sets all the properties associated with a record period
    ''' </summary>
    ''' <param name="period_id"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal period_id As Integer)
        _id = period_id

        If _id = 0 Then

        Else
            Using cnx As New SqlConnection(Config.ConnectionInfo)
                cnx.Open()
                Dim cmd As New SqlCommand("SP_RMS_Details_by_period_id", cnx)
                Dim dt As New DataTable

                cmd.CommandType = Data.CommandType.StoredProcedure
                cmd.Parameters.Add("@period_id", SqlDbType.Int).Value = _id

                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)

                For Each row As DataRow In dt.Rows
                    _rms_record_id = row("rms_record_id")
                    _beg_dt = row("period_beg_dt")
                    _end_dt = row("period_end_dt")
                    _status = row("status_va").ToString
                    _status_set_by = row("status_set_by").ToString
                    _status_set_by_role = row("status_set_by_role_va").ToString
                    _status_after_rework = row("status_after_rework_va").ToString
                    _rework_status_set_by_role = row("rework_status_set_by_role_va").ToString
                    _worked_by = row("worked_by_uid").ToString
                    _worked_dt = row("worked_dt")
                    _checked_by = row("checked_by_uid").ToString
                    If row("checked_dt") Is DBNull.Value Then
                        _checked_dt = Nothing
                    Else
                        _checked_dt = row("checked_dt")
                    End If
                    _reviewed_by = row("reviewed_by_uid").ToString
                    If row("reviewed_dt") Is DBNull.Value Then
                        _reviewed_dt = Nothing
                    Else
                        _reviewed_dt = row("reviewed_dt")
                    End If
                    _analysis_notes = row("analysis_notes_va").ToString
                Next

                Dim cmd1 As New SqlCommand("SP_Locks", cnx)
                Dim dt1 As New DataTable

                cmd1.CommandType = Data.CommandType.StoredProcedure
                cmd1.Parameters.Add("@rms_record_id", SqlDbType.Int).Value = 0
                cmd1.Parameters.Add("@office_id", SqlDbType.Int).Value = 0
                cmd1.Parameters.Add("@period_id", SqlDbType.Int).Value = period_id

                Dim da1 As New SqlDataAdapter(cmd1)
                da1.Fill(dt1)

                If dt1.Rows.Count = 0 Then
                    _locked = False
                Else
                    _locked = True
                End If

                cnx.Close()
            End Using
        End If
    End Sub

#Region "Class Properties"
    ''' <summary>
    ''' Gets the period ID for this period
    ''' </summary>
    Public ReadOnly Property ID() As Integer
        Get
            ID = _id
        End Get
    End Property

    ''' <summary>
    ''' Gets the RMS record ID tied to this period 
    ''' </summary>
    Public ReadOnly Property RMSRecordID() As Integer
        Get
            RMSRecordID = _rms_record_id
        End Get
    End Property

    ''' <summary>
    ''' Gets or sets the period begin date
    ''' </summary>
    Public Property BeginDate() As DateTime
        Get
            BeginDate = _beg_dt
        End Get
        Set(ByVal value As DateTime)
            _beg_dt = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the period end date
    ''' </summary>
    Public Property EndDate() As DateTime
        Get
            EndDate = _end_dt
        End Get
        Set(ByVal value As DateTime)
            _end_dt = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the period status
    ''' </summary>
    Public Property Status() As String
        Get
            Status = _status
        End Get
        Set(ByVal value As String)
            _status = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the role of the person who last changed the status of the period (i.e. Reviewer, Checker, Worker)
    ''' </summary>
    Public Property StatusSetByRole() As String
        Get
            StatusSetByRole = _status_set_by_role
        End Get
        Set(ByVal value As String)
            _status_set_by_role = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the userid of the person who worked the record period
    ''' </summary>
    Public Property WorkedBy() As String
        Get
            WorkedBy = _worked_by
        End Get
        Set(ByVal value As String)
            _worked_by = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the date of when the record period was worked
    ''' </summary>
    Public Property WorkedDate() As DateTime
        Get
            WorkedDate = _worked_dt
        End Get
        Set(ByVal value As DateTime)
            _worked_dt = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the userid of the person who checked the record period
    ''' </summary>
    Public Property CheckedBy() As String
        Get
            CheckedBy = _checked_by
        End Get
        Set(ByVal value As String)
            _checked_by = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the date of when the record period was checked
    ''' </summary>
    Public Property CheckedDate() As DateTime
        Get
            CheckedDate = _checked_dt
        End Get
        Set(ByVal value As DateTime)
            _checked_dt = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the userid of the person who reviewed the record period
    ''' </summary>
    Public Property ReviewedBy() As String
        Get
            ReviewedBy = _reviewed_by
        End Get
        Set(ByVal value As String)
            _reviewed_by = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the date of when the record period was reviewed
    ''' </summary>
    Public Property ReviewedDate() As DateTime
        Get
            ReviewedDate = _reviewed_dt
        End Get
        Set(ByVal value As DateTime)
            _reviewed_dt = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the analysis notes for the record period
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AnalysisNotes() As String
        Get
            AnalysisNotes = _analysis_notes
        End Get
        Set(ByVal value As String)
            _analysis_notes = value
        End Set
    End Property

    ''' <summary>
    ''' Gets whether or not the period is locked
    ''' </summary>
    Public ReadOnly Property PeriodIsLocked() As Boolean
        Get
            PeriodIsLocked = _locked
        End Get
    End Property
#End Region

#Region "Class Methods"
    ''' <summary>
    ''' Gets the dialogs related to a period.
    ''' </summary>
    ''' <param name="status">Depending on the status that is passed, the dialogs will be limited accordingly - for use in the modify period status interface</param>
    Public Function GetDialogs(ByVal period_id As Integer, ByVal status As String) As DataTable
        Dim dtDialogs As New DataTable()

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SP_Dialogs_Deletion", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@period_id", SqlDbType.Int).Value = period_id
            cmd.Parameters.Add("@status", SqlDbType.NVarChar, 8).Value = status

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dtDialogs)

            cnx.Close()
        End Using

        Return dtDialogs
    End Function

    ''' <summary>
    ''' Deletes the dialogs based on the dialog_ids returned by the GetDialogs function.  Used when resetting period status.
    ''' </summary>
    Public Sub DeleteDialogs(ByVal period_id As Integer, ByVal status As String)
        Dim dialog_id_str As String = Nothing
        Dim sql As String = Nothing
        Dim dtDialogs As DataTable = GetDialogs(period_id, status)

        For Each row As DataRow In dtDialogs.Rows
            dialog_id_str = dialog_id_str & row("dialog_id") & ","
        Next

        dialog_id_str = Mid(dialog_id_str, 1, Len(dialog_id_str) - 1)

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            sql = "DELETE FROM RMS_Dialog" & _
                " WHERE (dialog_id IN (" & dialog_id_str & "))"
            Dim cmd As New SqlCommand(sql, cnx)
            cmd.ExecuteNonQuery()

            cnx.Close()
        End Using
    End Sub

    ''' <summary>
    ''' Edits the period status based on period ID and the current status; for use by the Edit Period Status interface
    ''' </summary>
    Public Sub EditPeriodStatus(ByVal period_id As Integer, ByVal status As String)
        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand("SP_RMS_Period_Status_Update", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@period_id", SqlDbType.Int).Value = period_id
            cmd.Parameters.Add("@status", SqlDbType.NVarChar, 20).Value = status
            cmd.ExecuteNonQuery()

            cnx.Close()
        End Using
    End Sub

    ''' <summary>
    ''' Edits the period date based on period ID and date type passed; for use by the Edit Period Dates interface
    ''' </summary>
    Public Sub EditPeriodDates(ByVal period_id1 As Integer, ByVal period_id2 As Integer, ByVal new_date As Date)
        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand("SP_RMS_Period_Dates_Update", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@period_id1", SqlDbType.Int).Value = period_id1
            cmd.Parameters.Add("@period_id2", SqlDbType.Int).Value = period_id2
            cmd.Parameters.Add("@new_date", SqlDbType.DateTime).Value = new_date
            cmd.ExecuteNonQuery()

            cnx.Close()
        End Using
    End Sub

    ''' <summary>
    ''' Adds a new dialog entry for a record period
    ''' </summary>
    Public Sub AddDialogForPeriod(ByVal period_id1 As Integer, ByVal period_id2 As Integer, ByVal dialog_uid As String, ByVal origin_va As String, ByVal comments As String)
        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand("SP_Dialog_Insert", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@dialog_dt", SqlDbType.DateTime).Value = Now()
            cmd.Parameters.Add("@dialog_uid", SqlDbType.NVarChar, 20).Value = dialog_uid
            cmd.Parameters.Add("@period_id", SqlDbType.Int).Value = period_id2
            cmd.Parameters.Add("@origin_va", SqlDbType.NVarChar, 20).Value = origin_va
            cmd.Parameters.Add("@comments_va", SqlDbType.NVarChar, 1000).Value = comments
            cmd.ExecuteNonQuery()

            If period_id1 > 1 Then
                Dim cmd1 As New SqlCommand("SP_Dialog_Insert", cnx)
                cmd1.CommandType = Data.CommandType.StoredProcedure
                cmd1.Parameters.Add("@dialog_dt", SqlDbType.DateTime).Value = Now()
                cmd1.Parameters.Add("@dialog_uid", SqlDbType.NVarChar, 20).Value = dialog_uid
                cmd1.Parameters.Add("@period_id", SqlDbType.Int).Value = period_id1
                cmd1.Parameters.Add("@origin_va", SqlDbType.NVarChar, 20).Value = origin_va
                cmd1.Parameters.Add("@comments_va", SqlDbType.NVarChar, 1000).Value = comments
                cmd1.ExecuteNonQuery()
            End If

            cnx.Close()
        End Using
    End Sub
#End Region
End Class
