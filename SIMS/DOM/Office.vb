Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

''' <summary>
''' This is the class that holds all properties and methods associated with an office
''' </summary>
''' <remarks></remarks>
Public Class Office
    Private _id As Integer
    Private _name As String
    Private _street_address As String
    Private _city_st_zip As String
    Private _cd As String
    Private _wsc_id As Integer
    Private _subnet As String
    Private _dec_lat_va As Double
    Private _dec_long_va As Double
    Private _ph_no As String
    Private _data_chief_email As String
    Private _reviewer_email As String
    Private _start_new_rec As Boolean

    ''' <summary>
    ''' Initializes the Office class and gets the properties based on the office ID
    ''' </summary>
    ''' <param name="office_id"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal office_id As Integer)
        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As SqlCommand = New SqlCommand("SP_Office_Info", cnx)
            Dim da As SqlDataAdapter
            Dim dt As DataTable = New DataTable

            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@office_id", SqlDbType.Int).Value = office_id
            cmd.Parameters.Add("@office_cd", SqlDbType.NVarChar, 3).Value = "0"
            cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@action", SqlDbType.NVarChar, 12).Value = "byofficeid"

            da = New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                _id = row("office_id").ToString
                _name = row("office_nm").ToString
                _cd = row("office_cd").ToString
                _street_address = row("street_addrs").ToString
                _city_st_zip = row("city_st_zip").ToString
                _ph_no = row("ph_no").ToString
                _wsc_id = row("wsc_id")
                If row("reviewer_email") Is DBNull.Value Then
                    _reviewer_email = ""
                Else
                    _reviewer_email = row("reviewer_email").ToString
                End If
            Next

            cnx.Close()
        End Using
    End Sub

    Public Sub New(ByVal office_cd As String)
        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As SqlCommand = New SqlCommand("SP_Office_Info", cnx)
            Dim da As SqlDataAdapter
            Dim dt As DataTable = New DataTable

            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@office_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@office_cd", SqlDbType.NVarChar, 3).Value = office_cd
            cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@action", SqlDbType.NVarChar, 12).Value = "byofficeid"

            da = New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                _id = row("office_id").ToString
                _name = row("office_nm").ToString
                _cd = row("office_cd").ToString
                _street_address = row("street_addrs").ToString
                _city_st_zip = row("city_st_zip").ToString
                _ph_no = row("ph_no").ToString
                _wsc_id = row("wsc_id")
                If row("reviewer_email") Is DBNull.Value Then
                    _reviewer_email = ""
                Else
                    _reviewer_email = row("reviewer_email").ToString
                End If
            Next

            cnx.Close()
        End Using
    End Sub

#Region "Class Properties"
    ''' <summary>
    ''' Gets or sets the office ID
    ''' </summary>
    Public Property ID() As Integer
        Get
            ID = _id
        End Get
        Set(ByVal value As Integer)
            _id = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the office code
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
    ''' Gets or sets the office name
    ''' </summary>
    Public Property Name() As String
        Get
            Name = _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the WSC ID of the office
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
    ''' Gets or sets the office's street address
    ''' </summary>
    Public Property StreetAddress() As String
        Get
            StreetAddress = _street_address
        End Get
        Set(ByVal value As String)
            _street_address = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the office's city, state, and zip
    ''' </summary>
    Public Property CityStateZip() As String
        Get
            CityStateZip = _city_st_zip
        End Get
        Set(ByVal value As String)
            _city_st_zip = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the office's phone number
    ''' </summary>
    Public Property PhoneNo() As String
        Get
            PhoneNo = _ph_no
        End Get
        Set(ByVal value As String)
            _ph_no = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the reviewer email for the office
    ''' </summary>
    ''' <remarks>An email is sent to this address whenever a record has been checked or reviewed under
    ''' a particular office</remarks>
    Public Property ReviewerEmail() As String
        Get
            ReviewerEmail = _reviewer_email
        End Get
        Set(ByVal value As String)
            _reviewer_email = value
        End Set
    End Property
#End Region

#Region "Class Methods"
    ''' <summary>
    ''' Returns a list of sites that are ready for work/check/review based on the passed choice
    ''' </summary>
    ''' <param name="listtype">The list to be retrieved; accepted are work, check, or review</param>
    ''' <param name="office_id">The office_id of the office</param>
    ''' <param name="range_start">The start value for the last approved in ADAPS days range; enter "min" for all</param>
    ''' <param name="range_end">The end value for the last approved in ADAPS days range; enter "max" for all</param>
    ''' <returns>A DataTable to be bound to a control</returns>
    Public Function GetRecordsToBeProcessed(ByVal listtype As String, ByVal office_id As Integer, ByVal range_start As String, ByVal range_end As String, ByVal rms_record_id As Integer) As DataTable
        Dim records As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand("SP_Records_To_Be_Processed", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@listtype", SqlDbType.NVarChar, 6).Value = listtype
            cmd.Parameters.Add("@office_id", SqlDbType.Int).Value = office_id
            cmd.Parameters.Add("@daysago_start", SqlDbType.NVarChar, 3).Value = range_start
            cmd.Parameters.Add("@daysago_end", SqlDbType.NVarChar, 3).Value = range_end
            cmd.Parameters.Add("@rms_record_id", SqlDbType.Int).Value = rms_record_id
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(records)

            cnx.Close()
        End Using

        Return records
    End Function

    ''' <summary>
    ''' Returns a list of field trips for the office
    ''' </summary>
    Public Function GetFieldTrips(ByVal office_id As Integer) As DataTable
        Dim fieldtrips As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand("SP_Trip_Info", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@office_id", SqlDbType.Int).Value = office_id
            cmd.Parameters.Add("@trip_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@trip_nm", SqlDbType.NVarChar, 255).Value = "0"
            cmd.Parameters.Add("@user_id", SqlDbType.NVarChar, 50).Value = "0"
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(fieldtrips)

            cnx.Close()
        End Using

        Return fieldtrips
    End Function

#End Region

End Class
