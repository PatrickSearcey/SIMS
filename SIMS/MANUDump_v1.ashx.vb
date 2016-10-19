Imports System.Web
Imports System.Web.Services
Imports System.Xml

Public Class MANUDump_v1
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

        For Each x In lstElems
            strElementName = x.ElementName.ToString().Replace(" (MANU)", "")
            strElementInfo = x.ElementInfo

            'Don't send EXTREMES FOR CURRENT YEAR element to NWIS
            If InStr(strElementName, "EXTREMES FOR CURRENT YEAR") = 0 And InStr(strElementName, "PEAK DISCHARGES FOR CURRENT YEAR") = 0 Then
                XMLwrite.WriteWhitespace(Environment.NewLine)
                XMLwrite.WriteStartElement("Element")

                XMLwrite.WriteWhitespace(Environment.NewLine)
                XMLwrite.WriteElementString("name", strElementName)

                XMLwrite.WriteWhitespace(Environment.NewLine)
                XMLwrite.WriteElementString("info", strElementInfo)

                XMLwrite.WriteWhitespace(Environment.NewLine)
                XMLwrite.WriteEndElement()
            End If
            'End Element
        Next

        XMLwrite.WriteWhitespace(Environment.NewLine)
        XMLwrite.WriteEndElement()
        'End Manuscript

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
