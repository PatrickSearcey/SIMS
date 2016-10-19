Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

Public Class Hospital
    Private _id As Integer
    Private _wsc_id As Integer
    Private _name As String
    Private _street_addrs As String
    Private _city As String
    Private _state As String
    Private _zip As String
    Private _ph_no As String
    Private _dec_lat_va As Double
    Private _dec_long_va As Double

    Public Sub New(ByVal hospital_id As Integer)
        _id = hospital_id

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As SqlCommand = New SqlCommand("SP_Hospitals", cnx)
            Dim da As SqlDataAdapter
            Dim dt As DataTable = New DataTable

            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@hospital_nm", SqlDbType.NVarChar, 100).Value = "0"
            cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@hospital_id", SqlDbType.Int).Value = hospital_id

            da = New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                _wsc_id = row("wsc_id")
                _name = row("hospital_nm").ToString
                _street_addrs = row("street_addrs").ToString
                _city = row("city").ToString
                _state = row("state").ToString
                _zip = row("zip").ToString
                _ph_no = row("ph_no").ToString
                If row("dec_lat_va") Is DBNull.Value Then
                    _dec_lat_va = Nothing
                Else
                    _dec_lat_va = row("dec_lat_va").ToString
                End If
                If row("dec_long_va") Is DBNull.Value Then
                    _dec_long_va = Nothing
                Else
                    _dec_long_va = row("dec_long_va").ToString
                End If
            Next

            cnx.Close()
        End Using
    End Sub

#Region "Class Properties"
    ''' <summary>
    ''' Gets or sets the hospital ID
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
    ''' Gets or sets the hospital name
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
    ''' Gets or sets the WSC ID of the hospital
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
    ''' Gets or sets the hospital's street address
    ''' </summary>
    Public Property StreetAddress() As String
        Get
            StreetAddress = _street_addrs
        End Get
        Set(ByVal value As String)
            _street_addrs = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the hospital's city
    ''' </summary>
    Public Property City() As String
        Get
            City = _city
        End Get
        Set(ByVal value As String)
            _city = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the hospital's state
    ''' </summary>
    Public Property State() As String
        Get
            State = _state
        End Get
        Set(value As String)
            _state = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the hospital's zip code
    ''' </summary>
    Public Property Zip() As String
        Get
            Zip = _zip
        End Get
        Set(value As String)
            _zip = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the hospital's phone number
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
    ''' Gets or sets the decimal latitude of the hosiptal
    ''' </summary>
    Public Property Latitude() As Double
        Get
            Latitude = _dec_lat_va
        End Get
        Set(value As Double)
            _dec_lat_va = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the decimal longitude of the hospital
    ''' </summary>
    Public Property Longitude() As Double
        Get
            Longitude = _dec_long_va
        End Get
        Set(value As Double)
            _dec_long_va = value
        End Set
    End Property
#End Region

