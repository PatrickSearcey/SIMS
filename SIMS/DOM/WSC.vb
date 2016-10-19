Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

Public Class WSC
    Private _id As Integer
    Private _cd As String
    Private _name As String
    Private _nwisops_email As String
    Private _request_ID As Integer
    Private _region_cd As String
    Private _AD_OU As String
    Private _swr_url As String
    Private svcSIMS As SIMSService.SIMSServiceClient

    ''' <summary>
    ''' Initializes the WSC class and fills properties with general 
    ''' info about the WSC from SP_WSC_Info stored procedure
    ''' </summary>
    ''' <param name="wsc_id"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal wsc_id As Integer)
        _id = wsc_id

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand("SP_WSC_Info", cnx)
            Dim dt As New DataTable
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = _id
            cmd.Parameters.Add("@primaryOU", SqlDbType.NVarChar, 255).Value = "0"
            cmd.Parameters.Add("@region_cd", SqlDbType.NVarChar, 50).Value = "0"

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                _name = row("wsc_nm").ToString
                _nwisops_email = row("nwisops_email").ToString
                _request_ID = row("reqID")
                _cd = row("wsc_cd").ToString
                _region_cd = row("region_cd").ToString
                _AD_OU = row("AD_OU").ToString
                _swr_url = row("swr_url").ToString
            Next

            cnx.Close()
        End Using
    End Sub

#Region "Class Properties"
    ''' <summary>
    ''' Gets the ID for the  WSC
    ''' </summary>
    Public ReadOnly Property ID() As Integer
        Get
            ID = _id
        End Get
    End Property
    ''' <summary>
    ''' Gets or sets the code for the WSC - a spin on the district abbreviation
    ''' </summary>
    Public Property Code() As String
        Get
            Code = _cd
        End Get
        Set(value As String)
            _cd = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the address used for the NWIS Ops Help Request emails
    ''' </summary>
    Public Property NWISOpsEmail() As String
        Get
            NWISOpsEmail = _nwisops_email
        End Get
        Set(ByVal value As String)
            _nwisops_email = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the name of the WSC
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
    ''' Gets or sets the NWIS Ops Help request ID to be used in the subject line of emails
    ''' </summary>
    Public Property RequestID() As Integer
        Get
            RequestID = _request_ID
        End Get
        Set(ByVal value As Integer)
            _request_ID = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the region code for the WSC
    ''' </summary>
    Public Property RegionCode() As String
        Get
            RegionCode = _region_cd
        End Get
        Set(ByVal value As String)
            _region_cd = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the Active Directory OUs that can be associated with this WSC
    ''' </summary>
    Public Property AD_OU() As String
        Get
            AD_OU = _AD_OU
        End Get
        Set(value As String)
            _AD_OU = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the SWR link for the WSC
    ''' </summary>
    Public Property SWR_URL() As String
        Get
            SWR_URL = _swr_url
        End Get
        Set(value As String)
            _swr_url = value
        End Set
    End Property
#End Region

#Region "Class Methods"
    ''' <summary>
    ''' Returns a list of personnel that have been assigned to a WSC to be used in Worker/Checker/Reviewer 
    ''' drop-down lists
    ''' </summary>
    Public Function GetPersonnelList(ByVal wsc_id As Integer, ByVal manage As String) As ArrayList
        Dim personnelList As New ArrayList

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SP_personnel_by_WSC_office_or_user_id", cnx)
            Dim dt As New DataTable
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = wsc_id
            cmd.Parameters.Add("@office_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@user_id", SqlDbType.NVarChar, 30).Value = "0"
            cmd.Parameters.Add("@show_reports", SqlDbType.NVarChar, 3).Value = "no"
            cmd.Parameters.Add("@status", SqlDbType.NVarChar, 4).Value = "True"
            cmd.Parameters.Add("@manage", SqlDbType.NVarChar, 3).Value = manage

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                personnelList.Add(row("user_id").ToString)
            Next
            cnx.Close()
        End Using

        Return personnelList
    End Function

    ''' <summary>
    ''' Returns all the personnel data for users assigned to the WSC
    ''' </summary>
    ''' <returns>A DataTable</returns>
    Public Function GetPersonnel(ByVal wsc_id As Integer, ByVal active_only As String, ByVal manage As String) As DataTable
        Dim dt As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SP_personnel_by_WSC_office_or_user_id", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = wsc_id
            cmd.Parameters.Add("@office_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@user_id", SqlDbType.NVarChar, 30).Value = "0"
            cmd.Parameters.Add("@show_reports", SqlDbType.NVarChar, 3).Value = "no"
            cmd.Parameters.Add("@status", SqlDbType.NVarChar, 4).Value = active_only
            cmd.Parameters.Add("@manage", SqlDbType.NVarChar, 3).Value = manage

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            cnx.Close()
        End Using

        Return dt
    End Function

    ''' <summary>
    ''' Returns lists of record-types by WSC
    ''' </summary>
    ''' <param name="type">Pass 'all' for all record-types returned, 'cont' for only continuous record-types, or
    ''' 'noncont' for only non-continuous record-types</param>
    ''' <param name="se">Sort expression; pass nothing to ignore</param>
    Public Function GetRecordTypeList(ByVal wsc_id As Integer, ByVal type As String, ByVal se As String) As DataTable
        Dim recordTypeList As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SP_Record_Type_by_WSC", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = wsc_id
            cmd.Parameters.Add("@option", SqlDbType.NVarChar, 7).Value = type
            cmd.Parameters.Add("@sortexpression", SqlDbType.NVarChar, 20).Value = se

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(recordTypeList)

            cnx.Close()
        End Using

        Return recordTypeList
    End Function

    ''' <summary>
    ''' Gets a datatable of offices for a WSC
    ''' </summary>
    Public Function GetOfficeList(ByVal wsc_id As Integer) As DataTable
        Dim officelist As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As New SqlCommand("SP_Office_Info", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@office_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@office_cd", SqlDbType.NVarChar, 3).Value = "0"
            cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = wsc_id
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@action", SqlDbType.NVarChar, 20).Value = "bywscid"

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(officelist)

            cnx.Close()
        End Using

        Return officelist
    End Function

    ''' <summary>
    ''' Gets a datatable of hospitals for a WSC
    ''' </summary>
    Public Function GetHospitals(ByVal wsc_id As Integer) As DataTable
        Dim hospitallist As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand("SP_Hospitals", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@hospital_nm", SqlDbType.Int).Value = "0"
            cmd.Parameters.Add("@wsc_id", SqlDbType.NVarChar, 3).Value = wsc_id
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@hospital_id", SqlDbType.Int).Value = 0

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(hospitallist)

            cnx.Close()
        End Using

        Return hospitallist
    End Function

    ''' <summary>
    ''' Gets a datatable of contacts for a WSC
    ''' </summary>
    Public Function GetContacts(ByVal wsc_id As Integer) As DataTable
        Dim contactlist As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand("SP_Contacts", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@contact_nm", SqlDbType.Int).Value = "0"
            cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = wsc_id
            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = 0
            cmd.Parameters.Add("@contact_id", SqlDbType.Int).Value = 0

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(contactlist)

            cnx.Close()
        End Using

        Return contactlist
    End Function

    ''' <summary>
    ''' Gets a datatable of cableways for a WSC
    ''' </summary>
    ''' <param name="criteria">Pass nothing for all cableways; pass "norstatus" for all cableways except ones with a status of "r"; pass "rstatus" for only cableways with a status of "r"</param>
    Public Function GetCableways(ByVal wsc_id As Integer, ByVal criteria As String) As DataTable
        Dim cablewaylist As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim where_stmt As String = ""
            If criteria = "norstatus" Then
                where_stmt = " AND scm.cableway_status_cd Not In('r','ro')"
            ElseIf criteria = "rstatus" Then
                where_stmt = " AND scm.cableway_status_cd In('r','ro')"
            End If

            Dim sql As String = "SELECT scm.cableway_id, (CASE WHEN scm.cableway_nm Is Not Null THEN (CASE WHEN scm.cableway_nm <> '' THEN" & _
                " RTrim(ssm.site_no) + ' ' + s.station_nm + ' (' + scm.cableway_nm + ')' ELSE RTrim(ssm.site_no) + ' ' + s.station_nm END) " & _
                " ELSE RTrim(ssm.site_no) + ' ' + s.station_nm END) AS site_no_nm, ssm.site_no, scm.site_id, scm.cableway_nm, scm.cableway_status_cd, scm.cableway_type_cd," & _
                " scm.cableway_inspection_freq, (CASE WHEN scm.aerial_marker_req Is Not Null THEN scm.aerial_marker_req ELSE 'U' END) AS aerial_marker_req," & _
                " (CASE WHEN scm.aerial_marker_inst Is Not Null THEN scm.aerial_marker_inst ELSE 'U' END) AS aerial_marker_inst, scm.created_by, scm.created_dt, scm.updated_by," & _
                " scm.updated_dt, lcsc.cableway_status_desc, lctc.cableway_type_desc, lo.office_cd, (scm.cableway_status_cd + ' - ' + lcsc.cableway_status_desc) AS status_cd_desc," & _
                " (scm.cableway_type_cd + ' - ' + lctc.cableway_type_desc) AS type_cd_desc" & _
                " FROM lut_cableway_status_cd AS lcsc INNER JOIN Safety_Cableway_Master AS scm ON lcsc.cableway_status_cd = scm.cableway_status_cd INNER JOIN" & _
                " lut_cableway_type_cd AS lctc ON scm.cableway_type_cd = lctc.cableway_type_cd INNER JOIN SIMS_Site_Master AS ssm ON scm.site_id = ssm.site_id INNER JOIN" & _
                " lut_Office AS lo ON ssm.office_id = lo.office_id INNER JOIN nwisweb.dbo.SITEFILE AS s ON ssm.nwisweb_site_id = s.site_id" & _
                " WHERE (lo.wsc_id = " & wsc_id & ")" & where_stmt & _
                " ORDER BY ssm.site_no"

            Dim cmd As New SqlCommand(sql, cnx)
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(cablewaylist)

            cnx.Close()
        End Using

        Return cablewaylist
    End Function

    ''' <summary>
    ''' Returns a list of WSCs as a DataTable
    ''' </summary>
    Public Function GetWSCs() As DataTable
        Dim dt As New DataTable

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand("SP_WSC_Info", cnx)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.Add("@wsc_id", SqlDbType.Int).Value = 0
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
