Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

Public Class SiteElement
    Private _id As Integer
    Private _site_id As Integer
    Private _element_id As Integer
    Private _element_info As String
    Private _revised_by As String
    Private _revised_dt As DateTime
    Private _entered_by As String
    Private _entered_dt As DateTime

    Public Sub New(ByVal element_id As Integer, ByVal site_id As Integer)
        _element_id = element_id
        _site_id = site_id

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand("SP_Element_Info_by_element_and_site_id", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@element_id", SqlDbType.Int).Value = element_id
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = site_id
            Dim dt As New DataTable
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                _id = row("elem_site_id")
                If Not row("element_info") Is DBNull.Value Then
                    _element_info = row("element_info")
                Else
                    _element_info = ""
                End If
                If Not row("revised_by") Is DBNull.Value Then
                    _revised_by = row("revised_by")
                End If
                If Not row("entered_by") Is DBNull.Value Then
                    _entered_by = row("entered_by")
                End If
                If Not row("revised_dt") Is DBNull.Value Then
                    _revised_dt = row("revised_dt")
                End If
                If Not row("entered_dt") Is DBNull.Value Then
                    _entered_dt = row("entered_dt")
                End If
            Next

            cnx.Close()
        End Using
    End Sub

#Region "Class Properties"
    ''' <summary>
    ''' Gets the elem_site_id, primary key for the Elem_Site_Element table 
    ''' </summary>
    Public ReadOnly Property ID() As Integer
        Get
            ID = _id
        End Get
    End Property
    ''' <summary>
    ''' Gets or sets the site_id from the Elem_Site_Element table
    ''' </summary>
    Public Property SiteID() As Integer
        Get
            SiteID = _site_id
        End Get
        Set(value As Integer)
            _site_id = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the element_id from the Elem_Site_Element table
    ''' </summary>
    Public Property ElementID() As Integer
        Get
            ElementID = _element_id
        End Get
        Set(value As Integer)
            _element_id = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the element_info from the Elem_Site_Element table
    ''' </summary>
    Public Property ElementInfo() As String
        Get
            ElementInfo = _element_info
        End Get
        Set(value As String)
            _element_info = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the revised_by from the Elem_Site_Element table
    ''' </summary>
    Public Property RevisedBy() As String
        Get
            RevisedBy = _revised_by
        End Get
        Set(value As String)
            _revised_by = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the revised_dt from the Elem_Site_Element table
    ''' </summary>
    Public Property RevisedDate() As DateTime
        Get
            RevisedDate = _revised_dt
        End Get
        Set(value As DateTime)
            _revised_dt = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the entered_by from the Elem_Site_Element table
    ''' </summary>
    Public Property EnteredBy() As String
        Get
            EnteredBy = _entered_by
        End Get
        Set(value As String)
            _entered_by = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the entered_dt from the Elem_Site_Element table
    ''' </summary>
    Public Property EnteredDate() As DateTime
        Get
            EnteredDate = _entered_dt
        End Get
        Set(value As DateTime)
            _entered_dt = value
        End Set
    End Property
#End Region

#Region "Class Methods"
    ''' <summary>
    ''' Backs up and then updates the element information for a single element and site
    ''' </summary>
    Public Function UpdateSiteElementInfo(ByVal userid As String) As String
        Dim errmessage As String = "success"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                'First confirm that there is element information to back up
                Dim temp_elem_info As String = ""
                Dim sql As String = "SELECT element_info FROM Elem_Site_Element WHERE Elem_Site_Element.site_id = " & SiteID & " AND Elem_Site_Element.element_id = " & ElementID
                Dim cmd0 As New SqlCommand(sql, cnx)
                Dim dt As New DataTable
                Dim da As New SqlDataAdapter(cmd0)
                da.Fill(dt)

                For Each row As DataRow In dt.Rows
                    temp_elem_info = row("element_info").ToString
                Next

                'If element_info is not blank, then back up before updating
                If temp_elem_info <> "" Then
                    sql = "INSERT INTO Elem_Site_Element_Backup" & _
                        " (site_id, element_id, element_info, revised_by, revised_dt, entered_by, entered_dt, backup_by, backup_dt)" & _
                        " SELECT site_id, element_id, element_info, revised_by, revised_dt, entered_by, entered_dt, '" & userid & "' as backup_by, GETDATE() as backup_dt" & _
                        " FROM Elem_Site_Element" & _
                        " WHERE Elem_Site_Element.site_id = " & SiteID & " AND Elem_Site_Element.element_id = " & ElementID
                    Dim cmd1 As New SqlCommand(sql, cnx)
                    cmd1.ExecuteNonQuery()
                End If

                'Update the element info
                sql = "UPDATE Elem_Site_Element" & _
                " SET element_info = '" & ElementInfo.ToString.Replace("'", "''") & "', revised_by = '" & userid & "', revised_dt = GETDATE()" & _
                " WHERE element_id = " & ElementID & " AND site_id = " & SiteID
                Dim cmd2 As New SqlCommand(sql, cnx)
                cmd2.ExecuteNonQuery()

                'Run the stored procedure that updates the max revised date in Elem_Report_Sum
                Dim cmd3 As New SqlCommand("SP_Report_Update_Site_LastEdited", cnx)
                cmd3.CommandType = Data.CommandType.StoredProcedure
                cmd3.Parameters.Add("@site_id", SqlDbType.Int).Value = SiteID
                cmd3.ExecuteNonQuery()
            Catch ex As Exception
                errmessage = ex.Message
            End Try

            cnx.Close()
        End Using

        Return errmessage
    End Function

    ''' <summary>
    ''' Adds a new element for a site to the table Elem_Site_Element
    ''' </summary>
    ''' <returns>Either a success message or error message</returns>
    Public Function AddNewElementToSite(ByVal userid As String) As String
        Dim errmessage As String = "success"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim sql1 As String = Nothing

            Try
                Dim temp_elem_info As String
                If ElementInfo = Nothing Then
                    temp_elem_info = ""
                Else
                    temp_elem_info = ElementInfo
                End If

                sql1 = "INSERT INTO Elem_Site_Element" & _
                    " (site_id, element_id, element_info, entered_by, entered_dt, revised_by, revised_dt)" & _
                    " VALUES (" & SiteID & ", " & ElementID & ", '" & temp_elem_info & "', '" & userid & "', GETDATE(), '" & userid & "', GETDATE())"
                Dim cmd As New SqlCommand(sql1, cnx)
                cmd.ExecuteNonQuery()

                'Run the stored procedure that updates the max revised date in Elem_Report_Sum
                Dim cmd1 As New SqlCommand("SP_Report_Update_Site_LastEdited", cnx)
                cmd1.CommandType = Data.CommandType.StoredProcedure
                cmd1.Parameters.Add("@site_id", SqlDbType.Int).Value = SiteID
                cmd1.ExecuteNonQuery()
            Catch ex As Exception
                errmessage = ex.Message
            End Try

            cnx.Close()
        End Using

        Return errmessage
    End Function

#End Region
End Class
