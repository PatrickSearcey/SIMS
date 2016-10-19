Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

Public Class District
    Private _cd As String
    Private _name As String
    Private _abbrev As String
    Private _wsc_id As Integer
    Private svcSIMS As SIMSService.SIMSServiceClient

    ''' <summary>
    ''' Initializes the District class and fills properties with general 
    ''' info about the district from SP_District_Info stored procedure
    ''' </summary>
    ''' <param name="district_cd"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal district_cd As String)
        _cd = district_cd

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand("SP_District_Info", cnx)
            Dim dt As New DataTable
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@district_cd", SqlDbType.NVarChar, 3).Value = _cd
            cmd.Parameters.Add("@primaryOU", SqlDbType.NVarChar, 255).Value = "0"
            cmd.Parameters.Add("@region_cd", SqlDbType.NVarChar, 50).Value = "0"

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                _name = row("district_nm").ToString
                _abbrev = row("district_abbrev").ToString
                _wsc_id = Convert.ToInt32(row("wsc_id"))
            Next

            cnx.Close()
        End Using
    End Sub

#Region "Class Properties"
    ''' <summary>
    ''' Gets or sets the code for the district (or WSC)
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
    ''' Gets or sets the name of the district
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
    ''' Gets or sets the district (state) abbreviation
    ''' </summary>
    Public Property Abbrev() As String
        Get
            Abbrev = _abbrev
        End Get
        Set(ByVal value As String)
            _abbrev = value
        End Set
    End Property
    ''' <summary>
    ''' Gets the ID of the WSC that the district belongs to
    ''' </summary>
    Public Property WSCID() As Integer
        Get
            WSCID = _wsc_id
        End Get
        Set(value As Integer)
            _wsc_id = value
        End Set
    End Property
#End Region

#Region "Class Methods"
    ''' <summary>
    ''' Returns a list of district abbreviations as a DataTable
    ''' </summary>
    Public Function GetDistricts() As DataTable
        Dim dt As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand("SP_District_Info", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@district_cd", SqlDbType.NVarChar).Value = "0"
            cmd.Parameters.Add("@primaryOU", SqlDbType.NVarChar).Value = "0"
            cmd.Parameters.Add("@region_cd", SqlDbType.NVarChar).Value = "0"

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            cnx.Close()
        End Using

        Return dt
    End Function
#End Region
End Class
