Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

''' <summary>
''' This is the class that should be used when working on record specific (RMS) applications
''' </summary>
''' <remarks></remarks>
Public Class Record
    Private _rms_record_id As Integer
    Private _type_id As Integer
    Private _type_ds As String
    Private _full_type_ds As String
    Private _parm_nm As String
    Private _worker As String
    Private _checker As String
    Private _reviewer As String
    Private _site_id As Integer
    Private _ts_fg As Boolean
    Private _multi_DD As Boolean
    Private _category_no As Integer
    Private _category_reason As String
    Private _category_reason_remarks As String
    Private _not_used_fg As Boolean
    Private _not_published_fg As Boolean
    Private _alt_office_id As Integer
    Private _wsc_id As Integer

    ''' <summary>
    ''' Initializes the Record class and sets all the properties associated with a record
    ''' </summary>
    ''' <param name="rms_record_id"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal rms_record_id As Integer)
        _rms_record_id = rms_record_id

        If _rms_record_id = 0 Then
            _type_id = Nothing
            _type_ds = Nothing
            _worker = Nothing
            _checker = Nothing
            _reviewer = Nothing
            _site_id = Nothing
            _alt_office_id = Nothing
            _wsc_id = 0
            _category_no = Nothing
            _category_reason = Nothing
            _category_reason_remarks = Nothing
            _not_used_fg = False
            _not_published_fg = False
            _ts_fg = True
            _multi_DD = False
            _parm_nm = Nothing
        Else
            Using cnx As New SqlConnection(Config.ConnectionInfo)
                cnx.Open()
                Dim cmd As New SqlCommand("SP_Station_Details_by_rms_record_id", cnx)
                Dim dt As New DataTable

                cmd.CommandType = Data.CommandType.StoredProcedure
                cmd.Parameters.Add("@office_id", SqlDbType.Int).Value = 0
                cmd.Parameters.Add("@rms_record_id", SqlDbType.Int).Value = rms_record_id
                cmd.Parameters.Add("@inc_inactive", SqlDbType.NVarChar, 5).Value = "True"

                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)

                For Each row As DataRow In dt.Rows
                    _type_id = row("record_type_id")
                    _type_ds = row("type_ds").ToString
                    _worker = row("operator_va").ToString
                    _checker = row("assigned_checker_uid").ToString
                    _reviewer = row("assigned_reviewer_uid").ToString
                    _site_id = row("site_id")
                    _alt_office_id = row("alt_office_id")
                    _wsc_id = row("wsc_id")
                    If row("category_no") Is DBNull.Value Then
                        _category_no = 0
                    Else
                        _category_no = row("category_no")
                    End If
                    If row("cat_reason_desc") Is DBNull.Value Then
                        _category_reason = "unavailable"
                    Else
                        _category_reason = row("cat_reason_desc")
                    End If
                    If row("cat_reason_remarks") Is DBNull.Value Then
                        _category_reason_remarks = "none"
                    Else
                        _category_reason_remarks = row("cat_reason_remarks")
                    End If
                    If row("not_used_fg") Is DBNull.Value Then
                        _not_used_fg = False
                    Else
                        _not_used_fg = row("not_used_fg")
                    End If
                    If row("not_published_fg") Is DBNull.Value Then
                        _not_published_fg = False
                    Else
                        _not_published_fg = row("not_published_fg")
                    End If
                Next

                'Determine if this is a time-series record
                Dim cmd1 As New SqlCommand("SELECT rsmca.* FROM vRMS_Record_Master_C_All AS rsmca WHERE(rms_record_id = " & rms_record_id & ")", cnx)
                Dim dt1 As New DataTable
                Dim da1 As New SqlDataAdapter(cmd1)
                da1.Fill(dt1)

                If dt1.Rows.Count > 0 Then
                    _ts_fg = True
                    _parm_nm = GetParmName(rms_record_id)
                    _multi_DD = RecordHasMultiDDs(rms_record_id)
                Else
                    _ts_fg = False
                    _parm_nm = ""
                End If

                cnx.Close()
            End Using
        End If
    End Sub

