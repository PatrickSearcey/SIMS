Imports System.Data.SqlClient
Imports System.Web.Services

Public Class ElementReport
    Private _approve_id As Integer
    Private _sum_id As Integer
    Private _site_id As Integer
    Private _report_type_cd As String
    Private _approved_by As String
    Private _approved_dt As DateTime
    Private _approver_comments As String
    Private _publish_complete As String
    Private _needs_approval As String
    Private _revised_dt As DateTime
    Private svcSIMS As SIMSService.SIMSServiceClient

    Public Sub New(ByVal sum_id As Integer)
        _sum_id = sum_id

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As SqlCommand = New SqlCommand("SP_Elem_Report_ID", cnx)
            Dim da As SqlDataAdapter
            Dim dt As DataTable = New DataTable

            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@report_type", SqlDbType.NVarChar).Value = "0"
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@id_type", SqlDbType.NVarChar).Value = "Sum"
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = sum_id

            da = New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                _site_id = row("site_id")
                _report_type_cd = row("report_type_cd").ToString()
                If row("revised_dt") Is DBNull.Value Then
                    _revised_dt = Nothing
                Else
                    _revised_dt = row("revised_dt")
                End If
            Next

            Try
                Dim cmd1 As SqlCommand = New SqlCommand("SP_Elem_Report_ID", cnx)
                Dim da1 As SqlDataAdapter
                Dim dt1 As DataTable = New DataTable

                cmd1.CommandType = Data.CommandType.StoredProcedure
                cmd1.Parameters.Add("@report_type", SqlDbType.NVarChar).Value = _report_type_cd
                cmd1.Parameters.Add("@site_id", SqlDbType.Int).Value = _site_id
                cmd1.Parameters.Add("@id_type", SqlDbType.NVarChar).Value = "Approve"
                cmd1.Parameters.Add("@id", SqlDbType.Int).Value = 0

                da1 = New SqlDataAdapter(cmd1)
                da1.Fill(dt1)

                For Each row As DataRow In dt1.Rows
                    _approve_id = row("report_approve_id")
                    _approved_by = row("approved_by").ToString()
                    If row("approved_dt") Is DBNull.Value Then
                        _approved_dt = Nothing
                    Else
                        _approved_dt = row("approved_dt")
                    End If
                    If row("approver_comments") Is DBNull.Value Then
                        _approver_comments = ""
                    Else
                        _approver_comments = row("approver_comments")
                    End If
                    _publish_complete = row("publish_complete")
                Next

                If Not _approved_dt = Nothing Then
                    If _revised_dt > _approved_dt Then
                        _needs_approval = "YES"
                    Else
                        _needs_approval = "NO"
                    End If
                Else
                    _needs_approval = "YES"
                End If
            Catch ex As Exception
            End Try

            cnx.Close()
        End Using
    End Sub

#Region "Properties"
    ''' <summary>
    ''' Gets the report_approve_id from Elem_Report_Approve
    ''' </summary>
    Public Property ReportApproveID() As Integer
        Get
            ReportApproveID = _approve_id
        End Get
        Set(value As Integer)
            _approve_id = value
        End Set
    End Property
    ''' <summary>
    ''' Gets the report_sum_id from Elem_Report_Sum
    ''' </summary>
    Public Property ReportSumID() As Integer
        Get
            ReportSumID = _sum_id
        End Get
        Set(value As Integer)
            _sum_id = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the site_id for the report
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
    ''' Gets or sets the report_type_cd for the report
    ''' </summary>
    Public Property ReportTypeCode() As String
        Get
            ReportTypeCode = _report_type_cd
        End Get
        Set(value As String)
            _report_type_cd = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the userid of the person who last approved the report
    ''' </summary>
    Public Property ApprovedBy() As String
        Get
            ApprovedBy = _approved_by
        End Get
        Set(value As String)
            _approved_by = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the date of the last approved for the report
    ''' </summary>
    Public Property ApprovedDate() As DateTime
        Get
            ApprovedDate = _approved_dt
        End Get
        Set(value As DateTime)
            _approved_dt = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the comments made by the approver during approval of the report
    ''' </summary>
    Public Property ApproverComments() As String
        Get
            ApproverComments = _approver_comments
        End Get
        Set(value As String)
            _approver_comments = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the publish_complete flag for the report
    ''' </summary>
    Public Property PublishComplete() As String
        Get
            PublishComplete = _publish_complete
        End Get
        Set(value As String)
            _publish_complete = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the needs approval value for the report
    ''' </summary>
    Public Property NeedsApproval() As String
        Get
            NeedsApproval = _needs_approval
        End Get
        Set(value As String)
            _needs_approval = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the date of the last revision of any element in the report
    ''' </summary>
    Public Property RevisedDate() As DateTime
        Get
            RevisedDate = _revised_dt
        End Get
        Set(value As DateTime)
            _revised_dt = value
        End Set
    End Property
#End Region

#Region "Methods"
    ''' <summary>
    ''' Gets a data table of element_id, element_nm, element_info, revised_by, revised_dt, remark, report_type_cd, priority, for a site by report type
    ''' </summary>
    ''' <param name="report_tp">MANU, SDESC, or SANAL</param>
    Public Function GetAllElementsByReport(ByVal site_no As String, ByVal agency_cd As String, ByVal report_tp As String) As DataTable
        Dim dt As New DataTable

        svcSIMS = New SIMSService.SIMSServiceClient()
        Dim lstElems() As SIMSService.Element

        Try
            lstElems = svcSIMS.GetElementsBySiteAndReport(site_no, agency_cd, report_tp)
        
            dt.Columns.Add("element_id")
            dt.Columns.Add("element_nm")
            dt.Columns.Add("element_info")
            dt.Columns.Add("revised_by")
            dt.Columns.Add("revised_dt")
            dt.Columns.Add("remark")
            dt.Columns.Add("report_type_cd")
            dt.Columns.Add("priority")

            For Each x In lstElems
                dt.Rows.Add(New Object() {x.ElementID, x.ElementName.ToString().Replace(" (MANU)", ""), x.ElementInfo, x.RevisedBy, x.RevisedDate, x.Remark, x.ReportTypeCd, x.Priority})
            Next

        Catch ex As Exception
        End Try

        Return dt
    End Function

    ''' <summary>
    ''' Updates the information in Elem_Report_Approve when someone approves a element report
    ''' </summary>
    Public Function ApproveReport(ByVal approved_by As String, ByVal comments As String) As Boolean
        Dim success As Boolean = False

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim cmd As New SqlCommand("SP_Report_Approve", cnx)
                cmd.CommandType = Data.CommandType.StoredProcedure
                cmd.Parameters.Add("@report_approve_id", SqlDbType.Int).Value = ReportApproveID
                cmd.Parameters.Add("@approved_by", SqlDbType.NVarChar).Value = approved_by
                cmd.Parameters.Add("@comments", SqlDbType.NVarChar).Value = comments
                cmd.ExecuteNonQuery()

                success = True
            Catch ex As Exception
                success = False
            End Try

            cnx.Close()
        End Using

        Return success
    End Function
#End Region

End Class
