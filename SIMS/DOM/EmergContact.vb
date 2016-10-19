Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

Public Class EmergContact
    Private _id As Integer
    Private _wsc_id As Integer
    Private _name As String
    Private _street_addrs As String
    Private _city As String
    Private _state As String
    Private _zip As String
    Private _ph_no As String

    Public Sub New(ByVal contact_id As Integer)
        _id = contact_id

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As SqlCommand = New SqlCommand("SP_Contacts", cnx)
            Dim da As SqlDataAdapter
            Dim dt As DataTable = New DataTable

            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@contact_nm", SqlDbType.NVarChar, 100).Value = "0"
            cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@contact_id", SqlDbType.Int).Value = contact_id

            da = New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                _wsc_id = row("wsc_id")
                _name = row("contact_nm").ToString
                _street_addrs = row("street_addrs").ToString
                _city = row("city").ToString
                _state = row("state").ToString
                _zip = row("zip").ToString
                _ph_no = row("ph_no").ToString
            Next

            cnx.Close()
        End Using
    End Sub

#Region "Class Properties"
    ''' <summary>
    ''' Gets or sets the contact ID
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
    ''' Gets or sets the contact name
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
    ''' Gets or sets the WSC ID of the contact
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
    ''' Gets or sets the contact's street address
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
    ''' Gets or sets the contact's city
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
    ''' Gets or sets the contact's state
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
    ''' Gets or sets the contact's zip code
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
    ''' Gets or sets the contact's phone number
    ''' </summary>
    Public Property PhoneNo() As String
        Get
            PhoneNo = _ph_no
        End Get
        Set(ByVal value As String)
            _ph_no = value
        End Set
    End Property
#End Region

#Region "Class Methods"
    ''' <summary>
    ''' Updates the information for a contact
    ''' </summary>
    Public Function UpdateContactDetails() As String
        Dim return_va As String = "success"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim dbcomm As New SqlCommand("sp_Contacts_Update", cnx)
                dbcomm.CommandType = Data.CommandType.StoredProcedure
                dbcomm.Parameters.Add("@contact_id", SqlDbType.Int).Value = _id
                dbcomm.Parameters.Add("@contact_nm", SqlDbType.NVarChar, 150).Value = _name
                dbcomm.Parameters.Add("@street_addrs", SqlDbType.NVarChar, 150).Value = _street_addrs
                dbcomm.Parameters.Add("@city", SqlDbType.NVarChar, 150).Value = _city
                dbcomm.Parameters.Add("@state", SqlDbType.NVarChar, 2).Value = _state
                dbcomm.Parameters.Add("@zip", SqlDbType.NVarChar, 10).Value = _zip
                dbcomm.Parameters.Add("@ph_no", SqlDbType.NVarChar, 15).Value = _ph_no

                dbcomm.ExecuteNonQuery()
            Catch ex As Exception
                return_va = "There was an error updating the contact. Error: " + ex.Message
            End Try

            cnx.Close()
        End Using

        Return return_va
    End Function

    ''' <summary>
    ''' Creates an emergency contact
    ''' </summary>
    Public Function AddContact() As String
        Dim return_va As String = "success"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim dbcomm As New SqlCommand("spz_AddContact", cnx)
                dbcomm.CommandType = Data.CommandType.StoredProcedure
                dbcomm.Parameters.Add("@wsc_id", SqlDbType.Int).Value = _wsc_id
                dbcomm.Parameters.Add("@contact_nm", SqlDbType.NVarChar, 150).Value = _name
                dbcomm.Parameters.Add("@street_addrs", SqlDbType.NVarChar, 150).Value = _street_addrs
                dbcomm.Parameters.Add("@city", SqlDbType.NVarChar, 150).Value = _city
                dbcomm.Parameters.Add("@state", SqlDbType.NVarChar, 2).Value = _state
                dbcomm.Parameters.Add("@zip", SqlDbType.NVarChar, 10).Value = _zip
                dbcomm.Parameters.Add("@ph_no", SqlDbType.NVarChar, 15).Value = _ph_no
                dbcomm.Parameters.Add("@contact_id", SqlDbType.Int).Value = DBNull.Value
                dbcomm.Parameters.Add("@sha_site_id", SqlDbType.Int).Value = DBNull.Value

                dbcomm.ExecuteNonQuery()
            Catch ex As Exception
                return_va = "There was an error adding the contact to the database. Error: " + ex.Message
            End Try

            cnx.Close()
        End Using

        Return return_va
    End Function

    ''' <summary>
    ''' Add a pre-existing emergency contact to a site's SHA
    ''' </summary>
    Public Function AddContactToSite(ByVal sha_site_id As Integer) As String
        Dim return_va As String = "success"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim dbcomm As New SqlCommand("spz_AddContact", cnx)
                dbcomm.CommandType = Data.CommandType.StoredProcedure
                dbcomm.Parameters.Add("@wsc_id", SqlDbType.Int).Value = DBNull.Value
                dbcomm.Parameters.Add("@contact_nm", SqlDbType.NVarChar, 150).Value = DBNull.Value
                dbcomm.Parameters.Add("@street_addrs", SqlDbType.NVarChar, 150).Value = DBNull.Value
                dbcomm.Parameters.Add("@city", SqlDbType.NVarChar, 150).Value = DBNull.Value
                dbcomm.Parameters.Add("@state", SqlDbType.NVarChar, 2).Value = DBNull.Value
                dbcomm.Parameters.Add("@zip", SqlDbType.NVarChar, 10).Value = DBNull.Value
                dbcomm.Parameters.Add("@ph_no", SqlDbType.NVarChar, 15).Value = DBNull.Value
                dbcomm.Parameters.Add("@contact_id", SqlDbType.Int).Value = _id
                dbcomm.Parameters.Add("@sha_site_id", SqlDbType.Int).Value = sha_site_id

                dbcomm.ExecuteNonQuery()
            Catch ex As Exception
                return_va = "There was an error adding the contact to this site. Error: " + ex.Message
            End Try

            cnx.Close()
        End Using

        Return return_va
    End Function
#End Region
End Class
