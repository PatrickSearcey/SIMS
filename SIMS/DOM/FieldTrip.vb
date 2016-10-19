Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

Public Class FieldTrip
    Private _id As Integer
    Private _office_id As Integer
    Private _name As String
    Private _user_id As String
    Private _full_nm As String

    Public Sub New(ByVal trip_id As Integer)
        _id = trip_id

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As SqlCommand = New SqlCommand("SP_Trip_Info", cnx)
            Dim da As SqlDataAdapter
            Dim dt As DataTable = New DataTable

            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@office_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@trip_id", SqlDbType.Int).Value = trip_id
            cmd.Parameters.Add("@trip_nm", SqlDbType.NVarChar, 50).Value = "0"
            cmd.Parameters.Add("@user_id", SqlDbType.NVarChar, 50).Value = "0"

            da = New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                _office_id = row("office_id")
                _name = row("trip_nm").ToString
                _user_id = row("user_id").ToString
                _full_nm = _name & " - " & _user_id
            Next

            cnx.Close()
        End Using
    End Sub

#Region "Class Properties"
    ''' <summary>
    ''' Gets or sets the ID of the trip
    ''' </summary>
    Public Property ID() As Integer
        Get
            Return _id
        End Get
        Set(ByVal value As Integer)
            _id = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the ID of the office the trip belongs to
    ''' </summary>
    Public Property OfficeID() As Integer
        Get
            Return _office_id
        End Get
        Set(ByVal value As Integer)
            _office_id = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the name of the field trip
    ''' </summary>
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the user ID of the person assigned to the field trip
    ''' </summary>
    Public Property UserID() As String
        Get
            Return _user_id
        End Get
        Set(ByVal value As String)
            _user_id = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the full trip name - it's just the name plus the user ID
    ''' </summary>
    Public Property FullTripName() As String
        Get
            Return _full_nm
        End Get
        Set(ByVal value As String)
            _full_nm = value
        End Set
    End Property
#End Region

End Class
