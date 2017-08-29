Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data
Imports System.Reflection

''' <summary>
''' This is the class that holds the properties associated with a user, including access level
''' </summary>
''' <remarks></remarks>
Public Class User
    Private _id As String
    Private _first_nm As String
    Private _last_nm As String
    Private _wsc_id As Integer
    Private _office_id As Integer
    Private _access_level As String
    Private _pass_access As String
    Private _approver_va As Boolean
    Private _active As Boolean
    Private _show_reports As Boolean
    Private _primaryOU As String
    Private _user_exception As Array
    Private _wsc_exception As Array

    ''' <summary>
    ''' Initializes the User class and populates properties with info pertaining to passed user ID
    ''' </summary>
    ''' <param name="user_id">The user ID identified by integrated authentication</param>
    Public Sub New(ByVal user_id As String)
#If DEBUG Then
        user_id = "GS\dterry"
#End If
        _id = Replace(user_id, "GS\", "")
        _wsc_id = GetUserWSCID(_id)
        _user_exception = GetUserExceptions(_id)
        _wsc_exception = GetWSCExceptions(PrimaryOU)
        GetUserInfo(_id)
    End Sub

#Region "Class Properties"
    ''' <summary>
    ''' Gets or sets the user id of the current authenticated user
    ''' </summary>
    Public Property ID() As String
        Get
            ID = _id
        End Get
        Set(ByVal value As String)
            _id = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the first name of the user
    ''' </summary>
    Public Property FirstName() As String
        Get
            FirstName = _first_nm
        End Get
        Set(value As String)
            _first_nm = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the last name of the user
    ''' </summary>
    Public Property LastName() As String
        Get
            LastName = _last_nm
        End Get
        Set(value As String)
            _last_nm = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the WSC ID of the current authenticated user
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
    ''' Gets or sets the office ID to which the current authenticated user belongs
    ''' </summary>
    ''' <remarks>This is based on the office_id value in tblEmployees</remarks>
    Public Property OfficeID() As Integer
        Get
            OfficeID = _office_id
        End Get
        Set(ByVal value As Integer)
            _office_id = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the access level of the current authenticated user
    ''' </summary>
    Public Property AccessLevel() As String
        Get
            AccessLevel = _access_level
        End Get
        Set(ByVal value As String)
            _access_level = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the level of access the user has to the PASS system
    ''' </summary>
    Public Property PASSAccess() As String
        Get
            PASSAccess = _pass_access
        End Get
        Set(value As String)
            _pass_access = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the safety approver value for the current authenticated user
    ''' </summary>
    ''' <remarks>If true, then the user has the ability to approve safety documents in SIMS</remarks>
    Public Property ApproverValue() As Boolean
        Get
            ApproverValue = _approver_va
        End Get
        Set(ByVal value As Boolean)
            _approver_va = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the status of the user, either active or inactive
    ''' </summary>
    Public Property Active() As Boolean
        Get
            Active = _active
        End Get
        Set(value As Boolean)
            _active = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the toggle for the user to show up on the RMS Reports page
    ''' </summary>
    Public Property ShowReports() As Boolean
        Get
            ShowReports = _show_reports
        End Get
        Set(value As Boolean)
            _show_reports = value
        End Set
    End Property
    ''' <summary>
    ''' Gets the PrimaryOU from Active Directory of the current user
    ''' </summary>
    Public ReadOnly Property PrimaryOU() As String
        Get
            PrimaryOU = _primaryOU
        End Get
    End Property
    ''' <summary>
    ''' Gets or sets the user exception for the authenticated user
    ''' </summary>
    Public Property UserException() As Array
        Get
            UserException = _user_exception
        End Get
        Set(ByVal value As Array)
            _user_exception = value()
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the WSC exception for the authenticated user's primary OU from AD
    ''' </summary>
    Public Property WSCException() As Array
        Get
            WSCException = _wsc_exception
        End Get
        Set(ByVal value As Array)
            _wsc_exception = value()
        End Set
    End Property
#End Region

#Region "Class Methods"
    ''' <summary>
    ''' Gets the wsc ID associated with the user; based on AD info
    ''' </summary>
    Public Function GetUserWSCID(ByVal user_id As String) As String
        Dim wsc_id As Integer = 0

        Try
            Using cnx As New SqlConnection(Config.ConnectionInfo)
                cnx.Open()
                Dim cmd As New SqlCommand("spz_GetUserInfoFromAD", cnx)
                Dim da As SqlDataAdapter
                Dim dt As DataTable = New DataTable

                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.Add("@user_id", SqlDbType.NVarChar, 50).Value = user_id

                da = New SqlDataAdapter(cmd)
                da.Fill(dt)

                For Each row As DataRow In dt.Rows
                    _primaryOU = row("primaryOU").ToString
                Next

                If user_id = "krspicer" Or user_id = "saunders" Or user_id = "mosbruck" Then
                    _primaryOU = "TacomaWA-W"
                End If

                Dim cmd1 As New SqlCommand("spz_GetUserWSCID", cnx)
                Dim dt1 As New DataTable
                cmd1.CommandType = CommandType.StoredProcedure
                cmd1.Parameters.Add("@PrimaryOU", SqlDbType.NVarChar, 50).Value = PrimaryOU

                Dim da1 As New SqlDataAdapter(cmd1)
                da1.Fill(dt1)

                For Each row As DataRow In dt1.Rows
                    wsc_id = row("wsc_id")
                Next

                cnx.Close()
            End Using
        Catch ex As Exception
            wsc_id = 31
        End Try

        Return wsc_id
    End Function

    ''' <summary>
    ''' Gets the first and last name, separated by comma, from AD
    ''' </summary>
    Public Function GetUserNameFromAD(ByVal user_id As String) As String
        Dim name As String = ""
        Dim ui As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("spz_GetUserInfoFromAD", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@user_id", SqlDbType.NVarChar).Value = user_id

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(ui)

            For Each row As DataRow In ui.Rows
                name = row("givenName") & "," & row("SN")
            Next

            cnx.Close()
        End Using

        Return name
    End Function

    ''' <summary>
    ''' Gets the WSC ID of the WSC for which the user has been setup as having an exception made
    ''' </summary>
    ''' <remarks>The exception is set in the lut_Exception table for a user</remarks>
    Public Function GetUserExceptions(ByVal user_id As String) As Array
        Dim dceArray() As String = Nothing
        Dim exception As String = Nothing

        Try
            Using cnx As New SqlConnection(Config.ConnectionInfo)
                cnx.Open()
                Dim cmd As New SqlCommand("SP_Personnel_Exceptions ", cnx)
                Dim dt As DataTable = New DataTable

                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.Add("@user_id", SqlDbType.NVarChar, 30).Value = user_id

                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)

                For Each row As DataRow In dt.Rows
                    exception = exception & row("exc_wsc_id").ToString & ","
                Next

                If Not String.IsNullOrEmpty(exception) Then
                    exception = Mid(exception, 1, Len(exception) - 1)
                    dceArray = exception.Split(",")
                End If

                cnx.Close()
            End Using
        Catch ex As Exception
        End Try
        
        Return dceArray
    End Function

    ''' <summary>
    ''' Gets the WSC IDs of the WSC for which the user's primary OU in AD has been setup as having an exception made
    ''' </summary>
    ''' <remarks>The exception is set in the lut_Exception_WSC table for a primary OU</remarks>
    Public Function GetWSCExceptions(ByVal AD_OU As String) As Array
        Dim wsceArray() As String = Nothing
        Dim wsc_exception As String = Nothing

        Try
            Using cnx As New SqlConnection(Config.ConnectionInfo)
                cnx.Open()
                Dim cmd As New SqlCommand("SP_WSC_Exceptions", cnx)
                Dim dt As DataTable = New DataTable

                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.Add("@primaryOU", SqlDbType.NVarChar, 100).Value = AD_OU

                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)

                For Each row As DataRow In dt.Rows
                    wsc_exception = wsc_exception & row("exc_wsc_id").ToString & ","
                Next

                If Not String.IsNullOrEmpty(wsc_exception) Then
                    wsc_exception = Mid(wsc_exception, 1, Len(wsc_exception) - 1)
                    wsceArray = wsc_exception.Split(",")
                End If

                cnx.Close()
            End Using

        Catch ex As Exception
        End Try

        Return wsceArray
    End Function

    ''' <summary>
    ''' Gets the information for the current user based on settings in tblEmployees
    ''' </summary>
    ''' <remarks>Information gathered: access level, user's office ID, safety approver value</remarks>
    Public Sub GetUserInfo(ByVal user_id As String)
        _access_level = "None"
        _approver_va = False

        Try
            Using cnx As New SqlConnection(Config.ConnectionInfo)
                cnx.Open()
                Dim cmd As New SqlCommand("SP_Personnel_by_WSC_office_or_user_id", cnx)
                Dim dt As DataTable = New DataTable

                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = 0
                cmd.Parameters.Add("@office_id", SqlDbType.Int).Value = 0
                cmd.Parameters.Add("@user_id", SqlDbType.NVarChar, 30).Value = user_id
                cmd.Parameters.Add("@show_reports", SqlDbType.NVarChar, 3).Value = "no"
                cmd.Parameters.Add("@status", SqlDbType.NVarChar, 4).Value = "True"
                cmd.Parameters.Add("@manage", SqlDbType.NVarChar, 3).Value = "no"

                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)

                If dt.Rows.Count = 0 Then
                    _access_level = "None"
                    _office_id = 0
                    _approver_va = False
                    _first_nm = ""
                    _last_nm = ""
                    _pass_access = False
                    _active = False
                    _show_reports = False
                Else
                    For Each row As DataRow In dt.Rows
                        _access_level = row("administrator_va").ToString
                        _office_id = row("office_id")
                        _first_nm = row("first_nm").ToString
                        _last_nm = row("last_nm").ToString
                        _pass_access = row("pass_access")
                        _active = row("active")
                        _show_reports = row("show_reports")
                        If row("approver_va") Is DBNull.Value Then
                            _approver_va = False
                        Else
                            _approver_va = row("approver_va")
                        End If
                    Next
                End If

                cnx.Close()
            End Using
        Catch ex As Exception
            _access_level = "None"
            _office_id = 0
            _approver_va = False
            _first_nm = ""
            _last_nm = ""
            _pass_access = False
            _active = False
            _show_reports = False
        End Try

    End Sub

    ''' <summary>
    ''' Gets the office ID associated with the user; based on IP address
    ''' </summary>
    Public Function GetUserIPOfficeID(ByVal user_ip As String) As Integer
        Dim sql As String = Nothing
        Dim dbcomm As SqlCommand
        Dim dr As SqlDataReader
        Dim office_id As Integer
        Dim arrIP As Array = Split(user_ip, ".")

        Try
            user_ip = arrIP(0) & "." & arrIP(1) & "." & arrIP(2)

            Using cnx As New SqlConnection(Config.ConnectionInfo)
                cnx.Open()
                sql = "SELECT lo.office_id" & _
                    " FROM lut_Office AS lo" & _
                    " WHERE lo.office_subnet = '" & user_ip & "'"
                dbcomm = New SqlCommand(sql, cnx)
                dr = dbcomm.ExecuteReader

                If dr.HasRows Then
                    dr.Read()
                    office_id = dr.GetValue(0)
                End If

                dr.Close()
                cnx.Close()
            End Using
        Catch ex As Exception
            office_id = 631
        End Try

        Return office_id
    End Function

    ''' <summary>
    ''' Gets the email alias for a user based on AD information
    ''' </summary>
    ''' <param name="user_id">This is the 8 character or less user_id that a person logs in with</param>
    Public Function GetUserEmailAliasFromAD(ByVal user_id As String) As String
        Dim email_address As String = user_id & "@usgs.gov"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("spz_GetUserEmailAliasFromAD", cnx)
            Dim dt As DataTable = New DataTable

            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.Add("@user_id", SqlDbType.NVarChar, 30).Value = user_id

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            If dt.Rows.Count > 0 Then
                For Each row As DataRow In dt.Rows
                    email_address = row("mail").ToString
                Next
            End If

            cnx.Close()
        End Using

        Return email_address
    End Function

    ''' <summary>
    ''' Updates the tblEmployees table with general user information
    ''' </summary>
    ''' <returns>A string "success" if no errors, or errors if found.</returns>
    Public Function UpdateUserInfo(ByVal userid As String) As String
        Dim errmessage As String = "success"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim cmd As New SqlCommand("SP_Personnel_Update", cnx)
                Dim dt As DataTable = New DataTable

                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.Add("@user_id", SqlDbType.NVarChar, 15).Value = userid
                cmd.Parameters.Add("@first_nm", SqlDbType.NVarChar, 75).Value = FirstName
                cmd.Parameters.Add("@last_nm", SqlDbType.NVarChar, 75).Value = LastName
                cmd.Parameters.Add("@office_id", SqlDbType.Int).Value = OfficeID
                cmd.Parameters.Add("@administrator_va", SqlDbType.NVarChar, 50).Value = AccessLevel
                cmd.Parameters.Add("@pass_access", SqlDbType.NVarChar, 50).Value = PASSAccess
                cmd.Parameters.Add("@approver_va", SqlDbType.Bit).Value = ApproverValue
                cmd.Parameters.Add("@active", SqlDbType.Bit).Value = Active
                cmd.Parameters.Add("@show_reports", SqlDbType.Bit).Value = ShowReports
                cmd.Parameters.Add("@pre_user_id", SqlDbType.NVarChar, 15).Value = userid
                cmd.ExecuteNonQuery()
            Catch ex As Exception
                errmessage = ex.Message
            End Try

            cnx.Close()
        End Using

        Return errmessage
    End Function

    ''' <summary>
    ''' Adds a new user into the tblEmployees table.
    ''' </summary>
    ''' <returns>A string "success" if no errors, or errors if found.</returns>
    Public Function InsertUserInfo(ByVal userid As String) As String
        Dim errmessage As String = "success"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim cmd As New SqlCommand("SP_Personnel_Add", cnx)
                Dim dt As DataTable = New DataTable

                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.Add("@user_id", SqlDbType.NVarChar, 15).Value = userid
                cmd.Parameters.Add("@first_nm", SqlDbType.NVarChar, 75).Value = FirstName
                cmd.Parameters.Add("@last_nm", SqlDbType.NVarChar, 75).Value = LastName
                cmd.Parameters.Add("@office_id", SqlDbType.Int).Value = OfficeID
                cmd.Parameters.Add("@administrator_va", SqlDbType.NVarChar, 50).Value = AccessLevel
                cmd.Parameters.Add("@pass_access", SqlDbType.NVarChar, 50).Value = PASSAccess
                cmd.Parameters.Add("@approver_va", SqlDbType.Bit).Value = ApproverValue
                cmd.Parameters.Add("@active", SqlDbType.Bit).Value = Active
                cmd.Parameters.Add("@show_reports", SqlDbType.Bit).Value = ShowReports
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
