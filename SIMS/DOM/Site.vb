Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

''' <summary>
''' This is the class that should be used when working on site specific (SIMS) applications
''' </summary>
''' <remarks></remarks>
Public Class Site
    Private _id As Integer
    Private _no As String
    Private _name As String
    Private _full_nm As String
    Private _agency_cd As String
    Private _office_id As Integer
    Private _wsc_id As Integer
    Private _sims_site_tp As String
    Private _sha_site_id As Integer
    Private _emerg_service As Boolean
    Private _cell_service As Boolean
    Private _SHA_updated_by As String
    Private _SHA_updated_dt As Date
    Private _SHA_reviewed_by As String
    Private _SHA_reviewed_dt As Date
    Private _SHA_approved_by As String
    Private _SHA_approved_dt As Date
    Private _reviewer_comments As String
    Private _element_id_list As New ArrayList
    Private _manu_sum_id As Integer
    Private _manu_approve_id As Integer
    Private _sdesc_sum_id As Integer
    Private _sanal_sum_id As Integer

    ''' <summary>
    ''' Initializes the Site class and gets the properties based on the passed site ID
    ''' </summary>
    Public Sub New(ByVal site_id As Integer)
        _id = site_id

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As SqlCommand = New SqlCommand("SP_station_details_by_site_id", cnx)
            Dim da As SqlDataAdapter
            Dim dt As DataTable = New DataTable

            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = site_id
            cmd.Parameters.Add("@nwisweb_site_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@site_no", SqlDbType.NVarChar, 15).Value = "0"
            cmd.Parameters.Add("@agency_cd", SqlDbType.NVarChar, 5).Value = "0"

            da = New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                _no = Trim(row("site_no").ToString)
                _agency_cd = row("agency_cd").ToString
                _name = row("station_nm").ToString
                _full_nm = row("station_full_nm").ToString
                _office_id = row("office_id")
                _wsc_id = row("wsc_id")
                _sims_site_tp = row("sims_site_tp").ToString
            Next

            'Determine safety related information for the site
            Dim cmd1 As New SqlCommand("SP_Safety_Info", cnx)
            Dim da1 As SqlDataAdapter
            Dim dt1 As DataTable = New DataTable

            cmd1.CommandType = Data.CommandType.StoredProcedure
            cmd1.Parameters.Add("@site_jha_id", SqlDbType.Int).Value = 0
            cmd1.Parameters.Add("@site_id", SqlDbType.Int).Value = _id
            cmd1.Parameters.Add("@type", SqlDbType.Int).Value = 0
            cmd1.Parameters.Add("@what", SqlDbType.NVarChar, 10).Value = "desc"

            da1 = New SqlDataAdapter(cmd1)
            da1.Fill(dt1)

            _sha_site_id = Nothing
            _emerg_service = Nothing
            _cell_service = Nothing
            _SHA_updated_by = Nothing
            _SHA_updated_dt = "1/1/1900"
            _SHA_reviewed_by = Nothing
            _SHA_reviewed_dt = "1/1/1900"
            _SHA_approved_by = Nothing
            _SHA_approved_dt = "1/1/1900"
            _reviewer_comments = Nothing

            Dim updated_dt As Date = "1/1/1900"
            Dim reviewed_dt As Date = "1/1/1900"
            Dim approved_dt As Date = "1/1/1900"

            If dt1.Rows.Count > 0 Then
                For Each row As DataRow In dt1.Rows
                    _sha_site_id = row("sha_site_id")

                    If Not row("emerg_service") Is DBNull.Value Then
                        _emerg_service = row("emerg_service")
                    End If

                    If Not row("cell_service") Is DBNull.Value Then
                        _cell_service = row("cell_service")
                    End If

                    If (Not row("updated_dt") Is DBNull.Value) And (Not updated_dt = Nothing) Then
                        If row("updated_dt") > updated_dt Then
                            _SHA_updated_by = row("updated_by")
                            _SHA_updated_dt = row("updated_dt_short")
                            updated_dt = row("updated_dt")
                        End If
                    ElseIf (Not row("updated_dt") Is DBNull.Value) And (updated_dt = "1/1/1900") Then
                        _SHA_updated_by = row("updated_by")
                        _SHA_updated_dt = row("updated_dt_short")
                        updated_dt = row("updated_dt")
                    End If

                    If (Not row("reviewed_dt") Is DBNull.Value) And (Not reviewed_dt = Nothing) Then
                        If row("reviewed_dt") > reviewed_dt Then
                            _SHA_reviewed_by = row("reviewed_by")
                            _SHA_reviewed_dt = row("reviewed_dt_short")
                            reviewed_dt = row("reviewed_dt")
                        End If
                    ElseIf (Not row("reviewed_dt") Is DBNull.Value) And (reviewed_dt = "1/1/1900") Then
                        _SHA_reviewed_by = row("reviewed_by")
                        _SHA_reviewed_dt = row("reviewed_dt_short")
                        reviewed_dt = row("reviewed_dt")
                    End If

                    If (Not row("reviewer_comments") Is DBNull.Value) Then
                        _reviewer_comments = row("reviewer_comments")
                    End If

                    If (Not row("approved_dt") Is DBNull.Value) And (Not approved_dt = Nothing) Then
                        If row("approved_dt") > approved_dt Then
                            _SHA_approved_by = row("approved_by")
                            _SHA_approved_dt = row("approved_dt_short")
                            approved_dt = row("approved_dt")
                        End If
                    ElseIf (Not row("approved_dt") Is DBNull.Value) And (approved_dt = "1/1/1900") Then
                        _SHA_approved_by = row("approved_by")
                        _SHA_approved_dt = row("approved_dt_short")
                        approved_dt = row("approved_dt")
                    End If
                Next
            End If

            Try
                _element_id_list = GetElementsForSite(_no)
            Catch ex As Exception
                _element_id_list = Nothing
            End Try

            cnx.Close()
        End Using
    End Sub

    ''' <summary>
    ''' Initializes the Site class and gets the properties based on passed site number and agency code
    ''' </summary>
    Public Sub New(ByVal site_no As String, ByVal agency_cd As String)
        _no = site_no
        _agency_cd = agency_cd

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As SqlCommand = New SqlCommand("SP_station_details_by_site_id", cnx)
            Dim da As SqlDataAdapter
            Dim dt As DataTable = New DataTable

            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@nwisweb_site_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@site_no", SqlDbType.NVarChar, 15).Value = site_no
            cmd.Parameters.Add("@agency_cd", SqlDbType.NVarChar, 5).Value = agency_cd

            da = New SqlDataAdapter(cmd)
            da.Fill(dt)

            If dt.Rows.Count = 0 Then
                _id = 0
                _name = Nothing
                _full_nm = Nothing
                _office_id = 0
                _wsc_id = 0
                _sims_site_tp = Nothing
            Else
                For Each row As DataRow In dt.Rows
                    _id = row("site_id")
                    _name = row("station_nm").ToString
                    _full_nm = row("station_full_nm").ToString
                    _office_id = row("office_id")
                    _wsc_id = row("wsc_id")
                    _sims_site_tp = row("sims_site_tp").ToString
                Next

                'Determine safety related information for the site
                Dim cmd1 As New SqlCommand("SP_Safety_Info", cnx)
                Dim da1 As SqlDataAdapter
                Dim dt1 As DataTable = New DataTable

                cmd1.CommandType = Data.CommandType.StoredProcedure
                cmd1.Parameters.Add("@site_jha_id", SqlDbType.Int).Value = 0
                cmd1.Parameters.Add("@site_id", SqlDbType.Int).Value = _id
                cmd1.Parameters.Add("@type", SqlDbType.Int).Value = 0
                cmd1.Parameters.Add("@what", SqlDbType.NVarChar, 10).Value = "desc"

                da1 = New SqlDataAdapter(cmd1)
                da1.Fill(dt1)

                _sha_site_id = Nothing
                _emerg_service = Nothing
                _cell_service = Nothing
                _SHA_updated_by = Nothing
                _SHA_updated_dt = "1/1/1900"
                _SHA_reviewed_by = Nothing
                _SHA_reviewed_dt = "1/1/1900"
                _SHA_approved_by = Nothing
                _SHA_approved_dt = "1/1/1900"
                _reviewer_comments = Nothing

                Dim updated_dt As Date = "1/1/1900"
                Dim reviewed_dt As Date = "1/1/1900"
                Dim approved_dt As Date = "1/1/1900"

                If dt1.Rows.Count > 0 Then
                    For Each row As DataRow In dt1.Rows
                        _sha_site_id = row("sha_site_id")

                        If Not row("emerg_service") Is DBNull.Value Then
                            _emerg_service = row("emerg_service")
                        End If

                        If Not row("cell_service") Is DBNull.Value Then
                            _cell_service = row("cell_service")
                        End If

                        If (Not row("updated_dt") Is DBNull.Value) And (Not updated_dt = Nothing) Then
                            If row("updated_dt") > updated_dt Then
                                _SHA_updated_by = row("updated_by")
                                _SHA_updated_dt = row("updated_dt_short")
                                updated_dt = row("updated_dt")
                            End If
                        ElseIf (Not row("updated_dt") Is DBNull.Value) And (updated_dt = "1/1/1900") Then
                            _SHA_updated_by = row("updated_by")
                            _SHA_updated_dt = row("updated_dt_short")
                            updated_dt = row("updated_dt")
                        End If

                        If (Not row("reviewed_dt") Is DBNull.Value) And (Not reviewed_dt = Nothing) Then
                            If row("reviewed_dt") > reviewed_dt Then
                                _SHA_reviewed_by = row("reviewed_by")
                                _SHA_reviewed_dt = row("reviewed_dt_short")
                                reviewed_dt = row("reviewed_dt")
                            End If
                        ElseIf (Not row("reviewed_dt") Is DBNull.Value) And (reviewed_dt = "1/1/1900") Then
                            _SHA_reviewed_by = row("reviewed_by")
                            _SHA_reviewed_dt = row("reviewed_dt_short")
                            reviewed_dt = row("reviewed_dt")
                        End If

                        If (Not row("reviewer_comments") Is DBNull.Value) Then
                            _reviewer_comments = row("reviewer_comments")
                        End If

                        If (Not row("approved_dt") Is DBNull.Value) And (Not approved_dt = Nothing) Then
                            If row("approved_dt") > approved_dt Then
                                _SHA_approved_by = row("approved_by")
                                _SHA_approved_dt = row("approved_dt_short")
                                approved_dt = row("approved_dt")
                            End If
                        ElseIf (Not row("approved_dt") Is DBNull.Value) And (approved_dt = "1/1/1900") Then
                            _SHA_approved_by = row("approved_by")
                            _SHA_approved_dt = row("approved_dt_short")
                            approved_dt = row("approved_dt")
                        End If
                    Next
                End If

                Try
                    _element_id_list = GetElementsForSite(_no)
                Catch ex As Exception
                    _element_id_list = Nothing
                End Try
            End If

            cnx.Close()
        End Using
    End Sub

