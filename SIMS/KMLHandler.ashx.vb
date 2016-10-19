Imports System.Web
Imports Google.KML
Imports System.Xml
Imports System.Net
Imports System.IO
Imports System.Configuration
Imports System.Data

''' <summary>
''' This class will handle returning all of the dynamic KML/KMZ requests coming from Google Earth
''' Usually from a NetworkLink.
''' </summary>
Public Class KMLHandler
    Implements IHttpHandler

    Private Const office_id_qs As String = "office_id"
    Private Const wsc_id_qs As String = "wsc_id"

    Private wsc_id As Integer = 0
    Private office_id As String = "0"

    ''' <summary>
    ''' This is your main, or page_load.  It's where we're gonna start.
    ''' </summary>
    ''' <param name="context"></param>
    Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest

        Setup(context)

        'string csv = GetGeoCodeCSV(address);

        createSiteKML(office_id, wsc_id, context)

        'Send everything now and close the server response.        
        context.Response.Flush()
    End Sub

    ''' <summary>
    ''' Set up our environment based on current session or query string info.
    ''' </summary>
    ''' <param name="context"></param>
    Private Sub Setup(context As HttpContext)
        If context.Request.QueryString(wsc_id_qs) <> Nothing Then
            wsc_id = context.Request.QueryString(wsc_id_qs)
        End If

        If context.Request.QueryString(office_id_qs) IsNot Nothing Then
            office_id = context.Request.QueryString(office_id_qs)
        End If
    End Sub

    ''' <summary>
    ''' Needed to implement IHttpHandler, we probably won't use it.
    ''' </summary>
    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

    Private Function GetGeoCodeCSV(address As String) As String
        Dim url As String = ("http://maps.google.com/maps/geo?q=" & address & "&output=csv&oe=utf8&sensor=true&key=") + ConfigurationManager.AppSettings("GoogleKey")
        Dim html As String = ""
        Dim req As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
        Try
            Dim res As HttpWebResponse = DirectCast(req.GetResponse(), HttpWebResponse)
            If res IsNot Nothing Then
                If res.StatusCode = HttpStatusCode.OK Then
                    Dim stream As Stream = res.GetResponseStream()
                    Using reader As New StreamReader(stream)
                        html = reader.ReadToEnd()
                    End Using
                End If
                res.Close()
            End If
        Catch
        End Try
        Return html
    End Function

    Protected Sub createSiteKML(office_id As String, wsc_id As Integer, context As HttpContext)
        Dim dt As DataTable = KML.GetSites(office_id, wsc_id, 0)
        Dim dt1 As DataTable = KML.GetFieldTrips(office_id)
        Dim strOfficeName As String
        Dim strWSCName As String
        Dim strDocName As String
        Dim strFieldTrip As String
        Dim strGeocodes As String
        Dim strSiteType As String
        Dim strDescription As String
        Dim strName As String
        Dim strNumber As String
        Dim coordinates As String()
        Dim dGeoLatitude As String
        Dim dGeoLongitude As String
        Dim strIconFileName As String

        If office_id <> "0" Then
            strOfficeName = KML.GetOfficeInfo(office_id, "name", wsc_id)
            strDocName = "Sites for " & strOfficeName
        Else
            strWSCName = KML.GetWSCInfo(wsc_id, "name")
            strDocName = "Sites for " & strWSCName
        End If

        context.Response.ContentType = "application/vnd.google-earth.kml+xml"
        context.Response.ContentEncoding = System.Text.Encoding.UTF8
        Dim stream As New System.IO.MemoryStream()
        Dim XMLwrite As New XmlTextWriter(stream, System.Text.Encoding.UTF8)

        XMLwrite.WriteStartDocument()
        XMLwrite.WriteWhitespace(Environment.NewLine)
        XMLwrite.WriteStartElement("kml")
        XMLwrite.WriteAttributeString("xmlns", "http://www.opengis.net/kml/2.2")
        XMLwrite.WriteWhitespace(Environment.NewLine)

        XMLwrite.WriteWhitespace(Environment.NewLine)
        XMLwrite.WriteStartElement("Document")
        XMLwrite.WriteWhitespace(Environment.NewLine)
        XMLwrite.WriteElementString("name", strDocName)
        XMLwrite.WriteWhitespace(Environment.NewLine)
        XMLwrite.WriteStartElement("Folder")
        XMLwrite.WriteAttributeString("id", strDocName)
        XMLwrite.WriteWhitespace(Environment.NewLine)
        XMLwrite.WriteElementString("name", strDocName)

        For Each row As DataRow In dt.Rows
            strGeocodes = Convert.ToString(row("dec_long_va")) & ", " & Convert.ToString(row("dec_lat_va")) & ", 0.000000"
            strSiteType = DirectCast(row("site_tp_cd"), String)

            Select Case strSiteType
                Case "LK", "OC", "ST", "ES", "GL", "OC-CO", _
                 "ST-CA", "ST-DCH", "ST-TS", "WE"
                    strIconFileName = "http://sims.water.usgs.gov/SIMS/images/SiteIcons/SW.png"
                    Exit Select
                Case "GW", "GW-CR", "GW-EX", "GW-HZ", "GW-IE", "GW-MW", _
                 "GW-TH", "SB", "SB-CV", "SB-GWD", "SB-TSM", "SB-UZ"
                    strIconFileName = "http://sims.water.usgs.gov/SIMS/images/SiteIcons/GW.png"
                    Exit Select
                Case "AT"
                    strIconFileName = "http://sims.water.usgs.gov/SIMS/images/SiteIcons/CL.png"
                    Exit Select
                Case "SP"
                    strIconFileName = "http://sims.water.usgs.gov/SIMS/images/SiteIcons/SP.png"
                    Exit Select
                Case Else
                    strIconFileName = "http://sims.water.usgs.gov/SIMS/images/SiteIcons/OT.png"
                    Exit Select
            End Select

            strDescription = "<![CDATA[<html xmlns:fo='http://www.w3.org/1999/XSL/Format' xmlns:msxsl='urn:schemas-microsoft-com:xslt'> <head> <META http-equiv='Content-Type' content='text/html'> </head> <body style='margin:0px 0px 0px 0px;overflow:auto;background:#FFFFFF;'> <table style='font-family:Arial,Verdana,Times;font-size:12px;text-align:left;width:300;border-collapse:collapse;padding:3px 3px 3px 3px'> <tr style='text-align:center;font-weight:bold;background:#9CBCE2'> <td>USGS - US GEOLOGICAL SURVEY</td> </tr> <tr> <td> <table style='font-family:Arial,Verdana,Times;font-size:12px;text-align:left;width:300;border-spacing:0px; padding:3px 3px 3px 3px'> <tr> <td>USGS site identifier</td> <td>" & Convert.ToString(row("site_no")) & "</td> </tr> <tr bgcolor='#D4E4F3'> <td>Station name</td> <td>" & Convert.ToString(row("station_nm")) & "</td> </tr> <tr> <td>Latitude, DD</td> <td>" & Convert.ToString(row("dec_lat_va")) & "</td> </tr> <tr bgcolor='#D4E4F3'> <td>Longitude, DD</td> <td>" & Convert.ToString(row("dec_long_va")) & "</td> </tr> <tr> <td>Site type</td> <td>" & Convert.ToString(row("site_tp_cd")) & "</td> </tr> <tr bgcolor='#D4E4F3'> <td>Office code</td> <td>" & Convert.ToString(row("office_cd")) & "</td> </tr> <tr> <td>Field trip</td> <td>" & Convert.ToString(row("trip_full_nm")) & "</td> </tr> </table> </td> </tr> </table> </body> </html>]]>"
            strName = DirectCast(row("station_nm"), String)
            strNumber = DirectCast(row("site_no"), String)

            coordinates = strGeocodes.Split(","c)
            dGeoLatitude = coordinates(1)
            dGeoLongitude = coordinates(0)

            XMLwrite.WriteWhitespace(Environment.NewLine)
            XMLwrite.WriteStartElement("Placemark")
            XMLwrite.WriteWhitespace(Environment.NewLine)
            XMLwrite.WriteElementString("name", strNumber)
            XMLwrite.WriteWhitespace(Environment.NewLine)
            XMLwrite.WriteElementString("Snippet", strNumber & " " & strName)
            XMLwrite.WriteWhitespace(Environment.NewLine)
            XMLwrite.WriteElementString("description", strDescription)
            XMLwrite.WriteWhitespace(Environment.NewLine)
            XMLwrite.WriteStartElement("Style")
            XMLwrite.WriteWhitespace(Environment.NewLine)
            XMLwrite.WriteStartElement("IconStyle")
            XMLwrite.WriteWhitespace(Environment.NewLine)
            XMLwrite.WriteStartElement("Icon")
            XMLwrite.WriteElementString("href", strIconFileName)
            XMLwrite.WriteEndElement()
            'End Icon
            XMLwrite.WriteWhitespace(Environment.NewLine)
            XMLwrite.WriteElementString("scale", "0.500")
            XMLwrite.WriteWhitespace(Environment.NewLine)
            XMLwrite.WriteEndElement()
            'End IconStyle
            XMLwrite.WriteWhitespace(Environment.NewLine)
            XMLwrite.WriteStartElement("LabelStyle")
            XMLwrite.WriteWhitespace(Environment.NewLine)
            XMLwrite.WriteElementString("scale", "0.667")
            XMLwrite.WriteWhitespace(Environment.NewLine)
            XMLwrite.WriteEndElement()
            'End LabelStyle
            XMLwrite.WriteWhitespace(Environment.NewLine)
            XMLwrite.WriteEndElement()
            'End Style
            XMLwrite.WriteWhitespace(Environment.NewLine)

            XMLwrite.WriteStartElement("Point")
            XMLwrite.WriteWhitespace(Environment.NewLine)
            XMLwrite.WriteElementString("extrude", "0")
            XMLwrite.WriteWhitespace(Environment.NewLine)
            XMLwrite.WriteElementString("altitudeMode", "clampToGround")
            XMLwrite.WriteWhitespace(Environment.NewLine)
            XMLwrite.WriteElementString("coordinates", dGeoLongitude & ", " & dGeoLatitude & ", 0")
            XMLwrite.WriteWhitespace(Environment.NewLine)
            XMLwrite.WriteEndElement()
            'End Point
            XMLwrite.WriteWhitespace(Environment.NewLine)
            'End Placemark
            XMLwrite.WriteEndElement()
        Next

        XMLwrite.WriteWhitespace(Environment.NewLine)
        XMLwrite.WriteEndElement()
        'End Folder

        If office_id <> "0" Then
            For Each row As DataRow In dt1.Rows
                Dim dt2 As DataTable = KML.GetSites("0", 0, row("trip_id"))
                strFieldTrip = row("trip_nm") & " - " & row("user_id")

                XMLwrite.WriteStartElement("Folder")
                XMLwrite.WriteAttributeString("id", strFieldTrip)
                XMLwrite.WriteWhitespace(Environment.NewLine)
                XMLwrite.WriteElementString("name", strFieldTrip)

                For Each row1 As DataRow In dt2.Rows
                    strGeocodes = Convert.ToString(row1("dec_long_va")) & ", " & Convert.ToString(row1("dec_lat_va")) & ", 0.000000"
                    strSiteType = DirectCast(row1("site_tp_cd"), String)

                    Select Case strSiteType
                        Case "LK", "OC", "ST", "ES", "GL", "OC-CO", _
                         "ST-CA", "ST-DCH", "ST-TS", "WE"
                            strIconFileName = "http://sims.water.usgs.gov/SIMS/images/SiteIcons/SW.png"
                            Exit Select
                        Case "GW", "GW-CR", "GW-EX", "GW-HZ", "GW-IE", "GW-MW", _
                         "GW-TH", "SB", "SB-CV", "SB-GWD", "SB-TSM", "SB-UZ"
                            strIconFileName = "http://sims.water.usgs.gov/SIMS/images/SiteIcons/GW.png"
                            Exit Select
                        Case "AT"
                            strIconFileName = "http://sims.water.usgs.gov/SIMS/images/SiteIcons/CL.png"
                            Exit Select
                        Case "SP"
                            strIconFileName = "http://sims.water.usgs.gov/SIMS/images/SiteIcons/SP.png"
                            Exit Select
                        Case Else
                            strIconFileName = "http://sims.water.usgs.gov/SIMS/images/SiteIcons/OT.png"
                            Exit Select
                    End Select

                    strDescription = "<![CDATA[<html xmlns:fo='http://www.w3.org/1999/XSL/Format' xmlns:msxsl='urn:schemas-microsoft-com:xslt'> <head> <META http-equiv='Content-Type' content='text/html'> </head> <body style='margin:0px 0px 0px 0px;overflow:auto;background:#FFFFFF;'> <table style='font-family:Arial,Verdana,Times;font-size:12px;text-align:left;width:300;border-collapse:collapse;padding:3px 3px 3px 3px'> <tr style='text-align:center;font-weight:bold;background:#9CBCE2'> <td>USGS - US GEOLOGICAL SURVEY</td> </tr> <tr> <td> <table style='font-family:Arial,Verdana,Times;font-size:12px;text-align:left;width:300;border-spacing:0px; padding:3px 3px 3px 3px'> <tr> <td>USGS site identifier</td> <td>" & Convert.ToString(row1("site_no")) & "</td> </tr> <tr bgcolor='#D4E4F3'> <td>Station name</td> <td>" & Convert.ToString(row1("station_nm")) & "</td> </tr> <tr> <td>Latitude, DD</td> <td>" & Convert.ToString(row1("dec_lat_va")) & "</td> </tr> <tr bgcolor='#D4E4F3'> <td>Longitude, DD</td> <td>" & Convert.ToString(row1("dec_long_va")) & "</td> </tr> <tr> <td>Site type</td> <td>" & Convert.ToString(row1("site_tp_cd")) & "</td> </tr> <tr bgcolor='#D4E4F3'> <td>Office code</td> <td>" & Convert.ToString(row1("office_cd")) & "</td> </tr> <tr> <td>Field trip</td> <td>" & Convert.ToString(row1("trip_full_nm")) & "</td> </tr> </table> </td> </tr> </table> </body> </html>]]>"
                    strName = DirectCast(row1("station_nm"), String)
                    strNumber = DirectCast(row1("site_no"), String)

                    coordinates = strGeocodes.Split(","c)
                    dGeoLatitude = coordinates(1)
                    dGeoLongitude = coordinates(0)

                    XMLwrite.WriteWhitespace(Environment.NewLine)
                    XMLwrite.WriteStartElement("Placemark")
                    XMLwrite.WriteWhitespace(Environment.NewLine)
                    XMLwrite.WriteElementString("name", strNumber)
                    XMLwrite.WriteWhitespace(Environment.NewLine)
                    XMLwrite.WriteElementString("Snippet", strNumber & " " & strName)
                    XMLwrite.WriteWhitespace(Environment.NewLine)
                    XMLwrite.WriteElementString("description", strDescription)
                    XMLwrite.WriteWhitespace(Environment.NewLine)
                    XMLwrite.WriteStartElement("Style")
                    XMLwrite.WriteWhitespace(Environment.NewLine)
                    XMLwrite.WriteStartElement("IconStyle")
                    XMLwrite.WriteWhitespace(Environment.NewLine)
                    XMLwrite.WriteStartElement("Icon")
                    XMLwrite.WriteElementString("href", strIconFileName)
                    XMLwrite.WriteEndElement()
                    'End Icon
                    XMLwrite.WriteWhitespace(Environment.NewLine)
                    XMLwrite.WriteElementString("scale", "0.500")
                    XMLwrite.WriteWhitespace(Environment.NewLine)
                    XMLwrite.WriteEndElement()
                    'End IconStyle
                    XMLwrite.WriteWhitespace(Environment.NewLine)
                    XMLwrite.WriteStartElement("LabelStyle")
                    XMLwrite.WriteWhitespace(Environment.NewLine)
                    XMLwrite.WriteElementString("scale", "0.667")
                    XMLwrite.WriteWhitespace(Environment.NewLine)
                    XMLwrite.WriteEndElement()
                    'End LabelStyle
                    XMLwrite.WriteWhitespace(Environment.NewLine)
                    XMLwrite.WriteEndElement()
                    'End Style
                    XMLwrite.WriteWhitespace(Environment.NewLine)

                    XMLwrite.WriteStartElement("Point")
                    XMLwrite.WriteWhitespace(Environment.NewLine)
                    XMLwrite.WriteElementString("extrude", "0")
                    XMLwrite.WriteWhitespace(Environment.NewLine)
                    XMLwrite.WriteElementString("altitudeMode", "clampToGround")
                    XMLwrite.WriteWhitespace(Environment.NewLine)
                    XMLwrite.WriteElementString("coordinates", dGeoLongitude & ", " & dGeoLatitude & ", 0")
                    XMLwrite.WriteWhitespace(Environment.NewLine)
                    XMLwrite.WriteEndElement()
                    'End Point
                    XMLwrite.WriteWhitespace(Environment.NewLine)
                    'End Placemark
                    XMLwrite.WriteEndElement()
                Next

                XMLwrite.WriteWhitespace(Environment.NewLine)
                XMLwrite.WriteEndElement()
                'End Folder
            Next
        End If

        XMLwrite.WriteWhitespace(Environment.NewLine)
        XMLwrite.WriteEndElement()
        'End Document
        XMLwrite.WriteWhitespace(Environment.NewLine)
        XMLwrite.WriteEndDocument()
        'End kml
        XMLwrite.Flush()
        Dim reader As System.IO.StreamReader
        stream.Position = 0
        reader = New System.IO.StreamReader(stream)
        Dim bytes As [Byte]() = System.Text.Encoding.UTF8.GetBytes(reader.ReadToEnd())
        context.Response.BinaryWrite(bytes)
        context.Response.[End]()
    End Sub

End Class