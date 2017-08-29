Imports System.Web
Imports System.Web.Services
Imports System.Xml
Imports System.Data.SqlClient

Public Class MANUDump_v2
    Implements System.Web.IHttpHandler

    Private svcSIMS As SIMSService.SIMSServiceClient
    Private Const site_no_qs As String = "site_no"
    Private Const agency_cd_qs As String = "agency_cd"

    Private site_no As String
    Private agency_cd As String
    Private s As Site

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Setup(context)

        createSiteMANUXML(site_no, agency_cd, context)

        'Send everything now and close the server response.        
        context.Response.Flush()
    End Sub

    ''' <summary>
    ''' Set up our environment based on current session or query string info.
    ''' </summary>
    ''' <param name="context"></param>
    Private Sub Setup(context As HttpContext)
        If context.Request.QueryString(site_no_qs) IsNot Nothing Then
            site_no = context.Request.QueryString(site_no_qs)
        End If
        If context.Request.QueryString(agency_cd_qs) IsNot Nothing Then
            agency_cd = context.Request.QueryString(agency_cd_qs)
        End If
        s = New Site(site_no, agency_cd)
        's = New Site("434307112382601", "USGS")
    End Sub

    ''' <summary>
    ''' Needed to implement IHttpHandler, we probably won't use it.
    ''' </summary>
    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

    Protected Sub createSiteMANUXML(site_no As String, agency_cd As String, context As HttpContext)
        svcSIMS = New SIMSService.SIMSServiceClient()
        Dim lstElems() As SIMSService.Element

        Try
            lstElems = svcSIMS.GetElementsBySiteAndReport(s.Number, s.AgencyCode, "MANU")
        Catch ex As Exception
        End Try

        Dim strElementName As String = ""
        Dim strElementInfo As String = ""
        Dim strRevisedDate As String = ""
        Dim strStatus As String = ""
        Dim pf5 As String = ""
        Dim pf6 As String = ""

        context.Response.ContentType = "text/xml"
        context.Response.ContentEncoding = System.Text.Encoding.UTF8
        Dim stream As New System.IO.MemoryStream()
        Dim XMLwrite As New XmlTextWriter(stream, System.Text.Encoding.UTF8)

        XMLwrite.WriteStartDocument()

        XMLwrite.WriteWhitespace(Environment.NewLine)
        XMLwrite.WriteStartElement("USGS")

        XMLwrite.WriteWhitespace(Environment.NewLine)
        XMLwrite.WriteStartElement("Manuscript")

        XMLwrite.WriteWhitespace(Environment.NewLine)
        XMLwrite.WriteElementString("Description", "Manuscript for site " & s.Number & " " & s.FullName)
        XMLwrite.WriteWhitespace(Environment.NewLine)
        XMLwrite.WriteElementString("site_no", s.Number)
        XMLwrite.WriteWhitespace(Environment.NewLine)
        XMLwrite.WriteElementString("agency_cd", s.AgencyCode)
        XMLwrite.WriteWhitespace(Environment.NewLine)
        XMLwrite.WriteElementString("station_full_nm", s.FullName)

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd As New SqlCommand("SELECT era.approved_dt, ers.revised_dt" & _
                " FROM Elem_Report_Approve AS era LEFT OUTER JOIN" & _
                " Elem_Report_Sum AS ers ON era.site_id = ers.site_id" & _
                " WHERE (ers.report_type_cd = 'MANU') AND era.site_id = " & s.ID.ToString(), cnx)
            Dim dt As New DataTable

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            If dt.Rows.Count = 0 Then
                XMLwrite.WriteWhitespace(Environment.NewLine)
                XMLwrite.WriteElementString("status", "provisional")
            Else
                Dim approved_dt As DateTime
                Dim revised_dt As DateTime

                For Each row As DataRow In dt.Rows
                    If Not row("approved_dt") Is DBNull.Value Then
                        approved_dt = row("approved_dt")
                        If Not row("revised_dt") Is DBNull.Value Then
                            revised_dt = row("revised_dt")

                            If revised_dt < approved_dt Then
                                strStatus = "approved"
                            Else
                                strStatus = "in review"
                            End If
                        Else
                            strStatus = "approved"
                        End If
                    Else
                        strStatus = "provisional"
                    End If

                    XMLwrite.WriteWhitespace(Environment.NewLine)
                    XMLwrite.WriteElementString("status", strStatus)
                Next
            End If

            cnx.Close()
        End Using

        For Each x In lstElems
            strElementName = x.ElementName.ToString().Replace(" (MANU)", "")
            strElementInfo = x.ElementInfo
            strRevisedDate = String.Format("{0:MM/dd/yyyy}", x.RevisedDate)

            'Don't send EXTREMES FOR CURRENT YEAR element to NWIS
            If InStr(strElementName, "EXTREMES FOR CURRENT YEAR") = 0 And InStr(strElementName, "PEAK DISCHARGES FOR CURRENT YEAR") = 0 Then
                XMLwrite.WriteWhitespace(Environment.NewLine)
                XMLwrite.WriteStartElement("Element")

                XMLwrite.WriteWhitespace(Environment.NewLine)
                XMLwrite.WriteElementString("name", strElementName)

                XMLwrite.WriteWhitespace(Environment.NewLine)
                XMLwrite.WriteElementString("revised_dt", strRevisedDate)

                XMLwrite.WriteWhitespace(Environment.NewLine)
                XMLwrite.WriteElementString("info", strElementInfo)

                XMLwrite.WriteWhitespace(Environment.NewLine)
                XMLwrite.WriteEndElement()
            End If
            'End Element
        Next

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()

            Dim cmd1 As New SqlCommand("SELECT ssm.agency_cd, ssm.site_no, led.element_nm, ese.element_info, ese.revised_dt" & _
                " FROM dbo.Elem_Site_Master AS esm INNER JOIN" & _
                "   dbo.Elem_Site_Element AS ese ON esm.site_id = ese.site_id INNER JOIN" & _
                "   dbo.SIMS_Site_Master AS ssm ON esm.site_id = ssm.site_id INNER JOIN" & _
                "   dbo.Elem_Lut_ElemDetail AS led ON ese.element_id = led.element_id" & _
                " WHERE (ese.element_id IN (39, 40)) AND ssm.site_no = '" & s.Number & "'", cnx)
            Dim dt1 As New DataTable

            Dim da1 As New SqlDataAdapter(cmd1)
            da1.Fill(dt1)

            For Each row As DataRow In dt1.Rows
                strElementName = row("element_nm")
                strElementInfo = row("element_info")
                strRevisedDate = String.Format("{0:MM/dd/yyyy}", row("revised_dt"))

                XMLwrite.WriteWhitespace(Environment.NewLine)
                XMLwrite.WriteStartElement("Element")

                XMLwrite.WriteWhitespace(Environment.NewLine)
                XMLwrite.WriteElementString("name", strElementName)

                XMLwrite.WriteWhitespace(Environment.NewLine)
                XMLwrite.WriteElementString("revised_dt", strRevisedDate)

                XMLwrite.WriteWhitespace(Environment.NewLine)
                XMLwrite.WriteElementString("info", strElementInfo)

                XMLwrite.WriteWhitespace(Environment.NewLine)
                XMLwrite.WriteEndElement()
            Next
            cnx.Close()
        End Using

        XMLwrite.WriteWhitespace(Environment.NewLine)
        XMLwrite.WriteEndElement()
        'End Manuscript

        XMLwrite.WriteWhitespace(Environment.NewLine)
        XMLwrite.WriteStartElement("PeakFlowFile")

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd2 As New SqlCommand("SELECT pvt.site_no, pvt.pf5, pvt.pf6" & _
                " FROM (SELECT ssm.site_no, 'pf5' AS data_in, 'X' AS data_va" & _
                "   FROM OPENQUERY(a, 'select DISTINCT site_id from PEAK where (peak_cd like ''5'')') AS t INNER JOIN" & _
                "   simsdb.dbo.SIMS_Site_Master AS ssm ON ssm.nwisweb_site_id = t.site_id" & _
                "   UNION" & _
                "   SELECT ssm.site_no, 'pf6' AS data_in, 'X' AS data_va" & _
                "   FROM OPENQUERY(a, 'select DISTINCT site_id from PEAK where (peak_cd like ''6'')') AS t INNER JOIN" & _
                "   simsdb.dbo.SIMS_Site_Master AS ssm ON ssm.nwisweb_site_id = t.site_id) as t1 PIVOT (MAX(data_va) FOR data_in IN ([pf5],[pf6])) AS pvt" & _
                " WHERE site_no = '" & s.Number & "'", cnx)
            Dim dt2 As New DataTable

            Dim da2 As New SqlDataAdapter(cmd2)
            da2.Fill(dt2)

            For Each row As DataRow In dt2.Rows
                If Not row("pf5") Is DBNull.Value Then
                    pf5 = row("pf5")
                Else
                    pf5 = "false"
                End If
                If Not row("pf6") Is DBNull.Value Then
                    pf6 = row("pf6")
                Else
                    pf6 = "false"
                End If

                If pf5 = "X" Then
                    pf5 = "true"
                End If
                If pf6 = "X" Then
                    pf6 = "true"
                End If

                XMLwrite.WriteWhitespace(Environment.NewLine)
                XMLwrite.WriteElementString("pf5", pf5)

                XMLwrite.WriteWhitespace(Environment.NewLine)
                XMLwrite.WriteElementString("pf6", pf6)
            Next

            cnx.Close()
        End Using

        XMLwrite.WriteWhitespace(Environment.NewLine)
        XMLwrite.WriteEndElement()
        'End PeakFlowFile

        XMLwrite.WriteWhitespace(Environment.NewLine)
        XMLwrite.WriteEndDocument()
        'End USGS

        XMLwrite.Flush()
        Dim reader As System.IO.StreamReader
        stream.Position = 0
        reader = New System.IO.StreamReader(stream)
        Dim bytes As [Byte]() = System.Text.Encoding.UTF8.GetBytes(reader.ReadToEnd())
        context.Response.BinaryWrite(bytes)
        context.Response.[End]()
    End Sub

End Class