#Region "Class Properties"
    ''' <summary>
    ''' Gets or sets the site ID
    ''' </summary>
    Public Property ID() As Integer
        Get
            ID = _id
        End Get
        Set(ByVal Value As Integer)
            _id = Value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the site number
    ''' </summary>
    Public Property Number() As String
        Get
            Number = _no
        End Get
        Set(ByVal Value As String)
            _no = Value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the site's assigned office ID
    ''' </summary>
    Public Property OfficeID() As Integer
        Get
            OfficeID = _office_id
        End Get
        Set(ByVal value As Integer)
            _office_id = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the site's assigned district code
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
    ''' Gets or sets the agency code
    ''' </summary>
    Public Property AgencyCode() As String
        Get
            AgencyCode = _agency_cd
        End Get
        Set(ByVal Value As String)
            _agency_cd = Value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the station name; same as the NWIS SITEFILE station_nm
    ''' </summary>
    Public Property Name() As String
        Get
            Name = _name
        End Get
        Set(ByVal Value As String)
            _name = Value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the station full name used for publication purposes
    ''' </summary>
    Public Property FullName() As String
        Get
            FullName = _full_nm
        End Get
        Set(ByVal Value As String)
            _full_nm = Value
        End Set
    End Property
    ''' <summary>
    ''' Gets the SIMS site type for the site (either SW or GW)
    ''' </summary>
    Public ReadOnly Property SIMSSiteType() As String
        Get
            SIMSSiteType = _sims_site_tp
        End Get
    End Property
    ''' <summary>
    ''' Gets or sets the Site Hazard Analysis site ID (sha_site_id)
    ''' </summary>
    Public Property SHASiteID() As Integer
        Get
            SHASiteID = _sha_site_id
        End Get
        Set(value As Integer)
            _sha_site_id = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the emergency service to true or false depending on if it's available at the site
    ''' </summary>
    Public Property EmergencyService() As Boolean
        Get
            EmergencyService = _emerg_service
        End Get
        Set(ByVal value As Boolean)
            _emerg_service = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the cell service to true or false depending on if it's available at the site
    ''' </summary>
    Public Property CellService() As Boolean
        Get
            CellService = _cell_service
        End Get
        Set(value As Boolean)
            _cell_service = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the user id of the last person to update a SHA at the site
    ''' </summary>
    Public Property SHAUpdatedBy() As String
        Get
            SHAUpdatedBy = _SHA_updated_by
        End Get
        Set(ByVal Value As String)
            _SHA_updated_by = Value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the date of when a SHA at the site was last updated
    ''' </summary>
    Public Property SHAUpdatedDate() As Date
        Get
            SHAUpdatedDate = _SHA_updated_dt
        End Get
        Set(ByVal Value As Date)
            _SHA_updated_dt = Value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the user id of the last person to review a SHA at the site
    ''' </summary>
    Public Property SHAReviewedBy() As String
        Get
            SHAReviewedBy = _SHA_reviewed_by
        End Get
        Set(ByVal Value As String)
            _SHA_reviewed_by = Value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the date of when a SHA at the site was last reviewed
    ''' </summary>
    Public Property SHAReviewedDate() As Date
        Get
            SHAReviewedDate = _SHA_reviewed_dt
        End Get
        Set(ByVal Value As Date)
            _SHA_reviewed_dt = Value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the user id of the last person to approve a SHA at the site
    ''' </summary>
    Public Property SHAApprovedBy() As String
        Get
            SHAApprovedBy = _SHA_approved_by
        End Get
        Set(ByVal Value As String)
            _SHA_approved_by = Value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the date of when a SHA at the site was last approved
    ''' </summary>
    Public Property SHAApprovedDate() As Date
        Get
            SHAApprovedDate = _SHA_approved_dt
        End Get
        Set(ByVal Value As Date)
            _SHA_approved_dt = Value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the comments made by the reviewer at time of reviewing
    ''' </summary>
    Public Property ReviewerComments() As String
        Get
            ReviewerComments = _reviewer_comments
        End Get
        Set(value As String)
            _reviewer_comments = value
        End Set
    End Property
    ''' <summary>
    ''' Gets the combined site number and station name for use on form pages
    ''' </summary>
    Public ReadOnly Property NumberName() As String
        Get
            NumberName = _no & " " & _name
        End Get
    End Property
    ''' <summary>
    ''' Gets or sets an array list containing the element IDs assigned to the site
    ''' </summary>
    Public Property ElementIDs() As ArrayList
        Get
            ElementIDs = _element_id_list
        End Get
        Set(ByVal value As ArrayList)
            _element_id_list = value
        End Set
    End Property
    ''' <summary>
    ''' Gets the MANU specific report_sum_id value from Elem_Report_Sum for the site
    ''' </summary>
    Public Property MANUSummaryID() As Integer
        Get
            MANUSummaryID = _manu_sum_id
        End Get
        Set(value As Integer)
            _manu_sum_id = value
        End Set
    End Property
    ''' <summary>
    ''' Gets the MANU specific report_approve_id value from Elem_Report_Approve for the site
    ''' </summary>
    Public Property MANUApproveID() As Integer
        Get
            MANUApproveID = _manu_approve_id
        End Get
        Set(value As Integer)
            _manu_approve_id = value
        End Set
    End Property
    ''' <summary>
    ''' Gets the SDESC specific report_sum_id value from Elem_Report_Sum for the site
    ''' </summary>
    Public Property SDESCSummaryID() As Integer
        Get
            SDESCSummaryID = _sdesc_sum_id
        End Get
        Set(value As Integer)
            _sdesc_sum_id = value
        End Set
    End Property
    ''' <summary>
    ''' Gets the SANAL specific report_sum_id value from Elem_Report_Sum for the site
    ''' </summary>
    Public Property SANALSummaryID() As Integer
        Get
            SANALSummaryID = _sanal_sum_id
        End Get
        Set(value As Integer)
            _sanal_sum_id = value
        End Set
    End Property