#Region "Class Properties"
    ''' <summary>
    ''' Gets or sets the record-type ID for the record
    ''' </summary>
    ''' <remarks>This is the record_type_id in the RMS_Record_Types table</remarks>
    Public Property TypeID() As Integer
        Get
            TypeID = _type_id
        End Get
        Set(ByVal value As Integer)
            _type_id = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the site ID that the record is tied to
    ''' </summary>
    Public Property SiteID() As Integer
        Get
            SiteID = _site_id
        End Get
        Set(ByVal value As Integer)
            _site_id = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the alternate office ID for the record
    ''' </summary>
    ''' <remarks>This value defaults to the office ID for the site if no alternate office ID has been defined</remarks>
    Public Property AltOfficeID() As Integer
        Get
            AltOfficeID = _alt_office_id
        End Get
        Set(ByVal value As Integer)
            _alt_office_id = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the district code for the record
    ''' </summary>
    Public Property WSCID() As String
        Get
            WSCID = _wsc_id
        End Get
        Set(ByVal value As String)
            _wsc_id = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the record-type description for the record
    ''' </summary>
    ''' <remarks>This is the type_ds in the RMS_Record_Types table</remarks>
    Public Property TypeDS() As String
        Get
            TypeDS = _type_ds
        End Get
        Set(ByVal value As String)
            _type_ds = value
        End Set
    End Property
    ''' <summary>
    ''' Gets the full record-type description for the record, which is the parameter name of the DD assigned
    ''' to the record and the custom assigned record-type description.
    ''' </summary>
    ''' <remarks>The _parm_nm portion is left blank for multi-parameter and non-time-series records</remarks>
    Public ReadOnly Property FullTypeDS() As String
        Get
            FullTypeDS = _parm_nm & _type_ds
        End Get
    End Property
    ''' <summary>
    ''' Gets a value of true for multi-parameter records and a value of false for single-parameter records
    ''' </summary>
    Public ReadOnly Property MultiDD() As Boolean
        Get
            MultiDD = _multi_DD
        End Get
    End Property
    ''' <summary>
    ''' Gets or sets the category number for the record
    ''' </summary>
    Public Property CategoryNo() As Integer
        Get
            CategoryNo = _category_no
        End Get
        Set(ByVal value As Integer)
            _category_no = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the reason for choosing category number 2 or 3 for the record
    ''' </summary>
    Public Property CategoryReason() As String
        Get
            CategoryReason = _category_reason
        End Get
        Set(ByVal value As String)
            _category_reason = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the remarks for choosing category reason "Other"
    ''' </summary>
    Public Property CategoryReasonRemarks() As String
        Get
            CategoryReasonRemarks = _category_reason_remarks
        End Get
        Set(ByVal value As String)
            _category_reason_remarks = value
        End Set
    End Property
    ''' <summary>
    ''' Gets the full reason for choosing category number 2 or 3 for the record (reason + remarks)
    ''' </summary>
    Public ReadOnly Property CategoryReasonFull() As String
        Get
            If _category_reason_remarks = "none" Then
                CategoryReasonFull = CategoryReason
            Else
                CategoryReasonFull = CategoryReason & ": " & CategoryReasonRemarks
            End If
        End Get
    End Property
    ''' <summary>
    ''' Gets or sets the assigned worker for the record
    ''' </summary>
    ''' <remarks>This is the operator_va in the RMS_Record_Master table</remarks>
    Public Property Worker() As String
        Get
            Worker = _worker
        End Get
        Set(ByVal value As String)
            _worker = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the assigned checker for the record
    ''' </summary>
    Public Property Checker() As String
        Get
            Checker = _checker
        End Get
        Set(ByVal value As String)
            _checker = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the assigned reviewer for the record
    ''' </summary>
    Public Property Reviewer() As String
        Get
            Reviewer = _reviewer
        End Get
        Set(ByVal value As String)
            _reviewer = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the rms_record_id for the record
    ''' </summary>
    Public Property RMSSiteID() As Integer
        Get
            RMSSiteID = _rms_record_id
        End Get
        Set(ByVal value As Integer)
            _rms_record_id = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the not_used_fg for the record. If set to true, the record has been discontinued
    ''' </summary>
    Public Property NotUsedFlag() As Boolean
        Get
            NotUsedFlag = _not_used_fg
        End Get
        Set(ByVal value As Boolean)
            _not_used_fg = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the not_published_fg for the record.  If set to true, the record is not published in the ADR
    ''' </summary>
    Public Property NotPublishedFlag() As Boolean
        Get
            NotPublishedFlag = _not_published_fg
        End Get
        Set(ByVal value As Boolean)
            _not_published_fg = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the time-series status for the record
    ''' </summary>
    ''' <remarks>If DD's exist for the record, then it should be a time-series record</remarks>
    Public Property TimeSeriesFlag() As Boolean
        Get
            TimeSeriesFlag = _ts_fg
        End Get
        Set(ByVal value As Boolean)
            _ts_fg = value
        End Set
    End Property
    ''' <summary>
    ''' Gets the help blurb for the ADAPS data descriptors
    ''' </summary>
    Public ReadOnly Property DDHelp() As String
        Get
            DDHelp = "(<a href=""#"" class=""infobox""><span><strong>Explain this:</strong><br />" & _
                "<!--introstart-->DDs with their checkboxes checked are currently<br />assigned to this record.<br /><br />" & _
                "You may choose to assign multiple DDs to a record,<br /> or change the currently assigned DD by checking<br />" & _
                "the box to the right labeled 'This is a multi-<br />parameter record, " & _
                "or change the DD'.<br /><br />" & _
                "As an example, a record consisting of multiple<br />DDs, such as a Water Quality Monitor " & _
                ", might have<br />the boxes for DDs 3, 4, and 5 checked. This shows<br />" & _
                "that the Water Quality Monitor record consists<br />of DD " & _
                "3-Temperature, 4-Specific Conductance,<br />and 5-pH.</span><b>?</b></a>)"
        End Get
    End Property
    ''' <summary>
    ''' Gets the help blurb for the record-type drop-down lists
    ''' </summary>
    Public ReadOnly Property RecordTypeHelp() As String
        Get
            RecordTypeHelp = "(<a href=""#"" class=""infobox""><span><strong>Explain this:</strong><br />" & _
                "<!--introstart-->A record-type is a WSC defined field used to<br />categorize the record " & _
                "or records being tracked. A<br />record-type can describe time-series " & _
                "data and can<br />have single or multiple data descriptors assigned to<br />it. " & _
                "A record-type can also describe non-time-series<br />data ie:. crest " & _
                "stage gage or miscellaneous discharge<br />measurements.</span><b>?</b></a>)"
        End Get
    End Property
#End Region

#Region "Class Methods"
    ''' <summary>
    ''' Returns the category numbers for CRP purposes
    ''' </summary>
    Public Function GetCategoryNumbers() As ArrayList
        Dim categoryList As New ArrayList

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SP_RMS_Get_Category_Desc", cnx)
            Dim dt As New DataTable
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@category_no", SqlDbType.Int).Value = 0

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                categoryList.Add(row("category_no").ToString)
            Next
            cnx.Close()
        End Using

        Return categoryList
    End Function

    ''' <summary>
    ''' Returns the reasons for why a certain category number was chosen
    ''' </summary>
    Public Function GetCategoryReasons(ByVal category_no As Integer) As ArrayList
        Dim reasonList As New ArrayList

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SELECT cat_reason_desc FROM lut_CRP_Category_Reasons WHERE category_no = " & category_no, cnx)
            Dim dt As New DataTable

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                If Not row("cat_reason_desc").ToString = "Record is non-time-series" Then
                    reasonList.Add(row("cat_reason_desc").ToString)
                End If
            Next
            cnx.Close()
        End Using

        Return reasonList
    End Function

    ''' <summary>
    ''' Returns the category reason ID based on a passed description
    ''' </summary>
    Public Function GetCatReasonID(ByVal cat_reason_desc As String, ByVal category_no As Integer) As Integer
        Dim cat_reason_id As Integer

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SELECT cat_reason_id FROM lut_CRP_Category_Reasons WHERE cat_reason_desc = '" & cat_reason_desc & "' And category_no = " & category_no, cnx)
            Dim dt As New DataTable

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                cat_reason_id = row("cat_reason_id")
            Next
            cnx.Close()
        End Using

        Return cat_reason_id
    End Function

    ''' <summary>
    ''' Returns the explanation of the category numbers
    ''' </summary>
    ''' <param name="type">Send "ts" to return category help for a time-series record. 
    ''' Send "nts" to return category help for a non-time-series record.</param>
    Public Function GetCategoryHelp(ByVal type As String) As String
        Dim help As String = "(<a href=""#"" class=""infobox""><span><strong>Explain this:</strong><br /><!--introstart-->"

        If type = "ts" Then
            Using cnx As New SqlConnection(Config.ConnectionInfo)
                cnx.Open()
                Dim cmd As New SqlCommand("SP_RMS_Get_Category_Desc", cnx)
                Dim dt As New DataTable
                cmd.CommandType = Data.CommandType.StoredProcedure
                cmd.Parameters.Add("@category_no", SqlDbType.Int).Value = 0

                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)

                For Each row As DataRow In dt.Rows
                    help = help & "Category " & row("category_no").ToString & ": " & row("category_full_desc") & "<br /><br />"
                Next
                cnx.Close()
            End Using
        Else
            help = help & "Because this is a non-time-series record, category 3 is selected by default."
        End If

        help = help & "</span><b>?</b></a>)"

        Return help
    End Function

    ''' <summary>
    ''' Returns the ADAPS data descriptors with parm codes and parm names for a record
    ''' </summary>
    ''' <returns>A DataTable</returns>
    Public Function GetDDs(ByVal rms_record_id As Integer) As DataTable
        Dim dds As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand("SP_RMS_Get_Record_DDs", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@rms_record_id", SqlDbType.Int).Value = rms_record_id

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dds)

            cnx.Close()
        End Using

        Return dds
    End Function

    ''' <summary>
    ''' Returns the parameter name for a record to be used in the full record-type description
    ''' </summary>
    ''' <remarks>Returns blank if this is a multi-parameter record</remarks>
    Public Function GetParmName(ByVal rms_record_id As Integer) As String
        Dim parm_nm As String = "no DD, "

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SP_RMS_Get_Record_DDs", cnx)
            Dim dt As New DataTable
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@rms_record_id", SqlDbType.Int).Value = rms_record_id

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            If dt.Rows.Count > 1 Then
                parm_nm = ""
            Else
                For Each row As DataRow In dt.Rows
                    parm_nm = row("parm_nm").ToString & ", "
                Next
            End If

            cnx.Close()
        End Using

        Return parm_nm
    End Function

    ''' <summary>
    ''' Returns the parameter code for a record to be used in the full record-type description
    ''' </summary>
    ''' <remarks>Returns blank if this is a multi-parameter record</remarks>
    Public Function GetParmCode(ByVal rms_record_id As Integer) As String
        Dim parm_cd As String = "none"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SP_RMS_Get_Record_DDs", cnx)
            Dim dt As New DataTable
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@rms_record_id", SqlDbType.Int).Value = rms_record_id

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            If dt.Rows.Count > 1 Then
                parm_cd = ""
            Else
                For Each row As DataRow In dt.Rows
                    parm_cd = row("parm_cd").ToString
                Next
            End If

            cnx.Close()
        End Using

        Return parm_cd
    End Function

    ''' <summary>
    ''' Returns the parameter codes and the days since last approved in ADAPS for a record to be used in 
    ''' the work/check/review lists
    ''' </summary>
    ''' <remarks>A DataTable</remarks>
    Public Function GetRecordsParms(ByVal rms_record_id As Integer) As DataTable
        Dim parms As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SP_Records_To_Be_Processed_Parms", cnx)
            Dim dt As New DataTable
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@rms_record_id", SqlDbType.Int).Value = rms_record_id

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(parms)

            cnx.Close()
        End Using

        Return parms
    End Function

    ''' <summary>
    ''' Returns true if a record has multiple DDs tied to it, and false if it does not
    ''' </summary>
    Public Function RecordHasMultiDDs(ByVal rms_record_id As Integer) As Boolean
        Dim hasMultiDDs As Boolean = False

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand("SP_RMS_Get_Record_DDs", cnx)
            Dim dt As New DataTable
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@rms_record_id", SqlDbType.Int).Value = rms_record_id

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            If dt.Rows.Count > 1 Then
                hasMultiDDs = True
            Else
                hasMultiDDs = False
            End If

            cnx.Close()
        End Using

        Return hasMultiDDs
    End Function

    ''' <summary>
    ''' Returns true if a record-type is flagged as a time-series record-type in the RMS_Record_Types table
    ''' </summary>
    Public Function RecordTypeIsCont(ByVal record_type_id As Integer) As Boolean
        Dim rtIsCont As Boolean = True

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand("SP_Record_Type_by_record", cnx)
            Dim dt As New DataTable
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@record_type_id", SqlDbType.Int).Value = record_type_id
            cmd.Parameters.Add("@type_ds", SqlDbType.NVarChar, 250).Value = "0"
            cmd.Parameters.Add("@type_cd", SqlDbType.NVarChar, 20).Value = "0"
            cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = 0

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                rtIsCont = row("ts_fg")
            Next

            cnx.Close()
        End Using

        Return rtIsCont
    End Function

    ''' <summary>
    ''' Updates the information in RMS_Record_Master for a record 
    ''' </summary>
    Public Sub UpdateRecordDetails()
        Dim not_used_fg As Boolean = NotUsedFlag
        Dim nuf As Integer
        Dim operator_va As String = Worker
        Dim checker_va As String = Checker
        Dim reviewer_va As String = Reviewer
        Dim not_published_fg As Boolean = NotPublishedFlag
        Dim npf As Integer
        Dim record_type_id As Integer = TypeID
        Dim rms_record_id As Integer = RMSSiteID
        Dim ts_fg As Boolean = TimeSeriesFlag
        Dim category_no As Integer = CategoryNo
        Dim category_reason As String = CategoryReason
        Dim category_reason_remarks As String = CategoryReasonRemarks
        Dim category_stmt As String

        If not_published_fg Then
            npf = 1
        Else
            npf = 0
        End If
        If not_used_fg Then
            nuf = 1
        Else
            nuf = 0
        End If
        If ts_fg Then
            If category_no = 1 Then
                category_stmt = ", category_no=" & category_no & ", cat_reason_id = Null, cat_reason_remarks = Null"
            Else
                Dim cat_reason_id As Integer = GetCatReasonID(category_reason, category_no)
                If category_reason_remarks Is Nothing Or category_reason_remarks = "" Or category_reason_remarks = "none" Then
                    category_reason_remarks = "Null"
                Else
                    category_reason_remarks = "'" & Replace(category_reason_remarks, "'", "''") & "'"
                End If

                category_stmt = ", category_no=" & category_no & ", cat_reason_id=" & cat_reason_id & _
                    ", cat_reason_remarks=" & category_reason_remarks
            End If
        Else
            category_stmt = ", category_no = 3, cat_reason_id = 6, cat_reason_remarks = Null"
        End If

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim sql As String = "UPDATE RMS_Record_Master SET operator_va='" & operator_va & _
                    "', assigned_checker_uid='" & checker_va & "', assigned_reviewer_uid='" & _
                    reviewer_va & "', not_published_fg=" & npf.ToString & ", not_used_fg=" & nuf.ToString & ", record_type_id=" & _
                    record_type_id & category_stmt & _
                    " WHERE rms_record_id=" & rms_record_id.ToString
            Dim dbcomm As New SqlCommand(sql, cnx)
            dbcomm.ExecuteNonQuery()

            cnx.Close()
        End Using
    End Sub

    ''' <summary>
    ''' Inserts the information into RMS_Record_Master for a new record and returns the newly created rms_record_id
    ''' </summary>
    Public Function AddRecordDetails() As Integer
        Dim not_used_fg As Boolean = NotUsedFlag
        Dim nuf As Integer
        Dim operator_va As String = Worker
        Dim checker_va As String = Checker
        Dim reviewer_va As String = Reviewer
        Dim not_published_fg As Boolean = NotPublishedFlag
        Dim npf As Integer
        Dim record_type_id As Integer = TypeID
        Dim site_id As Integer = SiteID
        Dim ts_fg As Boolean = TimeSeriesFlag
        Dim category_no As Integer = CategoryNo
        Dim category_reason As String = CategoryReason
        Dim category_reason_remarks As String = CategoryReasonRemarks
        Dim category_stmt As String
        Dim rms_record_id As Integer = RMSSiteID

        If not_published_fg Then
            npf = 1
        Else
            npf = 0
        End If
        If not_used_fg Then
            nuf = 1
        Else
            nuf = 0
        End If
        If ts_fg Then
            If category_no = 1 Then
                category_stmt = category_no & ", Null, Null"
            Else
                Dim cat_reason_id As Integer = GetCatReasonID(category_reason, category_no)

                If category_reason_remarks Is Nothing Or category_reason_remarks = "" Or category_reason_remarks = "none" Then
                    category_reason_remarks = "Null"
                Else
                    category_reason_remarks = "'" & Replace(category_reason_remarks, "'", "''") & "'"
                End If

                category_stmt = category_no & ", " & cat_reason_id & ", " & category_reason_remarks
            End If
        Else
            Dim cat_reason_id As Integer = GetCatReasonID(category_reason, category_no)

            category_stmt = category_no & ", " & cat_reason_id & ", Null"
        End If

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim sql1 As String = "SELECT rms_record_id FROM RMS_Record_Master WHERE site_id = " & site_id & _
                " AND record_type_id = " & record_type_id
            Dim cmd1 As New SqlCommand(sql1, cnx)
            Dim dt1 As New DataTable

            Dim da1 As New SqlDataAdapter(cmd1)
            da1.Fill(dt1)

            If dt1.Rows.Count = 0 Then
                Dim sql As String = "INSERT INTO RMS_Record_Master " & _
                    "(site_id, operator_va, assigned_checker_uid, assigned_reviewer_uid, not_published_fg, " & _
                    "not_used_fg, record_type_id, category_no, cat_reason_id, cat_reason_remarks) " & _
                    "VALUES(" & site_id & ",'" & operator_va & "','" & checker_va & "','" & reviewer_va & "'," & _
                    npf.ToString & "," & nuf.ToString & "," & record_type_id & "," & category_stmt & ")"

                Dim dbcomm As New SqlCommand(sql, cnx)
                dbcomm.ExecuteNonQuery()

                Dim sql2 As String = "SELECT rms_record_id FROM RMS_Record_Master WHERE site_id = " & site_id & _
                " AND record_type_id = " & record_type_id
                Dim cmd2 As New SqlCommand(sql2, cnx)
                Dim dt2 As New DataTable

                Dim da2 As New SqlDataAdapter(cmd2)
                da2.Fill(dt2)

                For Each row As DataRow In dt2.Rows
                    rms_record_id = row("rms_record_id")
                Next
            Else
                For Each row As DataRow In dt1.Rows
                    rms_record_id = row("rms_record_id")
                Next
            End If

            cnx.Close()
        End Using

        Return rms_record_id
    End Function

    ''' <summary>
    ''' Removes any duplicate DDs for a record from RMS_Site_DD
    ''' </summary>
    Public Sub CleanUpDuplicateDDs(ByVal rms_record_id As Integer)
        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand("SP_RMS_Modify_Record_DDs", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@action", SqlDbType.NVarChar, 5).Value = "dupes"
            cmd.Parameters.Add("@rms_record_id", SqlDbType.Int).Value = rms_record_id
            cmd.Parameters.Add("@dd_nu", SqlDbType.Int).Value = 0
            cmd.ExecuteNonQuery()

            cnx.Close()
        End Using
    End Sub

    ''' <summary>
    ''' Gets the lock information for a record to be used in creating the locks column of the records processing
    ''' RadGrid. 
    ''' </summary>
    ''' <param name="rms_record_id">The rms_record_id of the record</param>
    ''' <param name="user_id">The user_id of the currently logged in user</param>
    ''' <param name="listtype">Enter 'work' for the records to be worked list, 'check' for the records to be checked list,
    ''' or 'review' for the records to be reviewed list.</param>
    ''' <param name="proptype">The property type of interest. Enter 'imageURL' to return then NavigateURL property
    ''' or 'imageAltTag' to return the AlternateText property.</param>
    Public Function GetRecordLocks(ByVal rms_record_id As Integer, ByVal user_id As String, ByVal listtype As String, ByVal proptype As String) As String
        Dim lock_type As String = Nothing
        Dim lock_uid As String = Nothing
        Dim lock_dt As String = Nothing

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand("SP_Locks", cnx)
            Dim dt As New DataTable
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@rms_record_id", SqlDbType.Int).Value = rms_record_id
            cmd.Parameters.Add("@office_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@period_id", SqlDbType.Int).Value = 0

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            If dt.Rows.Count > 0 Then
                For Each row As DataRow In dt.Rows
                    lock_type = row("lock_type")
                    lock_uid = row("lock_uid")
                    lock_dt = row("lock_dt")
                Next
            End If

            cnx.Close()
        End Using

        Return GetLockImageProperties(proptype, user_id, listtype, lock_type, lock_uid, lock_dt)

    End Function

    ''' <summary>
    ''' Gets the actual image properties to be used to display locks in the records processing lists
    ''' </summary>
    ''' <param name="proptype">The property type of interest. Enter 'imageURL' to return then NavigateURL property
    ''' or 'imageAltTag' to return the AlternateText property.</param>
    ''' <param name="user_id">The user_id of the currently logged in user</param>
    ''' <param name="listtype">Enter 'work' for the records to be worked list, 'check' for the records to be checked list,
    ''' or 'review' for the records to be reviewed list.</param>
    ''' <param name="lock_type">The type of lock for the record</param>
    ''' <param name="lock_uid">The user id of the person who created the lock</param>
    ''' <param name="lock_dt">The date of the lock</param>
    Public Function GetLockImageProperties(ByVal proptype As String, ByVal user_id As String, ByVal listtype As String, ByVal lock_type As String, ByVal lock_uid As String, ByVal lock_dt As String) As String
        Dim imageURL As String = ""
        Dim imageAltTag As String = ""

        Select Case listtype
            Case "work"
                If Not lock_type = Nothing Then
                    If lock_type = "check" Or lock_type = "review" Then
                        imageURL = "images/lock.png"
                        imageAltTag = "lock type is " & lock_type & ", locked by " & lock_uid & " " & lock_dt
                    ElseIf (lock_type = "work" Or lock_type = "rework") And lock_uid <> user_id Then
                        imageURL = "images/lock.png"
                        imageAltTag = "lock type is " & lock_type & ", locked by " & lock_uid & " " & lock_dt
                    ElseIf lock_type = "checking" Or lock_type = "reviewing" Then
                        imageURL = "images/save_icon.gif"
                        imageAltTag = lock_type & " in progress by " & lock_uid & " " & lock_dt
                    ElseIf (lock_type = "working" Or lock_type = "reworking") And lock_uid <> user_id Then
                        imageURL = "images/save_icon.gif"
                        imageAltTag = "working in progress by " & lock_uid & " " & lock_dt
                    End If
                End If
            Case "check"
                If Not lock_type = Nothing Then
                    If lock_type = "work" Or lock_type = "review" Then
                        imageURL = "images/lock.png"
                        imageAltTag = "lock type is " & lock_type & ", locked by " & lock_uid & " " & lock_dt
                    ElseIf lock_type = "check" And lock_uid <> user_id Then
                        imageURL = "images/lock.png"
                        imageAltTag = "lock type is " & lock_type & ", locked by " & lock_uid & " " & lock_dt
                    ElseIf lock_type = "working" Or lock_type = "reviewing" Then
                        imageURL = "images/save_icon.gif"
                        imageAltTag = "updates pending by " & lock_uid & " " & lock_dt
                    ElseIf lock_type = "checking" And lock_uid <> user_id Then
                        imageURL = "images/save_icon.gif"
                        imageAltTag = "updates pending by " & lock_uid & " " & lock_dt
                    End If
                End If
            Case "review"
                If Not lock_type = Nothing Then
                    If lock_type = "work" Or lock_type = "check" Then
                        imageURL = "images/lock.png"
                        imageAltTag = "lock type is " & lock_type & ", locked by " & lock_uid & " " & lock_dt
                    ElseIf lock_type = "review" And lock_uid <> user_id Then
                        imageURL = "images/lock.png"
                        imageAltTag = "lock type is " & lock_type & ", locked by " & lock_uid & " " & lock_dt
                    ElseIf lock_type = "working" Or lock_type = "checking" Then
                        imageURL = "images/save_icon.gif"
                        imageAltTag = "updates pending by " & lock_uid & " " & lock_dt
                    ElseIf lock_type = "reviewing" And lock_uid <> user_id Then
                        imageURL = "images/save_icon.gif"
                        imageAltTag = "updates pending by " & lock_uid & " " & lock_dt
                    End If
                End If
        End Select

        If proptype = "imageURL" Then
            Return imageURL
        Else
            Return imageAltTag
        End If
    End Function

    ''' <summary>
    ''' Gets the period_id, period_beg_dt, period_end_dt, and status_va of the periods for a particular record and WY
    ''' </summary>
    Public Function GetPeriodsByWY(ByVal rms_record_id As Integer, ByVal WY As String) As DataSet
        Dim periods As New DataSet
        Dim start_dt As DateTime
        Dim end_dt As DateTime

        Try
            Dim wy_int As Integer = CInt(WY)
            wy_int = wy_int - 1
            Dim wy_start As String = CStr(wy_int)
            Dim start As String = "10/1/" & wy_start
            Dim enddt As String = "9/30/" & WY

            start_dt = start
            end_dt = enddt
        Catch ex As Exception
            start_dt = "1/1/1900"
            end_dt = "1/1/1900"
        End Try

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SP_RMS_Periods_per_WY", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@rms_record_id", SqlDbType.Int).Value = rms_record_id
            cmd.Parameters.Add("@start_dt", SqlDbType.DateTime).Value = start_dt
            cmd.Parameters.Add("@end_dt", SqlDbType.DateTime).Value = end_dt
            cmd.Parameters.Add("@status_va", SqlDbType.NVarChar, 15).Value = "0"

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(periods)

            cnx.Close()
        End Using

        Return periods
    End Function

    ''' <summary>
    ''' Gets the details for the two most recent periods of a record
    ''' </summary>
    ''' <param name="top_no">Submit value equal to 1 for the most recent period info, and submit 2 for the 
    ''' second most recent period info</param>
    Public Function Get2MostRecentPeriods(ByVal rms_record_id As Integer, ByVal top_no As Integer) As DataSet
        Dim periods As New DataSet

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SP_RMS_Periods_top1", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@rms_record_id", SqlDbType.Int).Value = rms_record_id
            cmd.Parameters.Add("@top_no", SqlDbType.Int).Value = top_no

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(periods)

            cnx.Close()
        End Using

        Return periods
    End Function
#End Region
End Class