#Region "Class Methods"
    ''' <summary>
    ''' Updates the information for a hospital
    ''' </summary>
    Public Function UpdateHospitalDetails() As String
        Dim return_va As String = "success"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim dbcomm As New SqlCommand("sp_Hospitals_Update", cnx)
                dbcomm.CommandType = Data.CommandType.StoredProcedure
                dbcomm.Parameters.Add("@hospital_id", SqlDbType.Int).Value = _id
                dbcomm.Parameters.Add("@hospital_nm", SqlDbType.NVarChar, 150).Value = _name
                dbcomm.Parameters.Add("@street_addrs", SqlDbType.NVarChar, 150).Value = _street_addrs
                dbcomm.Parameters.Add("@city", SqlDbType.NVarChar, 150).Value = _city
                dbcomm.Parameters.Add("@state", SqlDbType.NVarChar, 2).Value = _state
                dbcomm.Parameters.Add("@zip", SqlDbType.NVarChar, 10).Value = _zip
                dbcomm.Parameters.Add("@ph_no", SqlDbType.NVarChar, 15).Value = _ph_no
                dbcomm.Parameters.Add("@dec_lat_va", SqlDbType.Float).Value = _dec_lat_va
                dbcomm.Parameters.Add("@dec_long_va", SqlDbType.Float).Value = _dec_long_va

                dbcomm.ExecuteNonQuery()
            Catch ex As Exception
                return_va = "There was an error updating the hospital. Error: " + ex.Message
            End Try

            cnx.Close()
        End Using

        Return return_va
    End Function

    ''' <summary>
    ''' Creates a hospital
    ''' </summary>
    Public Function AddHospital() As String
        Dim return_va As String = "success"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim dbcomm As New SqlCommand("spz_AddHospital", cnx)
                dbcomm.CommandType = Data.CommandType.StoredProcedure
                dbcomm.Parameters.Add("@wsc_id", SqlDbType.Int).Value = _wsc_id
                dbcomm.Parameters.Add("@hospital_nm", SqlDbType.NVarChar, 150).Value = _name
                dbcomm.Parameters.Add("@street_addrs", SqlDbType.NVarChar, 150).Value = _street_addrs
                dbcomm.Parameters.Add("@city", SqlDbType.NVarChar, 150).Value = _city
                dbcomm.Parameters.Add("@state", SqlDbType.NVarChar, 2).Value = _state
                dbcomm.Parameters.Add("@zip", SqlDbType.NVarChar, 10).Value = _zip
                dbcomm.Parameters.Add("@ph_no", SqlDbType.NVarChar, 15).Value = _ph_no
                dbcomm.Parameters.Add("@dec_lat_va", SqlDbType.Float).Value = _dec_lat_va
                dbcomm.Parameters.Add("@dec_long_va", SqlDbType.Float).Value = _dec_long_va
                dbcomm.Parameters.Add("@hospital_id", SqlDbType.Int).Value = DBNull.Value
                dbcomm.Parameters.Add("@sha_site_id", SqlDbType.Int).Value = DBNull.Value

                dbcomm.ExecuteNonQuery()
            Catch ex As Exception
                return_va = "There was an error adding the hospital to the database. Error: " + ex.Message
            End Try

            cnx.Close()
        End Using

        Return return_va
    End Function

    ''' <summary>
    ''' Add a pre-existing hospital to a site's SHA
    ''' </summary>
    Public Function AddHospitalToSite(ByVal sha_site_id As Integer) As String
        Dim return_va As String = "success"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim dbcomm As New SqlCommand("spz_AddHospital", cnx)
                dbcomm.CommandType = Data.CommandType.StoredProcedure
                dbcomm.Parameters.Add("@wsc_id", SqlDbType.Int).Value = DBNull.Value
                dbcomm.Parameters.Add("@hospital_nm", SqlDbType.NVarChar, 150).Value = DBNull.Value
                dbcomm.Parameters.Add("@street_addrs", SqlDbType.NVarChar, 150).Value = DBNull.Value
                dbcomm.Parameters.Add("@city", SqlDbType.NVarChar, 150).Value = DBNull.Value
                dbcomm.Parameters.Add("@state", SqlDbType.NVarChar, 2).Value = DBNull.Value
                dbcomm.Parameters.Add("@zip", SqlDbType.NVarChar, 10).Value = DBNull.Value
                dbcomm.Parameters.Add("@ph_no", SqlDbType.NVarChar, 15).Value = DBNull.Value
                dbcomm.Parameters.Add("@dec_lat_va", SqlDbType.Float).Value = DBNull.Value
                dbcomm.Parameters.Add("@dec_long_va", SqlDbType.Float).Value = DBNull.Value
                dbcomm.Parameters.Add("@hospital_id", SqlDbType.Int).Value = _id
                dbcomm.Parameters.Add("@sha_site_id", SqlDbType.Int).Value = sha_site_id

                dbcomm.ExecuteNonQuery()
            Catch ex As Exception
                return_va = "There was an error adding the hospital to this site. Error: " + ex.Message
            End Try

            cnx.Close()
        End Using

        Return return_va
    End Function
#End Region
End Class