#End Region

#Region "Class Methods"
    ''' <summary>
    ''' Returns all the field trips assigned to this site
    ''' </summary>
    Public Overloads Function GetFieldTrips(ByVal site_id As Integer) As ArrayList
        Dim fieldtrips As New ArrayList

        fieldtrips = GetFieldTrips(site_id, "fieldtrips")

        Return fieldtrips
    End Function

    ''' <summary>
    ''' Returns all the operators assigned to this site
    ''' </summary>
    Public Function GetSiteOperators(ByVal site_id As Integer) As ArrayList
        Dim operators As New ArrayList

        operators = GetFieldTrips(site_id, "operators")

        Return operators
    End Function

    ''' <summary>
    ''' Returns either all field trips assigned to this site, or all userids of assigned field trips for this site, or all operators assigned to this site
    ''' </summary>
    ''' <param name="info_type">User "fieldtrips" for the full field trip info, and "userids" for just the userids, and "operators" for just the operators</param>
    ''' <remarks>Was created to not repeat code when it was necessary to get the userids associated
    ''' with the assigned field trips for use in the NWISOpsRequest email form.</remarks>
    Public Overloads Function GetFieldTrips(ByVal site_id As Integer, ByVal info_type As String) As ArrayList
        Dim fieldtripinfo As New ArrayList
        Dim trip_nm As String = ""

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SP_Stations_Admin", cnx)
            Dim dt As New DataTable
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@office_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@rms_record_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = site_id

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                Select Case info_type
                    Case "operators"
                        fieldtripinfo.Add(row("operator_va").ToString)
                    Case Else
                        If row("trip_nm").ToString <> "" And row("trip_nm").ToString <> trip_nm Then
                            trip_nm = row("trip_nm").ToString

                            If info_type = "userids" Then
                                fieldtripinfo.Add(row("user_id").ToString)
                            Else
                                fieldtripinfo.Add(trip_nm & " - " & row("user_id").ToString)
                            End If
                        End If
                End Select
            Next
            cnx.Close()
        End Using

        Return fieldtripinfo
    End Function

    ''' <summary>
    ''' Returns all the ADAPS data descriptors tied to this site
    ''' </summary>
    ''' <remarks>Information comes from nwisweb table Eval_CRP_Status</remarks>
    Public Function GetAllDDs(ByVal site_id As Integer) As ArrayList
        Dim dds As New ArrayList

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SP_RMS_Get_Site_DDs", cnx)
            Dim dt As New DataTable
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = site_id
            cmd.Parameters.Add("@options", SqlDbType.NVarChar, 20).Value = "allforsite"

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                dds.Add(row("dd_nu").ToString & " - " & row("parm_cd").ToString)
            Next

            cnx.Close()
        End Using

        Return dds
    End Function

    ''' <summary>
    ''' Returns the ADAPS data descriptors tied to this site that have not been tied to a record yet
    ''' </summary>
    Public Function GetUnusedDDs(ByVal site_id As Integer) As DataTable
        Dim dds As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SP_RMS_Get_Site_DDs", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = site_id
            cmd.Parameters.Add("@options", SqlDbType.NVarChar, 20).Value = "unusedforsite"

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dds)

            cnx.Close()
        End Using

        Return dds
    End Function

    ''' <summary>
    ''' Returns all the records tied to this site
    ''' </summary>
    ''' <param name="rms_record_id">If an rms_record_id is passed, then do not include its record details</param>
    ''' <returns>Returns a DataTable</returns>
    Public Function GetRecordDetails(ByVal site_id As Integer, ByVal rms_record_id As Integer) As DataTable
        Dim recordDetails As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SP_Record_Details_by_site_id", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = site_id
            cmd.Parameters.Add("@rms_record_id", SqlDbType.Int).Value = rms_record_id

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(recordDetails)

            cnx.Close()
        End Using

        Return recordDetails
    End Function

    ''' <summary>
    ''' Similar to GetRecordDetails, but only returns rms_record_ids as an ArrayList
    ''' </summary>
    Public Function GetRecordIDs(ByVal site_id As Integer) As ArrayList
        Dim record_ids As New ArrayList

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SP_rms_record_ids_by_site_id", cnx)
            Dim dt As New DataTable
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = site_id

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                record_ids.Add(row("rms_record_id"))
            Next

            cnx.Close()
        End Using

        Return record_ids
    End Function

    ''' <summary>
    ''' Returns a list of records for the site to be used in the work/check/review lists
    ''' </summary>
    ''' <param name="site_id">The site ID of the site</param>
    ''' <param name="listtype">The list to be retrieved; accepted are work, check, or review</param>
    ''' <returns>A DataTable to be bound to a control</returns>
    Public Function GetRecordsToBeProcessed(ByVal site_id As Integer, ByVal listtype As String) As DataTable
        Dim records As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand("SP_Records_To_Be_Processed", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = site_id
            cmd.Parameters.Add("@listtype", SqlDbType.NVarChar, 6).Value = listtype

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(records)

            cnx.Close()
        End Using

        Return records
    End Function

    ''' <summary>
    ''' Gets all the hospitals that have been assigned to a site
    ''' </summary>
    ''' <returns>Hospitals DataTable</returns>
    Public Function GetSiteHospitals(ByVal site_id As Integer) As DataTable
        Dim hospitals As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SP_Hospitals", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@hospital_nm", SqlDbType.NVarChar, 100).Value = "0"
            cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = site_id
            cmd.Parameters.Add("@hospital_id", SqlDbType.Int).Value = 0

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(hospitals)

            cnx.Close()
        End Using

        Return hospitals
    End Function

    ''' <summary>
    ''' Gets all the contacts that have been assigned to a site
    ''' </summary>
    ''' <returns>Contacts DataTable</returns>
    Public Function GetSiteContacts(ByVal site_id As Integer) As DataTable
        Dim contacts As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SP_Contacts", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@contact_nm", SqlDbType.NVarChar, 100).Value = "0"
            cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = site_id
            cmd.Parameters.Add("@contact_id", SqlDbType.Int).Value = 0

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(contacts)

            cnx.Close()
        End Using

        Return contacts
    End Function

    ''' <summary>
    ''' Gets the measurement elements for the site to be used in the site safety summary
    ''' </summary>
    ''' <param name="element_id">Use 13 for DISCHARGE MEASUREMENT, 57 for GROUNDWATER MEASUREMENT, 124 for WATER QUALITY MEASUREMENT, 1002 for LAKE MEASUREMENTS, 1003 for ECOLOGICAL MEASUREMENTS, 1004 for ATMOSPHERIC MEASUREMENTS (check web.config app settings for element_ids), and 0 for all six</param>
    Public Function GetMeasurementElements(ByVal site_id As Integer, ByVal element_id As Integer) As DataTable
        Dim elements As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SP_Element_Info_by_element_and_site_id", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@element_id", SqlDbType.Int).Value = element_id
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = site_id

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(elements)

            cnx.Close()

            'info = Replace(row("element_info"), vbCrLf & vbCrLf, "<br /><br />" & vbCrLf)

            cnx.Close()
        End Using

        Return elements
    End Function

    ''' <summary>
    ''' Gets an array list of the SHAs that have been assigned to the site
    ''' </summary>
    ''' <returns>SHAs ArrayList</returns>
    Public Function GetSiteJHAs(ByVal site_id As Integer) As ArrayList
        Dim JHAs As New ArrayList

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SP_Safety_Info", cnx)
            Dim dt As New DataTable
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@site_jha_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = site_id
            cmd.Parameters.Add("@type", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@what", SqlDbType.NVarChar, 10).Value = "desc"

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                JHAs.Add(row("site_jha_id"))
            Next

            cnx.Close()
        End Using

        Return JHAs
    End Function

    ''' <summary>
    ''' Gets the site_jha_id based on the site_id, element_id, and jha_id
    ''' </summary>
    ''' <param name="element_id">Examples: 13 for discharge meas, 57 for water level meas, etc.</param>
    ''' <param name="jha_id">Examples: 2 for wading meas, 3 for bridge meas, 4 for cableway meas, etc.</param>
    Public Function GetSiteJHAID(ByVal site_id As Integer, ByVal element_id As Integer, ByVal jha_id As Integer) As Integer
        Dim site_jha_id As Integer = Nothing

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SELECT shsj.site_jha_id FROM SHA_Lut_ElemSHA AS shle INNER JOIN" & _
                " SHA_Site_SHA AS shsj ON shle.elem_jha_id = shsj.elem_jha_id INNER JOIN" & _
                " SHA_Site_Master AS shsm ON shsj.sha_site_id = shsm.sha_site_id" & _
                " WHERE (shsm.site_id = " & site_id & ") AND (shle.element_id = " & element_id & ") AND (shle.jha_id = " & jha_id & ")", cnx)
            Dim dt As New DataTable

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                site_jha_id = row("site_jha_id")
            Next

            cnx.Close()
        End Using

        Return site_jha_id
    End Function

    ''' <summary>
    ''' Gets the site specific conditions for a site by SHA
    ''' </summary>
    ''' <param name="site_jha_id">An integer representing the site SHA ID</param>
    ''' <returns>SSC ArrayList</returns>
    Public Function GetSiteSpecificConditions(ByVal site_jha_id As Integer) As ArrayList
        Dim SSC As New ArrayList

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SP_Safety_Info", cnx)
            Dim dt As New DataTable
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@site_jha_id", SqlDbType.Int).Value = site_jha_id
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@type", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@what", SqlDbType.NVarChar, 10).Value = "ssc"

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                SSC.Add(row("remarks"))
            Next

            cnx.Close()
        End Using

        Return SSC
    End Function

    ''' <summary>
    ''' Gets the servicing site specific conditions for a site
    ''' </summary>
    ''' <remarks>This pulls from the table SHA_Site_Servicing</remarks>
    Public Function GetServicingSiteSpecificConditions(ByVal site_id As Integer) As DataTable
        Dim SSSC As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SP_Safety_Info", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@site_jha_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = site_id
            cmd.Parameters.Add("@type", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@what", SqlDbType.NVarChar, 10).Value = "sssc"

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(SSSC)

            cnx.Close()
        End Using

        Return SSSC
    End Function

    ''' <summary>
    ''' Gets the all the recommended safety equipment for a site
    ''' </summary>
    ''' <returns>equip DataTable</returns>
    Public Function GetRecommendedEquipment(ByVal site_id As Integer) As DataTable
        Dim equip As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SP_Safety_Info", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@site_jha_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = site_id
            cmd.Parameters.Add("@type", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@what", SqlDbType.NVarChar, 10).Value = "equip"

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(equip)

            cnx.Close()
        End Using

        Return equip
    End Function

    ''' <summary>
    ''' Updates the updated_by and updated_dt fields of the SHA_Site_Master table - to be called after any change to a SHA or JHA
    ''' </summary>
    Public Sub UpdateSHA(ByVal userid As String)
        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim sql As String = "UPDATE SHA_Site_Master" & _
                " SET updated_by = '" & userid & "', updated_dt = '" & Now().ToString() & "'" & _
                " WHERE sha_site_id = " & SHASiteID
                Dim cmd As New SqlCommand(sql, cnx)
                cmd.ExecuteNonQuery()
            Catch ex As Exception
            End Try

            cnx.Close()
        End Using
    End Sub

    ''' <summary>
    ''' Updates the reviewed_by, reviewed_dt, and reviewer_comments fields of the SHA_Site_Master table
    ''' </summary>
    Public Sub ReviewSHA(ByVal userid As String, ByVal comments As String)
        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim sql As String = "UPDATE SHA_Site_Master" & _
                    " SET reviewed_by = '" & userid & "', reviewed_dt = '" & Now().ToString() & "', reviewer_comments = '" & comments & "'" & _
                    " WHERE sha_site_id = " & SHASiteID
                Dim cmd As New SqlCommand(sql, cnx)
                cmd.ExecuteNonQuery()
            Catch ex As Exception
            End Try

            cnx.Close()
        End Using
    End Sub

    ''' <summary>
    ''' Updates the approved_by and approved_dt fields of the SHA_Site_Master table
    ''' </summary>
    Public Sub ApproveSHA(ByVal userid As String)
        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Try
                Dim sql As String = "UPDATE SHA_Site_Master" & _
                    " SET approved_by = '" & userid & "', approved_dt = '" & Now().ToString() & "'" & _
                    " WHERE sha_site_id = " & SHASiteID
                Dim cmd As New SqlCommand(sql, cnx)
                cmd.ExecuteNonQuery()
            Catch ex As Exception
            End Try

            cnx.Close()
        End Using
    End Sub

    ''' <summary>
    ''' To be used by the site selection drop-down search lists
    ''' </summary>
    ''' <param name="text_va">Some part of a site number or name</param>
    ''' <returns>site_no, station_nm, site_no_nm</returns>
    Public Function GetSiteNameFromSearch(text_va As String, wsc_id As Integer) As DataTable
        Dim dt As New DataTable()

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim sql As String

            If text_va = "" Then
                sql = "SELECT DISTINCT  ssm.agency_cd, ssm.site_no, ssm.site_id, ssm.station_full_nm, (ssm.site_no + ' ' + ssm.station_full_nm) as site_no_nm FROM SIMS_Site_Master AS ssm INNER JOIN lut_Office AS lo ON ssm.office_id = lo.office_id WHERE lo.wsc_id = " & wsc_id & " ORDER BY ssm.site_no"
            ElseIf text_va = "swonly" Then
                sql = "SELECT DISTINCT ssm.agency_cd, ssm.site_no, ssm.site_id, ssm.station_full_nm, ssm.site_no + ' ' + s.station_nm AS site_no_nm" & _
                    " FROM SIMS_Site_Master AS ssm INNER JOIN lut_Office AS lo ON ssm.office_id = lo.office_id INNER JOIN" & _
                    " nwisweb.dbo.SITEFILE AS s ON ssm.nwisweb_site_id = s.site_id INNER JOIN lut_site_tp AS lst ON s.site_tp_cd = lst.site_tp_cd" & _
                    " WHERE (lo.wsc_id = " & wsc_id & ") AND (lst.sims_site_tp = 'sw')" & _
                    " ORDER BY ssm.site_no"
            ElseIf text_va = "forMAI" Then
                sql = "SELECT DISTINCT ssm.agency_cd, ssm.site_no, ssm.site_id, ssm.station_full_nm, (ssm.site_no + ' ' + ssm.station_full_nm) as site_no_nm" & _
                    " FROM Elem_Report_Sum AS ers INNER JOIN SIMS_Site_Master AS ssm ON ers.site_id = ssm.site_id INNER JOIN Elem_Report_Approve AS era ON ssm.site_id = era.site_id" & _
                    " INNER JOIN nwisweb.dbo.SITEFILE AS s ON ssm.nwisweb_site_id = s.site_id INNER JOIN lut_Office AS lo ON ssm.office_id = lo.office_id INNER JOIN" & _
                    " RMS_Record_Master AS rrm ON ssm.site_id = rrm.site_id INNER JOIN RMS_Record_Types AS rrt ON rrm.record_type_id = rrt.record_type_id INNER JOIN" & _
                    " RMS_Record_DD AS rrd ON rrm.rms_record_id = rrd.rms_record_id" & _
                    " WHERE (ers.report_type_cd = 'MANU') AND lo.wsc_id = " & wsc_id
            Else
                sql = "SELECT DISTINCT ssm.agency_cd, ssm.site_no, ssm.site_id, ssm.station_full_nm, (ssm.site_no + ' ' + ssm.station_full_nm) as site_no_nm FROM SIMS_Site_Master AS ssm INNER JOIN lut_Office AS lo ON ssm.office_id = lo.office_id WHERE lo.wsc_id = " & wsc_id & " AND (ssm.site_no LIKE '" & text_va & "%')"
            End If

            Dim cmd As New SqlCommand(sql, cnx)
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            cnx.Close()
        End Using

        Return dt
    End Function

    ''' <summary>
    ''' Gets a list of element_ids for a site and returns an ArrayList
    ''' </summary>
    Public Function GetElementsForSite(site_no As String, Optional agency_cd As String = "USGS") As ArrayList
        Dim elements As New ArrayList

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim dt As New DataTable
            Dim cmd As New SqlCommand("SP_Element_Info_by_site_id_and_type", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@doc_type", SqlDbType.NVarChar).Value = DBNull.Value
            cmd.Parameters.Add("@site_no", SqlDbType.NVarChar).Value = site_no
            cmd.Parameters.Add("@agency_cd", SqlDbType.NVarChar).Value = agency_cd

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                elements.Add(row("element_id"))
            Next

            cnx.Close()
        End Using

        Return elements
    End Function

    ''' <summary>
    ''' Sets up the ID properties for the MANU approval interface
    ''' </summary>
    Public Sub SetElemReportIDs(site_id As Integer)
        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim dt As New DataTable
            Dim cmd As New SqlCommand("SP_Elem_Report_ID", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@report_type", SqlDbType.NVarChar).Value = "MANU"
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = site_id
            cmd.Parameters.Add("@id_type", SqlDbType.NVarChar).Value = "Approve"
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = 0

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                _manu_approve_id = row("report_approve_id")
            Next

            Dim dt1 As New DataTable
            Dim cmd1 As New SqlCommand("SP_Elem_Report_ID", cnx)
            cmd1.CommandType = Data.CommandType.StoredProcedure
            cmd1.Parameters.Add("@report_type", SqlDbType.NVarChar).Value = "MANU"
            cmd1.Parameters.Add("@site_id", SqlDbType.Int).Value = site_id
            cmd1.Parameters.Add("@id_type", SqlDbType.NVarChar).Value = "Sum"
            cmd1.Parameters.Add("@id", SqlDbType.Int).Value = 0

            Dim da1 As New SqlDataAdapter(cmd1)
            da1.Fill(dt1)

            For Each row As DataRow In dt1.Rows
                _manu_sum_id = row("report_sum_id")
            Next

            Dim dt2 As New DataTable
            Dim cmd2 As New SqlCommand("SP_Elem_Report_ID", cnx)
            cmd2.CommandType = Data.CommandType.StoredProcedure
            cmd2.Parameters.Add("@report_type", SqlDbType.NVarChar).Value = "SDESC"
            cmd2.Parameters.Add("@site_id", SqlDbType.Int).Value = site_id
            cmd2.Parameters.Add("@id_type", SqlDbType.NVarChar).Value = "Sum"
            cmd2.Parameters.Add("@id", SqlDbType.Int).Value = 0

            Dim da2 As New SqlDataAdapter(cmd2)
            da2.Fill(dt2)

            For Each row As DataRow In dt2.Rows
                _sdesc_sum_id = row("report_sum_id")
            Next

            Dim dt3 As New DataTable
            Dim cmd3 As New SqlCommand("SP_Elem_Report_ID", cnx)
            cmd3.CommandType = Data.CommandType.StoredProcedure
            cmd3.Parameters.Add("@report_type", SqlDbType.NVarChar).Value = "SANAL"
            cmd3.Parameters.Add("@site_id", SqlDbType.Int).Value = site_id
            cmd3.Parameters.Add("@id_type", SqlDbType.NVarChar).Value = "Sum"
            cmd3.Parameters.Add("@id", SqlDbType.Int).Value = 0

            Dim da3 As New SqlDataAdapter(cmd3)
            da3.Fill(dt3)

            For Each row As DataRow In dt3.Rows
                _sanal_sum_id = row("report_sum_id")
            Next

            cnx.Close()
        End Using
    End Sub

    ''' <summary>
    ''' Gets the NWISWeb site id and station name from the Sitefile using a site number and agency code
    ''' </summary>
    Public Function GetNWISWEBSiteID(ByVal site_no As String, ByVal agency_cd As String) As ArrayList
        Dim nwisweb_site_info As New ArrayList

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim dt As New DataTable
            Dim cmd As New SqlCommand("SP_station_details_from_SITEFILE", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@site_no", SqlDbType.NVarChar).Value = site_no
            cmd.Parameters.Add("@agency_cd", SqlDbType.NVarChar).Value = agency_cd
            cmd.Parameters.Add("@nwisweb_site_id", SqlDbType.Int).Value = 0

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                nwisweb_site_info.Add(row("site_id").ToString())
                nwisweb_site_info.Add(row("station_nm").ToString())
            Next

            cnx.Close()
        End Using

        Return nwisweb_site_info
    End Function

    Public Function AddSiteToSIMS(ByVal nwisweb_site_id As String, ByVal station_full_nm As String, ByVal office_id As String) As String
        Dim confirmAdd As String = "error"

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim new_site_id As Integer = 0
            Dim site_no As String = Nothing
            Dim agency_cd As String = Nothing
            Dim db_no As String = Nothing
            Dim nwis_host As String = Nothing

            Try
                'First get information from the SITEFILE for adding to SIMS_Site_Master
                Dim dt As New DataTable
                Dim cmd As New SqlCommand("SP_Station_Details_by_site_id", cnx)
                cmd.CommandType = Data.CommandType.StoredProcedure
                cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = 0
                cmd.Parameters.Add("@nwisweb_site_id", SqlDbType.Int).Value = Convert.ToInt32(nwisweb_site_id)
                cmd.Parameters.Add("@site_no", SqlDbType.NVarChar).Value = "0"
                cmd.Parameters.Add("@agency_cd", SqlDbType.NVarChar).Value = "0"

                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)

                For Each row As DataRow In dt.Rows
                    site_no = row("site_no")
                    agency_cd = row("agency_cd")
                    db_no = row("db_no")
                    nwis_host = row("nwis_host")
                Next

                'Try adding the new site to SIMS_Site_Master
                Dim sql As String = "INSERT INTO SIMS_Site_Master (nwisweb_site_id, agency_cd, site_no, station_full_nm, office_id, db_no, nwis_host)" & _
                    " SELECT " & nwisweb_site_id & " AS nwisweb_site_id, '" & agency_cd & "' AS agency_cd, '" & site_no & "' AS site_no, '" & station_full_nm & "' As station_full_nm, " & office_id & " AS office_id," & _
                    " '" & db_no & "' AS db_no, '" & nwis_host & "' AS nwis_host"
                Dim cmd1a As New SqlCommand(sql, cnx)
                cmd1a.ExecuteNonQuery()

                'Get the just added site_id from SIMS_Site_Master
                sql = "SELECT site_id FROM SIMS_Site_Master WHERE nwisweb_site_id = " & nwisweb_site_id
                Dim cmd1b As New SqlCommand(sql, cnx)
                Dim dt1 As New DataTable
                Dim da1 As New SqlDataAdapter(cmd1b)
                da1.Fill(dt1)

                For Each row As DataRow In dt1.Rows
                    new_site_id = row("site_id")
                Next

                'Add the site to Elem_Site_Master
                sql = "INSERT INTO Elem_Site_Master ( site_id ) SELECT " & new_site_id.ToString() & " AS site_id"
                Dim cmd2 As New SqlCommand(sql, cnx)
                cmd2.ExecuteNonQuery()

                'Create a LOCATION element for the site
                sql = "INSERT INTO Elem_Site_Element ( site_id, element_id )" & _
                    " SELECT " & new_site_id.ToString() & " AS site_id, 28 AS element_id"
                Dim cmd3 As New SqlCommand(sql, cnx)
                cmd3.ExecuteNonQuery()

                'Setup the necessary information for the MANU Approval
                sql = "INSERT INTO Elem_Report_Approve ( site_id, report_type_cd, publish_complete )" & _
                    " VALUES ( " & new_site_id.ToString() & ", 'MANU', 'U' )"
                Dim cmd4 As New SqlCommand(sql, cnx)
                cmd4.ExecuteNonQuery()

                sql = "INSERT INTO Elem_Report_Sum ( site_id, report_type_cd, revised_dt )" & _
                    " VALUES ( " & new_site_id.ToString() & ", 'MANU', GETDATE() )"
                Dim cmd5 As New SqlCommand(sql, cnx)
                cmd5.ExecuteNonQuery()

                sql = "INSERT INTO Elem_Report_Sum ( site_id, report_type_cd, revised_dt )" & _
                    " VALUES ( " & new_site_id.ToString() & ", 'SDESC', GETDATE() )"
                Dim cmd6 As New SqlCommand(sql, cnx)
                cmd6.ExecuteNonQuery()

                sql = "INSERT INTO Elem_Report_Sum ( site_id, report_type_cd, revised_dt )" & _
                    " VALUES ( " & new_site_id.ToString() & ", 'SANAL', GETDATE() )"
                Dim cmd7 As New SqlCommand(sql, cnx)
                cmd7.ExecuteNonQuery()

                confirmAdd = new_site_id.ToString()
            Catch ex As Exception
                confirmAdd = "error"
            End Try

            cnx.Close()
        End Using

        Return confirmAdd
    End Function

#End Region
End Class
