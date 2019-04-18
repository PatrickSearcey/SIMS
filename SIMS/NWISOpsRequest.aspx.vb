Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb
Imports System.Net.Mail
Imports System.Web.HttpContext
Imports Telerik.Web.UI


Public Class NWISOpsRequest
    Inherits System.Web.UI.Page

    Private s As Site
    Private o As Office
    Private u As User

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Master.PageTitle = "NWIS Ops Help Request"
        Master.ResponsibleOffice = False
        Page.Title = "SIMS - NWIS Ops Help Request"

        Dim office_id As Integer = Nothing
        Dim site_no As String = Nothing
        Dim uid As String = Current.User.Identity.Name
        u = New User(uid)

        Session("user_id") = u.ID

        If Request.QueryString("office_id") <> "" Then
            office_id = Request.QueryString("office_id")
            Session("office_id") = office_id
        Else
            office_id = Session("office_id")
            'Session("office_id") = 48
        End If

        If Request.QueryString("site_no") <> "" Then
            site_no = Trim(Request.QueryString("site_no"))
            Session("agency_cd") = "USGS"
        ElseIf Request.QueryString("site_id") <> "" Then
            s = New Site(Request.QueryString("site_id"))
            site_no = Trim(s.Number)
            Session("agency_cd") = s.AgencyCode
        Else
            site_no = "0"
            Session("agency_cd") = "USGS"
        End If

        If Not Page.IsPostBack Then
            Call PrePopulateForm(Session("user_id"), site_no)
            pnlStep1.Visible = True
        End If
    End Sub

    Private Sub PrePopulateForm(ByVal user_id As String, ByVal site_no As String)
        If site_no = "0" Then
            tbSiteNo.Visible = True
            lblOptional.Visible = True
            lblSiteNo.Visible = False
        Else
            s = New Site(site_no, Session("agency_cd"))
            lblOptional.Visible = False
            tbSiteNo.Visible = False
            lblSiteNo.Visible = True
            hfSiteNo.Value = site_no
            lblSiteNo.Text = s.NumberName
        End If

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As SqlCommand = New SqlCommand("spz_GetUserInfoFromAD", cnx)
            Dim da As SqlDataAdapter
            Dim dt As DataTable = New DataTable
            cmd.CommandType = Data.CommandType.StoredProcedure

            cmd.Parameters.Add("@user_id", SqlDbType.NVarChar, 50).Value = user_id

            da = New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                lblName.Text = row("givenName") & " " & row("SN")
                lblPhone.Text = row("TelephoneNumber")
                lblEmail.Text = row("mail")
            Next
            cnx.Close()
        End Using

    End Sub

    Public Sub Back_Command(ByVal sender As Object, ByVal Args As CommandEventArgs)
        If Args.CommandName = "back2" Then
            pnlStep1.Visible = True
            pnlStep2.Visible = False
        ElseIf Args.CommandName = "back3" Then
            pnlStep2.Visible = True
            pnlStep3.Visible = False
        ElseIf Args.CommandName = "back4" Then
            pnlStep3.Visible = True
            pnlStep4.Visible = False
        End If
    End Sub

    Public Sub Next_Command(ByVal sender As Object, ByVal Args As CommandEventArgs)
        If Args.CommandName = "next1" Then
            hfName.Value = lblName.Text
            hfPhone.Value = lblPhone.Text
            hfEmail.Value = lblEmail.Text
            If tbSiteNo.Text <> Nothing Then
                s = New Site(tbSiteNo.Text, "USGS")
                Dim sn As String = s.Name
                If sn = Nothing Then
                    lblWarning.Text = "<br />No site found in NWIS with this site number"
                    Exit Sub
                Else
                    lblWarning.Text = ""
                End If
                hfSiteNo.Value = tbSiteNo.Text
                Master.SiteNumberLabel = "<a href=""" & Config.SitePath & "StationInfo.asp?site_id=" & _
                    s.ID & """>" & tbSiteNo.Text & " " & sn & "</a>"
                Dim site_no_label As UpdatePanel = Master.FindControl("up1")
                site_no_label.Update()
            End If
            If hfSiteNo.Value <> Nothing Then
                Dim oi As Integer = New Site(hfSiteNo.Value, "USGS").OfficeID
                If oi = 0 Then
                    hfOfficeID.Value = Session("office_id")
                Else
                    hfOfficeID.Value = oi
                End If
            Else
                hfOfficeID.Value = Session("office_id")
            End If
            pnlStep1.Visible = False
            pnlStep2.Visible = True
        ElseIf Args.CommandName = "next2" Then
            If Page.IsValid Then
                Dim subject As String = rblRequestType.SelectedItem.ToString
                hfRequestType.Value = subject
                Select Case subject
                    Case "Real-time"
                        formatpnlStep3(subject, rblRTReqTypes.SelectedItem.ToString)
                    Case "General NWIS"
                        formatpnlStep3(subject, rblNWISReqTypes.SelectedItem.ToString)
                    Case "ADR Request"
                        formatpnlStep3(subject, rblADRReqTypes.SelectedItem.ToString)
                    Case "Other Support"
                        formatpnlStep3(subject, rblOtherReqTypes.SelectedItem.ToString)
                End Select
                pnlStep2.Visible = False
                pnlStep3.Visible = True
            End If
        ElseIf Args.CommandName = "next3" Then
            If Page.IsValid Then
                formatpnlStep4(tbRequest.Text)
                pnlStep3.Visible = False
                pnlStep4.Visible = True
            End If
        ElseIf Args.CommandName = "send" Then
            SubmitRequest()
            pnlStep4.Visible = False
            pnlConfirmSend.Visible = True
        End If
    End Sub

    Public Sub formatSubRequestType(ByVal sender As Object, ByVal e As System.EventArgs) Handles rblRequestType.SelectedIndexChanged
        Dim subject As String = rblRequestType.SelectedItem.ToString
        lblReqTypesHeading.Visible = True
        If subject = "Real-time" Then
            rblADRReqTypes.Visible = False
            rblNWISReqTypes.Visible = False
            rblOtherReqTypes.Visible = False
            rblRTReqTypes.Visible = True
        ElseIf subject = "General NWIS" Then
            rblADRReqTypes.Visible = False
            rblNWISReqTypes.Visible = True
            rblOtherReqTypes.Visible = False
            rblRTReqTypes.Visible = False
        ElseIf subject = "ADR Request" Then
            rblADRReqTypes.Visible = True
            rblNWISReqTypes.Visible = False
            rblOtherReqTypes.Visible = False
            rblRTReqTypes.Visible = False
        ElseIf subject = "Other Support" Then
            rblADRReqTypes.Visible = False
            rblNWISReqTypes.Visible = False
            rblOtherReqTypes.Visible = True
            rblRTReqTypes.Visible = False
        End If
    End Sub

    Public Sub formatpnlStep3(ByVal subject As String, ByVal sub_req_tp As String)
        lblSubject.Text = "Request help for <span style=""color:red;"">" & subject & " - " & sub_req_tp & "</span>"
        hfRequestType.Value = subject & " - " & sub_req_tp
    End Sub

    Public Sub formatpnlStep4(ByVal request As String)
        Dim pOut As String = Nothing
        Dim site_no As String = hfSiteNo.Value

        If site_no = Nothing Then
            site_no = "<i>not referenced</i>"
            ltlEmails.Text = fcnMailToNWISOps("email", 0)
        Else
            s = New Site(site_no, Session("agency_cd"))
            site_no = s.NumberName
            ltlEmails.Text = fcnMailToNWISOps("email", s.ID)
        End If

        pOut = "<table style=""background-color:#d4ffbc;border:1px solid #4d9e1f;"" cellpadding=""5"" cellspacing=""0"">" & vbCrLf & _
            "  <tr>" & vbCrLf & _
            "    <td><b>Name:</b> " & hfName.Value & "</td>" & vbCrLf & _
            "    <td><b>Email:</b> " & hfEmail.Value & "</td>" & vbCrLf & _
            "    <td><b>Phone:</b> " & hfPhone.Value & "</td>" & vbCrLf & _
            "  </tr>" & vbCrLf & _
            "  <tr>" & vbCrLf & _
            "    <td><b>Site Number:</b> " & site_no & "</td>" & vbCrLf & _
            "    <td colspan=""2""><b>Request Subject:</b> " & hfRequestType.Value & "</td>" & vbCrLf & _
            "  </tr>" & vbCrLf & _
            "  <tr>" & vbCrLf & _
            "    <td colspan=""3""><b>Request:</b> <br />" & vbCrLf & _
            "    " & request & "</td>" & vbCrLf & _
            "  </tr>" & vbCrLf & _
            "</table>"

        ltlCompleteRequest.Text = pOut
        hfRequest.Value = request
    End Sub

    Public Sub SubmitRequest()
        Dim message As New MailMessage()
        Dim requestID As String = fcnMailToNWISOps("reqID", 0)
        Dim site_no As String = hfSiteNo.Value
        Dim site_information As String = Nothing
        Dim assigned_operators As ArrayList
        Dim trip_userids As ArrayList
        o = New Office(hfOfficeID.Value)

        If site_no <> Nothing Then
            site_information = GetSiteMailInfo(site_no)
            site_no = " for " & hfSiteNo.Value

            assigned_operators = s.GetSiteOperators(s.ID)
            trip_userids = s.GetFieldTrips(s.ID, "userids")

            For Each ao As String In assigned_operators
                If Not trip_userids.Contains(ao) And ao <> "" And ao <> Nothing Then
                    message.CC.Add(u.GetUserEmailAliasFromAD(ao.ToString))
                End If
            Next

            For Each tu As String In trip_userids
                If tu <> "" And tu <> Nothing Then
                    message.CC.Add(u.GetUserEmailAliasFromAD(tu.ToString))
                End If
            Next
        Else
            site_information = ""
            site_no = ""
        End If
        Dim subject As String = "NWIS Ops Request " & requestID & site_no & ": " & hfRequestType.Value
        Dim CCTo As String = tbCCEmail.Text
        If CCTo <> Nothing Then
            Dim a As Array = Split(CCTo, ",")
            For Each x In a
                If InStr(x.ToString, "@") Then
                    message.CC.Add(x.ToString)
                Else
                    message.CC.Add(u.GetUserEmailAliasFromAD(x.ToString))
                End If
            Next
        End If
        Dim ToAddress1 As String = fcnMailToNWISOps("toemail1", 0)
        If ToAddress1 <> "" Then
            Dim a1 As Array = Split(ToAddress1, ",")
            For Each x In a1
                message.To.Add(x.ToString)
            Next
        End If
        Dim ToAddress2 As String = fcnMailToNWISOps("toemail2", 0)
        If ToAddress2 <> "" Then
            Dim a2 As Array = Split(ToAddress2, ",")
            For Each x In a2
                Dim e As String = x.ToString
                If InStr(e, "<") <> 0 Then
                    e = Mid(e, InStr(e, "<") + 1)
                    e = Trim(Replace(e, ">", ""))
                End If
                message.To.Add(e)
            Next
        End If

        message.CC.Add(hfEmail.Value.ToString)
        message.From = New MailAddress(hfEmail.Value)
        message.Subject = subject
        message.Body = "E-Mail: " & hfEmail.Value & Chr(10) & Chr(10) & _
            "Request CC'ed to: " & tbCCEmail.Text & Chr(10) & Chr(10) & _
            "Office: " & o.Name & Chr(10) & Chr(10) & _
            "Request Type: " & hfRequestType.Value & Chr(10) & Chr(10) & _
            site_information & _
            "Request:" & Chr(10) & hfRequest.Value

        Dim smtp As New SmtpClient()
        smtp.Host = "smtp.usgs.gov"
        smtp.Send(message)

        IncreaseRequestID(requestID)
    End Sub


    Public Sub lbReset_Command(ByVal sender As Object, ByVal Args As CommandEventArgs)
        If Args.CommandName = "reset" Then
            hfRequest.Value = Nothing
            tbRequest.Text = ""
            hfRequestType.Value = Nothing
            If tbSiteNo.Text <> Nothing Then
                hfSiteNo.Value = Nothing
                tbSiteNo.Text = ""
                Master.SiteNumberLabel = ""
                Dim site_no_label As UpdatePanel = Master.FindControl("up1")
                site_no_label.Update()
            End If
            tbCCEmail.Text = ""
            rblRequestType.ClearSelection()
            rblADRReqTypes.ClearSelection()
            rblNWISReqTypes.ClearSelection()
            rblOtherReqTypes.ClearSelection()
            rblRTReqTypes.ClearSelection()
            lblReqTypesHeading.Visible = False
            rblADRReqTypes.Visible = False
            rblNWISReqTypes.Visible = False
            rblOtherReqTypes.Visible = False
            rblRTReqTypes.Visible = False
            pnlConfirmSend.Visible = False
            pnlStep4.Visible = False
            pnlStep3.Visible = False
            pnlStep2.Visible = False
            pnlStep1.Visible = True
        End If
    End Sub

    Public Sub IncreaseRequestID(ByVal requestid As String)
        Dim req_id As Integer = Convert.ToInt32(requestid) + 1
        Using cnx As New SqlConnection(Config.ConnectionInfo)
            Dim sql As String = "UPDATE lut_WSC" & _
            " SET reqID = " & req_id & _
            " FROM lut_WSC INNER JOIN" & _
            " lut_Office ON lut_WSC.wsc_id = lut_Office.wsc_id" & _
            " WHERE (lut_Office.office_id = " & hfOfficeID.Value & ")"
            cnx.Open()
            Dim cmd As SqlCommand = New SqlCommand(sql, cnx)
            cmd.ExecuteNonQuery()
            cnx.Close()
        End Using
    End Sub

    Function fcnMailToNWISOps(ByVal options As String, ByVal site_id As Integer) As String
        Dim pOut As String = "<ul>"
        Dim reqID As String = Nothing
        Dim ToAddress1 As String = Nothing
        Dim ToAddress2 As String = Nothing

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim sql As String = "SELECT nwisops_email, CONVERT(nvarchar(10),reqID) As reqID, lut_Office.data_chief_email" & _
               " FROM lut_WSC INNER JOIN lut_Office On lut_Office.wsc_id = lut_WSC.wsc_id" & _
               " WHERE (office_id = " & hfOfficeID.Value & ")"
            Dim cmd As SqlCommand = New SqlCommand(sql, cnx)
            Dim da As SqlDataAdapter = New SqlDataAdapter(cmd)
            Dim dt As DataTable = New DataTable
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                If options = "email" Then
                    Dim groupemail As String = row("nwisops_email").ToString
                    Dim dcemail As String = row("data_chief_email").ToString
                    If groupemail <> "" Then
                        Dim a As Array = Split(groupemail, ",")
                        For Each x In a
                            pOut = pOut & "<li>" & x.ToString & "</li>" & vbCrLf
                        Next
                    End If
                    If dcemail <> "" Then
                        Dim a1 As Array = Split(dcemail, ",")
                        For Each x In a1
                            Dim e As String = Replace(x.ToString, "<", "&lt;")
                            e = Replace(e, ">", "&gt;")
                            pOut = pOut & "<li>" & e & "</li>" & vbCrLf
                        Next
                    End If

                    If site_id > 0 Then
                        s = New Site(site_id)
                        Dim assigned_operators As ArrayList = s.GetSiteOperators(site_id)
                        Dim field_trip_userids As ArrayList = s.GetFieldTrips(site_id, "userids")

                        For Each ao As String In assigned_operators
                            If Not field_trip_userids.Contains(ao) And ao <> "" And ao <> Nothing Then
                                pOut = pOut & "<li>" & u.GetUserEmailAliasFromAD(ao.ToString) & "</li>" & vbCrLf
                            End If
                        Next
                        For Each tu As String In field_trip_userids
                            If tu <> "" And tu <> Nothing Then
                                pOut = pOut & "<li>" & u.GetUserEmailAliasFromAD(tu.ToString) & "</li>" & vbCrLf
                            End If
                        Next
                    End If
                ElseIf options = "toemail1" Then
                    ToAddress1 = row("nwisops_email").ToString
                ElseIf options = "toemail2" Then
                    ToAddress2 = row("data_chief_email").ToString
                Else
                    reqID = row("reqID").ToString
                End If
            Next

            pOut = pOut & "</ul>"

            cnx.Close()
        End Using

        If options = "email" Then
            fcnMailToNWISOps = pOut
        ElseIf options = "toemail1" Then
            fcnMailToNWISOps = ToAddress1
        ElseIf options = "toemail2" Then
            fcnMailToNWISOps = ToAddress2
        Else
            fcnMailToNWISOps = reqID
        End If

        Return fcnMailToNWISOps
    End Function

    Public Function GetSiteMailInfo(ByVal site_no As String) As String
        Dim dcp_id As String = Nothing
        s = New Site(site_no, Session("agency_cd"))

        Using cnx As New SqlConnection(Config.ConnectionInfo)
            cnx.Open()
            Dim cmd As SqlCommand = New SqlCommand("spz_GetDCPInfo", cnx)
            Dim da As SqlDataAdapter
            Dim dt As DataTable = New DataTable
            cmd.CommandType = Data.CommandType.StoredProcedure

            cmd.Parameters.Add("@site_id", SqlDbType.Int).Value = s.ID

            da = New SqlDataAdapter(cmd)
            da.Fill(dt)

            For Each row As DataRow In dt.Rows
                dcp_id = row("dcp_id")
            Next
            cnx.Close()
        End Using

        Dim site_no_nm As String = s.NumberName
        Dim pOut As String = "Site ID: " & site_no_nm & Chr(10) & Chr(10) & _
            "DCPID: " & dcp_id & Chr(10) & Chr(10) & _
            "SIMS Station Info Page: " & Config.ServerURL & Config.SitePath & "StationInfo.asp?office_id=" & _
            hfOfficeID.Value & "&site_no=" & site_no & Chr(10) & Chr(10) & _
            "NWIS Web: http://waterdata.usgs.gov/nwis/nwisman/?site_no=" & site_no & Chr(10) & Chr(10)

        Return pOut
    End Function

End Class
