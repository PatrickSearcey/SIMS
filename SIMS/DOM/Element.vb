Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

Public Class Element
    Private _id As Integer
    Private _name As String
    Private _priority As Integer
    Private _remark As String
    Private _report_type_cd As New ArrayList

    Public Sub New(ByVal element_id As Integer)
        _id = element_id

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As SqlCommand = New SqlCommand("SELECT eled.element_id, eled.element_nm, eled.priority, " & _
                "eled.remark, elrr.report_type_cd" & _
                " FROM Elem_Lut_ElemDetail AS eled INNER JOIN" & _
                " Elem_Lut_ReportRef AS elrr ON eled.element_id = elrr.element_id" & _
                " WHERE eled.element_id = " & element_id, cnx)
            Dim da As SqlDataAdapter
            Dim dt As DataTable = New DataTable

            da = New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                _name = row("element_nm")
                _priority = row("priority")
                If Not row("remark") Is DBNull.Value Then
                    _remark = row("remark")
                End If
                If Not row("report_type_cd") Is DBNull.Value Then
                    _report_type_cd.Add(row("report_type_cd"))
                End If
            Next

            cnx.Close()
        End Using
    End Sub

#Region "Class Properties"
    ''' <summary>
    ''' Gets or sets the name of the element
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
    ''' Gets or sets the priority of the element (the value that determins the placement in the element list)
    ''' </summary>
    Public Property Priority() As Integer
        Get
            Priority = _priority
        End Get
        Set(ByVal value As Integer)
            _priority = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets remarks made about the element
    ''' </summary>
    Public Property Remark() As String
        Get
            Remark = _remark
        End Get
        Set(ByVal value As String)
            _remark = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets an array list containing the report type codes of the reports the element can be found in
    ''' </summary>
    Public Property ReportType() As ArrayList
        Get
            ReportType = _report_type_cd
        End Get
        Set(ByVal value As ArrayList)
            _report_type_cd = value
        End Set
    End Property
#End Region

End Class
