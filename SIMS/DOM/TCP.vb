Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

''' <summary>
''' This is the class that should be used when working on safety documents applications
''' </summary>
''' <remarks></remarks>
Public Class TCP
    Private _no As Integer
    Private _name As String

    ''' <summary>
    ''' Initializes the TCP class and gets the properties based on the passed plan number
    ''' </summary>
    Public Sub New(ByVal plan_no As Integer)
        _no = plan_no

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As SqlCommand = New SqlCommand("SELECT plan_no, plan_nm, plan_subnm, notes, speed_tbl," & _
                " stream, lake, estuary, specificsource, spring, groundwater, meteorological, outfall," & _
                " diversion, landapplication, aggregategroundwater, wateruse_placeofuse, coastalqw," & _
                " aggregatesurfacewater FROM TCP_Lut_PlanDetails WHERE (plan_no = " & _no & ")", cnx)
            Dim da As SqlDataAdapter
            Dim dt As DataTable = New DataTable

            da = New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                _name = row("plan_nm").ToString
            Next

            cnx.Close()
        End Using
    End Sub

#Region "Class Properties"
    ''' <summary>
    ''' Gets or sets the name of the TCP
    ''' </summary>
    Public Property Name() As String
        Get
            Name = _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property
#End Region
End Class
